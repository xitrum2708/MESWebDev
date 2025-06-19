namespace MESWebDev.Models.UVASSY
{
    public class UVAssyProductResult
    {
        public long No { get; set; }
        public string? Div { get; set; }
        public string? Category { get; set; }
        public string? Model { get; set; }
        public string? BModel { get; set; }
        public string LotNo { get; set; } = string.Empty;
        public int? LotSize { get; set; } = 0;
        public int TargetQty { get; set; } = 0;
        public int ProdQty { get; set; } = 0;
        public DateTime? ProdDate { get; set; }
        public string? ProdLine { get; set; }
    }
}