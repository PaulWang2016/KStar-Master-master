using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class ProcessApprovalConsumingSecondDTO
    {

        public int ProcInstID { get; set; }

        /// <summary>
        /// 共多少秒
        /// </summary>
        public int CasumeSecond { get; set; }

        public string CasumeSecondFomatStr { get; set; }

        /// <summary>
        /// 共多少分钟
        /// </summary>
        public double CasumeMinute { get { return CasumeSecond * 1.0 / 60; } }

        /// <summary>
        /// 共多少小时
        /// </summary>
        public double CasumeHour { get { return CasumeSecond * 1.0 / 3600; } }

        public string ActivityName { get; set; }

        public string Folio { get; set; }

        public string YFProcInstID
        {
            get
            {
                string[] folios = Folio.Split('-');
                return folios.Length == 2 ? folios[1] : ProcInstID.ToString();
            }
        }
    }
}
