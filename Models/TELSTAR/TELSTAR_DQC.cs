namespace MESWebDev.Models.TELSTAR
{
    public class TELSTAR_DQC
    {
        public long Id { get; set; }
        public string QRCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? Model { get; set; }
        public string? LotNo { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
    }
}