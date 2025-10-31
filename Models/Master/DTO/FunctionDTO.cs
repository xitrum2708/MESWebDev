using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class FunctionDTO
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

        // public string? role_id { get; set; } = "ALL";// maybe list of role: ADMI;IS01


        public bool IsActive { get; set; } = true;
    }
}
