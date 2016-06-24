using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Wcf.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DelegateService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DelegateService.svc or DelegateService.svc.cs at the Solution Explorer and start debugging.
    public class DelegateService : IDelegateService
    {
        private readonly KStar.DelegationService _delegationservice;
        private readonly UserService _userService;
        public DelegateService()
        { 
            _delegationservice = new KStar.DelegationService();
            _userService = new UserService();
        }

        public List<DelegateInfo> GetDelegations(DelegateInfo delegateInfo,ref PagerInfo pager,bool isOverdue)
        {
            List<DataFieldFilter> dataFieldFilter = new List<DataFieldFilter>();
            PageCriteria pageCriteria = new PageCriteria() { PageSize = pager.PageSize, IsFirstQuery = true, PageIndex = pager.PageIndex };
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });            
            if (!string.IsNullOrEmpty(delegateInfo.FromUser))
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Like, FieldName = "FromUser", Value1 = delegateInfo.FromUser, StartLogical = CriteriaLogical.And });
            }
            if (!string.IsNullOrEmpty(delegateInfo.ToUser))
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Like, FieldName = "ToUser", Value1 = delegateInfo.ToUser, StartLogical = CriteriaLogical.And });
            }
            if (delegateInfo.StartDate != null && delegateInfo.EndDate != null)
            {                
                pageCriteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "StartDate", Compare = CriteriaCompare.GreaterOrEqual, Value1 = delegateInfo.StartDate });
                pageCriteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "StartDate", Compare = CriteriaCompare.LessOrEqual, Value1 = delegateInfo.EndDate });                
            }
            //过期
            if (isOverdue)
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "EndDate", Compare = CriteriaCompare.Less, Value1 = DateTime.Now });                                      
            }
            else
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "EndDate", Compare = CriteriaCompare.GreaterOrEqual, Value1 = DateTime.Now });                                      
            }
            var list = _delegationservice.GetAllDelegations(pageCriteria);                        
            List<DelegateInfo> delegateItems = new List<DelegateInfo>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegateInfo()
                {
                    ToUser = string.Join(",", _userService.GetUsersDisplayNameByUserName(item.ToUsers)),
                    FromUser = _userService.ReadUserBase(item.FromUser).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }
            pager.Total = pageCriteria.TotalCount;
            return delegateItems;
        }


        public bool DeleteDelegation(int id)
        {
            bool flag=true;
            try
            {
                _delegationservice.DeleteDelegation(id);
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public bool SetDelegationStatus(int id, bool status, string updatedBy)
        {
            bool flag = true;
            try
            {
                _delegationservice.SetDelegationEnable(id,status,updatedBy);
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public bool CreateDelegation(string tenantId, string userName, DelegateInfo delegateInfo, List<string> delegatetoList, List<string> processList)
        {            
            bool flag = true;
            try
            {
                foreach (var process in processList)
                {
                    Delegation dele = new Delegation();
                    dele.DeleType = Enum.GetName(typeof(DelegationType), DelegationType.Process);
                    dele.FullName = process;
                    dele.ActionType = Enum.GetName(typeof(ActionType), ActionType.Approval);
                    dele.GroupID = Guid.NewGuid();
                    dele.FromUser = delegateInfo.FromUser;
                    dele.ToUsers = delegatetoList;////
                    dele.StartDate = delegateInfo.StartDate.Value;
                    dele.EndDate = delegateInfo.EndDate.Value;
                    dele.Status = Enum.GetName(typeof(DelegationStatus), DelegationStatus.Available);
                    dele.ActivityName = string.Empty;
                    dele.Reason = delegateInfo.Reason;
                    dele.CreatedBy = userName;
                    dele.CreatedDate = DateTime.Now;
                    dele.UpdatedBy = userName;
                    dele.UpdatedDate = DateTime.Now;
                    dele.WorkTypeID = Guid.NewGuid().ToString();
                    dele.IsEnable = true;
                    dele.DeleInstCount = 0;
                    dele.TenantID = tenantId;

                    _delegationservice.SaveDelegation(dele);
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }       
    }
}
