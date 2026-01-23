using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineCalendarDtlModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SMTLineCalendarHdrModel")]
        public int HeaderId { get; set; }
        public SMTLineCalendarModel SMTLineCalendarHdrModel { get; set; }

        public string ConditionType { get; set; } // Weekday, Date Range, Exact Date
        public string? WeekDay { get; set; } // Mon, Tue, Wed, Thu, Fri, Sat, Sun
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
