using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.ServiceProcess;
using System.Threading;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Service host for the ZeptoServer.
    /// </summary>
    public sealed class ZeptoServerService : ServiceBase
    {
        /// <summary>
        /// ZeptoServer host.
        /// </summary>
        private ZeptoServerHost serverHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeptoServerService"/> class.
        /// </summary>
        public ZeptoServerService()
        {
            serverHost = new ZeptoServerHost();
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        protected override void OnStart(string[] args)
        {
            EventLog.Source = GetServiceName();

            try
            {
                serverHost.Start();
            }
            catch (Exception error)
            {
                EventLog.WriteEntry(error.Message, EventLogEntryType.Error, 100);
                throw;
            }
        }

        /// <summary>
        /// Queries the current service name from WMI.
        /// </summary>
        /// <returns>Current server name, if query succeded; default service name otherwise.</returns>
        private static string GetServiceName()
        {
            try
            {
                var objectSearcher =
                    new ManagementObjectSearcher(
                        String.Format(
                            CultureInfo.InvariantCulture,
                            "SELECT * FROM Win32_Service where ProcessId = {0}",
                            Process.GetCurrentProcess().Id));

                foreach (var managementObject in objectSearcher.Get())
                {
                    return managementObject["Name"].ToString();
                }
            }
            catch { }

            return Resources.DefaultServiceName;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        protected override void OnStop()
        {
            serverHost.Stop();
        }

        /// <summary>
        /// Stops the server and deallocates all resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = Interlocked.Exchange(ref serverHost, null);

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
