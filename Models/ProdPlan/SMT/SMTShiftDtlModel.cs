using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTShiftDtlModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SMTShiftModel")]
        public string ShiftCode { get; set; }
        SMTShiftModel SMTShiftModel { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int StartMinute { get; set; }
        public int EndMinute { get; set; }
    }
}
