using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.REPAIR
{
    [Table("UV_REPAIRRESULT")]
    public class UV_REPAIRRESULT
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(50)]
        public string Qrcode { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [StringLength(50)]
        public string Lot { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        [DefaultValue(0)]
        public int DailyOutput { get; set; } = 0;

        [StringLength(100)]
        public string? PcbCode { get; set; }

        [StringLength(50)]
        public string? Pcbtype { get; set; }

        [StringLength(50)]
        public string? Process { get; set; }

        [StringLength(50)]
        public string? Errorposition { get; set; }

        [StringLength(100)]
        public string? Partcode { get; set; }

        [StringLength(100)]
        public string? Errortype { get; set; }

        [StringLength(100)]
        public string? Causetype { get; set; }

        [StringLength(100)]
        public string? DeptError { get; set; }

        [StringLength(100)]
        public string? Repairmethod { get; set; }

        [StringLength(100)]
        public string? Statusresult { get; set; }

        [StringLength(100)]
        public string? UserDept { get; set; }

        [StringLength(100)]
        public string? Linename { get; set; }

        public int? Qty { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [StringLength(250)]
        public string? Remark { get; set; }

        [StringLength(100)]
        public string? Soldermachine { get; set; }

        [StringLength(100)]
        public string? Tinwire { get; set; }

        [StringLength(100)]
        public string? Flux { get; set; }

        [StringLength(100)]
        public string? Alcohol { get; set; }

        [StringLength(100)]
        public string? Other { get; set; }

        public DateTime? DDRDate { get; set; }

        [StringLength(100)]
        public string? DDRKeyin { get; set; }

        [StringLength(50)]
        public string? DDRCHECK { get; set; }

        [Column(TypeName = "int")]
        [DefaultValue(0)]
        public int DDRDailyUpdate { get; set; } = 0;
    }
}