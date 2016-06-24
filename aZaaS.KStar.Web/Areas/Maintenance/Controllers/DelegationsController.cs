using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Workflow.Configuration;
namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class DelegationsController : BaseMvcController
    {
        private static KStar.DelegationService _delegationservice;
        private static ConfigManager configmanager;                    
        private static UserService userService;

        public DelegationsController()
        {
            _delegationservice = new KStar.DelegationService();            
            userService = new UserService();
            configmanager = new ConfigManager(this.AuthType);
            configmanager.TenantID = TenantID();
        }

        private string _userName()
        {
            return this.CurrentWorkflowUser;
        }

        #region 获取 My Delegation
        /// <summary>
        /// 获取 My Delegation
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDelegation(string pane)
        {
            PageCriteria pageCriteria = new PageCriteria();
            pageCriteria.PageSize = int.MaxValue;
            pageCriteria.IsFirstQuery = true;
            pageCriteria.PageIndex = 0;
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "FromUser", Value1 = _userName(), StartLogical = CriteriaLogical.And });
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "TenantID", Value1 = pane, StartLogical = CriteriaLogical.And });

            var list = _delegationservice.GetAllDelegations(pageCriteria);
            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }


            return Json(delegateItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelegations(string pane,
            int take = 15, int skip = 0, int page = 1, int pageSize = 15,
            [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            PageCriteria pageCriteria = new PageCriteria();
            pageCriteria.PageSize = pageSize;
            pageCriteria.IsFirstQuery = true;
            pageCriteria.PageIndex = page - 1;
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "FromUser", Value1 = _userName(), StartLogical = CriteriaLogical.And });
            //pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "TenantID", Value1 = pane, StartLogical = CriteriaLogical.And });
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });

            foreach (var item in sort)
            {
                pageCriteria.AddSortFilter(new SortFilter() { FieldName = item.field, SortDirection = item.dir });
            }

            var list = _delegationservice.GetAllDelegations(pageCriteria);
            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }


            return Json(new { total = pageCriteria.TotalCount, data = delegateItems }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FindDelegations(string pane, string start, string end, string delegateTo, string isOverdue)
        {
            DateTime? startDate = null, endDate = null;           
            PageCriteria pageCriteria = new PageCriteria();
            pageCriteria.PageSize = int.MaxValue;
            pageCriteria.IsFirstQuery = true;
            pageCriteria.PageIndex =0;
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "FromUser", Value1 = _userName(), StartLogical = CriteriaLogical.And });
            //pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "TenantID", Value1 = pane, StartLogical = CriteriaLogical.And });
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });

            if (!string.IsNullOrEmpty(delegateTo))
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Like, FieldName = "ToUser", Value1 = GetUserWithLabel(delegateTo), StartLogical = CriteriaLogical.And });
            }          

            var list = _delegationservice.GetAllDelegations(pageCriteria);
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                startDate = DateTime.Parse(start);
                endDate = DateTime.Parse(end);
                list = list.Where(x => x.StartDate >= startDate && x.StartDate <= endDate).ToList();
            }
 
            //过期
            if (isOverdue=="true")
            {
                list = list.Where(x => x.EndDate < DateTime.Now).ToList();
            }
            else if(isOverdue=="false")
            {
                list = list.Where(x => x.EndDate >= DateTime.Now).ToList();
            }

            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = configmanager.GetProcessSetByFullName(this.CurrentUser, item.FullName),
                    Reason = item.Reason
                });
            }
            if (!string.IsNullOrEmpty(delegateTo))
            {
                delegateItems = delegateItems.Where(x => x.ToUser.Contains(delegateTo)).ToList();
            }

            return Json(delegateItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelegationById(int delegateId)
        {
            Delegation delinfo=_delegationservice.GetDelegation(delegateId);
            if (delinfo != null)
            {
                List<string> fromusers=new List<string>();
                fromusers.Add(delinfo.FromUser);

                DelegationView delegation = new DelegationView()
                     {
                         ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(delinfo.ToUsers))),
                         FromUser = userService.ReadUserBase(GetUserWithOutLabel(delinfo.FromUser)).FullName,
                         IsEnable = delinfo.IsEnable,
                         DelegationID = delinfo.ID,
                         StartDate = delinfo.StartDate,
                         EndDate = delinfo.EndDate,
                         FullName = delinfo.FullName,
                         Reason = delinfo.Reason,
                         ToUserName = userService.GetUsersInfoByUserName(GetUserWithOutLabel(delinfo.ToUsers)),
                         FromUserName = userService.GetUsersInfoByUserName(GetUserWithOutLabel(fromusers))
                     };
                return Json(delegation, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  失效委托   --批量
        /// <summary>
        /// 失效委托   --批量
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDisableDelegation(List<int> idList, bool status)
        {
            foreach (var id in idList)
            {
                _delegationservice.DeleteDelegation(id);                
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加委托   --批量
        /// <summary>
        /// 添加委托   --批量
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoCreateDelegation(string pane, DelegationView model, List<string> delegatetoList, List<string> processList)
        {
            if (model.DelegationID > 0)
            {
                _delegationservice.DeleteDelegation(model.DelegationID);
            }
            List<Delegation> items = new List<Delegation>();
            foreach (var process in processList)
            {
                Delegation dele = new Delegation();
                dele.DeleType = Enum.GetName(typeof(DelegationType), DelegationType.Process);
                dele.FullName = process;
                dele.ActionType = Enum.GetName(typeof(ActionType), ActionType.Approval);
                dele.GroupID = Guid.NewGuid();
                dele.FromUser = _userName();
                dele.ToUsers = GetUserWithLabel(delegatetoList);//
                dele.StartDate = model.StartDate;
                dele.EndDate = model.EndDate;
                dele.Status = Enum.GetName(typeof(DelegationStatus), DelegationStatus.Available);
                dele.ActivityName = string.Empty;
                dele.Reason = model.Reason;
                dele.CreatedBy = _userName();
                dele.CreatedDate = DateTime.Now;
                dele.UpdatedBy = _userName();
                dele.UpdatedDate = DateTime.Now;
                dele.WorkTypeID = Guid.NewGuid().ToString();
                dele.IsEnable = true;
                dele.DeleInstCount = 0;
                dele.TenantID = pane;

                _delegationservice.SaveDelegation(dele);
                items.Add(dele);
            }



            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in items)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }


            return Json(delegateItems, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public JsonResult GetAdminDelegation()
        {
            //var list = _delegationservice.GetUserDelegations(new Framework.ServiceContext(), _userName);
            List<DataFieldFilter> dataFieldFilter = new List<DataFieldFilter>();
            //dataFieldFilter.Add(new DataFieldFilter() { FieldName = "Process", Compare=CriteriaCompare.In });
            var list = _delegationservice.GetAllDelegations(
                new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue, IsFirstQuery = true, PageIndex = 0 });

            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }
            return Json(delegateItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdminDelegations(int take = 15, int skip = 0, int page = 1, int pageSize = 15,
            [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            //var list = _delegationservice.GetUserDelegations(new Framework.ServiceContext(), _userName);
            List<DataFieldFilter> dataFieldFilter = new List<DataFieldFilter>();
            //dataFieldFilter.Add(new DataFieldFilter() { FieldName = "Process", Compare=CriteriaCompare.In });
            PageCriteria pageCriteria = new PageCriteria() { PageSize = pageSize, IsFirstQuery = true, PageIndex = page - 1 };
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });
            foreach (var item in sort)
            {
                pageCriteria.AddSortFilter(new SortFilter() { FieldName = item.field, SortDirection = item.dir });
            }
            var list = _delegationservice.GetAllDelegations(pageCriteria);

            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = item.FullName,
                    Reason = item.Reason
                });
            }
            return Json(new { total = pageCriteria.TotalCount, data = delegateItems }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FindAdminDelegations(string start, string end, string delegateFrom, string delegateTo, string isOverdue)
        {            
            List<DataFieldFilter> dataFieldFilter = new List<DataFieldFilter>();
            PageCriteria pageCriteria = new PageCriteria() { PageSize = int.MaxValue, IsFirstQuery = true, PageIndex = 0 };
            pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });
            DateTime? startDate = null, endDate = null;
            if (!string.IsNullOrEmpty(delegateFrom))
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Like, FieldName = "FromUser", Value1 = GetUserWithLabel(delegateFrom), StartLogical = CriteriaLogical.And });
            }
            if (!string.IsNullOrEmpty(delegateTo))
            {
                pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Like, FieldName = "ToUser", Value1 = GetUserWithLabel(delegateTo), StartLogical = CriteriaLogical.And });
            }
            var list = _delegationservice.GetAllDelegations(pageCriteria);
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                startDate = DateTime.Parse(start);
                endDate = DateTime.Parse(end);
                list = list.Where(x => x.StartDate >= startDate && x.StartDate <= endDate).ToList();
            }

            //过期
            if (isOverdue == "true")
            {
                list = list.Where(x => x.EndDate < DateTime.Now).ToList();
            }
            else if (isOverdue == "false")
            {
                list = list.Where(x => x.EndDate >= DateTime.Now).ToList();
            }

            List<DelegationView> delegateItems = new List<DelegationView>();
            foreach (var item in list)
            {
                delegateItems.Add(new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(item.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(item.FromUser)).FullName,
                    IsEnable = item.IsEnable,
                    DelegationID = item.ID,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FullName = configmanager.GetProcessSetByFullName(this.CurrentUser, item.FullName),
                    Reason = item.Reason
                });
            }

            if (!string.IsNullOrEmpty(delegateFrom))
            {
                delegateItems = delegateItems.Where(x => x.FromUser.Contains(delegateFrom)).ToList();
            }
            if (!string.IsNullOrEmpty(delegateTo))
            {
                delegateItems = delegateItems.Where(x => x.ToUser.Contains(delegateTo)).ToList();
            }

            return Json(delegateItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DoDisableAdminDelegation(List<int> idList,bool status)
        {
            foreach (var id in idList)
            {
                _delegationservice.DeleteDelegation(id);                
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DoCreateAdminDelegation(string pane, DelegationView model, List<string> delegatetoList, List<string> processList)
        {
            if (model.DelegationID > 0)
            {
                _delegationservice.DeleteDelegation(model.DelegationID);
            }
            List<Delegation> items = new List<Delegation>();
            foreach (var process in processList)
            {
                Delegation dele = new Delegation();
                dele.DeleType = Enum.GetName(typeof(DelegationType), DelegationType.Process);
                dele.FullName = process;
                dele.ActionType = Enum.GetName(typeof(ActionType), ActionType.Approval);
                dele.GroupID = Guid.NewGuid();
                dele.FromUser = GetUserWithLabel(model.FromUser);
                dele.ToUsers = GetUserWithLabel(delegatetoList);////
                dele.StartDate = model.StartDate;
                dele.EndDate = model.EndDate;
                dele.Status = Enum.GetName(typeof(DelegationStatus), DelegationStatus.Available);
                dele.ActivityName = string.Empty;
                dele.Reason = model.Reason;
                dele.CreatedBy = _userName();
                dele.CreatedDate = DateTime.Now;
                dele.UpdatedBy = _userName();
                dele.UpdatedDate = DateTime.Now;
                dele.WorkTypeID = Guid.NewGuid().ToString();
                dele.IsEnable = true;
                dele.DeleInstCount = 0;
                dele.TenantID = pane;

                _delegationservice.SaveDelegation(dele);
                items.Add(dele);
            }

            return Json(items.Select(s =>
                new DelegationView()
                {
                    ToUser = string.Join(",", userService.GetUsersDisplayNameByUserName(GetUserWithOutLabel(s.ToUsers))),
                    FromUser = userService.ReadUserBase(GetUserWithOutLabel(s.FromUser)).FullName,
                    IsEnable = s.IsEnable,
                    DelegationID = s.ID,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    FullName = s.FullName,
                    Reason = s.Reason
                }), JsonRequestBehavior.AllowGet);
        }

    }
}
