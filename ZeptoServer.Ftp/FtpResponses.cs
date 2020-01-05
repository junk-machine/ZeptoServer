using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ZeptoServer.Telnet.Responses;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Defines well-known FTP responses used by the server.
    /// </summary>
    internal static class FtpResponses
    {
        /// <summary>
        /// File status okay; about to open data connection.
        /// </summary>
        public static readonly IResponse OpenningDataChannel =
            new MessageResponse(150, FtpMessages.OpenningDataChannel);

        /// <summary>
        /// The requested action has been successfully completed.
        /// </summary>
        public static readonly IResponse Success =
            new MessageResponse(200, FtpMessages.Success);

        /// <summary>
        /// The requested action is not necessary.
        /// </summary>
        public static readonly IResponse CommandSuperfluous =
            new MessageResponse(202, FtpMessages.Success);

        /// <summary>
        /// Response to the <see cref="FtpCommands.SystemType"/> command.
        /// </summary>
        public static readonly IResponse SystemType =
            new MessageResponse(215, FtpMessages.SystemType);

        /// <summary>
        /// First server response after client connection is made.
        /// </summary>
        public static readonly IResponse ServiceReady =
            new MessageResponse(220, FtpMessages.ServiceReady);

        /// <summary>
        /// Goodbye message response.
        /// </summary>
        public static readonly IResponse Disconnect =
            new MessageResponse(221, FtpMessages.Disconnect);

        /// <summary>
        /// Closing data connection. Requested file action successful (for example, file transfer or file abort).
        /// </summary>
        public static readonly IResponse TransferComplete =
            new MessageResponse(226, FtpMessages.TransferComplete);

        /// <summary>
        /// Response to the <see cref="FtpCommands.Password"/> command when password is accepted.
        /// </summary>
        public static readonly IResponse LoggedIn =
            new MessageResponse(230, FtpMessages.LoggedIn);

        /// <summary>
        /// Requested file action okay, completed.
        /// </summary>
        public static readonly IResponse FileActionOk =
            new MessageResponse(250, FtpMessages.FileActionOk);

        /// <summary>
        /// Response to the <see cref="FtpCommands.User"/> command when username is accepted.
        /// </summary>
        public static readonly IResponse UserOk =
            new MessageResponse(331, FtpMessages.UserOk);

        /// <summary>
        /// Response to the file-related commands that require another command to be issue to complete the request.
        /// For example, file rename consists of two command RNFR and RNTO.
        /// </summary>
        public static readonly IResponse FileMoreInfoRequired =
            new MessageResponse(350, FtpMessages.FileMoreInfoRequired);

        /// <summary>
        /// Cannot open data connection.
        /// </summary>
        public static readonly IResponse CanNotOpenDataChannel =
            new MessageResponse(425, FtpMessages.CanNotOpenDataChannel);

        /// <summary>
        /// Invalid username.
        /// </summary>
        public static readonly IResponse InvalidUsername =
            new MessageResponse(430, FtpMessages.InvalidUsername);

        /// <summary>
        /// Response for internal server error.
        /// </summary>
        public static readonly IResponse InternalError =
            new MessageResponse(500, FtpMessages.InternalError);

        /// <summary>
        /// Syntax error in parameters or arguments.
        /// </summary>
        public static readonly IResponse ParameterSyntaxError =
            new MessageResponse(501, FtpMessages.NotImplemented);

        /// <summary>
        /// Command not implemented.
        /// </summary>
        public static readonly IResponse NotImplemented =
            new MessageResponse(502, FtpMessages.NotImplemented);

        /// <summary>
        /// Command not implemented for that parameter.
        /// </summary>
        public static readonly IResponse NotImplementedForParameter =
            new MessageResponse(504, FtpMessages.NotImplementedForParameter);

        /// <summary>
        /// User is not logged in.
        /// </summary>
        public static readonly IResponse NotLoggedIn =
            new MessageResponse(530, FtpMessages.NotLoggedIn);

        /// <summary>
        /// Requested action not taken. File unavailable (e.g., file not found, no access).
        /// </summary>
        public static readonly IResponse FileUnavailable =
            new MessageResponse(550, FtpMessages.FileUnavailable);

        /// <summary>
        /// Requested file action aborted. Exceeded storage allocation (for current directory or dataset).
        /// </summary>
        public static readonly IResponse NotEnoughSpace =
            new MessageResponse(552, FtpMessages.NotEnoughSpace);

        /// <summary>
        /// Response to the <see cref="FtpCommands.Features"/> command.
        /// </summary>
        /// <param name="features">List of supported features</param>
        /// <param name="lineFeed">Line separator sequence</param>
        /// <returns>Response to the client listing features supported by the server.</returns>
        public static IResponse Features(IEnumerable<string> features, byte[] lineFeed)
        {
            return new ListResponse(211, FtpMessages.Features, features, lineFeed);
        }

        /// <summary>
        /// Response to file-related commands, e.g. MDTM, SIZE.
        /// </summary>
        /// <param name="status">File status info</param>
        /// <returns>Response to the client containing file status.</returns>
        public static IResponse FileStatus(string status)
        {
            return new MessageResponse(213, status);
        }

        /// <summary>
        /// Passive mode listener active.
        /// </summary>
        /// <param name="address">Listener address segments</param>
        /// <param name="port">Listener port number</param>
        /// <returns>Response to the client containing information about passive mode listener.</returns>
        public static IResponse PassiveMode(byte[] address, int port)
        {
            return new MessageResponse(
                227,
                String.Format(
                    CultureInfo.InvariantCulture,
                    FtpMessages.PassiveModeFormat,
                    address[0], address[1], address[2], address[3],
                    port / 256, port % 256));
        }

        /// <summary>
        /// Response to the path-related commands, e.g. PWD, MKD.
        /// </summary>
        /// <param name="path">Full path</param>
        /// <param name="encoding">Path encoding to use</param>
        /// <returns>Response to the client with the current path.</returns>
        public static IResponse Path(string path, Encoding encoding)
        {
            return new PathResponse(257, path, encoding);
        }
    }
}
