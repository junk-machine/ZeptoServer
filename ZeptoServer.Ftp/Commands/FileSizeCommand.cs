using System;
using System.Globalization;
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
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
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
                item = session.FileSystem.GetItem(itemPath);
            }

            if (item == null || item.IsDirectory)
            {
                return FtpResponses.FileUnavailable;
            }

            return FtpResponses.FileStatus(item.Size.ToString(CultureInfo.InvariantCulture));
        }
    }
}
