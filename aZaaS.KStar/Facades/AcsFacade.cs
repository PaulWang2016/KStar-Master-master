using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Acs.Models;
using aZaaS.Framework.ACS.Core;
using aZaaS.Framework.Extensions;

namespace aZaaS.KStar.Facades
{
    public class AcsFacade
    {
        private AcsManager _acsManager;

        public AcsFacade()
        {
            _acsManager = new AcsManager();
        }


        public List<AuthorizationMatrices> GetAllAuthorization()
        {
            return _acsManager.GetAllAuthorization();
        }
        public AuthorizationMatrices GetAuthorization(Guid id)
        {
            id.EmptyThrowArgumentEx("id is Null");

            return _acsManager.GetAuthorization(id);
        }
        public List<UserAndRole> GetUserAndRole()
        {
            return _acsManager.GetUserAndRole();
        }
        public List<UserAndRole> FindUserAndRole(string input)
        {
            return _acsManager.FindUserAndRole(input);
        }
        public List<AuthorizationMatrices> _FindAuthori(string input)
        {
            return _acsManager.FindAuthori(input);
        }
        public List<ResourcePermissionView> FindResourcePermisssion(string input)
        {
            return _acsManager.FindResourcePermisssion(input);
        }
        public void CreateAuthorization(Guid AuthorityId, bool Granted, List<Guid> ResourcePermissionIdList)
        {
            AuthorityId.EmptyThrowArgumentEx("AuthorityId is Null");
            ResourcePermissionIdList.NullOrEmptyThrowArgumentEx("ResourcePermissionIdList is Null");

            _acsManager.CreateAuthorization(AuthorityId, Granted, ResourcePermissionIdList);
        }

        public void UpdateAuthorization(AuthorizationMatrix item)
        {
            item.NullThrowArgumentEx("AuthorizationMatrix is Null");

            _acsManager.UpdateAuthorization(item);
        }
        public void DelAuthorization(List<string> idList)
        {
            idList.NullOrEmptyThrowArgumentEx("Delete idList is Null");

            _acsManager.DelAuthorization(idList);
        }
        public void DisableAuthorization(List<string> idList)
        {
            idList.NullOrEmptyThrowArgumentEx("Disable idList is Null");

            _acsManager.DisableAuthorization(idList);
        }
        public void EnableAuthorization(List<string> idList)
        {
            idList.NullOrEmptyThrowArgumentEx("Enable idList is Null");

            _acsManager.EnableAuthorization(idList);
        }



        public List<ResourcePermissionView> GetAllResourcePermission()
        {
            return _acsManager.GetAllResourcePermission();
        }
        public ResourcePermissionView GetResourcePermission(Guid id)
        {
            return _acsManager.GetResourcePermission(id);
        }
        public List<Permission> FindPermission(string input)
        {
            return _acsManager.FindPermission(input);
        }
        public void CreateResourcePermission(Guid PermissionSysId, string ResourceType, List<string> ResourceIdList)
        {
            _acsManager.CreateResourcePermission(PermissionSysId, ResourceType, ResourceIdList);
        }
        public void DelResourcePermission(List<string> idList)
        {
            _acsManager.DelResourcePermission(idList);
        }



        public IEnumerable<Permission> GetPermission()
        {
            return _acsManager.GetPermission();
        }
        public void CreatePermission(Permission item)
        {
            _acsManager.CreatePermission(item);
        }
        public void UpdatePermission(Permission item)
        {
            _acsManager.UpdatePermission(item);
        }
        public void DelPermission(List<string> idList)
        {
            _acsManager.DelPermission(idList);
        }

        public List<Menu> AuthorityTop(string userName)
        {
            return _acsManager.AuthorityTop(userName);
        }
        public Menu AuthorityMenuBar(string menuKey, string userName)
        {
            return _acsManager.AuthorityMenuBar(menuKey, userName);
        }
        public DocumentLibrary AuthorityDocumentItem(string userName, string DocLibraryKey)
        {
            return _acsManager.AuthorityDocumentItem(userName, DocLibraryKey);
        }
        public List<string> AuthorityDocumentLibrary(string userName, string MenuKey)
        {
            return _acsManager.AuthorityDocumentLibrary(userName, MenuKey);
        }
        public List<string> AuthorityResourcesPermissionList(Guid roleID, string type)
        {
            return _acsManager.AuthorityResourcesPermissionList(roleID, type);
        }
        public void ChangeStatus(string RoleID, string id, bool status)
        {
            _acsManager.ChangeStatus(RoleID, id, status);
        }


        public DynamicWidget AuthorityDynamiWidget(string key, string userName)
        {
           return _acsManager.AuthorityDynamiWidget( key,userName);
        }














    }
}
