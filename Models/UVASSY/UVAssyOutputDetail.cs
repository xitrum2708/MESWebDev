namespace MESWebDev.Models.UVASSY
{
    public class UVAssyOutputDetail
    {
        public long No { get; set; }
        public string? Line { get; set; }
        public string Lot { get; set; }
        public string Model { get; set; }
        public int? Unit_Carton { get; set; }
        public string Model_Serial { get; set; }
        public int? Carton { get; set; }
        public string? PackingID { get; set; }
        public string? Dbox_Serial { get; set; }
        public string? Unit_Serial { get; set; }
        public string? Date_Time { get; set; }
        public string? Remark { get; set; }
        public string? Keys { get; set; }
        public string? Keys2 { get; set; }
        public string? BatchNo { get; set; }
        public string? ChangeBatchTo { get; set; }
        public string? ErrorDetail { get; set; }
        public string? ErrorStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public int? Qtyunit { get; set; } = 0;
        public string? Remark2 { get; set; }
        public string? Remark3 { get; set; }
    }
}