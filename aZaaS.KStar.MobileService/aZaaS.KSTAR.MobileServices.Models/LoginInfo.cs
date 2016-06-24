using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class LoginInfo
    {
        private string _language = "zh-cn";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Language
        {
            get { return _language; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _language = value;
            }
        }
        public string ClientID { get; set; }
        public string ClientType { get; set; }
        public string ClientOS { get; set; }
    }
}