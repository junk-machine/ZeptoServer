using System;
using System.Text;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to change session options.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc2389
    /// </remarks>
    internal sealed class OptionsCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Updates the current FTP session context with the requested options.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            var optionArguments = arguments.Split(' ');

            if (optionArguments.Length == 0)
            {
                return FtpResponses.ParameterSyntaxError;
            }

            if (FtpOptions.UTF8.Equals(optionArguments[0], StringComparison.OrdinalIgnoreCase))
            {
                if (optionArguments.Length == 1)
                {
                    return FtpResponses.ParameterSyntaxError;
                }

                if (FtpOptions.SetOn.Equals(optionArguments[1], StringComparison.OrdinalIgnoreCase))
                {
                    session.PathEncoding = Encoding.UTF8;
                }
                else if (FtpOptions.SetOff.Equals(optionArguments[1], StringComparison.OrdinalIgnoreCase))
                {
                    session.PathEncoding = Encoding.ASCII;
                }
                else
                {
                    return FtpResponses.ParameterSyntaxError;
                }

                return FtpResponses.Success;
            }
            else if (FtpOptions.UTF_8.Equals(optionArguments[0], StringComparison.OrdinalIgnoreCase))
            {
                session.PathEncoding = Encoding.UTF8;
                // TODO: Support NLST argument
                return FtpResponses.Success;
            }
            else
            {
                return FtpResponses.ParameterSyntaxError;
            }
        }
    }
}
