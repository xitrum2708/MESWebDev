using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;

namespace MESWebDev.Common
{
    public class Export2Excel
    {
        public FileContentResult DownloadData(DataTable dt, string fileName)
        {
            using (var app = new ExcelPackage())
            {
                // Create excel sheet
                var sheet = app.Workbook.Worksheets.Add(fileName);

                // set font
                sheet.Cells.Style.Font.Name = "Courier New";

                // set size 
                sheet.Cells.Style.Font.Size = 12;

                if (dt != null && dt.Rows.Count > 0)
                {
                    sheet.Cells["A1"].LoadFromDataTable(dt, true);
                }

                // autofit column
                if (dt != null && dt.Rows.Count > 0)
                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                // convert excel data to array
                var array = app.GetAsByteArray();
                // return excel file $"Programs_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx"

                return new FileContentResult(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                };
            }
        }
        public FileContentResult DownloadDataSet(DataSet ds, string fileName)
        {
            using (var app = new ExcelPackage())
            {
                // Create excel sheet
                if (ds != null)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        var sheet = app.Workbook.Worksheets.Add(dt.TableName);

                        // set font
                        sheet.Cells.Style.Font.Name = "Arial";

                        // set size 
                        sheet.Cells.Style.Font.Size = 11;

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            sheet.Cells["A1"].LoadFromDataTable(dt, true);
                        }

                        // autofit column
                        if (dt.Rows.Count > 0)
                            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                    }
                }

                // convert excel data to array
                var array = app.GetAsByteArray();
                // return excel file $"Programs_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx"

                return new FileContentResult(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                };
            }
        }
    }
}

    