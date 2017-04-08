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
        /// <param name="serverFactory">Server factory</param>
        /// <param name="logger">Current logger instance</param>
        public ServerHost(IPEndPoint endPoint, IServerFactory serverFactory, ILogger logger)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            if (serverFactory == null)
            {
                throw new ArgumentNullException("serverFactory");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.serverFactory = serverFactory;
            this.logger = logger;

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

            // Create logger scope for the connection
            var loggerScope = new LoggerScope(clientSocket.RemoteEndPoint, logger);

            loggerScope.WriteInfo(TraceResources.ClientConnected);

            // Create the server instance to handle all client communications
            serverFactory
                .Create(clientSocket, loggerScope)
                .Run();
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
