using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master.DTO
{
    public class RoleFuncPmsDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int FuncId { get; set; }
        public string? FuncName { get; set; }
        public int PmsId { get; set; }
        public string? PmsName { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }
    }
}
