using aZaaS.KStar.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class CotnrolSettingModel
    {
        public Guid SysId { get; set; }
        public int ActivityId { get; set; }
        public WorkMode WorkMode { get; set; }
        public string ControlRenderId { get; set; }
        public string ControlName { get; set; }
        public ControlType ControlType { get; set; }
        public bool IsHide { get; set; }
        public bool IsDisable { get; set; }
        public bool IsCustom { get; set; }
        public string RenderTemplateId { get; set; }
        public string ProcessFullName { get; set; }
        public string RenderTemplateName { get; set; }
        public string ComponentId { get; set; }

        public static CotnrolSettingModel ByJSON(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException("jsonData");
            
            CotnrolSettingModel formModel = JsonHelper.ConvertToModel<CotnrolSettingModel>(jsonData);        

            return formModel;
        }
    }
}
