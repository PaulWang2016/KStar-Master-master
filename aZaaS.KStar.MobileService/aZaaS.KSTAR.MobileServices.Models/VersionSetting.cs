using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class VersionSetting
    {
 
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { set; get; }
        /// <summary>
        /// logo byte64字符串
        /// </summary>
        public string LogoByte64 { set; get; }
        /// <summary>
        /// 欢迎界面
        /// </summary>
        public string InterfaceByte64 { set; get;} 
    }
}
