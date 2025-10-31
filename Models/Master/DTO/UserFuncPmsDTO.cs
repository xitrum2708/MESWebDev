using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master.DTO
{
    public class UserFuncPmsDTO
    {
        public int? id { get; set; }
        [Required]
        public string user_id { get; set; }
        public string full_name { get; set; }
        [Required]
        public int func_id { get; set; }
        public string func_name { get; set; }
        [Required]
        public int pms_id { get; set; }
        public string pms_name { get; set; }

        public bool is_active { get; set; } = true;
    }
}
