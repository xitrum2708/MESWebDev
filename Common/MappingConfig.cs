using AutoMapper;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
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

            // Operation Detail
            CreateMap<OperationDetailModel, OperationDetailDTO>()
                .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.OperationMaster.Name))
                .ReverseMap();

            CreateMap<TimeStudyDtlDTO, TimeStudyDtlModel>().ReverseMap();
            CreateMap<TimeStudyHdrDTO, TimeStudyHdrModel>().ReverseMap();
            CreateMap<TimeStudyStepDtlDTO, TimeStudyStepDtlModel>().ReverseMap();

            CreateMap<TimeStudyDtlDTO, TimeStudyDTO>().ReverseMap();  // tells AutoMapper how to map TimeStudyDtlDTO
            CreateMap<TimeStudyStepDtlDTO, TimeStudyDTO>().ReverseMap(); // tells AutoMapper how to map TimeStudyStepDtlDTO
            CreateMap<TimeStudyOtherDTO, TimeStudyDTO>().IncludeMembers(src => src.TimeStudyDtl,src => src.TimeStudyStepDtl); // tells AutoMapper how to map TimeStudyOtherDTO
                                                                                                                              // 

            CreateMap<TimeStudyUploadDTO, TimeStudyStepDtlModel>().ReverseMap();
            CreateMap<TimeStudyUploadDTO, TimeStudyHdrModel>().ReverseMap();
            CreateMap<TimeStudyUploadDTO, TimeStudyDtlModel>().ReverseMap();

            CreateMap<TimeStudyNewHdrDTO, TimeStudyNewHdrModel>().ReverseMap();
            CreateMap<TimeStudyNewDtlDTO, TimeStudyNewDtlModel>().ReverseMap();
            CreateMap<TimeStudyNewStepDtlDTO, TimeStudyNewStepDtlModel>().ReverseMap();
        }
    }
}

/*
             CreateMap<RoleFuncViewModel, RoleFuncViewDTO>()
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.Role.role_name))
                .ForMember(des => des.func_name, opt => opt.MapFrom(src => src.Function.name_en))
                .ForMember(des => des.view_name, opt => opt.MapFrom(src => src.View.view_name));
 
 */