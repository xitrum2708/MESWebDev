namespace MESWebDev.Models.PE.DTO
{
    public class TimeStudyUploadDTO
    {
            public string Customer { get; set; }
            public string Section { get; set; }
            public string Model { get; set; }
            public string BModel { get; set; }
            public string LotNo { get; set; }
            public string Unit { get; set; }
            public string PcbName { get; set; }
            public string PcbNo { get; set; }

            public string OperationKind { get; set; }
            public int StepNo { get; set; }
            public string StepContent { get; set; }
            public int SeqNo { get; set; }
            public string OperationName { get; set; }
            public string OperationDetailName { get; set; }
            public string Remark { get; set; }

            public decimal Time01 { get; set; }
            public decimal Time02 { get; set; }
            public decimal Time03 { get; set; }
            public decimal Time04 { get; set; }
            public decimal Time05 { get; set; }
            public int UnitQty { get; set; }
            public int AllocatedOpr { get; set; }
            public decimal TimeAvg { get; set; }
       
    }
}
