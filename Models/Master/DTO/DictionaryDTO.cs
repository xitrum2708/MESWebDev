using MESWebDev.Models.Master;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MESWebDev.Models.Master.DTO
{
    public class DictionaryDTO
    {
        [Key]
        public int Id { get; set; }
        public int LangId { get; set; }
        public string LangName { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public string? Note { get; set; }

        public bool IsActive { get; set; }
    }
}
