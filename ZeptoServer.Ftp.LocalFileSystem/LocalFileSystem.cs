using System;
using System.Collections.Generic;
using System.IO;
using ZeptoServer.Ftp.FileSystems;

namespace ZeptoServer.Ftp.LocalFileSystem
{
    /// <summary>
    /// File system that maps to local directory.
    /// </summary>
    public sealed class LocalFileSystem : IFileSystem
    {
        /// <summary>
        /// Path separator for local file paths.
        /// </summary>
        private static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// Path to the local directory representing the root of the virtual file system.
        /// </summary>
        private readonly string basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileSystem"/> class
        /// with the provided local base path.
        /// </summary>
        /// <param name="basePath">
        /// Path to the local directory representing the root of the virtual file system
        /// </param>
        public LocalFileSystem(string basePath)
        {
            if (String.IsNullOrEmpty(basePath))
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            this.basePath = basePath;
        }

        /// <summary>
        /// Retrieves the information about the item.
        /// </summary>
        /// <param name="path">Path to the item</param>
        /// <returns>Information about the stored item or null, if item does not exist.</returns>
        public FileSystemItem GetItem(VirtualPath path)
        {
            var localPath = ToLocalPath(path);

            FileSystemInfo fileSystemInfo = new DirectoryInfo(localPath);

            if (fileSystemInfo.Exists)
            {
                return
                    new FileSystemItem(
                        fileSystemInfo.Name,
                        fileSystemInfo.LastWriteTimeUtc);
            }

            fileSystemInfo = new FileInfo(localPath);

            if (fileSystemInfo.Exists)
            {
                return
                    new FileSystemItem(
                        fileSystemInfo.Name,
                        ((FileInfo)fileSystemInfo).Length,
                        fileSystemInfo.LastWriteTimeUtc);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the information about all items in the directory.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>Information about the stored items or null, if directory does not exist.</returns>
        public IEnumerable<FileSystemItem> ListItems(VirtualPath path)
        {
            try
            {
                // It is important not to use lazy EnumerateFileSystemInfos() here,
                // because list operation may be invoked as part of delete workflow,
                // but files will become locked until lazy enumeration is finished.
                return ToFileSystemItems(
                    new DirectoryInfo(ToLocalPath(path)).GetFileSystemInfos());
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the file exists in the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if file exists, otherwise false.</returns>
        public bool IsFileExist(VirtualPath path)
        {
            return File.Exists(ToLocalPath(path));
        }

        /// <summary>
        /// Renames the file in the file system.
        /// </summary>
        /// <param name="source">Name of the file to rename</param>
        /// <param name="target">New name for the file</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public bool RenameFile(VirtualPath source, VirtualPath target)
        {
            try
            {
                File.Move(ToLocalPath(source), ToLocalPath(target));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Open the file for write.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        public Stream WriteFile(VirtualPath path)
        {
            try
            {
                return new LocalFileStream(
                    File.Open(ToLocalPath(path), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Open the file to append the data.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        public Stream AppendFile(VirtualPath path)
        {
            try
            {
                return new LocalFileStream(
                    File.Open(ToLocalPath(path), FileMode.Append, FileAccess.Write, FileShare.Read));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to read data from the file or null, if file could not be opened.</returns>
        public Stream ReadFile(VirtualPath path)
        {
            try
            {
                return File.Open(ToLocalPath(path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the file from the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if file was removed, otherwise false.</returns>
        public bool RemoveFile(VirtualPath path)
        {
            try
            {
                File.Delete(ToLocalPath(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the directory exists in the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory exists, otherwise false.</returns>
        public bool IsDirectoryExist(VirtualPath path)
        {
            return Directory.Exists(ToLocalPath(path));
        }

        /// <summary>
        /// Creates new directory in the file system.
        /// </summary>
        /// <param name="path">Name of the new directory</param>
        /// <returns>true if directory was successfully created, otherwise false.</returns>
        public bool CreateDirectory(VirtualPath path)
        {
            try
            {
                Directory.CreateDirectory(ToLocalPath(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Renames the directory in the file system.
        /// </summary>
        /// <param name="source">Name of the directory to rename</param>
        /// <param name="target">New name for the directory</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public bool RenameDirectory(VirtualPath source, VirtualPath target)
        {
            try
            {
                Directory.Move(ToLocalPath(source), ToLocalPath(target));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the directory from the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory was removed, otherwise false.</returns>
        public bool RemoveDirectory(VirtualPath path)
        {
            try
            {
                Directory.Delete(ToLocalPath(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts collection of <see cref="FileSystemInfo"/> objects to the collection of <see cref="FileSystemItem"/> objects.
        /// </summary>
        /// <param name="fileSystemInfos">
        /// Collection of <see cref="FileSystemInfo"/> holding information about items stored in the local file system.
        /// </param>
        /// <returns>Collection of <see cref="FileSystemItem"/> objects.</returns>
        private IEnumerable<FileSystemItem> ToFileSystemItems(IEnumerable<FileSystemInfo> fileSystemInfos)
        {
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                if (fileSystemInfo is FileInfo)
                {
                    yield return
                        new FileSystemItem(
                            fileSystemInfo.Name,
                            ((FileInfo)fileSystemInfo).Length,
                            fileSystemInfo.LastWriteTimeUtc);
                }
                else
                {
                    yield return
                        new FileSystemItem(
                            fileSystemInfo.Name,
                            fileSystemInfo.LastWriteTimeUtc);
                }
            }
        }

        /// <summary>
        /// Converts virtual file path to a local file path.
        /// </summary>
        /// <param name="path">Virtual file path</param>
        /// <returns>Local file path to the stored item.</returns>
        private string ToLocalPath(VirtualPath path)
        {
            return Path.Combine(basePath, String.Join(PathSeparator, path.Segments));
        }
    }
}
