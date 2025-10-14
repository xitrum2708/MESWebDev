using AutoMapper;
using Azure.Core;
using ExcelDataReader.Log;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.COMMON;
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
using NuGet.Protocol.Core.Types;
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
        private readonly Export2SpecificExcel _ese;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _web;

        public PEController(AppDbContext context, ITranslationService translationService, IPEService peService, IMapper map, IWebHostEnvironment web)
            : base(context)
        {
            _translationService = translationService;
            _peService = peService;
            _ee = new();
            _mapper = map;
            _web = web;
            _ese = new(_web);
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

        //======================>> Time Study <<======================\\
        #region TIME STUDY
        [HttpGet]
        public async Task<IActionResult> TimeStudy()
        {
            await ResetSession();
            DataTable dt = await _peService.GetTimeStudy(new());
            PEViewModel pev = new();
            pev.data = dt;
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return View("TimeStudy/Index", pev);
        }
        // GET Info for other combobox
        [HttpGet]
        public async Task<IActionResult> CbTimeStudy(string? customer, string? section, string? model, string? b_model,
                                                        string? lot_no, string? pcb_name, string? pcb_no, string? operation_name)
        {
            Dictionary<string, object> dic = new();
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
                operationDtlNames = pev.operationDtlNames
            });
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
            // When add new, reset step No
            HttpContext.Session.Remove("StepNo");

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
        public async Task<IActionResult> IniAddTimeStudyDetail(PEViewModel pev)
        {
            pev = pev == null? new(): pev;
            if(pev.timeStudyList != null && pev.timeStudyList.Count > 0)
            {
                pev.StepNo = pev.timeStudyList.Max(i => i.StepNo) + 1;
            }
            else
            {
                pev.StepNo = (HttpContext.Session.GetInt32("StepNo") ?? 0) + 1;

                pev.TimeStudyStepDtl = new();
            }

            HttpContext.Session.SetInt32("StepNo", Convert.ToInt32( pev.StepNo));
            return PartialView("TimeStudy/___AddDetail", pev);            
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
                tsd.StepId = tsdList.Count > 0 ? tsdList.First().StepId : tsd.StepId;
                tsdList.Add(tsd);
                pev.timeStudyStepDtlList = tsdList;
            }
            return PartialView("TimeStudy/___DetailData", pev);
        }

        [HttpPost]
        public IActionResult EditTimeStudyStepDtl(PEViewModel pev, int index)
        {
            // model = form fields (OperationName, OperationDetailName, etc.)
            // index = the value you appended manually in JS
            TimeStudyStepDtlDTO tds = pev.TimeStudyStepDtl ?? new();
            if(tds != null)
            {
                decimal[] a = [tds.Time01, tds.Time02, tds.Time03, tds.Time04, tds.Time05];
                var validNumber = a.Where(i => i > 0).ToArray();
                tds.TimeAvg = validNumber.Length > 0 ? Math.Round(validNumber.Average(), 2) : 0;
                tds.SeqNo = index + 1;
                if(pev.timeStudyStepDtlList == null)
                {
                    pev.timeStudyStepDtlList = new List<TimeStudyStepDtlDTO>();
                }
                pev.timeStudyStepDtlList[index] = tds;
            }
            return PartialView("TimeStudy/___DetailData", pev);
        }




        [HttpPost]
        public async Task<IActionResult> AddTimeStudyDtl(PEViewModel pev)
        {
            if (pev != null)
            {
                List<TimeStudyStepDtlDTO> tsdList = pev.timeStudyStepDtlList ?? new();
                List<TimeStudyDTO> tsdDtlList = HttpContext.Session.GetComplexData<List<TimeStudyDTO>>("TimeStudyList") ?? new();

                if (tsdList.Count> 0)
                {
                    TimeStudyDtlDTO tsDTO = pev.TimeStudyDtl ?? new();

                    // Delete old step
                    tsdDtlList.RemoveAll(i => i.StepNo == tsDTO.StepNo);

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
                }
                else
                {
                    //reduce step = stepNo -1;
                    TimeStudyDtlDTO tsDTO = pev.TimeStudyDtl ?? new();
                    // Delete old step
                    tsdDtlList.RemoveAll(i => i.StepNo == tsDTO.StepNo);
                    int step = (HttpContext.Session.GetInt32("StepNo") ?? 0) + 1;
                    //save it 
                    HttpContext.Session.SetInt32("StepNo", step - 1);
                }


                
                //List<TimeStudyDTO> tsdDetail = HttpContext.Session.GetComplexData<List<TimeStudyDTO>>("EditTimeStudyDetail") ?? new();
                //tsdDetail.AddRange(tsdDtlList);


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
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);

            return PartialView("TimeStudy/_Result", pev);
        }

        //------------------------------------------------------------------------------------------------------------\\

        [HttpGet]
        public async Task<IActionResult> EditTimeStudy(int id)
        {
            PEViewModel pev = new();
            pev = await _peService.GetTimeStudyEdit(id);

            //save detail here
            HttpContext.Session.SetComplexData("TimeStudyList", pev.timeStudyList ?? new());
            return PartialView("TimeStudy/__Edit", pev);
        }

        [HttpPost]
        public async Task<IActionResult> EditTimeStudy(PEViewModel pev)
        {
            string msg = string.Empty;
            msg = await _peService.EditTimeStudy(pev);
            pev.data = await _peService.GetTimeStudy(new());
            pev.error_msg = msg;
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return PartialView("TimeStudy/_Result", pev);
        }

        [HttpGet]
        public async Task<IActionResult> EditTimeStudyDtl(int stepNo)
        {
            PEViewModel pev = new();
            List<TimeStudyDTO> list = HttpContext.Session.GetComplexData<List<TimeStudyDTO>>("TimeStudyList") ?? new();
            var listDetail = list.Where(i => i.StepNo == stepNo);
            if (listDetail.Any())
            {
                pev.TimeStudyDtl = _mapper.Map<TimeStudyDtlDTO>(listDetail.First());
                pev.timeStudyStepDtlList = _mapper.Map<List<TimeStudyStepDtlDTO>>(listDetail);
            }
            pev.StepNo = stepNo;
            return PartialView("TimeStudy/___AddDetail", pev);
        }
        [HttpGet]
        public async Task<IActionResult> EditTimeStudyDtl2(int stepID)
        {
            PEViewModel pev = new();
            pev = await _peService.GetTimeStudyDtlEdit(stepID);
            return PartialView("TimeStudy/___AddDetail", pev);
        }


        // UPLOAD FILE
        [HttpPost]
        public async Task<IActionResult> TimeStudyUploadFile(IFormFile file1)
        {
            PEViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "Please upload no file.";
                return PartialView("TimeStudy/_Result", pev);
            }
            try
            {
                pev = await _peService.UploadTimeStudy(file1);
                //pev.error_msg = msg;
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);

            return PartialView("TimeStudy/_Result", pev);
        }
        [HttpGet]
        public async Task<IActionResult> TimeStudySearch()
        {
            TimeStudyHdrDTO timeStudyHdr = new TimeStudyHdrDTO();
            return PartialView("TimeStudy/__Search", timeStudyHdr);
        }
        [HttpPost]
        public async Task<IActionResult> TimeStudySearch(TimeStudyHdrDTO tsh)
        {
            PEViewModel pev = new();
            Dictionary<string, object> dic = new();
            if (tsh != null)
            {
                // Search dictionary base on tsh
                dic.Add("@cus_name", tsh.Customer ?? string.Empty);
                dic.Add("@sec_name", tsh.Section ?? string.Empty);
                dic.Add("@model", tsh.Model ?? string.Empty);
                dic.Add("@b_model", tsh.BModel ?? string.Empty);
                dic.Add("@lot", tsh.LotNo ?? string.Empty);
                dic.Add("@pcb_name", tsh.PcbName ?? string.Empty);
                dic.Add("@pcb_no", tsh.PcbNo ?? string.Empty);
            }
            pev.data = await _peService.GetTimeStudy(dic);
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return PartialView("TimeStudy/_Result", pev);
        }
        #endregion

        //======================>> Time Study New <<======================\\
        #region TIME STUDY NEW
        [HttpGet]
        public async Task<IActionResult> TimeStudyNew()
        {
            await ResetSession();
            DataTable dt = await _peService.GetTimeStudyNew(new());
            PEViewModel pev = new();
            pev.data = dt;
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return View("TimeStudyNew/Index", pev);
        }
        // GET Info for other combobox
        [HttpGet]
        public async Task<IActionResult> GetTimeStudyNew()
        {
            DataTable dt = await _peService.GetTimeStudyNew(new());
            PEViewModel pev = new();
            pev.data = dt;
            return PartialView("TimeStudyNew/_Result", pev);
        }

        [HttpGet]
        public async Task<IActionResult> IniTimeStudyNew()
        {
            // When add new, reset step No
            HttpContext.Session.Remove("StepNo");

            DataSet ds = await _peService.IniTimeStudy(new());
            PEViewModel pev = new();
            if (ds != null)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    pev.customers = ds.Tables[0].AsEnumerable().Select(i => i[0].ToString()).Where(j => !string.IsNullOrEmpty(j)).ToList()!;
                }
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    pev.sections = ds.Tables[1].AsEnumerable().Select(i => i[0].ToString()).Where(j => !string.IsNullOrEmpty(j)).ToList()!;
                }
            }
            return PartialView("TimeStudyNew/__Add", pev);
        }

     



        //AddTimeStudyDtl
        //AddTimeStudyStepDtl
        [HttpPost]
        public async Task<IActionResult> AddTimeStudyNewStepDtl(PEViewModel pev)
        {
            if (pev != null)
            {
                TimeStudyNewStepDtlDTO tsd = pev.TimeStudyNewStepDtl ?? new();
                int index = (pev.TimeStudyNewStepDtlList != null ? pev.TimeStudyNewStepDtlList.Count : 0);
                tsd = await CalStepDtl(tsd, index);

                List<TimeStudyNewStepDtlDTO> tsdList = pev.TimeStudyNewStepDtlList ?? new();
                tsd.StepId = tsdList.Count > 0 ? tsdList.First().StepId : tsd.StepId;

                tsdList.Add(tsd);
                pev.TimeStudyNewStepDtlList = tsdList;
            }
            return PartialView("TimeStudyNew/___DetailData", pev);
        }

        [HttpPost]
        public async Task<IActionResult> EditTimeStudyNewStepDtl(PEViewModel pev, int index)
        {
            // model = form fields (OperationName, OperationDetailName, etc.)
            // index = the value you appended manually in JS
            TimeStudyNewStepDtlDTO tsd = pev.TimeStudyNewStepDtl ?? new();
            if (tsd != null)
            {
                tsd.StepId = pev.TimeStudyNewDtl.Id;
                tsd.StepContent = pev.TimeStudyNewDtl.StepContent;
                if (pev.TimeStudyNewStepDtlList == null)
                {
                    pev.TimeStudyNewStepDtlList = new List<TimeStudyNewStepDtlDTO>();
                }
                pev.TimeStudyNewStepDtlList[index] = await CalStepDtl(tsd,index);
            }
            return PartialView("TimeStudyNew/___DetailData", pev);
        }

        public async Task<TimeStudyNewStepDtlDTO> CalStepDtl(TimeStudyNewStepDtlDTO tsd, int index)
        {
            tsd.Time01 = tsd.Time01 < 0 ? 0 : Math.Round(tsd.Time01 / tsd.ProcessQty, 2);
            tsd.Time02 = tsd.Time02 < 0 ? 0 : Math.Round(tsd.Time02 / tsd.ProcessQty, 2);
            tsd.Time03 = tsd.Time03 < 0 ? 0 : Math.Round(tsd.Time03 / tsd.ProcessQty, 2);
            tsd.Time04 = tsd.Time04 < 0 ? 0 : Math.Round(tsd.Time04 / tsd.ProcessQty, 2);
            tsd.Time05 = tsd.Time05 < 0 ? 0 : Math.Round(tsd.Time05 / tsd.ProcessQty, 2);
            decimal[] a = [tsd.Time01, tsd.Time02, tsd.Time03, tsd.Time04, tsd.Time05];
            var validNumber = a.Where(i => i > 0).ToArray();
            tsd.TimeAvg = validNumber.Length > 0 ? Math.Round(validNumber.Average(), 2) : 0;
            tsd.SeqNo = index + 1;
            return tsd;
        }


        [HttpPost]
        public async Task<IActionResult> AddTimeStudyNewDtl(PEViewModel pev)
        {
            if (pev != null)
            {
                List<TimeStudyNewStepDtlDTO> tsdList = pev.TimeStudyNewStepDtlList ?? new();
                List<TimeStudyNewDtlDTO> tsdDtlList = HttpContext.Session.GetComplexData<List<TimeStudyNewDtlDTO>>("TimeStudyDtlList") ?? new();
                List<TimeStudyNewStepDtlDTO> tsdStepDtlList = HttpContext.Session.GetComplexData<List<TimeStudyNewStepDtlDTO>>("TimeStudyStepDtlList") ?? new();

                //public decimal Sumary { get; set; } = 0; // Sum of all step time
                //public decimal SetTime { get; set; } = 0; // Set = Sumary * UnitQty       
                //public int TargetQty { get; set; } = 1; // Target Qty = 460 * 60 / SetTime  
                //public decimal ProcessTime { get; set; } = 0; // Process Time = SetTime / AllocatedOpr


                if (tsdList.Count > 0)
                {
                    TimeStudyNewDtlDTO tsDTO = pev.TimeStudyNewDtl ?? new();
                    tsDTO.Sumary = tsdList.Sum(i=>i.TimeAvg);
                    tsDTO.SetTime = Math.Round(tsDTO.Sumary * tsDTO.UnitQty, 2);
                    tsDTO.TargetQty = tsDTO.SetTime > 0 ? Convert.ToInt32(Math.Floor(460 * 60 / tsDTO.SetTime)) : 0;
                    tsDTO.ProcessTime = tsDTO.AllocatedOpr > 0 ? Math.Round(tsDTO.SetTime / tsDTO.AllocatedOpr, 2) : 0;

                    // Delete old step
                    tsdDtlList.RemoveAll(i => i.StepNo == tsDTO.StepNo);

                    tsdDtlList.Add(tsDTO);
                    tsdStepDtlList.RemoveAll(i => i.StepId == tsDTO.Id);
                    tsdStepDtlList.AddRange(tsdList);
                }
                else
                {
                    //reduce step = stepNo -1;
                    TimeStudyNewDtlDTO tsDTO = pev.TimeStudyNewDtl ?? new();
                    // Delete old step
                    tsdDtlList.RemoveAll(i => i.StepNo == tsDTO.StepNo);
                    int step = (HttpContext.Session.GetInt32("StepNo") ?? 0) + 1;
                    //save it 
                    HttpContext.Session.SetInt32("StepNo", step - 1);
                }

                //List<TimeStudyNewDTO> tsdDetail = HttpContext.Session.GetComplexData<List<TimeStudyNewDTO>>("EditTimeStudyNewDetail") ?? new();
                //tsdDetail.AddRange(tsdDtlList);

                // save it to use next time
                HttpContext.Session.SetComplexData("TimeStudyDtlList", tsdDtlList);
                HttpContext.Session.SetComplexData("TimeStudyStepDtlList", tsdStepDtlList);
                pev.TimeStudyNewDtlList = tsdDtlList.Count > 0 ? tsdDtlList.OrderBy(i=>i.StepNo).ToList(): tsdDtlList;
                pev.TimeStudyNewStepDtlList = tsdStepDtlList;
            }
            return PartialView("TimeStudyNew/___DetailBody", pev);
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeStudyNew(PEViewModel pev)
        {
            string msg = string.Empty;
            msg = await _peService.AddTimeStudyNew(pev);
            pev.data = await _peService.GetTimeStudyNew(new());
            pev.error_msg = msg;
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);

            return PartialView("TimeStudyNew/_Result", pev);
        }

        //------------------------------------------------------------------------------------------------------------\\

        [HttpGet]
        public async Task<IActionResult> EditTimeStudyNew(int id)
        {
            PEViewModel pev = new();
            pev = await _peService.GetTimeStudyNewEdit(id);

            //save detail here
            HttpContext.Session.SetComplexData("TimeStudyDtlList", pev.TimeStudyNewDtlList ?? new());
            HttpContext.Session.SetComplexData("TimeStudyStepDtlList", pev.TimeStudyNewStepDtlList ?? new());
            return PartialView("TimeStudyNew/__Edit", pev);
        }

        [HttpPost]
        public async Task<IActionResult> EditTimeStudyNew(PEViewModel pev)
        {
            string msg = string.Empty;
            List<TimeStudyNewStepDtlDTO> Steplist = HttpContext.Session.GetComplexData<List<TimeStudyNewStepDtlDTO>>("TimeStudyStepDtlList") ?? new();

            pev.TimeStudyNewStepDtlList = Steplist;
            msg = await _peService.EditTimeStudyNew(pev);
            pev.data = await _peService.GetTimeStudyNew(new());
            pev.error_msg = msg;
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return PartialView("TimeStudyNew/_Result", pev);
        }

        [HttpGet]
        public async Task<IActionResult> EditTimeStudyNewDtl(int stepNo)
        {
            PEViewModel pev = new();
            List<TimeStudyNewDtlDTO> Dtllist = HttpContext.Session.GetComplexData<List<TimeStudyNewDtlDTO>>("TimeStudyDtlList") ?? new();
            List<TimeStudyNewStepDtlDTO> Steplist = HttpContext.Session.GetComplexData<List<TimeStudyNewStepDtlDTO>>("TimeStudyStepDtlList") ?? new();
            var listDetail = Dtllist.Where(i => i.StepNo == stepNo);
            if (listDetail.Any())
            {
                pev.TimeStudyNewDtl = _mapper.Map<TimeStudyNewDtlDTO>(listDetail.First());
                pev.TimeStudyNewStepDtlList = _mapper.Map<List<TimeStudyNewStepDtlDTO>>(Steplist.Where(i=>i.StepId == pev.TimeStudyNewDtl.Id));
            }
            pev.StepNo = stepNo;
            return PartialView("TimeStudyNew/___AddDetail", pev);
        }

        // UPLOAD FILE
        [HttpPost]
        public async Task<IActionResult> TimeStudyNewUploadFile(IFormFile file1)
        {
            PEViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "Please upload no file.";
                return PartialView("TimeStudyNew/_Result", pev);
            }
            try
            {
                pev = await _peService.UploadTimeStudyNew(file1);
                //pev.error_msg = msg;
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);

            return PartialView("TimeStudyNew/_Result", pev);
        }
        [HttpGet]
        public async Task<IActionResult> TimeStudyNewSearch()
        {
            TimeStudyNewHdrDTO TimeStudyNewHdr = new TimeStudyNewHdrDTO();
            return PartialView("TimeStudyNew/__Search", TimeStudyNewHdr);
        }
        [HttpPost]
        public async Task<IActionResult> TimeStudyNewSearch(TimeStudyNewHdrDTO tsh)
        {
            PEViewModel pev = new();
            Dictionary<string, object> dic = new();
            if (tsh != null)
            {
                // Search dictionary base on tsh
                dic.Add("@sec_name", tsh.Section ?? string.Empty);
                dic.Add("@model", tsh.Model ?? string.Empty);
                dic.Add("@b_model", tsh.BModel ?? string.Empty);
                dic.Add("@lot", tsh.LotNo ?? string.Empty);
                //dic.Add("@is_prepare", tsh.IsPrepare);
            }
            pev.data = await _peService.GetTimeStudyNew(dic);
            HttpContext.Session.SetComplexDatatable(SD.SessionParametter.DataDT, pev.data);
            return PartialView("TimeStudyNew/_Result", pev);
        }

        public async Task<IActionResult> DownloadTimeStudy(int id)
        {
            DataSet ds = new();
            Dictionary<string, object> dic = new();
            dic.Add("@id", id);
            ds = await _peService.ExportTimeStudyNew(dic);
            List<UploadFileMaster> ufm = await _peService.GetUploadFileMaster("PETimeStudyReport");
            return await _ese.ExportTimeStudy(ds, ufm);
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

        public async Task<IActionResult> DownloadData()
        {
            DataTable dt = HttpContext.Session.GetComplexDatatable(SD.SessionParametter.DataDT)?? new DataTable();

            return _ee.DownloadData(dt, $"TimeStudy");
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
            HttpContext.Session.SetComplexData("TimeStudyDtlList", null);
            HttpContext.Session.SetComplexData("TimeStudyDtl", null);
            HttpContext.Session.SetComplexData("TimeStudyStepDtlList", null);

            //EditTimeStudyDetail
            HttpContext.Session.SetComplexData("EditTimeStudyDetail", null);



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
