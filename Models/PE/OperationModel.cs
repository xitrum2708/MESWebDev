using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.PE
{
    public class OperationModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required !")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
