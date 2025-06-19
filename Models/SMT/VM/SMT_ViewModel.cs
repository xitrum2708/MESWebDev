using MESWebDev.Common;
using MESWebDev.Models.OQC.VM;

namespace MESWebDev.Models.SMT.VM
{
    public class SMT_ViewModel
    {
        public PagedResult<UVSMT_MODEL_MATRIX_MASTER>? matrixMaster { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
