using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLineModel
    {
        [Key]
        public string LineCode { get; set; }
        public string? LineName { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? DisplayColor { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
