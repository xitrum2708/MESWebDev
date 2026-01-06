namespace MESWebDev.Models.Setting.DTO
{
    public class FormatRazorDTO
    {
        public string NumberFormat { get; set; } = "#,0.####";
        public string DateFormat { get; set; } = "yyyy/MM/dd";
        public string DatetimeFormat { get; set; } = "yyyy/MM/dd HH:mm:ss";

        public string NumberCss { get; set; } = "text-end";
        public string DateCss { get; set; } = "text-center";
        public string TextCss { get; set; } = "text-start";
    }
}
