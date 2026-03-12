using MESWebDev.Models.MRP;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.ProdPlan.SMT;
using System.Data;

namespace MESWebDev.Services.IService
{
    public interface IMRPService
    {
        // MRP 
        Task<DataTable> MRPDataList(Dictionary<string, object> dic);
        Task<DataTable> MRPDataRun(Dictionary<string, object> dic);


        //OBL
        Task<DataTable> MRPOBLList(Dictionary<string, object> dic);
        Task<MRPViewModel> MRPOBLUpload(IFormFile file);
        Task<MRPOBLModel> MRPOBLDetail(int Id);
        Task<string> MRPOBLAdd(MRPOBLModel obl);
        Task<string> MRPOBLEdit(MRPOBLModel obl);
        Task<string> MRPOBLDelete(int Id);


        //BOM
        Task<DataTable> MRPBOMList(Dictionary<string, object> dic);
        Task<MRPViewModel> MRPBOMUpload(IFormFile file);
        Task<MRPBOMModel> MRPBOMDetail(int Id);
        Task<string> MRPBOMAdd(MRPBOMModel bom);
        Task<string> MRPBOMEdit(MRPBOMModel bom);
        Task<string> MRPBOMDelete(int Id);


        //SPO
        Task<DataTable> MRPSPOList(Dictionary<string, object> dic);
        Task<MRPViewModel> MRPSPOUpload(IFormFile file);
        Task<MRPSPOModel> MRPSPODetail(int Id);
        Task<string> MRPSPOAdd(MRPSPOModel spo);
        Task<string> MRPSPOEdit(MRPSPOModel spo);
        Task<string> MRPSPODelete(int Id);


        //OH
        Task<DataTable> MRPOHList(Dictionary<string, object> dic);
        Task<MRPViewModel> MRPOHUpload(IFormFile file);
        Task<MRPOHModel> MRPOHDetail(int Id);
        Task<string> MRPOHAdd(MRPOHModel oh);
        Task<string> MRPOHEdit(MRPOHModel oh);
        Task<string> MRPOHDelete(int Id);


    }
}
