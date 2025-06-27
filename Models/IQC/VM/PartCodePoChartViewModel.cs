namespace MESWebDev.Models.IQC.VM
{
    public class PartCodePoChartViewModel
    {
        public string[] Labels { get; set; }
        public int[] POAccept { get; set; }
        public int[] POReject { get; set; }
        public double[] AcceptRate { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
