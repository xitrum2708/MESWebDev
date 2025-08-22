using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.PE
{
    public class ManpowerModel
    {
        [Key]
        public int id { get; set; }
        public string company { get; set; }
        public string u_model { get; set; }
        public string b_model { get; set; }

        
        public int smt_headcount { get; set; } // SMT headcount
        public int insert_headcount { get; set; } // Insert headcount
        public int assy_headcount { get; set; } // Assembly headcount
        public int scl_headcount { get; set; } // SCL headcount
        [Precision(18, 4)]
        public decimal smt_cost { get; set; } // SMT cost
        [Precision(18, 4)]
        public decimal insert_cost { get; set; } // Insert cost
        [Precision(18, 4)]
        public decimal scl_cost { get; set; } // SCL cost
        [Precision(18, 4)]
        public decimal assy_cost { get; set; } // Assembly cost
        [Precision(18, 4)]
        public decimal average_cost { get; set; } // average

        public string? upload_file { get; set; } // Total cost
        public DateTime updated_dt { get; set; } // Total cost

        public string? note { get; set; } // 

        public DateTime upload_dt { get; set; } //
        public string upload_by { get; set; } //

    }
}
