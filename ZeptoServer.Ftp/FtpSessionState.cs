using System;
using System.Net;
using System.Text;
using ZeptoServer.Ftp.Data;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Log;
using ZeptoServer.Telnet;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Holds the state for a single FTP client connection.
    /// </summary>
    internal sealed class FtpSessionState
    {
        /// <summary>
        /// Gets the address of the server that client is connected to.
        /// </summary>
        public IPAddress ServerAddress { get; private set; }

        /// <summary>
        /// Gets the external IP address of the server.
        /// </summary>
        public IPAddress PublicServerAddress { get; private set; }

        /// <summary>
        /// Gets the current line termination sequence.
        /// </summary>
        public byte[] LineFeed { get; private set; }

        /// <summary>
        /// Gets the encoding of the data sent through the control channel.
        /// </summary>
        public Encoding ControlEncoding { get; }

        /// <summary>
        /// Gets or sets the encoding for the paths sent over control channel.
        /// </summary>
        public Encoding PathEncoding { get; set; }

        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the virtual file system associated with the user.
        /// </summary>
        public IFileSystem FileSystem { get; set; }

        /// <summary>
        /// Gets or sets the current directory in the virtual file system.
        /// </summary>
        public VirtualPath CurrentDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the item to be renamed with the next rename command.
        /// </summary>
        public VirtualPath RenameSource { get; set; }

        /// <summary>
        /// Gets or sets the offset to restart the next file transfer from.
        /// </summary>
        public long TransferRestartOffset { get; set; }

        /// <summary>
        /// Gets or sets the file transfer type.
        /// </summary>
        public FileTransferType TransferType { get; set; }

        /// <summary>
        /// Gets the control channel for the session.
        /// </summary>
        public IControlChannel ControlChannel { get; private set; }

        /// <summary>
        /// Gets or sets the currently active data channel.
        /// </summary>
        public IDataChannel DataChannel { get; set; }

        /// <summary>
        /// Gets the current logger instance for the session.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpSessionState"/> class
        /// with the provided server address, control channel, server options and logger.
        /// </summary>
        /// <param name="serverAddress">Address of the server</param>
        /// <param name="publicServerAddress">
        /// External IP address of the server. This address will be used in passive mode
        /// </param>
        /// <param name="controlChannel">Control channel for the session</param>
        /// <param name="serverOptions">FTP server options</param>
        /// <param name="logger">Current logger instance</param>
        public FtpSessionState(
            IPAddress serverAddress,
            IPAddress publicServerAddress,
            IControlChannel controlChannel,
            FtpServerOptions serverOptions,
            ILogger logger)
        {
            if (serverAddress == null)
            {
                throw new ArgumentNullException("serverAddress");
            }

            if (publicServerAddress == null)
            {
                throw new ArgumentNullException("publicServerAddress");
            }

            if (controlChannel == null)
            {
                throw new ArgumentNullException("controlChannel");
            }

            if (serverOptions == null)
            {
                throw new ArgumentNullException("serverOptions");
            }
            
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            ServerAddress = serverAddress;
            PublicServerAddress = publicServerAddress;
            ControlChannel = controlChannel;
            LineFeed = serverOptions.CommandEncoding.GetBytes(serverOptions.LineFeed);
            Logger = logger;

            ControlEncoding = serverOptions.CommandEncoding;
            PathEncoding = Encoding.ASCII;

            TransferType = FileTransferType.ASCII;
        }
    }
}
