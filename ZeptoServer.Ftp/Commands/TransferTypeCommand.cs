using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to set file transfer type.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class TransferTypeCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// ASCII transfer type.
        /// </summary>
        private const string AsciiTransferType = "A";

        /// <summary>
        /// Binary transfer type.
        /// </summary>
        private const string ImageTransferType = "I";

        /// <summary>
        /// Non-print transfer format.
        /// </summary>
        private const string NonPrintFormat = "N";

        /// <summary>
        /// Updates the transfer type in the FTP session context.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            var typeArguments = arguments.Split(' ');

            if (typeArguments.Length == 0)
            {
                return FtpResponsesAsync.ParameterSyntaxError;
            }

            switch (typeArguments[0])
            {
                case AsciiTransferType:
                    session.TransferType = FileTransferType.ASCII;

                    if (typeArguments.Length > 1)
                    {
                        switch (typeArguments[1])
                        {
                            case NonPrintFormat:
                                return FtpResponsesAsync.Success;
                            default:
                                return FtpResponsesAsync.NotImplementedForParameter;
                        }
                    }

                    return FtpResponsesAsync.Success;

                case ImageTransferType:
                    session.TransferType = FileTransferType.Image;
                    return FtpResponsesAsync.Success;

                default:
                    return FtpResponsesAsync.NotImplementedForParameter;
            }
        }
    }
}
