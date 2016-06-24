using aZaaS.KStar;
using aZaaS.Framework.Organization.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ExField;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Localization;
using System.IO;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers 
{
    [EnhancedHandleError]
    public class OrganizationController : BaseMvcController
    {
        OrgChartBO chartBO = new OrgChartBO();
        PositionBO positionBO = new PositionBO();
        UserBO userBO = new UserBO();

        //TODO:
        private readonly UserService userService;
        private readonly OrgChartService chartService;
        private readonly PositionService positionService;
        private readonly ConfigManager configManager;            

        private readonly int NumLimit = 15;

        public OrganizationController()
        {
            this.userService = new UserService();
            this.chartService = new OrgChartService();
            this.positionService = new PositionService();
            this.configManager = new ConfigManager(this.AuthType);
            configManager.TenantID = TenantID();
        }

        #region 获取组织树列表
        /// <summary>
        /// 获取组织树列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrganizationsDrop()
        {
            var chartList = this.chartService.GetAllChartBases();
            List<Position> OrganizationDropList = new List<Position>();
            foreach (var item in chartList)
            {
                OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.SysID.ToString(), PositionID = item.SysID.ToString() });
            }

            //var chartList = chartBO.GetAllCharts().ToList();
            //List<Position> OrganizationDropList = new List<Position>();
            //foreach (var item in chartList)
            //{
            //    OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.Id.ToString(), PositionID = item.Id.ToString() });
            //}


            return Json(OrganizationDropList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取 Organization Tree 节点信息 By ID
        /// <summary>
        /// 获取 Organization Tree 节点信息 By ID
        /// </summary>
        /// <param name="id">结点 ID  默认值  1</param>
        /// <returns></returns>
        public JsonResult GetOrganization(string ListID, string Type, string ID = "0")
        {
            IEnumerable<OrgNodeWithFieldsDto> nodeList;
            List<Organization> organizationlist = new List<Organization>();

            if (ID == "0")
            {
                var items = new List<OrgNodeWithFieldsDto>();
                var chart = this.chartService.ReadChartWithRoot(Guid.Parse(ListID));

                var nodeitem = this.chartService.ReadNodeWithFields(chart.Root.SysID);
                items.Add(nodeitem);
                nodeList = items;

            }
            else
                nodeList = this.chartService.GetChildNodesWithFields(Guid.Parse(ID));

            foreach (var item in nodeList)
            {
                string parentid = null;

                var nodeWithParent = this.chartService.ReadNodeWithParent(item.SysID);
                if (nodeWithParent.Parent != null)
                    parentid = nodeWithParent.Parent.SysID.ToString();

                var nodeWithChildNodes = this.chartService.ReadNodeWithChildNodes(item.SysID);
                bool hasChildNode = nodeWithChildNodes.ChildNodes != null && nodeWithChildNodes.ChildNodes.Count() > 0;

                Organization organization = new Organization
                {
                    ID = item.SysID.ToString(),
                    ParentID = parentid,
                    Type = item.Type,
                    HasChildNode = hasChildNode,
                    NodeName = item.Name,                    
                    ExFields = GetOrgExField(item.ExtendItems)
                };
                if (organization.ExFields != null && organization.ExFields.ToList().Count > 1)
                {
                    int index = 0;
                    var exfield=organization.ExFields.ToList().Where(x=>x.Name=="排序编号").FirstOrDefault();
                    if (exfield != null)
                    {
                        int.TryParse(exfield.Value, out index);
                    }
                    organization.OrderBy = index;
                }
                organizationlist.Add(organization);
            }

            //List<Organization> organizationlist = new List<Organization>();
            //IEnumerable<OrgNodeDTO> list;
            //if (ID == "0")
            //{
            //    var chart = chartBO.ReadChart(Guid.Parse(ListID));
            //    var items = new List<OrgNodeDTO>();
            //    var nodeitem = chartBO.ReadNode(chart.Root.Id);
            //    items.Add(nodeitem);
            //    list = items;

            //}
            //else
            //    list = chartBO.GetChildNodes(Guid.Parse(ID));

            //foreach (var item in list)
            //{
            //    string parentid = null;
            //    if (item.Parent != null)
            //        parentid = item.Parent.Id.ToString();
            //    Organization organization = new Organization
            //    {
            //        ID = item.Id.ToString(),
            //        ParentID = parentid,
            //        Type = item.Type,
            //        HasChildNode = (item.ChildNodes.Count == 0 ? false : true),
            //        NodeName = item.Name,

            //        ExFields = GetOrgExField(item.ExFields)
            //    };
            //    organizationlist.Add(organization);
            //}


            return Json(organizationlist.OrderBy(o => o.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除选中的组织节点 DeleteNodesOrganization
        /// <summary>
        /// 选中的树的节点删除
        /// </summary>
        /// <param name="ListID"> listID 以及Type 两个都是用来识别那一棵树用的，可以单独使用Type也可以</param>
        /// <param name="Type">   </param>
        /// <param name="id"> 要删除的节点的ID</param>
        /// <returns></returns>
        public JsonResult DeleteNodesOrganization(string ListID, string Type, string id)
        {
            var resultid = id;
            var nodeWithFields = this.chartService.ReadNodeWithFields(Guid.Parse(id));
            var nodeFields = nodeWithFields.ExtendItems ?? new List<ExtensionFieldDto>();

            foreach (var field in nodeFields)
            {
                chartBO.RemoveExField(Guid.Parse(id), field.Name);
            }
            if (nodeWithFields.Type == "Company")
            {
                var nodeWithChart = this.chartService.ReadNodeWithChart(Guid.Parse(id));
                resultid = nodeWithChart.Chart.SysID.ToString();

                RecursiveDeleteNode(Guid.Parse(id));                
                chartBO.DeleteChart(nodeWithChart.Chart.SysID);                
            }
            else
            {
                RecursiveDeleteNode(Guid.Parse(id));
            }

            //var resultid = id;
            //var node = chartBO.ReadNode(Guid.Parse(id));

            //foreach (var item in node.ExFields)
            //{
            //    chartBO.RemoveExField(Guid.Parse(id), item.Id);
            //}
            //chartBO.DeleteNode(Guid.Parse(id));
            //if (node.Type == "Company")
            //{
            //    resultid = node.Chart.Id.ToString();
            //    chartBO.DeleteChart(node.Chart.Id);
            //}

            return Json(resultid, JsonRequestBehavior.AllowGet);
        }

        private void RecursiveDeleteNode(Guid id)
        {
            
            var nodes = this.chartService.ReadNodeWithChildNodes(id);
            foreach (var item in nodes.ChildNodes)
            {
                RecursiveDeleteNode(item.SysID);
            }
            chartBO.DeleteNode(id);
            try
            {
                configManager.DeleteStartUserBySysId(id, Configuration_UserType.OrgNode);
            }
            catch (Exception ex) { }
        }

        #endregion

        #region 选中的元素下添加组织节点 AddNodesOrganization
        /// <summary>
        /// 选中的元素下添加组织节点 AddNodesOrganization
        /// </summary>
        /// <param name="ListID">listID 以及Type 两个都是用来识别那一棵树用的，可以单独使用Type也可以</param>
        /// <param name="Type"></param>
        /// <param name="Organizationitem"></param>
        /// <returns></returns>
        public JsonResult AddNodesOrganization(string ListID, string Type, Organization Organizationitem, [ModelBinder(typeof(JsonListBinder<OrgExField>))]IList<OrgExField> ExFields)
        {
            Guid parentNodeId = Guid.Parse(Organizationitem.ParentID);
            var nodeWithChart = this.chartService.ReadNodeWithChart(parentNodeId);

            OrgNodeWithChartParentDto node = new OrgNodeWithChartParentDto { SysID = Guid.NewGuid(), Chart = new OrgChartBaseDto() { SysID = nodeWithChart.Chart.SysID }, Name = Organizationitem.NodeName, Parent = new OrgNodeBaseDto() { SysID = parentNodeId }, Type = Organizationitem.Type, };
            var nodeid = chartBO.CreateNode(node);

            //转换 ExFields
            IList<OrgNodeExFieldDTO> exFieldDTOs = GetOrgExField(ExFields);

            //插入 exFieldDTOs
            foreach (var exFieldDTO in exFieldDTOs)
            {
                chartBO.AppendExField(nodeid, exFieldDTO);
            }

            var Item = new Organization()
            {
                ID = nodeid.ToString(),
                NodeName = Organizationitem.NodeName,
                HasChildNode = false,//添加的元素都是false  同时被选中的节点要变成true
                ParentID = Organizationitem.ParentID,
                Type = Organizationitem.Type
            };
            Item.ExFields = ExFields;

            //var parentNode = chartBO.ReadNode(Guid.Parse(Organizationitem.ParentID));
            //OrgNodeDTO node = new OrgNodeDTO { Id = Guid.NewGuid(), Chart = parentNode.Chart, Name = Organizationitem.NodeName, Parent = parentNode, Type = Organizationitem.Type, };
            //var nodeid = chartBO.CreateNode(node);

            ////转换 ExFields
            //IList<OrgNodeExFieldDTO> exFieldDTOs = GetOrgExField(ExFields);

            ////插入 exFieldDTOs
            //foreach (var exFieldDTO in exFieldDTOs)
            //{
            //    chartBO.AppendExField(nodeid, exFieldDTO);
            //}

            //var Item = new Organization()
            //{
            //    ID = nodeid.ToString(),
            //    NodeName = Organizationitem.NodeName,
            //    HasChildNode = false,//添加的元素都是false  同时被选中的节点要变成true
            //    ParentID = Organizationitem.ParentID,
            //    Type = Organizationitem.Type
            //};
            //Item.ExFields = ExFields;

            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 保存对应的树的相应节点信息 SaveOrganization
        /// <summary>
        ///  保存对应的树的相应节点信息
        /// </summary>
        /// <param name="ListID">listID 以及Type 两个都是用来识别那一棵树用的，可以单独使用Type也可以</param>
        /// <param name="Type"></param>
        /// <param name="organ"></param>
        /// <returns></returns>
        public JsonResult SaveOrganization(List<string> PositionIdList, List<string> ManagerIdList, Organization organ, [ModelBinder(typeof(JsonListBinder<OrgNodeExFieldDTO>))]IList<OrgNodeExFieldDTO> ExFields)// string ListID, string TypeMenuTree,
        {
            Guid curNodeId = Guid.Parse(organ.ID);
            var curNodeInst = this.chartService.ReadNode(curNodeId);
            var curNodeFields = curNodeInst.ExtendItems ?? new List<ExtensionFieldDto>();
            var curNodePositions = curNodeInst.Positions ?? new List<PositionBaseDto>();
            var curNodeUsers = curNodeInst.Users ?? new List<UserBaseDto>();

            var updatedNode = new OrgNodeBaseDto()
            {
                SysID = curNodeId,
                Name = organ.NodeName,
                Type = organ.Type
            };
            chartBO.UpdateNode(updatedNode);

            ////转换 ExFields
            IList<OrgNodeExFieldDTO> exFieldDTOs = ExFields;
            ////编辑 或 插入 exFieldDTOs
            foreach (var exFieldDTO in exFieldDTOs)
            {
                var oldExFieldDTO = curNodeFields.SingleOrDefault(s => s.Name == exFieldDTO.Name);
                if (oldExFieldDTO != null)
                {
                    exFieldDTO.SysID = oldExFieldDTO.SysID;//取旧Id
                    chartBO.UpdateExField(exFieldDTO);//更新
                    //TODO:
                    curNodeFields.Remove(oldExFieldDTO);
                }
                else
                {
                    chartBO.AppendExField(curNodeInst.SysID, exFieldDTO);
                }
            }
            //把不存在的删除
            foreach (var exFieldDTO in curNodeFields)
            {
                chartBO.RemoveExField(curNodeInst.SysID, exFieldDTO.Name);
            }

            //Remove Positions
            foreach (var position in curNodePositions)
            {
                chartBO.RemovePosition(curNodeId, position.SysID);
            }
            //Remove Users
            foreach (var user in curNodeUsers)
            {
                chartBO.RemoveUser(curNodeId, user.SysID);
            }

            if (PositionIdList != null)
                foreach (var sPositionId in PositionIdList)
                {
                    chartBO.AppendPosition(curNodeId, Guid.Parse(sPositionId));
                }
            if (ManagerIdList != null)
                foreach (var sUserId in ManagerIdList)
                {
                    chartBO.AppendUser(curNodeId, Guid.Parse(sUserId));
                }
            
            return Json(organ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加一个组织，即添加一棵树
        /// <summary>
        /// 添加一个组织，即添加一棵树
        /// </summary>
        /// <param name="Name">树的名称（组织名称）</param>
        /// <returns></returns>
        public JsonResult AddOrganization(string Name)
        {
            //生成一棵树
            Guid nodeid = Guid.NewGuid();
            Guid chartid = Guid.NewGuid();
            OrgNodeBaseDto orgNodeDTO = new OrgNodeBaseDto { SysID = nodeid, Name = Name, Type = "Company" };
            OrgChartWithRootDto orgChartDTO = new OrgChartWithRootDto { SysID = chartid, Name = Name, Root = orgNodeDTO };            
            var id = chartBO.CreateChart(orgChartDTO);

            var chart = this.chartService.ReadChartWithRoot(id);

            //chartBO.UpdateChartRoot(id, chart.Root.SysID);  //SetChartRoot
            chartBO.UpdateNodeChart(nodeid, chartid);

            var chartList = this.chartService.GetAllChartBases();
            List<Position> OrganizationDropList = new List<Position>();
            foreach (var item in chartList)
            {
                OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.SysID.ToString(), PositionID = item.SysID.ToString() });
            }

            //OrgNodeDTO orgNodeDTO = new OrgNodeDTO { Id = Guid.NewGuid(), Name = Name, Type = "Company" };
            //OrgChartDTO orgChartDTO = new OrgChartDTO { Id = Guid.NewGuid(), Name = Name, Root = orgNodeDTO };
            //var id = chartBO.CreateChart(orgChartDTO);
            //var chart = chartBO.ReadChart(id);
            //chartBO.UpdateChartRoot(id, chart.Root.Id);  //SetChartRoot

            //var chartList = chartBO.GetAllCharts().ToList();
            //List<Position> OrganizationDropList = new List<Position>();
            //foreach (var item in chartList)
            //{
            //    OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.Id.ToString(), PositionID = item.Id.ToString() });
            //}

            return Json(OrganizationDropList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult EditOrganization(string Name, Guid OrgChartId)
        {
            //编辑树名称
            var updatedChart = new OrgChartWithRelationshipsDto()
            {
                SysID = OrgChartId,
                Name = Name
            };
            this.chartBO.UpdateChart(updatedChart);

            var chartList = this.chartService.GetAllChartBases();
            List<Position> OrganizationDropList = new List<Position>();
            foreach (var item in chartList)
            {
                OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.SysID.ToString(), PositionID = item.SysID.ToString() });
            }

            //OrgChartBO orgChartBo = new OrgChartBO();
            //var itemOrgChart = orgChartBo.ReadChart(OrgChartId);
            //itemOrgChart.Name = Name;
            //orgChartBo.UpdateChart(itemOrgChart);

            //var chartList = chartBO.GetAllCharts().ToList();
            //List<Position> OrganizationDropList = new List<Position>();
            //foreach (var item in chartList)
            //{
            //    OrganizationDropList.Add(new Position { DisplayName = item.Name, CategoryID = item.Id.ToString(), PositionID = item.Id.ToString() });
            //}

            return Json(OrganizationDropList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgChartTree(string ID = "0_0", bool isshownonreference=false,string tree="")
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
                if (isshownonreference)
                {
                    string nonname = string.Empty;
                    string mapPath =Server.MapPath("~");
                    string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
                    string cultureName = ResxService.GetAvailableCulture();
                    string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/Parts/_OrganizationView_cshtml." + cultureName + ".resx");
                    FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
                    if (fi_parentCulture.Exists)
                    {
                        nonname = ResxService.GetResouces("NonReference", cshtmlResxFilePath);
                    }
                    else
                    {
                        nonname = "NonReference";
                    }
                    tnode.Add(new OrgNodeWithFieldsDto() { SysID = new Guid("00000000-0000-0000-0000-000000000000"), Name = nonname, Type = "Root", ExtendItems = new List<ExtensionFieldDto>() });
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
                bool hasChildNode=false;
                string id = "1";

                if (item.Type == "Root")
                {
                    //未分类
                    if (isshownonreference&&item.SysID == new Guid("00000000-0000-0000-0000-000000000000"))
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
                    HasChildNode = hasChildNode,
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

        public JsonResult GetPositionByNode(string id)
        {
            List<Position> items = new List<Position>();
            var nodeWithPositions = this.chartService.ReadNodeWithPositions(Guid.Parse(id));
            var nodePositions = (nodeWithPositions == null ? new List<PositionBaseDto>(): (nodeWithPositions.Positions ?? new List<PositionBaseDto>()));
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

        #region  选人控件       
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
                string nonname = string.Empty;
                string mapPath = Server.MapPath("~");
                string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
                string cultureName = ResxService.GetAvailableCulture();
                string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/Parts/_OrganizationView_cshtml." + cultureName + ".resx");
                FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
                if (fi_parentCulture.Exists)
                {
                    nonname = ResxService.GetResouces("SystemRoles", cshtmlResxFilePath);
                }
                else
                {
                    nonname = "SystemRoles";
                }
                organizationlist.Add(new Organization
                {
                    ID = "1_00000000-0000-0000-0000-000000000000",
                    SysId = "00000000-0000-0000-0000-000000000000",
                    ParentID = "",
                    Type = "Role",
                    HasChildNode = true,
                    NodeName = nonname,
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
                        ID ="2_"+item.RoleID,
                        SysId = item.RoleID,
                        ParentID = "",
                        Type = "Role",
                        HasChildNode = false,
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
                roles = roles.Where(x => Chinese2Spell.GetFirstSpelling((x.DisplayName ?? "")).Contains(keyword.ToLower()) || (x.DisplayName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
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


        /// <summary>
        /// 通用选人控件获取职位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSelectPersonPositionByNode(string id, string type, int pageIndex, int pageSize, bool isshownonreference = false, string keyword = "", bool allowpage=true)
        {
            id = id.Contains("_") ? id.Split('_')[1] : id;

            if (!allowpage)
            {
                pageIndex = 1;
                pageSize = int.MaxValue;
            }
            List<ListBoxItem> items = new List<ListBoxItem>();
            List<PositionBaseDto> positionTemp = new List<PositionBaseDto>();
            List<PositionBaseDto> positions = new List<PositionBaseDto>();
            if (id == "00000000-0000-0000-0000-000000000000")
            {
                var positionlist=positionBO.GetAllPositons();                
                positionlist = positionlist.Where(x => (x.Name.Contains(keyword) || Chinese2Spell.GetFirstSpelling(x.Name).Contains(keyword.ToLower()))).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();                              
                foreach (var position in positionlist)
                {
                    positions.Add(new PositionBaseDto()
                    {
                         SysID=position.SysID,
                         Name=position.Name
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
                    if (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && (tposition.Name.Contains(keyword) || Chinese2Spell.GetFirstSpelling(tposition.Name).Contains(keyword.ToLower()))))
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
                    FirstName="",
                    LastName=""
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
            if (allowpage&&positions.Count >= pageSize)
            {
                return;
            }
            var childcategory = this.positionService.GetChildCategoriesBase(id).OrderBy(x => x.Name).ToList();
            var category =positionBO.ReadCategory(id);
            var positionlist = category == null ? new List<PositionBaseDto>() : category.Positions.OrderBy(x => x.Name).ToList();
            
            for (int i = 0; i < positionlist.Count; i++)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (positionTemp.Count >= ((pageIndex - 1) * pageSize) && ((positionlist[i].Name == null ? "" : positionlist[i].Name.ToUpper()).Contains(keyword.ToUpper()) || Chinese2Spell.GetFirstSpelling((positionlist[i].Name ?? "")).Contains(keyword.ToLower()) ))
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
                if (allowpage&&positions.Count >= pageSize)
                {
                    break;
                }
                positionTemp.Add(positionlist[i]);
            }
            if (allowpage&&positions.Count >= pageSize)
            {
                return;
            }
            if (childcategory.Count > 0)
            {
                foreach (var child in childcategory)
                {
                    GetRecursivePosition(child.SysID, positions, positionTemp, pageIndex, pageSize, keyword, allowpage);
                }
            }
        }

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
                IEnumerable<UserWithRelationshipsDto> userlist = userService.GetUsersWithRelationships((t => (t.Email.Contains(keyword) || t.UserName.Contains(keyword) || (t.FirstName + t.LastName).Contains(keyword) || (t.LastName + t.FirstName).Contains(keyword)) && t.Status!=null && t.Status.ToLower() == "true"), pageIndex, pageSize);                
                if (userlist != null)
                {
                    foreach (var user in userlist)
                    {
                        nodeUsers.Add(new UserBaseDto() {
                            SysID = user.SysID, 
                            UserId=user.UserId,
                            UserName=user.UserName,
                            FirstName=user.FirstName,
                            LastName=user.LastName,
                            Email=user.Email                            
                        });
                    }
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
                                nodeUsers = nonUsers.Where(x => (Chinese2Spell.GetFirstSpelling(CustomHelper.UserNameFormat(x.LastName, x.FirstName, x.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((x.FirstName ?? "") + (x.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((x.LastName ?? "") + (x.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || x.UserName.ToUpper().Contains(keyword.ToUpper()) || (x.Email ?? "").ToUpper().Contains(keyword.ToUpper())) && (x.Status ?? "").ToLower()=="true").Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                            }
                            else
                            {
                                nodeUsers = nonUsers.Where(x=>(x.Status ?? "").ToLower()=="true").Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
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
                    if (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && (Chinese2Spell.GetFirstSpelling(CustomHelper.UserNameFormat(tuser.LastName, tuser.FirstName, tuser.UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((tuser.FirstName ?? "") + (tuser.LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((tuser.LastName ?? "") + (tuser.FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || tuser.UserName.ToUpper().Contains(keyword.ToUpper()) || (tuser.Email ?? "").ToUpper().Contains(keyword.ToUpper()))))
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
                UserWithPositionsDto userpositions=userService.ReadUserWithPositions(user.SysID);
                UserWithNodesDto usernodes=userService.ReadUserWithNodes(user.SysID);                
                //获取职位
                if (userpositions.Positions!= null && userpositions.Positions.Count > 0)
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
                        department = department +","+ x.NodeName;
                    });
                    department = department.Substring(1);
                    if (usernodes.Nodes.FirstOrDefault() != null && usernodes.Nodes.FirstOrDefault().Chart != null)
                    {
                        company = usernodes.Nodes.FirstOrDefault().Chart.Name;
                    }
                }


                ListBoxItem staff = new ListBoxItem { 
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

        private void GetRecursiveUser1(Guid id, List<UserBaseDto> users)
        {
            var parentnode=this.chartService.ReadNode(id);
            var Users = (parentnode == null ? new List<UserBaseDto>() : (parentnode.Users ?? new List<UserBaseDto>()));
            foreach (var user in Users)
            {
                users.Add(user);
            }
            Queue<OrgNodeBaseDto> queue = new Queue<OrgNodeBaseDto>(parentnode.ChildNodes);
            while (queue.Count > 0)
            {
                OrgNodeWithRelationshipsDto qnode = this.chartService.ReadNode(queue.Dequeue().SysID);
                var nodeUsers = (qnode == null ? new List<UserBaseDto>() : (qnode.Users ?? new List<UserBaseDto>()));
                foreach (var user in nodeUsers)
                {
                    users.Add(user);
                }                
                if (qnode.ChildNodes != null && qnode.ChildNodes.Count > 0)
                {
                    foreach (var o in qnode.ChildNodes)
                    {
                        OrgNodeWithChildNodesDto item = this.chartService.ReadNodeWithChildNodes(o.SysID);                        
                        queue.Enqueue(item);
                    }
                }
            }
        }

        private void GetRecursiveUser(Guid id, List<UserBaseDto> users, List<UserBaseDto> userTemp, int pageIndex, int pageSize, string keyword = "", bool allowpage = true)
        {            
            if (users == null)
            {                
                return;
            }
            if (allowpage&&users.Count >= pageSize)
            {                
                return;
            }
            var node = this.chartService.ReadNode(id);
            var nodeUsers = (node == null ? new List<UserBaseDto>() : (node.Users ?? new List<UserBaseDto>()));            
            for (int i = 0; i < nodeUsers.Count; i++)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (userTemp.Count >= ((pageIndex - 1) * pageSize) && !ExistsUserById(users, nodeUsers[i].SysID) && (Chinese2Spell.GetFirstSpelling(CustomHelper.UserNameFormat(nodeUsers[i].LastName, nodeUsers[i].FirstName, nodeUsers[i].UserName, "DisplayNameFormatter_")).Contains(keyword.ToLower()) || ((nodeUsers[i].FirstName ?? "") + (nodeUsers[i].LastName ?? "")).ToUpper().Contains(keyword.ToUpper()) || ((nodeUsers[i].LastName ?? "") + (nodeUsers[i].FirstName ?? "")).ToUpper().Contains(keyword.ToUpper()) || nodeUsers[i].UserName.ToUpper().Contains(keyword.ToUpper()) || (nodeUsers[i].Email ?? "").ToUpper().Contains(keyword.ToUpper())) && (nodeUsers[i].Status ?? "").ToLower() == "true")
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
                    GetRecursiveUser(child.SysID, users, userTemp, pageIndex, pageSize, keyword, allowpage);
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
                nodelist = nodelist.Where(x => (x.Name.Contains(keyword) || Chinese2Spell.GetFirstSpelling(x.Name).Contains(keyword.ToLower()))).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                foreach (var node in nodelist)
                {
                    nodes.Add(new OrgNodeBaseDto()
                    {
                        SysID = node.SysID,
                        Name = node.Name,
                        Type=node.Type                        
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
                            chartnodes = chartnodes.Where(x => (x.Name ?? "").ToUpper().Contains(keyword.ToUpper()) || Chinese2Spell.GetFirstSpelling((x.Name ?? "")).Contains(keyword.ToLower())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
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
                ListBoxItem item = new ListBoxItem { id = node.SysID.ToString(), text = node.Name, FirstName="", LastName="" };
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
            if (allowpage&&nodes.Count >= pageSize)
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
                    if (nodeTemp.Count >= ((pageIndex - 1) * pageSize) && ((childnodes[i].Name == null ? "" : childnodes[i].Name.ToUpper()).Contains(keyword.ToUpper()) || Chinese2Spell.GetFirstSpelling((childnodes[i].Name ?? "")).Contains(keyword.ToLower()) ))
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
                if (allowpage&&nodes.Count >= pageSize)
                {
                    break;
                }
                nodeTemp.Add(childnodes[i]);
            }
            if (allowpage&&nodes.Count >= pageSize)
            {
                return;
            }

            if (node.ChildNodes.Count > 0)
            {
                foreach (var child in node.ChildNodes)
                {
                    GetRecursiveDept(child.SysID, nodes, nodeTemp, pageIndex, pageSize, keyword, allowpage);
                }
            }
        }

        #endregion

        [NonAction]
        private IList<OrgExField> GetOrgExField(IList<OrgNodeExFieldDTO> ExFields)
        {
            IList<OrgExField> items = new List<OrgExField>();
            OrgExField item;
            //foreach (var exField in ExFields)
            //{
            //    item = new OrgExField() { Id = exField.Id, PropertyName = exField.PropertyName, TypeCode = exField.DataType.ToString() };
            //    switch (exField.DataType)
            //    {
            //        case FieldTypeCode.String:
            //            item.ValueString = exField.ValueString;
            //            break;
            //        case FieldTypeCode.DateTime:
            //            item.ValueString = exField.ValueDateTime != null ? exField.ValueDateTime.ToString() : null;
            //            break;
            //        case FieldTypeCode.Decimal:
            //            item.ValueString = exField.ValueNumber != null ? exField.ValueNumber.ToString() : null;
            //            break;
            //    }
            //    items.Add(item);
            //}
            return items;
        }

        [NonAction]
        private IList<OrgExField> GetOrgExField(IList<ExtensionFieldDto> ExFields)
        {
            IList<OrgExField> items = new List<OrgExField>(); 
            foreach (var exField in ExFields)
            {           
                items.Add( new OrgExField() { SysId=exField.SysID, Name=exField.Name, Value=exField.Value });
            }
            return items;
        }

        [NonAction]
        private IList<OrgNodeExFieldDTO> GetOrgExField(IList<OrgExField> ExFields)
        {
            IList<OrgNodeExFieldDTO> items = new List<OrgNodeExFieldDTO>();
            //OrgNodeExFieldDTO exFieldDTO;
            //foreach (var ExField in ExFields)
            //{
            //    exFieldDTO = new OrgNodeExFieldDTO()
            //    {
            //        Id = Guid.NewGuid(),
            //        TypeCode = (int)(FieldTypeCode)Enum.Parse(typeof(FieldTypeCode), ExField.TypeCode),
            //        PropertyName = ExField.PropertyName
            //    };

            //    switch (exFieldDTO.DataType)
            //    {
            //        case FieldTypeCode.String:
            //            exFieldDTO.ValueString = ExField.ValueString;
            //            break;
            //        case FieldTypeCode.DateTime:
            //            DateTime time;
            //            if (DateTime.TryParse(ExField.ValueString, out time))
            //            {
            //                exFieldDTO.ValueDateTime = time;
            //            }
            //            break;
            //        case FieldTypeCode.Decimal:
            //            Decimal number;
            //            if (Decimal.TryParse(ExField.ValueString, out number))
            //            {
            //                exFieldDTO.ValueNumber = number;
            //            }
            //            break;
            //    }

            //    items.Add(exFieldDTO);
            //}
            return items;
        }
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
}
