using System.Collections.Generic;

namespace ZeptoServer.Telnet.Responses
{
    /// <summary>
    /// Telnet server response.
    /// </summary>
    public interface IResponse : IDataStream
    {
        /// <summary>
        /// Gets collection of text lines that represent the response.
        /// </summary>
        /// <returns>Collection of text lines that represent the response.</returns>
        IEnumerable<string> PrettyPrint();
    }
}
