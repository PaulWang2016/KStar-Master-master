using System;
using System.Collections.Generic;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormMessageProvider
    {
        ResultMessage GetSaveSuccessMsg(StorageContext storageContext);

        ResultMessage GetSubmitSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext);

        ResultMessage GetRedirectSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, string userName);

        ResultMessage GetDelegateSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, List<string> userList);

        ResultMessage GetGotoActivitySuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, string activityName);

        ResultMessage GetAddSignerSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, List<string> userList);

        ResultMessage GetCarbonCopySuccessMsg(List<string> userList);

        ResultMessage GetReviewSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext);

        ResultMessage GetDeleteSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext);

        ResultMessage GetUndoSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext);

        ResultMessage GetSaveFailMsg(Exception ex);

        ResultMessage GetSubmitFailMsg(Exception ex);

        ResultMessage GetRedirectFailMsg(Exception ex);

        ResultMessage GetDelegateFailMsg(Exception ex);

        ResultMessage GetGotoActivityFailMsg(Exception ex);

        ResultMessage GetAddSignerFailMsg(Exception ex);

        ResultMessage GetCarbonCopyFailMsg(Exception ex);

        ResultMessage GetReviewFailMsg(Exception ex);

        ResultMessage GetDeleteFailMsg(Exception ex);

        ResultMessage GetUndoFailMsg(Exception ex);


    }
}
