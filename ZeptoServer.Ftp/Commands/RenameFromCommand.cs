using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to specify the item to be renamed
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class RenameFromCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Stores the original file name in the FTP session context.
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

            var sourcePath = session.CurrentDirectory.Clone();

            if (sourcePath.Navigate(arguments)
                    && (session.FileSystem.IsFileExist(sourcePath)
                        || session.FileSystem.IsDirectoryExist(sourcePath)))
            {
                session.RenameSource = sourcePath;
                return FtpResponses.FileMoreInfoRequired;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
