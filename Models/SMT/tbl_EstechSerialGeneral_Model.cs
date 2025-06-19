namespace MESWebDev.Models.SMT
{
    public class tbl_EstechSerialGeneral_Model
    {
        public long Id { get; set; }
        public string Serial_Number { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? ModelCode { get; set; }
        public string? PCBCode { get; set; }
        public string? TypeOfProduct { get; set; }
        public string? EndOfYear { get; set; }
        public string? WeeksInYear { get; set; }
        public string? DaysOfWeek { get; set; }
        public int? SerialOfProduct { get; set; }
        public DateTime DateT { get; set; } = DateTime.Now;
        public string? Customercode { get; set; }
        public string? Soft_version { get; set; }
        public string? Lot_no { get; set; }
        public DateTime? outputdate { get; set; }
        public DateTime? outputdate2 { get; set; }
        public DateTime? printdate { get; set; }
        public string? Printstatus { get; set; }
        public string? Note { get; set; }
        public string? createby { get; set; }
    }
}