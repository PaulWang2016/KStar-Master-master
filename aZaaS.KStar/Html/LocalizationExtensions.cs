using aZaaS.KStar.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace aZaaS.KStar.Html
{
   public static class LocalizationExtensions
    {

       public static string CSHtmlResx(this HtmlHelper htmlHelper,String resxKey)
       {

           return LocalizationResxHelper.CSHtmlResx(htmlHelper,resxKey);
       
       
       }
       public static string DBResxFor_aZaaSKStar_MenuBar_DisplayName(this HtmlHelper htmlHelper,String resxKey)
       {
         return  LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(resxKey);
       }


    }
}
