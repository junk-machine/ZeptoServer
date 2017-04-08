using System;
using System.ServiceProcess;
using System.Threading;

namespace ZeptoServer.ServiceHost
{
    /// <summary>
    /// Defines entry point for the ZeptoServer host process.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Command line switch to run the application as regular cosole app, rather than a Windows service.
        /// </summary>
        private const string RunAsConsoleSwitch = "-c";

        /// <summary>
        /// Entry point for the ZeptoServer host process.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0
                && RunAsConsoleSwitch.Equals(args[0], StringComparison.Ordinal))
            {
                // Interactive mode
                using (var host = new ZeptoServerHost())
                {
                    new Thread(host.Start).Start();
                    Console.ReadLine();
                }
            }
            else
            {
                // Windows service mode
                using (var service = new ZeptoServerService())
                {
                    ServiceBase.Run(service);
                }
            }
        }
    }
}
