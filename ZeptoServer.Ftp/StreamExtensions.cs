using System;
using System.IO;
using System.Threading.Tasks;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Defines extension methods for the <see cref="Stream"/> class.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Adjust stream position to the desired offset.
        /// </summary>
        /// <param name="stream">Stream to adjust</param>
        /// <param name="offset">Offset</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public static async Task SetOffset(this Stream stream, long offset)
        {
            if (stream.CanSeek)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }
            else if (stream.CanRead)
            {
                var buffer = new byte[8192];

                while (offset > 0)
                {
                    offset -=
                        await stream.ReadAsync(
                            buffer, 0, (int)Math.Min(buffer.Length, offset));
                }
            }
        }
    }
}
