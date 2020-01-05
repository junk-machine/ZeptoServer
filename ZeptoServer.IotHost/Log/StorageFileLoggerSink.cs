using System;
using Windows.Storage;
using ZeptoServer.TextLogger;

namespace ZeptoServer.IotHost.Log
{
    /// <summary>
    /// Sink for the generic <see cref="TextLogger.TextLogger"/> that writes all messages to
    /// the <see cref="IStorageFile"/>.
    /// </summary>
    internal sealed class StorageFileLoggerSink : ITextLoggerSink
    {
        /// <summary>
        /// Target log file.
        /// </summary>
        private readonly IStorageFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageFileLoggerSink"/> class
        /// with the provided storage file.
        /// </summary>
        /// <param name="file"></param>
        public StorageFileLoggerSink(IStorageFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.file = file;
        }

        /// <summary>
        /// Appends given <paramref name="message"/> to the log file.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Write(string message)
        {
            // Do not await, as we log asynchronously
            _ = FileIO.AppendTextAsync(file, message + Environment.NewLine);
        }
    }
}
