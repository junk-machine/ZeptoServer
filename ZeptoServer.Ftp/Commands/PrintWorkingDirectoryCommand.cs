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
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            return FtpResponses.Path(session.CurrentDirectory.ToString(), session.PathEncoding);
        }
    }
}
