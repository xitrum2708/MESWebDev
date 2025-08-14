using System.Data;

namespace MESWebDev.Models
{
    public class DashboardViewModel
    {
        public List<ChartItem> chart_data { get; set; }
        public List<ChartItem2> line_chart { get; set; }
        public List<ChartItem> chart_data2 { get; set; } // For SMT Dashboard

        public DataTable detail_data { get; set; }
        public DataTable detail_data2 { get; set; }
        public DataTable sum_data { get; set; }
        public DataTable sum_data2 { get; set; }
    }
    public class ChartItem
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
    public class ChartItem2
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
    }
}
