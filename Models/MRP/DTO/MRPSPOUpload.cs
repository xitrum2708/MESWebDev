using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPSPOUpload
    {

        public string Model { get; set; }
        public int SPOQty { get; set; }
        public DateTime PlanShipDt { get; set; }

        public string? UploadedFile { get; set; }

        public string ErrorMsg { get; set; }
    }
}
