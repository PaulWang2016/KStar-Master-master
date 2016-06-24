using aZaaS.KStar.Web.Models.ExField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class Organization
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public string SysId { get; set; }
        public string NodeName { get; set; }
        public bool HasChildNode { get; set; }
        public string ParentID { get; set; }
        public int OrderBy { get; set; }

        //NodeExField
        //public string EnglishName_Full { get; set; }
        //public string EnglishAddress_First { get; set; }
        //public string EnglishAddress_Second { get; set; }
        //public string EnglishAddress_Third { get; set; }
        //public string ChineseName_Full { get; set; }
        //public string ChineseAddress_First { get; set; }
        //public string ChineseAddress_Second { get; set; }
        //public string ChineseAddress_Third { get; set; }
        //public string Code { get; set; }

        public IList<OrgExField> ExFields { get; set; }
    }
}