using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.BasisEntity
{
    public class UserEntire
    { 
        public string UserId { get; set; }
        public System.Guid SysId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public System.Guid Cluster_SysId { get; set; }
        public string Cluster_Name { get; set; }
        public string Cluster_Type { get; set; }

        /// <summary>
        /// 科室
        /// </summary>
        public System.Guid Property_SysId { get; set; }
        public string Property_Name { get; set; }
        public string Property_Type { get; set; }
        public string Type { get; set; }
        public Nullable<System.Guid> Parent_SysId { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public System.Guid Position_SysId { get; set; }
        public string Position_Name { get; set; }
    }
}