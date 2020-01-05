using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to retrieve the file.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class RetrieveFileCommand : FtpFileTransferCommandBase
    {
        /// <summary>
        /// Sends the requested file to the client over the data channel.
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
                using (var fileStream = await session.FileSystem.ReadFile(filePath, cancellation))
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
                    
                    if (await WriteToDataChannel(session, SendFile, fileStream, cancellation) ==
                            FtpResponses.TransferComplete)
                    {
                        session.Logger.WriteInfo(TraceResources.RetrievedFileFormat, filePath);
                    }
                }
            }
            else
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable, cancellation);
            }
        }

        /// <summary>
        /// Sends the file contents to the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="fileStream">Source file stream</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Always returns <see cref="FtpResponses.TransferComplete"/>.</returns>
        private async Task<IResponse> SendFile(
            Stream dataStream,
            FtpSessionState session,
            Stream fileStream,
            CancellationToken cancellation)
        {
            await fileStream.CopyToAsync(dataStream);
            await dataStream.FlushAsync(cancellation);

            return FtpResponses.TransferComplete;
        }
    }
}
