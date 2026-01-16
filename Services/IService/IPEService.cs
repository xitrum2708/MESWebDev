using MESWebDev.Models.COMMON;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace MESWebDev.Services.IService
{
    public interface IPEService
    {

        // Manpower
        Task<DataTable> GetManpower(Dictionary<string, object> dic);
        Task<PEViewModel> UploadManpower(IFormFile f);
        Task<string> DeleteManpower(List<int> ids);
        Task<string> EditManpower(ManpowerModel mm);
        Task<string> AddManpower(ManpowerModel mm);
        Task<ManpowerModel> GetManpowerDetail(int id);

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
        Task<string> EditTimeStudy(PEViewModel pev);
        Task<string> AddTimeStudy(PEViewModel pev);
        Task<PEViewModel> GetTimeStudyEdit(int id);
        

        Task<PEViewModel> GetTimeStudyDtlEdit(int stepID);

        Task<DataSet> IniTimeStudy(Dictionary<string, object> dic);
        Task<PEViewModel> IniTimeStudyDetail(string? operationName);



        // Time Study New
        Task<DataTable> GetTimeStudyNew(Dictionary<string, object> dic);
        Task<PEViewModel> UploadTimeStudyNew(IFormFile f);
        Task<string> EditTimeStudyNew(PEViewModel pev);
        Task<string> AddTimeStudyNew(PEViewModel pev);
        Task<PEViewModel> GetTimeStudyNewEdit(int id);


        Task<PEViewModel> GetTimeStudyNewDtlEdit(int stepID);

        Task<DataSet> IniTimeStudyNew(Dictionary<string, object> dic);
        Task<PEViewModel> IniTimeStudyNewDetail(string? operationName);

        Task<DataSet> ExportTimeStudyNew(Dictionary<string, object> dic);

        Task<List<UploadFileMaster>> GetUploadFileMaster(string name);

    }
}
