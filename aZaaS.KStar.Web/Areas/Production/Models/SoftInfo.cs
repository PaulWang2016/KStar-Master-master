using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    public class SoftInfo
    {  
        /// <summary>
        /// 软件分类
        /// </summary>
        public string Category { set; get; }
        /// <summary>
        /// 工具类型
        /// </summary>
        public string ToolType { set; get; }
        /// <summary>
        /// 版本文件名称
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// 大小
        /// </summary>
        public decimal Size { set; get; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? IssueDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }
    }
}