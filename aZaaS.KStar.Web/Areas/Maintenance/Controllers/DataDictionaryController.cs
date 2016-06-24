using aZaaS.KStar.DataDictionary;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Facades;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class DataDictionaryController : BaseMvcController
    {
        private static readonly DataDictionaryFacade _datadicfacade = new DataDictionaryFacade();
        //
        // GET: /Maintenance/DataDictionary/

        public JsonResult GetDataDictionaryCategory(string id)
        {
            List<DataDictionaryTreeItem> trees = new List<DataDictionaryTreeItem>();
            List<DataDictionaryWithChildDto> datalist = null;
            if (string.IsNullOrEmpty(id))
            {
                datalist = _datadicfacade.GetAllDataDicCategory().ToList();
            }
            else
            {
                datalist = _datadicfacade.GetDataDicCategoryByParentId(Guid.Parse(id)).ToList();
            }
            foreach (var item in datalist)
            {
                trees.Add(new DataDictionaryTreeItem()
                {
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Code = item.Code,
                    Name = item.Name,
                    Value = item.Value,
                    Type = item.Type,
                    Order = item.Order,
                    Remark = item.Remark,
                    HasChildren = (item.Type==(int)DataDictionaryType.DataCategory?false:((item.Childs ?? new List<DataDictionaryBaseDto>()).Count > 0 ? true : false))//(item.Type < (int)DataDictionaryType.DataCategory ? true : false)
                });
            }
            return Json(trees.OrderBy(x=>x.Order).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDataDictionary(string id, string initParentId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                initParentId = id;
            }
            List<DataDictionaryTreeItem> trees = new List<DataDictionaryTreeItem>();
            var datalist = _datadicfacade.GetDataDictionaryByParentId(Guid.Parse(initParentId)).ToList();

            foreach (var item in datalist)
            {
                trees.Add(new DataDictionaryTreeItem()
                {
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Code = item.Code,
                    Name = item.Name,
                    Value = item.Value,
                    Type = item.Type,
                    Order = item.Order,
                    Remark = item.Remark,
                    HasChildren = (item.Childs.Count > 0 ? true : false)
                });
            }
            return Json(trees.OrderBy(x => x.Order).ToList(), JsonRequestBehavior.AllowGet);
        }
        #region
        public JsonResult AddDataDicCategory(DataDictionaryBaseDto datadic)
        {
            var id = _datadicfacade.AddDataDictionary(datadic);            
            datadic.Id = id;
            var treeitem = new DataDictionaryTreeItem()
            {
                Id = datadic.Id,
                ParentId = datadic.ParentId,
                Code = datadic.Code,
                Name = datadic.Name,
                Value = datadic.Value,
                Type = datadic.Type,
                Order = datadic.Order,
                Remark = datadic.Remark,
                HasChildren = false
            };
            return Json(treeitem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditDataDicCategory(DataDictionaryBaseDto datadic)
        {
            _datadicfacade.EditDataDictionary(datadic);
            DataDictionaryWithChildDto item = _datadicfacade.GetDataDictionaryById(datadic.Id);
            var treeitem = new DataDictionaryTreeItem()
            {
                Id = datadic.Id,
                ParentId = datadic.ParentId,
                Code = datadic.Code,
                Name = datadic.Name,
                Value = datadic.Value,
                Type = datadic.Type,
                Order=datadic.Order,
                Remark = datadic.Remark,
                HasChildren = (item.Type == (int)DataDictionaryType.DataCategory ? false : ((item.Childs ?? new List<DataDictionaryBaseDto>()).Count > 0 ? true : false))
            };
            return Json(treeitem, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region

        public JsonResult AddDataDictionary(DataDictionaryBaseDto datadic)
        {
            var id = _datadicfacade.AddDataDictionary(datadic);
            datadic.Id = id;
            var treeitem = new DataDictionaryTreeItem()
            {
                Id = datadic.Id,
                ParentId = datadic.ParentId,
                Code = datadic.Code,
                Name = datadic.Name,
                Value = datadic.Value,
                Type = datadic.Type,
                Order = datadic.Order,
                Remark = datadic.Remark,
                HasChildren = false
            };
            return Json(treeitem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditDataDictionary(DataDictionaryBaseDto datadic)
        {
            _datadicfacade.EditDataDictionary(datadic);
            var data = _datadicfacade.GetDataDictionaryById(datadic.Id);
            var treeitem = new DataDictionaryTreeItem()
            {
                Id = datadic.Id,
                ParentId = datadic.ParentId,
                Code = datadic.Code,
                Name = datadic.Name,
                Value = datadic.Value,
                Type = datadic.Type,
                Order = datadic.Order,
                Remark = datadic.Remark,
                HasChildren = (data.Childs.Count > 0 ? true : false)
            };
            return Json(treeitem, JsonRequestBehavior.AllowGet);
        }


        #endregion


        public JsonResult DelDataDictionary(string id)
        {
            var data = _datadicfacade.GetDataDictionaryById(Guid.Parse(id));
            if (data != null)
            {
                switch (data.Type)
                {
                    case 0:
                    case 2:
                        RecursiveDelDataDictionary(Guid.Parse(id));
                        break;
                    case 1:
                        _datadicfacade.DelDataDicByParentId(Guid.Parse(id));
                        _datadicfacade.DelDataDictionary(Guid.Parse(id));
                        break;
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExistCode(string code, string type)
        {
            bool flag = true;
            if (type == ((int)DataDictionaryType.Data).ToString())
            {
                flag = _datadicfacade.ExistCode(code, false);
            }
            else
            {
                flag = _datadicfacade.ExistCode(code, true);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataDictionaryById(string id)
        {
            DataDictionaryTreeItem treeitem = new DataDictionaryTreeItem();
            var data = _datadicfacade.GetDataDictionaryById(Guid.Parse(id));
            if (data != null)
            {

                treeitem.Id = data.Id;
                treeitem.ParentId = data.ParentId;
                treeitem.Code = data.Code;
                treeitem.Name = data.Name;
                treeitem.Value = data.Value;
                treeitem.Order = data.Order;
                treeitem.Type = data.Type;
                treeitem.Remark = data.Remark;
                if (data.Type < (int)DataDictionaryType.Data)
                {
                    treeitem.HasChildren = (data.Type < (int)DataDictionaryType.DataCategory ? true : false);
                }
                else
                {
                    treeitem.HasChildren = (data.Childs.Count > 0 ? true : false);
                }

            }
            return Json(treeitem, JsonRequestBehavior.AllowGet);
        }


        private void RecursiveDelDataDictionary(Guid id)
        {
            var data = _datadicfacade.GetDataDictionaryById(id);
            foreach (var child in data.Childs)
            {
                RecursiveDelDataDictionary(child.Id);
            }
            _datadicfacade.DelDataDictionary(id);
        }


    }
}
