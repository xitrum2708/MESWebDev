using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class UserCreateDTO
    {

        [Required]
        public string user_id { get; set; }
        [Required]
        public string user_code { get; set; }

        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }

        public string email { get; set; }
        public string? division { get; set; }
        public string? department { get; set; }
        public string? section { get; set; }
        public string? position { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Maximum length is 11 characters")]
        public string phone_number { get; set; }

        [Required]
        public string role_id { get; set; }

        public string? dept_id { get; set; }
        [Required]
        public string password { get; set; }
        public bool valid_user { get; set; }
        public bool is_logged_in { get; set; }

        public DateTime? last_login { get; set; }
        public DateTime? updated_date { get; set; }
    }
}
