using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    public class ConfigFile
    { 
        public string Name { set; get; }
        /// <summary>
        /// 发布时间 
        /// </summary>
        public DateTime? IssueDate { set; get; }
        /// <summary>
        /// 大小
        /// </summary>
        public int Size { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 文件数据  临时变量
        /// </summary>
        public string FileDate { set; get; }
    }
}