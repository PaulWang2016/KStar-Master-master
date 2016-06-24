using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar.Form
{
    public class ModelMapperConfig
    {
        public static void Initialize()
        {
            Mapper.CreateMap<AttachmentModel, FormAttachment>();
            Mapper.CreateMap<FormAttachment, AttachmentModel>();
            Mapper.CreateMap<KStarFormModel, ProcessFormHeader>();
            Mapper.CreateMap<ProcessFormHeader, KStarFormModel>();
            Mapper.CreateMap<ProcessFormContent, KStarFormModel>()
                .ForMember(dest => dest.ContentData, mop => mop.MapFrom(pfc => pfc.JsonData));
            Mapper.CreateMap<KStarFormModel, ProcessFormContent>()
                .ForMember(dest => dest.JsonData, mop => mop.MapFrom(kfm => kfm.ContentData));
            Mapper.CreateMap<PositionBaseDto, ComboxContext>();
            Mapper.CreateMap<OrgNodeBaseDto, ComboxContext>();

            Mapper.CreateMap<ActivityControlSetting, CotnrolSettingModel>();
            Mapper.CreateMap<CotnrolSettingModel, ActivityControlSetting>();            

            Mapper.CreateMap<ControlRenderTemplate, ControlTemplateModel>();
            Mapper.CreateMap<ControlTemplateModel, ControlRenderTemplate>();

            Mapper.CreateMap<ControlTemplateCategory, TemplateCagetoryModel>();
            Mapper.CreateMap<TemplateCagetoryModel, ControlTemplateCategory>();            
        }
    }
}
