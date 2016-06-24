using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Areas.WorkOvertimeApply.Models;
using aZaaS.Kstar.DAL;
using aZaaS.KStar.Web.Models.BasisEntity;
using aZaaS.KStar.Web.Utilities;

namespace aZaaS.KStar.Web.Areas.WorkOvertimeApply.Controllers
{
    public class WorkOvertimeApplyController : FormController
    {
        //
        // GET: /WorkOvertimeApply/WorkOvertimeApply/

        public ActionResult Index()
        {
            WorkOvertimeModel s = new WorkOvertimeModel();
            s.CreateTime = DateTime.Now;
            WorkOvertimeDetail workOvertime = new WorkOvertimeDetail();
            workOvertime.BeginDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            workOvertime.EndDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            workOvertime.BeginTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            workOvertime.EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            s.Detail.Add(workOvertime);
            return KStarFormView(s);
        }

        public override void OnWorkflowNewTaskStarting(Form.Infrastructure.WorkflowTaskContext context)
        {
            context.ProcessName = this.ProcessName;

            AddDateField(context);
            base.OnWorkflowNewTaskStarting(context);
        }

        private void AddDateField(Form.Infrastructure.WorkflowTaskContext context)
        {
            string userName = context.FormModel.SubmitterAccount;
            UserInfo userInfo = OrgUtility.GetUserInfo(userName);

            var model = context.DataModel<WorkOvertimeModel>();
            //绝对会有一条记录
            UserInfo proxy = OrgUtility.GetUserInfo(model.Detail[0].UserName);
             
                //判断是否是部门负责人
            if (userInfo.UserUpDepart.OrgType != OrgUtility.strCluster && userInfo.UserDepart.IsLeader)
                {
                    context.DataFields.Add("IsLeader", "True");
                    context.DataFields.Add("020Audit", "");

                    //部门负责人需要判断发起人是否是子公司
                    bool isDivision = GetUserDepartmentLevel(userName, OrgUtility.strDivision);

                    //判断是否是子公司
                    if (!isDivision)
                    {
                        context.DataFields.Add("IsDivision", "False");
                    }
                    else
                    {
                        context.DataFields.Add("IsDivision", "True");

                        //子公司总经理 当前发起人
                        context.DataFields.Add("030Division", GetDivisions(userInfo.UserID));
                    }
                }
                else
                {
                    context.DataFields.Add("IsLeader", "False");
                    context.DataFields.Add("020Audit", GetCluster(proxy.UserID));

                    //根据加班人的userName 去判断是否是子公司
                    bool isDivision = GetUserDepartmentLevel(proxy.UserName, OrgUtility.strDivision);

                    //判断是否是子公司 加班人的
                    if (!isDivision)
                    {
                        context.DataFields.Add("IsDivision", "False");
                    }
                    else
                    {
                        context.DataFields.Add("IsDivision", "True");
                        //子公司总经理 
                        context.DataFields.Add("030Division", GetDivisions(proxy.UserID));
                    }

                }
             
        }

        public override void OnWorkflowNewTaskStarted(aZaaS.KStar.Form.Infrastructure.WorkflowTaskContext context)
        {
            base.OnWorkflowNewTaskStarted(context);
            var model = context.DataModel<WorkOvertimeModel>();
            //发起流程时产生抄送
            if (model.CCMan != null)
            {
                string userName = UserEntireUtilites.GetSimpleUserUserName(model.CCMan);
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    this.WorkflowTocc(userName, context.FormId);
                }
            }
        }
         
        public override void OnWorkflowTaskExecuting(Form.Infrastructure.WorkflowTaskContext context)
        {
            context.ProcessName = this.ProcessName;

            base.OnWorkflowTaskExecuting(context);

            if (context.ActivityName.Equals("015_重新提交"))
            {
                var model = context.DataModel<WorkOvertimeModel>();
                if (model.CCMan != null)
                {
                    string userName = UserEntireUtilites.GetSimpleUserUserName(model.CCMan);
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        this.WorkflowTocc(userName, context.FormId);
                    }
                }
                AddDateField(context); 
            }

            if (context.ActivityName.Equals("040_HR部长审批") && EnumCollection.ActionName.Consent == context.ActionName)
            {
                var model = context.DataModel<WorkOvertimeModel>();
                string userName = string.Empty;
                foreach (var entity in model.Detail)
                {
                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        userName += entity.UserName;
                    }
                    else
                    {
                        userName += "," + entity.UserName;
                    }
                }
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    this.WorkflowTocc(userName, context.FormId);
                }
                //040同意的时候发起数据同步
                //OAEhrDAL oaEhrDal = new OAEhrDAL();

                //List<OverTime> overTimeList = new List<OverTime>();
                //foreach (WorkOvertimeDetail entity in model.Detail)
                //{
                //    OverTime item = new OverTime();
                //    item.DateEnd = entity.EndDate.ToString("yyyy-MM-dd");
                //    item.DateStart = entity.BeginDate.ToString("yyyy-MM-dd");
                //    item.IsRepeater = (entity.IsRepeater + string.Empty).Trim() == "是" ? true : false;
                //    item.Number = entity.UserId;
                //    item.TimeEnd = entity.EndTime.ToString("HH:mm");
                //    item.TimeStart = entity.BeginTime.ToString("HH:mm");
                //    overTimeList.Add(item);
                //}
                //oaEhrDal.InsertOvertimeList(overTimeList.ToArray());
            }
        }


        public override void OnWorkflowTaskExecuted(Form.Infrastructure.WorkflowTaskContext context)
        {
            base.OnWorkflowTaskExecuted(context);
            //if (context.ActivityName.Equals("040_HR部长审批") && EnumCollection.ActionName.Consent == context.ActionName)
            //{
            //    var model = context.DataModel<WorkOvertimeModel>(); 
            //    //040同意的时候发起数据同步
            //    OAEhrDAL oaEhrDal = new OAEhrDAL();

            //    List<OverTime> overTimeList = new List<OverTime>();
            //    foreach (WorkOvertimeDetail entity in model.Detail)
            //    {
            //        OverTime item = new OverTime();
            //        item.DateEnd = entity.EndDate.ToString("yyyy-MM-dd");
            //        item.DateStart = entity.BeginDate.ToString("yyyy-MM-dd");
            //        item.IsRepeater = (entity.IsRepeater = string.Empty).Trim() == "是" ? true : false;
            //        item.Number = entity.UserId;
            //        item.TimeEnd = entity.EndTime.ToString("HH:mm");
            //        item.TimeStart = entity.BeginTime.ToString("HH:mm");
            //        overTimeList.Add(item);
            //    }
            //    oaEhrDal.InsertOvertimeList(overTimeList.ToArray());
            //}

        }

        protected override void OnFormSubmitting(Form.Infrastructure.WorkflowTaskContext context)
        {
            //初始化的时候才执行
            if (string.IsNullOrWhiteSpace(context.ActionName))
            {
                WorkOvertimeModel entity = context.DataModel<WorkOvertimeModel>();
                entity.BillNO = "JBD" + DateTime.Now.ToString("yyyyMMdd") + CustomExtUtility.GetSerialNo("WorkOvertime");
                context.FormModel.ContentData = Newtonsoft.Json.JsonConvert.SerializeObject(entity, Newtonsoft.Json.Formatting.None);
                string userName = context.FormModel.SubmitterAccount;
                UserInfo userInfo = OrgUtility.GetUserInfo(userName);
                if (userInfo == null)
                {
                    throw new Exception("【" + userName + "】找不到对应的部门。");
                }
            }
            base.OnFormSubmitting(context);
        }

        [HttpPost]
        public JsonResult IsLeader()
        {
            UserInfo userInfo = null;

            bool isOK = false;
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            string message = "";
            try
            {
                userInfo = OrgUtility.GetUserInfo(this.UserName);
                if (userInfo == null)
                {
                    throw new Exception("找不到该工号信息。");
                }
                //上级是部门，则当前人处于科室
                if (userInfo.UserUpDepart.OrgType == OrgUtility.strCluster)
                {
                    ht.Add("IsLeader", false);
                }
                else
                {
                    ht.Add("IsLeader", userInfo.UserDepart.IsLeader);
                }
              
                isOK = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                ht.Add("message", message);
                isOK = false;
            }
            ht.Add("success", isOK);

            return Json(ht, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSimpleUserInfo(string userName)
        {
            UserInfo userInfo = null;
            bool isOK = false;
            string message = "";
            try
            {
                userInfo = OrgUtility.GetUserInfo(userName);
                if (userInfo == null)
                {
                    throw new Exception("找不到该工号信息。");
                }
                isOK = true;
            }
            catch (Exception ex)
            {
                isOK = false;
                message = ex.Message;

            }
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("success", isOK);
            if (isOK)
            {
                ht.Add("UserID", userInfo.UserID);
                ht.Add("RealName", userInfo.RealName);
                ht.Add("UserName", userInfo.UserName);

                //有科室
                if (userInfo.UserDepart.OrgType == OrgUtility.strProperty)
                {
                    ht.Add("AOTOffices", userInfo.UserDepart.OrgName);
                    if (userInfo.UserUpDepart.OrgType == OrgUtility.strCluster)
                    {
                        ht.Add("Department", userInfo.UserUpDepart.OrgName);
                    }
                    else
                    {
                        ht.Add("Department", "");
                    }

                }//有部门
                else if (userInfo.UserDepart.OrgType == OrgUtility.strCluster)
                {
                    ht.Add("Department", userInfo.UserDepart.OrgName);
                    ht.Add("AOTOffices", "");
                }
                else
                {
                    ht.Add("Department", "");
                    ht.Add("AOTOffices", "");
                }
                ht.Add("Postion", userInfo.Postion);
                ht.Add("UserLevel", userInfo.UserLevel);
            }
            else
            {
                ht.Add("message", message);
            }
            return Json(ht, JsonRequestBehavior.AllowGet);
        }

        private bool GetUserDepartmentLevel(string userId, string level)
        {
            var user = OrgUtility.GetUserDepartmentLevel(userId, level);
            if (user == null) return false;
             
            return user.Rows.Count > 0 ? true : false; 
        }

        /// <summary>
        /// 获取填单人子公司总经理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetDivisions(string userID)
        { 
            string orgType;

            var header = GetUserHead(userID, out orgType);

            //科长
            if (orgType == OrgUtility.strProperty)
            {
                header = GetUserHead(header, out orgType);
            }

            //部长
            if (orgType == OrgUtility.strCluster)
            {
                header = GetUserHead(header, out orgType);
            }

            //子公司总经理
            //if (orgType == OrgUtility.strDivision)
            //{
            //    userList.Add(header);
            //}

            return header;
        }

        private string GetUserHead(string userID, out string orgType)
        {
            var dt = OrgUtility.GetUserHead(userID);
            orgType = string.Empty;

            if (dt == null || dt.Rows.Count == 0)
            {
                return "";
            }

            orgType = dt.Rows[0]["Type"].ToString();

            return dt.Rows[0]["UserId"].ToString();
        }

        /// <summary>
        /// 获取填单人部长
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetCluster(string userID)
        {
            var userName = userID;

            var userList = new HashSet<string>();

            string orgType;
            string cluster = string.Empty;

            var header = GetUserHead(userName, out orgType);

            //首次
            cluster = header;

            //科长
            if (orgType == OrgUtility.strProperty)
            { 
                header = GetUserHead(header, out orgType);
                //上级是部长
                if (orgType == OrgUtility.strCluster)
                {
                    cluster = header;
                }
                //上级不是部长则保持不变 
            }
            return cluster;
        }


        public override string ProcessName
        {
            get { return @"Innos.KStar.Workflow\WorkOvertimeApply"; }
        }
    }
}
