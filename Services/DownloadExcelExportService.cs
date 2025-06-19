using OfficeOpenXml;
using System.Reflection;
using System.Text;

namespace MESWebDev.Services
{
    public class DownloadExcelExportService
    {
        public DownloadExcelExportService()
        {
            // Required for EPPlus in non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] DownloadExportToExcel<T>(IEnumerable<T> data, Dictionary<string, string> columnMappings = null, string dateFormat = "yyyy/MM/dd")
        {
            if (data == null || !data.Any())
            {
                return Encoding.UTF8.GetBytes("No data available");
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var type = typeof(T);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Write headers
                var headers = columnMappings != null
                    ? columnMappings.Keys
                    : properties.Select(p => p.Name);
                for (int i = 0; i < headers.Count(); i++)
                {
                    var header = columnMappings != null && columnMappings.ContainsKey(headers.ElementAt(i))
                        ? columnMappings[headers.ElementAt(i)]
                        : headers.ElementAt(i);
                    worksheet.Cells[1, i + 1].Value = header;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Write data
                int row = 2;
                foreach (var item in data)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var prop = properties[i];
                        var value = prop.GetValue(item);

                        if (value == null)
                        {
                            worksheet.Cells[row, i + 1].Value = "";
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (value is DateTime dt)
                            {
                                worksheet.Cells[row, i + 1].Style.Numberformat.Format = dateFormat;
                                worksheet.Cells[row, i + 1].Value = dt;
                            }
                            else
                            {
                                worksheet.Cells[row, i + 1].Value = "";
                            }
                        }
                        else
                        {
                            worksheet.Cells[row, i + 1].Value = value.ToString();
                        }
                    }
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.View.FreezePanes(2, 3);

                // Return Excel file as byte array
                return package.GetAsByteArray();
            }
        }
    }
}