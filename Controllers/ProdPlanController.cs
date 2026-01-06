using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.Setting;
using MESWebDev.Repositories;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using static MESWebDev.Common.Export2Excel;

namespace MESWebDev.Controllers
{
    public class ProdPlanController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IProdPlanService _ppService;
        private readonly ILoggingService _loggingService;
        private readonly ISettingService _settingService;
        private readonly Export2Excel _ee;

        public ProdPlanController(AppDbContext context, ITranslationService translationService, IProdPlanService repository,
            ILoggingService loggingService, ISettingService settingService)
            : base(context)
        {
            _translationService = translationService;
            _ppService = repository;
            _loggingService = loggingService;
            _ee = new();
            _settingService = settingService;
        }

        #region Common Setting
        // List
        [HttpGet]
        public async Task<IActionResult> CommonSetting()
        {            
            ProdPlanViewModel ppv = await SettingData();
            return View("Setting/Index",ppv);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> SettingAdd()
        {
            ProjectSettingModel ppv = new();
            return PartialView("Setting/__Add", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> SettingAdd(ProjectSettingModel psm)
        {
            string msg = await _settingService.ProjectSettingAdd(psm);


            ProdPlanViewModel ppv = await SettingData();

            if (string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }

            return PartialView("Setting/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> SettingEdit(int id)
        {
            ProjectSettingModel pms = await _settingService.ProjectSettingDetail(id);
            return PartialView("Setting/__Edit", pms);
        }

        [HttpPost]
        public async Task<IActionResult> SettingEdit(ProjectSettingModel psm)
        {
            string msg = await _settingService.ProjectSettingEdit(psm);
            ProdPlanViewModel ppv = await SettingData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("Setting/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> SettingDelete(int id)
        {
            string msg = await _settingService.ProjectSettingDelete(id);
            ProdPlanViewModel ppv = await SettingData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("Setting/_Result", ppv);
        }

        public async Task<ProdPlanViewModel> SettingData(Dictionary<string, object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();
            DataTable data = await _settingService.ProjectSettingList(dic ?? new Dictionary<string, object>());
            ppv.Data = data;
            return ppv;
        }

        #endregion

        #region PC Production Plan
        // GET: Permission/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ProdPlanViewModel ppv = await _ppService.ViewProdPlan(new RequestDTO());
            return View(ppv);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file1, IFormFile file2)
        {
            ProdPlanViewModel ppv = new();
            if (file1 == null || file2 == null)
            {
                ViewBag.Error = "Please upload all three files.";
                return View("Index");
            }
            try
            {
                // Process the uploaded files
                RequestDTO request = new();
                List<IFormFile> files = new() { file1, file2 };
                request.Files = files;
                ppv = await _ppService.GetDataFromUploadFile(request);
                ViewBag.Success = "Files uploaded and processed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while processing the files: {ex.Message}";
            }
            ppv.start_sch_dt = DateTime.Now;
            return View("Index", ppv);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProdPlan([FromBody] ProdPlanViewModel ppv)
        {
            // Process and save updated events
            //ppv.start_sch_dt = DateTime.Now;
            string msg = string.Empty;
            ppv = await _ppService.ReloadProdPlan(ppv);
            msg = await _ppService.SaveProdPlan(ppv);
            return Json(new
            {
                events = ppv.events,
                holidays = ppv.holidays,
                start_sch_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                resources = ppv.resources,
                message = msg// if needed // Add more properties as needed
            });
            //return View("Index", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> ReloadProdPlan([FromBody] ProdPlanViewModel ppv)
        {
            //Process and save updated events
            //ppv.start_sch_dt = DateTime.Now;
            ppv = await _ppService.ReloadProdPlan(ppv);
            return Json(new
            {
                events = ppv.events,
                holidays = ppv.holidays,
                start_sch_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                resources = ppv.resources,
                message = "Reload data successfully !"// If needed // Add more properties as needed
            });
            //return View("Index", ppv);
        }

        //[HttpPost]
        public async Task<IActionResult> DownloadProdPlan()
        {
            //Process and save updated events
            DataSet ds = new();
            ds = await _ppService.ExportProdPlan(new());
            return _ee.DownloadProdPlan(ds, $"ProdPlan");
            //return View("Index", ppv);
        }


        //[HttpGet("Holiday/Get")]
        [HttpGet]
        public async Task<IActionResult> GetHolidays(int year, int month)
        {
            var holidays = await _ppService.GetHoliday(new RequestDTO() { year = year, month = month });
            return Ok(holidays);
        }

        //[HttpPost("Holiday/Save")]
        [HttpPost]
        public async Task<IActionResult> SaveHolidays([FromBody] List<string> request)
        {
            // Save to DB here

            return Ok(await _ppService.SaveHoliday(request));
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePara(ProdPlanViewModel ppv)
        {
            string msg = await _ppService.SavePara(ppv);
            return Content(msg);
        }
        #endregion

        #region Master

        #region Machine Master
        // List
        [HttpGet]
        public async Task<IActionResult> Machine()
        {
            ProdPlanViewModel ppv = await GetMachine();
            return View("Master/Machine/Index",ppv);
        }

        //Add
        [HttpGet]
        public async Task<IActionResult> MachineAdd()
        {
            SMTMachineModel model = new SMTMachineModel();
            return PartialView("Master/Machine/__Add", model);
        }
        [HttpPost]
        public async Task<IActionResult> MachineAdd(SMTMachineModel smm)
        {
            string msg  = await _ppService.MachineAdd(smm);

            ProdPlanViewModel ppv =  await GetMachine();
            if(!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("existed")} {msg}";            

            return PartialView("Master/Machine/_Result",ppv);
        }

        //Edit
        [HttpGet]
        public async Task<IActionResult> MachineEdit(string machineId)
        {
            SMTMachineModel smm = await _ppService.MachineDetail(machineId);
            return PartialView("Master/Machine/__Edit",smm);
        }
        [HttpPost]
        public async Task<IActionResult> MachineEdit(SMTMachineModel smm)
        {
            string msg = await _ppService.MachineEdit(smm);

            ProdPlanViewModel ppv = await GetMachine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/Machine/_result",ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MachineDelete(string machineId)
        {
            string msg = await _ppService.MachineDelete(machineId);

            ProdPlanViewModel ppv = await GetMachine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/Machine/_result", ppv);
        }

        public async Task<ProdPlanViewModel> GetMachine(Dictionary<string,object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();
            ppv.Data = await _ppService.MachineList(dic?? new Dictionary<string, object>());
            return ppv;
        }

        #endregion

        #region Line Master
        [HttpGet]
        public async Task<IActionResult> Line()
        {
            ProdPlanViewModel ppv = await GetLine();
            return View("Master/Line/Index",ppv);            
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> LineAdd()
        {
            SMTLineModel slm = new();
            return PartialView("Master/Line/__Add",slm);
        }
        [HttpPost]
        public async Task<IActionResult> LineAdd(SMTLineModel slm)
        {
            string msg = await _ppService.LineAdd(slm);

            ProdPlanViewModel ppv = await GetLine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("existed")} {msg}";

            return PartialView("Master/Line/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> LineEdit(string lineId)
        {
            SMTLineModel slm = await _ppService.LineDetail(lineId);
            return PartialView("Master/Line/__Edit", slm);
        }
        [HttpPost]
        public async Task<IActionResult> LineEdit(SMTLineModel slm)
        {
            string msg = await _ppService.LineEdit(slm); 
            ProdPlanViewModel ppv = await GetLine();

            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/Line/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> LineDelete(string lineId)
        {
            string msg = await _ppService.LineDelete(lineId);
            ProdPlanViewModel ppv = await GetLine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";
            return PartialView("Master/Line/_Result", ppv);
        }
    

        [HttpGet]
        public async Task<ProdPlanViewModel> GetLine(Dictionary<string,object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();
            
            ppv.Data = await _ppService.LineList(dic?? new Dictionary<string, object>());
            return ppv;
        }

        #endregion

        #region Machine Condition
        [HttpGet]
        public async Task<IActionResult> MachineCondition()
        {
            ProdPlanViewModel ppv = await GetMachineCondition();
            return View("Master/MachineCondition/Index", ppv);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> MachineConditionAdd()
        {
            SMTMachineConditionModel slm = new();
            return PartialView("Master/MachineCondition/__Add", slm);
        }
        [HttpPost]
        public async Task<IActionResult> MachineConditionAdd(SMTMachineConditionModel slm)
        {
            string msg = await _ppService.MachineConditionAdd(slm);

            ProdPlanViewModel ppv = await GetMachineCondition();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("existed")} {msg}";

            return PartialView("Master/MachineCondition/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> MachineConditionEdit(int Id)
        {
            SMTMachineConditionModel slm = await _ppService.MachineConditionDetail(Id);
            return PartialView("Master/MachineCondition/__Edit", slm);
        }
        [HttpPost]
        public async Task<IActionResult> MachineConditionEdit(SMTMachineConditionModel slm)
        {
            string msg = await _ppService.MachineConditionEdit(slm);
            ProdPlanViewModel ppv = await GetMachineCondition();

            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/MachineCondition/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MachineConditionDelete(int Id)
        {
            string msg = await _ppService.MachineConditionDelete(Id);
            ProdPlanViewModel ppv = await GetMachineCondition();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";
            return PartialView("Master/MachineCondition/_Result", ppv);
        }


        [HttpGet]
        public async Task<ProdPlanViewModel> GetMachineCondition(Dictionary<string, object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            ppv.Data = await _ppService.MachineConditionList(dic ?? new Dictionary<string, object>());
            return ppv;
        }

        #endregion

        #region Line Machine
        [HttpGet]
        public async Task<IActionResult> LineMachine()
        {
            ProdPlanViewModel ppv = await GetLineMachine();
            return View("Master/LineMachine/Index", ppv);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> LineMachineAdd()
        {
            SMTLineMachineDataModel slm = new();
            return PartialView("Master/LineMachine/__Add", slm);
        }
        [HttpPost]
        public async Task<IActionResult> LineMachineAdd(SMTLineMachineDataModel slm)
        {
            string msg = await _ppService.LineMachineAdd(slm);

            ProdPlanViewModel ppv = await GetLineMachine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("existed")} {msg}";

            return PartialView("Master/LineMachine/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> LineMachineEdit(int Id)
        {
            SMTLineMachineDataModel slm = await _ppService.LineMachineDetail(Id);
            return PartialView("Master/LineMachine/__Edit", slm);
        }
        [HttpPost]
        public async Task<IActionResult> LineMachineEdit(SMTLineMachineDataModel slm)
        {
            string msg = await _ppService.LineMachineEdit(slm);
            ProdPlanViewModel ppv = await GetLineMachine();

            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/LineMachine/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> LineMachineDelete(int Id)
        {
            string msg = await _ppService.LineMachineDelete(Id);
            ProdPlanViewModel ppv = await GetLineMachine();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";
            return PartialView("Master/LineMachine/_Result", ppv);
        }


        [HttpGet]
        public async Task<ProdPlanViewModel> GetLineMachine(Dictionary<string, object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            ppv.Data = await _ppService.LineMachineList(dic ?? new Dictionary<string, object>());
            return ppv;
        }

        #endregion


        #region Shift Master
        [HttpGet]
        public async Task<IActionResult> Shift()
        {
            ProdPlanViewModel ppv = await GetShift();
            return View("Master/Shift/Index", ppv);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> ShiftAdd()
        {
            SMTShiftModel slm = new();
            return PartialView("Master/Shift/__Add", slm);
        }
        [HttpPost]
        public async Task<IActionResult> ShiftAdd(SMTShiftModel slm)
        {
            string msg = await _ppService.ShiftAdd(slm);

            ProdPlanViewModel ppv = await GetShift();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("existed")} {msg}";

            return PartialView("Master/Shift/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> ShiftEdit(string ShiftId)
        {
            SMTShiftModel slm = await _ppService.ShiftDetail(ShiftId);
            return PartialView("Master/Shift/__Edit", slm);
        }
        [HttpPost]
        public async Task<IActionResult> ShiftEdit(SMTShiftModel slm)
        {
            string msg = await _ppService.ShiftEdit(slm);
            ProdPlanViewModel ppv = await GetShift();

            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";

            return PartialView("Master/Shift/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> ShiftDelete(string ShiftId)
        {
            string msg = await _ppService.ShiftDelete(ShiftId);
            ProdPlanViewModel ppv = await GetShift();
            if (!string.IsNullOrEmpty(msg)) ppv.error_msg = $"{_translationService.Trans("not_existed")} {msg}";
            return PartialView("Master/Shift/_Result", ppv);
        }


        [HttpGet]
        public async Task<ProdPlanViewModel> GetShift(Dictionary<string, object>? dic = null)
        {
            ProdPlanViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            ppv.Data = await _ppService.ShiftList(dic ?? new Dictionary<string, object>());
            return ppv;
        }

        #endregion

        #endregion

        #region COMMON
        // Download Data
        [HttpPost] // Download Web
        public async Task<IActionResult> DownloadWeb([FromBody] TableFilterRequest request)
        {
            var fileBytes = await _ee.AjaxExcelExport(request, "#,##0.0000");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredData.xlsx");
        }

        #endregion

    }
}
