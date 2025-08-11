using MESWebDev.Common;
using MESWebDev.Data;
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

        public Task<DataTable> SearchResult(string search)
        {
            return _proc.Proc_GetDatatable("sp_UVASSY_PPRODUCT_HISTORY_SearchCascade", new Dictionary<string, object>
            {
                { "@key", search }
            });
        }


    }
}
