using DocumentFormat.OpenXml.Wordprocessing;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.OQC.VM;
using MESWebDev.Models.SMT;
using MESWebDev.Models.SMT.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;

namespace MESWebDev.Controllers
{
    public class AOIErrorController : BaseController    
    {
        readonly ISMTService _smtService;
        private readonly Export2Excel _ee;
        public AOIErrorController(AppDbContext context, ISMTService smtService) : base(context)
        {
            _smtService = smtService;
            _ee = new();
        }

        public IActionResult AOIMatrixHistory(string? SearchTerm, int page = 1, int pageSize = 10)
        {
            var query = _context.UVSMT_MODEL_MATRIX_MASTERs.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(d =>
                    d.Model.Contains(SearchTerm) ||
                    d.PCB_No.Contains(SearchTerm) ||
                    d.Program_Name.Contains(SearchTerm) ||
                    d.Key_Work.Contains(SearchTerm)||
                    d.PCB_TYPE.Contains(SearchTerm) ||
                    d.LotNo_1.Contains(SearchTerm) ||
                    d.LotNo_2.Contains(SearchTerm) 
                    );                
            }
            var resultPage = query
                .OrderByDescending(x => x.CreatedDate)                
                .ToPagedResult(page, pageSize);
            var viewModel = new SMT_ViewModel
            {   
                SearchTerm = SearchTerm,
                matrixMaster = resultPage
            };
            ViewBag.SearchText = SearchTerm ?? "";

            if (resultPage == null || !resultPage.Items.Any())
            {
                ViewBag.Message = "NotFoundData";
            }
            return View(viewModel);
        }
        private int? TryParseInt(string val) => int.TryParse(val, out var i) ? i : null;
        private float? TryParseFloat(string val) => float.TryParse(val, out var f) ? f : null;
        private int ToInt(object val, int defaultValue = 0)
        {
            if (val == null || val == DBNull.Value) return defaultValue;
            if (int.TryParse(val.ToString(), out var i)) return i;
            if (double.TryParse(val.ToString(), out var d)) return (int)Math.Round(d);
            return defaultValue;
        }

        private  float ToFloat(object val, float defaultValue = 0f)
        {
            if (val == null || val == DBNull.Value) return defaultValue;
            if (float.TryParse(val.ToString(), out var f)) return f;
            if (double.TryParse(val.ToString(), out var d)) return (float)d;
            return defaultValue;
        }

        private static string ToStr(object val) => val?.ToString()?.Trim() ?? "";
        [HttpPost]
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // skip header
                {
                    var boardPcsPerSheet = TryParseInt(worksheet.Cells[row, 5].Text);
                    var pcbPerModel = TryParseInt(worksheet.Cells[row, 6].Text);
                    var chipsPerModel = TryParseInt(worksheet.Cells[row, 20].Text);
                    var cph = TryParseFloat(worksheet.Cells[row, 21].Text);

                    //var data = new UVSMT_MODEL_MATRIX_MASTER
                    //{
                    //    Model = worksheet.Cells[row, 1].Text,
                    //    PCB_No = worksheet.Cells[row, 2].Text,
                    //    PCB_SIDE = worksheet.Cells[row, 3].Text,
                    //    PCB_TYPE = worksheet.Cells[row, 4].Text,
                    //    Board_Pcs_Per_Sheet = boardPcsPerSheet ?? 0, // Default to 0 if null
                    //    PCB_Per_Model = pcbPerModel ?? 0, // Default to 0 if null
                    //    RoHS = worksheet.Cells[row, 7].Text,
                    //    RV_TYPE_1 = worksheet.Cells[row, 8].Text,
                    //    LotNo_1 = worksheet.Cells[row, 9].Text,
                    //    RV_TYPE_2 = worksheet.Cells[row, 10].Text,
                    //    LotNo_2 = worksheet.Cells[row, 11].Text,                        
                    //    Load_IC_OR_Jig_Check = worksheet.Cells[row, 12].Text,
                    //    Type = worksheet.Cells[row, 13].Text,
                    //    Program_Name = worksheet.Cells[row, 14].Text,
                    //    ADD_Info = worksheet.Cells[row, 15].Text,
                    //    Reel_Of_Part_Qty = TryParseInt(worksheet.Cells[row, 16].Text) ?? 0, // Default to 0 if null
                    //    Chips_Per_PCS = TryParseInt(worksheet.Cells[row, 17].Text) ?? 0, // Default to 0 if null
                    //    Chips_Per_Board = TryParseInt(worksheet.Cells[row, 18].Text) ?? 0, // Default to 0 if null
                    //    Chips_Per_Model = TryParseInt(worksheet.Cells[row, 19].Text) ?? 0, // Default to 0 if null
                    //    CPH = TryParseFloat(worksheet.Cells[row, 20].Text) ?? 0.0f, // Default to 0.0f if null
                    //    GXH1_SIM_Time_Seconds = TryParseFloat(worksheet.Cells[row, 21].Text) ?? 0.0f, // Default to 0.0f if null
                    //    GXH3_SIM_Time_Seconds = $"{worksheet.Cells[row, 1].Text}{worksheet.Cells[row, 4].Text}-{worksheet.Cells[row, 3].Text}",
                    //    TACT_Time_Seconds = TryParseFloat(worksheet.Cells[row, 23].Text) ?? 0.0f, // Default to 0.0f if null
                    //    SIM_OUT_PCS_Per_Hour = TryParseInt(worksheet.Cells[row, 24].Text) ?? 0, // Default to 0 if null
                    //    Output_1h = TryParseInt(worksheet.Cells[row, 25].Text) ?? 0, // Default to 0 if null
                    //    Output_2h = TryParseInt(worksheet.Cells[row, 26].Text) ?? 0, // Default to 0 if null
                    //    Output_Day = TryParseInt(worksheet.Cells[row, 27].Text) ?? 0, // Default to 0 if null
                    //    Output_Night = TryParseInt(worksheet.Cells[row, 28].Text) ?? 0, // Default to 0 if null
                    //    X_mm = TryParseFloat(worksheet.Cells[row, 29].Text),
                    //    Y_mm = TryParseFloat(worksheet.Cells[row, 30].Text),
                    //    T_mm = TryParseFloat(worksheet.Cells[row, 31].Text),
                    //    Mark_LotNo = worksheet.Cells[row, 32].Text,
                    //    Paste_mg = TryParseInt(worksheet.Cells[row, 33].Text) ?? 0, // Default to 0 if null
                    //    DIP_mg = TryParseInt(worksheet.Cells[row, 34].Text) ?? 0, // Default to 0 if null
                    //    Finish_Status = worksheet.Cells[row, 35].Text,
                    //    Remark = worksheet.Cells[row, 36].Text,
                    //    Solder_Type = worksheet.Cells[row, 37].Text,
                    //    Key_Work = worksheet.Cells[row, 38].Text,
                    //    CreatedBy = User.Identity?.Name ?? "system",
                    //};
                    var data = new UVSMT_MODEL_MATRIX_MASTER
                    {
                        Model = ToStr(worksheet.Cells[row, 1].Value),
                        PCB_No = ToStr(worksheet.Cells[row, 2].Value),
                        PCB_SIDE = ToStr(worksheet.Cells[row, 3].Value),
                        PCB_TYPE = ToStr(worksheet.Cells[row, 4].Value),
                        Board_Pcs_Per_Sheet = boardPcsPerSheet ?? 0,
                        PCB_Per_Model = pcbPerModel ?? 0,
                        RoHS = ToStr(worksheet.Cells[row, 7].Value),
                        RV_TYPE_1 = ToStr(worksheet.Cells[row, 8].Value),
                        LotNo_1 = ToStr(worksheet.Cells[row, 9].Value),
                        RV_TYPE_2 = ToStr(worksheet.Cells[row, 10].Value),
                        LotNo_2 = ToStr(worksheet.Cells[row, 11].Value),
                        Load_IC_OR_Jig_Check = ToStr(worksheet.Cells[row, 12].Value),
                        Type = ToStr(worksheet.Cells[row, 13].Value),
                        Program_Name = ToStr(worksheet.Cells[row, 14].Value),
                        ADD_Info = ToStr(worksheet.Cells[row, 15].Value),

                        Reel_Of_Part_Qty = ToInt(worksheet.Cells[row, 16].Value),
                        Chips_Per_PCS = ToInt(worksheet.Cells[row, 17].Value),
                        Chips_Per_Board = ToInt(worksheet.Cells[row, 18].Value),
                        Chips_Per_Model = ToInt(worksheet.Cells[row, 19].Value),

                        CPH = ToFloat(worksheet.Cells[row, 20].Value),
                        GXH1_SIM_Time_Seconds = ToFloat(worksheet.Cells[row, 21].Value),

                        // Ghép chuỗi theo yêu cầu
                        GXH3_SIM_Time_Seconds = $"{ToStr(worksheet.Cells[row, 1].Value)}{ToStr(worksheet.Cells[row, 4].Value)}-{ToStr(worksheet.Cells[row, 3].Value)}",

                        TACT_Time_Seconds = ToFloat(worksheet.Cells[row, 23].Value),
                        SIM_OUT_PCS_Per_Hour = ToInt(worksheet.Cells[row, 24].Value),
                        Output_1h = ToInt(worksheet.Cells[row, 25].Value),
                        Output_2h = ToInt(worksheet.Cells[row, 26].Value),
                        Output_Day = ToInt(worksheet.Cells[row, 27].Value),
                        Output_Night = ToInt(worksheet.Cells[row, 28].Value),

                        X_mm = ToFloat(worksheet.Cells[row, 29].Value),
                        Y_mm = ToFloat(worksheet.Cells[row, 30].Value),
                        T_mm = ToFloat(worksheet.Cells[row, 31].Value),

                        Mark_LotNo = ToStr(worksheet.Cells[row, 32].Value),
                        Paste_mg = ToInt(worksheet.Cells[row, 33].Value),
                        DIP_mg = ToInt(worksheet.Cells[row, 34].Value),
                        Finish_Status = ToStr(worksheet.Cells[row, 35].Value),
                        Remark = ToStr(worksheet.Cells[row, 36].Value),
                        Solder_Type = ToStr(worksheet.Cells[row, 37].Value),
                        Key_Work = ToStr(worksheet.Cells[row, 38].Value),

                        CreatedBy = User.Identity?.Name ?? "system"
                    };
                    _context.UVSMT_MODEL_MATRIX_MASTERs.Add(data);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AOIMatrixHistory");
        }


        // AOIMachineSpectionData
        [HttpGet]
        public async Task<IActionResult> AOIMachineSpectionData()
        {
            DateTime startDate = DateTime.Now.AddDays(-2).Date;
            DateTime endDate = DateTime.Now.Date;

            SMT_ViewModel viewModel = await GetMachineSpectionData(startDate, endDate);

            return View("MachineSpectionData/Index",viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> AOIMachineSpectionData(SMT_ViewModel viewModel)
        {
            viewModel = await GetMachineSpectionData(Convert.ToDateTime(viewModel.StartDate), Convert.ToDateTime(viewModel.EndDate));

            return View("MachineSpectionData/Index", viewModel);
        }
        //[HttpPost]
        public async Task<IActionResult> AOIMachineSpectionData_Download(string dateRange)
        {
            string[] dates = dateRange.Split('*');
            DataTable dt = await _smtService.GetMachineSpectionData_Download(new Dictionary<string, object>
            {
                { "@start_dt", Convert.ToDateTime(dates[0]) },
                { "@end_dt", Convert.ToDateTime(dates[1]) }
            });
            return _ee.DownloadData(dt, "MachineSpectionData");
        }


        public async Task<SMT_ViewModel> GetMachineSpectionData(DateTime startDate, DateTime endEnd)
        {

            Dictionary<string, object> data = new()
            {
                { "@start_dt", startDate },
                { "@end_dt", endEnd }
            };

            DataTable dt = await _smtService.GetMachineSpectionData(data);

            SMT_ViewModel viewModel = new SMT_ViewModel
            {
                StartDate = startDate,
                EndDate = endEnd,
                MachineSpectionData = dt
            };

            return viewModel;
        }
    }
}
