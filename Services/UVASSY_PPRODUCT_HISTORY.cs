using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.DTO;
using System.Data;

namespace MESWebDev.Services
{
    public class UVASSY_PPRODUCT_HISTORY:IUVASSY_PPRODUCT_HISTORY
    {
        private readonly AppDbContext _context;
        private readonly Procedure _proc;
              

        public UVASSY_PPRODUCT_HISTORY(AppDbContext context)
        {
            _context = context;
            _proc = new Procedure(context); ;
        }

        public Task<DataTable> GetAttachedFile(int Id)
        {
            return _proc.Proc_GetDatatable("sp_Attach_GetById", new Dictionary<string, object>
            {
                { "@Id", Id }
            });
        }

        public Task<DataTable> GetAttachedList(int processID, string qrcode)
        {
            return _proc.Proc_GetDatatable("sp_Attach_ListByProcQr", new Dictionary<string, object>
            {
                { "@processID", processID },
                { "@Qrcode", qrcode }
            });
        }

        public async Task<DataTable> GetProductionErrorList(Dictionary<string, object> param)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_Assy_ErrorList", param);
            return dt;
        }

        public Task<DataTable> SearchResult(ProductHistoryDto search)
        {
            object DbNullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();
            object DbNullIfNull<T>(T? v) where T : struct => v.HasValue ? v.Value : (object)DBNull.Value;

            return _proc.Proc_GetDatatable(
                "sp_UVASSY_PPRODUCT_HISTORY_SearchCascade_edit3",
                new Dictionary<string, object>
                {
                    { "@key",          DbNullIfEmpty(search.Key) },
                    { "@lotno",        DbNullIfEmpty(search.LotNo) },            
                    { "@categoryName", DbNullIfEmpty(search.CategoryName) }
                }
            );
        }

    }
}
