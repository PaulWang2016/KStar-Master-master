using aZaaS.KStar.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class JSResourceController : BaseMvcController
    {
        //
        // GET: /JSResource/

        public ActionResult Index()
        {
            return View();
        }

        public JavaScriptResult GetJavaScriptResx(string jsPageName)
        {
            JavaScriptResult result = new JavaScriptResult();
            result.Script = LocalizationResxHelper.JSResxScriptForResult(jsPageName);
            return result;

        }

    }
}
