using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace UserSync
{
    public class Logging
    {
        public string createLogFile()
        {
            string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
            string LogFileNamePrefix = ConfigurationManager.AppSettings["LogFileNamePrefix"];

            // Create full log file path
            string fullFilePath = string.Format("{0}\\{1}_{2}{3}{4}.txt", logFilePath, LogFileNamePrefix, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            // Create log directory if it doesn't exist
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            return fullFilePath;
        }

        public void ProcessLog(string message)
        {
            using (StreamWriter sw = new StreamWriter(createLogFile(), true))
            {
                sw.WriteLine(string.Format("{0}: {1}", DateTime.Now, message));
            }
        }
    }
}
