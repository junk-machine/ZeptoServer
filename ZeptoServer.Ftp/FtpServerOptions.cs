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
        /// Gets the public address of the server.
        /// </summary>
        /// <remarks>
        /// If server is hosted behind NAT, then passive mode may not work correctly, 
        /// as it will always use local IP address. This property defines a public hostname
        /// or an external IP address of the server.
        /// </remarks>
        public string PublicAddress { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerOptions"/> class
        /// with the provided list of users.
        /// </summary>
        /// <param name="publicAddress">Public address of the server</param>
        /// <param name="users">Registered FTP users</param>
        public FtpServerOptions(string publicAddress, IEnumerable<FtpUser> users)
        {
            if (users == null)
            {
                throw new ArgumentNullException("users");
            }

            PublicAddress = publicAddress;
            Users = users;
        }
    }
}
