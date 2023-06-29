namespace ABS.FileGeneration
{
    public sealed class FileGenerationService
    {
        private readonly TemporaryFilePathProvider fileProvider;

        private readonly IWorkbookGenerator workbookGenerator;

        public FileGenerationService(TemporaryFilePathProvider fileProvider)
            : this(fileProvider, new WorkbookGenerator())
        {
        }

        internal FileGenerationService(TemporaryFilePathProvider fileProvider, IWorkbookGenerator workbookGenerator)
        {
            this.fileProvider = fileProvider;
            this.workbookGenerator = workbookGenerator;
        }

        public TemporaryFilePath GenerateFile()
        {
            try
            {
                var filePath = this.fileProvider.Create(".xlsx"); // ClosedXml requires the file extension when saving
                this.workbookGenerator.Create(filePath);
                return filePath;
            }
            catch (SystemException ex) when
                (ex is FileNotFoundException
                || ex is IOException
                || ex is UnauthorizedAccessException
                || ex is System.Security.SecurityException)
            {
                throw new FileGenerationException("Unable to generate workbook", ex);
            }
        }
    }
}
