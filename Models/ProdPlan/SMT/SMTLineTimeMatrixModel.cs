using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineTimeMatrixModel
    {
        [Key]
        public int Id { get; set; }

        public string LineCode { get; set; }

        public string MachineCode { get; set; }

        public string ConditionType { get; set; } // Weekday, Date Range, Exact Date
        public string? WeekDay { get; set; } // Mon, Tue, Wed, Thu, Fri, Sat, Sun
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int StartMinute { get; set; } // Start time in minutes from midnight (e.g., 480 for 8:00 AM)
        public int EndMinute { get; set; } // End time in minutes from midnight (e.g., 1020 for 5:00 PM)
        public int Priority { get; set; } // To determine which record to apply when multiple conditions match

        //--------------- Check this later to make sure that is enough for the time matrix or not, if not we can add more fields like CreatedBy, CreatedDate, etc. to track the changes
    }
}
