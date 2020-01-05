using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZeptoServer.Telnet.Responses
{
    /// <summary>
    /// Default implementation of the Telnet server response.
    /// </summary>
    public class Response : IResponse
    {
        /// <summary>
        /// Suffix for the truncated pretty-printed response.
        /// </summary>
        private const string LongResponseSuffix = "...";

        /// <summary>
        /// Gets the response data.
        /// </summary>
        protected byte[] Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reponse"/> class
        /// with the provided data.
        /// </summary>
        /// <param name="data">Response data</param>
        protected Response(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        public Task WriteTo(Stream stream, CancellationToken cancellation)
        {
            return stream.WriteAsync(Data, 0, Data.Length, cancellation);
        }

        /// <summary>
        /// Prints first 20 bytes of repsonse data as HEX.
        /// </summary>
        /// <returns>Collection of HEX strings.</returns>
        public virtual IEnumerable<string> PrettyPrint()
        {
            var isLong = Data.Length > 20;

            return
                new[]
                {
                    BitConverter.ToString(Data, 0, isLong ? 20 : Data.Length)
                        + (isLong ? LongResponseSuffix : string.Empty)
                };
        }
    }
}
