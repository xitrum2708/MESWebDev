using DocumentFormat.OpenXml.Office2021.Drawing.SketchyShapes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTProdPlanDtlModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SMTProdPlanHdrModel")]
        public int HeaderId { get; set; }
        public SMTProdPlanHdrModel? SMTProdPlanHdrModel { get; set; }

        public string Lotno { get; set; } // from SMTPlanModel
        public string Model { get; set; } // from SMTPlanModel
        public string PCBKey { get; set; }
        public string LineCode { get; set; } // from SMTProdPlanModel

        public DateTime StartScheduleDate { get; set; } // check Config
        public DateTime StartDt { get; set; } // from SMTPlanModel
        public DateTime EndDt { get; set; } // from SMTPlanModel
        
        public int BalQty { get; set; } // Balance Quantity at this detail record
        public int Qty { get; set; } // Quantity for this detail record
        public int TimeTotal { get; set; } // Total Time in minutes for this detail record
        public int SortOrder { get; set; } // To maintain order of detail records
        public string? backgroundColor { get; set; } // For UI Highlighting
        public string? borderColor { get; set; } // For UI Highlighting
        public int OldId { get; set; } // To track previous record if needed


        public int RunOrder { get; set; } // To track the sequence of execution

        public int SetupMinute { get; set; } = 0;

        public int ShiftWindowMinutes { get; set; } = 0; // To track the shift window in minutes
        public DateTime? WinStartDt { get; set; }

    }
}
