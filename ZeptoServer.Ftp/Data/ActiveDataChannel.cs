using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Log;

namespace ZeptoServer.Ftp.Data
{
    /// <summary>
    /// Active FTP data channel.
    /// </summary>
    internal sealed class ActiveDataChannel : DataChannelBase
    {
        /// <summary>
        /// Socket for the client connection.
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// Task completion source to support public interface of obtaining the data stream.
        /// </summary>
        private TaskCompletionSource<Stream> connectionTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDataChannel"/> class
        /// with the provided end-point and logger.
        /// </summary>
        /// <param name="endPoint">End-point to connect to</param>
        /// <param name="logger">Current logger instance</param>
        public ActiveDataChannel(EndPoint endPoint, ILogger logger)
            : base(logger)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            connectionTask = new TaskCompletionSource<Stream>();

            clientSocket =
                new Socket(
                    endPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            // Connect right now, as there are some clients that do not send
            // additional commands before connection is established
            Logger.WriteVerbose(TraceResources.ActiveModeConnecting);
            
            clientSocket.BeginConnect(endPoint, EndConnect, clientSocket);
        }

        /// <summary>
        /// Obtains the stream to send and receive the data.
        /// </summary>
        /// <returns>Stream to send and receive the data.</returns>
        public override Task<Stream> GetDataStream()
        {
            return connectionTask.Task;
        }

        /// <summary>
        /// Finishes the connection to the client and populates inner task completion source with the network stream.
        /// </summary>
        /// <param name="result">Handle of the asynchronous connect operation</param>
        private void EndConnect(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                socket.EndConnect(result);
            }
            catch (ObjectDisposedException error)
            {
                connectionTask.SetException(error);
                return;
            }

            Logger.WriteInfo(TraceResources.ActiveModeConnected);

            Stream stream = new NetworkStream(socket, true);

            if (Logger.LogLevel >= LoggerSeverity.Debug)
            {
                stream = new StreamTracer(stream, Logger);
            }

            connectionTask.SetResult(stream);
        }

        /// <summary>
        /// Closes and releases the client socket.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var client =
                    Interlocked.Exchange(ref clientSocket, null);

                if (client != null)
                {
                    try
                    {
                        client.Close();
                        client.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}
