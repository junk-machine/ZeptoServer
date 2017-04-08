using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to set file transfer mode.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class TransferModeCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Stream transfer mode.
        /// </summary>
        private const string StreamMode = "S";

        /// <summary>
        /// Verifies if requested transfer mode is supported by the server.
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

            if (StreamMode.Equals(arguments, StringComparison.OrdinalIgnoreCase))
            {
                return FtpResponses.Success;
            }

            return FtpResponses.NotImplementedForParameter;
        }
    }
}
