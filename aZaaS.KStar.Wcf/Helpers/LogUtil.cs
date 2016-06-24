using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using aZaaS.Framework.Logging;

namespace aZaaS.KStar.Wcf
{
    public static class LogUtil
    {
        public static LogEvent CreateLog(string category,string source,string message,Exception exception)
        {
            return new LogEvent()
            {
                Category = category,
                Message = message,
                OccurTime = DateTime.Now,
                Exception = exception,
                Source = source
            };
        }
    }
}