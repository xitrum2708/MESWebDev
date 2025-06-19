using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.VM
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}