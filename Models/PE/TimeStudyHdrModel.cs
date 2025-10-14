using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyHdrModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required !")]
        public string Customer { get; set; }

        [Required]
        public string Section { get; set; }

        [Required(ErrorMessage = "This field is required !")]
        public string Model { get; set; }
        public string? BModel { get; set; }
        public string LotNo { get; set; }
        public string? Unit { get; set; }
        public string? PcbName { get; set; }
        public string? PcbNo { get; set; }

        // Other in


        public string? UploadFile { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation
        public ICollection<TimeStudyDtlModel> TimeStudyDtl { get; set; } = new List<TimeStudyDtlModel>();
    }
}
