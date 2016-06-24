using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.ParticipantSetService;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.Repositories;
using System.Data;
using System.Data.SqlClient;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.WorkflowConfiguration.Models;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class ActivityParticipantsController : Controller
    {
        //
        // GET: /Maintenance/ActivityParticipants/
        ActivityParticipantSetService service = new ActivityParticipantSetService();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetActivityParticipantsSet(string ProcessFullName, string ActivityName)
        {
            int pageIndex = 0;
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int.TryParse(page, out pageIndex);
            int.TryParse(Size, out pageSize);
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            var skip = (pageIndex - 1) * pageSize;
            List<AssigneGroup> list = new List<AssigneGroup>();
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                //var linq = from t in entity.ProcessActivityParticipantSet join e in entity.ProcessActivityParticipantSetEntry on t.SetID equals e.SetID 
                //           select new { AssignerName = t.AssignerName, EntryType = e.EntryType, EntryName = e.EntryName, IsPeeked = t.IsPeeked };

                var r = entity.ProcessActivityParticipantSet.Where(p => p.ProcessFullName == ProcessFullName && p.ActivityName == ActivityName&& p.ProcInstID==0).OrderBy(t=>t.Priority).ToList();
                if (r.Count > 0)
                {
                    var maxPriority = r[r.Count - 1].Priority;
                    foreach (ProcessActivityParticipantSet model in r)
                    {
                        AssigneGroup group = new AssigneGroup();
                        var tempType = string.Empty;
                        var last = string.Empty;
                        var entryName = string.Empty;
                        var type = string.Empty;
                        var setid = model.SetID;
                        group.SetID = setid.ToString();
                        group.AssignerName = model.AssignerName;

                        var s = entity.ProcessActivityParticipantSetEntry.Where(t => t.SetID == setid).ToList();
                        
                        if (s.Count > 1)
                        {
                            type = "加签组";
                        }
                        else
                        {
                            if (s.Count > 0)
                            {
                                type = GetTypeName(s[0].EntryType);
                            }
                        }
                        foreach (ProcessActivityParticipantSetEntry entry in s)
                        {
                            entryName += entry.EntryName + ",";
                        }
                        entryName = entryName.TrimEnd(',');
                        group.EntryName = entryName;
                        group.EntryType = type;
                        group.Priority = model.Priority.ToString();
                        group.IsPeeked = model.IsPeeked == true ? "已处理" : "待处理";
                        group.MaxPriority = maxPriority.ToString();
                        list.Add(group);

                    }
                }
                var total = list.Count;
                list = list.Skip(skip).Take(pageSize).ToList();
                var output = new { data = list, total = total };
                return Json(output, JsonRequestBehavior.AllowGet);
            }  
        }
        public JsonResult SaveParticipantSetAndEntry(string classString, string userListString, string SetID)
        {
            //00000000-0000-0000-0000-000000000000
            var msg = "保存成功！";
            List<ProcessActivityParticipantSetEntry> list = new List<ProcessActivityParticipantSetEntry>();
           
            try
            {
                Guid guid = System.Guid.NewGuid();
                var model =  Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessActivityParticipantSet>(classString);
                if (!string.IsNullOrEmpty(SetID))
                {
                    model.SetID = new Guid(SetID);
                }
                else
                {
                    model.SetID = guid;
                }
                model.DateAssigned = DateTime.Now;
               
                var enrtyList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ProcessActivityParticipantSetEntry>>(userListString);
                foreach (ProcessActivityParticipantSetEntry pa in enrtyList)
                {
                    //pa.SetID = model.SetID;
                    ProcessActivityParticipantSetEntry newModel = new ProcessActivityParticipantSetEntry();
                    newModel.SetID = model.SetID;
                    newModel.EntryID = pa.EntryID;
                    newModel.EntryName = pa.EntryName;
                    newModel.EntryType = pa.EntryType;
                    list.Add(newModel);
                }
                using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
                {
                    var max = int.MaxValue;

                    int priority = 1;
                    var pset = entity.ProcessActivityParticipantSet.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.ProcessFullName == model.ProcessFullName && p.ActivityName ==model.ActivityName&& p.Priority<max);
                    if (pset != null)
                    {
                        priority = (int)pset.Priority + 1;
                    }
                    else
                    {
                        if (model.ProcInstID != null && model.ProcInstID != 0)
                        {
                             pset = entity.ProcessActivityParticipantSet.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.ProcInstID== model.ProcInstID && p.ActivityName == model.ActivityName && p.Priority < max);
                             if (pset != null)
                             {
                                 priority = (int)pset.Priority + 1;
                             }
                        }
                    }
                    var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == model.SetID);
                    if (string.IsNullOrEmpty(model.ProcessFullName))
                    {
                        model.ProcessFullName = GetProcessName(model.ProcInstID.ToString());
                    }
                    if (result != null)
                    {
                        result.Assigner = model.Assigner;
                        result.AssignerName = model.AssignerName;
                        result.Remark = model.Remark;
                        result.SkipAssigner = model.SkipAssigner;
                    }
                    else
                    {
                        model.Priority = priority;
                        entity.ProcessActivityParticipantSet.Add(model);
                    }
                    var entry = entity.ProcessActivityParticipantSetEntry.Where(p => p.SetID == model.SetID).ToList();
                    entity.ProcessActivityParticipantSetEntry.RemoveRange(entry);
                    entity.ProcessActivityParticipantSetEntry.AddRange(list);
                    entity.SaveChanges();
                }
            }
            catch(Exception ex)
            { 
                msg="保存失败！";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public string GetProcessName(string ProcessId)
        {
            string processFullName = string.Empty;
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                string strSql = string.Format(@"SELECT ProcessFullName FROM [aZaaS.KSTAR].[dbo].[Configuration_ProcessSet] where ProcessSetID=
                    (select top 1 ProcSetID from k2.ServerLog.[Proc] where id=(SELECT [ProcID]
                 FROM [aZaaS.Framework].[dbo].[view_ProcinstList] where [ProcInstID]={0}))", ProcessId);

                using (SqlConnection conn = new SqlConnection(entity.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(strSql, conn))
                    {
                        if (conn.State == ConnectionState.Closed) { conn.Open(); }
                        var o = cmd.ExecuteScalar();
                        if (o != null)
                        {
                            processFullName = o.ToString();
                        }

                    }
                }
            }
            return processFullName;
        }
        public JsonResult GetActivityParticipantsSetByProcessInstID(string ProcessInstID, string ActivityName,string ProcFullName)
        {
            int pageIndex = 0;
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int.TryParse(page, out pageIndex);
            int.TryParse(Size, out pageSize);
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            int InstID = 0;
            int.TryParse(ProcessInstID, out InstID);
            var skip = (pageIndex - 1) * pageSize;
            List<AssigneGroup> list = new List<AssigneGroup>();
            var preinstall = 0;
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                ProcFullName = GetProcessName(ProcessInstID);
                var setList = entity.ProcessActivityParticipantSet.Where(p => p.ProcessFullName == ProcFullName && p.ActivityName == ActivityName&&p.ProcInstID==0).ToList();
                if (setList != null && setList.Count > 0)
                {
                    preinstall = 1;
                }
                var maxValue = int.MaxValue;
                var r = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == InstID && p.ActivityName == ActivityName&&p.Priority<maxValue).OrderBy(t => t.Priority).ToList();
                if (r.Count > 0)
                {
                    var maxPriority = r.Where(x => x.Priority < maxValue).Max(x => x.Priority).Value;
                    var l = r.Where(x=>x.IsPeeked==false).ToList();
                    int MinValue=0;
                    if (l != null&&l.Count>0)
                    {
                        MinValue = l.Min(x => x.Priority).Value;
                    }
                    foreach (ProcessActivityParticipantSet model in r)
                    {
                        AssigneGroup group = new AssigneGroup();
                        var tempType = string.Empty;
                        var last = string.Empty;
                        var entryName = string.Empty;
                        var type = string.Empty;
                        var setid = model.SetID;
                        group.SetID = setid.ToString();
                        group.AssignerName = model.AssignerName;

                        var s = entity.ProcessActivityParticipantSetEntry.Where(t => t.SetID == setid).ToList();

                        if (s.Count > 1)
                        {
                            type = "加签组";
                        }
                        else
                        {
                            if (s.Count > 0)
                            {
                                type = GetTypeName(s[0].EntryType);
                            }
                        }
                        foreach (ProcessActivityParticipantSetEntry entry in s)
                        {
                            entryName += entry.EntryName + ",";
                        }
                        entryName = entryName.TrimEnd(',');
                        group.EntryName = entryName;
                        group.EntryType = type;
                        group.Priority = model.Priority.ToString();
                        group.Peeked = model.IsPeeked == true ? 1 : 0;
                        group.MaxPriority = maxPriority.ToString();
                        group.MinPriority = MinValue.ToString();
                        group.FixedPriority = model.Priority == int.MaxValue ? 1 : 0;
                        group.Preinstall = model.IsPeeked == true ? 1 : 0;
                        list.Add(group);

                    }
                }
                var total = list.Count;
                list = list.Skip(skip).Take(pageSize).ToList();
                var output = new { data = list, total = total };
                return Json(output, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetParticipantSetEntry(string guid)
        {
            ProcessActivityParticipantSet set = null;
            var result = service.GetActivityParticipantsSetEntry(guid);
            if (!string.IsNullOrEmpty(guid))
            {
                Guid setID = new Guid(guid);
                 set = service.GetActivityParticipantsSet(setID);
            }
            var output = new { data = result, set = set };
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteParticipatnEntry(string ID)
        {
            int pk =0;
            int.TryParse(ID,out pk);
            service.RemoveActivityParticipantSetEntry(pk);
            return Json("删除成功！", JsonRequestBehavior.AllowGet);
        }
        public string GetTypeName(string strType)
        {
            string strName = string.Empty;
            switch (strType.ToLower())
            {
                case "user": strName = "用户"; break;
                case "orgnode": strName = "部门"; break;
                case "role": strName = "角色"; break;
                case "customtype": strName = "自定义角色"; break;
                case "position": strName = "职位"; break;
                default: strName = "用户"; break;
            }
            return strName;
        }
        public JsonResult UpperParticipantPorioty(string SetID)
        {
            var msg = "成功";
            try
            {
                Guid guid = new Guid(SetID);
                service.UpperActivityParticipantSet(guid);
                
            }
            catch (Exception ex)
            {
                msg = "失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteParticipantSet(string SetID)
        {
            var msg = "成功";
            try
            {
                Guid guid = new Guid(SetID);
                service.DeleteActivityParticipantSet(guid);

            }
            catch (Exception ex)
            {
                msg = "失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DownParticipantPorioty(string SetID)
        {
            var msg = "成功";
            try
            {
                Guid guid = new Guid(SetID);
                service.DownActivityParticipantSet(guid);

            }
            catch (Exception ex)
            {
                msg = "失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRouteActivityNameList(int procSetID, int actID, string fullName, string actName)
        {
            ConfigManager cm = new ConfigManager( Framework.Workflow.AuthenticationType.Form);
            var result = cm.GetLinkActivityNameRule(procSetID, actID, fullName, actName);
            var output = new { data = result };
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveLineRuleSetting(string strJson)
        {
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Configuration_LineRule>>(strJson);
            ConfigManager cm = new ConfigManager(Framework.Workflow.AuthenticationType.Form);
            //foreach (Configuration_LineRule line in list)
            //{
            //    line.SysID = System.Guid.NewGuid();
            //}
            if (list != null && list.Count > 0)
            {
                //var fullName = list[0].FullName;
                //var actName = list[0].SourceActivityName;
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    foreach (Configuration_LineRule line in list)
                    {
                        line.SysID = System.Guid.NewGuid();
                        var result = dbContext.Configuration_LineRule.FirstOrDefault(p => p.FullName == line.FullName && p.SourceActivityName == line.SourceActivityName&&p.TargetActivityName==line.TargetActivityName);
                        if (result != null)
                        {
                            result.RuleString = line.RuleString;
                            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                        }

                        else
                        {
                            dbContext.Configuration_LineRule.Add(line);
                        }
                        dbContext.SaveChanges();
                    }

                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


    }
    public class AssigneGroup { 
         public string SetID { get; set; }
         public string EntryName { get; set; }
         public string EntryType { get; set; }
         public string IsPeeked { get; set; }
         public int Peeked { get; set; }
         public string Priority { get; set; }
         public string MaxPriority { get; set; }
         public string MinPriority { get; set; }
         public int FixedPriority { get; set; }
         public string AssignerName { get; set; }
         public int Preinstall { get; set; }
    }
    public class ActivityNameRoute {
        public string FullName { get; set; }
        public string ActivityName { get; set; }
        public string LineRule { get; set; }
    }
}
