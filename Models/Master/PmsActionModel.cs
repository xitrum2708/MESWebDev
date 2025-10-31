using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.Master
{
    public class PmsActionModel
    {
        [Key]
        public int Id { get; set; }
        //public string pms_id { get; set; }

        [ForeignKey("PmsModel")]
        public int PmsId { get; set; }
        [ValidateNever]
        public PmsModel PmsModel { get; set; }

        [ForeignKey("ActionModel")]
        public int ActionId { get; set; }
        [ValidateNever]
        public ActionModel ActionModel { get; set; }

        public string? Note { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
    }
}
