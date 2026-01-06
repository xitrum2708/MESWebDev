using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineMachineDataModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Line")]
        public string LineCode { get; set; }
        public SMTLineModel? Line { get; set; }

        [ForeignKey("Machine")]
        public string MachineCode { get; set; }
        public SMTMachineModel? Machine { get; set; }

        public DateTime? UsageDate { get; set; }
        [Precision(18, 2)]
        public decimal? UsagePercent { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }
}
