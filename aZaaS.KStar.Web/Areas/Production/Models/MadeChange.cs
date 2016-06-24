using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{
    public class MadeChange
    { 

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Clientele { set; get; }

        /// <summary>
        ///型号
        /// </summary>
        public string Model { set; get; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 计划人
        /// </summary>
        public string PlannerMan { set; get; }
        /// <summary>
        /// 生产计划单号
        /// </summary>
        public string PlanNumber { set; get; }
        /// <summary>
        ///要求完工时间
        /// </summary>
        public DateTime PlanFinishDate { set; get; }
        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductionCode { set; get; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductionName { set; get; }

        /// <summary>
        /// 改变产品代码
        /// </summary>
        public string ChangeProductionCode { set; get; }
        /// <summary>
        /// 改变后产品名称
        /// </summary>
        public string ChangeProductionName { set; get; }
        /// <summary>
        /// 贴标类型
        /// </summary>
        public bool DecalsType { set; get; }
        /// <summary>
        /// 是否改造
        /// </summary>
        public bool TransformCause { set; get; }
        /// <summary>
        /// 是否升级软件
        /// </summary>
        public bool IsUpgradeSoft { set; get; }
        /// <summary>
        /// 改变之前软件版本
        /// </summary>
        public string BeforeSoftVersion { set; get; }

        /// <summary>
        /// 改变之后的软件版本
        /// </summary>
        public string AfterSoftVersion { set; get; }

        /// <summary>
        /// 是否修改波特率
        /// </summary>
        public bool IsUpdateBaudRate { set; get; }
        /// <summary>
        /// 之前波特率
        /// </summary>
        public string BeforeBaudRate { set; get; }
        /// <summary>
        /// 之后波特率
        /// </summary>
        public string AfterBaudRate { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 改造人
        /// </summary>
        public SimpleUser ChangeMan { set; get; }
        /// <summary>
        /// 发放人
        /// </summary>
        public SimpleUser IssueMan { set; get; }

        public ManagerReviewMade ManagerReview { set; get; }

    }
    /// <summary>
    /// 项目经理审核 （改）
    /// </summary>
    public class ManagerReviewMade
    {
        /// <summary>
        /// 改造流程
        /// </summary>
        public string TransformProcess { set; get; }

        /// <summary>
        /// 是否更换标贴
        /// </summary>
        public bool IsReplaceLabel { set; get; }

        /// <summary>
        /// 标贴模板
        /// </summary>
        public string Labeltemplate { set; get; }

        public string LabeltemplateNumber { set; get; }
        
    }
}