using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to remove the directory.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class RemoveDirectoryCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Removes the directory from the file system.
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

            if (itemPath.Navigate(arguments) && session.FileSystem.RemoveDirectory(itemPath))
            {
                session.Logger.WriteInfo(TraceResources.DeletedDirectoryFormat, itemPath);
                return FtpResponses.FileActionOk;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
