//define(function (require, exports, module) {
//var InitProcessSetList = function ()
//{
//    $.getJSON("/Maintenance/Process/GetProcess", { key: "", _t: new Date() }, function (items) {

//    });
//}
$.ajax({
    url: "/Report/CommonReport/GetAllProcessList", async: true, dataType: "json", success: function (items) {
        InitDropDownList("sltProcess", "ProcessName", "ProcessSetID", "--请选择流程--", items, "onChange");
        InitDropDownListEx("txtProcessNames", "ProcessName", "ProcessSetID", "--请选择流程--", items, "");
    }
});
function InitDropDownListEx(target, dataText, dataValue, optionLabel, items, Callback) {
    $("#" + target).kendoDropDownList({
        dataTextField: dataText,
        dataValueField: dataValue,
        dataSource: {
            data: items,
            schema: {
                model: {
                    id: dataValue,
                    fields: {
                        dataValue: { type: "String" },
                        dataText: { type: "String" }
                    }
                }
            }
        },
        //change: Callback,
        optionLabel: optionLabel
    });
}
function InitDropDownList(target, dataText, dataValue, optionLabel, items, Callback) {
    $("#" + target).kendoDropDownList({
        dataTextField: dataText,
        dataValueField: dataValue,
        dataSource: {
            data: items,
            schema: {
                model: {
                    id: dataValue,
                    fields: {
                        dataValue: { type: "String" },
                        dataText: { type: "String" }
                    }
                }
            }
        },
        change: onChange,
        optionLabel: optionLabel
    });
}

function onChange() {
    ProcSetID = $("#sltProcess").val();
    Status = $("#ProcessStatus").val();
    CurrentUser = KStar.User.UserName;

    CurrentSysID = KStar.User.SysID;
    arrayObj = [];
    LoadData();
};
var strUrl = function () {
    // GenerateSearchCondition();
    data = {
        ProcSetID: ProcSetID,
        strWhere: strWhere,
        CurrentUser: CurrentUser,
        SysID: CurrentSysID,
        Status: Status
    };
    return "/Report/CommonReport/GetAllRecord?" + SerializeJsonObject(data);
}
function LoadData() {
    if (ProcSetID != undefined && ProcSetID != null && ProcSetID != "") {
        //GenerateSearchArea();
        Init();
    }
}
function Init() {
    debugger;
    var columns = [
          {
              title: "", width: 35, template: function (item) {
                  return "<input value='" + item.ID + "' type='checkbox' />";
              }, headerTemplate: "<input type='checkbox' />", filterable: false
          },
        { field: "FieldID", title: "字段ID", filterable: false },
        { field: "FieldName", title: "字段名称", filterable: false },
        { field: "FieldType", title: "字段类型", filterable: false },
        { field: "DataResource", title: "数据来源", filterable: false },
        { field: "XPATH", title: "XPATH", filterable: false },
        { field: "Memo", title: "备注", filterable: false }
                   
    ]
   
    columns.push({ command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditReportDisplay(data.ID) } }], width: 58 });
    
        var model = kendo.data.Model.define({
        id: "ID",
        fields: {
            ID: { type: "string" },
            FieldID: { type: "string" },
            FieldName: { type: "string" },
            FieldType: { type: "string" },
            DataResource: { type: "string" },
            XPATH: { type: "string" },
            Memo: { type: "string" }
        }
    });
        InitServerCustomKendoExcelGridForCommonReport("CommonReport_TableList", model, columns, "/Maintenance/CommonReportMait/FindSearch", {
        ProcessSetID: ProcSetID

    }, 1200, "流程通用报表配置-查询", strUrl,
           function () {
               bindAndLoad("CommonReport_TableList");
               $("#CommonReport_TableList .k-toolbar")                
              .append("<a id='UserDelete' class='more k-button' href='javascript:void(0)'><span class='glyphicon glyphicon-remove'></span></a>")
              .append(" <a id='UserAdd'  class='more k-button'  href='javascript:void(0)'><span class='glyphicon glyphicon-plus'></span></a>");
               $("#UserAdd").click(AddReport);
               $("#UserDelete").click(DeleUsers);
           });

}
var AddReport = function ()
{
    $("#AddStaffWindow").kendoWindow({
        title: "Title",
        width: 900,
        height: 450,
        actions: [
            "Pin",
            "Minimize",
            "Maximize",
            "Close"
        ],
        modal: true
    });
    $("#AddStaffWindow").data("kendoWindow").center().title("通用报表维护-查询").open();
}
var EditReportDisplay = function (id)
{
    if (id != undefined) {
        
        $("#AddStaffWindow").kendoWindow({
            title: "Title",
            width: 900,
            height: 450,
            actions: [
                "Pin",
                "Minimize",
                "Maximize",
                "Close"
            ],
            modal: true
        });
        debugger;
        var model = getKendoGrid("CommonReport_TableList").dataSource.get(id);
        $("#AddStaffWindow .windowConfirm").attr("data-id", model.ID);
        var ddl = $("#txtProcessNames").data("kendoDropDownList");
        ddl.value(model.ProcSetID);
            //$("#txtProcessNames").val(model.ProcSetID);
            $("#txtFieldIDs").val(model.FieldID);
            $("#txtFieldNames").val(model.FieldName);
            $("#txtFieldTypes").val(model.FieldType);
            $("#txtDataSources").val(model.DataResource);
            $("#txtXpaths").val(model.XPATH);
            $("#txtMemos").val(model.Memo);
            $("#AddStaffWindow .windowConfirm").attr("data-url", "/Maintenance/CommonReportMait/EditReportSearch");
            $("#AddStaffWindow").data("kendoWindow").center().title("通用报表维护-查询").open();
            $("#staffTab").data("kendoValidator").hideMessages();
        }
}
var DeleUsers = function () {
    var idList = new Array();
    $("#CommonReport_TableList .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm("你确定删除吗？", function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/CommonReportMait/DeleteSearch",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            //for (var i = 0; i < idList.length; i++) {
                            //    var dataItem = getKendoGrid("UserManaView").dataSource.get(idList[i]);
                            //    getKendoGrid("UserManaView").dataSource.remove(dataItem);
                            //}
                            ShowTip("删除成功!");
                            getKendoGrid("CommonReport_TableList").dataSource.read();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }
    else {
        ShowTip("你没有选择任何行!");
    }
}
var AddCommonReportConfig = function () {
    debugger;
    //$("#AddStaffWindow .windowConfirm").attr("data-url", "/Maintenance/Staff/AddStaff");
    var validator = $("#staffTab").data("kendoValidator");
    if (validator.validate()) {
        
        var url = $("#AddStaffWindow .windowConfirm").attr("data-url");
        if (url == null || url == undefined || url == "")
        {
            url = "/Maintenance/CommonReportMait/AddSearch";
        }
        //ProcSetID = $("#sltProcess").val();
        var data = {
            ID:$("#AddStaffWindow .windowConfirm").attr("data-id"),
            ProcSetID: $("#txtProcessNames").val(),
            FieldID: $("#txtFieldIDs").val(),
            FieldName: $("#txtFieldNames").val(),
            FieldType: $("#txtFieldTypes").val(),
            DataResource: $("#txtDataSources").val(),
            XPATH: $("#txtXpaths").val(),
            Memo: $("#txtMemos").val()
        }
        $.ajax({
            url: url,
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {
                debugger;
                 //KStar.Modaldialog.alert({ msg: "添加成功！" })
                //alert("添加成功");
                resetAddStaffWindow();
                ShowTip(item);
                $("#AddStaffWindow").data("kendoWindow").close()
                getKendoGrid("CommonReport_TableList").dataSource.read();
            },
            dataType: "json"
        }).fail(function (e) {
            debugger;
            // hideOperaMask("AddStaffWindow");
            //that.bind("click", StaffConfirm);
        })
    }
}
function resetAddStaffWindow() {
    hideOperaMask("AddStaffWindow");
    $("#staffTab .k-textbox").val("");//清除输入框

    $("#txtProcessNames").data("kendoDropDownList").select(0);//清除下拉框
    //$("#StaffStatus").data("kendoDropDownList").select(0);
   // $("#StaffStatus").data("kendoDropDownList").value("true");
}
function InitServerCustomKendoExcelGridForCommonReport(target, viewModel, columns, url, parameterdata, height, title, downloadurl, callBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var filter = [];
                    if (data.filter != undefined) {
                        var temp = data.filter.filters;
                        for (var index in temp) {
                            filter.push(obj2str({ Field: temp[index].field, Operator: temp[index].operator, Value: temp[index].value }));
                        }
                    }
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize,
                        filter: "[" + filter + "]"
                    }
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: function (d) {
                //debugger;
                return d.data;
            },
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: false//,
        //serverGrouping: true
    });
    if (grid) {
        grid.destroy();
        //debugger
        $("#" + target).replaceWith($("<div class=\"sectionGrid\" id=\"" + target + "\"></div>"));
    }
    $("#" + target).kendoExcelGrid({
        dataSource: dataSource,
        groupable: {
            messages: {
                empty: jsResxbaseInitView.Dropcolumnshere
            }
        },
        selectable: false,
        sortable: false,
        //scrollable: false,
        columnMenu: {
            messages: {
                sortAscending: jsResxbaseInitView.Sortasc,
                sortDescending: jsResxbaseInitView.Sortdesc,
                columns: jsResxbaseInitView.Choosecolumns,
                filter: jsResxbaseInitView.Filter,
            }
        },
        pageable: {
            pageSizes: true,
            messages: {
                itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                display: jsResxbaseInitView.datadisplay,
                empty: jsResxbaseInitView.Noitemstodisplay
            }
        },
        filterable: {
            extra: false,
            messages: {
                info: jsResxbaseInitView.Showitemswithvaluethat,
                clear: jsResxbaseInitView.Clear,
                filter: jsResxbaseInitView.Filter
            },
            operators: {
                string: {
                    eq: jsResxbaseInitView.Isequalto,
                    neq: jsResxbaseInitView.Isnotequalto,
                    startswith: jsResxbaseInitView.Startswith,
                    contains: jsResxbaseInitView.Contains,
                    doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                    endswith: jsResxbaseInitView.Endswith
                },
            }
        },
        reorderable: true,
        resizable: true,
        columns: columns,
        "export": {
            allowExport: true,
            cssClass: "excel-ico",//"glyphicon glyphicon-export",
            title: title,
            createUrl: "/Export/ToExcel",
            downloadUrl: "/Export/Get",
            createCSVUrl: "/Export/ToCSV",
            downloadSCVUrl: "/Export/GetCSV",
            isDownloadFromServer: true,
            downloadFromServerUrl: downloadurl
        },
        dataBound: function () {
            refreshCurrentScrolls();//数据绑定完成  刷新滚动条
            //if (dataSource.data().length == dataSource.pageSize()) {
            //    HideGridVerticalScroll(target);//隐藏Scroll
            //}
            $("#" + target + " .k-grid-content").css("height", "auto");
        },
        height: "auto"
    });
    grid = $("#" + target).data("kendoExcelGrid");
    $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
    var limitheight = 20;
    $("#" + target + " .k-grid-content").siblings().each(function () {
        limitheight += $(this).height();
    })
    $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
    GridHeaderAppendDiv(target);
    //HideGridVerticalScroll(target);
    AddSplitters(grid);
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
//})