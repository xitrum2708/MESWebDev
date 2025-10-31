using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class RoleFuncPmsModel
    {
        [Key]

        public int Id { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        [ValidateNever]
        public RoleModel Role { get; set; }

        [ForeignKey("Function")]
        public int FuncId { get; set; }
        [ValidateNever]
        public FunctionModel Function { get; set; }

        [ForeignKey("Pms")]
        public int PmsId { get; set; }
        [ValidateNever]
        public PmsModel Pms { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }


        public string? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
