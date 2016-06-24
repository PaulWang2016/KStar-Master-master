using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace aZaaS.KStar.Wcf.DataContracts
{
    [DataContract]
    public class DelegateInfo
    {
        /// <summary>
        /// 代理 编号
        /// </summary>
        [DataMember]
        public int DelegationID { get; set; }

        /// <summary>
        /// 过程
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// 托办人
        /// </summary>
        [DataMember]
        public string FromUser { get; set; }   

        /// <summary>
        /// 代理人
        /// </summary>
        [DataMember]
        public string ToUser { get; set; }

        /// <summary>
        /// 代理开始时间
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 代理结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public bool IsEnable { get; set; }
    }
}