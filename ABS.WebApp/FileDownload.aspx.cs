using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ABS.FileGeneration;

namespace ABS.WebApp
{
    public partial class FileDownload : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fileProvider = new TemporaryFilePathProvider(@"C:\Temp"))
                {
                    var fileGenService = new FileGenerationService(fileProvider);
                    TemporaryFilePath filePath = fileGenService.GenerateFile();
                    this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    this.Response.AddHeader("Content-Disposition", $"attachment; filename=\"ArbitraryData.xlsx\"");
                    this.Response.TransmitFile(filePath.Value);
                    this.CompleteRequest();
                }
            }
            catch (FileGenerationException ex)
            {
                Console.WriteLine(ex.ToString()); // This is the simplest logging to add for now.
                this.Response.Write($@"<!doctype html>
<html lang=""en"">
    <title>Error Downloading File</title>
    <pre>Unable to download file due to exception: {System.Net.WebUtility.HtmlEncode(ex.ToString())}</pre>
</html>");
                this.CompleteRequest();
            }
        }

        /// <summary>
        /// Send data to the client and prevent further processing.
        /// </summary>
        private void CompleteRequest()
        {
            this.Response.Flush();
            this.Response.SuppressContent = true; // after flushing , prevent normal response from interfering
            this.Context.ApplicationInstance.CompleteRequest();

        }
    }
}