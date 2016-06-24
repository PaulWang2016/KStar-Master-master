using aZaaS.KStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Helper;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Localization;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.MgmtDtos;
using System.Linq.Expressions;
using aZaaS.KStar.Helper;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class ApplicationsController : BaseMvcController
    {
        private static readonly MenuFacade _menufacade = new MenuFacade();
        private readonly RoleBO roleBO;

        public ApplicationsController()
        {
            roleBO = new RoleBO();
        }

        public JsonResult GetApps()
        {
            return Json(_menufacade.GetTop(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateMenu(Menu menu)
        {
            menu.Id = Guid.NewGuid();
            return Json(_menufacade.CreateMenu(menu), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateMenu(Menu menu)
        {
            return Json(_menufacade.UpdateMenu(menu), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelMenu(List<string> keyList)
        {
            return Json(_menufacade.DelMenu(keyList), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleList()
        {
            RoleService roleService = new RoleService();
            List<string> SysRoleList = new List<string>();
            foreach (var item in roleService.GetAllRoles())
            {
                SysRoleList.Add(item.Name);
            }
            return Json(SysRoleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveConfiguration(string RoleID, [ModelBinder(typeof(JsonListBinder<TreeViews>))]List<TreeViews> Items)// string Type,
        {
            AcsFacade acsFacade = new AcsFacade();
            foreach (var item in Items)
                acsFacade.ChangeStatus(RoleID, item.ID, item.Status);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveWidgetPermission(string RoleID, [ModelBinder(typeof(JsonListBinder<TreeViews>))]List<TreeViews> Items)// string Type,
        {
            AcsFacade acsFacade = new AcsFacade();
            foreach (var item in Items)
                acsFacade.ChangeStatus(RoleID, item.ID, item.Status);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #region 获取权限管理树

        /// <summary>
        /// 获取权限管理菜单树  异步获取全部节点
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public JsonResult GetPermissionMenus(string RoleID, string Type)
        {
            List<TreeViews> menuList = new List<TreeViews>();
            //获取权限
            AcsFacade acsFacade = new AcsFacade();
            List<string> ResourcePermissionIdList = acsFacade.AuthorityResourcesPermissionList(Guid.Parse(RoleID), "Read");            
            ResourceFacade rf = new ResourceFacade();
            List<Resource> Nodes = rf.GetMenuTreeResources(null, Type);
            foreach (var node in Nodes)
            { 
                string parentid = null;
                if (node.ParentID != null)
                    parentid = node.ParentID.ToString();
                TreeViews treenode = new TreeViews()
                {
                    ID = node.ID.ToString(),
                    HasChildren = true,
                    Link = node.Links,
                    Target = node.Target,
                    ParentID = parentid,
                    Type = node.Type,
                    Name = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(node.DisplayName),                    
                    Status = ResourcePermissionIdList.Contains(Nodes[0].ID.ToString())
                };
                GetRecursivePermission(RoleID, Type, treenode, ResourcePermissionIdList);
                menuList.Add(treenode);
            }
            return Json(menuList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 递归获取权限数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public void GetRecursivePermission(string RoleID, string Type, TreeViews node, List<string> ResourcePermissionIdList)
        {
            ResourceFacade rf = new ResourceFacade();
            List<Resource> children = rf.GetMenuTreeResources(Guid.Parse(node.ID), Type);            
            foreach (var child in children)
            {
                string parentid = null;
                if (child.ParentID != null)
                    parentid = child.ParentID.ToString();
                TreeViews childtree = new TreeViews()
                {
                    ID = child.ID.ToString(),
                    HasChildren = true,
                    Link = child.Links,
                    Target = child.Target,
                    ParentID = parentid,
                    Type = child.Type,
                    Name = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(child.DisplayName),
                    Status = ResourcePermissionIdList.Contains(child.ID.ToString())
                };
                GetRecursivePermission(RoleID, Type, childtree, ResourcePermissionIdList);
                node.items.Add(childtree);
            }
        }

        /// <summary>
        /// 获取权限管理菜单树 异步单节点
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [NonAction]
        public JsonResult GetPermissionMenus(string RoleID, string Type, string ID = "")
        {
            ResourceFacade rf = new ResourceFacade();
            List<Resource> Nodes;

            if (ID == "")
                Nodes = rf.GetMenuTreeResources(null, Type);
            else
                Nodes = rf.GetMenuTreeResources(Guid.Parse(ID), Type);
            List<TreeViews> MenuList = new List<TreeViews>();

            AcsFacade acsFacade = new AcsFacade();
            var ResourcePermissionIdList = acsFacade.AuthorityResourcesPermissionList(Guid.Parse(RoleID), "Read");
            foreach (var node in Nodes)
            {
                string parentid = null;
                if (node.ParentID != null)
                    parentid = node.ParentID.ToString();
                MenuList.Add(new TreeViews()
                {
                    ID = node.ID.ToString(),
                    HasChildren = true,
                    Link = node.Links,
                    Target = node.Target,
                    ParentID = parentid,
                    Type = node.Type,
                    Name = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(node.DisplayName),
                    Status = ResourcePermissionIdList.Contains(node.ID.ToString())
                });
            }
            return Json(MenuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWidgetPermissionGrid(string RoleID, string pane)
        {
            AcsFacade acsFacade = new AcsFacade();
            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            var widgetpermissionList = WidgetFacade.GetAllWidget(pane);
            var ResourcePermissionIdList = acsFacade.AuthorityResourcesPermissionList(Guid.Parse(RoleID), "Read");
            return Json(widgetpermissionList.Select(s => new
            {
                Status = ResourcePermissionIdList.Contains(s.ID.ToString()),
                ID = s.ID,
                Key = s.Key,
                DisplayName = s.DisplayName
            }).ToList()
             , JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPermissionDoc(string RoleID, string Type, string ID = "")
        {
            ResourceFacade rf = new ResourceFacade();
            List<Resource> Nodes;

            if (ID == "")
                Nodes = rf.GetDocumentTreeResources(null, Type);
            else
                Nodes = rf.GetDocumentTreeResources(Guid.Parse(ID), Type);
            List<TreeViews> MenuList = new List<TreeViews>();

            AcsFacade acsFacade = new AcsFacade();
            var ResourcePermissionIdList = acsFacade.AuthorityResourcesPermissionList(Guid.Parse(RoleID), "Read");
            foreach (var node in Nodes)
            {
                string parentid = null;
                if (node.ParentID != null)
                    parentid = node.ParentID.ToString();
                MenuList.Add(new TreeViews()
                {
                    ID = node.ID.ToString(),
                    HasChildren = true,
                    Link = node.Links,
                    Target = node.Target,
                    ParentID = parentid,
                    Type = node.Type,
                    Name = node.DisplayName,
                    Status = ResourcePermissionIdList.Contains(node.ID.ToString())
                });
            }
            return Json(MenuList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取权限管理菜单
        /// <summary>
        /// 获取权限管理菜单树
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetMenus(string key, string ID = "")
        {
            ResourceFacade rf = new ResourceFacade();
            List<Resource> Nodes;

            if (ID == "")
                Nodes = rf.GetMenuTreeResources(null, key);
            else
                Nodes = rf.GetMenuTreeResources(Guid.Parse(ID), key);
            List<TreeViews> MenuList = new List<TreeViews>();

            foreach (var node in Nodes)
            {
                string parentid = null;
                if (node.ParentID != null)
                    parentid = node.ParentID.ToString();
                MenuList.Add(new TreeViews()
                {
                    OrderBy = node.OrderBy,
                    IconKey = node.IconKey,
                    ID = node.ID.ToString(),
                    HasChildren = node.Kind == "Item" ? false : true,
                    Link = node.Links,
                    Target = node.Target,
                    ParentID = parentid,
                    Type = node.Type,
                    Data = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName_All(node.DisplayName),
                    Name = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(node.DisplayName)//node.DisplayName
                });
            }
            return Json(MenuList.OrderBy(o => o.OrderBy), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 添加 菜单
        /// <summary>
        /// 添加 菜单
        /// </summary>
        [ValidateInput(false)]
        public JsonResult AddMenu(TreeViews item)
        {
            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(((string[])item.Data)[0]);
            var key = string.Empty;
            foreach (var k in dic["value"].Keys)
            {
                if (!string.IsNullOrEmpty(dic["value"][k]))
                {
                    key = dic["value"][k];
                    break;
                }
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Display Name is empty!");
            }

            if (item.Type == "MenuItem")
                item.HasChildren = false;
            else item.HasChildren = true;
            ResourceFacade rf = new ResourceFacade();
            Resource resource = new Resource();
            Guid id = Guid.NewGuid();
            resource.IconKey = item.IconKey;
            resource.ID = id;
            resource.DisplayName = key;
            resource.Links = item.Link;
            resource.ParentID = Guid.Parse(item.ID);
            resource.Type = "MenuItem";
            resource.OrderBy = item.OrderBy;
            resource.Target = item.Target;
            rf.AddResource(resource);
            item.ID = id.ToString();

            item.ID = id.ToString();
            item.ParentID = item.ID;
            item.Type = "MenuItem";

            LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName_Update(resource.DisplayName, dic["value"]);

            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 编辑 菜单
        /// <summary>
        /// 编辑 菜单
        /// </summary>
        [ValidateInput(false)]
        public JsonResult EditMenu(TreeViews item)
        {
            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(((string[])item.Data)[0]);

            ResourceFacade rf = new ResourceFacade();
            Resource resource = new Resource();
            Guid? parentid = null;
            if (item.ParentID != null)
                parentid = Guid.Parse(item.ParentID);
            resource.ID = Guid.Parse(item.ID);
            resource.IconKey = item.IconKey;
            resource.DisplayName = dic["key"].Keys.First();//item.Name;
            resource.Links = item.Link;
            resource.OrderBy = item.OrderBy;
            resource.Target = item.Target;
            resource.ParentID = parentid;
            rf.EditResource(resource);

            LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName_Update(resource.DisplayName, dic["value"]);

            item.Data = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName_All(resource.DisplayName);
            item.Name = LocalizationResxExtend.DBResxFor_aZaaSKStar_MenuBar_DisplayName(resource.DisplayName);

            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region  删除 菜单
        /// <summary>
        /// 删除 菜单    --------批量
        /// </summary>
        public JsonResult DeleteMenu(string id)//List<string> 
        {
            ResourceFacade rf = new ResourceFacade();

            rf.DeleteResource(Guid.Parse(id));

            return Json(id, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 获取权限管理DOC树
        /// <summary>
        /// 获取权限管理菜单树
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetDocuemtns(string key, string ID = "")
        {
            ResourceFacade rf = new ResourceFacade();
            List<Resource> Nodes;

            if (ID == "")
                Nodes = rf.GetDocumentTreeResources(null, key);
            else
                Nodes = rf.GetDocumentTreeResources(Guid.Parse(ID), key);
            List<TreeViews> DocList = new List<TreeViews>();

            foreach (var node in Nodes)
            {
                string parentid = null;
                if (node.ParentID != null)
                    parentid = node.ParentID.ToString();
                DocList.Add(new TreeViews()
                {
                    IconKey = node.Key, // documentlibrary 中的key 暂用iconkey替代  documentitem中没有
                    ID = node.ID.ToString(),
                    HasChildren = node.Type == "DocumentItem" ? false : true,
                    Link = node.Links,  //documentitem中的links  documentlibrary中没有
                    Target = node.IconPath, // document 中的IconPath 暂用Target替代
                    ParentID = parentid,
                    Type = node.Type,
                    Name = node.DisplayName
                });
            }
            return Json(DocList, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 添加 doc
        /// <summary>
        /// 添加 菜单
        /// </summary>
        public JsonResult AddDoc(TreeViews item)
        {
            ResourceFacade rf = new ResourceFacade();
            Resource resource = new Resource();
            Guid id = Guid.NewGuid();
            if (item.Type == "Menu")
            {
                item.Type = "DocumentLibrary";
                resource.Key = item.IconKey;
                resource.Links = "";
                resource.IconPath = item.Target;

            }
            else if (item.Type == "DocumentLibrary")
            {
                item.Type = "DocumentItem";
                resource.Key = "";
                resource.Links = item.Link;
                resource.IconPath = item.Target;
            }
            resource.ID = id;
            resource.DisplayName = item.Name;
            resource.ParentID = Guid.Parse(item.ID);
            resource.Type = item.Type;
            rf.AddResource(resource);

            item.ID = id.ToString();
            item.ParentID = item.ID;
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 编辑 doc
        /// <summary>
        /// 编辑 菜单
        /// </summary>
        public JsonResult EditDoc(TreeViews item)
        {
            ResourceFacade rf = new ResourceFacade();
            Resource resource = new Resource();
            Guid? parentid = null;
            if (item.ParentID != null)
                parentid = Guid.Parse(item.ParentID);
            resource.ID = Guid.Parse(item.ID);
            resource.DisplayName = item.Name;
            resource.ParentID = parentid;
            resource.Key = item.IconKey;
            resource.Links = item.Link;
            resource.IconPath = item.Target;
            rf.EditResource(resource);
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region  删除 doc
        /// <summary>
        /// 删除 菜单    --------批量
        /// </summary>
        public JsonResult DeleteDoc(string id)//List<string> 
        {
            ResourceFacade rf = new ResourceFacade();

            rf.DeleteResource(Guid.Parse(id));

            return Json(id, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region   系统角色管理
        #region 获取角色分类树形
        /// <summary>
        /// 获取角色分类树形  当 Parent ID 为 0 时  为获取分类
        /// </summary>
        /// <param name="id">Parent ID</param>
        /// <returns>List_Position</returns>
        public JsonResult GetRoleTree(string ID = "")
        {
            List<RoleTree> items = new List<RoleTree>();
            List<RoleCategoryWithChildCategoriesDto> categories = new List<RoleCategoryWithChildCategoriesDto>();
            List<RoleWithCategoryDto> rolelist = new List<RoleWithCategoryDto>();
            if (ID == "")
            {
                categories = this.roleBO.GetAllCategories().OrderBy(x => x.Name).ToList();
            }
            else
            {
                categories = this.roleBO.GetChildCategories(Guid.Parse(ID)).OrderBy(x => x.Name).ToList();

                rolelist = this.roleBO.GetCategoryRoles(Guid.Parse(ID)).ToList();

            }
            foreach (var item in categories)
            {
                items.Add(new RoleTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = (roleBO.ReadCategory(item.SysID).Roles.Count > 0 || item.ChildCategories.Count > 0) ? true : false,
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = (ID == "" ? null : ID),
                    Type = RoleType.Category.ToString()
                });
            }
            foreach (var item in rolelist)
            {
                var role = new RoleTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = false,
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = item.Category.SysID.ToString(),
                    Type = RoleType.Role.ToString()
                };
                items.Add(role);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 编辑 系统角色
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public JsonResult EditRole(Role item)
        {
            RoleBO roleBO = new RoleBO();
            var roleitem = roleBO.ReadRole(Guid.Parse(item.ID));
            roleitem.Name = item.Name;
            roleBO.UpdateRole(roleitem);
            //var afterupdaterole = roleBO.ReadRole(Guid.Parse(item.ID));
            RoleTree role = new RoleTree()
            {
                Type = RoleType.Role.ToString(),
                ParentID = item.CategorySysId,
                ID = item.ID,
                HasChildren = false,
                DisplayName = item.Name
            };
            return Json(role, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加 系统角色
        /// <summary>
        /// 添加 系统角色
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public JsonResult AddRole(Role item, string Pane)
        {
            RoleBO roleBO = new RoleBO();
            var allRoles = roleBO.GetAllRoles();
            if (!allRoles.Select(S => S.Name).Contains(item.Name))
            {
                var newRoleID = roleBO.CreateRole(new RoleWithRelationshipsDto { SysID = Guid.NewGuid(), Name = item.Name});
                item.ID=newRoleID.ToString();
                roleBO.UpdateRoleCategory(new Guid(item.ID), new Guid(item.CategorySysId));
            }            
            AppRoleFacade appRoleFacade = new AppRoleFacade();
            var ID = roleBO.GetAllRoles().Where(m => m.Name == item.Name).Select(s => s.SysID).FirstOrDefault();
            appRoleFacade.AddAppRole(ID, Pane);
            var roleItem = roleBO.ReadRole(ID);
            RoleTree role = new RoleTree()
            {
                Type = RoleType.Role.ToString(),
                ParentID = roleItem.Category.SysID.ToString(),
                ID = roleItem.SysID.ToString(),
                HasChildren = false,
                DisplayName = roleItem.Name
            };

            return Json(role, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  删除 系统角色关联    --------批量
        /// <summary>
        /// 删除 系统角色    --------批量
        /// </summary>
        /// <param name="ListID"></param>
        /// <returns></returns>
        public JsonResult DeleteRole(List<Guid> ListID, string Pane)
        {
            AppRoleFacade appRoleFacade = new AppRoleFacade();
            appRoleFacade.DelAppRoleByListID(ListID, Pane);

            ListID.ForEach(item=>{
                roleBO.DeleteRole(item);
            });            

            return Json(ListID, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 关联系统角色 列表
        /// <summary>
        /// 获取 系统角色 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRelevanceRoleList(string pane)
        {
            List<SysRole> SysRoleList = new List<SysRole>();
            AppRoleFacade appRoleFacade = new AppRoleFacade();

            foreach (var item in appRoleFacade.GetRoleByPane(pane))
            {
                SysRole roleitem = new SysRole { DisplayName = item.DisplayName, RoleID = item.RoleID.ToString() };
                SysRoleList.Add(roleitem);
            }
            return Json(SysRoleList.OrderBy(x => x.DisplayName).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加用户到角色      --------批量
        /// <summary>
        /// 添加用户到角色      --------批量
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public JsonResult AppendUserstoRole(Guid roleId, List<Guid> userIds)
        {
            UserBO userBO = new UserBO();
            RoleBO roleBO = new RoleBO();
            List<Guid> ids = new List<Guid>();
            foreach (var userId in userIds)
            {
                if (!roleBO.RoleUserExists(userId, roleId))
                {
                    userBO.AppendRole(userId, roleId);
                    ids.Add(userId);
                }
            }



            var userlist = roleBO.GetRoleUsers(roleId);//.GetUsers   .Read
            List<StaffView> items = new List<StaffView>();
            //foreach (var item in userlist)
            //{
            //    string Department = "";
            //    string JobTitle = "";
            //    int JobRank = 0;
            //    IList<UserExFieldDTO> test = userBO.ReadUser(item.Id).ExFields;
            //    foreach (var fieldItem in test)
            //    {
            //        switch (fieldItem.PropertyName)
            //        {
            //            case "Department": Department = fieldItem.ValueString;
            //                break;
            //            case "JobTitle": JobTitle = fieldItem.ValueString;
            //                break;
            //            case "JobRank": JobRank = Convert.ToInt32(fieldItem.ValueNumber.Value);
            //                break;
            //        }
            //    }

            //    StaffView staff = new StaffView { StaffId = item.Id.ToString(), JobTitle = JobTitle, Department = Department, DisplayName = item.FirstName + " " + item.LastName, UserName = item.UserName };
            //    items.Add(staff);
            //}
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 从角色删除用户    --------批量
        /// <summary>
        /// 从角色 删除用户    --------批量
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public JsonResult RemoveUsersfromRole(Guid roleId, List<Guid> userIds)
        {
            UserBO userBO = new UserBO();
            RoleBO roleBO = new RoleBO();
            List<Guid> ids = new List<Guid>();
            foreach (var userId in userIds)
            {
                if (roleBO.RoleUserExists(userId, roleId))
                {
                    userBO.RemoveRole(userId, roleId);
                    ids.Add(userId);
                }
            }
            return Json(ids, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 保存编辑用户与角色的关联       -------批量
        public JsonResult SaveUsersTOrole(Guid roleId, List<Guid> addUserIds, List<Guid> removeUserIds)
        {
            UserBO userBO = new UserBO();
            RoleBO roleBO = new RoleBO();
            if (addUserIds != null)
                foreach (var userId in addUserIds)
                {
                    if (!roleBO.RoleUserExists(userId, roleId))
                    {
                        userBO.AppendRole(userId, roleId);
                    }
                }
            if (removeUserIds != null)
                foreach (var userId in removeUserIds)
                {
                    if (roleBO.RoleUserExists(userId, roleId))
                    {
                        userBO.RemoveRole(userId, roleId);
                    }
                }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  添加角色分类
        public JsonResult CreateRoleCategory(Category model)
        {
            RoleCategoryWithRelationshipsDto Parent = null;
            Guid ParentId;
            if (Guid.TryParse(model.ParentID, out ParentId))
            {
                Parent = new RoleCategoryWithRelationshipsDto() { SysID = ParentId };
            }
            RoleCategoryWithRelationshipsDto roleCategeory = new RoleCategoryWithRelationshipsDto { SysID = Guid.NewGuid(), Name = model.DisplayName, Parent = Parent };
            model.CategoryID = roleBO.CreateCategory(roleCategeory).ToString();
            RoleTree item = new RoleTree()
            {
                Type = RoleType.Category.ToString(),
                ParentID = model.ParentID,
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 编辑角色分类
        public JsonResult UpdateRoleCategory(Category model)
        {
            Guid curCategoryId = Guid.Parse(model.CategoryID);
            var curCategory = roleBO.GetRoleCategoryWithParent(curCategoryId);

            RoleCategoryWithRelationshipsDto updateCategory = new RoleCategoryWithRelationshipsDto()
            {
                SysID = curCategoryId,
                Name = model.DisplayName
            };
            roleBO.UpdateCategory(updateCategory);

            RoleTree item = new RoleTree()
            {
                Type = RoleType.Category.ToString(),
                ParentID = curCategory.Parent == null ? null : curCategory.Parent.SysID.ToString(),
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  删除角色分类
        public JsonResult DeleteCategory(string categoryID)
        {            
            roleBO.DeleteCategory(Guid.Parse(categoryID));
            return Json(categoryID, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion



        public JsonResult GetAllUser()
        {
            RoleBO role = new RoleBO();
            UserBO userBO = new UserBO();
            var userlist = userBO.GetAllUsers();//.GetUsers   .Read
            List<StaffView> items = new List<StaffView>();
            //foreach (var item in userlist)
            //{
            //    string Department = "";
            //    string JobTitle = "";
            //    int JobRank = 0;
            //    IList<UserExFieldDTO> test = userBO.ReadUser(item.Id).ExFields;
            //    foreach (var fieldItem in test)
            //    {
            //        switch (fieldItem.PropertyName)
            //        {
            //            case "Department": Department = fieldItem.ValueString;
            //                break;
            //            case "JobTitle": JobTitle = fieldItem.ValueString;
            //                break;
            //            case "JobRank": JobRank = Convert.ToInt32(fieldItem.ValueNumber.Value);
            //                break;
            //        }
            //    }

            //    StaffView staff = new StaffView { StaffId = item.Id.ToString(), JobTitle = JobTitle, Department = Department, DisplayName = item.FirstName + " " + item.LastName, UserName = item.UserName };
            //    items.Add(staff);
            //}
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserByRole(string id)
        {
            //IEnumerable<Models.ViewModel.StaffView> items = createRandomStaffData(10);
            RoleBO role = new RoleBO();
            UserBO userBO = new UserBO();
            var userlist = role.GetRoleUsers(Guid.Parse(id));//.GetUsers   .Read
            List<StaffView> items = new List<StaffView>();
            foreach (var item in userlist)
            {
                StaffView staff = new StaffView { StaffId = item.SysID.ToString(), DisplayName = item.FullName, UserName = item.UserName };
                items.Add(staff);
            }
            return Json(items.OrderBy(x=>x.DisplayName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleUsersWithPage(string id,int page = 1, int pageSize = 20,
            [ModelBinder(typeof(JsonListBinder<ListFilter>))]IList<ListFilter> filter = null)
        {            
            RoleBO role = new RoleBO();
            UserBO userBO = new UserBO();
            int total = 0;
            if(filter!=null&&filter.Count>0)
            {
                ListFilterOperator oper=filter[0].Operator;
                string value=filter[0].Value.ToString();
                filter.Clear();
                filter.Add(new ListFilter() { Field = "FirstName", Operator = oper, Value = value });                
                filter.Add(new ListFilter() { Field = "LastName", Operator = oper, Value = value, logic=LinkLogic.Or });
            }
            Expression<Func<Framework.Organization.UserManagement.User, bool>> express = ListFilterExpress.GetFilterExpress<Framework.Organization.UserManagement.User>((filter == null ? new List<ListFilter>() : filter.ToList()));
            var userlist = role.GetRoleUsersWithPage(Guid.Parse(id), express, page, pageSize, out total);//.GetUsers   .Read
            List<StaffView> items = new List<StaffView>();
            foreach (var item in userlist)
            {
                StaffView staff = new StaffView { StaffId = item.SysID.ToString(), DisplayName = item.FullName, UserName = item.UserName };
                items.Add(staff);
            }
            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetDefaultPage(Guid id, string pane, bool statu)
        {
            MenuFacade menuFacade = new MenuFacade();
            menuFacade.SetDefaultPage(id, statu);



            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            var DefaultPage = menuFacade.GetMenuDefaultPage(pane);
            var tt = DefaultPage.Substring(15);
            return Json(WidgetFacade.GetAllWidget(pane).Select(s => new
            {
                DisplayName = s.DisplayName,
                Description = s.Description,
                ID = s.ID,
                Key = s.Key,
                MenuID = s.MenuID,
                RazorContent = s.RazorContent,
                Statu = DefaultPage.Substring(15) == s.Key ? true : false
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRelevanceWidgetList(string pane)
        {
            // string s2 = s.Substring(6, 2);
            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            MenuFacade menuFacade = new MenuFacade();
            var DefaultPage = menuFacade.GetMenuDefaultPage(pane);
            var tt = DefaultPage.Substring(15);
            return Json(WidgetFacade.GetAllWidget(pane).Select(s => new
            {
                DisplayName = s.DisplayName,
                Description = s.Description,
                ID = s.ID,
                Key = s.Key,
                MenuID = s.MenuID,
                RazorContent = s.RazorContent,
                Statu = DefaultPage.Substring(15) == s.Key ? true : false
            }), JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult AddWidget(DynamicWidget item, string Pane)
        {
            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            item.ID = Guid.NewGuid();
            WidgetFacade.AddWidget(item, Pane);
            return Json(WidgetFacade.GetWidgetByID(item.ID), JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult EditWidget(DynamicWidget item)
        {
            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            WidgetFacade.EditWidget(item);
            return Json(WidgetFacade.GetWidgetByID(item.ID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteWidget(List<Guid> idList, string pane)
        {
            DynamicWidgetFacade WidgetFacade = new DynamicWidgetFacade();
            WidgetFacade.DelWidget(idList);
            return Json(WidgetFacade.GetAllWidget(pane), JsonRequestBehavior.AllowGet);
        }



        #region delegate
        public JsonResult GetProcess(string pane)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            AppDelegateFacade appelegate = new AppDelegateFacade(this.AuthType);
            var processNames = appelegate.GetDelegateByPane(pane);
            //var processNames = new List<string>() { "bms_Process\\bms" };
            WorkflowManagementService wfMngService = new WorkflowManagementService(this.AuthType);
            var processset = wfMngService.GetProcessSets();
            //List<ProcessSet> processset = new List<ProcessSet>() { 
            //new ProcessSet(){ FullName="AMS_Process\\AMS",Name="AMS"},
            //new ProcessSet(){ FullName="bms_Process\\bms",Name="AMS"},
            //};
            var processItems = processset.Select(s => new { Name = s.FullName,DisplayName=svc.GetProcessSetByFullName(this.CurrentUser,s.FullName), Statu = processNames.Contains(s.FullName) ? true : false });
            return Json(processItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDelegateByPane(string pane, [ModelBinder(typeof(JsonListBinder<TreeViews>))]List<TreeViews> Items) //ITEMS只是用到id与status 其余是没有用的。省去了自身的model
        {
            AppDelegateFacade appelegate = new AppDelegateFacade(this.AuthType);
            List<ProcessSet> processlist=appelegate.GetProcessSetByPane(pane);

            foreach (var process in processlist)
            {
                TreeViews tv = Items.Where(x => x.ID == process.FullName).FirstOrDefault();
                if (tv == null)
                {
                    appelegate.DelAppDelegateByListID(process.FullName, pane);
                }
            }
            foreach (var item in Items)
            {               
                if (item.Status == true)
                {
                    appelegate.AddAppDelegate(HttpUtility.UrlDecode(item.ID), pane);
                }
                else
                {
                    appelegate.DelAppDelegateByListID(HttpUtility.UrlDecode(item.ID), pane);
                }                
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetAllProcess()
        //{
        //    AppDelegateFacade appelegate = new AppDelegateFacade();
        //    var processNames = appelegate.GetAllDelegate().Select(s => s.FullName).ToList();
        //    return Json(processNames, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult AddProcess(string pane, string name)
        //{
        //    AppDelegateFacade appelegate = new AppDelegateFacade();
        //    appelegate.AddAppDelegate(name, pane);
        //    return Json(new { Name = name }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult DelProcess(string pane, List<string> ListName)
        //{
        //    AppDelegateFacade appelegate = new AppDelegateFacade();
        //    appelegate.DelAppDelegateByListID(ListName, pane);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
        #endregion
    }
}
