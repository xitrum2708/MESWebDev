using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTMachineModel
    {
        [Key]
        public string MachineCode { get; set; }
        
        public string? MachineName { get; set; } = string.Empty;
        public string? Remark { get; set; }

        
        public bool IsActive { get; set; } = true;

    }
}
