using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MESWebDev.Services.IService
{
    public interface ILanguageService
    {
        // Get all languages
        Task<List<LanguageModel>> GetAllLanguagesAsync();

        Task<LanguageModel> GetLanguageById(int id);
        Task<string> CreateLanguageAsync(LanguageModel dic);
        Task<string> UpdateLanguageAsync(LanguageModel dic);
        Task<bool> DeleteLanguageAsync(int Id);

        Task<List<SelectListItem>> GetLangSL();

        // Dictionaries by language
        Task<List<DictionaryDTO>> GetDictionaryAsync(DictionaryDTO? dic = null);
        Task<DictionaryDTO> GetDictionaryById(int id);        
        Task<string> CreateDictionaryAsync(DictionaryDTO dic);
        Task<string> UpdateDictionaryAsync(DictionaryDTO dic);
        Task<bool> DeleteDictionaryAsync(int Id);

    }
}
