using aZaaS.KStar.Interface;
using aZaaS.KStar.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InTransitTaskService
{
    public class LaunchTask : IKStarJob
    {
          string connectionString;

        string taskUrl;
        public void Run(Dictionary<string, object> context)
        {
            if (context.ContainsKey("ConnectionString"))
            {
                connectionString = context["ConnectionString"].ToString();
            }
            if (context.ContainsKey("ConnectionString"))
            {
                taskUrl = context["TaskUrl"].ToString();
            }

            //获取GetProcInstList

            var procInstList = GetProcInstList();
            try
            {
                foreach (DataRow row in procInstList.Rows)
                {

                    GetUrltoHtml(string.Format(taskUrl, row["ProcInstID"] + string.Empty));
                }

            }
            catch (Exception ex)
            {

            }
        }

        private System.Data.DataTable GetProcInstList()
        {
            string sql = @"SELECT  ProcInstID FROM [aZaaS.Framework].[dbo].[ProcessPrognosisTask] ";

            return SqlHelper.ExecuteTable(connectionString, sql);
        } 
        public static string GetUrltoHtml(string Url)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }
    }
}
