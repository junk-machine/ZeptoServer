using System;
using System.Text;

namespace ZeptoServer.Utilities
{
    /// <summary>
    /// View of the buffer restricted by the range.
    /// </summary>
    public sealed class ArrayBufferView : IArrayBufferView
    {
        /// <summary>
        /// Ciew of the empty buffer.
        /// </summary>
        public static readonly IArrayBufferView Empty = new ArrayBufferView(new ArrayBuffer(), 0, 0);

        /// <summary>
        /// Underlying array buffer.
        /// </summary>
        private readonly ArrayBuffer buffer;

        /// <summary>
        /// Starting index of the view range.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// Gets the length of the view range.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBufferView"/> class
        /// with the provided underlying buffer, starting index and length.
        /// </summary>
        /// <param name="buffer">Underlying array buffer</param>
        /// <param name="index">Starting index of the view range</param>
        /// <param name="length">Length of the view range</param>
        public ArrayBufferView(ArrayBuffer buffer, int index, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length < index)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (buffer.Length - index < length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this.buffer = buffer;
            this.index = index;
            Length = length;
        }

        /// <summary>
        /// Converts all data in the view range into its string form using provided encoding.
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <returns>String representation of the data.</returns>
        public string ToString(Encoding encoding)
        {
            return ToString(0, Length, encoding);
        }

        /// <summary>
        /// Converts the portion of the data in the view range into its string form using provided encoding.
        /// </summary>
        /// <param name="index">Starting index</param>
        /// <param name="count">Number of bytes to convert</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>String representation of the data.</returns>
        public string ToString(int index, int count, Encoding encoding)
        {
            return buffer.ToString(this.index + index, count, encoding);
        }
    }
}
