using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.Master
{
    public class ActionModel
    {
        [Key]
        public int Id { get; set; }
        //public string action_id { get; set; }
        public string ActionName { get; set; }
        public bool IsActive { get; set; }
        public string? Note { get; set; }

    }
}
