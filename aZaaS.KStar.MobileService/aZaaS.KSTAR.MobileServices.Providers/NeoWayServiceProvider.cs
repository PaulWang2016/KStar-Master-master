using aZaaS.Framework;
using aZaaS.Framework.Authentication;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.KSTAR.MobileServices.Helpers;
using aZaaS.KSTAR.MobileServices.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    public class NeoWayServiceProvider : BaseServiceProvider
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
        private const string COMMON_DATAFIELD_TASK_ACTIONTAKER = "_TASK_ACTIONTAKER";
        private const string COMMON_DATAFIELD_TASK_ACTIONCOMMENT = "_TASK_ACTIONCOMMENT";
         
        private AuthenticationType _authMode;
        private BaseDataProvider _dataProvider;
        private string _domain;
        private string _label;
        private string OnWorkflowTaskExecuting = "/api/NeoWay_MobileTask/OnWorkflowTaskExecuting";
        private string OnWorkflowTaskExecuted = "/api/NeoWay_MobileTask/OnWorkflowTaskExecuted";

        public NeoWayServiceProvider()
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
                var loginResult = authManager.Verify(info.UserName, info.Password);
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
            List<WorklistItem> wiList = this.GetWorklistItems(facade, context);
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


                if (string.IsNullOrWhiteSpace(item.Format) == false)
                {
                    item.Value = DateConvert(item.Value, item.Format);
                }

                list.Add(item);
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
            //获取对应的 WorklistItem
            if (string.IsNullOrEmpty(destination) || destination.ToLower() == (_label + userName).ToLower())
            {
                wiItem = facade.OpenWorklistItem(context, sn);
            }
            else
            {
                wiItem = facade.OpenWorklistItem(context, destination, sn, PlatformType.ASP, true);
            }
            //找到对应流程的配置信息
            var taskInfoDefinition = _dataProvider.GetGroupDefinitionExtend(wiItem.ProcessInstance.FullName, TASKINFONAME, language);
            //procbaseinfo 的配置信息
            var baseInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCBASEINFONAME);
            //procloginfo 的配置信息
            var logInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == PROCLOGINFONAME);
            //bizInfo  的配置信息
            var bizInfo = taskInfoDefinition.GroupList.SingleOrDefault(r => r.Name.ToLower() == BIZINFONAME);
            //获取bizInfo的数据源
            string dataSource = bizInfo != null ? getFormXmlDate(wiItem.ProcInstID) : string.Empty;
            //添加Biz 信息到数据
            foreach (var subGroup in bizInfo.GroupList)
            {
                info.BizInfo.Groups.Add(GetXmlItemsValueFromDB(subGroup, dataSource));
            }

            ProcessFormHeader dataSourceProcBaseInfo = getFormHeadXmlDate(wiItem.ProcInstID);
            dataSourceProcBaseInfo = dataSourceProcBaseInfo == null ? new ProcessFormHeader() : dataSourceProcBaseInfo;

            GroupExtend _baseInfo = new GroupExtend();
            _baseInfo.Collapsed = baseInfo.Collapsed;
            _baseInfo.ConnectionString = baseInfo.ConnectionString;
            _baseInfo.ID = baseInfo.ID;
            _baseInfo.LabelID = baseInfo.LabelID;
            _baseInfo.Name = baseInfo.Name;
            _baseInfo.Type = baseInfo.Type;
            _baseInfo.ItemList = new List<ItemExtend>();
      
            //过滤出ProcessFormHeader 没有的属性在WorklistItem 查找
            foreach (var item in baseInfo.ItemList)
            {
              var property=  dataSourceProcBaseInfo.GetType().GetProperty(item.Name);
               if(property==null){
                   //WorklistItem 中 判断是否有值
                   if (wiItem.GetType().GetProperty(item.Name) != null)
                   {
                       _baseInfo.ItemList.Add(item);
                   } 
               } 
            }
            foreach (var item in _baseInfo.ItemList)
            {
                baseInfo.ItemList.Remove(item);
            } 
            //添加pro 信息到pro
            info.ProcBaseInfo.Group = GetItemsValueFromWorklistItem(dataSourceProcBaseInfo, baseInfo, null);

            if (_baseInfo.ItemList.Count > 0)
            {
                Group _baseInfoGroup = GetItemsValueFromWorklistItem(wiItem, _baseInfo, null);
                foreach (var item in _baseInfoGroup.Items)
                {
                    info.ProcBaseInfo.Group.Items.Insert(0, item);
                }
            }  
            // info.ProcBaseInfo.Group.Items
            var logList = new ProcessLogFacade().GetProcessLogByProcInstID(context, wiItem.ProcInstID);
            if (logList.Count > 0) 
                    info.ProcLogInfo.Group = GetItemsValueFromWorklistItem(null, logInfo, logList);
               
            else
                info.ProcLogInfo.Group = GetItemsValueFromWorklistItem(null, logInfo, logList);

            info.Actions = new Models.Actions() { Items = new List<Item>() };
            foreach (var action in wiItem.Actions)
            {
                if (action.Name.Trim() != "加签")
                    info.Actions.Items.Add(
                        new Item() { Name = action.Name, Value = action.Name }
                    );
            }
            return info;
        }

        public override ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion, string destination)
        { 
            if ((action + string.Empty).Trim() == "不同意" && string.IsNullOrEmpty(opinion))
            {
                throw new Exception("请在审批意见中中输入\"不同意\"的原因!");
            }

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

            #region   DataField
            List<DataField> dataField = new List<DataField>();
            DataField field = new DataField();
            field.Name = COMMON_DATAFIELD_TASK_ACTIONCOMMENT;
            if (string.IsNullOrEmpty(opinion))
            {
                field.Value = ConfigurationManager.AppSettings["DefaultApprovalComment"].ToString();
            }
            else
            {
                field.Value = opinion;
            }
            dataField.Add(field);

            field = new DataField();
            field.Name = COMMON_DATAFIELD_TASK_ACTIONTAKER;
            field.Value = userName;
            dataField.Add(field);
            #endregion

            WorklistItem worklistItem = facade.OpenWorklistItem(context, sn);

            //OnWorkflowTaskExecuting
            MobileTaskDataField taskDataField = new MobileTaskDataField();
            taskDataField.UserName = userName;
            taskDataField.SN = sn;
            taskDataField.ActivityName = worklistItem.ActivityName;
            taskDataField.FullName = worklistItem.FullName;
            taskDataField.ProcInstID = worklistItem.ProcInstID;
            taskDataField.Folio = worklistItem.Folio; 
            // OnWorkflowTaskExecuting 事前触发
            string param = Newtonsoft.Json.JsonConvert.SerializeObject(taskDataField, Newtonsoft.Json.Formatting.None);

            string postResultJosn = PostWebRequest(OnWorkflowTaskExecuting, param, System.Text.Encoding.UTF8);
            PostResult postResult = Newtonsoft.Json.JsonConvert.DeserializeObject<PostResult>(postResultJosn);

            if (postResult.succeed)
            {
                //执行ExecuteAction
                if (string.IsNullOrEmpty(destination) || destination.ToLower() == (_label + userName).ToLower())
                {
                    facade.ExecuteAction(context, sn, new List<DataField>(), dataField, action, opinion, true);
                }
                else
                {
                    facade.ExecuteAction(context, sn, destination, new List<DataField>(), dataField, action, opinion, true);
                }
                // OnWorkflowTaskExecuted 事后触发
                postResultJosn = PostWebRequest(OnWorkflowTaskExecuted, param, System.Text.Encoding.UTF8);
                postResult = Newtonsoft.Json.JsonConvert.DeserializeObject<PostResult>(postResultJosn);
                if (postResult.succeed)
                {
                    result.Result = "S";
                    result.Message = _dataProvider.GetLabelContent(new Guid(EXECUTE_ACTION_SUCCESS), language);
                    return result;
                }
                else
                {
                    throw new Exception(postResult.message);
                }
            }
            else
            {
                throw new Exception(postResult.message);
            }

        }


        public override VersionSetting GetVersion(string version)
        {

            VersionSetting setting = new VersionSetting();
            string _version = ConfigurationManager.AppSettings["CurrenVersion"];
            setting.Version = _version;
            if (_version != version)
            {
                setting.Version = _version;
                string _LogoByte64 = ConfigurationManager.AppSettings["LogoByte64"];
                if (File.Exists(_LogoByte64))
                {
                    byte[] bytes = File.ReadAllBytes(_LogoByte64);
                    string base64String = Convert.ToBase64String(bytes);
                    setting.LogoByte64 = base64String;
                }
                else
                {
                    setting.LogoByte64 =null;
                } 

                string _InterfaceByte64 = ConfigurationManager.AppSettings["InterfaceByte64"];

                if (File.Exists(_InterfaceByte64))
                {
                    byte[] bytes = File.ReadAllBytes(_InterfaceByte64);
                    string base64String = Convert.ToBase64String(bytes);
                    setting.InterfaceByte64 = base64String;
                }
                else
                {
                    setting.InterfaceByte64 = null;
                } 
            }
            return setting;
        }

        public override string GetAppIco()
        {
            string version = ConfigurationManager.AppSettings["AppIco"];

            return version;
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objItem"></param>
        /// <param name="groupExt"></param>
        /// <param name="dataList"></param>
        /// <returns></returns>
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
                            Item _item = item.ConvertToItem();
                            if (string.IsNullOrWhiteSpace(_item.Format) == false)
                            {
                                _item.Value = DateConvert(_item.Value, _item.Format);
                            }
                            group.Header.Items.Add(_item);
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
                            if (!itemExt.Visible) continue;
                            var fieldName = string.IsNullOrEmpty(itemExt.Mapping) ? itemExt.Name : itemExt.Mapping;
                            Item item = itemExt.ConvertToItem();
                            item.Value = getSingleXmlItemsValue(dataSource, item.Name);
                            if (string.IsNullOrWhiteSpace(itemExt.Format) ==false)
                            {
                                item.Value = DateConvert(item.Value, item.Format);
                            }
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
                            if (!item.Visible) continue;
                            Item _item = item.ConvertToItem();
                            if (string.IsNullOrWhiteSpace(_item.Format) == false)
                            {
                                _item.Value = DateConvert(_item.Value, _item.Format);
                            }
                            group.Header.Items.Add(_item);
                        }
                    }
                    var dataRow = groupExt.GroupList.SingleOrDefault(r => r.Name.ToLower() == ROWNAME);
                     
                    if (dataRow != null)
                    {
                        var rowData = dataRow.GroupList.SingleOrDefault(r => r.Name.ToLower() == MORENAME);

                        List<Row> rowList = new List<Row>();

                        //row
                        foreach (var item in group.Header.Items)
                        {
                            int nodeIndex = 0;
                            XmlNodeList nodeList = getTableXmlItemsValue(dataSource, item.Name);
                            foreach (XmlNode node in nodeList)
                            {
                                if (rowList.Count <= nodeIndex)
                                {
                                    Row row = new Row() { Data = new Data() { Items = new List<Item>() }, More = new More() { Items = new List<Item>() } };
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
                                 
                                if (string.IsNullOrWhiteSpace(_item.Format) == false)
                                {
                                    _item.Value = DateConvert(_item.Value, _item.Format);
                                }

                                rowList[nodeIndex].Data.Items.Add(_item);

                                nodeIndex++;
                            }
                        }
                        //more
                        foreach (var itemExtend in rowData.ItemList)
                        {
                            if (!itemExtend.Visible) continue;
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
                                if (string.IsNullOrWhiteSpace(_item.Format) == false)
                                {
                                    _item.Value = DateConvert(_item.Value, _item.Format);
                                }
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

        /// <summary>
        /// 获取代办列表
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<WorklistItem> GetWorklistItems(WFClientFacade facade, ServiceContext context)
        {
            List<WorklistItem> wiList = facade.GetWorklistItems(context, PlatformType.ASP);
            //过滤后的workListItem
            List<WorklistItem> filterList = new List<WorklistItem>();

            using (KSTARServiceDbContext dbContext = new KSTARServiceDbContext())
            {
                List<aZaaS.KSTAR.MobileServices.Models.Definitions.ProcessPermission> processPermissionList = dbContext.ProcessPermissionSet.ToList();

                using (FrameworkDbContext _dbContext = new FrameworkDbContext())
                {
                    foreach (WorklistItem item in wiList)
                    {
                        var _processPermissionList = processPermissionList.Where(x => x.ProcessFullName == item.FullName && x.ActivityName == item.ActivityName);
                        if (_processPermissionList.Count() > 0)
                        {
                            List<string> userList = _dbContext.Database.SqlQuery<string>("select [FirstName] from [User] where [UserName]='" + item.Originator + "'").ToList();

                            if (userList != null && userList.Count > 0)
                                item.Originator = userList[0];
                            filterList.Add(item);
                        }
                    }
                }
            }
            return filterList;
        }
         
        /// <summary>
        /// 获取表单内容的xml 数据
        /// </summary>
        /// <param name="procInstID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取表单head内容的xml 数据
        /// </summary>
        /// <param name="procInstID"></param>
        /// <returns></returns>
        private ProcessFormHeader getFormHeadXmlDate(int? procInstID)
        {
            ProcessFormHeader processFormHeader = null;
            using (FrameworkDbContext dbContext = new FrameworkDbContext())
            {
                processFormHeader= dbContext.ProcessFormHeaderSet.Where(x => x.ProcInstID == procInstID).FirstOrDefault();

            }
            return processFormHeader;
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

        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                string service = ConfigurationManager.AppSettings["webapi"];

                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(service+postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/json;charset=UTF-8";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(),dataEncode==null? Encoding.Default:dataEncode);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }
     
        public override void WriteServiceLog(LogEntity log)
        {
            //base.WriteServiceLog(log);
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <returns></returns>
        public string DateConvert(object date,string format)
        {
            try
            {
                if (date is DateTime || DateTime.Parse(date + string.Empty) != null)
                {
                    DateTime dateTime = date is DateTime ? (DateTime)date : DateTime.Parse(date + string.Empty);
                    //默认24小时 HH 
                    return string.IsNullOrWhiteSpace(format) ? dateTime.ToString("yyyy-MM-dd HH:mm:ss") : dateTime.ToString(format);
                }
                return date + string.Empty; ;
              
            }
            catch (Exception ex)
            {
                return date+string.Empty;
            }
        }
 } 
    public class MobileTaskDataField
    {
        public string UserName { set; get; }
        public string SN { set; get; }

        public string Folio { set; get; }

        public int ProcInstID { set; get; }

        public string FullName { set; get; }

        public string ActivityName { set; get; }
    } 
    public class PostResult
    {
        public bool succeed { set; get; }

        public string message { set; get; }

    }
}
