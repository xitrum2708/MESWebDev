using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.PC
{
    public class LineWorkingHourModel
    {
        [Key]
        public int id { get; set; }
        public string line { get; set; } // production line
        public DateTime date { get; set; }
        public int working_hour { get; set; } = 8; // total working hour a day
        public string? note { get; set; }
    }
}
