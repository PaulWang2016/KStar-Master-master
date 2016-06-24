using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace aZaaS.KSTAR.MobileServices
{
    public abstract class BaseDataProvider
    {
        protected const string LOGIN_USER_OK = "771f7b6c-936c-48cb-b57e-e2d40bac89f4";
        protected const string LOGIN_USER_ACCOUNT_INACTIVE = "5d45ef67-4387-4d49-ba81-0b20e0bb21ae";
        protected const string LOGIN_USER_DOESNT_EXIST = "492e7507-d08d-458a-8992-9932cedd93a6";
        protected const string LOGIN_USER_PASSWORD_INCORRECT = "30d2b74c-8662-4bc8-893e-81e512e068b3";
        protected const string EXECUTE_ACTION_SUCCESS = "f569a981-28c7-4853-bebd-e00a50cbaa2c";

        public static BaseDataProvider CreateInstance()
        {
            string assemblyName = ConfigurationManager.AppSettings["DataProviderAssembly"];
            string providerName = ConfigurationManager.AppSettings["DataProvider"];
            Assembly assembly;
            if (string.IsNullOrEmpty(assemblyName))
            {
                assembly = Assembly.GetExecutingAssembly();
            }
            else
            {
                assembly = Assembly.LoadFrom(assemblyName);
            }

            if (string.IsNullOrEmpty(providerName))
            {
                return assembly.CreateInstance("aZaaS.KSTAR.MobileServices.MockDataProvider") as BaseDataProvider;
            }

            return assembly.CreateInstance(providerName) as BaseDataProvider;
        }

        public virtual LoginResult Login(LoginInfo info)
        {
            throw new NotImplementedException();
        }

        public virtual TaskInfo GetTaskInfo(string userToken, string mask, string sn)
        {
            throw new NotImplementedException();
        }

        public virtual List<Task> GetTaskList(string userToken, string mask, Filter filter, Paging paging, Sorting sorting)
        {
            throw new NotImplementedException();
        }

        public virtual ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion)
        {
            throw new NotImplementedException();
        }
    }
}