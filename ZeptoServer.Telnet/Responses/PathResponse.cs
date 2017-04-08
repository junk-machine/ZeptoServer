using System;
using System.Collections.Generic;
using System.Text;

namespace ZeptoServer.Telnet.Responses
{
    /// <summary>
    /// Telnet server response containing a file path.
    /// </summary>
    public sealed class PathResponse : Response
    {
        /// <summary>
        /// Character to use when quoting the actual path.
        /// </summary>
        private const char Quote = '"';

        /// <summary>
        /// Initializes a new instance of the <see cref="PathResponse"/> class
        /// with the code, path and path encoding.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="path">File path</param>
        /// <param name="pathEncoding">Text encoding to use when sending the path</param>
        public PathResponse(int code, string path, Encoding pathEncoding)
            : base(GetData(code, path, pathEncoding))
        {
        }

        /// <summary>
        /// Generates binary data that represents the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="path">File path</param>
        /// <param name="pathEncoding">Text encoding to use when sending the path</param>
        /// <returns>Binary data that represents the request.</returns>
        private static byte[] GetData(int code, string path, Encoding pathEncoding)
        {
            // Path responses should always quote the actual path
            path = Quote + path + Quote;

            var header = code + " ";

            var headerLength = Encoding.ASCII.GetByteCount(header);
            var pathLength = pathEncoding.GetByteCount(path);

            var data = new byte[headerLength + pathLength];

            Array.Copy(Encoding.ASCII.GetBytes(header), data, headerLength);
            Array.Copy(pathEncoding.GetBytes(path), 0, data, headerLength, pathLength);

            return data;
        }

        /// <summary>
        /// Gets collection of text lines that represent the response.
        /// </summary>
        /// <returns>Collection of text lines that represent the response.</returns>
        public override IEnumerable<string> PrettyPrint()
        {
            // Nasty trick here - instead of storing the original path encoding, use UTF8
            // This should cover ASCII and UTF8 which are two possible encodings
            return new[] { Encoding.UTF8.GetString(Data) };
        }
    }
}
