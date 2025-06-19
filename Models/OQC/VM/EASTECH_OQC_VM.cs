using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.OQC.VM
{
    public class EASTECH_OQC_VM
    {
        [Required]
        public string QRCode { get; set; }

        [Required]
        public string Status { get; set; } = "OK";

        public string? Remark { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }
}