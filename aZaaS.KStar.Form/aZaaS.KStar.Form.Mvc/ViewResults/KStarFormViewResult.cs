using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;

namespace aZaaS.KStar.Form.Mvc.ViewResults
{
    public class KStarFormViewResult : ViewResultBase
    {
        public string MasterName { get; set; }
        
        const string COMMON_VIEWNOTFOUND = "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";

        public KStarFormViewResult() { }

        public KStarFormViewResult(string masterName)
        {
            MasterName = masterName;
        }

        protected override ViewEngineResult FindView(ControllerContext context)
        {
            ViewEngineResult result = base.ViewEngineCollection.FindView(context, base.ViewName, MasterName);
            if (result.View != null)
            {
                return result;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str in result.SearchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, COMMON_VIEWNOTFOUND, new object[] { base.ViewName, builder }));
        }
    }
}