using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.KstarMobile.Controllers
{
     [EnhancedHandleError]
    public class HomeController : BaseMvcController
    {
        //
        // GET: /KstarMobile/Home/

        public ActionResult MobileConfig(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/KstarMobile/Views/MobileConfig.cshtml");
            }
            return View("_SingPage");
        }
    }
}
