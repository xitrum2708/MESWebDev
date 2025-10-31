using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class UserEditDTO
    {
        public string user_id { get; set; }
       
        public string? first_name { get; set; }
 
        public string? last_name { get; set; }

        public string? full_name { get; set; }
        public string? role_name { get; set; }

        public string? email { get; set; }

        public string? phone_number { get; set; }

        public string? role_id { get; set; }      
      
        public string? division { get; set; }
        public string? department { get; set; }
        public string? section { get; set; }
        public string? position { get; set; }
        public bool is_logged_in { get; set; }
        public bool is_approval { get; set; }

       
        public string? sex { get; set; }
        public string? ID_number { get; set; }
        public DateTime? birthday { get; set; }
        public string? native_address { get; set; }
        public DateTime? hire_dt { get; set; }
        public string? account_no { get; set; }
        //public byte[]? photo_image { get; set; }

        // New 2014/04/19
        public string? card_no { get; set; }
        public string? updated_by { get; set; }
    }
}
