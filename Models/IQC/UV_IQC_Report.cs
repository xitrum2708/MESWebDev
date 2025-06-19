using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.IQC
{
    public class UV_IQC_Report
    {
        [Key]
        public string? ReportID { get; set; }

        public string? LottagId { get; set; }
        public string? VendorName { get; set; }
        public string? VendorCode { get; set; }
        public string? PartName { get; set; }
        public string? PartCode { get; set; }
        public string? PO_NO { get; set; }
        public string InvoiceNo { get; set; }
        public int POQty { get; set; } = 0;
        public int POCount { get; set; } = 0;
        public DateTime? InspectionDate { get; set; }
        public int InspectionGroupID { get; set; }
        public string? FinalJudgment { get; set; }
        public string? Status { get; set; }
        public string? CheckerStatus { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Notes { get; set; }
        public string? NotesReturn { get; set; }
        public string? TextRemark { get; set; }

        public UV_IQC_InspectionGroup InspectionGroups { get; set; }

        // Quan hệ 1-n: Một báo cáo có nhiều ReportItems
        public ICollection<UV_IQC_ReportItem> ReportItems { get; set; } = new List<UV_IQC_ReportItem>();
    }
}