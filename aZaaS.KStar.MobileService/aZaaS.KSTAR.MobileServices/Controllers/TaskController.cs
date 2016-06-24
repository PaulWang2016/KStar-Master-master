using aZaaS.KSTAR.MobileServices.Models;
using System.Collections.Generic;
using System.Web.Http;
using System;
using aZaaS.KSTAR.MobileServices.Providers;
using System.Configuration;
using System.Net.Http;
 

namespace aZaaS.KSTAR.MobileServices.Controllers
{
    public class TaskController : WebAPIController
    {
        [HttpPost]
        public TaskInfo GetTaskInfo([FromBody]TaskInfo_In param)
        { 
             
            try
            {
                var result = _provider.GetTaskInfo(param.UserToken, param.Mask, param.SN, param.Destination);
                this.WriteLog("GetTaskInfo", Newtonsoft.Json.JsonConvert.SerializeObject(param), new Exception("Success"));
                return result;
            }
            catch (Exception ex)
            {
                this.WriteLog("GetTaskInfo", Newtonsoft.Json.JsonConvert.SerializeObject(param), ex);
                return null;
            }
        }

        [HttpPost]
        public System.Net.Http.HttpResponseMessage GetTaskList([FromBody]TaskList_In param)
        {
            List<Task> result = null;
            try
            {
                result = _provider.GetTaskList(param.UserToken, param.Mask, param.Filter, param.Paging, param.Sorting);
                this.WriteLog("GetTaskList", Newtonsoft.Json.JsonConvert.SerializeObject(param), new Exception("Success"));
             
            }
            catch (Exception ex)
            {
                this.WriteLog("GetTaskList", Newtonsoft.Json.JsonConvert.SerializeObject(param), ex);
                result = new List<Task>();
            }

            return new System.Net.Http.HttpResponseMessage() { Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8) };
        }

        [HttpGet]
        public System.Net.Http.HttpResponseMessage GetTaskLists()
        {
            List<Task> result = _provider.GetTaskList("", "", null, null, null);


            return new System.Net.Http.HttpResponseMessage() { Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8) };
        }

        [HttpPost]
        public ExecuteTaskResult ExecuteTask([FromBody]ExecuteTask_In param)
        {
            try
            {
                var result = _provider.ExecuteTask(param.UserToken, param.Mask, param.SN, param.Action, param.Opinion, param.Destination);
                this.WriteLog("ExecuteTask", Newtonsoft.Json.JsonConvert.SerializeObject(param), new Exception("Success"));
                return result;
            }
            catch (Exception ex)
            {
                this.WriteLog("ExecuteTask", Newtonsoft.Json.JsonConvert.SerializeObject(param), ex);
                return new ExecuteTaskResult()
                {
                    Message = ex.Message,
                    Result = "E",
                    MessageDetails = ex.StackTrace
                };
            }
        } 
        [HttpPost]
        public HttpResponseMessage GetVersion([FromUri]string version)
        {
            VersionSetting settign = _provider.GetVersion(version);
            return new System.Net.Http.HttpResponseMessage() { Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(settign), System.Text.Encoding.UTF8) };
        }
         
    } 
}
