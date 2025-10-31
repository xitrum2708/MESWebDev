using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master.DTO
{
    public class FunctionSearchDTO
    {
        public int? Id { get; set; }

        public int? ParentId { get; set; }
        public int? Order { get; set; }
        public string? EnName { get; set; }
        //public string? name_ja { get; set; }
        public string? ViName { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
    }
}
