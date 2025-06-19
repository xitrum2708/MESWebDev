namespace MESWebDev.Models.VM
{
    public class MenuTranslationViewModel
    {
        public int MenuId { get; set; }
        public string MenuTitle { get; set; } // For display purposes
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}