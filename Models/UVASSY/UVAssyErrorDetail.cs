namespace MESWebDev.Models.UVASSY
{
    public class UVAssyErrorDetail
    {
        public long No { get; set; }
        public string? Qrcode { get; set; }
        public string? Model { get; set; }
        public string? Lot { get; set; }
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
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remark { get; set; }
        public string? Soldermachine { get; set; }
        public string? Tinwire { get; set; }
        public string? Flux { get; set; }
        public string? Alcohol { get; set; }
        public string? Other { get; set; }
        public string? DDRDate { get; set; }
        public string? DDRKeyin { get; set; }
        public string? DDRCHECK { get; set; }
        public int? DDRDailyUpdate { get; set; } = 0;
    }
}