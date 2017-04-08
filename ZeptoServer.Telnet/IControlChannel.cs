using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Telnet
{
    /// <summary>
    /// Control channel of the Telnet server.
    /// </summary>
    public interface IControlChannel
    {
        /// <summary>
        /// Sends the response back to the client.
        /// </summary>
        /// <param name="response">Server response</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task Send(IResponse response);
    }
}
