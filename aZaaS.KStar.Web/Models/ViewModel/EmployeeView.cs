using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class EmployeeView
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffId { get; set; }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string JobTitle { get; set; }
    }
}