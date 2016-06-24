using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    public class AdaptivePlanModel
    {
        public AdaptivePlanModel()
        {
            PlanProduction = new PlanProduction();
            MadeChanges = new MadeChange();
            ConfigFiles = new List<ConfigFile>();
         
        }
        public PlanProduction PlanProduction { set; get; }

        public MadeChange MadeChanges { set; get; }

        public IList<ConfigFile> ConfigFiles { set; get; }

        public AdaptiveSupplyModel AdaptiveSupplyModels { set; get; }
        /// <summary>
        /// 软件版本全称
        /// </summary>
        public string SoftwareVersionfullName { set; get; }
        /// <summary>
        /// 号段(IMEI/MEID/MAC) 
        /// </summary>
        public string Dnsegdocument { set; get; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool DetectionPass { set; get; }
        /// <summary>
        /// 计划单号
        /// </summary>
        public string PlanBillNO { set; get; }

        /// <summary>
        ///QC 是否通过
        /// </summary>
        public string QCDetectionPass { set; get; }

        /// <summary>
        /// 项目经理
        /// </summary>
        public SimpleUser ProjectManager { set; get; }
         
    }
}