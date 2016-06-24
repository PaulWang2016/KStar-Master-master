using aZaaS.KStar.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class TemplateCagetoryModel
    {
        public Guid SysId { get; set; }
        public Guid? ParentId { get; set; }
        public string CategoryName { get; set; }

        public static TemplateCagetoryModel ByJSON(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException("jsonData");

            TemplateCagetoryModel formModel = JsonHelper.ConvertToModel<TemplateCagetoryModel>(jsonData);

            return formModel;
        }
    }
}
