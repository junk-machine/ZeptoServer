using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to create new directory.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class MakeDirectoryCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Creates a new directory in the file system.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var itemPath = session.CurrentDirectory.Clone();

            if (itemPath.Navigate(arguments) && session.FileSystem.CreateDirectory(itemPath))
            {
                return FtpResponses.Path(itemPath.ToString(), session.PathEncoding);
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
