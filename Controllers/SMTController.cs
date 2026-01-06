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
    public class SMTController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IPEService _peService;
        private readonly Export2Excel _ee;
        private readonly Export2SpecificExcel _ese;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _web;

        public SMTController(AppDbContext context, ITranslationService translationService, IPEService peService, IMapper map, IWebHostEnvironment web)
            : base(context)
        {
            _translationService = translationService;
            _peService = peService;
            _ee = new();
            _mapper = map;
            _web = web;
            _ese = new(_web);
        }

        //======================>> Project Setting <<======================\\
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
            

            HttpContext.Session.Remove(SD.SessionParametter.ID);
            HttpContext.Session.Remove(SD.SessionParametter.IdList);
            HttpContext.Session.Remove(SD.SessionParametter.FuncID);
            HttpContext.Session.Remove(SD.SessionParametter.UserOnly);
            HttpContext.Session.Remove(SD.SessionParametter.FileName);
        }
        #endregion
    }

}
