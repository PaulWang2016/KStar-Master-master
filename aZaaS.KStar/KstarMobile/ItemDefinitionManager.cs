using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public sealed class ItemDefinitionManager
    {
        private static GroupDefinitionManager gdm = new GroupDefinitionManager();
        public ItemDefinitionEntity GetItemDefinitionById(int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ItemDefinition.Where(x => x.ID==id).SingleOrDefault();                
            }
        }

        public int AddItemDefinition(ItemDefinitionEntity entity)
        {
            int result = 0;
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
                    ctx.ItemDefinition.Add(entity);
                    ctx.SaveChanges();
                    result = entity.ID;                    
                }
                catch (Exception)
                {
                    result = 0;
                }
            }
            return result;

        }

        public bool ExistsItemName(string itemname, int id,int groupid)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                string orignitemname = string.Empty;
                if (id > 0)
                {
                    orignitemname = GetItemDefinitionById(id).Name;
                    if (orignitemname == itemname)
                    {
                        return false;
                    }
                }
                GroupDefinitionEntity group=gdm.GetGroupDefinitionById(groupid);
                if ((group.Name == "BaseInfo" && "sn,destination".Contains(itemname.ToLower())) || (group.Name == "ExtendInfo" && "stafficon,displayname".Contains(itemname.ToLower())) || (group.Name == "ProcBaseInfo" && "sn,folio,processname".Contains(itemname.ToLower())))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UpdateItemDefinition(ProcessDefinitionEntity process, ItemDefinitionEntity newEntity)
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

                    ProcessDefinitionEntity processheader = ctx.ProcessDefinition.FirstOrDefault(x => x.ID == oldProcess.ParentID);
                    //如果更新的item为Header子节点则同步更新Data
                    if (processheader.ChildID == gdm.GetGroupIdByName("Header"))
                    {
                        int childid=gdm.GetGroupIdByName("Data");

                        ProcessDefinitionEntity processrow = ctx.ProcessDefinition.Where(p => p.ParentID == processheader.ParentID && p.ID != processheader.ID).FirstOrDefault();
                        ProcessDefinitionEntity processdata = ctx.ProcessDefinition.Where(p => p.ParentID == processrow.ID && p.ChildID == childid).FirstOrDefault();
                        ProcessDefinitionEntity processdataitem = ctx.ProcessDefinition.Where(p => p.ParentID == processdata.ID && p.ChildID == oldProcess.ChildID).FirstOrDefault();
                        processdataitem.ConnectionString = process.ConnectionString;
                        processdataitem.Mapping = process.Mapping;
                        processdataitem.WhereString = process.WhereString;
                    }

                    ItemDefinitionEntity oldItemDefinition = ctx.ItemDefinition.FirstOrDefault(x => x.ID == newEntity.ID);
                    oldItemDefinition.Name = newEntity.Name;
                    //oldItemDefinition.LabelID = newEntity.LabelID;
                    if (oldItemDefinition.LabelID != null)
                    {
                        LabelContentEntity label = ctx.LabelContent.FirstOrDefault(x => x.LabelID == oldItemDefinition.LabelID);
                        label.Content = newEntity.LabelName;
                    }
                    else {
                        LabelContentEntity label = new LabelContentEntity();
                        label.Content = newEntity.LabelName;
                        label.LabelID = Guid.NewGuid();
                        label.Language = "zh-cn";
                        ctx.LabelContent.Add(label);
                        oldItemDefinition.LabelID = label.LabelID;
                    }
                    oldItemDefinition.Visible = newEntity.Visible;
                    oldItemDefinition.Editable = newEntity.Editable;
                    oldItemDefinition.Format = newEntity.Format;                    
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }
        public bool DeleteItemDefinition(ItemDefinitionEntity entity)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    ctx.ItemDefinition.Remove(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }

    }
}
