namespace MESWebDev.DTO
{
    public class ReportItemDto
    {
        public int ReportItemID { get; set; }
        public string ReportID { get; set; } = string.Empty;
        public int ErrorCodeID { get; set; } = 0;
        public string? ErrorName { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int SamplingSize { get; set; } = 0;
        public string? Spec { get; set; }
        public string? SpecDetail { get; set; }
        public int CRI { get; set; } = 0;
        public int MAJ { get; set; } = 0;
        public int MIN { get; set; } = 0;
        public int NG_Total { get; set; } = 0;
        public decimal NG_Rate { get; set; } = 0;
        public string? Standard { get; set; }
        public string? Judgment { get; set; }
        public string? Remark { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}