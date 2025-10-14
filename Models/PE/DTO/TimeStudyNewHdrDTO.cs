using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyNewHdrDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required !")]
        public string Customer { get; set; }

        [Required]
        public string Section { get; set; }

        [Required(ErrorMessage = "This field is required !")]
        public string Model { get; set; }
        public string? BModel { get; set; }
        public string ModelCat { get; set; } = string.Empty; // Model Category
        public string LotNo { get; set; }
        public string? Unit { get; set; }
        public string? PcbName { get; set; }
        public string? PcbNo { get; set; }

        //public bool IsPrepare { get; set; } = false;


        // Other info
        public int OperatorQty { get; set; } = 1; // Number of operators = Sum ( AllocatedOPR) in detail
        [Precision(18, 4)]
        public decimal BottleNeckProcess { get; set; } = 0; // Max ProcessTime in detail
        [Precision(18, 4)]
        public decimal TimeTotal { get; set; } = 0; // Sum of SetTime in detail
        [Precision(18, 4)]
        public decimal OutputTarget { get; set; } = 0; // OutputTarget = Fix Target = 460 * 60 * OperatorQty/ TimeTotal
        [Precision(18, 4)]
        public decimal PitchTime { get; set; } = 0; // PitchTime =   (460*60)/OutputTarget
        // --> This value will be compared with Process Time in detail to make waring if Process Time > PitchTime

        [Precision(18, 2)]
        public decimal LineBalance { get; set; } = 0; // = TimeTotal / ( BottleNeckProcess * OperatorQty) 

    }
}
