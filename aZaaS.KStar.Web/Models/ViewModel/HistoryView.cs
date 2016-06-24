using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class HistoryView
    {

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 阶段
        /// </summary>
        public string Stage { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Date { get; set; }


        /// <summary>
        /// 行动
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 任务负责人
        /// </summary>
        public string TaskOwner { get; set; }


        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }
    }
}