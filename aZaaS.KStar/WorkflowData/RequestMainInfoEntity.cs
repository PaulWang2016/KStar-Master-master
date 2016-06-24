using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.WorkflowData
{
    public class RequestMainInfoEntity : AbstractEntity
    {
        public string FormNo { get; set; }
        public int ProcInstID { get; set; }
        public string FormType { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantAccount { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantDepartment { get; set; }
        public string ApplicantJobTitle { get; set; }
        public string RequesterAccount { get; set; }
        public string RequesterName { get; set; }
        public string RequesterJobTitle { get; set; }
        public string RequesterEmail { get; set; }
        public string RequesterDepartment { get; set; }
        public string FormStatus { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string RecordLevelType { get; set; }
        public string TradeMix { get; set; }
        public string CustomerType { get; set; }
        public string UnitType { get; set; }
        public string CustomerCode { get; set; }
        public string PropertyCode { get; set; }
        public string UnitCode { get; set; }
        public string RecordContent { get; set; }
        public string RefFormID { get; set; }
        public string ChangeReason { get; set; }
        public string Comment { get; set; }

        #region Reviewer Info
        public string ReviewerAccount { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerJobTitle { get; set; }
        public string ReviewerDepartment { get; set; }
        public string RecipReviewerAccount { get; set; }
        public string RecipReviewerName { get; set; }
        public string RecipReviewerJobTitle { get; set; }
        public string RecipReviewerDepartment { get; set; }
        public string ReviewerEmail { get; set; }
        public string RecipReviewerEmail { get; set; }
        #endregion

        #region Last WorkflowStep Info
        public string ActivityState { get; set; }
        public DateTime? ActivityDate { get; set; }
        #endregion

        #region Action Info
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        #endregion


        //[NotMapped]
        //public virtual IList<AttachmentInfoEntity> AttachmentInfoItems { get; set; }
        [NotMapped]
        public virtual IList<ChangeLogEntity> ChangeLogItems { get; set; }

        


        public void Update(RequestMainInfoEntity form)
        {
            this.ID = form.ID;
            this.FormNo = form.FormNo;
            this.ProcInstID = form.ProcInstID;
            this.FormType = form.FormType;
            this.RequesterAccount = form.RequesterAccount;
            this.RequesterName = form.RequesterName;
            this.RequesterJobTitle = form.RequesterJobTitle;
            this.RequesterDepartment = form.RequesterDepartment;
            this.RequesterEmail = form.RequesterEmail;
            this.FormStatus = form.FormStatus;
            this.ApplicationDate = form.ApplicationDate;
            this.RecordLevelType = form.RecordLevelType;
            this.TradeMix = form.TradeMix;
            this.CustomerType = form.CustomerType;
            this.UnitType = form.UnitType;
            this.CustomerCode = form.CustomerCode;
            this.PropertyCode = form.PropertyCode;
            this.UnitCode = form.UnitCode;
            this.RecordContent = form.RecordContent;
            this.RefFormID = form.RefFormID;
            this.ChangeReason = form.ChangeReason;
            this.Comment = form.Comment;
            this.ReviewerName = form.ReviewerName;
            this.ReviewerAccount = form.ReviewerAccount;
            this.ReviewerJobTitle = form.ReviewerJobTitle;
            this.ReviewerDepartment = form.ReviewerDepartment;
            this.RecipReviewerAccount = form.RecipReviewerAccount;
            this.RecipReviewerName = form.RecipReviewerName;
            this.RecipReviewerJobTitle = form.RecipReviewerJobTitle;
            this.RecipReviewerDepartment = form.RecipReviewerDepartment;
            this.ReviewerEmail = form.ReviewerEmail;
            this.RecipReviewerEmail = form.RecipReviewerEmail;

            this.ActivityState = string.IsNullOrEmpty(form.ActivityState) ? this.ActivityState : form.ActivityState;
            this.ActivityDate = form.ActivityDate == null ? this.ActivityDate : form.ActivityDate;

            //this.CreatedBy = form.CreatedBy;
            //this.CreatedAt = form.CreatedAt;
            this.ModifiedBy = form.ModifiedBy;
            this.ModifyDate = form.ModifyDate;
        }

        public void UpdateRefFrom(RequestMainInfoEntity form)
        {
            //this.ID = form.ID;
            //this.FormNo = form.FormNo;
            //this.ProcInstID = form.ProcInstID;
            //this.FormType = form.FormType;
            //this.RequesterAccount = form.RequesterAccount;
            //this.RequesterName = form.RequesterName;
            //this.RequesterJobTitle = form.RequesterJobTitle;
            //this.RequesterDepartment = form.RequesterDepartment;
            //this.RequesterEmail = form.RequesterEmail;
            //this.FormStatus = form.FormStatus;
            //this.ApplicationDate = form.ApplicationDate;
            this.RecordLevelType = form.RecordLevelType;
            this.TradeMix = form.TradeMix;
            this.CustomerType = form.CustomerType;
            this.UnitType = form.UnitType;
            this.CustomerCode = form.CustomerCode;
            this.PropertyCode = form.PropertyCode;
            this.UnitCode = form.UnitCode;
            this.RecordContent = form.RecordContent;
            //this.RefFormID = form.RefFormID;
            //this.ChangeReason = form.ChangeReason;
            //this.Comment = form.Comment;


            //this.ReviewerName = form.ReviewerName;
            //this.ReviewerAccount = form.ReviewerAccount;
            //this.ReviewerJobTitle = form.ReviewerJobTitle;
            //this.ReviewerDepartment = form.ReviewerDepartment;
            //this.RecipReviewerAccount = form.RecipReviewerAccount;
            //this.RecipReviewerName = form.RecipReviewerName;
            //this.RecipReviewerJobTitle = form.RecipReviewerJobTitle;
            //this.RecipReviewerDepartment = form.RecipReviewerDepartment;
            //this.ReviewerEmail = form.ReviewerEmail;
            //this.RecipReviewerEmail = form.RecipReviewerEmail;

            //this.CreatedBy = form.CreatedBy;
            //this.CreatedAt = form.CreatedAt;
            this.ModifiedBy = form.ModifiedBy;
            this.ModifyDate = form.ModifyDate;
        }
    }
}