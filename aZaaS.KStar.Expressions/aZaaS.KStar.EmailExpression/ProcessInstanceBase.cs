using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using aZaaS.KStar;
using aZaaS.KStar.MgmtServices;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Logging;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Extensions;
using Cle = SourceCode.Workflow.Client;
using Mgm = SourceCode.Workflow.Management;



namespace aZaaS.KStar.EmailExpression
{
    /// <summary>
    /// Process instance expression model.
    /// * Before you go to invoke the Set methods,you must be setting the ID value first.
    /// Note:
    /// We reading the fields infomation through smartobject api,
    /// that would be cause performance to slowly,but in order to implement this we choose this way first.
    /// </summary>
    public class ProcessInstanceBase
    {
        public ProcessInstanceBase() { }

        public ProcessInstanceBase(ServiceContext context)
        {
            this.SetProperties(context);
        }

        protected void SetProperties(ServiceContext context)
        {
            int procInstID = 0;
            if (context["ProcessInstanceID"].NullOrEmpty())
                return;

            int.TryParse(context["ProcessInstanceID"], out procInstID);
            if (procInstID == 0)
                return;

            var activityName = context["ActivityName"];
            this.ActivityName = activityName ?? string.Empty;

            if (!TryGetProcessInstanceByK2ClientAPI(context, procInstID))
                TryGetProcessInstanceByK2ManagementAPI(context, procInstID);            
        }

        private bool TryGetProcessInstanceByK2ClientAPI(ServiceContext context, int procInstID)
        {
            var success = true;
            Cle.Connection conn = null;

            try
            {
                conn = K2WorkflowUtility.GetConnection(context);
                Cle.ProcessInstance processInstance = conn.OpenProcessInstance(procInstID);

                this.ID = processInstance.ID;
                this.Folio = processInstance.Folio;
                this.Name = processInstance.Name;
                this.Folder = processInstance.Folder;
                this.FullName = processInstance.FullName;
                this.StartDate = processInstance.StartDate;
                //this.FinishDate = processInstance.FinishDate;
                this.Status = processInstance.Status1.ToString();
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
                    Message = string.Format("K2ClientAPI:{0}", ex.Message),
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

        private bool TryGetProcessInstanceByK2ManagementAPI(ServiceContext context, int procInstID)
        {
            var success = true;
            Mgm.WorkflowManagementServer server = null;

            try
            {
                server = K2WorkflowUtility.GetManagermentInstance(context);
                Mgm.Criteria.ProcessInstanceCriteriaFilter filter = new Mgm.Criteria.ProcessInstanceCriteriaFilter();
                filter.AddRegularFilter(Mgm.ProcessInstanceFields.ProcInstID, Mgm.Criteria.Comparison.Equals, procInstID);
                Mgm.ProcessInstance processInstance = server.GetProcessInstancesAll(filter)[0];

                this.ID = processInstance.ID;
                this.Folio = processInstance.Folio;
                var fullName = processInstance.Process.FullName;
                this.Name = fullName.Substring(fullName.IndexOf(@"\") + 1);
                this.Folder = fullName.Substring(0, fullName.IndexOf(@"\")); ;
                this.FullName = processInstance.ProcSetFullName;
                this.StartDate = processInstance.StartDate;
                this.FinishDate = processInstance.FinishDate;
                this.Status = processInstance.Status.ToString();
                this.DataFields = Utility.GetDataFields(processInstance.ID);
                this.Originator = Utility.GetUserProperties(processInstance.Originator);
            }
            catch (Exception ex)
            {
                success = false;
                LogFactory.GetLogger().Write(new LogEvent()
                {
                    Exception = ex,
                    Category = "KStar Email Expression",
                    Message = string.Format("K2ManagementAPI:{0}", ex.Message),
                    Source = "Process Email Expression",
                    OccurTime = DateTime.Now
                });
                
            }
            finally
            {
                K2WorkflowUtility.CloseManagementConnection(server);
            }

            return success;
        }



        public int ID { get; set; }
        public string Folio { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public string FullName { get; set; }
        public string _Originator { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string Status { get; set; }
        public dynamic Originator { get; set; }
        public dynamic DataFields { get; set; }

        public string ActivityName { get; set; }

        //TODO:Support reading the XmlFileds.
    }
}
