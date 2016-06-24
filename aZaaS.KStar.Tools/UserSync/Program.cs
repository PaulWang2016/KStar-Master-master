
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserSync.Models;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;

namespace UserSync
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = null;
            Logging log = new Logging();

            try
            {
                log.ProcessLog("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                log.ProcessLog("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                log.ProcessLog("Intializing framework ...");

                Framework.Initialize();
                logger = LogFactory.GetLogger();

                log.ProcessLog("Reading app settings ...");
                ConsoleMsg.Information("Reading app settings ...");

                var dctoDNMapString = ConfigurationManager.AppSettings["DCtoDNMap"];
                var excludeDNString = ConfigurationManager.AppSettings["ExcludeDNs"];

                var ldapServer = ConfigurationManager.AppSettings["LDAPServer"];
                var ldapUser = ConfigurationManager.AppSettings["LDAPUser"];
                var ldapPassword = ConfigurationManager.AppSettings["LDAPPassword"];
                var ldapNamingContext = ConfigurationManager.AppSettings["LDAPNamingContext"];

                var connectionString = ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString;

                var dctoDNMap = new Dictionary<string, string>();
                var excludeDNs = new HashSet<string>();

                dctoDNMapString
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList().ForEach(s =>
                     {
                         var dcParts = s.Split('=');
                         dctoDNMap.Add(dcParts[0], dcParts[1]);
                     });
                excludeDNString
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList().ForEach(s => excludeDNs.Add(s));

               log.ProcessLog("Setting domain resolver ...");
               ConsoleMsg.Information("Setting domain resolver ...");
                User.SetDomainResolver(distinguishedName =>
                {
                    if (string.IsNullOrEmpty(distinguishedName))
                        return string.Empty;
                    var rootDCString = ConfigurationManager.AppSettings["RootDC"];
                    var rootDCs = rootDCString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    distinguishedName = distinguishedName.ToUpper().Replace(rootDCString.ToUpper(), string.Empty);
                    var nameValues = distinguishedName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var dcNameValues = nameValues.Where(n => n.StartsWith("DC", StringComparison.OrdinalIgnoreCase)).ToList();

                    var dc = dcNameValues.Count == 0 ? rootDCs.First().Split('=')[1] : dcNameValues.First().Split('=')[1];
                    return dctoDNMap.ContainsKey(dc) ? dctoDNMap[dc] : dc;
                });

                var serverSets = new List<LDAPSetting>()
                {
                    new LDAPSetting(ldapServer,ldapNamingContext,ldapUser, ldapPassword)
                };

                var adUsers = ADUserReader.GetUsers(serverSets);

                ConsoleMsg.Information("Synchronizing AD users to @KStar ...");
                log.ProcessLog("Synchronizing AD users to @KStar ...");
                UserSynchronizer.Sync(adUsers, excludeDNs);

                var sqlUMUsers = SQLUMUserReader.GetUsers(connectionString);

                ConsoleMsg.Information("Synchronizing SQLUM users to @KStar ...");
                log.ProcessLog("Synchronizing SQLUM users to @KStar ...");
                UserSynchronizer.Sync(sqlUMUsers);

                Console.WriteLine(">>> Enjoy!!! <<<");
                log.ProcessLog(string.Format("@_@-> {0} AD users + {1} SQLUM users have been processed.", adUsers.Count,sqlUMUsers.Count));
                
            }
            catch (Exception ex)
            {

               logger.Write(new LogEvent
                {
                    Category = "FMC External",
                    Source = "FMCUserSync",
                    Exception = ex,
                    Message = ex.Message, 
                    OccurTime = DateTime.Now
                });

               log.ProcessLog(string.Format("Exception: {0}", ex.Message));
                ConsoleMsg.Error("Ooh...");
                ConsoleMsg.Error(ex.ToString());

                Console.ReadKey();
            }
            
            logger.Write(new LogEvent
            {
                Category = "FMC External",
                Source = "FMCUserSync - Running",
                Exception = null,
                Message = string.Format("The last run on {0}", DateTime.Now),
                OccurTime = DateTime.Now
            });

        }
    }
}
