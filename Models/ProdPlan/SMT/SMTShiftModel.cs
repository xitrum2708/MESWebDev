using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTShiftModel
    {
        [Key]
        public string ShiftCode { get; set; }
        public string? ShiftName { get; set; }
        public string Pattern { get; set; }
        public int Priority { get; set; }
        public string? Remark { get; set; }
    }
}
