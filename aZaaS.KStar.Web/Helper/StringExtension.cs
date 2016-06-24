using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public static class StringExtension
    {
        public static IHtmlString JsString(this string str)
        {
            return new HtmlString(string.Format(@"decodeURI('{0}')", HttpUtility.JavaScriptStringEncode(str)));
        }

        public static IHtmlString DateTimeString(this DateTime? datetime)
        {
            return datetime == null ? new HtmlString("") : new HtmlString(datetime.Value.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}