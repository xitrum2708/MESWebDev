using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.IQC
{
    [Table("UV_IQC_RESULT")]
    public class UV_IQC_RESULT
    {
        [Column("seq")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long seq { get; set; }

        [Key]
        [Column("id")]
        [StringLength(50)]
        public string id { get; set; } = string.Empty;

        [Column("yusen_invno")]
        [StringLength(350)]
        public string? yusen_invno { get; set; }

        [Column("rcv_date")]
        public DateTime? rcv_date { get; set; }

        [Column("invoice")]
        [StringLength(350)]
        public string? invoice { get; set; }

        [Column("vender_name")]
        [StringLength(350)]
        public string? vender_name { get; set; }

        [Column("vender_code")]
        [StringLength(320)]
        public string? vender_code { get; set; }

        [Column("abbre_group")]
        [StringLength(350)]
        public string? abbre_group { get; set; }

        [Column("partcode")]
        [StringLength(350)]
        public string? partcode { get; set; }

        [Column("partname")]
        [StringLength(150)]
        public string? partname { get; set; }

        [Column("partspec")]
        [StringLength(250)]
        public string? partspec { get; set; }

        [Column("purchase_order")]
        [StringLength(500)]
        public string? purchase_order { get; set; }

        [Column("orgcount_pono")]
        public int? orgcount_pono { get; set; } = 0;

        [Column("count_pono")]
        public int? count_pono { get; set; } = 0;

        [Column("orgqty")]
        public int? orgqty { get; set; } = 0;

        [Column("qty")]
        public int? qty { get; set; } = 0;

        [Column("location_rec")]
        [StringLength(150)]
        public string? location_rec { get; set; }

        [Column("iqc_status")]
        [StringLength(100)]
        public string? iqc_status { get; set; }

        [Column("iqc_rec_lottag")]
        public DateTime? iqc_rec_lottag { get; set; }

        [Column("iqc_rec_person")]
        [StringLength(150)]
        public string? iqc_rec_person { get; set; }

        [Column("status_lottag")]
        [StringLength(150)]
        public string? status_lottag { get; set; }

        [Column("part_group")]
        [StringLength(150)]
        public string? part_group { get; set; }

        [Column("name_inspection")]
        [StringLength(150)]
        public string? name_inspection { get; set; }

        [Column("inspection_date")]
        public DateTime? inspection_date { get; set; }

        [Column("iqc_report_no")]
        [StringLength(20)]
        public string? iqc_report_no { get; set; }

        [Column("judment")]
        [StringLength(20)]
        public string? judment { get; set; }

        [Column("checked_rec_date")]
        public DateTime? checked_rec_date { get; set; }

        [Column("checkedby")]
        [StringLength(150)]
        public string? checkedby { get; set; }

        [Column("checkeddate")]
        public DateTime? checkeddate { get; set; }

        [Column("checkedremark", TypeName = "nvarchar(max)")]
        public string? checkedremark { get; set; }

        [Column("ngmettingresult")]
        [StringLength(100)]
        public string? ngmettingresult { get; set; }

        [Column("lotno")]
        [StringLength(10)]
        public string? lotno { get; set; }

        [Column("lot_shortage")]
        public int? lot_shortage { get; set; }

        [Column("judment_approve")]
        [StringLength(50)]
        public string? judment_approve { get; set; }

        [Column("approve_rec_date")]
        public DateTime? approve_rec_date { get; set; }

        [Column("approveby")]
        [StringLength(150)]
        public string? approveby { get; set; }

        [Column("approvedate")]
        public DateTime? approvedate { get; set; }

        [Column("approveremark", TypeName = "nvarchar(max)")]
        public string? approveremark { get; set; }

        [Column("judment_send")]
        [StringLength(50)]
        public string? judment_send { get; set; }

        [Column("send_rec_date")]
        public DateTime? send_rec_date { get; set; }

        [Column("sendby")]
        [StringLength(150)]
        public string? sendby { get; set; }

        [Column("sendtype")]
        [StringLength(100)]
        public string? sendtype { get; set; }

        [Column("senddate")]
        public DateTime? senddate { get; set; }

        [Column("sendremark", TypeName = "nvarchar(max)")]
        public string? sendremark { get; set; }

        [Column("remarkfinal")]
        [StringLength(500)]
        public string? remarkfinal { get; set; }

        [Column("finaljudment")]
        [StringLength(50)]
        public string? finaljudment { get; set; }

        [Column("combinelottag", TypeName = "nvarchar(max)")]
        public string? combinelottag { get; set; }

        // public virtual WHS_IQC_CONTROLModel WHS_IQC_CONTROLModel { get; set; }
    }
}