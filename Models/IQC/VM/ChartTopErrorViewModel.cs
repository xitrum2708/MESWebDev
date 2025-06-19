namespace MESWebDev.Models.IQC.VM
{
    public class ChartTopErrorViewModel
    {
        public string[] Labels { get; set; }
        public int[] Data { get; set; }
        public int[] POQty { get; set; }
        public double[] ErrorRateData { get; set; }
    }
}