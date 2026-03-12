using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineUtilizationModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Line")]
        public string LineCode { get; set; }
        public SMTLineModel? Line { get; set; }

        public DateTime? UsageDate { get; set; }
        [Precision(18, 2)]
        public decimal? UsagePercent { get; set; }
        public string? Remark { get; set; }
    }
}
