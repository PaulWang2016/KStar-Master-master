using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.SchedulerService;
using aZaaS.Framework.SchedulerService;
using aZaaS.KStar.Web.Controllers;
namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class ScheduleController : BaseMvcController
    {
        private static readonly ScheduleTaskService _schedulerservice = new ScheduleTaskService();
        #region 获取 任务维护 列表
        /// <summary>
        /// 获取 任务维护 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetScheduleConfigList()
        {
            IEnumerable<Tuple<TaskInfo, IntervalTrigger, IEnumerable<TaskExtraData>>> items = _schedulerservice.GetTasks();

            return Json(items.Select(s => new ScheduleConfigView()
            {
                //TaskInfo
                TaskName = s.Item1.TaskName,
                TypeName = s.Item1.TypeName,
                DateCreated = s.Item1.DateCreated,
                LastRunTime = s.Item1.LastRunTime,
                Status = s.Item1.Status,
                AssemblyName = s.Item1.AssemblyName,
                PrivateBinPath = s.Item1.PrivateBinPath,
                Description = s.Item1.Description,

                NotificationReceiver = s.Item1.NotificationReceiver,
                OnError = s.Item1.OnErrorNotification,
                OnExec = s.Item1.OnExecNotification,

                //IntervalTrigger
                ExitOn = s.Item2.ExitOn,
                Interval = s.Item2.Interval,
                IntervalType = s.Item2.IntervalType,
                StartTime = s.Item2.StartTime,
                TriggerDescription = s.Item2.Description,

                //TaskExtraDatas
                SystemName = GetExtraDataValue(s.Item3, "SystemName"),
                TargetName = GetExtraDataValue(s.Item3, "TargetName"),
                SourceName = GetExtraDataValue(s.Item3, "SourceName")
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult DisableTask(string taskName)
        {
            _schedulerservice.DisableTask(taskName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnableTask(string taskName)
        {
            _schedulerservice.EnableTask(taskName);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        private string GetExtraDataValue(IEnumerable<TaskExtraData> extraData, string key)
        {
            TaskExtraData data = null;
            if (extraData != null)
            {
                data = extraData.SingleOrDefault(e => e.DataKey == key);
            }

            return data != null ? data.DataValue : null;
        }

        #region 查询 任务维护 列表
        /// <summary>
        /// 查询 任务维护 列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public JsonResult FindScheduleConfigList(string name, DateTime? startDate, DateTime? endDate)
        {
            //int id = HomeController.ScheduleConfigList.Count + 1;
            //HomeController.ScheduleConfigList.Add(new ScheduleConfigView() { ScheduleID = id, TargetName = "Sql Server", SourceName = "Oracle", CreateTime = DateTime.Now, NextRunTime = DateTime.Now.AddHours(2), Status = true, TerminationTime = DateTime.MaxValue, DisplayName = "Automatic approval process " + id, IntervalPeriod = id + " minutes" });
            return Json(HomeController.ScheduleConfigList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 来源系统 列表         for  Schedule Config
        /// <summary>
        /// 获取 来源系统 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTargetList()
        {
            return Json(HomeController.TargetList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 目标系统 列表         for  Schedule Config
        /// <summary>
        /// 获取 目标系统 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSourceList()
        {
            return Json(HomeController.SourceList, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 添加 任务维护
        /// <summary>
        /// 添加 任务维护
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoCreateSchedule(ScheduleConfigView model)
        {
            TaskInfo taskinfo = new TaskInfo()
            {
                TaskName = model.TaskName,
                TypeName = model.TypeName,
                DateCreated = DateTime.Now,
                Status = (int)TaskStatus.Running,
                AssemblyName = model.AssemblyName,
                PrivateBinPath = model.PrivateBinPath,
                Description = model.Description,

                NotificationReceiver = model.NotificationReceiver,
                OnErrorNotification = model.OnError,
                OnExecNotification = model.OnExec
            };
            IntervalTrigger intervaltrigger = new IntervalTrigger()
            {
                TriggerName = model.TaskName,
                ExitOn = model.ExitOn,
                Interval = model.Interval,
                IntervalType = model.IntervalType,
                StartTime = model.StartTime,
                Description = model.TriggerDescription
            };

            IList<TaskExtraData> extraData = InitExtraData(model, taskinfo.TaskName);

            _schedulerservice.AddTask(taskinfo, extraData, intervaltrigger);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private static IList<TaskExtraData> InitExtraData(ScheduleConfigView model, string taskName)
        {
            IList<TaskExtraData> extraData = new List<TaskExtraData>();
            extraData.Add(new TaskExtraData() { DataKey = "SystemName", DataValue = model.SystemName, TaskName = taskName });
            extraData.Add(new TaskExtraData() { DataKey = "TargetName", DataValue = model.TargetName, TaskName = taskName });
            extraData.Add(new TaskExtraData() { DataKey = "SourceName", DataValue = model.SourceName, TaskName = taskName });
            return extraData;
        }
        #endregion

        #region 更新 任务维护
        /// <summary>
        /// 更新 任务维护
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoUpdateSchedule(ScheduleConfigView model)
        {
            TaskInfo taskinfo = new TaskInfo()
            {
                TaskName = model.TaskName,
                TypeName = model.TypeName,
                LastRunTime = model.LastRunTime,
                AssemblyName = model.AssemblyName,
                PrivateBinPath = model.PrivateBinPath,
                Description = model.Description,

                NotificationReceiver = model.NotificationReceiver,
                OnErrorNotification = model.OnError,
                OnExecNotification = model.OnExec
            };

            IntervalTrigger intervaltrigger = new IntervalTrigger()
            {
                TriggerName = model.TaskName,
                ExitOn = model.ExitOn,
                Interval = model.Interval,
                IntervalType = model.IntervalType,
                StartTime = model.StartTime,
                Description = model.TriggerDescription
            };
            IList<TaskExtraData> extraData = InitExtraData(model, taskinfo.TaskName);//暂时没方法更新
            _schedulerservice.UpdateTask(taskinfo, intervaltrigger, extraData);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除 任务维护      ---------批量
        /// <summary>
        /// 删除 任务维护      ---------批量
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDestroySchedule(List<string> idList)
        {
            foreach (var item in idList)
            {
                _schedulerservice.RemoveTask(item);
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
