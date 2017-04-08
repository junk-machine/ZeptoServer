using System;
using System.Globalization;

namespace ZeptoServer.Log
{
    /// <summary>
    /// Logger implementation that encapsulates real logger and appends predifined scope name
    /// to all messages.
    /// </summary>
    public sealed class LoggerScope : ILogger
    {
        /// <summary>
        /// Scope name.
        /// </summary>
        private readonly string scopePrefix;

        /// <summary>
        /// Underlying logger instance.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the minimum severity of the messages that should be written to the log.
        /// </summary>
        public LoggerSeverity LogLevel
        {
            get { return logger.LogLevel; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerScope"/> class
        /// with the provided scope and underlying logger.
        /// </summary>
        /// <param name="scope">Scope identifier</param>
        /// <param name="logger">Underlying logger instance</param>
        public LoggerScope(object scope, ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.scopePrefix =
                String.Format(
                    CultureInfo.InvariantCulture,
                    TraceResources.ScopeNameFormat,
                    scope);

            this.logger = logger;
        }

        /// <summary>
        /// Appends the scope and writes debug message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteDebug(string message, params object[] arguments)
        {
            logger.WriteDebug(scopePrefix + message, arguments);
        }

        /// <summary>
        /// Appends the scope and writes verbose message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteVerbose(string message, params object[] arguments)
        {
            logger.WriteVerbose(scopePrefix + message, arguments);
        }

        /// <summary>
        /// Appends the scope and writes informational message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteInfo(string message, params object[] arguments)
        {
            logger.WriteInfo(scopePrefix + message, arguments);
        }

        /// <summary>
        /// Appends the scope and writes warning message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteWarning(string message, params object[] arguments)
        {
            logger.WriteWarning(scopePrefix + message, arguments);
        }

        /// <summary>
        /// Appends the scope and writes error message to the log.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="arguments">Message arguments</param>
        public void WriteError(string message, params object[] arguments)
        {
            logger.WriteError(scopePrefix + message, arguments);
        }
    }
}
