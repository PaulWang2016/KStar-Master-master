using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using aZaaS.KStar;
using aZaaS.KStar.MgmtServices;
using aZaaS.Framework;
using aZaaS.Framework.Template;
using aZaaS.Framework.Extensions;
using aZaaS.Framework.Workflow;
using Client = SourceCode.Workflow.Client;
using Management = SourceCode.Workflow.Management;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;


namespace aZaaS.KStar.EmailExpression
{
    /// <summary>
    /// Worklist item expression model.
    /// Note:
    /// #WANRING#:HyperLink is not valid now,due to we can't OpenWorklistItem on error state to read WorklistItem.Data.
    /// We reading the fields infomation through smartobject api,
    /// that would be cause performance to slowly,but in order to implement this we choose this way first.
    /// </summary>
    public class WorklistItemBase
    {
        public WorklistItemBase() { }

        public WorklistItemBase(ServiceContext context)
        {
            this.SetProperties(context);
        }

        protected void SetProperties(ServiceContext context)
        {
            var itemUrl = context["ItemUrl"];
            string serialNumber = context["SerialNumber"];
            string destination = context["Destination"];
            var activityName = context["ActivityName"];

            if (serialNumber.NullOrEmpty())
                return;
            if (destination.NullOrEmpty())
                return;

            int procInstId = 0, actInstDestId = 0;
            var flags = serialNumber.Split(new char[] { '_' });
            if (flags.Length != 2)
                throw new InvalidCastException("SerialNumber");

            int.TryParse(flags[0], out procInstId);
            int.TryParse(flags[1], out actInstDestId);

            if (procInstId == 0 || actInstDestId == 0)
                throw new InvalidOperationException("Invalid SerialNumber");

            this.SerialNumber = serialNumber;
            this._Destination = destination;
            this.ActInstDestID = actInstDestId;
            this.ProcessInstanceID = procInstId;

            TryGetProcessInstanceByK2ClientAPI(context, procInstId);

            this.StartDate = DateTime.Now;
            this.HyperLink = itemUrl;
            this.ActivityName = activityName;
            this.Destination = Utility.GetUserProperties(this._Destination);

            
        }

        private bool TryGetProcessInstanceByK2ClientAPI(ServiceContext context, int procInstID)
        {
            var success = true;
            Client.Connection conn = null;

            try
            {
                conn = K2WorkflowUtility.GetConnection(context);
                Client.ProcessInstance processInstance = conn.OpenProcessInstance(procInstID);

                this.Folio = processInstance.Folio;
                this.ProcessName = processInstance.Name;
                this.ProcessFolder = processInstance.Folder;
                this.ProcessFullName = processInstance.FullName;
                this.ProcessStartDate = processInstance.StartDate;
                this.ProcessInstanceStatus = processInstance.Status1.ToString();
                this.DataFields = Utility.GetDataFields(processInstance.DataFields);
                this.Originator = Utility.GetUserProperties(processInstance.Originator);
            }
            catch (Exception ex)
            {
                success = false;
                LogFactory.GetLogger().Write(new LogEvent()
                {
                    Exception = ex,
                    Category = "KStar Email Expression",
                    Message = string.Format("WorkistItem-K2ClientAPI:{0}", ex.Message),
                    Source = "Process Email Expression",
                    OccurTime = DateTime.Now
                });
            }
            finally
            {
                K2WorkflowUtility.CloseConnection(conn);
            }

            return success;
        }

        #region FYI: Not available at all (By BingYi)

        private bool TryGetWorklistItemByK2DB(int procInstID, int actInstDestID, string destination)
        {
            var success = true;

            try
            {
                var queryText = @"SELECT [WH].[ProcInstID] ,[ActID],[Act].[Name] AS [ActivityName],[ProcInstFieldID] ,[ActInstID],[Status]
		                                [ActInstFieldID],[AIPriority],[AIExpectedDuration],[AIStartDate],[AISlots],[Instances],[WH].[ActInstDestID],
		                                [ActInstDestFieldID] ,[EventInstID],[EIPriority],[EIExpectedDuration],[EIStartDate],[Platform],[Data]
                                  FROM [Server].[WorklistHeader]  AS [WH]
                                  INNER JOIN [Server].[Act] AS [Act] ON [WH].[ActID] = [Act].[ID]
                                  INNER JOIN [ServerLog].[Worklist] AS [WL] ON  [WH].ProcInstID = [WL].[ProcInstID] AND [WH].[ActInstDestID] = [WL].[ActInstDestID]
                                  WHERE [WH].[ProcInstID] =@ProcInstID  AND [WH].[ActInstDestID] = @ActInstDestID ";

                var k2ConnectionString = ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString;
                var sqlParameters = new SqlParameter[] { 
                        new SqlParameter("ProcInstID", procInstID), 
                        new SqlParameter("ActInstDestID", actInstDestID) 
                    };
                using (var sqlReader = SqlHelper.ExecuteReader(k2ConnectionString, CommandType.Text, queryText, sqlParameters))
                {
                    while (sqlReader.Read())
                    {
                        this.HyperLink = sqlReader["Data"].ToString();
                        this.Status = sqlReader["Status"].ToString();
                        this.ActID = Convert.ToInt32(sqlReader["ActID"].ToString());
                        this.ActInstID = Convert.ToInt32(sqlReader["ActInstID"].ToString());
                        this.ActivityName = sqlReader["ActivityName"].ToString();
                    }
                }

                this.Destination = Utility.GetUserProperties(destination);

            }
            catch (Exception ex)
            {
                success = false;
                LogFactory.GetLogger().Write(new LogEvent()
                {
                    Exception = ex,
                    Category = "KStar Email Expression",
                    Message = string.Format("K2DB:{0}", ex.Message),
                    Source = "Worklist Email Expression",
                    OccurTime = DateTime.Now
                });
            }

            return success;

        }

        private bool TryGetWorklistItemByK2ClientAPI(ServiceContext context, string serialNumber, string destination)
        {
            var success = true;
            Client.Connection conn = null;

            try
            {
                conn = K2WorkflowUtility.GetConnection(context);
                conn.ImpersonateUser(destination);
                var wlItem = conn.OpenWorklistItem(serialNumber, "ASP", false, true);

                this.Folio = wlItem.ProcessInstance.Folio;
                this.ProcessName = wlItem.ProcessInstance.Name;
                this.ProcessFolder = wlItem.ProcessInstance.Folder;
                this.ProcessStartDate = wlItem.ProcessInstance.StartDate;

                this.HyperLink = wlItem.Data;
                this.Status = wlItem.Status.ToString();
                this.ActID = wlItem.ActivityInstanceDestination.ActID;
                this.ActInstID = wlItem.ActivityInstanceDestination.ActInstID;
                this.ActivityName = wlItem.ActivityInstanceDestination.Name;
                this.DataFields = Utility.GetDataFields(wlItem.ProcessInstance.DataFields);
                this.Originator = Utility.GetUserProperties(wlItem.ProcessInstance.Originator);
                this.Destination = Utility.GetUserProperties(this._Destination);

            }
            catch (Exception ex)
            {
                success = false;
                LogFactory.GetLogger().Write(new LogEvent()
                {
                    Exception = ex,
                    Category = "KStar Email Expression",
                    Message = string.Format("K2ClientAPI:{0}", ex.Message),
                    Source = "Worklist Email Expression",
                    OccurTime = DateTime.Now
                });
            }
            finally
            {
                K2WorkflowUtility.CloseConnection(conn);
            }

            return success;
        }

        private bool TryGetWorklistItemByK2ManagementAPI(ServiceContext context, int procInstID, string serialNumber, string destination)
        {
            var success = true;
            Management.WorkflowManagementServer server = null;

            try
            {
                server = K2WorkflowUtility.GetManagermentInstance(context);

                var filter = new Management.WorklistCriteria();
                filter.AddFilterField(Management.WCField.SerialNumber, Management.WCCompare.Equal, serialNumber);
                var wlItems = server.GetWorklistItems(filter);

                if (wlItems == null || wlItems.Count == 0)
                    throw new InvalidOperationException("The specified WorklistItem was not found!");

                var wlItem = wlItems[0];

                TryGetProcessInstanceByK2ClientAPI(context, procInstID);

                this.HyperLink = "#Not Support by Management API"; //TODO:
                this.Status = wlItem.Status.ToString();
                this.ActID = wlItem.ActID;
                this.ActInstID = wlItem.ActInstID;
                this.ActivityName = wlItem.ActivityName;
                this.Destination = Utility.GetUserProperties(this._Destination);

            }
            catch (Exception ex)
            {
                success = false;
                LogFactory.GetLogger().Write(new LogEvent()
                {
                    Exception = ex,
                    Category = "KStar Email Expression",
                    Message = string.Format("K2ManagementAPI:{0}", ex.Message),
                    Source = "Worklist Email Expression",
                    OccurTime = DateTime.Now
                });
            }
            finally
            {
                K2WorkflowUtility.CloseManagementConnection(server);
            }

            return success;
        }

        #endregion


        public string Folio { get; set; }
        public string SerialNumber { get; set; }
        public int ActID { get; set; }//
        public int ActInstID { get; set; }//
        public string ActivityName { get; set; }
        public int ActInstDestID { get; set; } //
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
        public string HyperLink { get; set; }
        public string _Destination { get; set; }

        public int ProcessInstanceID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessFolder { get; set; }
        public string ProcessFullName { get; set; }
        public DateTime ProcessStartDate { get; set; }
        public string ProcessInstanceStatus { get; set; }

        public dynamic Originator { get; set; }
        public dynamic DataFields { get; set; }
        public dynamic Destination { get; set; }

        //TODO:Support reading the XmlFileds.
    }
}
