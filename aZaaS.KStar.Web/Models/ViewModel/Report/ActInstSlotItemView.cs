using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel.Report
{
    public class ActInstSlotItemView
    {

        /// <summary>
        /// 审批人
        /// </summary>
        public string User { get; set; }

        //所占的百分比
        public string Percentage { get; set; }

        //平均耗时分钟
        public string Avg_Consuming_Second { get; set; }

        public string Avg_Consuming_Second_Value { get; set; }



    }
}