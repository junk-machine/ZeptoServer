using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Base class for any FTP command that needs to send data over the data channel.
    /// </summary>
    internal abstract class FtpDataCommandBase : FtpPathCommandBase
    {
        /// <summary>
        /// Performs all necessary actions for the command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected sealed override Task HandleCommand(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            return HandleDataCommand(arguments, session, cancellation);
        }

        /// <summary>
        /// Performs all necessary actions for the data command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected abstract Task HandleDataCommand(string arguments, FtpSessionState session, CancellationToken cancellation);

        /// <summary>
        /// This method is not used by data commands, as data commands return multiple responses during the data transfer.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session state</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response.</returns>
        protected sealed override Task<IResponse> Handle(string arguments, FtpSessionState session, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes data to the currently active data channel.
        /// </summary>
        /// <typeparam name="TState">Type of the state object to pass to writer delegate.</typeparam>
        /// <param name="session">FTP session state</param>
        /// <param name="writer">Delegate that writes the actual data to the data channel</param>
        /// <param name="state">State object to pass to writer delegate</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>FTP server response.</returns>
        protected async Task<IResponse> WriteToDataChannel<TState>(
            FtpSessionState session,
            Func<Stream, FtpSessionState, TState, CancellationToken, Task<IResponse>> writer,
            TState state,
            CancellationToken cancellation)
        {
            var dataChannel = session.DataChannel;

            if (dataChannel == null)
            {
                await session.ControlChannel.Send(FtpResponses.CanNotOpenDataChannel, cancellation);
                return FtpResponses.CanNotOpenDataChannel;
            }

            IResponse transferResult;

            try
            {
                using (var dataStream = await dataChannel.GetDataStream())
                {
                    await session.ControlChannel.Send(FtpResponses.OpenningDataChannel, cancellation);

                    transferResult = await writer(dataStream, session, state, cancellation);
                }
            }
            catch (ObjectDisposedException)
            {
                await session.ControlChannel.Send(FtpResponses.CanNotOpenDataChannel, cancellation);
                return FtpResponses.CanNotOpenDataChannel;
            }

            try
            {
                session.DataChannel.Dispose();
            }
            catch { }

            session.DataChannel = null;

            await session.ControlChannel.Send(transferResult, cancellation);
            return transferResult;
        }
    }
}
