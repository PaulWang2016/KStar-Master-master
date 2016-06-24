using SSOLoginWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOLoginWeb.Code.Interface
{
    public interface ISSOLogin
    {
     
        LoginUser SSOLogin(LoginModel model);
    }
}