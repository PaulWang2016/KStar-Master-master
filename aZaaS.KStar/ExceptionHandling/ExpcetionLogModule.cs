using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using aZaaS.Framework.Logging;
using aZaaS.Framework.Facade;

namespace aZaaS.KStar.ExceptionHandling
{
    public class ExpcetionLogModule : IHttpModule
    {
        private readonly ILogger _logger = LogFactory.GetLogger();

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Error += new EventHandler(context_Error);
        }

        void context_Error(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            var ex = app.Server.GetLastError();

            _logger.Write(new LogEvent
            {
                Source = app.Request.RawUrl,
                Exception = ex,
                Message = ex.Message,
                OccurTime = DateTime.Now
            });
        }
    }
}
