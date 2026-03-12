using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPOHUpload
    {
        
        public string Item { get; set; }
        public int OHQty { get; set; }
        public string Location { get; set; }
        public string LocationType { get; set; }

        public string? UploadedFile { get; set; }

        public string ErrorMsg { get; set; }
    }
}
