using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Template;
using aZaaS.Framework.Expressions;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar
{
    public class TemplateService
    {
        private readonly TemplateFacade templateFacade;
        private readonly AuthenticationType _authType;

        public TemplateService(AuthenticationType authType)
        {
            this._authType = authType;
            this.templateFacade = new TemplateFacade(); 
        }



        public Guid CreateEmailTemplate(EmailTemplate template)
        {

            return this.templateFacade.CreateEmailTemplate(new ServiceContext(), template);
        }


        public EmailTemplate ReadEmailTemplate(Guid templateId)
        {

            return this.templateFacade.ReadEmailTemplate(new ServiceContext(), templateId);
        }


        public EmailTemplate ReadEmailTemplate(string templateName)
        {

            return this.templateFacade.ReadEmailTemplate(new ServiceContext(), templateName);
        }


        public void UpdateEmailTemplate(EmailTemplate template)
        {

            this.templateFacade.UpdateEmailTemplate(new ServiceContext(), template);
        }


        public void DeleteEmailTemplate(EmailTemplate template)
        {

            this.templateFacade.DeleteEmailTemplate(new ServiceContext(), template);
        }


        public void DeleteEmailTemplate(Guid templateId)
        {

            this.templateFacade.DeleteEmailTemplate(new ServiceContext(), templateId);
        }


        public bool IsExist(string templateName)
        {

            return this.templateFacade.IsExist(new ServiceContext(), templateName);
        }


        public IEnumerable<EmailTemplate> GetEmailTemplates(QueryExpression expression, int total, int pageSize, int pageIndex)
        {

            return this.templateFacade.GetEmailTemplates(new ServiceContext(), expression, total, pageSize, pageIndex);
        }


        public string Generate(int procInstID,string sn, string templateName, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = procInstID.ToString();
            context["SerialNumber"] = sn;
            context["ActivityName"] = string.Empty;
            context["ItemUrl"] = string.Empty;
            context["Destination"] = string.Empty;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, templateName, out subject);
        }


        public string Generate(int procInstID,string sn, Guid templateId, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = procInstID.ToString();
            context["SerialNumber"] = sn;
            context["ActivityName"] = string.Empty;
            context["ItemUrl"] = string.Empty;
            context["Destination"] = string.Empty;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, templateId, out subject);
        }


        public string Generate(int procInstID,string sn, EmailTemplate template, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = procInstID.ToString();
            context["SerialNumber"] = sn;
            context["ActivityName"] = string.Empty;
            context["ItemUrl"] = string.Empty;
            context["Destination"] = string.Empty;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, template, out subject);
        }

        public string Generate(int procInstID, string sn,string destination, EmailTemplate template, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = procInstID.ToString();
            context["SerialNumber"] = sn;
            context["ActivityName"] = string.Empty;
            context["ItemUrl"] = string.Empty;
            context["Destination"] = destination;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, template, out subject);
        }


        public string Transform(int procInstID,string activityName,EmailTemplate template, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = procInstID.ToString();
            context["SerialNumber"] = string.Empty;
            context["ActivityName"] = activityName;
            context["ItemUrl"] = string.Empty;
            context["Destination"] = string.Empty;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, template, out subject);
        }


        public string Transform( string serialNumber,string activityName,string itemUrl, string destination, EmailTemplate template, out string subject)
        {
            ServiceContext context = new ServiceContext();
            context["ProcessInstanceID"] = string.Empty;
            context["SerialNumber"] = serialNumber;
            context["ActivityName"] = activityName;
            context["ItemUrl"] = itemUrl;
            context["Destination"] = destination;
            context["__AuthenticationType"] = this._authType.ToString();

            return this.templateFacade.Generate(context, template, out subject);
        }

        public IEnumerable<EmailTemplate> GetAllEmailTemplates()
        {
            return this.templateFacade.GetAllEmailTemplates(new ServiceContext());
        }
    }
}
