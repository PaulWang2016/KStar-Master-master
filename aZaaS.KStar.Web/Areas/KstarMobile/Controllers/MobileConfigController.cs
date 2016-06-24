using aZaaS.KStar.KstarMobile;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Workflow.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.KstarMobile.Controllers
{
    [EnhancedHandleError]
    public class MobileConfigController : BaseMvcController, IDisposable
    {
        //
        // GET: /KstarMobile/MobileConfig/
        private static GroupDefinitionManager groupmanage = new GroupDefinitionManager();
        private static ProcessDefinitionManager processmanage = new ProcessDefinitionManager();
        private static ItemDefinitionManager itemmanage = new ItemDefinitionManager();
        private static LabelContentManager labelmanage = new LabelContentManager();
        public ActionResult Index()
        {
            GroupDefinitionEntity groupDefinition = groupmanage.GetGroupDefinitionById(1);
            return PartialView();
        }
        /// <summary>
        /// 添加流程
        /// </summary>
        /// <param name="processname"></param>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public JsonResult AddProcess(ProcessDefinitionEntity process)
        {
            bool flag = processmanage.AddProcessDefinition(process);
            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetProcess(int ID = 0)
        {
            List<ProcessTreeItem> items = new List<ProcessTreeItem>();
            List<ProcessDefinitionEntity> processs = new List<ProcessDefinitionEntity>();
            List<ProcessDefinitionEntity> query = new List<ProcessDefinitionEntity>();

            if (ID == 0)
            {
                query = processmanage.GetProcessDefinitionByParendId(0).Distinct(new ProcessComparer()).ToList<ProcessDefinitionEntity>();
            }
            else
            {
                ProcessDefinitionEntity srcprocess = processmanage.GetProcessDefinitionById(Math.Abs(ID));
                if (srcprocess.ParentID == 0 && ID > 0)
                {
                    ID = 0;
                }
                string processname = srcprocess.ProcessFullName;
                query = processmanage.GetProcessDefinitionByParendIdandName(Math.Abs(ID), processname).ToList<ProcessDefinitionEntity>();
                for (int i = 0; i < query.Count; i++)
                {
                    if (query[i].ChildType.ToLower() == "group")
                    {
                        GroupDefinitionEntity group = groupmanage.GetGroupDefinitionById(query[i].ChildID);
                        if (group != null)
                        {
                            query[i].ProcessFullName = group.Name;
                        }
                     
                        if (query[i].ParentID == 0)
                        {
                            query[i].ParentID = -1;
                        }
                    }
                    else if (query[i].ChildType.ToLower() == "item")
                    {
                        ItemDefinitionEntity item = itemmanage.GetItemDefinitionById(query[i].ChildID);
                        query[i].ProcessFullName = item.Name;
                    }
                }
            }
            processs.AddRange(query);
            processs = processs.OrderBy(x => x.OrderNo).ToList<ProcessDefinitionEntity>();
            foreach (var process in processs)
            {
                ProcessTreeItem item = new ProcessTreeItem
                {
                    ID = process.ID,
                    DisplayName = process.ProcessFullName,
                    ChildType = process.ChildType,
                    ChildID = process.ChildID,
                    ParentID = process.ParentID,
                    OrderNo = process.OrderNo,
                    ConnectionString = process.ConnectionString,
                    Mapping = process.Mapping,
                    WhereString = process.WhereString,
                    HasChildren = (process.ChildType.ToLower() == "group" ? true : false)
                };
                if (process.ParentID == -1)
                {
                    item.ID = -process.ID;
                }
                items.Add(item);
            }
            return Json(items.OrderBy(o => o.DisplayName), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProcessList()
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            List<Configuration_ProcessSetDTO> processList = svc.GetProcessSets(this.CurrentUser,false);
            if (processList == null)
            {
                processList = new List<Configuration_ProcessSetDTO>();
            }
            else
            {                
                List<ProcessDefinitionEntity> processEntityList = processmanage.GetAllProcessDefinition();
                foreach (var item in processEntityList)
                {
                    var selectitem = processList.Where(x => x.ProcessFullName == item.ProcessFullName).FirstOrDefault();
                    if (selectitem != null)
                    {
                        processList.Remove(selectitem);
                    }
                }
            }
            return Json(processList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult DelProcessDefinition(int ID)
        {
            bool flag = processmanage.DeleteProcessDefinition(ID);
            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取流程Item
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetProcessItem(int ID, int ChildID)
        {
            ItemDefinitionEntity item = itemmanage.GetItemDefinitionById(ChildID);
            if (item.LabelID != null)
            {
                LabelContentEntity label = labelmanage.GetLabelContentById(item.LabelID);
                item.LabelName = label.Content;
            }
            ProcessDefinitionEntity process = processmanage.GetProcessDefinitionById(ID);
            ExtendResult exresult = new ExtendResult() { ConnectionString = process.ConnectionString, Mapping = process.Mapping, WhereString = process.WhereString };
            return Json(new Result() { flag = true, data = item, extenddata = exresult, message = "" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult AddProcessItem(ProcessDefinitionEntity process, ItemDefinitionEntity entity)
        {
            List<ProcessDefinitionEntity> processitem = processmanage.AddItemProcessDefinition(process, entity);
            if (processitem.Count > 0)
            {
                ItemDefinitionEntity groupitem1 = itemmanage.GetItemDefinitionById(processitem[0].ChildID);
                processitem[0].ProcessFullName = groupitem1.Name;
                ProcessTreeItem treeitem1 = new ProcessTreeItem
                {
                    ID = processitem[0].ID,
                    DisplayName = processitem[0].ProcessFullName,
                    ChildType = processitem[0].ChildType,
                    ChildID = processitem[0].ChildID,
                    ParentID = processitem[0].ParentID,
                    OrderNo = processitem[0].OrderNo,
                    ConnectionString = processitem[0].ConnectionString,
                    Mapping = processitem[0].Mapping,
                    WhereString = processitem[0].WhereString,
                    HasChildren = (processitem[0].ChildType.ToLower() == "group" ? true : false)
                };
                ProcessTreeItem treeitem2 = null;
                if (processitem[1].ChildID > 0)
                {
                    ItemDefinitionEntity groupitem2 = itemmanage.GetItemDefinitionById(processitem[1].ChildID);
                    processitem[1].ProcessFullName = groupitem2.Name;
                    treeitem2 = new ProcessTreeItem
                    {
                        ID = processitem[1].ID,
                        DisplayName = processitem[1].ProcessFullName,
                        ChildType = processitem[1].ChildType,
                        ChildID = processitem[1].ChildID,
                        ParentID = processitem[1].ParentID,
                        OrderNo = processitem[1].OrderNo,
                        ConnectionString = processitem[1].ConnectionString,
                        Mapping = processitem[1].Mapping,
                        WhereString = processitem[1].WhereString,
                        HasChildren = (processitem[1].ChildType.ToLower() == "group" ? true : false)
                    };
                }
                else
                {
                    treeitem2 = new ProcessTreeItem();
                }
                return Json(new Result() { flag = true, data = treeitem1, extend = treeitem2, message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result() { flag = false, message = "操作失败!" }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 保存流程item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveProcessItem(int processid, ProcessDefinitionEntity process, ItemDefinitionEntity entity)
        {
            process.ID = processid;
            bool flag = itemmanage.UpdateItemDefinition(process, entity);
            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取流程Group
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetProcessGroup(int ID, int ChildID)
        {
            GroupDefinitionEntity group = groupmanage.GetGroupDefinitionById(ChildID);
            if (group.LabelID != null)
            {
                LabelContentEntity label = labelmanage.GetLabelContentById(group.LabelID);
                group.LabelName = label.Content;
            }
            ProcessDefinitionEntity process = processmanage.GetProcessDefinitionById(ID);
            ExtendResult exresult = new ExtendResult() { ConnectionString = process.ConnectionString, Mapping = process.Mapping, WhereString = process.WhereString };
            return Json(new Result() { flag = true, data = group, extenddata = exresult, message = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProcessGroup(ProcessDefinitionEntity process, GroupDefinitionEntity entity)
        {
            ProcessDefinitionEntity processgroup = processmanage.AddGroupProcessDefinition(process, entity);
            if (processgroup != null)
            {
                GroupDefinitionEntity groupitem = groupmanage.GetGroupDefinitionById(processgroup.ChildID);
                processgroup.ProcessFullName = groupitem.Name;
            }
            ProcessTreeItem treeitem = new ProcessTreeItem
            {
                ID = processgroup.ID,
                DisplayName = processgroup.ProcessFullName,
                ChildType = processgroup.ChildType,
                ChildID = processgroup.ChildID,
                ParentID = processgroup.ParentID,
                OrderNo = processgroup.OrderNo,
                ConnectionString = processgroup.ConnectionString,
                Mapping = processgroup.Mapping,
                WhereString = processgroup.WhereString,
                HasChildren = (processgroup.ChildType.ToLower() == "group" ? true : false)
            };
            return Json(new Result() { flag = (processgroup == null ? false : true), data = treeitem, message = "操作！" + (processgroup != null ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存流程Group
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveProcessGroup(int processid, ProcessDefinitionEntity process, GroupDefinitionEntity entity)
        {
            process.ID = processid;
            bool flag = groupmanage.UpdateGroupDefinition(process, entity);
            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 判断流程名称是否存在
        /// </summary>
        /// <param name="processname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ExistsProcessName(string processname, int id)
        {
            bool flag = processmanage.ExistsProcessName(processname, id);
            return Json(new Result() { flag = flag, message = "" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 用于判断名称是否与公用的groupname重复
        /// </summary>
        /// <param name="groupname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ExistsGroupName(string groupname, int id)
        {
            bool flag = groupmanage.ExistsGroupName(groupname, id);
            return Json(new Result() { flag = flag, message = "" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 用于判断名称是否与公用的itemname重复
        /// </summary>
        /// <param name="groupname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ExistsItemName(string itemname, int id, int groupid)
        {
            bool flag = itemmanage.ExistsItemName(itemname, id, groupid);
            return Json(new Result() { flag = flag, message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新流程名称
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateProcessName(string entityJson)
        {
            UpdateProcessActivity entity = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateProcessActivity>(entityJson);
            bool flag = true;
            ProcessDefinitionEntity process = processmanage.GetProcessDefinitionById(entity.ID);
            if (entity.ProcessFullName.Trim() != process.ProcessFullName)
            {
                flag = processmanage.UpdateProcessName(entity.ProcessFullName.Trim(), entity.ID);
            }
            UpdateProcessActivity(entity);

            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更改process排序
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="destinationid"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public JsonResult UpdateProcessOrderNo(int sourceid, int destinationid, string position)
        {
            bool flag = processmanage.UpdateProcessOrderNo(sourceid, destinationid, position);
            return Json(new Result() { flag = flag, message = "操作！" + (flag ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CopyProcessItem(int groupid, int itemid)
        {
            bool flag = true;
            ProcessTreeItem treeitem = new ProcessTreeItem();
            ProcessDefinitionEntity result = new ProcessDefinitionEntity();
            int res = processmanage.CopyProcessItem(groupid, itemid, out result);
            if (res < 0)
            {
                flag = false;
            }
            else
            {
                if (result != null)
                {
                    ItemDefinitionEntity itementity = itemmanage.GetItemDefinitionById(result.ChildID);
                    treeitem.ID = result.ID;
                    treeitem.DisplayName = itementity.Name;
                    treeitem.ChildType = result.ChildType;
                    treeitem.ChildID = result.ChildID;
                    treeitem.ParentID = result.ParentID;
                    treeitem.OrderNo = result.OrderNo;
                    treeitem.ConnectionString = result.ConnectionString;
                    treeitem.Mapping = result.Mapping;
                    treeitem.WhereString = result.WhereString;
                    treeitem.HasChildren = (result.ChildType.ToLower() == "group" ? true : false);
                }
            }
            return Json(new Result() { flag = flag, data = treeitem, extend = res, message = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaxProcess(string processName)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                string sql = @"select isnull(pe.ID,0) as ID,a.Name, isnull(pe.Checked,'') as Checked  
                                from [K2].[ServerLog].[Act]  as a
                                left join (
                                          SELECT  [ActivityName], 'checked' as Checked,ID FROM [KSTARService].[dbo].[ProcessPermission] where [ProcessFullName]=@ProcessFullName
                                          ) as pe on  pe.ActivityName =a.Name
                                where a.ProcID in(
                                          select max(v.ProcessVersionID) from [aZaaS.KStar].[dbo].[Configuration_ProcessSet] as p
                                          left join [aZaaS.KStar].[dbo].[Configuration_ProcessVersion] as v on v.Configuration_ProcessSetID =p.ID 
                                          where [ProcessFullName]=@ProcessFullName
		                                  ) ";

                SqlParameter[] sqlParameters = { new SqlParameter { ParameterName = "@ProcessFullName", Value = processName } };
                List<ProcessActivity> dic = null;
                try
                {
                   dic = dbContext.Database.SqlQuery<ProcessActivity>(sql, sqlParameters).ToList();

                }
                catch (Exception ex)
                {

                }
              
                string sql2 = "SELECT [ControllerFullName] FROM [KSTARService].[dbo].[ProcessExtend] where [ProcessFullName]=@ProcessFullName";

                SqlParameter[] sqlParameters2 = { new SqlParameter { ParameterName = "@ProcessFullName", Value = processName } };
                string controllerFullName = dbContext.Database.SqlQuery<string>(sql2, sqlParameters2).FirstOrDefault();
              
               System.Collections.Hashtable ht = new System.Collections.Hashtable();
               ht.Add("ControllerFullName", controllerFullName);
               ht.Add("ProcessActivitys", dic);

               return Json(ht, JsonRequestBehavior.AllowGet);
            } 
        }

        private void UpdateProcessActivity(UpdateProcessActivity entity)
        {
            using (KSTARServiceDBContext dbContext = new KSTARServiceDBContext())
            {
                SqlParameter[] sqlParameters = { new SqlParameter { ParameterName = "@ProcessFullName", Value = entity.ProcessFullName } };
                string deleteSql = "delete FROM [KSTARService].[dbo].[ProcessExtend]  where [ProcessFullName]=@ProcessFullName ";
                deleteSql += " delete FROM [KSTARService].[dbo].[ProcessPermission]  where [ProcessFullName]=@ProcessFullName";
                dbContext.Database.ExecuteSqlCommand(deleteSql, sqlParameters);
                ProcessExtend processExtend = new ProcessExtend();
                processExtend.ControllerFullName = entity.ControllerFullName;
                processExtend.ProcessFullName = entity.ProcessFullName;
                dbContext.ProcessExtendSet.Add(processExtend);
                foreach (ProcessActivity item in entity.ProcessActivitys)
                {
                    dbContext.ProcessPermissionSet.Add(new ProcessPermission { ActivityName = item.Name, ProcessFullName = entity.ProcessFullName });
                }
                dbContext.SaveChanges();
             } 
        }
    } 
    public class UpdateProcessActivity
    {
        public int ID { set; get; }

        public string ProcessFullName { set; get; }

        public string ControllerFullName { set; get; }

        public List<ProcessActivity> ProcessActivitys { set; get; }
    } 

    public class ProcessActivity
    {
        public int ID { set; get; }

        public string Name { set; get; }

        public string Checked { set; get; }
    }

    public class Result
    {
        public bool flag { get; set; }
        public object data { get; set; }
        public object extend { get; set; }
        public ExtendResult extenddata { get; set; }
        public string message { get; set; }
    }

    public class ExtendResult
    {
        public string ConnectionString { get; set; }
        public string Mapping { get; set; }
        public string WhereString { get; set; }
    }
}
