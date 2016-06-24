using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSOLoginWeb.Models
{
    public class LoginModel
    {
        public string Account { set; get; }
        public string Password { set; get; }
        public string Code { set; get; } 
        public string ReturnUrl { get; set; }
    }
}