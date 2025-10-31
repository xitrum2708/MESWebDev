using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MESWebDev.Models.VM;

namespace MESWebDev.Models.Master
{
    public class FunctionModel
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public int Order { get; set; }
        public string EnName { get; set; }
        //public string? name_ja { get; set; }
        public string? ViName { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? IconString { get; set; }
        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }

        // public string? role_id { get; set; } = "ALL";// maybe list of role: ADMI;IS01
        public FunctionModel? Parent { get; set; }

        public List<FunctionModel> Children { get; set; } = new(); // child menu
        [NotMapped]
        public List<FunctionModel> AvailableParents { get; set; } = new();

        [NotMapped]
        public int Level { get; set; } = 0;
    }
}
