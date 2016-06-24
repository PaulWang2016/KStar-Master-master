define(function (require, exports, module) {
    var model = kendo.data.Model.define({
        id: "Id",
        fields: {
            Id: { type: "string" },
            Key: { type: "string" },
            DisplayName: { type: "string" },
            DefaultPage: { type: "string" },
            Scope: { type: "string" },
            MenuOrder: { type: "string" }
        }
    });
    var columns = [
        {
            title: jsResxMaintenance_SeaApplicaiton.Checked, width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.Key + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
        {
            //field: "Key", title: jsResxMaintenance_SeaApplicaiton.Key, template: function (item) {
            field: "Key", title: "Key", template: function (item) {
                return "<a href='#widget=/Maintenance/ApplicationDetail-" + item.Key + "'> " + item.Key + "</a>";
            }, filterable: false
        },
        { field: "DisplayName", title: jsResxMaintenance_SeaApplicaiton.DisplayName, filterable: false },
        { field: "DefaultPage", title: jsResxMaintenance_SeaApplicaiton.DefaultPage, filterable: false },
        { field: "MenuOrder", title: jsResxMaintenance_SeaApplicaiton.MenuOrder, filterable: false },
        { field: "Scope", title: jsResxMaintenance_SeaApplicaiton.Scope, filterable: false },
        { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditApp(data.Id) } }], width: 58 }
    ]

    var resetWindow = function () {
        hideOperaMask("AddAppWindow");
        $("#AddAppWindow .k-textbox").val("");//清除输入框
        $("#AddAppWindow .Key").attr("disabled", false);
    }
    var Cancel = function () {
        $("#AddAppWindow").data("kendoWindow").close()
    }
    var Confirm = function () {
        var that = $(this);
        that.unbind("click", Confirm);
        showOperaMask("AddAppWindow");
        var url = $(this).attr("data-url");
        var data = {
            Id: $("#AddAppWindow").find(".windowConfirm").attr("data-id"),
            Key: $("#AddAppWindow .Key").val(),
            DisplayName: $("#AddAppWindow .DisplayName").val(),
            DefaultPage: $("#AddAppWindow .DefaultPage").val(),
            Scope: $("#Scope").val(),
            MenuOrder: $("#AddAppWindow .MenuOrder").val()
        }
        $.ajax({
            url: url,
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {
                var grid = getKendoGrid("GridView");
                var model = grid.dataSource.get(item.Id);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                }
                else {
                    grid.dataSource.add(item)
                }
                $("#AddAppWindow").data("kendoWindow").close();
            }
        }).fail(function () { that.bind("click", Confirm); hideOperaMask("AddAppWindow"); })
    }

    var Add = function () {
        $("#AddAppWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/CreateMenu").attr("data-id", "");
        $("#AddAppWindow").data("kendoWindow").title(jsResxMaintenance_SeaApplicaiton.AddApp).center().open();
    }
    //var Edit = function () {
    //    var key = $("#GridView .k-grid-content").find(":checked").first().val();
    //    EditApp(key);
    //}
    var ExportApps = function () {
        $.post("/Export/ExportAppstoExcel", { pane: "" }, function (title) {
            window.location.replace("/Export/Get?title=" + title);
        });
    }
    var ImportApps = function () {
        var ImportAppsWindow = $("#ImportAppsWindow").data("kendoWindow");
        if (!ImportAppsWindow) {
            $("#ImportAppsWindow").kendoWindow({
                width: "800px",
                height: "100px",
                title: jsResxMaintenance_SeaApplicaiton.ImportWindow,
                actions: [
                                "Pin",
                                "Minimize",
                                "Maximize",
                                "Close"
                ],
                iframe: true,
                resizable: false,
                content: "/Export/ImportfromExcel"
            });
            window.AddSplitters($("#ImportAppsWindow").data("kendoWindow"));

            ImportAppsWindow = $("#ImportAppsWindow").data("kendoWindow");
        }
        ImportAppsWindow.center().open();
        $("#ImportAppsWindow").css("overflow", "hidden");
    }
    var EditApp = function (Id) {
        if (Id) {

            var item = getKendoGrid("GridView").dataSource.get(Id);
            $("#AddAppWindow .Key").val(item.Key).attr("disabled", true);
            $("#AddAppWindow .DisplayName").val(item.DisplayName);
            $("#AddAppWindow .DefaultPage").val(item.DefaultPage);
            $("#Scope").val(item.Scope);
            $("#AddAppWindow .MenuOrder").val(item.MenuOrder);
            $("#AddAppWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/UpdateMenu").attr("data-id", item.Id);
            $("#AddAppWindow").data("kendoWindow").title(jsResxMaintenance_SeaApplicaiton.EditApp).center().open();
        }
    }
    var Del = function () {
        var idList = new Array();
        $("#GridView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaApplicaiton.Areyousure, function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Applications/DelMenu",
                        type: "POST",
                        data: { KeyList: idList },
                        traditional: true,
                        success: function (ids) {

                            var grid = getKendoGrid("GridView");
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
    LoadApplicaiton = function () {
        title = "Application Management - Kendo UI";
        $.getJSON("/Maintenance/Applications/GetApps", { _t: new Date() }, function (items) {
            InitBaseKendoGrid("GridView", model, columns, items, function () {
                //bindAndLoad("delegationList");
                bindGridCheckbox("GridView");

                //$("#delegationList .k-toolbar").append("<a id='delegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>")
                //.append("<a id='delegationDisable' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>");

                //$("#delegationAdd").click(AddDelegation)
                //$("#delegationDisable").click(DisableDelegation)
            });
        });

        $("#AddAppWindow").kendoWindow({
            width: "800px",
            title: jsResxMaintenance_SeaApplicaiton.AddApp,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddAppWindow .windowCancel").bind("click", Cancel)
                $("#AddAppWindow .windowConfirm").bind("click", Confirm)
            },
            close: function (e) {
                resetWindow();
                $("#AddAppWindow .windowCancel").unbind("click", Cancel)
                $("#AddAppWindow .windowConfirm").unbind("click", Confirm)
            },
            resizable: false,
            modal: true
        });
        window.AddSplitters($("#AddAppWindow").data("kendoWindow"));

        $(".Add").click(Add);
        $(".Del").click(Del);
        $(".ExportApps").click(ExportApps);
        $(".ImportApps").click(ImportApps);
    }
    module.exports = LoadApplicaiton;
})