using System;
using System.IO;
using System.Threading.Tasks;

namespace ZeptoServer.Ftp.Data
{
    /// <summary>
    /// FTP data channel.
    /// </summary>
    internal interface IDataChannel : IDisposable
    {
        /// <summary>
        /// Obtains the stream to send and receive the data.
        /// </summary>
        /// <returns>Stream to send and receive the data.</returns>
        Task<Stream> GetDataStream();
    }
}
