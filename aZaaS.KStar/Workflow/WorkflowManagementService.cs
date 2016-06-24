using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Facades;

namespace aZaaS.KStar 
{
   public  class WorkflowManagementService
    {
		private readonly WFManagementFacade wfManagementFacade;

        public WorkflowManagementService(AuthenticationType authType)
		{
            this.wfManagementFacade = new WFManagementFacade(authType);
		}

		
		
		public List<WorklistItem> GetWorklistItems( PlatformType platform,WorklistItemCriteria criteria)
		{

			return this.wfManagementFacade.GetWorklistItems(new ServiceContext(), platform, criteria);
		}
		
		///<summary>
        ///获取与工作项关联的业务数据
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="item"></param>
        ///<returns></returns>
		public IEnumerable<BusinessDataItem> GetBusinessData(WorklistItem item)
		{

			return this.wfManagementFacade.GetBusinessData(new ServiceContext(), item);
		}
		
		///<summary>
        ///获取单个流程实例 (aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
		public ProcessInstance GetProcessInstance( int procInstID)
		{

			return this.wfManagementFacade.GetProcessInstance(new ServiceContext(), procInstID);
		}
		
		///<summary>
        ///获取某个流程的流程实例列表(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="fullName"></param>
        ///<returns></returns>
		public List<ProcessInstance> GetProcessInstancesByFullName( string fullName)
		{

			return this.wfManagementFacade.GetProcessInstancesByFullName(new ServiceContext(), fullName);
		}
		
		
		public List<ProcessInstance> GetProcessInstances( ProcessInstanceCriteria criteria)
		{

			return this.wfManagementFacade.GetProcessInstances(new ServiceContext(), criteria);
		}
		
		///<summary>
        ///获取流程查看的XML(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
		public string GetViewFlowXml( int procInstID)
		{

			return this.wfManagementFacade.GetViewFlowXml(new ServiceContext(), procInstID);
		}
		
		///<summary>
        ///删除一个流程定义(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="fullName"></param>
        ///<param name="version"></param>
        ///<param name="deleteLog"></param>
		public void DeleteProcessDefinition( string fullName, int version, bool deleteLog)
		{

			 this.wfManagementFacade.DeleteProcessDefinition(new ServiceContext(), fullName, version, deleteLog);
		}
		
		///<summary>
        ///删除一个流程实例(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="deleteLog"></param>
		public void DeleteProcessInstance( int procInstID, bool deleteLog)
		{

			 this.wfManagementFacade.DeleteProcessInstance(new ServiceContext(), procInstID, deleteLog);
		}
		
		///<summary>
        ///停止一个流程实例(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
		public void StopProcessInstance( int procInstID)
		{

			 this.wfManagementFacade.StopProcessInstance(new ServiceContext(), procInstID);
		}
		
		///<summary>
        ///开始\恢复一个流程实例(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
		public void StartProcessInstance( int procInstID)
		{

			 this.wfManagementFacade.StartProcessInstance(new ServiceContext(), procInstID);
		}
		
		///<summary>
        ///将流程实例跳转到指定关卡(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="activityName"></param>
        ///<param name="expireAll"></param>
		public void GotoActivity( int procInstID, string activityName, bool expireAll)
		{

			 this.wfManagementFacade.GotoActivity(new ServiceContext(), procInstID, activityName, expireAll);
		}

        public void CancelActivity(int procInstID)
        {
            this.wfManagementFacade.CancelActivity(new ServiceContext(), procInstID, PortalEnvironment.CancelActivityName);
        }
		
		///<summary>
        ///转派一个工作项(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="sn"></param>
        ///<param name="toUserName"></param>
        ///<param name="sendNotification"></param>
		public void RedirectWorklistItem( string sn, string toUserName, bool sendNotification)
		{

			 this.wfManagementFacade.RedirectWorklistItem(new ServiceContext(), sn, toUserName, sendNotification);
		}
		
		
		public void RedirectWorklistItemByID( Int64 worklistItemID, string toUserName, bool sendNotification)
		{

			 this.wfManagementFacade.RedirectWorklistItemByID(new ServiceContext(), worklistItemID, toUserName, sendNotification);
		}
		
		
		public void ReleaseWorklistItem( Int64 worklistItemID)
		{

			 this.wfManagementFacade.ReleaseWorklistItem(new ServiceContext(), worklistItemID);
		}
		
		
		public List<ErrorLog> GetErrorLogs( PageCriteria criteria)
		{

			return this.wfManagementFacade.GetErrorLogs(new ServiceContext(), criteria);
		}
		
		///<summary>
        ///重试修复流程错误(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="errorID"></param>
        ///<returns></returns>
		public bool RetryError( int procInstID, int errorID)
		{

			return this.wfManagementFacade.RetryError(new ServiceContext(), procInstID, errorID);
		}
		
		
		public void DeleteErrorLogs( List<Int32> errorIDs)
		{

			 this.wfManagementFacade.DeleteErrorLogs(new ServiceContext(), errorIDs);
		}
		
		///<summary>
        ///获取流程分类(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="categoryID"></param>
        ///<returns></returns>
		public ProcessCategory GetProcessCategory( int categoryID)
		{

			return this.wfManagementFacade.GetProcessCategory(new ServiceContext(), categoryID);
		}
		
		
		public List<ProcessCategory> GetProcessCategories( PageCriteria criteria)
		{

			return this.wfManagementFacade.GetProcessCategories(new ServiceContext(), criteria);
		}
		
		///<summary>
        ///保存流程分类信息(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procCategory"></param>
        ///<returns></returns>
		public int SaveProcessCategory( ProcessCategory procCategory)
		{

			return this.wfManagementFacade.SaveProcessCategory(new ServiceContext(), procCategory);
		}
		
		
		public void DeleteProcessCategorys( List<Int32> categoryIDs)
		{

			 this.wfManagementFacade.DeleteProcessCategorys(new ServiceContext(), categoryIDs);
		}
		
		
		public List<ProcessSet> GetProcessSets( PageCriteria criteria)
		{

			return this.wfManagementFacade.GetProcessSets(new ServiceContext(), criteria);
		}

        public List<ProcessSet> GetProcessSets()
        {
            return this.wfManagementFacade.GetProcessSets(new ServiceContext());
        }
		

		///<summary>
        ///更新流程设置信息(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procSet"></param>
		public void UpdateProcessSet( ProcessSet procSet)
		{

			 this.wfManagementFacade.UpdateProcessSet(new ServiceContext(), procSet);
		}
		
		
		public List<ProcessVersion> GetProcessVersions( int procSetID, PageCriteria criteria)
		{

			return this.wfManagementFacade.GetProcessVersions(new ServiceContext(), procSetID, criteria);
		}
		
		///<summary>
        ///获取流程关卡列表(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procID">流程版本ID</param>
        ///<returns></returns>
		public List<Activity> GetProcessActivities( int procID)
		{

			return this.wfManagementFacade.GetProcessActivities(new ServiceContext(), procID);
		}
		
		///<summary>
        ///获取流程普通(审批人)关卡列表(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procSetID"></param>
        ///<returns></returns>
		public List<Activity> GetProcessManualActivities( int procSetID)
		{

			return this.wfManagementFacade.GetProcessManualActivities(new ServiceContext(), procSetID);
		}
		
		///<summary>
        ///获取关卡审批人设置(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="id"></param>
        ///<returns></returns>
		public ActDestination GetActDestination( int id)
		{

			return this.wfManagementFacade.GetActDestination(new ServiceContext(), id);
		}
		
		///<summary>
        ///获取关卡审批人设置列表(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="actID"></param>
        ///<returns></returns>
		public List<ActDestination> GetActDestinations( int actID)
		{

			return this.wfManagementFacade.GetActDestinations(new ServiceContext(), actID);
		}
		
		///<summary>
        ///保存关卡审批人设置(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="actDest"></param>
        ///<returns></returns>
		public int SaveActDestination( ActDestination actDest)
		{

			return this.wfManagementFacade.SaveActDestination(new ServiceContext(), actDest);
		}
		
		
		public void DeleteActDestinations( List<Int32> idList)
		{

			 this.wfManagementFacade.DeleteActDestinations(new ServiceContext(), idList);
		}
		
		///<summary>
        ///新增一个StringTable设置 (aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="environment"></param>
        ///<param name="name"></param>
        ///<param name="value"></param>
        ///<returns></returns>
		public bool AddStringTable( string environment, string name, string value)
		{

			return this.wfManagementFacade.AddStringTable(new ServiceContext(), environment, name, value);
		}
		
		///<summary>
        ///更新一个StringTable设置(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="environment"></param>
        ///<param name="name"></param>
        ///<param name="value"></param>
		public void UpdateStringTable( string environment, string name, string value)
		{

			 this.wfManagementFacade.UpdateStringTable(new ServiceContext(), environment, name, value);
		}
		
		///<summary>
        ///删除一个StringTable设置(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="environment"></param>
        ///<param name="name"></param>
		public void DeleteStringTable( string environment, string name)
		{

			 this.wfManagementFacade.DeleteStringTable(new ServiceContext(), environment, name);
		}
		
		
		public List<StringTable> GetStringTables( string environment, PageCriteria criteria)
		{

			return this.wfManagementFacade.GetStringTables(new ServiceContext(), environment,  criteria);
		}
		
		///<summary>
        ///获取流程实例的DataField值(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="fieldName"></param>
        ///<returns></returns>
		public Object GetProcInstDataFieldValue( int procInstID, string fieldName)
		{

			return this.wfManagementFacade.GetProcInstDataFieldValue(new ServiceContext(), procInstID, fieldName);
		}
		
		///<summary>
        ///获取流程实例的DataField字典(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
		public Dictionary<string,DataField> GetProcInstDataFields( int procInstID)
		{

			return this.wfManagementFacade.GetProcInstDataFields(new ServiceContext(), procInstID);
		}
		
		
		public void UpdateProcInstDataFields( int procInstID, List<DataField> dataFields)
		{

			 this.wfManagementFacade.UpdateProcInstDataFields(new ServiceContext(), procInstID, dataFields);
		}
		
		///<summary>
        ///更新流程实例的DataField值（aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="fieldName"></param>
        ///<param name="fieldValue"></param>
		public void UpdateProcInstDataFieldValue( int procInstID, string fieldName, Object fieldValue)
		{

			 this.wfManagementFacade.UpdateProcInstDataFieldValue(new ServiceContext(), procInstID, fieldName, fieldValue);
		}
		
		///<summary>
        ///获取单个流程权限设置 (aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="id"></param>
        ///<returns></returns>
		public ProcessPermission GetProcessPermission( int id)
		{

			return this.wfManagementFacade.GetProcessPermission(new ServiceContext(), id);
		}
		
		
		public List<ProcessPermission> GetProcessPermissions( PageCriteria criteria)
		{

			return this.wfManagementFacade.GetProcessPermissions(new ServiceContext(), criteria);
		}
		
		///<summary>
        ///获取流程权限设置列表(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procSetID"></param>
        ///<returns></returns>
		public List<ProcessPermission> GetProcessPermissions( int procSetID)
		{

			return this.wfManagementFacade.GetProcessPermissions(new ServiceContext(), procSetID);
		}
		
		
		public void SaveProcessPermissions( List<ProcessPermission> perms)
		{

			 this.wfManagementFacade.SaveProcessPermissions(new ServiceContext(), perms);
		}
		
		
		public void DeleteProcessPermissions( List<Int32> idList)
		{

			 this.wfManagementFacade.DeleteProcessPermissions(new ServiceContext(), idList);
		}
		
		///<summary>
        ///获取用户流程权限（aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="userName"></param>
        ///<returns></returns>
		public Dictionary<string,int> GetUserProcessPermissions( string userName)
		{

			return this.wfManagementFacade.GetUserProcessPermissions(new ServiceContext(), userName);
		}
		
		///<summary>
        ///判断用户是否有此流程权限(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="fullName"></param>
        ///<param name="userName"></param>
        ///<param name="permType"></param>
        ///<returns></returns>
		public bool HasProcessPermissionByFullName( string fullName, string userName, ProcessPermType permType)
		{

			return this.wfManagementFacade.HasProcessPermissionByFullName(new ServiceContext(), fullName, userName, permType);
		}
		
		///<summary>
        ///判断用户是否有此流程权限(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="procSetID"></param>
        ///<param name="userName"></param>
        ///<param name="permType"></param>
        ///<returns></returns>
		public bool HasProcessPermissionByProcSetID( int procSetID, string userName, ProcessPermType permType)
		{

			return this.wfManagementFacade.HasProcessPermissionByProcSetID(new ServiceContext(), procSetID, userName, permType);
		}
		
		///<summary>
        ///获取单个服务器权限(aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="id"></param>
        ///<returns></returns>
		public ServerPermission GetServerPermissionByID( int id)
		{

			return this.wfManagementFacade.GetServerPermissionByID(new ServiceContext(), id);
		}
		
		///<summary>
        ///获取用户的服务器权限(aZaaS,K2)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="id"></param>
        ///<returns></returns>
		public ServerPermission GetServerPermissionByUserName( string userName)
		{

			return this.wfManagementFacade.GetServerPermissionByUserName(new ServiceContext(), userName);
		}
		
		
		public List<ServerPermission> GetServerPermissions( PageCriteria criteria)
		{

			return this.wfManagementFacade.GetServerPermissions(new ServiceContext(), criteria);
		}
		
		
		public void SaveServerPermissions( List<ServerPermission> perms)
		{

			 this.wfManagementFacade.SaveServerPermissions(new ServiceContext(), perms);
		}
		
		
		public void DeleteServerPermissions( List<Int32> idList)
		{

			 this.wfManagementFacade.DeleteServerPermissions(new ServiceContext(), idList);
		}
		
		///<summary>
        ///根据流程定义Xml部署流程（aZaaS)
        ///</summary>
        ///<param name="usernew ServiceContext()"></param>
        ///<param name="categoryID"></param>
        ///<param name="procDefXml"></param>
        ///<returns></returns>
		public string DeployProcessByXml( int categoryID, string procDefXml)
		{

			return this.wfManagementFacade.DeployProcessByXml(new ServiceContext(), categoryID, procDefXml);
		}
    }
}
