using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.TELSTAR.VM
{
    public class TELSTAR_DQC_VM
    {
        [Required]
        public string QRCode { get; set; }

        [Required]
        public string Status { get; set; } = "OK";

        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }
}