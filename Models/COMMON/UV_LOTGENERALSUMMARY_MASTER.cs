using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.COMMON
{
    public class UV_LOTGENERALSUMMARY_MASTER
    {
        [Key]
        public int SummaryID { get; set; }

        [Required(ErrorMessage = "LotNo is required")]
        [MaxLength(50)]
        public string LotNo { get; set; }

        [Required(ErrorMessage = "SerialNumber is required")]
        [MaxLength(50)]
        public string SerialNumber { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("LotNo")]
        public UV_LOTCONTROL_MASTER UV_LOTCONTROL_MASTER { get; set; }
    }
}
