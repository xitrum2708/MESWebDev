namespace MESWebDev.Services
{
    public interface ITranslationService
    {
        string GetTranslation(string key, string languageCode);
        string Trans(string key);

        void ClearCache(string languageCode);
    }
}