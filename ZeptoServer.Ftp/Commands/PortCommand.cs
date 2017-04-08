using System;
using System.Net;
using ZeptoServer.Ftp.Data;
using ZeptoServer.Log;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to enter active transfer mode.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class PortCommand : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Creates a new active mode data channel for the FTP session.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            if (String.IsNullOrEmpty(arguments))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            var segments = arguments.Split(',');

            EndPoint endPoint;
            if (!TryGetEndPoint(segments, out endPoint))
            {
                return FtpResponses.ParameterSyntaxError;
            }

            session.DataChannel =
                new ActiveDataChannel(
                    endPoint,
                    new LoggerScope(TraceResources.ActiveModeLoggerScope, session.Logger));

            return FtpResponses.Success;
        }

        /// <summary>
        /// Tries to parse provided segments as an end-point definition.
        /// </summary>
        /// <param name="segments">End-point segments</param>
        /// <param name="endPoint">Parsed end-point definition</param>
        /// <returns>true if end-point segments were parsed successfully, otherwise false.</returns>
        private bool TryGetEndPoint(string[] segments, out EndPoint endPoint)
        {
            endPoint = null;

            if (segments.Length != 6)
            {
                return false;
            }

            var octets = new byte[6];

            for (var index = 0; index < octets.Length; ++index)
            {
                if (!byte.TryParse(segments[index], out octets[index]))
                {
                    return false;
                }
            }

            var port = octets[4] * 256 + octets[5];
            Array.Resize(ref octets, 4);

            endPoint = new IPEndPoint(new IPAddress(octets), port);

            return true;
        }
    }
}
