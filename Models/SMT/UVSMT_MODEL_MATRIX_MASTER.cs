using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Models.SMT
{
    [Table("UVSMT_MODEL_MATRIX_MASTER")]
    public class UVSMT_MODEL_MATRIX_MASTER
    {
        [Key]
        public long Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string PCB_No { get; set; } = string.Empty;
        public string PCB_SIDE { get; set; } = string.Empty;
        public string PCB_TYPE { get; set; } = string.Empty;
        public int Board_Pcs_Per_Sheet { get; set; } = 0;
        public int PCB_Per_Model { get; set; } = 0;
        public string RoHS { get; set; } = string.Empty;
        public string? RV_TYPE_1 { get; set; }
        public string? LotNo_1 { get; set; }
        public string? RV_TYPE_2 { get; set; }
        public string? LotNo_2 { get; set; }
        public string? Load_IC_OR_Jig_Check { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Program_Name { get; set; } = string.Empty;
        public string? ADD_Info { get; set; }
        public int Reel_Of_Part_Qty { get; set; } = 0;
        public int Chips_Per_PCS { get; set; } = 0;
        public int Chips_Per_Board { get; set; } = 0;
        public int Chips_Per_Model { get; set; } = 0;
        [Precision(10,1)]
        public decimal CPH { get; set; } = 0;
        [Precision(10, 1)]
        public decimal GXH1_SIM_Time_Seconds { get; set; } = 0;
        public string GXH3_SIM_Time_Seconds { get; set; } =string.Empty;
        [Precision(10, 1)]
        public decimal TACT_Time_Seconds { get; set; } = 0;
        [Precision(10, 1)]
        public int SIM_OUT_PCS_Per_Hour { get; set; } = 0;
        public int Output_1h { get; set; } = 0;
        public int Output_2h { get; set; } = 0;
        public int Output_Day { get; set; } = 0;
        public int Output_Night { get; set; } = 0;
        [Precision(10, 4)]
        public decimal? X_mm { get; set; }
        [Precision(10, 4)]
        public decimal? Y_mm { get; set; }
        [Precision(10, 4)]
        public decimal? T_mm { get; set; }
        public string Mark_LotNo { get; set; } = string.Empty;
        public int Paste_mg { get; set; } = 0;
        public int DIP_mg { get; set; } = 0;
        public string? Finish_Status { get; set; }        
        public string? Remark { get; set; }
        public string? Solder_Type { get; set; }
        public string Key_Work { get; set; }=string.Empty;
        public string CreatedBy { get; set; } =string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
