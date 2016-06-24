using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class BDConfigView
    {
        public string WorklistID { get; set; }
        public string ApplicationName { get; set; }
        public string ProcessName { get; set; }
        public string ConnectionString { get; set; }
        public string DataTable { get; set; }
        public string WhereQuery { get; set; }
    }
}