using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtServices;
using Cle = SourceCode.Workflow.Client;
using Mgm = SourceCode.Workflow.Management;
using System.Data;


namespace aZaaS.KStar.EmailExpression
{
    public static class Utility
    {

        public static ExObject GetDataFields(int procInstId)
        {
            var exObj = new ExObject();
            var context = new ServiceContext();
            var fields = new Dictionary<string, object>();

            var criteria = new Dictionary<string, object>() { { "ProcessInstanceID", procInstId } };
            context.AuthType = ServiceAuthType.Form;
            var fieldTable = K2SmartObjectHelper.SmartObjectGetList(context, criteria, "Process_Data", "List");

            if (fieldTable != null && fieldTable.Rows.Count > 0)
            {
                foreach (DataRow row in fieldTable.Rows)
                {
                    string fieldName = row["DataName"].ToString();
                    if (!fields.ContainsKey(fieldName))
                    {
                        fields.Add(fieldName, row["DataValue"].ToString());
                    }
                }
            }

            exObj.SetItems(fields);

            return exObj;
        }

        public static ExObject GetDataFields(Cle.DataFields dataFields)
        {
            var exObj = new ExObject();
            var fields = new Dictionary<string, object>();

            if (dataFields != null && dataFields.Count > 0)
            {
                foreach (Cle.DataField dataField in dataFields)
                {
                    string fieldName = dataField.Name;
                    if (!fields.ContainsKey(fieldName))
                    {
                        fields.Add(fieldName, dataField.Value);
                    }
                }
            }

            exObj.SetItems(fields);

            return exObj;
        }

        public static ExObject GetUserProperties(Cle.User k2User)
        {
            var userService = new UserService();
            var user = userService.ReadUserBase(ConvertToKStarUser(k2User.Name));

            var exObj = new ExObject();
            var fields = new Dictionary<string, object>();

            if (user != null)
            {
                fields.Add("FirstName", user.FirstName);
                fields.Add("LastName", user.LastName);
                fields.Add("DisplayName", string.Format("{0} {1}", user.FirstName, user.LastName));
                fields.Add("Email", user.Email);
                fields.Add("Sex", user.Sex);
                fields.Add("Status", user.Status);
                fields.Add("Phone", user.Phone);
                fields.Add("Description", user.Remark);
            }

            exObj.SetItems(fields);

            return exObj;
        }

        public static ExObject GetUserProperties(string inputUser)
        {
            var userService = new UserService();
            var user = userService.ReadUserBase(ConvertToKStarUser(inputUser));

            var exObj = new ExObject();
            var fields = new Dictionary<string, object>();

            if (user != null)
            {
                fields.Add("FirstName", user.FirstName);
                fields.Add("LastName", user.LastName);
                fields.Add("DisplayName", string.Format("{0} {1}", user.FirstName, user.LastName));
                fields.Add("Email", user.Email);
                fields.Add("Sex", user.Sex);
                fields.Add("Status", user.Status);
                fields.Add("Phone", user.Phone);
                fields.Add("Description", user.Remark);
            }

            exObj.SetItems(fields);

            return exObj;
        }

        private static string ConvertToKStarUser(string account)
        {
            if (string.IsNullOrEmpty(account))
                return string.Empty;

            int index = account.IndexOf(":");
            return index > 0 ? account.Substring(index + 1) : account;
        }

        private static string ConvertToK2User(string userName)
        {
            var context = new ServiceContext();
            return userName.IndexOf(':') > 0 ? userName : string.Format("{0}:{1}", context[SettingVariable.SecurityLabelName], userName);
        }
    }
}
