using System;
using System.IO;
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
                using (var fileStream = session.FileSystem.ReadFile(filePath))
                {
                    if (fileStream == null)
                    {
                        await session.ControlChannel.Send(FtpResponses.FileUnavailable);
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
                            await session.ControlChannel.Send(FtpResponses.FileUnavailable);
                            return;
                        }
                        
                    }
                    
                    if (await WriteToDataChannel(session, SendFile, fileStream) ==
                            FtpResponses.TransferComplete)
                    {
                        session.Logger.WriteInfo(TraceResources.RetrievedFileFormat, filePath);
                    }
                }
            }
            else
            {
                await session.ControlChannel.Send(FtpResponses.FileUnavailable);
            }
        }

        /// <summary>
        /// Sends the file contents to the client over the data channel.
        /// </summary>
        /// <param name="dataStream">Data channel stream</param>
        /// <param name="session">FTP session context</param>
        /// <param name="fileStream">Source file stream</param>
        /// <returns>Always returns <see cref="FtpResponses.TransferComplete"/>.</returns>
        private async Task<IResponse> SendFile(Stream dataStream, FtpSessionState session, Stream fileStream)
        {
            await fileStream.CopyToAsync(dataStream);
            await dataStream.FlushAsync();

            return FtpResponses.TransferComplete;
        }
    }
}
