using System;
using System.Text;

namespace ZeptoServer.Telnet
{
    /// <summary>
    /// Light-weight Telnet server options.
    /// </summary>
    public class TelnetServerOptions : ServerOptions
    {
        /// <summary>
        /// Gets or sets the encoding to use for Telnet commands.
        /// </summary>
        public Encoding CommandEncoding { get; set; }

        /// <summary>
        /// Gets or sets the line termination sequence.
        /// </summary>
        public string LineFeed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelnetServerOptions"/> class.
        /// </summary>
        public TelnetServerOptions()
            : base()
        {
            CommandEncoding = Encoding.ASCII;
            LineFeed = Environment.NewLine;
        }
    }
}
