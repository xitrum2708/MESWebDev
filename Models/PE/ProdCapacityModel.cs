
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProdCapacityModel
{
    [Key]
    public int Id { get; set; }

    [DisplayName("Type")]
    public string Type { get; set; }

    [DisplayName("Customer1")]
    public string? Customer1 { get; set; }

    [DisplayName("Customer2")]
    public string? Customer2 { get; set; }

    [DisplayName("U-No")]
    public string? UNo { get; set; }

    [DisplayName("B-Model")]
    public string? BModel { get; set; }

    [DisplayName("PCB No")]
    public string? PCBNo { get; set; }

    [DisplayName("PCB Type")]
    public string? PCBType { get; set; }

    [DisplayName("UpdateDate")]
    public DateTime? UpdateDate { get; set; }

    [DisplayName("SMT Points Total")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal SMTPointsTotal { get; set; } = 0;

    [DisplayName("COB HC")]
    public decimal COBHC { get; set; } = 0;

    [DisplayName("COB Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal COBOutput { get; set; } = 0;

    [DisplayName("SMT HC")]
    public decimal SMTHC { get; set; } = 0;

    [DisplayName("SMT Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal SMTOutput { get; set; } = 0;

    [DisplayName("Insert HC")]
    public decimal InsertHC { get; set; } = 0;

    [DisplayName("Insert Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal InsertOutput { get; set; } = 0;

    [DisplayName("SC HC")]
    public decimal SCHC { get; set; } = 0;

    [DisplayName("SC Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal SCOutput { get; set; } = 0;

    [DisplayName("Assy HC")]
    public decimal AssyHC { get; set; } = 0;

    [DisplayName("Assy Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal AssyOutput { get; set; } = 0;

    [DisplayName("Mic HC")]
    public decimal MicHC { get; set; } = 0;

    [DisplayName("Mic Output")]
    [Column(TypeName = "decimal(18,6)")]
    public decimal MicOutput { get; set; } = 0;

    [DisplayName("Other HC")]
    public decimal OtherHC { get; set; } = 0;

    [DisplayName("Other Output")]
    public decimal OtherOutput { get; set; } = 0;

    [DisplayName("SMT Top")]
    public int SMTTop { get; set; } = 0;

    [DisplayName("SMT Bottom")]
    public int SMTBottom { get; set; } = 0;

    [DisplayName("SMT Total")]
    public int SMTTotal { get; set; } = 0;

    [DisplayName("Insert")]
    public int Insert { get; set; } = 0;

    [DisplayName("Pcs Per Sheet")]
    public int PcsPerSheet { get; set; } = 0;
    public string? UploadFile { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedDt { get; set; }
    public string? UpdatedBy { get; set; }
}


