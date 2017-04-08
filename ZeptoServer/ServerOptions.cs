namespace ZeptoServer
{
    /// <summary>
    /// Light-weight server options.
    /// </summary>
    public class ServerOptions
    {
        /// <summary>
        /// Gets or sets the maximum size of the chunk when reading the data.
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOptions"/> class.
        /// </summary>
        public ServerOptions()
        {
            BufferSize = 8192;
        }
    }
}
