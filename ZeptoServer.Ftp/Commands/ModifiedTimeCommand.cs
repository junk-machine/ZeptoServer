using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to display last modification time of the file system item.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc3659
    /// </remarks>
    internal sealed class ModifiedTimeCommand : FtpPathCommandBase
    {
        /// <summary>
        /// Format for the file system item last modification timestamp.
        /// </summary>
        private const string TimestampFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Obtains the last modification time of the file system item.
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

            FileSystemItem item = null;

            if (itemPath.Navigate(arguments))
            {
                item = await session.FileSystem.GetItem(itemPath, cancellation);
            }

            if (item == null)
            {
                return FtpResponses.FileUnavailable;
            }

            return FtpResponses.FileStatus(
                item.LastModifiedTime.ToString(TimestampFormat, CultureInfo.InvariantCulture));
        }
    }
}
