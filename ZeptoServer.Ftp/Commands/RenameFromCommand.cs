using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override async Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var sourcePath = session.CurrentDirectory.Clone();

            if (sourcePath.Navigate(arguments)
                    && (await session.FileSystem.IsFileExist(sourcePath, cancellation)
                        || await session.FileSystem.IsDirectoryExist(sourcePath, cancellation)))
            {
                session.RenameSource = sourcePath;
                return FtpResponses.FileMoreInfoRequired;
            }

            return FtpResponses.FileUnavailable;
        }
    }
}
