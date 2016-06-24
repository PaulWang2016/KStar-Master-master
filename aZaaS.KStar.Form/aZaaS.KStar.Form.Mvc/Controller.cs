using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Form.Mvc.Controllers;

namespace aZaaS.KStar.Form.Mvc
{
    public static class Controller
    {
        public static int GetFormId(this KStarFormController controller)
        {
            int formId = 0;
            var sFormId = controller.Request["_FormId"];

            int.TryParse(sFormId, out formId);

            return formId;
        }

        public static string GetSerialNo(this KStarFormController controller)
        {
            return controller.Request["SerialNo"];
        }
    }
}
