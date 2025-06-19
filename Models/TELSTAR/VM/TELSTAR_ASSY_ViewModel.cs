using MESWebDev.Common;

namespace MESWebDev.Models.TELSTAR.VM
{
    public class TELSTAR_ASSY_ViewModel
    {
        public PagedResult<TelstarAssyVM>? TelstarVM { get; set; }
        public PagedResult<TelstarDQCVM>? TelstarDQCVM { get; set; }
        public PagedResult<TonlyDQCVM>? TonlyDQCVM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class TelstarAssyVM
    {
        public string? SelectedLotNo { get; set; }
        public string? Model { get; set; }
        public string? SelectedLine { get; set; }
        public string? QRCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class TelstarDQCVM
    {
        public string? LotNo { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? QRCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class TonlyDQCVM
    {
        public string? LotNo { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? PCBCode { get; set; }
        public string? QRCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }
}