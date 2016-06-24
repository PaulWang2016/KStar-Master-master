using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using aZaaS.Framework.Workflow;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Caching;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    class DefaultDataProvider : BaseDataProvider
    {
        public DefaultDataProvider() { }

        protected override GroupExtend GetGroupDefinition(string processFullName, string groupName, string language)
        {
            using (KSTARServiceDbContext context = new KSTARServiceDbContext())
            {
                var infoGroup = context.GroupDefinitionSet.FirstOrDefault(r => r.Name.ToLower() == groupName);
                if (infoGroup == null)
                    return null;
                var infoDef = context.ProcessDefinitionSet.FirstOrDefault(r => r.ProcessFullName.ToLower() == processFullName.ToLower() && r.ChildID == infoGroup.ID && r.ParentID == 0);
                if (infoDef == null)
                    return null;

                GroupExtend groupDef = new GroupExtend(infoGroup);
                groupDef.ConnectionString = infoDef.ConnectionString;
                groupDef.Mapping = infoDef.Mapping;
                groupDef.WhereString = infoDef.WhereString;
                SetProcessDefinition(context, infoDef, groupDef, language);
                return groupDef;
            }
        }

        #region get GroupExtend

        private void SetProcessDefinition(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
        {
            var collection = context.ProcessDefinitionSet.Where(r => r.ParentID == procDef.ID).OrderBy(r => r.OrderNo).ToList();
            foreach (var item in collection)
            {
                switch (item.ChildType.ToLower())
                {
                    case GROUPNAME:
                        SetGroupExtend(context, item, groupExt, language);
                        break;
                    case ITEMNAME:
                        SetItemExtend(context, item, groupExt, language);
                        break;
                }
            }
        }

        private void SetGroupExtend(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
        {
            var groupDef = context.GroupDefinitionSet.SingleOrDefault(r => r.ID == procDef.ChildID);
            if (groupDef == null)
                return;

            GroupExtend group = new GroupExtend(groupDef);
            if (string.IsNullOrEmpty(procDef.ConnectionString))
            {
                group.ConnectionString = groupExt.ConnectionString;
            }
            else
            {
                group.ConnectionString = procDef.ConnectionString;
            }

            if (string.IsNullOrEmpty(procDef.Mapping))
            {
                group.Mapping = groupExt.Mapping;
            }
            else
            {
                group.Mapping = procDef.Mapping;
            }

            if (string.IsNullOrEmpty(procDef.WhereString))
            {
                group.WhereString = groupExt.WhereString;
            }
            else
            {
                group.WhereString = procDef.WhereString;
            }

            if (groupDef.LabelID.HasValue)
            {
                group.Label = GetLabelContent(context, groupDef.LabelID.Value, language);
            }

            groupExt.GroupList.Add(group);

            SetProcessDefinition(context, procDef, group, language);
        }

        private void SetItemExtend(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
        {
            var itemDef = context.ItemDefinitionSet.SingleOrDefault(r => r.ID == procDef.ChildID);
            if (itemDef == null)
                return;

            ItemExtend sub = new ItemExtend(itemDef);
            sub.Mapping = procDef.Mapping;
            if (itemDef.LabelID.HasValue)
            {
                sub.Label = GetLabelContent(context, itemDef.LabelID.Value, language);
            }

            groupExt.ItemList.Add(sub);
        }

        private string GetLabelContent(KSTARServiceDbContext context, Guid labelId, string language)
        {
            var label = context.LabelContentSet.FirstOrDefault(r => r.LabelID == labelId && r.Language == language);
            return label.Content;
        }

        #endregion

        public override string GetLabelContent(Guid labelId, string language)
        {
            using (var context = new KSTARServiceDbContext())
            {
                return GetLabelContent(context, labelId, language);
            }
        }

        public override void WriteLog(LogEntity log)
        {
            using (var context = new KSTARServiceDbContext())
            {
                context.LogEntitySet.Add(log);
                context.SaveChanges();
            }
        }

        public override void SetDataSourceList(GroupExtend ext, object objItem, List<Dictionary<string, object>> list)
        {
            if (!string.IsNullOrEmpty(ext.ConnectionString))
            {
                using (DbContext context = new DbContext(ext.ConnectionString))
                {
                    SetDataSource(ext, objItem, context, list);
                }
            }
            else
            {
                foreach (var group in ext.GroupList)
                {
                    SetDataSourceList(group, objItem, list);
                }
            }
        }

        private void SetDataSource(GroupExtend ext, object objItem, DbContext context, List<Dictionary<string, object>> list)
        {
            if (!string.IsNullOrEmpty(ext.Mapping))
            {
                string sql = "";
                List<DbParameter> paraList = new List<DbParameter>();
                System.Data.CommandType cmdType = System.Data.CommandType.Text;
                if (ext.Mapping.ToLower().StartsWith("exec "))
                {
                    cmdType = System.Data.CommandType.StoredProcedure;
                    sql = ext.Mapping.Substring(5, ext.Mapping.Length - 5);
                }
                else
                {
                    cmdType = System.Data.CommandType.Text;
                    List<string> fieldList = new List<string>();
                    ext.ItemList.ForEach(r =>
                    {
                        string fieldName = string.IsNullOrEmpty(r.Mapping) ? r.Name : r.Mapping;
                        if (!fieldList.Contains(fieldName))
                            fieldList.Add(fieldName);
                    });
                    sql = string.Format("Select {0} from {1}", "[" + string.Join("],[", fieldList) + "]", ext.Mapping);
                }
                if (!string.IsNullOrEmpty(ext.WhereString))
                {
                    string whereStr = ext.WhereString;
                    string[] arr = whereStr.Split(new string[] { " and ", " or ", " And ", " Or ", " AND ", " OR " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var wh in arr)
                    {
                        string[] whItems = wh.Replace(" ", "").Split(new string[] { "=@" }, StringSplitOptions.RemoveEmptyEntries);
                        if (whItems.Length != 2)
                            continue;
                        string propertyName = whItems[1].Trim(new char[] { ' ', ')' });
                        var val = GetPropertyValue(objItem, propertyName);
                        string paraName = propertyName.Replace(".", "");
                        if (propertyName.IndexOf('.') > -1)
                        {
                            whereStr = whereStr.Replace("@" + propertyName, "@" + paraName);
                        }
                        if (cmdType == System.Data.CommandType.StoredProcedure)
                        {
                            paraName = whItems[0].Trim(new char[] { ' ', ')' });
                        }

                        paraList.Add(new SqlParameter("@" + paraName, val));
                    }
                    if (cmdType != System.Data.CommandType.StoredProcedure)
                    {
                        if (!whereStr.ToLower().StartsWith("where"))
                        {
                            whereStr = "where " + whereStr;
                        }
                        sql += " " + whereStr;
                    }
                }

                SetDataSource(context, cmdType, ext, sql, list, paraList.ToArray());
            }
            else
            {
                foreach (var group in ext.GroupList)
                {
                    SetDataSource(group, objItem, context, list);
                }
            }
        }

        private void SetDataSource(DbContext context, System.Data.CommandType cmdType, GroupExtend ext, string sql, List<Dictionary<string, object>> list, params DbParameter[] paras)
        {
            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = cmdType;
            cmd.Parameters.AddRange(paras);
            context.Database.Connection.Open();
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var dic = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        dic.Add(dr.GetName(i), dr[i]);
                    }
                    list.Add(dic);
                }
            }
        }

    }
}