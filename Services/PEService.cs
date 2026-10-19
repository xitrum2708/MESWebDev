using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;

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

        #region ProdCapacity

        public async Task<DataTable> GetProdCapacity(Dictionary<string, object> dic)
        {
            return await _proc.Proc_GetDatatable("spweb_UV_PE_GetProdCapacity", dic);
        }

        public async Task<ProdCapacityModel> GetProdCapacityDetail(int id)
        {
            return await _con.UV_PE_ProdCapacity_tbl
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive)
                ?? new ProdCapacityModel();
        }

        public async Task<string> AddProdCapacity(ProdCapacityModel model)
        {
            try
            {
                if (model == null)
                    return "Input model is null.";

                var validation = ValidateProdCapacity(model);
                if (validation != null)
                    return validation;

                var previous = await _con.UV_PE_ProdCapacity_tbl
                    .Where(x => x.IsActive
                        && x.Type == model.Type
                        && x.Customer1 == model.Customer1
                        && x.Customer2 == model.Customer2
                        && x.UNo == model.UNo
                        && x.BModel == model.BModel
                        && x.PCBNo == model.PCBNo
                        && x.PCBType == model.PCBType
                        && x.UpdateDate == model.UpdateDate)
                    .ToListAsync();

                await using var transaction = await _con.Database.BeginTransactionAsync();

                _con.UV_PE_ProdCapacity_tbl.RemoveRange(previous);

                model.Id = 0;
                model.IsActive = true;
                model.CreatedDt = DateTime.Now;
                model.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;

                await _con.UV_PE_ProdCapacity_tbl.AddAsync(model);
                await _con.SaveChangesAsync();
                await transaction.CommitAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> EditProdCapacity(ProdCapacityModel model)
        {
            try
            {
                if (model == null)
                    return "Input model is null.";

                var validation = ValidateProdCapacity(model);
                if (validation != null)
                    return validation;

                var current = await _con.UV_PE_ProdCapacity_tbl
                    .FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive);

                if (current == null)
                    return "Production capacity record was not found.";

                ApplyProdCapacityValues(current, model);
                current.UpdatedDt = DateTime.Now;
                current.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteProdCapacity(List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return string.Empty;

                var records = await _con.UV_PE_ProdCapacity_tbl
                    .Where(x => x.IsActive && ids.Contains(x.Id))
                    .ToListAsync();

                var user = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                var now = DateTime.Now;
                foreach (var record in records)
                {
                    record.IsActive = false;
                    record.UpdatedDt = now;
                    record.UpdatedBy = user;
                }

                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<PEViewModel> UploadProdCapacity(IFormFile file)
        {
            var result = new PEViewModel();

            if (file == null || file.Length == 0)
            {
                result.error_msg = "Please select a non-empty Excel file.";
                return result;
            }

            if (!new[] { ".xlsx", ".xls" }.Contains(
                    Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
            {
                result.error_msg = "Only .xlsx and .xls files are supported.";
                return result;
            }

            const long maxUploadBytes = 20 * 1024 * 1024;
            if (file.Length > maxUploadBytes)
            {
                result.error_msg = "The Excel file must be 20 MB or smaller.";
                return result;
            }

            try
            {
                // Exact file_name comparison: prevents loading config of other modules.
                var configs = await _con.Master_UploadFile_mst.AsNoTracking()
                    .Where(x => x.file_name == "PEProdCapacity")
                    .OrderBy(x => x.col_index)
                    .ToListAsync();

                if (configs.Count == 0)
                {
                    result.error_msg = "PEProdCapacity upload mapping is not configured.";
                    return result;
                }

                var colMap = configs.ToDictionary(
                    x => x.db_col_name,
                    x => x.col_index,
                    StringComparer.OrdinalIgnoreCase);

                string[] requiredColumns =
                {
             "Type", "Customer1", "Customer2", "UNo", "BModel", "PCBNo",
             "PCBType", "UpdateDate", "SMTPointsTotal", "COBHC", "COBOutput",
             "SMTHC", "SMTOutput", "InsertHC", "InsertOutput", "SCHC",
             "SCOutput", "AssyHC", "AssyOutput", "MicHC", "MicOutput",
             "OtherHC", "OtherOutput", "SMTTop", "SMTBottom", "SMTTotal", "Insert"
         };

                var missing = requiredColumns.Where(x => !colMap.ContainsKey(x)).ToArray();
                if (missing.Length > 0)
                {
                    result.error_msg = "Missing upload mapping: " + string.Join(", ", missing);
                    return result;
                }

                int headerRow = configs[0].header_row; // Usually 1 (first row is column names).
                var rows = new List<ProdCapacityModel>();
                // Keep UploadFile within common NVARCHAR(100) database constraints.
                var safeOriginalName = Path.GetFileName(file.FileName);
                if (safeOriginalName.Length > 70)
                    safeOriginalName = safeOriginalName.Substring(0, 70);
                var uploadName = $"{safeOriginalName}_{DateTime.Now:yyyyMMddHHmmssfff}";
                var user = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                var now = DateTime.Now;

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0; // Important after CopyToAsync.

                // Reads ONLY worksheet 1, which must be the flat "Upload Data" sheet.
                using var reader = ExcelReaderFactory.CreateReader(stream);
                int excelRow = 0;
                while (reader.Read())
                {
                    excelRow++;
                    if (excelRow <= headerRow)
                        continue;

                    object? Raw(string column)
                    {
                        if (!colMap.TryGetValue(column, out int index))
                            return null; // Optional column (PcsPerSheet) not configured.
                        if (index < 0 || index >= reader.FieldCount)
                            return null;
                        return reader.GetValue(index);
                    }

                    string Txt(string column) => Convert.ToString(Raw(column), CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;
                    int Int(string column) => ParseProdCapacityInt(Raw(column), excelRow, column);
                    int Output(string column) => ParseProdCapacityOutputInt(Raw(column), excelRow, column);
                    decimal Dec(string column) => ParseProdCapacityDecimal(Raw(column), excelRow, column);

                    // Skip entirely blank rows, not rows where numeric values happen to be 0.
                    if (requiredColumns.All(x => IsBlankProdCapacityCell(Raw(x)))
                        && IsBlankProdCapacityCell(Raw("PcsPerSheet")))
                        continue;

                    var model = new ProdCapacityModel
                    {
                        Type = Txt("Type"),
                        Customer1 = Txt("Customer1"),
                        Customer2 = Txt("Customer2"),
                        UNo = Txt("UNo"),
                        BModel = Txt("BModel"),
                        PCBNo = Txt("PCBNo"),
                        PCBType = Txt("PCBType"),
                        UpdateDate = ParseProdCapacityDate(Raw("UpdateDate"), excelRow),
                        // Match decimal(18,4) in the current entity/database model.
                        SMTPointsTotal = Math.Round(Dec("SMTPointsTotal"), 4, MidpointRounding.AwayFromZero),
                        COBHC = Dec("COBHC"),
                        COBOutput = Output("COBOutput"),
                        SMTHC = Dec("SMTHC"),
                        SMTOutput = Output("SMTOutput"),
                        InsertHC = Dec("InsertHC"),
                        InsertOutput = Output("InsertOutput"),
                        SCHC = Dec("SCHC"),
                        SCOutput = Output("SCOutput"),
                        AssyHC = Dec("AssyHC"),
                        AssyOutput = Output("AssyOutput"),
                        MicHC = Dec("MicHC"),
                        MicOutput = Output("MicOutput"),
                        OtherHC = Dec("OtherHC"),
                        OtherOutput = Output("OtherOutput"),
                        SMTTop = Int("SMTTop"),
                        SMTBottom = Int("SMTBottom"),
                        SMTTotal = Int("SMTTotal"),
                        Insert = Int("Insert"),
                        PcsPerSheet = Int("PcsPerSheet"),
                        UploadFile = uploadName,
                        IsActive = true,
                        CreatedDt = now,
                        CreatedBy = user
                    };

                    var validation = ValidateProdCapacity(model);
                    if (validation != null)
                    {
                        result.error_msg = $"Row {excelRow}: {validation}";
                        return result;
                    }

                    rows.Add(model);
                }

                if (rows.Count == 0)
                {
                    result.error_msg = "No data found in the uploaded Excel file.";
                    return result;
                }

                var duplicate = rows.Where(i => i.UpdateDate != null).GroupBy(ProdCapacityKey, StringComparer.OrdinalIgnoreCase)
                    //.Select(ProdCapacityKey)
                    .FirstOrDefault(x => x.Count() > 1);
                if (duplicate != null)
                {
                    result.error_msg = $"Excel contains duplicate rows for the same product key and UpdateDate: {duplicate.Key}.";
                    return result;
                }

                // Full-file validation finishes BEFORE modifying the database.
                // Query likely candidates once instead of querying once per Excel row.
                var unos = rows.Select(x => x.UNo!).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var importedKeys = new HashSet<string>(
                    rows.Select(ProdCapacityKey), StringComparer.OrdinalIgnoreCase);

                await using (var transaction = await _con.Database.BeginTransactionAsync())
                {
                    var candidates = new List<ProdCapacityModel>();
                    const int batchSize = 500; // Avoid excessive SQL IN parameters.
                    for (int offset = 0; offset < unos.Count; offset += batchSize)
                    {
                        var batch = unos.Skip(offset).Take(batchSize).ToList();
                        var batchRows = await _con.UV_PE_ProdCapacity_tbl
                            .Where(x => x.IsActive && x.UNo != null && batch.Contains(x.UNo))
                            .ToListAsync();
                        candidates.AddRange(batchRows);
                    }

                    var oldVersions = candidates
                        .Where(x => importedKeys.Contains(ProdCapacityKey(x)))
                        .ToList();

                    _con.UV_PE_ProdCapacity_tbl.RemoveRange(oldVersions);
                    await _con.UV_PE_ProdCapacity_tbl.AddRangeAsync(rows);
                    await _con.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                result.data = await GetProdCapacity(new Dictionary<string, object>
                {
                    ["@filename"] = uploadName
                });
            }
            catch (Exception ex)
            {
                result.error_msg = ex.Message;
            }

            return result;
        }

        public async Task<byte[]> ExportProdCapacityExcel(DataTable table)
        {
            //var table = await GetProdCapacity(dic);
            using var workbook = new XLWorkbook();
            BuildProdCapacityReportSheet(workbook, table);
            BuildProdCapacityUploadSheet(workbook, table);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // --------- for Search Filter Options ----------------
        public async Task<List<ProdCapacitySearchCriteria>> GetProdCapacityFilterOptions()
        {
            return await _con.UV_PE_ProdCapacity_tbl
                .AsNoTracking()
                .Where(x => x.IsActive
                    && x.Type != null
                    && x.Type.Trim() != "")
                .Select(x => new ProdCapacitySearchCriteria
                {
                    Type = x.Type!.Trim(),
                    Customer1 = x.Customer1 == null
                        ? ""
                        : x.Customer1.Trim()
                })
                .Distinct()
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Customer1)
                .ToListAsync();
        }

        // ----------------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------------

        private static string? ValidateProdCapacity(ProdCapacityModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Type)) return "Type is required.";
            if (string.IsNullOrWhiteSpace(model.UNo)) return "UNo is required.";
            //if (string.IsNullOrWhiteSpace(model.BModel)) return "BModel is required.";
            //if (string.IsNullOrWhiteSpace(model.PCBNo)) return "PCBNo is required.";
            //if (string.IsNullOrWhiteSpace(model.PCBType)) return "PCBType is required.";
            return null;
        }

        private static string ProdCapacityKey(ProdCapacityModel m) => string.Join("\u001F",
            (m.Type ?? "").Trim().ToUpperInvariant(),
            (m.Customer1 ?? "").Trim().ToUpperInvariant(),
            (m.Customer2 ?? "").Trim().ToUpperInvariant(),
            (m.UNo ?? "").Trim().ToUpperInvariant(),
            (m.BModel ?? "").Trim().ToUpperInvariant(),
            (m.PCBNo ?? "").Trim().ToUpperInvariant(),
            (m.PCBType ?? "").Trim().ToUpperInvariant(),
            m.UpdateDate?.Ticks.ToString(CultureInfo.InvariantCulture) ?? "NULL");

        private static bool IsBlankProdCapacityCell(object? value) =>
            value == null || value == DBNull.Value ||
            (value is string text && string.IsNullOrWhiteSpace(text));

        private decimal ParseProdCapacityDecimal2(object value, int excelRow, string column)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            if (value is decimal dec)
                return dec;

            if (value is double dbl)
                return Convert.ToDecimal(dbl);

            if (value is float flt)
                return Convert.ToDecimal(flt);

            if (value is int i)
                return i;

            if (value is long l)
                return l;

            string text = Convert.ToString(
                value, CultureInfo.InvariantCulture)?.Trim();

            if (decimal.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }

            throw new Exception(
                $"Row {excelRow}, Column {column}: " +
                $"Invalid decimal value '{text}'.");
        }

        private static decimal ParseProdCapacityDecimal(object? value, int row, string column)
        {
            if (IsBlankProdCapacityCell(value)) return 0m;
            try
            {
                if (value is not string)
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);

                var text = ((string)value).Trim();
                // Prefer an explicit decimal separator before allowing thousands separators.
                // This prevents "12.5" being interpreted as 125 under a vi-VN culture.
                if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                    || decimal.TryParse(text, NumberStyles.Float, CultureInfo.GetCultureInfo("vi-VN"), out number)
                    || decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out number)
                    || decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
                    return number;
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                // Wrapped below with Excel row and column.
            }
            throw new FormatException($"Invalid decimal at row {row}, column {column}: '{value}'.");
        }

        // Output fields accept decimal Excel cells and round halves away from zero.
        // Examples: 12.49 -> 12, 12.50 -> 13, -12.50 -> -13.
        private static int ParseProdCapacityOutputInt(object? value, int row, string column)
        {
            var number = ParseProdCapacityDecimal(value, row, column);
            return RoundProdCapacityToInt(number, $"Row {row}, column {column}");
        }

        private static int RoundProdCapacityToInt(decimal number, string source)
        {
            var rounded = Math.Round(number, 0, MidpointRounding.AwayFromZero);
            if (rounded < int.MinValue || rounded > int.MaxValue)
                throw new OverflowException($"{source}: rounded value {rounded} is outside Int32 range.");

            return (int)rounded;
        }

        // Keep strict validation for SMTTop, SMTBottom, SMTTotal, Insert, PcsPerSheet.
        private static int ParseProdCapacityInt(object? value, int row, string column)
        {
            if (IsBlankProdCapacityCell(value)) return 0;
            var number = ParseProdCapacityDecimal(value, row, column);
            if (number != decimal.Truncate(number)
                || number < int.MinValue || number > int.MaxValue)
                throw new FormatException($"Invalid integer at row {row}, column {column}: '{value}'.");
            return (int)number;
        }

        private static DateTime? ParseProdCapacityDate(object? value, int row)
        {
            if (IsBlankProdCapacityCell(value)) return null;
            if (value is DateTime dt) return dt;

            if (value is double serial)
            {
                try { return DateTime.FromOADate(serial); }
                catch (ArgumentException) { /* handled below */ }
            }

            var text = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (DateTime.TryParse(text, CultureInfo.GetCultureInfo("vi-VN"),
                    DateTimeStyles.None, out var parsed)
                || DateTime.TryParse(text, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out parsed))
                return parsed;

            throw new FormatException($"Invalid date at row {row}, column UpdateDate: '{value}'.");
        }

        private static void ApplyProdCapacityValues(ProdCapacityModel dst, ProdCapacityModel src)
        {
            dst.Type = src.Type;
            dst.Customer1 = src.Customer1;
            dst.Customer2 = src.Customer2;
            dst.UNo = src.UNo;
            dst.BModel = src.BModel;
            dst.PCBNo = src.PCBNo;
            dst.PCBType = src.PCBType;
            dst.UpdateDate = src.UpdateDate;
            dst.SMTPointsTotal = src.SMTPointsTotal;
            dst.COBHC = src.COBHC;
            dst.COBOutput = src.COBOutput;
            dst.SMTHC = src.SMTHC;
            dst.SMTOutput = src.SMTOutput;
            dst.InsertHC = src.InsertHC;
            dst.InsertOutput = src.InsertOutput;
            dst.SCHC = src.SCHC;
            dst.SCOutput = src.SCOutput;
            dst.AssyHC = src.AssyHC;
            dst.AssyOutput = src.AssyOutput;
            dst.MicHC = src.MicHC;
            dst.MicOutput = src.MicOutput;
            dst.OtherHC = src.OtherHC;
            dst.OtherOutput = src.OtherOutput;
            dst.SMTTop = src.SMTTop;
            dst.SMTBottom = src.SMTBottom;
            dst.SMTTotal = src.SMTTotal;
            dst.Insert = src.Insert;
            dst.PcsPerSheet = src.PcsPerSheet;
        }

        private static readonly string[] ProdCapacityDbColumns =
        {
     "Type", "Customer1", "Customer2", "UNo", "BModel", "PCBNo",
     "PCBType", "UpdateDate", "SMTPointsTotal", "COBHC", "COBOutput",
     "SMTHC", "SMTOutput", "InsertHC", "InsertOutput", "SCHC",
     "SCOutput", "AssyHC", "AssyOutput", "MicHC", "MicOutput",
     "OtherHC", "OtherOutput", "SMTTop", "SMTBottom", "SMTTotal",
     "Insert", "PcsPerSheet"
 };

        private static string CapacityNumberFormat(
                                decimal value,
                                int maxDecimals = 6)
        {
            decimal rounded = Math.Round(
                value,
                maxDecimals,
                MidpointRounding.AwayFromZero);

            string text = rounded.ToString(
                "0." + new string('#', maxDecimals),
                CultureInfo.InvariantCulture);

            int dotIndex = text.IndexOf('.');

            if (dotIndex < 0)
                return "#,##0";

            int decimalPlaces = text.Length - dotIndex - 1;

            return decimalPlaces == 0
                ? "#,##0"
                : "#,##0." + new string('0', decimalPlaces);
        }

        private static void SetCapacityDecimal(
            IXLWorksheet ws,
            int row,
            int col,
            decimal value)
        {
            SetDecimal(
                ws,
                row,
                col,
                value,
                CapacityNumberFormat(value));
        }

        private static void BuildProdCapacityReportSheet(
            XLWorkbook workbook,
            DataTable data)
        {
            const int groupRow = 1;
            const int headerRow = 2;
            const int firstDataRow = 3;

            // ==================== HEADERS ====================

            string[] headings =
            {
         "Type", "Customer 1", "Customer 2", "U-No",
         "B-Model", "PCB No", "PCB Type",
         "Update Date", "FY", "VersionStatus", "SMT Points",
         "SMT", "Insert", "SC", "Assy", "MIC", "Other", "TOTAL",
         "COB HC", "COB Output",
         "SMT HC", "SMT Output",
         "Insert HC", "Insert Output",
         "SC HC", "SC Output",
         "Assy HC", "Assy Output",
         "MIC HC", "MIC Output",
         "Other HC", "Other Output",
         "TOP", "Bottom", "TTL", "Insert", "Pcs / Sheet"
     };

            int lastCol = headings.Length;

            // Ensure SQL returns calculated values.
            string[] manDayFields =
            {
         "MdSMT",
         "MdInsert",
         "MdSC",
         "MdAssy",
         "MdMic",
         "MdOther",
         "MdTotal"
     };

            string[] requiredFields =
            {
         "FY",
         "VersionStatus",
         "SMTPointsTotal"
     };

            foreach (string field in requiredFields.Concat(manDayFields))
            {
                if (!data.Columns.Contains(field))
                    throw new ArgumentException(
                        $"Missing column '{field}' in Production Capacity data.");
            }

            // ==================== WORKSHEET ====================

            var ws = workbook.Worksheets.Add("Capacity Report");
            ws.ShowGridLines = false;

            var navy = XLColor.FromHtml("#14243A");
            var teal = XLColor.FromHtml("#087E8B");
            var paleTeal = XLColor.FromHtml("#E8F6F4");
            var gray = XLColor.FromHtml("#64748B");
            var alternate = XLColor.FromHtml("#F6F9FC");

            // ==================== GROUP HEADER ====================

            var groups = new (string title, int from, int to, string hex)[]
            {
         ("PRODUCT INFORMATION", 1, 7, "#263A52"),
         ("PERIOD / POINTS", 8, 11, "#475569"),
         ("MAN-DAY / PERSON-DAYS PER PIECE", 12, 18, "#087E8B"),
         ("COB", 19, 20, "#263A52"),
         ("SMT", 21, 22, "#263A52"),
         ("INSERT", 23, 24, "#263A52"),
         ("SC", 25, 26, "#263A52"),
         ("ASSEMBLY", 27, 28, "#263A52"),
         ("MIC", 29, 30, "#263A52"),
         ("OTHER", 31, 32, "#263A52"),
         ("SMT / PCB SIDE", 33, 35, "#445D75"),
         ("INSERT", 36, 36, "#445D75"),
         ("SHEET", 37, 37, "#445D75")
            };

            ws.Row(groupRow).Height = 29;

            foreach (var group in groups)
            {
                var range = ws.Range(
                    groupRow, group.from,
                    groupRow, group.to);

                if (group.from != group.to)
                    range.Merge();

                range.FirstCell().Value = group.title;

                range.Style.Fill.BackgroundColor =
                    XLColor.FromHtml(group.hex);

                range.Style.Font.FontColor = XLColor.White;
                range.Style.Font.FontSize = 10;
                range.Style.Font.Bold = true;

                range.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                range.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;
            }

            // ==================== COLUMN HEADER ====================

            ws.Row(headerRow).Height = 38;

            for (int c = 1; c <= lastCol; c++)
            {
                var cell = ws.Cell(headerRow, c);

                cell.Value = headings[c - 1];

                cell.Style.Fill.BackgroundColor =
                    c >= 12 && c <= 18
                        ? XLColor.FromHtml("#D2EEE9")
                        : XLColor.FromHtml("#E8EDF4");

                cell.Style.Font.FontColor = navy;
                cell.Style.Font.FontSize = 10;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontName = "Aptos";

                cell.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                cell.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                cell.Style.Alignment.WrapText = true;

                cell.Style.Border.BottomBorder =
                    XLBorderStyleValues.Medium;

                cell.Style.Border.BottomBorderColor = teal;
            }

            // ==================== SOURCE FIELDS ====================

            string[] productFields =
            {
         "Type", "Customer1", "Customer2",
         "UNo", "BModel", "PCBNo", "PCBType"
     };

            string[] metricFields =
            {
         "COBHC", "COBOutput",
         "SMTHC", "SMTOutput",
         "InsertHC", "InsertOutput",
         "SCHC", "SCOutput",
         "AssyHC", "AssyOutput",
         "MicHC", "MicOutput",
         "OtherHC", "OtherOutput",
         "SMTTop", "SMTBottom", "SMTTotal",
         "Insert", "PcsPerSheet"
     };

            // ==================== DATA ====================

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow record = data.Rows[i];
                int r = firstDataRow + i;

                ws.Row(r).Height = 25;

                var line = ws.Range(r, 1, r, lastCol);

                line.Style.Fill.BackgroundColor =
                    i % 2 == 0 ? XLColor.White : alternate;

                line.Style.Font.FontName = "Aptos";
                line.Style.Font.FontSize = 10;
                line.Style.Font.FontColor = navy;

                line.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                line.Style.Border.BottomBorder =
                    XLBorderStyleValues.Hair;

                line.Style.Border.BottomBorderColor =
                    XLColor.FromHtml("#E5ECF1");

                // ================= PRODUCT A:G =================

                for (int c = 1; c <= productFields.Length; c++)
                {
                    ws.Cell(r, c).Value =
                        DataText(record, productFields[c - 1]);
                }

                ws.Cell(r, 4).Style.Font.Bold = true;
                ws.Cell(r, 5).Style.Font.Bold = true;

                // ================= UPDATE DATE H =================

                var date = DataDate(record, "UpdateDate");

                if (date.HasValue)
                {
                    var dateCell = ws.Cell(r, 8);

                    dateCell.Value = date.Value;
                    dateCell.Style.DateFormat.Format = "yyyy/MM/dd";
                }

                // ================= FY I =================
                // Value calculated by SQL.

                ws.Cell(r, 9).Value = DataText(record, "FY");

                // ================= VERSION STATUS J =================

                string version = DataText(record, "VersionStatus").Trim();

                string versionText = version switch
                {
                    "1" => "Latest",
                    "2" => "Others",
                    _ => version
                };

                var versionCell = ws.Cell(r, 10);

                versionCell.Value = versionText;
                versionCell.Style.Font.Bold = true;

                if (versionText.Equals(
                    "Latest", StringComparison.OrdinalIgnoreCase))
                {
                    versionCell.Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#DCFCE7");

                    versionCell.Style.Font.FontColor =
                        XLColor.FromHtml("#166534");
                }
                else if (versionText.Equals(
                    "Others", StringComparison.OrdinalIgnoreCase))
                {
                    versionCell.Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#F1F5F9");

                    versionCell.Style.Font.FontColor =
                        XLColor.FromHtml("#64748B");
                }

                // ================= SMT POINTS K =================

                SetCapacityDecimal(
                    ws, r, 11,
                    DataDecimal(record, "SMTPointsTotal"));

                // ================= MAN-DAY L:R =================
                // Use values calculated by SQL.
                // No Excel formula required.

                for (int c = 12; c <= 18; c++)
                {
                    string field = manDayFields[c - 12];

                    decimal value = DataDecimal(record, field);

                    SetCapacityDecimal(ws, r, c, value);
                }

                // Highlight Total Man-day.
                var totalCell = ws.Cell(r, 18);

                totalCell.Style.Fill.BackgroundColor = paleTeal;
                totalCell.Style.Font.Bold = true;
                totalCell.Style.Font.FontColor =
                    XLColor.FromHtml("#065E68");

                // ================= HC / OUTPUT S:AF =================
                // Odd columns 19,21,...,31 are decimal HC; even columns are integer Output.

                for (int c = 19; c <= 32; c++)
                {
                    string field = metricFields[c - 19];

                    if (c % 2 == 1)
                        SetCapacityDecimal(ws, r, c, DataDecimal(record, field));
                    else
                        SetInt(ws, r, c, DataInt(record, field));
                }

                // ================= PCB / LAYOUT AG:AK =================
                // Keep existing integer fields.

                for (int c = 33; c <= 37; c++)
                {
                    string field = metricFields[c - 19];

                    SetInt(
                        ws, r, c,
                        DataInt(record, field));
                }

                // ================= ALIGNMENT =================

                ws.Range(r, 8, r, lastCol)
                    .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;
            }

            // ==================== EMPTY DATA ====================

            if (data.Rows.Count == 0)
            {
                ws.Range(
                    firstDataRow, 1,
                    firstDataRow, lastCol).Merge();

                ws.Cell(firstDataRow, 1).Value =
                    "No production capacity records found.";

                ws.Cell(firstDataRow, 1).Style.Font.FontColor = gray;

                ws.Cell(firstDataRow, 1)
                    .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                ws.Row(firstDataRow).Height = 31;
            }
            else
            {
                int lastDataRow = firstDataRow + data.Rows.Count - 1;

                ws.Range(
                    headerRow, 1,
                    lastDataRow, lastCol).SetAutoFilter();
            }

            // ==================== COLUMN WIDTH ====================

            for (int c = 1; c <= lastCol; c++)
            {
                ws.Column(c).Width = c <= 7 ? 16 : 14;
            }

            ws.Column(2).Width = 18;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 18;
            ws.Column(6).Width = 19;
            ws.Column(7).Width = 16;

            ws.Column(8).Width = 17;   // Update Date
            ws.Column(9).Width = 11;   // FY
            ws.Column(10).Width = 18;  // VersionStatus
            ws.Column(11).Width = 17;  // SMT Points
            ws.Column(18).Width = 17;  // Total Man-day

            foreach (int c in new[] { 20, 22, 24, 26, 28, 30, 32 })
            {
                ws.Column(c).Width = 18;
            }

            ws.Column(37).Width = 18;

            // ==================== FREEZE ====================

            ws.SheetView.FreezeRows(headerRow);
            ws.SheetView.FreezeColumns(7);

            // ==================== PAGE SETUP ====================

            ws.PageSetup.PageOrientation =
                XLPageOrientation.Landscape;
        }


        private static void BuildProdCapacityUploadSheet(XLWorkbook workbook, DataTable data)
        {
            var ws = workbook.Worksheets.Add("Upload Data");
            ws.ShowGridLines = false;
            for (int c = 1; c <= ProdCapacityDbColumns.Length; c++)
            {
                var header = ws.Cell(1, c);
                header.Value = ProdCapacityDbColumns[c - 1];
                header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E344F");
                header.Style.Font.FontColor = XLColor.White;
                header.Style.Font.Bold = true;
                header.Style.Font.FontName = "Aptos";
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Column(c).Width = (c <= 8) ? 17 : 16;
            }
            ws.Row(1).Height = 31;
            ws.Column(6).Width = 20;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                var row = data.Rows[i];
                int r = i + 2;
                ws.Row(r).Height = 23;
                for (int j = 0; j < ProdCapacityDbColumns.Length; j++)
                {
                    var name = ProdCapacityDbColumns[j];
                    var cell = ws.Cell(r, j + 1);
                    if (name == "UpdateDate")
                    {
                        var dt = DataDate(row, name);
                        if (dt.HasValue)
                        {
                            cell.Value = dt.Value;
                            cell.Style.DateFormat.Format = "yyyy/MM/dd";
                        }
                    }
                    else if (j <= 6)
                    {
                        cell.Value = DataText(row, name);
                    }
                    else if (name == "SMTPointsTotal")
                    {
                        var value = DataDecimal(row, name);
                        SetDecimal(ws, r, j + 1, value, CapacityNumberFormat(value, 4));
                    }
                    else if (name is "COBHC" or "SMTHC" or "InsertHC" or "SCHC"
                             or "AssyHC" or "MicHC" or "OtherHC")
                    {
                        SetCapacityDecimal(ws, r, j + 1, DataDecimal(row, name));
                    }
                    else
                    {
                        // All Output and PCB/layout columns are integers.
                        SetInt(ws, r, j + 1, DataInt(row, name));
                    }

                    cell.Style.Font.FontName = "Aptos";
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Font.FontColor = XLColor.FromHtml("#087C59"); // DB-linked values
                    cell.Style.Fill.BackgroundColor = i % 2 == 0
                        ? XLColor.White : XLColor.FromHtml("#F4F8FC");
                }
            }
            if (data.Rows.Count > 0)
                ws.Range(1, 1, data.Rows.Count + 1, ProdCapacityDbColumns.Length).SetAutoFilter();
            ws.SheetView.FreezeRows(1);
        }

        private static string DataText(DataRow row, string field)
        {
            if (!row.Table.Columns.Contains(field) || row.IsNull(field)) return string.Empty;
            return Convert.ToString(row[field], CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static decimal DataDecimal(DataRow row, string field)
        {
            if (!row.Table.Columns.Contains(field) || row.IsNull(field)) return 0m;
            return Convert.ToDecimal(row[field], CultureInfo.InvariantCulture);
        }

        private static int DataInt(DataRow row, string field)
        {
            if (!row.Table.Columns.Contains(field) || row.IsNull(field)) return 0;
            // Handle legacy/stored procedure decimal values consistently with Excel upload.
            return RoundProdCapacityToInt(DataDecimal(row, field), $"Column {field}");
        }

        private static DateTime? DataDate(DataRow row, string field)
        {
            if (!row.Table.Columns.Contains(field) || row.IsNull(field)) return null;
            return Convert.ToDateTime(row[field], CultureInfo.InvariantCulture);
        }

        private static void SetDecimal(IXLWorksheet ws, int row, int col,
            decimal value, string numberFormat)
        {
            var cell = ws.Cell(row, col);
            cell.Value = value;
            cell.Style.NumberFormat.Format = numberFormat;
        }

        private static void SetInt(IXLWorksheet ws, int row, int col, int value)
        {
            var cell = ws.Cell(row, col);
            cell.Value = value;
            cell.Style.NumberFormat.Format = "#,##0";
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
                        Remark = i.Remark,
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
        public async Task<string> EditTimeStudy(PEViewModel pev)
        {
            try
            {
                if (pev.timeStudyList != null && pev.timeStudyList.Count > 0)
                {
     

                    // Delete Time Study Detail and Time Study Step Detail and re-add
                    var detailToDelete = await _con.UV_PE_TimeStudy_Dtl.Where(i => i.ParentId == pev.TimeStudyHdr.Id).ToListAsync();
                    if (detailToDelete.Any())
                    {
                        await _proc.Proc_ExcecuteNonQuery("spweb_UV_PE_InputTimeStudy_DelDetail", new Dictionary<string, object>
                        {
                            { "@id", pev.TimeStudyHdr.Id },
                            { "@user_id", _hca.HttpContext?.User?.Identity?.Name ?? string.Empty }
                        });
                    }
                   
                    // Change Time Study Header
                    var data = await _con.UV_PE_TimeStudy_Hdr.FindAsync(pev.TimeStudyHdr.Id);
                    data.Customer = pev.TimeStudyHdr.Customer;
                    data.Section = pev.TimeStudyHdr.Section;
                    //data.Model = pev.TimeStudyHdr.Model;
                    //data.BModel = pev.TimeStudyHdr.BModel;
                    data.LotNo = pev.TimeStudyHdr.LotNo;
                    data.Unit = pev.TimeStudyHdr.Unit;
                    data.Active = true;
                    data.PcbName = pev.TimeStudyHdr.PcbName;
                    data.PcbNo = pev.TimeStudyHdr.PcbNo;
                    data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                    data.UpdatedDt = DateTime.Now;
                    await _con.SaveChangesAsync();

                    // Re-add Time Study Detail
                    List<TimeStudyDtlModel> tdm = pev.timeStudyList
                        .GroupBy(i => new { i.OperationKind, i.StepNo, i.StepContent, i.UnitQty, i.AllocatedOpr })
                        .Select(g => new TimeStudyDtlModel
                        {
                            ParentId = pev.TimeStudyHdr.Id,  // Now this has a valid value
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
                    await _con.UV_PE_TimeStudy_Dtl.AddRangeAsync(tdm);
                    await _con.SaveChangesAsync();
                    // Re-add Time Study Step Detail
                    List<TimeStudyStepDtlModel> tsdm = pev.timeStudyList
                        .Select(i => new TimeStudyStepDtlModel
                        {
                            StepId = tdm.FirstOrDefault(t => t.ParentId == pev.TimeStudyHdr.Id && t.StepNo == i.StepNo).Id,
                            SeqNo = i.SeqNo,
                            OperationName = i.OperationName,
                            OperationDetailName = i.OperationDetailName,
                            Time01 = i.Time01,
                            Time02 = i.Time02,
                            Time03 = i.Time03,
                            Time04 = i.Time04,
                            Time05 = i.Time05,
                            TimeAvg = i.TimeAvg,
                            Remark = i.Remark,
                            CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                            CreatedDt = DateTime.Now,
                        })
                        .ToList();
                    await _con.UV_PE_TimeStudyStep_Dtl.AddRangeAsync(tsdm);
                    await _con.SaveChangesAsync();
                }
                else
                {
                    // Delete Time Study Detail and Time Study Step Detail and re-add
                    await _proc.Proc_ExcecuteNonQuery("spweb_UV_PE_InputTimeStudy_Del", new Dictionary<string, object>
                        {
                            { "@id", pev.TimeStudyHdr.Id },
                            { "@user_id", _hca.HttpContext?.User?.Identity?.Name ?? string.Empty }
                        });
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<DataSet> IniTimeStudy(Dictionary<string, object> dic)
        {
            DataSet ds = await _proc.Proc_GetDataset("spweb_UV_PE_InputTimeStudy_Initial", dic);
            return ds;
        }

        public async Task<PEViewModel> IniTimeStudyDetail(string? operationName)
        {
            PEViewModel pvm = new PEViewModel();
            var opr = _con.Master_Operation_Hdr.Where(i => i.Name.Contains(operationName??string.Empty) && i.IsActive);
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

        public async Task<PEViewModel> GetTimeStudyEdit(int id)
        {
            PEViewModel pvm = new PEViewModel();

            try
            {
                var data = await _con.UV_PE_TimeStudy_Hdr.FindAsync(id);
                if (data == null) return new PEViewModel { error_msg = "Data not found." };
                var detail = await _con.UV_PE_TimeStudy_Dtl.Where(i => i.ParentId == id).ToListAsync();
                if (!detail.Any()) return new PEViewModel { error_msg = "Detail data not found." };

                List<TimeStudyStepDtlModel> stepList = new List<TimeStudyStepDtlModel>();
                foreach (var t in detail)
                {
                    var step = await _con.UV_PE_TimeStudyStep_Dtl.Where(i => i.StepId == t.Id).ToListAsync();
                    if (step.Any())
                    {
                        stepList.AddRange(step);
                    }
                }
                if (!stepList.Any()) return new PEViewModel { error_msg = "Step detail data not found." };

                TimeStudyHdrDTO thd = _mapper.Map<TimeStudyHdrDTO>(data);
                List<TimeStudyDtlDTO> tdd = _mapper.Map<List<TimeStudyDtlDTO>>(detail.ToList());
                List<TimeStudyStepDtlDTO> tsdd = _mapper.Map<List<TimeStudyStepDtlDTO>>(stepList);

                //var otherList = tdd.Zip(tsdd, (detail, step) => new TimeStudyOtherDTO
                //{
                //    TimeStudyDtl = detail,
                //    TimeStudyStepDtl = step
                //}).ToList();

                var otherList = (from dtl in tdd
                                 join step in tsdd
                                 on dtl.Id equals step.StepId   // 🔑 adjust key here
                                 select new TimeStudyOtherDTO
                                 {
                                     TimeStudyDtl = dtl,
                                     TimeStudyStepDtl = step
                                 }).ToList();



                List<TimeStudyDTO> tsd = _mapper.Map<List<TimeStudyDTO>>(otherList);

                pvm.TimeStudyHdr = thd;
                tsd.ForEach(i => i.TimeTotal = i.TimeAvg * i.UnitQty);
                pvm.timeStudyList = tsd;
            }
            catch(Exception ex)
            {
                pvm.error_msg = ex.Message;
            }
            

            return pvm;
        }

        public async Task<PEViewModel> GetTimeStudyDtlEdit(int stepID)
        {
            PEViewModel pvm = new PEViewModel();
            try {
                var data = await _con.UV_PE_TimeStudy_Dtl.FindAsync(stepID);
                if (data == null) return new PEViewModel { error_msg = "Data not found." };
                var detail = await _con.UV_PE_TimeStudyStep_Dtl.Where(i => i.StepId == stepID).ToListAsync();
                if (!detail.Any()) return new PEViewModel { error_msg = "Detail data not found." };
                TimeStudyDtlDTO tdd = _mapper.Map<TimeStudyDtlDTO>(data);
                List<TimeStudyStepDtlDTO> tsdd = _mapper.Map<List<TimeStudyStepDtlDTO>>(detail);
                pvm.TimeStudyDtl = tdd;
                pvm.timeStudyStepDtlList = tsdd;
            }
            catch (Exception ex) {
                pvm.error_msg = ex.Message;
            }
            return pvm;
        }


        public async Task<T> GetValueOrDefault<T>(IDataReader reader, Dictionary<string, int> colMap, string colName, T defaultValue = default)
        {
            if (!colMap.TryGetValue(colName, out int idx))
                return defaultValue;

            object val = reader.GetValue(idx);
            if (val == DBNull.Value || val == null)
                return defaultValue;

            return (T)Convert.ChangeType(val, typeof(T));
        }


        public async Task<List<TimeStudyUploadDTO>> UploadTimeStudyData(IFormFile ff)
        {
            List<TimeStudyUploadDTO> list = new();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await ff.CopyToAsync(stream);
                    // get master upload file information
                    List<UploadFileMaster> ufm = _con.Master_UploadFile_mst.Where(i => i.file_name.ToLower().Contains("timestudy")).ToList();
                    // build dictionary: db_col_name -> col_index
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
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
                                decimal t1 = CommonFormat.GetValueOrDefault<decimal>(reader, colMap, "Time01", 0m);
                                decimal t2 = CommonFormat.GetValueOrDefault<decimal>(reader, colMap, "Time02", 0m);
                                decimal t3 = CommonFormat.GetValueOrDefault<decimal>(reader, colMap, "Time03", 0m);
                                decimal t4 = CommonFormat.GetValueOrDefault<decimal>(reader, colMap, "Time04", 0m);
                                decimal t5 = CommonFormat.GetValueOrDefault<decimal>(reader, colMap, "Time05", 0m);

                                string model = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Model", string.Empty);
                                string bModel = CommonFormat.GetValueOrDefault<string>(reader, colMap, "BModel", string.Empty);

                                int stepNo = CommonFormat.GetValueOrDefault<int>(reader, colMap, "StepNo", 0);
                                int unitQty = CommonFormat.GetValueOrDefault<int>(reader, colMap, "UnitQty", 0);

                                decimal[] col = new[] { t1, t2, t3, t4, t5 };

                                
                                if(col.Sum()==0 || string.IsNullOrEmpty(model) || stepNo == 0
                                        || unitQty == 0) return list;

                                TimeStudyUploadDTO tsu = new TimeStudyUploadDTO()
                                {
                                    Customer = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Customer", string.Empty),
                                    Section = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Section", string.Empty),
                                    Model = model,
                                    BModel = bModel,
                                    LotNo = CommonFormat.GetValueOrDefault<string>(reader, colMap, "LotNo", string.Empty),
                                    Unit = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Unit", string.Empty),
                                    PcbName = CommonFormat.GetValueOrDefault<string>(reader, colMap, "PcbName", string.Empty),
                                    PcbNo = CommonFormat.GetValueOrDefault(reader, colMap, "PcbNo", string.Empty),
                                    OperationKind = CommonFormat.GetValueOrDefault<string>(reader, colMap, "OperationKind", string.Empty),
                                    StepNo = stepNo,
                                    StepContent = CommonFormat.GetValueOrDefault<string>(reader, colMap, "StepContent", string.Empty),
                                    UnitQty = unitQty,
                                    AllocatedOpr = CommonFormat.GetValueOrDefault<int>(reader, colMap, "AllocatedOpr", 0),
                                    //SeqNo = CommonFormat.GetValueOrDefault<int>(reader, colMap, "SeqNo", 0),
                                    OperationName = CommonFormat.GetValueOrDefault<string>(reader, colMap, "OperationName", string.Empty),
                                    OperationDetailName = CommonFormat.GetValueOrDefault<string>(reader, colMap, "OperationDetailName", string.Empty),
                                    Time01 = t1,
                                    Time02 = t2,
                                    Time03 = t3,
                                    Time04 = t4,
                                    Time05 = t5,
                                    TimeAvg = col.Where(i => i > 0).Any() ? Math.Round(col.Where(i => i > 0).Average(), 2) : 0m
                                };
                                list.Add(tsu);
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }            
        }
        public async Task<PEViewModel> UploadTimeStudy(IFormFile ff)
        {
            PEViewModel pev = new();
            List<TimeStudyHdrModel> tshList = new();
            List<TimeStudyDtlModel> tsdList = new();
            List<TimeStudyStepDtlModel> tssdList = new();
            string file_name = $"{ff.FileName}_{DateTime.Now:yyyyMMddHHmmss}";
            List<TimeStudyUploadDTO> uploadList = new();
            uploadList = await UploadTimeStudyData(ff);
            if (uploadList.Count == 0)
            {
                pev.error_msg = "Wrong format data or No data found in the uploaded file.";
                return pev;
            }
            try
            {
                var groups = uploadList.GroupBy(r => new { r.Customer, r.Section, r.Model, r.BModel, r.LotNo, r.Unit, r.PcbName, r.PcbNo });
                foreach (var g in groups)
                {
                    var existingHdr = _con.UV_PE_TimeStudy_Hdr
                        .Where(h =>
                            h.Customer == g.Key.Customer &&
                            h.Section == g.Key.Section &&
                            h.Model == g.Key.Model &&
                            h.BModel == g.Key.BModel &&
                            h.LotNo == g.Key.LotNo &&
                            h.Unit == g.Key.Unit &&
                            h.PcbName == g.Key.PcbName &&
                            h.PcbNo == g.Key.PcbNo);
                    if (existingHdr.Any()) {
                        existingHdr.ToList().ForEach(i => {
                            i.Active = false; 
                            i.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                            i.UpdatedDt = DateTime.Now;
                        });
                        await _con.SaveChangesAsync();
                    }
                    var hdr = new TimeStudyHdrModel
                    {
                        Customer = g.Key.Customer,
                        Section = g.Key.Section,
                        Model = g.Key.Model,
                        BModel = g.Key.BModel,
                        LotNo = g.Key.LotNo,
                        Unit = g.Key.Unit,
                        PcbName = g.Key.PcbName,
                        PcbNo = g.Key.PcbNo,
                        UploadFile = file_name,
                        CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                        CreatedDt = DateTime.Now
                    };
                    // Group by Dtl-level fields
                    var dtlGroups = g.GroupBy(r => new { r.OperationKind, r.StepNo, r.StepContent, r.UnitQty, r.AllocatedOpr});
                    foreach (var d in dtlGroups)
                    {
                        var dtl = new TimeStudyDtlModel
                        {
                            OperationKind = d.Key.OperationKind,
                            StepNo = d.Key.StepNo,
                            StepContent = d.Key.StepContent,
                            UnitQty = d.Key.UnitQty,
                            AllocatedOpr = d.Key.AllocatedOpr,
                            Sumary = d.Sum(x => x.TimeAvg) * d.Key.UnitQty,
                            CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                            CreatedDt = DateTime.Now
                        };
                        int i = 0;
                        foreach (var s in d)
                        {
                            i++;
                            var stepDtl = new TimeStudyStepDtlModel
                            {
                                SeqNo = i,
                                OperationName = s.OperationName,
                                OperationDetailName = s.OperationDetailName,
                                Remark = s.Remark,
                                Time01 = s.Time01,
                                Time02 = s.Time02,
                                Time03 = s.Time03,
                                Time04 = s.Time04,
                                Time05 = s.Time05,
                                TimeAvg = s.TimeAvg,
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                                CreatedDt = DateTime.Now
                            };
                            dtl.TimeStudyStepDtl.Add(stepDtl);
                        }
                        hdr.TimeStudyDtl.Add(dtl);
                    }
                    await _con.UV_PE_TimeStudy_Hdr.AddAsync(hdr);
                }
                await _con.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                pev.error_msg = ex.Message;
                return pev;
            }

            // group by header-level fields          

            if (uploadList.Count > 0)
            {
                pev.data = await GetTimeStudy(new Dictionary<string, object>
                {
                    { "@upload_file", file_name }
                });
            }
            return pev;
        }
        #endregion


        //===============>> Time Study New <<==================\\
        #region Time Study New
        public async Task<string> AddTimeStudyNew(PEViewModel pev)
        {
            try
            {
                if(pev.TimeStudyNewStepDtlList != null && pev.TimeStudyNewStepDtlList.Count > 0)
                {
                    // Map step detail
                    //List<TimeStudyNewStepDtlModel> stepList = _mapper.Map<List<TimeStudyNewStepDtlModel>>(pev.TimeStudyNewStepDtlList);
                    List<TimeStudyNewStepDtlDTO> stepList = pev.TimeStudyNewStepDtlList;

                    List<TimeStudyNewDtlModel> detailList = _mapper.Map<List<TimeStudyNewDtlModel>>(pev.TimeStudyNewDtl);

                    detailList.ForEach(i =>
                    {
                        var sList = stepList.Where(j => j.StepContent == i.StepContent);
                        List<TimeStudyNewStepDtlModel> stepDtlModels = _mapper.Map<List<TimeStudyNewStepDtlModel>>(sList);
                        stepDtlModels.ForEach(j=> {
                            j.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                            j.CreatedDt = DateTime.Now;
                        });
                        i.Sumary = pev.TimeStudyNewStepDtlList.Where(k => k.StepContent == i.StepContent).Sum(k => k.TimeAvg);
                        i.SetTime = i.Sumary * i.UnitQty;
                        i.TargetQty = i.SetTime > 0 ? (int)Math.Floor(460 * 60 / i.SetTime) : 0;
                        i.ProcessTime = i.AllocatedOpr > 0 ? Math.Round(i.SetTime / i.AllocatedOpr, 4) : 0;
                        i.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                        i.CreatedDt = DateTime.Now;
                        i.TimeStudyStepDtl.AddRange(stepDtlModels);
                    });

                    // 1. Map and add header
                    TimeStudyNewHdrModel thm = _mapper.Map<TimeStudyNewHdrModel>(pev.TimeStudyNewHdr);
                    thm.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                    thm.CreatedDt = DateTime.Now;

                    // Other info
                    //public int OperatorQty { get; set; } = 1; // Number of operators = Sum ( AllocatedOPR) in detail
                    //        public decimal BottleNeckProcess { get; set; } = 0; // Max ProcessTime in detail
                    //        public decimal TimeTotal { get; set; } = 0; // Sum of SetTime in detail
                    //        public decimal OutputTarget { get; set; } = 0; // OutputTarget = Fix Target = 460 * 60 * OperatorQty/ TimeTotal
                    //        public decimal PitchTime { get; set; } = 0; // PitchTime =   (460*60)/OutputTarget
                    //public decimal LineBalance { get; set; } = 0; // = TimeTotal / ( BottleNeckProcess * OperatorQty) 
                    thm.OperatorQty = detailList.Sum(i => i.AllocatedOpr);
                    thm.BottleNeckProcess = detailList.Any() ? detailList.Max(i => i.ProcessTime) : 0;
                    thm.TimeTotal = detailList.Sum(i => i.SetTime);
                    thm.OutputTarget = thm.TimeTotal > 0 ? Math.Floor(460 * 60 * thm.OperatorQty / thm.TimeTotal) : 0;
                    thm.PitchTime = thm.OutputTarget > 0 ? Math.Round((460 * 60) / thm.OutputTarget, 4) : 0;
                    thm.LineBalance = thm.BottleNeckProcess > 0 && thm.OperatorQty > 0 ? Math.Round((thm.TimeTotal / (thm.BottleNeckProcess * thm.OperatorQty))*100, 0):0;
                 
                    thm.TimeStudyDtl.AddRange(detailList);

                    await _con.UV_PE_TimeStudyNew_Hdr.AddAsync(thm);
                    await _con.SaveChangesAsync();  // <-- This will populate thm.Id with the generated PK                   
                } 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
        public Task<string> DeleteTimeStudyNew(List<int> ids)
        {
            throw new NotImplementedException();
        }
        public async Task<DataTable> GetTimeStudyNew(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_PE_InputTimeStudyNew_List", dic);
            return dt;
        }
        public async Task<string> EditTimeStudyNew(PEViewModel pev)
        {
            using var tran = await _con.Database.BeginTransactionAsync();

            try
            {
                var userName = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;

                if (pev.TimeStudyNewStepDtlList != null && pev.TimeStudyNewStepDtlList.Count > 0)
                {
                    // ====== 1. Load Header ======
                    var data = await _con.UV_PE_TimeStudyNew_Hdr
                        .Include(i => i.TimeStudyDtl)
                        .ThenInclude(d => d.TimeStudyStepDtl)
                        .FirstOrDefaultAsync(i => i.Id == pev.TimeStudyNewHdr.Id);

                    if (data == null)
                        return "Header not found.";

                    // ====== 2. Update Header Fields ======
                    data.Customer = pev.TimeStudyNewHdr.Customer ?? string.Empty;
                    data.Section = pev.TimeStudyNewHdr.Section;
                    data.ModelCat = pev.TimeStudyNewHdr.ModelCat;
                    data.LotNo = pev.TimeStudyNewHdr.LotNo;
                    data.Unit = pev.TimeStudyNewHdr.Unit;
                    data.Active = true;
                    data.PcbName = pev.TimeStudyNewHdr.PcbName;
                    data.PcbNo = pev.TimeStudyNewHdr.PcbNo;
                    data.UpdatedBy = userName;
                    data.UpdatedDt = DateTime.Now;

                    // ====== 3. Remove all old details ======
                    _con.UV_PE_TimeStudyNewStep_Dtl.RemoveRange(
                        data.TimeStudyDtl.SelectMany(d => d.TimeStudyStepDtl)
                    );
                    _con.UV_PE_TimeStudyNew_Dtl.RemoveRange(data.TimeStudyDtl);
                    await _con.SaveChangesAsync(); // commit delete before re-add

                    data.TimeStudyDtl = new List<TimeStudyNewDtlModel>();

                    // ====== 4. Rebuild Detail + StepDetail ======
                    foreach (var i in pev.TimeStudyNewDtlList)
                    {
                        var mapped = _mapper.Map<TimeStudyNewDtlModel>(i);
                        mapped.Id = 0; // reset identity
                        mapped.ParentId = data.Id;
                        mapped.CreatedBy = userName;
                        mapped.CreatedDt = DateTime.Now;

                        // map and reset step details
                        var sList = _mapper.Map<List<TimeStudyNewStepDtlModel>>(
                            pev.TimeStudyNewStepDtlList.Where(j => j.StepId == i.Id).ToList()
                        );

                        // compute metrics
                        mapped.Sumary = sList.Sum(k => k.TimeAvg);
                        mapped.SetTime = mapped.Sumary * mapped.UnitQty;
                        mapped.TargetQty = mapped.SetTime > 0 ? (int)Math.Floor(460 * 60 / mapped.SetTime) : 0;
                        mapped.ProcessTime = mapped.AllocatedOpr > 0 ? Math.Round(mapped.SetTime / mapped.AllocatedOpr, 4) : 0;

                        // add new detail first to get its ID
                        await _con.UV_PE_TimeStudyNew_Dtl.AddAsync(mapped);
                        await _con.SaveChangesAsync();

                        // now link child steps
                        foreach (var k in sList)
                        {
                            k.Id = 0; // reset identity
                            k.StepId = mapped.Id; // assign FK
                            k.CreatedBy = userName;
                            k.CreatedDt = DateTime.Now;
                        }

                        await _con.UV_PE_TimeStudyNewStep_Dtl.AddRangeAsync(sList);
                        await _con.SaveChangesAsync();

                        mapped.TimeStudyStepDtl = sList;
                        data.TimeStudyDtl.Add(mapped);
                    }

                    // ====== 5. Update Header Aggregates ======
                    data.OperatorQty = pev.TimeStudyNewDtlList.Sum(i => i.AllocatedOpr);
                    data.BottleNeckProcess = pev.TimeStudyNewDtlList.Any() ? pev.TimeStudyNewDtlList.Max(i => i.ProcessTime) : 0;
                    data.TimeTotal = pev.TimeStudyNewDtlList.Sum(i => i.SetTime);
                    data.OutputTarget = data.TimeTotal > 0 ? Math.Floor(460 * 60 * data.OperatorQty / data.TimeTotal) : 0;
                    data.PitchTime = data.OutputTarget > 0 ? Math.Round((460 * 60) / data.OutputTarget, 4) : 0;
                    data.LineBalance = data.BottleNeckProcess > 0 && data.OperatorQty > 0 ? Math.Round((data.TimeTotal / (data.BottleNeckProcess * data.OperatorQty)) * 100, 0) : 0;

                    await _con.SaveChangesAsync();
                }
                else
                {
                    // ====== Disable Header ======
                    var data = await _con.UV_PE_TimeStudyNew_Hdr
                        .FirstOrDefaultAsync(i => i.Id == pev.TimeStudyNewHdr.Id);

                    if (data != null)
                    {
                        data.Active = false;
                        data.UpdatedBy = userName;
                        data.UpdatedDt = DateTime.Now;
                        await _con.SaveChangesAsync();
                    }
                }

                await tran.CommitAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ex.Message;
            }
        }

        public async Task<string> EditTimeStudyNew2(PEViewModel pev)
        {
            try
            {
                if (pev.TimeStudyNewStepDtlList != null && pev.TimeStudyNewStepDtlList.Count > 0)
                {

                    // Change Time Study Header
                    var data = await _con.UV_PE_TimeStudyNew_Hdr.Include(i=>i.TimeStudyDtl).FirstOrDefaultAsync(i=>i.Id == pev.TimeStudyNewHdr.Id);
                    data.Customer = pev.TimeStudyNewHdr.Customer ?? string.Empty;
                    data.Section = pev.TimeStudyNewHdr.Section;
                    //data.Model = pev.TimeStudyNewHdr.Model;
                    //data.BModel = pev.TimeStudyNewHdr.BModel;
                    data.ModelCat = pev.TimeStudyNewHdr.ModelCat;
                    data.LotNo = pev.TimeStudyNewHdr.LotNo;
                    data.Unit = pev.TimeStudyNewHdr.Unit;
                    data.Active = true;
                    data.PcbName = pev.TimeStudyNewHdr.PcbName;
                    data.PcbNo = pev.TimeStudyNewHdr.PcbNo;
                    data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                    data.UpdatedDt = DateTime.Now;

                    _con.UV_PE_TimeStudyNew_Dtl.RemoveRange(data.TimeStudyDtl);
                    await _con.SaveChangesAsync(); // commit delete first

                    data.TimeStudyDtl = new List<TimeStudyNewDtlModel>();

                    foreach (var i in pev.TimeStudyNewDtlList)
                    {
                        var mapped = _mapper.Map<TimeStudyNewDtlModel>(i);
                        mapped.Id = 0;
                        mapped.ParentId = data.Id; // ensure FK is set correctly
                        mapped.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                        mapped.CreatedDt = DateTime.Now;

                        // compute derived fields safely
                        var sList = _mapper.Map<List<TimeStudyNewStepDtlModel>>(pev.TimeStudyNewStepDtlList.Where(j => j.StepId == i.Id).ToList());

                        mapped.Sumary = sList.Sum(k => k.TimeAvg);
                        mapped.SetTime = mapped.Sumary * mapped.UnitQty;
                        mapped.TargetQty = mapped.SetTime > 0 ? (int)Math.Floor(460 * 60 / mapped.SetTime) : 0;
                        mapped.ProcessTime = mapped.AllocatedOpr > 0 ? Math.Round(mapped.SetTime / mapped.AllocatedOpr, 4) : 0;

                        await _con.UV_PE_TimeStudyNew_Dtl.AddAsync(mapped);
                        foreach (var k in sList)
                        {
                            k.Id = 0;
                            k.CreatedBy = mapped.CreatedBy;
                            k.CreatedDt = mapped.CreatedDt;
                            k.StepId = mapped.Id; // ensure foreign key assigned after SaveChanges if identity
                        }
                        await _con.UV_PE_TimeStudyNewStep_Dtl.AddRangeAsync(sList);

                        data.TimeStudyDtl.Add(mapped);
                    }

                    data.OperatorQty = data.TimeStudyDtl.Sum(i => i.AllocatedOpr);
                    data.BottleNeckProcess = data.TimeStudyDtl.Any() ? data.TimeStudyDtl.Max(i => i.ProcessTime) : 0;
                    data.TimeTotal = data.TimeStudyDtl.Sum(i => i.SetTime);
                    data.OutputTarget = data.TimeTotal > 0 ? Math.Floor(460 * 60 * data.OperatorQty / data.TimeTotal) : 0;
                    data.PitchTime = data.OutputTarget > 0 ? Math.Round((460 * 60) / data.OutputTarget, 4) : 0;

                    await _con.SaveChangesAsync();
                }
                else
                {
                    var data = await _con.UV_PE_TimeStudyNew_Hdr.Include(i => i.TimeStudyDtl).FirstOrDefaultAsync(i => i.Id == pev.TimeStudyNewHdr.Id);
                    data.Active = false;
                    data.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                    data.UpdatedDt = DateTime.Now;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<DataSet> IniTimeStudyNew(Dictionary<string, object> dic)
        {
            DataSet ds = await _proc.Proc_GetDataset("spweb_UV_PE_InputTimeStudyNew_Initial", dic);
            return ds;
        }

        public async Task<PEViewModel> IniTimeStudyNewDetail(string? operationName)
        {
            PEViewModel pvm = new PEViewModel();
            var opr = _con.Master_Operation_Hdr.Where(i => i.Name.Contains(operationName ?? string.Empty) && i.IsActive);
            if (opr.Any())
            {
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

        public async Task<PEViewModel> GetTimeStudyNewEdit(int id)
        {
            PEViewModel pvm = new PEViewModel();

            try
            {
                var data = await _con.UV_PE_TimeStudyNew_Hdr.FindAsync(id);
                if (data == null) return new PEViewModel { error_msg = "Data not found." };
                var detail = await _con.UV_PE_TimeStudyNew_Dtl.Where(i => i.ParentId == id).ToListAsync();
                if (!detail.Any()) return new PEViewModel { error_msg = "Detail data not found." };

                List<TimeStudyNewStepDtlModel> stepList = new List<TimeStudyNewStepDtlModel>();
                foreach (var t in detail)
                {
                    var step = await _con.UV_PE_TimeStudyNewStep_Dtl.Where(i => i.StepId == t.Id).ToListAsync();
                    if (step.Any())
                    {
                        stepList.AddRange(step);
                    }
                }
                if (!stepList.Any()) return new PEViewModel { error_msg = "Step detail data not found." };

                TimeStudyNewHdrDTO thd = _mapper.Map<TimeStudyNewHdrDTO>(data);
                List<TimeStudyNewDtlDTO> tdd = _mapper.Map<List<TimeStudyNewDtlDTO>>(detail.ToList());
                List<TimeStudyNewStepDtlDTO> tsdd = _mapper.Map<List<TimeStudyNewStepDtlDTO>>(stepList);

                pvm.TimeStudyNewDtlList = tdd;
                pvm.TimeStudyNewStepDtlList = tsdd;
                pvm.TimeStudyNewHdr = thd;
            }
            catch (Exception ex)
            {
                pvm.error_msg = ex.Message;
            }


            return pvm;
        }

        public async Task<PEViewModel> GetTimeStudyNewDtlEdit(int stepID)
        {
            PEViewModel pvm = new PEViewModel();
            try
            {
                var data = await _con.UV_PE_TimeStudyNew_Dtl.FindAsync(stepID);
                if (data == null) return new PEViewModel { error_msg = "Data not found." };
                var detail = await _con.UV_PE_TimeStudyNewStep_Dtl.Where(i => i.StepId == stepID).ToListAsync();
                if (!detail.Any()) return new PEViewModel { error_msg = "Detail data not found." };
                TimeStudyNewDtlDTO tdd = _mapper.Map<TimeStudyNewDtlDTO>(data);
                List<TimeStudyNewStepDtlDTO> tsdd = _mapper.Map<List<TimeStudyNewStepDtlDTO>>(detail);
                pvm.TimeStudyNewDtl = tdd;
                pvm.TimeStudyNewStepDtlList = tsdd;
            }
            catch (Exception ex)
            {
                pvm.error_msg = ex.Message;
            }
            return pvm;
        }



        public async Task<List<TimeStudyNewUploadDTO>> UploadTimeStudyNewData(IFormFile ff)
        {
            List<TimeStudyNewUploadDTO> list = new();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await ff.CopyToAsync(stream);
                    // get master upload file information
                    List<UploadFileMaster> ufm = _con.Master_UploadFile_mst.Where(i => i.file_name.ToLower().Contains("TimeStudyNew")).ToList();
                    // build dictionary: db_col_name -> col_index
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
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

                                string model = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Model", string.Empty);
                               // string bModel = CommonFormat.GetValueOrDefault<string>(reader, colMap, "BModel", string.Empty);

                                int stepNo = CommonFormat.GetValueOrDefault<int>(reader, colMap, "StepNo", 0);
                                int unitQty = CommonFormat.GetValueOrDefault<int>(reader, colMap, "UnitQty", 0);

                                TimeStudyNewUploadDTO tsu = new TimeStudyNewUploadDTO()
                                {
                                    //Customer = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Customer", string.Empty),
                                    Section = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Section", string.Empty),
                                    Model = model,
                                    //BModel = bModel,
                                    LotNo = CommonFormat.GetValueOrDefault<string>(reader, colMap, "LotNo", string.Empty),
                                    Unit = CommonFormat.GetValueOrDefault<string>(reader, colMap, "Unit", string.Empty),
                                    OperationKind = CommonFormat.GetValueOrDefault<string>(reader, colMap, "OperationKind", string.Empty),
                                    //StepNo = stepNo,
                                    StepContent = CommonFormat.GetValueOrDefault<string>(reader, colMap, "StepContent", string.Empty),
                                    UnitQty = unitQty,
                                    AllocatedOpr = CommonFormat.GetValueOrDefault<int>(reader, colMap, "AllocatedOpr", 0),
                                    ProcessName = CommonFormat.GetValueOrDefault<string>(reader, colMap, "ProcessName", string.Empty),
                                    ModelCat = CommonFormat.GetValueOrDefault<string>(reader, colMap, "ModelCat", string.Empty),
                                   // IsPrepare = (reader.GetValue(colMap.GetValueOrDefault("IsPrepare"))?.ToString() ?? string.Empty).ToLower() == "no" ? false : true
                                };
                                list.Add(tsu);
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }

        }
        public async Task<PEViewModel> UploadTimeStudyNew(IFormFile ff)
        {
            PEViewModel pev = new();
            List<TimeStudyNewHdrModel> tshList = new();
            List<TimeStudyNewDtlModel> tsdList = new();
            List<TimeStudyNewStepDtlModel> tssdList = new();
            string file_name = $"{ff.FileName}_{DateTime.Now:yyyyMMddHHmmss}";
            List<TimeStudyNewUploadDTO> uploadList = new();
            uploadList = await UploadTimeStudyNewData(ff);
            if (uploadList.Count == 0)
            {
                pev.error_msg = "Wrong format data or No data found in the uploaded file.";
                return pev;
            }
            try
            {
                var groups = uploadList.GroupBy(r => new {r.Section, r.Model, r.LotNo, r.ModelCat });
                foreach (var g in groups)
                {
                    var existingHdr = await _con.UV_PE_TimeStudyNew_Hdr
                        .Where(h =>
                            h.Section == g.Key.Section &&
                            h.Model == g.Key.Model &&
                            //h.BModel == g.Key.BModel &&
                            h.LotNo == g.Key.LotNo &&
                            //h.Unit == g.Key.Unit &&
                            //h.IsPrepare == g.Key.IsPrepare &&
                            h.ModelCat == g.Key.ModelCat).ToListAsync();
                    if (existingHdr.Any())
                    {
                        existingHdr.ToList().ForEach(i => {
                            i.Active = false;
                            i.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty;
                            i.UpdatedDt = DateTime.Now;
                        });
                        await _con.SaveChangesAsync();
                    }
                    var hdr = new TimeStudyNewHdrModel
                    {
                        Customer=string.Empty,
                        Section = g.Key.Section,
                        Model = g.Key.Model,
                        //BModel = g.Key.BModel,
                        LotNo = g.Key.LotNo,
                        Unit = g.First().Unit,
                        //IsPrepare = g.Key.IsPrepare,
                        ModelCat = g.Key.ModelCat,
                        UploadFile = file_name,
                        CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                        CreatedDt = DateTime.Now
                    };
                    // Group by Dtl-level fields
                    var dtlGroups = g.GroupBy(r => new { r.OperationKind, r.StepContent, r.UnitQty, r.AllocatedOpr });
                    int j = 0;
                    foreach (var d in dtlGroups)
                    {
                        j++;
                        var dtl = new TimeStudyNewDtlModel
                        {
                            OperationKind = d.Key.OperationKind,
                            StepNo = j,
                            StepContent = d.Key.StepContent,
                            UnitQty = d.Key.UnitQty,
                            AllocatedOpr = d.Key.AllocatedOpr,
                            Sumary = 0,
                            CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                            CreatedDt = DateTime.Now
                        };
                        int i = 0;
                        foreach (var s in d)
                        {
                            i++;
                            var stepDtl = new TimeStudyNewStepDtlModel
                            {
                                SeqNo = i,
                                ProcessName = s.ProcessName,
                                ProcessQty = 1,
                                CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? string.Empty,
                                CreatedDt = DateTime.Now
                            };
                            dtl.TimeStudyStepDtl.Add(stepDtl);
                        }
                        hdr.TimeStudyDtl.Add(dtl);
                    }
                    await _con.UV_PE_TimeStudyNew_Hdr.AddAsync(hdr);
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                pev.error_msg = ex.Message;
                return pev;
            }

            // group by header-level fields          

            if (uploadList.Count > 0)
            {
                pev.data = await GetTimeStudyNew(new Dictionary<string, object>
                {
                    { "@upload_file", file_name }
                });
            }
            return pev;
        }

        public async Task<DataSet> ExportTimeStudyNew(Dictionary<string, object> dic)
        {
            DataSet ds = new();
            ds = await _proc.Proc_GetDataset("spweb_UV_PE_InputTimeStudyNew_Report", dic);
            return ds;
        }

        public async Task<List<UploadFileMaster>> GetUploadFileMaster(string name)
        {
            List<UploadFileMaster> ufm = new();
            try { 
                ufm = await _con.Master_UploadFile_mst.Where(i => i.file_name.ToLower().Contains(name.ToLower())).ToListAsync();
            }
            catch { }
            return ufm;
        }
        #endregion

    }
}
