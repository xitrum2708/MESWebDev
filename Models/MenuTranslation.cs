namespace MESWebDev.Models
{
    public class MenuTranslation
    {
        public int MenuId { get; set; }
        public int LanguageId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public Menu Menu { get; set; }
        public Language Language { get; set; }
    }
}