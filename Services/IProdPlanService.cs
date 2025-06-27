using MESWebDev.Models.ProdPlan;
using System.Data;

namespace MESWebDev.Services
{
    public interface IProdPlanService
    {
        Task<ProdPlanViewModel> GetDataFromUploadFile(RequestDTO _request);
        Task<string> SaveProdPlan(ProdPlanViewModel ppv);
        Task<ProdPlanViewModel> ReloadProdPlan(ProdPlanViewModel ppv);
        Task<ProdPlanViewModel> ViewProdPlan(RequestDTO _request);
    }
}
