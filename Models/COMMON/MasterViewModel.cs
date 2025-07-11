using MESWebDev.Common;
using MESWebDev.Models.WHS.VM;
using MESWebDev.Models.WHS;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.COMMON
{
    public class MasterViewModel
    {
        public PagedResult<LotControlViewModel> LotControlMasterList { get; set; }        
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class LotControlViewModel
    {        
        public int LotControlID { get; set; }
        public string Model { get; set; } 
        public string LotNo { get; set; }   
        public int Quantity { get; set; } = 0;
        public string SpecialInfo { get; set; }        
        public string DateCode { get; set; }        
        public string Code { get; set; }        
        public string SerialStart { get; set; }        
        public string SerialEnd { get; set; }        
        public string ApprovedBy { get; set; }        
        public string Revised { get; set; }      
        public string IssuedBy { get; set; }       
        public DateTime ProDate { get; set; }       
        public DateTime IssueDate { get; set; }        
        public string PONumber { get; set; }
        public string? CreatedBy { get; set; } = "System";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
