using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Base class for any FTP command that requires an authenticated user.
    /// </summary>
    internal abstract class FtpAuthorizedCommandBase : FtpCommandBase
    {
        /// <summary>
        /// Verifies user authentication status and performs all necessary actions for the command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public override async Task Handle(IArrayBufferView arguments, FtpSessionState session, CancellationToken cancellation)
        {
            if (session.FileSystem == null)
            {
                await session.ControlChannel.Send(FtpResponses.NotLoggedIn, cancellation);
                return;
            }

            await base.Handle(arguments, session, cancellation);
        }
    }
}
