using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Base class for all FTP command handlers.
    /// </summary>
    internal abstract class FtpCommandBase : IFtpCommand
    {
        /// <summary>
        /// Cleans up volatile session state and performs all necessary actions for the command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public virtual async Task Handle(IArrayBufferView arguments, FtpSessionState session)
        {
            CleanupRenameSource(session);
            CleanupTransferRestartOffset(session);
            await HandleCommand(ReadArguments(arguments, session), session);
        }

        /// <summary>
        /// Converts command arguments from binary form to string using current control channel encoding.
        /// </summary>
        /// <param name="arguments">Arguments in binary form</param>
        /// <param name="session">FTP session context</param>
        /// <returns>Command arguments as string.</returns>
        protected virtual string ReadArguments(IArrayBufferView arguments, FtpSessionState session)
        {
            return arguments.ToString(session.ControlEncoding);
        }

        /// <summary>
        /// Performs all necessary actions for the command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected virtual async Task HandleCommand(string arguments, FtpSessionState session)
        {
            await session.ControlChannel.Send(Handle(arguments, session));
        }

        /// <summary>
        /// Handles the command and returns a single FTP server response that will be sent to the client.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected abstract IResponse Handle(string arguments, FtpSessionState session);

        /// <summary>
        /// Empties the name of the item to be renamed with the next rename command.
        /// </summary>
        /// <param name="session">FTP session context</param>
        protected virtual void CleanupRenameSource(FtpSessionState session)
        {
            session.RenameSource = null;
        }

        /// <summary>
        /// Resets the offset to restart the next file transfer from.
        /// </summary>
        /// <param name="session">FTP session context</param>
        protected virtual void CleanupTransferRestartOffset(FtpSessionState session)
        {
            session.TransferRestartOffset = 0;
        }
    }
}
