using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSOLoginWeb.Manage
{
    public class EnumCollection
    {
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
}
