using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOLoginWeb.Code
{
    public class SSOLoginException:Exception
    {
        private ActionResult result;

        public ActionResult Result
        {
            get { return result; }
            set { result = value; }
        }
        public SSOLoginException(ActionResult result)
        {
            this.Result = result; 
        }
    }
}