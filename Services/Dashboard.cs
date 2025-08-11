using MESWebDev.Common;
using MESWebDev.Data;
using System.Data;

namespace MESWebDev.Services
{
    public class Dashboard : IDashboard
    {
        private readonly AppDbContext _context;
        private readonly Procedure _proc;

        public Dashboard(AppDbContext context)
        {
            _context = context;
            _proc = new Procedure(context);
        }
        public async Task<DataSet> GetIQCDashboard(Dictionary<string, object> dic)
        {
            DataSet ds = new();
            ds = await _proc.Proc_GetDataset("Dashboard_IQC", dic);
            return ds;
        }
    }
}
