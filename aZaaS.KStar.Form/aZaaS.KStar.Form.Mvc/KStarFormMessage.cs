using System;
using System.Collections.Generic;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Mvc
{
    public class KStarFormMessage
    {
        public static Func<StorageContext, ResultMessage> SaveSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, ResultMessage> SubmitSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, string, ResultMessage> RedirectSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, List<string>, ResultMessage> DelegateSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext,string, ResultMessage> GotoActivitySuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, List<string>, ResultMessage> AddSignerSuccessMsg;

        public static Func<List<string>, ResultMessage> CarbonCopySuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, ResultMessage> ReviewSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, ResultMessage> DeleteSuccessMsg;

        public static Func<WorkflowTaskContext, StorageContext, ResultMessage> UndoSuccessMsg;

        public static Func<Exception, ResultMessage> SaveFailMsg;

        public static Func<Exception, ResultMessage> SubmitFailMsg;

        public static Func<Exception, ResultMessage> RedirectFailMsg;

        public static Func<Exception, ResultMessage> DelegateFailMsg;

        public static Func<Exception, ResultMessage> GotoActivityFailMsg;

        public static Func<Exception, ResultMessage> AddSignerFailMsg;

        public static Func<Exception, ResultMessage> CarbonCopyFailMsg;

        public static Func<Exception, ResultMessage> ReviewFailMsg;

        public static Func<Exception, ResultMessage> DeleteFailMsg;

        public static Func<Exception, ResultMessage> UndoFailMsg;
    }
}
