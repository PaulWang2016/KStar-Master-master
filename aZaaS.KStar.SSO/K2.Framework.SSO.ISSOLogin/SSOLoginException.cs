namespace K2.Framework.SSO.ISSOLogin
{
    using System;
    using System.Web.Mvc;

    public class SSOLoginException : Exception
    {
        private ActionResult result;

        public SSOLoginException(ActionResult result)
        {
            this.Result = result;
        }

        public ActionResult Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }
    }
}

