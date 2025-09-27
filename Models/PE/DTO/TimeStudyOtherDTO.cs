using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESWebDev.Models.PE
{
    public class TimeStudyOtherDTO
    {
        public TimeStudyDtlDTO TimeStudyDtl { get; set; }
        public TimeStudyStepDtlDTO TimeStudyStepDtl { get; set; }

    }
}
