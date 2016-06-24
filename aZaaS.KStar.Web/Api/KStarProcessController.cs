using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Mvc;
using aZaaS.KStar.Form;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Form.Infrastructure;
//using aZaaS.KStar.Web.Models.BasisEntity;
using Newtonsoft.Json;
using aZaaS.KStar.Web.Areas.Maintenance.Controllers;
using aZaaS.KStar.Workflow.Configuration;
using CsQuery;
using aZaaS.KStar.Web.Helper;

namespace aZaaS.KStar.Web.Api
{
    public class WorkflowParameter
    {
        public string Applicant { get; set; }
        public string Folio { get; set; }
        public string workFullName { get; set; }
        public string JsonData { get; set; }
        public int ProcInstID { get; set; }
        public string Approvers { get; set; }
        public Dictionary<string, object> datafields { get; set; }
    }
    public class KStarProcessController : ApiController
    {
         //private IStorageProvider _storageProvider=new KStarFormStorageProvider();
         //private IStorageProvider _storageProvider = new KStarFormStorageService(_storageProvider, this);

        [HttpGet]
        public string GetString()
        {
            return "string";
        }


        //
        // GET: /KStarProcess/
        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="Applicant">申请人</param>
        /// <param name="Folio">流程主题</param>
        /// <param name="model">表单数据(json格式)</param>
        /// <param name="datafields">流程数据（datafields）</param>
        /// <returns>流程实例</returns>
        [HttpPost]
        public  int StartProcess([FromBody]WorkflowParameter model)
        {
            try
            {
                UserService userService = new UserService();
                var k2User = userService.ReadUserInfo(model.Applicant);
                ServiceContext context = new ServiceContext();
                context.UserName = k2User.UserName;
                ProcessFormHeader header = InitFormHeader(model.Folio, model.workFullName, k2User);

                ProcessFormContent formContent = new ProcessFormContent()
                {
                    JsonData = model.JsonData
                };
                var formid = SaveForm(header, formContent);

                if (null == model.datafields)
                {
                    model.datafields = new Dictionary<string, object>();
                }
                if (!model.datafields.ContainsKey("FormId"))
                {
                    model.datafields.Add("FormId", formid);
                }
                var position = string.Empty;
                if (k2User.Positions.Count > 0)
                {
                    position = k2User.Positions[0].Name;
                }
                else
                {
                    position = "none";
                }
                model.datafields.Add("Position", k2User.Positions[0].Name);

                WFClientFacade wfClientFacade = new WFClientFacade(AuthenticationType.Form);


                header.ProcInstID = wfClientFacade.StartProcessInstance(context, model.workFullName, model.Folio, model.datafields);

                //AddProcessLog(header.ProcInstID,"外系统调用发起流程", "初始化");
                UpdateProcHeader(header);

                return formid;
            }
            catch (Exception ex)
            {
                
                return -1;
            }
        }
        /// <summary>
        /// 退回到申请人重填环节
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public string Rework([FromBody]WorkflowParameter model)
        {
            try
            {
                string activityName = ConfigurationManager.AppSettings["ReworkActivityName"].ToString();

                ProcessFormContent formContent = new ProcessFormContent()
                {
                    JsonData = model.JsonData

                };
                var result = GetProcessInstanceInfo(model.ProcInstID);
                var CurrentUserName = result.Destination;
                
                var _workflowService = new KStarFormWorkflowService(AuthenticationType.Form);

                _workflowService.GotoActivity(CurrentUserName, model.ProcInstID, activityName);

                AddProcessLog(model.ProcInstID, "重新提交流程", "重新提交流程");
                return "Success";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //return 0;
        }
        [HttpPost]
        public string CancelProcess([FromBody]WorkflowParameter model)
        {
            try
            {
               
                var _workflowService = new KStarFormWorkflowService(AuthenticationType.Form);
                _workflowService.CancelActivity(model.ProcInstID);
                AddProcessLog(model.ProcInstID, "作废", "废弃流程");
                SaveCancelRecord(model.ProcInstID);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }  
        }
        [HttpPost]
        public string DeleteProcess([FromBody]WorkflowParameter model)
        {
            try
            {
                var result = GetProcessInstanceInfo(model.ProcInstID); 
                ServiceContext context = new ServiceContext();
                context.UserName = result.Destination==null?result.Originator.Replace("K2:",""):result.Destination.Replace("K2:","");
                WFManagementFacade wfFacade = new WFManagementFacade(AuthenticationType.Form);
                wfFacade.DeleteProcessInstance(context, model.ProcInstID, true);
                DeleteKStarInstanceData(model.ProcInstID);
                //_workflowService.(ProcInstID);
                return "Success";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        public List<ProcessLogModel> GetProcessLogInfo([FromBody]WorkflowParameter model)
        {
            
            try 
            {
                var _workflowService = new KStarFormWorkflowService(AuthenticationType.Form);
                var result = _workflowService.GetProcessLog(model.ProcInstID);
                return result;
                //return "Success"+JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return null;
            }
 
        }
        [HttpPost]
        public string GetFormControlList([FromBody]WorkflowParameter model)
        {
            string html = string.Empty;
            ConfigManager _config = new ConfigManager(AuthenticationType.Form);
            var FormUrl = GetProcessUrl(model.workFullName);
            if (!string.IsNullOrEmpty(FormUrl))
            {
                var _temp_url = string.Format("{0}?IsControlSetting=true&ControlSettingMode=3", FormUrl);
                CQ _doc = HttpHelper.GetTemplateContent(_temp_url,AuthenticationType.Form);
                IList<IDomObject> _kstar_componentlist = _doc[".kstar-component"].ToList();
                foreach (var component in _kstar_componentlist)
                {
                    var compentID = component.GetAttribute("role");
                    if (compentID == "_kstarform_content")
                    {
                        CQ _component_temp = CQ.CreateDocument(component.InnerHTML);
                        html = component.OuterHTML;
                        //IList<IDomObject> _kstar_controllist = _component_temp[".form-control"].ToList();
                        //foreach (var control in _kstar_controllist)
                        //{
                        //    var id = control.Id;
                        //    var name = control.Name;
                        //    var title = control.GetAttribute("title");
 
                        //}
                    }
                }
            }
            return html;
        }
        private  ProcessFormHeader InitFormHeader(string folio, string workFlowFullName, UserDto k2User)
        {
            ProcessFormHeader header = new ProcessFormHeader();
            header.Priority = 0;
            header.FormID = 0;
            header.IsDraft = false;
            header.ProcessFolio = folio;
            header.FormSubject = workFlowFullName;
            header.SubmitterAccount = header.ApplicantAccount = k2User.UserName;
            header.SubmitterDisplayName = header.ApplicantDisplayName = k2User.FirstName + k2User.LastName;
            header.ApplicantEmail = k2User.Email;
            header.SubmitDate = DateTime.Now;
            return header;
        }
        private void UpdateForm(ProcessFormContent content,int ProcInstID)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var header = db.ProcessFormHeaders.FirstOrDefault(p => p.ProcInstID == ProcInstID);
                var FormID = header.FormID;
                var result = db.ProcessFormContents.FirstOrDefault(p => p.FormID == FormID);
                if (result != null)
                {
                    result.JsonData = content.JsonData;
                    result.XmlData = JsonHelper.JsonToXml(content.JsonData);
                    db.Entry<ProcessFormContent>(result).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private  int SaveForm(ProcessFormHeader header, ProcessFormContent content)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                db.ProcessFormHeaders.Add(header);
                db.SaveChanges();
                content.FormID = header.FormID;
                content.XmlData = JsonHelper.JsonToXml(content.JsonData);
                db.ProcessFormContents.Add(content);
                db.SaveChanges();
                return header.FormID;
            }
        }


        public  void UpdateProcHeader(ProcessFormHeader header)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var sourceHeader = db.ProcessFormHeaders.FirstOrDefault(n => n.FormID == header.FormID);
                sourceHeader.ProcInstID = header.ProcInstID;
                sourceHeader.ProcessFolio = header.ProcessFolio;
                db.Entry<ProcessFormHeader>(sourceHeader).CurrentValues.SetValues(header);
                db.SaveChanges();
                
            }
        }
        private string GetProcessName(string fullName)
        {
            using (KStarDbContext db = new KStarDbContext())
            {
                var result = db.Configuration_ProcessSetSet.FirstOrDefault(p => p.ProcessFullName == fullName);
                if (result != null)
                {
                    return result.ProcessName;
                }
            }
            return "";

        }
        private aZaaS.KStar.Web.Models.BasisEntity.view_ProcinstList GetProcessInstanceInfo(int ProcInstID)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer entity = new aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer())
            {
                var result = entity.view_ProcinstList.FirstOrDefault(p => p.ProcInstID == ProcInstID);
                return result;
            }
        }
        private void AddProcessLog(int? ProcInstID,string ActionName,string Comment)
        {
            var log = new aZaaS.KStar.Web.Models.BasisEntity.ProcessLog();
            log.ActionName = ActionName;
            UserService userService = new UserService();
           // var k2User = userService.ReadUserInfo(model.Applicant);
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer entity = new aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer())
            {
                var result = entity.view_ProcinstList.FirstOrDefault(p => p.ProcInstID == ProcInstID);
                if (result != null)
                {
                    log.ActivityName = result.ActName;
                    log.Comment = Comment;
                    log.CommentDate = DateTime.Now;
                    log.ProcInstID = ProcInstID;
                    log.OrigUserAccount = result.Originator;
                    log.OrigUserName = result.StartName;
                    log.SN = result.SN;
                    log.UserAccount = result.Destination.Replace("K2:", "");
                    log.UserName = userService.ReadUserInfo(log.UserAccount).FirstName;
                    log.ProcessName = GetProcessName(result.FullName);
                    
                }
                entity.ProcessLog.Add(log);
                entity.SaveChanges();
                
            }  
        }
        private void SaveCancelRecord(int ProcIntID)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer entity = new aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer())
            {
                aZaaS.KStar.Web.Models.BasisEntity.ProcessFormCancel pfc = new Models.BasisEntity.ProcessFormCancel();
                pfc.ProcInstId = ProcIntID;
                entity.ProcessFormCancel.Add(pfc);
                entity.SaveChanges();
            }
        }
        private void DeleteKStarInstanceData(int ProcInstID)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer entity = new aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer())
            {
                var result = entity.ProcessFormCancel.FirstOrDefault(p => p.ProcInstId == ProcInstID);
                if (result != null)
                {
                    entity.ProcessFormCancel.Remove(result);
                }
                var header = entity.ProcessFormHeader.FirstOrDefault(p => p.ProcInstID == ProcInstID);
                var FormID = header.FormID;
                var formContent = entity.ProcessFormContents.FirstOrDefault(p => p.FormID == FormID);
                var processLog = entity.ProcessLog.Where(p => p.ProcInstID == ProcInstID).ToList();
                var ProcessCC = entity.ProcessFormCC.Where(p => p.FormId == FormID).ToList();
                if (ProcessCC != null)
                {
                    entity.ProcessFormCC.RemoveRange(ProcessCC);
                }
                entity.ProcessFormHeader.Remove(header);
                entity.ProcessFormContents.Remove(formContent);
                if (processLog != null)
                {
                    entity.ProcessLog.RemoveRange(processLog);
                }
                entity.SaveChanges();
            }
        }
        private string GetProcessUrl(string fullName)
        {
            using (KStarDbContext db = new KStarDbContext())
            {
                var result = db.Configuration_ProcessSetSet.FirstOrDefault(p => p.ProcessFullName == fullName);
                if (result != null)
                {
                    return result.StartUrl;
                }
            }
            return "";
        }

    }
}
