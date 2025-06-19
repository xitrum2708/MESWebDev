using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.WHS
{
    [Table("WHS_RECEIVING_TOTAL")]
    public class WHS_RECEIVING_TOTAL
    {       
        [Column("ID")]
        [StringLength(50)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        [Column("SK_INVOICE")]
        [StringLength(50)]
        public string? SK_INVOICE { get; set; }

        [Column("RECEIVED_DATE")]
        public DateTime? RECEIVED_DATE { get; set; }

        [Column("MFG")]
        public DateTime? MFG { get; set; }

        [Column("SUPPLIER_INVOICE")]
        [StringLength(50)]
        public string? SUPPLIER_INVOICE { get; set; }

        [Column("SUPPLIER")]
        [StringLength(50)]
        public string? SUPPLIER { get; set; }

        [Column("SUPPLIER_CODE")]
        [StringLength(10)]
        public string? SUPPLIER_CODE { get; set; }

        [Column("SUPPLIER_GROUP")]
        [StringLength(20)]
        public string? SUPPLIER_GROUP { get; set; }

        [Column("PART_CODE")]
        [StringLength(50)]
        public string? PART_CODE { get; set; }

        [Column("PART_DESC")]
        [StringLength(50)]
        public string? PART_DESC { get; set; }

        [Column("SPEC")]
        [StringLength(100)]
        public string SPEC { get; set; }

        [Column("PURCHASE_ORDER")]
        [StringLength(500)]
        public string? PURCHASE_ORDER { get; set; }

        [Column("QTY")]
        public double? QTY { get; set; }

        [Column("QtyOK")]
        public double? QtyOK { get; set; }

        [Column("QtyNG")]
        public double? QtyNG { get; set; }

        [Column("PART_LOCATION")]
        [StringLength(50)]
        public string? PART_LOCATION { get; set; }

        [Column("IQC_STATUS")]
        [StringLength(10)]
        public string? IQC_STATUS { get; set; }

        [Column("PACKING")]
        [StringLength(100)]
        public string? PACKING { get; set; }

        [Column("USE_DATE")]
        public DateTime? USE_DATE { get; set; }

        [Column("LOT_NO")]
        [StringLength(50)]
        public string? LOT_NO { get; set; }

        [Column("REMARK")]
        [StringLength(150)]
        public string? REMARK { get; set; }

        [Column("PRINTDATE")]
        public DateTime? PRINTDATE { get; set; }

        [Column("UPDATEBY")]
        [StringLength(200)]
        public string? UPDATEBY { get; set; }

        [Column("Fileupload")]
        [StringLength(100)]
        public string? Fileupload { get; set; }

        [Column("PART_STATUS")]
        [StringLength(200)]
        public string? PART_STATUS { get; set; }

        [Column("CREATEDATE")]
        public DateTime? CREATEDATE { get; set; }

        [Column("AppvSpec")]
        [StringLength(100)]
        public string? AppvSpec { get; set; }

        [Column("QRcodecustomer")]
        [StringLength(2000)]
        public string? QRcodecustomer { get; set; }

        //public virtual WHS_IQC_CONTROLModel WHS_IQC_CONTROLModel { get; set; }
    }
}