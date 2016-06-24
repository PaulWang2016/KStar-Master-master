using aZaaS.KStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Facades;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class DynamicWidgetController : BaseMvcController
    {
        private static readonly DynamicWidgetFacade _facade = new DynamicWidgetFacade();
        private static readonly SuperADFacade _superADFacade = new SuperADFacade();
        private static readonly AcsFacade _acsfacade = new AcsFacade();

        public ActionResult RenderWidget(string key, bool isAjax = false)
        {


            if (isAjax)
            {
                DynamicWidget dynamicWidget;
                if (_superADFacade.IsSuperAD(this.CurrentUser))
                {
                    dynamicWidget = _facade.GetWidget(key);
                }
                else
                {
                    dynamicWidget = _acsfacade.AuthorityDynamiWidget(key, this.CurrentUser);
                }
                if (dynamicWidget != null)
                    return Content(dynamicWidget.RazorContent);
                else
                    return Content("<h1> Sorry,you do not have permission to access. </h1>");
            }
            return View("_SingPage");
        }

    }
}
