using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class OperationDetailModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OperationMaster")]
        public int OperationId { get; set; }
        public OperationModel OperationMaster { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
