using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ZeptoServer.Ftp.Commands;
using ZeptoServer.Log;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Factory to create instances of the FTP server.
    /// </summary>
    public class FtpServerFactory : IServerFactory
    {
        /// <summary>
        /// FTP server options to use with all newly created server instances.
        /// </summary>
        private readonly FtpServerOptions serverOptions;

        /// <summary>
        /// Mapping of all commands supported by the server to their handlers.
        /// </summary>
        private readonly IReadOnlyDictionary<string, IFtpCommand> supportedCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerFactory"/> class
        /// with the provided server options.
        /// </summary>
        /// <param name="serverOptions">FTP server options</param>
        public FtpServerFactory(FtpServerOptions serverOptions)
        {
            if (serverOptions == null)
            {
                throw new ArgumentNullException("serverOptions");
            }

            this.serverOptions = serverOptions;
            supportedCommands = GetSupportedCommands(serverOptions);
        }

        /// <summary>
        /// Creates a new instance of FTP server to handle all client communications.
        /// </summary>
        /// <param name="socket">Socket to commincate with the client</param>
        /// <param name="logger">Logger instance to use in the created server</param>
        /// <returns>New instance of the FTP server.</returns>
        public IServer Create(Socket socket, ILogger logger)
        {
            return new FtpServer(socket, serverOptions, supportedCommands, logger);
        }

        /// <summary>
        /// Creates the list of all FTP commands supported by the server.
        /// </summary>
        /// <param name="serverOptions">FTP server options</param>
        /// <returns>Dictionary of all supported commands with their handlers</returns>
        private static IReadOnlyDictionary<string, IFtpCommand> GetSupportedCommands(FtpServerOptions serverOptions)
        {
            return
                new Dictionary<string, IFtpCommand>(StringComparer.OrdinalIgnoreCase)
                {
                    { FtpCommands.User, new UserCommand(serverOptions.Users) },
                    { FtpCommands.Password, new PasswordCommand(serverOptions.Users) },
                    { FtpCommands.SystemType, new SystemTypeCommand() },
                    { FtpCommands.Features,
                        new FeaturesCommand(
                            new[]
                            {
                                FtpOptions.UTF8,
                                FtpOptions.UTF_8,
                                FtpCommands.ModifiedTime,
                                FtpCommands.FileSize,
                                FtpOptions.RestartableFileTransfer
                            }) },
                    { FtpCommands.Options, new OptionsCommand() },
                    { FtpCommands.PrintWorkingDirectory, new PrintWorkingDirectoryCommand() },
                    { FtpCommands.PrintWorkingDirectoryExtended, new PrintWorkingDirectoryCommand() },
                    { FtpCommands.ChangeWorkingDirectory, new ChangeWorkingDirectoryCommand() },
                    { FtpCommands.ChangeWorkingDirectoryExtended, new ChangeWorkingDirectoryCommand() },
                    { FtpCommands.ChangeDirectoryUp, new ChangeDirectoryUpCommand() },
                    { FtpCommands.ChangeDirectoryUpExtended, new ChangeDirectoryUpCommand() },
                    { FtpCommands.TransferType, new TransferTypeCommand() },
                    { FtpCommands.TransferMode, new TransferModeCommand() },
                    { FtpCommands.FileStructure, new FileStructureCommand() },
                    { FtpCommands.PassiveMode, new PassiveModeCommand() },
                    { FtpCommands.Port, new PortCommand() },
                    { FtpCommands.ListFiles, new ListFilesCommand() },
                    { FtpCommands.ListFileNames, new ListFileNamesCommand() },
                    { FtpCommands.ModifiedTime, new ModifiedTimeCommand() },
                    { FtpCommands.FileSize, new FileSizeCommand() },
                    { FtpCommands.RenameFrom, new RenameFromCommand() },
                    { FtpCommands.RenameTo, new RenameToCommand() },
                    { FtpCommands.AppendFile, new AppendFileCommand() },
                    { FtpCommands.StoreFile, new StoreFileCommand() },
                    { FtpCommands.RetrieveFile, new RetrieveFileCommand() },
                    { FtpCommands.RestartFileTransfer, new RestartFileTransferCommand() },
                    { FtpCommands.AbortFileTransfer, new AbortFileTransferCommand() },
                    { FtpCommands.DeleteFile, new DeleteFileCommand() },
                    { FtpCommands.MakeDirectory, new MakeDirectoryCommand() },
                    { FtpCommands.MakeDirectoryExtended, new MakeDirectoryCommand() },
                    { FtpCommands.RemoveDirectory, new RemoveDirectoryCommand() },
                    { FtpCommands.RemoveDirectoryExtended, new RemoveDirectoryCommand() },
                    { FtpCommands.Allocate, new AllocateCommand() },
                    { FtpCommands.NoOperation, new NoOperationCommand() },
                    { FtpCommands.Reinitialize, new ReinitializeCommand() },
                    { FtpCommands.Quit, new QuitCommand() },
                };
        }
    }
}
