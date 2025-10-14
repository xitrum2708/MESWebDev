using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyDtlDTO
    {
        [Key]
        public int Id { get; set; }
        public int ParentId { get; set; }

        public string OperationKind { get; set; }
        public int StepNo { get; set; }
        public string StepContent { get; set; } = string.Empty;

        public int UnitQty { get; set; } = 1;
        [Precision(18, 4)]
        public decimal Sumary { get; set; } = 0;

        //ALLOCATED OPR
        public int AllocatedOpr { get; set; } = 1;

        
        public bool IsActive { get; set; } = true;

    }
}
