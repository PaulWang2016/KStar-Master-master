using aZaaS.KStar;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class WorklistController : BaseMvcController
    {
        BusinessDataBO _businessdataBO = new BusinessDataBO();

        //
        // GET: /Maintenance/Worklist/

        public JsonResult GetWorklist()
        {
            IEnumerable<BDConfigView> items = _businessdataBO.GetAllConfigs().Select(s => new BDConfigView()
                {
                    WorklistID = s.SysID.ToString(),
                    ApplicationName = s.ApplicationName,
                    ProcessName = s.ProcessName,
                    ConnectionString = s.DbConnectionString,
                    DataTable = s.DataTable,
                    WhereQuery = s.WhereQuery
                });

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkByProcessName(string processname)
        {
            BusinessDataConfigDTO bdcf = _businessdataBO.ReadConfig(processname);
            BDConfigView item = new BDConfigView();
            if(bdcf!=null)
            {            
               item.WorklistID = bdcf.SysID.ToString();
               item.ApplicationName = bdcf.ApplicationName;
               item.ProcessName = bdcf.ProcessName;
               item.ConnectionString = bdcf.DbConnectionString;
               item.DataTable = bdcf.DataTable;
               item.WhereQuery = bdcf.WhereQuery;
           }
           return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorklistData(Guid configId)
        {
            IEnumerable<BDColumnView> items = _businessdataBO.GetConfigColumns(configId).Select(s => new BDColumnView()
                {
                    WorklistDataID = s.SysID.ToString(),
                    WorklistID = s.Config.SysID.ToString(),
                    ColumnName = s.ColumnName,
                    DisplayName = s.DisplayName,
                    Description = s.Description,
                    IsVisible = s.IsVisible,
                    ValueType = s.ValueType
                });

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchFields(Guid? configId, string table, string connectionString)
        {
            var items = _businessdataBO.FetchConfigFields(table, connectionString).Select(s => new BusinessDataColumnDTO() { ColumnName = s.Field, DisplayName = s.Field }).ToList();

            IEnumerable<BusinessDataColumnDTO> columns;
            if (configId != null)
            {
                columns = _businessdataBO.GetConfigColumns(configId.Value);

                foreach (var column in columns)
                {
                    var field = items.SingleOrDefault(s => s.ColumnName == column.ColumnName);
                    if (field != null)
                    {
                        field.DisplayName = column.DisplayName;
                        field.IsVisible = true;//暂时标记 勾选
                        field.Description = column.Description;
                        field.ValueType = column.ValueType;
                    }
                }
            }

            return Json(items.Select(s => new { Field = s.ColumnName, DisplayName = s.DisplayName, ValueType=s.ValueType, IsChecked = s.IsVisible, Description = s.Description }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult FetchFieldsById(Guid configId)
        //{
        //    var items = _businessdataBO.FetchConfigFields(configId);

        //    return Json(items, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult AddWorklist(BDConfigView item, bool isFetch, [ModelBinder(typeof(JsonListBinder<BusinessDataColumnDTO>))]List<BusinessDataColumnDTO> columns)
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
            if (isFetch)
            {
                foreach (var column in columns)
                {
                    column.SysID = Guid.NewGuid();
                    column.IsVisible = true;
                    column.ValueType = "Text";

                    _businessdataBO.AppendColumn(Guid.Parse(item.WorklistID), column);
                }
            }

            return Json(item);
        }
        public JsonResult EditWorklist(BDConfigView item, bool isFetch, [ModelBinder(typeof(JsonListBinder<BusinessDataColumnDTO>))]List<BusinessDataColumnDTO> columns)
        {
            var config = new BusinessDataConfigDTO()
            {
                SysID = Guid.Parse(item.WorklistID),
                ApplicationName = item.ApplicationName,
                ProcessName = item.ProcessName,
                DbConnectionString = item.ConnectionString,
                DataTable = item.DataTable,
                WhereQuery = item.WhereQuery
            };

            _businessdataBO.UpdateConfig(config);

            if (isFetch)
            {
                IList<BusinessDataColumnDTO> oldcolumns = _businessdataBO.GetConfigColumns(config.SysID).ToList();

                foreach (var column in columns)
                {
                    var oldcolumn = oldcolumns.SingleOrDefault(s => s.ColumnName == column.ColumnName);
                    if (null == oldcolumn)
                    {
                        column.SysID = Guid.NewGuid();
                        column.IsVisible = true;
                        column.ValueType = "Text";
                        //if (_businessdataBO.ConfigColumnExists(config.Id, column.DisplayName))
                        //    continue;//重复  跳过当前
                        _businessdataBO.AppendColumn(config.SysID, column);
                    }
                    else
                    {
                        //if (column.ColumnName != column.DisplayName)
                        //{
                        oldcolumn.DisplayName = column.DisplayName;
                        oldcolumn.Description = column.Description;
                        oldcolumn.ValueType = column.ValueType;
                        //if (_businessdataBO.ConfigColumnExists(config.Id, column.DisplayName))
                        //    continue;//重复  跳过当前
                        _businessdataBO.UpdateColumn(oldcolumn);
                        oldcolumns.Remove(oldcolumn);
                        //}
                    }
                }

                foreach (var oldcolumn in oldcolumns)
                {
                    _businessdataBO.RemoveColumn(oldcolumn.SysID);
                }
            }



            return Json(item);
        }

        public JsonResult DelWorklist(List<Guid> idList)
        {
            _businessdataBO.RemoveConfig(idList);

            return Json(idList);
        }

        public JsonResult AddWorklistData(BDColumnView item)
        {
            var column = new BusinessDataColumnDTO()
            {
                SysID = Guid.NewGuid(),
                ColumnName = item.ColumnName,
                DisplayName = item.DisplayName,
                Description = item.Description,
                IsVisible = item.IsVisible,
                ValueType = item.ValueType
            };

            _businessdataBO.AppendColumn(Guid.Parse(item.WorklistID), column);
            item.WorklistDataID = column.SysID.ToString();

            return Json(item);
        }

        public JsonResult EditWorklistData(BDColumnView item)
        {
            var column = new BusinessDataColumnDTO()
            {
                SysID = Guid.Parse(item.WorklistDataID),
                ColumnName = item.ColumnName,
                DisplayName = item.DisplayName,
                Description = item.Description,
                IsVisible = item.IsVisible,
                ValueType = item.ValueType
            };

            //TODO:Validate duplex column name ?

            //if (_businessdataBO.ConfigColumnExists(Guid.Parse(item.WorklistID), column.DisplayName))
            //    throw new InvalidOperationException("the specified column is already exists.");
            //else
            //{
            _businessdataBO.UpdateColumn(column);

            return Json(item);
            //}
        }

        public JsonResult DelWorklistData(List<Guid> idList)
        {
            _businessdataBO.RemoveColumn(idList);
            return Json(idList);
        }

    }
}
