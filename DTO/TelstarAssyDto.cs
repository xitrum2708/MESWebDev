using System.ComponentModel.DataAnnotations;

namespace MESWebDev.DTO
{
    public class TelstarAssyDto
    {
        [Required]
        public string SelectedLotNo { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string SelectedLine { get; set; }

        [Required]
        public string QRCode { get; set; }
    }
}