using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KSTAR.MobileServices.Helpers;
using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Security.Authentication;
using System.Configuration;
using System.Xml;
using aZaaS.Framework.Authentication;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    class WFServiceProvider : BaseServiceProvider
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

        private BaseDataProvider _dataProvider;
        private string _domain;
        private string _label;
        private AuthenticationType _authMode;
        public WFServiceProvider()
        {
            this._label = ConfigurationManager.AppSettings[SettingVariable.SecurityLabelName.ToString()] + ":";
            this._domain = ConfigurationManager.AppSettings[SettingVariable.WindowDomain.ToString()] + "\\";
            this._dataProvider = BaseDataProvider.CreateInstance(this.DataProviderAssemblyName, this.DataProviderName);
            string authMode = ConfigurationManager.AppSettings["AuthenticationMode"];
            if (string.IsNullOrEmpty(authMode) || authMode.ToLower().ToString() == "windows")
            {
                this._authMode = AuthenticationType.Windows;
            }
            else
            {
                this._authMode = AuthenticationType.Form;
            }
        }

        public override LoginResult Login(LoginInfo info)
        {
            LoginResult result = new LoginResult();
            if (this._authMode == AuthenticationType.Windows)
            {
                var adLoginResult = ADHelper.LoginByAccount(info.UserName, info.Password);
                switch (adLoginResult)
                {
                    case ADHelper.LoginResult.LOGIN_USER_OK:
                        result.Result = "S";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_OK), info.Language);
                        result.Content = new ResultContent()
                        {
                            Mask = ApiHelper.Encode(DateTime.Now.ToString(), ApiHelper.DefaultMask),
                            UserInfo = new UserInfo() { UserName = info.UserName }
                        };
                        result.Content.UserToken = ApiHelper.Encode(info.UserName + OPERATORSTRING + info.Language, result.Content.Mask);
                        break;
                    case ADHelper.LoginResult.LOGIN_USER_ACCOUNT_INACTIVE:
                        result.Result = "E";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_ACCOUNT_INACTIVE), info.Language);
                        break;
                    case ADHelper.LoginResult.LOGIN_USER_DOESNT_EXIST:
                        result.Result = "E";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_DOESNT_EXIST), info.Language);
                        break;
                    case ADHelper.LoginResult.LOGIN_USER_PASSWORD_INCORRECT:
                        result.Result = "E";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_PASSWORD_INCORRECT), info.Language);
                        break;
                }
            }
            else
            {
                var authManager = new DefaultAuthentication(aZaaS.Framework.Configuration.Config.Default);
                var loginResult =authManager.Verify(info.UserName, info.Password);
                switch (loginResult)
                {
                    case VerifyResult.PassworkIncorrect:
                        result.Result = "E";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_PASSWORD_INCORRECT), info.Language);
                        break;
                    case VerifyResult.Successful:
                        result.Result = "S";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_OK), info.Language);
                        result.Content = new ResultContent()
                        {
                            Mask = ApiHelper.Encode(DateTime.Now.ToString(), ApiHelper.DefaultMask),
                            UserInfo = new UserInfo() { UserName = info.UserName }
                        };
                        break;
                    case VerifyResult.UserIsNotexist:
                        result.Result = "E";
                        result.Message = _dataProvider.GetLabelContent(new Guid(LOGIN_USER_DOESNT_EXIST), info.Language);
                        break;
                }
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
            if (!userName.StartsWith(_domain))
            {
                userName = _domain + userName;
            }
            context.UserName = userName;
            WFClientFacade facade = new WFClientFacade(this._authMode);
            List<WorklistItem> wiList = facade.GetWorklistItems(context, PlatformType.ASP);
            var wiType = typeof(WorklistItem);
            foreach (WorklistItem wiItem in wiList)
            {
                var taskDefinition = _dataProvider.GetGroupDefinitionExtend(wiItem.ProcessInstance.FullName, TASKNAME, language);
                if (taskDefinition == null)
                    continue;

                Models.Task task = new Task();
                var baseInfo = taskDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == BASEINFONAME);
                var extInfo = taskDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == EXTENDINFONAME);
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
                       // var group = GetItemsValueFromDB(wiItem, extInfo);
                        task.ExtendInfo.Items = new List<Item>(); // group.Items;
                    }
                }
                list.Add(task);
            }

            return list;
        }

        public override TaskInfo GetTaskInfo(string userToken, string mask, string sn, string destination)
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
            if (!userName.StartsWith(_domain))
            {
                userName = _domain + userName;
            }
            context.UserName = userName;
            context[SettingVariable.ConnectionString.ToString()] = ConfigurationManager.AppSettings[SettingVariable.ConnectionString.ToString()];
            WFClientFacade facade = new WFClientFacade(this._authMode);
            WorklistItem wiItem = null;
            if (string.IsNullOrEmpty(destination) || destination.ToLower() == (_label + userName).ToLower())
            {
                wiItem = facade.OpenWorklistItem(context, sn);
            }
            else
            {
                wiItem = facade.OpenWorklistItem(context, destination, sn, PlatformType.ASP, true);
            }
            //var businessDataList = facade.GetBusinessData(context, wiItem);
            var taskInfoDefinition = _dataProvider.GetGroupDefinitionExtend(wiItem.ProcessInstance.FullName, TASKINFONAME, language);
            //if (_taskInfoDefinition == null)
            //    return info;

            var baseInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCBASEINFONAME);
            var logInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCLOGINFONAME);

            var bizInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == BIZINFONAME);
            string dataSource = bizInfo != null ? getFormXmlDate(wiItem.ProcInstID) : string.Empty; ;
            foreach (var subGroup in bizInfo.GroupList)
            {
                info.BizInfo.Groups.Add(GetXmlItemsValueFromDB(subGroup, dataSource));
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

        public override ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion, string destination)
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
            if (!userName.StartsWith(_domain))
            {
                userName = _domain + userName;
            }

            ServiceContext context = new ServiceContext();
            context.UserName = userName;
            WFClientFacade facade = new WFClientFacade(this._authMode);
            if (string.IsNullOrEmpty(destination) || destination.ToLower() == (_label + userName).ToLower())
            {
                facade.ExecuteAction(context, sn, new List<DataField>(), action, opinion, true);
            }
            else
            {
                facade.ExecuteAction(context, sn, destination, new List<DataField>(), action, opinion, true);
            }

            result.Result = "S";
            result.Message = _dataProvider.GetLabelContent(new Guid(EXECUTE_ACTION_SUCCESS), language);

            return result;
        }

        public override void WriteServiceLog(LogEntity log)
        {
            _dataProvider.WriteLog(log);
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
                item.Value = _dataProvider.GetPropertyValue(dataItem, fieldName);
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
                    _dataProvider.SetDataSourceList(groupExt, objItem, list);
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
                        _dataProvider.SetDataSourceList(rowExt, objItem, list);

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

        private Group GetXmlItemsValueFromDB(GroupExtend groupExt, string dataSource)
        {
            Group group = groupExt.ConvertToGroup();
            switch (groupExt.Type.ToLower())
            {
                case SINGLETYPENAME: 
                    if (groupExt.ItemList.Count > 0)
                    { 
                        foreach (var itemExt in groupExt.ItemList)
                        {
                            var fieldName = string.IsNullOrEmpty(itemExt.Mapping) ? itemExt.Name : itemExt.Mapping;
                            Item item = itemExt.ConvertToItem();
                            item.Value = getSingleXmlItemsValue(dataSource,item.Name);
                            group.Items.Add(item);
                        } 
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

                        List<Row> rowList = new List<Row>();
                      
                        //data表结构
                        foreach (var item in group.Header.Items)
                        {
                            int nodeIndex = 0;
                            XmlNodeList nodeList = getTableXmlItemsValue(dataSource, item.Name);
                            foreach (XmlNode node in nodeList)
                            {
                                if (rowList.Count <= nodeIndex)
                                {
                                    Row row = new Row() { Data = new Data() { Items = new List<Item>() }, More = new More() {  Items=new List<Item>()} };
                                    rowList.Add(row);
                                }
                                Item _item = new Item();
                                _item.Value = node.InnerText;
                                _item.Visible = item.Visible;
                                _item.Name = item.Name;
                                _item.Label = item.Label;
                                _item.Format = item.Format;
                                _item.Editable = item.Editable;
                                _item.DetailsUrl = item.DetailsUrl;
                                rowList[nodeIndex].Data.Items.Add(_item);

                                nodeIndex++;
                            }
                        }
                        //data表结构
                        foreach (var itemExtend in rowData.ItemList)
                        {
                            var item = itemExtend.ConvertToItem();

                            int nodeIndex = 0;
                            XmlNodeList nodeList = getTableXmlItemsValue(dataSource, item.Name);
                            foreach (XmlNode node in nodeList)
                            {
                                if (rowList.Count <= nodeIndex)
                                {
                                    Row row = new Row() { Data = new Data(), More = new More() };
                                    rowList.Add(row);
                                }
                                Item _item = new Item();
                                _item.Value = node.InnerText;
                                _item.Visible = item.Visible;
                                _item.Name = item.Name;
                                _item.Label = item.Label;
                                _item.Format = item.Format;
                                _item.Editable = item.Editable;
                                _item.DetailsUrl = item.DetailsUrl;
                                rowList[nodeIndex].More.Items.Add(_item); 
                                nodeIndex++;
                            }
                        } 
                        foreach (var dataItem in rowList)
                        {
                            group.Rows.Add(dataItem);
                        }
                    }

                    break;
            }
            return group;
        }

        private string getFormXmlDate(int? procInstID)
        {
          using (FrameworkDbContext dbContext = new FrameworkDbContext())
            {
                var linq = from h in dbContext.ProcessFormHeaderSet
                           join c in dbContext.ProcessFormContentSet on h.FormID equals c.FormID
                           into XmlData
                           from pro in XmlData.DefaultIfEmpty()
                           where h.ProcInstID == procInstID
                           select pro.XmlData;

                string xmlData = linq.FirstOrDefault(); 
                return xmlData; 
            }  
        }

        private string getSingleXmlItemsValue(string dataSource, string field)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(dataSource);
            XmlNodeList xmlNodeList = xDoc.SelectNodes("/ContentData/" + field);

            return xmlNodeList.Count > 0 ? xmlNodeList[0].InnerText : string.Empty;
        }

        private XmlNodeList getTableXmlItemsValue(string dataSource, string field)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(dataSource);
            XmlNodeList xmlNodeList = xDoc.SelectNodes("/ContentData/" + field);
            return xmlNodeList;
        }
    }
}