using aZaaS.KStar;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Extend;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Utilities;
using System.Reflection;
using System.Linq.Expressions;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Facades;
using System.Configuration;
using System.Data;
using System.Text;
using aZaaS.KStar.Localization;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class StaffController : BaseMvcController
    {
        private readonly PositionService positionService;
        private readonly UserService userService;
        private readonly RoleService roleService;        

        UserBO userBO = new UserBO();
        PositionBO positionBO = new PositionBO();
        OrgChartBO chartBO = new OrgChartBO();
        //RoleBO roleBO = new RoleBO();
        protected string Take
        {
            get
            {
                return Request["take"] ?? string.Empty;
            }
        }

        protected string Skip
        {
            get
            {
                return Request["skip"] ?? string.Empty;
            }
        }

        protected string Page
        {
            get
            {
                return Request["page"] ?? string.Empty;
            }
        }

        protected string PageSize
        {
            get
            {
                return Request["pageSize"] ?? string.Empty;
            }
        }

        protected string Sort
        {
            get
            {
                return Request["sort"] ?? string.Empty;
            }
        }
        //sort[0][field]:Folio
        //sort[0][dir]:desc
        //sort[0][compare]:
        protected List<string> Filter
        {
            get
            {
                if (HttpContext.Request.Cookies.Get("UserManaViewArrayInFilters") != null)
                {
                    string UserManaViewArrayInFilters = Server.UrlDecode(HttpContext.Request.Cookies.Get("UserManaViewArrayInFilters").Value);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(UserManaViewArrayInFilters);
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        //filter[logic]:and
        //filter[filters][0][field]:Requester
        //filter[filters][0][operator]:eq
        //filter[filters][0][value]:d


        public StaffController()
        {
            this.positionService = new PositionService();
            this.userService = new UserService();
            this.roleService = new RoleService();
        }

        #region 获取 User Management    获取员工列表
        /// <summary>
        /// 获取 User Management 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStaff()
        {
            List<StaffView> items = new List<StaffView>();

            var userlist = userService.GetUsersWithRelationships();// userBO.GetAllUsers();
            foreach (var item in userlist)
            {
                StaffView staffItem = new StaffView
                {
                    UserName = item.UserName,
                    Remark = item.Remark,
                    Sex = (item.Sex == "Male" ? "Male" : "Female"),
                    StaffId = item.SysID.ToString(),
                    Status = (item.Status == "True" ? true : false),
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    MobileNo = item.Phone,
                    DisplayName = item.FullName,
                    Department = ((item.Nodes == null || item.Nodes.Count == 0) ? "" : item.Nodes.FirstOrDefault().Name),
                    Position = ((item.Positions == null || item.Positions.Count == 0) ? "" : item.Positions.FirstOrDefault().Name),
                    ReportTo = ((item.ReportTo == null || item.ReportTo.Count == 0) ? "" : item.ReportTo.FirstOrDefault().FullName)
                };

                items.Add(staffItem);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffExtendFields()
        {
            FieldBase[] fields = userService.GetUserExtendFieldsDefition();
            List<ExtensionFieldDto> items = new List<ExtensionFieldDto>();
            foreach (FieldBase field in fields)
            {
                items.Add(new ExtensionFieldDto() { Name = field.Name, Value = field.Value, Type = field.GetType().Name, DisplayName = field.DisplayName });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffs(int take = 15, int skip = 0, int page = 1, int pageSize = 15,
            [ModelBinder(typeof(JsonListBinder<ListFilter>))]IList<ListFilter> filter = null,
            [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            int total = 0;
            IEnumerable<UserWithRelationshipsDto> userlist = new List<UserWithRelationshipsDto>();
            
            Expression<Func<Framework.Organization.UserManagement.User, bool>> express = ListFilterExpress.GetFilterExpress<Framework.Organization.UserManagement.User>((filter == null ? new List<ListFilter>() : filter.ToList()));

            total = userService.QueryUserCount(express);
            userlist = userService.GetUsersWithRelationships(express, page, pageSize);

            //排序
            if (sort != null && sort.Count > 0)
            {
                string sortfield = string.Empty;
                foreach (SortDescriptor s in sort)
                {
                    switch (s.field)
                    {
                        case "MobileNo":
                            sortfield = "Phone";
                            break;
                        case "DisplayName":
                            sortfield = "FullName";
                            break;
                        default:
                            sortfield = s.field;
                            break;
                    }
                    userlist = DataSorting<UserWithRelationshipsDto>(userlist.AsQueryable(), sortfield, s.dir);
                }
            }
            //默认排序
            else
            {
                userlist = DataSorting<UserWithRelationshipsDto>(userlist.AsQueryable(), "CreateDate", "desc");
            }
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllStaffsForExcel()
        {
            List<ListFilter> filter = new List<ListFilter>();
            if (Filter.Count > 0)
            {
                foreach (var item in Filter)
                {
                    string[] filteritems = item.Split('.');
                    filter.Add(new ListFilter() { Field = filteritems[0], Operator = (ListFilterOperator)Enum.Parse(typeof(ListFilterOperator), filteritems[1]), Value = filteritems[2] });
                }
            }
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            int total = 0;
            IEnumerable<UserWithRelationshipsDto> userlist = new List<UserWithRelationshipsDto>();            

            Expression<Func<Framework.Organization.UserManagement.User, bool>> express = ListFilterExpress.GetFilterExpress<Framework.Organization.UserManagement.User>(filter);

            total = userService.QueryUserCount(express);
            userlist = userService.GetUsersWithRelationships(express, 1, total);
            
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region   从excel导入user
        public JsonResult UserImportFromExcel()
        {
            bool flag = true;
            StringBuilder msg = new StringBuilder();
            var path = Server.MapPath("/") + @"TempFiles\User\";
            var fileName = Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf("\\") + 1);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Request.Files[0].SaveAs(path + fileName);
            }
            //读取excel文件验证格式
            Guid roleId = Guid.Parse("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8");
            if (System.IO.File.Exists(path + fileName))
            {
                DataTable dt = ExcelHelper.ExcelToTable(path + fileName);
                System.IO.File.Delete(path + fileName);
                if (dt.Rows.Count > 0)
                {
                    List<string> userids = new List<string>();
                    List<string> usernames = new List<string>();
                    string templateerror = string.Empty;
                    string existsstaffname = string.Empty;
                    string existsuserid = string.Empty;

                    string mapPath = Server.MapPath("~");
                    string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
                    string cultureName = ResxService.GetAvailableCulture();
                    string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/Parts/_StaffView_cshtml." + cultureName + ".resx");
                    FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
                    if (fi_parentCulture.Exists)
                    {
                        templateerror = ResxService.GetResouces("Templateerror", cshtmlResxFilePath);
                        existsstaffname = ResxService.GetResouces("ExistsStaffName", cshtmlResxFilePath);
                        existsuserid = ResxService.GetResouces("ExistsUserId", cshtmlResxFilePath);
                    }                    
                    //验证列数是否与标准模板匹配
                    if (dt.Columns.Count != 9)
                    {
                        flag = false;
                        msg.Append(templateerror);
                    }
                    else
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Guid id = Guid.NewGuid();
                            if (ExistUserId(dt.Rows[i]["UserId"].ToString()))
                            {
                                userids.Add(dt.Rows[i]["UserId"].ToString());
                                continue;
                            }

                            if (ExistsUserName(dt.Rows[i]["UserName"].ToString()))
                            {
                                usernames.Add(dt.Rows[i]["UserName"].ToString());
                                continue;
                            }
                            UserBaseDto user = new UserBaseDto
                            {
                                SysID = id,
                                UserId = dt.Rows[i]["UserId"].ToString(),
                                UserName = dt.Rows[i]["UserName"].ToString(),
                                LastName = dt.Rows[i]["LastName"].ToString(),
                                FirstName = dt.Rows[i]["FirstName"].ToString(),
                                Email = dt.Rows[i]["Email"].ToString(),
                                Phone = dt.Rows[i]["Phone"].ToString(),
                                Sex = dt.Rows[i]["Sex"].ToString(),
                                Status = dt.Rows[i]["Status"].ToString(),
                                Remark = dt.Rows[i]["Remark"].ToString(),
                                CreateDate = DateTime.Now
                            };
                            var userID = userBO.CreateUser(user);
                            userBO.InitUserPassword(user.UserName,PortalEnvironment.FormPassWord);
                            if (userID != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                            {
                                userBO.AppendRole(userID, roleId);
                                CacheHelper.AddUserCache(user.UserName, user.FullName);
                            }
                        }
                        if (userids.Count > 0)
                        {
                            msg.Append(existsuserid + ":" + string.Join(",", userids.ToArray()));
                        }
                        if (usernames.Count > 0)
                        {
                            msg.Append(existsstaffname+":" + string.Join(",", usernames.ToArray()));
                        }
                    }
                }
            }
            return Json(new {flag=flag,msg=msg.ToString()}, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 模糊  查找人员
        /// <summary>
        /// 模糊  查找人员
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost] //****修改成各个字段
        public JsonResult FindStaffs(string input)
        {
            IEnumerable<UserWithFieldsDto> userlist;
            if (input == "")
                userlist = userService.GetUsersWithFields();//userBO.GetAllUsers();
            else
            {
                userlist = userService.GetUsersWithFields((t => t.FirstName.Contains(input) || t.LastName.Contains(input))); //userBO.GetUsers(exp);
            }
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindNameStaff(string input)
        {
            IEnumerable<UserWithFieldsDto> userlist;
            if (input == "")
                userlist = userService.GetUsersWithFields();//userBO.GetAllUsers();
            else
            {
                userlist = userService.GetUsersWithFields((t => t.FirstName.Contains(input) || t.LastName.Contains(input))); //userBO.GetUsers(exp);
            }
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindNameStaffs(string input, int take = 15, int skip = 0, int page = 1, int pageSize = 15)
        {
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            int total = 0;
            IEnumerable<UserWithRelationshipsDto> userlist;
            if (input == "")
            {
                total = userService.GetUserCount();//userBO.GetUserCount();
                userlist = userService.GetUsersWithRelationships(page, pageSize);//userBO.GetAllUsers(page, pageSize);
            }
            else
            {
                input = System.Text.RegularExpressions.Regex.Replace(input, @"\s", string.Empty);
                total = userService.QueryUserCount((t => t.Email.Contains(input) || t.UserName.Contains(input) || (t.LastName + t.FirstName).Contains(input) || (t.FirstName + t.LastName).Contains(input) || t.UserId.Contains(input)));// userBO.QueryUserCount(exp);
                userlist = userService.GetUsersWithRelationships((t => t.Email.Contains(input) || t.UserName.Contains(input) || (t.FirstName + t.LastName).Contains(input) || (t.LastName + t.FirstName).Contains(input) || t.UserId.Contains(input)), page, pageSize);// userBO.GetUsers(exp, page, pageSize);
            }
            userlist = DataSorting<UserWithRelationshipsDto>(userlist.AsQueryable(), "CreateDate", "desc");
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindNameStaffsFilterStatus(string input, int take = 15, int skip = 0, int page = 1, int pageSize = 15)
        {
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            int total = 0;
            IEnumerable<UserWithFieldsDto> userlist;
            if (input == "")
            {
                total = userService.GetUserCount();//userBO.GetUserCount();
                userlist = userService.GetUsersWithFields(page, pageSize);//userBO.GetAllUsers(page, pageSize);
            }
            else
            {
                total = userService.QueryUserCount((t => t.FirstName.Contains(input) || t.LastName.Contains(input)));// userBO.QueryUserCount(exp);
                userlist = userService.GetUsersWithFields((t => t.FirstName.Contains(input) || t.LastName.Contains(input)), page, pageSize);// userBO.GetUsers(exp, page, pageSize);
            }
            foreach (var item in userlist)
            {
                items.Add(GetStaffView(item));
            }

            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 员工 其他信息    GetStaffOtherInfo(string id)
        /// <summary>
        /// 获取 员工 其他信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetStaffOtherInfo(string id)
        {
            int PositionID = 1;

            List<StaffView> stafflist = new List<StaffView>();
            List<SysRole> rolelist = new List<SysRole>();
            List<Position> positionlist = new List<Position>();
            List<Department> departmentlist = new List<Department>();
            //UserDTO userDTO = userBO.ReadUser(Guid.Parse(id));

            IList<ExtensionFieldDto> extenditems = userService.ReadUserWithFields(Guid.Parse(id)).ExtendItems;
            var userpositions = userService.ReadUserWithPositions(Guid.Parse(id)).Positions;// userDTO.Positions;
            var userroles = userService.ReadUserWithRoles(Guid.Parse(id)).Roles;//userDTO.Roles;
            var userowner = userService.ReadUserWithOwners(Guid.Parse(id)).ReportTo;// userDTO.Owners;
            var userdepartments = userService.ReadUserWithNodes(Guid.Parse(id)).Nodes;

            foreach (var item in userpositions)
            {
                Position position = new Position { DisplayName = item.Name, PositionID = item.SysID.ToString() };//CategoryID = item.Category.Id.ToString(),
                positionlist.Add(position);
            }
            foreach (var item in userroles)
            {
                SysRole role = new SysRole { DisplayName = item.Name, RoleID = item.SysID.ToString() };
                rolelist.Add(role);
            }
            foreach (var item in userowner)
            {
                StaffView staff = new StaffView { StaffId = item.SysID.ToString(), DisplayName = item.FullName };
                stafflist.Add(staff);
            }
            foreach (var item in userdepartments)
            {
                Department depart = new Department() { DepartmentID = item.SysID.ToString(), DisplayName = item.Name };
                departmentlist.Add(depart);
            }

            return Json(new { PositionID = PositionID, ExtendItems = extenditems, ReportToList = stafflist, RoleList = rolelist, PositionList = positionlist, departmentlist = departmentlist }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //public StaffView GetStaffView(UserWithFieldsDto item)
        //{
        //    IList<ExtensionFieldDto> test = item.ExtendItems;            

        //    return new StaffView
        //      {
        //          UserName = item.UserName,
        //          Remark = item.Remark,
        //          Sex = (item.Sex == "Male" ? "Male" : "Female"),
        //          StaffId = item.SysID.ToString(),
        //          Status = (item.Status == "True" || item.Status == "true" ? true : false),
        //          FirstName = item.FirstName,
        //          LastName = item.LastName,
        //          Email = item.Email,
        //          MobileNo = item.Phone,
        //          DisplayName = item.FullName
        //      };

        //}

        public Dictionary<string, object> GetStaffView(UserWithRelationshipsDto item)
        {
            //职位数据过滤条件
            List<ListFilter> positionFilter = HttpRuntime.Cache["PositionDataFilter"] as List<ListFilter>;
            Expression<Func<PositionBaseDto, bool>> positionExpress = ListFilterExpress.GetFilterExpress<PositionBaseDto>(positionFilter);

            List<ListFilter> departFilter = HttpRuntime.Cache["DepartmentDataFilter"] as List<ListFilter>;
            Expression<Func<OrgNodeWithChartDto, bool>> departExpress = ListFilterExpress.GetFilterExpress<OrgNodeWithChartDto>(departFilter);

            IList<ExtensionFieldDto> test = item.ExtendItems;
            Dictionary<string, object> items = new Dictionary<string, object>();
            foreach (var e in test)
            {
                items.Add(e.Name, e.Value);
            }
            var person = new
            {
                UserId=(item.UserId??""),
                UserName = (item.UserName??""),
                Remark = (item.Remark??""),
                Sex = (item.Sex ?? ""),
                StaffId = item.SysID.ToString(),
                Status = (item.Status == "True" || item.Status == "true" ? true : false),
                FirstName = (item.FirstName??""),
                LastName = (item.LastName??""),
                Email = (item.Email??""),
                MobileNo = (item.Phone??""),
                DisplayName = (item.FullName??""),
                Department = ((item.Nodes == null || item.Nodes.Where(departExpress.Compile()).Count() == 0) ? "" : item.Nodes.FirstOrDefault().Name),
                Position = ((item.Positions == null || item.Positions.Where(positionExpress.Compile()).Count()== 0) ? "" : item.Positions.Where(positionExpress.Compile()).FirstOrDefault().Name),
                ReportTo = ((item.ReportTo == null || item.ReportTo.Count == 0) ? "" : (item.ReportTo.FirstOrDefault().FullName + (string.IsNullOrEmpty(item.ReportTo.FirstOrDefault().UserId) ?"":"(" + item.ReportTo.FirstOrDefault().UserId + ")")))
            };
            foreach (System.Reflection.PropertyInfo p in person.GetType().GetProperties())
            {
                items.Add(p.Name, p.GetValue(person));
            }

            return items;
        }

        public Dictionary<string, object> GetStaffView(UserWithFieldsDto item)
        {
            IList<ExtensionFieldDto> test = item.ExtendItems;
            Dictionary<string, object> items = new Dictionary<string, object>();
            foreach (var e in test)
            {
                items.Add(e.Name, e.Value);
            }
            var person = new
            {
                UserId=item.UserId,
                UserName = item.UserName,
                Remark = item.Remark,
                Sex = item.Sex,
                StaffId = item.SysID.ToString(),
                Status = (item.Status == "True" || item.Status == "true" ? true : false),
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                MobileNo = item.Phone,
                DisplayName = item.FullName
            };
            foreach (System.Reflection.PropertyInfo p in person.GetType().GetProperties())
            {
                items.Add(p.Name, p.GetValue(person));
            }

            return items;
        }

        #region 增加 员工
        /// <summary>
        /// 增加 员工
        /// </summary>
        /// <param name="staffItem">员工信息</param>
        /// <param name="ReportTo"></param>
        /// <param name="SystemRole"></param>
        /// <param name="Position"></param>
        /// <param name="Department"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddStaff(StaffView staffItem, [ModelBinder(typeof(JsonListBinder<UserExFieldDTO>))]IList<UserExFieldDTO> ExFields, List<string> ReportToidlist, List<string> Roleidlist, List<string> Positionidlist, List<string> Departmentidlist)
        {
            Guid id = Guid.NewGuid();
            UserBaseDto user = new UserBaseDto
            {
                SysID = id,
                UserId=staffItem.UserId,
                Sex = staffItem.Sex,
                Remark = staffItem.Remark,
                Address = staffItem.Address,
                UserName = staffItem.UserName,
                Email = staffItem.Email,
                Phone = staffItem.MobileNo,
                Status = staffItem.Status.ToString(),
                LastName = staffItem.LastName,
                FirstName = staffItem.FirstName,
                CreateDate=DateTime.Now
            };
            var userID = userBO.CreateStaff(user,this.AuthType,ReportToidlist,Positionidlist,Roleidlist,Departmentidlist,ExFields);
            if (userID != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                //添加缓存                
                CacheHelper.AddUserCache(user.UserName, user.FullName);
            }

            #region exfield
            //if (userID != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            //{
            //    //添加Everyone角色
            //    userBO.AppendRole(userID, Guid.Parse("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8"));
            //    CacheHelper.AddUserCache(user.UserName, user.LastName + " " + user.FirstName);
            //}

            ////如果为form验证登陆，添加用户时则同时更新form验证初始化密码
            //if (this.AuthType == AuthenticationType.Form)
            //{
            //    userBO.InitUserPassword(staffItem.UserName, PortalEnvironment.FormPassWord);
            //}

            //if (ReportToidlist != null)
            //{
            //    foreach (var item in ReportToidlist)
            //    {
            //        userBO.AppendOwner(userID, Guid.Parse(item));
            //    }
            //}
            //if (Positionidlist != null)
            //{
            //    foreach (var item in Positionidlist)
            //    {
            //        positionBO.AppendUser(Guid.Parse(item), userID);
            //    }
            //}
            //if (Roleidlist != null)
            //{
            //    foreach (var item in Roleidlist)
            //    {
            //        userBO.AppendRole(userID, Guid.Parse(item));
            //    }
            //}
            //if (Departmentidlist != null)
            //{
            //    foreach (var item in Departmentidlist)
            //    {
            //        chartBO.AppendUser(Guid.Parse(item),userID);
            //    }
            //}
            
            //if (ExFields != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            //{
            //    foreach (var item in ExFields)
            //    {
            //        userBO.AppendExField(userID, item);
            //    }
            //}
            #endregion
            var userWithFields = userService.ReadUserWithFields(user.SysID);// userBO.ReadUser(user.Id);
            return Json(GetStaffView(userWithFields), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AD导入用户
        public JsonResult ImportStaffFromAD()
        {
            var adUserList = ADUtility.GetAllUsers();
            Guid roleId = Guid.Parse("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8");
            foreach (var adUser in adUserList)
            {
                UserBaseDto user = new UserBaseDto
                {
                    SysID = Guid.NewGuid(),
                    Address = adUser.Value[ADUserProperty.Address.StateOrProvince] + adUser.Value[ADUserProperty.Address.City] + adUser.Value[ADUserProperty.Address.Street],
                    UserName = ConfigurationManager.AppSettings["WindowDomain"] + "\\" + adUser.Value[ADUserProperty.Account.UserLogonNamePreWin2000],
                    Email = adUser.Value[ADUserProperty.General.Email],
                    Phone = adUser.Value[ADUserProperty.Telephones.Mobile],
                    Status = "True",
                    LastName = adUser.Value[ADUserProperty.General.LastName],
                    FirstName = adUser.Value[ADUserProperty.General.FirstName],
                    CreateDate=DateTime.Now
                };
                var userID = userBO.CreateUser(user);
                if (userID != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    userBO.AppendRole(userID, roleId);
                    CacheHelper.AddUserCache(user.UserName, user.FullName);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }       
        #endregion

        #region 编辑 员工信息
        /// <summary>
        /// 编辑 员工信息
        /// </summary>
        /// <param name="staffItem"></param>
        /// <param name="ReportTo"></param>
        /// <param name="SystemRole"></param>
        /// <param name="Position"></param>
        /// <param name="Department"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditStaff(StaffView staffItem, [ModelBinder(typeof(JsonListBinder<UserExFieldDTO>))]IList<UserExFieldDTO> ExFields, List<string> ReportToidlist, List<string> Roleidlist, List<string> Positionidlist, List<string> Departmentidlist)
        {

            var useritem = new UserBaseDto();// userBO.ReadUser(Guid.Parse(staffItem.StaffId));
            useritem.SysID = Guid.Parse(staffItem.StaffId);
            useritem.Sex = staffItem.Sex;
            useritem.Remark = staffItem.Remark;
            useritem.Address = staffItem.Address;
            useritem.UserName = staffItem.UserName;
            useritem.Email = staffItem.Email;
            useritem.Phone = staffItem.MobileNo;
            useritem.Status = staffItem.Status.ToString();
            useritem.LastName = staffItem.LastName;
            useritem.FirstName = staffItem.FirstName;
            useritem.UserId = staffItem.UserId;

            userBO.UpdateUser(useritem);
            //更新缓存
            CacheHelper.UpdateUserCache(useritem.UserName, useritem.LastName + " " + useritem.FirstName);

            #region AppendExField
            var curUserInst = this.userService.ReadUserWithFields(useritem.SysID);
            var curUserFields = curUserInst.ExtendItems ?? new List<ExtensionFieldDto>();

            IList<UserExFieldDTO> exFieldDTOs = ExFields;
            foreach (var exFieldDTO in exFieldDTOs)
            {
                var oldExFieldDTO = curUserFields.SingleOrDefault(s => s.Name == exFieldDTO.Name);
                if (oldExFieldDTO != null)
                {
                    exFieldDTO.SysID = oldExFieldDTO.SysID;//取旧Id
                    userBO.UpdateExField(exFieldDTO);//更新
                    //TODO:
                    curUserFields.Remove(oldExFieldDTO);
                }
                else
                {
                    userBO.AppendExField(useritem.SysID, exFieldDTO);
                }
            }
            //把不存在的删除
            foreach (var exFieldDTO in curUserFields)
            {
                userBO.RemoveExField(useritem.SysID, exFieldDTO.Name);
            }
            #endregion

            //var afterUpdateUser = userBO.ReadUser(useritem.Id);
            var UserOwners = userService.ReadUserWithOwners(useritem.SysID).ReportTo;
            var UserPositions = userService.ReadUserWithPositions(useritem.SysID).Positions;
            var UserRoles = userService.ReadUserWithRoles(useritem.SysID).Roles;
            var UserDepartments = userService.ReadUserWithNodes(useritem.SysID).Nodes;

            if (UserOwners.ToList().Count > 0)
            {
                foreach (var item in UserOwners)
                {
                    userBO.RemoveOwner(useritem.SysID, item.SysID);
                }
            }
            if (UserPositions.ToList().Count > 0)
            {
                foreach (var item in UserPositions)
                {
                    positionBO.RemoveUser(item.SysID, useritem.SysID);
                }
            }
            if (UserRoles.ToList().Count > 0)
            {
                foreach (var item in UserRoles)
                {
                    userBO.RemoveRole(useritem.SysID, item.SysID);
                }
            }
            if (UserDepartments.ToList().Count > 0)
            {
                foreach (var item in UserDepartments)
                {
                    chartBO.RemoveUser(item.SysID, useritem.SysID);                    
                }
            }
            


            if (ReportToidlist != null)
            {
                foreach (var item in ReportToidlist)
                {
                    if (userBO.UserOwnerExists(Guid.Parse(item), useritem.SysID) == false)//.IsOwnerOfUser
                        userBO.AppendOwner(useritem.SysID, Guid.Parse(item));
                }
            }
            if (Positionidlist != null)
            {
                foreach (var item in Positionidlist)
                {
                    if (positionBO.PositionUserExists(useritem.SysID, Guid.Parse(item)) == false)//.IsUserOfPosition
                        positionBO.AppendUser(Guid.Parse(item), useritem.SysID);
                }
            }
            if (Roleidlist != null)
            {
                foreach (var item in Roleidlist)
                {
                    if (userBO.UserRoleExists(Guid.Parse(item), useritem.SysID) == false)//.IsRoleOfUser
                        userBO.AppendRole(useritem.SysID, Guid.Parse(item));
                }
            }
            if (Departmentidlist != null)
            {
                foreach (var item in Departmentidlist)
                {
                    if (chartBO.NodeUserExists(useritem.SysID, Guid.Parse(item)) == false)
                    {
                        chartBO.AppendUser(Guid.Parse(item), useritem.SysID);                        
                    }                  
                }
            }

            //positionBO.get
            var userWithFields = userService.ReadUserWithFields(useritem.SysID);// useritem = userBO.ReadUser(useritem.Id);
            return Json(GetStaffView(userWithFields), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 判断员工编号是否已经使用
        public JsonResult ExistStaffId(string userid)
        {
            bool flag = ExistUserId(userid);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public bool ExistUserId(string userid)
        {
            bool flag = true;
            var user = userService.GetUsersWithFields(t => t.UserId == userid);
            if (user == null || user.Count() == 0)
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 判断员工账号是否已经使用
        public JsonResult ExistsStaffName(string username)
        {
            bool flag = ExistsUserName(username);           
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public bool ExistsUserName(string username)
        {
            bool flag = true;
            var user = userService.GetUsersWithFields(t => t.UserName == username);
            if (user == null || user.Count() == 0)
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #region 批量停用 员工
        /// <summary>
        /// 批量停用 员工
        /// </summary>
        /// <param name="idList">员工idlist</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDisableStaff(List<string> idList)
        {
            foreach (var item in idList)
            {
                //var user = userService.ReadUser(Guid.Parse(item)); // userBO.ReadUser(Guid.Parse(item));

                //user.Status = "false";
                UserBaseDto user = userService.ReadUserBase(Guid.Parse(item));
                user.Status = "false";
                userBO.UpdateUser(user);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

         #region 批量激活 员工
        /// <summary>
        /// 批量激活 员工
        /// </summary>
        /// <param name="idList">员工idlist</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoActiveStaff(List<string> idList)
        {
            foreach (var item in idList)
            {
                UserBaseDto user = userService.ReadUserBase(Guid.Parse(item));
                user.Status = "true";
                userBO.UpdateUser(user);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion        

        #region 删除 员工    --------批量
        [HttpPost]
        public JsonResult DoDeleUsers(List<string> idList)
        {

            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            foreach (var item in idList)
            {
                UserBaseDto user = userBO.ReadUser(Guid.Parse(item));
                if (user != null)
                {
                    userBO.DeleteUser(Guid.Parse(item));
                    userBO.DeleteUserPassword(user.UserName);

                    try
                    {
                        svc.DeleteStartUserBySysId(Guid.Parse(item), Configuration_UserType.User);
                    }
                    catch (Exception ex) { }

                    //更新缓存
                    CacheHelper.DeleteUserCache(user.UserName);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 获取 系统角色 列表
        /// <summary>
        /// 获取 系统角色 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleList()
        {
            List<SysRole> SysRoleList = new List<SysRole>();
            foreach (var item in roleService.GetAllRoles())// roleBO.GetAllRoles())
            {
                SysRole roleitem = new SysRole { DisplayName = item.Name, RoleID = item.SysID.ToString() };
                SysRoleList.Add(roleitem);
            }
            return Json(SysRoleList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 职务 列表
        /// <summary>
        /// 获取 职务 列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPositionList()
        {
            List<Position> positionList = new List<Position>();
            var positionlist = positionService.GetAllPositonBases();// positionBO.GetAllPositons();
            foreach (var item in positionlist)
            {
                Position position = new Position { DisplayName = item.Name, PositionID = item.SysID.ToString() };
                positionList.Add(position);
            }
            return Json(positionList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  获取 用户列表 for  ReportTo
        /// <summary>
        /// 获取 用户列表 for  ReportTo
        /// </summary>
        /// <returns></returns>
        public JsonResult GetReportToList()
        {
            List<StaffView> stafflist = new List<StaffView>();

            foreach (var item in userBO.GetAllUsers())
            {
                StaffView staff = new StaffView { StaffId = item.SysID.ToString(), DisplayName = item.FullName };
                stafflist.Add(staff);
            }
            return Json(stafflist, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 获取 Add Delegation 中 Delegate to 的 AutoComplete 数据
        /// <summary>
        /// 获取 Add Delegation 中 Delegate to 的 AutoComplete 数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStaffNames()
        {
            var stafflist = userBO.GetAllUsers();

            return Json(stafflist.Select(s => new
            {
                StaffId = s.SysID.ToString(),
                DisplayName = s.FullName,
                UserName = s.UserName
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

    public class GridFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
