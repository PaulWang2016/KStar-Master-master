using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class BDColumnView
    {
        public string WorklistDataID { get; set; }
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ValueType { get; set; }
        public bool IsVisible { get; set; }

        public string WorklistID { get; set; }
    }
}