using System;
using System.Collections.Generic;
using ZeptoServer.Ftp.Configuration;
using ZeptoServer.Telnet;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Defines options specific to the FTP server.
    /// </summary>
    public class FtpServerOptions : TelnetServerOptions
    {
        /// <summary>
        /// Gets the collection of registered FTP users.
        /// </summary>
        public IEnumerable<FtpUser> Users { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerOptions"/> class
        /// with the provided list of users.
        /// </summary>
        /// <param name="users">Registered FTP users</param>
        public FtpServerOptions(IEnumerable<FtpUser> users)
        {
            if (users == null)
            {
                throw new ArgumentNullException("users");
            }

            Users = users;
        }
    }
}
