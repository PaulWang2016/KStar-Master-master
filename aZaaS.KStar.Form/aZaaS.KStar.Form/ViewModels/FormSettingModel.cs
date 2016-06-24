using aZaaS.KStar.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class FormSettingModel
    {
        public Guid SysId { get; set; }
        public int ActivityId { get; set; }
        public WorkMode WorkMode { get; set; }
        public bool IsEditable { get; set; }
        public bool IsCustom { get; set; }
        public bool IsSettingEnabled { get; set; }

        public static FormSettingModel ByJSON(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException("jsonData");

            FormSettingModel formModel = JsonHelper.ConvertToModel<FormSettingModel>(jsonData);

            return formModel;
        }
    }
}
