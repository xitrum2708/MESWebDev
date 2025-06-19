using MESWebDev.Common;

namespace MESWebDev.Models.OQC.VM
{
    public class EASTECH_ViewModel
    {
        public PagedResult<EASTECHOQCVM>? eastechOQCVM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class EASTECHOQCVM
    {
        public string? LotNo { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? PCBCode { get; set; }
        public string? Market { get; set; }
        public string? QRCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }
}