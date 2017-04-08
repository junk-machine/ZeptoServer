using System;
using System.Configuration;
using System.Threading;
using System.Windows.Markup;
using ZeptoServer.ServiceHost.Configuration;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Starts and stops the server.
    /// </summary>
    internal sealed class ZeptoServerHost : IDisposable
    {
        private ManualResetEventSlim stopEvent;

        /// <summary>
        /// Name of the ZeptoServer configuration section.
        /// </summary>
        private const string ZeptoServerConfigurationSectionName = "ZeptoServer";

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeptoServerHost"/> class.
        /// </summary>
        public ZeptoServerHost()
        {
            stopEvent = new ManualResetEventSlim();
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            var configSection =
                ConfigurationManager.GetSection(ZeptoServerConfigurationSectionName)
                    as ZeptoServerConfigurationSection;

            if (configSection == null)
            {
                throw Errors.MissingConfigurationSection(ZeptoServerConfigurationSectionName);
            }

            using (var serverHost =
                XamlReader.Parse(configSection.ServerHostConfiguration)
                    as ServerHost)
            {
                if (serverHost == null)
                {
                    throw Errors.RootIsNotServerHost();
                }

                stopEvent.Wait();
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Stop()
        {
            stopEvent.Set();
        }

        /// <summary>
        /// Stops the server and releases all resources.
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref stopEvent, null);

            if (disposable != null)
            {
                disposable.Set();
                disposable.Dispose();
            }
        }
    }
}
