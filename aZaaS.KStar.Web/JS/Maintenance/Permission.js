function LoadPermissionManagement() {

    $.getJSON("/Maintenance/Permission/GetPermission",{ _t: new Date() }, function (items) {
        InitBaseKendoGridWidthPage("PermissionView", model, columns, items, 10, function () {
            bindGridCheckbox("PermissionView");
        });
    });
    $("#AddPermissionWindow").kendoWindow({
        width: "500px",
        title: "Add Permission",
        actions: [
            "Close"
        ],
        open: function (e) {
            $("#AddPermissionWindow .windowCancel").bind("click", Cancel)
            $("#AddPermissionWindow .windowConfirm").bind("click", Confirm)
        },
        close: function (e) {
            resetWindow();
            $("#AddPermissionWindow .windowCancel").unbind("click", Cancel)
            $("#AddPermissionWindow .windowConfirm").unbind("click", Confirm)
        },
        resizable: false,
        modal: true
    });

}
var resetWindow = function () {
    $("#AddPermissionWindow .k-textbox").val("");//清除输入框
}
var Cancel = function () {
    $("#AddPermissionWindow").data("kendoWindow").close()
}
var Confirm = function () {
    var that = $(this);
    that.unbind("click", Confirm);
    var url = $(this).attr("data-url");
    var data = {
        SysId: $("#AddPermissionWindow").find(".windowConfirm").attr("data-id"),
        Name: $("#AddPermissionWindow .Name").val(),
        Code: $("#AddPermissionWindow .Code").val(),
        Description: $("#AddPermissionWindow .Description").val(),
    }
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {

            var grid = getKendoGrid("PermissionView");
            var model = grid.dataSource.get(item.SysId);
            if (model) {
                grid.dataSource.remove(model);
                grid.dataSource.add(item);
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddPermissionWindow").data("kendoWindow").close();
        }
    }).fail(function () { that.bind("click", Confirm); });
}


var Add = function () {
    $("#AddPermissionWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Permission/CreatePermission").attr("data-id", "");

    $("#AddPermissionWindow").data("kendoWindow").title("Add Permission").center().open();
}

var EditPermission = function (id) {
    if (id) {
        var item = getKendoGrid("PermissionView").dataSource.get(id);
        $("#AddPermissionWindow .Name").val(item.Name);
        $("#AddPermissionWindow .Code").val(item.Code);
        $("#AddPermissionWindow .Description").val(item.Description);

        $("#AddPermissionWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Permission/UpdatePermission").attr("data-id", item.SysId);
        $("#AddPermissionWindow").data("kendoWindow").title("Edit Permission").center().open();
    }
}

var Del = function () {
    var idList = new Array();
    $("#PermissionView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Permission/DelPermission",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (ids) {
                        var grid = getKendoGrid("PermissionView");
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
}

var model = kendo.data.Model.define({
    id: "SysId",
    fields: {
        SysId: { type: "string" },
        Code: { type: "string" },
        Name: { type: "string" },
        Description: { type: "string" }
    }
});
var columns = [
    {
        title: "Checked", width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.SysId + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    //{ field: "SysId", title: "ID", filterable: false },
    { field: "Name", title: "Name", filterable: false },
    { field: "Code", title: "Code", filterable: false },
    { field: "Description", title: "Description", filterable: false },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditPermission(data.SysId) } }], width: 58 }
]