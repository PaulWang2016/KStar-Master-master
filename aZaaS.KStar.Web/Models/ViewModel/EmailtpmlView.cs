using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class EmailtpmlView
    {
        /// <summary>
        /// 模版ID
        /// </summary>
        public string TpmlID { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string Process { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 标题信息
        /// </summary>
        public string SubjectInfo { get; set; }
        ///// <summary>
        ///// 标题备注
        ///// </summary>
        //public string SubjectRemark { get; set; }
        /// <summary>
        /// 内容信息
        /// </summary>
        public string ContentInfo { get; set; }
        ///// <summary>
        ///// 内容备注
        ///// </summary>
        //public string ContentRemark { get; set; }
        /// <summary>
        /// 模版类型
        /// </summary>
        public string TpmlName { get; set; }
    }
}