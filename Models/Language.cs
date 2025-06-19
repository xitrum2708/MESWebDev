namespace MESWebDev.Models
{
    public class Language
    {
        public int LanguageId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}