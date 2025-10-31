using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master.DTO
{
    public class PmsActionDTO
    {
        public int Id { get; set; }
        public int PmsId { get; set; }
        public int ActionId { get; set; }
        public string? Note { get; set; }
    }
}
