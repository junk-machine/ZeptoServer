using System;
using System.Runtime.Serialization;

namespace ZeptoServer.Ftp.AzureStorage
{
    /// <summary>
    /// Exception that is thrown when BLOB copy operation fails.
    /// </summary>
    [Serializable]
    public class BlobCopyFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCopyFailedException"/> class.
        /// </summary>
        public BlobCopyFailedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCopyFailedException"/> class
        /// with the provided message.
        /// </summary>
        /// <param name="message">Error message</param>
        public BlobCopyFailedException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCopyFailedException"/> class
        /// with the provided message and inner exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Original error that caused this exception</param>
        public BlobCopyFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCopyFailedException"/> class
        /// with the provided serialization info and streaming context.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data
        /// about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information
        /// about the source or destination.
        /// </param>
        protected BlobCopyFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
