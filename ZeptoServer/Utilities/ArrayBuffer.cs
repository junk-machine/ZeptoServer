using System;
using System.Text;

namespace ZeptoServer.Utilities
{
    /// <summary>
    /// Buffer that dynamically grows and shrinks its underlying storage array as needed.
    /// </summary>
    public sealed class ArrayBuffer
    {
        /// <summary>
        /// Length of the underlying storage array, when there is no data in it.
        /// </summary>
        private const int DesiredSize = 1024;

        /// <summary>
        /// Actual storage array.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Gets the current number of bytes stored in the buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBuffer"/> class.
        /// </summary>
        public ArrayBuffer()
        {
            buffer = new byte[DesiredSize];
        }

        /// <summary>
        /// Appends data to the buffer.
        /// </summary>
        /// <param name="data">Data to append</param>
        public void Append(byte[] data)
        {
            Append(data, 0, data.Length);
        }

        /// <summary>
        /// Appends data to the buffer reading specified number of bytes from specified index.
        /// </summary>
        /// <param name="data">Data to append</param>
        /// <param name="index">Index to read data from</param>
        /// <param name="length">Number of data bytes to read</param>
        public void Append(byte[] data, int index, int length)
        {
            // Ensure enough size
            while (length - (buffer.Length - Length) > 0)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            Array.Copy(data, index, buffer, Length, length);

            Length += length;
        }

        /// <summary>
        /// Checks if stored data ends with the provided sequence.
        /// </summary>
        /// <param name="sequence">Sequence to test</param>
        /// <returns>true if stored data ends with the sequence, otherwise false.</returns>
        public bool EndsWith(byte[] sequence)
        {
            if (Length < sequence.Length)
            {
                return false;
            }

            for (var index = 0; index < sequence.Length; ++index)
            {
                if (buffer[Length - sequence.Length + index] != sequence[index])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes specified number of bytes from the buffer.
        /// </summary>
        /// <param name="count">Number of bytes to remove</param>
        public void TrimEnd(int count)
        {
            if (Length < count)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            Length -= count;
        }

        /// <summary>
        /// Looks up the specified syntax and returns its starting index.
        /// If sequence is not found in the buffer - this method returns the length of the buffer.
        /// </summary>
        /// <param name="sequence">Sequence to look up</param>
        /// <returns>Starting index of the requested sequence or length of the buffer</returns>
        public int CountUntil(byte[] sequence)
        {
            var sequenceIndex = 0;
            var maxSequenceStartIndex = Length - sequence.Length + 1;

            for (var index = 0; index < maxSequenceStartIndex; ++index)
            {
                for (sequenceIndex = 0; sequenceIndex < sequence.Length; ++sequenceIndex)
                {
                    if (buffer[index + sequenceIndex] != sequence[sequenceIndex])
                    {
                        break;
                    }
                }

                if (sequenceIndex == sequence.Length)
                {
                    return index;
                }
            }

            return Length;
        }

        /// <summary>
        /// Removes all data from the buffer.
        /// </summary>
        public void Clear()
        {
            Length = 0;

            if (buffer.Length > DesiredSize)
            {
                Array.Resize(ref buffer, DesiredSize);
            }
        }

        /// <summary>
        /// Returns raw data.
        /// </summary>
        /// <returns>Byte array containing all data stored in the buffer.</returns>
        public byte[] GetBuffer()
        {
            if (buffer.Length > Length)
            {
                // Shrink the buffer first
                Array.Resize(ref buffer, Length);
            }

            return buffer;
        }

        /// <summary>
        /// Gets the view of the buffer restricted by the specified range.
        /// </summary>
        /// <param name="index">View starting index</param>
        /// <param name="count">Number of bytes to include in the view</param>
        /// <returns>An instance of the array view for the specified range.</returns>
        public IArrayBufferView GetView(int index, int count)
        {
            return new ArrayBufferView(this, index, count);
        }

        /// <summary>
        /// Converts the data in the buffer into its string form using provided encoding.
        /// </summary>
        /// <param name="index">Starting index</param>
        /// <param name="count">Number of bytes to convert</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>String representation of the data.</returns>
        public string ToString(int index, int count, Encoding encoding)
        {
            return encoding.GetString(buffer, index, count);
        }
    }
}
