using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    /// <summary>
    /// 适配提供生产信息
    /// </summary>
    public class AdaptiveSupplyModel
    {
        //TODO: 操作时间等基本信息没考虑  ，后期需要继承新基本实体
        public AdaptiveSupplyModel()
        {
            SoftInfos = new List<SoftInfo>();
            SMTPasterInfos = new List<SMTPasterInfo>();
            ToolSoftInfos = new List<Custom_BaseData_ToolSoft>();
            Procedure = new List<string>();
        }
        public int ID { set; get; }
        /// <summary>
        /// 生产工序
        /// </summary>
        public List<string> Procedure { set; get; }
        /// <summary>
        /// 是否返回生产计划
        /// </summary>
        public bool IsReturnPlan { set; get; }
        /// <summary>
        /// 返回原因
        /// </summary>
        public string ReturnCause { set; get; }
        /// <summary>
        /// 变更原因
        /// </summary>
        public string ChangeCause { set; get; }
        /// <summary>
        /// 是否复检
        /// </summary>
        public bool Review { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }

        public IList<SoftInfo> SoftInfos { set; get; }

        public IList<SMTPasterInfo> SMTPasterInfos { set; get; }

        public IList<Custom_BaseData_ToolSoft> ToolSoftInfos { set; get; }
 

    }
}