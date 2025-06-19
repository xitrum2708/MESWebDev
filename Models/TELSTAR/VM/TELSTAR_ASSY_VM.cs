using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.TELSTAR.VM
{
    public class TELSTAR_ASSY_VM
    {
        [Required]
        public string SelectedLotNo { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string SelectedLine { get; set; }

        [Required]
        public string QRCode { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public List<SelectListItem> LotNoList { get; set; }
        public List<SelectListItem> LineList { get; set; }
        public int QRCodesTodayByUser { get; set; } = 0;
        public List<TelstarAssyVM> ScannedData { get; set; } = new List<TelstarAssyVM>();
    }
}