using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IExecutionBehavior
    {
        void OnWorkflowNewTaskStarting(WorkflowTaskContext context);
        void OnWorkflowNewTaskStarted(WorkflowTaskContext context);

        void OnWorkflowTaskDelegating(WorkflowTaskContext context);
        void OnWorkflowTaskDelegated(WorkflowTaskContext context);

        void OnWorkflowTaskRedirecting(WorkflowTaskContext context);
        void OnWorkflowTaskRedirected(WorkflowTaskContext context);

        void OnWorkflowTaskExecuting(WorkflowTaskContext context);
        void OnWorkflowTaskExecuted(WorkflowTaskContext context);

        void OnWorkflowTaskAddingSigner(WorkflowTaskContext context);
        void OnWorkflowTaskAddedSigner(WorkflowTaskContext context);

        void OnWorkflowTaskGoingtoActivity(WorkflowTaskContext context, string activityName);
        void OnWorkflowTaskGotoActivity(WorkflowTaskContext context, string activityName);

        void OnAttachmentUploading();
        void OnAttachmentUploaded();
    }
}
