using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class CustomRoleModel
    {
        public string CustomRoleID { get; set; }
        public string DisplayName { get; set; }

        public string CategoryID { get; set; }
    }

    public class CustomCategory
    {
        public string CategoryID { get; set; }
        public string DisplayName { get; set; }
        public string CustomRoleType { get; set; }
        public string ParentID { get; set; }
        public string Comment { get; set; }
    }

    public class CustomRoleTree
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public bool HasChildren { get; set; }
        public string ParentID { get; set; }
        public string Type { get; set; }
    }

    public class CustomClassify
    {
        public string CustomRoleID { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string CategoryID { get; set; }
    }

    public enum CustomRoleTreeAddType
    {
        Category,
        Classify
    }

    public class ClassInfo
    {
        public string Key { get; set; }

        public string AssembleName { get; set; }

        public string ClassName { get; set; }

        public string RoleName { get; set; }
    }
}