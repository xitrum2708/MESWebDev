using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class UserModel
    {
        [Key]
        [Required(ErrorMessage = "Please input user name.")]      
        public string Username { get; set; }

        [Required(ErrorMessage = "Please input password.")]
        public string Password { get; set; } // For create/edit, not displayed in list

        public string Email { get; set; }
        public string? Fullname { get; set; }

        [ForeignKey("Language")]
        public int? LangId { get; set; }
        [ValidateNever]
        public LanguageModel? Language { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        [ValidateNever]
        public RoleModel? Role { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
