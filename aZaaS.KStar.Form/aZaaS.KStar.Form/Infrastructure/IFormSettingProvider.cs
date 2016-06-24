using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormSettingProvider
    {        
        Guid AddControlSettings(CotnrolSettingModel controlSettings);        
        void EditFormControlSetting(CotnrolSettingModel controlSetting);
        IList<CotnrolSettingModel> GetControlSettings(string processFullName); 
        IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId);
        IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId, ControlType type);
        IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId, WorkMode workMode);
    }
}
