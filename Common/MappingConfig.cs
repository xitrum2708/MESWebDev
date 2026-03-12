using AutoMapper;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Models.MRP;
using MESWebDev.Models.MRP.DTO;
using MESWebDev.Models.PE;
using MESWebDev.Models.PE.DTO;
using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.ProdPlan.SMT.DTO;

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


            CreateMap<UserModel, UserDTO>()
                .ForMember(dest => dest.LangName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : string.Empty))
                ;
            CreateMap<UserDTO, UserModel>();

            CreateMap<RoleFuncPmsDTO, RoleFuncPmsModel>();
            CreateMap<RoleFuncPmsModel, RoleFuncPmsDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : string.Empty))
                .ForMember(dest => dest.FuncName, opt => opt.MapFrom(src => src.Function != null ? src.Function.EnName : string.Empty))
                .ForMember(dest => dest.PmsName, opt => opt.MapFrom(src => src.Pms != null ? src.Pms.PmsName : string.Empty));

            CreateMap<DictionaryDTO, DictionaryModel>();
            CreateMap<DictionaryModel, DictionaryDTO>()
                .ForMember(dest => dest.LangName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));

            CreateMap<RoleDTO, RoleModel>().ReverseMap();

            CreateMap<SMTProdPlanDtlModel, SMTEventsDTO>()
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null? src.SMTProdPlanHdrModel.Model: string.Empty))
                .ForMember(dest => dest.PCBNo, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null? src.SMTProdPlanHdrModel.PCBNo: string.Empty))
                .ForMember(dest => dest.PCBType, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null? src.SMTProdPlanHdrModel.PCBType: string.Empty))
                .ForMember(dest => dest.PCBPerModel, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null? src.SMTProdPlanHdrModel.PCBPerModel: 0))
                .ForMember(dest => dest.TargetPerHour85, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null? src.SMTProdPlanHdrModel.TargetPerHour85 : 0))
                ;

            CreateMap<SMTProdPlanDTO, SMTProdPlanDtlModel>();
            CreateMap<SMTProdPlanDtlModel, SMTProdPlanDTO>()
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.Model : string.Empty))
            .ForMember(dest => dest.PCBNo, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.PCBNo : string.Empty))
            .ForMember(dest => dest.PCBType, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.PCBType : string.Empty))
            .ForMember(dest => dest.PCBPerModel, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.PCBPerModel : 0))
            .ForMember(dest => dest.BalanceQty, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.BalanceQty : 0))
            .ForMember(dest => dest.IssuedQty, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.IssuedQty : 0))
            .ForMember(dest => dest.TargetPerHour85, opt => opt.MapFrom(src => src.SMTProdPlanHdrModel != null ? src.SMTProdPlanHdrModel.TargetPerHour85 : 0))
            ;

            CreateMap<SMTEventsDTO, SMTProdPlanDTO>()
                .ForMember(des => des.Id,
                            opt => opt.MapFrom(src => src.id));
            CreateMap<SMTProdPlanDTO, SMTEventsDTO>()
                .ForMember(dest => dest.start,
                    opt => opt.MapFrom(src => src.StartDt.ToString("yyyy-MM-ddTHH:mm:ss")))

                .ForMember(dest => dest.end,
                    opt => opt.MapFrom(src => src.EndDt.ToString("yyyy-MM-ddTHH:mm:ss")))

                .ForMember(dest => dest.id,
                    opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.resourceId,
                    opt => opt.MapFrom(src => src.LineCode))

                .ForMember(dest => dest.OldStartDt,
                    opt => opt.MapFrom(src => src.StartDt));

            //------- MRP -------\\
            CreateMap<MRPBOMDTO, MRPBOMModel>().ReverseMap();
            CreateMap<MRPDataDTO, MRPDataModel>().ReverseMap();
            CreateMap<MRPOBLDTO, MRPOBLModel>().ReverseMap();
            CreateMap<MRPOHDTO, MRPOHModel>().ReverseMap();
            CreateMap<MRPSPODTO, MRPSPOModel>().ReverseMap();

            CreateMap<MRPBOMDTO, MRPBOMUpload>().ReverseMap();
            CreateMap<MRPOBLDTO, MRPOBLUpload>().ReverseMap();
            CreateMap<MRPOHDTO, MRPOHUpload>().ReverseMap();
            CreateMap<MRPSPODTO, MRPSPOUpload>().ReverseMap();
        }
    }
}

/*
             CreateMap<RoleFuncViewModel, RoleFuncViewDTO>()
                .ForMember(des => des.role_name, opt => opt.MapFrom(src => src.Role.role_name))
                .ForMember(des => des.func_name, opt => opt.MapFrom(src => src.Function.name_en))
                .ForMember(des => des.view_name, opt => opt.MapFrom(src => src.View.view_name));
 
 */