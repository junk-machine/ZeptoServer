using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to list directory contents.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class ListFilesCommand : FtpDataCommandBase
    {
        /// <summary>
        /// Separator character for columns in the output.
        /// </summary>
        private const char ColumnSeparator = '\t';

        /// <summary>
        /// File extensions of executable files.
        /// </summary>
        private const string ExecutableExtension = ".exe";

        /// <summary>
        /// Format for the item last modification timestamp.
        /// </summary>
        private const string TimestampFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";

        /// <summary>
        /// Command argument to list all items in the current folder.
        /// </summary>
        private const string ListAll = "-a";

        /// <summary>
        /// Enumerates list of items in the directory and sends them to the client over the data channel.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task HandleDataCommand(string arguments, FtpSessionState session)
        {
            IEnumerable<FileSystemItem> items = null;

            if (String.IsNullOrEmpty(arguments) || ListAll.Equals(arguments, StringComparison.OrdinalIgnoreCase))
            {
                items = session.FileSystem.ListItems(session.CurrentDirectory);
            }
            else
            {
                var itemPath = session.CurrentDirectory.Clone();

                if (itemPath.Navigate(arguments))
                {
                    var item = session.FileSystem.GetItem(itemPath);

                    if (item != null)
                    {
                        items = new[] { item };
                    }
                }
            }

            if (items == null)
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                return;
            }

            await WriteToDataChannel(session, WriteFileList, items);
        }

        /// <summary>
        /// Sends the list of file system items to the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="items">File system items</param>
        /// <returns>Always returns <see cref="FtpResponses.TransferComplete"/>.</returns>
        private async Task<IResponse> WriteFileList(Stream dataStream, FtpSessionState session, IEnumerable<FileSystemItem> items)
        {
            var record = new StringBuilder(128);
            byte[] recordBuffer;

            foreach (var item in items)
            {
                // Permissions
                record.Append(
                    item.IsDirectory
                        ? "drwxrwxrwx"
                        : item.Name.EndsWith(ExecutableExtension, StringComparison.OrdinalIgnoreCase)
                            ? "-rwxrwxrwx"
                            : "-rw-rw-rw-");
                record.Append(ColumnSeparator);

                // Number of hard links
                record.Append("1");
                record.Append(ColumnSeparator);

                // Name of the owner
                record.Append("owner");
                record.Append(ColumnSeparator);

                // Name of the group
                record.Append("group");
                record.Append(ColumnSeparator);

                // Size
                record.Append(item.Size);
                record.Append(ColumnSeparator);

                // Last modification timestamp
                record.Append(item.LastModifiedTime.ToString(TimestampFormat, CultureInfo.InvariantCulture));
                record.Append(ColumnSeparator);

                // Name
                record.Append(item.Name);

                recordBuffer = session.ControlEncoding.GetBytes(record.ToString());
                await dataStream.WriteAsync(recordBuffer, 0, recordBuffer.Length);
                await dataStream.WriteAsync(session.LineFeed, 0, session.LineFeed.Length);

                record.Clear();
            }
            
            await dataStream.WriteAsync(session.LineFeed, 0, session.LineFeed.Length);

            return FtpResponses.TransferComplete;
        }
    }
}
