using MESWebDev.Models.UVASSY;

namespace MESWebDev.Models.VMProcedure
{
    public class UVAssyProductionViewModel
    {
        public UVAssyProduction Summary { get; set; }
        public List<UVAssyOutputDetail> UVAssyOutputDetails { get; set; } = new List<UVAssyOutputDetail>();
        public List<UVAssyErrorDetail> UVAssyErrorDetails { get; set; } = new List<UVAssyErrorDetail>();
    }
}