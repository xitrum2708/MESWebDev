namespace MESWebDev.Models.IQC.VM
{
    public class IQCReportCreateVM
    {
        public string LottagId { get; set; } = ""; // Lottag được quét
        public List<string> ScannedLottags { get; set; } = new(); // Lottag được thêm
        public int SelectedGroupId { get; set; }               // Group đã chọn
        public List<UV_IQC_InspectionGroup> AvailableGroups { get; set; } // Đổ combobox group
    }
}