using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.CustomRole.Models;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Workflow.Configuration;
using ICSharpCode.SharpZipLib.Zip;
using aZaaS.KStar.Web.Helper;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class CustomRoleController : BaseMvcController
    {
        private readonly CustomManager roleService;        

        public CustomRoleController()
        {
            roleService = new CustomManager();
        }

        #region CustomRoleCategory
        /// <summary>
        /// 添加Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateCategory(CustomCategory model)
        {
            CustomRoleCategory customRoleCategeory = new CustomRoleCategory();
            customRoleCategeory.SysID = Guid.NewGuid();
            customRoleCategeory.Name = model.DisplayName;
            if (model.ParentID != null)
            {
                Guid ParentId;
                Guid.TryParse(model.ParentID, out ParentId);
                customRoleCategeory.Parent_SysId = ParentId;
            }

            model.CategoryID = roleService.CreateCategory(customRoleCategeory).ToString();
            CustomRoleTree item = new CustomRoleTree()
            {
                Type = CustomRoleTreeAddType.Category.ToString(),
                ParentID = model.ParentID,
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCategory(CustomCategory model)
        {
            Guid curCategoryId = Guid.Parse(model.CategoryID);

            CustomRoleCategory updateCategory = new CustomRoleCategory
            {
                SysID = curCategoryId,
                Name = model.DisplayName
            };
            roleService.UpdateCategory(updateCategory);

            CustomRoleTree item = new CustomRoleTree()
            {
                Type = CustomRoleTreeAddType.Category.ToString(),
                ParentID = model.ParentID,
                ID = model.CategoryID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCustomRoleType()
        {
            var items = roleService.GetCategoryByParentId(Guid.Empty);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomRole(string type, string ID = "")
        {
            List<CustomRoleTree> items = new List<CustomRoleTree>();
            var categories = new List<CustomRoleCategory>();
            var classifyList = new List<CustomRoleClassify>();

            if (ID == "")
            {
                categories = roleService.GetCategoryByParentId(Guid.Parse(type));
            }
            else
            {
                categories = roleService.GetCategoryByParentId(Guid.Parse(ID));
                classifyList = roleService.GetClassifyByCategoryId(Guid.Parse(ID)).OrderBy(r => r.RoleName).ToList();
            }

            foreach (var item in categories)
            {
                items.Add(new CustomRoleTree()
                {
                    DisplayName = item.Name,
                    HasChildren = (roleService.GetCategoryByParentId(item.SysID).Count() > 0) || (roleService.GetClassifyByCategoryId(item.SysID).Count() > 0) ? true : false,
                    ID = item.SysID.ToString(),
                    ParentID = null,
                    Type = CustomRoleTreeAddType.Category.ToString()
                });
            }

            foreach (var item in classifyList)
            {
                items.Add(new CustomRoleTree()
                {
                    DisplayName = item.RoleName,
                    HasChildren = false,
                    ID = item.SysID.ToString(),
                    ParentID = null,
                    Type = CustomRoleTreeAddType.Classify.ToString()
                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除Category
        /// </summary>
        /// <param name="models">List_Category</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DestroyCategory(string categoryID)
        {
            roleService.DestroyCategory(Guid.Parse(categoryID));
            return Json(categoryID, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CustomRoleClassify
        [HttpPost]
        public JsonResult CreateCustomRole(CustomRoleActionDTO model)
        {
            CustomRoleClassify roleClassify = new CustomRoleClassify
            {
                SysID = Guid.NewGuid(),
                RoleName = model.RoleName,
                RoleKey = model.RoleKey,
                ClassName = model.ClassName,
                AssembleName = model.AssembleName,
                Category_SysId = model.Category_SysId,
                Status = "N"// 默认为不启用
            };

            model.Id = roleService.CreateCustomRoleClassify(roleClassify);

            CustomRoleTree item = new CustomRoleTree()
            {
                Type = CustomRoleTreeAddType.Classify.ToString(),
                ParentID = model.Category_SysId.ToString(),
                ID = model.Id.ToString(),
                HasChildren = false,
                DisplayName = model.RoleName
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassifyByCategoryId(Guid id)
        {
            var items = roleService.GetClassifyByCategoryId(id);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassifyById(Guid id)
        {
            var item = roleService.GetClassifyById(id);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCustomRole(CustomClassify model)
        {
            Guid currClassifyId = Guid.Parse(model.CustomRoleID);

            CustomRoleClassify updateClassify = new CustomRoleClassify
            {
                SysID = currClassifyId,
                RoleName = model.DisplayName,
                Status = model.Status
            };
            roleService.UpdateCustomRole(updateClassify);

            CustomRoleTree item = new CustomRoleTree()
            {
                Type = CustomRoleTreeAddType.Classify.ToString(),
                ParentID = model.CategoryID,
                ID = model.CustomRoleID,
                HasChildren = false,
                DisplayName = model.DisplayName
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCustomRoleStatus(string id, string status)
        {
            Guid currClassifyId = Guid.Parse(id);

            CustomRoleClassify updateClassify = new CustomRoleClassify
            {
                SysID = currClassifyId,
                Status = status
            };

            var parentId = roleService.UpdateCustomRoleStatus(updateClassify);

            var items = roleService.GetClassifyByCategoryId(parentId);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除Classify
        /// </summary>
        /// <param name="id">List_Position</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DestroyRoleClassify(string customRoleID)
        {
            roleService.DestroyRoleClassify(Guid.Parse(customRoleID));
            return Json(customRoleID, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CommonSelectPersonControl
        public JsonResult GetCustomRoleByCommonControl(string ID = "")
        {
            List<Organization> organizationlist = new List<Organization>();
            var categories = new List<CustomRoleCategory>();
            var classifyList = new List<CustomRoleClassify>();

            if (ID == "")
            {
                var date = roleService.GetCategoryByParentId(Guid.Empty);
                categories = date.ToList();
            }
            else
            {
                categories = roleService.GetCategoryByParentId(Guid.Parse(ID));
                classifyList = roleService.GetClassifyByCategoryId(Guid.Parse(ID)).OrderBy(r => r.RoleName).ToList();
            }

            foreach (var item in categories)
            {
                organizationlist.Add(new Organization()
                {
                    ID = item.SysID.ToString(),
                    SysId = item.SysID.ToString(),
                    ParentID = null,
                    Type = "Root",
                    HasChildNode = (roleService.GetCategoryByParentId(item.SysID).Count() > 0) ? true : false,
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
                nodes=roleService.GetEnabledClassify();
                nodes = nodes.Where(x => Chinese2Spell.GetFirstSpelling((x.RoleName ?? "")).Contains(keyword.ToLower()) || (x.RoleName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                nodes = roleService.GetEnabledClassifyByCategoryId(id);
                if (nodes != null)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        nodes = nodes.Where(x => Chinese2Spell.GetFirstSpelling((x.RoleName ?? "")).Contains(keyword.ToLower()) || (x.RoleName ?? "").ToUpper().Contains(keyword.ToUpper())).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
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

        #region UploadDll
        public ActionResult UploadCustomRoleDll(IList<HttpPostedFileBase> files, string Field)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    string extension = System.IO.Path.GetExtension(fileName);
                    if (extension == ".Zip")
                    {
                        string serverPath = ConfigurationManager.AppSettings["CustomRole_UploadDir"].ToString();

                        if (!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }
                        var physicalPath = System.IO.Path.Combine(serverPath, fileName);

                        file.SaveAs(physicalPath);

                        return Content("");
                    }
                    else
                    {
                        return Content("Only .Zip files can be uploaded");
                    }

                }
            }
            return Content("files is Null");
        }

        public JsonResult GetAssembleInfo(string fileName)
        {
            string uploadDir = ConfigurationManager.AppSettings["CustomRole_UploadDir"].ToString();
            string dllDir = ConfigurationManager.AppSettings["CustomRole_DllDir"].ToString();
            var zipPath = Path.Combine(uploadDir, fileName);
            string directoryPath = Path.GetFileNameWithoutExtension(zipPath);
            string xmlName = directoryPath + ".xml";
            string dllName = directoryPath + ".dll";
            string xmlPath = Path.Combine(uploadDir, directoryPath, xmlName);
            string dllPath = Path.Combine(uploadDir, directoryPath, dllName);
            
            //解压文件
            UnZipFile(zipPath, null);

            if (!Directory.Exists(dllDir))
            {
                Directory.CreateDirectory(dllDir);
            }

            //将DLL文件复制到WCF服务目录下
            System.IO.File.Copy(dllPath, Path.Combine(dllDir, dllName), true);

            //解析xml说明文件
            List<ClassInfo> list = Desrialize<List<ClassInfo>>(xmlPath);
            List<CustomRoleAction> items = new List<CustomRoleAction>();

            foreach (var item in list)
            {
                items.Add(new CustomRoleAction()
                {
                    Id = Guid.NewGuid(),
                    ClassName = item.ClassName,
                    RoleName = item.RoleName,
                    RoleKey = item.Key,
                    Status = "N"
                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipedFileName">压缩文件路径</param>
        /// <param name="targetDirectory">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        private void UnZipFile(string zipedFileName, string targetDirectory)
        {
            if (string.IsNullOrEmpty(targetDirectory))
            {
                targetDirectory = zipedFileName.Replace(Path.GetFileName(zipedFileName), Path.GetFileNameWithoutExtension(zipedFileName));
            }

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(zipedFileName, targetDirectory, string.Empty);
        }

        /// <summary>
        /// 反序列化方法
        /// </summary>     
        /// <param name="xml">xml文件路径</param>   
        /// <param name="type">反序列化对象的类型</param>   
        /// <returns>反序列化后的对象</returns>
        private T Desrialize<T>(string xml)
        {
            T obj = default(T);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            TextReader tr = new StreamReader(xml);

            using (tr)
            {
                obj = (T)xs.Deserialize(tr);
            }

            return obj;
        }
        #endregion
    }
}
