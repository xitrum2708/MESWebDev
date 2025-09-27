using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE.DTO
{
    public class OperationDetailDTO
    {
        
        public int OperationId { get; set; }
        public string OperationName { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        //public DateTime CreatedDt { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? UpdatedDt { get; set; }
        //public string? UpdatedBy { get; set; }
    }
}
