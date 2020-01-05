using System;
using Microsoft.Azure.Storage.Blob;

namespace ZeptoServer.Ftp.AzureStorage
{
    /// <summary>
    /// Defines all errors thrown by the Azure Storage file system.
    /// </summary>
    internal static class Errors
    {
        /// <summary>
        /// Error that is being thrown when unsupported BLOB type is encountered.
        /// </summary>
        /// <param name="blobType">Type of the BLOB</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception UnsupportedBlobType(Type blobType)
        {
            return new NotSupportedException(
                String.Format(Resources.UnsupportedBlobTypeFormat, blobType.Name));
        }

        /// <summary>
        /// Error that is being thrown when BLOB copy operation fails.
        /// </summary>
        /// <param name="status">Operation status name</param>
        /// <param name="statusDescription">Operation status description</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception BlobCopyFailed(CopyStatus status, string statusDescription)
        {
            return new BlobCopyFailedException(
                String.Format(Resources.BlobCopyFailedFormat, status, statusDescription));
        }
    }
}
