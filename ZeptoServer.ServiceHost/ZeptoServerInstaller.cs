using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Service installer for the ZeptoServer service.
    /// </summary>
    [RunInstaller(true)]
    public sealed class ZeptoServerInstaller : Installer
    {
        /// <summary>
        /// Name of the InstallUtil option to override default service name.
        /// </summary>
        private const string ServiceNameOption = "ServiceName";

        /// <summary>
        /// Name of the InstallUtil option to override default service display name.
        /// </summary>
        private const string DisplayNameOption = "DisplayName";

        /// <summary>
        /// Name of the InstallUtil option to override default service description.
        /// </summary>
        private const string DescriptionOption = "Description";

        /// <summary>
        /// Service process installer for the ZeptoServer service.
        /// </summary>
        private ServiceProcessInstaller serviceProcessInstaller;

        /// <summary>
        /// Service installer for the ZeptoServer service.
        /// </summary>
        private ServiceInstaller serviceInstaller;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeptoServerInstaller"/> class.
        /// </summary>
        public ZeptoServerInstaller()
        {
            serviceProcessInstaller =
                new ServiceProcessInstaller
                {
                    Account = ServiceAccount.NetworkService,
                    Username = null,
                    Password = null
                };

            Installers.Add(serviceProcessInstaller);

            serviceInstaller =
                new ServiceInstaller
                {
                    ServiceName = Resources.DefaultServiceName,
                    DisplayName = Resources.DefaultServiceName,
                    Description = Resources.DefaultServiceDescription,
                    StartType = ServiceStartMode.Automatic
                };

            Installers.Add(serviceInstaller);
        }

        /// <summary>
        /// Performs required overrides before installing the service.
        /// </summary>
        /// <param name="savedState">State of the computer before service is installed</param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            SetCustomParameters(Context.Parameters);
            base.OnBeforeInstall(savedState);
        }

        /// <summary>
        /// Performs required overrides before uninstalling the service.
        /// </summary>
        /// <param name="savedState">State of the computer before service is uninstalled</param>
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            SetCustomParameters(Context.Parameters);
            base.OnBeforeUninstall(savedState);
        }

        /// <summary>
        /// Overrides custom installer parameters based on the configuration provided to InstallUtil.
        /// </summary>
        /// <param name="context"></param>
        private void SetCustomParameters(StringDictionary context)
        {
            if (context.ContainsKey(ServiceNameOption))
            {
                serviceInstaller.ServiceName = serviceInstaller.DisplayName =
                    context[ServiceNameOption];
            }

            if (context.ContainsKey(DisplayNameOption))
            {
                serviceInstaller.DisplayName = context[DisplayNameOption];
            }

            if (context.ContainsKey(DescriptionOption))
            {
                serviceInstaller.Description = context[DescriptionOption];
            }
        }

        /// <summary>
        /// Disposes all created installers.
        /// </summary>
        /// <param name="disposing">
        /// true to release all resources, false if only unmanaged resources should be released.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable disposable =
                    Interlocked.Exchange(ref serviceProcessInstaller, null);

                if (disposable != null)
                {
                    disposable.Dispose();
                }

                disposable =
                    Interlocked.Exchange(ref serviceInstaller, null);

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
