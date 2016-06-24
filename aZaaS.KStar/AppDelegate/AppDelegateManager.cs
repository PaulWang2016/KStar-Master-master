using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using aZaaS.KStar.Acs;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Repositories;
using aZaaS.Framework.ACS.Core;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.AppDelegate
{
    internal class AppDelegateManager
    {
        private AuthenticationType _authType;
        public AppDelegateManager(AuthenticationType authType)
        {
            this._authType = authType;
        }

        public List<string> GetDelegateByPane(string pane)
        {
            //WorkflowManagementService wfMngService = new WorkflowManagementService(AuthenticationType.Windows);
            //var processset = wfMngService.GetProcessSets();
            List<string> Processes = new List<string>();
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuid != null)
                {
                    Processes = context.AppDelegate.Where(m => m.AppId == menuid).Select(s => s.ProcessFullName).ToList();
                }
                return Processes;
            }
        }

        public List<ProcessSet> GetProcessSetByPane(string pane)
        {
            List<ProcessSet> Processes = null;
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuid != null)
                {
                    Processes = context.AppDelegate.Where(m => m.AppId == menuid).Select(s => new ProcessSet() { FullName = s.ProcessFullName }).ToList();
                }

                return Processes;
            }
        }

        public List<ProcessSet> GetProcessSet()
        {
            List<ProcessSet> Processes = null;
            using (KStarDbContext context = new KStarDbContext())
            {
                Processes = context.AppDelegate.Select(s => new ProcessSet() { FullName = s.ProcessFullName }).ToList();

                return Processes;
            }
        }

        public List<ProcessSet> GetAllDelegate()
        {
            WorkflowManagementService wfMngService = new WorkflowManagementService(_authType);
            return wfMngService.GetProcessSets();
        }

        public void AddAppDelegate(string processFullName, string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                var appDelegate = context.AppDelegate.Where(m => m.AppId == menuid && m.ProcessFullName == processFullName).FirstOrDefault();
                if (appDelegate != null)
                    return;//已经存在
                if (menuid != null)
                {
                    context.AppDelegate.Add(new AppDelegateEntity() { ProcessFullName = processFullName, AppId = menuid, Id = Guid.NewGuid() });
                    context.SaveChanges();
                }
            }
        }

        public void DelAppDelegateByListID(string Name, string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuid != null)
                {
                    var appDelegateItem = context.AppDelegate.Where(m => m.ProcessFullName == Name && m.AppId == menuid).FirstOrDefault();
                    if (appDelegateItem != null)
                    {
                        context.AppDelegate.Remove(appDelegateItem);
                        context.SaveChanges();
                    }
                }
            }
        }

        public string GetAppKeyByProcess(string ProcessName)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                string pane = null;
                var Process = context.AppDelegate.SingleOrDefault(s => s.ProcessFullName == ProcessName);
                if (Process != null)
                {
                    pane = "";
                }
                return pane;
            }
        }
    }

}
