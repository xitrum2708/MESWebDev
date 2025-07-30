using System.Data;

namespace MESWebDev.Services
{
    public interface ISMTService
    {
        Task<DataTable> GetMachineSpectionData(Dictionary<string, object> data);
    }
}
