using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.IQC
{
    public class UV_IQC_InspectionGroup
    {
        [Key]
        public int InspectionGroupID { get; set; }

        public string GroupName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ICollection<UV_IQC_Report> Reports { get; set; } = new List<UV_IQC_Report>();
    }
}