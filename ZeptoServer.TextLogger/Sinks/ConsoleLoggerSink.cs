using System;

namespace ZeptoServer.TextLogger.Sinks
{
    /// <summary>
    /// Sink for the generic <see cref="TextLogger"/> that prints all messages to the console.
    /// </summary>
    public sealed class ConsoleLoggerSink : ITextLoggerSink
    {
        /// <summary>
        /// Prints the message to the console.
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
