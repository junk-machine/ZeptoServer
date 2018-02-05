using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to specify the target name of the item and actually rename it.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class RenameToCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Prevents original file name from being removed from the FTP session context before command executes.
        /// </summary>
        /// <param name="session">FTP session context</param>
        protected override void CleanupRenameSource(FtpSessionState session)
        {
            // Do not cleanup the session state before this command executes
        }

        /// <summary>
        /// Renames the file in the file system.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            var sourcePath = session.RenameSource;

            session.RenameSource = null;

            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var targetPath = session.CurrentDirectory.Clone();

            if (targetPath.Navigate(arguments))
            {
                if (session.FileSystem.IsFileExist(sourcePath)
                        && session.FileSystem.RenameFile(sourcePath, targetPath))
                {
                    session.Logger.WriteInfo(TraceResources.RenamedFileFormat, sourcePath, targetPath);
                    return FtpResponses.FileActionOk;
                }

                if (session.FileSystem.IsDirectoryExist(sourcePath)
                        && session.FileSystem.RenameDirectory(sourcePath, targetPath))
                {
                    session.Logger.WriteInfo(TraceResources.RenamedDirectoryFormat, sourcePath, targetPath);
                    return FtpResponses.FileActionOk;
                }
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
