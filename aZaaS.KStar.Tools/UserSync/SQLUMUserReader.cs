using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using UserSync.Models;
using System.Configuration;

namespace UserSync
{
    public static class SQLUMUserReader
    {
        public static HashSet<User> GetUsers(string connectionString)
        {
            var allUsers = new HashSet<User>();
            Logging log = new Logging();

            log.ProcessLog("Fetching SQLUM users");
            try
            {
                var sql = "SELECT [UserName],[UserDescription],[UserEmail] FROM [CustomUM].[User]";
                var userTable = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, sql).Tables[0];
                if (userTable != null && userTable.Rows.Count > 0)
                {
                    foreach (DataRow row in userTable.Rows)
                    {
                        var userName = Convert.ToString(row["UserName"]);
                        var userEmail = Convert.ToString(row["UserEmail"]);
                        var userDescription = Convert.ToString(row["UserDescription"]);

                        allUsers.Add(new User()
                        {
                            UserName = userName,
                            LastName = userName,
                            Email = userEmail,
                            Description = userDescription
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.ProcessLog(string.Format("** SQLUMUserReader Exception ** {0}", ex.Message));
            }


            return allUsers;
        }
    }
}
