using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.AppDelegate;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Facades
{
    public class AppDelegateFacade
    {
        private AppDelegateManager appDelegateManager;
        public AppDelegateFacade(AuthenticationType authType)
        {
            appDelegateManager = new AppDelegateManager(authType);
        }
        public List<string> GetDelegateByPane(string pane)
        {
            return appDelegateManager.GetDelegateByPane(pane);
        }

        public List<ProcessSet> GetProcessSetByPane(string pane)
        {
            List<ProcessSet> Processes = appDelegateManager.GetProcessSetByPane(pane);
            foreach (var process in Processes)
            {
                var items = process.FullName.Split('\\');
                process.Folder = items[0];
                process.Name = items[1];
            }
            return Processes;
        }

        public List<ProcessSet> GetProcessSet()
        {
            List<ProcessSet> Processes = appDelegateManager.GetProcessSet();
            foreach (var process in Processes)
            {
                var items = process.FullName.Split('\\');
                process.Folder = items[0];
                process.Name = items[1];
            }
            return Processes;
        }
        public List<ProcessSet> GetAllDelegate()
        {
            return appDelegateManager.GetAllDelegate();
        }
        public void AddAppDelegate(string processFullName, string pane)
        {
            appDelegateManager.AddAppDelegate(processFullName, pane);
        }
        public void DelAppDelegateByListID(string Name, string pane)
        {
            appDelegateManager.DelAppDelegateByListID(Name, pane);
        }

        public string GetAppKeyByProcess(string ProcessName)
        {
            return appDelegateManager.GetAppKeyByProcess(ProcessName);
        }
    }
}
