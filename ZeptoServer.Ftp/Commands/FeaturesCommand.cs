using System;
using System.Collections.Generic;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to display supported server features.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc2389
    /// </remarks>
    internal sealed class FeaturesCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Collection of features supported by the server.
        /// </summary>
        private readonly IEnumerable<string> features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeaturesCommand"/> class
        /// with the provided collection of features.
        /// </summary>
        /// <param name="features">Collection of features supported by the server</param>
        public FeaturesCommand(IEnumerable<string> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            this.features = features;
        }

        /// <summary>
        /// Generates the response with the list of supported server features.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            return FtpResponses.Features(features, session.LineFeed);
        }
    }
}
