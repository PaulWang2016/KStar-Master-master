using aZaaS.Framework.Organization.Expressions;
using aZaaS.KStar;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.CustomRole.Models;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Helper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KStarFormMvcApplication.Controllers
{
    public class CustomPeopleController : Controller
    {
        //
        // GET: /CustomPeople/
        OrgChartBO chartBO = new OrgChartBO();
        PositionBO positionBO = new PositionBO();
        UserBO userBO = new UserBO();

        private readonly UserService userService;
        private readonly OrgChartService chartService;
        private readonly PositionService positionService;

        private readonly CustomManager customRoleService;        

        public CustomPeopleController()
        {
            this.userService = new UserService();
            this.chartService = new OrgChartService();
            this.positionService = new PositionService();
            this.customRoleService = new CustomManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrgChartTree(string ID = "0_0", bool isshownonreference = false, string tree = "")
        {
            IEnumerable<OrgNodeWithFieldsDto> nodeList;
            List<Organization> organizationlist = new List<Organization>();
            string[] Params = ID.Split('_');
            if (Params[0] == "0")
            {
                List<OrgNodeWithFieldsDto> nodes = new List<OrgNodeWithFieldsDto>();
                List<OrgNodeWithFieldsDto> tnode = new List<OrgNodeWithFieldsDto>();
                var charts = this.chartService.GetAllChartBases();
                foreach (var item in charts)
                {
                    OrgNodeWithFieldsDto node = new OrgNodeWithFieldsDto() { SysID = item.SysID, Name = item.Name, Type = "Root", ExtendItems = new List<ExtensionFieldDto>() };
                    nodes.Add(node);
                }
                //排序
                nodes = nodes.OrderBy(x => x.Name).ToList();
                foreach (var node in nodes)
                {
                    tnode.Add(node);
                }
                //添加自定义未分类
                if (isshownonreference && tree != "Department")
                {
                    //string nonname = string.Empty;
                    //string mapPath = Server.MapPath("~");
                    //string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
                    //string cultureName = "未分类";//ResxService.GetAvailableCulture();
                    //string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/Parts/_OrganizationView_cshtml." + cultureName + ".resx");
                    //FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
                    //if (fi_parentCulture.Exists)
                    //{
                    //    nonname = ResxService.GetResouces("NonReference", cshtmlResxFilePath);
                    //}
                    //else
                    //{
                    //    nonname = "NonReference";
                    //}
                    tnode.Add(new OrgNodeWithFieldsDto() { SysID = new Guid("00000000-0000-0000-0000-000000000000"), Name = "未分类", Type = "Root", ExtendItems = new List<ExtensionFieldDto>() });
                }
                
                nodeList = tnode;
            }
            else if (Params[0] == "1")
            {
                //未分类
                if (isshownonreference && Params[1] == "00000000-0000-0000-0000-000000000000")
                {
                    List<OrgNodeWithFieldsDto> temp = new List<OrgNodeWithFieldsDto>();
                    switch (tree)
                    {
                        case "Person":
                            var users = this.userService.GetNonReferenceUsers();
                            foreach (var user in users)
                            {
                                temp.Add(new OrgNodeWithFieldsDto() { SysID = user.SysID, Name = user.UserName, Type = "Person", ExtendItems = new List<ExtensionFieldDto>() });
                            }
                            nodeList = temp;
                            break;
                        case "Position":
                            var positions = this.positionService.GetNonReferencePositions();
                            foreach (var position in positions)
                            {
                                temp.Add(new OrgNodeWithFieldsDto() { SysID = position.SysID, Name = position.Name, Type = "Position", ExtendItems = new List<ExtensionFieldDto>() });
                            }
                            nodeList = temp;
                            break;
                        default:
                            nodeList = temp;
                            break;
                    }
                    //固定排序
                    if (nodeList != null)
                    {
                        nodeList = nodeList.OrderBy(x => x.Name);
                    }
                }
                else
                {
                    var exp = new LogicalExpression(LogicalKind.And)
                    {
                        Fields = new List<Field>
                        {
                             new Field("Type", "'Company'", "OrgNode", OperatorKind.Equal),    
                             new Field("Chart_SysId", "'"+Params[1]+"'", "OrgNode", OperatorKind.Equal)   
                        }
                    };
                    nodeList = this.chartService.GetNodesWithFields(exp);
                    //固定排序
                    if (nodeList != null)
                    {
                        nodeList = nodeList.OrderBy(x => x.Name);
                    }
                }
            }
            else
            {
                nodeList = this.chartService.GetChildNodesWithFields(Guid.Parse(Params[1]));
                //固定排序
                if (nodeList != null)
                {
                    nodeList = nodeList.OrderBy(x => x.Name);
                }
            }
            foreach (var item in nodeList)
            {
                string parentid = null;
                bool hasChildNode = false;
                string id = "1";

                if (item.Type == "Root")
                {
                    //未分类
                    if (isshownonreference && item.SysID == new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        hasChildNode = false;
                    }
                    else
                    {
                        var chart = this.chartService.ReadChartWithRoot(item.SysID);
                        if (chart.Root != null)
                        {
                            hasChildNode = true;
                        }
                    }
                }
                else
                {
                    //未分类
                    if (isshownonreference && Params[1] == "00000000-0000-0000-0000-000000000000")
                    {
                        hasChildNode = false;
                        id = "2";
                        parentid = "00000000-0000-0000-0000-000000000000";
                    }
                    else
                    {
                        var nodeWithParent = this.chartService.ReadNodeWithParent(item.SysID);
                        var nodeWithChildNodes = this.chartService.ReadNodeWithChildNodes(item.SysID);
                        if (nodeWithParent.Parent != null)
                            parentid = nodeWithParent.Parent.SysID.ToString();
                        hasChildNode = nodeWithChildNodes.ChildNodes != null && nodeWithChildNodes.ChildNodes.Count() > 0;
                        id = "2";
                    }
                }
                Organization organization = new Organization
                {
                    ID = id + "_" + item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = parentid,
                    Type = item.Type,
                    isParent = hasChildNode,
                    NodeName = item.Name,
                    ExFields = GetOrgExField(item.ExtendItems)
                };
                if (organization.ExFields != null && organization.ExFields.ToList().Count > 1)
                {
                    int index = 0;
                    var exfield = organization.ExFields.ToList().Where(x => x.Name == "排序编号").FirstOrDefault();
                    if (exfield != null)
                    {
                        int.TryParse(exfield.Value, out index);
                    }
                    organization.OrderBy = index;
                }
                organizationlist.Add(organization);
            }
            return Json(organizationlist.OrderBy(x=>x.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 岗位树形菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetPosition2(string ID = "")
        {            
            List<PositionTree> items = new List<PositionTree>();
            List<PositionCategoryWithChildCategoriesDto> categories = new List<PositionCategoryWithChildCategoriesDto>();
            List<PositionWithFieldsDto> positionlist = new List<PositionWithFieldsDto>();
            if (ID == "")
            {
                categories = this.positionService.GetAllCategoriesBase().OrderBy(x => x.Name).ToList();
            }
            else
            {
                categories = this.positionService.GetChildCategoriesBase(Guid.Parse(ID)).OrderBy(x => x.Name).ToList();
                positionlist = positionBO.GetPositionsWithFieldByCategory(Guid.Parse(ID)).ToList();

            }
            foreach (var item in categories)
            {
                items.Add(new PositionTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = (positionBO.ReadCategory(item.SysID).Positions.Count > 0 || item.ChildCategories.Count > 0) ? true : false,
                    isParent = (positionBO.ReadCategory(item.SysID).Positions.Count > 0 || item.ChildCategories.Count > 0) ? true : false,
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = (ID == "" ? null : ID),
                    Type = PositionType.Category.ToString()
                });
            }
            foreach (var item in positionlist)
            {

                var position = new PositionTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = false,
                    isParent = false,
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = null,
                    Type = PositionType.Position.ToString()
                };
                if (item.ExtendItems != null && item.ExtendItems.ToList().Count > 1)
                {
                    int index = 0;
                    var exfield = item.ExtendItems.ToList().Where(x => x.Name == "排序编号").FirstOrDefault();
                    if (exfield != null)
                    {
                        int.TryParse(exfield.Value, out index);
                    }
                    position.OrderBy = index;
                }
                items.Add(position);
            }
            return Json(items.OrderBy(x => x.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPosition(string ID = "")
        {
            
            List<PositionTree> items = new List<PositionTree>();
            IEnumerable<OrgNodeWithFieldsDto> nodeList;
            List<OrgNodeWithFieldsDto> nodes = new List<OrgNodeWithFieldsDto>();


            int type = 1;
            if (string.IsNullOrWhiteSpace(ID))
            {
                var charts = this.chartService.GetAllChartBases();
                foreach (var item in charts)
                {
                    OrgNodeWithFieldsDto node = new OrgNodeWithFieldsDto() { SysID = item.SysID, Name = item.Name, Type = "Root", ExtendItems = new List<ExtensionFieldDto>() };
                    nodes.Add(node); 
                }
                //排序
                nodeList = nodes.OrderBy(x => x.Name).ToList();
            }
            else
            {
                type = 2;
                string[] Params = ID.Split('_');
                if (Params[0] == "1")
                {
                    var exp = new LogicalExpression(LogicalKind.And)
                    {
                        Fields = new List<Field>
                        {
                             new Field("Type", "'Company'", "OrgNode", OperatorKind.Equal),    
                             new Field("Chart_SysId", "'"+Params[1]+"'", "OrgNode", OperatorKind.Equal)   
                        }
                    };
                    nodeList = this.chartService.GetNodesWithFields(exp);
              
                }
                else
                {
                  
                    nodeList = this.chartService.GetChildNodesWithFields(Guid.Parse(Params[1]));

                } 
            } 
            //固定排序
            if (nodeList != null)
            {
                nodeList = nodeList.OrderBy(x => x.Name);
            }

            foreach (var item in nodeList)
            {

                var OrderBy = 0;
                var ExFields = GetOrgExField(item.ExtendItems);
                if (ExFields != null && ExFields.ToList().Count > 1)
                {
                    int index = 0;
                    var exfield = ExFields.ToList().Where(x => x.Name == "排序编号").FirstOrDefault();
                    if (exfield != null)
                    {
                        int.TryParse(exfield.Value, out index);
                    }
                    OrderBy = index;
                }
                 var isParent=false;
                if(type==1){

                     var exp = new LogicalExpression(LogicalKind.And)
                    {
                        Fields = new List<Field>
                        {
                             new Field("Type", "'Company'", "OrgNode", OperatorKind.Equal),    
                             new Field("Chart_SysId", "'"+item.SysID+"'", "OrgNode", OperatorKind.Equal)   
                        }
                    };
                    isParent=    this.chartService.GetNodesWithFields(exp).Count()>0?true:false;
                }else{
                    isParent = this.chartService.GetChildNodesWithFields(item.SysID).Count() > 0 ? true : false;
                }
         
                items.Add(new PositionTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = false,
                    isParent = isParent,
                    ID =type+"_"+ item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = (item.SysID.ToString() == "" ? null : ID),
                    Type = PositionType.Category.ToString(),
                    OrderBy = OrderBy
                });  
            }
          
            return Json(items.OrderBy(x => x.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private IList<OrgExField> GetOrgExField(IList<ExtensionFieldDto> ExFields)
        {
            IList<OrgExField> items = new List<OrgExField>();
            foreach (var exField in ExFields)
            {
                items.Add(new OrgExField() { SysId = exField.SysID, Name = exField.Name, Value = exField.Value });
            }
            return items;
        }

        public JsonResult GetPositionByNode(string id)
        {
            List<Position> items = new List<Position>();
            var nodeWithPositions = this.chartService.ReadNodeWithPositions(Guid.Parse(id));
            var nodePositions = (nodeWithPositions == null ? new List<PositionBaseDto>() : (nodeWithPositions.Positions ?? new List<PositionBaseDto>()));
            nodePositions = nodePositions.OrderBy(x => x.Name).ToList();
            foreach (var position in nodePositions)
            {
                Position positionitem = new Position
                {
                    PositionID = position.SysID.ToString(),
                    DisplayName = position.Name
                };
                items.Add(positionitem);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByNode(string id)
        {
            List<StaffView> items = new List<StaffView>();
            var nodeWithUsers = this.chartService.ReadNodeWithUsers(Guid.Parse(id));
            var nodeUsers = (nodeWithUsers == null ? new List<UserBaseDto>() : (nodeWithUsers.Users ?? new List<UserBaseDto>()));
            nodeUsers = nodeUsers.OrderBy(x => x.UserName).ToList();
            foreach (var user in nodeUsers)
            {
                var userWithFields = this.userService.ReadUserWithFields(user.SysID);
                var userFields = userWithFields.ExtendItems ?? new List<ExtensionFieldDto>();
                StaffView staff = new StaffView
                {
                    UserName = user.UserName,
                    Remark = user.Remark,
                    Sex = (user.Sex == "Male" ? "Male" : "Female"),
                    StaffId = user.SysID.ToString(),
                    Status = (user.Status == "True" || user.Status == "true" ? true : false),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobileNo = user.Phone,
                    DisplayName = user.FullName
                };
                items.Add(staff);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }


        #region 获取自定义角色分类
        public JsonResult GetCustomRoleByCommonControl(string ID = "")
        {
            List<Organization> organizationlist = new List<Organization>();
            var categories = new List<CustomRoleCategory>();
            var classifyList = new List<CustomRoleClassify>();

            if (ID == "")
            {
                var date = customRoleService.GetCategoryByParentId(Guid.Empty);
                categories = date.ToList();
            }
            else
            {
                categories = customRoleService.GetCategoryByParentId(Guid.Parse(ID));
                classifyList = customRoleService.GetClassifyByCategoryId(Guid.Parse(ID)).OrderBy(r => r.RoleName).ToList();
            }

            foreach (var item in categories)
            {
                organizationlist.Add(new Organization()
                {
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = null,
                    Type = "Root",
                    isParent = (customRoleService.GetCategoryByParentId(item.SysID).Count() > 0) ? true : false,
                    NodeName = item.Name
                });
            }

            return Json(organizationlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassifyByCommonControl(Guid id, int pageIndex, int pageSize, string keyword = "", bool allowpage = true)
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            List<CustomRoleClassify> nodes = new List<CustomRoleClassify>();
            if (id == Guid.Empty)
            {
                nodes = customRoleService.GetEnabledClassify();
                nodes = nodes.Where(x => PinyinHelper.GetShortPinyin((x.RoleName ?? "")).Contains(keyword.ToLower()) || (x.RoleName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                nodes = customRoleService.GetEnabledClassifyByCategoryId(id);
                if (nodes != null)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        nodes = nodes.Where(x => PinyinHelper.GetShortPinyin((x.RoleName ?? "")).Contains(keyword.ToLower()) || (x.RoleName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        nodes = nodes.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                    nodes = nodes.OrderBy(x => x.RoleName).ToList();
                }
            }
            foreach (var node in nodes)
            {
                ListBoxItem item = new ListBoxItem { id = node.RoleKey.ToString(), text = node.RoleName, FirstName = "", LastName = "" };
                items.Add(item);
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 选人控件获取系统角色列表
        /// <summary>
        /// 选人控件获取系统角色列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRolesList(string ID = "0_0", string pane = "Dashboard")
        {
            List<Organization> organizationlist = new List<Organization>();
            string[] Params = ID.Split('_');
            if (Params[0] == "0")
            {
                //string nonname = string.Empty;
                //string mapPath = Server.MapPath("~");
                //string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
                //string cultureName = ResxService.GetAvailableCulture();
                //string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/Parts/_OrganizationView_cshtml." + cultureName + ".resx");
                //FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
                //if (fi_parentCulture.Exists)
                //{
                //    nonname = ResxService.GetResouces("SystemRoles", cshtmlResxFilePath);
                //}
                //else
                //{
                //    nonname = "SystemRoles";
                //}
                organizationlist.Add(new Organization
                {
                    ID = "1_00000000-0000-0000-0000-000000000000",
                    SysId = "00000000-0000-0000-0000-000000000000",
                    ParentID = null,
                    Type = "Role",
                    isParent = true,
                    NodeName = "未分类",
                    ExFields = null
                });                
            }
            else if (Params[0] == "1")
            {
                //RoleBO roleBO = new RoleBO();
                foreach (var item in GetSysRole(pane))
                {
                    Organization organization = new Organization
                    {
                        ID = "2_" + item.RoleID,
                        SysId = item.RoleID,
                        ParentID = "",
                        Type = "Role",
                        isParent = false,
                        NodeName = item.DisplayName,
                        ExFields = null
                    };
                    organizationlist.Add(organization);
                }
            }
            return Json(organizationlist.OrderBy(x => x.NodeName).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据自定义分类查找系统角色列表
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetRolesListByCategory(string ID, int pageIndex, int pageSize, string pane = "Dashboard", string keyword = "")
        {
            List<ListBoxItem> items = new List<ListBoxItem>();

            List<SysRole> roles = GetSysRole(pane);
            if (!string.IsNullOrEmpty(keyword))
            {
                roles = roles.Where(x => PinyinHelper.GetShortPinyin((x.DisplayName ?? "")).Contains(keyword.ToLower())  || (x.DisplayName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                roles = roles.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            foreach (var item in roles)
            {
                ListBoxItem listitem = new ListBoxItem
                {
                    id = item.RoleID,
                    text = item.DisplayName,
                    FirstName = "",
                    LastName = ""
                };
                items.Add(listitem);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private List<SysRole> GetSysRole(string pane)
        {
            List<SysRole> SysRoleList = new List<SysRole>();
            AppRoleFacade appRoleFacade = new AppRoleFacade();

            foreach (var item in appRoleFacade.GetRoleByPane(pane))
            {
                SysRole roleitem = new SysRole { DisplayName = item.DisplayName, RoleID = item.RoleID.ToString() };
                SysRoleList.Add(roleitem);
            }
            return SysRoleList;
        }
        #endregion

        #region 通用选人控件获取职位
        /// <summary>
        /// 通用选人控件获取职位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSelectPersonPositionByNode(string id, string type, int pageIndex, int pageSize, bool isshownonreference = false, string keyword = "", bool allowpage = true)
        {
            if (!allowpage)
            {
                pageIndex = 1;
                pageSize = int.MaxValue;
            }
            id = id.Contains("_") ? id.Split('_')[1] : id;
            List<ListBoxItem> items = new List<ListBoxItem>();
            List<PositionBaseDto> positionTemp = new List<PositionBaseDto>();
            List<PositionBaseDto> positions = new List<PositionBaseDto>();
            if (id == "00000000-0000-0000-0000-000000000000")
            {
                var positionlist = positionBO.GetAllPositons();
                //positionlist = positionlist.Where(x => (x.Name.Contains(keyword) || PinyinHelper.GetShortPinyin(x.Name).Contains(keyword.ToLower()))).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                positionlist = positionlist.Where(x => (x.Name.Contains(keyword) || PinyinHelper.GetShortPinyin(x.Name).Contains(keyword.ToLower()))).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                foreach (var position in positionlist)
                {
                    positions.Add(new PositionBaseDto()
                    {
                        SysID = position.SysID,
                        Name = position.Name
                    });
                }
            }
            else
            {
                if (type == PositionType.Category.ToString())
                {
                    GetRecursivePosition(Guid.Parse(id), positions, positionTemp, pageIndex, pageSize, keyword, allowpage);
                }
                else if (type == PositionType.Position.ToString())
                {
                    PositionWithRelationshipsDto tposition = this.positionBO.ReadPosition(Guid.Parse(id));
                    if (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && (tposition.Name.Contains(keyword) || PinyinHelper.GetShortPinyin(tposition.Name).Contains(keyword.ToLower()))))
                    {
                        positions.Add(new PositionBaseDto() { SysID = tposition.SysID, Name = tposition.Name });
                        positions = positions.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }
                else
                {
                    GetRecursivePosition(Guid.Parse(id), positions, positionTemp, pageIndex, pageSize, keyword, allowpage);
                }
            }
            if (positions != null)
            {
                positions = positions.OrderBy(x => x.Name).ToList();
            }
            foreach (var position in positions)
            {
                ListBoxItem positionitem = new ListBoxItem
                {
                    id = position.SysID.ToString(),
                    text = position.Name,
                    FirstName = "",
                    LastName = ""
                };
                items.Add(positionitem);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private void GetRecursivePosition(Guid id, List<PositionBaseDto> positions, List<PositionBaseDto> positionTemp, int pageIndex, int pageSize, string keyword = "", bool allowpage = true)
        {
            if (positions == null)
            {
                return;
            }
            if (allowpage && positions.Count >= pageSize)
            {
                return;
            }
            var childcategory = this.positionService.GetChildCategoriesBase(id).OrderBy(x => x.Name).ToList();
            var category = positionBO.ReadCategory(id);
            var positionlist = category == null ? new List<PositionBaseDto>() : category.Positions.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < positionlist.Count; i++)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (positionTemp.Count >= ((pageIndex - 1) * pageSize) && ((positionlist[i].Name == null ? "" : positionlist[i].Name.ToUpper()).Contains(keyword.ToUpper()) || PinyinHelper.GetShortPinyin((positionlist[i].Name ?? "")).Contains(keyword.ToLower()) ))
                    {
                        positions.Add(positionlist[i]);
                    }
                }
                else
                {
                    if (positionTemp.Count >= ((pageIndex - 1) * pageSize))
                    {
                        positions.Add(positionlist[i]);
                    }
                }
                if (allowpage && positions.Count >= pageSize)
                {
                    break;
                }
                positionTemp.Add(positionlist[i]);
            }
            if (allowpage && positions.Count >= pageSize)
            {
                return;
            }
            if (childcategory.Count > 0)
            {
                foreach (var child in childcategory)
                {
                    GetRecursivePosition(child.SysID, positions,positionTemp,pageIndex,pageSize, keyword, allowpage);
                }
            }
        }

        #endregion


        #region 通用选人控件获取人员
        /// <summary>
        /// 通用选人控件获取人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSelectPersonUserByNode(string id, string type, int pageIndex, int pageSize, bool isshownonreference = false, string keyword = "", bool allowpage = true)
        {
            if (!allowpage)
            {
                pageIndex = 1;
                pageSize = int.MaxValue;
            }
            List<ListBoxItem> items = new List<ListBoxItem>();
            List<UserBaseDto> userTemp = new List<UserBaseDto>();
            List<UserBaseDto> nodeUsers = new List<UserBaseDto>();
            if (string.IsNullOrEmpty(id))
            {
                IEnumerable<UserBaseDto> userlist = userService.GetAllUsers();
                 userlist=  userlist.Where(x => x.Status == "True");
                //IEnumerable<UserWithRelationshipsDto>  userlist = userService.GetUsersWithRelationships((t => t.Email.Contains(keyword) || t.UserName.Contains(keyword) || (t.FirstName + t.LastName).Contains(keyword) || (t.LastName + t.FirstName).Contains(keyword)), pageIndex, pageSize);                
                if (userlist != null)
                {
                    //userlist = userlist.Where(x => PinyinHelper.GetShortPinyin(CustomHelper.UserNameFormat(x.LastName, x.FirstName, x.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((x.FirstName ?? "") + (x.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((x.LastName ?? "") + (x.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || x.UserName.ToUpper().Contains(keyword.ToUpper()) || (x.Email ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    userlist = userlist.Where(x => PinyinHelper.GetShortPinyin(CustomHelper.UserNameFormat(x.LastName, x.FirstName, x.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower())
                        || PinyinHelper.GetPinyin(CustomHelper.UserNameFormat(x.LastName, x.FirstName, x.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) 
                        || ((x.FirstName ?? "") + (x.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) 
                        || ((x.LastName ?? "") + (x.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) 
                        || x.UserName.ToUpper().Contains(keyword.ToUpper()) 
                        || (x.Email ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    nodeUsers = userlist.ToList();
                }
            }
            else
            {
                if (type == "Root")
                {
                    //未分类
                    if (isshownonreference && id == "00000000-0000-0000-0000-000000000000")
                    {
                        var nonUsers = userService.GetNonReferenceUsers().ToList();
                        if (nonUsers != null)
                        {
                            if (!string.IsNullOrEmpty(keyword))
                            {
                                nodeUsers = nonUsers.Where(x => (PinyinHelper.GetShortPinyin(CustomHelper.UserNameFormat(x.LastName, x.FirstName, x.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) 
                                    || ((x.FirstName ?? "") + (x.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) 
                                    || ((x.LastName ?? "") + (x.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) 
                                    || x.UserName.ToUpper().Contains(keyword.ToUpper()) 
                                    || (x.Email ?? "").ToUpper().Contains(keyword.ToUpper()))
                                    && (x.Status ?? "").ToLower() == "true"
                                    ).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                            }
                            else
                            {
                                nodeUsers = nonUsers.Where(x => (x.Status ?? "").ToLower() == "true").Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                            }
                        }
                    }
                    else
                    {
                        OrgChartWithRelationshipsDto chart = this.chartService.GetChartWithNodes(Guid.Parse(id));
                        foreach (var childnode in chart.Nodes)
                        {
                            GetRecursiveUser(childnode.SysID, nodeUsers, userTemp, pageIndex, pageSize, keyword, allowpage);
                        }
                    }
                }
                else if (type == "Person")//未分类的Person直接读取用户信息
                {
                    UserBaseDto tuser = this.userBO.ReadUser(Guid.Parse(id));
                    if (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && (PinyinHelper.GetShortPinyin(CustomHelper.UserNameFormat(tuser.LastName, tuser.FirstName, tuser.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((tuser.FirstName ?? "") + (tuser.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((tuser.LastName ?? "") + (tuser.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || tuser.UserName.ToUpper().Contains(keyword.ToUpper()) || (tuser.Email ?? "").ToUpper().Contains(keyword.ToUpper()))))
                    {
                        nodeUsers.Add(tuser);
                        nodeUsers = nodeUsers.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                    }
                }
                else
                {
                    GetRecursiveUser(Guid.Parse(id), nodeUsers, userTemp, pageIndex, pageSize, keyword, allowpage);
                }
            }
            if (nodeUsers != null)
            {
                nodeUsers = nodeUsers.OrderBy(x => x.UserName).ToList();
            }
            foreach (var user in nodeUsers)
            {
                string company = string.Empty;
                string department = string.Empty;
                string position = string.Empty;
                UserWithPositionsDto userpositions = userService.ReadUserWithPositions(user.SysID);
                UserWithNodesDto usernodes = userService.ReadUserWithNodes(user.SysID);
                //获取职位
                if (userpositions.Positions != null && userpositions.Positions.Count > 0)
                {
                    List<PositionTree> positionlist = new List<PositionTree>();
                    foreach (var item in userpositions.Positions)
                    {
                        if (!item.Name.ToLower().StartsWith("v_"))
                        {
                            var p = new PositionTree()
                            {
                                DisplayName = item.Name,
                                NodeName = item.Name,
                                HasChildren = false,
                                ID = item.SysID.ToString(),
                                SysId = item.SysID.ToString(),
                                ParentID = null,
                                Type = PositionType.Position.ToString()
                            };
                            if (item.ExtendItems != null && item.ExtendItems.ToList().Count > 1)
                            {
                                int index = 0;
                                var exfield = item.ExtendItems.ToList().Where(x => x.Name == "排序编号").FirstOrDefault();
                                if (exfield != null)
                                {
                                    int.TryParse(exfield.Value, out index);
                                }
                                p.OrderBy = index;
                            }
                            positionlist.Add(p);
                        }
                    }
                    positionlist.OrderBy(x => x.OrderBy).ToList().ForEach(x =>
                    {
                        position = position + "," + x.NodeName;
                    });
                    position = position.Substring(1);
                }
                //获取部门
                if (usernodes.Nodes != null && usernodes.Nodes.Count > 0)
                {
                    List<Organization> organizationlist = new List<Organization>();
                    foreach (var item in usernodes.Nodes)
                    {
                        if (!item.Name.ToLower().StartsWith("v_"))
                        {
                            Organization organization = new Organization
                            {
                                ID = item.SysID.ToString(),
                                NodeName = item.Name,
                                ExFields = GetOrgExField(item.ExtendItems)
                            };
                            if (organization.ExFields != null && organization.ExFields.ToList().Count > 1)
                            {
                                int index = 0;
                                var exfield = organization.ExFields.ToList().Where(x => x.Name == "排序编号").FirstOrDefault();
                                if (exfield != null)
                                {
                                    int.TryParse(exfield.Value, out index);
                                }
                                organization.OrderBy = index;
                            }
                            organizationlist.Add(organization);
                        }
                    }
                    organizationlist.OrderBy(x => x.OrderBy).ToList().ForEach(x =>
                    {
                        department = department + "," + x.NodeName;
                    });
                    department = department.Substring(1);
                    if (usernodes.Nodes.FirstOrDefault() != null && usernodes.Nodes.FirstOrDefault().Chart != null)
                    {
                        company = usernodes.Nodes.FirstOrDefault().Chart.Name;
                    }
                }
                ListBoxItem staff = new ListBoxItem
                {
                    id = user.SysID.ToString(),
                    text = user.UserName,
                    FirstName = (user.FirstName ?? ""),
                    LastName = (user.LastName ?? ""),
                    DisplayName = CustomHelper.UserNameFormat(user.LastName, user.FirstName, user.UserName, "DisplayNameFormatter_"),
                    Company = company,
                    Department = department,
                    Position = position,
                };
                items.Add(staff);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private void GetRecursiveUser(Guid id, List<UserBaseDto> users, List<UserBaseDto> userTemp, int pageIndex, int pageSize, string keyword = "", bool allowpage = true)
        {
            if (users == null)
            {
                return;
            }
            if (allowpage && users.Count >= pageSize)
            {
                return;
            }
            var node = this.chartService.ReadNode(id);
            var nodeUsers = (node == null ? new List<UserBaseDto>() : (node.Users ?? new List<UserBaseDto>()));
            for (int i = 0; i < nodeUsers.Count; i++)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (userTemp.Count >= ((pageIndex - 1) * pageSize) && !ExistsUserById(users, nodeUsers[i].SysID) && (PinyinHelper.GetShortPinyin(CustomHelper.UserNameFormat(nodeUsers[i].LastName, nodeUsers[i].FirstName, nodeUsers[i].UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((nodeUsers[i].FirstName ?? "") + (nodeUsers[i].LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((nodeUsers[i].LastName ?? "") + (nodeUsers[i].FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || nodeUsers[i].UserName.ToUpper().Contains(keyword.ToUpper()) || (nodeUsers[i].Email ?? "").ToUpper().Contains(keyword.ToUpper())) && (nodeUsers[i].Status ?? "").ToLower() == "true")
                    {
                        users.Add(nodeUsers[i]);
                    }
                }
                else
                {
                    if (userTemp.Count >= ((pageIndex - 1) * pageSize) && !ExistsUserById(users, nodeUsers[i].SysID) && (nodeUsers[i].Status ?? "").ToLower() == "true")
                    {
                        users.Add(nodeUsers[i]);
                    }                    
                }
                 if (allowpage&&users.Count >=pageSize)
                {
                    break;
                }
                userTemp.Add(nodeUsers[i]);
            }
            if (allowpage&&users.Count >= pageSize)
            {                
                return;
            }
            if (node.ChildNodes.Count > 0)
            {
                foreach (var child in node.ChildNodes)
                {
                    GetRecursiveUser(child.SysID, users,userTemp,pageIndex,pageSize, keyword, allowpage);
                }
            }
        }
        public bool ExistsUserById(List<UserBaseDto> users, Guid sysId)
        {
            bool flag = false;
            foreach (var user in users)
            {
                if (user.SysID == sysId)
                {
                    flag = true;
                }
            }
            return flag;
        }

        #endregion


        #region 通用选人控件获取部门
        /// <summary>
        /// 通用选人控件获取部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSelectPersonDeptByNode(string id, string type, int pageIndex, int pageSize, bool isshownonreference = false, string keyword = "", bool allowpage = true)
        {
            if (!allowpage)
            {
                pageIndex = 1;
                pageSize = int.MaxValue;
            }
            List<ListBoxItem> items = new List<ListBoxItem>();
            List<OrgNodeBaseDto> nodeTemp = new List<OrgNodeBaseDto>();
            List<OrgNodeBaseDto> nodes = new List<OrgNodeBaseDto>();
            if (string.IsNullOrEmpty(id))
            {
                var nodelist = chartService.GetAllNodes();
                nodelist = nodelist.Where(x => (x.Name.Contains(keyword) || PinyinHelper.GetShortPinyin(x.Name).Contains(keyword.ToLower()))).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                foreach (var node in nodelist)
                {
                    nodes.Add(new OrgNodeBaseDto()
                    {
                        SysID = node.SysID,
                        Name = node.Name,
                        Type = node.Type
                    });
                }
            }
            else
            {
                if (type == "Root")
                {
                    OrgChartWithRelationshipsDto chart = this.chartService.GetChartWithNodes(Guid.Parse(id));
                    var chartnodes = chart.Nodes;
                    if (chartnodes != null)
                    {
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            chartnodes = chartnodes.Where(x => (x.Name ?? "").ToUpper().Contains(keyword.ToUpper()) || PinyinHelper.GetShortPinyin((x.Name ?? "")).Contains(keyword.ToLower())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        else
                        {
                            chartnodes = chartnodes.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                        }
                        foreach (var childnode in chartnodes)
                        {
                            nodes.Add(new OrgNodeBaseDto() { SysID = childnode.SysID, Name = childnode.Name, Type = childnode.Type });
                        }
                    }
                }
                else
                {
                    GetRecursiveDept(Guid.Parse(id), nodes, nodeTemp, pageIndex, pageSize, keyword, allowpage);
                }
            }

            if (nodes != null)
            {
                nodes = nodes.OrderBy(x => x.Name).ToList();
            }
            foreach (var node in nodes)
            {
                ListBoxItem item = new ListBoxItem { id = node.SysID.ToString(), text = node.Name, FirstName = "", LastName = "" };
                items.Add(item);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private void GetRecursiveDept(Guid id, List<OrgNodeBaseDto> nodes, List<OrgNodeBaseDto> nodeTemp, int pageIndex, int pageSize, string keyword = "", bool allowpage = true)
        {
            if (nodes == null)
            {
                return;
            }
            if (allowpage && nodes.Count >= pageSize)
            {
                return;
            }
            var node = this.chartService.ReadNode(id);
            if (node != null)
            {
                OrgNodeBaseDto curnode = new OrgNodeBaseDto() { Name = node.Name, SysID = node.SysID, Type = node.Type };
                if (nodeTemp.Count >= ((pageIndex - 1) * pageSize))
                {
                    nodes.Add(curnode);
                }
                nodeTemp.Add(curnode);
            }

            var childnodes = (node == null ? new List<OrgNodeBaseDto>() : (node.ChildNodes ?? new List<OrgNodeBaseDto>()));

            for (int i = 0; i < childnodes.Count; i++)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (nodeTemp.Count >= ((pageIndex - 1) * pageSize) && ((childnodes[i].Name == null ? "" : childnodes[i].Name.ToUpper()).Contains(keyword.ToUpper()) || PinyinHelper.GetShortPinyin((childnodes[i].Name ?? "")).Contains(keyword.ToLower()) ))
                    {
                        nodes.Add(childnodes[i]);
                    }
                }
                else
                {
                    if (nodeTemp.Count >= ((pageIndex - 1) * pageSize))
                    {
                        nodes.Add(childnodes[i]);
                    }
                }
                if (allowpage && nodes.Count >= pageSize)
                {
                    break;
                }
                nodeTemp.Add(childnodes[i]);
            }
            if (allowpage && nodes.Count >= pageSize)
            {
                return;
            }

            if (node.ChildNodes.Count > 0)
            {
                foreach (var child in node.ChildNodes)
                {
                    GetRecursiveDept(child.SysID, nodes,nodeTemp,pageIndex,pageSize, keyword, allowpage);
                }
            }
        }
        #endregion



    }

    public class Organization
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public string SysId { get; set; }
        public string NodeName { get; set; }
        public bool isParent { get; set; }
        public string ParentID { get; set; }
        public int OrderBy { get; set; }

        //NodeExField
        //public string EnglishName_Full { get; set; }
        //public string EnglishAddress_First { get; set; }
        //public string EnglishAddress_Second { get; set; }
        //public string EnglishAddress_Third { get; set; }
        //public string ChineseName_Full { get; set; }
        //public string ChineseAddress_First { get; set; }
        //public string ChineseAddress_Second { get; set; }
        //public string ChineseAddress_Third { get; set; }
        //public string Code { get; set; }

        public IList<OrgExField> ExFields { get; set; }
    }

    public class OrgExField
    {
        public Guid SysId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Position
    {
        public string PositionID { get; set; }
        public string DisplayName { get; set; }

        public string CategoryID { get; set; }
    }

    public class PositionTree
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public bool HasChildren { get; set; }
        public bool isParent { get; set; }
        public string ParentID { get; set; }
        public string Type { get; set; }
        public string SysId { get; set; }
        public string NodeName { get; set; }
        public int OrderBy { get; set; }
    }

    public enum PositionType
    {
        Position,
        Category
    }

    public class StaffView
    {
        /// <summary>
        /// 员工系统ID
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 显示名称 username
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 固定电话号码
        /// </summary>
        public string TelNo { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        public string FaxNo { get; set; }

        /// <summary>
        /// 移动手机号码
        /// </summary>
        public string MobileNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public string Sex { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string StaffNo { get; set; }

        public string Department { get; set; }
        public string Position { get; set; }
        public string ReportTo { get; set; }

    }

    public class ListBoxItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
    }

    public class SysRole
    {
        public string RoleID { get; set; }
        public string DisplayName { get; set; }
    }

}
