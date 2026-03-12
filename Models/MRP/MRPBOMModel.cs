using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP
{
    public class MRPBOMModel
    {
        //Model	Item	Qty_set	Drawing Position No
        [Key]  
        public int Id { get; set; }
        public string Model { get; set; }
        public string Item { get; set; }
        public int BOMQty { get; set; }
        public string DrawingPstNo { get; set; }

        public string? UploadedFile { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }

    }
}
