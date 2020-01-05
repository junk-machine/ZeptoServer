using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using ZeptoServer.Configuration.Xml;
using ZeptoServer.Ftp;

namespace ZeptoServer.IotHost
{
    /// <summary>
    /// Entry-point for the background application.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        /// <summary>
        /// Name of the configuration file in the AppData folder.
        /// </summary>
        private const string ConfigurationFile = "Configuration.xml";

        /// <summary>
        /// Cancellation token source to stop the application.
        /// </summary>
        private CancellationTokenSource stopTokenSource;

        /// <summary>
        /// Starts the server with the embedded configuration.
        /// </summary>
        /// <param name="taskInstance">Instance of the background task associated with the application</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            stopTokenSource = new CancellationTokenSource();

            taskInstance.Canceled += Stop;

            var deferral = taskInstance.GetDeferral();

            try
            {
                EnsureSystemNet();

                var configurationXml =
                    await FileIO.ReadTextAsync(
                        await ApplicationData.Current.LocalFolder.GetFileAsync(ConfigurationFile));

                using (var serverHost =
                    new XmlConfigurationLoader().Load<ServerHost>(configurationXml))
                {
                    try
                    {
                        await Task.Delay(-1, stopTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Stop requested
                    }
                }
            }
            catch (Exception error)
            {
                // Log any initialization errors
                var logFile =
                    await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
                        $"{DateTime.Now:ddMMyyyy-hhmmss}.txt",
                        CreationCollisionOption.ReplaceExisting);

                await FileIO.AppendTextAsync(logFile, error.ToString() + Environment.NewLine);
            }

            stopTokenSource.Dispose();
            taskInstance.Canceled -= Stop;

            deferral.Complete();
        }

        /// <summary>
        /// Sends cancellation request to the running server task.
        /// </summary>
        /// <param name="sender">Sender of the stop request</param>
        /// <param name="reason">Termination reason</param>
        private void Stop(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            stopTokenSource.Cancel();
        }

        /// <summary>
        /// Ensure that System.Net classes can be accessed through reflection.
        /// </summary>
        /// <remarks>
        /// There are issues when using reflection in UWP/Win IoT.
        /// This method ensures that we can create <see cref="IPEndPoint"/> and
        /// use <see cref="IPAddress.Any"/> from configuration file.
        /// </remarks>
        private void EnsureSystemNet()
        {
            Activator.CreateInstance(
                typeof(IPEndPoint),
                new object[] { IPAddress.Any, 0 });
        }
    }
}
