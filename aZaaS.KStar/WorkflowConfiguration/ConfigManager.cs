using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

using AutoMapper;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.SQLQuery;

using aZaaS.KStar.Workflow.Configuration.Models;
using aZaaS.KStar.Repositories;
using aZaaS.Framework.Workflow.Util;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Localization;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Workflow.Pager;
using System.Collections;
using aZaaS.KStar.WorkflowConfiguration.Models;
using aZaaS.KStar.WorkflowConfiguration;
using aZaaS.Framework.Logging;
using ExpressionEvaluator;
using aZaaS.KStar.CustomExpression;



namespace aZaaS.KStar.Workflow.Configuration
{
    public class ConfigManager
    {
        private WFManagementFacade _managementFacade;
        private readonly PositionService _positionService;
        private readonly OrgChartService _chartService;
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly BusinessDataBO _businessdataBO;
        private string _defaultCategory = "NonConfiguration_Category";
        private string _tenantID = string.Empty;
        public string DefaultCategory
        {
            get
            {
                return _defaultCategory;
            }
            set
            {
                this._defaultCategory = value;
            }
        }

        public string TenantID
        {
            get
            {
                return _tenantID;
            }
            set
            {
                this._tenantID = value;
            }
        }


        public ConfigManager(AuthenticationType authType)
        {
            _managementFacade = new WFManagementFacade(authType);
            _userService = new UserService();
            _positionService = new PositionService();
            _chartService = new OrgChartService();
            _roleService = new RoleService();
            _businessdataBO = new BusinessDataBO();
        }

        #region 流程发起

        /// <summary>
        /// 保存常用流程
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="configuration_ProcessSetID"></param>
        public void SaveCommonProcess(string currentUser, int configuration_ProcessSetID)
        {
            if (configuration_ProcessSetID <= 0)
                return;

            var common = new Configuration_ProcCommon() { UserName = currentUser, ConfigProcSetID = configuration_ProcessSetID };
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                dbContext.Configuration_ProcCommonSet.Add(common);
                dbContext.SaveChanges();
            }
        }

        public void DeleteCommonProcess(string currentUser, int configuration_ProcessSetID)
        {
            if (configuration_ProcessSetID <= 0)
                return;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var common = dbContext.Configuration_ProcCommonSet.SingleOrDefault(r => r.UserName == currentUser && r.ConfigProcSetID == configuration_ProcessSetID);
                if (common != null)
                {
                    dbContext.Configuration_ProcCommonSet.Remove(common);
                    dbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<Configuration_CategoryDTO> GetStartProcessList(string currentUser, bool byCommon, int ProcessCategory, string ProcessName)
        {
            //Guid currentUserID = new UserBO().ReadUser(currentUser).SysID;
            List<Configuration_CategoryDTO> list = new List<Configuration_CategoryDTO>();

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                List<Configuration_Category> categoryList = null;
                var commonList = dbContext.Configuration_ProcCommonSet.Where(r => r.UserName == currentUser).ToList();
                if (byCommon)
                {
                    if (commonList == null || commonList.Count() == 0)
                    {
                        return list;
                    }
                }
                if (ProcessCategory > 0)
                {
                    categoryList = dbContext.Configuration_CategorySet.Where(x => x.ID == ProcessCategory).ToList();
                }
                else
                {
                    categoryList = dbContext.Configuration_CategorySet.ToList();
                }
                foreach (var category in categoryList)
                {
                    var categoryDto = ConvertToCategoryDTO(category);
                    var processSetList = GetProcessSetListByCategory(currentUser, category.ID);
                    if (!string.IsNullOrEmpty(ProcessName))
                    {
                        processSetList = processSetList.Where(x => x.ProcessName.Contains(ProcessName)).ToList();
                    }

                    foreach (var item in processSetList)
                    {
                        if (VerfyStartPermission(item.ProcessFullName, currentUser))
                        {
                            if (commonList.Exists(r => r.ConfigProcSetID == item.ID))
                            {
                                item.IsCommon = true;
                            }
                            if (!byCommon)
                            {
                                categoryDto.ProcessSetList.Add(item);
                            }
                            else if (item.IsCommon)
                            {
                                categoryDto.ProcessSetList.Add(item);
                            }
                        }
                    }

                    if (categoryDto.ProcessSetList.Count > 0)
                        list.Add(categoryDto);
                }
            }
            return list;
        }

        private bool VerfyStartPermission(Configuration_ProcessSetDTO procSetDto, Guid currentUserID)
        {
            PositionBO positionBO = new PositionBO();
            OrgChartBO orgCharBO = new OrgChartBO();
            foreach (var user in procSetDto.StartUserList)
            {
                switch (user.UserType)
                {
                    case Configuration_UserType.User:
                        if (user.Key.ToLower() == currentUserID.ToString().ToLower())
                            return true;
                        break;
                    case Configuration_UserType.Position:
                        if (positionBO.PositionUserExists(currentUserID, new Guid(user.Key)))
                            return true;
                        break;
                    case Configuration_UserType.OrgNode:
                        if (orgCharBO.NodeUserExists(currentUserID, new Guid(user.Key)))
                            return true;
                        break;
                    case Configuration_UserType.CustomType:
                        //暂时未实现
                        break;
                    case Configuration_UserType.Role:
                        if (_userService.UserRoleExists(Guid.Parse(user.Key), currentUserID))
                            return true;
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// 验证是否有发起权限
        /// </summary>
        /// <param name="procSetID"></param>
        /// <param name="currentUserID"></param>
        /// <param name="includeAct"></param>
        /// <returns></returns>
        public bool VerfyStartPermission(string processFullName, string userName)
        {
            string result = string.Empty;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                SqlParameter[] sp = new SqlParameter[2];
                sp[0] = new SqlParameter("ProcessFullName", processFullName);
                sp[1] = new SqlParameter("UserName", userName);
                result = dbContext.Database.SqlQuery(typeof(string), "exec CheckAuthorizationAccessProcess @ProcessFullName,@UserName", sp).Cast<string>().First();
            }
            if (result == "1")
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Category

        /// <summary>
        /// 获取所有配置类别及子项,仅返回ID和Name
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        public IEnumerable<Configuration_CategoryDTO> GetAllCategoryNameList(string currentUser)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            List<Configuration_CategoryDTO> list = new List<Configuration_CategoryDTO>();
            Configuration_CategoryDTO defaultCategory = new Configuration_CategoryDTO()
            {
                ID = 0,
                Description = "",
                Name = _defaultCategory,
                ProcessSetList = new List<Configuration_ProcessSetDTO>()
            };
            list.Add(defaultCategory);

            List<ProcessSet> procSetList = _managementFacade.GetProcessSets(context);
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var categoryList = dbContext.Configuration_CategorySet.ToList();
                foreach (var category in categoryList)
                {
                    list.Add(ConvertToCategoryDTO(category));
                }
                foreach (var procSet in procSetList)
                {
                    var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessSetID == procSet.ID && r.ProcessFullName == procSet.FullName);
                    var setDTO = ConvertToProcSetDTO(procSet, set);
                    SetConfiguration_ProcessVersionDTO(context, dbContext, setDTO, false);
                    foreach (var version in setDTO.ProcessVersionList)
                    {
                        SetConfiguration_ActivityDTO(context, dbContext, version, false);
                    }

                    var category = list.SingleOrDefault(r => r.ID == setDTO.Configuration_CategoryID);
                    if (category == null)
                    {
                        defaultCategory.ProcessSetList.Add(setDTO);
                    }
                    else
                    {
                        category.ProcessSetList.Add(setDTO);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有配置类别
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public IEnumerable<Configuration_CategoryDTO> GetAllCategoryList(string currentUser, bool include = false)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            List<string> processnamelist = new List<string>();
            //根据用户角色获取流程权限
            List<Role_ProcessSetDTO> roleprocess = GetRoleProcessSet(currentUser, false);
            if (roleprocess != null)
            {
                roleprocess.ForEach(x => { processnamelist.Add(x.ProcessFullName); });
            }
            List<Configuration_CategoryDTO> list = new List<Configuration_CategoryDTO>();
            Configuration_CategoryDTO defaultCategory = new Configuration_CategoryDTO()
            {
                ID = 0,
                Description = "",
                Name = _defaultCategory,
                ProcessSetList = new List<Configuration_ProcessSetDTO>()
            };
            list.Add(defaultCategory);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var categoryList = dbContext.Configuration_CategorySet.ToList();
                foreach (var category in categoryList)
                {
                    list.Add(ConvertToCategoryDTO(category));
                }
            }

            if (include)
            {
                List<ProcessSet> procSetList = _managementFacade.GetProcessSets(context);
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    foreach (var procSet in procSetList)
                    {
                        //过滤当前用户角色，有权限访问的流程
                        if (processnamelist.Contains(procSet.FullName))
                        {
                            var setDTO = GetConfiguration_ProcessSetDTO(context, dbContext, procSet, true);
                            var category = list.SingleOrDefault(r => r.ID == setDTO.Configuration_CategoryID);
                            if (category == null)
                            {
                                defaultCategory.ProcessSetList.Add(setDTO);
                            }
                            else
                            {
                                category.ProcessSetList.Add(setDTO);
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取单个配置类别
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="id">主键</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public Configuration_CategoryDTO GetCategoryByID(string currentUser, int id, bool include = false)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var category = dbContext.Configuration_CategorySet.SingleOrDefault(r => r.ID == id);
                if (category == null)
                {
                    category = new Configuration_Category()
                    {
                        ID = 0,
                        Description = "",
                        Name = _defaultCategory
                    };
                }

                var dto = ConvertToCategoryDTO(category);
                if (include)
                {
                    dto.ProcessSetList.AddRange(GetProcessSetListByCategory(currentUser, id, true));
                }
                return dto;
            }

        }

        /// <summary>
        /// 添加单个配置类别
        /// </summary>
        /// <param name="category">配置类别</param>
        /// <returns>Configuration_Category ID</returns>
        public int AddCategory(Configuration_CategoryDTO category)
        {
            var config = ConvertToCategory(category);
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                dbContext.Configuration_CategorySet.Add(config);
                dbContext.SaveChanges();
            }
            return config.ID;
        }

        /// <summary>
        /// 修改单个配置类别
        /// </summary>
        /// <param name="category">配置类别</param>
        /// <returns></returns>
        public bool UpdateCategory(Configuration_CategoryDTO category)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var config = dbContext.Configuration_CategorySet.SingleOrDefault(r => r.ID == category.ID);
                if (config == null)
                    return false;

                config.Name = category.Name;
                config.Description = category.Description;
                dbContext.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 删除单个配置类别
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public bool DeleteCategory(int id)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var category = dbContext.Configuration_CategorySet.SingleOrDefault(r => r.ID == id);
                if (category == null)
                    return false;

                var setList = dbContext.Configuration_ProcessSetSet.Where(r => r.Configuration_CategoryID == id);
                foreach (var item in setList)
                {
                    item.Configuration_CategoryID = 0;
                }
                dbContext.Configuration_CategorySet.Remove(category);
                dbContext.SaveChanges();
            }
            return true;
        }

        #endregion

        #region ProcessSet

        /// <summary>
        /// 初始化流程集配置
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="procSetID">流程集ID</param>
        /// <param name="configuration_CategoryID">流程分类ID</param>
        public void InitProcessSet(string currentUser, int procSetID, int configuration_CategoryID)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            //Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria();
            //criteria.AddRegularFilter(new Framework.Workflow.Pager.RegularFilter(Framework.Workflow.Pager.CriteriaLogical.None, "ID", Framework.Workflow.Pager.CriteriaCompare.Equal, procSetID));
            var setList = _managementFacade.GetProcessSets(context);

            ProcessSet procSet = setList.Single(r => r.ID == procSetID);
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                if (dbContext.Configuration_ProcessSetSet.Where(r => r.ProcessFullName == procSet.FullName && r.ProcessSetID == procSet.ID).Count() > 0)
                    return;
            }

            Configuration_ProcessSet set = new Configuration_ProcessSet()
            {
                ApproveUrl = "",
                Configuration_CategoryID = configuration_CategoryID,
                Description = procSet.Descr,
                NotAssignIfApproved = false,
                OrderNo = 0,
                ProcessFullName = procSet.FullName,
                ProcessName = procSet.DisplayName,
                ProcessSetID = procSet.ID,
                ProcessSetNo = procSet.ID.ToString(),
                StartUrl = procSet.StartPageUrl,
                ViewUrl = procSet.ViewPageUrl,
                ProcessVersionList = new List<Configuration_ProcessVersion>()
            };

            var criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
            var procVersionList = _managementFacade.GetProcessVersions(context, procSetID, criteria);
            foreach (var version in procVersionList)
            {
                Configuration_ProcessVersion configVersion = GetConfiguration_ProcessVersion(version.ID, 0, context);
                set.ProcessVersionList.Add(configVersion);
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                foreach (var version in set.ProcessVersionList)
                {
                    dbContext.Configuration_ActivitySet.AddRange(version.ActivityList);
                    dbContext.Configuration_ProcessVersionSet.Add(version);
                }
                dbContext.Configuration_ProcessSetSet.Add(set);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 获取配置类别下的所有流程集
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="categoryId">配置类别ID</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public IEnumerable<Configuration_ProcessSetDTO> GetProcessSetListByCategory(string currentUser, int categoryId, bool include = false)
        {
            List<Configuration_ProcessSetDTO> list = new List<Configuration_ProcessSetDTO>();
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (categoryId > 0)
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var setList = dbContext.Configuration_ProcessSetSet.Where(r => r.Configuration_CategoryID == categoryId);
                    foreach (var set in setList)
                    {
                        var setDTO = GetConfiguration_ProcessSetDTO(context, dbContext, null, set, include);
                        list.Add(setDTO);
                    }
                }
            }
            else
            {
                List<ProcessSet> procSetList = _managementFacade.GetProcessSets(context);
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    foreach (var procSet in procSetList)
                    {
                        if (dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessFullName == procSet.FullName && r.ProcessSetID == procSet.ID) == null)
                        {
                            var setDTO = GetConfiguration_ProcessSetDTO(context, dbContext, procSet, null, include);
                            list.Add(setDTO);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取配置流程集列表
        /// </summary>
        public List<Configuration_ProcessSetDTO> GetProcessSets()
        {
            var dtos = new List<Configuration_ProcessSetDTO>();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var processSet = dbContext.Configuration_ProcessSetSet.ToList();
                if (processSet == null)
                    return dtos;

                foreach (var process in processSet)
                {
                    dtos.Add(new Configuration_ProcessSetDTO() { ProcessFullName = process.ProcessFullName, ProcessName = process.ProcessName });
                }
            }
            return dtos;
        }

        /// <summary>
        /// 获取配置流程集列表
        /// </summary>
        public List<Configuration_ProcessSetDTO> GetProcessSets(string currentUser, bool include = false)
        {
            List<Configuration_ProcessSetDTO> dtos = new List<Configuration_ProcessSetDTO>();
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var set = dbContext.Configuration_ProcessSetSet.ToList();
                if (set == null)
                    return null;
                foreach (var process in set)
                {
                    dtos.Add(GetConfiguration_ProcessSetDTO(context, dbContext, null, process, include));
                }
            }
            return dtos;
        }


        /// <summary>
        /// 获取单个配置流程集
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="Configuration_ProcessSetID">Configuration_ProcessSetDTO ID</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public Configuration_ProcessSetDTO GetProcessSetByID(string currentUser, int Configuration_ProcessSetID, bool include = false)
        {
            Configuration_ProcessSetDTO dto = null;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (Configuration_ProcessSetID > 0)
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ID == Configuration_ProcessSetID);
                    if (set == null)
                        return null;

                    dto = GetConfiguration_ProcessSetDTO(context, dbContext, null, set, include);
                }
            }
            //else if (dto.ProcessSetID > 0)
            //{
            //    Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria();
            //    criteria.AddRegularFilter(new Framework.Workflow.Pager.RegularFilter(Framework.Workflow.Pager.CriteriaLogical.None, "ID", Framework.Workflow.Pager.CriteriaCompare.Equal, dto.ProcessSetID));
            //    ProcessSet procSet = _managementFacade.GetProcessSets(context, criteria).FirstOrDefault();
            //    using (KStarDbContext dbContext = new KStarDbContext())
            //    {
            //        dto = GetConfiguration_ProcessSetDTO(context, dbContext, procSet, include);

            //    }
            //}
            return dto;
        }

        public Configuration_ProcessSetDTO GetProcessSetByProcInstID(string currentUser, int procInstID, bool include = false)
        {
            Configuration_ProcessSetDTO dto = null;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (procInstID > 0)
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessSetID == procInstID);
                    if (set == null)
                        return null;

                    dto = GetConfiguration_ProcessSetDTO(context, dbContext, null, set, include);
                }
            }
            return dto;
        }


        /// <summary>
        /// 获取单个配置流程集
        /// </summary>
        public Configuration_ProcessSetDTO GetProcessSetByFullName(string processFullName)
        {
            if (string.IsNullOrEmpty(processFullName))
                throw new ArgumentNullException("processFullName");

            Configuration_ProcessSetDTO dto = null;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessFullName == processFullName);
                if (set == null)
                    return null;

                dto = new Configuration_ProcessSetDTO()
                {
                    ID = set.ID,
                    ProcessName = set.ProcessName,
                    OrderNo = set.OrderNo,
                    ProcessFullName = set.ProcessFullName,
                    ProcessSetID = set.ProcessSetID,
                    ProcessSetNo = set.ProcessSetNo,
                    NotAssignIfApproved = set.NotAssignIfApproved,
                    StartUrl = set.StartUrl,
                    ViewUrl = set.ViewUrl,
                    ApproveUrl = set.ApproveUrl,
                    Description = set.Description
                };

            }
            return dto;
        }

        /// <summary>
        /// 获取单个配置流程集
        /// </summary>
        public Configuration_ProcessSetDTO GetProcessSetByFullName(string currentUser, string fullname, bool include = false)
        {
            Configuration_ProcessSetDTO dto = null;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (fullname.Length > 0)
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessFullName == fullname);
                    if (set == null)
                        return null;

                    dto = GetConfiguration_ProcessSetDTO(context, dbContext, null, set, include);
                }
            }
            return dto;
        }

        /// <summary>
        /// 获取流程 环节处理时效
        /// </summary>
        /// <param name="processFullName"></param>
        /// <param name="activityId"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public DateTime? GetProcessTime(string processFullName, int activityId, DateTime startDate)
        {
            DateTime? dt = new DateTime();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.Configuration_ProcessSetSet.Include("ProcessVersionList").FirstOrDefault(r => r.ProcessFullName == processFullName);
                if (item != null)
                {
                    List<Configuration_Activity> list = new List<Configuration_Activity>();
                    foreach (var version in item.ProcessVersionList)
                    {
                        if (version != null && version.ActivityList != null && version.ActivityList.Count > 0)
                        {
                            list.AddRange(version.ActivityList);
                        }
                    }

                    Configuration_Activity activity = list.Where(x => x.ActivityID == activityId).FirstOrDefault();
                    if (activity != null && activity.ProcessTime != null)
                    {
                        dt = startDate.AddHours(Convert.ToDouble(8 * activity.ProcessTime));
                    }
                    else
                    {
                        dt = null;
                    }
                }
            }
            return dt;
        }

        public string GetProcessFullNameByStartUrl(string startUrl, string realPath)
        {
            if (string.IsNullOrWhiteSpace(startUrl))
            {
                return string.Empty;
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.Configuration_ProcessSetSet.FirstOrDefault(r => r.StartUrl == startUrl || r.StartUrl == realPath);

                if (item == null)
                {
                    return string.Empty;
                }

                return item.ProcessFullName;
            }
        }

        public string GetProcessNameByStartUrl(string startUrl)
        {
            if (string.IsNullOrWhiteSpace(startUrl))
            {
                return string.Empty;
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.Configuration_ProcessSetSet.FirstOrDefault(r => r.StartUrl == startUrl || startUrl.EndsWith(r.StartUrl));

                if (item == null)
                {
                    return string.Empty;
                }

                return item.ProcessName;
            }
        }

        /// <summary>
        /// 获取流程配置中文名
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public string GetProcessSetByFullName(string currentUser, string fullname)
        {
            Configuration_ProcessSetDTO process = GetProcessSetByFullName(currentUser, fullname, false);
            if (process == null)
            {
                return fullname;
            }
            return process.ProcessName;
        }

        public Configuration_ProcessSetDTO GetProcessSetByActivityId(string currentUser, int activityId)
        {
            Configuration_ProcessSetDTO dto = null;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (activityId > 0)
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var act = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ActivityID == activityId);
                    if (act == null)
                    {
                        var version = dbContext.Configuration_ProcessVersionSet.SingleOrDefault(r => r.ID == act.Configuration_ProcessVersionID);
                        if (version != null)
                        {
                            var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ID == version.Configuration_ProcessSetID);
                            if (set == null)
                                return null;

                            dto = GetConfiguration_ProcessSetDTO(context, dbContext, null, set, false);
                        }
                    }
                }
            }
            return dto;
        }


        /// <summary>
        /// 修改流程集配置
        /// </summary>
        /// <param name="dto">Configuration_ProcessSetDTO</param>
        /// <returns></returns>
        public bool UpdateProcessSet(Configuration_ProcessSetDTO dto, bool include = false)
        {
            int procSetId = dto.ID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var config = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ID == dto.ID);
                bool isNew = config == null;

                ConvertSetProcSet(dto, config);

                if (isNew)
                {
                    dbContext.Configuration_ProcessSetSet.Add(config);
                    dbContext.SaveChanges();
                    procSetId = config.ID;
                    dto.ID = config.ID;
                }

                UpdateEndCc(dbContext, procSetId, dto.EndCc);
                UpdateReworkCc(dbContext, procSetId, dto.ReworkCc);

                if (include)
                {
                    UpdateStartUserList(dto.ID, dto.StartUserList, false);
                    UpdateProcVersion(dto.ID, dto.ProcessVersionList);
                }
                dbContext.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 修改流程配置的发起人
        /// </summary>
        /// <param name="procSetId">Configuration_ProcessSet ID</param>
        /// <param name="startUserList">发起人列表</param>
        /// <returns></returns>
        public bool UpdateStartUserList(int procSetId, IEnumerable<Configuration_UserDTO> startUserList, bool isDeleteNotExist = true)
        {
            if (procSetId <= 0)
                return false;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var userList = GetConfiguration_StartUserList(dbContext, procSetId);
                foreach (var item in startUserList)
                {
                    Configuration_User config = userList.SingleOrDefault(r => r.ID == item.ID);
                    bool isNew = config == null;
                    item.OperateType = Configuration_OperationType.StartProcess;
                    item.RefType = Configuration_RefType.ProcessSet;
                    item.RefID = procSetId;
                    ConvertSetUser(item, ref config);
                    if (isNew)
                    {
                        dbContext.Configuration_UserSet.Add(config);
                    }
                }
                //删除已经不存在的
                if (isDeleteNotExist)
                {
                    foreach (var item in userList)
                    {
                        var org = startUserList.SingleOrDefault(r => r.ID == item.ID);
                        if (org == null)
                        {
                            var ditem = dbContext.Configuration_UserSet.FirstOrDefault(r => r.ID == item.ID);
                            dbContext.Configuration_UserSet.Remove(ditem);
                        }
                    }
                }

                dbContext.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 获取当前用户可访问的流程
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<Role_ProcessSetDTO> GetRoleProcessSet(string username, bool include = true)
        {
            List<Role_ProcessSetDTO> process = new List<Role_ProcessSetDTO>();
            List<Guid> roleids = new List<Guid>();
            //获取角色
            IEnumerable<RoleBaseDto> roles = _roleService.GetUserRoles(username);
            if (roles != null)
            {
                roles.ToList().ForEach(x =>
                {
                    roleids.Add(x.SysID);
                });
            }
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var set = dbContext.Role_ProcessSet.Where(x => roleids.Contains(x.Role_SysId)).ToList();
                if (set != null)
                {
                    if (include)
                    {
                        set.ForEach(x =>
                        {
                            process.Add(new Role_ProcessSetDTO()
                            {
                                Role_SysId = x.Role_SysId,
                                ProcessFullName = x.ProcessFullName,
                                ProcessDispalyName = GetProcessSetByFullName(username, x.ProcessFullName),
                                RoleName = roles.Where(r => r.SysID == x.Role_SysId).FirstOrDefault().Name
                            });
                        });
                    }
                    else
                    {
                        set.ForEach(x =>
                         {
                             process.Add(new Role_ProcessSetDTO()
                             {
                                 Role_SysId = x.Role_SysId,
                                 ProcessFullName = x.ProcessFullName
                             });
                         });
                    }
                }
            }
            return process;
        }

        public List<Role_ProcessSetDTO> GetRoleProcessSet(Guid roleid, string currentUser)
        {
            List<Role_ProcessSetDTO> process = new List<Role_ProcessSetDTO>();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var set = dbContext.Role_ProcessSet.Where(x => x.Role_SysId == roleid).ToList();
                if (set != null)
                {
                    set.ForEach(x =>
                    {
                        process.Add(new Role_ProcessSetDTO()
                        {
                            Role_SysId = x.Role_SysId,
                            ProcessFullName = x.ProcessFullName,
                            ProcessDispalyName = GetProcessSetByFullName(currentUser, x.ProcessFullName),
                            RoleName = ""
                        });
                    });
                }
            }
            return process;
        }

        public bool CheckRoleProcessSet(string userName, string processFullName)
        {
            //获取角色
            IEnumerable<RoleBaseDto> roles = _roleService.GetUserRoles(userName);

            if (roles == null || roles.Count() == 0)
            {
                return false;
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var set = dbContext.Role_ProcessSet.Where(x => x.ProcessFullName == processFullName).ToList();

                foreach (var role in roles)
                {
                    if (set.FirstOrDefault(r => r.Role_SysId == role.SysID) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddRoleProcessSet(Guid roleid, string processFullName)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var process = dbContext.Role_ProcessSet.Where(x => x.Role_SysId == roleid && x.ProcessFullName == processFullName).FirstOrDefault();
                if (process == null)
                {
                    dbContext.Role_ProcessSet.Add(new Role_ProcessSet() { ProcessFullName = processFullName, Role_SysId = roleid });
                    dbContext.SaveChanges();
                }
            }
        }

        public void DeleteRoleProcessSet(Guid roleid, string processFullName)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var process = dbContext.Role_ProcessSet.Where(x => x.Role_SysId == roleid && x.ProcessFullName == processFullName).FirstOrDefault();
                if (process != null)
                {
                    dbContext.Role_ProcessSet.Remove(process);
                    dbContext.SaveChanges();
                }
            }
        }

        private void UpdateEndCc(KStarDbContext dbContext, int procSetId, IEnumerable<Configuration_UserDTO> endCc)
        {
            if (procSetId <= 0)
                return;

            var userList = GetConfiguration_EndCc(dbContext, procSetId);
            foreach (var item in endCc)
            {
                Configuration_User config = userList.SingleOrDefault(r => r.ID == item.ID);
                bool isNew = config == null;
                item.OperateType = Configuration_OperationType.EndCc;
                item.RefType = Configuration_RefType.ProcessSet;
                item.RefID = procSetId;
                ConvertSetUser(item, ref config);
                if (isNew)
                {
                    dbContext.Configuration_UserSet.Add(config);
                }
            }
            //删除已经不存在的
            foreach (var item in userList)
            {
                var org = endCc.SingleOrDefault(r => r.ID == item.ID);
                if (org == null)
                {
                    var ditem = dbContext.Configuration_UserSet.FirstOrDefault(r => r.ID == item.ID);
                    dbContext.Configuration_UserSet.Remove(ditem);
                }
            }
        }

        private void UpdateReworkCc(KStarDbContext dbContext, int procSetId, IEnumerable<Configuration_UserDTO> reworkCc)
        {
            if (procSetId <= 0)
                return;

            var userList = GetConfiguration_ReworkCc(dbContext, procSetId);
            foreach (var item in reworkCc)
            {
                Configuration_User config = userList.SingleOrDefault(r => r.ID == item.ID);
                bool isNew = config == null;
                item.OperateType = Configuration_OperationType.ReworkCc;
                item.RefType = Configuration_RefType.ProcessSet;
                item.RefID = procSetId;
                ConvertSetUser(item, ref config);
                if (isNew)
                {
                    dbContext.Configuration_UserSet.Add(config);
                }
            }
            //删除已经不存在的
            foreach (var item in userList)
            {
                var org = reworkCc.SingleOrDefault(r => r.ID == item.ID);
                if (org == null)
                {
                    var ditem = dbContext.Configuration_UserSet.FirstOrDefault(r => r.ID == item.ID);
                    dbContext.Configuration_UserSet.Remove(ditem);
                }
            }
        }

        /// <summary>
        /// 根据用户名获取相关流程
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<Configuration_User_ProcessSet> GetRelatedProcessSetByUserName(string userName)
        {
            List<Configuration_User_ProcessSet> list = new List<Configuration_User_ProcessSet>();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                SqlParameter sp = new SqlParameter("UserName", userName);
                list = dbContext.Database.SqlQuery<Configuration_User_ProcessSet>("exec GetRelatedProcessSetByUserName @UserName", sp).ToList();
            }
            return list;
        }

        public bool TransferStartPermission(string userName, string toUserName)
        {
            int result = 0;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                SqlParameter[] sp = new SqlParameter[2];
                sp[0] = new SqlParameter("UserName", userName);
                sp[1] = new SqlParameter("ToUserName", toUserName);
                result = dbContext.Database.ExecuteSqlCommand("exec UpdateProcessSetStartPerson @UserName,@ToUserName", sp);
            }
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public bool DeleteStartUserBySysId(Guid sysId, Configuration_UserType userType)
        {
            bool flag = false;
            try
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var user = dbContext.Configuration_UserSet.Where(x => x.UserType == userType.ToString() && x.Key == sysId.ToString()).FirstOrDefault();
                    if (user != null)
                    {
                        dbContext.Configuration_UserSet.Remove(user);
                        dbContext.SaveChanges();
                    }
                    flag = true;
                }
            }
            catch (Exception ex) { }
            return flag;
        }
        #endregion

        #region ProcessVersion
        /// <summary>
        /// 初始化流程版本配置
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="procID">流程版本ID</param>
        /// <param name="configuration_ProcessSetID">流程集的配置ID</param>
        public void InitProcessVersion(string currentUser, int procID, int configuration_ProcessSetID)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            var procSetID = 0;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                procSetID = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == configuration_ProcessSetID).ProcessSetID;
            }

            Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
            //criteria.AddRegularFilter(new Framework.Workflow.Pager.RegularFilter(Framework.Workflow.Pager.CriteriaLogical.None, "ID", Framework.Workflow.Pager.CriteriaCompare.Equal, procID));
            var procVersion = _managementFacade.GetProcessVersions(context, procSetID, criteria).SingleOrDefault(r => r.ID == procID);

            Configuration_ProcessVersion configVersion = GetConfiguration_ProcessVersion(procID, configuration_ProcessSetID, context);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                dbContext.Configuration_ActivitySet.AddRange(configVersion.ActivityList);
                dbContext.Configuration_ProcessVersionSet.Add(configVersion);
                dbContext.SaveChanges();
            }
        }

        private Configuration_ProcessVersion GetConfiguration_ProcessVersion(int procID, int configuration_ProcessSetID, ServiceContext context)
        {
            Configuration_ProcessVersion configVersion = new Configuration_ProcessVersion()
            {
                ProcessVersionID = procID,
                Configuration_ProcessSetID = configuration_ProcessSetID,
                ActivityList = new List<Configuration_Activity>()
            };

            var actList = this.GetProcessActivities(context, procID);
            foreach (var act in actList)
            {
                Configuration_Activity configAct = new Configuration_Activity()
                {
                    ActivityID = act.ID,
                    OperateUserList = new List<Configuration_User>(),
                    ReworkActivityList = new List<Configuration_RefActivity>()
                };
                configVersion.ActivityList.Add(configAct);
            }
            return configVersion;
        }

        /// <summary>
        /// 获取流程集下的所有版本
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="configuration_ProcessSetID">Configuration_ProcessSetDTO ID</param>
        /// <param name="procSetID">流程 ID</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public IEnumerable<Configuration_ProcessVersionDTO> GetProcessVersionListByProcessSet(string currentUser, int configuration_ProcessSetID, int procSetID, bool include = false)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            Configuration_ProcessSetDTO dto = null;
            List<Configuration_ProcessVersionDTO> list = new List<Configuration_ProcessVersionDTO>();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                if (configuration_ProcessSetID > 0)
                {
                    var set = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == configuration_ProcessSetID);
                    dto = ConvertToProcSetDTO(null, set);
                }
                else
                {
                    ProcessSet procSet = _managementFacade.GetProcessSets(context).SingleOrDefault(r => r.ID == procSetID);
                    dto = ConvertToProcSetDTO(procSet, null);
                }
                SetConfiguration_ProcessVersionDTO(context, dbContext, dto, include);
                list = dto.ProcessVersionList;
            }
            return list;
        }

        /// <summary>
        /// 导入流程版本配置
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="procID">流程版本ID</param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public bool ImportConfigurationVersion(string currentUser, int procID, string xml)
        {
            Configuration_ProcessVersionDTO dto = XMLHelper.Deserialize<Configuration_ProcessVersionDTO>(xml);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                //现在导入操作的版本
                var config = dbContext.Configuration_ProcessVersionSet.SingleOrDefault(r => r.Configuration_ProcessSetID == dto.Configuration_ProcessSetID && r.ProcessVersionID == procID);
                //对新的ProcessVersion导入(未初始化的版本)
                if (config == null)
                {
                    config = new Configuration_ProcessVersion()
                    {
                        ProcessVersionID = procID,
                        Configuration_ProcessSetID = dto.Configuration_ProcessSetID,
                        ActivityList = new List<Configuration_Activity>()
                    };

                    ServiceContext context = new ServiceContext();
                    context.UserName = currentUser;
                    context.TenantID = _tenantID;
                    var actList = this.GetProcessActivities(context, procID);
                    foreach (var item in actList)
                    {
                        var configAct = new Configuration_Activity() { ActivityID = item.ID, OperateUserList = new List<Configuration_User>(), ReworkActivityList = new List<Configuration_RefActivity>() };
                        var configActDto = dto.ActivityList.SingleOrDefault(r => r.ActivityID == item.ID);
                        if (configActDto != null)
                        {
                            foreach (var user in configActDto.OperateUserList)
                            {
                                var configUser = new Configuration_User() { OperateType = Configuration_OperationType.OperateProcess.ToString(), RefType = Configuration_RefType.Activity.ToString(), UserType = user.UserType.ToString(), Value = user.Value, Key = user.Key };
                                configAct.OperateUserList.Add(configUser);
                                dbContext.Configuration_UserSet.Add(configUser);
                            }
                            foreach (var id in configActDto.ReworkActivityList)
                            {
                                var configRework = new Configuration_RefActivity() { ActivityID = id };
                                configAct.ReworkActivityList.Add(configRework);
                                dbContext.Configuration_RefActivitySet.Add(configRework);
                            }
                        }

                        config.ActivityList.Add(configAct);
                        dbContext.Configuration_ActivitySet.Add(configAct);

                        dbContext.Configuration_ProcessVersionSet.Add(config);
                    }
                }
                else //对旧的ProcessVersion导入
                {
                    //对原有版本导入操作
                    if (dto.ID == config.ID)
                    {
                        foreach (var act in dto.ActivityList)
                        {
                            UpdateOperateUserList(act.ID, act.OperateUserList, dbContext);
                            UpdateReworkActivityList(act.ID, act.ReworkActivityList, dbContext);
                        }
                    }
                    else //对非原有版本导入操作
                    {
                        var actList = dbContext.Configuration_ActivitySet.Where(r => r.Configuration_ProcessVersionID == config.ID);
                        foreach (var item in actList)
                        {
                            var act = dto.ActivityList.SingleOrDefault(r => r.ActivityID == item.ActivityID);
                            if (act != null)
                            {
                                UpdateOperateUserList(act.ID, act.OperateUserList, dbContext);
                                UpdateReworkActivityList(act.ID, act.ReworkActivityList, dbContext);
                            }
                        }
                    }
                }
                dbContext.SaveChanges();

            }
            return true;
        }

        /// <summary>
        /// 导出流程版本下的配置
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="configuration_ProcessVersionID">Configuration_ProcessVersionDTO ID</param>
        /// <returns></returns>
        public string ExportConfigurationVersion(string currentUser, int configuration_ProcessVersionID)
        {
            string xml = string.Empty;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var version = dbContext.Configuration_ProcessVersionSet.Single(r => r.ID == configuration_ProcessVersionID);
                var procSetID = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == version.Configuration_ProcessSetID).ProcessSetID;

                Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
                var procVersion = _managementFacade.GetProcessVersions(context, procSetID, criteria).SingleOrDefault(r => r.ID == version.ProcessVersionID);
                var dto = ConvertToProcVersionDTO(procVersion, version);
                SetConfiguration_ActivityDTO(context, dbContext, dto, true);

                xml = XMLHelper.Serializer(dto);
            }
            return xml;
        }

        public int SynchronizeConfigurationVersion(string currentUser, int configuration_ProcessSetID)
        {
            int count = 0;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            Configuration_ProcessSet set = new Configuration_ProcessSet();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                set = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == configuration_ProcessSetID);

                List<Configuration_ProcessVersion> oldVersionList = new List<Configuration_ProcessVersion>();
                oldVersionList.AddRange(set.ProcessVersionList);

                var criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
                var procVersionList = _managementFacade.GetProcessVersions(context, set.ProcessSetID, criteria);
                procVersionList = procVersionList.OrderBy(x => x.ID).ToList();
                foreach (var version in procVersionList)
                {
                    //存在新版本--添加                                       
                    Configuration_ProcessVersion v = oldVersionList.Where(x => x.ProcessVersionID == version.ID).FirstOrDefault();
                    if (v == null)
                    {
                        //获取新环节
                        var procactivity = this.GetProcessActivities(context, version.ID);
                        //排序
                        procactivity = procactivity.OrderBy(x => x.ID).ToList();

                        //获取最近一个版本的环节 并排序
                        Configuration_ProcessVersion latestversion = dbContext.Configuration_ProcessVersionSet.Where(x => x.Configuration_ProcessSetID == configuration_ProcessSetID).OrderByDescending(x => x.ProcessVersionID).FirstOrDefault();
                        List<Configuration_Activity> latestactivity = new List<Configuration_Activity>();
                        List<Activity> latestactivity_server = new List<Activity>();
                        if (latestversion != null && latestversion.ActivityList != null && latestversion.ActivityList.Count > 0)
                        {
                            latestactivity.AddRange(latestversion.ActivityList);
                            //获取k2服务器最近一个版本的环节                            
                            latestactivity_server.AddRange(this.GetProcessActivities(context, latestversion.ProcessVersionID));
                            latestactivity_server = latestactivity_server.OrderBy(x => x.ID).ToList();
                            for (int i = 0; i < latestactivity.Count; i++)
                            {
                                var act_k2act = latestactivity_server.Where(x => x.ID == latestactivity[i].ActivityID).FirstOrDefault();
                                if (act_k2act != null)
                                {
                                    latestactivity[i].Name = act_k2act.Name;
                                }
                            }
                            latestactivity = latestactivity.OrderBy(x => x.ID).ToList();
                        }


                        //同步新版本
                        Configuration_ProcessVersion _version = new Configuration_ProcessVersion() { ProcessVersionID = version.ID, Configuration_ProcessSetID = configuration_ProcessSetID };
                        dbContext.Configuration_ProcessVersionSet.Add(_version);
                        dbContext.SaveChanges();
                        //同步环节
                        //记录已经同步过的环节
                        List<int> hadupdateact = new List<int>();
                        foreach (var act in procactivity)
                        {
                            Configuration_Activity _activity = new Configuration_Activity() { Configuration_ProcessVersionID = _version.ID, ActivityID = act.ID };
                            dbContext.Configuration_ActivitySet.Add(_activity);
                            dbContext.SaveChanges();
                            //同步环节处理人
                            List<Configuration_User> operateUserList = new List<Configuration_User>();
                            //获取最近一个版本第ind个环节                                 
                            var curact = latestactivity.Where(x => x.Name == act.Name).FirstOrDefault();
                            //获取最近一个版本第index个环节的处理人
                            if (curact != null && curact.OperateUserList != null && curact.OperateUserList.Count > 0)
                            {
                                var operateUser = curact.OperateUserList.ToArray();
                                foreach (var item in operateUser)
                                {
                                    dbContext.Configuration_UserSet.Add(new Configuration_User() { RefID = _activity.ID, RefType = item.RefType, OperateType = item.OperateType, Value = item.Value, Key = item.Key, UserType = item.UserType });
                                }
                            }
                            //同步环节模板控件配置
                            using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                            {
                                if (curact != null)
                                {
                                    List<ActivityControlSetting> acs = kfwContext.ActivityControlSetting.Where(x => x.ProcessFullName == set.ProcessFullName && x.ActivityId == curact.ActivityID).ToList();
                                    List<ActivityControlSetting> curacs = new List<ActivityControlSetting>();
                                    if (acs != null && acs.Count > 0)
                                    {
                                        acs.ForEach(item =>
                                        {
                                            int workmode = (int)item.WorkMode;
                                            var originalacs = kfwContext.ActivityControlSetting.Where(x => x.ProcessFullName == set.ProcessFullName && x.ActivityId == act.ID && x.WorkMode == workmode && x.ControlType == item.ControlType.ToString() && x.ControlRenderId == item.ControlRenderId && !hadupdateact.Contains(x.ActivityId)).FirstOrDefault();
                                            if (originalacs == null)
                                            {
                                                curacs.Add(new ActivityControlSetting()
                                                {
                                                    SysId = Guid.NewGuid(),
                                                    ActivityId = act.ID,
                                                    ControlName = item.ControlName,
                                                    ControlType = item.ControlType,
                                                    ControlRenderId = item.ControlRenderId,
                                                    IsCustom = item.IsCustom,
                                                    IsDisable = item.IsDisable,
                                                    IsHide = item.IsHide,
                                                    ProcessFullName = item.ProcessFullName,
                                                    RenderTemplateId = item.RenderTemplateId,
                                                    WorkMode = workmode
                                                });
                                            }
                                        });
                                        kfwContext.ActivityControlSetting.AddRange(curacs);
                                        kfwContext.SaveChanges();
                                        hadupdateact.Add(act.ID);
                                    }
                                }
                            }
                        }

                        //获取已同步的环节
                        var activity_save = dbContext.Configuration_ActivitySet.Where(x => x.Configuration_ProcessVersionID == _version.ID).OrderBy(x => x.ID).ToList();
                        //同步可退回环节
                        foreach (var act in activity_save)
                        {
                            List<Configuration_RefActivity> reworkActivityList = new List<Configuration_RefActivity>();

                            //获取最近一个版本的环节
                            var curact = latestactivity.Where(x => x.Name == act.Name).FirstOrDefault();
                            //获取最近一个版本的环节的可退回环节
                            if (curact != null && curact.ReworkActivityList != null && curact.ReworkActivityList.Count > 0)
                            {
                                //上一个版本本地服务器的环节的可退回环节信息
                                var reworkactivity = curact.ReworkActivityList.ToArray();
                                foreach (var item in reworkactivity)
                                {
                                    var caset = dbContext.Configuration_ActivitySet.Where(x => x.ID == item.ActivityID).FirstOrDefault();
                                    //获取名称
                                    var caset_server = latestactivity_server.Where(x => x.ID == caset.ActivityID).FirstOrDefault();
                                    //根据名称获取新环节的activeid
                                    var procactivityitem = procactivity.Where(x => x.Name == caset_server.Name).FirstOrDefault();
                                    if (procactivityitem != null)
                                    {
                                        //根据activeid获取本地环节的id
                                        var destact = activity_save.Where(x => x.ActivityID == procactivityitem.ID).FirstOrDefault();
                                        dbContext.Configuration_RefActivitySet.Add(new Configuration_RefActivity() { Configuration_ActivityID = act.ID, ActivityID = destact.ID });
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var version in oldVersionList)
                {
                    //残留旧版本--移除
                    ProcessVersion v = procVersionList.Where(x => x.ID == version.ProcessVersionID).FirstOrDefault();
                    if (v == null)
                    {
                        List<Configuration_Activity> activities = new List<Configuration_Activity>();
                        if (version.ActivityList != null && version.ActivityList.Count > 0)
                        {
                            activities.AddRange(version.ActivityList);
                        }
                        foreach (var activity in activities)
                        {
                            //移除可退回环节
                            List<Configuration_RefActivity> refactivities = dbContext.Configuration_RefActivitySet.Where(x => x.Configuration_ActivityID == activity.ID).ToList();
                            if (refactivities != null && refactivities.Count > 0) dbContext.Configuration_RefActivitySet.RemoveRange(refactivities);

                            //移除处理人
                            List<Configuration_User> users = dbContext.Configuration_UserSet.Where(x => x.RefID == activity.ID && x.RefType == Configuration_RefType.Activity.ToString()).ToList();
                            if (users != null && users.Count > 0) dbContext.Configuration_UserSet.RemoveRange(users);

                            //移除环节控件配置
                            using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                            {
                                List<ActivityControlSetting> acs = kfwContext.ActivityControlSetting.Where(x => x.ProcessFullName == set.ProcessFullName && x.ActivityId == activity.ActivityID).ToList();
                                kfwContext.ActivityControlSetting.RemoveRange(acs);
                                kfwContext.SaveChanges();
                            }

                            //移除环节
                            dbContext.Configuration_ActivitySet.Remove(activity);
                        }
                        //移除版本
                        dbContext.Configuration_ProcessVersionSet.Remove(version);
                    }
                    else
                    {
                        var procactivity = this.GetProcessActivities(context, version.ProcessVersionID);
                        List<Configuration_Activity> oldactivity = new List<Configuration_Activity>();
                        oldactivity.AddRange(version.ActivityList);
                        foreach (var act in procactivity)
                        {
                            //存在新环节
                            Configuration_Activity a = oldactivity.Where(x => x.ActivityID == act.ID).FirstOrDefault();
                            if (a == null)
                            {
                                dbContext.Configuration_ActivitySet.Add(new Configuration_Activity() { ActivityID = act.ID, Configuration_ProcessVersionID = version.ID, OperateUserList = new List<Configuration_User>(), ReworkActivityList = new List<Configuration_RefActivity>() });
                            }
                        }

                        foreach (var activity in oldactivity)
                        {
                            //残留旧环节
                            Activity a = procactivity.Where(x => x.ID == activity.ActivityID).FirstOrDefault();
                            if (a == null)
                            {
                                //移除可退回环节
                                List<Configuration_RefActivity> refactivities = dbContext.Configuration_RefActivitySet.Where(x => x.Configuration_ActivityID == activity.ID).ToList();
                                if (refactivities != null && refactivities.Count > 0) dbContext.Configuration_RefActivitySet.RemoveRange(refactivities);

                                //移除处理人
                                List<Configuration_User> users = dbContext.Configuration_UserSet.Where(x => x.RefID == activity.ID && x.RefType == Configuration_RefType.Activity.ToString()).ToList();
                                if (users != null && users.Count > 0) dbContext.Configuration_UserSet.RemoveRange(users);

                                //移除环节控件配置
                                using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                                {
                                    List<ActivityControlSetting> acs = kfwContext.ActivityControlSetting.Where(x => x.ProcessFullName == set.ProcessFullName && x.ActivityId == activity.ActivityID).ToList();
                                    kfwContext.ActivityControlSetting.RemoveRange(acs);
                                    kfwContext.SaveChanges();
                                }
                                //移除环节
                                dbContext.Configuration_ActivitySet.Remove(activity);
                            }
                        }
                    }
                }
                count = set.ProcessVersionList.Count;
                dbContext.SaveChanges();
            }
            return count;
        }


        public Configuration_ProcessSetDTO ImportProcessConfiguration(string currentUser, int configuration_ProcessID, Configuration_ProcessConfigDTO dto)
        {
            Configuration_ProcessSetDTO result = null;
            Configuration_ProcessSet processset = null;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            if (dto != null)
            {
                //using (TransactionScope ts = new TransactionScope())
                //{       
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    processset = dbContext.Configuration_ProcessSetSet.Where(x => x.ID == configuration_ProcessID).FirstOrDefault();
                    if (processset != null && processset.ProcessFullName == dto.ProcessFullName)
                    {
                        processset.ProcessName = dto.ProcessName;
                        processset.OrderNo = dto.OrderNo;
                        processset.StartUrl = dto.StartUrl;
                        processset.ViewUrl = dto.ViewUrl;
                        processset.ApproveUrl = dto.ApproveUrl;
                        processset.NotAssignIfApproved = dto.NotAssignIfApproved;
                        processset.Description = dto.Description;
                        processset.ProcessPredict = dto.ProcessPredict;
                        processset.LoopRemark = dto.LoopRemark;


                        List<Configuration_UserDTO> users = new List<Configuration_UserDTO>();
                        if (dto.EndCc != null && dto.EndCc.Count > 0)
                        {
                            users.AddRange(dto.EndCc);
                        }
                        if (dto.ReworkCc != null && dto.ReworkCc.Count > 0)
                        {
                            users.AddRange(dto.ReworkCc);
                        }
                        if (dto.StartUserList != null && dto.StartUserList.Count > 0)
                        {
                            users.AddRange(dto.StartUserList);
                        }
                        foreach (var user in users)
                        {
                            var item = dbContext.Configuration_UserSet.Where(x => x.Key == user.Key && x.UserType == user.UserType.ToString() && x.RefType == user.RefType.ToString() && x.OperateType == user.OperateType.ToString() && x.RefID == processset.ID).FirstOrDefault();
                            if (item == null)
                            {
                                dbContext.Configuration_UserSet.Add(new Configuration_User()
                                {
                                    Key = user.Key,
                                    OperateType = user.OperateType.ToString(),
                                    RefType = user.RefType.ToString(),
                                    UserType = user.UserType.ToString(),
                                    Value = user.Value,
                                    RefID = processset.ID
                                });
                            }
                        }

                        var version = dbContext.Configuration_ProcessVersionSet.Where(x => x.Configuration_ProcessSetID == configuration_ProcessID).OrderByDescending(x => x.ProcessVersionID).FirstOrDefault();
                        if (version != null)
                        {
                            var activity = dbContext.Configuration_ActivitySet.Where(x => x.Configuration_ProcessVersionID == version.ID).ToList();
                            List<Activity> activity_server = new List<Activity>();

                            //获取k2服务器的环节                            
                            activity_server.AddRange(this.GetProcessActivities(context, version.ProcessVersionID));
                            activity_server = activity_server.OrderBy(x => x.ID).ToList();
                            for (int i = 0; i < activity.Count; i++)
                            {
                                var act_k2act = activity_server.Where(x => x.ID == activity[i].ActivityID).FirstOrDefault();
                                if (act_k2act != null)
                                {
                                    activity[i].Name = act_k2act.Name;
                                }
                            }
                            activity = activity.OrderBy(x => x.ID).ToList();
                            foreach (var act in activity)
                            {
                                var latestversion = dto.ProcessVersionList.OrderByDescending(x => x.ProcessVersionID).FirstOrDefault();
                                List<Configuration_Activity> latestactivity = new List<Configuration_Activity>();
                                if (latestversion != null && latestversion.ActivityList != null && latestversion.ActivityList.Count > 0)
                                {
                                    latestversion.ActivityList.ForEach(x =>
                                    {
                                        List<Configuration_User> operateuser = new List<Configuration_User>();
                                        x.OperateUserList.ForEach(j =>
                                        {
                                            operateuser.Add(ConvertToUser(j));
                                        });
                                        latestactivity.Add(new Configuration_Activity()
                                        {
                                            ActivityID = x.ActivityID,
                                            Configuration_ProcessVersionID = x.Configuration_ProcessVersionID,
                                            ProcessTime = x.ProcessTime,
                                            Name = x.Name,
                                            OperateUserList = operateuser,
                                            ReworkActivityList = ConvertToReActivity(x.ActivityID, x.ReworkActivityList)
                                        });
                                    });
                                    latestactivity = latestactivity.OrderBy(x => x.ID).ToList();
                                }
                                //同步环节处理人
                                List<Configuration_User> operateUserList = new List<Configuration_User>();
                                //获取最近一个版本的环节                                 
                                var curact = latestactivity.Where(x => x.Name == act.Name).FirstOrDefault();
                                //获取最近一个版本环节的处理人
                                if (curact != null && curact.OperateUserList != null && curact.OperateUserList.Count > 0)
                                {
                                    var operateUser = curact.OperateUserList.ToArray();
                                    foreach (var item in operateUser)
                                    {
                                        dbContext.Configuration_UserSet.Add(new Configuration_User() { RefID = act.ID, RefType = item.RefType, OperateType = item.OperateType, Value = item.Value, Key = item.Key, UserType = item.UserType });
                                    }
                                }
                                //同步环节模板控件配置
                                using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                                {
                                    List<Process_ControlSettingDTO> acs = dto.settings.Where(x => x.ProcessFullName == processset.ProcessFullName && x.ActivityId == curact.ActivityID).ToList();
                                    List<ActivityControlSetting> curacs = new List<ActivityControlSetting>();
                                    if (acs != null && acs.Count > 0)
                                    {
                                        acs.ForEach(item =>
                                        {
                                            curacs.Add(new ActivityControlSetting()
                                            {
                                                SysId = Guid.NewGuid(),
                                                ActivityId = act.ActivityID,
                                                ControlName = item.ControlName,
                                                ControlType = item.ControlType.ToString(),
                                                ControlRenderId = item.ControlRenderId,
                                                IsCustom = item.IsCustom,
                                                IsDisable = item.IsDisable,
                                                IsHide = item.IsHide,
                                                ProcessFullName = item.ProcessFullName,
                                                RenderTemplateId = item.RenderTemplateId,
                                                WorkMode = (int)item.WorkMode
                                            });
                                        });
                                        kfwContext.ActivityControlSetting.AddRange(curacs);
                                        kfwContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                        {
                            //更新流程控件配置
                            List<Process_ControlSettingDTO> process_settings = dto.settings.Where(x => x.ProcessFullName == processset.ProcessFullName && x.ActivityId == 0).ToList();
                            List<ActivityControlSetting> curacs = new List<ActivityControlSetting>();
                            if (process_settings != null && process_settings.Count > 0)
                            {
                                process_settings.ForEach(item =>
                                {
                                    curacs.Add(new ActivityControlSetting()
                                    {
                                        SysId = Guid.NewGuid(),
                                        ActivityId = 0,
                                        ControlName = item.ControlName,
                                        ControlType = item.ControlType.ToString(),
                                        ControlRenderId = item.ControlRenderId,
                                        IsCustom = item.IsCustom,
                                        IsDisable = item.IsDisable,
                                        IsHide = item.IsHide,
                                        ProcessFullName = item.ProcessFullName,
                                        RenderTemplateId = item.RenderTemplateId,
                                        WorkMode = (int)item.WorkMode
                                    });
                                });
                                kfwContext.ActivityControlSetting.AddRange(curacs);
                                kfwContext.SaveChanges();
                            }
                        }

                    }

                    //更新businessConfig
                    if (processset != null && !string.IsNullOrEmpty(dto.DbConnectionString) && !string.IsNullOrEmpty(dto.DataTable) && !string.IsNullOrEmpty(dto.WhereQuery))
                    {
                        var config = _businessdataBO.ReadConfig(processset.ProcessFullName);
                        if (config != null)
                        {
                            config.ApplicationName = dto.ProcessFullName;
                            config.ProcessName = dto.ProcessFullName;
                            config.DbConnectionString = dto.DbConnectionString;
                            config.DataTable = dto.DataTable;
                            config.WhereQuery = dto.WhereQuery;
                            _businessdataBO.UpdateConfig(config);
                        }
                        else
                        {
                            config = new BusinessDataConfigDTO()
                            {
                                ApplicationName = dto.ProcessFullName,
                                ProcessName = dto.ProcessFullName,
                                DbConnectionString = dto.DbConnectionString,
                                DataTable = dto.DataTable,
                                WhereQuery = dto.WhereQuery
                            };
                            _businessdataBO.CreateConfig(config);
                        }
                    }

                    //导入线规则
                    if (dto.lineRules != null)
                    {
                        foreach (var item in dto.lineRules)
                        {
                            var target = dbContext.Configuration_LineRule.Where(x => x.SysID == item.SysID).FirstOrDefault();
                            if (target != null)
                            {
                                target.RuleString = item.RuleString;
                                target.SourceActivityName = item.SourceActivityName;
                                target.TargetActivityName = item.TargetActivityName;
                                target.FullName = item.FullName;
                            }
                            else
                            {
                                dbContext.Configuration_LineRule.Add(item);
                            }
                        }
                    }
                     
                    dbContext.SaveChanges();
                    result = GetConfiguration_ProcessSetDTO(context, dbContext, null, processset, true);
                }
                //    ts.Complete();
                //}
            }
            return result;
        }

        /// <summary>
        /// 流程解除绑定
        /// </summary>
        /// <param name="processSetID"></param>
        /// <returns></returns>
        public bool ProcessUnbundling(int processSetID)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Configuration_ProcessSet set = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == processSetID);

                List<Configuration_ProcessVersion> versions = new List<Configuration_ProcessVersion>();
                versions.AddRange(set.ProcessVersionList);

                foreach (var version in versions)
                {
                    List<Configuration_Activity> activities = new List<Configuration_Activity>();
                    if (version.ActivityList != null && version.ActivityList.Count > 0)
                    {
                        activities.AddRange(version.ActivityList);
                    }
                    foreach (var activity in activities)
                    {
                        //移除可退回环节
                        List<Configuration_RefActivity> refactivities = dbContext.Configuration_RefActivitySet.Where(x => x.Configuration_ActivityID == activity.ID).ToList();
                        if (refactivities != null && refactivities.Count > 0) dbContext.Configuration_RefActivitySet.RemoveRange(refactivities);

                        //移除处理人
                        List<Configuration_User> users = dbContext.Configuration_UserSet.Where(x => x.RefID == activity.ID && x.RefType == Configuration_RefType.Activity.ToString()).ToList();
                        if (users != null && users.Count > 0) dbContext.Configuration_UserSet.RemoveRange(users);

                        //移除环节
                        dbContext.Configuration_ActivitySet.Remove(activity);
                    }
                    //移除版本
                    dbContext.Configuration_ProcessVersionSet.Remove(version);
                }
                //移除收藏流程
                Configuration_ProcCommon setcommon = dbContext.Configuration_ProcCommonSet.Where(x => x.ConfigProcSetID == processSetID).FirstOrDefault();
                if (setcommon != null) dbContext.Configuration_ProcCommonSet.Remove(setcommon);

                //移除处理人
                List<Configuration_User> operateusers = dbContext.Configuration_UserSet.Where(x => x.RefID == processSetID && x.RefType == Configuration_RefType.ProcessSet.ToString()).ToList();
                if (operateusers != null && operateusers.Count > 0) dbContext.Configuration_UserSet.RemoveRange(operateusers);

                //移除控件配置
                using (KStarFramekWorkDbContext kfwContext = new KStarFramekWorkDbContext())
                {
                    List<ActivityControlSetting> acs = kfwContext.ActivityControlSetting.Where(x => x.ProcessFullName == set.ProcessFullName).ToList();
                    kfwContext.ActivityControlSetting.RemoveRange(acs);
                    kfwContext.SaveChanges();
                }

                //解绑流程
                dbContext.Configuration_ProcessSetSet.Remove(set);
                dbContext.SaveChanges();
            }
            return true;
        }


        public bool IsNeedUpdateConfigurationVersion(string currentUser, int configuration_ProcessSetID)
        {
            bool flag = false;
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            Configuration_ProcessSet set = new Configuration_ProcessSet();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                set = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == configuration_ProcessSetID);

                var oldVersionList = set.ProcessVersionList;
                var criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
                var procVersionList = _managementFacade.GetProcessVersions(context, set.ProcessSetID, criteria);

                //版本条数不一致 需要更新
                if (oldVersionList.Count != procVersionList.Count)
                {
                    flag = true;
                }

                //版本的环节条数不一致 
                for (int i = 0; i < procVersionList.Count; i++)
                {
                    var version = procVersionList[i];
                    Configuration_ProcessVersion v = oldVersionList.Where(x => x.ProcessVersionID == version.ID).FirstOrDefault();
                    if (v != null)
                    {
                        var procactivity = this.GetProcessActivities(context, version.ID);
                        if (v.ActivityList.Count != procactivity.Count)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            return flag;
        }

        #endregion

        //#region CustomRole
        ///// <summary>
        ///// 获取配置节点下的已上传程序集信息
        ///// </summary>
        ///// <param name="categoryId">配置节点ID</param>
        ///// <returns></returns>
        //public IEnumerable<Configuration_CustomRole> GetAllAssembleList(int categoryId)
        //{
        //    using (KStarDbContext dbContext = new KStarDbContext())
        //    {
        //        var customRoleList = dbContext.Configuration_CustomRole.Where(r => r.Configuration_CategoryID == categoryId && r.ParentId == 0);

        //        return customRoleList;
        //    }
        //}

        ///// <summary>
        ///// 获取程序集下的角色信息
        ///// </summary>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //public IEnumerable<Configuration_CustomRole> GetCustomRoleById(int assembleId)
        //{
        //    using (KStarDbContext dbContext = new KStarDbContext())
        //    {
        //        var customRoleList = dbContext.Configuration_CustomRole.Where(r => r.ParentId == assembleId);

        //        return customRoleList;
        //    }
        //}

        ///// <summary>
        ///// 更新是否启用状态
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="enabledFlag"></param>
        ///// <returns></returns>
        //public bool UpdateCustomRoleEnabled(int id, string enabledFlag)
        //{
        //    using (KStarDbContext dbContext = new KStarDbContext())
        //    {
        //        var customRole = dbContext.Configuration_CustomRole.SingleOrDefault(r => r.ID == id);

        //        if (customRole == null)
        //        {
        //            return false;
        //        }

        //        customRole.EnabledFlag = enabledFlag;

        //        dbContext.SaveChanges();
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// 删除自定义程序集信息
        ///// </summary>
        ///// <param name="assembleId"></param>
        ///// <returns></returns>
        //public bool DeleteCustomRole(int assembleId)
        //{
        //    using (KStarDbContext dbContext = new KStarDbContext())
        //    {
        //        var customRoleList = dbContext.Configuration_CustomRole.Where(r => r.ID == assembleId || r.ParentId == assembleId);

        //        if (customRoleList == null)
        //        {
        //            return false;
        //        }

        //        foreach (var customRole in customRoleList)
        //        {
        //            dbContext.Configuration_CustomRole.Remove(customRole);
        //        }

        //        dbContext.SaveChanges();
        //    }

        //    return true;
        //}
        //#endregion

        #region Activity

        /// <summary>
        /// 获取流程版本下的所有节点
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="configuration_ProcessVersionID">Configuration_ProcessVersionDTO ID</param>
        /// <param name="procID">流程版本 ID</param>
        /// <param name="procSetID">流程 ID</param>
        /// <param name="include">是否包含子项</param>
        /// <returns></returns>
        public IEnumerable<Configuration_ActivityDTO> GetActivityListByProcessVersion(string currentUser, int configuration_ProcessVersionID, int procID = 0, int procSetID = 0, bool include = false)
        {
            List<Configuration_ActivityDTO> list = new List<Configuration_ActivityDTO>();
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Configuration_ProcessVersion version = null;
                if (configuration_ProcessVersionID > 0)
                {
                    version = dbContext.Configuration_ProcessVersionSet.Single(r => r.ID == configuration_ProcessVersionID);
                    procSetID = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == version.Configuration_ProcessSetID).ProcessSetID;
                    procID = version.ProcessVersionID;
                }
                //var procSetID = dbContext.Configuration_ProcessSetSet.Single(r => r.ID == version.Configuration_ProcessSetID).ProcessSetID;
                Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria() { PageSize = int.MaxValue };
                //criteria.AddRegularFilter(new Framework.Workflow.Pager.RegularFilter(Framework.Workflow.Pager.CriteriaLogical.None, "ID", Framework.Workflow.Pager.CriteriaCompare.Equal, version.ProcessVersionID));
                var procVersion = _managementFacade.GetProcessVersions(context, procSetID, criteria).SingleOrDefault(r => r.ID == procID);
                var dto = ConvertToProcVersionDTO(procVersion, version);
                SetConfiguration_ActivityDTO(context, dbContext, dto, include);
                list = dto.ActivityList;
            }
            return list;
        }

        /// <summary>
        /// 获取单个Configuration_ActivityDTO
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="configuration_ActivityID">Configuration_ActivityDTO ID</param>
        /// <returns></returns>
        public Configuration_ActivityDTO GetActivityByID(string currentUser, int configuration_ActivityID)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var actConfig = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ID == configuration_ActivityID);
                var version = dbContext.Configuration_ProcessVersionSet.Single(r => r.ID == actConfig.Configuration_ProcessVersionID);
                var activity = this.GetProcessActivities(context, version.ProcessVersionID).SingleOrDefault(r => r.ID == actConfig.ActivityID);
                return ConvertToActivityDTO(activity, actConfig);
            }
        }

        public Configuration_ActivityDTO GetUndoActivityByCurrentActivityId(string currentUser, int currentActivityId)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = currentUser;
            context.TenantID = _tenantID;
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var actConfig = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ActivityID == currentActivityId);
                var version = dbContext.Configuration_ProcessVersionSet.Single(r => r.ID == actConfig.Configuration_ProcessVersionID);
                var activities = this.GetProcessActivities(context, version.ProcessVersionID);
                var undoact = activities.Where(x => x.Name.StartsWith("015_")).FirstOrDefault();
                actConfig = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ActivityID == undoact.ID);
                return ConvertToActivityDTO(undoact, actConfig);
            }
        }

        /// <summary>
        /// 获取单个节点的可退回环节
        /// </summary>
        /// <param name="configuration_ActivityID">Configuration_ActivityDTO ID</param>
        /// <returns></returns>
        public IEnumerable<int> GetReworkActivityList(int configuration_ActivityID)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                return dbContext.Configuration_RefActivitySet.Where(r => r.Configuration_ActivityID == configuration_ActivityID).Select(r => r.ActivityID).ToList();
            }
        }

        /// <summary>
        /// 获取单个节点的可退回环节
        /// </summary>
        /// <param name="activityId">ActivityId</param>
        /// <returns></returns>
        public IList<Activity> GetReworkActivityListByActId(int activityId)
        {
            var activityIds = GetReworkActivityIdList(activityId);
            var list = new List<Activity>();
            var acts = new StringBuilder();
            activityIds.ToList().ForEach(item =>
            {
                acts.AppendFormat("{0},", item);
            });

            if (acts.Length == 0)
            {
                return list;
            }

            var sql = string.Format(SQLQueryBroker.GetQuery("SQL_ConfigManager_GetReworkActivityListByActId"), acts.ToString().Trim(','));
           
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var dr = SqlHelper.ExecuteReader(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);
                using (dr)
                {
                    while (dr.Read())
                    {
                        Activity act = new Activity();
                        act.ID = Convert.ToInt32(dr["ID"]);
                        act.Name = dr["Name"].ToString();
                        list.Add(act);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// 根据流程实例编号(ProcInstId)和流程节点(ActivityName)获取流程节点ID(ActivityID)
        /// </summary>
        /// <param name="activityId">ActivityId</param>
        /// <returns></returns>
        public Activity GetActivityInfo(int procInstId, string activityName)
        {
            string sql = string.Format(SQLQueryBroker.GetQuery("SQL_ConfigManager_GetActivityInfo"), procInstId, activityName);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Activity act = new Activity();
                var dr = SqlHelper.ExecuteReader(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);
                using (dr)
                {
                    while (dr.Read())
                    {
                        act.ID = Convert.ToInt32(dr["ActID"]);
                        act.Name = dr["ActName"].ToString();
                    }
                }

                return act;
            }
        }

        /// <summary>
        /// 根据流程实例编号(ProcInstId)和流程节点ID(ActivityID)获取流程节点(ActivityName)
        /// </summary>
        /// <param name="activityId">ActivityId</param>
        /// <returns></returns>
        public Activity GetActivityInfo(int procInstId, int activityId)
        {
            var condition = string.Empty;
            if (activityId == 0)
            {
                condition = string.Format("AND [Act].Name like '{0}%'", "010[_]");
            }
            else
            {
                condition = string.Format("AND [Act].ID = N'{0}'", activityId);
            }

            string sql = string.Format(@"SELECT 
                                        [Act].[ID] AS ActID,[Act].Name AS [ActName]
                                        FROM [K2].[Server].[Act] AS [Act] --节点表
                                        LEFT JOIN [K2].[ServerLog].[ProcInst] AS [ProcInst] --流程实例表
                                        ON [Act].ProcID = [ProcInst].ProcID
                                        Where [ProcInst].ID = {0} {1}
                                        ", procInstId, condition);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Activity act = new Activity();
                var dr = SqlHelper.ExecuteReader(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);
                using (dr)
                {
                    while (dr.Read())
                    {
                        act.ID = Convert.ToInt32(dr["ActID"]);
                        act.Name = dr["ActName"].ToString();
                    }
                }

                return act;
            }
        }

        /// <summary>
        /// 根据流程实例编号(ProcInstId)获取流程当前节点
        /// </summary>
        /// <param name="activityId">ActivityId</param>
        /// <returns></returns>
        public Activity GetCurrActivityInfo(int procInstId)
        {
            var sql = string.Format(SQLQueryBroker.GetQuery("SQL_ConfigManager_GetCurrActivityInfo"), procInstId);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Activity act = new Activity()
                {
                    ID = 0,
                    Name = ""
                };

                var dr = SqlHelper.ExecuteReader(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);
                using (dr)
                {
                    while (dr.Read())
                    {
                        act.ID = Convert.ToInt32(dr["ActID"]);
                        act.Name = dr["ActName"].ToString();
                    }
                }

                return act;
            }
        }

        /// <summary>
        /// 获取单个节点的可退回环节
        /// </summary>
        /// <param name="configuration_ActivityID">Configuration_ActivityDTO ID</param>
        /// <returns></returns>
        private IList<int> GetReworkActivityIdList(int activityId)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var reWorkActList = new List<int>();
                var actConfig = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ActivityID == activityId);

                if (actConfig != null)
                {
                    var refActConfig = GetReworkActivityList(actConfig.ID);

                    if (refActConfig != null)
                    {
                        refActConfig.ToList().ForEach(item =>
                        {
                            var reWorkActItem = dbContext.Configuration_ActivitySet.SingleOrDefault(r => r.ID == item);
                            reWorkActList.Add(reWorkActItem.ActivityID);
                        });
                    }
                }

                return reWorkActList;
            }
        }

        /// <summary>
        /// 获取单个节点的审批人配置
        /// </summary>
        /// <param name="configuration_ActivityID">Configuration_ActivityDTO ID</param>
        /// <returns></returns>
        public IEnumerable<Configuration_UserDTO> GetOperateUserList(int configuration_ActivityID)
        {
            List<Configuration_UserDTO> list = new List<Configuration_UserDTO>();
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                GetConfiguration_OperateUserList(dbContext, configuration_ActivityID).ForEach(r => list.Add(ConvertToUserDTO(r)));
            }
            return list;
        }

        /// <summary>
        /// 修改流程节点的可退回环节配置
        /// </summary>
        /// <param name="configurationActivityId">流程配置的节点ID</param>
        /// <param name="reworkActivityList">流程配置的节点的可退回环节列表</param>
        /// <returns></returns>
        public bool UpdateReworkActivityList(int configurationActivityId, List<int> reworkActivityList)
        {
            if (configurationActivityId <= 0)
                return false;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                UpdateReworkActivityList(configurationActivityId, reworkActivityList, dbContext);
                dbContext.SaveChanges();
            }
            return true;
        }

        private void UpdateReworkActivityList(int configurationActivityId, List<int> reworkActivityList, KStarDbContext dbContext)
        {
            if (configurationActivityId <= 0)
                return;

            var refList = dbContext.Configuration_RefActivitySet.Where(r => r.Configuration_ActivityID == configurationActivityId);
            var idList = refList.Select(r => r.ActivityID);

            foreach (var item in reworkActivityList)
            {
                if (!idList.Contains(item))
                {
                    dbContext.Configuration_RefActivitySet.Add(new Configuration_RefActivity() { ActivityID = item, Configuration_ActivityID = configurationActivityId });
                }
            }
            foreach (var org in refList)
            {
                if (!reworkActivityList.Contains(org.ActivityID))
                {
                    dbContext.Configuration_RefActivitySet.Remove(org);
                }
            }
        }

        /// <summary>
        /// 修改流程节点的审批人配置
        /// </summary>
        /// <param name="configurationActivityId">流程配置的节点ID</param>
        /// <param name="operateUserList">流程配置的审批人列表</param>
        /// <returns></returns>
        public bool UpdateOperateUserList(int configurationActivityId, IEnumerable<Configuration_UserDTO> operateUserList)
        {
            if (configurationActivityId <= 0)
                return false;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                UpdateOperateUserList(configurationActivityId, operateUserList, dbContext);

                dbContext.SaveChanges();
            }
            return true;
        }

        private void UpdateOperateUserList(int configurationActivityId, IEnumerable<Configuration_UserDTO> operateUserList, KStarDbContext dbContext)
        {
            if (configurationActivityId <= 0)
                return;

            var userList = GetConfiguration_OperateUserList(dbContext, configurationActivityId);
            foreach (var item in operateUserList)
            {
                Configuration_User config = userList.SingleOrDefault(r => r.ID == item.ID);
                bool isNew = config == null;
                item.OperateType = Configuration_OperationType.OperateProcess;
                item.RefType = Configuration_RefType.Activity;
                item.RefID = configurationActivityId;
                ConvertSetUser(item, ref config);

                if (isNew)
                {
                    dbContext.Configuration_UserSet.Add(config);
                }
            }
            //删除已经不存在的
            foreach (var item in userList)
            {
                var org = operateUserList.SingleOrDefault(r => r.ID == item.ID);
                if (org == null)
                {
                    var ditem = dbContext.Configuration_UserSet.FirstOrDefault(r => r.ID == item.ID);
                    dbContext.Configuration_UserSet.Remove(ditem);
                }
            }
        }

        public bool UpdateActivityBasicInfo(int configurationActivityId, int processTime)
        {
            if (configurationActivityId <= 0)
                return false;

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                Configuration_Activity activity = dbContext.Configuration_ActivitySet.Where(x => x.ID == configurationActivityId).FirstOrDefault();
                activity.ProcessTime = processTime;
                dbContext.SaveChanges();
            }
            return true;
        }

        /// <summary>
        ///  获取对应act 对应lien 下的actname
        /// </summary>
        /// <param name="actID"></param>
        /// <param name="procID">版本id</param>
        /// <returns></returns>
        public IList<Configuration_ActDesc> GetLinkActivityNames(int procID, int actID, string fullName, string actName)
        { 
            string sql = aZaaS.Framework.SQLQuery.SQLQueryBroker.GetQuery("SQL_ConfigManager_GetLinkActivityNames");
            string queueLable =System.Configuration.ConfigurationManager.AppSettings["QueueLable"]??"加签"; 
            sql = string.Format(sql, procID, actID, queueLable);
            IList<Configuration_ActDesc> actDescs = null;

            try
            { 
                using (K2DBContext k2DB = new K2DBContext())
                {

                    actDescs = k2DB.Database.SqlQuery<Configuration_ActDesc>(sql).ToList();

                    if (actDescs != null)
                    {
                        foreach (var act in actDescs)
                        {
                            act.SourceName = actName;
                        }
                    }
                } 
            }
            catch (Exception ex)
            {

            }

            return actDescs;
        }

        public IList<Configuration_LineRule> GetLinkActivityNameRule(int procSetID, int actID, string fullName, string actName)
        {
            string sql = aZaaS.Framework.SQLQuery.SQLQueryBroker.GetQuery("SQL_ConfigManager_GetLinkActivityNameRule");
            string queueLable = System.Configuration.ConfigurationManager.AppSettings["QueueLable"] ?? "加签";
            sql = string.Format(sql, procSetID, actID, queueLable);
            IList<Configuration_LineRule> lineRules = new List<Configuration_LineRule>();
            List<string> nameList = null;
            try
            {

                using (K2DBContext k2DB = new K2DBContext())
                {

                    nameList = k2DB.Database.SqlQuery<string>(sql).ToList();
                }

                using (KStarDbContext kStarDb = new KStarDbContext())
                {


                    foreach (string str in nameList)
                    {
                        var entity = kStarDb.Configuration_LineRule.Where(x => x.FullName == fullName && x.SourceActivityName == actName && x.TargetActivityName == str).FirstOrDefault();

                        if (entity == null)
                        {
                            entity = new Configuration_LineRule();
                            entity.SysID = Guid.NewGuid();
                            entity.SourceActivityName = actName;
                            entity.TargetActivityName = str;
                            entity.RuleString = string.Empty;
                            entity.FullName = fullName;

                        }
                        lineRules.Add(entity);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return lineRules;
        }


        /// <summary>
        /// 获取流程实例对应的配置信息
        /// </summary>
        /// <param name="procInstID"></param>
        public IList<Configuration_ProcInstDesc> GetProcInstStateDesc(int procInstID)
        {
            string sql = aZaaS.Framework.SQLQuery.SQLQueryBroker.GetQuery("SQL_ConfigManager_GetProcInstStateDesc");
            sql = string.Format(sql, procInstID);
            IList<Configuration_ProcInstDesc> descList = new  List<Configuration_ProcInstDesc>();
            using (KStarDbContext kStarDb = new KStarDbContext())
            {

                descList = kStarDb.Database.SqlQuery<Configuration_ProcInstDesc>(sql).ToList();
            }
            return descList;
        }

        /// <summary>
        /// 获取线对应的规则
        /// </summary>
        /// <param name="procInstID"></param>
        /// <param name="lienInstID"></param>
        /// <param name="fillName"></param>
        /// <param name="actName"></param>
        /// <returns></returns>
        public string GetLienRule(int procInstID, string lienName, string fillName, string actName)
        {
            string sql = aZaaS.Framework.SQLQuery.SQLQueryBroker.GetQuery("SQL_ConfigManager_GetLineRule");
            sql = string.Format(sql, procInstID, lienName);

            string name = "";
            using (K2DBContext k2DB = new K2DBContext())
            {

                name = k2DB.Database.SqlQuery<string>(sql).FirstOrDefault();
            }

            using (KStarDbContext kStarDb = new KStarDbContext())
            {

                var ruleEntity = kStarDb.Configuration_LineRule.Where(x => x.FullName == fillName && x.SourceActivityName == actName && x.TargetActivityName == name).FirstOrDefault();

                if (ruleEntity != null)
                    return ruleEntity.RuleString;
            }

            return null;
        }



        public bool ExecuteLienRule(int procInstId, string actName, string fullName, string action, string ruleString)
        {
            try
            {
                RuleContext context = new RuleContext(procInstId);
                context.ProcessFullName = fullName;
                context.Outcome = action;
                context.ActName = actName;

                CustomContext custom = new CustomContext();

                var reg = new TypeRegistry();
                reg.RegisterSymbol("@contex", context);//系统扩展
                reg.RegisterSymbol("@custom", custom);//自定义扩展
                var expression = new CompiledExpression(ruleString);
                expression.TypeRegistry = reg;

                var result = (bool)expression.Eval();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Participants

        /// <summary>
        /// Resolves the general configured type of the given activity and return the configured users.
        /// NOTE:The general type includes User,Position,OrgNode.
        /// IMPORTANT:In this case,we would not resolve the [CustomType].so you should remember to call CustomRole service to resolve that type after and merger the users. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetActivityConfiguredGeneralParticipants(int procId, string activityName)
        {
            IEnumerable<string> userSet = new HashSet<string>(); 
            List<Configuration_User> cfgSets = GetActivityConfigParticipants(procId, activityName);

            foreach (var item in cfgSets)
            {
                var keyId = Guid.Parse(item.Key);

                userSet = userSet.Union(ResolveParticipants(item.UserType, keyId));
            }

            return userSet;
        }

        public List<Configuration_User> GetActivityConfigParticipants(int procId, string activityName)
        {
            var activityId = this.GetActivityIdFromServer(procId, activityName);
            using (KStarDbContext db = new KStarDbContext())
            {
                var actCfg = db.Configuration_ActivitySet.FirstOrDefault(cfg => cfg.ActivityID == activityId);
                if (actCfg == null)
                    throw new InvalidOperationException(string.Format("The acitivty: {0}'s config was not found!", activityId));

                var cfgSets = db.Configuration_UserSet.Where(cfg => cfg.RefType.Equals("Activity") && cfg.RefID == actCfg.ID);
                return cfgSets.ToList();
            } 
        }
             
        /// <summary>
        /// Resolve handle man
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public IEnumerable<string> ResolveParticipants(string userType, Guid keyId)
        {
            var userSet = new HashSet<string>();
            switch (userType)
            {
                case "User":
                    userSet.Add(ResolveUserName(keyId));
                    break;
                case "Position":
                    this.ResolvePositionUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                    break;
                case "OrgNode":
                    this.ResolveOrgNodeUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                    break;
                case "Role":
                    this.ResolveRoleUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                    break;
            }
            return userSet;
        }
         
        /// <summary>
        /// Gets the configured custom role keys of the given activity.
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetActivityConfiguredCustomRoles(int procId, string activityName)
        {
            var keySet = new HashSet<Guid>();

            var activityId = this.GetActivityIdFromServer(procId, activityName);

            using (KStarDbContext db = new KStarDbContext())
            {
                var actCfg = db.Configuration_ActivitySet.FirstOrDefault(cfg => cfg.ActivityID == activityId);
                if (actCfg == null)
                    throw new InvalidOperationException(string.Format("The acitivty: {0}'s config was not found!", activityId));

                var cfgSets = db.Configuration_UserSet.Where(cfg => cfg.RefType.Equals("Activity") && cfg.RefID == actCfg.ID
                                                             && cfg.UserType.Equals("CustomType"));
                cfgSets.ToList().ForEach(item =>
                {
                    var key = Guid.Parse(item.Key);
                    keySet.Add(key);
                });
            }

            return keySet;
        }

        /// <summary>
        /// Resolves the general configured type of the given activity and return the configured users.
        /// NOTE:The general type includes User,Position,OrgNode.
        /// IMPORTANT:In this case,we would not resolve the [CustomType].so you should remember to call CustomRole service to resolve that type after and merger the users. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetActivityConfiguredGeneralParticipants(int procId, int procInstId, string activityName, string refType, string operateType)
        {
            var userSet = new HashSet<string>();

            var activityId = this.GetActivityIdFromServer(procId, activityName);

            using (KStarDbContext db = new KStarDbContext())
            {
                var actCfg = db.Configuration_ActivitySet.FirstOrDefault(cfg => cfg.ActivityID == activityId);
                if (actCfg == null)
                    throw new InvalidOperationException(string.Format("The acitivty: {0}'s config was not found!", activityId));

                var cfgProcessVersion = db.Configuration_ProcessVersionSet.FirstOrDefault(r => r.ID == actCfg.Configuration_ProcessVersionID);

                var cfgSets = db.Configuration_UserSet.Where(cfg => cfg.RefType.Equals(refType) && cfg.OperateType.Equals(operateType) && cfg.RefID == cfgProcessVersion.Configuration_ProcessSetID);
                cfgSets.ToList().ForEach(item =>
                {
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        switch (item.Value)
                        {
                            case "流程发起人":
                                var startUser = ResolveStartupUser(procInstId);
                                if (!string.IsNullOrEmpty(startUser))
                                {
                                    userSet.Add(startUser);
                                }
                                break;
                            case "流程参与人":
                                this.ResolveParticipation(procInstId).ToList().ForEach(user => userSet.Add(user));
                                break;
                        }
                    }
                    else
                    {
                        var keyId = Guid.Parse(item.Key);

                        switch (item.UserType)
                        {
                            case "User":
                                userSet.Add(ResolveUserName(keyId));
                                break;
                            case "Position":
                                this.ResolvePositionUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                                break;
                            case "OrgNode":
                                this.ResolveOrgNodeUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                                break;
                            case "Role":
                                this.ResolveRoleUsers(keyId).ToList().ForEach(user => userSet.Add(user));
                                break;
                        }
                    }
                });
            }

            return userSet;
        }

        /// <summary>
        /// Gets the configured custom role keys of the given activity.
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetActivityConfiguredCustomRoles(int procId, string activityName, string refType, string operateType)
        {
            var keySet = new HashSet<Guid>();

            var activityId = this.GetActivityIdFromServer(procId, activityName);

            using (KStarDbContext db = new KStarDbContext())
            {
                var actCfg = db.Configuration_ActivitySet.FirstOrDefault(cfg => cfg.ActivityID == activityId);
                if (actCfg == null)
                    throw new InvalidOperationException(string.Format("The acitivty: {0}'s config was not found!", activityId));

                var cfgProcessVersion = db.Configuration_ProcessVersionSet.FirstOrDefault(r => r.ID == actCfg.Configuration_ProcessVersionID);

                var cfgSets = db.Configuration_UserSet.Where(cfg => cfg.RefType.Equals(refType) && cfg.RefID == cfgProcessVersion.Configuration_ProcessSetID
                                                             && cfg.UserType.Equals("CustomType"));
                cfgSets.ToList().ForEach(item =>
                {
                    var key = Guid.Parse(item.Key);
                    keySet.Add(key);
                });
            }

            return keySet;
        }

        public ProcessFormHeader GetProcesFormHeader(int procId)
        {
            using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
            {
                var item = dbContext.ProcessFormHeader.FirstOrDefault(r => r.ProcInstID == procId);

                return item;
            }
        }

        /// <summary>
        /// Resolve the users of the given position.
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        private HashSet<string> ResolvePositionUsers(Guid positionId)
        {
            HashSet<string> userSet = new HashSet<string>();

            var users = _positionService.GetPositionUsersBase(positionId);
            if (users != null)
                users.ToList().ForEach(u => userSet.Add(u.UserName));

            return userSet;
        }

        /// <summary>
        /// Resolve the users of the given org node.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private HashSet<string> ResolveOrgNodeUsers(Guid nodeId)
        {
            HashSet<string> userSet = new HashSet<string>();

            var node = _chartService.ReadNodeWithUsers(nodeId);
            if (node != null)
                node.Users.ToList().ForEach(u => userSet.Add(u.UserName));

            return userSet;
        }

        private string ResolveUserName(Guid userId)
        {
            var user = _userService.ReadUserBase(userId);

            return user == null ? string.Empty : user.UserName;
        }

        /// <summary>
        /// Resolve the users of the given org node.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private HashSet<string> ResolveRoleUsers(Guid roleId)
        {
            HashSet<string> userSet = new HashSet<string>();

            var users = _roleService.GetRoleUsers(roleId);
            if (users != null)
                users.ToList().ForEach(u => userSet.Add(u.UserName));

            return userSet;
        }

        private string ResolveStartupUser(int procInstId)
        {
            var logService = new ProcessLogService();
            var logs = logService.GetProcessLogByProcInstID(procInstId);

            if (logs != null && logs.Count() > 0)
            {
                logs = logs.OrderBy(r => r.CommentDate).ToList();
                return logs[0].OrigUserAccount;
            }

            return "";
        }

        private HashSet<string> ResolveParticipation(int procInstId)
        {
            var logService = new ProcessLogService();
            var logs = logService.GetProcessLogByProcInstID(procInstId);

            var users = new HashSet<string>();

            logs.ForEach(log => users.Add(log.OrigUserAccount));

            return users;
        }

        #endregion

        #region GetMyParticipatedProcessInstances

        /// <summary>
        /// 获取我参与的流程实例列表(aZaaS)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [Obsolete("该方法已被其它方法代替，参考WorkflowClientService的扩展方法")]
        public List<ProcessInstance> GetMyParticipatedProcessInstancesByAction(ServiceContext context, ref ProcessInstanceCriteria criteria)
        {
//            var logCondition = " ";
//            var logService = new ProcessLogService();
//            var logs = logService.GetProcessLogByUserAccount(context["CurrentUser"]);
//            var myParticipatedInstances = new List<ProcessLog>();
//            context["ActionName"].Split(',').Each(actName =>
//            {
//                myParticipatedInstances.AddRange(logs.Where(r => r.ActionName == actName).ToList());
//            });

//            if (myParticipatedInstances != null && myParticipatedInstances.Count() > 0)
//            {
//                StringBuilder sbInst = new StringBuilder();
//                myParticipatedInstances.ForEach(log =>
//                {
//                    sbInst.Append(log.ProcInstID);
//                    sbInst.Append(",");
//                });
//                logCondition = string.Format(" or pit.id in ( {0} )", sbInst.ToString().Trim(','));
//            }

//            List<ProcessInstance> items = new List<ProcessInstance>();
//            string sql = @"select * from 
//                            (select row_number()over(order by {5})rownumber,
//                                 pit.ID as Procinstid ,
//                                ( SELECT  TOP 1 Act.Name as ActName 
//                                  FROM [K2].[ServerLog].[ActInst] AS ActInst
//                                  INNER JOIN [K2].[ServerLog].[Act] As Act
//                                  ON ActInst.ActID = Act.ID
//                                  WHERE   [ProcInstID] =pit.ID
//                                  ORDER BY [StartDate] DESC ) AS [ActivityName], --ActivityName
//                                pit.[Priority], pit.[Status], pit.StartDate ,pit.FinishDate, pit.Originator , pit.Folio, ps.FullName,cps.ProcessName, cps.ViewUrl
//
//                                from [K2].[ServerLog].[procinst] pit
//                                inner join [K2].[ServerLog].[Proc] pc
//                                     on pit.ProcID = pc.id
//                                inner join [K2].[ServerLog].[ProcSet] ps
//                                     on pc.ProcSetID = ps.ID
//                                inner join dbo.Configuration_ProcessSet cps
//                                     on ps.ID = cps.ProcessSetID
//                                     where (pit.id in ( 
//	                                    select procinstid from [K2].[ServerLog] .[ActInstSlot]
//	                                    where [user] = '{0}:{1}' ) {2} ) 
//                                    {3}
//                              )a
//		                    {4}    
//                        ;select COUNT(1) Total 
//                                from [K2].[ServerLog].[procinst] pit
//                                inner join [K2].[ServerLog].[Proc] pc
//                                     on pit.ProcID = pc.id
//                                inner join [K2].[ServerLog].[ProcSet] ps
//                                     on pc.ProcSetID = ps.ID
//                                inner join dbo.Configuration_ProcessSet cps
//                                     on ps.ID = cps.ProcessSetID
//                                     where (pit.id in ( 
//	                                    select procinstid from [K2].[ServerLog] .[ActInstSlot]
//	                                    where [user] = '{0}:{1}' ) {2} ) 
//                                    {3}   
//                   ";
//            try
//            {
//                string sql_condition = " ", sql_pager = " ", sql_order = "";
//                foreach (RegularFilter filter in criteria.RegularFilters)
//                {
//                    switch (filter.Compare)
//                    {
//                        case CriteriaCompare.Greater:
//                            sql_condition += " and " + filter.FieldName + " >'" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.GreaterOrEqual:
//                            sql_condition += " and " + filter.FieldName + " >='" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.Less:
//                            sql_condition += " and " + filter.FieldName + " <'" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.LessOrEqual:
//                            sql_condition += " and " + filter.FieldName + " <='" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.Like:
//                            sql_condition += " and " + filter.FieldName + " like '%" + filter.Value1 + "%'";
//                            break;
//                        case CriteriaCompare.Equal:
//                            sql_condition += " and " + filter.FieldName + " ='" + filter.Value1 + "'";
//                            break;
//                    }
//                }
//                for (int i = 0; i < criteria.SortFilters.Count; i++)
//                {
//                    SortFilter filter = criteria.SortFilters[i];
//                    sql_order += "," + filter.FieldName + " " + filter.SortDirection;
//                }

//                if (string.IsNullOrEmpty(sql_order))
//                {
//                    sql_order = ",pit.ID ";
//                }

//                if (criteria.PageSize > 0)
//                {
//                    sql_pager = "where rownumber>" + (criteria.PageIndex * criteria.PageSize).ToString() + " and rownumber<=" + ((criteria.PageIndex + 1) * criteria.PageSize).ToString();
//                }
//                sql = string.Format(sql, context["CurrentWorkflowSecurityLabel"], context["CurrentUser"], logCondition, sql_condition, sql_pager, sql_order.Substring(1));

//                using (SqlDataReader reader = SqlHelper.ExecuteReader(context[SettingVariable.ConnectionString], CommandType.Text, sql))
//                {
//                    while (reader.Read())
//                    {
//                        items.Add(ConvertToProcessInstance(reader));
//                    }
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        criteria.TotalCount = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);
//                    }
//                }
//                return items;
//            }
//            catch (Exception)
//            {
//                throw;
//            }

            throw new NotImplementedException();
        }

        [Obsolete("该方法已被其它方法代替，参考WorkflowClientService的扩展方法")]
        public List<ProcessInstance> GetMyStartedProcessInstances(ServiceContext context, ref ProcessInstanceCriteria criteria)
        {
//            List<ProcessInstance> items = new List<ProcessInstance>();
//            string sql = @"select * from 
//                            (select row_number()over(order by {4})rownumber,
//                                 pit.ID as Procinstid ,pfh.SubmitterAccount,pfh.SubmitterDisplayName,
//                                ( SELECT  TOP 1 Act.Name as ActName 
//                                  FROM [K2].[ServerLog].[ActInst] AS ActInst
//                                  INNER JOIN [K2].[ServerLog].[Act] As Act
//                                  ON ActInst.ActID = Act.ID
//                                  WHERE   [ProcInstID] =pit.ID
//                                  ORDER BY [StartDate] DESC ) AS [ActivityName], --ActivityName
//                                pit.[Priority], pit.[Status], pit.StartDate ,pit.FinishDate, pit.Originator , pit.Folio, ps.FullName,cps.ProcessName, cps.ViewUrl
//                                from [aZaaS.Framework].[dbo].[ProcessFormHeader] pfh
//								inner join [K2].[ServerLog].[procinst] pit
//								     on '{0}:'+pfh.SubmitterAccount COLLATE Chinese_PRC_CI_AS = pit.Originator
//									 and pfh.ProcInstId=pit.ID
//                                     and pfh.IsDraft=0
//                                inner join [K2].[ServerLog].[Proc] pc
//                                     on pit.ProcID = pc.id
//                                inner join [K2].[ServerLog].[ProcSet] ps
//                                     on pc.ProcSetID = ps.ID
//                                inner join dbo.Configuration_ProcessSet cps
//                                     on ps.ID = cps.ProcessSetID
//                                    where pfh.ApplicantAccount = '{1}' 
//									and pfh.SubmitterAccount!='{1}'
//                                    {2}    
//                                )a
//		                    {3}    
//                        ;select COUNT(1) Total 
//                                from [aZaaS.Framework].[dbo].[ProcessFormHeader] pfh
//								inner join [K2].[ServerLog].[procinst] pit
//								on '{0}:'+pfh.SubmitterAccount COLLATE Chinese_PRC_CI_AS =pit.Originator
//									 and pfh.ProcInstId=pit.ID
//                                     and pfh.IsDraft=0
//                                inner join [K2].[ServerLog].[Proc] pc
//                                     on pit.ProcID = pc.id
//                                inner join [K2].[ServerLog].[ProcSet] ps
//                                     on pc.ProcSetID = ps.ID
//                                inner join dbo.Configuration_ProcessSet cps
//                                     on ps.ID = cps.ProcessSetID
//                                    where pfh.ApplicantAccount = '{1}' 
//									and pfh.SubmitterAccount!='{1}'
//                                    {2}          
//                   ";

//            try
//            {
//                string sql_condition = " ", sql_pager = " ", sql_order = string.Empty;
//                foreach (RegularFilter filter in criteria.RegularFilters)
//                {
//                    switch (filter.Compare)
//                    {
//                        case CriteriaCompare.Greater:
//                            sql_condition += " and " + filter.FieldName + " >'" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.GreaterOrEqual:
//                            sql_condition += " and " + filter.FieldName + " >='" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.Less:
//                            sql_condition += " and " + filter.FieldName + " <'" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.LessOrEqual:
//                            sql_condition += " and " + filter.FieldName + " <='" + filter.Value1 + "'";
//                            break;
//                        case CriteriaCompare.Like:
//                            sql_condition += " and " + filter.FieldName + " like '%" + filter.Value1 + "%'";
//                            break;
//                        case CriteriaCompare.Equal:
//                            sql_condition += " and " + filter.FieldName + " ='" + filter.Value1 + "'";
//                            break;
//                    }
//                }
//                for (int i = 0; i < criteria.SortFilters.Count; i++)
//                {
//                    SortFilter filter = criteria.SortFilters[i];
//                    sql_order += "," + filter.FieldName + " " + filter.SortDirection;
//                }
//                if (string.IsNullOrEmpty(sql_order))
//                {
//                    sql_order = ",pit.ID ";
//                }

//                if (criteria.PageSize > 0)
//                {
//                    sql_pager = "where rownumber>" + (criteria.PageIndex * criteria.PageSize).ToString() + " and rownumber<=" + ((criteria.PageIndex + 1) * criteria.PageSize).ToString();
//                }

//                sql = string.Format(sql, context["CurrentWorkflowSecurityLabel"], context["CurrentUser"], sql_condition, sql_pager, sql_order.Substring(1));

//                using (SqlDataReader reader = SqlHelper.ExecuteReader(context[SettingVariable.ConnectionString], CommandType.Text, sql))
//                {
//                    while (reader.Read())
//                    {
//                        items.Add(ConvertToProcessInstance(reader));
//                    }
//                    reader.NextResult();
//                    while (reader.Read())
//                    {
//                        criteria.TotalCount = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);
//                    }
//                }
//                return items;
//            }
//            catch (Exception)
//            {
//                throw;
//            }

            throw new NotImplementedException();
        }


        private static ProcessInstance ConvertToProcessInstance(SqlDataReader reader)
        {
            ProcessInstance obj = new ProcessInstance();
            obj.ID = (reader["Procinstid"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Procinstid"]);
            obj.Priority = (reader["Priority"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Priority"]);
            obj.Status = (reader["Status"] == DBNull.Value) ? (ProcInstStatus)0 : (ProcInstStatus)Convert.ToInt32(reader["Status"]);
            obj.StartDate = (reader["StartDate"] == DBNull.Value) ? DateTime.MaxValue : Convert.ToDateTime(reader["StartDate"]);
            obj.FinishDate = (reader["FinishDate"] == DBNull.Value) ? DateTime.MaxValue : Convert.ToDateTime(reader["FinishDate"]);
            obj.Originator = (reader["Originator"] == DBNull.Value) ? "" : reader["Originator"].ToString();
            obj.ActivityName = (reader["ActivityName"] == DBNull.Value) ? "" : reader["ActivityName"].ToString();
            obj.Folio = (reader["Folio"] == DBNull.Value) ? "" : reader["Folio"].ToString();
            obj.FullName = (reader["FullName"] == DBNull.Value) ? "" : reader["FullName"].ToString();
            obj.ProcessName = (reader["ProcessName"] == DBNull.Value) ? "" : reader["ProcessName"].ToString();
            obj.ViewUrl = (reader["ViewUrl"] == DBNull.Value) ? "" : reader["ViewUrl"].ToString();
            obj.BOID = (reader.GetValue(2) == DBNull.Value) ? "" : reader.GetValue(2).ToString();//在我的申请中为提单人账号
            obj.BOOwner = (reader.GetValue(3) == DBNull.Value) ? "" : reader.GetValue(3).ToString();//在我的申请中为提单人名称
            return obj;
        }

        #endregion

        #region private methods

        private Configuration_ProcessSetDTO GetConfiguration_ProcessSetDTO(ServiceContext context, KStarDbContext dbContext, ProcessSet procSet, bool include)
        {
            try
            {
                var set = dbContext.Configuration_ProcessSetSet.SingleOrDefault(r => r.ProcessSetID == procSet.ID && r.ProcessFullName == procSet.FullName);

                return GetConfiguration_ProcessSetDTO(context, dbContext, procSet, set, include);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }
            return null; 
        }

        private Configuration_ProcessSetDTO GetConfiguration_ProcessSetDTO(ServiceContext context, KStarDbContext dbContext, ProcessSet procSet, Configuration_ProcessSet configProcSet, bool include)
        {
            Configuration_ProcessSetDTO setDTO = ConvertToProcSetDTO(procSet, configProcSet);

            SetEndCc(dbContext, setDTO);
            SetReworkCc(dbContext, setDTO);
            SetStartUserList(dbContext, setDTO);

            if (include)
            {
                SetConfiguration_ProcessVersionDTO(context, dbContext, setDTO, true);
            }

            return setDTO;
        }

        private void SetConfiguration_ProcessVersionDTO(ServiceContext context, KStarDbContext dbContext, Configuration_ProcessSetDTO setDTO, bool include)
        {
            setDTO.ProcessVersionList = new List<Configuration_ProcessVersionDTO>();

            Framework.Workflow.Pager.PageCriteria criteria = new Framework.Workflow.Pager.PageCriteria();
            criteria.PageSize = int.MaxValue;
            var versionList = _managementFacade.GetProcessVersions(context, setDTO.ProcessSetID, criteria);
            var configList = new List<Configuration_ProcessVersion>();
            if (setDTO.ID > 0)
            {
                using (KStarDbContext newContext = new KStarDbContext())
                {
                    configList.AddRange(newContext.Configuration_ProcessVersionSet.Where(r => r.Configuration_ProcessSetID == setDTO.ID));
                }
            }
            foreach (var item in versionList)
            {
                var config = configList.SingleOrDefault(r => r.ProcessVersionID == item.ID);
                var versionDTO = ConvertToProcVersionDTO(item, config);
                setDTO.ProcessVersionList.Add(versionDTO);
                if (include)
                {
                    SetConfiguration_ActivityDTO(context, dbContext, versionDTO, true);
                }
            }
        }

        private List<Activity> GetProcessActivities(ServiceContext context, int procID)
        {
            var workFlowEngine = System.Configuration.ConfigurationManager.AppSettings["WorkFlowEngine"];
            List<Activity> list = new List<Activity>();
            if (workFlowEngine != null && workFlowEngine.ToString() == "aZaaS")
            {
                list = _managementFacade.GetProcessActivities(context, procID);
            }
            else
            {
                //string sql = "select [ID],[Name] from [K2].[ServerLog].[Act] where [ProcID]=" + procID.ToString();

                var sql = string.Format(SQLQueryBroker.GetQuery("SQL_ConfigManager_GetProcessActivities"), procID.ToString());

                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var dr = SqlHelper.ExecuteReader(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);
                    using (dr)
                    {
                        while (dr.Read())
                        {
                            Activity act = new Activity();
                            act.ID = Convert.ToInt32(dr["ID"]);
                            act.Name = dr["Name"].ToString();
                            list.Add(act);
                        }
                    }
                }
            }
            return list;
        }

        public int GetActivityIdFromServer(int procId, string activityName)
        {
            var sql = string.Format(SQLQueryBroker.GetQuery("SQL_ConfigManager_GetActivityIdFromServer"), procId, activityName);

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var result = SqlHelper.ExecuteScalar(dbContext.Database.Connection.ConnectionString, System.Data.CommandType.Text, sql);

                return result == null ? 0 : Convert.ToInt32(result);
            }
        }

        private void SetConfiguration_ActivityDTO(ServiceContext context, KStarDbContext dbContext, Configuration_ProcessVersionDTO versionDTO, bool include)
        {
            versionDTO.ActivityList = new List<Configuration_ActivityDTO>();

            var activityList = this.GetProcessActivities(context, versionDTO.ProcessVersionID);
            var configList = new List<Configuration_Activity>();
            if (versionDTO.ID > 0)
            {
                using (KStarDbContext newContext = new KStarDbContext())
                {
                    configList.AddRange(newContext.Configuration_ActivitySet.Where(r => r.Configuration_ProcessVersionID == versionDTO.ID));
                }
            }
            foreach (var item in activityList)
            {
                var config = configList.SingleOrDefault(r => r.ActivityID == item.ID);
                var activityDTO = ConvertToActivityDTO(item, config);

                if (include)
                {
                    SetReworkActivityList(dbContext, activityDTO);
                    SetOperateUserList(dbContext, activityDTO);
                }

                versionDTO.ActivityList.Add(activityDTO);
            }
        }

        private void SetReworkActivityList(KStarDbContext dbContext, Configuration_ActivityDTO activityDTO)
        {
            activityDTO.ReworkActivityList = new List<int>();
            if (activityDTO.ID > 0)
            {
                using (KStarDbContext newContext = new KStarDbContext())
                {
                    activityDTO.ReworkActivityList.AddRange(newContext.Configuration_RefActivitySet.Where(r => r.Configuration_ActivityID == activityDTO.ID).Select(r => r.ActivityID));
                }
            }
        }

        private void SetOperateUserList(KStarDbContext dbContext, Configuration_ActivityDTO activityDTO)
        {
            activityDTO.OperateUserList = new List<Configuration_UserDTO>();
            if (activityDTO.ID > 0)
            {
                GetConfiguration_OperateUserList(dbContext, activityDTO.ID).ForEach(r => activityDTO.OperateUserList.Add(ConvertToUserDTO(r)));
            }
        }

        private void SetEndCc(KStarDbContext dbContext, Configuration_ProcessSetDTO setDTO)
        {
            setDTO.EndCc = new List<Configuration_UserDTO>();
            if (setDTO.ID > 0)
            {
                GetConfiguration_EndCc(dbContext, setDTO.ID).ForEach(r => setDTO.EndCc.Add(ConvertToUserDTO(r)));
            }
        }
        private void SetReworkCc(KStarDbContext dbContext, Configuration_ProcessSetDTO setDTO)
        {
            setDTO.ReworkCc = new List<Configuration_UserDTO>();
            if (setDTO.ID > 0)
            {
                GetConfiguration_ReworkCc(dbContext, setDTO.ID).ForEach(r => setDTO.ReworkCc.Add(ConvertToUserDTO(r)));
            }
        }
        private void SetStartUserList(KStarDbContext dbContext, Configuration_ProcessSetDTO setDTO)
        {
            setDTO.StartUserList = new List<Configuration_UserDTO>();
            if (setDTO.ID > 0)
            {
                GetConfiguration_StartUserList(dbContext, setDTO.ID).ForEach(r => setDTO.StartUserList.Add(ConvertToUserDTO(r)));
            }
        }

        private List<Configuration_User> GetConfiguration_StartUserList(KStarDbContext dbContext, int refId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Configuration_UserSet.Where(r => r.OperateType == Configuration_OperationType.StartProcess.ToString() && r.RefType == Configuration_RefType.ProcessSet.ToString() && r.RefID == refId).ToList();
            }
        }
        private List<Configuration_User> GetConfiguration_ReworkCc(KStarDbContext dbContext, int refId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Configuration_UserSet.Where(r => r.OperateType == Configuration_OperationType.ReworkCc.ToString() && r.RefType == Configuration_RefType.ProcessSet.ToString() && r.RefID == refId).ToList();
            }
        }
        private List<Configuration_User> GetConfiguration_EndCc(KStarDbContext dbContext, int refId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Configuration_UserSet.Where(r => r.OperateType == Configuration_OperationType.EndCc.ToString() && r.RefType == Configuration_RefType.ProcessSet.ToString() && r.RefID == refId).ToList();
            }
        }
        private List<Configuration_User> GetConfiguration_OperateUserList(KStarDbContext dbContext, int refId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Configuration_UserSet.Where(r => r.OperateType == Configuration_OperationType.OperateProcess.ToString() && r.RefType == Configuration_RefType.Activity.ToString() && r.RefID == refId).ToList();
            }
        }

        private void ConvertSetUser(Configuration_UserDTO dto, ref Configuration_User config)
        {
            if (config == null)
            {
                config = new Configuration_User()
                {
                    ID = dto.ID,
                    OperateType = dto.OperateType.ToString(),
                    RefID = dto.RefID,
                    RefType = dto.RefType.ToString()
                };
            }
            config.UserType = dto.UserType.ToString();
            config.Value = dto.Value;
            config.Key = dto.Key;
        }
        private void ConvertSetProcSet(Configuration_ProcessSetDTO dto, Configuration_ProcessSet config)
        {
            if (config == null)
            {
                config = new Configuration_ProcessSet()
                {
                    ProcessFullName = dto.ProcessFullName,
                    ProcessSetID = dto.ProcessSetID,
                    ProcessSetNo = dto.ProcessSetNo
                };
            }
            config.ID = dto.ID;
            config.ApproveUrl = dto.ApproveUrl;
            config.Configuration_CategoryID = dto.Configuration_CategoryID;
            config.Description = dto.Description;
            config.NotAssignIfApproved = dto.NotAssignIfApproved;
            config.ProcessPredict = dto.ProcessPredict;
            config.LoopRemark = dto.LoopRemark;
            config.OrderNo = dto.OrderNo;
            config.ProcessName = dto.ProcessName;
            config.StartUrl = dto.StartUrl;
            config.ViewUrl = dto.ViewUrl;
        }
        private Configuration_Category ConvertToCategory(Configuration_CategoryDTO obj)
        {
            return new Configuration_Category()
            {
                ID = obj.ID,
                Description = obj.Description,
                Name = obj.Name
            };
        }
        private Configuration_CategoryDTO ConvertToCategoryDTO(Configuration_Category obj)
        {
            return new Configuration_CategoryDTO()
                        {
                            ID = obj.ID,
                            Description = obj.Description,
                            Name = obj.Name,
                            ProcessSetList = new List<Configuration_ProcessSetDTO>()
                        };
        }
        private Configuration_UserDTO ConvertToUserDTO(Configuration_User obj)
        {
            Guid userid = Guid.Empty;
            Guid.TryParse(obj.Key, out userid);
            UserBaseDto user = _userService.ReadUserBase(userid);
            return new Configuration_UserDTO()
            {
                ID = obj.ID,
                OperateType = (Configuration_OperationType)Enum.Parse(typeof(Configuration_OperationType), obj.OperateType),
                RefID = obj.RefID,
                RefType = (Configuration_RefType)Enum.Parse(typeof(Configuration_RefType), obj.RefType),
                UserType = (Configuration_UserType)Enum.Parse(typeof(Configuration_UserType), obj.UserType),
                Value = (user == null ? obj.Value : user.FullName),
                Key = obj.Key
            };
        }
        private Configuration_User ConvertToUser(Configuration_UserDTO obj)
        {
            return new Configuration_User()
            {
                ID = obj.ID,
                OperateType = obj.OperateType.ToString(),
                RefID = obj.RefID,
                RefType = obj.RefType.ToString(),
                UserType = obj.UserType.ToString(),
                Value = obj.Value,
                Key = obj.Key
            };
        }
        private Configuration_ActivityDTO ConvertToActivityDTO(Activity obj, Configuration_Activity config)
        {
            var dto = new Configuration_ActivityDTO()
            {
                ActivityID = obj.ID,
                ActivityNo = obj.ID.ToString(),
                MetaData = obj.DisplayName,
                Name = obj.Name
            };
            if (config != null)
            {
                dto.ID = config.ID;
                dto.Configuration_ProcessVersionID = config.Configuration_ProcessVersionID;
                dto.ProcessTime = config.ProcessTime;
            }
            return dto;
        }
        private List<Configuration_RefActivity> ConvertToReActivity(int actid, List<int> reActivity)
        {
            List<Configuration_RefActivity> list = new List<Configuration_RefActivity>();
            foreach (var item in reActivity)
            {
                list.Add(new Configuration_RefActivity()
                {
                    ActivityID = item,
                    Configuration_ActivityID = actid
                });
            }
            return list;
        }

        private Configuration_ProcessSetDTO ConvertToProcSetDTO(ProcessSet obj, Configuration_ProcessSet config)
        {
            if (obj == null && config == null)
                return null;

            var dto = new Configuration_ProcessSetDTO();
            if (obj != null)
            {
                dto.ProcessFullName = obj.FullName;
                dto.ProcessName = obj.DisplayName;
                dto.ProcessSetID = obj.ID;
                dto.ProcessSetNo = obj.ID.ToString();
                dto.Description = obj.Descr;
                dto.StartUrl = obj.StartPageUrl;
                dto.ViewUrl = obj.ViewPageUrl;
            }
            else
            {
                dto.ProcessFullName = config.ProcessFullName;
                dto.ProcessSetID = config.ProcessSetID;
                dto.ProcessSetNo = config.ProcessSetNo;
            }
            if (config != null)
            {
                dto.Configuration_CategoryID = config.Configuration_CategoryID;
                dto.ID = config.ID;
                dto.ProcessName = config.ProcessName;
                dto.OrderNo = config.OrderNo;
                //dto.ProcessFullName = config.ProcessFullName;
                //dto.ProcessSetID = config.ProcessSetID;
                //dto.ProcessSetNo = config.ProcessSetNo;
                dto.NotAssignIfApproved = config.NotAssignIfApproved;
                dto.StartUrl = config.StartUrl;
                dto.ViewUrl = config.ViewUrl;
                dto.ApproveUrl = config.ApproveUrl;
                dto.Description = config.Description;
                dto.ProcessPredict = config.ProcessPredict;
                dto.LoopRemark = config.LoopRemark;
            }

            return dto;
        }
        private Configuration_ProcessVersionDTO ConvertToProcVersionDTO(ProcessVersion obj, Configuration_ProcessVersion config)
        {
            var dto = new Configuration_ProcessVersionDTO()
            {
                DeployDate = obj.ChangeDate,
                IsCurrent = obj.DefaultVerID == obj.ID,
                ProcessVersionID = obj.ID,
                VersionNo = obj.Version.ToString()
            };
            if (config != null)
            {
                dto.ProcessVersionID = config.ProcessVersionID;
                dto.Configuration_ProcessSetID = config.Configuration_ProcessSetID;
                dto.ID = config.ID;
            }
            return dto;
        }
        private void UpdateProcVersion(int configuration_ProcessSetID, List<Configuration_ProcessVersionDTO> config)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var processVersions = dbContext.Configuration_ProcessVersionSet.Where(x => x.Configuration_ProcessSetID == configuration_ProcessSetID).ToList();
                for (int i = 0; i < processVersions.Count; i++)
                {
                    var versionconfig = config.Where(x => x.ProcessVersionID == processVersions[i].ProcessVersionID).FirstOrDefault();
                    if (versionconfig != null)
                    {
                        UpdateProcActivity(processVersions[i].ID, versionconfig.ActivityList);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private void UpdateProcActivity(int configuration_ProcessVersionID, List<Configuration_ActivityDTO> config)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var processActivity = dbContext.Configuration_ActivitySet.Where(x => x.Configuration_ProcessVersionID == configuration_ProcessVersionID).ToList();
                for (int i = 0; i < processActivity.Count; i++)
                {
                    var activityconfig = config.Where(x => x.ActivityID == processActivity[i].ActivityID).FirstOrDefault();
                    if (activityconfig != null)
                    {
                        processActivity[i].ProcessTime = activityconfig.ProcessTime;
                        UpdateProcActivityOperateUser(processActivity[i].ID, activityconfig.OperateUserList);
                        UpdateProcActivityReworkActivity(processActivity[i].ID, activityconfig.ReworkActivityList);
                    }
                }
                dbContext.SaveChanges();
            }
        }

        private void UpdateProcActivityOperateUser(int configuration_ActivityID, List<Configuration_UserDTO> config)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var processActivityOperateUser = dbContext.Configuration_UserSet.Where(x => x.RefID == configuration_ActivityID && x.RefType == Configuration_RefType.Activity.ToString()).ToList();


                for (int i = 0; i < config.Count; i++)
                {
                    var operateUserconfig = processActivityOperateUser.Where(x => x.Key == config[i].Key && x.UserType == config[i].UserType.ToString()).FirstOrDefault();
                    if (operateUserconfig != null)
                    {
                        operateUserconfig.OperateType = config[i].OperateType.ToString();
                        operateUserconfig.Value = config[i].Value;
                        operateUserconfig.RefType = config[i].RefType.ToString();
                    }
                    else
                    {
                        Configuration_User user = new Configuration_User();
                        user.RefID = configuration_ActivityID;
                        user.UserType = config[i].UserType.ToString();
                        user.RefType = config[i].RefType.ToString();
                        user.OperateType = config[i].OperateType.ToString();
                        user.Value = config[i].Value;
                        user.Key = config[i].Key;
                        dbContext.Configuration_UserSet.Add(user);
                    }
                }
                dbContext.SaveChanges();
            }
        }

        private void UpdateProcActivityReworkActivity(int configuration_ActivityID, List<int> config)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var processReWorkActivity = dbContext.Configuration_RefActivitySet.Where(x => x.ActivityID == configuration_ActivityID).ToList();
                for (int i = 0; i < config.Count; i++)
                {
                    var reWorkactivityconfig = processReWorkActivity.Where(x => x.Configuration_ActivityID == config[i]).FirstOrDefault();
                    if (reWorkactivityconfig == null)
                    {
                        Configuration_RefActivity react = new Configuration_RefActivity();
                        react.ActivityID = configuration_ActivityID;
                        react.Configuration_ActivityID = config[i];
                        dbContext.Configuration_RefActivitySet.Add(react);
                    }
                }
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region 


        public List<List<Configuration_ActDesc>> GetPrognosisRoutes(int procInstID, string actionName)
        { 
            //1、获取当前实例所在环节的描述
            var proInstDescList = this.GetProcInstStateDesc(procInstID);

            List<List<Configuration_ActDesc>> serrActDesc = new List<List<Configuration_ActDesc>>();

            //2、存在对应的实例数据和实例状态
            if (proInstDescList != null && proInstDescList.Count > 0)
            {
                var actList = new List<Configuration_ActDesc>();
                //3、 循环当前环节获取下一个环节信息
                foreach (var proInstDesc in proInstDescList)
                {
                    if (proInstDesc.Status == 2)
                    {
                        //3.1 获取实例信息
                        var _actList = this.GetLinkActivityNames(proInstDesc.ProcID, proInstDesc.ActID, proInstDesc.FullName, proInstDesc.Name);

                        if (_actList != null && _actList.Count > 0)
                            actList.AddRange(_actList);
                    }
                
                }
                if (actList.Count > 0)
                {
                    RecursionRoute(proInstDescList[0], actList, serrActDesc, actionName);
                }
            }
             
            return serrActDesc;
        }


        public List<List<Configuration_ActDesc>> GetDBPrognosisRoutes(int procInstID, string actName)
        {

            List<List<Configuration_ActDesc>> serrActDesc = new List<List<Configuration_ActDesc>>();
            try
            { 
                using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
                { 
                    var entity = dbContext.ProcessPrognosis.Where(x => x.SourceName == actName && x.ProcInstID == procInstID).FirstOrDefault();
                    if (entity != null)
                    {
                        var linq = from ps in dbContext.ProcessPrognosis
                                   join psd in dbContext.ProcessPrognosisDetail on ps.SysID equals psd.RSysID
                                   into joinTemp
                                   from tmp in joinTemp.DefaultIfEmpty()
                                   where ps.ProcInstID == procInstID && ps.LinkOrder >= entity.LinkOrder
                                   orderby ps.LinkOrder, ps.Name ascending
                                   select new Configuration_ActDesc()
                                   {
                                       ActID = ps.LinkOrder, //获取db 中的预判路径的actid 无用，此处用来保存linkorder
                                       LineName = ps.LineName,
                                       Name = ps.Name,
                                       SourceName = ps.SourceName,
                                       UserNames = tmp.UserName

                                   };
                        var pList = linq.ToList();

                        #region
                        if (pList != null && pList.Count>0)
                        {
                            List<Configuration_ActDesc> actList = null;
                            int _LinkOrder = -1;
                            foreach (var item in pList)
                            {
                                if (_LinkOrder < 0)
                                {
                                    _LinkOrder = item.ActID;
                                    actList = new List<Configuration_ActDesc>();

                                    AddActDesc(actList, item);
                                }
                                else
                                {
                                    if (_LinkOrder != item.ActID)
                                    {
                                        serrActDesc.Add(actList);
                                        _LinkOrder = item.ActID;
                                        actList = new List<Configuration_ActDesc>();
                                        AddActDesc(actList, item);
                                    }
                                    else
                                    {
                                        AddActDesc(actList, item);
                                    }
                                }
                            }
                            serrActDesc.Add(actList);
                        }
                        #endregion 
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return serrActDesc; 
        }
  
        private static void AddActDesc(List<Configuration_ActDesc> actList, Configuration_ActDesc item)
        {
            if (actList.Where(x => x.Name == item.Name).Count() <= 0)
            {
                actList.Add(item);
            }
            else
            {
                var sameItem = actList.Where(x => x.Name == item.Name).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(sameItem.UserNames))
                {
                    sameItem.UserNames = item.UserNames;
                }
                else
                {
                    sameItem.UserNames += "," + item.UserNames;
                }
            }
        }



        private void RecursionRoute(Configuration_ProcInstDesc proInstDesc, IList<Configuration_ActDesc> actDescList, List<List<Configuration_ActDesc>> collection, string action)
        {
            if (actDescList == null || actDescList.Count == 0)
                return;

            List<Configuration_ActDesc> actDesc = new List<Configuration_ActDesc>();

            List<Configuration_ActDesc> nextActDescList = new List<Configuration_ActDesc>();

            foreach (var actdesc in actDescList)
            {

                if (actdesc.Name == actdesc.SourceName)
                {
                    actdesc.UserNames = proInstDesc.LoopRemark ?? "自循环节点";
                    actDesc.Add(actdesc);
                    continue;
                } 

                //获取对应规则
                string ruleString = this.GetLienRule(proInstDesc.ProcInstID, actdesc.LineName, proInstDesc.FullName, actdesc.SourceName);
                //有规则则计算
                if (!string.IsNullOrWhiteSpace(ruleString))
                {
                    //执行线规则
                    if (this.ExecuteLienRule(proInstDesc.ProcInstID, actdesc.SourceName, proInstDesc.FullName, action, ruleString))
                    {
                        actDesc.Add(actdesc);
                    }
                }
                else//没规则直接添加
                {
                    actDesc.Add(actdesc);
                } 
               
                //获取下一级别连接的环节(自循环节点只出现一次）
                var actList = this.GetLinkActivityNames(proInstDesc.ProcID, actdesc.ActID, proInstDesc.FullName, actdesc.Name);

                if (actList != null && actList.Count > 0)
                {
                    nextActDescList.AddRange(actList);
                }

            }


            //判断是否是死循环
            bool isExist = false;
            foreach (var itemList in collection)
            {
                bool isEqual = false;
                foreach (var item in itemList)
                {
                    if (actDesc.Where(x => x.Name == item.Name && x.SourceName!=x.Name).Count() > 0)
                    {
                        isEqual = true;
                    }
                    else
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                {
                    isExist = true;
                    break;
                }
            }
            //存在死循环则直接结束
            if (!isExist && actDesc.Count > 0)
            {
                collection.Add(actDesc);
                //递归获取下一步.
                RecursionRoute(proInstDesc, nextActDescList, collection, action);
            }


        }
     
        #endregion
    }
}
