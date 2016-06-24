using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;

using aZaaS.Framework.Workflow.Util;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QuartzMailService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select QuartzMailService.svc or QuartzMailService.svc.cs at the Solution Explorer and start debugging.
    public class QuartzMailService : IQuartzMailService
    {
        private readonly string _connectionString;

        public QuartzMailService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["aZaaSFramework"].ConnectionString;
        }

        public void SendMail(string recipients, string subject, string message)
        {
            if (string.IsNullOrEmpty(recipients))
                throw new ArgumentNullException("recipients");
            if (string.IsNullOrEmpty("subject"))
                throw new ArgumentNullException("subject");
            if (string.IsNullOrEmpty("message"))
                throw new ArgumentNullException("message");

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Title",subject),
                new SqlParameter("@Body",message),
                new SqlParameter("@Recipients",recipients)
            };

            SqlHelper.ExecuteNonQuery(_connectionString, "SP_QuartzMailService", parameters.ToArray());
        }
    }
}
