using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP
{
    public class MRPOBLModel
    {
        //Item	OBL Qty	PO No

        [Key]
        public int Id { get; set; }
        public string Item { get; set; }
        public int OBLQty { get; set; }
        public string PONo { get; set; }

        public string? UploadedFile { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
