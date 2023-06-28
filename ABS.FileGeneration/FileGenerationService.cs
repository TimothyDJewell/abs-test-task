namespace ABS.FileGeneration
{
    public sealed class FileGenerationService : IDisposable
    {
        private readonly string folderPath;
        private readonly List<string> generatedFilePaths = new List<string>();
        public FileGenerationService(string folder)
        {
            this.folderPath = folder ?? throw new ArgumentNullException(nameof(folder));
        }

        public FileStream GenerateFile()
        {
            string filePath = Path.Combine(this.folderPath, Guid.NewGuid().ToString("N"));
            try
            {
                new WorkbookGenerator().CreateWorkbook(filePath);
                this.generatedFilePaths.Add(filePath);
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
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

        public void Dispose()
        {
            foreach (var filePath in this.generatedFilePaths)
            {
                File.Delete(filePath);
            }

            this.generatedFilePaths.Clear();
        }
    }

    // keeping extra classes within the file since they're trivial for now
    public class FileGenerationException : Exception
    {
        public FileGenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    // In the future, we may want to spin off `interface ICreateFile { void Create(string fileName); }` or the like, but for now
    // keeping this as a separate class preserves an easy transition as well as the opportunity to add dependency injection
    internal class WorkbookGenerator
    {
        public void CreateWorkbook(string fileName)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet();
                worksheet.Cell("A1").Value = "Name";
                worksheet.Cell("B1").Value = "Completed Date";
                worksheet.Cell("A2").Value = "Timothy Jewell";
                worksheet.Cell("B2").Value = DateTime.Now.ToString("O");

                workbook.SaveAs(fileName);
            }
        }
    }
}
