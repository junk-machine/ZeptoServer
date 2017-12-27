using System;
using System.IO;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to store a file.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal class StoreFileCommand : FtpFileTransferCommandBase
    {
        /// <summary>
        /// Receives the requested file from the client over the data channel.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="restartOffset">File transfer restart offset</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task HandleFileTransferCommand(string arguments, FtpSessionState session, long restartOffset)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                await session.ControlChannel.Send(FtpResponses.ParameterSyntaxError);
                return;
            }

            var filePath = session.CurrentDirectory.Clone();

            if (filePath.Navigate(arguments))
            {
                using (var fileStream = OpenFile(session.FileSystem, filePath))
                {
                    if (fileStream == null)
                    {
                        await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                    }

                    if (restartOffset > 0)
                    {
                        try
                        {
                            await fileStream.SetOffset(restartOffset);
                        }
                        catch
                        {
                            // If offset is out of range
                            await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                            return;
                        }
                    }

                    await WriteToDataChannel(session, ReceiveFile, fileStream);
                }
            }
            else
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable);
            }
        }

        protected virtual Stream OpenFile(IFileSystem fileSystem, VirtualPath path)
        {
            return fileSystem.WriteFile(path);
        }

        /// <summary>
        /// Receives the file from the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="fileStream">Target file stream</param>
        /// <returns>FTP response that should be sent to the client.</returns>
        private async Task<IResponse> ReceiveFile(Stream dataStream, FtpSessionState session, Stream fileStream)
        {
            try
            {
                await dataStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                return FtpResponses.TransferComplete;
            }
            catch (NotEnoughSpaceException)
            {
                return FtpResponses.NotEnoughSpace;
            }
        }
    }
}
