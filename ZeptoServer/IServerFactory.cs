using System.Net.Sockets;
using ZeptoServer.Log;

namespace ZeptoServer
{
    /// <summary>
    /// Factory for light-weight servers.
    /// </summary>
    public interface IServerFactory
    {
        /// <summary>
        /// Creates a new instance of the server.
        /// </summary>
        /// <param name="socket">Socket for the client connection</param>
        /// <param name="logger">Current logger instance</param>
        /// <returns>New instance of the server</returns>
        IServer Create(Socket socket, ILogger logger);
    }
}
