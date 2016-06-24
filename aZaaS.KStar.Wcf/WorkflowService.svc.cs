using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using aZaaS.KStar.Form;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Wcf.DataContracts;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WorkflowService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WorkflowService.svc or WorkflowService.svc.cs at the Solution Explorer and start debugging.
    public class WorkflowService : IWorkflowService
    {
        private readonly ConfigManager _workflowConfigManager;
        private readonly KStarFormWorkflowService _formWorkflowService;
        private readonly KStarFormStorageProvider _formStorageProvider;
        private readonly KStarFormOrganizationService _organizationService;

        private const string COMMON_DATAFIELD_FORMID = "FormId";

        public WorkflowService()
        {
            _workflowConfigManager = new ConfigManager(AuthenticationType.Windows);
            _formStorageProvider = new KStarFormStorageProvider();
            _organizationService = new KStarFormOrganizationService();
            _formWorkflowService = new KStarFormWorkflowService(AuthenticationType.Windows);
        }

        public int StartWorkflow(SubmitForm formData, string processName, Dictionary<string, object> datafields)
        {
            var procInstID = 0;
            var formModel = ToKStarFormModel(formData);

            var formId = _formStorageProvider.SaveForm(formModel);
            if (datafields == null)
                datafields = new Dictionary<string, object>()
                {
                   { COMMON_DATAFIELD_FORMID,formId}
                };
            else
                datafields.Add(COMMON_DATAFIELD_FORMID, formId);

            try
            {
                procInstID = _formWorkflowService.StarNewTask(formData.Submitter, processName, formData.Folio, datafields);
                _formStorageProvider.UpdateFormDraftStatus(formId, procInstID, processName, formData.Folio, false);
            }
            catch (Exception ex)
            {
                var procSetCfg = _workflowConfigManager.GetProcessSetByFullName(processName);
                if (procSetCfg != null)
                {
                    _formStorageProvider.UpdateFormDraftStatus(formId, true);
                    _formStorageProvider.UpdateFormDraftUrl(formId, string.Format("{0}?_DraftId={1}", procSetCfg.StartUrl, formId));
                }

                throw ex;
            }

            return formId;
        }

        public int StartProcessInstance(string userName, string processName, string folio, Dictionary<string, object> datafields)
        {
            return _formWorkflowService.StarNewTask(userName, processName, folio, datafields);
        }


        private KStarFormModel ToKStarFormModel(SubmitForm formData)
        {
            if (formData == null)
                throw new ArgumentNullException("formData");

            if (string.IsNullOrEmpty(formData.Submitter))
                throw new ArgumentNullException("Submitter");
            if (string.IsNullOrEmpty(formData.Applicant))
                throw new ArgumentNullException("Applicant");

            var submitter = _organizationService.GetUser(formData.Submitter);
            if (submitter == null)
                throw new InvalidOperationException("The specified submitter was not found!");

            var applicant = _organizationService.GetUserInfo(formData.Applicant);
            if (applicant == null)
                throw new InvalidOperationException("The specified applicant was not found!");

            var applicantPosition = applicant.Positions.FirstOrDefault();
            if (formData.PositionID != null)
                applicantPosition = applicant.Positions.FirstOrDefault(p => p.SysID == formData.PositionID.Value);

            var applicantDepartment = applicant.Departments.FirstOrDefault();
            if (formData.DepartmentID != null)
                applicantDepartment = applicant.Departments.FirstOrDefault(d => d.SysID == formData.DepartmentID.Value);

            if (applicantPosition == null)
                throw new InvalidOperationException("Applicant should be assigned Position!");
            if (applicantDepartment == null)
                throw new InvalidOperationException("Applicant should be assigned Department!");

            return new KStarFormModel()
            {
                SubmitDate = DateTime.Now,

                Priority = 1,
                FormSubject = formData.Subject,
                ProcessFolio = formData.Folio,

                SubmitterAccount = formData.Submitter,
                SubmitterDisplayName = string.Format("{0} {1}", submitter.FirstName, submitter.LastName),

                ApplicantAccount = formData.Applicant,
                ApplicantDisplayName = applicant.ApplicantDisplayName,
                ApplicantTelNo = applicant.ApplicantTelNo,
                ApplicantEmail = applicant.ApplicantEmail,
                ApplicantPositionID = applicantPosition.SysID.ToString(),
                ApplicantPositionName = applicantPosition.Name,
                ApplicantOrgNodeID = applicantDepartment.SysID.ToString(),
                ApplicantOrgNodeName = applicantDepartment.Name,

                SubmitComment = formData.Comment,

                ContentData = formData.JsonData,

                 Attachments = new List<AttachmentModel>(),
                 ProcessLogs = new List<ProcessLogModel>()
            };
        }
    }
}
