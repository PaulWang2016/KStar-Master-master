using aZaaS.KStar.MgmtDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class DelegationView
    {
        /// <summary>
        /// 代理 编号
        /// </summary>
        public int DelegationID { get; set; }

        /// <summary>
        /// 过程
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 托办人
        /// </summary>
        public string FromUser { get; set; }

        /// <summary>
        /// 托办人
        /// </summary>
        public List<UserBaseDto> FromUserName { get; set; }

        /// <summary>
        /// 代理人
        /// </summary>
        public string ToUser { get; set; }

        /// <summary>
        /// 代理人
        /// </summary>
        public List<UserBaseDto> ToUserName { get; set; }

        /// <summary>
        /// 代理开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 代理结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; }
    }
}