using System;
using System.Collections.Generic;
using System.Text;

namespace ABS.FileGeneration
{
    /// <summary>
    /// A wrapper class for a file path to a temporary/short-lived file.
    /// </summary>
    /// <remarks>
    /// This could implement an interface to provide common abstraction if the need should arise for file access to non-temporary files.
    /// </remarks>
    public class TemporaryFilePath
    {
        public string Value { get; init; }

        internal TemporaryFilePath(string filePath)
        {
            this.Value = filePath;
        }
    }

    /// <summary>
    /// A provider for instances of <see cref="TemporaryFilePath"/>.
    /// </summary>
    /// <remarks>
    /// While the dispose method for this class attempts to remove the created files,
    /// it is probably still worthwhile to have a cleanup process for the temp directory to handle failures/non-disposing code.
    /// </remarks>
    public class TemporaryFilePathProvider : IDisposable
    {
        private readonly string folderPath;

        private readonly LinkedList<string> createdFilePaths = new LinkedList<string>();

        public TemporaryFilePathProvider(string folder)
        {
            // it would be lovely to ensure that this value only points to a valid temp directory, but that seems out of scope for now.
            this.folderPath = folder ?? throw new ArgumentNullException(nameof(folder));
        }

        public TemporaryFilePath Create(string? fileExtension = null)
        {
            string filePath = Path.Combine(this.folderPath, Guid.NewGuid().ToString("N") + fileExtension);
            if (fileExtension != null && Path.GetExtension(filePath) != fileExtension)
            {
                throw new ArgumentException($"Invalid extension '{fileExtension}' for file.", nameof(fileExtension));
            }

            createdFilePaths.AddLast(filePath);
            return new TemporaryFilePath(filePath);
        }

        public void Dispose()
        {
            foreach (var filePath in this.createdFilePaths)
            {
                File.Delete(filePath);
            }

            this.createdFilePaths.Clear();
        }
    }
}
