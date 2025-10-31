using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class RoleDTO
    {
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string? Note { get; set; }
    }
}
