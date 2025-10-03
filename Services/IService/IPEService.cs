using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using System.ComponentModel;
using System.Data;

namespace MESWebDev.Services.IService
{
    public interface IPEService
    {
        // Operation
        Task<List<OperationModel>> GetOperation();
        Task<string> DeleteOperation(int id);
        Task<string> EditOperation(PEViewModel pvm);
        Task<string> AddOperation(PEViewModel pvm);
        Task<PEViewModel> GetOperationDetail(int id);

        // Operation Detail
        Task<List<OperationDetailDTO>> GetOperationDtl();
        Task<string> DeleteOperationDtl(int id);
        Task<string> EditOperationDtl(OperationDetailModel odm);
        Task<string> AddOperationDtl(OperationDetailModel odm);
        Task<OperationDetailModel> GetOperationDtlDetail(int id);

        // Operation Time Studay
        Task<DataTable> GetTimeStudy(Dictionary<string, object> dic);
        Task<PEViewModel> UploadTimeStudy(IFormFile f);
        Task<string> DeleteTimeStudy(List<int> ids);
        Task<string> EditTimeStudy(PEViewModel pev);
        Task<string> AddTimeStudy(PEViewModel pev);
        Task<OperationDetailModel> GetTimeStudyDetail(int id);
        Task<PEViewModel> GetTimeStudyEdit(int id);
        

        Task<PEViewModel> GetTimeStudyDtlEdit(int stepID);

        Task<DataSet> IniTimeStudy(Dictionary<string, object> dic);
        Task<PEViewModel> IniTimeStudyDetail(string? operationName);

        // Manpower
        Task<DataTable> GetManpower(Dictionary<string,object> dic);
        Task<PEViewModel> UploadManpower(IFormFile f);
        Task<string> DeleteManpower(List<int> ids);
        Task<string> EditManpower(ManpowerModel mm);
        Task<string> AddManpower(ManpowerModel mm);
        Task<ManpowerModel> GetManpowerDetail(int id);
    }
}
