using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan.SMT
{
    public class SMTLotPcbModel
    {
        [Key]
        public int Id { get; set; }
        public string Model { get; set; }
        public string Lotno { get; set; }
        public string PCBNo { get; set; }
        public string? UploadedFile { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedDt { get; set; }
        public string CreatedBy { get; set; }
    }
}
//Id	Model	Lotno	PCBNo	Remark	CreatedDt	CreatedBy	UpdatedDt	UpdatedBy
