using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    public abstract class BaseServiceProvider
    {
        protected const string LOGIN_USER_OK = "771f7b6c-936c-48cb-b57e-e2d40bac89f4";
        protected const string LOGIN_USER_ACCOUNT_INACTIVE = "5d45ef67-4387-4d49-ba81-0b20e0bb21ae";
        protected const string LOGIN_USER_DOESNT_EXIST = "492e7507-d08d-458a-8992-9932cedd93a6";
        protected const string LOGIN_USER_PASSWORD_INCORRECT = "30d2b74c-8662-4bc8-893e-81e512e068b3";
        protected const string EXECUTE_ACTION_SUCCESS = "f569a981-28c7-4853-bebd-e00a50cbaa2c";

        public virtual string DataProviderAssemblyName { get { return string.Empty; } }
        public virtual string DataProviderName { get { return string.Empty; } }

        public static BaseServiceProvider CreateInstance(string assemblyName,string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return new WFServiceProvider();
            }

            Assembly assembly;
            if (string.IsNullOrEmpty(assemblyName))
            {
                assembly = typeof(BaseServiceProvider).Assembly;
            }
            else
            {
                assembly = Assembly.LoadFrom(assemblyName);
            }

            return assembly.CreateInstance(providerName) as BaseServiceProvider;
        }

        public virtual LoginResult Login(LoginInfo info)
        {
            throw new NotImplementedException();
        }

        public virtual TaskInfo GetTaskInfo(string userToken, string mask, string sn,string destination)
        {
            throw new NotImplementedException();
        }

        public virtual List<Task> GetTaskList(string userToken, string mask, Filter filter, Paging paging, Sorting sorting)
        {
            throw new NotImplementedException();
        }

        public virtual ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion,string destination)
        {
            throw new NotImplementedException();
        }

        public virtual VersionSetting GetVersion(string version)
        {
            throw new NotImplementedException();
        }

        public virtual string GetAppIco()
        {
            throw new NotImplementedException();
        }

        public virtual void WriteServiceLog(LogEntity log)
        {
            throw new NotImplementedException();
        }
    }
}