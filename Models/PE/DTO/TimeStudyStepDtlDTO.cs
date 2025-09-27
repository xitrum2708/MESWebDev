using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyStepDtlDTO
    {
        [Key]
        public int Id { get; set; }
        public int StepId { get; set; }
        public int SeqNo { get; set; }
        public string OperationName { get; set; }

        public string OperationDetailName { get; set; }
        public string? Remark { get; set; }

        [Precision(18, 4)]
        public decimal Time01 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time02 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time03 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Time04 { get; set; }= 0;
        [Precision(18, 4)]
        public decimal Time05 { get; set; } = 0;
        [Precision(18, 4)]
        public decimal TimeAvg { get; set; } = 0;

    }
}
