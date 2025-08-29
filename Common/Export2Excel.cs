using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;
using System.Globalization;

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


        public FileContentResult DownloadProdPlan(DataSet ds, string fileName)
        {
            using (var app = new ExcelPackage())
            {
                // Create excel sheet
                if (ds != null && ds.Tables.Count>2)
                {
                    DataTable holiday = ds.Tables[0];
                    ds.Tables.RemoveAt(0);
                    foreach (DataTable dt in ds.Tables)
                    {
                        var sheet = app.Workbook.Worksheets.Add(dt.TableName);

                        // set font
                        sheet.Cells.Style.Font.Name = "Arial";

                        // set size 
                        sheet.Cells.Style.Font.Size = 11;

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int start_row = 2;
                            int start_col = 2;
                            int total_row = dt.Rows.Count + 2;
                            int total_col = dt.Columns.Count + 1;
                            // Load data starting from B2
                            sheet.Cells[start_col,start_row].LoadFromDataTable(dt, true);

                            // Border
                            var dataRange = sheet.Cells[start_row, start_col, total_row, total_col];
                            var border = dataRange.Style.Border;
                            border.Left.Style = border.Right.Style = border.Top.Style = border.Bottom.Style = ExcelBorderStyle.Thin;

                            // Header
                            var headerRange = sheet.Cells[ start_row, start_col, start_row, total_col];
                            headerRange.Style.Font.Bold = true;
                            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                            // Holiday
                            if(holiday!= null && holiday.Rows.Count > 0)
                            {
                                List<string> hld = holiday.AsEnumerable().Select(i => i.Field<string>("date")).ToList();
                                foreach (var cell in headerRange)
                                {
                                    if (cell.Value!=null && hld.Contains(cell.Value.ToString() ?? "xx"))
                                    {
                                        var colRange = sheet.Cells[start_row, cell.Start.Column, total_row, cell.Start.Column];
                                        colRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        colRange.Style.Fill.BackgroundColor.SetColor(Color.MistyRose);
                                    }
                                }
                            }

                            // Replace all value = 0 to string.empty;
                            var range = sheet.Cells[sheet.Dimension.Address];
                            foreach(var cell in range)
                            {
                                if(cell.Value != null && cell.Value.ToString() == "0")
                                {
                                    cell.Value = string.Empty;
                                }
                            }
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


        //public void FormatExcel()

        // Export to excel from Ajax Jquery
        public class TableFilterRequest
        {
            public List<string> Headers { get; set; }
            public List<List<string>> Rows { get; set; }
        }

        public async Task<byte[]> AjaxExcelExport(TableFilterRequest request, string numberFormat)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("FilteredData");

                // Header
                for (int c = 0; c < request.Headers.Count; c++)
                {
                    worksheet.Cells[1, c + 1].Value = request.Headers[c];
                }

                //@decValue.ToString("#,##0.0000", System.Globalization.CultureInfo.InvariantCulture)
                //Data 
                for (int r = 0; r < request.Rows.Count; r++)
                {
                    for (int c = 0; c < request.Rows[r].Count; c++)
                    {
                        string value = request.Rows[r][c].ToString().Replace(",", "");
                        // double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var num)
                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var num))
                        {
                            worksheet.Cells[r + 2, c + 1].Value = num;
                            worksheet.Cells[r + 2, c + 1].Style.Numberformat.Format = numberFormat; // 2 decimals
                        }
                        else
                        {
                            worksheet.Cells[r + 2, c + 1].Value = request.Rows[r][c]?.ToString();
                        }
                    }
                }
                //var stream = new MemoryStream();
                //package.SaveAs(stream);
                //stream.Position = 0;
                return package.GetAsByteArray();
            }   
        }
    }
}

    