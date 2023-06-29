using ClosedXML.Excel;

namespace ABS.FileGeneration
{
    // In the future, we may want to generalize the naming to something more like `ICreateFile`, but while there's only one implementation,
    // it's easiest to keep the name matching the single class.
    internal interface IWorkbookGenerator
    {
        void Create(TemporaryFilePath fileName);
    }

    internal class WorkbookGenerator : IWorkbookGenerator
    {
        public void Create(TemporaryFilePath fileName)
        {
            using (IXLWorkbook workbook = CreateInMemoryWorkbook())
            {
                var worksheet = workbook.AddWorksheet();
                worksheet.Cell("A1").Value = "Name";
                worksheet.Cell("B1").Value = "Completed Date";
                worksheet.Cell("A2").Value = "Timothy Jewell";
                worksheet.Cell("B2").Value = DateTime.Now.ToString("O");

                workbook.SaveAs(fileName.Value);
            }
        }

        internal virtual IXLWorkbook CreateInMemoryWorkbook() => new XLWorkbook();
    }
}
