using MESWebDev.Common;

namespace MESWebDev.Models.IQC.VM
{
    public class ReportIQCViewModel
    {
        public PagedResult<ReportVM> ReportIQC { get; set; }
        public string? VenderCode { get; set; }
        public string? Partcode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}