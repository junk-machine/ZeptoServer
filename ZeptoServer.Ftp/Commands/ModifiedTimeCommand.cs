using System;
using System.Globalization;
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
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var itemPath = session.CurrentDirectory.Clone();

            FileSystemItem item = null;

            if (itemPath.Navigate(arguments))
            {
                item = session.FileSystem.GetItem(itemPath);
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
