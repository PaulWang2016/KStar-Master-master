using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]
namespace aZaaS.KStar.Helper
{
   public  class LogHelper
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        public static void LogInfoMsg(object info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void LogErrorMsg(object info)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info);
            }
        }


    }
}
