using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ZeptoServer.Telnet.Responses
{
    public class Response : IResponse
    {
        private const string LongResponseSuffix = "...";

        protected byte[] Data { get; private set; }

        protected Response(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        public Task WriteTo(Stream stream)
        {
            return stream.WriteAsync(Data, 0, Data.Length);
        }

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
