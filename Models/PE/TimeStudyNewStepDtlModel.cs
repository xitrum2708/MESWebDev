using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyNewStepDtlModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TimeStudyNewDtl")]
        public int StepId { get; set; }
        public TimeStudyNewDtlModel TimeStudyNewDtl { get; set; }

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
        public decimal Time04 { get; set; }= 0;
        [Precision(18, 2)]
        public decimal Time05 { get; set; } = 0;
        [Precision(18, 2)]
        public decimal TimeAvg { get; set; } = 0;

        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }

    }
}
