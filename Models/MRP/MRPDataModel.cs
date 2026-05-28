using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.MRP
{
    public class MRPDataModel
    {
        //Item	Plan PO Qty	Req Qty	Supply Qty
        [Key]
        public int Id { get; set; }
        public string Item { get; set; }
        public int ReqQty { get; set; }
        public int OHQty { get; set; }
        public int OBLQty { get; set; }

        public int SupplyQty { get; set; }

        public int AfterAllLocation { get; set; }
        public int PlanPOQty { get; set; }


        public DateTime PlanShipDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
}
