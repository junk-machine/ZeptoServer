namespace ZeptoServer.Log
{
    /// <summary>
    /// Logger implementation that discards all messages.
    /// </summary>
    public sealed class NullLogger : ILogger
    {
        /// <summary>
        /// Gets the <see cref="LoggerSeverity.Error"/> log level.
        /// </summary>
        public LoggerSeverity LogLevel
        {
            get { return LoggerSeverity.Error; }
        }

        /// <summary>
        /// Ignores all debug messages.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteDebug(string message, params object[] arguments)
        {
        }

        /// <summary>
        /// Ignores all verbose messages.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteVerbose(string message, params object[] arguments)
        {
        }

        /// <summary>
        /// Ignores all informational messages.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteInfo(string message, params object[] arguments)
        {
        }

        /// <summary>
        /// Ignores all warning messages.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteWarning(string message, params object[] arguments)
        {
        }

        /// <summary>
        /// Ignores all error messages.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteError(string message, params object[] arguments)
        {
        }
    }
}
