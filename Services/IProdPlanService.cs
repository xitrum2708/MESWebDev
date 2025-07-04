using MESWebDev.Models.ProdPlan;
using System.Data;

namespace MESWebDev.Services
{
    public interface IProdPlanService
    {
        // Prod plan
        Task<ProdPlanViewModel> GetDataFromUploadFile(RequestDTO _request);
        Task<string> SaveProdPlan(ProdPlanViewModel ppv);
        Task<ProdPlanViewModel> ReloadProdPlan(ProdPlanViewModel ppv);
        Task<ProdPlanViewModel> ViewProdPlan(RequestDTO _request);
        Task<DataSet> ExportProdPlan(Dictionary<string,object> dic);


        // holiday
        Task<List<string>> SaveHoliday(List<string> holidays);
        Task<List<string>> GetHoliday(RequestDTO _request);

        // para
        Task<string> SavePara(ProdPlanViewModel ppv);
        Task<Dictionary<string, object>> GetPara();
    }
}
