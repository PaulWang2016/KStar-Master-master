using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using aZaaS.KStar.CustomRole;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "IEchoService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select IEchoService.svc or IEchoService.svc.cs at the Solution Explorer and start debugging.
    public class EchoService : IEchoService
    {
        public string DoWork(string userId)
        {
            var customRoles = CustomRoleManager.Current.CustomRoles;

            List<string> results = new List<string>();
            //var context = new CustomRoleContext() { UserId = userId };
            //foreach (var role in customRoles)
            //{
            //    results.AddRange(role.Execute(context));
            //}

            return string.Join(";",results);
        }


        public void DoRefresh()
        {
            CustomRoleManager.Current.RestartRoleLoader();
        }
    }
}
