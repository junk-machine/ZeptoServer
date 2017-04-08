using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// Base class for any FTP command that accepts file system path as an argument.
    /// </summary>
    internal abstract class FtpPathCommandBase : FtpAuthorizedCommandBase
    {
        /// <summary>
        /// Converts command arguments from binary form to string using current paths encoding.
        /// </summary>
        /// <param name="arguments">Arguments in binary form</param>
        /// <param name="session">FTP session context</param>
        /// <returns>Command arguments as string.</returns>
        protected override string ReadArguments(IArrayBufferView arguments, FtpSessionState session)
        {
            return arguments.ToString(session.PathEncoding);
        }
    }
}
