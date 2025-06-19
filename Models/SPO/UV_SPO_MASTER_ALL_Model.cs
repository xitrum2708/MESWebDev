namespace MESWebDev.Models.SPO
{
    public class UV_SPO_MASTER_ALL_Model
    {
        public long ID { get; set; }
        public string? Customer { get; set; }
        public string? DIV { get; set; }
        public string? LotSMT { get; set; }
        public string LotNo { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? BModel { get; set; }
        public string? Category { get; set; }
        public int LotSize { get; set; } = 0;
        public int Originqty { get; set; } = 0;
        public int Shipped_Qty { get; set; } = 0;
        public string? Customer_Lot { get; set; }
        public string? Lot_Status { get; set; }
        public string? Invoice_No { get; set; }
        public string? PoNo { get; set; }
        public string? PartNo { get; set; }
        public string? ModelDesc { get; set; }
        public string? Womaterial { get; set; }
        public DateTime? PO_Rec { get; set; }
        public DateTime? Shipped_Date { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remark { get; set; }
    }
}