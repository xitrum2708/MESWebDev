using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyHdrDTO
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required !")]
        public string Customer { get; set; }

        [Required(ErrorMessage = "This field is required !")]
        public string Section { get; set; }

        [Required(ErrorMessage = "This field is required !")]
        public string Model { get; set; }
        public string? BModel { get; set; }
        public string LotNo { get; set; }
        public string? Unit { get; set; }
        public string? PcbName { get; set; }
        public string? PcbNo { get; set; }
      
        public bool IsActive { get; set; } = true;

    }
}
