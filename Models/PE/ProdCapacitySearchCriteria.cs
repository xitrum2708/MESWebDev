using System;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.PE
{
    // Bound to the POST form. No DataTable filtering: all predicates run in SQL.
    public sealed class ProdCapacitySearchCriteria
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(255)]
        public string? Type { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Customer1 { get; set; } = string.Empty;

        [StringLength(255)]
        public string? UNo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? BModel { get; set; } = string.Empty;

        [StringLength(255)]
        public string? PCBNo { get; set; } = string.Empty;

        public int? Version { get; set; } // 0: All; 1 lastest; 2 others

        public bool HasSearch =>
            StartDate.HasValue || EndDate.HasValue ||
            !string.IsNullOrWhiteSpace(Customer1) ||
            !string.IsNullOrWhiteSpace(UNo) ||
            !string.IsNullOrWhiteSpace(BModel) ||
            !string.IsNullOrWhiteSpace(PCBNo);

        public bool InvalidDateRange =>
            StartDate.HasValue && EndDate.HasValue &&
            StartDate.Value.Date > EndDate.Value.Date;
    }
}
