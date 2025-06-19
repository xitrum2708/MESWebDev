using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.IQC
{
    [Table("UV_IQC_ReportFiles", Schema = "dbo")]
    public class UV_IQC_ReportFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        public DateTime? UploadedDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public virtual UV_IQC_Report Report { get; set; }
    }
}