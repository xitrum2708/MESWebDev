using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTProdPlanModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LineCode { get; set; }       
        public string Market { get; set; } // Thị Trường --> Get StartDt from SMTPlanModel based on Model and Lotno

        public DateTime StartScheduleDate { get; set; } // check Config
        public DateTime StartDt { get; set; } // from SMTPlanModel
        public DateTime EndDt { get; set; } // from SMTPlanModel

        public string BModel { get; set; }
        public string PcbKey { get; set; }
        public string ComponentLot { get; set; } //Lot_cấp_linh_kiện
        public string PcbType { get; set; }
        public string PcbNo { get; set; }
        public string Machine { get; set; }
        public string KeyCode { get; set; }
        public int PcbPerModel { get; set; } // HS: Pcb per Model
        public int LotSize { get; set; } // Lot Size (Qty)
        public int IssuedQty { get; set; } // Qty_issued  ?? why almost same as BalanceQty
        public int BalanceQty { get; set; }
        public int TargetPerHour85 { get; set; } //TARGET_H_85
        [Precision(18, 2)]
        public decimal TargetPerShift { get; set; }
        [Precision(18, 2)]
        public decimal TimeF { get; set; }
        public string? Remark { get; set; }
        public string? Warning { get; set; } // Cảnh báo
        public int? ExcessStock { get; set; } // STOCK_HÀNG_THỪA

        public string? BackgroundColor { get; set; } // For UI Highlighting
        public string? BorderColor { get; set; } // For UI Highlighting
        public int OldId { get; set; } // To track previous record if needed

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
}
/*
Line	Line
Thị Trường	Market
B-MODEL	BModel
PCB KEY	PcbKey
Lot cấp linh kiện	ComponentLot
Pcb type	PcbType
PCB NO	PcbNo


Machine	Machine
KEY	KeyCode
HS	HS
Qty	Quantity
Qty issued	QuantityIssued
Balance	Balance
TARGET/H 85%	TargetPerHour85
TARGET /SHIFT	TargetPerShift
TIME F	TimeF
Remark	Remark
Cảnh báo	Warning
STOCK HÀNG THỪA 14-10	ExcessStock_14_10
*/