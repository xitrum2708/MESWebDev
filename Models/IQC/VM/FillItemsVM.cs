using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.IQC.VM
{
    public class FillItemsVM : IValidatableObject
    {
        public string ReportId { get; set; }
        public int ReportItemID { get; set; }
        public int GroupId { get; set; }
        public bool IsAlreadyFilled { get; set; }
        public string? FinishedStatus { get; set; }
        public List<string> SpecList { get; set; }
        public List<string> SpecDetailList { get; set; }
        public bool IsFinished { get; set; }
        public UV_IQC_ItemName ItemName { get; set; }
        public UV_IQC_Report Report { get; set; }
        public List<UV_IQC_ItemName> Items { get; set; }
        public ReportItemVM ReportItem { get; set; }
        public List<UV_IQC_ErrorsItemMaster> ErrorsItemMasterList { get; set; }
        public string FormPartial { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1) SamplingSize phải > 0
            if (ReportItem.SamplingSize <= 0)
            {
                yield return new ValidationResult(
                    "Sampling Size phải lớn hơn 0.",
                    new[] { "ReportItem.SamplingSize" });
            }

            // 2) Nếu có nhập CRI thì không vượt SamplingSize
            if (ReportItem.CRI > ReportItem.SamplingSize)
            {
                yield return new ValidationResult(
                    $"Số lỗi CRI không được vượt quá Sampling Size ({ReportItem.SamplingSize}).",
                    new[] { "ReportItem.CRI" });
            }

            // 3) Tương tự cho MAJ
            if (ReportItem.MAJ > ReportItem.SamplingSize)
            {
                yield return new ValidationResult(
                    $"Số lỗi MAJ không được vượt quá Sampling Size ({ReportItem.SamplingSize}).",
                    new[] { "ReportItem.MAJ" });
            }

            // 4) Và cho MIN
            if (ReportItem.MIN > ReportItem.SamplingSize)
            {
                yield return new ValidationResult(
                    $"Số lỗi MIN không được vượt quá Sampling Size ({ReportItem.SamplingSize}).",
                    new[] { "ReportItem.MIN" });
            }
        }
    }
}