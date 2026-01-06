using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.PC
{
    public class CalendarModel
    {
        [Key]
        public DateTime date { get; set; }
        public bool? is_holiday { get; set; }
        public string? note { get; set; }
    }
}
