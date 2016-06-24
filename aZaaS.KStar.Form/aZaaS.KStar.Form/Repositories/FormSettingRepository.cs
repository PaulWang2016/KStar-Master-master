using AutoMapper;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class FormSettingRepository
    {

        public IList<CotnrolSettingModel> GetAllControlSettings()
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ActivityControlSetting> controlsettings = context.ActivityControlSettings.ToList();
                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsettings);
            }
        }
        public Guid AddControlSettings(CotnrolSettingModel controlSettings)
        {
            Guid sysId = new Guid();
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ActivityControlSetting controlsetting = Mapper.Map<CotnrolSettingModel,ActivityControlSetting>(controlSettings);
                if (controlsetting.SysId != Guid.Empty)
                {
                    var temp = context.ActivityControlSettings.Where(x => x.SysId == controlsetting.SysId).FirstOrDefault();
                    if (temp != null)
                    {
                        temp.ActivityId = controlsetting.ActivityId;
                        temp.WorkMode = controlsetting.WorkMode;
                        temp.ControlRenderId = controlsetting.ControlRenderId;
                        temp.ControlName = controlsetting.ControlName;
                        temp.ControlType = controlsetting.ControlType;
                        temp.IsCustom = controlsetting.IsCustom;
                        temp.IsDisable = controlsetting.IsDisable;
                        temp.IsHide = controlsetting.IsHide;
                        temp.RenderTemplateId = controlsetting.RenderTemplateId;
                    }
                    else
                    {                        
                        context.ActivityControlSettings.Add(controlsetting);
                    }
                }
                else
                {
                    controlsetting.SysId = Guid.NewGuid();
                    context.ActivityControlSettings.Add(controlsetting);
                }                
                context.SaveChanges();
                sysId = controlsetting.SysId;
            }
            return sysId;
        }

        public void EditFormControlSetting(CotnrolSettingModel controlSetting)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ActivityControlSetting controlsetting = Mapper.Map<CotnrolSettingModel, ActivityControlSetting>(controlSetting);
                ActivityControlSetting oldcontrolsetting = context.ActivityControlSettings.Where(x => x.SysId == controlsetting.SysId).FirstOrDefault();
                oldcontrolsetting.ActivityId = controlsetting.ActivityId;
                oldcontrolsetting.WorkMode = controlsetting.WorkMode;
                oldcontrolsetting.ControlRenderId = controlsetting.ControlRenderId;
                oldcontrolsetting.ControlName = controlsetting.ControlName;
                oldcontrolsetting.ControlType = controlsetting.ControlType;
                oldcontrolsetting.IsCustom = controlsetting.IsCustom;
                oldcontrolsetting.IsDisable = controlsetting.IsDisable;
                oldcontrolsetting.IsHide = controlsetting.IsHide;
                oldcontrolsetting.RenderTemplateId = controlsetting.RenderTemplateId;
                context.SaveChanges();
            }
        }

        public void DeleteFormControlSetting(Guid sysId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ActivityControlSetting controlSettings = context.ActivityControlSettings.Where(x => x.SysId == sysId).FirstOrDefault();
                context.ActivityControlSettings.Remove(controlSettings);
                context.SaveChanges();
            }
        }

        public IList<CotnrolSettingModel> GetControlSettings(int pagesize, int pageindex, out int total)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ActivityControlSetting> controlsettings = context.ActivityControlSettings.ToList();
                total = controlsettings.Count;
                IList<ActivityControlSetting> controlsetting = controlsettings.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsetting);
            }
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ActivityControlSetting> controlsetting = context.ActivityControlSettings.Where(x =>x.ProcessFullName == processFullName).ToList();
                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsetting);
            }
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ActivityControlSetting> controlsetting = context.ActivityControlSettings.Where(x => x.ActivityId == activityId && x.ProcessFullName == processFullName).ToList();
                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsetting);
            }
        }

        public CotnrolSettingModel GetControlSettings(Guid sysId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ActivityControlSetting controlsetting = context.ActivityControlSettings.Where(x => x.SysId == sysId).FirstOrDefault();
                return Mapper.Map<ActivityControlSetting, CotnrolSettingModel>(controlsetting);
            }
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId, ControlType type)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ActivityControlSetting> controlsetting = context.ActivityControlSettings.Where(x => x.ActivityId == activityId && x.ProcessFullName == processFullName && x.ControlType == type.ToString()).ToList();
                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsetting);
            }
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName, int activityId, WorkMode workMode)
        {
            var mode = Convert.ToInt32(workMode);
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var controlsetting = new List<ActivityControlSetting>();
                if (activityId == 0)
                {
                    controlsetting = context.ActivityControlSettings.Where(x => x.ActivityId == activityId && x.ProcessFullName == processFullName).ToList();
                }
                else
                {
                    controlsetting = context.ActivityControlSettings.Where(x => x.ActivityId == activityId && x.WorkMode == mode).ToList();
                }

                return Mapper.Map<IList<ActivityControlSetting>, IList<CotnrolSettingModel>>(controlsetting);
            }
        }

        public bool IsCombinRights(string processFullName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ActivityControlSettings.FirstOrDefault(x => x.ProcessFullName == processFullName 
                    && x.ControlType=="FormSetting" && x.IsHide == false);

                var isCombin = true;
                if (item != null)
                {
                    isCombin = item.IsHide;
                }

                return isCombin;
            }
        }

    }
}
