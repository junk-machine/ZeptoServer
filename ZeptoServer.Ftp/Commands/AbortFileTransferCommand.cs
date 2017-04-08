using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to abort the file transfer.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class AbortFileTransferCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Closes the active data channel.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            try
            {
                session.DataChannel.Dispose();
            }
            catch { }

            session.DataChannel = null;

            return FtpResponses.TransferComplete;
        }
    }
}
