using System;
using System.Collections.Generic;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Models;
using System.Web;
using System.IO;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Helper;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IStorageProvider
    {
        KStarFormModel ReadForm(int formId, string loggedOnUser);
        int SaveForm(KStarFormModel formModel);
        void UpdateForm(KStarFormModel formModel);
        void UpdateForm(int formId, string processFolio);
        void UpdateFormContent(int formId, string contentData);
        void UpdateFormDraftStatus(int formId, bool isDraft = true);
        void UpdateFormDraftStatus(int formId, int procInstId, string processName, string processFolio, bool isDraft = true);
        void UpdateFormDraftUrl(int formId, string draftUrl);
        void DeleteForm(int formId);
        IList<MyDraftModel> GetDrafts(string userName);
        IList<MyDraftModel> GetDraftsByPage(string userName,DateTime? startDate, DateTime? endDate, string folio, int pageIndex, int pageSize, List<SortDescriptor> sort, out int total);
        void DeleteDraft(int draftId);
        int GetFormId(int procInstId);
        ProcessFormHeader GetProcessFormHeaderByProcInstId(int procInstId);
        ProcessFormHeader GetProcessFormHeaderByFormId(int formId);
        IEnumerable<ProcessFormHeader> GetInsteadRequest(string applyAccount);
        ProcessFormContent GetProcessFormContent(int formId);
        Activity GetActivityByViewMode(int formId, string userName);
        HashSet<int> GetOperActivitys(int formId, string userName);
        IList<AttachmentModel> AttachmentUpload(HttpFileCollectionBase files, string uploader, string activityName, string downloadUrl);
        AttachmentModel GetDownloadFile(Guid fileGuid);
        Stream GetFetchFile(AttachmentModel file);
        CacheFileModel GetCachedFile(Guid fileGuid);
        void ProcInstCancel(int procInstId);
        bool IsApplicatant(int formId, string userName);
        void SaveApprovalDraft(string serialNo, string actionName, string actionComment);
        ProcessFormApprovalDraft GetProcessFormApprovalDraft(string serialNo);
    }
}
