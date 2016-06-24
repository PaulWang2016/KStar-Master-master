using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class FavouriteTreeItem
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public bool HasChildren { get; set; }
        public string ParentID { get; set; }
        public string Type { get; set; }
    }
}