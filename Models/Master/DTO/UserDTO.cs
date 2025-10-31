using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class UserDTO
    {
        [Key]
        [Required(ErrorMessage = "Please input user name.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please input password.")]
        public string Password { get; set; } // For create/edit, not displayed in list

        public string Email { get; set; }
        public string? Fullname { get; set; }

        
        public int? LangId { get; set; }
        public string? LangName { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public bool IsActive { get; set; }
    }
}
