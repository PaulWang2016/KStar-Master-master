using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar;
using aZaaS.Framework.Expressions;
using aZaaS.Framework.Template;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class EmailTpmlController : BaseMvcController
    {

        //WorkflowManagementService process = new WorkflowManagementService(Framework.Workflow.AuthenticationType.Windows);
        TemplateService tempservice;
        public EmailTpmlController()
        {
            tempservice = new TemplateService(this.AuthType);
        }
        private const string CONTENT_VARIABLES = "Process;Fun;Environment;SmartObject;Worklist";

        #region 获取 邮件模版列表
        /// <summary>
        /// 获取 邮件模版列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmailtpmlList()
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var items = tempservice.GetAllEmailTemplates();
            List<EmailtpmlView> EmailtpmlList = new List<EmailtpmlView>();
            foreach (var item in items)
            {
                var test = tempservice.ReadEmailTemplate(item.Id);
                EmailtpmlView emailTpm = new EmailtpmlView();
                emailTpm.TpmlID = item.Id.ToString();                
                emailTpm.Process = item.Type;
                emailTpm.ProcessName = svc.GetProcessSetByFullName(this.CurrentUser, item.Type);
                emailTpm.SubjectInfo = item.Subject.Template;
                emailTpm.ContentInfo = item.Body.Template;
                emailTpm.TpmlName = item.Name;
                EmailtpmlList.Add(emailTpm);
            }
            return Json(EmailtpmlList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加 Email 模版
        /// <summary>
        /// 添加 Email 模版
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DoCreateEmailTpml(EmailtpmlView model)
        {
            ContentTemplate contentSubject = new ContentTemplate();
            ContentTemplate contentBody = new ContentTemplate();
            EmailTemplate item = new EmailTemplate();
            item.Id = Guid.NewGuid();
            item.Name = model.TpmlName;
            item.Type = model.Process;
            contentSubject.Id = Guid.NewGuid();
            contentBody.Id = Guid.NewGuid();
            contentSubject.Name = string.Format("{0}-{1}-Subject", model.Process, Guid.NewGuid());
            contentBody.Name = string.Format("{0}-{1}-Body", model.Process, Guid.NewGuid());
            contentBody.Template = model.ContentInfo;
            contentSubject.Template = model.SubjectInfo;
            contentSubject.Variable = CONTENT_VARIABLES;
            contentBody.Variable = CONTENT_VARIABLES;
            item.Subject = contentSubject;
            item.Body = contentBody;
            tempservice.CreateEmailTemplate(item);
            model.TpmlID = item.Id.ToString();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 更新 Email 模版
        /// <summary>
        /// 更新 Email 模版
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DoUpdateEmailTpml(EmailtpmlView model)
        {
            EmailTemplate item = tempservice.ReadEmailTemplate(Guid.Parse(model.TpmlID));
            item.Id = Guid.Parse(model.TpmlID);
            item.Name = model.TpmlName;
            item.Type = model.Process;
            item.Body.Template = model.ContentInfo;
            item.Subject.Template = model.SubjectInfo;

            tempservice.UpdateEmailTemplate(item);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除 Email 模版    --------批量
        /// <summary>
        /// 删除 Email 模版    --------批量
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDestroyEmailTpml(List<string> idList)
        {
            foreach (var id in idList)
            {
                tempservice.DeleteEmailTemplate(Guid.Parse(id));
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
