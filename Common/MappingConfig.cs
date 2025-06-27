using AutoMapper;
using MESWebDev.Models.ProdPlan;

namespace MESWebDev.Common
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // User

            // Function
            CreateMap<EventsDTO, ProdPlanModel>().ReverseMap();
        }
    }
}
