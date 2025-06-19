namespace MESWebDev.Models
{
    public class ExecutionLog
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? DurationMs { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}