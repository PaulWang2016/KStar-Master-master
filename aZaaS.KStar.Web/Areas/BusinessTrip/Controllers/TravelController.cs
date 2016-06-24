using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.ViewResults;
using aZaaS.KStar.Form.Mvc.Controllers;
using aZaaS.KStar.Form.ViewModels;

using aZaaS.KStar.Web.Areas.BusinessTrip.Models;
using aZaaS.KStar.Web.Areas.BusinessTrip.Services;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Controllers
{
    public class TravelController : KStarFormController
    {
        public TravelController()
            : base()//使用KStarForm模板
        {

        }

        /// <summary>
        /// KStarForm业务视图,
        /// 创建任意MVC View作为业务表单的视图，在视图中进行业务扩展以及模型数据绑定。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //TODO:
            ViewBag.Title = "差旅申请";

            //初始化并注册业务数据模型至KStarForm模板
            var model = new TravelRequestModel();

            /*使用KStarForm模板必须返回KStarFormView(Model)*/
            return KStarFormView(model);
        }

        /// <summary>
        /// KStarForm进行表单数据存储后会触发该方法，
        /// 可以重写该方法进行表单数据存储后的同步数据工作，与OnAfterDataStored不同的是，
        /// 该方法只在数据存储之后触发，而已OnAfterDataStored会在数据被存储或者更新时触发。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object OnDataStoring(StorageContext context)
        {
            //E.g:将差旅数据同步存储至本地数据库
            var dataService = new BusinessTripDataService(context);
            return  dataService.AddTravelRequest();
        }

        /// <summary>
        /// KStarForm发起表单流程前会触发该方法，
        /// 可以重写该方法设置流程发起名称以及Folio,并设置对应流程实例的DataField，根据规范
        /// 流程会默认包含[FormId]这个DataField,KStarForm也默认会把当前表单Id设置给这个字段。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        {
            context.ProcessName = this.ProcessName;
            context.Folio = string.Empty;//因选择在流程节点中生成Folio,所以这里不设置Folio。

            //E.g:从上下文获取业务数据模型，并读取对应的业务数据字段
            var model = context.DataModel<TravelRequestModel>();
            context.AddDataField("IsUsingCar", model == null ? false : model.IsUsingCar);

            //添加/设置其他自定义的流程DataField字段
            //context.AddDataField("myField", "FieldValue");
        }





        /*********************************************************************************
         * 其他可重写方法参考（根据实际业务需求进行重写）
         * ******************************************************************************/

        /// <summary>
        /// KStarForm表单数据存储前数据验证会触发该方法，
        /// 可以重写该方法对实际业务表单进行数据进行验证，通过返回True/False来标识验证结果，
        /// 如果验证结果为Flase，KStarForm会停止执行后边的代码，停止数据存储以及触发后续操作。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool OnDataValidating(StorageContext context)
        {
            return base.OnDataValidating(context);
        }

        /// <summary>
        /// KStarForm进行表单数据更新后触发该方法，
        /// 与OnDataStoring类似，该方法只在数据被更新后触发，之后触发OnAfterDataStored，
        /// 重写方法同样可以在数据被更新后同步更新数据工作。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object OnDataUpdating(StorageContext context)
        {
            return base.OnDataUpdating(context);
        }

        /// <summary>
        /// KStarForm执行表单草稿存储之后触发该方法，
        /// 可以重写方法进行表单数据草稿存储之后同步数据，获取草稿存储Url地址信息等工作。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object OnDataDrafting(StorageContext context)
        {
            return base.OnDataDrafting(context);
        }

        /// <summary>
        /// KStarForm数据存储异常后会触发该方法，
        /// 可以重写该方法可获取上下文以及异常数据信息，对数据存储的异常进行记录或者处理等工作。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void OnDataStoredError(StorageContext context, Exception exception)
        {
            base.OnDataStoredError(context, exception);
        }

        /// <summary>
        /// KStarForm存储表单数据前触发该方法，
        /// 可以重写该方法在表单数据被存储之前做数据同步或者数据操作日志，或者检查前置行为等工作。
        /// </summary>
        /// <param name="context"></param>
        public override void OnBeforeDataStored(StorageContext context)
        {
            base.OnBeforeDataStored(context);
        }

        /// <summary>
        /// KStarForm存储/更新表单数据后触发该方法，
        /// 可以重写该方法实现自己的数据存储逻辑，对应的上下文数据可从StorageContext获取得到。
        /// </summary>
        /// <param name="context"></param>
        public override void OnAfterDataStored(StorageContext context)
        {
            base.OnAfterDataStored(context);
        }

        /// <summary>
        /// KStarForm流程表单发起后会触发该方法，
        /// 可以重写该方法进行流程发起后数据更新，比如更新表单状态以及同步数据至其他系统等工作。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowNewTaskStarted(WorkflowTaskContext context)
        {
            base.OnWorkflowNewTaskStarted(context);
        }

        /// <summary>
        /// KStarForm执行工作项审批动作前触发该方法，
        /// 可以重写该方法进行工作项被执行前的工作，比如获取当前要执行的节点，根据节点做数据更新等。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskExecuting(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskExecuting(context);
        }

        /// <summary>
        /// KStarForm执行工作项审批动作后触发该方法，
        /// 可以重写该方法进行工作相被执行后的工作，比如获取当前工作项最后执行的动作，根据动作进行数据更新等。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskExecuted(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskExecuted(context);
        }

        /// <summary>
        /// KStarForm执行工作项代理前触发该方法，
        /// 可以重写该方法进行业务相关的工作，比如连接其他内部业务系统检查当前用户是否拥有代理权限等。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskDelegating(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskDelegating(context);
        }

        /// <summary>
        /// KStarForm执行工作项代理后触发方法，
        /// 可以重写该方法进行业务相关的工作，比如将当前代理信息组织成电子邮件信息抄送给相关部门人员等。
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskDelegated(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskDelegated(context);
        }

        /// <summary>
        /// KStarForm执行工作项转交前触发该方法
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskRedirecting(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskRedirecting(context);
        }

        /// <summary>
        /// KStarForm执行工作项转交后触发该方法
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskRedirected(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskRedirected(context);
        }

        /// <summary>
        /// KStarForm执行工作项节点跳转前触发该方法，
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskGoingtoActivity(WorkflowTaskContext context, string activityName)
        {
            base.OnWorkflowTaskGoingtoActivity(context, activityName);
        }

        /// <summary>
        /// KStarForm执行工作项节点跳转后触发该方法
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskGotoActivity(WorkflowTaskContext context, string activityName)
        {
            base.OnWorkflowTaskGotoActivity(context, activityName);
        }

        /// <summary>
        /// KStarForm执行工作项加签前触发该方法
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskAddingSigner(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskAddingSigner(context);
        }

        /// <summary>
        /// KStarForm执行工作项加签后触发该方法
        /// TODO:
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskAddedSigner(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskAddedSigner(context);
        }

        /// <summary>
        /// KStarForm执行附件存储前触发该方法
        /// TODO:
        /// </summary>
        public override void OnAttachmentUploading()
        {
            base.OnAttachmentUploading();
        }

        /// <summary>
        /// KStarForm执行附件存储后触发该方法
        /// TODO:
        /// </summary>
        public override void OnAttachmentUploaded()
        {
            base.OnAttachmentUploaded();
        }


        public override string ProcessName
        {
            get { return @"KStarWorkflow\TravelRequest"; }
        }
    }
}
