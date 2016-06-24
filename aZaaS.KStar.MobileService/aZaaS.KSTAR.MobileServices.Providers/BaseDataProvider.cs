using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    public abstract class BaseDataProvider
    {
        protected const string GROUPNAME = "group";
        protected const string ITEMNAME = "item";

        public static BaseDataProvider CreateInstance(string assemblyName, string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return new DefaultDataProvider();
            }

            Assembly assembly;
            if (string.IsNullOrEmpty(assemblyName))
            {
                assembly = typeof(BaseDataProvider).Assembly;
            }
            else
            {
                assembly = Assembly.LoadFrom(assemblyName);
            }

            return assembly.CreateInstance(providerName) as BaseDataProvider;
        }

        public GroupExtend GetGroupDefinitionExtend(string processFullName, string groupName, string language)
        {
            string definitionKey = groupName + processFullName + language;

            var cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) };

            if (!cache.Contains(definitionKey))
            {
                cache.Add(definitionKey, "", policy);
            }

            if (cache[definitionKey] == "")
            {
                var groupDef = GetGroupDefinition(processFullName, groupName, language);
                if (groupDef != null)
                    cache[definitionKey] = groupDef;
                return groupDef;
            }
            else
            {
                return cache[definitionKey] as GroupExtend;
            }
        }

        public virtual string GetLabelContent(Guid labelId, string language)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteLog(LogEntity log)
        {
            throw new NotImplementedException();
        }

        public virtual void SetDataSourceList(GroupExtend ext, object objItem, List<Dictionary<string, object>> list)
        {
            throw new NotImplementedException();
        }

        protected virtual GroupExtend GetGroupDefinition(string processFullName, string groupName, string language)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(object obj, string name)
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
