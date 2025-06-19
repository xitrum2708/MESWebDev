namespace MESWebDev.Models
{
    public class Translation
    {
        public int TranslationId { get; set; }
        public string Keyvalue { get; set; }
        public int LanguageId { get; set; }
        public string Value { get; set; }

        public Language Language { get; set; }
    }
}