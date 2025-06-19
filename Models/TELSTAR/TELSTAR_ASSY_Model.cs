namespace MESWebDev.Models.TELSTAR
{
    public class TELSTAR_ASSY_Model
    {
        public long Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string LotNo { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }
}