using System;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to change working directory.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class ChangeWorkingDirectoryCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Changes the current path to the new one.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override async Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var newPath = session.CurrentDirectory.Clone();

            if (newPath.Navigate(arguments) && await session.FileSystem.IsDirectoryExist(newPath, cancellation))
            {
                session.CurrentDirectory = newPath;
                return FtpResponses.FileActionOk;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
