using System.Net;
using ZeptoServer.Ftp.Data;
using ZeptoServer.Log;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to enter passive transfer mode.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class PassiveModeCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Creates a new passive mode data channel for the FTP session.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            var channel =
                new PassiveDataChannel(
                    new IPEndPoint(session.ServerAddress, 0),
                    new LoggerScope(TraceResources.PassiveModeLoggerScope, session.Logger));

            session.DataChannel = channel;
                
            return
                FtpResponses.PassiveMode(
                    session.PublicServerAddress.GetAddressBytes(),
                    channel.EndPoint.Port);
        }
    }
}
