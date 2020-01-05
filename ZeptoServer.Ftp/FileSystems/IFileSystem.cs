using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZeptoServer.Ftp.FileSystems
{
    /// <summary>
    /// Generic file system interface.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Retrieves the information about the item.
        /// </summary>
        /// <param name="path">Path to the item</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Information about the stored item or null, if item does not exist.</returns>
        Task<FileSystemItem> GetItem(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Retrieves the information about all items in the directory.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Information about the stored items or null, if directory does not exist.</returns>
        Task<IEnumerable<FileSystemItem>> ListItems(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Checks if the file exists in the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if file exists, otherwise false.</returns>
        Task<bool> IsFileExist(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Renames the file in the file system.
        /// </summary>
        /// <param name="source">Name of the file to rename</param>
        /// <param name="target">New name for the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        Task<bool> RenameFile(VirtualPath source, VirtualPath target, CancellationToken cancellation);

        /// <summary>
        /// Open the file for write.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        Task<Stream> WriteFile(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Open the file to append the data.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        Task<Stream> AppendFile(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to read data from the file or null, if file could not be opened.</returns>
        Task<Stream> ReadFile(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Removes the file from the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if file was removed, otherwise false.</returns>
        Task<bool> RemoveFile(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Checks if the directory exists in the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory exists, otherwise false.</returns>
        Task<bool> IsDirectoryExist(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Creates new directory in the file system.
        /// </summary>
        /// <param name="path">Name of the new directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory was successfully created, otherwise false.</returns>
        Task<bool> CreateDirectory(VirtualPath path, CancellationToken cancellation);

        /// <summary>
        /// Renames the directory in the file system.
        /// </summary>
        /// <param name="source">Name of the directory to rename</param>
        /// <param name="target">New name for the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        Task<bool> RenameDirectory(VirtualPath source, VirtualPath target, CancellationToken cancellation);

        /// <summary>
        /// Removes the directory from the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory was removed, otherwise false.</returns>
        Task<bool> RemoveDirectory(VirtualPath path, CancellationToken cancellation);
    }
}
