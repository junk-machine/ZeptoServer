using System;
using System.IO;
using System.Threading;
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
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task HandleFileTransferCommand(
            string arguments,
            FtpSessionState session,
            long restartOffset,
            CancellationToken cancellation)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                await session.ControlChannel.Send(FtpResponses.ParameterSyntaxError, cancellation);
                return;
            }

            var filePath = session.CurrentDirectory.Clone();

            if (filePath.Navigate(arguments))
            {
                using (var fileStream = await OpenFile(session.FileSystem, filePath, cancellation))
                {
                    if (fileStream == null)
                    {
                        await session.ControlChannel.Send(FtpResponses.FileUnavailable, cancellation);
                        return;
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
                            await session.ControlChannel.Send(FtpResponses.FileUnavailable, cancellation);
                            return;
                        }
                    }

                    if (await WriteToDataChannel(session, ReceiveFile, fileStream, cancellation) ==
                            FtpResponses.TransferComplete)
                    {
                        session.Logger.WriteInfo(TraceResources.FileStoredFormat, filePath);
                    }
                }
            }
            else
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable, cancellation);
            }
        }

        /// <summary>
        /// Opens a file on the file system for writing.
        /// </summary>
        /// <param name="fileSystem">Virtual file system</param>
        /// <param name="path">Path to the file stored in the virtual file system</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that should be used to write to a file.</returns>
        protected virtual Task<Stream> OpenFile(
            IFileSystem fileSystem,
            VirtualPath path,
            CancellationToken cancellation)
        {
            return fileSystem.WriteFile(path, cancellation);
        }

        /// <summary>
        /// Receives the file from the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="fileStream">Target file stream</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP response that should be sent to the client.</returns>
        private async Task<IResponse> ReceiveFile(
            Stream dataStream,
            FtpSessionState session,
            Stream fileStream,
            CancellationToken cancellation)
        {
            try
            {
                await dataStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync(cancellation);

                return FtpResponses.TransferComplete;
            }
            catch (NotEnoughSpaceException)
            {
                return FtpResponses.NotEnoughSpace;
            }
        }
    }
}
