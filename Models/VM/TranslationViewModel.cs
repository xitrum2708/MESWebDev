using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MESWebDev.Models.VM
{
    public class TranslationViewModel : ISearchable<TranslationViewModel>
    {
        public int TranslationId { get; set; }
        public string Keyvalue { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } // Hiển thị mã ngôn ngữ (vi, en, ...)
        public string Value { get; set; }

        // Danh sách ngôn ngữ để hiển thị trong dropdown
        public List<SelectListItem> AvailableLanguages { get; set; }

        public IQueryable<TranslationViewModel> ApplySearch(IQueryable<TranslationViewModel> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            return query.Where(t => t.Keyvalue.Contains(searchTerm) ||
                               t.Value.Contains(searchTerm) ||
                               t.LanguageCode.Contains(searchTerm));
        }
    }
}