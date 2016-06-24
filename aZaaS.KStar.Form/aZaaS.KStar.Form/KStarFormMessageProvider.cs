using System;
using System.Collections.Generic;
using System.Text;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form
{
    public class KStarFormMessageProvider : IFormMessageProvider
    {
        public ResultMessage GetSaveSuccessMsg(StorageContext storageContext)
        {
            string msg = string.Format("Save successfully! <br/> Reference FormId is: {0}", storageContext.FormId);

            return new ResultMessage(MessageType.Warning, msg);
        }

        public ResultMessage GetSubmitSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format(@"
                                                Submit successfully! <br/> 
                                                Folio is {0} <br/>
                                                Reference FormId is  {1}", taskContext.Folio, taskContext.FormId);
            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetRedirectSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, string userName)
        {
            string msg = string.Format("Redirect successfully! <br/> Redirect By User: {0}", userName);

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetDelegateSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, List<string> userList)
        {
            var users = new StringBuilder();
            userList.ForEach(user => users.AppendFormat("{0};", user));
            string msg = string.Format("Delegate successfully! <br/> Delegate By Users: {0}", users.ToString());

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetGotoActivitySuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, string activityName)
        {
            string msg = string.Format("GotoActivity successfully! <br/> GotoActivity By ActivityName: {0}", activityName);

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetAddSignerSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext, List<string> userList)
        {
            var signers = new StringBuilder();
            userList.ForEach(signer => signers.AppendFormat("{0};", signer));
            string msg = string.Format("AddSigner successfully! <br/> AddSigner By Users: {0}", signers.ToString());

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetCarbonCopySuccessMsg(List<string> userList)
        {
            var users = new StringBuilder();
            userList.ForEach(user => users.AppendFormat("{0};", user));
            string msg = string.Format("CC successfully! <br/> CC By Users: {0}", users.ToString());

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetReviewSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format("Review successfully! <br/> Reference FormId is: {0}", storageContext.FormId);

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetDeleteSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format("Cancel successfully! ");

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetUndoSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format("Undo successfully! ");

            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage GetSaveFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetSubmitFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetRedirectFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetDelegateFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetGotoActivityFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetAddSignerFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetCarbonCopyFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetReviewFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetDeleteFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }

        public ResultMessage GetUndoFailMsg(Exception ex)
        {
            return new ResultMessage(MessageType.Error, ex.Message);
        }      

    }
}
