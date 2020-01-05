using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to print working directory.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class PrintWorkingDirectoryCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Creates a response that contains current working directory path.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            return FtpResponsesAsync.Path(session.CurrentDirectory.ToString(), session.PathEncoding);
        }
    }
}
