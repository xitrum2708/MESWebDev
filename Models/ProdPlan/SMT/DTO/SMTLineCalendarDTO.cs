namespace MESWebDev.Models.ProdPlan.SMT.DTO
{
    public class SMTLineCalendarDTO
    {
        public string LineCodeOne { get; set; }
        public string ShiftCodeOne { get; set; }
        public int Priority { get; set; }
        public string ConditionType { get;set; }
        public string? Weekday { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }
    }
}
