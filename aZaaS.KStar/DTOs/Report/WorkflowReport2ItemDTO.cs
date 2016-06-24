using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
  public  class WorkflowReport2ItemDTO
    {
        public int ActivityID { get; set; }


      //环节名称
        public string ActivityName { get; set; }

      //所占的百分比
        public double Percentage { get; set; }

      //平均耗时秒钟
        public int Avg_Consuming_Second { get; set; }


    }
}
