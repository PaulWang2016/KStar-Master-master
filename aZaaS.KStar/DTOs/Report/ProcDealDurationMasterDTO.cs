using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class ProcDealDurationMasterDTO
    {
        public int ProcInstID { get; set; }

        public string Folio { get; set; }

        public string YFProcInstID
        {
            get
            {
                string[] folios = Folio.Split('-');
                return folios.Length >= 2 ? folios[folios.Length - 1] : ProcInstID.ToString();
            }
        }
        
        public string ProcessName { get; set; }
    
        public DateTime Startswith { get; set; }

        public DateTime Finishwith { get; set; }

        public string StartUser { get; set; }

        public int TotalConsumingSecond { get; set; }
        public string TotalConsumingSecondStr { get; set; }

        public string ActiveActName { get; set; }

        public int Status { get; set; }
        public string StatusStr { get { return ProcInstStatusGroupDTO.getWorkflowStatusDesc(Status); } }


    }

    public class ProcInstStatusGroupDTO
    {
        public int Status { get; set; }
        public string StatusStr { get { return getWorkflowStatusDesc(Status); } }
        public int Num { get; set; }

        /// <summary>
        /// 获取流程状态的描述
        /// </summary>
        /// <param name="status">状态编号</param>
        /// <returns></returns>
        static public string getWorkflowStatusDesc(int status)
        {
            switch (status)
            {
                case 0:
                    return "错误";
                case 1:
                    return "运行中";
                case 2:
                    return "进行中";
                case 3:
                    return "已完成";
                case 4:
                    return "已停止";
                case 5:
                    return "作废";
                default:
                    break;
            }
            return "Unkown";
        }
    }
}
