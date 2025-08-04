using System.Data;

namespace MESWebDev.Services
{
    public interface ISMTService
    {
        Task<DataTable> GetMachineSpectionData(Dictionary<string, object> data);
        Task<DataTable> GetMachineSpectionData_Download(Dictionary<string, object> data);
    }
}
