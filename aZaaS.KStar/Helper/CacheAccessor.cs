

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Helper
{
    public static class CacheAccessor
    {
        public static string GetProcessName(string processFullName)
        {
            var dics = CheckProcessCache(false);

            if (dics.ContainsKey(processFullName))
            {
                return dics[processFullName];
            }
            return string.Empty;
        }

        private static Dictionary<string, string> CheckProcessCache(bool onlyLoadingConfiguredProcess = true)
        {
            var dics = HttpRuntime.Cache["ProcessSets"] as Dictionary<string, string>;

            if (dics == null || dics.Count == 0)
            {
                dics = new Dictionary<string, string>();

                //缓存中没有，则从数据库中加载
                var svc = new ConfigManager(AuthenticationType.Windows);
                List<Configuration_ProcessSetDTO> process = svc.GetProcessSets();

                foreach (var item in process)
                {
                    if (!dics.ContainsKey(item.ProcessFullName))
                    {
                        dics.Add(item.ProcessFullName, item.ProcessName);
                    }
                }

                if (!onlyLoadingConfiguredProcess)
                {
                    var wfMngService = new WorkflowManagementService(AuthenticationType.Windows);
                    var processSet = wfMngService.GetProcessSets();

                    foreach (var item in processSet)
                    {
                        if (dics.ContainsKey(item.FullName))
                            continue;

                        dics.Add(item.FullName, item.FullName);
                    }
                }

                HttpRuntime.Cache.Remove("ProcessSets");
                HttpRuntime.Cache.Insert("ProcessSets", dics);
            }
            return dics;
        }

        public static string GetUserFullName(string userName)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics == null || dics.Count == 0)
            {
                dics = new Dictionary<string, string>();

                //缓存中没有，则从数据库中加载
                UserBO userBO = new UserBO();
                var users = userBO.GetAllBaseUsers(); //BaseUserDto is (performance)better.

                foreach (var user in users)
                {
                    if (!dics.ContainsKey(user.UserName.ToLower()))
                    {
                        dics.Add(user.UserName.ToLower(), user.FullName);
                    }
                }
                HttpRuntime.Cache.Remove("Users");
                HttpRuntime.Cache.Insert("Users", dics);
            }

            //Remove k2 securiy label if username is k2 user.
            userName = !userName.Contains(":") ? userName : userName.Split(':')[1];

            if (dics.ContainsKey(userName.ToLower()))
            {
                return dics[userName.ToLower()];
            }
            return userName; //If user not exists on db,we'll show origin user name!
        }

        /// <summary>
        /// 获取代理人用户名
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<string> GetDelegationUserName(string processName, string userName)
        {
            List<string> delegationToUsers = new List<string>();
            List<Delegation> delegations = HttpRuntime.Cache["ProcessDelegations"] as List<Delegation>;
            if (delegations == null || delegations.Count == 0)
            {
                //缓存中没有，则从数据库中加载  
                DelegationService _delegationService = new DelegationService();
                delegations = _delegationService.GetAllDelegations(DateTime.Now, true);
                HttpRuntime.Cache.Remove("ProcessDelegations");
                HttpRuntime.Cache.Insert("ProcessDelegations", delegations);
            }
            var tempdelegations = delegations.Where(x => x.FullName == processName && x.FromUser.ToLower() == userName.ToLower()).ToList();
            tempdelegations.ForEach(x =>
            {
                delegationToUsers.AddRange(x.ToUsers);
            });
            return delegationToUsers;
        }
        /// <summary>
        /// 获取审批以及代理人姓名
        /// </summary>
        /// <param name="processFullName"></param>
        /// <param name="prevApprovers"></param>
        /// <returns></returns>
        public static string GetApproversAndDelegationName(string processFullName, string prevApprovers)
        {
            if (string.IsNullOrEmpty(prevApprovers))
                return string.Empty;
            List<string> approversAndDelegations = new List<string>();
            List<string> delegationUsers = new List<string>();
            string[] prevApproverName = prevApprovers.Split(',');
            for (int i = 0; i < prevApproverName.Length; i++)
            {
                var tempdelegation = GetDelegationUserName(processFullName, prevApproverName[i]);
                if (tempdelegation != null && tempdelegation.Count > 0)
                {
                    tempdelegation = CustomHelper.GetUserWithOutLabel(tempdelegation);
                    tempdelegation.ForEach(x =>
                    {
                        delegationUsers.Add(GetUserFullName(x));
                    });
                    approversAndDelegations.Add(string.Format("{0}({1})", GetUserFullName(CustomHelper.GetUserWithOutLabel(prevApproverName[i])), string.Join(",", delegationUsers)));
                }
                else
                {
                    approversAndDelegations.Add(string.Format("{0}", GetUserFullName(CustomHelper.GetUserWithOutLabel(prevApproverName[i]))));
                }
            }
            return string.Join(",", approversAndDelegations);
        }

        public static string[] ProcessNamesFilter(string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
                return new string[] { };

            var processes = CheckProcessCache(false);
            var filteredProcessess = processes.Where(pair => pair.Key.Contains(filterValue)
                                                               || pair.Value.Contains(filterValue));

            return filteredProcessess.Select(pair => pair.Key).ToArray();
        }
    }
}
