using MESWebDev.Models.PE;
using System.ComponentModel;
using System.Data;

namespace MESWebDev.Services
{
    public interface IPEService
    {
        Task<DataTable> GetManpower(Dictionary<string,object> dic);

        Task<PEViewModel> UploadManpower(IFormFile f);

        Task<string> DeleteManpower(List<int> ids);
        Task<string> EditManpower(ManpowerModel mm);
        Task<string> AddManpower(ManpowerModel mm);
        Task<ManpowerModel> GetManpowerDetail(int id);
    }
}
