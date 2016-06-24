using aZaaS.KStar.SuperAdmin;
using aZaaS.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Facades
{
    public class SuperADFacade
    {
        private SuperADManager _superADManager;

        public SuperADFacade()
        {
            _superADManager = new SuperADManager();
        }

        public bool IsSuperAD(Guid UserId)
        {
            UserId.EmptyThrowArgumentEx("UserId is Empty");

            return _superADManager.IsSuperAD(UserId);
        }

        public bool IsSuperAD(string UserName)
        {
            UserName.NullOrEmptyThrowArgumentEx("UserName is Null");

            return _superADManager.IsSuperAD(UserName);
        }
    }
}
