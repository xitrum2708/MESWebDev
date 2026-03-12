using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP.DTO
{
    public class MRPDataDTO
    {
        //Item	Plan PO Qty	Req Qty	Supply Qty
        [Key]
        public int Id { get; set; }
        public string Item { get; set; }
        public int GrossQty { get; set; }
        public int SupplyQty { get; set; }
        public int NetQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
}
