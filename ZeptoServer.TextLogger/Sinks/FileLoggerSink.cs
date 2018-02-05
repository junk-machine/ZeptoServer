using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace ZeptoServer.TextLogger.Sinks
{
    /// <summary>
    /// Sink for the generic <see cref="TextLogger"/> that writes all messages to the file.
    /// Files are being rolled over on a daily basis and old files are removed based on
    /// the provided archive duration.
    /// </summary>
    public sealed class FileLoggerSink : ITextLoggerSink, IDisposable
    {
        /// <summary>
        /// Date format to use in log file names.
        /// </summary>
        private const string DateFileNameFormat = "yyyy-MM-dd";

        /// <summary>
        /// Name pattern for log files.
        /// </summary>
        private const string LogFileNamePattern = "{0}-{1:yyyy-MM-dd}.log";

        /// <summary>
        /// File search pattern to match log files.
        /// </summary>
        private const string LogFileSearchPattern = "{0}-????-??-??.log";

        /// <summary>
        /// Path to the log folder.
        /// </summary>
        private readonly string logPath;

        /// <summary>
        /// Base name for the log files.
        /// </summary>
        private readonly string baseFileName;

        /// <summary>
        /// Maximum number of days to keep log files.
        /// </summary>
        private readonly int archiveDays;

        /// <summary>
        /// Date for which log is being currently written.
        /// </summary>
        private DateTime currentDate;

        /// <summary>
        /// Active log file writer.
        /// </summary>
        private TextWriter writer;

        /// <summary>
        /// Synchronization object for log file writer rollover operation.
        /// </summary>
        private object writerRolloverSyncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerSink"/> class
        /// with the provided log path, base file name and archive duration.
        /// </summary>
        /// <param name="logPath">Path to the log folder</param>
        /// <param name="baseFileName">Base name for the log files</param>
        /// <param name="archiveDays">Maximum number of days to keep log files</param>
        public FileLoggerSink(string logPath, string baseFileName, int archiveDays)
        {
            if (String.IsNullOrEmpty(logPath))
            {
                throw new ArgumentNullException(nameof(logPath));
            }

            if (String.IsNullOrEmpty(baseFileName))
            {
                throw new ArgumentNullException(nameof(baseFileName));
            }

            if (archiveDays < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(archiveDays));
            }

            this.logPath = logPath;
            this.baseFileName = baseFileName;
            this.archiveDays = archiveDays;

            currentDate = DateTime.MinValue;
        }

        /// <summary>
        /// Writes the message to the file.
        /// </summary>
        /// <param name="message">Message to write</param>
        public void Write(string message)
        {
            var timestamp = DateTime.Now;

            if (timestamp.Date > currentDate)
            {
                lock (writerRolloverSyncObj)
                {
                    if (timestamp.Date > currentDate)
                    {
                        if (writer != null)
                        {
                            writer.Dispose();
                        }

                        writer = GetLogWriter(timestamp);
                        currentDate = timestamp.Date;

                        ThreadPool.QueueUserWorkItem(CleanupLogFolder, currentDate);
                    }
                }
            }
            
            try
            {
                writer.WriteLine(message);
            }
            catch (ObjectDisposedException)
            {
                // Original writer was rolled over, lets obtain rollover lock and write to the new writer.
                // Inside the rollover lock it should be guaranteed that writer is open.
                lock (writerRolloverSyncObj)
                {
                    writer.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Creates new log writer instance that writes to a file for the given timestamp.
        /// </summary>
        /// <param name="timestamp">Timestamp to create a log file for</param>
        /// <returns>Log writer for the given timestamp</returns>
        private TextWriter GetLogWriter(DateTime timestamp)
        {
            Directory.CreateDirectory(logPath);

            var logFilePath = 
                Path.Combine(
                    logPath,
                    String.Format(
                        CultureInfo.InvariantCulture,
                        LogFileNamePattern,
                        baseFileName,
                        timestamp));

            return TextWriter.Synchronized(new StreamWriter(logFilePath, true));
        }

        /// <summary>
        /// Removes old log files from the log folder.
        /// </summary>
        /// <param name="state">Current log file timestamp</param>
        private void CleanupLogFolder(object state)
        {
            try
            {
                var allowedOldestDate = ((DateTime)state).AddDays(-archiveDays);

                foreach (var file
                    in Directory.EnumerateFiles(
                        logPath,
                        String.Format(CultureInfo.InvariantCulture, LogFileSearchPattern, baseFileName)))
                {
                    try
                    {
                        DateTime fileTimestamp;

                        if (DateTime.TryParseExact(
                                Path.GetFileNameWithoutExtension(file).Substring(baseFileName.Length),
                                DateFileNameFormat,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeUniversal,
                                out fileTimestamp)
                            && fileTimestamp < allowedOldestDate)
                        {
                            File.Delete(file);
                        }
                    }
                    catch { /* Best effort */ }
                }
            }
            catch
            {
                // Clean-up is best effort only, make sure we don't throw any exceptions
                // on the background thread
            }
        }

        /// <summary>
        /// Releases the active log file writer.
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref writer, null);

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
