using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to display system type.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class SystemTypeCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Creates a response containing system type information.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            return FtpResponsesAsync.SystemType;
        }
    }
}
