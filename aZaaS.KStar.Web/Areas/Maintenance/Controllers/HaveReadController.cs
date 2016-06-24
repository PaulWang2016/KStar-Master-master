using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class HaveReadController : BaseMvcController
    {
        private IFormCCProvider _formCCProvider;

        public HaveReadController()
        {
            _formCCProvider = new KStarFormCCProvider();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
