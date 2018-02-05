using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZeptoServer.Ftp.Commands;
using ZeptoServer.Log;
using ZeptoServer.Telnet;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// FTP server implementation.
    /// </summary>
    internal class FtpServer : TelnetServerBase<FtpServerOptions>
    {
        /// <summary>
        /// Mapping of all commands supported by the server to their handlers.
        /// </summary>
        private readonly IReadOnlyDictionary<string, IFtpCommand> supportedCommands;
        
        /// <summary>
        /// Context of the current FTP session.
        /// </summary>
        private readonly FtpSessionState session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServer"/> class
        /// with the provided socket, server options, list of supported commands and
        /// logger instance.
        /// </summary>
        /// <param name="socket">Socket for the client connection</param>
        /// <param name="serverOptions">FTP server options</param>
        /// <param name="supportedCommands">Collection of supported commands with their handlers</param>
        /// <param name="logger">Current logger instance</param>
        public FtpServer(
            Socket socket,
            FtpServerOptions serverOptions,
            IReadOnlyDictionary<string, IFtpCommand> supportedCommands,
            ILogger logger)
                : base(socket, serverOptions, logger)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (supportedCommands == null)
            {
                throw new ArgumentNullException(nameof(supportedCommands));
            }

            this.supportedCommands = supportedCommands;

            var localServerAddress = ((IPEndPoint)socket.LocalEndPoint).Address;

            session =
                new FtpSessionState(
                    localServerAddress,
                    GetPublicAddress(localServerAddress, serverOptions, logger),
                    this,
                    serverOptions,
                    logger);
        }

        /// <summary>
        /// Sends the response indicating that server is ready to accept commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task OnStart()
        {
            await Send(FtpResponses.ServiceReady);
        }

        /// <summary>
        /// Processes the command and writes the response in the control channel.
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="arguments">Command arguments</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected override async Task OnCommand(string command, IArrayBufferView arguments)
        {
            IFtpCommand commandHandler;

            if (supportedCommands.TryGetValue(command, out commandHandler))
            {
                try
                {
                    await commandHandler.Handle(arguments, session);
                }
                catch (Exception error)
                {
                    session.Logger.WriteError(error.Message);
                    await Send(FtpResponses.InternalError);
                }
            }
            else
            {
                session.Logger.WriteWarning(TraceResources.CommandNotSupported);
                await Send(FtpResponses.NotImplemented);
            }
        }

        /// <summary>
        /// Retrieves the public IP address of the server to use in passive mode.
        /// </summary>
        /// <param name="localIpAddress">Local IP address of the server</param>
        /// <param name="serverOptions">FTP server options</param>
        /// <param name="logger">Current logger instance</param>
        /// <returns>Public IP address of the server.</returns>
        private IPAddress GetPublicAddress(IPAddress localIpAddress, FtpServerOptions serverOptions, ILogger logger)
        {
            var publicIpAddress = localIpAddress;

            if (!String.IsNullOrEmpty(serverOptions.PublicAddress)
                && !IPAddress.TryParse(serverOptions.PublicAddress, out publicIpAddress))
            {
                // Try resolve provided host name to IPv4 address
                try
                {
                    publicIpAddress =
                        Dns.GetHostEntry(serverOptions.PublicAddress)
                            .AddressList
                            .First(a => a.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (Exception error)
                {
                    logger.WriteWarning(
                        TraceResources.CouldNotResolvePublicAddressFormat,
                        serverOptions.PublicAddress,
                        error.Message,
                        publicIpAddress = localIpAddress);
                }
            }

            return publicIpAddress;
        }
    }
}
