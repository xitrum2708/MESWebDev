using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using MESWebDev.Data;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace MESWebDev.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _map;
        private readonly IHttpContextAccessor _hca;
        private readonly ITranslationService _translationService;

        public LanguageService(IMapper map, IHttpContextAccessor hca, AppDbContext context, ITranslationService translationService  )
        {
            _context = context;
            _map = map;
            _hca = hca;
            _translationService = translationService;
        }

        #region ------------------- LANGUAGE  --------------------

        // Language
        public async Task<List<LanguageModel>> GetAllLanguagesAsync()
        {
            var languages = await Task.Run(() => _context.Master_Language.ToList());
            return languages;
        }
        public async Task<List<SelectListItem>> GetLangSL()
        {
            var data = await Task.Run(() => _context.Master_Language
                .Where(l => l.IsActive)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList());
            return data;
        }

        public async Task<LanguageModel> GetLanguageById(int id)
        {
            var data = await _context.Master_Language.FindAsync(id);
            return data;
        }

        public async Task<string> CreateLanguageAsync(LanguageModel lang)
        {
            var data = _context.Master_Language.Where(i => i.Culture == lang.Culture);
            if (data.Any())
            {
                return "Language culture already exists.";
            }

            await _context.Master_Language.AddAsync(lang);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> UpdateLanguageAsync(LanguageModel lang)
        {
            var data = _context.Master_Language.Where(i => i.Culture == lang.Culture && i.Id != lang.Id);
            if (data.Any())
            {
                return "Language culture already exists.";
            }
            _context.Master_Language.Update(lang);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<bool> DeleteLanguageAsync(int Id)
        {
            var langModel = _context.Master_Language.FirstOrDefault(d => d.Id == Id);
            if (langModel == null)
            {
                return false;
            }
            _context.Master_Language.Remove(langModel);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region ---------------- DICTIONARY FOR LANGUAGE -----------------

        public async Task<string> CreateDictionaryAsync(DictionaryDTO dic)
        {
            var checkExist = _context.Master_Language_Dic
                .FirstOrDefault(d => d.Key == dic.Key && d.LangId == dic.LangId);
            if (checkExist != null)
            {
                return "Dictionary key already exists for this language.";
            }
            var dicModel = _map.Map<DictionaryModel>(dic);
            dicModel.CreatedBy = _hca.HttpContext.User.Identity.Name;
            dicModel.CreatedDt = DateTime.Now;
            await _context.Master_Language_Dic.AddAsync(dicModel);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<bool> DeleteDictionaryAsync(int Id)
        {
            var dicModel = _context.Master_Language_Dic.FirstOrDefault(d => d.Id == Id);
            if (dicModel == null)
            {
                return false;
            }
            _context.Master_Language_Dic.Remove(dicModel);
            await _context.SaveChangesAsync();

            // Xóa cache cho ngôn ngữ tương ứng
            var languageCode = await GetLanguageById(dicModel.LangId);
            _translationService.ClearCache(languageCode.Culture);
            return true;
        }

        public async Task<List<DictionaryDTO>> GetDictionaryAsync(DictionaryDTO? dic = null)
        {
            var query = _context.Master_Language_Dic.AsQueryable();
            if(dic != null)
            {
                if (dic.LangId != 0)
                {
                    query = query.Where(d => d.LangId == dic.LangId);
                }
                if (!string.IsNullOrEmpty(dic.Key))
                {
                    query = query.Where(d => d.Key.Contains(dic.Key));
                }
                if (!string.IsNullOrEmpty(dic.Value))
                {
                    query = query.Where(d => d.Value.Contains(dic.Value));
                }
            }
            if (!query.Any())
            {
                return new List<DictionaryDTO>();
            }
            var dicList = await Task.Run(() => query.ToList());
            var dicDTOList = _map.Map<List<DictionaryDTO>>(dicList);

            return dicDTOList;
        }

        public async Task<string> UpdateDictionaryAsync(DictionaryDTO dic)
        {
            var checkExist = _context.Master_Language_Dic
                .FirstOrDefault(d => d.Key == dic.Key && d.LangId == dic.LangId && d.Id != dic.Id);
            if (checkExist != null)
            {
                return "Dictionary key already exists for this language.";
            }
            var dicModel = _context.Master_Language_Dic.FirstOrDefault(d => d.Id == dic.Id);
            if (dicModel == null)
            {
                return "Dictionary entry not found.";
            }
            _map.Map(dic, dicModel);
            dicModel.UpdatedBy = _hca.HttpContext.User.Identity.Name;
            dicModel.UpdatedDt = DateTime.Now;
            _context.Master_Language_Dic.Update(dicModel);
            await _context.SaveChangesAsync();


            // Lưu ngôn ngữ cũ để xóa cache
            var oldLanguageId = dic.LangId;

            // Xóa cache cho ngôn ngữ cũ và mới (nếu thay đổi ngôn ngữ)
            var oldLanguageCode = await GetLanguageById(oldLanguageId);
            _translationService.ClearCache(oldLanguageCode.Culture);

            return string.Empty;
        }

        public async Task<DictionaryDTO> GetDictionaryById(int id)
        {
            var data = await _context.Master_Language_Dic.FindAsync(id);
            return _map.Map<DictionaryDTO>(data);
        }
#endregion
                
    }
}
