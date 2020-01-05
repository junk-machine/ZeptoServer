using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            if (!knownUsers.Any(u => u.Name == arguments))
            {
                return FtpResponsesAsync.InvalidUsername;
            }

            session.Username = arguments;
            return FtpResponsesAsync.UserOk;
        }
    }
}
