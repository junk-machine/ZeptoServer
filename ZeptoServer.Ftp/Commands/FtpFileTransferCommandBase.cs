using System.Threading;
using System.Threading.Tasks;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Base class for any FTP command that performs a file transfer.
    /// </summary>
    internal abstract class FtpFileTransferCommandBase : FtpDataCommandBase
    {
        /// <summary>
        /// Prevents restart offset from being removed from the FTP session context before command executes.
        /// </summary>
        /// <param name="session">FTP session context</param>
        protected sealed override void CleanupTransferRestartOffset(FtpSessionState session)
        {
            // Do not cleanup the session state before this command executes
        }

        /// <summary>
        /// Obtains the restart offset from the FTP session context and transfers the file.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected sealed override Task HandleDataCommand(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            var restartOffset = session.TransferRestartOffset;

            session.TransferRestartOffset = 0;

            return HandleFileTransferCommand(arguments, session, restartOffset, cancellation);
        }

        /// <summary>
        /// Transfers the file to or from the client.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="restartOffset">File transfer restart offset</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected abstract Task HandleFileTransferCommand(string arguments, FtpSessionState session, long restartOffset, CancellationToken cancellation);
    }
}
