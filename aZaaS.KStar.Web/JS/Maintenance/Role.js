function LoadRoleManagement() {

    $.getJSON("/Maintenance/Role/GetRole", { _t: new Date() }, function (items) {
        InitBaseKendoGrid("RoleView", model, columns, items, function () {
            bindGridCheckbox("RoleView");
        });
    });
    $("#AddRoleWindow").kendoWindow({
        width: "500px",
        title: "Add App",
        actions: [
            "Close"
        ],
        open: function (e) {
            $("#AddRoleWindow .windowCancel").bind("click", Cancel)
            $("#AddRoleWindow .windowConfirm").bind("click", Confirm)
        },
        close: function (e) {
            resetWindow();
            $("#AddRoleWindow .windowCancel").unbind("click", Cancel)
            $("#AddRoleWindow .windowConfirm").unbind("click", Confirm)
        },
        resizable: false,
        modal: true
    });

}
var resetWindow = function () {
    $("#AddRoleWindow .k-textbox").val("");//清除输入框
}
var Cancel = function () {
    $("#AddRoleWindow").data("kendoWindow").close()
}
var Confirm = function () {
    var that = $(this);
    that.unbind("click", Confirm);
    var url = $(this).attr("data-url");
    var data = {
        ID: $("#AddRoleWindow").find(".windowConfirm").attr("data-id"),
        Name: $("#AddRoleWindow .RoleName").val(),
    }
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {
            var grid = getKendoGrid("RoleView");
            var model = grid.dataSource.get(item.ID);
            if (model) {
                grid.dataSource.remove(model);
                grid.dataSource.add(item);
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddRoleWindow").data("kendoWindow").close();
        }
    }).fail(function () {
        that.bind("click", Confirm);
    })
}


var Add = function () {
    $("#AddRoleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Role/CreateRole").attr("data-id", "");
    $("#AddRoleWindow").data("kendoWindow").title("Add Role").center().open();
}
//var Edit = function () {
//    var ID = $("#RoleView .k-grid-content").find(":checked").first().val();
//    EditRole(ID);
//}
var EditRole = function (id) {
    if (id) {
        var item = getKendoGrid("RoleView").dataSource.get(id);
        $("#AddRoleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Role/UpdateRole").attr("data-id", item.ID);
        $("#AddRoleWindow").data("kendoWindow").title("Edit Role").center().open();
        $("#AddRoleWindow .RoleName").val(item.Name);
    }
}
var Del = function () {
    var idList = new Array();
    $("#RoleView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Role/DelRole",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (ids) {
                        var grid = getKendoGrid("RoleView");
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
    id: "ID",
    fields: {
        ID: { type: "string" },
        Name: { type: "string" }
    }
});
var columns = [
    {
        title: "Checked", width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.ID + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    //{ field: "ID", title: "ID", filterable: false },
    { field: "Name", title: "Name", filterable: false },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditRole(data.ID) } }], width: 58 }
]