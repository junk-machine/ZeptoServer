using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Log;
using ZeptoServer.Utilities;

namespace ZeptoServer
{
    /// <summary>
    /// Base class for any light-weight server.
    /// </summary>
    /// <typeparam name="TServerOptions">Type of server-specific options</typeparam>
    public abstract class ServerBase<TServerOptions> : IServer, IDisposable
        where TServerOptions : ServerOptions
    {
        /// <summary>
        /// Network stream to send and receive data from the client.
        /// </summary>
        private NetworkStream dataStream;

        /// <summary>
        /// Temporary buffer to read chunk of data from the client.
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// Temporary buffer to accumulate received data before it can be processed.
        /// </summary>
        private readonly ArrayBuffer data;

        /// <summary>
        /// Gets the current logger instance.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBase"/> class
        /// with the provided client socket, server options and logger.
        /// </summary>
        /// <param name="socket">Socket for the client connection</param>
        /// <param name="serverOptions">Configuration for the server</param>
        /// <param name="logger">Current logger instance</param>
        public ServerBase(Socket socket, TServerOptions serverOptions, ILogger logger)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }

            if (serverOptions == null)
            {
                throw new ArgumentNullException("serverOptions");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            Logger = logger;

            dataStream = new NetworkStream(socket, true);
            data = new ArrayBuffer();
            buffer = new byte[serverOptions.BufferSize];
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public async Task Run()
        {
            try
            {
                await OnStart();
                await ReadData();
            }
            catch (Exception error)
            {
                Logger.WriteError(TraceResources.CriticalServerErrorFormat, error.Message);
                CloseDataStream();
            }
        }

        /// <summary>
        /// Performs all necessary actions when server starts.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected abstract Task OnStart();

        /// <summary>
        /// Reads the data from the client.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        private async Task ReadData()
        {
            int count;

            while ((count = await ReadDataSafe()) > 0)
            {
                LogRawRequest(buffer, count);

                data.Append(buffer, 0, count);

                await OnData(data);

                // Done processing the request, start listening again
                Logger.WriteDebug(TraceResources.AccumulatedDataProcessed);
            }

            Logger.WriteInfo(TraceResources.ConnectionClosingOnReceiveFormat, count);

            CloseDataStream();
        }

        /// <summary>
        /// Reads data in a safe manner, so that no exceptions are being thrown.
        /// </summary>
        /// <returns>Number of bytes read from the client.</returns>
        private async Task<int> ReadDataSafe()
        {
            try
            {
                return await dataStream.ReadAsync(buffer, 0, buffer.Length);
            }
            catch (Exception error)
            {
                Logger.WriteWarning(TraceResources.ErrorReceivingDataFormat, error.GetType());
                return 0;
            }
        }

        /// <summary>
        /// Performs processing of all the accumulated data received from the client.
        /// </summary>
        /// <param name="data">Data received from the client so far</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected abstract Task OnData(ArrayBuffer data);

        /// <summary>
        /// Send data from the provided <see cref="IDataStream"/> to the client.
        /// </summary>
        /// <param name="data">Data stream to read the data to be sent</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected Task WriteData(IDataStream data)
        {
            return data.WriteTo(dataStream);
        }

        /// <summary>
        /// Sends the data to the client.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected Task WriteData(byte[] data)
        {
            return dataStream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Closes the underlying data stream and client connection.
        /// </summary>
        private void CloseDataStream()
        {
            Logger.WriteInfo(TraceResources.ConnectionClosed);
            Dispose();
        }

        /// <summary>
        /// Releases all resources held by the server instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes the underlying data stream and client connection.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable =
                    Interlocked.Exchange(ref dataStream, null);

                if (disposable != null)
                {
                    try
                    {
                        disposable.Close();
                        disposable.Dispose();
                    }
                    catch
                    {
                        // Socket can be already disposed
                    }
                }
            }
        }

        #region Logging

        /// <summary>
        /// Logs raw data received from the client.
        /// </summary>
        /// <param name="data">Received data</param>
        /// <param name="count">Number of bytes to log</param>
        private void LogRawRequest(byte[] data, int count)
        {
            if (Logger.LogLevel >= LoggerSeverity.Debug)
            {
                Logger.WriteDebug(TraceResources.ReceivedDataFormat, BitConverter.ToString(data, 0, count));
            }
        }

        /// <summary>
        /// Logs raw data sent to the client.
        /// </summary>
        /// <param name="data">Sent data</param>
        /// <param name="count">Number of bytes to log</param>
        private void LogRawResponse(byte[] data, int count)
        {
            if (Logger.LogLevel >= LoggerSeverity.Debug)
            {
                Logger.WriteDebug(TraceResources.SentDataFormat, BitConverter.ToString(data, 0, count));
            }
        }

        #endregion Logging
    }
}
