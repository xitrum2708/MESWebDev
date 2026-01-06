using MESWebDev.Models.Setting.DTO;
using System.Data;

namespace MESWebDev.Models.PE
{
    public class PEViewModel
    {
        public DataTable? data { get; set; }
        public DataTable? detail { get; set; }
        public ManpowerModel? manpower { get; set; }
        public OperationModel? operation { get; set; }
        public OperationDetailModel? operationDtl { get; set; }
        public TimeStudyHdrDTO TimeStudyHdr { get; set; }
        public TimeStudyDtlDTO TimeStudyDtl { get; set; }
        public TimeStudyStepDtlDTO TimeStudyStepDtl { get; set; }
        public List<OperationModel>? operationList { get; set; }
        public List<OperationDetailModel>? operationDtlList { get; set; }
        public string? error_msg { get; set; }

        //input study
        public List<string>? customers { get; set; }
        public List<string>? sections { get; set; }
        public List<string>? models { get; set; }
        public List<string>? bModels { get; set; }
        public List<string>? lotNos { get; set; }
        public List<string>? units { get; set; }
        public List<string>? pcbNames { get; set; }
        public List<string>? pcbNos { get; set; }
        public List<int>? stepNos { get; set; }
        public List<TimeStudyHdrDTO>? timeStudyHdrList { get; set; }
        public List<TimeStudyDtlDTO>? timeStudyDtlList { get; set; }
        public List<TimeStudyStepDtlDTO>? timeStudyStepDtlList { get; set; }
        public List<TimeStudyDTO>? timeStudyList { get; set; }

        //step no
        public int? StepNo { get; set; }
        public List<string>? operationNames { get; set; }
        public List<string>? operationDtlNames { get; set; }

        // Time Study New
        public TimeStudyNewHdrDTO TimeStudyNewHdr { get; set; }
        public TimeStudyNewDtlDTO TimeStudyNewDtl { get; set; }
        public TimeStudyNewStepDtlDTO TimeStudyNewStepDtl { get; set; }
        public List<TimeStudyNewHdrDTO>? TimeStudyNewHdrList { get; set; }
        public List<TimeStudyNewDtlDTO>? TimeStudyNewDtlList { get; set; }
        public List<TimeStudyNewStepDtlDTO>? TimeStudyNewStepDtlList { get; set; }

        //2025-12-22 add Format for Razor
        public FormatRazorDTO FormatRazorDTO { get; set; } = new FormatRazorDTO();
    }
}
