using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master
{
    public class LanguageModel
    {
        [Key]
        public int Id { get; set; }
        public string Culture { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }        
    }
}
