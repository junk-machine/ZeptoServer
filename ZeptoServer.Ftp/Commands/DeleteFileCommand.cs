using System;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to delete a file.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class DeleteFileCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Deletes a file from the file system.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override async Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var itemPath = session.CurrentDirectory.Clone();

            if (itemPath.Navigate(arguments) && await session.FileSystem.RemoveFile(itemPath, cancellation))
            {
                session.Logger.WriteInfo(TraceResources.DeletedFileFormat, itemPath);
                return FtpResponses.FileActionOk;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
