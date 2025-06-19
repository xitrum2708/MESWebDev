using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.VM
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please input user name.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please input password.")]
        public string Password { get; set; } // For create/edit, not displayed in list

        public string Email { get; set; }
        public string? FullName { get; set; }
        public int? LanguageId { get; set; }
        public string LanguageName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}