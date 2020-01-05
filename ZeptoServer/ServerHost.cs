using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ZeptoServer.Log;

namespace ZeptoServer
{
    /// <summary>
    /// Hosts a <see cref="IServerFactory"/> instance.
    /// </summary>
    public sealed class ServerHost : IDisposable
    {
        /// <summary>
        /// Maximum number of outstanding connection requests.
        /// </summary>
        const int ConnectionQueue = 10;

        /// <summary>
        /// tcp_keepalive structure for SIO_KEEPALIVE_VALS socket setting.
        /// </summary>
        private readonly byte[] keepAliveSettings;

        /// <summary>
        /// Light-weight servers factory.
        /// </summary>
        private readonly IServerFactory serverFactory;

        /// <summary>
        /// Current logger instance.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Listener socket.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHost"/> class
        /// with the provided end-point, server factory and logger.
        /// </summary>
        /// <param name="endPoint">Listener end-point</param>
        /// <param name="keepAliveInterval">Interval in seconds to send TCP keepalive packets</param>
        /// <param name="serverFactory">Server factory</param>
        /// <param name="logger">Current logger instance</param>
        public ServerHost(IPEndPoint endPoint, int keepAliveInterval, IServerFactory serverFactory, ILogger logger)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (serverFactory == null)
            {
                throw new ArgumentNullException(nameof(serverFactory));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.serverFactory = serverFactory;
            this.logger = logger;

            if (keepAliveInterval > 0)
            {
                // Convert to milliseconds
                keepAliveInterval *= 1000;

                keepAliveSettings =
                    new byte[]
                    {
                        // onoff
                        1, 0, 0, 0,
                        // keepalivetime
                        (byte)(keepAliveInterval & 255),
                        (byte)(keepAliveInterval >> 8 & 255),
                        (byte)(keepAliveInterval >> 16 & 255),
                        (byte)(keepAliveInterval >> 24 & 255),
                        // keepaliveinterval
                        (byte)(keepAliveInterval & 255),
                        (byte)(keepAliveInterval >> 8 & 255),
                        (byte)(keepAliveInterval >> 16 & 255),
                        (byte)(keepAliveInterval >> 24 & 255)
                    };
            }

            socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endPoint);
            socket.Listen(ConnectionQueue);

            logger.WriteInfo(TraceResources.ServerStarted);

            socket.BeginAccept(EndAccept, socket);
        }

        /// <summary>
        /// Accepts the new client connection.
        /// </summary>
        /// <param name="asyncResult">Handle of the asynchronous accept operation</param>
        private void EndAccept(IAsyncResult asyncResult)
        {
            var socket = (Socket)asyncResult.AsyncState;

            Socket clientSocket;

            try
            {
                clientSocket = socket.EndAccept(asyncResult);
            }
            catch (ObjectDisposedException)
            {
                // Listener socket was closed
                return;
            }

            // Listen for another connections right away
            socket.BeginAccept(EndAccept, socket);

            // Disable Nagle algorithm for control channel
            clientSocket.NoDelay = true;

            // Enable TCP keepalive for control channel
            if (keepAliveSettings != null)
            {
                clientSocket.IOControl(IOControlCode.KeepAliveValues, keepAliveSettings, null);
            }

            // Create logger scope for the connection
            var loggerScope = new LoggerScope(clientSocket.RemoteEndPoint, logger);

            loggerScope.WriteInfo(TraceResources.ClientConnected);

            // Create the server instance to handle all client communications
            serverFactory
                .Create(clientSocket, loggerScope)
                .Run(CancellationToken.None);
        }

        /// <summary>
        /// Stops accepting new connections and closes listener socket.
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref socket, null);

            if (disposable != null)
            {
                disposable.Close();
                disposable.Dispose();
            }
        }
    }
}
