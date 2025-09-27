using AutoMapper;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using MESWebDev.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MESWebDev.Services
{
    public class PEService : IPEService
    {
        private readonly AppDbContext _con;
        private readonly IMapper _mapper;
        private readonly Procedure _proc;
        private readonly IHttpContextAccessor _hca;
        public PEService(AppDbContext con, IMapper mapper, IHttpContextAccessor hca)
        {
            _con = con;
            _mapper = mapper;
            _proc = new Procedure(_con);
            _hca = hca;
        }


        //===============>> Manpower <<==================\\
        #region Manpower
        public async Task<string> AddManpower(ManpowerModel mm)
        {
            string msg = string.Empty;
            try
            {
                var check = _con.UV_PE_Manpower_tbl.Where(i => i.UModel == mm.UModel && i.Company == mm.Company && i.UpdatedModelDt == mm.UpdatedModelDt && i.IsActive);
                if (check.Any()) { 
                    _con.RemoveRange(check);
                    await _con.SaveChangesAsync();
                }
                mm.CreatedDt = DateTime.Now;
                mm.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                await _con.UV_PE_Manpower_tbl.AddAsync(mm);
                await _con.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return msg;
            }
            return msg;
        }



        public async Task<string> EditManpower(ManpowerModel mm)
        {
            string msg = string.Empty; 
            
            try
            {
                var m = await _con.UV_PE_Manpower_tbl.FindAsync(mm.Id);
                m.Description = mm.Description;
                m.AssyCost = mm.AssyCost;
                m.SmtCost = mm.SmtCost;
                m.SclCost = mm.SclCost;
                m.InsertCost = mm.InsertCost;
                m.AssyHeadcount = mm.AssyHeadcount;
                m.SmtHeadcount = mm.SmtHeadcount;
                m.SclHeadcount = mm.SclHeadcount;
                m.InsertHeadcount = mm.InsertHeadcount;
                m.AverageCost = mm.AssyHeadcount == 0 ? 0 : mm.AssyCost / mm.AssyHeadcount;
                m.UpdatedModelDt = mm.UpdatedModelDt;

                m.UpdatedDt = DateTime.Now;
                m.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;

                await _con.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return msg;
            }

            return msg;
        }


        public async Task<DataTable> GetManpower(Dictionary<string, object> dic)
        {
            DataTable table = new DataTable();
            table = await _proc.Proc_GetDatatable("spweb_UV_PE_GetManpower", dic);
            return table;
        }

       

        public async Task<PEViewModel> UploadManpower(IFormFile ff)
        {
            PEViewModel pev = new();
            List<ManpowerModel> manpowerList = new List<ManpowerModel>();
            string file_name = $"{ff.FileName}_{DateTime.Now:yyyyMMddHHmmss}";  
            try
            {
                using (var stream = new MemoryStream())
                {
                    await ff.CopyToAsync(stream);

                    // get master upload file information
                    List<UploadFileMaster> ufm = _con.Master_UploadFile_mst.Where(i => i.file_name.Contains("manpower")).ToList();

                    // build dictionary: db_col_name -> col_index
                    Dictionary<string,int> colMap = ufm.ToDictionary(i=>i.db_col_name,i=> i.col_index);

                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int has_header = 0;                    

                    //read excel data 
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            has_header++;
                            if (has_header > header_row)
                            {
                                DataTable dt = new DataTable();
                                string[]? headers = null;
                                DateTime updated_dt = DateTime.Now;
                                string model = reader.GetValue(colMap.GetValueOrDefault("UModel"))?.ToString() ?? string.Empty;
                                string company = reader.GetValue(colMap.GetValueOrDefault("Company"))?.ToString() ?? string.Empty;
                                try
                                {
                                    updated_dt = Convert.ToDateTime(reader.GetValue(colMap.GetValueOrDefault("UpdatedModelDt")));
                                }
                                catch (Exception ex)
                                {
                                    pev.error_msg = $"Uploaded date is wrong format at row: {has_header.ToString()}";
                                    return pev;
                                }
                                if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(company))
                                {
                                    pev.error_msg = $"Model or Company is empty at row: {has_header.ToString()}";
                                    return pev;
                                }

                                var check = _con.UV_PE_Manpower_tbl.Where(i => i.UModel == model && i.Company == company && i.UpdatedModelDt == updated_dt && i.IsActive);
                                if (check.Any())
                                {
                                    _con.UV_PE_Manpower_tbl.RemoveRange(check);
                                    await _con.SaveChangesAsync();
                                }
                                ManpowerModel mm = new ManpowerModel()
                                {
                                    Company = company
                                    ,
                                    UModel = model
                                    ,
                                    BModel = reader.GetValue(colMap.GetValueOrDefault("BModel"))?.ToString() ?? string.Empty
                                    ,
                                    Description = reader.GetValue(colMap.GetValueOrDefault("Description"))?.ToString() ?? string.Empty
                                    ,
                                    SmtHeadcount = reader.GetValue(colMap.GetValueOrDefault("SmtHeadcount")) != null ?
                                                    Convert.ToInt32(reader.GetValue(colMap.GetValueOrDefault("SmtHeadcount"))) : 0
                                    ,
                                    InsertHeadcount = reader.GetValue(colMap.GetValueOrDefault("InsertHeadcount")) != null ?
                                                    Convert.ToInt32(reader.GetValue(colMap.GetValueOrDefault("InsertHeadcount"))) : 0
                                    ,
                                    SclHeadcount = reader.GetValue(colMap.GetValueOrDefault("SclHeadcount")) != null ?
                                                    Convert.ToInt32(reader.GetValue(colMap.GetValueOrDefault("SclHeadcount"))) : 0
                                    ,
                                    AssyHeadcount = reader.GetValue(colMap.GetValueOrDefault("AssyHeadcount")) != null ?
                                                    Convert.ToInt32(reader.GetValue(colMap.GetValueOrDefault("AssyHeadcount"))) : 0
                                    ,
                                    SmtCost = reader.GetValue(colMap.GetValueOrDefault("SmtCost")) != null ?
                                                    Convert.ToDecimal(reader.GetValue(colMap.GetValueOrDefault("SmtCost"))) : 0
                                    ,
                                    InsertCost = reader.GetValue(colMap.GetValueOrDefault("InsertCost")) != null ?
                                                    Convert.ToDecimal(reader.GetValue(colMap.GetValueOrDefault("InsertCost"))) : 0
                                    ,
                                    SclCost = reader.GetValue(colMap.GetValueOrDefault("SclCost")) != null ?
                                                    Convert.ToDecimal(reader.GetValue(colMap.GetValueOrDefault("SclCost"))) : 0
                                    ,
                                    AssyCost = reader.GetValue(colMap.GetValueOrDefault("AssyCost")) != null ?
                                                    Convert.ToDecimal(reader.GetValue(colMap.GetValueOrDefault("AssyCost"))) : 0
                                    ,
                                    UpdatedModelDt = updated_dt
                                    ,
                                    UploadFile = file_name
                                    ,
                                    Note = string.Empty
                                    ,
                                    IsActive = true
                                    ,
                                    CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty
                                    ,
                                    CreatedDt = DateTime.Now
                                };
                                manpowerList.Add(mm);                              

  

                            }
                        }
                                             
                    }

                    if (manpowerList.Count > 0)
                    {
                        // insert to database
                        await _con.UV_PE_Manpower_tbl.AddRangeAsync(manpowerList);
                        await _con.SaveChangesAsync();
                    }
                    else
                    {
                        pev.error_msg = "No data found in the uploaded file.";
                        return pev;
                    }
                }
            }
            catch (Exception ex)
            {
                pev.error_msg = ex.Message;
            }

           
            if(manpowerList.Count > 0)
            {
                pev.data = await GetManpower(new Dictionary<string, object>
                {
                    { "@filename", file_name }
                });
            }
            return pev;
        }



        public async Task<string> DeleteManpower(List<int> ids)
        {
            string msg = string.Empty;
            try
            {
                var user_id = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                var updated_dt = DateTime.Now;
                foreach (var item in ids)
                {
                    ManpowerModel? data = _con.UV_PE_Manpower_tbl.FirstOrDefault(x => x.Id == item);
                    if (data != null)
                    {
                        data.IsActive = false;
                        data.UpdatedBy = user_id;
                        data.UpdatedDt = updated_dt;
                    }
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }


        public async Task<ManpowerModel> GetManpowerDetail(int id)
        {
            return await _con.UV_PE_Manpower_tbl.FindAsync(id) ?? new ManpowerModel();
        }

        #endregion

        //===============>> Operation Master <<==================\\
        #region Operation Master
        public async Task<string> AddOperation(PEViewModel pvm)
        {
            try
            {
                if(pvm !=null )
                {
                    if(pvm.operation != null)
                    {
                        var check = _con.Master_Operation_Hdr.Where(i => i.Name == pvm.operation.Name && i.IsActive);
                        if (check.Any()) { return "Operation name already exists."; }
                        else
                        {
                            pvm.operation.CreatedDt = DateTime.Now;
                            pvm.operation.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                            await _con.Master_Operation_Hdr.AddAsync(pvm.operation);
                            await _con.SaveChangesAsync();
                        }
                    }
                    if(pvm.operationDtlList != null)
                    {
                        var checkExist = _con.Master_Operation_Hdr.Where(i => i.Name == pvm.operation.Name);
                        {
                            if (!checkExist.Any()) { return "Operation name does not exists."; }
                            else
                            {
                                int id = checkExist.FirstOrDefault().Id;
                                foreach (var t in pvm.operationDtlList)
                                {
                                    t.OperationId = id;
                                    t.CreatedDt = DateTime.Now;
                                    t.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                                    await _con.Master_Operation_Dtl.AddAsync(t);
                                }
                                await _con.SaveChangesAsync();
                            }
                        }
                    }
                }

                
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> DeleteOperation(int id)
        {
            try
            {
                var user_id = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                var updated_dt = DateTime.Now;

                OperationModel? data = _con.Master_Operation_Hdr.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    data.IsActive = false;
                    data.UpdatedBy = user_id;
                    data.UpdatedDt = updated_dt;
                }
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteOperationDtl(int id)
        {
            try
            {
                var user_id = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                var updated_dt = DateTime.Now;
                OperationDetailModel? data = _con.Master_Operation_Dtl.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    data.IsActive = false;
                    data.UpdatedBy = user_id;
                    data.UpdatedDt = updated_dt;
                }
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<List<OperationModel>> GetOperation()
        {
            //spweb_UV_PE_Master_Operation
            List<OperationModel> list = _con.Master_Operation_Hdr.Where(i => i.IsActive == true).OrderByDescending(i=>i.CreatedDt).ToList();
            return list;
            //DataSet ds = new DataSet();
            //ds = await _proc.Proc_GetDataset("spweb_UV_PE_Master_Operation", dic);
           // return ds;
        }

        public async Task<string> EditOperation(PEViewModel pvm)
        {
            try
            {
                if (pvm != null)
                {
                    if (pvm.operation != null)
                    {
                        var check = _con.Master_Operation_Hdr.Where(i => i.Id == pvm.operation.Id && i.IsActive);
                        if (!check.Any()) { return "Operation name does not exists."; }
                        else
                        {
                            var op = check.FirstOrDefault();
                            op.Name = pvm.operation.Name;
                            op.Description = pvm.operation.Description;
                            op.UpdatedDt = DateTime.Now;
                            op.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                            await _con.SaveChangesAsync();
                        }
                    }
                    if (pvm.operationDtlList != null)
                    {
                        foreach(var t in pvm.operationDtlList)
                        {
                            var check = _con.Master_Operation_Dtl.Where(i => i.Id == t.Id && i.IsActive);
                            if (check.Any())
                            {
                                var opd = check.FirstOrDefault();
                                opd.Name = t.Name;
                                opd.Description = t.Description;
                                opd.UpdatedDt = DateTime.Now;
                                opd.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;                                
                            }
                            else
                            {
                                t.OperationId = pvm.operation.Id;
                                t.CreatedDt = DateTime.Now;
                                t.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                                await _con.Master_Operation_Dtl.AddAsync(t);
                            }
                            await _con.SaveChangesAsync();
                        }   
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<PEViewModel> GetOperationDetail(int id)
        {
            OperationModel om = await _con.Master_Operation_Hdr.FindAsync(id);
            List<OperationDetailModel> odm = new();
                
            var check =  _con.Master_Operation_Dtl.Where(i => i.OperationId == id && i.IsActive);
            if (check.Any())
            {
                odm = check.ToList();
            }

            return new PEViewModel
            {
                operation = om,
                operationDtlList = odm
            };
        }
        #endregion

        //===============>> Operation Detail Master <<==================\\
        #region Operation Detail Master
        public Task<string> AddOperationDtl(OperationDetailModel odm)
        {
            throw new NotImplementedException();
        }       

        //public Task<string> DeleteOperationDtl(List<int> ids)
        //{
        //    throw new NotImplementedException();
        //}


        public Task<PEViewModel> UploadOperationDtl(IFormFile f)
        {
            throw new NotImplementedException();
        } 
        public async Task<List<OperationDetailDTO>> GetOperationDtl()
        {
            List<OperationDetailDTO> list = new();
            var data = _con.Master_Operation_Dtl.Include(i=>i.OperationMaster).Where(i => i.IsActive && i.OperationMaster.IsActive);
            if(data.Any())
            {
                list = _mapper.Map<List<OperationDetailDTO>>(data);
            }
            return list;
        }

        public Task<OperationDetailModel> GetOperationDtlDetail(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditOperationDtl(OperationDetailModel odm)
        {
            throw new NotImplementedException();
        }
        #endregion

        //===============>> Time Study <<==================\\
        #region Time Study
        public async Task<string> AddTimeStudy(PEViewModel pev)
        {
            try
            {
                // 1. Map and add header
                TimeStudyHdrModel thm = _mapper.Map<TimeStudyHdrModel>(pev.TimeStudyHdr);
                thm.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                thm.CreatedDt = DateTime.Now;

                await _con.UV_PE_TimeStudy_Hdr.AddAsync(thm);
                await _con.SaveChangesAsync();  // <-- This will populate thm.Id with the generated PK

                int id_check = thm.Id;
                // 2. Now thm.Id is available
                List<TimeStudyDtlModel> tdm = pev.timeStudyList
                    .GroupBy(i => new { i.OperationKind, i.StepNo, i.StepContent, i.UnitQty, i.AllocatedOpr })
                    .Select(g => new TimeStudyDtlModel
                    {
                        ParentId = thm.Id,  // Now this has a valid value
                        OperationKind = g.Key.OperationKind,
                        StepNo = g.Key.StepNo,
                        StepContent = g.Key.StepContent,
                        UnitQty = g.Key.UnitQty,
                        AllocatedOpr = g.Key.AllocatedOpr,
                        Sumary = g.Sum(i => i.TimeAvg),
                        CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                        CreatedDt = DateTime.Now,
                    })
                    .ToList();

                // 3. Add details
                await _con.UV_PE_TimeStudy_Dtl.AddRangeAsync(tdm);
                await _con.SaveChangesAsync();


                var check = tdm.FirstOrDefault(t => t.ParentId == thm.Id).Id;
                // 4. 
                List<TimeStudyStepDtlModel> tsdm = pev.timeStudyList
                    .Select(i => new TimeStudyStepDtlModel
                    {
                        StepId = tdm.FirstOrDefault(t => t.ParentId == thm.Id && t.StepNo == i.StepNo).Id,
                        SeqNo = i.SeqNo,
                        OperationName = i.OperationName,
                        OperationDetailName = i.OperationDetailName,
                        Time01 = i.Time01,
                        Time02 = i.Time02,
                        Time03 = i.Time03,
                        Time04 = i.Time04,
                        Time05 = i.Time05,
                        TimeAvg = i.TimeAvg,
                        CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                        CreatedDt = DateTime.Now,
                    })
                    .ToList();
                await _con.UV_PE_TimeStudyStep_Dtl.AddRangeAsync(tsdm);
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }            

        }
        public Task<string> DeleteTimeStudy(List<int> ids)
        {
            throw new NotImplementedException();
        }
        public Task<PEViewModel> UploadTimeStudy(IFormFile f)
        {
            throw new NotImplementedException();
        }
        public async Task<DataTable> GetTimeStudy(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_PE_InputTimeStudy_List", dic);
            return dt;
        }
        public Task<OperationDetailModel> GetTimeStudyDetail(int id)
        {
            throw new NotImplementedException();
        }
        public Task<string> EditTimeStudy(OperationDetailModel odm)
        {
            throw new NotImplementedException();
        }

        public async Task<DataSet> IniTimeStudy(Dictionary<string, object> dic)
        {
            DataSet ds = await _proc.Proc_GetDataset("spweb_UV_PE_InputTimeStudy_Initial", dic);
            return ds;
        }

        public async Task<PEViewModel> IniTimeStudyDetail(string? operationName)
        {
            PEViewModel pvm = new PEViewModel();
            var opr = _con.Master_Operation_Hdr.Where(i => i.Name.Contains(operationName??string.Empty));
            if (opr.Any()) {
                pvm.operationNames = opr.Select(i => i.Name).ToList();
                if (!string.IsNullOrEmpty(operationName))
                {
                    var oprd = _con.Master_Operation_Dtl.Where(i => i.OperationId == opr.FirstOrDefault().Id && i.IsActive);
                    if (oprd.Any())
                    {
                        pvm.operationDtlNames = oprd.Select(i => i.Name).ToList();
                    }
                }
                
            } 
            pvm.operationDtlNames = pvm.operationDtlNames ?? new();
            return pvm;
        }
        #endregion

    }
}
