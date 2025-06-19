namespace MESWebDev.DTO
{
    public class RepairResultDto
    {
        public string Qrcode { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Lot { get; set; } = string.Empty;
        public int DailyOutput { get; set; } = 0;
        public string? PcbCode { get; set; }
        public string? Pcbtype { get; set; }
        public string? Process { get; set; }
        public string? Errorposition { get; set; }
        public string? Partcode { get; set; }
        public string? Errortype { get; set; }
        public string? Causetype { get; set; }
        public string? DeptError { get; set; }
        public string? Repairmethod { get; set; }
        public string? Statusresult { get; set; }
        public string? UserDept { get; set; }
        public string? Linename { get; set; }
        public int? Qty { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remark { get; set; }
        public string? Soldermachine { get; set; }
        public string? Tinwire { get; set; }
        public string? Flux { get; set; }
        public string? Alcohol { get; set; }
        public string? Other { get; set; }
        public DateTime? DDRDate { get; set; }
        public string? DDRKeyin { get; set; }
        public string? DDRCHECK { get; set; }
        public int DDRDailyUpdate { get; set; } = 0;
        public string? DateCode { get; set; }
    }
}