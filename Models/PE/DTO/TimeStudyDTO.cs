using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyDTO
    {
        public int StepId { get; set; }        
        public int StepNo { get; set; }
        public string StepContent { get; set; } = string.Empty;
        public string OperationKind { get; set; }

        public int SeqNo { get; set; }
        public string OperationName { get; set; }
        public string OperationDetailName { get; set; }
        public string? Remark { get; set; } = "";

        [Precision(18, 4)]
        public decimal Time01 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time02 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time03 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time04 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time05 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal TimeAvg { get; set; } = 0;


        public int UnitQty { get; set; } = 1;
        [Precision(18, 4)]
        public decimal TimeTotal { get; set; } = 0;

        //ALLOCATED OPR
        public int AllocatedOpr { get; set; } = 1;

    }
}
