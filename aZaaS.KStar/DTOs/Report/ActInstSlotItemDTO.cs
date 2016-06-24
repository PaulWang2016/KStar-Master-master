using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
   public class ActInstSlotItemDTO
    {


       /// <summary>
       /// 审批人
       /// </summary>
       public string User { get; set; }

        //所占的百分比
        public double Percentage { get; set; }

        //平均耗时分钟
        public int Avg_Consuming_Second { get; set; }



    }
}
