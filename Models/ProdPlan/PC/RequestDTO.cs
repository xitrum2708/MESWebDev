namespace MESWebDev.Models.ProdPlan.PC
{
    public class RequestDTO
    {
        public List<IFormFile>? Files { get; set; }
        public DateTime? start_sch_dt { get; set; } // start date for running schedule
        public int? year { get; set; }
        public int? month { get; set; }
    }
}
