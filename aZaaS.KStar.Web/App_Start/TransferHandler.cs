using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace aZaaS.KStar.Web
{
    public class TransferHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ChecTransferkHandler();
        }

    }
    public class ChecTransferkHandler : IHttpHandler
    {

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {

            if (string.IsNullOrWhiteSpace(context.User.Identity.Name))
            {
                int index = context.Request.Url.AbsoluteUri.LastIndexOf("strUrl=");
                if (index > 0)
                {
                    context.Response.Redirect("/Account/Login?ReturnUrl="+ context.Server.UrlEncode(context.Request.Url.AbsoluteUri.Substring(index + 7)));
                } 
           
            }
            else
            {
                string[] keys = context.Request.QueryString.AllKeys;
                string url = "";
                foreach (var key in keys)
                {
                    if (url == "")
                    {
                        url += context.Request.QueryString[key];
                    }
                    else
                    {
                        url += "&" + key + "=" + context.Request.QueryString[key];
                    }
                }
                context.Response.Redirect(url);
              
            }
        }
    }
}