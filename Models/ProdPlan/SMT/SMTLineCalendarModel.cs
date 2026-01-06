using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineCalendarModel
    {
        [Key]
        public int Id { get; set; }
        // if * is applied to all lines
        public string LineCode { get; set; }
        public string WeekDayOrDate { get; set; }

        [ForeignKey("Shift")]
        public string ShiftCode { get; set; }
        public SMTShiftModel? Shift { get; set; }        

        public int Priority { get; set; }
        public string? Remark { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
