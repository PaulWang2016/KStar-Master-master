using aZaaS.KStar;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class PositionController : BaseMvcController
    {
        PositionBO positionBO = new PositionBO();
        UserBO userBO = new UserBO();
        private readonly OrgChartService chartService;
        //TODO:
        private readonly PositionService positionService;
        private readonly UserService userService;
        public PositionController()
        {
            this.positionService = new PositionService();
            this.userService = new UserService();
            this.chartService = new OrgChartService();
        }

        #region 添加Category
        /// <summary>
        /// 添加Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateCategory(Category model)
        {
            PositionCategoryWithRelationshipsDto Parent = null;
            Guid ParentId;
            if (Guid.TryParse(model.ParentID, out ParentId))
            {
                Parent = new PositionCategoryWithRelationshipsDto() { SysID = ParentId };
            }
            PositionCategoryWithRelationshipsDto positionCategeory = new PositionCategoryWithRelationshipsDto { SysID = Guid.NewGuid(), Name = model.DisplayName, Parent = Parent };
            model.CategoryID = positionBO.CreateCategory(positionCategeory).ToString();
            PositionTree item = new PositionTree()
            {
                Type = PositionType.Category.ToString(),
                ParentID = model.ParentID,
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 更新Category
        /// <summary>
        /// 更新Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCategory(Category model)
        {
            Guid curCategoryId = Guid.Parse(model.CategoryID);
            var curCategory = this.positionService.ReadPositionCategoryWithParent(curCategoryId);

            PositionCategoryWithRelationshipsDto updateCategory = new PositionCategoryWithRelationshipsDto()
            {
                SysID = curCategoryId, Name = model.DisplayName
            };
            positionBO.UpdateCategory(updateCategory);

            PositionTree item = new PositionTree()
            {
                Type = PositionType.Category.ToString(),
                ParentID = curCategory.Parent == null ? null : curCategory.Parent.SysID.ToString(),
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };


            //var positionItem = positionBO.ReadCategory(Guid.Parse(model.CategoryID));
            //positionItem.Name = model.DisplayName;
            //positionBO.UpdateCategory(positionItem);

            //PositionTree item = new PositionTree()
            //{
            //    Type = PositionType.Category.ToString(),
            //    ParentID = positionItem.Parent == null ? null : positionItem.Parent.Id.ToString(),
            //    ID = model.CategoryID,
            //    HasChildren = false,
            //    DisplayName = model.DisplayName
            //};


            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除Category
        /// <summary>
        /// 删除Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DestroyCategory(string categoryID)
        {
            //foreach (int id in idList)
            //{
            //    Positions.RemoveAll(c => c.CategoryID == id);
            //    Positions.RemoveAll(c => c.PositionID == id);
            //}
            positionBO.DeleteCategory(Guid.Parse(categoryID));
            return Json(categoryID, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  获取 Position By Parent ID
        /// <summary>
        /// 获取 Position By Parent ID   当 Parent ID 为 0 时  为获取分类
        /// </summary>
        /// <param name="id">Parent ID</param>
        /// <returns>List_Position</returns>
        public JsonResult GetPosition2(string ID = "")
        {
            //List<Position> items = new List<Position>();
            //var positionlist = positionBO.ReadCategory(Guid.Parse(CategoryID)).Positions.ToList();
            //foreach (var position in positionlist)
            //{
            //    Position item = new Position { DisplayName = position.Name, CategoryID = CategoryID, PositionID = position.Id.ToString() };
            //    items.Add(item);
            //}
            //return Json(items, JsonRequestBehavior.AllowGet);
            List<PositionTree> items = new List<PositionTree>();
            List<PositionCategoryWithChildCategoriesDto> categories = new List<PositionCategoryWithChildCategoriesDto>();
            //List<PositionCategoryDTO> categories = new List<PositionCategoryDTO>();
            List<PositionWithFieldsDto> positionlist = new List<PositionWithFieldsDto>();
            if (ID == "") 
            {
                categories = this.positionService.GetAllCategoriesBase().OrderBy(x => x.Name).ToList();//positionBO.GetAllCategories().ToList();
            }
            else
            {
                categories = this.positionService.GetChildCategoriesBase(Guid.Parse(ID)).OrderBy(x => x.Name).ToList();// positionBO.GetChildCategories(Guid.Parse(ID)).ToList();

                positionlist=positionBO.GetPositionsWithFieldByCategory(Guid.Parse(ID)).ToList();                

            }
            foreach (var item in categories)
            {
                items.Add(new PositionTree()
                {
                    DisplayName = item.Name,
                    NodeName = item.Name,
                    HasChildren = (positionBO.ReadCategory(item.SysID).Positions.Count > 0 || item.ChildCategories.Count > 0) ? true : false,
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
            return Json(items.OrderBy(x=>x.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPosition(string ID = "")
        {
            return GetManagePosition(ID);

        }

        public JsonResult GetManagePosition(string ID = "")
        { 
            List<PositionTree> items = new List<PositionTree>();
            IEnumerable<OrgNodeWithFieldsDto> nodeList;
            List<OrgNodeWithFieldsDto> nodes = new List<OrgNodeWithFieldsDto>();
            int type = 1;
            string pType = "";
            string pID = "";
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
                pType = Params[0];
                pID = Params[1];
                if (pType == "1")
                {
                    var exp = new LogicalExpression(LogicalKind.And)
                    {
                        Fields = new List<Field>
                        {
                             new Field("Type", "'Company'", "OrgNode", OperatorKind.Equal),    
                             new Field("Chart_SysId", "'"+pID+"'", "OrgNode", OperatorKind.Equal)   
                        }
                    };
                    nodeList = this.chartService.GetNodesWithFields(exp);
                }
                else if (pType == "2")
                {
                    nodeList = this.chartService.GetChildNodesWithFields(Guid.Parse(pID));
                }
                else
                {
                    nodeList = null;
                }

            }
            //固定排序
            if (nodeList != null)
            {
                nodeList = nodeList.OrderBy(x => x.Name);
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
                    var isParent = false;
                    if (type == 1)
                    {

                        var exp = new LogicalExpression(LogicalKind.And)
                        {
                            Fields = new List<Field>
                        {
                             new Field("Type", "'Company'", "OrgNode", OperatorKind.Equal),    
                             new Field("Chart_SysId", "'"+item.SysID+"'", "OrgNode", OperatorKind.Equal)   
                        }
                        };
                        isParent = this.chartService.GetNodesWithFields(exp).Count() > 0 ? true : false;
                    }
                    else
                    {
                        isParent = this.chartService.GetChildNodesWithFields(item.SysID).Count() > 0 ? true : false;
                    }

                    var isPosition = false;
                    if (!isParent)
                    {
                        isParent = GetChildPositionCout(item.SysID) > 0 ? true : false;
                        isPosition = isParent;
                    }
                    items.Add(new PositionTree()
                    {
                        DisplayName = item.Name,
                        NodeName = item.Name,
                        HasChildren = isParent,
                        ID = (!isPosition ? type : 3) + "_" + item.SysID.ToString(),
                        SysId = item.SysID.ToString(),
                        ParentID = (item.SysID.ToString() == "" ? null : ID),
                        Type = pType == "3" ? PositionType.Position.ToString() : PositionType.Category.ToString(),
                        OrderBy = OrderBy
                    });
                }

                if (!string.IsNullOrEmpty(pID))
                {
                    var childcategory = GetChildPosition(Guid.Parse(pID)).OrderBy(x => x.Name).ToList();

                    foreach (var item in childcategory)
                    {

                        items.Add(new PositionTree()
                        {
                            DisplayName = item.Name,
                            NodeName = item.Name,
                            HasChildren = false,
                            ID = item.SysId.ToString(),
                            SysId = item.SysId.ToString(),
                            ParentID = (item.SysId.ToString() == "" ? null : ID),
                            Type = PositionType.Position.ToString(),
                            OrderBy = 0
                        });
                    }
                }
            }
            else
            {
                var childcategory = GetChildPosition(Guid.Parse(pID)).OrderBy(x => x.Name).ToList();

                foreach (var item in childcategory)
                {

                    items.Add(new PositionTree()
                    {
                        DisplayName = item.Name,
                        NodeName = item.Name,
                        HasChildren = false,
                        ID = item.SysId.ToString(),
                        SysId = item.SysId.ToString(),
                        ParentID = (item.SysId.ToString() == "" ? null : ID),
                        Type = PositionType.Position.ToString(),
                        OrderBy = 0
                    });
                }

            }

            return Json(items.OrderBy(x => x.OrderBy).ToList(), JsonRequestBehavior.AllowGet);
        }


        private int GetChildPositionCout(Guid sysID)
        {
            try
            {
                using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
                {
                    int count = db.PositionOrgNodes.Where(x => x.OrgNode_SysId == sysID).Count();
                    return count;
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }


        private List<aZaaS.KStar.ProcessForm.Position> GetChildPosition(Guid orgNodeID)
        {
            try
            {
                using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
                {
                    var linq = from a in db.Positions join b in db.PositionOrgNodes on a.SysId equals b.Position_SysId where b.OrgNode_SysId == orgNodeID select a;


                    return linq.ToList();
                }
            }
            catch (Exception ex)
            {

            } 
            return null;
        }

        [NonAction]
        private IList<KStarFormMvcApplication.Controllers.OrgExField> GetOrgExField(IList<ExtensionFieldDto> ExFields)
        {
            IList<KStarFormMvcApplication.Controllers.OrgExField> items = new List<KStarFormMvcApplication.Controllers.OrgExField>();
            foreach (var exField in ExFields)
            {
                items.Add(new KStarFormMvcApplication.Controllers.OrgExField() { SysId = exField.SysID, Name = exField.Name, Value = exField.Value });
            }
            return items;
        }
        public JsonResult GetCategory()
        {
            List<Category> items = new List<Category>();

            var categorylist = this.positionService.GetAllCategoriesBase().ToList(); // positionBO.GetAllCategories().ToList();
            foreach (var category in categorylist)
            {
                Category item = new Category { DisplayName = category.Name, ParentID = "0", CategoryID = category.SysID.ToString(), };
                items.Add(item);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加Position
        /// <summary>
        /// 添加Position
        /// </summary>
        /// <param name="models">List_Position</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreatePosition(Position model)
        {
            if (!string.IsNullOrEmpty(model.CategoryID))
            {
                if (model.CategoryID.IndexOf("_") > -1)
                {
                    model.CategoryID = model.CategoryID.Substring(model.CategoryID.IndexOf("_") + 1);
                }
            }
            using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
            {
                var positionID = Guid.NewGuid();
                model.PositionID = positionID.ToString();
                db.Positions.Add(new ProcessForm.Position() { SysId = positionID, Name = model.DisplayName, Category_SysId = Guid.Parse(model.CategoryID) });

                db.PositionOrgNodes.Add(new ProcessForm.PositionOrgNodes() { OrgNode_SysId = new Guid(model.CategoryID), Position_SysId = new Guid(model.PositionID) });
                db.SaveChanges();
            }
            PositionTree item = new PositionTree()
            {
                Type = PositionType.Position.ToString(),
                ParentID = model.CategoryID,
                ID = model.PositionID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 更新Position   并更新人员
        /// <summary>
        /// 更新Position
        /// </summary>
        /// <param name="models">List_Position</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdatePosition(Position model, List<Guid> addids, List<Guid> removeids, [ModelBinder(typeof(JsonListBinder<PositionExFieldDTO>))]IList<PositionExFieldDTO> ExFields)
        {
            var curPositionId = Guid.Parse(model.PositionID);
            var curPositionWithCategory = this.positionService.ReadPositionWithFields(curPositionId);

            var curNodeFields = curPositionWithCategory.ExtendItems ?? new List<ExtensionFieldDto>();

            var updatedPosition = new PositionWithRelationshipsDto()
            {
                SysID = curPositionId,
                Name = model.DisplayName
            };
            positionBO.UpdatePosition(updatedPosition);//Update
            #region Add User
            if (addids != null)
            {
                foreach (var userid in addids)
                {
                    var IsUserInPosition = positionBO.PositionUserExists(userid, curPositionId); //IsUserOfPosition
                    if (IsUserInPosition == false)
                    {
                        positionBO.AppendUser(curPositionId, userid);
                    }
                }
            }
            #endregion
            #region Remove User
            if (removeids != null)
            {
                foreach (var userid in removeids)
                {
                    positionBO.RemoveUser(curPositionId, userid);
                }
            }
            #endregion

            #region 扩展字段
            ////转换 ExFields
            IList<PositionExFieldDTO> exFieldDTOs = ExFields;
            ////编辑 或 插入 exFieldDTOs
            foreach (var exFieldDTO in exFieldDTOs)
            {
                var oldExFieldDTO = curNodeFields.SingleOrDefault(s => s.Name == exFieldDTO.Name);
                if (oldExFieldDTO != null)
                {
                    exFieldDTO.SysID = oldExFieldDTO.SysID;//取旧Id
                    positionBO.UpdateExField(exFieldDTO);//更新
                    //TODO:
                    curNodeFields.Remove(oldExFieldDTO);
                }
                else
                {
                    positionBO.AppendExField(curPositionWithCategory.SysID, exFieldDTO);
                }
            }
            //把不存在的删除
            foreach (var exFieldDTO in curNodeFields)
            {
                positionBO.RemoveExField(curPositionWithCategory.SysID, exFieldDTO.Name);
            }
            #endregion

            PositionTree item = new PositionTree()
            {
                Type = PositionType.Position.ToString(),
                ParentID = curPositionWithCategory.Category.SysID.ToString(),
                ID = model.PositionID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };


            #region
            //var positionItem = positionBO.ReadPosition(Guid.Parse(model.PositionID));//Read
            //positionItem.Name = model.DisplayName;
            //positionBO.UpdatePosition(positionItem);//Update
            //#region Add User
            //if (addids != null)
            //{
            //    foreach (var userid in addids)
            //    {
            //        var IsUserInPosition = positionBO.PositionUserExists(userid, positionItem.Id); //IsUserOfPosition
            //        if (IsUserInPosition == false)
            //        {
            //            positionBO.AppendUser(positionItem.Id, userid);
            //        }
            //    }
            //}
            //#endregion
            //#region Remove User
            //if (removeids != null)
            //{
            //    foreach (var userid in removeids)
            //    {
            //        positionBO.RemoveUser(positionItem.Id, userid);
            //    }
            //}
            //#endregion
            //PositionTree item = new PositionTree()
            //{
            //    Type = PositionType.Position.ToString(),
            //    ParentID = positionItem.Category.Id.ToString(),
            //    ID = model.PositionID,
            //    HasChildren = false,
            //    DisplayName = model.DisplayName
            //};
            #endregion
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除Position
        /// <summary>
        /// 删除Position
        /// </summary>
        /// <param name="id">List_Position</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DestroyPosition(string positionID)
        {
            //foreach (int id in idList)
            //{
            //    Positions.RemoveAll(p => p.PositionID == id);
            //}
            positionBO.DeletePosition(Guid.Parse(positionID)); //Delete

            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                svc.DeleteStartUserBySysId(Guid.Parse(positionID), Configuration_UserType.Position);
            }
            catch (Exception ex) { }

            return Json(positionID, JsonRequestBehavior.AllowGet);
        }
        #endregion
         
        #region 获取  某职务下的人员
        /// <summary>
        /// 获取  某职务下的人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetEmployee(string id = "1")
        {
            id = id.Contains("_") ? id.Split('_')[1] : id;
            List<StaffView> items = new List<StaffView>();
            //var users = this.positionService.GetPositionUsersWithFields(Guid.Parse(id));
            var users = this.positionService.GetPositionUsersWithRelationships(Guid.Parse(id));
            if (users != null)
            {
                users = users.OrderBy(x => x.UserName);
            }
            foreach (var user in users)
            {
                StaffView staff = new StaffView()
                {
                    StaffId = user.SysID.ToString(),
                    DisplayName = user.FullName,
                    UserName = user.UserName,
                    Department = ((user.Nodes == null || user.Nodes.Count == 0) ? "" : user.Nodes.FirstOrDefault().Name),
                    Position = ((user.Positions == null || user.Positions.Count == 0) ? "" : user.Positions.FirstOrDefault().Name)
                };
                items.Add(staff);
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPositionExtend(Guid id)
        { 
            var curPosition = this.positionService.ReadPositionWithFields(id);

            var curPosFields = new List<ExtensionFieldDto>(); ;
            if (curPosition == null || curPosition.ExtendItems == null)
            {
                curPosFields = new List<ExtensionFieldDto>();
            }
            else
            {
                curPosFields = curPosition.ExtendItems.ToList();
            }

            return Json(curPosFields, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  批量删除 某职务下的人员    -------已经弃用
        /// <summary>
        /// 批量删除 某职务下的人员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDestroyEmployee(string id, List<string> idList)
        {
            foreach (var item in idList)
            {
                positionBO.RemoveUser(Guid.Parse(id), Guid.Parse(item));
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 批量增加 某职务下的人员       -------已经弃用
        /// <summary>
        /// 批量增加 某职务下的人员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="staffid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoCreateEmployee(string id, List<string> idList)
        {
            List<StaffView> items = new List<StaffView>();
            foreach (var item in idList)
            {
                var IsUserInPosition = positionBO.PositionUserExists(Guid.Parse(item), Guid.Parse(id)); //IsUserOfPosition
                if (IsUserInPosition == false)
                {
                    positionBO.AppendUser(Guid.Parse(id), Guid.Parse(item));

                    items.Add(GetStaffView(Guid.Parse(item)));
                }
            }
            //IEnumerable<Models.ViewModel.StaffView> items = StaffList.Take(10);

            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public StaffView GetStaffView(Guid id)
        {
            var item = userService.ReadUserWithFields(id); //userBO.ReadUser(id);
            string TelNo = "";
            string ChineseName = "";
            string FaxNo = "";
            string StaffNo = "";     
            IList<ExtensionFieldDto> test = item.ExtendItems;
            //foreach (var fieldItem in test)
            //{
            //    switch (fieldItem.PropertyName)
            //    {
            //        case "JobClass": JobClass = fieldItem.ValueString;
            //            break;
            //        case "JobTitle": JobTitle = fieldItem.ValueString;
            //            break;
            //        case "JobRank": JobRank = Convert.ToInt32(fieldItem.ValueNumber.Value);
            //            break;
            //        case "ChineseName": ChineseName = fieldItem.ValueString;
            //            break;
            //        case "TelNo": TelNo = fieldItem.ValueString;
            //            break;
            //        case "FaxNo": FaxNo = fieldItem.ValueString;
            //            break;
            //        case "Department": Department = fieldItem.ValueString;
            //            break;
            //        case "StaffNo": StaffNo = fieldItem.ValueString;
            //            break;
            //    }
            //}

            return new StaffView
            {
                StaffNo = StaffNo,     
                FaxNo = FaxNo,
                TelNo = TelNo,
                ChineseName = ChineseName,
                UserName = item.UserName,
                Remark = item.Remark,
                Sex = (item.Sex == "Male" ? "Male" : "Female"),
                StaffId = item.SysID.ToString(),
                Status = (item.Status == "True" ? true : false),
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                MobileNo = item.Phone,
                DisplayName = item.FirstName + " " + item.LastName
            };

        }

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

        #region 查询 职务 列表
        /// <summary>
        /// 查询 职务 列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResult FindPosition(string input)
        {

            IEnumerable<PositionWithCategoryDto> positionlist;
            if (input == "")
                positionlist = positionService.GetAllPositonsWithCategory();//positionBO.GetAllPositons();
            else
            {
                var exp = new LogicalExpression(LogicalKind.And)
                {
                    Fields = new List<Field>
                        {
                            new Field("Name", input, "Position", OperatorKind.Contain)
                        }
                };
                positionlist = positionService.GetPositions(exp); //positionBO.GetPositions(exp);
            }
            List<Position> items = new List<Position>();
            foreach (var item in positionlist)
            {
                Position positionitem = new Position
                {
                    PositionID = item.SysID.ToString(),
                    DisplayName = item.Name,
                    CategoryID = item.Category.SysID.ToString()
                };
                items.Add(positionitem);
            }
            //IEnumerable<Models.Position> items = Positions.Skip(10).Take(2);
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
