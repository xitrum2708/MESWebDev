using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTPlanModel
    {
        [Key]
        public int Id { get; set; }
        public string Model { get; set; }
        public string Lotno { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public int Size { get; set; }
        public int Balance { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
//Id	Model	Lotno	StartDt	EndDt	Size	Balance	CreatedDt	CreatedBy	UpdatedDt	UpdatedBy
