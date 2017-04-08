using System;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to set file structure for transmission.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class FileStructureCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Binary file (no particular structure).
        /// </summary>
        private const string FileStructure = "F";

        /// <summary>
        /// Verifies if requested file structure is supported by the server.
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

            if (FileStructure.Equals(arguments, StringComparison.OrdinalIgnoreCase))
            {
                return FtpResponses.Success;
            }

            return FtpResponses.NotImplementedForParameter;
        }
    }
}
