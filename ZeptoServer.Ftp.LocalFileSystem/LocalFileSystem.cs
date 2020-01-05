using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Information about the stored item or null, if item does not exist.</returns>
        public Task<FileSystemItem> GetItem(VirtualPath path, CancellationToken cancellation)
        {
            var localPath = ToLocalPath(path);

            FileSystemInfo fileSystemInfo = new DirectoryInfo(localPath);

            if (fileSystemInfo.Exists)
            {
                return Task.FromResult(
                    new FileSystemItem(
                        fileSystemInfo.Name,
                        fileSystemInfo.LastWriteTimeUtc));
            }

            fileSystemInfo = new FileInfo(localPath);

            if (fileSystemInfo.Exists)
            {
                return Task.FromResult(
                    new FileSystemItem(
                        fileSystemInfo.Name,
                        ((FileInfo)fileSystemInfo).Length,
                        fileSystemInfo.LastWriteTimeUtc));
            }

            return Task.FromResult<FileSystemItem>(null);
        }

        /// <summary>
        /// Retrieves the information about all items in the directory.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Information about the stored items or null, if directory does not exist.</returns>
        public Task<IEnumerable<FileSystemItem>> ListItems(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                // It is important not to use lazy EnumerateFileSystemInfos() here,
                // because list operation may be invoked as part of delete workflow,
                // but files will become locked until lazy enumeration is finished.
                return Task.FromResult(
                    ToFileSystemItems(
                        new DirectoryInfo(ToLocalPath(path)).GetFileSystemInfos()));
            }
            catch (DirectoryNotFoundException)
            {
                return Task.FromResult<IEnumerable<FileSystemItem>>(null);
            }
        }

        /// <summary>
        /// Checks if the file exists in the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if file exists, otherwise false.</returns>
        public Task<bool> IsFileExist(VirtualPath path, CancellationToken cancellation)
        {
            return Task.FromResult(File.Exists(ToLocalPath(path)));
        }

        /// <summary>
        /// Renames the file in the file system.
        /// </summary>
        /// <param name="source">Name of the file to rename</param>
        /// <param name="target">New name for the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public Task<bool> RenameFile(VirtualPath source, VirtualPath target, CancellationToken cancellation)
        {
            try
            {
                File.Move(ToLocalPath(source), ToLocalPath(target));
                return CommonTasks.True;
            }
            catch
            {
                return CommonTasks.False;
            }
        }

        /// <summary>
        /// Open the file for write.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        public Task<Stream> WriteFile(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                return Task.FromResult<Stream>(
                    new LocalFileStream(
                        File.Open(ToLocalPath(path), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)));
            }
            catch
            {
                return Task.FromResult<Stream>(null);
            }
        }

        /// <summary>
        /// Open the file to append the data.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to write data to the file or null, if file could not be opened.</returns>
        public Task<Stream> AppendFile(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                return Task.FromResult<Stream>(
                    new LocalFileStream(
                        File.Open(ToLocalPath(path), FileMode.Append, FileAccess.Write, FileShare.Read)));
            }
            catch
            {
                return Task.FromResult<Stream>(null);
            }
        }

        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Stream that allows to read data from the file or null, if file could not be opened.</returns>
        public Task<Stream> ReadFile(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                return Task.FromResult<Stream>(
                    File.Open(ToLocalPath(path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            catch
            {
                return Task.FromResult<Stream>(null);
            }
        }

        /// <summary>
        /// Removes the file from the file system.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if file was removed, otherwise false.</returns>
        public Task<bool> RemoveFile(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                File.Delete(ToLocalPath(path));
                return CommonTasks.True;
            }
            catch
            {
                return CommonTasks.False;
            }
        }

        /// <summary>
        /// Checks if the directory exists in the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory exists, otherwise false.</returns>
        public Task<bool> IsDirectoryExist(VirtualPath path, CancellationToken cancellation)
        {
            return Task.FromResult(Directory.Exists(ToLocalPath(path)));
        }

        /// <summary>
        /// Creates new directory in the file system.
        /// </summary>
        /// <param name="path">Name of the new directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory was successfully created, otherwise false.</returns>
        public Task<bool> CreateDirectory(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                Directory.CreateDirectory(ToLocalPath(path));
                return CommonTasks.True;
            }
            catch
            {
                return CommonTasks.False;
            }
        }

        /// <summary>
        /// Renames the directory in the file system.
        /// </summary>
        /// <param name="source">Name of the directory to rename</param>
        /// <param name="target">New name for the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public Task<bool> RenameDirectory(VirtualPath source, VirtualPath target, CancellationToken cancellation)
        {
            try
            {
                Directory.Move(ToLocalPath(source), ToLocalPath(target));
                return CommonTasks.True;
            }
            catch
            {
                return CommonTasks.False;
            }
        }

        /// <summary>
        /// Removes the directory from the file system.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>true if directory was removed, otherwise false.</returns>
        public Task<bool> RemoveDirectory(VirtualPath path, CancellationToken cancellation)
        {
            try
            {
                Directory.Delete(ToLocalPath(path));
                return CommonTasks.True;
            }
            catch
            {
                return CommonTasks.False;
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
