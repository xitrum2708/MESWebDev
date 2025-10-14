using ExcelDataReader.Log;

namespace MESWebDev.Models.PE.DTO
{
    public class TimeStudyNewUploadDTO
    {
            public string Section { get; set; }
            public string Model { get; set; }
            public string BModel { get; set; }
            public string ModelCat { get; set; }
            public bool IsPrepare { get; set; }
            public string LotNo { get; set; }
            public string Unit { get; set; }

            public string OperationKind { get; set; }
            public string StepContent { get; set; }
            public string ProcessName { get; set; }
            public int UnitQty { get; set; }
            public int AllocatedOpr { get; set; }
       
    }
}
