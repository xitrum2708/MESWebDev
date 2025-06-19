namespace MESWebDev.Models.OQC
{
    public class A_OperatorProcessData
    {
        public long Id { get; set; }
        public string? Model { get; set; }
        public string? Market { get; set; }
        public string? TypePcb { get; set; }
        public string? Process { get; set; }
        public string? QrCode { get; set; }
        public string? Checkert { get; set; }
        public DateTime? DateT { get; set; }
        public string? Remark1 { get; set; }
        public string? Remark2 { get; set; }
        public string? Remark3 { get; set; }
        public string? Wireless { get; set; }
        public string? LineName { get; set; }
        public string? BoxBarcode { get; set; }
        public int? NumberPCB { get; set; }
        public string? DateCode { get; set; }
        public string? LotNo { get; set; }
        public string? lotSMT { get; set; }
        public string? modelSMT { get; set; }
        public string? Remarkbyuser { get; set; }
        public string? ReworkStatus { get; set; }
        public int? ReworkNumber { get; set; }
    }
}