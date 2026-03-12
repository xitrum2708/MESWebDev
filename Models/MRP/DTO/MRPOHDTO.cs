using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPOHDTO
    {
        // Item	OhandQty	Location	Location Type
        [Key]
        public int Id { get; set; }
        public string Item { get; set; }
        public int OHQty { get; set; }
        public string Location { get; set; }
        public string LocationType { get; set; }

        public string? UploadedFile { get; set; }

        public string ErrorMsg { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
