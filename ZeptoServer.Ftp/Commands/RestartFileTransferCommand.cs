using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to restart a file transfer at specific marker.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc3659#section-5
    /// </remarks>
    internal sealed class RestartFileTransferCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Stores the offset to restart the next file transfer from in the FTP session context.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            long offset;

            if (!long.TryParse(arguments, out offset))
            {
                return FtpResponsesAsync.ParameterSyntaxError;
            }

            session.TransferRestartOffset = offset;

            return FtpResponsesAsync.FileMoreInfoRequired;
        }
    }
}
