using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Models.ProdPlan.SMT.DTO
{
    public class SMTProdPlanDTO
    {
        public string Market { get; set; } // Thị Trường --> Get StartDt from SMTPlanModel based on Model and Lotno
        public string Model { get; set; }
        public string PCBKey { get; set; }
        public string Lotno { get; set; } //Lot_cấp_linh_kiện
        public string PCBType { get; set; }
        public string PCBNo { get; set; }
        public string LineCode { get; set; }
        public string MachineCode { get; set; }
        public string KeyCode { get; set; }
        public int PCBPerModel { get; set; } // HS: PCB per Model
        public int LotSize { get; set; } // Lot Size (Qty)
        public int? IssuedQty { get; set; } // Qty_issued  ?? why almost same as BalanceQty
        public int? BalanceQty { get; set; }
        public int? TargetPerHour85 { get; set; } //TARGET_H_85
        [Precision(18, 2)]
        public decimal? TargetPerShift { get; set; }
        [Precision(18, 2)]
        public decimal? TimeF { get; set; }
        public string? Remark { get; set; }

        public DateTime? PlanCompletedDt { get; set; }

        //---dtl
        public DateTime PlanStartDt { get; set; } // StartDt from SMTPlanModel

        public DateTime StartScheduleDate { get; set; } // check Config
        public DateTime StartDt { get; set; } // from SMTPlanModel
        public DateTime EndDt { get; set; } // from SMTPlanModel

        public int? BalQty { get; set; } // Balance Quantity at this detail record
        public int? Qty { get; set; } // Quantity for this detail record
        public int? TimeTotal { get; set; } // Total Time in minutes for this detail record
        public int? SortOrder { get; set; } // To maintain order of detail records
        public string? backgroundColor { get; set; } // For UI Highlighting
        public string? borderColor { get; set; } // For UI Highlighting
        public int? OldId { get; set; } // To track previous record if needed

        public int RunOrder { get; set; } // To track the sequence of execution

        public int SetupMinute { get; set; } = 0;

        public int ShiftWindowMinutes { get; set; } = 0; // To track the shift window in minutes
        public DateTime? WinStartDt { get; set; }

        // add new int
        public int Id { get; set; }
    }

}

