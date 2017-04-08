using System;
using System.Collections.Generic;
using System.Text;
using ZeptoServer.Utilities;

namespace ZeptoServer.Telnet.Responses
{
    /// <summary>
    /// Multi-line Telnet server response.
    /// </summary>
    public sealed class ListResponse : Response
    {
        /// <summary>
        /// Indentation of every line containing the list item.
        /// </summary>
        private const string ListIndentation = " ";

        /// <summary>
        /// Current line termination sequence.
        /// </summary>
        private readonly byte[] lineFeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResponse"/> class
        /// with the provided code, message, items list and line feed.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Response text message</param>
        /// <param name="items">Items to send</param>
        /// <param name="lineFeed">Line termination sequence</param>
        public ListResponse(int code, string message, IEnumerable<string> items, byte[] lineFeed)
            : base(GetData(code, message, items, lineFeed))
        {
            this.lineFeed = lineFeed;
        }

        /// <summary>
        /// Generates binary data that represents the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Response text message</param>
        /// <param name="items">Items to send</param>
        /// <param name="lineFeed">Line termination sequence</param>
        /// <returns>Binary data that represents the request.</returns>
        private static byte[] GetData(int code, string message, IEnumerable<string> items, byte[] lineFeed)
        {
            var response = new ArrayBuffer();

            response.Append(Encoding.ASCII.GetBytes(code + "-" + message));
            response.Append(lineFeed);

            foreach (var item in items)
            {
                response.Append(Encoding.ASCII.GetBytes(ListIndentation));
                response.Append(Encoding.ASCII.GetBytes(item));
                response.Append(lineFeed);
            }

            response.Append(Encoding.ASCII.GetBytes(code + " End"));

            return response.GetBuffer();
        }

        /// <summary>
        /// Gets collection of text lines that represent the response.
        /// </summary>
        /// <returns>Collection of text lines that represent the response.</returns>
        public override IEnumerable<string> PrettyPrint()
        {
            return Encoding.ASCII.GetString(Data)
                .Split(new[] { Encoding.ASCII.GetString(lineFeed) }, StringSplitOptions.None);
        }
    }
}
