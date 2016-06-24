
//define(function (require, exports, module) {
var CommonReportColumns;
var CommonReportModel;
var ProcSetID;
var strWhere;
var chType;
var CurrentUser = KStar.User.UserName;
var Status;
//var arrayObj = new Array();
var dataParam = { UserName: KStar.User.SysID };
$.ajax({
    url: "/Report/CommonReport/GetAllProcessListByUserName",data:dataParam, async: true, dataType: "json", success: function (items) {
        InitDropDownList("sltProcess", "ProcessName", "ProcessSetID", "--请选择流程--", items, "onChange");
    }
});
var url = function () {
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
function GenerateSearchCondition() {
    //debugger;
    var items = $(".section input");
    strWhere = "";
    strWhere += "Folio|" + $("#txtFolio").val() + ",";
    strWhere += "StartName|" + $("#txtOriginator").val() + ",";
    strWhere += "StartDate|" + $("#ProcStartDate").val() + ",";
    strWhere += "EndDate|" + $("#ProcEndDate").val() + ",";
    for (var i = 5; i < items.length; i++) {
        var id = $(items[i]).attr("id");
        if (id != "btnQuery") {
            strWhere += id + "|" + $("#" + id).val() + ",";
        }
    }
    Init();
}
function GenerateSearchArea() {
    $.ajax({
        url: "/Report/CommonReport/GetCommonReportConfig", data: { ProcSetID: ProcSetID }, async: true, dataType: "json", success: function (data) {
            //debugger;
            items = eval("(" + data + ")");
            if (items.length == 0) return;
            var searcrArea = "";
            for (var i = 0; i < items.Table.length; i++) {
                if ((i + 1) % 4 == 1) {
                    searcrArea += " <div class=\"toolbar\" style=\"min-width:900px; float:left !important; width:100%\">";
                }
                searcrArea += " <label class=\"category-label\" for=\"Folio\" style=\"width: 8%;\">" + items.Table[i].FieldName + "：</label>";
                if (items.Table[i].FieldType == "文本")
                    searcrArea += "<input type=\"text\" id=" + items.Table[i].FieldID + " class=\"k-textbox\" style=\"width: 14%; margin-right:25px;\" />";
                if (items.Table[i].FieldType == "时间") {
                    searcrArea + "<input type=\"text\" id=" + items.Table[i].FieldID + "  style=\"width: 14%;margin-right:20px;\" data-date-format=\"yyyy-mm-dd\" />";
                }
                if ((i + 1) % 4 == 0) {
                    searcrArea += " </div>";
                }
            }

            $("#CommonReport_Context").html(searcrArea);
        }
    });
}
function Init() {

    $.ajax({
        url: "/Report/CommonReport/GetCommonReportConfig", data: { ProcSetID: ProcSetID }, async: true, dataType: "json", success: function (data) {
            //debugger;
            chType = $("#sltType").val();
            items = eval("(" + data + ")");
            //if (items.length == 0) return;
            var model = "";
            model = "{ID:'ProcInstID',fields:{FlowNo:{type:'string'},";
            model += "SUBJECT:{type:'string'},"
            model += "StartName:{type:'string'},"
            model += "Destination:{type:'string'},"
            model += "StartDate:{type:'string'},"
            model += "ViewUrl:{type:'string'},"
            model += "ViewFlowUrl:{type:'string'}"
            var arrayObj = [];

            arrayObj.push({
                field: "IsReaded", title: "", width: 30, template: function (item) {
                    return "<a href='" + item.ViewFlowUrl + "' target='_blank'><img src='/images/ViewFlow.gif' /></a>";
                }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
            });
            arrayObj.push({
                field: "FlowNo", title: "流程实例编号", width: 80, filterable: false, sortable: false, template: function (item) {
                    return "<a href='" + item.ViewUrl + "' target='_blank' class='Folio' >" + item.FlowNo + "</a>";
                }
            });
            arrayObj.push({ field: "SUBJECT", title: "流程主题" });
            arrayObj.push({ field: "StartName", title: "发起人" });
            arrayObj.push({ field: "StartDate", title: "发起时间" });
            arrayObj.push({ field: "Destination",title:"当前审批人" })
            arrayObj.push({ field: "Status", title: "状态" });
            //if (items.length > 0) {
            for (var i = 0; i < items.Table1.length; i++) {
                if ((i + 1) == items.Table1.length) {
                    model += "," + items.Table1[i].FieldID + ":{type:'string'}";
                }
                else {
                    model += "," + items.Table1[i].FieldID + ":{type:'string'}";
                }
                if (items.Table1[i].FieldType == "时间")
                {
                    arrayObj.push({ field: "" + items.Table1[i].FieldID + "", title: "" + items.Table1[i].FieldName + "" });
                }
                else {
                    arrayObj.push({ field: "" + items.Table1[i].FieldID + "", title: "" + items.Table1[i].FieldName + "" });
                }
            }
            //}
            model += "}}";
            //debugger;
            var reportModel = eval("(" + model + ")");
            CommonReportModel = kendo.data.Model.define(reportModel);
            InitServerCustomKendoExcelGridForCommonReport("CommonReport_TableList", CommonReportModel, arrayObj, "/Report/CommonReport/Find", {
                ProcSetID: ProcSetID,
                strWhere: strWhere,
                CurrentUser: CurrentUser,
                SysID: CurrentSysID,
                Status: Status
            }, 1200, "流程通用报表", url,
               function () {
                   bindAndLoad("CommonReport_TableList");
               });
        }
    });
}
function LoadData() {
    if (ProcSetID != undefined && ProcSetID != null && ProcSetID != "") {
        GenerateSearchArea();
        Init();
    }
}

$("#btnQuery").bind("click", function () {
    GenerateSearchCondition();
    Status = $("#ProcessStatus").val();
    // debugger;

});
$("#btnReset").bind("click", function () {
    var items = $(".section input");
    for (var i = 0; i < items.length; i++) {
        var id = $(items[i]).attr("id");
        if (id != "btnQuery" && id != "btnReset") {
            $(items[i]).val("");
        }
    }
});
var dTypeSource = [
            { text: "全部", value: "" },
            { text: "进行中", value: "2" },
            { text: "已完成", value: "3" },
            { text: "作废", value: "9" }
];
$("#ProcessStatus").kendoDropDownList({
    dataTextField: 'text',
    dataValueField: 'value',
    dataSource: dTypeSource
    //change: TypeChange
});
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
function TypeChange() {
    chType = $("#sltType").val();

    CurrentUser = KStar.User.UserName;

    CurrentSysID = KStar.User.SysID;

    Init();
}
//module.exports = LoadData;
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
            data: function (d) { return eval('(' + d.data + ')'); },
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

