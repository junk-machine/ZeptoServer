using System;

namespace ZeptoServer.Ftp.FileSystems
{
    /// <summary>
    /// Contains information about an item stored in the file system.
    /// </summary>
    public sealed class FileSystemItem
    {
        /// <summary>
        /// Gets the value indicating whether item is a directory or a file.
        /// </summary>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the size in bytes of the item.
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Gets the last modification timestamp of the item.
        /// </summary>
        public DateTime LastModifiedTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItem"/> class
        /// with the information about the directory.
        /// </summary>
        /// <param name="name">Name of the directory</param>
        /// <param name="lastModifiedTime">Last modification timestamp of the directory</param>
        public FileSystemItem(string name, DateTime lastModifiedTime)
            : this(true, name, 0, lastModifiedTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItem"/> class
        /// with the information about the file.
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="size">Size of the file</param>
        /// <param name="lastModifiedTime">Last modification timestamp of the file</param>
        public FileSystemItem(string name, long size, DateTime lastModifiedTime)
            : this(false, name, size, lastModifiedTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItem"/> class
        /// with the provided information about the item.
        /// </summary>
        /// <param name="isDirectory">Whether item is a directory or a file</param>
        /// <param name="name">Name of the item</param>
        /// <param name="size">Size of the item</param>
        /// <param name="lastModifiedTime">Last modification timestamp of the file</param>
        private FileSystemItem(bool isDirectory, string name, long size, DateTime lastModifiedTime)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            IsDirectory = isDirectory;
            Name = name;
            Size = size;
            LastModifiedTime = lastModifiedTime;
        }
    }
}
