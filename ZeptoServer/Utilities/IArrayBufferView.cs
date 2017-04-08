using System.Text;

namespace ZeptoServer.Utilities
{
    /// <summary>
    /// View of the buffer restricted by the range.
    /// </summary>
    public interface IArrayBufferView
    {
        /// <summary>
        /// Gets the length of the view range.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Converts all data in the view range into its string form using provided encoding.
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <returns>String representation of the data.</returns>
        string ToString(Encoding encoding);

        /// <summary>
        /// Converts the portion of the data in the view range into its string form using provided encoding.
        /// </summary>
        /// <param name="index">Starting index</param>
        /// <param name="count">Number of bytes to convert</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>String representation of the data.</returns>
        string ToString(int index, int count, Encoding encoding);
    }
}
