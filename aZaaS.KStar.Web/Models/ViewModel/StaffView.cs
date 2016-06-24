using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class StaffView
    {
        /// <summary>
        /// 员工系统ID
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 显示名称 username
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 固定电话号码
        /// </summary>
        public string TelNo { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        public string FaxNo { get; set; }

        /// <summary>
        /// 移动手机号码
        /// </summary>
        public string MobileNo { get; set; }       

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public string Sex { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string StaffNo { get; set; }

        public string Department { get; set; }
        public string Position { get; set; }
        public string ReportTo { get; set; }

    }
}