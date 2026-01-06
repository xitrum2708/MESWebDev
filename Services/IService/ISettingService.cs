using MESWebDev.Models.Setting;
using MESWebDev.Models.Setting.DTO;
using System.Data;

namespace MESWebDev.Services.IService
{
    public interface ISettingService
    {
        Task<List<ProjectSettingModel>> GetSettingList();
        void ReloadSetting();

        // FormatZaror only
        Task<FormatRazorDTO> GetFormatRazor();


        //Common
        #region Common
        Task<DataTable> ProjectSettingList(Dictionary<string, object> dic);
        Task<string> ProjectSettingAdd(ProjectSettingModel psModel);
        Task<string> ProjectSettingEdit(ProjectSettingModel psModel);
        Task<string> ProjectSettingDelete(int id);

        Task<ProjectSettingModel> ProjectSettingDetail(int id);
        Task<string> ProjectSettingValue(string property);

        #endregion
    }
}
