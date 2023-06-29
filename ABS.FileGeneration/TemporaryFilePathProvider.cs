namespace ABS.FileGeneration
{
    /// <summary>
    /// A provider for instances of <see cref="TemporaryFilePath"/>.
    /// </summary>
    /// <remarks>
    /// While the dispose method for this class attempts to remove the created files, it would probably be better to have a
    /// deletion queue or even a cleanup process for the temp directory to handle failures/non-disposing code.
    /// </remarks>
    public class TemporaryFilePathProvider : IDisposable
    {
        private readonly string folderPath;

        private readonly LinkedList<string> createdFilePaths = new LinkedList<string>();

        public TemporaryFilePathProvider(string folder)
        {
            // it would be lovely to ensure that this value only points to a valid temp directory, but that seems out of scope for now.
            this.folderPath = !string.IsNullOrWhiteSpace(folder) ? folder : throw new ArgumentException("Expected a valid path to a folder", nameof(folder));
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
                this.DeleteFile(filePath);
            }

            this.createdFilePaths.Clear();
        }

        internal virtual void DeleteFile(string path) => File.Delete(path); // sadly, there isn't an easy way to test this via truly isolated unit tests
    }
}
