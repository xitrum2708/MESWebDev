using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class UserFuncPmsModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [ValidateNever]
        public UserModel User { get; set; }

        [ForeignKey("Function")]
        public int FuncId { get; set; }
        [ValidateNever]
        public FunctionModel Function { get; set; }

        [ForeignKey("PmsModel")]
        public int PmsId { get; set; }
        [ValidateNever]
        public PmsModel PmsModel { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }

        public string? CreeatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
