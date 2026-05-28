using AutoMapper;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.MRP;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MESWebDev.Models.MRP.DTO;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MESWebDev.Services
{
    public class MRPService : IMRPService
    {
        private readonly AppDbContext _con;
        private readonly IMapper _mapper;
        private readonly Procedure _proc;
        private readonly IHttpContextAccessor _hca;
        private readonly ITranslationService _trans;
        private readonly DatatableListMap _dlm;
        public MRPService(AppDbContext con, IMapper mapper, IHttpContextAccessor hca, ITranslationService tran)
        {
            _con = con;
            _mapper = mapper;
            _proc = new Procedure(con);
            _hca = hca;
            _trans = tran;
            _dlm = new DatatableListMap();
        }

        /*--------------------------------------------------
         MRP BOM : Bill of Material List
        --------------------------------------------------*/
        #region ------------- MRP BOM ---------------
        public async Task<DataTable> MRPBOMList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_BOM", dic);
            return dt;
        }
        public async Task<MRPViewModel> MRPBOMUpload(IFormFile file)
        {
            MRPViewModel ppv = new();
            try
            {
                ppv = await GetMRPBOMUploadData(file);

                if (ppv.UploadedError)
                {
                    ppv.Data = _dlm.ToDataTable<MRPBOMUpload>(ppv.BOMUploadList?? new());
                    ppv.error_msg = SD.ErrorMsg.UploadedError;
                    return ppv;
                }
                else if (ppv.BOMList != null && ppv.BOMList.Count > 0)
                {
                    // delete all existing data 
                    // DO IT LATER, NOW JUST FOR TESTING PURPOSE
                    // Testing just delete by Model
                    var check = ppv.BOMList.Select(i =>  i.Model+"|"+ i.Item ).ToList();
                    var existingData = await _con.UV_MRP_BOM
                                        .Where(i => check.Contains(i.Model + "|" + i.Item)).ExecuteDeleteAsync();
                    // add new one
                    await _con.UV_MRP_BOM.AddRangeAsync(ppv.BOMList);
                    await _con.SaveChangesAsync();
                    ppv.Dic = new Dictionary<string, object>()
                    {
                        {"@UploadedFile", ppv.UploadedFile}
                        ,{"@CreatedDt", ppv.CreatedDt} 
                    };
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<MRPViewModel> GetMRPBOMUploadData(IFormFile file)
        {
            MRPViewModel mvm = new();
            List<MRPBOMModel> bomList = new();
            List<MRPBOMDTO> bomDTOList = new();
            string uploadedFile = file.FileName;
            DateTime dt = DateTime.Now;

            string error_msg = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("MRPBOM"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            error_msg = string.Empty;
                            if (count <= header_row) continue; // skip header

                            //Model
                            //Item
                            //BOMQty
                            //DrawingPstNo

                            string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                            string item = CommonFormat.GetValueOrDefault(reader, colMap, "Item", string.Empty);
                            int BOMQty = CommonFormat.GetValueOrDefault(reader, colMap, "BOMQty", 0);
                            string drawingPstNo = CommonFormat.GetValueOrDefault(reader, colMap, "DrawingPstNo", string.Empty);


                            if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(item) || BOMQty == 0 || string.IsNullOrEmpty(drawingPstNo))
                            {
                                error_msg = SD.ErrorMsg.CheckValue;
                                mvm.UploadedError = true;
                            }

                            MRPBOMDTO bomDTO = new()
                            {
                                Model = model,
                                Item = item,
                                BOMQty = BOMQty,
                                DrawingPstNo = drawingPstNo,
                                ErrorMsg = error_msg,
                                UploadedFile = uploadedFile,
                                //Remark = "Data from IFS System",
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                CreatedDt = dt
                            };
                            bomDTOList.Add(bomDTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mvm.error_msg = ex.Message;
            }

            if (mvm.UploadedError)
            {
                mvm.BOMUploadList = _mapper.Map<List<MRPBOMUpload>>( bomDTOList);
            }
            else
            {
                bomList = _mapper.Map<List<MRPBOMModel>>(bomDTOList);
                mvm.BOMList = bomList;
                mvm.UploadedFile = uploadedFile; 
                mvm.CreatedDt = dt;
            }

            return mvm;
        }


        public async Task<MRPBOMModel> MRPBOMDetail(int Id)
        {
            return _con.UV_MRP_BOM.Find(Id) ?? new();
        }

        public async Task<string> MRPBOMAdd(MRPBOMModel bom)
        {
            string error_msg = string.Empty;
            try
            {
                //delete old data
                await _con.UV_MRP_BOM.Where(i => i.Model == bom.Model && i.Item == bom.Item).ExecuteDeleteAsync();

                bom.CreatedDt = DateTime.Now;
                bom.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                //add new one
                await _con.UV_MRP_BOM.AddAsync(bom);

                await _con.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                error_msg = ex.Message;
            }

            return error_msg;
        }

        public async Task<string> MRPBOMEdit(MRPBOMModel bom)
        {
            string error_msg = string.Empty;
            try
            {
                var data = await _con.UV_MRP_BOM.FindAsync(bom.Id);
                data.BOMQty = bom.BOMQty;
                data.DrawingPstNo = bom.DrawingPstNo;
                data.UpdatedDt = DateTime.Now;
                data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }
            return error_msg;
        }

        public async Task<string> MRPBOMDelete(int Id)
        {
            try
            {
                await _con.UV_MRP_BOM.Where(i => i.Id == Id).ExecuteDeleteAsync();
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion

        /*--------------------------------------------------
         MRP Data : Material Requirement Planning 
        --------------------------------------------------*/
        #region ------------- MRP Data ---------------

        public async Task<DataTable> MRPDataList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_Data", dic);
            return dt;
        }

        public async Task<DataTable> MRPDataRun(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dic.Add("@CreatedBy", _hca.HttpContext?.User?.Identity?.Name ?? "system");
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_Running", dic);
            return dt;
        }
        #endregion

        /*--------------------------------------------------
         MRP OBL : Open Balance List
        --------------------------------------------------*/
        #region ------------- MRP OBL ---------------

        public async Task<DataTable> MRPOBLList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_OBL", dic);
            return dt;
        }

        public async Task<MRPViewModel> MRPOBLUpload(IFormFile file)
        {
            MRPViewModel ppv = new();
            try
            {
                ppv = await GetMRPOBLUploadData(file);

                if (ppv.UploadedError)
                {
                    ppv.Data = _dlm.ToDataTable<MRPOBLUpload>(ppv.OBLUploadList?? new());
                    ppv.error_msg = SD.ErrorMsg.UploadedError;
                    return ppv;
                }
                else if (ppv.OBLList != null && ppv.OBLList.Count > 0)
                {
                    // delete all existing data 
                    // DO IT LATER, NOW JUST FOR TESTING PURPOSE
                    // Testing just delete by Model
                    var check = ppv.OBLList.Select(i => i.Item + "|" + i.PONo ).ToList();
                    var existingData = await _con.UV_MRP_OBL
                                        .Where(i => check.Contains(i.Item + "|" + i.PONo)).ExecuteDeleteAsync();
                    // add new one
                    await _con.UV_MRP_OBL.AddRangeAsync(ppv.OBLList);
                    await _con.SaveChangesAsync();
                    ppv.Dic = new Dictionary<string, object>()
                    {
                        { "@UploadedFile", ppv.UploadedFile}
                        ,{"@CreatedDt", ppv.CreatedDt} 
                    };
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<MRPViewModel> GetMRPOBLUploadData(IFormFile file)
        {
            MRPViewModel mvm = new();
            List<MRPOBLModel> OBLList = new();
            List<MRPOBLDTO> OBLDTOList = new();
            string uploadedFile = file.FileName;
            DateTime dt = DateTime.Now;

            string error_msg = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("MRPOBL"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            error_msg = string.Empty;
                            if (count <= header_row) continue; // skip header

                            //Item
                            //OBLQty
                            //PONo

                            string item = CommonFormat.GetValueOrDefault(reader, colMap, "Item", string.Empty);
                            int OBLQty = CommonFormat.GetValueOrDefault(reader, colMap, "OBLQty", 0);
                            string poNo = CommonFormat.GetValueOrDefault(reader, colMap, "PONo", string.Empty);

                            if (string.IsNullOrEmpty(item) || OBLQty == 0 || string.IsNullOrEmpty(poNo))
                                //if (string.IsNullOrEmpty(model) || OBLQty == 0 || string.IsNullOrEmpty(poNo))
                            {
                                error_msg = SD.ErrorMsg.CheckValue;
                                mvm.UploadedError = true;
                            }

                            MRPOBLDTO OBLDTO = new()
                            {
                                Item = item,
                                OBLQty = OBLQty,
                                PONo = poNo,
                                ErrorMsg = error_msg,
                                UploadedFile = uploadedFile,
                                //Remark = "Data from IFS System",
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                CreatedDt = dt
                            };
                            OBLDTOList.Add(OBLDTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mvm.error_msg = ex.Message;
            }

            if (mvm.UploadedError)
            {
                mvm.OBLUploadList = _mapper.Map<List<MRPOBLUpload>>(OBLDTOList);
            }
            else
            {
                OBLList = _mapper.Map<List<MRPOBLModel>>(OBLDTOList);
                mvm.OBLList = OBLList;
                mvm.UploadedFile = uploadedFile; mvm.CreatedDt = dt;
            }

            return mvm;
        }

        public async Task<MRPOBLModel> MRPOBLDetail(int Id)
        {
            return _con.UV_MRP_OBL.Find(Id) ?? new();
        }

        public async Task<string> MRPOBLAdd(MRPOBLModel OBL)
        {
            string error_msg = string.Empty;
            try
            {
                //delete old data
                await _con.UV_MRP_OBL.Where(i => i.Item == OBL.Item && i.PONo == OBL.PONo).ExecuteDeleteAsync();

                OBL.CreatedDt = DateTime.Now;
                OBL.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                //add new one
                await _con.UV_MRP_OBL.AddAsync(OBL);

                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }

            return error_msg;
        }

        public async Task<string> MRPOBLEdit(MRPOBLModel OBL)
        {
            string error_msg = string.Empty;
            try
            {
                var data = await _con.UV_MRP_OBL.FindAsync(OBL.Id);
                data.OBLQty = OBL.OBLQty;
                data.PONo = OBL.PONo;
                data.UpdatedDt = DateTime.Now;
                data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }
            return error_msg;
        }

        public async Task<string> MRPOBLDelete(int Id)
        {
            try
            {
                await _con.UV_MRP_OBL.Where(i => i.Id == Id).ExecuteDeleteAsync();
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion

        /*--------------------------------------------------
         MRP OH : ON-HAND
        --------------------------------------------------*/
        #region ------------- MRP OH ---------------


        public async Task<DataTable> MRPOHList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_OH", dic);
            return dt;
        }
        public async Task<MRPViewModel> MRPOHUpload(IFormFile file)
        {
            MRPViewModel ppv = new();
            try
            {
                ppv = await GetMRPOHUploadData(file);

                if (ppv.UploadedError)
                {
                    ppv.Data = _dlm.ToDataTable<MRPOHUpload>(ppv.OHUploadList ?? new());
                    ppv.error_msg = SD.ErrorMsg.UploadedError;
                    return ppv;
                }
                else if (ppv.OHList != null && ppv.OHList.Count > 0)
                {
                    // delete all existing data 
                    // Delete by Item and Location
                    var check = ppv.OHList.Select(i => i.Item+"|"+i.Location).ToList();
                    var existingData = await _con.UV_MRP_OH
                                        .Where(i => check.Contains(i.Item + "|" + i.Location)).ExecuteDeleteAsync();

                    // add new one
                    await _con.UV_MRP_OH.AddRangeAsync(ppv.OHList);
                    await _con.SaveChangesAsync();
                    ppv.Dic = new Dictionary<string, object>()
                    {
                        {"@UploadedFile", ppv.UploadedFile}
                        ,{"@CreatedDt", ppv.CreatedDt}  
                    };
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<MRPViewModel> GetMRPOHUploadData(IFormFile file)
        {
            MRPViewModel mvm = new();
            List<MRPOHModel> OHList = new();
            List<MRPOHDTO> OHDTOList = new();
            string uploadedFile = file.FileName;
            DateTime dt = DateTime.Now;

            string error_msg = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("MRPOH"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            error_msg = string.Empty;
                            if (count <= header_row) continue; // skip header

                            //Item
                            //OHQty
                            //Location
                            //LocationType

                            string item = CommonFormat.GetValueOrDefault(reader, colMap, "Item", string.Empty);
                            int OHQty = CommonFormat.GetValueOrDefault(reader, colMap, "OHQty", 0);
                            string location = CommonFormat.GetValueOrDefault(reader, colMap, "Location", string.Empty);
                            string locationType = CommonFormat.GetValueOrDefault(reader, colMap, "LocationType", string.Empty);

                            if (string.IsNullOrEmpty(item) && OHQty == 0)
                            {
                                error_msg = SD.ErrorMsg.CheckValue;
                                mvm.UploadedError = true;
                            }

                            MRPOHDTO OHDTO = new()
                            {
                                Item = item,
                                OHQty = OHQty,
                                Location = location,
                                LocationType = locationType,
                                ErrorMsg = error_msg,
                                UploadedFile = uploadedFile,
                                //Remark = "Data from IFS System",
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                CreatedDt = dt
                            };
                            OHDTOList.Add(OHDTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mvm.error_msg = ex.Message;
            }

            if (mvm.UploadedError)
            {
                mvm.OHUploadList = _mapper.Map<List<MRPOHUpload>>(OHDTOList);

            }
            else
            {
                OHList = _mapper.Map<List<MRPOHModel>>(OHDTOList);
                mvm.OHList = OHList;

                mvm.UploadedFile = uploadedFile; 
                mvm.CreatedDt = dt;
            }

            return mvm;
        }

        public async Task<MRPOHModel> MRPOHDetail(int Id)
        {
            return _con.UV_MRP_OH.Find(Id) ?? new();
        }

        public async Task<string> MRPOHAdd(MRPOHModel OH)
        {
            string error_msg = string.Empty;
            try
            {
                //delete old data
                await _con.UV_MRP_OH.Where(i => i.Item == OH.Item && i.Location == OH.Location).ExecuteDeleteAsync();
                OH.CreatedDt = DateTime.Now;
                OH.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";

                //add new one
                await _con.UV_MRP_OH.AddAsync(OH);

                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }

            return error_msg;
        }

        public async Task<string> MRPOHEdit(MRPOHModel OH)
        {
            string error_msg = string.Empty;
            try
            {
                var data = await _con.UV_MRP_OH.FindAsync(OH.Id);
                data.OHQty = OH.OHQty;
                data.Location = OH.Location;
                data.LocationType = OH.LocationType;
                data.UpdatedDt = DateTime.Now;
                data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }
            return error_msg;
        }

        public async Task<string> MRPOHDelete(int Id)
        {
            try
            {
                await _con.UV_MRP_OH.Where(i => i.Id == Id).ExecuteDeleteAsync();
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion


        /*--------------------------------------------------
         MRP SPO : Sheduling Purchase Order
        --------------------------------------------------*/

        #region ------------- MRP SPO ---------------

        public async Task<DataTable> MRPSPOList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_MRP_SPO", dic);
            return dt;
        }

        public async Task<MRPViewModel> MRPSPOUpload(IFormFile file)
        {
            MRPViewModel ppv = new();
            try
            {
                ppv = await GetMRPSPOUploadData(file); 
                
                if (ppv.UploadedError)
                {
                    ppv.Data = _dlm.ToDataTable<MRPSPOUpload>(ppv.SPOUploadList ?? new());
                    ppv.error_msg = SD.ErrorMsg.UploadedError;
                    return ppv;
                }
                else if (ppv.SPOList != null && ppv.SPOList.Count > 0)
                {
                    // delete all existing data 
                    // DO IT LATER, NOW JUST FOR TESTING PURPOSE
                    // Testing just delete by Model
                    var check = ppv.SPOList.Select(i => i.Model).ToList();
                    var existingData = await _con.UV_MRP_SPO
                                        .Where(i => check.Contains(i.Model)).ExecuteDeleteAsync();
                    // add new one
                    await _con.UV_MRP_SPO.AddRangeAsync(ppv.SPOList);
                    await _con.SaveChangesAsync();
                    ppv.Dic = new Dictionary<string, object>()
                    {
                        { "@UploadedFile", ppv.UploadedFile}
                        ,{"@CreatedDt", ppv.CreatedDt} 
                    };
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<MRPViewModel> GetMRPSPOUploadData(IFormFile file)
        {
            MRPViewModel mvm = new();
            List <MRPSPOModel> spoList = new();
            List <MRPSPODTO> spoDTOList  = new();
            string uploadedFile = file.FileName;
            DateTime dt = DateTime.Now;

            string error_msg = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("MRPSPO"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i=>i.db_col_name,i=> i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            error_msg = string.Empty;
                            if (count <= header_row) continue; // skip header

                            //Model
                            //SPOQty

                            string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                            int SPOQty = CommonFormat.GetValueOrDefault(reader, colMap, "SPOQty", 0);
                            DateTime? planShipDt = CommonFormat.GetValueOrDefault(reader, colMap, "PlanShipDt", (DateTime?)null);

                            if (string.IsNullOrEmpty(model) || SPOQty == 0 || planShipDt == null) {
                                error_msg = SD.ErrorMsg.CheckValue;
                                mvm.UploadedError = true;
                            }

                            MRPSPODTO spoDTO = new()
                            {
                                Model = model,
                                SPOQty = SPOQty,
                                PlanShipDt = (DateTime)planShipDt,
                                ErrorMsg = error_msg,
                                UploadedFile = uploadedFile,
                                //Remark = "Data from IFS System",
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                CreatedDt = dt
                            };
                            spoDTOList.Add(spoDTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mvm.error_msg = ex.Message;
            }

            if (mvm.UploadedError)
            {
                mvm.SPOUploadList = _mapper.Map<List<MRPSPOUpload>>( spoDTOList);
                
            }
            else
            {
                spoList = _mapper.Map<List<MRPSPOModel>>(spoDTOList);
                mvm.SPOList = spoList;

                mvm.UploadedFile = uploadedFile; mvm.CreatedDt = dt;
            }

            return mvm;
        }



        public async Task<MRPSPOModel> MRPSPODetail(int Id)
        {
            return _con.UV_MRP_SPO.Find(Id) ?? new();
        }

        public async Task<string> MRPSPOAdd(MRPSPOModel SPO)
        {
            string error_msg = string.Empty;
            try
            {
                //delete old data
                await _con.UV_MRP_SPO.Where(i => i.Model == SPO.Model && i.PlanShipDt == SPO.PlanShipDt).ExecuteDeleteAsync();

                SPO.CreatedDt = DateTime.Now;
                SPO.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";

                //add new one
                await _con.UV_MRP_SPO.AddAsync(SPO);

                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }

            return error_msg;
        }

        public async Task<string> MRPSPOEdit(MRPSPOModel SPO)
        {
            string error_msg = string.Empty;
            try
            {
                var data = await _con.UV_MRP_SPO.FindAsync(SPO.Id);
                data.SPOQty = SPO.SPOQty;
                data.PlanShipDt = SPO.PlanShipDt;
                data.UpdatedDt = DateTime.Now;
                data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
            }
            return error_msg;
        }

        public async Task<string> MRPSPODelete(int Id)
        {
            try
            {
                await _con.UV_MRP_SPO.Where(i => i.Id == Id).ExecuteDeleteAsync();
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        #endregion
    }
}
