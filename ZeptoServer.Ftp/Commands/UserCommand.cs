using System;
using System.Collections.Generic;
using System.Linq;
using ZeptoServer.Ftp.Configuration;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to validate username.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class UserCommand : FtpCommandBase
    {
        /// <summary>
        /// Collection of registered users.
        /// </summary>
        private readonly IEnumerable<FtpUser> knownUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCommand"/> class
        /// with the provided collection of known users.
        /// </summary>
        /// <param name="knownUsers">Collection of known FTP users</param>
        public UserCommand(IEnumerable<FtpUser> knownUsers)
        {
            if (knownUsers == null)
            {
                throw new ArgumentNullException(nameof(knownUsers));
            }

            this.knownUsers = knownUsers;
        }

        /// <summary>
        /// Verifies that provided user has access to the server.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            if (!knownUsers.Any(u => u.Name == arguments))
            {
                return FtpResponses.InvalidUsername;
            }

            session.Username = arguments;
            return FtpResponses.UserOk;
        }
    }
}
