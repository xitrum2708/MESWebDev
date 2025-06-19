using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.IQC
{
    [Table("UV_IQC_ErrorsItemMaster", Schema = "dbo")]
    public class UV_IQC_ErrorsItemMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorCodeID { get; set; }

        [Required]
        [StringLength(100)]
        public string ErrorCode { get; set; }

        // Sử dụng nvarchar(500) tương đương với string trong C#
        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string DescriptionVn { get; set; }

        // Các thuộc tính này có ràng buộc NOT NULL và giá trị mặc định là getdate() trong database.
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        public ICollection<UV_IQC_ReportItem> ReportItems { get; set; }
    }
}