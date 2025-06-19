using MESWebDev.Common;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.VM
{
    public class ExecutionLogViewModel
    {
        public PagedResult<ExecutionLog> PagedLogs { get; set; }

        [Display(Name = "Action Type")]
        public string ActionType { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> ActionTypes { get; set; }
    }
}