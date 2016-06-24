using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class DraftView
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 发送请求的人
        /// </summary>
        public string Requester { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Folio { get; set; }

        /// <summary>
        /// 客户级别
        /// </summary>
        public string CustomerLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitDate { get; set; }

        /// <summary>
        /// 提交类型
        /// </summary>
        public string Type { get; set; }

        public string ClusterCode { get; set; }

        public string CurrentActivity { get; set; }
        //public DateTime ImportantDate { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        //public string ShortDescription { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}