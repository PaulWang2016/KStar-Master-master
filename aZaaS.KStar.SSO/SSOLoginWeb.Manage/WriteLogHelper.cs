using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using log4net;
using System.Reflection;

namespace SSOLoginWeb.Manage
{
    public class WriteLogHelper
    {
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void WriteLogger(string msg)
        {
            //FileStream fs = new FileStream("D:\\Log\\WriteLog.txt", FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.WriteLine(msg);  
            //sw.Close();
            //fs.Close();
            logger.Debug(msg);
        }
    }
}