namespace MESWebDev.Services
{
    public interface ITranslationService
    {
        string GetTranslation(string key, string languageCode);

        void ClearCache(string languageCode);
    }
}