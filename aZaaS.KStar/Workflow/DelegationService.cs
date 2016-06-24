using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;

namespace aZaaS.KStar
{
    public class DelegationService
    {
        private readonly DelegationFacade delegationFacade;

        public DelegationService()
        {
            this.delegationFacade = new DelegationFacade();
        }

        public void DelegateByProcess(Guid workTypeID, string processName, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByProcess(new ServiceContext(), workTypeID, processName, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateByProcesses(Guid workTypeID, List<String> processNames, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByProcesses(new ServiceContext(), workTypeID, processNames, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateByAllProcess(Guid workTypeID, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByAllProcess(new ServiceContext(), workTypeID, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateByActivity(Guid workTypeID, string processName, string activityName, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByActivity(new ServiceContext(), workTypeID, processName, activityName, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateBySN(Guid workTypeID, string sn, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateBySN(new ServiceContext(), workTypeID, sn, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateByProcInstID(int procInstID, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByProcInstID(new ServiceContext(), procInstID, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void DelegateByActInstID(int actInstID, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateByActInstID(new ServiceContext(), actInstID, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void SaveDelegation(Delegation dele)
        {

            this.delegationFacade.SaveDelegation(new ServiceContext(), dele);
        }


        public List<Delegation> GetUserDelegations(string userName)
        {

            return this.delegationFacade.GetUserDelegations(new ServiceContext(), userName);
        }


        public List<Delegation> GetAllDelegations(PageCriteria criteria)
        {

            return this.delegationFacade.GetAllDelegations(new ServiceContext(), criteria);
        }

        public List<Delegation> GetAllDelegations(DateTime timePoint, bool enabled = true)
        {
            var filter = new PageCriteria();

            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "IsEnable", CriteriaCompare.GreaterOrEqual, enabled));
            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "StartDate", CriteriaCompare.LessOrEqual, timePoint.ToString("yyyy-MM-dd HH:mm")));
            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "EndDate", CriteriaCompare.GreaterOrEqual, timePoint.ToString("yyyy-MM-dd HH:mm")));

            return this.delegationFacade.GetAllDelegations(new ServiceContext(), filter);
        }
        public List<string> GetCurrentDelegationUsers(string processName, DateTime timePoint)
        {
            var users = new List<string>();
            var filter = new PageCriteria();

            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "FullName", CriteriaCompare.Equal, processName));
            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "StartDate", CriteriaCompare.LessOrEqual, timePoint));
            filter.AddRegularFilter(new RegularFilter(CriteriaLogical.And, "EndDate", CriteriaCompare.GreaterOrEqual, timePoint));

            var items = this.delegationFacade.GetAllDelegations(new ServiceContext(), filter);

            if (items == null || !items.Any()) return users;

            items.ForEach(item =>
            {
                var parts = item.FromUser.Split(new char[] { ':' });
                var fromUser = parts.Length > 1 ? parts.Last() : parts.First();
                users.Add(fromUser);
            });

            return users;
        }


        public Delegation GetDelegation(int deleID)
        {

            return this.delegationFacade.GetDelegation(new ServiceContext(), deleID);
        }


        public void DeleteDelegation(int deleID)
        {

            this.delegationFacade.DeleteDelegation(new ServiceContext(), deleID);
        }


        public void DeleteDelegations(List<Int32> deleIDs)
        {

            this.delegationFacade.DeleteDelegations(new ServiceContext(), deleIDs);
        }


        public void DelegateForRequest(List<String> processNames, string fromUser, List<string> toUsers, DateTime startTime, DateTime endTime, string comment, string createdBy, bool isEnable)
        {

            this.delegationFacade.DelegateForRequest(new ServiceContext(), processNames, fromUser, toUsers, startTime, endTime, comment, createdBy, isEnable);
        }


        public void SetDelegationEnable(int deleID, bool isEnable, string updatedBy)
        {

            this.delegationFacade.SetDelegationEnable(new ServiceContext(), deleID, isEnable, updatedBy);
        }


        public void SetDelegationEnable(List<Int32> deleIDs, bool isEnable, string updatedBy)
        {

            this.delegationFacade.SetDelegationEnable(new ServiceContext(), deleIDs, isEnable, updatedBy);
        }


        public void SetDelegationEnableByUser(string username, bool isEnable, string updatedBy)
        {

            this.delegationFacade.SetDelegationEnableByUser(new ServiceContext(), username, isEnable, updatedBy);
        }


        public int AddDelegationLog(DelegationLog log)
        {

            return this.delegationFacade.AddDelegationLog(new ServiceContext(), log);
        }


        public List<DelegationLog> GetUserDelegationLogs(string userName)
        {

            return this.delegationFacade.GetUserDelegationLogs(new ServiceContext(), userName);
        }


        public List<DelegationLog> GetAllDelegationLogs(PageCriteria criteria)
        {

            return this.delegationFacade.GetAllDelegationLogs(new ServiceContext(), criteria);
        }
    }
}
