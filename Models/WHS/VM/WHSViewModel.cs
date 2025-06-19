using MESWebDev.Common;

namespace MESWebDev.Models.WHS.VM
{
    public class WHSViewModel
    {
        public PagedResult<IQCRejectedReportViewModel> WHSSortingList { get; set; }
        public PagedResult<SortingMonitorReportViewModel> SortingMonitorList { get; set; }
        public UV_IQC_WHS_SORTING? UV_IQC_WHS_SORTING { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }      
    }
    public class SortingMonitorReportViewModel
    {
        public string ReportID { get; set; }
        public string LottagId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string PartName { get; set; }
        public string PartCode { get; set; }
        public string PO_NO { get; set; }
        public string InvoiceNo { get; set; }
        public int POQty { get; set; }
        public int POCount { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string Status { get; set; }
        public string CheckerStatus { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Notes { get; set; }
        public string NotesReturn { get; set; }
        public string TextRemark { get; set; }
        public string CombinedErrorDescriptions { get; set; }
        public decimal NG_Rate { get; set; }
        public DateTime SortingDate { get; set; }        
        public string SortingBy { get; set; }
        public int TotalQtyReport { get; set; }
        public int QtyOK { get; set; }
        public int QtyNG { get; set; }
        public int WaitSorting { get; set; }
        public decimal RateSortNG { get; set; }
        public string? SortingStatus { get; set; }
        public string? IssueLot { get; set; }
        public int? IssueQty { get; set; }
        public string? SignQ { get; set; }
        public int BalQty { get; set; }
        public int TotalManPower { get; set; }
        public decimal TotalHours { get; set; }
        public decimal CostPerHour { get; set; }
        public decimal TotalAM { get; set; } = 0.00m;
        public string? NameSort { get; set; }
        public string? Stock { get; set; }
        public string DateCode { get; set; }
        public string? Packing { get; set; }
        public string? Remark2 { get; set; }
        public string SLottagId { get; set; }
        public string NLottagId { get; set; }
        public string? ReportRemark { get; set; }
        public string? CreatedBy2 { get; set; }
        public DateTime CreatedDate2 { get; set; }
    }
    public class IQCRejectedReportViewModel
    {
        public string ReportID { get; set; }
        public string LottagId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string PartName { get; set; }
        public string PartCode { get; set; }
        public string PO_NO { get; set; }
        public string InvoiceNo { get; set; }
        public int POQty { get; set; }
        public int POCount { get; set; }
        public DateTime? InspectionDate { get; set; }
        public int InspectionGroupID { get; set; }
        public string FinalJudgment { get; set; }
        public string Status { get; set; }
        public string CheckerStatus { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Notes { get; set; }
        public string NotesReturn { get; set; }
        public string TextRemark { get; set; }
        public string CombinedErrorDescriptions { get; set; }
        public decimal NG_Rate { get; set; }
    }


}
