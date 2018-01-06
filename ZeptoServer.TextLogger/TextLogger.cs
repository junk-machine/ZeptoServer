using System;
using System.Globalization;
using ZeptoServer.Log;

namespace ZeptoServer.TextLogger
{
    /// <summary>
    /// Logger that writes all messages in to a specified sink in a text format.
    /// </summary>
    public sealed class TextLogger : ILogger
    {
        /// <summary>
        /// Text logger sink to write text messages to.
        /// </summary>
        private readonly ITextLoggerSink sink;

        /// <summary>
        /// Gets the minimum severity of the messages that should be written to the log.
        /// </summary>
        public LoggerSeverity LogLevel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextLogger"/> class
        /// with the provided log level and sink.
        /// </summary>
        /// <param name="logLevel">Minimum severity of the messages that should be logged</param>
        /// <param name="sink">Text logger sink to write messages to</param>
        public TextLogger(LoggerSeverity logLevel, ITextLoggerSink sink)
        {
            if (sink == null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            LogLevel = logLevel;
            this.sink = sink;
        }

        /// <summary>
        /// Writes debug message to the sink.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteDebug(string message, params object[] arguments)
        {
            if (LogLevel >= LoggerSeverity.Debug)
            {
                Write(LoggerResources.LogLevelDebug, message, arguments);
            }
        }

        /// <summary>
        /// Writes verbose message to the sink.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteVerbose(string message, params object[] arguments)
        {
            if (LogLevel >= LoggerSeverity.Verbose)
            {
                Write(LoggerResources.LogLevelVerbose, message, arguments);
            }
        }

        /// <summary>
        /// Writes informational message to the sink.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteInfo(string message, params object[] arguments)
        {
            if (LogLevel >= LoggerSeverity.Info)
            {
                Write(LoggerResources.LogLevelInfo, message, arguments);
            }
        }

        /// <summary>
        /// Writes warning message to the sink.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteWarning(string message, params object[] arguments)
        {
            if (LogLevel >= LoggerSeverity.Warning)
            {
                Write(LoggerResources.LogLevelWarning, message, arguments);
            }
        }

        /// <summary>
        /// Writes error message to the sink.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteError(string message, params object[] arguments)
        {
            if (LogLevel >= LoggerSeverity.Error)
            {
                Write(LoggerResources.LogLevelError, message, arguments);
            }
        }

        /// <summary>
        /// Formats the message with specified severity level and arguments and writes it to the sink.
        /// </summary>
        /// <param name="level">String representing the severity level of the message</param>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        private void Write(string level, string message, params object[] arguments)
        {
            if (arguments != null && arguments.Length > 0)
            {
                message = String.Format(CultureInfo.InvariantCulture, message, arguments);
            }

            sink.Write(String.Format(LoggerResources.LogMessageFormat, DateTime.Now, level, message));
        }
    }
}
