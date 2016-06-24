using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Form.Mvc;
using aZaaS.KStar.Form.HtmlFilter;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Mvc.Controllers;
using aZaaS.KStar.Form.Mvc.ViewResults;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Form.Infrastructure;

namespace aZaaS.KStar.Form.Mvc.ActionFilters
{
    [EnhancedHandleError]
    [System.Web.Mvc.Authorize] 
    public class KStarFormActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controller = filterContext.Controller;
            //var actionResult = filterContext.Result;
            //if (controller is KStarFormController)
            //{
            //    if (!controller.IsControlSetting())
            //    {
            //        var kstarfromController = controller as KStarFormController;
            //        var identity = kstarfromController.Authorization();

            //        if (!identity)
            //        {
            //            HttpContext.Current.Response.Redirect("/AccountInvalid.html");
            //        }
            //    }
            //}
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller;
            var actionResult = filterContext.Result;
            if (controller is KStarFormController && actionResult is KStarFormViewResult)
            {
                var response = filterContext.HttpContext.Response;
                var kstarfromController = controller as KStarFormController;
                ResponseFilterStream filter = new ResponseFilterStream(response.Filter);
                filter.TransformStream += (MemoryStream ms) =>
                {
                    var html = Encoding.UTF8.GetString(ms.ToArray());
                    var workMode = controller.GetWorkMode();

                    //TODO:Filtering the control by control settings
                    if (!controller.IsControlSetting())
                    {
                        if (workMode == WorkMode.View || workMode == WorkMode.Review)
                        {
                            var identity = kstarfromController.ViewAuthorization();

                            if (!identity)
                            {
                                response.Redirect("/KStarFormInvalid.html");
                            }
                        }

                        if (workMode == WorkMode.Draft || workMode == WorkMode.Startup)
                        {
                            var identity = kstarfromController.StartupAuthorization();

                            if (!identity)
                            {
                                response.Redirect("/KStarFormInvalid.html");
                            }
                        }

                        var formId = kstarfromController.GetFormId();

                        html = kstarfromController.OnKStarFormFiltering(html, formId);
                        html = kstarfromController.OnKStarFormFilter(html, formId);
                        html = kstarfromController.OnKStarFormFiltered(html, formId);
                    }

                    ms = new MemoryStream(html.Length);
                    byte[] buffer = Encoding.UTF8.GetBytes(html);
                    ms.Write(buffer, 0, buffer.Length);

                    return ms;
                };
                response.Filter = filter;
            }
        }
    }
}
