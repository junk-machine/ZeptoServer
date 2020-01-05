using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Defines well-known FTP responses used by the server to return from async methods.
    /// </summary>
    internal static class FtpResponsesAsync
    {
        /// <summary>
        /// File status okay; about to open data connection.
        /// </summary>
        public static readonly Task<IResponse> OpenningDataChannel =
            Task.FromResult(FtpResponses.OpenningDataChannel);

        /// <summary>
        /// The requested action has been successfully completed.
        /// </summary>
        public static readonly Task<IResponse> Success =
            Task.FromResult(FtpResponses.Success);

        /// <summary>
        /// The requested action is not necessary.
        /// </summary>
        public static readonly Task<IResponse> CommandSuperfluous =
            Task.FromResult(FtpResponses.CommandSuperfluous);

        /// <summary>
        /// Response to the <see cref="FtpCommands.SystemType"/> command.
        /// </summary>
        public static readonly Task<IResponse> SystemType =
            Task.FromResult(FtpResponses.SystemType);

        /// <summary>
        /// First server response after client connection is made.
        /// </summary>
        public static readonly Task<IResponse> ServiceReady =
            Task.FromResult(FtpResponses.ServiceReady);

        /// <summary>
        /// Goodbye message response.
        /// </summary>
        public static readonly Task<IResponse> Disconnect =
            Task.FromResult(FtpResponses.Disconnect);

        /// <summary>
        /// Closing data connection. Requested file action successful (for example, file transfer or file abort).
        /// </summary>
        public static readonly Task<IResponse> TransferComplete =
            Task.FromResult(FtpResponses.TransferComplete);

        /// <summary>
        /// Response to the <see cref="FtpCommands.Password"/> command when password is accepted.
        /// </summary>
        public static readonly Task<IResponse> LoggedIn =
            Task.FromResult(FtpResponses.LoggedIn);

        /// <summary>
        /// Requested file action okay, completed.
        /// </summary>
        public static readonly Task<IResponse> FileActionOk =
            Task.FromResult(FtpResponses.FileActionOk);

        /// <summary>
        /// Response to the <see cref="FtpCommands.User"/> command when username is accepted.
        /// </summary>
        public static readonly Task<IResponse> UserOk =
            Task.FromResult(FtpResponses.UserOk);

        /// <summary>
        /// Response to the file-related commands that require another command to be issue to complete the request.
        /// For example, file rename consists of two command RNFR and RNTO.
        /// </summary>
        public static readonly Task<IResponse> FileMoreInfoRequired =
            Task.FromResult(FtpResponses.FileMoreInfoRequired);

        /// <summary>
        /// Cannot open data connection.
        /// </summary>
        public static readonly Task<IResponse> CanNotOpenDataChannel =
            Task.FromResult(FtpResponses.CanNotOpenDataChannel);

        /// <summary>
        /// Invalid username.
        /// </summary>
        public static readonly Task<IResponse> InvalidUsername =
            Task.FromResult(FtpResponses.InvalidUsername);

        /// <summary>
        /// Response for internal server error.
        /// </summary>
        public static readonly Task<IResponse> InternalError =
            Task.FromResult(FtpResponses.InternalError);

        /// <summary>
        /// Syntax error in parameters or arguments.
        /// </summary>
        public static readonly Task<IResponse> ParameterSyntaxError =
            Task.FromResult(FtpResponses.ParameterSyntaxError);

        /// <summary>
        /// Command not implemented.
        /// </summary>
        public static readonly Task<IResponse> NotImplemented =
            Task.FromResult(FtpResponses.NotImplemented);

        /// <summary>
        /// Command not implemented for that parameter.
        /// </summary>
        public static readonly Task<IResponse> NotImplementedForParameter =
            Task.FromResult(FtpResponses.NotImplementedForParameter);

        /// <summary>
        /// User is not logged in.
        /// </summary>
        public static readonly Task<IResponse> NotLoggedIn =
            Task.FromResult(FtpResponses.NotLoggedIn);

        /// <summary>
        /// Requested action not taken. File unavailable (e.g., file not found, no access).
        /// </summary>
        public static readonly Task<IResponse> FileUnavailable =
            Task.FromResult(FtpResponses.FileUnavailable);

        /// <summary>
        /// Requested file action aborted. Exceeded storage allocation (for current directory or dataset).
        /// </summary>
        public static readonly Task<IResponse> NotEnoughSpace =
            Task.FromResult(FtpResponses.NotEnoughSpace);

        /// <summary>
        /// Response to the <see cref="FtpCommands.Features"/> command.
        /// </summary>
        /// <param name="features">List of supported features</param>
        /// <param name="lineFeed">Line separator sequence</param>
        /// <returns>Response to the client listing features supported by the server.</returns>
        public static Task<IResponse> Features(IEnumerable<string> features, byte[] lineFeed)
        {
            return Task.FromResult(FtpResponses.Features(features, lineFeed));
        }

        /// <summary>
        /// Response to file-related commands, e.g. MDTM, SIZE.
        /// </summary>
        /// <param name="status">File status info</param>
        /// <returns>Response to the client containing file status.</returns>
        public static Task<IResponse> FileStatus(string status)
        {
            return Task.FromResult(FtpResponses.FileStatus(status));
        }

        /// <summary>
        /// Passive mode listener active.
        /// </summary>
        /// <param name="address">Listener address segments</param>
        /// <param name="port">Listener port number</param>
        /// <returns>Response to the client containing information about passive mode listener.</returns>
        public static Task<IResponse> PassiveMode(byte[] address, int port)
        {
            return Task.FromResult(FtpResponses.PassiveMode(address, port));
        }

        /// <summary>
        /// Response to the path-related commands, e.g. PWD, MKD.
        /// </summary>
        /// <param name="path">Full path</param>
        /// <param name="encoding">Path encoding to use</param>
        /// <returns>Response to the client with the current path.</returns>
        public static Task<IResponse> Path(string path, Encoding encoding)
        {
            return Task.FromResult(FtpResponses.Path(path, encoding));
        }
    }
}
