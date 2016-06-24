using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace aZaaS.KStar.KstarMobile
{
    public sealed class GroupDefinitionManager
    {
        private static LabelContentManager lcm = new LabelContentManager();

        public GroupDefinitionEntity GetGroupDefinitionById(int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.GroupDefinition.Where(x => x.ID==id).SingleOrDefault();                
            }
        }

        public GroupDefinitionEntity GetGroupDefinitionByName(string name)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.GroupDefinition.Where(x => x.Name==name).SingleOrDefault();
            }
        }

        public int GetGroupIdByName(string name)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                GroupDefinitionEntity group = ctx.GroupDefinition.Where(x => x.Name == name).SingleOrDefault();
                if (group != null)
                {
                    return group.ID;
                }
                return 0;
            }
        }

        public int AddGroupDefinition(GroupDefinitionEntity entity)
        {
            int result =0;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    LabelContentEntity label = new LabelContentEntity();
                    label.Content = entity.LabelName;
                    label.LabelID = Guid.NewGuid();
                    label.Language = "zh-cn";
                    ctx.LabelContent.Add(label);
                    entity.LabelID = label.LabelID;

                    ctx.GroupDefinition.Add(entity);
                    ctx.SaveChanges();
                    result = entity.ID;
                }
                catch (Exception)
                {
                    result =0;
                }
            }
            return result;

        }

        public bool ExistsGroupName(string groupname, int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                string origngroupname = string.Empty;
                if (id > 0)
                {
                    origngroupname = GetGroupDefinitionById(id).Name;
                    if (origngroupname == groupname)
                    {
                        return false;
                    }
                }
                if ("task,taskinfo,baseinfo,extendinfo,procbaseinfo,bizinfo,procloginfo,header,row,more,data".Contains(groupname.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 初始化group数据
        /// </summary>
        /// <returns></returns>
        public bool InitGroupData()
        {
            var result = true;
            using (TransactionScope ts = new TransactionScope())
            {
                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    try
                    {
                        GroupDefinitionEntity grouptask = new GroupDefinitionEntity();
                        GroupDefinitionEntity grouptaskinfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupbaseinfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupextendinfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupprocbaseinfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupbizinfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupprocloginfo = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupheader = new GroupDefinitionEntity();
                        GroupDefinitionEntity grouprow = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupdata = new GroupDefinitionEntity();
                        GroupDefinitionEntity groupmore = new GroupDefinitionEntity();


                        grouptask.Name="Task";
                        grouptask.LabelID = lcm.AddLabelContent("zh-cn", "Task");
                        //grouptask.Type = "Single";

                        grouptaskinfo.Name = "TaskInfo";
                        grouptaskinfo.LabelID = lcm.AddLabelContent("zh-cn", "TaskInfo");
                        //grouptaskinfo.Type = "Single";

                        groupbaseinfo.Name = "BaseInfo";
                        groupbaseinfo.LabelID = lcm.AddLabelContent("zh-cn", "BaseInfo");
                        groupbaseinfo.Type = "Single";

                        groupextendinfo.Name = "ExtendInfo";
                        groupextendinfo.LabelID = lcm.AddLabelContent("zh-cn", "ExtendInfo");
                        groupextendinfo.Type = "Single";

                        groupprocbaseinfo.Name = "ProcBaseInfo";
                        groupprocbaseinfo.LabelID = lcm.AddLabelContent("zh-cn", "ProBaseInfo");
                        groupprocbaseinfo.Type = "Single";

                        groupbizinfo.Name = "BizInfo";
                        groupbizinfo.LabelID = lcm.AddLabelContent("zh-cn", "BizInfo");
                        //groupbizinfo.Type = "Single";

                        groupprocloginfo.Name = "ProcLogInfo";
                        groupprocloginfo.LabelID = lcm.AddLabelContent("zh-cn", "ProLogInfo");
                        groupprocloginfo.Type = "Table";

                        groupheader.Name = "Header";
                        groupheader.LabelID = lcm.AddLabelContent("zh-cn", "Header");
                        //groupheader.Type = "Single";
                        
                        grouprow.Name = "Row";
                        grouprow.LabelID = lcm.AddLabelContent("zh-cn", "Row");
                        //grouprow.Type = "Single";

                        groupdata.Name = "Data";
                        groupdata.LabelID = lcm.AddLabelContent("zh-cn", "Data");
                        //groupdata.Type = "Single";

                        groupmore.Name = "More";
                        groupmore.LabelID = lcm.AddLabelContent("zh-cn", "More");
                        //groupmore.Type = "Single";
                         

                        ctx.GroupDefinition.Add(grouptask);
                        ctx.GroupDefinition.Add(grouptaskinfo);
                        ctx.GroupDefinition.Add(groupbaseinfo);
                        ctx.GroupDefinition.Add(groupextendinfo);
                        ctx.GroupDefinition.Add(groupprocbaseinfo);
                        ctx.GroupDefinition.Add(groupbizinfo);
                        ctx.GroupDefinition.Add(groupprocloginfo);
                        ctx.GroupDefinition.Add(groupheader);
                        ctx.GroupDefinition.Add(grouprow);
                        ctx.GroupDefinition.Add(groupdata);
                        ctx.GroupDefinition.Add(groupmore);
                        ctx.SaveChanges();
                        ts.Complete();
                    }
                    catch (Exception)
                    { 
                        result = false;
                        ts.Dispose();
                    }
                }
            }
            return result;
        }


        public bool UpdateGroupDefinition(ProcessDefinitionEntity process, GroupDefinitionEntity newEntity)
        {
            var result = true;            
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    ProcessDefinitionEntity oldProcess = ctx.ProcessDefinition.FirstOrDefault(x => x.ID == process.ID);
                    oldProcess.ConnectionString = process.ConnectionString;
                    oldProcess.Mapping = process.Mapping;
                    oldProcess.WhereString = process.WhereString;


                    var oldGroupDefinition = ctx.GroupDefinition.FirstOrDefault(x => x.ID == newEntity.ID);                    
                    oldGroupDefinition.Name = newEntity.Name;
                    //oldGroupDefinition.LabelID = newEntity.LabelID;
                    if (oldGroupDefinition.LabelID != null)
                    {
                        LabelContentEntity label = ctx.LabelContent.FirstOrDefault(x => x.LabelID == oldGroupDefinition.LabelID);
                        label.Content = newEntity.LabelName;
                    }
                    else
                    {
                        LabelContentEntity label = new LabelContentEntity();
                        label.Content = newEntity.LabelName;
                        label.LabelID = Guid.NewGuid();
                        label.Language = "zh-cn";
                        ctx.LabelContent.Add(label);
                        oldGroupDefinition.LabelID = label.LabelID;
                    }
                    oldGroupDefinition.Type = newEntity.Type;
                    oldGroupDefinition.Collapsed = newEntity.Collapsed;                    
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }
        public bool DeleteGroupDefinition(GroupDefinitionEntity entity)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    ctx.GroupDefinition.Remove(entity);
                    ctx.SaveChanges();
                }
                catch (Exception )
                {
                    result = false;
                }
            }
            return result;

        }

    }
}
