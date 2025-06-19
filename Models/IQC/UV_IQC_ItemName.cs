using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.IQC
{
    [Index(nameof(ItemName), IsUnique = true)]
    [Table("UV_IQC_ItemNames", Schema = "dbo")]
    public class UV_IQC_ItemName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }

        [Required]
        public int GroupID { get; set; }

        [Required]
        [StringLength(255)]
        public string ItemName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Các cột có default value getdate() sẽ được cấu hình trong DbContext thông qua Fluent API nếu cần
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        // Navigation property liên kết tới bảng UV_IQC_InspectionGroups thông qua khóa ngoại GroupID.
        [ForeignKey("GroupID")]
        public virtual UV_IQC_InspectionGroup InspectionGroup { get; set; }
    }
}