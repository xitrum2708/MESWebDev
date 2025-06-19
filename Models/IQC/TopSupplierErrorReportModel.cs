namespace MESWebDev.Models.IQC
{
    public class TopSupplierErrorReportModel
    {
        public string VENDER_NAME { get; set; }
        public string ABBRE_GROUP { get; set; }
        public int ACCEPTED { get; set; } = 0;
        public int REJECTED { get; set; } = 0;
        public int TOTAL { get; set; } = 0;
        public decimal RATE_ACCEPT { get; set; }
    }
}