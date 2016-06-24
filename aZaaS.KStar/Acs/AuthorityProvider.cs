using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.Acs
{
    public class AuthorityProvider : IAuthorityProvider
    {
        private readonly UserService userService = new UserService();
        private readonly RoleService roleService = new RoleService();
        public IEnumerable<Guid> ExpandForAuthorization(Guid authorityId)
        {
            IEnumerable<Guid> guids = null;
            try
            {
                guids = roleService.GetUserRoles(authorityId).Select(s => s.SysID);
            }
            catch (Exception)
            {
                guids = new List<Guid>();
            }
            return guids;
        }

        public int GetAuthorityType(Guid authorityId)
        {
            return userService.ReadUser(authorityId) == null ? (int)AuthorityType.Role : (int)AuthorityType.User;
            //return db.RoleUsers.Any(x => x.UserID == authorityId) ?
            //    (int)AuthorityType.Role : (int)AuthorityType.User;
        }
    }
}
