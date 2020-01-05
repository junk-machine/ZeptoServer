using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to display file size.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc3659
    /// </remarks>
    internal sealed class FileSizeCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Obtains the size of the file in the file system.
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

            if (session.TransferType != FileTransferType.Image)
            {
                // Only return size in Image file transfer mode
                return FtpResponses.FileUnavailable;
            }

            var itemPath = session.CurrentDirectory.Clone();

            FileSystemItem item = null;

            if (itemPath.Navigate(arguments))
            {
                item = await session.FileSystem.GetItem(itemPath, cancellation);
            }

            if (item == null || item.IsDirectory)
            {
                return FtpResponses.FileUnavailable;
            }

            return FtpResponses.FileStatus(item.Size.ToString(CultureInfo.InvariantCulture));
        }
    }
}
