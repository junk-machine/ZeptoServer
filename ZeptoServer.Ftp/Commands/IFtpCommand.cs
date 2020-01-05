using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Handler of the FTP command.
    /// </summary>
    internal interface IFtpCommand
    {
        /// <summary>
        /// Performs all necessary actions for the command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task Handle(IArrayBufferView arguments, FtpSessionState session, CancellationToken cancellation);
    }
}
