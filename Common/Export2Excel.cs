using Azure.Core;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using MESWebDev.Models.COMMON;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;

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
                        if (int.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var num))
                        {
                            worksheet.Cells[r + 2, c + 1].Value = num;
                            worksheet.Cells[r + 2, c + 1].Style.Numberformat.Format = "0"; // 2 decimals
                        }
                        else if(decimal.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var decValue))
                        {
                            worksheet.Cells[r + 2, c + 1].Value = decValue;
                            worksheet.Cells[r + 2, c + 1].Style.Numberformat.Format = numberFormat; // 4 decimals
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



        public async Task<byte[]> DownloadList<T>(IEnumerable<T> data, string sheetName = "Data")
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                PropertyInfo[] properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }
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
    }


    public class Export2SpecificExcel
    {
        private readonly IWebHostEnvironment _env;
        public Export2SpecificExcel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<FileContentResult> ExportTimeStudy(DataSet ds, List<UploadFileMaster> f)
        {  
            var filePath = System.IO.Path.Combine(_env.WebRootPath, "ReportTemplate","PE", "PETimeStudyReport.xlsx");
            using (var app = new ExcelPackage(new FileInfo(filePath)))
            {
                // Create excel sheet
                var sheet = app.Workbook.Worksheets[0];

                if(ds != null && ds.Tables.Count > 1)
                {
                    string Model = f.FirstOrDefault(x => x.db_col_name == "Model")?.col_type ?? "B2";
                    string ModelCat = f.FirstOrDefault(x => x.db_col_name == "ModelCat")?.col_type ?? "B3";
                    string Opts = f.FirstOrDefault(x => x.db_col_name == "Opts")?.col_type ?? "B4";
                    string LineBalance = f.FirstOrDefault(x => x.db_col_name == "LineBalance")?.col_type ?? "E2";
                    string PitchTime = f.FirstOrDefault(x => x.db_col_name == "PitchTime")?.col_type ?? "E3";
                    string BottleNeckProcess = f.FirstOrDefault(x => x.db_col_name == "BottleNeckProcess")?.col_type ?? "E4";
                    string TotalTime = f.FirstOrDefault(x => x.db_col_name == "TotalTime")?.col_type ?? "H2";
                    string TargetOutput = f.FirstOrDefault(x => x.db_col_name == "TargetOutput")?.col_type ?? "H3";
                    string FixTarget = f.FirstOrDefault(x => x.db_col_name == "FixTarget")?.col_type ?? "H4";
                    string Section = f.FirstOrDefault(x => x.db_col_name == "Section")?.col_type ?? "N2";
                    string IssueDate = f.FirstOrDefault(x => x.db_col_name == "IssueDate")?.col_type ?? "N3";
                    string IssueBy = f.FirstOrDefault(x => x.db_col_name == "IssueBy")?.col_type ?? "N4";
                    string Body = f.FirstOrDefault(x => x.db_col_name == "Body")?.col_type ?? "A8";

                    DataTable header = ds.Tables[0];
                    sheet.Cells[Model].Value = header.Rows[0]["Model"]?.ToString() ?? string.Empty;
                    sheet.Cells[ModelCat].Value = header.Rows[0]["ModelCat"]?.ToString() ?? string.Empty;
                    sheet.Cells[Opts].Value = Convert.ToInt32( header.Rows[0]["OperatorQty"] ?? 0);
                    sheet.Cells[LineBalance].Value = Math.Round(Convert.ToDecimal( header.Rows[0]["LineBalance"] ?? 0),2);
                    sheet.Cells[PitchTime].Value = Math.Round(Convert.ToDecimal(header.Rows[0]["PitchTime"] ?? 0),2);
                    sheet.Cells[BottleNeckProcess].Value = Math.Round(Convert.ToDecimal(header.Rows[0]["BottleNeckProcess"] ?? 0),2);
                    sheet.Cells[TotalTime].Value = Math.Round(Convert.ToDecimal(header.Rows[0]["TimeTotal"] ?? 0),2);
                    sheet.Cells[TargetOutput].Value = Math.Round(Convert.ToDecimal(header.Rows[0]["OutputTarget"] ?? 0),0);
                    sheet.Cells[FixTarget].Value = Math.Round(Convert.ToDecimal(header.Rows[0]["FixTarget"] ?? 0),0);

                    sheet.Cells[Section].Value = header.Rows[0]["Section"]?.ToString() ?? string.Empty;
                    sheet.Cells[IssueDate].Value = header.Rows[0]["CreatedDt"]?.ToString() ?? string.Empty;
                    sheet.Cells[IssueBy].Value = header.Rows[0]["CreatedBy"]?.ToString() ?? string.Empty;

                    DataTable body = ds.Tables[1];
                    sheet.Cells[Body].LoadFromDataTable(body, false);
                    // format excel

                    int start_row = 8;
                    try { start_row = Convert.ToInt32(Body.Substring(1)); }
                    catch { }

                    var groups = body.AsEnumerable()
                        .Select((x, idx) => new
                        {
                            RowIndex = idx + start_row,
                            OperationKind = x.Field<string>("OperationKind"),
                            StepContent = x.Field<string?>("StepContent"),
                            Sumary = x["Sumary"] == DBNull.Value ? 0m : Convert.ToDecimal(x["Sumary"]),
                            UnitQty = x["UnitQty"] == DBNull.Value ? 0 : Convert.ToInt32(x["UnitQty"]),
                            TargetQty = x["TargetQty"] == DBNull.Value ? 0m : Convert.ToDecimal(x["TargetQty"]),
                            NoOperator = x["NoOperator"] == DBNull.Value ? 0m : Convert.ToDecimal(x["NoOperator"]),
                            AllocatedOpr = x["AllocatedOpr"] == DBNull.Value ? 0m : Convert.ToDecimal(x["AllocatedOpr"]),
                            ProcessTime = x["ProcessTime"] == DBNull.Value ? 0m : Convert.ToDecimal(x["ProcessTime"]),
                        })
                        .GroupBy(x => new
                        {
                            x.OperationKind
                            ,x.StepContent
                            ,x.Sumary
                            ,x.UnitQty
                            ,x.TargetQty
                            ,x.NoOperator
                            ,x.AllocatedOpr
                            ,x.ProcessTime
                        });
                    //merge cell
                    foreach (var group in groups)
                    {
                        if (group.Count() > 1)
                        {
                            int from = group.Min(i => i.RowIndex);
                            var to = group.Max(i => i.RowIndex);
                            if (to > from)
                            {
                                for(int j = 1; j <= body.Columns.Count; j++)
                                {
                                    if(j < 3 || j > 9)
                                    {
                                        sheet.Cells[from, j, to, j].Merge = true;
                                        sheet.Cells[from, j, to, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        sheet.Cells[from, j, to, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                }
                            }
                        }
                    }

                    // style all body
                    sheet.Cells[start_row, 1, start_row + body.Rows.Count - 1, body.Columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[start_row, 1, start_row + body.Rows.Count - 1, body.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[start_row, 1, start_row + body.Rows.Count - 1, body.Columns.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[start_row, 1, start_row + body.Rows.Count - 1, body.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                // convert excel data to array
                var array = app.GetAsByteArray();
                // return excel file $"Programs_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx"
                return new FileContentResult(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"TimeStudy_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                };
            }
        }

        //public FileContentResult DownloadPayrollResign(DataSet ds, PayrollExport pe)
        //{
        //    string templatePath = Path.Combine(_env.WebRootPath, "TemplateData", "PayrollSheetResign1_Template.xlsx");
        //    FileInfo templateFile = new FileInfo(templatePath);
        //    List<ExcelFormatModel> f = pe?.fields ?? new();
        //    string header_cell = f.FirstOrDefault(x => x.name == "header_cell")?.position ?? "A2";
        //    string salary_in_cell = f.FirstOrDefault(x => x.name == "salary_in_cell")?.position ?? "A9";
        //    string period_cell = f.FirstOrDefault(x => x.name == "period_cell")?.position ?? "B10";
        //    string day_total_cell = f.FirstOrDefault(x => x.name == "day_total_cell")?.position ?? "B11";
        //    string table1_cell = f.FirstOrDefault(x => x.name == "table1_cell")?.position ?? "CG8";
        //    string table2_cell = f.FirstOrDefault(x => x.name == "table2_cell")?.position ?? "CG8";
        //    int start_row = Convert.ToInt32(f.FirstOrDefault(x => x.name == "start_row")?.position ?? "6");
        //    int start_col = Convert.ToInt32(f.FirstOrDefault(x => x.name == "start_col")?.position ?? "1");
        //    int del_col_total = Convert.ToInt32(f.FirstOrDefault(x => x.name == "del_col_total")?.position ?? "5");

        //    string end_col_name = f.FirstOrDefault(x => x.name == "end_col_name")?.position ?? "remarks";
        //    string total_col_name = f.FirstOrDefault(x => x.name == "total_col_name")?.position ?? "user_id";
        //    string merge_grand_name = f.FirstOrDefault(x => x.name == "merge_grand_name")?.position ?? "basic_salary";
        //    string merge_sub_name = f.FirstOrDefault(x => x.name == "merge_sub_name")?.position ?? "dept_name";


        //    using (var app = new ExcelPackage(templateFile))
        //    {
        //        // Create excel sheet
        //        var sheet = app.Workbook.Worksheets[0];
        //        if (ds != null && ds.Tables.Count > 0)
        //        {
        //            DataTable dt = ds.Tables[0];
        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                sheet.Cells[period_cell].Value = pe.period; // clear all cells
        //                sheet.Cells[header_cell].Value = pe.header.Replace("<hr>", Environment.NewLine); // clear all cells
        //                sheet.Cells[header_cell].Style.WrapText = true;
        //                sheet.Cells[salary_in_cell].Value = pe.salary_in; // clear all cells
        //                sheet.Cells[day_total_cell].Value = pe.total_day; // clear all cells
        //                sheet.Cells[table1_cell].LoadFromDataTable(dt, false);// false to not get header from datatable
        //                //sheet.Cells[pe.table1_cell].LoadFromDataTable(dt, false);// false to not get header from datatable
        //                if (ds.Tables.Count > 1)
        //                {
        //                    DataTable dt2 = ds.Tables[1];
        //                    if (dt2 != null && dt2.Rows.Count > 0)
        //                    {
        //                        if (dt2.Columns.Count > del_col_total)
        //                        {
        //                            for (int i = 1; i <= del_col_total; i++)
        //                            {
        //                                dt2.Columns.RemoveAt(dt2.Columns.Count - 1);
        //                            }
        //                            dt2.AcceptChanges();
        //                        }
        //                        foreach (DataRow row in dt2.Rows)
        //                        {
        //                            foreach (DataColumn col in dt2.Columns)
        //                            {
        //                                if (row[col.ColumnName] == DBNull.Value)
        //                                {
        //                                    row[col.ColumnName] = 0;
        //                                }
        //                            }
        //                        }
        //                        sheet.Cells[table2_cell].LoadFromDataTable(dt2, true);
        //                    }
        //                }
        //            }
        //            // format excel
        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                //format entire cell first
        //                ExcelFormat.ApplyAutofit(sheet);
        //                // merge cell sub toal and grand total
        //                int row_index = start_row;
        //                int total_row = row_index + dt.Rows.Count - 1; // last row index
        //                int total_col = (dt.Columns[end_col_name]?.Ordinal ?? 0) + 1; // last row index

        //                // dotted cells and thin round
        //                var range = sheet.Cells[start_row, start_col, total_row, total_col];
        //                ExcelFormat.ApplyDottedCellsThenThin(range);

        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    string cell_value = row.Field<string>(total_col_name) ?? string.Empty;
        //                    if (cell_value.ToLower().Contains("total"))
        //                    {
        //                        range = sheet.Cells[row_index, 1, row_index, total_col];
        //                        ExcelFormat.TotalRowFormat(range);
        //                        sheet.Cells[row_index, 1].Value = cell_value; // set total row value
        //                        range = sheet.Cells[row_index, 1, row_index, dt.Columns[cell_value.ToLower().Contains("sub") ? merge_sub_name : merge_grand_name]?.Ordinal ?? 3];
        //                        ExcelFormat.MergeCell(range); // merge cell for total row
        //                    }
        //                    row_index++;
        //                }
        //            }
        //        }
        //        // convert excel data to array
        //        var array = app.GetAsByteArray();
        //        // return excel file $"Programs_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx"

        //        return new FileContentResult(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {
        //            FileDownloadName = $"{pe.file_name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
        //        };
        //    }
        //}
    }
}


    