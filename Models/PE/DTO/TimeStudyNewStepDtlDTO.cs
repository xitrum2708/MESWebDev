using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyNewStepDtlDTO
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string StepContent { get; set; } = string.Empty;

        public int SeqNo { get; set; }

        public string ProcessName { get; set; }
        public int ProcessQty { get; set; } = 1;

        [Precision(18, 2)]
        public decimal Time01 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal Time02 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal Time03 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal Time04 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal Time05 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal TimeAvg { get; set; } = 0;

    }
}
