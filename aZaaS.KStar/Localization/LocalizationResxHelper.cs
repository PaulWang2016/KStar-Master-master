using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Globalization;
using System.Resources;
using System.Collections;
using System.Web.Mvc;
using System.Text;

namespace aZaaS.KStar.Localization
{
    public  class LocalizationResxHelper
    {

        public static string CSHtmlResx(HtmlHelper htmlHelper, String resxKey)
        {
            string cshtmlVirtualPath = ResxService.GetResxFilePathByHtmlhelper(htmlHelper);
            return  ResxService.GetResouces(resxKey,cshtmlVirtualPath);
        }

        public static string JSResxScriptForResult(string jsFilePath)
        {
            return ResxService.GetGenerateJavaScript(jsFilePath);

        }
    }
}