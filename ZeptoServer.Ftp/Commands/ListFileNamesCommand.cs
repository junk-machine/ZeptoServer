using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to list the names of directory contents.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class ListFileNamesCommand : FtpDataCommandBase
    {
        /// <summary>
        /// Enumerates list of item names in the directory and sends them to the client over the data channel.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task HandleDataCommand(string arguments, FtpSessionState session)
        {
            var requestedPath = session.CurrentDirectory;

            if (!String.IsNullOrEmpty(arguments))
            {
                requestedPath = requestedPath.Clone();

                if (!requestedPath.Navigate(arguments))
                {
                    await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                    return;
                }
            }

            var items = session.FileSystem.ListItems(requestedPath);

            if (items == null)
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                return;
            }
            
            await WriteToDataChannel(session, WriteFileList, items);
        }

        /// <summary>
        /// Sends the list of file system item names to the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="items">File system items</param>
        /// <returns>Always returns <see cref="FtpResponses.TransferComplete"/>.</returns>
        private async Task<IResponse> WriteFileList(Stream dataStream, FtpSessionState session, IEnumerable<FileSystemItem> items)
        {
            byte[] recordBuffer;

            foreach (var item in items)
            {
                recordBuffer = session.PathEncoding.GetBytes(item.Name);
                await dataStream.WriteAsync(recordBuffer, 0, recordBuffer.Length);
                await dataStream.WriteAsync(session.LineFeed, 0, session.LineFeed.Length);
            }

            await dataStream.WriteAsync(session.LineFeed, 0, session.LineFeed.Length);

            await dataStream.FlushAsync();

            return FtpResponses.TransferComplete;
        }
    }
}
