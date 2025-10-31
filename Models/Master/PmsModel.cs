using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master
{
    public class PmsModel
    {
        [Key]
        public int Id { get; set; }
        //public string pms_id { get; set; }
        public string PmsName { get; set; }        
        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
}
