using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to change working directory to its parent.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class ChangeDirectoryUpCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Changes the current path to the parent directory.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            var newPath = session.CurrentDirectory.Clone();

            if (newPath.NavigateUp() && session.FileSystem.IsDirectoryExist(newPath))
            {
                session.CurrentDirectory = newPath;
                return FtpResponses.FileActionOk;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
