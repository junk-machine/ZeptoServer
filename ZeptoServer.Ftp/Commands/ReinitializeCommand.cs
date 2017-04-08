using System.Text;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to reinitialize the session.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class ReinitializeCommand : FtpCommandBase
    {
        /// <summary>
        /// Logs out current user without disconnecting the client socket.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="session">FTP session context</param>
        /// <returns>FTP server response to send to the client.</returns>
        protected override IResponse Handle(string arguments, FtpSessionState session)
        {
            try
            {
                session.DataChannel.Dispose();
            }
            catch { }

            session.DataChannel = null;
            session.Username = null;
            session.FileSystem = null;
            session.CurrentDirectory = null;

            session.PathEncoding = Encoding.ASCII;
            session.TransferType = FileTransferType.ASCII;

            return FtpResponses.ServiceReady;
        }
    }
}
