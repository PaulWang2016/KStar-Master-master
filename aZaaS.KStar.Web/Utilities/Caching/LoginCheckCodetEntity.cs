using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities.Cache
{
    public class LoginCheckCodetEntity
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string address;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userID;
        /// <summary>
        /// 错误数
        /// </summary>
        public int errorCount;
        /// <summary>
        /// 
        /// </summary>
        public DateTime createTime;
    }
}