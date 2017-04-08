using System.Collections.Generic;
using System.Text;

namespace ZeptoServer.Telnet.Responses
{
    /// <summary>
    /// Simple Telnet server response with a message.
    /// </summary>
    public sealed class MessageResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponse"/> class
        /// with the provided code and message.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Message text</param>
        public MessageResponse(int code, string message)
            : base(GetData(code, message))
        {
        }

        /// <summary>
        /// Generates binary data that represents the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Response text message</param>
        /// <returns>Binary data that represents the request.</returns>
        private static byte[] GetData(int code, string message)
        {
            return Encoding.ASCII.GetBytes(code + " " + message);
        }

        /// <summary>
        /// Gets collection of text lines that represent the response.
        /// </summary>
        /// <returns>Collection of text lines that represent the response.</returns>
        public override IEnumerable<string> PrettyPrint()
        {
            return new[] { Encoding.ASCII.GetString(Data) };
        }
    }
}
