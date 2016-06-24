using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    /// <summary>
    /// SMT贴片信息
    /// </summary>
    public class SMTPasterInfo
    { 
        /// <summary>
        /// PCAB 代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 光绘贴片文件
        /// </summary>
        public string FileDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }
    }
}