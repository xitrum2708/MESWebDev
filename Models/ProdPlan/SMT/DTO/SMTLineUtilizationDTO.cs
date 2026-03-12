using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT.DTO
{
    public class SMTLineUtilizationDTO
    {
        public DateTime? UsageDate { get; set; }
        public string LineCode { get; set; }
        
        public decimal? UsagePercent { get; set; }
        public string? Remark { get; set; }
    }
}
