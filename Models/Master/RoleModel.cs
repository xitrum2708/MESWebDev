using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; } 
        [Required]
        public string RoleName { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDt { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }
}
