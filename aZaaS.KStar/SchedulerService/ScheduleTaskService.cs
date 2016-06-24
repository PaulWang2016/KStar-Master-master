using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using aZaaS.KStar.Repositories;
using aZaaS.Framework.SchedulerService;

namespace aZaaS.KStar.SchedulerService
{
    public class ScheduleTaskService
    {
        private readonly ITaskManagement taskManagement;

        public ScheduleTaskService()
        {
            KStarDbContext dbContext = new KStarDbContext();
            string connectionString = dbContext.Database.Connection.ConnectionString;

            this.taskManagement = new SqlTaskManagement(connectionString);
        }

        public void AddTask(TaskInfo task, IEnumerable<TaskExtraData> extraData)
        {
            this.taskManagement.AddTask(task, extraData);
        }

        public void AddTask(TaskInfo task, IEnumerable<TaskExtraData> extraData, IntervalTrigger trigger)
        {
            this.taskManagement.AddTask(task, extraData, trigger);
        }

        public void AddTaskTrigger(IntervalTrigger trigger)
        {
            this.taskManagement.AddTaskTrigger(trigger);
        }

        public void DisableTask(string taskName)
        {
            this.taskManagement.DisableTask(taskName);
        }

        public void EnableTask(string taskName)
        {
            this.taskManagement.EnableTask(taskName);
        }

        public void RemoveTask(string taskName)
        {
            this.taskManagement.RemoveTask(taskName);
        }

        public void RemoveTaskTrigger(string taskName)
        {
            this.taskManagement.RemoveTaskTrigger(taskName);
        }

        public void UpdateTask(TaskInfo task)
        {
            this.taskManagement.UpdateTask(task);
        }
        public void UpdateTask(TaskInfo task, IEnumerable<TaskExtraData> extraData)
        {
            this.taskManagement.UpdateTask(task, extraData);
        }
        public void UpdateTask(TaskInfo task, IntervalTrigger trigger)
        {
            this.taskManagement.UpdateTask(task, trigger);
        }
        public void UpdateTask(TaskInfo task, IntervalTrigger trigger, IEnumerable<TaskExtraData> extraData)
        {
            this.taskManagement.UpdateTask(task, trigger, extraData);
        }

        public void UpdateTaskTrigger(IntervalTrigger trigger)
        {
            this.taskManagement.UpdateTaskTrigger(trigger);
        }
        public IEnumerable<Tuple<TaskInfo, IntervalTrigger, IEnumerable<TaskExtraData>>> GetTasks()
        {
            return this.taskManagement.GetTasks();
        }

    }
}
