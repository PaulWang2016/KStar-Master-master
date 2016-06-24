using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.Controllers;
using aZaaS.KStar.Form.Mvc.ViewResults;

using aZaaS.KStar.Web.Areas.Depository.Models;

namespace aZaaS.KStar.Web.Areas.Depository.Controllers
{
    public class DepositController : KStarFormController
    {
        //
        // GET: /Depository/Deposit/

        public ActionResult Index()
        {
            ViewBag.Title = "物品托存申请";

            var model = new DepositRequestModel();

            return KStarFormView(model);
        }


        public override void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        {
            context.ProcessName = this.ProcessName;
            base.OnWorkflowNewTaskStarting(context);
        }

        public override string ProcessName
        {
            get { return @"KStarWorkflow\DepositRequest"; }
        }
    }
}
