var WorklistConfigModel = kendo.data.Model.define({
    id: "WorklistID",
    fields: {
        WorklistID: { type: "string" },
        ApplicationName: { type: "string" },
        ProcessName: { type: "string" },
        ConnectionString: { type: "string" },
        DataTable: { type: "string" },
        WhereQuery: { type: "string" }
    }
});
var worklistconfigcolumns = [
        {
            title: "Checked", width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.WorklistID + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    {
        field: "ProcessName", title: "Process", template: function (item) {
            return "<a href='#widget=/Maintenance/WorklistData-" + item.WorklistID + "'> " + item.ProcessName + "</a>";
        }, filterable: false
    },
    { field: "ApplicationName", title: "Application Name", filterable: false },
    { field: "ConnectionString", title: "Connection String", filterable: false },
    { field: "DataTable", title: "Data Table", filterable: false },
    { field: "WhereQuery", title: "Where Query", filterable: false },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditWConfig(data.WorklistID) } }], width: 58 }
]
var ConfigFieldModel = kendo.data.Model.define({
    id: "Field",
    fields: {
        Field: { type: "string", editable: false },
        DisplayName: {
            type: "string", editable: true, validation: {
                required: true,
                displaynamevalidation: function (input) {
                    if (input.is("[name='DisplayName']") && input.val() != "") {
                        input.attr("data-displaynamevalidation-msg", "Display Name should Unique");
                        var grid = getKendoGrid("ConfigFieldListView");
                        var data = grid.dataSource.data()
                        var count = 0;
                        $.each(data, function () {
                            if (this.DisplayName == input.val())
                                count++;
                        })
                        return count <= 1;
                    }
                    return true;
                }
            }
        },
        Description: { type: "string", editable: true },
        IsChecked: { type: "boolean" }
    }
});
var configfieldcolumns = [
        {
            title: "Checked", width: 35, template: function (item) {
                return item.IsChecked ? "<input type='checkbox' value='" + item.Field + "' checked='checked' />" : "<input type='checkbox' value='" + item.Field + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    { field: "Field", title: "Column Name", filterable: false },
    { field: "DisplayName", title: "Display Name", filterable: false },
    { field: "Description", title: "Description", filterable: false }
]
var AddWConfig = function () {
    CreateAddWorklistWindow();

    $("#AddWorklistWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Worklist/AddWorklist").attr("data-id", "");
    $("#AddWorklistWindow").data("kendoWindow").title("Add Worklist Config").center().open();
}
var EditWConfig = function (id) {
    CreateAddWorklistWindow();
    if (id) {
        var item = getKendoGrid("WorklistConfigView").dataSource.get(id);
        $("#AddWorklistWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Worklist/EditWorklist").attr("data-id", item.WorklistID);

        $("#AddWorklistWindow .ApplicationName").val(item.ApplicationName);
        $("#worklistProcess").data("kendoDropDownList").value(item.ProcessName);
        $("#AddWorklistWindow .ConnectionString").val(item.ConnectionString);
        $("#AddWorklistWindow .DataTable").val(item.DataTable);
        $("#AddWorklistWindow .WhereQuery").val(item.WhereQuery);

        $("#AddWorklistWindow").data("kendoWindow").title("Edit Worklist Config").center().open();
    }
    else {
        ShowTip(getJSMsg("WorklistJS", "SelectWorklist"));
    }
}
var DelWConfig = function () {
    var idList = new Array();
    $("#WorklistConfigView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base", "Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Worklist/DelWorklist",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (ids) {
                        var grid = getKendoGrid("WorklistConfigView");
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
        ShowTip(getJSMsg("WorklistJS", "SelectWorklist"));
    }
}

var resetAddWorklistWindow = function () {
    hideOperaMask("AddWorklistWindow");
    $("#AddWorklistWindow .k-textbox").val("");//清除输入框
    $("#worklistProcess").data("kendoDropDownList").select(0);//清除下拉框
}
var CreateAddWorklistWindow = function () {
    var AddWorklistWindow = $("#AddWorklistWindow").data("kendoWindow");
    if (!AddWorklistWindow) {
        $("#AddWorklistWindow").kendoWindow({
            width: "850px",
            title: "Add Worklist Config",
            actions: [
                "Close"
            ],
            open: function (e) {
                InitBaseEditableKendoGrid("ConfigFieldListView", ConfigFieldModel, configfieldcolumns, [], function () {
                    bindGridCheckbox("ConfigFieldListView");
                    $("#ConfigFieldListView").find(".k-grid-content").css("overflow", "auto").css("height", "130px");
                });

                $("#AddWorklistWindow .windowCancel").bind("click", WorklistCancel)
                $("#AddWorklistWindow .windowConfirm").attr("data-isfetch", false).bind("click", WorklistConfirm)
            },
            close: function (e) {
                resetAddWorklistWindow();
                $("#AddWorklistWindow .windowCancel").unbind("click", WorklistCancel)
                $("#AddWorklistWindow .windowConfirm").unbind("click", WorklistConfirm)
            },
            resizable: false,
            modal: true
        });
    }
}
var WorklistCancel = function () {
    $("#AddWorklistWindow").data("kendoWindow").close()
}
var WorklistConfirm = function () {
    var that = $(this);
    that.unbind("click", WorklistConfirm);
    showOperaMask("AddWorklistWindow");
    var url = that.attr("data-url");
    var columns = new Array();
    var Fieldgrid = getKendoGrid("ConfigFieldListView");
    $("#ConfigFieldListView .k-grid-content").find("input:checked").each(function () {
        var field = $(this).val();
        var column = Fieldgrid.dataSource.get(field);
        columns.push("{ ColumnName: '" + field + "', DisplayName: '" + column.DisplayName + "', Description:'" + column.Description + "' }");
    })
    var data = {
        WorklistID: that.attr("data-id"),
        ApplicationName: $("#AddWorklistWindow .ApplicationName").val(),
        ProcessName: $("#worklistProcess").data("kendoDropDownList").value(),
        ConnectionString: $("#AddWorklistWindow .ConnectionString").val(),
        DataTable: $("#AddWorklistWindow .DataTable").val(),
        WhereQuery: $("#AddWorklistWindow .WhereQuery").val(),
        isFetch: $("#AddWorklistWindow .windowConfirm").attr("data-isfetch"),
        columns: columns
    }
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {
            var grid = getKendoGrid("WorklistConfigView");
            var model = grid.dataSource.get(item.WorklistID);
            if (model) {
                for (var key in item) {
                    model.set(key, item[key]);
                }
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddWorklistWindow").data("kendoWindow").close();
        }
    }).fail(function () {
        that.bind("click", WorklistConfirm);
        hideOperaMask("AddWorklistWindow");
    })
}

var FetchConfigField = function () {
    var data = {
        configId: $("#AddWorklistWindow .windowConfirm").attr("data-id"),
        table: $("#AddWorklistWindow .DataTable").val(),
        ConnectionString: $("#AddWorklistWindow .ConnectionString").val()
    };

    $.ajax({
        url: "/Maintenance/Worklist/FetchFields",
        type: "POST",
        data: data,
        traditional: true,
        success: function (items) {
            $("#AddWorklistWindow .windowConfirm").attr("data-isfetch", true)
            var grid = getKendoGrid("ConfigFieldListView");
            grid.dataSource.data([]);//清除原数据
            $.each(items, function () {
                var item = this;
                grid.dataSource.add(item)
            });
        }
    })
}

function LoadWorklistView() {
    title = "Worklist Configuration - Kendo UI";

    $.getJSON("/Maintenance/Worklist/GetWorklist", { _t: new Date() }, function (items) {
        InitKendoExcelGrid("WorklistConfigView", WorklistConfigModel, worklistconfigcolumns, items, 20, "Worklist Configuration", function () {//
            bindAndLoad("WorklistConfigView");
            bindGridCheckbox("WorklistConfigView");

            $("#WorklistConfigView .k-toolbar")
                .append("<a id='WConfigDel' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                .append("<a id='WConfigAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#WConfigAdd").click(AddWConfig);
            $("#WConfigDel").click(DelWConfig);
        });
    })


    $.getJSON("/Maintenance/Process/Get", { _t: new Date() }, function (items) {//EmailTpml/GetProcessList
        $("#worklistProcess").kendoDropDownList({
            dataTextField: "FullName",
            dataValueField: "FullName",
            dataSource: {
                data: items,
                schema: {
                    model: {
                        id: "FullName",
                        fields: {
                            FullName: { type: "String" },
                            Name: { type: "String" }
                        }
                    }
                }
            },
            optionLabel: "--Select Process--"
        });
    }).fail(function () {
        $("#worklistProcess").kendoDropDownList({
            dataTextField: "FullName",
            dataValueField: "FullName",
            dataSource: {
                data: [{ "Folder": "AMS_Process", "Name": "AMS", "FullName": "AMS_Process\\AMS" }, { "Folder": "EmployeeFee", "Name": "EmployeeFee", "FullName": "EmployeeFee\\EmployeeFee" }],
                schema: {
                    model: {
                        id: "FullName",
                        fields: {
                            FullName: { type: "String" },
                            Name: { type: "String" }
                        }
                    }
                }
            },
            optionLabel: "--Select Process--"
        });
    })
}