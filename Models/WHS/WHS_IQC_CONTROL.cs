using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.WHS
{
    [Table("WHS_IQC_CONTROL")]
    public class WHS_IQC_CONTROL
    {
        [Key]
        [StringLength(50)]
        public string ID { get; set; } = string.Empty;

        [Column("WHS_GIAO_LOTTAG")]
        public DateTime? WhsGiaoLotTag { get; set; }

        [Column("REC_LOCATION")]
        [StringLength(20)]
        public string? RecLocation { get; set; }

        [Column("IQC_NHAN_LOTTAG")]
        public DateTime? IqcNhanLotTag { get; set; }

        [Column("IQC_PART_STATUS")]
        [StringLength(200)]
        public string? IqcPartStatus { get; set; }

        [Column("IQC_DATE_CHECK")]
        public DateTime? IqcDateCheck { get; set; }

        [Column("IQC_PIC_CHECK")]
        [StringLength(50)]
        public string? IqcPicCheck { get; set; }

        [Column("WHS_NHAN_HANG")]
        public DateTime? WhsNhanHang { get; set; }

        [Column("WHS_PIC_REC")]
        [StringLength(50)]
        public string? WhsPicRec { get; set; }

        [StringLength(100)]
        public string? Remark { get; set; }
    }
}