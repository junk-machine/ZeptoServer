using System;
using System.Runtime.Serialization;

namespace ZeptoServer.Ftp.FileSystems
{
    /// <summary>
    /// Exception that is thrown when there is not enough space in the file system to store the file.
    /// </summary>
    public class NotEnoughSpaceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughSpaceException"/> class.
        /// </summary>
        public NotEnoughSpaceException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughSpaceException"/> class
        /// with the provided message.
        /// </summary>
        /// <param name="message">Error message</param>
        public NotEnoughSpaceException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughSpaceException"/> class
        /// with the provided message and inner exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Original error that caused this exception</param>
        public NotEnoughSpaceException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughSpaceException"/> class
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
        protected NotEnoughSpaceException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
