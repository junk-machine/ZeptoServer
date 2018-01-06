using System;
using System.IO;
using System.Threading.Tasks;
using ZeptoServer.Log;

namespace ZeptoServer.Ftp.Data
{
    /// <summary>
    /// Base class for FTP data channels.
    /// </summary>
    internal abstract class DataChannelBase : IDataChannel
    {
        /// <summary>
        /// Gets the current logger instance.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataChannelBase"/> class
        /// with the provided logger.
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        public DataChannelBase(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        /// <summary>
        /// Obtains the stream to send and receive the data.
        /// </summary>
        /// <returns>Stream to send and receive the data.</returns>
        public abstract Task<Stream> GetDataStream();

        /// <summary>
        /// Releases all managed and unmanaged resources held by the data channel.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all unmanaged resources held by the data channel.
        /// </summary>
        ~DataChannelBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources held by the data channel.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected abstract void Dispose(bool disposing);
    }
}
