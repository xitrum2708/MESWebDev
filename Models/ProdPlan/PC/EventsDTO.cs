namespace MESWebDev.Models.ProdPlan.PC
{
    public class EventsDTO
    {
        public string? resourceId { get; set; } // line
        public int id { get; set; }
        public int old_id { get; set; } // this will be used when reload again  
        public string line { get; set; }
        public string model { get; set; }
        public string lot_no { get; set; }
        public int lot_size { get; set; }
        public int capa_qty { get; set; }
        public int bal_qty { get; set; }
        public string? backgroundColor { get; set; }
        public string? borderColor { get; set; }
        public DateTime? start { get; set; }
        public DateTime? old_start { get; set; }
        public DateTime? end { get; set; }
        public int qty { get; set; }
        public int? working_hour { get; set; }
        public bool is_new { get; set; } = false;
        public bool is_fpp { get; set; } = false;
    }
}
