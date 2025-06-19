using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.WHS
{
    public class UV_IQC_WHS_SORTING
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Sorting Date is required")]
        [DataType(DataType.Date)]
        public DateTime SortingDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportID { get; set; }

        [Required(ErrorMessage = "Sorting By is required")]
        [StringLength(1000)]
        public string SortingBy { get; set; }

        public int TotalQtyReport { get; set; } = 0;

        [Required(ErrorMessage = "Qty OK is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Qty OK must be 0 or more")]
        public int QtyOK { get; set; } = 0;

        [Required(ErrorMessage = "Qty NG is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Qty OK must be 0 or more")]
        public int QtyNG { get; set; } = 0;
        public int WaitSorting { get; set; } = 0;

        [DataType(DataType.Currency)]
        public decimal RateSortNG { get; set; } = 0.00m;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(1000)]        
        public string? SortingStatus { get; set; }

        [StringLength(500)]
        public string? IssueLot { get; set; }

        public int? IssueQty { get; set; } = 0;

        [StringLength(100)]
        public string? SignQ { get; set; }

        public int BalQty { get; set; } = 0;
        [Required(ErrorMessage = "ManPower is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ManPower must be great than 0 or more")]
        public int TotalManPower { get; set; } = 1;
        [Required(ErrorMessage = "Hours is required")]
        [Range(0.5, int.MaxValue, ErrorMessage = "Hours must be great than 0 or more")]
        public decimal TotalHours { get; set; } = 2;
        [Required(ErrorMessage = "Cost is required")]
        [Range(0.5, int.MaxValue, ErrorMessage = "Cost must be great than 0 or more")]
        [DataType(DataType.Currency)]
        public decimal CostPerHour { get; set; } = 2.25m;

        [DataType(DataType.Currency)]
        public decimal TotalAM { get; set; } = 0.00m;
        [Required(ErrorMessage = "Name Sort is required")]
        [StringLength(100)]
        public string? NameSort { get; set; }
        [Required(ErrorMessage = "Stock is required")]
        [StringLength(100)]
        public string? Stock { get; set; }

        [Required(ErrorMessage = "DateCode is required")]
        [StringLength(1000)]
        public string DateCode { get; set; }
        
        [StringLength(100)]
        public string? Packing { get; set; }

        [StringLength(200)]
        public string? Remark { get; set; }
        [Required(ErrorMessage = "Old lottag is required")]
        [StringLength(1000)]
        public string SLottagId { get; set; }

        [Required(ErrorMessage = "New lottag is required")]
        [StringLength(1000)]
        public string NLottagId { get; set; }
        public string? ReportRemark { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
