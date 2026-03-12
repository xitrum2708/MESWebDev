using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPBOMUpload
    {
        public string Model { get; set; }
        public string Item { get; set; }
        public int BOMQty { get; set; }
        public string DrawingPstNo { get; set; }

        public string? UploadedFile { get; set; }

        public string ErrorMsg { get; set; }

    }
}
