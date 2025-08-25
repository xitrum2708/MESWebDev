using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace MESWebDev.Models
{
    public class DashboardViewModel
    {
        public List<ChartItem> chart_data { get; set; }
        public List<ChartItem2> line_chart { get; set; }
        public List<ChartItem> chart_data2 { get; set; } // For SMT Dashboard
        public List<ChartItem3> bar_line_chart { get; set; } // For SMT Dashboard

        public DataTable detail_data { get; set; }
        public DataTable detail_data2 { get; set; }
        public DataTable sum_data { get; set; }
        public DataTable sum_data2 { get; set; }

        public string? line { get; set; } = "";
        public string? model { get; set; } = "";
        public string? lot { get; set; } = "";
        public List<SelectListItem>? lotList { get; set; }
        public int lot_size { get; set; } = 0;
        public int balance { get; set; } = 0;
        public decimal target1H { get; set; } = 0;
        public decimal losttime { get; set; } = 0;

        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;

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

    public class ChartItem3
    {
        public string Label { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public double Rate { get; set; } 
    }
}
