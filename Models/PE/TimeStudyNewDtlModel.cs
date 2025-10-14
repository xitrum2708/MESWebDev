using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyNewDtlModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TimeStudyNewHdr")]
        public int ParentId { get; set; }
        public TimeStudyNewHdrModel TimeStudyNewHdr { get; set; }

        public string OperationKind { get; set; }
        public int StepNo { get; set; }
        public string StepContent { get; set; } = string.Empty;

        public int UnitQty { get; set; } = 1;
        [Precision(18, 4)]
        public decimal Sumary { get; set; } = 0; // Sum of all step time

        [Precision(18, 4)]
        public decimal SetTime { get; set; } = 0; // Set = Sumary * UnitQty
        
        public int TargetQty { get; set; } = 1; // Target Qty = 460 * 60 / SetTime  


        //ALLOCATED OPR
        public int AllocatedOpr { get; set; } = 1; // Workers

        [Precision(18, 4)]
        public decimal ProcessTime { get; set; } = 0; // Process Time = SetTime / AllocatedOpr

        

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation
        public ICollection<TimeStudyNewStepDtlModel> TimeStudyStepDtl { get; set; } = new List<TimeStudyNewStepDtlModel>();

    }
}
