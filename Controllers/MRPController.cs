using Humanizer.Localisation;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Models.PE;
using MESWebDev.Models.MRP;
using MESWebDev.Models.Setting;
using MESWebDev.Repositories;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using static MESWebDev.Common.Export2Excel;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using MESWebDev.Models.ProdPlan;

namespace MESWebDev.Controllers
{
    public class MRPController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IMRPService _mrpService;
        private readonly ILoggingService _loggingService;
        private readonly ISettingService _settingService;
        private readonly Export2Excel _ee;

        public MRPController(AppDbContext context, ITranslationService translationService, IMRPService repository,
            ILoggingService loggingService, ISettingService MRPBOMService)
            : base(context)
        {
            _translationService = translationService;
            _mrpService = repository;
            _loggingService = loggingService;
            _ee = new();
            _settingService = MRPBOMService;
        }

        #region Ini Data: Upload data for BOM, OBL, OH, SPO

        #region ----------- BOM : Bill Of Material ------------
        // List
        [HttpGet]
        public async Task<IActionResult> MRPBOM()
        {
            MRPViewModel ppv = await MRPBOMData();
            return View("IniData/BOM/Index", ppv);
        }

        //Upload
        [HttpPost]
        public async Task<IActionResult> MRPBOMUpload(IFormFile file1)
        {
            MRPViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "No files to upload. ";
                return PartialView("IniData/BOM/_Result", pev);
            }
            try
            {
                pev = await _mrpService.MRPBOMUpload(file1);
                if (pev.UploadedError)
                {
                    pev.FormatRazorDTO = await _settingService.GetFormatRazor();
                    return PartialView("IniData/BOM/_Result", pev);
                }
                pev = await MRPBOMData(pev.Dic);
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            return PartialView("IniData/BOM/_Result", pev);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> MRPBOMAdd()
        {
            MRPBOMModel ppv = new();
            return PartialView("IniData/BOM/__Add", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> MRPBOMAdd(MRPBOMModel psm)
        {
            string msg = await _mrpService.MRPBOMAdd(psm);

            MRPViewModel ppv = await MRPBOMData();

            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }

            return PartialView("IniData/BOM/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> MRPBOMEdit(int id)
        {
            MRPBOMModel pms = await _mrpService.MRPBOMDetail(id);
            return PartialView("IniData/BOM/__Edit", pms);
        }

        [HttpPost]
        public async Task<IActionResult> MRPBOMEdit(MRPBOMModel psm)
        {
            string msg = await _mrpService.MRPBOMEdit(psm);
            MRPViewModel ppv = await MRPBOMData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/BOM/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MRPBOMDelete(int id)
        {
            string msg = await _mrpService.MRPBOMDelete(id);
            MRPViewModel ppv = await MRPBOMData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/BOM/_Result", ppv);
        }

        public async Task<MRPViewModel> MRPBOMData(Dictionary<string, object>? dic = null)
        {
            MRPViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            DataTable data = await _mrpService.MRPBOMList(dic ?? new Dictionary<string, object>());
            ppv.Data = data;
            return ppv;
        }
        #endregion

        #region ----------- OH : On Hand ------------
        // List
        [HttpGet]
        public async Task<IActionResult> MRPOH()
        {
            MRPViewModel ppv = await MRPOHData();
            return View("IniData/OH/Index", ppv);
        }

        //Upload
        [HttpPost]
        public async Task<IActionResult> MRPOHUpload(IFormFile file1)
        {
            MRPViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "No files to upload. ";
                return PartialView("IniData/OH/_Result", pev);
            }
            try
            {
                pev = await _mrpService.MRPOHUpload(file1);
                if (pev.UploadedError)
                {
                    pev.FormatRazorDTO = await _settingService.GetFormatRazor();
                    return PartialView("IniData/OH/_Result", pev);
                }
                pev = await MRPOHData(pev.Dic);
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            return PartialView("IniData/OH/_Result", pev);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> MRPOHAdd()
        {
            MRPOHModel ppv = new();
            return PartialView("IniData/OH/__Add", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> MRPOHAdd(MRPOHModel psm)
        {
            string msg = await _mrpService.MRPOHAdd(psm);

            MRPViewModel ppv = await MRPOHData();

            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }

            return PartialView("IniData/OH/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> MRPOHEdit(int id)
        {
            MRPOHModel pms = await _mrpService.MRPOHDetail(id);
            return PartialView("IniData/OH/__Edit", pms);
        }

        [HttpPost]
        public async Task<IActionResult> MRPOHEdit(MRPOHModel psm)
        {
            string msg = await _mrpService.MRPOHEdit(psm);
            MRPViewModel ppv = await MRPOHData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/OH/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MRPOHDelete(int id)
        {
            string msg = await _mrpService.MRPOHDelete(id);
            MRPViewModel ppv = await MRPOHData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/OH/_Result", ppv);
        }

        public async Task<MRPViewModel> MRPOHData(Dictionary<string, object>? dic = null)
        {
            MRPViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            DataTable data = await _mrpService.MRPOHList(dic ?? new Dictionary<string, object>());
            ppv.Data = data;
            return ppv;
        }
        #endregion

        #region ----------- SPO : Supply Purchase Order ------------
        // List
        [HttpGet]
        public async Task<IActionResult> MRPSPO()
        {
            MRPViewModel ppv = await MRPSPOData();
            return View("IniData/SPO/Index", ppv);
        }

        //Upload
        [HttpPost]
        public async Task<IActionResult> MRPSPOUpload(IFormFile file1)
        {
            MRPViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "No files to upload. ";
                return PartialView("IniData/SPO/_Result", pev);
            }
            try
            {
                pev = await _mrpService.MRPSPOUpload(file1);
                if (pev.UploadedError)
                {
                    pev.FormatRazorDTO = await _settingService.GetFormatRazor();
                    return PartialView("IniData/SPO/_Result", pev);
                }
                pev = await MRPSPOData(pev.Dic);
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            return PartialView("IniData/SPO/_Result", pev);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> MRPSPOAdd()
        {
            MRPSPOModel ppv = new();
            return PartialView("IniData/SPO/__Add", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> MRPSPOAdd(MRPSPOModel psm)
        {
            string msg = await _mrpService.MRPSPOAdd(psm);

            MRPViewModel ppv = await MRPSPOData();

            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }

            return PartialView("IniData/SPO/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> MRPSPOEdit(int id)
        {
            MRPSPOModel pms = await _mrpService.MRPSPODetail(id);
            return PartialView("IniData/SPO/__Edit", pms);
        }

        [HttpPost]
        public async Task<IActionResult> MRPSPOEdit(MRPSPOModel psm)
        {
            string msg = await _mrpService.MRPSPOEdit(psm);
            MRPViewModel ppv = await MRPSPOData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/SPO/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MRPSPODelete(int id)
        {
            string msg = await _mrpService.MRPSPODelete(id);
            MRPViewModel ppv = await MRPSPOData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/SPO/_Result", ppv);
        }

        public async Task<MRPViewModel> MRPSPOData(Dictionary<string, object>? dic = null)
        {
            MRPViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            DataTable data = await _mrpService.MRPSPOList(dic ?? new Dictionary<string, object>());
            ppv.Data = data;
            return ppv;
        }
        #endregion

        #region ----------- OBL : Open Balance ------------
        // List
        [HttpGet]
        public async Task<IActionResult> MRPOBL()
        {
            MRPViewModel ppv = await MRPOBLData();
            return View("IniData/OBL/Index", ppv);
        }

        //Upload
        [HttpPost]
        public async Task<IActionResult> MRPOBLUpload(IFormFile file1)
        {
            MRPViewModel pev = new();
            if (file1 == null)
            {
                pev.error_msg = "No files to upload. ";
                return PartialView("IniData/OBL/_Result", pev);
            }
            try
            {
                pev = await _mrpService.MRPOBLUpload(file1);
                if (pev.UploadedError)
                {
                    pev.FormatRazorDTO = await _settingService.GetFormatRazor();
                    return PartialView("IniData/OBL/_Result", pev);
                }
                pev = await MRPOBLData(pev.Dic);
            }
            catch (Exception ex)
            {
                pev.error_msg = $"An error occurred while processing the files: {ex.Message}";
            }
            return PartialView("IniData/OBL/_Result", pev);
        }

        // Add
        [HttpGet]
        public async Task<IActionResult> MRPOBLAdd()
        {
            MRPOBLModel ppv = new();
            return PartialView("IniData/OBL/__Add", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> MRPOBLAdd(MRPOBLModel psm)
        {
            string msg = await _mrpService.MRPOBLAdd(psm);

            MRPViewModel ppv = await MRPOBLData();

            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }

            return PartialView("IniData/OBL/_Result", ppv);
        }

        // Edit
        [HttpGet]
        public async Task<IActionResult> MRPOBLEdit(int id)
        {
            MRPOBLModel pms = await _mrpService.MRPOBLDetail(id);
            return PartialView("IniData/OBL/__Edit", pms);
        }

        [HttpPost]
        public async Task<IActionResult> MRPOBLEdit(MRPOBLModel psm)
        {
            string msg = await _mrpService.MRPOBLEdit(psm);
            MRPViewModel ppv = await MRPOBLData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/OBL/_Result", ppv);
        }

        // Delete
        [HttpPost]
        public async Task<IActionResult> MRPOBLDelete(int id)
        {
            string msg = await _mrpService.MRPOBLDelete(id);
            MRPViewModel ppv = await MRPOBLData();
            if (!string.IsNullOrEmpty(msg))
            {
                ppv.error_msg = msg;
            }
            return PartialView("IniData/OBL/_Result", ppv);
        }

        public async Task<MRPViewModel> MRPOBLData(Dictionary<string, object>? dic = null)
        {
            MRPViewModel ppv = new();
            ppv.FormatRazorDTO = await _settingService.GetFormatRazor();

            DataTable data = await _mrpService.MRPOBLList(dic ?? new Dictionary<string, object>());
            ppv.Data = data;
            return ppv;
        }
        #endregion

        #endregion


        #region MRP Data
        // List
        [HttpGet]
        public async Task<IActionResult> MRPData()
        {
            MRPViewModel mvm = await GetMRPData();
            return View("MRPData/Index", mvm);
        }

        // RUN
        [HttpPost]
        public async Task<IActionResult> MRPDataRun()
        {
            MRPViewModel mvm = await GetMRPDataRun();
            return PartialView("MRPData/_Result", mvm);
        }

        public async Task<MRPViewModel> GetMRPData(Dictionary<string,object>? dic = null)
        {
            MRPViewModel mvm = new();
            mvm.FormatRazorDTO = await _settingService.GetFormatRazor();
            mvm.Data = await _mrpService.MRPDataList(dic ?? new());
            return mvm;
        }

        public async Task<MRPViewModel> GetMRPDataRun(Dictionary<string, object>? dic = null)
        {
            MRPViewModel mvm = new();
            mvm.FormatRazorDTO = await _settingService.GetFormatRazor();
            mvm.Data = await _mrpService.MRPDataRun(dic ?? new());
            return mvm;
        }

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
