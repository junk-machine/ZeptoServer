using System;
using ZeptoServer.Ftp.FileSystems;

namespace ZeptoServer.Ftp.Configuration
{
    /// <summary>
    /// Contains information about registered FTP user.
    /// </summary>
    public sealed class FtpUser
    {
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the virtual file system for the user.
        /// </summary>
        public IFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpUser"/> class
        /// with the provided name, password and file system.
        /// </summary>
        /// <param name="name">Name of the user</param>
        /// <param name="password">Password of the user</param>
        /// <param name="fileSystem">Virtual file system for the user</param>
        public FtpUser(string name, string password, IFileSystem fileSystem)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            Name = name;
            Password = password;
            FileSystem = fileSystem;
        }
    }
}
