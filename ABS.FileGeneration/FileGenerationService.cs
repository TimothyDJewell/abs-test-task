namespace ABS.FileGeneration
{
    public sealed class FileGenerationService
    {
        private TemporaryFilePathProvider fileProvider;

        public FileGenerationService(TemporaryFilePathProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public TemporaryFilePath GenerateFile()
        {
            try
            {
                var filePath = this.fileProvider.Create(".xlsx"); // ClosedXml requires the file extension when saving
                new WorkbookGenerator().Create(filePath);
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

    // keeping extra classes within the file since they're trivial for now
    public class FileGenerationException : Exception
    {
        public FileGenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    // In the future, we may want to spin off `interface ICreateFile { void Create(TemporaryFilePath fileName); }` or the like, but for now
    // keeping this as a separate class preserves an easy transition as well as the opportunity to add dependency injection
    internal class WorkbookGenerator
    {
        public void Create(TemporaryFilePath fileName)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet();
                worksheet.Cell("A1").Value = "Name";
                worksheet.Cell("B1").Value = "Completed Date";
                worksheet.Cell("A2").Value = "Timothy Jewell";
                worksheet.Cell("B2").Value = DateTime.Now.ToString("O");

                workbook.SaveAs(fileName.Value);
            }
        }
    }
}
