using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ZeptoServer.Ftp.FileSystems;
using System.Threading;

namespace ZeptoServer.Ftp.AzureStorage
{
    /// <summary>
    /// File system that maps to Azure BLOB Storage account.
    /// </summary>
    public sealed class AzureStorageFileSystem : IFileSystem
    {
        /// <summary>
        /// Character sequence that separates BLOB path segments.
        /// </summary>
        private const string BlobPathSeparator = "/";

        /// <summary>
        /// Time stamp value to return when date is not known.
        /// </summary>
        private static readonly DateTime DefaultDateTime = new DateTime(1980, 1, 1);

        /// <summary>
        /// Azure BLOB container that represents the root of the virtual file system.
        /// </summary>
        private readonly CloudBlobContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageFileSystem"/> class
        /// with the provided Azure Storage connection string and container name.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string</param>
        /// <param name="containerName">Container name</param>
        public AzureStorageFileSystem(string connectionString, string containerName)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (String.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            container =
                CloudStorageAccount
                    .Parse(connectionString)
                    .CreateCloudBlobClient()
                    .GetContainerReference(containerName);

            if (container.CreateIfNotExists())
            {
                container.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off
                    });
            }
        }

        /// <summary>
        /// Retrieves the information about the item.
        /// </summary>
        /// <param name="path">Path to the item</param>
        /// <returns>Information about the stored item or null, if item does not exist.</returns>
        public FileSystemItem GetItem(VirtualPath path)
        {
            var cloudBlob =
                container.GetBlobReferenceFromServer(ToBlobName(path, false));

            if (cloudBlob.Exists())
            {
                return
                    new FileSystemItem(
                        GetBlobLocalName(cloudBlob),
                        cloudBlob.Properties.Length,
                        GetBlobModifiedTime(cloudBlob));
            }

            cloudBlob =
                container.GetBlobReferenceFromServer(ToBlobName(path, true));

            if (cloudBlob.Exists())
            {
                return
                    new FileSystemItem(
                        GetBlobLocalName(cloudBlob),
                        GetBlobModifiedTime(cloudBlob));
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
            return ToFileSystemItems(
                container.ListBlobs(ToBlobName(path, true)));
        }

        /// <summary>
        /// Checks if the BLOB exists in the Azure Storage.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if file exists, otherwise false.</returns>
        public bool IsFileExist(VirtualPath path)
        {
            return
                container
                    .GetBlobReference(ToBlobName(path, false))
                    .Exists();
        }

        /// <summary>
        /// Renames the BLOB in the Azure Storage.
        /// </summary>
        /// <param name="source">Name of the file to rename</param>
        /// <param name="target">New name for the file</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public bool RenameFile(VirtualPath source, VirtualPath target)
        {
            try
            {
                RenameBlob(
                    container.GetBlobReference(ToBlobName(source, false)),
                    container.GetBlobReference(ToBlobName(target, false)));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Open the BLOB for write.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the BLOB or null, if BLOB could not be opened.</returns>
        public Stream WriteFile(VirtualPath path)
        {
            try
            {
                return
                    container
                        .GetAppendBlobReference(ToBlobName(path, false))
                        .OpenWrite(true);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Open the BLOB to append the data.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to write data to the BLOB or null, if BLOB could not be opened.</returns>
        public Stream AppendFile(VirtualPath path)
        {
            try
            {
                return
                    container
                        .GetAppendBlobReference(ToBlobName(path, false))
                        .OpenWrite(false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Opens the BLOB for read.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Stream that allows to read data from the BLOB or null, if BLOB could not be opened.</returns>
        public Stream ReadFile(VirtualPath path)
        {
            try
            {
                return
                    container
                        .GetBlobReference(ToBlobName(path, false))
                        .OpenRead();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the BLOB from the Azure Storage.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>true if BLOB was removed, otherwise false.</returns>
        public bool RemoveFile(VirtualPath path)
        {
            try
            {
                container
                    .GetBlobReference(ToBlobName(path, false))
                    .Delete();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the directory exists in the Azure Storage.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory exists, otherwise false.</returns>
        public bool IsDirectoryExist(VirtualPath path)
        {
            // Since Azure Storage does not support directories - there is nothing to check.
            // Once there are BLOBs with path segments in the name - directories will show up
            // in ListBlobs.
            return true;
        }

        /// <summary>
        /// Creates new directory in the Azure Storage.
        /// </summary>
        /// <param name="path">Name of the new directory</param>
        /// <returns>true if directory was successfully created, otherwise false.</returns>
        public bool CreateDirectory(VirtualPath path)
        {
            try
            {
                container
                    .GetBlockBlobReference(ToBlobName(path, true))
                    .UploadText(string.Empty);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Renames the directory in the Azure Storage.
        /// </summary>
        /// <param name="source">Name of the directory to rename</param>
        /// <param name="target">New name for the directory</param>
        /// <returns>true if rename operation succeeded, otherwise false.</returns>
        public bool RenameDirectory(VirtualPath source, VirtualPath target)
        {
            try
            {
                var sourceBlobName = ToBlobName(source, true);
                var targetBlobName = ToBlobName(target, true);

                foreach (var blob
                    in container.ListBlobs(sourceBlobName, true))
                {
                    var cloudBlob = blob as CloudBlob;

                    if (cloudBlob == null)
                    {
                        throw Errors.UnsupportedBlobType(blob.GetType());
                    }

                    var newBlobName =
                        cloudBlob.Name.Replace(sourceBlobName, targetBlobName);

                    RenameBlob(
                        (CloudBlob)blob,
                        container.GetBlobReference(newBlobName));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the directory from the Azure Storage.
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>true if directory was removed, otherwise false.</returns>
        public bool RemoveDirectory(VirtualPath path)
        {
            try
            {
                // Only delete if it's an empty folder (single BLOB exists).
                // There is a race condition possible, where somebody uploads a file in this folder
                // while it's being deleted. In this case folder file may not exist, but folder will
                // still be discovered and will disappear when last file is deleted.
                var folderBlob =
                    container
                        .ListBlobs(ToBlobName(path, true), true)
                        .SingleOrDefault() as CloudBlob;

                if (folderBlob != null)
                {
                    folderBlob.Delete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts collection of <see cref="IListBlobItem"/> objects to the collection of <see cref="FileSystemItem"/> objects.
        /// </summary>
        /// <param name="blobs">
        /// Collection of <see cref="IListBlobItem"/> holding information about stored BLOBs.
        /// </param>
        /// <returns>Collection of <see cref="FileSystemItem"/> objects.</returns>
        private IEnumerable<FileSystemItem> ToFileSystemItems(IEnumerable<IListBlobItem> blobs)
        {
            foreach (var blob in blobs)
            {
                var cloudBlob = blob as ICloudBlob;

                if (cloudBlob != null)
                {
                    var fileName = GetBlobLocalName(cloudBlob);

                    // Empty file name indicates a special BLOB that represents an empty directory
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        yield return new FileSystemItem(
                            GetBlobLocalName(cloudBlob),
                            cloudBlob.Properties.Length,
                            GetBlobModifiedTime(cloudBlob));
                    }
                }
                else
                {
                    var directory = blob as CloudBlobDirectory;

                    if (directory != null)
                    {
                        var parentNameLength =
                            directory.Parent != null ? directory.Parent.Prefix.Length : 0;

                        yield return new FileSystemItem(
                            directory.Prefix.Substring(
                                parentNameLength,
                                directory.Prefix.Length - parentNameLength - 1),
                            DefaultDateTime);
                    }
                    else
                    {
                        throw Errors.UnsupportedBlobType(blob.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Copies the BLOB and removes the original one.
        /// </summary>
        /// <param name="sourceBlob">Source Cloud BLOB</param>
        /// <param name="targetBlob">Target Cloud BLOB</param>
        private void RenameBlob(CloudBlob sourceBlob, CloudBlob targetBlob)
        {
            targetBlob.StartCopy(sourceBlob.Uri);

            while (targetBlob.CopyState.Status == CopyStatus.Pending)
            {
                Thread.Sleep(250);
            }

            if (targetBlob.CopyState.Status != CopyStatus.Success)
            {
                throw Errors.BlobCopyFailed(targetBlob.CopyState.Status, targetBlob.CopyState.StatusDescription);
            }

            sourceBlob.Delete();
        }

        /// <summary>
        /// Converts virtual path in to a BLOB name. 
        /// </summary>
        /// <param name="path">Virtual path in the file system</param>
        /// <param name="isDirectory">Flag that indicates that this path is pointing to the directory</param>
        /// <returns>Name of the BLOB in the Storage Account.</returns>
        private string ToBlobName(VirtualPath path, bool isDirectory)
        {
            if (!path.Segments.Any())
            {
                return null;
            }

            var relativePath =
                String.Join(BlobPathSeparator, path.Segments);

            return
                isDirectory
                    ? relativePath + BlobPathSeparator
                    : relativePath;
        }

        /// <summary>
        /// Extracts file name from the Cloud BLOB.
        /// </summary>
        /// <param name="blob">Cloud BLOB</param>
        /// <returns>Name of the file.</returns>
        private string GetBlobLocalName(ICloudBlob blob)
        {
            return blob.Name.Substring(blob.Name.LastIndexOf(BlobPathSeparator) + 1);
        }

        /// <summary>
        /// Extracts last modified timestamp from the Cloud BLOB.
        /// </summary>
        /// <param name="blob">Cloud BLOB</param>
        /// <returns>Last modified timestamp.</returns>
        private DateTime GetBlobModifiedTime(ICloudBlob blob)
        {
            return
                blob.Properties.LastModified.HasValue
                    ? blob.Properties.LastModified.Value.DateTime
                    : DefaultDateTime;
        }
    }
}
