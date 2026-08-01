namespace MESWebDev.Models.TELSTAR
{
    public class TONLY_DQC_CTN
    {
        public long Id { get; set; }
		public string Checker {get; set; }=string.Empty;
        public string QRCodeCTN { get; set; } = string.Empty;
        public string Status { get; set; } 
        public string? Remark { get; set; }
	    public string? Model { get; set; }
	    public string? LotNo { get;set; }   
	    public DateTime CreatedAt { get; set; }
        public string CreatedBy { get;set; }
	    public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get;set; }
    }
}
