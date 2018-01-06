using System;
using System.IO;
using System.Text;
using ZeptoServer.Log;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.Data
{
    /// <summary>
    /// Traces all the data that comes through the stream.
    /// </summary>
    internal sealed class StreamTracer : WrapperStream
    {
        /// <summary>
        /// Logger to write the data to.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTracer"/> class
        /// with the provided nderlying stream and logger instance.
        /// </summary>
        /// <param name="stream">Underlying stream</param>
        /// <param name="logger">Logger to log the data to</param>
        public StreamTracer(Stream stream, ILogger logger)
            : base(stream)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying stream, advances the position
        /// within the stream by the number of bytes read and logs the read data.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between <paramref name="offset"/> and
        /// (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by
        /// the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read
        /// from the underlying stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the underlying stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number
        /// of bytes requested if that many bytes are not currently available, or zero (0)
        /// if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = Stream.Read(buffer, offset, count);

            logger.WriteDebug(Encoding.ASCII.GetString(buffer, offset, read));

            return read;
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream, advances the current position
        /// within the stream by the number of bytes written and logs the data that was written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the underlying stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the underlying stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            logger.WriteDebug(Encoding.ASCII.GetString(buffer, offset, count));
            Stream.Write(buffer, offset, count);
        }
    }
}
