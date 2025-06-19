namespace MESWebDev.Models.COMMON
{
    public class UV_PRO_LINE_Model
    {
        public long ID { get; set; }
        public string LineName { get; set; } = string.Empty;
        public int DeptID { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}