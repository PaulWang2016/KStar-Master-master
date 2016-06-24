using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using aZaaS.KStar;
using aZaaS.Framework.Template;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TemplateService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TemplateService.svc or TemplateService.svc.cs at the Solution Explorer and start debugging.
    [ServiceErrorHandlerBehaviour]
    public class TemplateService : ITemplateService
    {
        private readonly KStar.TemplateService _templateService;
        private static readonly object syncLock = new object();

        public TemplateService()
        {
            _templateService = new KStar.TemplateService(AuthConfig.AuthType);
        }

        public EmailData ParseGeneralEmailTemplate(int procInstId, string templateName)
        {
            lock (syncLock)
            {
                if (procInstId <= 0)
                    throw new ArgumentOutOfRangeException("procInstId");
                if (string.IsNullOrEmpty(templateName))
                    throw new ArgumentNullException("templateName");

                var template = _templateService.ReadEmailTemplate(templateName);
                if (template == null)
                    throw new InvalidOperationException(string.Format("The template: {0} was not found!", templateName));

                var subject = string.Empty;
                var content = _templateService.Generate(procInstId, string.Empty, template, out subject);

                return new EmailData() { Subject = subject, Content = content };
            }
        }

        public EmailData ParseParticipantEmailTemplate(string serialNumber, string participant, string templateName)
        {
            lock (syncLock)
            {
                //if (string.IsNullOrEmpty(serialNumber))
                //    throw new ArgumentOutOfRangeException("serialNumber");
                //if (string.IsNullOrEmpty(participant))
                //    throw new ArgumentNullException("participant");
                //if (string.IsNullOrEmpty(templateName))
                //    throw new ArgumentNullException("templateName");

                //var template = _templateService.ReadEmailTemplate(templateName);
                //if (template == null)
                //    throw new InvalidOperationException(string.Format("The template: {0} was not found!", templateName));

                //var subject = string.Empty;
                //var content = _templateService.Transform(0, serialNumber, participant, template, out subject);

                throw new NotSupportedException("ParseParticipantEmailTemplate");
            }
        }


        public EmailData TransformProcessTemplate(int procInstID, string activityName, string templateName)
        {
            lock (syncLock)
            {
                if (procInstID == 0)
                    throw new InvalidOperationException("procInstID");
                if (string.IsNullOrEmpty(activityName))
                    throw new ArgumentNullException("activityName");
                if (string.IsNullOrEmpty(templateName))
                    throw new ArgumentNullException("templateName");


                var template = _templateService.ReadEmailTemplate(templateName);
                if (template == null)
                    throw new InvalidOperationException(string.Format("The template: {0} was not found!", templateName));

                var subject = string.Empty;
                var content = _templateService.Transform(procInstID, activityName, template, out subject);

                return new EmailData() { Subject = subject, Content = content };
            }

        }

        public EmailData TransformWorklistTemplate(string serialNumber, string participant, string activityName, string templateName)
        {
            lock (syncLock)
            {
                if (string.IsNullOrEmpty(serialNumber))
                    throw new ArgumentNullException("serialNumber");
                if (string.IsNullOrEmpty(participant))
                    throw new ArgumentNullException("participant");
                if (string.IsNullOrEmpty(activityName))
                    throw new ArgumentNullException("activityName");
                //if (string.IsNullOrEmpty(itemUrl))
                //    throw new ArgumentNullException("ItemUrl");
                if (string.IsNullOrEmpty(templateName))
                    throw new ArgumentNullException("templateName");


                var template = _templateService.ReadEmailTemplate(templateName);
                if (template == null)
                    throw new InvalidOperationException(string.Format("The template: {0} was not found!", templateName));

                var subject = string.Empty;
                var content = _templateService.Transform(serialNumber, activityName, string.Empty, participant, template, out subject);

                return new EmailData() { Subject = subject, Content = content };
            }
        }
    }
}
