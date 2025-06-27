﻿using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.ProdPlan
{
    public class ProdPlanParaModel
    {
        [Key]
        public string name { get; set; }
        public string value { get; set; }
        public string? description { get; set; }
    }
}
