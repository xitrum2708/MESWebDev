using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP
{
    public class MRPSPOModel
    {
        //Model	Qty
        [Key]
        public int Id { get; set; }

        public string Model { get; set; }
        public int SPOQty { get; set; }

        public string? UploadedFile { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
