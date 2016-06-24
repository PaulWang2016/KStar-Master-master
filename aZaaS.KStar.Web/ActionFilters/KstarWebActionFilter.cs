using aZaaS.KStar.Helper;
using aZaaS.KStar.MgmtDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace aZaaS.KStar.Web.ActionFilters
{
    public class KstarWebActionFilter : System.Web.Mvc.IActionFilter, System.Web.Mvc.IExceptionFilter
    {

        public void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            string cname = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string requestUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            string requestType = filterContext.HttpContext.Request.RequestType;
            List<string> parameters = new List<string>();
            string message = "OK";
            string details = string.Empty;
            string iPAddress = CustomHelper.GetClientAddress();
            string requestUser = filterContext.HttpContext.User.Identity.Name;
            if (!"getjavascriptresx,nosuchpage,apperrors".Contains(actionName.ToLower()))
            {
                if (requestType == "GET")
                {
                    var query = filterContext.HttpContext.Request.QueryString;
                    foreach (string key in query.AllKeys)
                    {
                        parameters.Add(string.Format("{0}:{1}", key, query[key]));
                    }
                }
                else
                {
                    var form = filterContext.HttpContext.Request.Form;
                    foreach (string key in form.Keys)
                    {
                        parameters.Add(string.Format("{0}:{1}", key, form[key]));
                    }
                }
                LogRequestDto logentity = new LogRequestDto(actionName, requestUrl, requestType, string.Join(",", parameters), message, details, iPAddress, requestUser, DateTime.Now);
                LogHelper.LogInfoMsg(logentity);
            }
        }

        public void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled == true)
            {
                HttpException httpExce = filterContext.Exception as HttpException;
                if (httpExce.GetHttpCode() != 500)
                {
                    return;
                }
            }            
            //写入日志 记录
            string cname = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string requestUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            string requestType = filterContext.HttpContext.Request.RequestType;
            List<string> parameters = new List<string>();
            string message = "Error";
            string details = filterContext.Exception.ToString();
            string iPAddress = CustomHelper.GetClientAddress();
            string requestUser = filterContext.HttpContext.User.Identity.Name;
            if (requestType == "GET")
            {
                var query = filterContext.HttpContext.Request.QueryString;
                foreach (string key in query.AllKeys)
                {
                    parameters.Add(string.Format("{0}:{1}", key, query[key]));
                }
            }
            else
            {
                var form = filterContext.HttpContext.Request.Form;
                foreach (string key in form.Keys)
                {
                    parameters.Add(string.Format("{0}:{1}", key, form[key]));
                }
            }
            LogRequestDto logentity = new LogRequestDto(actionName, requestUrl, requestType, string.Join(",", parameters), message, details, iPAddress, requestUser, DateTime.Now);
            LogHelper.LogInfoMsg(logentity);

            HttpException httpException = filterContext.Exception as HttpException;
            if (httpException != null)
            {
                filterContext.Controller.ViewBag.UrlRefer = filterContext.HttpContext.Request.UrlReferrer;
                if (httpException.GetHttpCode() == 404)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/NoSuchPage");
                }
                else if (httpException.GetHttpCode() == 500)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/AppErrors");
                }
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("~/Home/AppErrors");
            }

            filterContext.ExceptionHandled = true;//设置异常已经处理
        }
    }
}