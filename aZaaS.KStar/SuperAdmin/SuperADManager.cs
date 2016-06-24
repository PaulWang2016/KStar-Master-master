using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.SuperAdmin
{
    public class SuperADManager
    {
        private readonly UserService userService;
        public SuperADManager()
        {
            this.userService = new UserService();
        }
        public bool IsSuperAD(Guid UserId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                int count = context.SuperAD.Where(s => s.UserID == UserId).Count();
                return count > 0;
            }
        }

        public bool IsSuperAD(string UserName)
        {
            //UserBO userBO = new UserBO();
            //UserDTO user = userBO.ReadUser(UserName);
            var user = userService.ReadUserBase(UserName);
            if (user != null)
            {
                return IsSuperAD(user.SysID);
            }
            return false;
        }
    }
}
