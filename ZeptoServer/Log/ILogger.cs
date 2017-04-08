namespace ZeptoServer.Log
{
    /// <summary>
    /// Common logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the minimum severity of the messages that should be written to the log.
        /// </summary>
        LoggerSeverity LogLevel { get; }

        /// <summary>
        /// Writes debug message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        void WriteDebug(string message, params object[] arguments);

        /// <summary>
        /// Writes verbose message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        void WriteVerbose(string message, params object[] arguments);

        /// <summary>
        /// Writes informational message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        void WriteInfo(string message, params object[] arguments);

        /// <summary>
        /// Writes warning message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        void WriteWarning(string message, params object[] arguments);

        /// <summary>
        /// Writes error message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        void WriteError(string message, params object[] arguments);
    }
}
