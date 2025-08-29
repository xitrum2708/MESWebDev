using AutoMapper;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.PE;
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

        public async Task<string> DeleteManpower(List<int> ids)
        {
            string msg = string.Empty;
            try
            {
                foreach (var item in ids)
                {
                    ManpowerModel? data = _con.UV_PE_Manpower_tbl.FirstOrDefault(x => x.Id == item);
                    if (data != null)
                    {
                        data.IsActive = false;
                        data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                        data.UpdatedDt = DateTime.Now;
                        _con.UV_PE_Manpower_tbl.Update(data);
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

        public async Task<DataTable> GetManpower(Dictionary<string, object> dic)
        {
            DataTable table = new DataTable();
            table = await _proc.Proc_GetDatatable("PE_GetManpower", dic);
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
    }
}
