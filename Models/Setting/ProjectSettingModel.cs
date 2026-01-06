using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Setting
{
    public class ProjectSettingModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Property { get; set; }// DatetimeFormat, DecimalFormat, etc
        [Required]
        public string Value { get; set; } // Format string "yyyy-MM-dd HH:mm:ss", "#,##0.00", etc
        public string Category { get; set; } = string.Empty; // DateTime, Number, UI, etc
        public string? Remark { get; set; } // Additional notes or description
        public bool IsActive { get; set; } = true; // Indicates if the setting is currently active


        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }


        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
                
    }
}
