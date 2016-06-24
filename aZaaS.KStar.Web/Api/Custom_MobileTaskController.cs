using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.KstarMobile;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.UserManagement;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.WorkflowData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
 

namespace aZaaS.KStar.Web.Api
{
    public class Custom_MobileTaskController : ApiController
    {
        [HttpPost]
        [ActionName("OnWorkflowTaskExecuting")]
        public HttpResponseMessage OnWorkflowTaskExecuting([FromBody]MobileTaskDataField taskDataField)
        {
            PostResult postResult = new PostResult();
            try
            { 
                WorkflowTaskExecutEvent(taskDataField, "OnWorkflowTaskExecuting");
                postResult.succeed = true;
            }
            catch (Exception ex)
            {
                postResult.succeed = false;
                postResult.message = ex.Message;
            }

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            responseMessage.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postResult), Encoding.UTF8);
            return responseMessage;
        }


        [HttpPost]
        [ActionName("OnWorkflowTaskExecuted")]
        public HttpResponseMessage OnWorkflowTaskExecuted([FromBody]MobileTaskDataField taskDataField)
        {

            PostResult postResult = new PostResult();
            try
            {
                WorkflowTaskExecutEvent(taskDataField, "OnWorkflowTaskExecuted");
                postResult.succeed = true;
            }
            catch (Exception ex)
            {
                postResult.succeed = false;
                postResult.message = ex.Message;
            }

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            responseMessage.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postResult), Encoding.UTF8);
            return responseMessage;
        }
         
        [NonAction]
        private void WorkflowTaskExecutEvent(MobileTaskDataField taskDataField,string eventString)
        {
            //接口有做错误处理，此处不需要在try cath
            var authType = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AuthType"].ToString());
            WFClientFacade wfClientFacade = new WFClientFacade((AuthenticationType)authType);
            ServiceContext context = new ServiceContext();
            context.UserName = taskDataField.UserName; 
            //获取ControllerUrl
            string ControllerFullName = "";
            using (KSTARServiceDBContext kSTARServiceDBContext = new KSTARServiceDBContext())
            {
                ProcessExtend processExtend = kSTARServiceDBContext.ProcessExtendSet.FirstOrDefault(x => x.ProcessFullName == taskDataField.FullName);
                if (processExtend != null)
                    ControllerFullName = processExtend.ControllerFullName;
            }
            //找到对应的Controller type
            Type modelType = System.Type.GetType(ControllerFullName, false);
            if (modelType != null)
            {
                MethodInfo methodInfo = modelType.GetMethod(eventString, new Type[] { typeof(WorkflowTaskContext) });

                int? formID = getFormID(taskDataField.ProcInstID);
                //构造对应执行参数
                KStarFormStorageProvider _storageProvider = new KStarFormStorageProvider();
                KStarFormModel kStarFormModel = _storageProvider.ReadForm(formID.Value, taskDataField.UserName);
                kStarFormModel.ActivityName = taskDataField.ActivityName;
                kStarFormModel.ActivityId = taskDataField.ProcInstID;
                kStarFormModel.ProcessFolio = taskDataField.Folio;

                WorkflowTaskContext workflowTaskContext = new Form.Infrastructure.WorkflowTaskContext(kStarFormModel, taskDataField.UserName, taskDataField.SN);
                FormController controllerObject = (FormController)modelType.Assembly.CreateInstance(modelType.FullName);

                HttpContextWrapper _context = new HttpContextWrapper(HttpContext.Current);
                RouteData _RouteData = new RouteData();
                RequestContext _RequestContext = new RequestContext(_context, _RouteData);

                controllerObject.ControllerContext = new System.Web.Mvc.ControllerContext(_RequestContext, controllerObject);
                controllerObject.ControllerContext.HttpContext.Request.Cookies.Add(new HttpCookie("SN", taskDataField.SN));
                controllerObject.ControllerContext.HttpContext.Request.Cookies.Add(new HttpCookie("_FormId", formID.Value.ToString()));
                
                controllerObject.OtherPlatformUserName = taskDataField.UserName;
                controllerObject.OtherPlatformProcessFullName = taskDataField.FullName;
                methodInfo.Invoke(controllerObject, new WorkflowTaskContext[] { workflowTaskContext }); 
            }
        }
         
        [NonAction]
        private int? getFormID(int? procInstID)
        {
            using (aZaaSKStarFormContext dbContext = new aZaaSKStarFormContext())
            {
                var linq = from h in dbContext.ProcessFormHeaders
                           join c in dbContext.ProcessFormContents on h.FormID equals c.FormID
                           into XmlData
                           from pro in XmlData.DefaultIfEmpty()
                           where h.ProcInstID == procInstID
                           select pro.FormID;
                int? formID = linq.FirstOrDefault();
                return formID;
            }
        } 
    }
     
    public class MobileTaskDataField
    {
        public string UserName { set; get; }
        public string SN { set; get; }

        public string Folio { set; get; }

        public int ProcInstID { set; get; }

        public string FullName { set; get; }

        public string ActivityName { set; get; }
    }

    public class PostResult
    {
        public bool succeed { set; get; }

        public string message { set; get; }

    }
}