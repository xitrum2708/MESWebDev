namespace MESWebDev.Common.UploadEngine.Services
{
    using ExcelDataReader;
    using System.Data;

    public class ExcelReaderService
    {
        public DataTable ReadExcel(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataset = reader.AsDataSet();
            return dataset.Tables[0]; // return the first DataTable 
        }
    }
}
