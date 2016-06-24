using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class TreeViews
    {
        public TreeViews()
        {
            items = new List<TreeViews>();
        }

        public string Name { get; set; }
        public string Link { get; set; }
        public string Target { get; set; }
        public string ID { get; set; }
        public bool HasChildren { get; set; }
        public string ParentID { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public string IconKey { get; set; }
        public string OrderBy { get; set; }
        public object Data { get; set; }
        public List<TreeViews> items { get; set; }
    }
}