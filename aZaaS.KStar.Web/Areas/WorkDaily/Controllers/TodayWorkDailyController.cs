using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.ViewResults;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Form.Helpers;
using aZaaS.Kstar.DAL;
using aZaaS.KStar.Web.Areas.WorkDaily.Models;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using aZaaS.KStar.Form;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Web.Models.BasisEntity;
using System.Net.Http;
using System.Text;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Controllers;


namespace aZaaS.KStar.Web.Areas.WorkDaily.Controllers
{
    public class TodayWorkDailyController : FormController
    {
        //
        // GET: /WorkDaily/TodayWorkDaily/

        public ActionResult Index()
        {
            var model = new WorkDailyModel();
            model.Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            model.StateDate = DateTime.Now;
            model.UserInfo = new Web.Models.BasisEntity.SimpleUser();
            UserEntireUtilites userEntireUtilites = new UserEntireUtilites();
            UserEntire userEntire = userEntireUtilites.GetUserEntire(this.UserName);
            //用户信息
            model.UserInfo.Name = userEntire.FirstName;
            model.UserInfo.Value = userEntire.SysId.ToString();
            model.UserInfo.UserName = userEntire.UserName;

            model.DepartmentsFirst = userEntire.Cluster_Name;
            model.Departments = userEntire.Property_Name;
            model.PostName = userEntire.Position_Name;
        
             
            //查询上一天当前用户信息
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var linq = basisEntity.Custom_Business_Daily.Where(x => x.CreateDate < model.Date && x.SysId == this.UserName).OrderByDescending(t => t.CreateDate).Take(1);

                Custom_Business_Daily entity = linq.FirstOrDefault();
 
                if (entity != null)
                {
                    List<ProcessFormContent> formList = basisEntity.ProcessFormContents.Where(x => x.FormID == entity.FormID).ToList();
                    //取首条记录
                    if (formList.Count > 0)
                    {
                        ProcessFormContent formEntity = formList.First();
                        WorkDailyModel workDailyModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkDailyModel>(formEntity.JsonData);
                        int count = 0;
                        foreach (var item in workDailyModel.WorkPlanList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.TomorrowDaily))
                            {
                                WorkPlan plan = new WorkPlan();
                                plan.TaskLevel = item.TaskLevel;
                                plan.TodayDaily = item.TomorrowDaily;
                                plan.ID = count;
                                model.WorkPlanList.Add(plan);
                                plan.IsRelate = true.ToString();
                                count++; 
                            }
                        
                        }
                        model.CCMan = workDailyModel.CCMan;
                        model.Verifier = workDailyModel.Verifier;
                        //关联导师
                        model.Advisor = workDailyModel.Advisor;
                    }   
                } 
            }
            if (model.WorkPlanList.Count == 0)
            {
                model.WorkPlanList.Add(new WorkPlan());
            }
            model.OtherMessageList.Add(new OtherMessage());
            return KStarFormView(model);
        }
        public override void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        { 
            context.ProcessName = this.ProcessName;//流程名称 
            base.OnWorkflowNewTaskStarting(context);
       
        }

        protected override void OnFormSubmitting(WorkflowTaskContext context)
        {
            base.OnFormSubmitting(context);
            //015  跳过验证 || context.ActivityName.LastIndexOf("015") >= 0
            if (string.IsNullOrWhiteSpace(context.ActionName) )
            {
                var model = context.DataModel<WorkDailyModel>();
                string date = model.Date.ToString("yyyy-MM-dd");//日报时间
                if (CheckTodayWork(date))
                {
                    throw new Exception("[" + date + "]的日报已经提交过，请勿重新提交。");
                }
            }
        }

        protected override void OnFormSubmitted(WorkflowTaskContext context)
        {
            base.OnFormSubmitted(context);
           
            ///只有发起的时候验证
            if (string.IsNullOrWhiteSpace(context.ActionName) || context.ActivityName.LastIndexOf("015") >= 0)
            {
                var model = context.DataModel<WorkDailyModel>();
                using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                {
                    Custom_Business_Daily entity = new Custom_Business_Daily();
                    entity.FormID = context.FormId;
                    entity.SysId = context.UserName;
                    entity.CreateDate = model.Date;
                    basisEntity.Custom_Business_Daily.Add(entity);
                    basisEntity.SaveChanges();
                }
            }
            if ((context.ActionName + string.Empty).Trim() == "不同意")
            {
                var model = context.DataModel<WorkDailyModel>();
                DeleteCurrentRelation(model);
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowUndoed(WorkflowTaskContext context)
        {
            base.OnWorkflowUndoed(context);
            var model = context.DataModel<WorkDailyModel>();
            DeleteCurrentRelation(model);
        }

        /// <summary>
        /// 废除
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskCanceled(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskCanceled(context);
            var model = context.DataModel<WorkDailyModel>();
            DeleteCurrentRelation(model);
            
        }
         
        private void DeleteCurrentRelation(WorkDailyModel model)
        {
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {

                var linq = basisEntity.Custom_Business_Daily.Where(x => x.CreateDate == model.Date && x.SysId == model.UserInfo.UserName).OrderByDescending(t => t.CreateDate).Take(1);

                Custom_Business_Daily entity = linq.FirstOrDefault();

                if (entity != null)
                {
                    basisEntity.Custom_Business_Daily.Remove(entity);
                    basisEntity.SaveChanges();
                }

            }
        }
        /// <summary>
        /// 获取日报
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public JsonResult GetWorkDaily(string date)
        {
            JsonResult jsonData = Json("", JsonRequestBehavior.AllowGet);
            try
            {
                if (this.UserName != null)
                { 
                    DateTime dTime = DateTime.Parse(date);
                    dTime = DateTime.Parse(dTime.ToString("yyyy-MM-dd"));
                    using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                    {

                        var linq = from daily in basisEntity.Custom_Business_Daily
                                   join c in basisEntity.ProcessFormContents
                                   on daily.FormID equals c.FormID
                                   into pro
                                   from contents in pro.DefaultIfEmpty()
                                   where daily.SysId == this.UserName && daily.CreateDate == dTime
                                   select new
                                   {
                                       FormID = daily.FormID,
                                       JsonData = contents.JsonData
                                   };
                        var objectList = linq.ToList();

                        List<object> JsonList = new List<object>();
                        if (objectList.Count > 0)
                        {
                            var model = JsonConvert.DeserializeObject<WorkDailyModel>(objectList[0].JsonData);
                            foreach (var entity in model.WorkPlanList)
                            {
                                entity.IsRelate = false.ToString();
                                entity.TodayDaily = entity.TomorrowDaily;
                                entity.TodaySummary = "";
                                entity.TomorrowDaily = "";
                            }
                            JsonList.Add(model.WorkPlanList);
                            JsonList.Add(model.OtherMessageList);
                            jsonData = Json(JsonList, JsonRequestBehavior.AllowGet);
                        } 
                    }
                }
                else
                {
                    throw new Exception("UserName is Null");
                }
            }
            catch (Exception ex)
            { 
            }

            return jsonData;
        }

        /// <summary>
        /// 获取当前时间的工作日情况
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public JsonResult GetWorkDailyDate(string date)
        {
            JsonResult jsonData = Json("", JsonRequestBehavior.AllowGet);
            try
            {
                if (this.UserName != null)
                {
                    DateTime dTime = DateTime.Parse(date);
                    dTime = DateTime.Parse(dTime.ToString("yyyy-MM-dd"));

                    DateTime currentDTime = DateTime.Now;
                    currentDTime = DateTime.Parse(currentDTime.ToString("yyyy-MM-dd"));
                    List<string> JsonList = new List<string>();
                    if (dTime > currentDTime)
                    {
                        JsonList.Add(currentDTime.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        JsonList.Add(dTime.ToString("yyyy-MM-dd"));
                    }
                    jsonData = Json(JsonList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                List<string> JsonList = new List<string>();
                JsonList.Add(DateTime.Now.ToString("yyyy-MM-dd"));
                jsonData = Json(JsonList, JsonRequestBehavior.AllowGet);
            }

            return jsonData; 
        }
         
        /// <summary>
        /// 判断今天的日报是否存在
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool CheckTodayWork(string date)
        { 
            try
            {
                //验证今天的日报是否填写
                if (this.UserName != null)
                { 
                    DateTime dTime = DateTime.Parse(date);
                    dTime = DateTime.Parse(dTime.ToString("yyyy-MM-dd"));//今天
                    using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                    {
                        var linq = from daily in basisEntity.Custom_Business_Daily
                                   join c in basisEntity.ProcessFormContents
                                   on daily.FormID equals c.FormID
                                   into pro
                                   from contents in pro.DefaultIfEmpty()
                                   where daily.SysId == this.UserName && daily.CreateDate == dTime
                                   select new
                                   {
                                       FormID = daily.FormID
                                   };
                        var objectList = linq.ToList();

                        List<int> JsonList = new List<int>();
                        if (objectList.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        } 
                    }
                }
                else
                {
                    throw new Exception("Time is Null");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        /// <summary>
        /// 流程任务结束完毕后的事件
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskExecuting(WorkflowTaskContext context)
        {
            context.ProcessName = @"Innos.KStar.Workflow\WorkDaily";//流程名称
            base.OnWorkflowTaskExecuting(context);
            var model = context.DataModel<WorkDailyModel>();
            if (context.ActivityName.Equals("020_审核日报") && context.ActionName=="同意")
            {
                if (model.CCMan != null)
                {
                    string userName = UserEntireUtilites.GetSimpleUserUserName(model.CCMan);
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        this.WorkflowTocc(userName, context.FormId);
                    } 
                }
                //抄送导师
                if (model.Advisor != null)
                {
                    string userName = UserEntireUtilites.GetSimpleUserUserName(model.Advisor);
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        this.WorkflowTocc(userName, context.FormId);
                    }
                }
                   
            }
        }


        public override string ProcessName
        {
            get { return @"aZaaS.KStar.Workflow\WorkDaily"; }
        }
         
    }
}
