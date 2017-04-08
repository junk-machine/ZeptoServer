namespace ZeptoServer.TextLogger
{
    /// <summary>
    /// Sink interface for the generic <see cref="TextLogger"/>.
    /// </summary>
    public interface ITextLoggerSink
    {
        /// <summary>
        /// Writes the message to the log.
        /// </summary>
        /// <param name="message">The message to log</param>
        void Write(string message);
    }
}
