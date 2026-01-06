using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.Setting;
using MESWebDev.Models.Setting.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace MESWebDev.Models.ProdPlan
{
    public class ProdPlanViewModel
    {
        public DataTable Data { get; set; }
        public string error_msg { get; set; }

        #region Setting
        public ProjectSettingModel SettingModel { get; set; }

        public FormatRazorDTO FormatRazorDTO { get; set; } = new FormatRazorDTO();

        #endregion

        #region SMT Production Plan

        #endregion


        #region PC Production Plan
        public List<string>? holidays { get; set; }
        public List<EventsDTO>? events { get; set; }
        public List<ResourcesDTO>? resources { get; set; }

        public List<CalendarModel>? calendar { get; set; }
        public List<ProdPlanModel>? prod_plans { get; set; }
        public DateTime? start_sch_dt { get; set; } //start date for running schedule
        public DateTime? sch_dt { get; set; } //schedule date 
        public int? working_hour { get; set; } = 8; // total working hour a day
        public string? line { get; set; }
        public double? start_hour { get; set; } // start running model
        public int? start_working_time { get; set; } = 8; // default = 8, means start 08:00
        public int? break_working_time { get; set; } = 12; // default = 12, means 12:00
        public int? break_hour { get; private set; } = 1; // total break hour
        public bool? is_new_model { get; set; }
        public bool? is_first_run_new_model { get; set; }
        public bool? is_other_model { get; set; }
        public bool? is_fpp_model { get; set; }
        public bool? is_first_run_fpp_model { get; set; }
        public int? new_model_rate { get; set; } = 65; // default = 1, means 1 model per hour

        public int? model_stransfer_time { get; set; } = 30;
        public ProdPlanModel? prodPlanModel { get; set; }

        public List<ProdPlanParaModel>? parameters { get; set; }

        // line itemlist
        public List<SelectListItem>? line_items { get; set; }
        #endregion


    }
}
