using System.Data;

namespace MESWebDev.Services
{
    public interface IDashboard
    {
        Task<DataSet> GetIQCDashboard(Dictionary<string, object> dic);
        Task<DataSet> GetSMTDashboard(Dictionary<string, object> dic);
        Task<DataSet> GetWHSDashboard(Dictionary<string, object> dic);
    }
}
