using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using ZeptoServer.ServiceHost.Configuration;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Starts and stops the server.
    /// </summary>
    internal sealed class ZeptoServerHost
    {
        /// <summary>
        /// Name of the ZeptoServer configuration section.
        /// </summary>
        private const string ZeptoServerConfigurationSectionName = "ZeptoServer";

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="cancelToken">Cancellation token to signal stop request</param>
        /// <returns>A <see cref="Task"/> that represents asyncrhonous hosting.</returns>
        public async Task Run(CancellationToken cancelToken)
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

                try
                {
                    await Task.Delay(-1, cancelToken);
                }
                catch (OperationCanceledException)
                {
                    // Stop requested
                }
            }
        }
    }
}
