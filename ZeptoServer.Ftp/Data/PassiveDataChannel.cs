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
    /// Passive FTP data channel.
    /// </summary>
    internal class PassiveDataChannel : DataChannelBase
    {
        /// <summary>
        /// Socket to listen for client connections.
        /// </summary>
        private Socket listenerSocket;

        /// <summary>
        /// Task completion source to support public interface of obtaining the data stream.
        /// </summary>
        private TaskCompletionSource<Stream> connectionTask;

        /// <summary>
        /// Gets the listener IP end-point.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PassiveDataChannel"/> class
        /// with the provided listener end-point and logger.
        /// </summary>
        /// <param name="endPoint">Listener end-point</param>
        /// <param name="logger">Current logger instance</param>
        public PassiveDataChannel(IPEndPoint endPoint, ILogger logger)
            : base(logger)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            connectionTask = new TaskCompletionSource<Stream>();

            listenerSocket =
                new Socket(
                    endPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

            listenerSocket.Bind(endPoint);
            EndPoint = (IPEndPoint)listenerSocket.LocalEndPoint;

            // Start listening right now, as there are some clients that do not send
            // additional commands before connection is established
            listenerSocket.Listen(1);
            listenerSocket.BeginAccept(EndAccept, listenerSocket);

            Logger.WriteVerbose(TraceResources.PassiveModeConnecting);
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
        /// Accepts the client connection and populates inner task completion source with the network stream.
        /// </summary>
        /// <param name="result">Handle of the asynchronous accept operation</param>
        private void EndAccept(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            Socket clientSocket;

            try
            {
                clientSocket = socket.EndAccept(result);
            }
            catch (ObjectDisposedException error)
            {
                connectionTask.SetException(error);
                return;
            }

            Logger.WriteInfo(TraceResources.PassiveModeConnected);

            Stream stream = new NetworkStream(clientSocket, true);

            if (Logger.LogLevel >= LoggerSeverity.Debug)
            {
                stream = new StreamTracer(stream, Logger);
            }

            connectionTask.SetResult(stream);

            DisposeListener();
        }

        /// <summary>
        /// Closes and releases the listener socket.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeListener();
            }
        }

        /// <summary>
        /// Closes and releases the listener socket.
        /// </summary>
        private void DisposeListener()
        {
            var listener =
                Interlocked.Exchange(ref listenerSocket, null);

            if (listener != null)
            {
                try
                {
                    listener.Close();
                    listener.Dispose();
                }
                catch { }
            }
        }
    }
}
