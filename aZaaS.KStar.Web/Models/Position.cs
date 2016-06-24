using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class Position
    {
        public string PositionID { get; set; }
        public string DisplayName { get; set; }

        public string CategoryID { get; set; }
    }
    
    public class PositionTree
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public bool HasChildren { get; set; }
        public string ParentID { get; set; }
        public string Type { get; set; }
        public string SysId { get; set; }
        public string NodeName { get; set; }
        public int OrderBy { get; set; }
    }
    public enum PositionType
    {
        Position,
        Category
    }
}