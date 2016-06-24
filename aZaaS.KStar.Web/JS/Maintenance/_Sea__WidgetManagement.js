define(function (require, exports, module) {
    var pane;
    var Widgetcolumns = [
    {
        title: "Checked", width: 35, template: function (item) {
            return "<input value='" + item.ID + "' type='checkbox' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false

    },
    { field: "DisplayName", width: 120, title: jsResxMaintenance_SeaWidgetManagement.DisplayName, filterable: false },
    { field: "Key", title: jsResxMaintenance_SeaWidgetManagement.Key, width: 120, filterable: false },
    {
        field: "RazorContent", title: jsResxMaintenance_SeaWidgetManagement.Content, template: function (item) {
            return subHtml(item.RazorContent, 100);
        }, filterable: false
    },
    { field: "Description", width: 120, title: jsResxMaintenance_SeaWidgetManagement.Description, filterable: false },
    //{ field: "MenuID", title: "MenuID", filterable: false },
    {
        title: jsResxMaintenance_SeaWidgetManagement.Default, width: 70, filterable: false,
        template: function (item) {
            return item.Statu ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button circle'><span class='glyphicon glyphicon-ban-circle'></span></a>"
        }
    },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditWidget(data.ID) } }], width: 58 }
    ]


    var resetWidgetWindow = function () {
        hideOperaMask("AddWidgetWindow");
        $("#AddWidgetWindow .k-textbox").val("");//清除输入框
        $("#RazorContent").val("");//清除输入框
        //$("#RazorContent").data("kendoEditor").value("");//清除输入框
    }

    var InitWidgetWindows = function () {
        var width = $(window).width() - 100 + "px";
        var height = $(window).height() - 100 + "px";
        var TEMP = $(window).height() - 235;
        //$("#AddWidgetWindow .k-editor").height(TEMP);
        $("#RazorContent").height(TEMP);

        $("#AddWidgetWindow").kendoWindow({
            width: width,
            height: height,
            title: jsResxMaintenance_SeaWidgetManagement.AddNode,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddWidgetWindow .windowCancel").bind("click", WidgetCancel)
                $("#AddWidgetWindow .windowConfirm").bind("click", WidgetConfirm)
            },
            close: function (e) {
                resetWidgetWindow();
                $("#AddWidgetWindow .windowCancel").unbind("click", WidgetCancel)
                $("#AddWidgetWindow .windowConfirm").unbind("click", WidgetConfirm)
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddWidgetWindow").data("kendoWindow"));

    }
    var InitWidgetGrid = function () {
        $.getJSON("/Maintenance/Applications/GetRelevanceWidgetList", { pane: pane }, function (items) {
            InitBaseKendoGrid("WidgetList", WidgetModel, Widgetcolumns, items, function () {
                bindGridCheckbox("WidgetList");
                setDefaultPage();
            });
        });
    }
    var setDefaultPage = function () {
        $("#WidgetTab").on("click", " .k-grid-content .circle", function () {//k-i-cancel
            //$(this).removeClass("circle");
            var item = $(this).parent().siblings().find("input").val()
            var grid = $("#WidgetList").data("kendoGrid");
            $.post("/Applications/SetDefaultPage", { id: item, pane: pane, statu: true }, function (data) {
                grid.dataSource.data(data);
                //setDefaultPage();
            });
        });
    }
    var WidgetCancel = function () {
        $("#AddWidgetWindow").data("kendoWindow").close();
    }
    var WidgetConfirm = function () {
        var that = $(this);
        that.unbind("click", WidgetConfirm);
        showOperaMask("AddWidgetWindow");
        $.post($(this).attr("data-url"), {
            ID: $(this).attr("data-ID"),
            Key: $("#AddWidgetWindow .Key").val(),
            DisplayName: $("#AddWidgetWindow .DisplayName").val(),
            RazorContent: $("#RazorContent").val(),
            //RazorContent: $("#RazorContent").data("kendoEditor").value(),
            Description: $("#AddWidgetWindow .Description").val(),
            Pane: pane
        }, function (item) {

            var grid = $("#WidgetList").data("kendoGrid");
            var model = grid.dataSource.get(item.ID);
            if (model) {
                item.Statu = model.Statu;
                grid.dataSource.remove(model);
            }
            grid.dataSource.add(item);
            bindGridCheckbox("WidgetList");
            $("#AddWidgetWindow").data("kendoWindow").close();
        }).fail(function () {
            that.bind("click", WidgetConfirm);
            hideOperaMask("AddWidgetWindow");
        })
    }
    var AddWidget = function () {
        $("#AddWidgetWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaWidgetManagement.AddWidget).open();
        $("#AddWidgetWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/AddWidget")
    }

    var EditWidget = function (id) {

        if (id != undefined) {
            var grid = $("#WidgetList").data("kendoGrid");
            var item = grid.dataSource.get(id)
            $("#AddWidgetWindow .Key").val(item.Key);
            $("#AddWidgetWindow .DisplayName").val(item.DisplayName);
            $("#RazorContent").val(item.RazorContent);
            //$("#RazorContent").data("kendoEditor").value(item.RazorContent);
            $("#AddWidgetWindow .Description").val(item.Description);

            $("#AddWidgetWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaWidgetManagement.EditWidget).open();
            $("#AddWidgetWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/EditWidget").attr("data-ID", item.ID);
        }
    }
    var DelWidget = function () {
        var list = new Array();
        $("#WidgetList").find(":checked").each(function () {
            if ($(this).val() == "on") return;
            list.push($(this).val());
        })
        if (list.length > 0) {
            bootbox.confirm(getJSMsg("Base", "Confirm"), function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Applications/DeleteWidget",
                        type: "POST",
                        data: { idList: list, pane: pane },
                        traditional: true,
                        success: function (data) {
                            var permissiongrid = $("#WidgetPermissionList").data("kendoGrid");
                            var grid = $("#WidgetList").data("kendoGrid");
                            grid.dataSource.data(data);
                            bindGridCheckbox("WidgetList");
                        },
                        dataType: "json"
                    })
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaWidgetManagement.PleaseselectWidget);
        }
    }

    var LoadWidgetManagement = function () {
        InitWidgetGrid();
        InitWidgetWindows();
        $("#WidgetTab .Delete").click(DelWidget);
        $("#WidgetTab .Add").click(AddWidget);
    }
    WidgetManagement = function (p) {
        pane = p;
    }
    WidgetManagement.prototype.init = LoadWidgetManagement;
    module.exports = WidgetManagement;
})