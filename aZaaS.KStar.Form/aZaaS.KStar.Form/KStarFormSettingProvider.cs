using AutoMapper;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form
{
    public class KStarFormSettingProvider : IFormSettingProvider
    {
        private readonly FormSettingRepository _formSettingRepository;

        public KStarFormSettingProvider()
        {
            _formSettingRepository = new FormSettingRepository();
        }

        public IList<CotnrolSettingModel> GetAllControlSettings()
        {
            return _formSettingRepository.GetAllControlSettings();
        }

        public Guid AddControlSettings(CotnrolSettingModel controlSettings)
        {
            return _formSettingRepository.AddControlSettings(controlSettings);
        }


        public void EditFormControlSetting(CotnrolSettingModel controlSetting)
        {
            _formSettingRepository.EditFormControlSetting(controlSetting);
        }

        public void DeleteFormControlSetting(Guid sysId)
        {
            _formSettingRepository.DeleteFormControlSetting(sysId);
        }
  
        public IList<CotnrolSettingModel> GetControlSettings(int pagesize, int pageindex, out int total)
        {
            return _formSettingRepository.GetControlSettings(pagesize, pageindex,out total);
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName)
        {
            return _formSettingRepository.GetControlSettings(processFullName);
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId)
        {
            return _formSettingRepository.GetControlSettings(processFullName,activityId);
        }

        public CotnrolSettingModel GetControlSettings(Guid sysId)
        {
            return _formSettingRepository.GetControlSettings(sysId);
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId, ControlType type)
        {
            return _formSettingRepository.GetControlSettings(processFullName,activityId, type);
        }

        public IList<CotnrolSettingModel> GetControlSettings(string processFullName,int activityId, WorkMode workMode)
        {
            return _formSettingRepository.GetControlSettings(processFullName,activityId, workMode);
        }

        public bool IsCombinRights(string processFullName)
        {
            return _formSettingRepository.IsCombinRights(processFullName);
        }
    }
}
