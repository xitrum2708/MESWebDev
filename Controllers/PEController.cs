using AutoMapper;
using Azure.Core;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Repositories;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using static MESWebDev.Common.Export2Excel;

namespace MESWebDev.Controllers
{
    public class PEController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IPEService _peService;
        private readonly Export2Excel _ee;
        private readonly IMapper _mapper;

        public PEController(AppDbContext context, ITranslationService translationService, IPEService peService, IMapper map)
            : base(context)
        {
            _translationService = translationService;
            _peService = peService;
            _ee = new();
            _mapper = map;
        }

        //======================>> Manpower <<======================\\
        #region MANPOWER
        [HttpGet]
        public async Task<IActionResult> Manpower()
        {
            DataTable dt = await _peService.GetManpower(new());
            PEViewModel pev = new();
            pev.data = dt;
            pev.manpower = new();
            return View("Manpower/Index", pev);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file1)
        {
            PEViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "Please upload no file.";
                return View("Manpower/Index", pev);
            }
            try
            {
                pev = await _peService.UploadManpower(file1);
                ViewBag.Success = "Files uploaded and processed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while processing the files: {ex.Message}";
            }
            pev.manpower = new();
            return View("Manpower/Index", pev);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadData()
        {
            //Process and save updated events
            DataSet ds = new();
            //ds = await _repository.ExportProdPlan(new());
            return _ee.DownloadProdPlan(ds, $"ProdPlan");
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] TableFilterRequest request)
        {
            var fileBytes = await _ee.AjaxExcelExport(request, "#,##0.0000");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredData.xlsx");
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> DeleteManpower([FromBody] List<int> ids)
        {
            string msg = string.Empty;
            msg = await _peService.DeleteManpower(ids);
            if (!string.IsNullOrEmpty(msg))
            {
                return Json(new { success = false, error = msg });
            }
            return Json(new { success = true });
        }

        // GET Manpower detail
        [HttpGet]
        public async Task<IActionResult> GetManpowerDetail(int id)
        {
            ManpowerModel mm = new();
            mm = await _peService.GetManpowerDetail(id);
            return PartialView("Manpower/_Edit", mm);
        }

        [HttpPost]
        public async Task<IActionResult> EditManpower(ManpowerModel mm)
        {
            string msg = string.Empty;
            PEViewModel pev = new();
            msg = await _peService.EditManpower(mm);
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            else
            {
                DataTable dt = await _peService.GetManpower(new());
                pev.data = dt;
            }
            pev.manpower = new();
            return View("Manpower/Index", pev);
        }

        [HttpPost]
        public async Task<IActionResult> AddManpower(ManpowerModel mm)
        {
            string msg = string.Empty;
            PEViewModel pev = new();
            msg = await _peService.AddManpower(mm);
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            else
            {
                DataTable dt = await _peService.GetManpower(new());
                pev.data = dt;
            }
            pev.manpower = new();
            return View("Manpower/Index", pev);
        }
        #endregion

        //======================>> Operation Master <<======================\\
        #region OPERATION MASTER
        [HttpGet]
        public async Task<IActionResult> OperationMaster()
        {
            PEViewModel pev = await GetOperationMaster();
            return View("Master/OperationMaster/Index", pev);
        }

        [HttpGet]
        public async Task<IActionResult> AddOperationMaster()
        {
            PEViewModel pev = new();
            pev.operation = new();
            pev.operationDtlList = new();
            return PartialView("Master/OperationMaster/__Add", pev);
        }

        [HttpPost]
        public async Task<IActionResult> AddOperationMaster(PEViewModel pev)
        {
            string msg = await _peService.AddOperation(pev);
            pev = await GetOperationMaster();
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            return PartialView("Master/OperationMaster/_Result", pev);
        }

        [HttpGet]
        public async Task<IActionResult> EditOperationMaster(int id)
        {
            PEViewModel pev = new();
            pev = await _peService.GetOperationDetail(id);
            return PartialView("Master/OperationMaster/__Edit", pev);
        }

        [HttpPost]
        public async Task<IActionResult> EditOperationMaster(PEViewModel pev)
        {
            string msg = await _peService.EditOperation(pev);
            pev = await GetOperationMaster();
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            return PartialView("Master/OperationMaster/_Result", pev);
        }

        //[HttpPost]
        [HttpPost]
        public async Task<IActionResult> DeleteOperationMaster(int id)
        {
            string msg = await _peService.DeleteOperation(id);
            PEViewModel pev = new();
            pev = await GetOperationMaster();
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            return PartialView("Master/OperationMaster/_Result", pev);
        }

        //[HttpPost]
        [HttpPost]
        public async Task<IActionResult> DeleteOperationMasterDtl(int id, int idDetail)
        {

            string msg = await _peService.DeleteOperationDtl(idDetail);
            PEViewModel pev = new();
            pev = await _peService.GetOperationDetail(id);
            if (!string.IsNullOrEmpty(msg))
            {
                pev.error_msg = msg;
            }
            return PartialView("Master/OperationMaster/___EditDtl", pev);

        }

        public async Task<PEViewModel> GetOperationMaster()
        {
            List<OperationModel> om = await _peService.GetOperation();

            //save it for download
            List<OperationDetailDTO> odm = await _peService.GetOperationDtl();
            HttpContext.Session.SetComplexData("DownloadData", odm);

            PEViewModel pev = new();
            pev.operationList = om;
            return pev;
        }

        //for dowload 

        #endregion

        //======================>> Operation Detail Master <<======================\\
        #region OPERATION DETAIL MASTER
        //[HttpGet]
        //public async Task<IActionResult> OperationDtlMaster()
        //{
        //    List<OperationDetailModel> odm = await _peService.GetOperationDtl(new());
        //    PEViewModel pev = new();
        //    pev.operationDtlList = odm;
        //    return View("OperationDtlMaster/Index", pev);
        //}

        #endregion

        //======================>> Operation Time Study <<======================\\
        #region OPERATION TIME STUDY
        [HttpGet]
        public async Task<IActionResult> TimeStudy()
        {
            await ResetSession();
            DataTable dt = await _peService.GetTimeStudy(new());
            PEViewModel pev = new();
            pev.data = dt;
            return View("TimeStudy/Index", pev);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeStudy()
        {
            DataTable dt = await _peService.GetTimeStudy(new());
            PEViewModel pev = new();
            pev.data = dt;
            return PartialView("TimeStudy/_Result", pev);
        }

        [HttpGet]
        public async Task<IActionResult> IniTimeStudy()
        {
            DataSet ds = await _peService.IniTimeStudy(new());
            PEViewModel pev = new();
            if(ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    pev.customers = ds.Tables[0].AsEnumerable().Select(i => i[0].ToString()).Where(j=> !string.IsNullOrEmpty(j)).ToList()!;
                }
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    pev.sections = ds.Tables[1].AsEnumerable().Select(i => i[0].ToString()).Where(j => !string.IsNullOrEmpty(j)).ToList()!;
                }
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    pev.units = ds.Tables[2].AsEnumerable().Select(i => i[0].ToString()).Where(j => !string.IsNullOrEmpty(j)).ToList()!;
                }
            }
            return PartialView("TimeStudy/__Add", pev);
        }

        [HttpGet]
        public async Task<IActionResult> IniAddTimeStudyDetail()
        {
            PEViewModel pev = new();
            pev.StepNo = ( HttpContext.Session.GetInt32("StepNo") ?? 0 ) + 1;
            pev.TimeStudyStepDtl = new();

            HttpContext.Session.SetInt32("StepNo", Convert.ToInt32( pev.StepNo));
            return PartialView("TimeStudy/___AddDetail", pev);            
        }

        // GET Info for other combobox
        [HttpGet]
        public async Task<IActionResult> CbTimeStudy(string? customer, string? section, string? model, string? b_model, 
                                                        string? lot_no, string? pcb_name, string? pcb_no, string? operation_name)
        {
            Dictionary<string,object> dic = new();
            dic["@cus_name"] = customer ?? string.Empty;
            dic["@sec_name"] = section ?? string.Empty;
            dic["@model"] = model ?? string.Empty;
            dic["@b_model"] = b_model ?? string.Empty;
            dic["@lot_no"] = lot_no ?? string.Empty;
            dic["@pcb_name"] = pcb_name ?? string.Empty;
            dic["@pcb_no"] = pcb_no ?? string.Empty;
            dic["@operation_name"] = operation_name ?? string.Empty;

            DataSet ds = await _peService.IniTimeStudy(dic);

            PEViewModel pev = new();

            if (ds != null)
            {             
                pev.customers = ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 ? 
                        ds.Tables[0].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.sections = ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 ?
                        ds.Tables[1].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.units = ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0 ?
                        ds.Tables[2].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.models = ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0 ?
                        ds.Tables[3].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.bModels = ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0 ?
                        ds.Tables[4].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.lotNos = ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0 ?
                        ds.Tables[5].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.pcbNames = ds.Tables[6] != null && ds.Tables[6].Rows.Count > 0 ?
                        ds.Tables[6].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.pcbNos = ds.Tables[7] != null && ds.Tables[7].Rows.Count > 0 ?
                        ds.Tables[7].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
                pev.operationNames = ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0 ?
                        ds.Tables[8].AsEnumerable().Select(i => i[0].ToString()).ToList()! : new List<string>();
            }

            return Json(new
            {
                customers = pev.customers,
                sections = pev.sections,
                units = pev.units,
                models = pev.models,
                bModels = pev.bModels,
                lotNos = pev.lotNos,
                pcbNos = pev.pcbNos,
                pcbNames = pev.pcbNames,
                operationNames = pev.operationNames
            });
        }

        // GET Info for other combobox
        [HttpGet]
        public async Task<IActionResult> CbTimeStudyDetail(string? operation_name)
        {
            PEViewModel pev = await _peService.IniTimeStudyDetail(operation_name);
            return Json(new
            {
                operationNames = pev.operationNames,
                operationDetailNames = pev.operationDtlNames
            });
        }

        //AddTimeStudyDtl
        //AddTimeStudyStepDtl
        [HttpPost]
        public async Task<IActionResult> AddTimeStudyStepDtl(PEViewModel pev)
        {
            if(pev != null)
            {
                TimeStudyStepDtlDTO tsd = pev.TimeStudyStepDtl ?? new();

                decimal[] a = [tsd.Time01 ,tsd.Time02 , tsd.Time03 , tsd.Time04 , tsd.Time05];
                var validNumber = a.Where(i => i > 0).ToArray();
                tsd.TimeAvg = validNumber.Length > 0 ? Math.Round(validNumber.Average(),2) : 0;
                List<TimeStudyStepDtlDTO> tsdList = pev.timeStudyStepDtlList ?? new();
                tsdList.Add(tsd);
                pev.timeStudyStepDtlList = tsdList;
            }
            return PartialView("TimeStudy/___DetailData", pev);
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeStudyDtl(PEViewModel pev)
        {
            if (pev != null)
            {
                List<TimeStudyStepDtlDTO> tsdList = pev.timeStudyStepDtlList ?? new();
                TimeStudyDtlDTO tsDTO = pev.TimeStudyDtl ?? new();

                List<TimeStudyDTO> tsdDtlList = HttpContext.Session.GetComplexData<List<TimeStudyDTO>>("TimeStudyList") ?? new();

                foreach (var item in tsdList)
                {
                    TimeStudyDTO std = _mapper.Map<TimeStudyDTO>(new TimeStudyOtherDTO()
                    {
                        TimeStudyDtl = tsDTO,
                        TimeStudyStepDtl = item
                    });
                    std.TimeTotal = std.TimeAvg * std.UnitQty;
                    tsdDtlList.Add(std);
                }
                // save it to use next time
                HttpContext.Session.SetComplexData("TimeStudyList", tsdDtlList);
                pev.timeStudyList = tsdDtlList;
            }
            return PartialView("TimeStudy/___DetailBody", pev);
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeStudy(PEViewModel pev)
        {
            string msg = string.Empty;
            msg = await _peService.AddTimeStudy(pev);
            pev.data = await _peService.GetTimeStudy(new());
            pev.error_msg = msg;
            return PartialView("TimeStudy/_Result", pev);
        }
        #endregion


        #region COMMON ZONE
        public async Task<IActionResult> DownloadDataList()
        {
            //Process and save updated events
            List<OperationDetailDTO> odm = HttpContext.Session.GetComplexData<List<OperationDetailDTO>>("DownloadData") ?? new List<OperationDetailDTO>();  
            var fileBytes = await _ee.DownloadList(odm);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OperationMaster.xlsx");
        }

        //------ RESET HTTP CONTEXT SESSION
        private async Task ResetSession()
        {
            HttpContext.Session.SetComplexData(SD.SessionParametter.DataDT, null);
            HttpContext.Session.SetComplexData(SD.SessionParametter.DataList, null);
            HttpContext.Session.SetComplexData(SD.SessionParametter.ListItem, null);
            HttpContext.Session.SetComplexData(SD.SessionParametter.SearchData, null);
            HttpContext.Session.SetComplexData(SD.SessionParametter.DataOther, null);
            //TimeStudyList
            HttpContext.Session.SetComplexData("TimeStudyList", null);


            HttpContext.Session.Remove(SD.SessionParametter.ID);
            HttpContext.Session.Remove(SD.SessionParametter.IdList);
            HttpContext.Session.Remove(SD.SessionParametter.FuncID);
            HttpContext.Session.Remove(SD.SessionParametter.UserOnly);
            HttpContext.Session.Remove(SD.SessionParametter.FileName);
            HttpContext.Session.Remove("StepNo");
        }
        #endregion
    }

}
