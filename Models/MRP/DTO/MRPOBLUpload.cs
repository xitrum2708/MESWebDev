using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPOBLUpload
    {
        public string Item { get; set; }
        public int OBLQty { get; set; }
        public string PONo { get; set; }

        public string? UploadedFile { get; set; }

        public string ErrorMsg { get; set; }
    }
}
