using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.Setting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace MESWebDev.Services.IService
{
    public interface IProdPlanService
    {
        #region Master
        
        Task<List<SelectListItem>> SLILine();
        Task<List<SelectListItem>> SLIMachine();
        Task<List<SelectListItem>> SLIShift();

        //----- Machine -----
        Task<DataTable> MachineList(Dictionary<string, object> dic);
        Task<SMTMachineModel> MachineDetail(string machineId);
        Task<string> MachineAdd(SMTMachineModel machine);
        Task<string> MachineEdit(SMTMachineModel machine);
        Task<string> MachineDelete(string machineId);

        //----- Line -----
        Task<DataTable> LineList(Dictionary<string, object> dic);
        Task<SMTLineModel> LineDetail(string lineId);
        Task<string> LineAdd(SMTLineModel line);
        Task<string> LineEdit(SMTLineModel line);
        Task<string> LineDelete(string lineId);

        //----- Machine Condition -----
        Task<DataTable> MachineConditionList(Dictionary<string, object> dic);
        Task<SMTMachineConditionModel> MachineConditionDetail(int Id);
        Task<string> MachineConditionAdd(SMTMachineConditionModel mc);
        Task<string> MachineConditionEdit(SMTMachineConditionModel mc);
        Task<string> MachineConditionDelete(int Id);

        //----- Line Machine -----
        Task<DataTable> LineMachineList(Dictionary<string, object> dic);
        Task<SMTLineMachineDataModel> LineMachineDetail(int Id);
        Task<string> LineMachineAdd(SMTLineMachineDataModel line);
        Task<string> LineMachineEdit(SMTLineMachineDataModel line);
        Task<string> LineMachineDelete(int Id);

        //----- Shift Setting -----
        Task<DataTable> ShiftList(Dictionary<string, object> dic);
        Task<SMTShiftModel> ShiftDetail(string shiftCode);
        Task<string> ShiftAdd(SMTShiftModel shift);
        Task<string> ShiftEdit(SMTShiftModel shift);
        Task<string> ShiftDelete(string shiftCode);

        //----- Line Calendar -----
        Task<DataTable> LineCalendarList(Dictionary<string, object> dic);
        Task<SMTLineCalendarModel> LineCalendarDetail(int Id);
        Task<string> LineCalendarAdd(SMTLineCalendarModel shift);
        Task<string> LineCalendarEdit(SMTLineCalendarModel shift);
        Task<string> LineCalendarDelete(int Id);

        #endregion

        #region SMT Prod Plan        
        //----- LOT - MODEL - PCB -----
        //spweb_UV_SMT_LOT_PCB
        Task<DataTable> SMTLotPcbList(Dictionary<string, object> dic);
        Task<SMTLotPcbModel> SMTLotPcbDetail(int Id);
        Task<string> SMTLotPcbAdd(SMTLotPcbModel shift);
        Task<ProdPlanViewModel> SMTLotPcbUpload(IFormFile file);
        Task<string> SMTLotPcbEdit(SMTLotPcbModel shift);
        Task<string> SMTLotPcbDelete(List<int> Ids);

        #endregion

        #region PC Prod Plan

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
        #endregion
    }
}
