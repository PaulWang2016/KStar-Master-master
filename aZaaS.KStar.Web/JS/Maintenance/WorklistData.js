var WorklistDataModel = kendo.data.Model.define({
    id: "WorklistDataID",
    fields: {
        WorklistDataID: { type: "string" },
        ColumnName: { type: "string" },
        DisplayName: { type: "string" },
        Description: { type: "string" },
        ValueType: { type: "string" },
        IsVisible: { type: "string" },
        WorklistID: { type: "string" }
    }
});
var worklistdatacolumns = [
        {
            title: "Checked", width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.WorklistDataID + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    { field: "ColumnName", title: "Column Name", filterable: false },
    { field: "DisplayName", title: "Display Name", filterable: false },
    { field: "Description", title: "Description", filterable: false },
    { field: "ValueType", title: "Value Type", filterable: false },
    { field: "IsVisible", title: "Is Visible", template: function (item) { return item.IsVisible == "true" ? "Yes" : "No" }, filterable: false },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditWData(data.WorklistDataID) } }], width: 58 }
]
var AddWData = function () {
    CreateAddWorklistDataWindow();



    $("#AddWorklistDataWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Worklist/AddWorklistData").attr("data-id", "");

    $("#AddWorklistDataWindow").data("kendoWindow").title("Add WorklistData Config").center().open();
}
var EditWData = function (id) {
    CreateAddWorklistDataWindow();
    if (id) {
        var item = getKendoGrid("WorklistDataView").dataSource.get(id);
        $("#AddWorklistDataWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Worklist/EditWorklistData").attr("data-id", item.WorklistDataID);

        $("#AddWorklistDataWindow .ColumnName").val(item.ColumnName);
        $("#AddWorklistDataWindow .DisplayName").val(item.DisplayName);
        $("#AddWorklistDataWindow .Description").val(item.Description);
        $("#ColumnValueType").data("kendoDropDownList").search(item.ValueType);
        $("#ColumnIsVisible").data("kendoDropDownList").value(item.IsVisible);

        $("#AddWorklistDataWindow").data("kendoWindow").title("Edit WorklistData Config").center().open();
    }
    else {
        ShowTip("Please select worklist!");
    }
}
var DelWData = function () {
    var idList = new Array();
    $("#WorklistDataView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Worklist/DelWorklistData",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (ids) {
                        var grid = getKendoGrid("WorklistDataView");
                        for (var i = 0; i < ids.length; i++) {
                            var item = grid.dataSource.get(ids[i])
                            grid.dataSource.remove(item);
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }
    else {
        ShowTip("Please select worklistdata!");
    }
}

var resetAddWorklistDataWindow = function () {
    hideOperaMask("AddWorklistDataWindow");
    $("#AddWorklistDataWindow .k-textbox").val("");//清除输入框

    $("#ColumnValueType").data("kendoDropDownList").select(0)
    $("#ColumnIsVisible").data("kendoDropDownList").select(0)
}
var CreateAddWorklistDataWindow = function () {
    var AddWorklistDataWindow = $("#AddWorklistDataWindow").data("kendoWindow");
    if (!AddWorklistDataWindow) {
        $("#AddWorklistDataWindow").kendoWindow({
            width: "850px",
            title: "Add WorklistData Config",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddWorklistDataWindow .windowCancel").bind("click", WorklistDataCancel)
                $("#AddWorklistDataWindow .windowConfirm").bind("click", WorklistDataConfirm)
            },
            close: function (e) {
                resetAddWorklistDataWindow();
                $("#AddWorklistDataWindow .windowCancel").unbind("click", WorklistDataCancel)
                $("#AddWorklistDataWindow .windowConfirm").unbind("click", WorklistDataConfirm)
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddWorklistDataWindow").data("kendoWindow"));

    }
}
var WorklistDataCancel = function () {
    $("#AddWorklistDataWindow").data("kendoWindow").close()
}
var WorklistDataConfirm = function () {
    var that = $(this);
    that.unbind("click", WorklistDataConfirm);
    showOperaMask("AddWorklistDataWindow");
    var url = that.attr("data-url");
    var data = {
        WorklistDataID: that.attr("data-id"),
        ColumnName: $("#AddWorklistDataWindow .ColumnName").val(),
        DisplayName: $("#AddWorklistDataWindow .DisplayName").val(),
        Description: $("#AddWorklistDataWindow .Description").val(),
        ValueType: $("#ColumnValueType").data("kendoDropDownList").text(),
        IsVisible: $("#ColumnIsVisible").data("kendoDropDownList").value(),
        WorklistID: that.attr("data-worklistid")
    }
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {
            var grid = getKendoGrid("WorklistDataView");
            var model = grid.dataSource.get(item.WorklistDataID);
            if (model) {
                for (var key in item) {
                    model.set(key, item[key]);
                }
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddWorklistDataWindow").data("kendoWindow").close();
        }
    }).fail(function () {
        that.bind("click", WorklistDataConfirm);
        hideOperaMask("AddWorklistDataWindow");
    })
}

function LoadWorklistDataView(key) {
    title = "Worklist Column - Kendo UI";
    $("#AddWorklistDataWindow .windowConfirm").attr("data-worklistid", key);
    $.getJSON("/Maintenance/Worklist/GetWorklistData", { _t: new Date(), configId: key }, function (items) {
        InitKendoExcelGrid("WorklistDataView", WorklistDataModel, worklistdatacolumns, items, 20, "Worklist Column Configuration", function () {//
            bindAndLoad("WorklistDataView");
            bindGridCheckbox("WorklistDataView");
            $("#WorklistDataView .k-toolbar a.k-grid-export").remove();
            $("#WorklistDataView .k-toolbar")
                .append("<a id='WDataDel' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                .append("<a id='WDataAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#WDataAdd").click(AddWData);
            $("#WDataDel").click(DelWData);
        });
    })
    $("#ColumnValueType").kendoDropDownList({
        dataSource: ["Text", "Number", "DateTime", "Xml"]
    });
    $("#ColumnIsVisible").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: {
            data: [{ Value: "true", Text: "Yes" }, { Value: "false", Text: "No" }]
        }
    });
}