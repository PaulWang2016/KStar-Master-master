using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Web.Utilities.OuterNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Linq;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.WorkflowConfiguration;
using aZaaS.KStar.Form.Mvc.Controllers;
namespace aZaaS.KStar.Web.Api
{
    public class Custom_UtilitiesController : ApiController
    {
        private static ConfigManager _ConfigManager;
        // GET: /Custom_Utilities/
        [HttpGet]
        [ActionName("FileDownloadPermission")]
        public HttpResponseMessage GetCustomers()
        {

            bool isPermission = false;
            var address = ((HttpContextWrapper)this.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            //判断是否登陆
            if (!string.IsNullOrWhiteSpace(this.User.Identity.Name))
            { 
                bool isOuterNetwork = OuterNetworkUtilites.IsOuterNetwork(address);
                //是否是外网
                if (isOuterNetwork == true)
                {
                    bool isOuterNetworkRole = OuterNetworkUtilites.IsNetworkDownloadRole(this.User.Identity.Name);
                    //判断是否有外网下载权限
                    if (isOuterNetworkRole)
                    {
                        isPermission = true;
                    }
                    else
                    {
                        isPermission = false;
                    }
                }
                else
                {
                    //内网默认都有权限
                    isPermission = true;
                }
            }
            else
            {
                //没有登陆 没权限
                isPermission = false;
            }

            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("Result", isPermission);
            ht.Add("address", address);

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ht);

            return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
        }

        [HttpGet]
        [ActionName("GetAgents")]
        public HttpResponseMessage GetAgents(string userID, int size)
        {
            var authType = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AuthType"].ToString());

            ServiceContext context = new ServiceContext();
            List<System.Collections.Hashtable> listHt = new List<System.Collections.Hashtable>();

            if (userID.IndexOf(@"\") == -1)
            {
                userID = ConfigurationManager.AppSettings["WindowDomain"].ToString() + @"\" + userID;
            }
            context.UserName = userID; 
            try
            {
                WorkflowDataService tenantDatabaseService = new WorkflowDataService((AuthenticationType)authType);
                WorklistItemCriteria criteria = new WorklistItemCriteria() { PageIndex = 0, PageSize = size };
                criteria.UserName = userID;
                DateTime startDate = InitStartDate(null);
                DateTime endDate = InitEndDate(null);
                criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "ActivityStartDate", Compare = CriteriaCompare.GreaterOrEqual, Value1 = startDate });
                criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "ActivityStartDate", Compare = CriteriaCompare.LessOrEqual, Value1 = endDate });
                criteria.AddSortFilter(new SortFilter() { FieldName = "ActivityStartDate", SortDirection = "Descending" });
                IEnumerable<WorklistItem> wiList = null; //tenantDatabaseService.FindPendingTasks(criteria, (AuthenticationType)authType); 
                int count = criteria.TotalCount;
                foreach (WorklistItem item in wiList)
                {
                    System.Collections.Hashtable _ht = new System.Collections.Hashtable();
                    _ht.Add("AssignedDate", item.AssignedDate.ToString("yyyy-MM-dd HH:mm"));

                    var CurrentDate = DateTime.Now;
                    var TaskStartDate = Convert.ToDateTime(item.AssignedDate);
                    TimeSpan ts = CurrentDate - TaskStartDate;
                    string runingTime = "";
                    if (ts.Days >= 1)
                    {
                        runingTime += string.Format("{0}天", ts.Days);
                        runingTime += string.Format("{0}小时", ts.Hours < 10 ? "0" + ts.Hours.ToString() : ts.Hours.ToString());
                    }
                    else
                    {
                        if (ts.Hours >= 1)
                        {
                            runingTime += string.Format("{0}小时", ts.Hours < 10 ? "0" + ts.Hours.ToString() : ts.Hours.ToString());
                            runingTime += string.Format("{0}分钟", ts.Minutes < 10 ? "0" + ts.Minutes.ToString() : ts.Minutes.ToString());
                        }
                        else
                        {
                            if (ts.Minutes >= 1)
                            {
                                runingTime += string.Format("0小时{0}分钟", ts.Minutes < 10 ? "0" + ts.Minutes.ToString() : ts.Minutes.ToString());
                            }
                        }
                    }
                   
                    _ht.Add("RunTime", runingTime);
                    _ht.Add("SN", item.SN);
                    _ht.Add("FormId", getFormID(item.ProcInstID));
                    _ht.Add("Folio", item.Folio);
                    _ht.Add("ApproveUrl", GetApproveUrl(item.FullName.Trim()));
                    _ht.Add("StartDate", item.ProcStartDate.ToString("yyyy-MM-dd HH:mm"));
                    _ht.Add("PronInstID", item.ProcInstID);
                    _ht.Add("TaskCount", count);
                    listHt.Add(_ht);
                }
            }
            catch (Exception ex)
            {

            } 
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(listHt);
            var callback = Request.RequestUri.ParseQueryString()["callback"];

            if (callback == null)
            {
                return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
            }
            else
            {
                return new HttpResponseMessage() { Content = new StringContent(callback + "(" + jsonData + ")", Encoding.UTF8) };
            }
        }

        [HttpGet]
        [ActionName("GetWaitRead")]
        public HttpResponseMessage GetWaitRead(string userName, int size)
        {
            IFormCCProvider _formCCProvider = new KStarFormCCProvider();

            IList<FormCCModel> formCCModelList = _formCCProvider.ReceiveFormCC(userName);
            DateTime? start = null, end = null;
            start = InitStartDate(null);
            end = InitEndDate(null);

            if (start != null)
            {
                formCCModelList = formCCModelList.Where(r => r.CreatedDate >= start).ToList();
            }

            if (end != null)
            {
                formCCModelList = formCCModelList.Where(r => r.CreatedDate <= end).ToList();
            }
            string receiveStatus = "0";
            if (!string.IsNullOrWhiteSpace(receiveStatus))
            {
                formCCModelList = formCCModelList.Where(r => r.ReceiverStatus == (receiveStatus == "1" ? true : false)).ToList();
            }

            List<System.Collections.Hashtable> listHt = new List<System.Collections.Hashtable>();
            formCCModelList = formCCModelList.OrderByDescending(r => r.CreatedDate).ToList();
            foreach (var item in formCCModelList)
            {
                System.Collections.Hashtable _ht = new System.Collections.Hashtable();

                _ht.Add("ProcessFolio", item.ProcessFolio);
                _ht.Add("FormViewUrl", item.FormViewUrl);
                if (item.CreatedDate == null)
                {
                    _ht.Add("CreatedDate", item.CreatedDate);
                }
                else
                {
                    _ht.Add("CreatedDate", item.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                }
                _ht.Add("TaskCount", formCCModelList.Count);
                listHt.Add(_ht);
                if (listHt.Count == size)
                {
                    break;
                }
            }
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(listHt);
            var callback = Request.RequestUri.ParseQueryString()["callback"];

            if (callback == null)
            {
                return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
            }
            else
            {
                return new HttpResponseMessage() { Content = new StringContent(callback + "(" + jsonData + ")", Encoding.UTF8) };
            }
        }

        [HttpGet]
        public List<List<Configuration_ActDesc>> GetPrognosis(int procInstID, string actName)
        {
            if (_ConfigManager == null)
            {
                _ConfigManager = new ConfigManager(AuthenticationType.Form);
            }

            List<List<Configuration_ActDesc>> serrActDesc = _ConfigManager.GetDBPrognosisRoutes(procInstID, actName);

            //获取当前实例备注
            List<Configuration_ActDesc> actDescList = new List<Configuration_ActDesc>();

            var proInstDescList = _ConfigManager.GetProcInstStateDesc(procInstID);
            ProcessServiceReference.ProcessServiceClient processServiceClient = new ProcessServiceReference.ProcessServiceClient();
            foreach (var itemEntity in proInstDescList)
            {
                var _userNames = processServiceClient.GetActivityNotPopParticipants(procInstID, itemEntity.Name);
                Configuration_ActDesc actDesc = new Configuration_ActDesc();
                actDesc.Name = itemEntity.Name+"（当前环节）";
                actDesc.ActID = itemEntity.ActID;
                actDesc.LineName = "";
                actDesc.SourceName = "";
                actDesc.UserNames = string.Join(",", _userNames); 
                actDescList.Add(actDesc); 
            }
            if (actDescList.Count > 0)
            { 
                serrActDesc.Insert(0, actDescList); 
            } 
            foreach (var list in serrActDesc)
            {
                foreach (var _item in list)
                {
                    if (string.IsNullOrWhiteSpace(_item.UserNames)) continue;

                    string[] participants = _item.UserNames.Split(',');
                    
                     participants = GetUserFirstName(participants);

                     if (participants != null)
                     { 
                         _item.UserNames = string.Join(",", participants); 
                     }
                }
            }
            return serrActDesc;
        }

        /// <summary>
        /// 在途任务api
        /// </summary>
        [HttpGet]
        public bool GetTransitTask(int procInstID)
        {
            return SavePrognosisData(procInstID);
        }

        #region Prognosis
        //保存预判数据
        public bool SavePrognosisData(int procInstID)
        {
            if (_ConfigManager == null)
            {
                _ConfigManager = new ConfigManager(AuthenticationType.Form);
            }
            try
            {
             
               using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
               {
                   //获取对应的任务
                   var task = dbContext.ProcessPrognosisTask.Where(x => x.ProcInstID == procInstID).FirstOrDefault();

                   if (task != null)
                   {
                    
                       #region 已经存在的路径需要删除
                       var oldRoues=  dbContext.ProcessPrognosis.Where(x => x.ProcInstID == procInstID).ToList();
                       if (oldRoues != null)
                       {
                           dbContext.ProcessPrognosis.RemoveRange(oldRoues);
                           var roueParticipantsLinq = from rd in dbContext.ProcessPrognosisDetail join r in dbContext.ProcessPrognosis on rd.RSysID equals r.SysID where r.ProcInstID == procInstID select rd;

                           var oldRoueParticipants = roueParticipantsLinq.ToList();
                           if (oldRoueParticipants != null)
                               dbContext.ProcessPrognosisDetail.RemoveRange(oldRoueParticipants);
                       }
                       #endregion

                       ProcessServiceReference.ProcessServiceClient processServiceClient = new ProcessServiceReference.ProcessServiceClient();
                       //获取预判路径
                       List<List<Configuration_ActDesc>> serrActDesc = _ConfigManager.GetPrognosisRoutes(procInstID, KStarFormController.COMMON_DATAFIELD_DEFAULT_ACTION);
                       
                       if (serrActDesc != null && serrActDesc.Count > 0)
                       {
                           int _listCount = 0;
                           foreach (var list in serrActDesc)
                           {
                               foreach (var item in list)
                               {
                                   //获取对应实例ID 所在环节对应的处理人（不出列）
                                   string[] participants = item.UserNames == null ? processServiceClient.GetActivityNotPopParticipants(procInstID, item.Name) : new string[] { item.UserNames };
                                   Guid sysID = Guid.NewGuid();
                                   //添加环节信息到流程预判中
                                   dbContext.ProcessPrognosis.Add(new ProcessForm.ProcessPrognosis() { ActID = item.ActID, LineName = item.LineName, LinkOrder = _listCount, Name = item.Name, ProcInstID = procInstID, SourceName = item.SourceName, SysID = sysID });
                                   //有审核人，则添加对应的审核人
                                   if (participants != null)
                                   {
                                       foreach (var participant in participants)
                                           dbContext.ProcessPrognosisDetail.Add(new ProcessForm.ProcessPrognosisDetail() { RSysID = sysID, SysID = Guid.NewGuid(), UserName = participant });
                                   }
                               }

                               _listCount++;
                           }

                           dbContext.SaveChanges(); 
                           dbContext.ProcessPrognosisTask.Remove(task);
                           dbContext.SaveChanges();
                       }
                   } 
               }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #endregion

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
        private string GetApproveUrl(string  fullName)
        {
            string appUrl = string.Empty;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var config_ps = dbContext.Configuration_ProcessSetSet.Where(x => x.ProcessFullName == fullName).FirstOrDefault();
                if (config_ps != null)
                {
                     appUrl = config_ps.ApproveUrl;
                      
                }
            }
            return appUrl;
        }

        [NonAction]
        protected static DateTime InitStartDate(DateTime? startDate)
        {
            if (startDate == null)
            {
                return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            }
            else
            {
                DateTime newStartDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day);
                return newStartDate;
            }
        }
        [NonAction]
        protected static DateTime InitEndDate(DateTime? endDate)
        {
            if (endDate == null)
            {
                return DateTime.MaxValue;
            }
            else
            {
                DateTime newEndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day);
                return newEndDate.AddDays(1);
            }
        }

        private string[] GetUserFirstName(string[] userNames)
        {
            List<string> firstNames = new List<string>();
            try
            {
                if (userNames == null || userNames.Length == 0) return userNames;
                using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
                {
                    foreach (string userName in userNames)
                    {
                        var _name = (from u in dbContext.User where u.UserName == userName select u.FirstName).FirstOrDefault();

                        firstNames.Add(_name ?? userName);
                    } 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return firstNames.ToArray();
        }
    }
}
