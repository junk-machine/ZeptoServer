using System;
using System.Configuration;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Defines all errors thrown by the console host.
    /// </summary>
    internal static class Errors
    {
        /// <summary>
        /// Error that is being thrown when required configuration section is missing.
        /// </summary>
        /// <param name="sectionName">Name of the required section</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception MissingConfigurationSection(string sectionName)
        {
            return new ConfigurationErrorsException(
                String.Format(Resources.MissingConfigurationSectionFormat, sectionName));
        }

        /// <summary>
        /// Error that is being thrown when root configuration node is not a ServerHost.
        /// </summary>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception RootIsNotServerHost()
        {
            return new ConfigurationErrorsException(Resources.RootIsNotServerHost);
        }
    }
}
