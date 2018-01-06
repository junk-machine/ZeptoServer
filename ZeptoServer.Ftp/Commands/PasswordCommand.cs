using System;
using System.Collections.Generic;
using System.Linq;
using ZeptoServer.Ftp.Configuration;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to validate user password.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class PasswordCommand : FtpCommandBase
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
        public PasswordCommand(IEnumerable<FtpUser> knownUsers)
        {
            if (knownUsers == null)
            {
                throw new ArgumentNullException(nameof(knownUsers));
            }

            this.knownUsers = knownUsers;
        }

        /// <summary>
        /// Verifies that provided password is correct for the user.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            var user =
                knownUsers
                    .FirstOrDefault(u =>
                        u.Name == session.Username &&
                        u.Password == arguments);

            if (user == null)
            {
                return FtpResponses.NotLoggedIn;
            }

            session.FileSystem = user.FileSystem;
            session.CurrentDirectory = new VirtualPath();

            return FtpResponses.LoggedIn;
        }
    }
}
