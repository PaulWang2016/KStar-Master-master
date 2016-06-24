
using aZaaS.KStar.WorkflowData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class TenantInfoView
    {
        public Guid Id { get; set; }
        public string ID_TDW { get; set; }
        public string DivisionCode { get; set; }
        public string ClusterCode { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyName { get; set; }
        public string UnitCode { get; set; }
        public string UnitType { get; set; }
        public string RecordType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string TradeMix { get; set; }
        public string FormType { get; set; } //Record Type
        public string RecordLevel { get; set; } //Record level
        public DateTime SubmitDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string UnitAddress { get; set; }
        public string LeaseID { get; set; }
        public string TenantName { get; set; }
        public DateTime? LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public string RequesterName { get; set; }
        public DateTime RequestCreationDate { get; set; }
        public DateTime RequestCompletionDate { get; set; }
        public string TradeName { get; set; }
        public virtual AbstractEntity RecordContent { get; set; }
    }
}