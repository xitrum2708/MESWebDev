using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.PE
{
    public class ManpowerModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Company { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string UModel { get; set; }
        public string BModel { get; set; }

        public string Description { get; set; }
        public int SmtHeadcount { get; set; } // SMT headcount
        public int InsertHeadcount { get; set; } // Insert headcount
        public int AssyHeadcount { get; set; } // Assembly headcount
        public int SclHeadcount { get; set; } // SCL headcount
        [Precision(18, 4)]
        public decimal SmtCost { get; set; } // SMT cost
        [Precision(18, 4)]
        public decimal InsertCost { get; set; } // Insert cost
        [Precision(18, 4)]
        public decimal SclCost { get; set; } // SCL cost
        [Precision(18, 4)]
        public decimal AssyCost { get; set; } // Assembly cost
        [Precision(18, 4)]
        public decimal AverageCost { get; set; } // average

        public string? UploadFile { get; set; } // Total cost
        [Required(ErrorMessage = "This field is required")]
        public DateTime UpdatedModelDt { get; set; } // Total cost

        public string? Note { get; set; } // 

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDt { get; set; } //
        public string CreatedBy { get; set; } //
        public DateTime? UpdatedDt { get; set; } //
        public string? UpdatedBy { get; set; } //

    }
}
