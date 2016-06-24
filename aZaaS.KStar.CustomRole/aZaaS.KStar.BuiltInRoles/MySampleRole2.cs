using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.CustomRole;

namespace aZaaS.KStar.BuiltInRoles
{
   public class MySampleRole2 : MarshalByRefObject ,ICustomRole
    {
        public string RoleType
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> Execute(CustomRoleContext context)
        {
            return new List<string>() { "MySampleRole2" };
        }
    }
}
