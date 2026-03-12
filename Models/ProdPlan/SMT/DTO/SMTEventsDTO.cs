using MESWebDev.Models.ProdPlan.SMT;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.ProdPlan.PC
{
    public class SMTEventsDTO
    {
        public string? resourceId { get; set; } // LineCode
        public int id { get; set; }
        public int OldId { get; set; } // this will be used when reload again  
        public string LineCode { get; set; }
        public string Model { get; set; }
        public string Lotno { get; set; }
        public string PCBKey { get; set; }
        public string PCBType { get; set; }
        public string PCBNo { get; set; }
        public string MachineCode { get; set; }
        public string KeyCode { get; set; }
        public int PCBPerModel { get; set; } // HS: PCB per Model
        public int LotSize { get; set; } // Lot Size (Qty)
        public int IssuedQty { get; set; } // Qty_issued  ?? why almost same as BalanceQty
        public int BalanceQty { get; set; }
        public int TargetPerHour85 { get; set; } //TARGET_H_85

        public DateTime PlanStartDt { get; set; } // StartDt from SMTPlanModel

        public DateTime StartDt { get; set; }
        public DateTime? OldStartDt { get; set; }
        public DateTime EndDt { get; set; }

        public string? start { get; set; }
        public string? end { get; set; }

        public int Qty { get; set; }
        public int SortOrder { get; set; }
        public int RunOrder { get; set; } // To track the sequence of execution

        public int ShiftWindowMinutes { get; set; } // To track the shift window in minutes
        public DateTime? WinStartDt { get; set; }
        public int SetupMinute { get; set; } = 0;

        public string? backgroundColor { get; set; }
        public string? borderColor { get; set; }
    }
}
