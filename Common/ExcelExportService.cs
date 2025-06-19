using OfficeOpenXml;
using System.Data;
using System.Reflection;

namespace MESWebDev.Common
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1")
        {
            // Đặt LicenseContext của EPPlus nếu dùng phiên bản mới
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Lấy danh sách các property của kiểu T thông qua reflection
                PropertyInfo[] properties = typeof(T).GetProperties();

                // Ghi header cho các cột (sử dụng tên thuộc tính, bạn có thể tùy chỉnh)
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Ghi dữ liệu
                int row = 2;
                foreach (var item in data)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        worksheet.Cells[row, col + 1].Value = value;
                    }
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

        public byte[] DatatableExportToExcel(DataTable data, string sheetName = "Sheet1")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Write headers
                for (int col = 0; col < data.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = data.Columns[col].ColumnName;
                }

                // Write data rows
                for (int row = 0; row < data.Rows.Count; row++)
                {
                    for (int col = 0; col < data.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = data.Rows[row][col];
                    }
                }

                return package.GetAsByteArray();
            }
        }
    }
}