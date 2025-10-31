using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class FunctionCreateDTO
    {
        public int? parent_id { get; set; }
        //[Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        //[Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        [Required]
        public int order { get; set; }
        [Required]
        public string name_en { get; set; }
        [Required]
        public string name_vi { get; set; }
        public string? name_ja { get; set; }
        public string? controller { get; set; }
        public string? action { get; set; }
        public string? icon_string { get; set; }
        public string? role_id { get; set; }
        public string? created_by { get; set; }  
    }
}
