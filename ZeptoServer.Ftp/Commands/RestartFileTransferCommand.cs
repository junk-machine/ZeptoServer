using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to restart a file transfer at specific marker.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc3659#section-5
    /// </remarks>
    internal sealed class RestartFileTransferCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Stores the offset to restart the next file transfer from in the FTP session context.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            long offset;

            if (!long.TryParse(arguments, out offset))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            session.TransferRestartOffset = offset;

            return FtpResponses.FileMoreInfoRequired;
        }
    }
}
