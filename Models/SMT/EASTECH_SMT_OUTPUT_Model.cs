namespace MESWebDev.Models.SMT
{
    public class EASTECH_SMT_OUTPUT_Model
    {
        public long Id { get; set; }
        public string QRCode { get; set; } = string.Empty;
        public string? programkey { get; set; }
        public string Feeder { get; set; } = string.Empty;
        public string? LineName { get; set; }
        public string? Partcode { get; set; }
        public string? NDesc { get; set; }
        public string? DateCode { get; set; }
        public int? usage { get; set; } = 0;
        public DateTime? DateT { get; set; }
        public string? Model { get; set; }
        public string? Remark { get; set; }
        public string? Remark1 { get; set; }
        public string? Remark2 { get; set; }
        public string? Remark3 { get; set; }
        public string? Remark4 { get; set; }
        public string? Remark5 { get; set; }
        public string? Dropcode { get; set; }
        public string? Droppos { get; set; }
        public string? lotSMT { get; set; }
        public string? modelSMT { get; set; }
        public string? IDpart { get; set; }
        public string? RemarkbyUser { get; set; }
    }
}