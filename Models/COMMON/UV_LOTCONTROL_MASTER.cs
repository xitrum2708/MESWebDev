using MESWebDev.Models.SPO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.COMMON
{
    public class UV_LOTCONTROL_MASTER
    {
        [Key]
        public int LotControlID { get; set; }

        [Required(ErrorMessage = "LotNo is required")]
        [MaxLength(50)]
        public string LotNo { get; set; }

        [Required(ErrorMessage = "Qty is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Qty must be great than 0 or more")]
        public int Quantity { get; set; } = 0;

        [MaxLength(255)]
        public string SpecialInfo { get; set; }

        [MaxLength(50)]
        public string DateCode { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "SerialStart is required")]
        [MaxLength(50)]
        public string SerialStart { get; set; }

        [Required(ErrorMessage = "SerialEnd is required")]
        [MaxLength(50)]
        public string SerialEnd { get; set; }

        [Required(ErrorMessage = "ApprovedBy is required")]
        [MaxLength(50)]
        public string ApprovedBy { get; set; }

        [MaxLength(50)]
        public string Revised { get; set; }

        [Required(ErrorMessage = "IssuedBy is required")]
        [MaxLength(50)]
        public string IssuedBy { get; set; }

        [Required(ErrorMessage = "ProDate is required")]
        [DataType(DataType.Date)]
        public DateTime ProDate { get; set; }

        [Required(ErrorMessage = "IssueDate is required")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "PONumber is required")]
        [MaxLength(50)]
        public string PONumber { get; set; }
        public string? CreatedBy { get; set; } = "System";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        // Navigation Property
        public ICollection<UV_LOTGENERALSUMMARY_MASTER> UV_LOTGENERALSUMMARY_MASTER { get; set; }
        [ForeignKey("LotNo")]
        public UV_SPO_MASTER_ALL_Model? SPO_MASTER { get; set; }
    }
}
