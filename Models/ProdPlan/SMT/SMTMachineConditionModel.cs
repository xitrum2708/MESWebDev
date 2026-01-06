using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTMachineConditionModel
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Machine")]
        public string MachineCode { get; set; }
        public SMTMachineModel? Machine { get; set; }
        public int ChipMin { get; set; }
        public int ChipMax { get; set; }
        public string? Remark { get; set; }
        public int Priority { get; set; }

    }
}
