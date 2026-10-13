using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.TELSTAR.VM
{
    public class TONLY_DQC_CTN_VM
    {
        [Required]
        public string Checker { get; set; }
        [Required]
        public string QRCode { get; set; }

        [Required]
        public string Status { get; set; } = "OK";

        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
