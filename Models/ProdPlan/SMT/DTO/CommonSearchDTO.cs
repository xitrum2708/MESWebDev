namespace MESWebDev.Models.ProdPlan.SMT.DTO
{
    public class CommonSearchDTO
    {
        public int? Id { get; set; }
        public bool IsActive { get; set; } =true;


        // Common Setting
        public string? Property { get; set; }
        public string? Value { get; set; }
        public string? Category { get; set; }
    }
}
