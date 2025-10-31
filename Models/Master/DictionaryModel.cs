using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class DictionaryModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Language")]
        public int LangId { get; set; }
        [ValidateNever]
        public LanguageModel Language { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public string? Note { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
