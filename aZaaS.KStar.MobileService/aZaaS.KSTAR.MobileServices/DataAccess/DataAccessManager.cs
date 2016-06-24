using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using aZaaS.Framework.Workflow;
using System.Data.Common;
using System.Data.SqlClient;

namespace aZaaS.KSTAR.MobileServices.DataAccess
{
    public class DataAccessManager
    {
        private const string GROUPNAME = "group";
        private const string ITEMNAME = "item";

        public static GroupExtend GetGroupDefinition(string processFullName, string groupName, string language)
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
                SetProcessDefinition(context, infoDef, groupDef, language);
                return groupDef;
            }
        }

        private static void SetProcessDefinition(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
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

        private static void SetGroupExtend(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
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

        private static void SetItemExtend(KSTARServiceDbContext context, ProcessDefinition procDef, GroupExtend groupExt, string language)
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

        private static string GetLabelContent(KSTARServiceDbContext context, Guid labelId, string language)
        {
            var label = context.LabelContentSet.FirstOrDefault(r => r.LabelID == labelId && r.Language == language);
            return label.Content;
        }

        public static string GetLabelContent(Guid labelId, string language)
        {
            using (var context = new KSTARServiceDbContext())
            {
                return GetLabelContent(context, labelId, language);
            }
        }

        public static void WriteLog(LogEntity log)
        {
            using (var context = new KSTARServiceDbContext())
            {
                context.LogEntitySet.Add(log);
                context.SaveChanges();
            }
        }

        public static void SetDataSource(GroupExtend ext, object objItem, List<Dictionary<string, object>> list)
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
                    SetDataSource(group, objItem, list);
                }
            }
        }

        private static void SetDataSource(GroupExtend ext, object objItem, DbContext context, List<Dictionary<string, object>> list)
        {
            if (!string.IsNullOrEmpty(ext.Mapping))
            {
                List<string> fieldList = new List<string>();
                List<DbParameter> paraList = new List<DbParameter>();
                ext.ItemList.ForEach(r =>
                {
                    string fieldName = string.IsNullOrEmpty(r.Mapping) ? r.Name : r.Mapping;
                    if (!fieldList.Contains(fieldName))
                        fieldList.Add(fieldName);
                });
                string sql = string.Format("Select {0} from {1}", "[" + string.Join("],[", fieldList) + "]", ext.Mapping);
                if (!string.IsNullOrEmpty(ext.WhereString))
                {
                    string whereStr = ext.WhereString;
                    string[] arr = whereStr.Split(new string[] { " and ", " or ", " And ", " Or ", " AND ", " OR " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var wh in arr)
                    {
                        string[] whItems = wh.Split(new string[] { "=@" }, StringSplitOptions.RemoveEmptyEntries);
                        if (whItems.Length != 2)
                            throw new Exception(ext.Name + " WhereString format error!");
                        string propertyName = whItems[1].Trim(new char[] { ' ', ')' });
                        var val = GetPropertyValue(objItem, propertyName);
                        string paraName=propertyName.Replace(".", "");
                        if (propertyName.IndexOf('.') > -1)
                        {
                            whereStr = whereStr.Replace("@" + propertyName, "@" + paraName);
                        }
                        paraList.Add(new SqlParameter("@" + paraName, val));
                    }
                    if (!whereStr.StartsWith("where"))
                    {
                        whereStr = "where " + whereStr;
                    }
                    sql += " " + whereStr;
                }
                SetDataSource(context, ext, sql, list, paraList.ToArray());
            }
            else
            {
                foreach (var group in ext.GroupList)
                {
                    SetDataSource(group, objItem, context, list);
                }
            }
        }

        private static void SetDataSource(DbContext context, GroupExtend ext, string sql, List<Dictionary<string, object>> list, params DbParameter[] paras)
        {
            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = sql;
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

        public static object GetPropertyValue(object obj, string name)
        {
            var arrName = name.Split('.');
            var propertyInfo = obj.GetType().GetProperty(arrName[0]);
            if (propertyInfo == null)
                return null;

            var objInfo = propertyInfo.GetValue(obj, null);

            if (arrName.Length == 1)
            {
                return objInfo;
            }
            else
            {
                return GetPropertyValue(objInfo, arrName[1]);
            }
        }
    }
}