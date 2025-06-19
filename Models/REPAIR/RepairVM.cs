using MESWebDev.Common;
using MESWebDev.DTO;

namespace MESWebDev.Models.REPAIR
{
    public class RepairVM
    {
        public PagedResult<RepairResultDto> RepairResult { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public string? UserDept { get; set; }
        public List<string> UserDeptList { get; set; } = new List<string>();
    }
}