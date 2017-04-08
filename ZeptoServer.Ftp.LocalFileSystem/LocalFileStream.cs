using System;
using System.IO;
using ZeptoServer.Ftp.FileSystems;
using ZeptoServer.Utilities;

namespace ZeptoServer.Ftp.LocalFileSystem
{
    /// <summary>
    /// File stream wrapper that catch out of space excpetions and
    /// converts them to <see cref="NotEnoughSpaceException"/>.
    /// </summary>
    internal sealed class LocalFileStream : WrapperStream
    {
        /// <summary>
        /// The disk is full error.
        /// </summary>
        private const int HR_ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);

        /// <summary>
        /// There is not enough space on the disk error.
        /// </summary>
        private const int HR_ERROR_DISK_FULL = unchecked((int)0x80070070);

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileStream"/> class
        /// with the provided underlying stream.
        /// </summary>
        /// <param name="stream">Underlying stream</param>
        public LocalFileStream(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream and advances the current position
        /// within the stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the underlying stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the underlying stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                base.Write(buffer, offset, count);
            }
            catch (Exception error)
            {
                if (error.HResult == HR_ERROR_HANDLE_DISK_FULL
                    || error.HResult == HR_ERROR_DISK_FULL)
                {
                    throw new NotEnoughSpaceException();
                }

                throw;
            }
        }
    }
}
