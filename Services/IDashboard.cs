using System.Data;

namespace MESWebDev.Services
{
    public interface IDashboard
    {
        Task<DataSet> GetIQCDashboard(Dictionary<string, object> dic);
    }
}
