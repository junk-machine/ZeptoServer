using System.IO;
using System.Threading.Tasks;

namespace ZeptoServer
{
    /// <summary>
    /// Basic interface for the data flowing through the server.
    /// </summary>
    public interface IDataStream
    {
        /// <summary>
        /// Writes all the data into another stream.
        /// </summary>
        /// <param name="stream">Stream to write data to</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task WriteTo(Stream stream);
    }
}
