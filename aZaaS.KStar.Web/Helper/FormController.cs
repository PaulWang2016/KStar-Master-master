using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.ViewResults;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Models;

using aZaaS.Framework.Workflow;
using System.Configuration;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.Kstar.DAL;
using System.Data;
using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Form.Mvc.Controllers;
using aZaaS.KStar.Form.Mvc;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using aZaaS.KStar.Form;
using aZaaS.KStar.WorkflowConfiguration;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public abstract class FormController :KStarFormController
    {

        public override void OnWorkflowReworkActivity(WorkflowTaskContext context)
        {

            var ActivityList = new List<Activity>();
            ///获取配置退回的流程环节集合
            var activitys = context.ReworkActivitys;
            //获取当前实例的流程审批记录
            var list = WorkflowService.GetProcessLog(context.ProcInstId);

            if (list == null) { return; }
            var i=0;
            while (i < activitys.Count)
            {
                //遍历流程环节比对流程实例审批日志是否存在，如果不存则移除。
                //移除一个环节信息，相应的也减少遍历数量。
                if (!IsRouteActivity(activitys[i].Name, list))
                {
                    context.ReworkActivitys.Remove(activitys[i]);
                    i = i - 1;
                }
                i++;
            }
            //for (int i = 0; i < activitys.Count; i++)
            //{

            //    if (!IsRouteActivity(activitys[i].Name, list))
            //    {
            //        context.ReworkActivitys.Remove(activitys[i]);
            //        i = 0;
            //    }
            //}
        }
        private bool IsRouteActivity(string Name, List<ProcessLogModel> list)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                if(Name.IndexOf("015")>-1)
                {
                    return true;
                }
            }
            bool isRoute = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ActivityName.Trim() == Name.Trim())
                {
                    isRoute = true;
                    break;
                }
            }
            return isRoute;
        }
        public override void BindKStarFormMessage()
        {
            KStarFormMessage.SaveSuccessMsg = PromptSuccessMsg;

            KStarFormMessage.SubmitSuccessMsg = SumbitSuccessMsg;
            KStarFormMessage.GotoActivitySuccessMsg = ReworkSuccessMsg;
            KStarFormMessage.DeleteSuccessMsg = DeleteProcessMsg;
            KStarFormMessage.ReviewSuccessMsg = ReviewSuccessMsg;
            KStarFormMessage.UndoSuccessMsg = UndoSuccessMsg;
            base.BindKStarFormMessage();
        }
        public ResultMessage PromptSuccessMsg(StorageContext storageContext)
        {
            string msg = string.Format(@"
                                                保存成功! <br/> 
                                                流程主题： {0} <br/>
                                                ", storageContext.FormModel.FormSubject);
            return new ResultMessage(MessageType.Warning, msg);
        }
        public ResultMessage SumbitSuccessMsg(WorkflowTaskContext taskContext,StorageContext storageContext)
        {
            
            string msg = string.Format(@"
                                                提交成功! <br/> 
                                                流程主题：{0} <br/>
                                                流程实例编号：  {1}", storageContext.FormModel.FormSubject,  taskContext.Folio.Split('-').Last());
            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage ReworkSuccessMsg( WorkflowTaskContext taskContext,StorageContext storageContext,string ActivityName)
        {
            string msg = string.Format(@"
                                                回退成功! <br/> "
                                                );
            return new ResultMessage(MessageType.Information, msg);
        }
        public ResultMessage ReviewSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format(@"
                                                审阅成功! <br/> 
                                                流程主题：{0} <br/>
                                                流程实例编号：  {1}", taskContext.FormModel.FormSubject, taskContext.Folio.Split('-').Last());
            return new ResultMessage(MessageType.Information, msg);
        }
        public ResultMessage UndoSuccessMsg(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            string msg = string.Format(@"
                                                撤回成功! <br/> 
                                                流程主题：{0} <br/>
                                                流程实例编号：  {1}", taskContext.FormModel.FormSubject, taskContext.Folio.Split('-').Last());
            return new ResultMessage(MessageType.Information, msg);
        }

        public ResultMessage DeleteProcessMsg( WorkflowTaskContext taskContext,StorageContext storageContext)
        {
            string msg = string.Format(@"
                                                作废成功! <br/> "
                                               );
            return new ResultMessage(MessageType.Information, msg);
        }
        public void WorkflowTocc(string usernames,int formId=0)
        {
            int InstID = ProcInstId;
            ProcessFormHeader header = null;
            if (formId > 0)
            {
                header=StorageProvider.GetProcessFormHeaderByFormId(formId);
            }
            InstID = header == null ? this.ProcInstId : header.ProcInstID.Value;
            var ds = CustomExtUtility.GetProcessReminderInfo(InstID.ToString());
            string url = string.Empty;
            if (string.IsNullOrEmpty(usernames))
            {
                return;
            }
            string[] users = usernames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
           
            HashSet<string> userList = new HashSet<string>();
            foreach (string user in users)
            {
                userList.Add(user);
                if (ds.Tables.Count > 1)
                {
                    url = ds.Tables[1].Rows[0][0].ToString();
                }
                try
                {
                    var model = OrganizatoinService.GetUserInfo(user);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            //var row = ds.Tables[0].Rows[0];
                            string title = GenerateEmailTitle(row);
                            int RowActID = 0;
                            int.TryParse(row["ActID"].ToString(), out RowActID);

                            if (this.ActivityID == RowActID)
                            {
                                string body = GenerateEmailFormat(row, url, model.ApplicantDisplayName, user);



                                CustomExtUtility.InsertReminderRecord(title, body, model.ApplicantEmail);
                            }
                        }
                    }
                    else
                    {
                        var userName = header.SubmitterAccount.Replace("K2:", "").Replace("neowaydc", "").Replace("NEOWAYDC", "").Replace(@"\", ""); ;
                        var displayName = header.SubmitterDisplayName;
                        var processName = header.ProcessFolio.Split('(').First();
                        var FlowNo = header.ProcessFolio.Split('-').Last();
                        var recieveName = OrganizatoinService.GetDisplayName(user);
                        var email = OrganizatoinService.GetUserInfo(user);
                        var viewUrl = string.Format("{0}?_FormId={1}&ActivityId={2}&FromMail=1&authorname={3}", Request.UrlReferrer.AbsoluteUri.Split('?').First(), formId, this.ActivityID, user);
                        string Title = GenerateEmailTitle(userName, displayName, processName);
                        string body = GenerateEmailFormat(userName, displayName, processName, header.SubmitDate.ToString(), viewUrl, recieveName,FlowNo);

                        CustomExtUtility.InsertReminderRecord(Title, body, email.ApplicantEmail);

                    }

                }
                catch (Exception ex)
                {
                    
                    LogFactory.GetLogger().Write(new LogEvent
                    {
                        Source = "结束邮件抄送主体",
                        Message = ex.Message,
                        Exception = ex,
                        OccurTime = DateTime.Now
                    });
                    //NeowayExtUtility.a
                }
            }
            if (formId > 0)
            {
                FormCarbonCopy(userList.ToList(), formId);
            }
            else
            {
                FormCarbonCopy(userList.ToList());
            } 
            //this.ProcInstId
        }
        public string GenerateEmailTitle(string startUserName, string userDisplayName, string ProcessName)
        {
          
            string Title = "审阅：流程中心 - {0}({1})发起的【{2}】需要您审阅";

            Title = string.Format(Title, userDisplayName, startUserName, ProcessName);
            //if (!string.IsNullOrEmpty(row["subject"].ToString()))
            //{
            //    Title = "" + row["subject"].ToString();
            //}
            return Title;
        }
        public string GenerateEmailTitle(DataRow row)
        {
            string Subject = row["subject"].ToString();
            string Title = "审阅：流程中心 - {0}({1})发起的【{2}】需要您审阅";
            string startUser = row["Originator"].ToString().Replace("K2:", "").Replace("neowaydc", "").Replace("NEOWAYDC","").Replace(@"\", "");
            Title = string.Format(Title, row["StartName"].ToString(), startUser, row["ProcessName"].ToString());
            if (!string.IsNullOrEmpty(row["subject"].ToString()))
            {
                Title = row["subject"].ToString().Replace("待办：", "审阅：").Replace("审批", "审阅");
            }
            return Title;
        }
        public string GenerateEmailFormat(string startUserName, string userDisplayName, string ProcessName, string startTime, string transferUrl, string RecieverName,string Folio)
        {

            StringBuilder strContext = new StringBuilder("<span style='font-family:Arial sans-serif ;color:#1F497D;'>Dear,{0}<br/><br/>您在流程中心中有一个<span style='color:red'>审阅</span>事项。 ");
            strContext.Append(" <a href='{6}'>请点击这里打开处理</a> <br/>");
            strContext.Append("以下是该事项的简要内容：</span><br/>");

            strContext.Append("<table style='border:none;border-collapse:collapse' cellspacing='0' cellpadding='0'><tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>");
            strContext.Append("流程名称:</td><td>&nbsp;</td><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{1}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>流程实例号:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{2}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>抄送环节名称:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{3}</td></tr>");

            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请人:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{4}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请时间:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{5}</td></tr>");
            strContext.Append("</table>");
            strContext.Append("<br/><span style='font-family:Arial sans-serif ;color:#1F497D;'>温馨提示：此邮件由系统自动发出，请不要答复该邮件。若对流程内容有疑问，请与发起人联系，若有技术问题，请与流程程架构部（peng.fuze@innos.com）联系。 </span>");
            string body = string.Format(strContext.ToString(), RecieverName, ProcessName, Folio, "010_发起申请", userDisplayName
                , Convert.ToDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"), transferUrl);
            return body;
        }
        public string GenerateEmailFormat(DataRow row,string transferUrl,string RecieverName,string toRecieverUsername)
        { 
            StringBuilder strContext = new StringBuilder("<span style='font-family:Arial sans-serif ;color:#1F497D;'>Dear,{0}<br/><br/>您在流程中心中有一个<span style='color:red'>审阅</span>事项。 ");
            strContext.Append(" <a href='{6}?strUrl={7}?_FormId={8}&ActivityId={9}&FromMail=1&authorname={10}'>请点击这里打开处理</a> <br/>");
            strContext.Append("以下是该事项的简要内容：</span><br/>");

            strContext.Append("<table style='border:none;border-collapse:collapse' cellspacing='0' cellpadding='0'><tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>");
            strContext.Append("流程名称:</td><td>&nbsp;</td><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{1}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>流程实例号:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{2}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>抄送环节名称:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{3}</td></tr>");

            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请人:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{4}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请时间:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{5}</td></tr>");
            strContext.Append("</table>");
            strContext.Append("<br/><span style='font-family:Arial sans-serif ;color:#1F497D;'>温馨提示：此邮件由系统自动发出，请不要答复该邮件。若对流程内容有疑问，请与发起人联系，若有技术问题，请与流程程架构部（peng.fuze@innos.com）联系。 </span>");
            string body = string.Format(strContext.ToString(), RecieverName, row["ProcessName"].ToString(), row["Folio"].ToString().Split('-').Last(), row["ActName"].ToString(), row["StartName"].ToString()
                , Convert.ToDateTime(row["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), transferUrl, row["ApproveUrl"].ToString(), row["FormID"].ToString(), this.ActivityID, toRecieverUsername);
            return body;      
        }

        public override void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        {
            //生成Folio
            string flowSerial = CustomExtUtility.InsertFlowSerialNo(context.FormId);
            //查询自定义主题
            string ruleString = GetFormSubject(context.ProcessName, context.FormModel.ContentData);
            if (!string.IsNullOrWhiteSpace(ruleString))
            {
                context.Folio = ruleString + "-" + flowSerial; 
            }
            else
            {
                //默认自定义主题
                context.Folio = context.FormModel.FormSubject + "-" + flowSerial;
            } 
        }
        public override void OnWorkflowTaskExecuting(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskExecuting(context);
            //当自定义之后进行更新
            string ruleString = GetFormSubject(context.ProcessName, context.FormModel.ContentData);
            if (!string.IsNullOrWhiteSpace(ruleString) && context.Folio.LastIndexOf("-")>0)
            { 
                string folio = context.Folio.Substring(context.Folio.LastIndexOf("-"));
                context.Folio = ruleString + folio;
                KStarFormWorkflowService service = new KStarFormWorkflowService(AuthenticationType.Form);
                service.UpdateProcessFolio(context.ProcInstId, context.Folio);
         }
        }

        public static string GetFormSubject(string ProcessName, string ContentData)
        {
            FlowThemeRepository flowThemeRepository = new FlowThemeRepository();
            ProcessFormFlowTheme entity = flowThemeRepository.Get(ProcessName);
            if (entity != null)
            {
                Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(ContentData);
                var matchs = Regex.Matches(entity.RuleString, @"({[\w\d|\.]*}+)");
                string ruleString = entity.RuleString;
                bool isReplace = false;
                foreach (Match item in matchs)
                {
                    string keys = item.Value;
                    keys = keys.Replace("{", "").Replace("}", "");

                    string[] keyArray = keys.Split('.');
                    string value = string.Empty;
                    if (keyArray.Length >= 1)
                    {
                        foreach (string attr in keyArray)
                        {
                            if (string.Empty == value)
                            {
                                value = JTokenFormatString(jObject[attr]);
                            }
                            else
                            {
                                value = JTokenFormatString(JToken.Parse(value)[attr]);
                            }

                            if (value == string.Empty)
                                break;
                            isReplace = true;
                        }
                    }
                        ruleString = ruleString.Replace(item.Value, value);
                }
                if (isReplace)
                { 
                    return ruleString;
                }
            }
            return string.Empty;
        }
         
        public static string JTokenFormatString(JToken data)
        {
            if (data == null) return string.Empty;
            if (data.Type == JTokenType.Date)
            {
                return data.Value<DateTime>().ToString("yyyyMMdd");
            }
            else if (data.Type == JTokenType.Integer)
            {
                if (data.Value<int>() == 0)
                {
                    return string.Empty;
                }
            }
            return data.ToString();
        }
         
        public override void OnWorkflowNewTaskStarted(WorkflowTaskContext context)
        {
            base.OnWorkflowNewTaskStarted(context);
            SavePrognosisData(context.ProcInstId);
        }

        public override void OnWorkflowTaskExecuted(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskExecuted(context);
            SavePrognosisData(context.ProcInstId);
        }
        #region Prognosis
        //保存预判数据
        public void SavePrognosisData(int procInstID)
        { 
            try
            {
                //判断是否开启预判
                using (KStarDbContext kstarDB = new KStarDbContext())
                {
                    var processSet = kstarDB.Configuration_ProcessSetSet.Where(x => x.ProcessPredict == true).FirstOrDefault();
                    if (processSet.ProcessPredict!=true)
                    {
                        return;
                    }
                }
                using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
                {
                    
                    dbContext.ProcessPrognosisTask.Add(new ProcessForm.ProcessPrognosisTask() { ProcInstID = procInstID, SysID = Guid.NewGuid() });
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
