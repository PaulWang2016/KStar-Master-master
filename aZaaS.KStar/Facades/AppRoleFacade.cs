using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.AppRole;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.Extensions;

namespace aZaaS.KStar.Facades
{
    public class AppRoleFacade
    {
        private AppRoleManager _appRoleManager;

        public AppRoleFacade()
        {
            _appRoleManager = new AppRoleManager();
        }


        public List<AppRoleDTO> GetRoleByPane(string pane)
        {
            pane.NullOrEmptyThrowArgumentEx("pane is Null");

            return _appRoleManager.GetRoleByPane(pane);
        }

        public void AddAppRole(Guid id, string pane)
        {
            id.EmptyThrowArgumentEx("id is Null");
            pane.NullOrEmptyThrowArgumentEx("pane is Null");

            _appRoleManager.AddAppRole(id, pane);
        }
        public void DelAppRoleByListID(List<Guid> ListID, string pane)
        {
            ListID.NullOrEmptyThrowArgumentEx("ListID is Null");
            pane.NullOrEmptyThrowArgumentEx("pane is Null");

            _appRoleManager.DelAppRoleByListID(ListID,  pane);

        }
    }
}
