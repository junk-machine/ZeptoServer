namespace ZeptoServer.Log
{
    /// <summary>
    /// Severity of the log message.
    /// </summary>
    public enum LoggerSeverity
    {
        /// <summary>
        /// Error message.
        /// </summary>
        Error = 0,

        /// <summary>
        /// Warning message.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Informational message.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Verbose message.
        /// </summary>
        Verbose = 3,

        /// <summary>
        /// Debug message.
        /// </summary>
        Debug = 4,
    }
}
