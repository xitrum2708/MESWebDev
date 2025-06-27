namespace MESWebDev.Models.ProdPlan
{
    public class RequestDTO
    {
        public List<IFormFile>? Files { get; set; }
        public DateTime? start_sch_dt { get; set; } // start date for running schedule
    }
}
