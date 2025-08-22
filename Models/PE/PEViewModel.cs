using System.Data;

namespace MESWebDev.Models.PE
{
    public class PEViewModel
    {
        public DataTable? data { get; set; }
        public ManpowerModel? manpower { get; set; } 

        public string? error_msg { get; set; }
    }
}
