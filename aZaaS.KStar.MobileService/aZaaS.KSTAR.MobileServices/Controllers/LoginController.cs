using aZaaS.KSTAR.MobileServices.Models;
using System.Web.Http;
using System;
using aZaaS.KSTAR.MobileServices.Providers;
using System.Configuration;

namespace aZaaS.KSTAR.MobileServices.Controllers
{
    public class LoginController : WebAPIController
    {
        [HttpPost]
        public LoginResult Login([FromBody]LoginInfo info)
        {
            try
            {
                var result = _provider.Login(info);
                this.WriteLog("Login", Newtonsoft.Json.JsonConvert.SerializeObject(info), new Exception(result.Message));
                return result;
            }
            catch (Exception ex)
            {
                this.WriteLog("Login", Newtonsoft.Json.JsonConvert.SerializeObject(info), ex);
                return new LoginResult()
                {
                    Message = ex.Message,
                    Result = "E",
                    MessageDetails = ex.StackTrace
                };
            }
        }
    }
}
