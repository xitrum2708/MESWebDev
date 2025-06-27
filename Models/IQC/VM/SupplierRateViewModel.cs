namespace MESWebDev.Models.IQC.VM
{
    public class SupplierRateViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string[] Labels { get; set; }
        public int[] Accepted { get; set; }
        public int[] Rejected { get; set; }
        public double[] AcceptRate { get; set; }
    }
}