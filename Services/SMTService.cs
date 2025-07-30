using MESWebDev.Common;
using MESWebDev.Data;
using System.Data;

namespace MESWebDev.Services
{
    public class SMTService : ISMTService
    {
        private readonly AppDbContext _context;
        private readonly Procedure _proc;

        public SMTService(AppDbContext context)
        {
            _context = context;
            _proc = new Procedure(context);
        }
        public async Task<DataTable> GetMachineSpectionData(Dictionary<string, object> data)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("SMT_Chart_MachineSpectionData", data);
            return dt;
        }
    }
}
