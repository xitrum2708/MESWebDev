using MESWebDev.Models.Master.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MESWebDev.Models.Master
{
    public class MasterVM
    {
        public List<UserDTO>? Users { get; set; }
        public UserDTO? User { get; set; }

        public RoleFuncPmsDTO? RoleFuncPms { get; set; }

        public List<SelectListItem>? RoleSL { get; set; }
        public List<SelectListItem>? LanguageSL { get; set; }
        public List<SelectListItem>? FuncSL { get; set; }

        public List<SelectListItem>? PmsSL { get; set; }


        public DictionaryDTO? Dictionary { get; set; }

        public string? ErrorMsg { get; set; }
}
}
