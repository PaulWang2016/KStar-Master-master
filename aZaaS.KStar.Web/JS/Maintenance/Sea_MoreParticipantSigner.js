
ParticipantSet = { SetID: "00000000-0000-0000-0000-000000000000",realID:"" };
//=================================model start======================================================

var model = kendo.data.Model.define({
    id: "SetID",
    fields: {
        EntryName: { type: "string" },
        EntryType: { type: "string" },
        IsPeeked: { type: "string" },
        AssignerName: { type: "string" }
    }
});
var SignerUserModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "string" },
        SetID:{type:"string"},
        EntryID: { type: "string" },
        EntryName: { type: "string" },
        EntryType: { type: "string" }
    }
});
//==================================model end=====================================================
//===================================columns start====================================================
var datacolumns = [
        {
            title: "", width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.SetID + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    {
        field: "EntryName", title: "处理人(组)", filterable: false, template: function (item) {
           
            return item.EntryName;
        }
    },
    { field: "EntryType", title: "类型", filterable: false },
    { field: "IsPeeked", title: "状态", filterable: false },
    { field: "AssignerName", title: "加签人", filterable: false },
    { command: [{ name: "upper", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-upper'><span class='glyphicon glyphicon-arrow-up'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); UpperParticipantSet(data.SetID,data.Priority); } }], width: 58 },
    { command: [{ name: "down", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-down'><span class='glyphicon glyphicon-arrow-down'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); DownParticipantSet(data.SetID, data.Priority,data.MaxPriority); } }], width: 58 },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); editParticipantSet(data.SetID); } }], width: 58 },
    { command: [{ name: "delete", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-delete'><span class='glyphicon glyphicon-remove'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); DeleteParticipantSet(data.SetID); } }], width: 58 }
]
var SignerUsercolumns = [
       {
           title: "", width: 35, template: function (item) {
               return "<input type='checkbox' value='" + item.ID + "' />";
           }, headerTemplate: "<input type='checkbox' />", filterable: false
       },
       { field: "EntryName", title: "名称", filterable: false },
       {
           field: "EntryType", title: "类型", filterable: false
       },
       //{ command: [{ name: "delete", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-delete'><span class='glyphicon glyphicon-remove'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); DeleteParticipantSetEntry(data.ID); } }], width: 58 }
]
//================================columns end=======================================================
function Load() {
    //debugger;
    var select = GetSelectTreeItem();
    var data = { ProcessFullName: currentProcessFullName, ActivityName: select.Name };

    InitServerKendoGrid("ActivityParticipantSet", model, datacolumns, "/Maintenance/ActivityParticipants/GetActivityParticipantsSet", data, 800, "", "", "");

}
var participant = new Object();
participant.Add = "/Maintenance/ActivityParticipants/SaveParticipantSetAndEntry";
participant.Update = "/Maintenance/ActivityParticipants/GetActivityParticipantsSet";
participant.Upper = "/Maintenance/ActivityParticipants/UpperParticipantPorioty";
participant.Down = "/Maintenance/ActivityParticipants/DownParticipantPorioty";
participant.Delete = "/Maintenance/ActivityParticipants/DeleteParticipantSet";

participant.Entry = new Object();
participant.Entry.GetEntry = "/Maintenance/ActivityParticipants/GetParticipantSetEntry";
participant.Entry.DelEntry = "/Maintenance/ActivityParticipants/DeleteParticipatnEntry";


$(function () {
    //btnAddUser
    //NewSigner
    $("#btnAddUser").click(function () {
        SelectApprovers.apply(this);
    });
    LoadParticipantEntry();
});
function LoadParticipantEntry(SetID)
{
    SetID = SetID == undefined ? "" : SetID;
    var params = { guid: SetID ,t:new Date()};
    //InitServerKendoGrid("SingerUserList", SignerUserModel, SignerUsercolumns, participant.Entry.GetEntry, params, 800, "", "", "");
    $getJSON(participant.Entry.GetEntry, params, function (data) { 
        if (SetID == "") $("#txtRemark").val("");
        if (data.set != null) {
            $("#txtRemark").val(data.set.Remark);
        }
        InitBaseKendoGrid("SingerUserList", SignerUserModel, SignerUsercolumns, data.data, function () {
            bindGridCheckbox("SingerUserList")
        });
    })
    
   
}
function DownParticipantSet(guid, p, mp) {
    if (p == mp) return;
    var data = { SetID: guid };
    
    $.ajax({
        url: participant.Down,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {
            debugger;
            
            Load();
            ShowTip(item);

        },
        dataType: "json"
    }).fail(function (e) {
    })
}
function UpperParticipantSet(guid, p) {
    if (p == "1") return;
    var data = { SetID: guid };
    $.ajax({
        url: participant.Upper,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {
            Load();
            ShowTip(item);

        },
        dataType: "json"
    }).fail(function (e) {
    })
}
function DeleteParticipantSet(guid) {
    KStar.Modaldialog.confirm({ msg: "你确定要删除吗？" }).on(function (e) {
        if (e) {
            var data = { SetID: guid };
            $.ajax({
                url: participant.Delete,
                type: "POST",
                data: data,
                traditional: true,
                success: function (item) {
                    Load();
                    ShowTip(item);

                },
                dataType: "json"
            }).fail(function (e) {
               
            });
        }
    });
}
function editParticipantSet(guid) {
   // debugger;
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
    ParticipantSet.SetID = guid;
    ParticipantSet.realID = guid;
    LoadParticipantEntry(guid);
    $("#AddStaffWindow").data("kendoWindow").center().title("加签编辑器").open();
}
function DeleteParticipantSetEntry(ID)
{
    KStar.Modaldialog.confirm({ msg: "你确定要删除吗？" }).on(function (e) {
        if (e) {
            var data = { ID: ID };
            $.ajax({
                url: participant.Entry.DelEntry,
                type: "POST",
                data: data,
                traditional: true,
                success: function (item) {
                    LoadParticipantEntry();
                    ShowTip(item);

                },
                dataType: "json"
            }).fail(function (e) {

            });
        }
    });
}
function SaveData()
{
    //debugger;
 var select = GetSelectTreeItem();

    var array = $("#SingerUserList").data("kendoGrid").dataSource._data;
    var json = JSON.stringify(array);
    var data = {
        SetID:"00000000-0000-0000-0000-000000000000",
        Assigner: KStar.User.SysID,
        AssignerName: KStar.User.FirstName,
        Setter: "",
        SetterName: "",
        Priority:0,
        ProcInstID: 0,
        ProcessFullName: currentProcessFullName,
        ActivityID: select.ActivityID,
        ActivityName: select.Name,
        IsPeeked: 0,
        IsOriginal: 0,
        SkipAssigner: 0,
        SkipSet: $("SkipSigner").checked,
        DateAssigned:null,
        Remark: $("#txtRemark").val()
    };
    var str = JSON.stringify(data);
    var paras = { classString: str, userListString: json, SetID: ParticipantSet.realID };
    $.ajax({
        url: participant.Add,
        type: "POST",
        data: paras,
        traditional: true,
        success: function (item) {
            KStar.Modaldialog.alert({ msg: item }).on(function () {
                Load();
                resetAddStaffWindow()
                $("#AddStaffWindow").data("kendoWindow").close();
            })
           
           // ShowTip(item);
        },
        dataType: "json"
    }).fail(function (e) {
    })
    //LoadParticipantEntry();

}
function SelectApprovers()
{
    var own = $(this);
    var griddiv = $("#SingerUserList");
    var data = $(griddiv).data("kendoGrid").dataSource._data;
    InitSelectPersonWindow(this, "All", function (json) {
        debugger;
        var userlist = json.Root.Users.Item;
        $.each(userlist, function (i, n) {
            if (!ExistsSelectPerson(n, data)) {
                $(griddiv).data("kendoGrid").dataSource.add({ ID: 0, SetID:ParticipantSet.SetID, EntryID: n.Value, EntryType: "User", EntryName: n.Name })
            }
        });

        var deptlist = json.Root.Depts.Item;
        $.each(deptlist, function (i, n) {
            if (!ExistsSelectPerson(n, data)) {
                $(griddiv).data("kendoGrid").dataSource.add({ ID: 0, SetID: ParticipantSet.SetID, EntryName: n.Name, EntryType: "OrgNode", EntryID: n.Value });
            }
        });

        var positionlist = json.Root.Positions.Item;
        $.each(positionlist, function (i, n) {
            if (!ExistsSelectPerson(n, data)) {
                $(griddiv).data("kendoGrid").dataSource.add({ ID: 0, SetID: ParticipantSet.SetID, EntryName: n.Name, EntryType: "Position", EntryID: n.Value });
            }
        });

        var custiomlist = json.Root.CustomRoles.Item;
        $.each(custiomlist, function (i, n) {
            if (!ExistsSelectPerson(n, data)) {
                $(griddiv).data("kendoGrid").dataSource.add({ ID: 0, SetID: ParticipantSet.SetID, EntryName: n.Name, EntryType: "CustomType", EntryID: n.Value });
            }
        });

        var roleslist = json.Root.SystemRoles.Item;
        $.each(roleslist, function (i, n) {
            if (!ExistsSelectPerson(n, data)) {
                $(griddiv).data("kendoGrid").dataSource.add({ ID: 0, SetID: ParticipantSet.SetID, EntryName: n.Name, EntryType: "Role", EntryID: n.Value });
            }
        });
    })
}

function resetAddStaffWindow() {
    hideOperaMask("AddStaffWindow");
    $("#staffTab .k-textbox").val("");//清除输入框

    //$("#txtProcessNames").data("kendoDropDownList").select(0);//清除下拉框
}
$("#SignerDeletefrom").click(function () {
    var users = $("#SingerUserList .k-grid-content").find(":checked");
    if (users.length == 0) {
        ShowTip("请选择林删除的记录");
        return;
    }
    bootbox.confirm("确定要删除吗？", function (result) {
        if (result) {
            $("#SingerUserList .k-grid-content").find(":checked").each(function () {
                var item = $("#SingerUserList").data("kendoGrid").dataSource.get(this.value);
                $("#SingerUserList").data("kendoGrid").dataSource.remove(item);
            })
        }
    });
}); 
function InitServerKendoGrid(target, viewModel, columns, url, parameterdata, height, callBack) {
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
            data: "data",
            total: "total"
        },
        pageSize: 20,

        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            //groupable: {
            //    messages: {
            //        empty: jsResxbaseInitView.Dropcolumnshere
            //    }
            //},
            selectable: false,
            sortable: true,
            //scrollable: false,
        
            reorderable: true,
            resizable: true,
            columns: columns,
            toolbar: kendo.template($("#template").html()),
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
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
function InitBaseKendoGrid(target, viewModel, columns, items, pageSize, callBack) {
    //columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
           
            pageable: {
                refresh: false
            },
            columns: columns,
            dataBound: function () {
                //refreshCurrentScrolls();
                HideGridVerticalScroll(target);
            }
        });
        grid = $("#" + target).data("kendoGrid");
        GridHeaderAppendDiv(target);
        HideGridVerticalScroll(target);

        //AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
/*
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
            },*/