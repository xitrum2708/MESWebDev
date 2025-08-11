using System.Data;

namespace MESWebDev.Models
{
    public class DashboardViewModel
    {
        public List<ChartItem> chart_data { get; set; }
        public DataTable detail_data { get; set; }
        public DataTable sum_data { get; set; }
    }
    public class ChartItem
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
}
