using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KSTAR.MobileServices.DataAccess;
using aZaaS.KSTAR.MobileServices.Helpers;
using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Security.Authentication;

namespace aZaaS.KSTAR.MobileServices
{
    public class WFDataProvider : BaseDataProvider
    {
        private const string TASKNAME = "task";
        private const string BASEINFONAME = "baseinfo";
        private const string EXTENDINFONAME = "extendinfo";
        private const string TASKINFONAME = "taskinfo";
        private const string PROCBASEINFONAME = "procbaseinfo";
        private const string BIZINFONAME = "bizinfo";
        private const string PROCLOGINFONAME = "procloginfo";
        private const string SINGLETYPENAME = "single";
        private const string TABLETYPENAME = "table";
        private const string HEADERNAME = "header";
        private const string ROWNAME = "row";
        private const string MORENAME = "more";
        private const string OPERATORSTRING = "&&";

        private GroupExtend _taskDefinition;
        private GroupExtend _taskInfoDefinition;

        public WFDataProvider()
        {
        }

        private void SetDefinitions(string processFullName, string language)
        {
            string taskDefinitionKey = "taskDefinition_" + processFullName + language;
            string taskInfoDefinitionKey = "taskInfoDefinition_" + processFullName + language;

            var cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) };

            if (!cache.Contains(taskDefinitionKey))
            {
                cache.Add(taskDefinitionKey, "", policy);
            }
            if (!cache.Contains(taskInfoDefinitionKey))
            {
                cache.Add(taskInfoDefinitionKey, "", policy);
            }

            if (cache[taskDefinitionKey] == "")
            {
                _taskDefinition = DataAccessManager.GetGroupDefinition(processFullName, TASKNAME, language);
                if (_taskDefinition != null)
                    cache[taskDefinitionKey] = _taskDefinition;
            }
            else
            {
                _taskDefinition = cache[taskDefinitionKey] as GroupExtend;
            }

            if (cache[taskInfoDefinitionKey] == "")
            {
                _taskInfoDefinition = DataAccessManager.GetGroupDefinition(processFullName, TASKINFONAME, language);
                if (_taskInfoDefinition != null)
                    cache[taskInfoDefinitionKey] = _taskInfoDefinition;
            }
            else
            {
                _taskInfoDefinition = cache[taskInfoDefinitionKey] as GroupExtend;
            }
        }

        public override LoginResult Login(LoginInfo info)
        {
            LoginResult result = new LoginResult();

            var adLoginResult = ADHelper.LoginByAccount(info.UserName, info.Password);
            switch (adLoginResult)
            {
                case ADHelper.LoginResult.LOGIN_USER_OK:
                    result.Result = "S";
                    result.Message = DataAccessManager.GetLabelContent(new Guid(LOGIN_USER_OK), info.Language);
                    result.Content = new ResultContent()
                    {
                        Mask = ApiHelper.Encode(DateTime.Now.ToString(), ApiHelper.DefaultMask),
                        UserInfo = new UserInfo() { UserName = info.UserName }
                    };
                    result.Content.UserToken = ApiHelper.Encode(info.UserName + OPERATORSTRING + info.Language, result.Content.Mask);
                    break;
                case ADHelper.LoginResult.LOGIN_USER_ACCOUNT_INACTIVE:
                    result.Result = "E";
                    result.Message = DataAccessManager.GetLabelContent(new Guid(LOGIN_USER_ACCOUNT_INACTIVE), info.Language);
                    break;
                case ADHelper.LoginResult.LOGIN_USER_DOESNT_EXIST:
                    result.Result = "E";
                    result.Message = DataAccessManager.GetLabelContent(new Guid(LOGIN_USER_DOESNT_EXIST), info.Language);
                    break;
                case ADHelper.LoginResult.LOGIN_USER_PASSWORD_INCORRECT:
                    result.Result = "E";
                    result.Message = DataAccessManager.GetLabelContent(new Guid(LOGIN_USER_PASSWORD_INCORRECT), info.Language);
                    break;
            }

            return result;
        }

        public override List<Task> GetTaskList(string userToken, string mask, Filter filter, Paging paging, Sorting sorting)
        {
            List<Task> list = new List<Task>();

            string tokenStartDate;
            string userInfo;
            string userName;
            string language;

            Helpers.ApiHelper.ParseUser(userToken, mask, out tokenStartDate, out userInfo);
            string[] arr = userInfo.Split(new string[] { OPERATORSTRING }, StringSplitOptions.None);
            userName = arr[0];
            language = arr[1];

            ServiceContext context = new ServiceContext();
            context.UserName = userName;
            WFClientFacade facade = new WFClientFacade(Framework.Workflow.AuthenticationType.Form);
            List<WorklistItem> wiList = facade.GetWorklistItems(context, PlatformType.ASP);
            var wiType = typeof(WorklistItem);
            foreach (WorklistItem wiItem in wiList)
            {
                SetDefinitions(wiItem.ProcessInstance.FullName, language);
                if (_taskDefinition == null)
                    continue;

                Models.Task task = new Task();
                var baseInfo = _taskDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == BASEINFONAME);
                var extInfo = _taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == EXTENDINFONAME);
                if (baseInfo != null)
                    task.BaseInfo.Items = GetItemsValueFromWorklistItem(wiItem, baseInfo.ItemList);
                if (extInfo != null)
                {
                    if (string.IsNullOrEmpty(extInfo.ConnectionString))
                    {
                        task.ExtendInfo.Items = GetItemsValueFromWorklistItem(wiItem, extInfo.ItemList);
                    }
                    else
                    {
                        var group = GetItemsValueFromDB(wiItem, extInfo);
                        task.ExtendInfo.Items = group.Items;
                    }
                }
                list.Add(task);
            }

            return list;
        }

        public override TaskInfo GetTaskInfo(string userToken, string mask, string sn)
        {
            TaskInfo info = new TaskInfo();

            string tokenStartDate;
            string userInfo;
            string userName;
            string language;

            Helpers.ApiHelper.ParseUser(userToken, mask, out tokenStartDate, out userInfo);
            string[] arr = userInfo.Split(new string[] { OPERATORSTRING }, StringSplitOptions.None);
            userName = arr[0];
            language = arr[1];

            ServiceContext context = new ServiceContext();
            context.UserName = userName;
            WFClientFacade facade = new WFClientFacade(Framework.Workflow.AuthenticationType.Form);
            WorklistItem wiItem = facade.OpenWorklistItem(context, sn);
            //var businessDataList = facade.GetBusinessData(context, wiItem);

            SetDefinitions(wiItem.ProcessInstance.FullName, language);
            //if (_taskInfoDefinition == null)
            //    return info;

            var baseInfo = _taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCBASEINFONAME);
            var logInfo = _taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCLOGINFONAME);

            var bizInfo = _taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == BIZINFONAME);
            foreach (var subGroup in bizInfo.GroupList)
            {
                info.BizInfo.Groups.Add(GetItemsValueFromDB(wiItem, subGroup));
            }

            info.ProcBaseInfo.Group = GetItemsValueFromWorklistItem(wiItem, baseInfo, null);
            if (string.IsNullOrEmpty(logInfo.ConnectionString))
            {
                var logList = new ProcessLogFacade().GetProcessLogByProcInstID(context, wiItem.ProcInstID);
                if (logList.Count > 0)
                    info.ProcLogInfo.Group = GetItemsValueFromWorklistItem(logList[0], logInfo, logList);
                else
                    info.ProcLogInfo.Group = GetItemsValueFromWorklistItem(null, logInfo, logList);
            }
            else
            {
                info.ProcLogInfo.Group = GetItemsValueFromDB(wiItem, logInfo);
            }

            info.Actions = new Models.Actions() { Items = new List<Item>() };
            foreach (var action in wiItem.Actions)
            {
                info.Actions.Items.Add(
                    new Item() { Name = action.Name, Value = action.Name }
                );
            }

            return info;
        }

        public override ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion)
        {
            ExecuteTaskResult result = new ExecuteTaskResult();
            string tokenStartDate;
            string userInfo;
            string userName;
            string language;

            Helpers.ApiHelper.ParseUser(userToken, mask, out tokenStartDate, out userInfo);
            string[] arr = userInfo.Split(new string[] { OPERATORSTRING }, StringSplitOptions.None);
            userName = arr[0];
            language = arr[1];

            ServiceContext context = new ServiceContext();
            context.UserName = userName;
            WFClientFacade facade = new WFClientFacade(Framework.Workflow.AuthenticationType.Form);

            facade.ExecuteAction(sn, new List<DataField>(), action, opinion, true);

            result.Result = "S";
            result.Message = DataAccessManager.GetLabelContent(new Guid(EXECUTE_ACTION_SUCCESS), language);

            return result;
        }

        private List<Item> GetItemsValueFromWorklistItem(object dataItem, List<ItemExtend> itemList)
        {
            List<Item> list = new List<Item>();
            if (dataItem == null)
                return list;
            foreach (var itemExt in itemList)
            {
                var fieldName = string.IsNullOrEmpty(itemExt.Mapping) ? itemExt.Name : itemExt.Mapping;
                Item item = itemExt.ConvertToItem();
                item.Value = DataAccessManager.GetPropertyValue(dataItem, fieldName);
                list.Add(item);
            }
            return list;
        }

        private List<Item> GetItemsValueFromDictionary(Dictionary<string, object> dataItem, List<ItemExtend> itemList)
        {
            List<Item> list = new List<Item>();
            foreach (var itemExt in itemList)
            {
                var fieldName = string.IsNullOrEmpty(itemExt.Mapping) ? itemExt.Name : itemExt.Mapping;
                Item item = itemExt.ConvertToItem();
                if (dataItem.ContainsKey(fieldName))
                    item.Value = dataItem[fieldName];
                list.Add(item);
            }
            return list;
        }

        private Group GetItemsValueFromWorklistItem(object objItem, GroupExtend groupExt, IEnumerable<object> dataList)
        {
            Group group = groupExt.ConvertToGroup();
            switch (groupExt.Type.ToLower())
            {
                case SINGLETYPENAME:
                    group.Items = GetItemsValueFromWorklistItem(objItem, groupExt.ItemList);
                    break;
                case TABLETYPENAME:
                    group.Header = new Header() { Items = new List<Item>() };
                    group.Rows = new List<Row>();
                    var header = groupExt.GroupList.SingleOrDefault(r => r.Name.ToLower() == HEADERNAME);
                    if (header != null)
                    {
                        foreach (var item in header.ItemList)
                        {
                            group.Header.Items.Add(item.ConvertToItem());
                        }
                    }

                    var dataRow = groupExt.GroupList.SingleOrDefault(r => r.Name.ToLower() == ROWNAME);
                    if (dataRow != null)
                    {
                        var rowData = dataRow.GroupList.SingleOrDefault(r => r.Name.ToLower() == MORENAME);
                        foreach (var dataItem in dataList)
                        {
                            Row row = new Row() { Data = new Data(), More = new More() };
                            row.Data.Items = GetItemsValueFromWorklistItem(dataItem, header.ItemList);
                            row.More.Items = GetItemsValueFromWorklistItem(dataItem, rowData.ItemList);
                            group.Rows.Add(row);
                        }
                    }
                    break;
            }
            return group;
        }

        private Group GetItemsValueFromDB(object objItem, GroupExtend groupExt)
        {
            Group group = groupExt.ConvertToGroup();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            switch (groupExt.Type.ToLower())
            {
                case SINGLETYPENAME:
                    DataAccessManager.SetDataSource(groupExt, objItem, list);
                    if (list.Count > 0)
                    {
                        group.Items = GetItemsValueFromDictionary(list[0], groupExt.ItemList);
                    }
                    break;
                case TABLETYPENAME:
                    group.Header = new Header() { Items = new List<Item>() };
                    group.Rows = new List<Row>();
                    var header = groupExt.GroupList.SingleOrDefault(r => r.Name.ToLower() == HEADERNAME);
                    if (header != null)
                    {
                        foreach (var item in header.ItemList)
                        {
                            group.Header.Items.Add(item.ConvertToItem());
                        }
                    }

                    var dataRow = groupExt.GroupList.SingleOrDefault(r => r.Name.ToLower() == ROWNAME);
                    if (dataRow != null)
                    {
                        var rowData = dataRow.GroupList.SingleOrDefault(r => r.Name.ToLower() == MORENAME);

                        //取Data Items and More Items
                        GroupExtend rowExt = new GroupExtend()
                        {
                            ConnectionString = groupExt.ConnectionString,
                            Mapping = groupExt.Mapping,
                            WhereString = groupExt.WhereString,
                            GroupList = new List<GroupExtend>(),
                            ItemList = new List<ItemExtend>()
                        };
                        rowExt.ItemList.AddRange(header.ItemList);
                        rowExt.ItemList.AddRange(rowData.ItemList);
                        DataAccessManager.SetDataSource(rowExt, objItem, list);

                        foreach (var dataItem in list)
                        {
                            Row row = new Row() { Data = new Data(), More = new More() };
                            row.Data.Items = GetItemsValueFromDictionary(dataItem, header.ItemList);
                            row.More.Items = GetItemsValueFromDictionary(dataItem, rowData.ItemList);
                            group.Rows.Add(row);
                        }
                    }
                    break;
            }
            return group;
        }
    }
}