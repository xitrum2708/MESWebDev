using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class UserFuncPmsSearchDTO
    {
        public string? user_id { get; set; }
        public string? full_name { get; set; }
        public int? func_id { get; set; }
        public string? func_name { get; set; }
        public int? pms_id { get; set; }
        public string? pms_name { get; set; }
    }
}
