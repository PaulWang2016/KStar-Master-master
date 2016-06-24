using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace aZaaS.KStar.Wcf
{
    public class EmailSender
    {
        /// <summary>
        /// Sends email message
        /// </summary>
        /// <param name="smtpServer">SMTP Server</param>
        /// <param name="port">SMTP Server Port</param>
        /// <param name="from">Sender</param>
        /// <param name="to">Recipients</param>
        /// <param name="cc">CC</param>
        /// <param name="subject">Email title</param>
        /// <param name="body">Email body</param>
        /// <param name="attachments">Email attachments</param>
        public static void Send(string smtpServer, int port, string from, List<string> to, List<string> cc, string subject, string body, params Attachment[] attachments)
        {
            SmtpClient client = new SmtpClient();
            client.Host = smtpServer;
            client.Port = port;
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            List<string> tmpTo = new List<string>();
            foreach (string item in to)
            {
                string tmp = item.ToLower();
                if (!tmpTo.Contains(tmp))
                    tmpTo.Add(tmp);
            }

            foreach (string s in tmpTo)
            {
                mailMessage.To.Add(new MailAddress(s));
            }

            if (cc != null)
            {
                foreach (string ccItem in cc)
                {
                    mailMessage.CC.Add(new MailAddress(ccItem));
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            if (attachments != null)
            {
                mailMessage.Attachments.Clear();
                foreach (Attachment item in attachments)
                {
                    mailMessage.Attachments.Add(item);
                }
            }

            client.Send(mailMessage);
            client = null;

            if (mailMessage != null)
            {
                mailMessage.Dispose();
            }
            mailMessage = null;
        }

    }
}