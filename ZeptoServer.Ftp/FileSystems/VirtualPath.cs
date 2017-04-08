using System;
using System.Collections.Generic;

namespace ZeptoServer.Ftp.FileSystems
{
    /// <summary>
    /// Virtual path to the item in the file system.
    /// </summary>
    public sealed class VirtualPath
    {
        /// <summary>
        /// Path separator.
        /// </summary>
        private const string PathSeparator = "/";

        /// <summary>
        /// Path segment referencing the parent directory.
        /// </summary>
        private const string ParentFolder = "..";

        /// <summary>
        /// Path separator stored in the array to use with <see cref="String.Split(string[], StringSplitOptions)"/>.
        /// </summary>
        private static readonly string[] PathSeparatorSplitter = new[] { PathSeparator };

        /// <summary>
        /// List of current path segments.
        /// </summary>
        private readonly List<string> currentPath;

        /// <summary>
        /// Gets the current path segments.
        /// </summary>
        public IEnumerable<string> Segments
        {
            get { return currentPath; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPath"/> class.
        /// </summary>
        public VirtualPath()
        {
            currentPath = new List<string>(8);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPath"/> class
        /// with the provided set of path segments.
        /// </summary>
        /// <param name="segments">Path segments</param>
        private VirtualPath(IEnumerable<string> segments)
        {
            currentPath = new List<string>(segments);
        }

        /// <summary>
        /// Changes the current virtual path to point to the new path.
        /// </summary>
        /// <param name="path">Path to navigate to</param>
        /// <returns>true if path was successfully changed, otherwise false.</returns>
        public bool Navigate(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return true;
            }

            if (path.StartsWith(PathSeparator))
            {
                currentPath.Clear();
            }

            foreach (var segment
                in path.Split(
                    PathSeparatorSplitter,
                    StringSplitOptions.RemoveEmptyEntries))
            {
                if (segment == ParentFolder)
                {
                    if (!NavigateUp())
                    {
                        return false;
                    }
                }
                else
                {
                    currentPath.Add(segment);
                }
            }

            return true;
        }

        /// <summary>
        /// Changes the current virtual path to point to the parent directory.
        /// </summary>
        /// <returns>true if path was successfully changed, otherwise false.</returns>
        public bool NavigateUp()
        {
            if (currentPath.Count == 0)
            {
                return false;
            }

            currentPath.RemoveAt(currentPath.Count - 1);
            return true;
        }

        /// <summary>
        /// Crates a copy of the current virtual path.
        /// </summary>
        /// <returns>Copy of the current virtual path</returns>
        public VirtualPath Clone()
        {
            return new VirtualPath(currentPath);
        }

        /// <summary>
        /// Generates a string representation of the current virtual path.
        /// </summary>
        /// <returns>String representation of the current virtual path.</returns>
        public override string ToString()
        {
            return PathSeparator + String.Join(PathSeparator, currentPath);
        }
    }
}
