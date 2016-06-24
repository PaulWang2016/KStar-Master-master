using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.BasisEntity
{
    public class EnumCollection
    {
        /// <summary>
        /// 用户选择控件选出来的类型
        /// </summary>
        public enum UserpickType
        {
            /// <summary>
            /// 部门
            /// </summary>
            Depts,
            /// <summary>
            /// 职位
            /// </summary>
            Positions,
            /// <summary>
            /// 用户
            /// </summary>
            Users,
            /// <summary>
            /// 系统角色
            /// </summary>
            SystemRoles,
            /// <summary>
            /// 自定义角色
            /// </summary>
            CustomRoles
        }
        /// <summary>
        /// 部门类型
        /// </summary>
        public enum OrgNodeType
        {
            /// <summary>
            /// 科室
            /// </summary>
            Property,
            /// <summary>
            /// 部门
            /// </summary>
            Cluster,
            /// <summary>
            /// 子公司
            /// </summary>
            Division,
            /// <summary>
            /// 集团级别
            /// </summary>
            Company
        }
        public class ActionName
        {
            /// <summary>
            /// 同意
            /// </summary>
            public const string Consent = "同意";
            /// <summary>
            /// 不同意
            /// </summary>
            public const string Disagree = "不同意";
        }

        /// <summary>
        /// 功能权限集合
        /// </summary>
        public enum FeaturePermissionCode
        { 
            /// <summary>
            /// 外网权限Code
            /// </summary>
            InternetRole,
            /// <summary>
            /// 外网下载权限
            /// </summary>
            InternetDownloadRole
        }
    }



    public enum StatusEnum : int
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disabled = -1,
        /// <summary>
        /// 冻结
        /// </summary>
        Freezing = 0,
        /// <summary>
        /// 可用
        /// </summary>
        Available = 1,
        /// <summary>
        /// 销假完成
        /// </summary>
        LeaveOff = 2
    }
    /// <summary>
    /// 职位   1:员工;2:科长;3:部长/副总经理;4:子公司总经理;5:集团总经理
    /// </summary>
    public enum UserLevel : int
    {
        Employee = 1,
        Section = 2,
        Minister = 3,
        Manager = 4,
        CEO = 5

    }

}