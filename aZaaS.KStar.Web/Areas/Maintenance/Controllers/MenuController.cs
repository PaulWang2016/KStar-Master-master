using aZaaS.KStar;
using aZaaS.Framework.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Facades;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Web.Models.ViewModel;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class MenuController : BaseMvcController
    {

        private ResourceFacade rf = new ResourceFacade();


        #region 获取权限管理菜单树
        /// <summary>
        /// 获取权限管理菜单树
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetMenus(string RoleID, string Type, string ID = "")
        {
            List<Resource> Nodes;

            if (ID == "")
                Nodes = rf.GetTreeResources(null, Type);
            else
                Nodes = rf.GetTreeResources(Guid.Parse(ID), Type);
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

        #region 添加 菜单
        /// <summary>
        /// 添加 菜单
        /// </summary>
        [ValidateInput(false)]
        public JsonResult AddMenu(TreeViews item)
        {
            Resource resource = new Resource();
            Guid id = Guid.NewGuid();
            if (item.ID != null)
            {
                id = Guid.Parse(item.ID);
            }
            resource.ID = id;
            resource.DisplayName = item.Name;
            resource.Links = item.Link;
            resource.ParentID = Guid.Parse(item.ParentID);
            resource.Type = item.Type;
            resource.Target = item.Target;
            rf.AddResource(resource);
            item.ID = id.ToString();
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
            Resource resource = new Resource();
            resource.ID = Guid.Parse(item.ID);
            resource.DisplayName = item.Name;
            resource.Links = item.Link;
            resource.Target = item.Target;
            resource.ParentID = Guid.Parse(item.ParentID);
            rf.EditResource(resource);
            //TreeViews model = HomeController.MenuList.SingleOrDefault(p => p.ID == item.ID);
            //model.Name = item.Name;
            //model.Link = item.Link;
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  删除 菜单    --------批量
        /// <summary>
        /// 删除 菜单    --------批量
        /// </summary>
        public JsonResult DeleteMenu(string ListID)//List<string> 
        {
            //foreach (var id in ListID)
            //{
            //    //var resourcePermission = acs.ResourcePermissions().Where(m => m.ResourceId == id).FirstOrDefault();
            // //resourcePermission.SysId;
            //if (resourcePermission != null)
            //{
            //    var authorization = acs.AuthorizationMatrixes().Where(m => m.ResourcePermissionSysId == resourcePermission.SysId).First();
            //    if (authorization != null)
            //        acs.RemoveAuthorizationMatrixes(authorization);
            //    acs.RemoveResourcePermissions(resourcePermission);
            //}
            rf.DeleteResource(Guid.Parse(ListID));
            //    //HomeController.MenuList.Remove(HomeController.MenuList.Where(o => o.ID == ListID[i].ToString()).FirstOrDefault());
            //}
            return Json(ListID, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public JsonResult SaveConfiguration(string RoleID, TreeViews node)// string Type,
        {
            AcsFacade acsFacade = new AcsFacade();
            acsFacade.ChangeStatus(RoleID, node.ID, node.Status);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


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
            var afterupdaterole = roleBO.ReadRole(Guid.Parse(item.ID));
            return Json(new SysRole { RoleID = afterupdaterole.SysID.ToString(), DisplayName = afterupdaterole.Name }, JsonRequestBehavior.AllowGet);
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
                var newRoleID = roleBO.CreateRole(new RoleWithRelationshipsDto { SysID = Guid.NewGuid(), Name = item.Name });
            }


            AppRoleFacade appRoleFacade = new AppRoleFacade();
            var ID = roleBO.GetAllRoles().Where(m => m.Name == item.Name).Select(s => s.SysID).FirstOrDefault();
            appRoleFacade.AddAppRole(ID, Pane);
            var roleItem = roleBO.ReadRole(ID);

            return Json(new SysRole() { RoleID = roleItem.SysID.ToString(), DisplayName = roleItem.Name }, JsonRequestBehavior.AllowGet);
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

            List<SysRole> SysRoleList = new List<SysRole>();

            foreach (var item in appRoleFacade.GetRoleByPane(Pane))
            {
                SysRole roleitem = new SysRole { DisplayName = item.DisplayName, RoleID = item.RoleID.ToString() };
                SysRoleList.Add(roleitem);
            }
            return Json(SysRoleList, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 获取 系统角色 列表
        /// <summary>
        /// 获取 系统角色 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleList()
        {
            RoleBO roleBO = new RoleBO();
            List<SysRole> SysRoleList = new List<SysRole>();
            foreach (var item in roleBO.GetAllRoles())
            {
                SysRole roleitem = new SysRole { DisplayName = item.Name, RoleID = item.SysID.ToString() };
                SysRoleList.Add(roleitem);
            }
            return Json(SysRoleList, JsonRequestBehavior.AllowGet);
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
            return Json(SysRoleList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult ExportPermission(Guid roleId)
        {

            //using (FileStream fs = new FileStream(@"C:\Users\MaxiTai_YeFeng\Desktop\newsql\qqq.xlsx", FileMode.OpenOrCreate))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        SqlConnection con = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=aZaaSBlackshell;Persist Security Info=True;User ID=sa;Password=MaxiTai123;");
            //        SqlCommand cmd = new SqlCommand("select * from MenuItem", con);
            //        con.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        DataTable tb = new DataTable();
            //        tb.Load(reader);
            //        reader.Close();
            //        cmd.Connection.Close();

            //        ExportExcel(tb, sw);
            //    }
            //}
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
