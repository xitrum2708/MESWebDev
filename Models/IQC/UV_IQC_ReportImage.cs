using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.IQC
{
    [Table("UV_IQC_ReportImages", Schema = "dbo")]
    public class UV_IQC_ReportImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string FileExtention { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        // Thuộc tính UploadedDate có giá trị mặc định là getdate() khi insert dữ liệu
        public DateTime? UploadedDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property: nếu bạn có model UV_IQC_Report
        [ForeignKey("ReportID")]
        public virtual UV_IQC_Report Report { get; set; }
    }
}