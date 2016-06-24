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
using aZaaS.KStar.Web.Areas.WeeklyNewspapers.Models;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Models.BasisEntity;
using aZaaS.KStar.Web.Controllers;


namespace aZaaS.KStar.Web.Areas.WeeklyNewspapers.Controllers
{
    public class WeeklyDailyController :FormController
    {
        //
        // GET: /WorkDaily/TodayWorkDaily/

        public ActionResult Index()
        {
            var model = new WeeklyModel();
            model.Date = DateTime.Now;
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
            //开始时间  和结束时间
            model.StartDate = GetWeeklyStartDate(DateTime.Now);
            model.EndDate = GetWeeklyEndDate(DateTime.Now);
              
            var date = DateTime.Parse(model.StartDate.ToString("yyyy-MM-dd"));
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                //查询周报的最近一周
                var linq = basisEntity.Custom_Business_WeeklyDaily.Where(x => x.StartDate < date && x.SysId == this.UserName).OrderByDescending(t => t.StartDate).Take(1);

                Custom_Business_WeeklyDaily entity = linq.FirstOrDefault();

                if (entity != null)
                {
                    List<ProcessFormContent> formList = basisEntity.ProcessFormContents.Where(x => x.FormID == entity.FormID).ToList();
                    //取首条记录
                    if (formList.Count > 0)
                    {
                        ProcessFormContent formEntity = formList.First();
                        WeeklyModel weeklyModelModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WeeklyModel>(formEntity.JsonData);
                        int count = 0;
                        foreach (var item in weeklyModelModel.WorkPlanList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.TomorrowDaily))
                            {
                                WeeklyDailyPlan plan = new WeeklyDailyPlan();
                                plan.TaskLevel = item.TaskLevel;
                                plan.TodayDaily = item.TomorrowDaily;
                                plan.ID = count;
                                plan.IsRelate = true.ToString();
                                model.WorkPlanList.Add(plan);
                                count++;
                            } 
                        }
                        model.CCMan = weeklyModelModel.CCMan;
                        model.Verifier = weeklyModelModel.Verifier;
                        //关联导师
                        model.Advisor = weeklyModelModel.Advisor;
                    } 
                } 
            }
            if (model.WorkPlanList.Count == 0)
            {
                model.WorkPlanList.Add(new WeeklyDailyPlan());
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
            if (string.IsNullOrWhiteSpace(context.ActionName) || context.ActivityName.LastIndexOf("015") >= 0)
            {
                var model = context.DataModel<WeeklyModel>();
                string date = model.StartDate.ToString("yyyy-MM-dd");
                int formID = CheckWeeklyDaily(date);
                if (formID > 0)
                {
                    throw new Exception("[" + date + "]至[" + model.EndDate.ToString("yyyy-MM-dd") + "]的周报已经提交过，请勿重新提交。");
                }

            }
            
        }

        protected override void OnFormSubmitted(WorkflowTaskContext context)
        {
            base.OnFormSubmitted(context);
            if (string.IsNullOrWhiteSpace(context.ActionName) || context.ActivityName.LastIndexOf("015") >= 0)
            {
                var model = context.DataModel<WeeklyModel>();

                using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                {
                    Custom_Business_WeeklyDaily entity = new Custom_Business_WeeklyDaily();
                    entity.FormID = context.FormId;
                    entity.SysId = context.UserName;
                    entity.CreateDate = DateTime.Now;
                    entity.StartDate = model.StartDate;
                    entity.EndDate = model.EndDate;
                    basisEntity.Custom_Business_WeeklyDaily.Add(entity);
                    basisEntity.SaveChanges();
                }
            }
            if ((context.ActionName + string.Empty).Trim() == "不同意")
            {
                var model = context.DataModel<WeeklyModel>();
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
            var model = context.DataModel<WeeklyModel>();
            DeleteCurrentRelation(model);
        }

        /// <summary>
        /// 废除
        /// </summary>
        /// <param name="context"></param>
        public override void OnWorkflowTaskCanceled(WorkflowTaskContext context)
        {
            base.OnWorkflowTaskCanceled(context);
             var model = context.DataModel<WeeklyModel>();
             DeleteCurrentRelation(model);

        }

        private void DeleteCurrentRelation(WeeklyModel model)
        { 
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                
                string date = model.StartDate.ToString("yyyy-MM-dd");
                DateTime dTime = DateTime.Parse(date);
                //查询周报的最近一周
                var linq = basisEntity.Custom_Business_WeeklyDaily.Where(x => x.StartDate == dTime && x.SysId == model.UserInfo.UserName).OrderByDescending(t => t.StartDate).Take(1);

                Custom_Business_WeeklyDaily entity = linq.FirstOrDefault();

                if (entity != null)
                {
                    basisEntity.Custom_Business_WeeklyDaily.Remove(entity);
                    basisEntity.SaveChanges();
                }
            }
        }

        [NonAction]
        public DateTime GetWeeklyStartDate(DateTime date)
        { 
            int weekIndex = Convert.ToInt32(date.DayOfWeek.ToString("d"));
            if (weekIndex == 0)
            {
                weekIndex = 7;//按照星期一 到星期天计算
            }
              
            date = date.AddDays((1-weekIndex)); 
            return date;
        }

       [NonAction]
        public DateTime GetWeeklyEndDate(DateTime date)
        {
            
            int weekIndex = Convert.ToInt32(date.DayOfWeek.ToString("d"));
            if (weekIndex == 0)
            {
                weekIndex = 7;//按照星期一 到星期天计算
            }
            date = date.AddDays((1 - weekIndex +6)); //结束时间 
            return date;
        }

        /// <summary>
        /// 获取给定的时间范围
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
       public JsonResult GetWeeklyDailyDate(string date)
        {
            JsonResult jsonData = Json("", JsonRequestBehavior.AllowGet);
            try
            {
                if (this.UserName != null)
                {
                    DateTime dTime = DateTime.Parse(date);
                    dTime = DateTime.Parse(dTime.ToString("yyyy-MM-dd"));//传递的时间
                    var StartDate = GetWeeklyStartDate(dTime);
                    if (StartDate <= GetWeeklyStartDate(DateTime.Now))
                    {
                        var EndDate = GetWeeklyEndDate(dTime);
                        List<string> JsonList = new List<string>();
                        JsonList.Add(StartDate.ToString("yyyy-MM-dd"));
                        JsonList.Add(EndDate.ToString("yyyy-MM-dd"));
                        jsonData = Json(JsonList, JsonRequestBehavior.AllowGet);
                    } 
                }
                
            }
            catch (Exception ex)
            {
                
            }

            return jsonData;

        }

       /// <summary>
       /// 获取周报
       /// </summary>
       /// <param name="date"></param>
       /// <returns></returns>
       public JsonResult GetWeeklyDaily(string date)
       {
           JsonResult jsonData = Json("", JsonRequestBehavior.AllowGet);
           try
           {
               if (this.UserName != null)
               {
                   DateTime dTime = DateTime.Parse(date);
                   dTime = DateTime.Parse(dTime.ToString("yyyy-MM-dd"));
                   dTime = GetWeeklyStartDate(dTime);//开始时间
                   using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                   {

                       var linq = from daily in basisEntity.Custom_Business_WeeklyDaily
                                  join c in basisEntity.ProcessFormContents
                                  on daily.FormID equals c.FormID
                                  into pro
                                  from contents in pro.DefaultIfEmpty()
                                  where daily.SysId == this.UserName && daily.StartDate == dTime
                                  select new
                                  {
                                      FormID = daily.FormID,
                                      JsonData = contents.JsonData
                                  };
                       var objectList = linq.ToList();

                       List<object> JsonList = new List<object>();
                       if (objectList.Count > 0)
                       {
                           var model = JsonConvert.DeserializeObject<WeeklyModel>(objectList[0].JsonData);
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
        /// 上个周是否填写
        /// </summary>
        /// <returns></returns>
       [HttpPost]
        public JsonResult IsLastWeeklyNull(string date)
       {
           bool isOK = false;
           string message = string.Empty;
           try
           {
               DateTime dTime = DateTime.Parse(date);
               using (BasisEntityContainer basisEntity = new BasisEntityContainer())
               {

                   var StartDate = GetWeeklyStartDate(dTime);
                   
                     var lastStartDate = StartDate.AddDays(-7);
                     Custom_Business_WeeklyDaily entity = basisEntity.Custom_Business_WeeklyDaily.FirstOrDefault(x => x.SysId == this.UserName && x.StartDate == lastStartDate);
                     if (entity == null)
                     {
                         isOK = true;
                         message = "您上周（" + lastStartDate.ToString("yyyy-MM-dd") + "~" + lastStartDate.AddDays(6).ToString("yyyy-MM-dd") + "）没有提交周报，确定提交（" + StartDate.ToString("yyyy-MM-dd") + "~" + StartDate.AddDays(6).ToString("yyyy-MM-dd") + "）吗？";
                     }
               }
           }
           catch (Exception ex)
           {
               isOK = false;
               message = ex.Message;
           }

           System.Collections.Hashtable ht = new System.Collections.Hashtable();
           ht.Add("succeed", isOK);
           ht.Add("message", message);
           return Json(ht, JsonRequestBehavior.AllowGet);
          
       }

       /// <summary>
       /// 判断今天的周报是否存在
       /// </summary>
       /// <param name="date"></param>
       /// <returns></returns>
      [NonAction]
       public int CheckWeeklyDaily(string date)
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
                       var linq = from daily in basisEntity.Custom_Business_WeeklyDaily
                                  join c in basisEntity.ProcessFormContents
                                  on daily.FormID equals c.FormID
                                  into pro
                                  from contents in pro.DefaultIfEmpty()
                                  where daily.SysId == this.UserName && daily.StartDate == dTime
                                  select new
                                  {
                                      FormID = daily.FormID
                                  };
                       var objectList = linq.ToList();
                     
                       List<int> JsonList = new List<int>();
                       if (objectList.Count > 0)
                       {
                           return objectList[0].FormID;
                       }
                       else
                       {
                           return 0;
                       }
                   }
               }
               else
               {
                   throw new Exception("UserInfo is Null");
               }
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


      public override void OnWorkflowTaskExecuting(WorkflowTaskContext context)
      {
          context.ProcessName = this.ProcessName;//流程名称
          base.OnWorkflowTaskExecuting(context);
          var model = context.DataModel<WeeklyModel>();
          if (context.ActivityName.Equals("020_审核周报") && context.ActionName == "同意")
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
          get { return @"Innos.KStar.Workflow\WeeklyDaily"; }
      }
    }
}
