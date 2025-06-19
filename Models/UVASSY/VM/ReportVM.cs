using System.Data;
using System.Globalization;

namespace MESWebDev.Models.UVASSY.VM
{
    public class ReportVM
    {
        public int totalRecord = 0;
        public string? Model { get; set; } = "";
        public List<int> YearList { get; set; } = new();
        public int SelectedYear { get; set; }
        public int startWeek { get; set; }
        public int endWeek { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public List<MonthlyData>? MonthlyData { get; set; }
        public List<WeeklyData>? WeeklyData { get; set; }
        public List<ModelData>? ModelData { get; set; }

        public DataTable MonthlyDetailTable { get; set; } = new DataTable();
        public DataTable WeeklyDetailTable { get; set; } = new DataTable();
        public DataTable ModelDetailTable { get; set; } = new DataTable();
        public DataTable DeptDetailTable { get; set; } = new DataTable();
        public DataTable ErrorDetailTable { get; set; } = new DataTable();
        public DataTable PartDetailTable { get; set; } = new DataTable();
        public DataTable ExportTopError { get; set; } = new DataTable();
        public List<DeptData>? DeptData { get; set; }
        public List<ErrorData>? ErrorData { get; set; }
        public List<PartData>? PartData { get; set; }

        // Constructor hoặc phương thức để tính toán tuần
        public ReportVM()
        {
            // Lấy tuần hiện tại
            int currentWeek = GetCurrentWeek();

            // Gán giá trị tuần hiện tại và tuần bắt đầu
            endWeek = currentWeek;
            startWeek = Math.Max(currentWeek - 7, 1); // Đảm bảo tuần không nhỏ hơn 1
        }

        // Hàm tính tuần hiện tại
        private int GetCurrentWeek()
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var calendar = currentCulture.Calendar;
            DateTime currentDate = DateTime.Now;

            // Lấy tuần hiện tại
            return calendar.GetWeekOfYear(
                currentDate,
                currentCulture.DateTimeFormat.CalendarWeekRule,  // Quy tắc tính tuần
                currentCulture.DateTimeFormat.FirstDayOfWeek     // Ngày đầu tiên của tuần
            );
        }
    }

    public class MonthlyData
    {
        public string Month { get; set; }
        public int InputQuantity { get; set; }
        public int NGQuantity { get; set; }
        public double Tppm { get; set; } // Đường biểu đồ tppm
    }

    public class WeeklyData
    {
        public string Week { get; set; }
        public int InputQuantity { get; set; }
        public int NGQuantity { get; set; }
        public double Tppm { get; set; } // Đường biểu đồ tppm
    }

    public class ModelData
    {
        public string Model { get; set; } = string.Empty;
        public int InputQty { get; set; }
        public int NGQty { get; set; }
        public decimal NGRate { get; set; }
    }

    public class DeptData
    {
        public string DeptError { get; set; } = string.Empty;
        public int InputQty { get; set; }
        public int NGQty { get; set; }
        public decimal NGRate { get; set; }
    }

    public class ErrorData
    {
        public string Errortype { get; set; } = string.Empty;
        public int InputQty { get; set; } = 0;
        public int NGQty { get; set; } = 0;
        public decimal NGRate { get; set; }
    }

    public class PartData
    {
        public string Partcode { get; set; } = string.Empty;
        public int InputQty { get; set; } = 0;
        public int NGQty { get; set; } = 0;
        public decimal NGRate { get; set; }
    }
}