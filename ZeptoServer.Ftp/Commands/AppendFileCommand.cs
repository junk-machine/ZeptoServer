using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZeptoServer.Ftp.FileSystems;

namespace ZeptoServer.Ftp.Commands
{
    /// <summary>
    /// FTP command to append data to an existing file.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc959
    /// </remarks>
    internal sealed class AppendFileCommand : StoreFileCommand
    {
        /// <summary>
        /// Opens the file in append mode.
        /// </summary>
        /// <param name="fileSystem">File system instance</param>
        /// <param name="path">Virtual path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>File stream to write the data</returns>
        protected override Task<Stream> OpenFile(IFileSystem fileSystem, VirtualPath path, CancellationToken cancellation)
        {
            return fileSystem.AppendFile(path, cancellation);
        }
    }
}
