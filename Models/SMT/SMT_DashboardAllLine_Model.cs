using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Data;

namespace MESWebDev.Models.SMT
{
    public class SMT_DashboardAllLine_Model
    {
        // Danh sách dữ liệu vẽ biểu đồ và hiển thị linh hoạt
        public List<HourOutput> HourlyOutputs { get; set; } = new();
        public List<QtyOverview> QualityOverviews { get; set; } = new();
        public List<TotalAchie> TotalAchievement { get; set; } = new(); // 👈 THÊM MỚI

        // DataTables đổ trực tiếp vào HTML Table
        public DataTable PlanActualLines { get; set; } = new();
        public DataTable QualityInfos { get; set; } = new();
        public DataTable tbQualityOverviews { get; set; } = new();
        public DataTable Stoplog { get; set; } = new();
        // Các thông số đơn lẻ
        public string linerun { get; set; } = "0";
        public string linestop { get; set; } = "0";
        public string lotchangeover { get; set; } = "4";
        public string lotstimeTotal { get; set; } = "0";

    

        // Biến dành riêng cho biểu đồ Achievement Gauge
        public double GaugePercentage { get; set; } = 0;
        public string GaugeTarget { get; set; } = "95%";
    }

    public class HourOutput
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
        public decimal AchievePercent { get; set; } = 0; // 👈 THÊM MỚI
    }

    public class QtyOverview
    {
        public string Label { get; set; } = "";
        public string Today { get; set; } = "";
        public string ThisWeek { get; set; } = "";
        public string ThisMonth { get; set; } = "";
    }

    // 👇 THÊM CLASS MỚI
    public class TotalAchie
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
        public int Target { get; set; }
        public string Achievement { get; set; } = "0%";
    }
}
