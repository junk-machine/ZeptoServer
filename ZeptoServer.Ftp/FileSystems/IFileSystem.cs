using System.Collections.Generic;
using System.IO;

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
        /// <returns>Information about the stored item or null, if item does not exist.</returns>
        FileSystemItem GetItem(VirtualPath path);

        /// <summary>
        /// Retrieves the information about all items in the directory.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>Information about the stored items or null, if directory does not exist.</returns>
        IEnumerable<FileSystemItem> ListItems(VirtualPath path);

        /// <summary>
        /// Checks if the file exists in the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if file exists, otherwise false.</returns>
        bool IsFileExist(VirtualPath path);

        /// <summary>
        /// Renames the file in the file system.
        /// </summary>
        /// <param name="source">Name of the file to rename</param>
        /// <param name="target">New name for the file</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        bool RenameFile(VirtualPath source, VirtualPath target);

        /// <summary>
        /// Open the file for write.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        Stream WriteFile(VirtualPath path);

        /// <summary>
        /// Open the file to append the data.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        Stream AppendFile(VirtualPath path);

        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to read data from the file or null, if file could not be opened.</returns>
        Stream ReadFile(VirtualPath path);

        /// <summary>
        /// Removes the file from the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if file was removed, otherwise false.</returns>
        bool RemoveFile(VirtualPath path);

        /// <summary>
        /// Checks if the directory exists in the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory exists, otherwise false.</returns>
        bool IsDirectoryExist(VirtualPath path);

        /// <summary>
        /// Creates new directory in the file system.
        /// </summary>
        /// <param name="path">Name of the new directory</param>
        /// <returns>true if directory was successfully created, otherwise false.</returns>
        bool CreateDirectory(VirtualPath path);

        /// <summary>
        /// Renames the directory in the file system.
        /// </summary>
        /// <param name="source">Name of the directory to rename</param>
        /// <param name="target">New name for the directory</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        bool RenameDirectory(VirtualPath source, VirtualPath target);

        /// <summary>
        /// Removes the directory from the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory was removed, otherwise false.</returns>
        bool RemoveDirectory(VirtualPath path);
    }
}
