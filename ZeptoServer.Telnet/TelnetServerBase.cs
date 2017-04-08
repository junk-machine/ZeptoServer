using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ZeptoServer.Log;
using ZeptoServer.Telnet.Responses;
using ZeptoServer.Utilities;

namespace ZeptoServer.Telnet
{
    /// <summary>
    /// Base class for any light-weight Telnet server.
    /// </summary>
    /// <typeparam name="TServerOptions">Type of Telnet server-specific options</typeparam>
    public abstract class TelnetServerBase<TServerOptions> : ServerBase<TServerOptions>, IControlChannel
        where TServerOptions : TelnetServerOptions
    {
        /// <summary>
        /// Sequence of bytes separating command name from its arguments in the control channel.
        /// </summary>
        private readonly byte[] argumentCommandSeparator;

        /// <summary>
        /// Current line termination sequence.
        /// </summary>
        private readonly byte[] lineFeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelnetServerBase"/> class
        /// with the provided socket, server options and logger.
        /// </summary>
        /// <param name="socket">Socket for the client connection</param>
        /// <param name="serverOptions">Telnet server options</param>
        /// <param name="logger">Current logger instance</param>
        public TelnetServerBase(Socket socket, TServerOptions serverOptions, ILogger logger)
            : base(socket, serverOptions, logger)
        {
            if (serverOptions == null)
            {
                throw new ArgumentNullException("serverOptions");
            }

            argumentCommandSeparator = serverOptions.CommandEncoding.GetBytes(" ");
            lineFeed = serverOptions.CommandEncoding.GetBytes(serverOptions.LineFeed);
        }

        /// <summary>
        /// Attempts to process the command if it was received in full.
        /// </summary>
        /// <param name="data">Received from the client so far</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected sealed override async Task OnData(ArrayBuffer data)
        {
            if (data.EndsWith(lineFeed))
            {
                data.TrimEnd(lineFeed.Length);

                LogRequest(data);

                var commandLength = data.CountUntil(argumentCommandSeparator);
                var argumentsLength = data.Length - commandLength - 1;

                await OnCommand(
                    data.ToString(0, commandLength, Encoding.ASCII),
                    argumentsLength > 0
                        ? data.GetView(commandLength + 1, argumentsLength)
                        : ArrayBufferView.Empty);

                data.Clear();
            }
        }

        /// <summary>
        /// Processes the command received from the client.
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="arguments">Command arguments</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        protected abstract Task OnCommand(string command, IArrayBufferView arguments);

        /// <summary>
        /// Sends the Telnet server response to the client.
        /// </summary>
        /// <param name="response">Telnet server response</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public async Task Send(IResponse response)
        {
            LogResponse(response);

            await WriteData(response);
            await WriteData(lineFeed);
        }

        #region Logging

        /// <summary>
        /// Logs the entire command received from the client.
        /// </summary>
        /// <param name="request">Received data representing the command</param>
        private void LogRequest(ArrayBuffer request)
        {
            if (Logger.LogLevel >= LoggerSeverity.Verbose)
            {
                Logger.WriteVerbose(TraceResources.RequestFormat, request.ToString(0, request.Length, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Logs the Telnet server response sent to the client.
        /// </summary>
        /// <param name="response">Telenet server response</param>
        private void LogResponse(IResponse response)
        {
            if (Logger.LogLevel >= LoggerSeverity.Verbose)
            {
                foreach (var line in response.PrettyPrint())
                {
                    Logger.WriteVerbose(TraceResources.ResponseFormat, line);
                }
            }
        }

        #endregion Logging
    }
}
