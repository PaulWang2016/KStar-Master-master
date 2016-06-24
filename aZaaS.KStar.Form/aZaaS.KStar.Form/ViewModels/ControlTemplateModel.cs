using aZaaS.KStar.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class ControlTemplateModel
    {
        public Guid SysId { get; set; }
        public Guid? CategoryId { get; set; }
        public string DisplayName { get; set; }
        public string HtmlTemplate { get; set; }
        public string CategoryName { get; set; }

        public static ControlTemplateModel ByJSON(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException("jsonData");

            ControlTemplateModel formModel = JsonHelper.ConvertToModel<ControlTemplateModel>(jsonData);

            return formModel;
        }
    }
}
