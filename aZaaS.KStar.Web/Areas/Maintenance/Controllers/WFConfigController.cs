using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Localization;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Workflow.Configuration.Models;
using aZaaS.KStar.WorkflowConfiguration.Models;
using CsQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers 
{
    public class SenderTableData
    {
        public string SenderNumber { get; set; }
        public string SenderObject { get; set; }
        public string SenderType { get; set; }
    }

    [EnhancedHandleError]
    public class WFConfigController : BaseMvcController
    {
        BusinessDataBO _businessdataBO = new BusinessDataBO();
        private readonly KStarFormSettingProvider _formSettingProvider;
        private readonly ControlTmplProvider _controlTmplProvider;

        public WFConfigController()
        {
            _formSettingProvider = new KStarFormSettingProvider();
            _controlTmplProvider = new ControlTmplProvider();
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 保存"发起人"信息
        /// </summary>
        /// <param name="procSetId"></param>
        /// <param name="startUserListOfString"></param>
        /// <returns></returns>
        public bool SaveSenderInfo(int procSetId, string startUserListOfString)
        {
            var startUserList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Configuration_UserDTO>>(startUserListOfString);
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                return svc.UpdateStartUserList(procSetId, startUserList);
            }
            catch
            {
                return false;
            }
        }

        public bool SaveActivityBasicInfo(int configurationActivityId, string processtime)
        {            
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                decimal time = 0;
                decimal.TryParse(processtime, out time);
                return svc.UpdateActivityBasicInfo(configurationActivityId, Convert.ToInt32(time * 8));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存"处理人"信息
        /// </summary>
        /// <param name="configurationActivityId"></param>
        /// <param name="operateUserListOfString"></param>
        /// <returns></returns>
        public bool SaveHandlerInfo(int configurationActivityId, string operateUserListOfString)
        {
            var operateUserList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Configuration_UserDTO>>(operateUserListOfString);
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                return svc.UpdateOperateUserList(configurationActivityId, operateUserList);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存"基本信息"
        /// </summary>
        /// <param name="dtoOfString"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveProcessSetBasicInfo(Configuration_ProcessSetDTO dto, [ModelBinder(typeof(JsonListBinder<Configuration_UserDTO>))]IList<Configuration_UserDTO> EndCc, [ModelBinder(typeof(JsonListBinder<Configuration_UserDTO>))]IList<Configuration_UserDTO> ReworkCc)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            if (EndCc.Count > 0)
            {
                dto.EndCc = EndCc.ToList();
            }
            if (ReworkCc.Count > 0)
            {
                dto.ReworkCc = ReworkCc.ToList();
            }
            try
            {
                svc.UpdateProcessSet(dto);
                return Json(svc.GetProcessSetByID(this.CurrentUser, dto.ID, true), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new Configuration_ProcessSetDTO(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存"可退回环节"
        /// </summary>
        /// <param name="configurationActivityId"></param>
        /// <param name="reworkActivityListOfString"></param>
        /// <returns></returns>
        public bool SaveRework(int configurationActivityId, string reworkActivityListOfString)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var reworkActivityList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(reworkActivityListOfString);
            try
            {
                return svc.UpdateReworkActivityList(configurationActivityId, reworkActivityList);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ImportConfigVersion(FormCollection fc)
        {
            var file = Request.Files[0];
            int procID = 0;
            Int32.TryParse(Request["procID"] as string, out procID);
            Session["configuration_ProcessVersionID"] = procID;
            XmlDocument xmlDoc = new XmlDocument();
            string xml = "";
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            using (Stream fs = file.InputStream)
            {
                try
                {
                    xmlDoc.Load(fs);
                    xml = xmlDoc.InnerXml;
                    svc.ImportConfigurationVersion(this.CurrentUser, procID, xml);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        public ActionResult ExportConfigurationVersion(int configuration_ProcessVersionID)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                string xmlString = svc.ExportConfigurationVersion(this.CurrentUser, configuration_ProcessVersionID);

                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml";
                string filePath = Server.MapPath("/") + fileName;
                byte[] contents = System.Text.Encoding.UTF8.GetBytes(xmlString);

                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.ContentType = "application/octet-stream";

                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                Response.BinaryWrite(contents);
                Response.Flush();
                Response.End();
            }
            catch
            {

            }
            return new EmptyResult();
        }

        #region  流程配置导入导出
        [HttpPost]
        public JsonResult ImportProcessConfiguration(int configuration_ProcessID)
        {
            var file = Request.Files[0];                   
            XmlDocument xmlDoc = new XmlDocument();
            string xml = "";
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();            
            using (StreamReader reader = new StreamReader(file.InputStream, Encoding.UTF8))
            {
                try
                {
                    xmlDoc.LoadXml(reader.ReadToEnd());
                    xml = xmlDoc.InnerXml;

                    Configuration_ProcessConfigDTO dto = XMLHelper.Deserialize<Configuration_ProcessConfigDTO>(xml);
                    Configuration_ProcessSetDTO process = svc.ImportProcessConfiguration(this.CurrentUser, configuration_ProcessID, dto);
                    return Json(new { process = process, flag = true }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(new { process = new Configuration_ProcessSet(), flag = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ExportProcessConfiguration(int configuration_ProcessID)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            try
            {
                Configuration_ProcessConfigDTO dto = new Configuration_ProcessConfigDTO();
                Configuration_ProcessSetDTO process = svc.GetProcessSetByID(this.CurrentUser, configuration_ProcessID, true);
                BusinessDataConfigDTO bdcf = _businessdataBO.ReadConfig(process.ProcessFullName);
                IList<CotnrolSettingModel> _settings = _formSettingProvider.GetControlSettings(process.ProcessFullName);
                   IList<Configuration_LineRule> _LineRule = null;
                   using (KStarDbContext db = new KStarDbContext())
                   {
                       _LineRule = db.Configuration_LineRule.Where(x => x.FullName == process.ProcessFullName).ToList();
                   }

                   GetConfiguration_ProcessSetDTO(dto, process, bdcf, _settings.ToList(), _LineRule);

                string xml = XMLHelper.Serializer(dto);

                string fileName = process.ProcessFullName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml";
                string filePath = Server.MapPath("/") + fileName;
                byte[] contents = System.Text.Encoding.UTF8.GetBytes(xml);

                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.ContentType = "application/octet-stream";

                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                Response.BinaryWrite(contents);
                Response.Flush();
                Response.End();
            }
            catch(Exception ex)
            {

            }
            return new EmptyResult();
        }
        private void GetConfiguration_ProcessSetDTO(Configuration_ProcessConfigDTO dto, Configuration_ProcessSetDTO process, BusinessDataConfigDTO bdcf, List<CotnrolSettingModel> setting, IList<Configuration_LineRule> lineRules)
        {            
            if (process != null)
            {
                dto.ID = process.ID;
                dto.ProcessSetID = process.ProcessSetID;
                dto.Configuration_CategoryID = process.Configuration_CategoryID;
                dto.ProcessSetNo = process.ProcessSetNo;
                dto.ProcessFullName = process.ProcessFullName;
                dto.ProcessName = process.ProcessName;
                dto.OrderNo = process.OrderNo;                               
                dto.StartUrl = process.StartUrl;
                dto.ViewUrl = process.ViewUrl;
                dto.ApproveUrl = process.ApproveUrl;
                dto.NotAssignIfApproved = process.NotAssignIfApproved;
                dto.Description = process.Description;
                dto.EndCc = process.EndCc;
                dto.ReworkCc = process.ReworkCc;
                dto.StartUserList = process.StartUserList;
                dto.ProcessVersionList = process.ProcessVersionList;
                dto.ProcessPredict = process.ProcessPredict;
                dto.LoopRemark = process.LoopRemark;
            }
            if (bdcf != null)
            {
                dto.DbConnectionString = bdcf.DbConnectionString;
                dto.DataTable = bdcf.DataTable;
                dto.WhereQuery = bdcf.WhereQuery;
            }

            dto.settings = new List<Process_ControlSettingDTO>();
            foreach (var set in setting)
            {
                dto.settings.Add(new Process_ControlSettingDTO()
                {
                    SysId = set.SysId,
                    ProcessFullName = set.ProcessFullName,
                    ActivityId = set.ActivityId,
                    RenderTemplateId = set.RenderTemplateId,
                    RenderTemplateName = set.RenderTemplateName,                                        
                    ComponentId=set.ComponentId,
                    ControlName=set.ControlName,
                    ControlRenderId=set.ControlRenderId,
                    ControlType = (ProcessControlType)Enum.Parse(typeof(ProcessControlType), set.ControlType.ToString()),
                    IsCustom=set.IsCustom,
                    IsDisable=set.IsDisable,
                    IsHide=set.IsHide,                    
                    WorkMode = (ProcessWorkMode)Enum.Parse(typeof(ProcessWorkMode),set.WorkMode.ToString())
                });
            }
            dto.lineRules = new List<Configuration_LineRule>();
            if (lineRules != null)
            {
                foreach (var entity in lineRules)
                {
                    dto.lineRules.Add(entity);
                }
            }
        }
        #endregion

        public JsonResult GetAllCategoryList()
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            string mapPath = Server.MapPath("~");
            string cshtmlResxRoot = Path.Combine(mapPath, "Resx");
            string cultureName = ResxService.GetAvailableCulture();
            string cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, "Areas/Maintenance/Views/WFConfig/Index_cshtml." + cultureName + ".resx");
            FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
            if (fi_parentCulture.Exists)
            {
                svc.DefaultCategory = ResxService.GetResouces("NonConfiguration_Category", cshtmlResxFilePath);
            }
            var tree = svc.GetAllCategoryList(this.CurrentUser, true);

            #region
            //var tree = new List<Configuration_CategoryDTO>
            //                        {
            //            #region 分类
            //                            new Configuration_CategoryDTO
            //                            {
            //                                ID=1,
            //                                Name="a",
            //                                Description="b",
            //                                 ProcessSetList=new List<Configuration_ProcessSetDTO>
            //                                {
            //            #region 流程
            //                                    new Configuration_ProcessSetDTO
            //                                    {
            //                                        ID=0,
            //                                        ProcessSetID=1,
            //                                        Configuration_CategoryID=1,
            //                                        ProcessSetNo="1",
            //                                        ProcessFullName="aabb",
            //                                        ProcessName="ba",
            //                                        OrderNo=1,
            //                                        StartUrl="/asdf/we",
            //                                        ViewUrl="/qwe/xc",
            //                                        ApproveUrl="/dd/kk",
            //                                        NotAssignIfApproved=true,
            //                                        Description="bb",
            //                                        EndCc=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                        },
            //                                        ReworkCc=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                        },
            //                                        StartUserList=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="afd"
            //                                            },
            //                                        },
            //                                         ProcessVersionList=new List<Configuration_ProcessVersionDTO>
            //                                        {
            //                                            new Configuration_ProcessVersionDTO
            //                                            {
            //                                                ID=0,
            //                                                Configuration_ProcessSetID=1,
            //                                                ProcessVersionID=2,
            //                                                VersionNo="asd",
            //                                                DeployDate=DateTime.Now,
            //                                                IsCurrent=false,
            //                                                 ActivityList=new List<Configuration_ActivityDTO>
            //                                                {
            //                                                    new Configuration_ActivityDTO
            //                                                    {
            //                                                        ID=0,
            //                                                        Configuration_ProcessVersionID=1,
            //                                                        ActivityID=1,
            //                                                        ActivityNo="asd",
            //                                                        MetaData="zxc",
            //                                                        Name="qwe",
            //                                                        ReworkActivityList=new List<int>{1,3,4},
            //                                                        OperateUserList=new List<Configuration_UserDTO>
            //                                                        {
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },
            //                                                        }
            //                                                    },
            //                                                }
            //                                            },
            //                                        }
            //                                    },
            //#endregion
            //                                }
            //                            },
            //            #endregion
            //                        };
            #endregion
            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategoryListWithNoChilds()
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            IEnumerable<Configuration_CategoryDTO> categorys = svc.GetAllCategoryList(this.CurrentUser, false);
            var tree = categorys.Where(x => x.ID > 0).ToList();
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessSetListByCategory(int id)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var tree = svc.GetProcessSetListByCategory(this.CurrentUser, id, true);

            #region
            //var set = tree.ToList()[0];
            //set.Configuration_CategoryID = 2;
            //svc.InitProcessSet(this.CurrentUser, set.ProcessSetID, 2);

            //var tree = new List<Configuration_ProcessSetDTO>
            //                                {
            //            #region 流程
            //                                    new Configuration_ProcessSetDTO
            //                                    {
            //                                        ID=0,
            //                                        ProcessSetID=1, /* if 0, process_set add 'not initialize' */
            //                                        Configuration_CategoryID=1,
            //                                        ProcessSetNo="1",
            //                                        ProcessFullName="ab",
            //                                        ProcessName="ba",
            //                                        OrderNo=1,
            //                                        StartUrl="/asdf/we",
            //                                        ViewUrl="/qwe/xc",
            //                                        ApproveUrl="/dd/kk",
            //                                        NotAssignIfApproved=true,
            //                                        Description="bb",
            //                                        EndCc=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                        },
            //                                        ReworkCc=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.CustomType,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                        },
            //                                        StartUserList=new List<Configuration_UserDTO>
            //                                        {
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.OrgNode,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="axd"
            //                                            },
            //                                            new Configuration_UserDTO
            //                                            {
            //                                                ID=0,
            //                                                RefID=2,
            //                                                UserType=Configuration_UserType.User,
            //                                                RefType=Configuration_RefType.Activity,
            //                                                OperateType=Configuration_OperationType.EndCc,
            //                                                Value="asd"
            //                                            },
            //                                        },
            //                                         ProcessVersionList=new List<Configuration_ProcessVersionDTO>
            //                                        {
            //                                            new Configuration_ProcessVersionDTO
            //                                            {
            //                                                ID=0,
            //                                                Configuration_ProcessSetID=1,
            //                                                ProcessVersionID=2,
            //                                                VersionNo="asd",
            //                                                DeployDate=DateTime.Now,
            //                                                IsCurrent=false,
            //                                                 ActivityList=new List<Configuration_ActivityDTO>
            //                                                {
            //                                                    new Configuration_ActivityDTO
            //                                                    {
            //                                                        ID=0,
            //                                                        Configuration_ProcessVersionID=1,
            //                                                        ActivityID=1,
            //                                                        ActivityNo="asd",
            //                                                        MetaData="zxc",
            //                                                        Name="qwe",
            //                                                        ReworkActivityList=new List<int>{1,3,4},
            //                                                        OperateUserList=new List<Configuration_UserDTO>
            //                                                        {
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },
            //                                                        }
            //                                                    },
            //                                                }
            //                                            },
            //                                        }
            //                                    },
            //#endregion
            //                                };
            #endregion
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessVersionListByProcessSet(int id, int procSetId)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var tree = svc.GetProcessVersionListByProcessSet(this.CurrentUser, id, procSetId, true);
            #region
            //var tree = new List<Configuration_ProcessVersionDTO>
            //                                        {

            //#region 版本 
            //                                            new Configuration_ProcessVersionDTO
            //                                            {
            //                                                ID=0,
            //                                                Configuration_ProcessSetID=1,
            //                                                ProcessVersionID=2, /* if 0, process_version add 'not initialize' */
            //                                                VersionNo="asd",
            //                                                DeployDate=DateTime.Now,
            //                                                IsCurrent=false,

            //                                                 ActivityList=new List<Configuration_ActivityDTO>
            //                                                {
            //                                                    new Configuration_ActivityDTO
            //                                                    {
            //                                                        ID=0,
            //                                                        Configuration_ProcessVersionID=1,
            //                                                        ActivityID=1,
            //                                                        ActivityNo="asd",
            //                                                        MetaData="zxc",
            //                                                        Name="qwe",
            //                                                        ReworkActivityList=new List<int>{1,3,4},
            //                                                        OperateUserList=new List<Configuration_UserDTO>
            //                                                        {
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="paid"
            //                                                            },
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },
            //                                                        },
            //                                                    },
            //                                                }
            //                                            },
            //#endregion
            //                                        };
            #endregion
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SynchronizeVersion(int id)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            //同步版本
            int count = svc.SynchronizeConfigurationVersion(this.CurrentUser, id);

            return Json(count, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ProcessUnbundling(int id)
        {
            var svc = new ConfigManager(this.AuthType);
            //解除绑定
            bool flag = svc.ProcessUnbundling(id);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断是否需要更新版本
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true 需要更新，false不需要</returns>
        public JsonResult IsNeedUpdateConfigurationVersion(int id)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            //判断是否需要更新版本
            bool flag = svc.IsNeedUpdateConfigurationVersion(this.CurrentUser, id);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivityListByProcessVersion(int id, int procID, int procSetID)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var tree = svc.GetActivityListByProcessVersion(this.CurrentUser, id, procID, procSetID, true);
            
            #region
            //var tree = new List<Configuration_ActivityDTO>
            //                                                {
            //#region 环节
            //                                                    new Configuration_ActivityDTO
            //                                                    {
            //                                                        ID=0,
            //                                                        Configuration_ProcessVersionID=1,
            //                                                        ActivityID=1,
            //                                                        ActivityNo="asd",
            //                                                        MetaData="zxc",
            //                                                        Name="qwe",
            //                                                        ReworkActivityList=new List<int>{1,3,4},
            //                                                        OperateUserList=new List<Configuration_UserDTO>
            //                                                        {
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="card"
            //                                                            },
            //                                                            new Configuration_UserDTO
            //                                                            {
            //                                                                ID=0,
            //                                                                RefID=2,
            //                                                                UserType=Configuration_UserType.CustomType,
            //                                                                RefType=Configuration_RefType.Activity,
            //                                                                OperateType=Configuration_OperationType.EndCc,
            //                                                                Value="asd"
            //                                                            },

            //                                                        },  

            //                                                    },

            //#endregion 
            //                                                };
            #endregion
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCategory(int id, string name, string description)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            Configuration_CategoryDTO category = null;
            if (id > 0)
            {
                category = svc.GetCategoryByID(this.CurrentUser, id, true);
                category.Name = name;
                category.Description = description;
                svc.UpdateCategory(category);
            }
            else
            {
                category = new Configuration_CategoryDTO() { Name = name, Description = description, ProcessSetList = new List<Configuration_ProcessSetDTO>() };
                category.ID = svc.AddCategory(category);
            }
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCategory(int id)
        {
            bool flag = false;
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            if (id > 0)
            {
                flag = svc.DeleteCategory(id);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public bool InitProcessSet(int procSetID, int categoryID, BDConfigView item)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.InitProcessSet(this.CurrentUser, procSetID, categoryID);

            if (item != null && !string.IsNullOrEmpty(item.ConnectionString) && !string.IsNullOrEmpty(item.DataTable) && !string.IsNullOrEmpty(item.WhereQuery))
            {
                var config = new BusinessDataConfigDTO()
                {
                    ApplicationName = item.ApplicationName,
                    ProcessName = item.ProcessName,
                    DbConnectionString = item.ConnectionString,
                    DataTable = item.DataTable,
                    WhereQuery = item.WhereQuery
                };
                item.WorklistID = _businessdataBO.CreateConfig(config).ToString();
            }
            return true;
        }

        public ActionResult FlowPartial()
        {
            return PartialView();
        }

        public ActionResult InitilizationPartial()
        {
            return PartialView();
        }

        public ActionResult SegmentPartial()
        {
            return PartialView();
        }

        #region  ControlSettings
        public ActionResult ControlSetting()
        {
            return PartialView("~/Areas/Settings/Views/ControlSetting.cshtml");
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveSettings([ModelBinder(typeof(JsonListBinder<CotnrolSettingModel>))]IList<CotnrolSettingModel> jsonData)
        {
            List<CotnrolSettingModel> result = jsonData.ToList();
            if (jsonData != null && jsonData.Count > 0)
            {
                List<CotnrolSettingModel> totalsettings = jsonData.ToList();
                List<CotnrolSettingModel> settings = new List<CotnrolSettingModel>();
                IList<CotnrolSettingModel> components = jsonData.Where(x => x.ControlType == ControlType.Component).ToList();
                IList<CotnrolSettingModel> controls = jsonData.Where(x => x.ControlType != ControlType.Component).ToList();
                foreach (var component in components)
                {
                    IList<CotnrolSettingModel> currentcontrols = controls.Where(x => x.ComponentId == component.ControlRenderId && (x.IsHide || x.IsDisable || x.IsCustom)).ToList();
                    if (component.IsHide || component.IsDisable || component.IsCustom || currentcontrols.Count > 0)
                    {
                        settings.Add(component);
                        var deletecomponent = totalsettings.Where(x => x.ControlRenderId == component.ControlRenderId).FirstOrDefault();
                        totalsettings.Remove(deletecomponent);
                    }
                    if (!component.IsHide && !component.IsCustom && currentcontrols.Count > 0)
                    {
                        settings.AddRange(currentcontrols);
                        foreach (var item in currentcontrols)
                        {
                            var deletecontrol = totalsettings.Where(x => x.ControlRenderId == item.ControlRenderId).FirstOrDefault();
                            totalsettings.Remove(deletecontrol);
                        }
                    }
                }
                //delete  totalsettings
                var deletesettings = totalsettings.Where(x => x.SysId != Guid.Empty).ToList();
                foreach (var item in deletesettings)
                {
                    _formSettingProvider.DeleteFormControlSetting(item.SysId);
                    var dtemp = result.Where(x => x.SysId == item.SysId).FirstOrDefault();
                    dtemp.SysId = Guid.Empty;
                    dtemp.IsCustom = false;
                    dtemp.IsDisable = false;
                    dtemp.IsHide = false;
                }
                //add  settings
                var addsettings = settings.Where(x => x.SysId == Guid.Empty).ToList();
                foreach (var item in addsettings)
                {
                    Guid newsysid = _formSettingProvider.AddControlSettings(item);

                    var atemp = result.Where(x => x.ControlRenderId == item.ControlRenderId && x.ControlType == item.ControlType).FirstOrDefault();
                    atemp.SysId = newsysid;
                }
                //edit settings
                var editsettings = settings.Where(x => x.SysId != Guid.Empty).ToList();
                foreach (var item in editsettings)
                {
                    _formSettingProvider.EditFormControlSetting(item);
                }
            }
            //还原KoModel
            Dictionary<string, object> komodel = new Dictionary<string, object>();
            if (result.Count > 0)
            {
                IList<CotnrolSettingModel> rcomponents = result.Where(x => x.ControlType == ControlType.Component).ToList();
                IList<CotnrolSettingModel> rcontrols = result.Where(x => x.ControlType != ControlType.Component).ToList();
                foreach (var rcomponent in rcomponents)
                {
                    komodel.Add("_" + rcomponent.ControlRenderId + "kocomponentModel", new { ComponentModel = rcomponent, ControlModel = rcontrols.Where(x => x.ComponentId == rcomponent.ControlRenderId).ToList() });
                }
            }
            return Json(komodel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenderKStarFormControlSettings(int activityId, int processId, string type)
        {
            string _html = string.Empty;           
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var process = svc.GetProcessSetByID(this.CurrentUser, processId);
            if (process != null)
            {
                string url= !string.IsNullOrEmpty(process.StartUrl) ? process.StartUrl : string.Empty;
                WorkMode mode=WorkMode.View;
                if (type == "Approve")
                {
                    mode = WorkMode.Approval;                
                }                
                else if (type == "Start")
                {
                    mode = WorkMode.Startup;                    
                }                
                if (!string.IsNullOrEmpty(url))
                {
                    string _temp_url=string.Empty;
                    var controlSettingMode = activityId > 0 ? WorkMode.Approval : WorkMode.Startup;
                    IList<CotnrolSettingModel> _settings = _formSettingProvider.GetControlSettings(process.ProcessFullName,activityId, mode);
                    IList<ControlTemplateModel> _template = _controlTmplProvider.GetTemplate();
                    if (url.IndexOf("?") >= 0)
                    {
                        //_temp_url = url + "&IsControlSetting=true";
                        _temp_url = string.Format("{0}&IsControlSetting=true&ControlSettingMode={1}", url, Convert.ToInt32(controlSettingMode));
                    }
                    else
                    {
                        //_temp_url = url + "?IsControlSetting=true";
                        _temp_url = string.Format("{0}?IsControlSetting=true&ControlSettingMode={1}", url, Convert.ToInt32(controlSettingMode));
                    }
                    CQ _doc = HttpHelper.GetTemplateContent(_temp_url,this.AuthType);
                    StringBuilder componenttemplate = new StringBuilder();
                    #region  -----模板html内容
                    componenttemplate.Append("");
                    componenttemplate.Append("<div class=\"panel panel-default kstar-component\">");
                    componenttemplate.Append("        <div class=\"panel-heading\">");
                    componenttemplate.Append("            <h4 class=\"panel-title\">");
                    componenttemplate.Append("                <a data-toggle=\"collapse\" href=\"#ApplicationInformation\">申请信息-(点击折叠)");
                    componenttemplate.Append("                </a>");
                    componenttemplate.Append("            </h4>");
                    componenttemplate.Append("        </div>");
                    componenttemplate.Append("        <div id=\"ApplicationInformation\" class=\"panel-collapse collapse in\">");
                    componenttemplate.Append("            <div class=\"panel-body\">");
                    componenttemplate.Append("                <div class=\"row\">");
                    componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                    componenttemplate.Append("                        <input id=\"showappinfo\" class='chkcontrol chkshow' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsHide\"  style=\"vertical-align: sub;\" /><label for=\"showappinfo\" style=\"cursor: pointer;font-weight: normal\">隐藏</label>");
                    componenttemplate.Append("                    </div>");
                    componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                    componenttemplate.Append("                        <input id=\"disabledappinfo\" class='chkcontrol chkdisabled' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsDisable\"  style=\"vertical-align: sub;\" /><label for=\"disabledappinfo\" style=\"cursor: pointer;font-weight: normal\">禁用子控件</label>");
                    componenttemplate.Append("                    </div>");
                    componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                    componenttemplate.Append("                        <input id=\"customappinfo\" class='chkcontrol chkcustom chkcustomtree' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsCustom\" style=\"vertical-align: sub;\" /><label for=\"templateappinfo\" style=\"cursor: pointer;font-weight: normal\">使用模板</label>");
                    componenttemplate.Append("                    </div>");
                    componenttemplate.Append("                    <div class=\"col-md-6\" style=\"\">");
                    componenttemplate.Append("                        <div class=\"row\">");
                    //componenttemplate.Append("                            <div class=\"col-sm-6\">");
                    //componenttemplate.Append("                                <label class=\"control-label\"></label>");
                    //componenttemplate.Append("                            </div>");
                    componenttemplate.Append("                            <div class=\"col-sm-12\">");
                    componenttemplate.Append("                                <input data-bind=\"value: ComponentModel.RenderTemplateName\" type=\"text\" readonly data-rule-required=\"true\" data-msg-required=\"请选择模板！\" class=\"form-control dropdowntree\" placeholder=\"请选择模板\" style=\"margin-top:-6px\" />");
                    componenttemplate.Append("                                <input data-bind=\"value: ComponentModel.RenderTemplateId\"  type=\"hidden\" />");
                    componenttemplate.Append("                            </div>");
                    componenttemplate.Append("                        </div>");
                    componenttemplate.Append("                    </div>");
                    componenttemplate.Append("                </div>");
                    componenttemplate.Append("                <div>");
                    componenttemplate.Append("                    <table class=\"table table-striped table-hover\">");
                    componenttemplate.Append("                        <thead>");
                    componenttemplate.Append("                            <tr>");
                    componenttemplate.Append("                                <th style=\"font-weight: normal\">控件#id</th>");
                    componenttemplate.Append("                                <th style=\"font-weight: normal\">控件名称</th>");
                    componenttemplate.Append("                                <th style=\"font-weight: normal\">操作选项</th>");
                    componenttemplate.Append("                            </tr>");
                    componenttemplate.Append("                        </thead>");
                    componenttemplate.Append("                        <tbody data-bind=\"foreach: $root.ControlModel\">");
                    componenttemplate.Append("                            <tr>");
                    componenttemplate.Append("                                <td><span data-bind=\"text: ControlRenderId\"></span></td>");
                    componenttemplate.Append("                                <td><span data-bind=\"text: ControlName\"></span></td>");
                    componenttemplate.Append("                                <td>");
                    componenttemplate.Append("                                    <div class=\"row\">");
                    componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                    componenttemplate.Append("                                            <input  class='chkcontrol chkshow' data-bind=\"checked: IsHide\"  type=\"checkbox\" value=\"\" style=\"vertical-align: sub;\" /><label  style=\"cursor: pointer;font-weight: normal\">隐藏</label>");
                    componenttemplate.Append("                                        </div>");
                    componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                    componenttemplate.Append("                                            <input class='chkcontrol chkdisabled'   data-bind=\"checked: IsDisable\"   type=\"checkbox\" value=\"\" style=\"vertical-align: sub;\" /><label  style=\"cursor: pointer;font-weight: normal\">禁用控件</label>");
                    componenttemplate.Append("                                        </div>");
                    componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                    componenttemplate.Append("                                            <input class='chkcontrol chkcustom chkcustomcontroltree' data-bind=\"checked: IsCustom\"   type=\"checkbox\" value=\"\" style=\"vertical-align: sub;\" /><label  style=\"cursor: pointer;font-weight: normal\">使用模板</label>");
                    componenttemplate.Append("                                        </div>");
                    componenttemplate.Append("                                    </div>");
                    componenttemplate.Append("                                    <div class=\"row templaterow\">");
                    componenttemplate.Append("                                        <div class=\"col-md-12\" style=\"\">");
                    componenttemplate.Append("                                            <div class=\"row\">");
                    componenttemplate.Append("                                                <div class=\"col-sm-8\">");
                    componenttemplate.Append("                                                    <label class=\"control-label\"></label>");
                    componenttemplate.Append("                                                </div>");
                    componenttemplate.Append("                                                <div class=\"col-sm-4\">");
                    componenttemplate.Append("                                                    <input data-bind=\"value: RenderTemplateName,attr:{'data': RenderTemplateId}\"  type=\"text\" readonly data-rule-required=\"true\" data-msg-required=\"请选择模板！\" class=\"form-control dropdowntree\" placeholder=\"请选择模板\" />");
                    componenttemplate.Append("                                                    <input data-bind=\"value: RenderTemplateId\"  type=\"hidden\" />");
                    componenttemplate.Append("                                                </div>");
                    componenttemplate.Append("                                            </div>");
                    componenttemplate.Append("                                        </div>");
                    componenttemplate.Append("                                    </div>");
                    componenttemplate.Append("                                </td>");
                    componenttemplate.Append("                            </tr>");
                    componenttemplate.Append("                        </tbody>");
                    componenttemplate.Append("                    </table>");
                    componenttemplate.Append("                </div>");
                    componenttemplate.Append("            </div>");
                    componenttemplate.Append("        </div>");
                    componenttemplate.Append("    </div>");
                    #endregion

                    List<IDomObject> _componentlist = new List<IDomObject>();
                    //component
                    IList<IDomObject> _kstar_componentlist = _doc[".kstar-component"].ToList();
                    //循环 获取组件
                    foreach (var component in _kstar_componentlist)
                    {
                        string componentId = component.GetAttribute("role");
                        string componentName = component.GetAttribute("title");
                        CQ _doctemplate = CQ.CreateDocument(componenttemplate.ToString());
                        CQ _component_temp = CQ.CreateDocument(component.InnerHTML);
                        //获取已保存的组件信息
                        CotnrolSettingModel _componentModel = _settings.Where(x => x.ControlRenderId == componentId && x.ControlType == ControlType.Component).FirstOrDefault();
                        //更新组件信息                    
                        if (_componentModel == null)
                        {
                            _componentModel = new CotnrolSettingModel();
                            _componentModel.ControlName = componentName;
                            _componentModel.ControlRenderId = componentId;
                            _componentModel.ControlType = ControlType.Component;
                            _componentModel.ActivityId = activityId;
                            _componentModel.WorkMode = mode;
                            _componentModel.ProcessFullName = process.ProcessFullName;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_componentModel.RenderTemplateId))
                            {
                                Guid tempid = Guid.Parse(_componentModel.RenderTemplateId);
                                ControlTemplateModel temp = _template.Where(x => x.SysId == tempid).FirstOrDefault();
                                _componentModel.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                            }
                        }

                        List<CotnrolSettingModel> _controlModel = new List<CotnrolSettingModel>();
                        //script
                        StringBuilder _script = new StringBuilder();
                        //替换标题
                        _doctemplate["div.panel-heading a"].Html(componentName);
                        //替换id
                        _doctemplate["div.panel-default"].Attr("id", componentId).Attr("model", "_" + componentId + "kocomponentModel");
                        _doctemplate["div.panel-collapse"].Attr("id", componentId + "_control");
                        _doctemplate["div.panel-heading a"].Attr("href", "#" + componentId + "_control");

                        //组件
                        var chkshow = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(0);
                        var chkdisabled = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(1);
                        var chkcustom = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(2);

                        chkshow.Attr("id", "show" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'show',$(this).prop('checked'));");
                        chkdisabled.Attr("id", "disabled" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'disabled',$(this).prop('checked'));");
                        chkcustom.Attr("id", "custom" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'custom',$(this).prop('checked'));");


                        _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(0).Attr("for", "show" + componentId);
                        _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(1).Attr("for", "disabled" + componentId);
                        _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(2).Attr("for", "custom" + componentId);


                        _doctemplate["tbody"].Attr("data-bind", "foreach:ControlModel");
                        //循环获取控件
                        IList<IDomObject> _kstar_controllist = _component_temp[".kstar-control"].ToList();
                        foreach (var control in _kstar_controllist)
                        {
                            //获取已存的控件信息
                            CotnrolSettingModel _controlModel_res = _settings.Where(x => x.ControlRenderId == control.Id && x.ControlType == ControlType.Control).FirstOrDefault();
                            if (_controlModel_res == null)
                            {
                                _controlModel_res = new CotnrolSettingModel()
                                {
                                    ControlRenderId = control.Id,
                                    ControlName = control.GetAttribute("title"),
                                    ControlType = ControlType.Control,
                                    ComponentId = componentId,
                                    ActivityId=activityId,
                                    WorkMode=mode,
                                    ProcessFullName = process.ProcessFullName
                                };
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(_controlModel_res.RenderTemplateId))
                                {
                                    Guid tempid = Guid.Parse(_controlModel_res.RenderTemplateId);
                                    ControlTemplateModel temp = _template.Where(x => x.SysId == tempid).FirstOrDefault();
                                    _controlModel_res.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                                }
                                _controlModel_res.ComponentId = componentId;
                            }
                            _controlModel.Add(_controlModel_res);
                        }
                        //循环获取按钮
                        IList<IDomObject> _kstar_buttonlist = _component_temp[".kstar-button"].ToList();
                        foreach (var control in _kstar_buttonlist)
                        {
                            //获取已存的控件信息
                            CotnrolSettingModel _controlModel_res = _settings.Where(x => x.ControlRenderId == control.Id && x.ControlType == ControlType.Button).FirstOrDefault();
                            if (_controlModel_res == null)
                            {
                                _controlModel_res = new CotnrolSettingModel()
                                {
                                    ControlRenderId = control.Id,
                                    ControlName = control.GetAttribute("title"),
                                    ControlType = ControlType.Button,
                                    ComponentId = componentId,
                                    ActivityId=activityId,
                                    WorkMode=mode,
                                    ProcessFullName = process.ProcessFullName
                                };
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(_controlModel_res.RenderTemplateId))
                                {
                                    Guid tempid = Guid.Parse(_controlModel_res.RenderTemplateId);
                                    ControlTemplateModel temp = _template.Where(x => x.SysId == tempid).FirstOrDefault();
                                    _controlModel_res.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                                }
                                _controlModel_res.ComponentId = componentId;
                            }
                            _controlModel.Add(_controlModel_res);
                        }
                        var _kocomponentModel = new { ComponentModel = _componentModel, ControlModel = _controlModel };
                        _script.Append("<script type=\"text/javascript\">");
                        _script.Append("var _" + componentId + "kocomponentModel = KStarForm.toKoModel(" + JsonHelper.SerializeObject(_kocomponentModel) + ");");
                        _script.Append("KStarForm.applyBindings(_" + componentId + "kocomponentModel, $('#" + componentId + "')[0]); ControlSettingWindow.initDropdownTreeStatus(\"chkcustomcontroltree\")");
                        _script.Append("</script>");
                        _componentlist.Add(_doctemplate[0]);
                        _componentlist.Add(CQ.CreateDocument(_script.ToString())[0]);
                    }

                    CQ _resultdoc = CQ.Create(_componentlist);
                    _html = _resultdoc.Render();
                }
            }
            return Content(_html, "text/html");
        }
        #endregion

    }

}
