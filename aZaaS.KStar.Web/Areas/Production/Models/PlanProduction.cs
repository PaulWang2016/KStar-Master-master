using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Production.Models
{

    /// <summary>
    /// 生产计划单
    /// </summary>
    public class PlanProduction
    {
          
        /// <summary>
        /// 计划单类型
        /// </summary>
        public bool PlanType { set; get; }
        /// <summary>
        /// 任务来源部门
        /// </summary>
        public SimpleUser SourceDepartment { set; get; }
        /// <summary>
        /// 生产计划单号
        /// </summary>
        public string PlanNumber { set; get; }
        /// <summary>
        /// 计划人
        /// </summary>
        public string PlannerMan { set; get; }
        /// <summary>
        /// 加工厂家
        /// </summary>
        public string ProcessingVendor { set; get; }
        /// <summary>
        /// REV版本
        /// </summary>
        public string REVVersion { set; get; }
        /// <summary>
        ///型号
        /// </summary>
        public string Model { set; get; }
        /// <summary>
        ///总加工数
        /// </summary>
        public string TotalWorkHour { set; get; }
        /// <summary>
        ///计划完工时间
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
        /// 软件版本
        /// </summary>
        public string SoftVersion { set; get; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Clientele { set; get; }
        /// <summary>
        /// 是否为客户定制产品
        /// </summary>
        public bool IsCustom { set; get; }
        /// <summary>
        /// 定制版本客户确认书 
        /// </summary>
        public string CustomFile { set; get; }
        /// <summary>
        /// 是否定制产品
        /// </summary>
        public bool IsCustomProduction { set; get; }
        /// <summary>
        /// 标贴要求说明
        /// </summary>
        public string CustomProductionTextarea { set; get; }
        /// <summary>
        /// 波特率
        /// </summary>
        public string  BaudRate { set; get; }
        /// <summary>
        /// 是否质检
        /// </summary>
        public bool QualityTesting { set; get; }
        /// <summary>
        /// 号段
        /// </summary>
        public string Dnseg { set; get; }
        /// <summary>
        /// 贴标代码
        /// </summary>
        public string DecalsCode { set; get; }
        /// <summary>
        /// 碳带代码
        /// </summary>
        public string TTRCode { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 软件经理
        /// </summary>
        public SimpleUser SoftManager { set; get; }
   
        /// <summary>
        /// 配置文件提供人
        /// </summary>
        public SimpleUser ConfigurationMan { set; get; }

        /// <summary>
        /// 生技工程师
        /// </summary>
        public SimpleUser EngineerMan { set; get; }
       
        /// <summary>
        /// 发放人
        /// </summary>
        public SimpleUser IssueMan { set; get; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime StartDate { get; set; }

        public ManagerReviewPlan ManagerReview { set; get; }
         
    }
    /// <summary>
    /// 项目经理审核
    /// </summary>
    public class ManagerReviewPlan 
    {
        /// <summary>
        /// 生产状态
        /// </summary>
        public string ProductionState { set; get; }
        /// <summary>
        /// 生产报告
        /// </summary>
        public string ProductionReport { set; get; }

        /// <summary>
        /// 标贴模板
        /// </summary>
        public string Labeltemplate { set; get; }

        /// <summary>
        /// 标贴模板
        /// </summary>
        public string LabeltemplateNumber { set; get; }

        /// <summary>
        /// 指派跟线工程师
        /// </summary>
        public SimpleUser EngineerMan { set; get; }
    }
  
}