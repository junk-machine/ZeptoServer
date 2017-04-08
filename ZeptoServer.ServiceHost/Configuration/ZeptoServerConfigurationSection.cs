using System.Configuration;
using System.Xml;

namespace ZeptoServer.ServiceHost.Configuration
{
    /// <summary>
    /// Defines configuration section for the server host configuration.
    /// </summary>
    public sealed class ZeptoServerConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the server host configuration.
        /// </summary>
        public string ServerHostConfiguration { get; private set; }

        /// <summary>
        /// Reads the configuration for the server host.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> to read the configuration from</param>
        protected override void DeserializeSection(XmlReader reader)
        {
            reader.MoveToContent();
            ServerHostConfiguration = reader.ReadInnerXml();
        }
    }
}
