using aZaaS.Framework.Logging.Utilities;
using aZaaS.Framework.Extensions;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using aZaaS.KSTAR.MobileServices.Models;
using aZaaS.KSTAR.MobileServices.Providers;
using System.Configuration;

namespace aZaaS.KSTAR.MobileServices.Controllers
{
    public class WebAPIController : ApiController
    {
        protected BaseServiceProvider _provider;
        public WebAPIController()
        {
            try
            {
                string assemblyName = ConfigurationManager.AppSettings["ServiceProviderAssembly"];
                string providerName = ConfigurationManager.AppSettings["ServiceProvider"];
                _provider = BaseServiceProvider.CreateInstance(assemblyName, providerName);
            }
            catch (Exception ex)
            {
                this.WriteLog("ServiceProvider", "", ex);
            }
        }

        [NonAction]
        protected void WriteLog(string logName, string paraString, Exception exception)
        {
            try
            {
                LogEntity log = new LogEntity()
                {
                    CreatedDate = DateTime.Now,
                    Details = exception.StackTrace,
                    Message = exception.Message,
                    Name = logName,
                    Parameters = paraString,
                    RequestUrl = Request.RequestUri.AbsoluteUri
                };
                _provider.WriteServiceLog(log);
            }
            catch (Exception ex)
            {
                EventLogger.Write(string.Format("{0}{1}{2}", Request.RequestUri, Environment.NewLine, exception.ToString()),
                                    logName, paraString, EventLogEntryType.Error);
            }
        }

        [NonAction]
        protected void ThrowLogError(string logName, string logSource, Exception exception)
        {
            EventLogger.Write(string.Format("{0}{1}{2}", Request.RequestUri, Environment.NewLine, exception.ToString()),
                                logName, logSource, EventLogEntryType.Error);

            throw exception;
        }

        [NonAction]
        protected string TrimUserName(string account)
        {
            if (account.Null()) return string.Empty;

            int index = account.IndexOf(":");
            return index > 0 ? account.Substring(index + 1) : account;
        }

        protected string CurrentUser
        {
            get
            {
                return User.Identity.Name;
            }
        }

        [NonAction]
        protected HttpResponseMessage Json(string jsonString)
        {
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };
        }

        [NonAction]
        protected HttpResponseMessage Xml(string xmlString)
        {
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }

        /// <summary>
        /// Outputs plain text content.
        /// </summary>
        /// <param name="textString">The plain text content to be outputed</param>
        /// <returns>HttpResponseMessage</returns>
        [NonAction]
        protected HttpResponseMessage Content(string textString)
        {
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(textString, Encoding.UTF8, "text/plain")
            };
        }

        [NonAction]
        protected HttpResponseMessage GetResponseMessage(string format, string outputString)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);

            switch (format.ToLower())
            {
                case "html":
                case "text":
                    response = this.Content(outputString);
                    break;
                case "json":
                    response = this.Json(outputString);
                    break;
                case "xml":
                    response = this.Xml(outputString);
                    break;
            }

            return response;
        }
    }
}