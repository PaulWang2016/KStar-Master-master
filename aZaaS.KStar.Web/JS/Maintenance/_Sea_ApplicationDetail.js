define(function (require, exports, module) {
    //引入模块
    var RoleManagement = require("/JS/Maintenance/_Sea__RoleManagement");
    var MenuManagementent = require("/JS/Maintenance/_Sea__MenuManagementent");
    var DocumentManagement = require("/JS/Maintenance/_Sea__DocumentManagement");
    var WidgetManagement = require("/JS/Maintenance/_Sea__WidgetManagement");
    var PermissionManagement = require("/JS/Maintenance/_Sea__PermissionManagement");
    var DelegationManagement = require("/JS/Maintenance/_Sea__DelegationManagement");

    //定义方法


    var LoadApplicationDetail = function () {
        var that = this;
        title = that.pane + " Management - Kendo UI";

        this.roleManagement = new RoleManagement(that.pane);
        this.menuManagementent = new MenuManagementent(that.pane);
        this.documentManagement = new DocumentManagement(that.pane);
        this.widgetManagement = new WidgetManagement(that.pane);
        this.permissionManagement = new PermissionManagement(that.pane);
        this.delegationManagement = new DelegationManagement(that.pane);

        //this.roleManagement.init();
        //this.menuManagementent.init();
        //this.documentManagement.init();
        //this.widgetManagement.init();
        //this.permissionManagement.init();
        //this.delegationManagement.init();

        var DetailsItems = $("#Details").children();

        $(DetailsItems[0]).data("Management", this.roleManagement);
        $(DetailsItems[1]).data("Management", this.menuManagementent);
        $(DetailsItems[2]).data("Management", this.documentManagement);
        $(DetailsItems[3]).data("Management", this.widgetManagement);
        $(DetailsItems[4]).data("Management", this.permissionManagement);
        $(DetailsItems[5]).data("Management", this.delegationManagement);

        $("#Details").kendoPanelBar({            
            activate: function (e) {
                $(window).resize();
                refreshCurrentScrolls();
                //ManagementSplitter();
                if (myScrolls && myScrolls[CurrentApp.pane]) {
                    myScrolls[CurrentApp.pane].scrollToElement(e.item)
                }
                if (!$(e.item).data("Init")) {
                    $(e.item).data("Init", true);
                    $(e.item).data("Management").init();
                }
            }
        });
        $("#Details").data("kendoPanelBar").expand($("#Details").children().first(), false)


        that._InitRoleGrid();
        $(".ImportApp").click(function () {
            that._ImportApp();
        });
        $(".ExportApp").click(function () {
            that._ExportApp();
        });
    }

    var ApplicationDetail = function (pane) {
        this.pane = pane;
    }
    ApplicationDetail.prototype.init = LoadApplicationDetail;

    ApplicationDetail.prototype._ExportApp = function () {
        $.post("/Export/ExportAppstoExcel", { pane: this.pane }, function (title) {
            window.location.replace("/Export/Get?title=" + title);
        });
    }

    ApplicationDetail.prototype._ImportApp = function () {
        var ImportAppWindow = $("#ImportAppWindow").data("kendoWindow");
        if (!ImportAppWindow) {
            $("#ImportAppWindow").kendoWindow({
                width: "800px",
                height: "100px",
                title: "Import Window",
                actions: [
                                "Pin",
                                "Minimize",
                                "Maximize",
                                "Close"
                ],
                iframe: true,
                resizable: false,
                content: "/Export/ImportfromExcel?pane=" + this.pane
            });
            ImportAppWindow = $("#ImportAppWindow").data("kendoWindow");
            AddSplitters($("#ImportAppWindow").data("kendoWindow"));

        }
        ImportAppWindow.center().open();
        $("#ImportAppWindow").css("overflow", "hidden");
    }

    ApplicationDetail.prototype._InitRoleGrid = function () {        
        var that = this;
        //$.getJSON("/Maintenance/Applications/GetRelevanceRoleList", { _t: new Date(), pane: this.pane }, function (items) {

            //修改为树形结构
            //InitBaseKendoGrid("RoleList", RoleModel, rolecolumns, items, function () {
            //    bindGridCheckbox("RoleList");
            //    that.roleManagement.InitUserList();
            //});
            //InitBaseKendoGrid("RolePermissionList", RoleModel, rolecolumns, items, function () {
            //    bindGridCheckbox("RolePermissionList");
            //    that.permissionManagement.ChangePermissionTree();
            //});
            //getKendoGrid("RolePermissionList").setDataSource(getKendoGrid("RoleList").dataSource);
        //});
        //初始化角色分类树形
        that.roleManagement.InitRoleList();
        that.permissionManagement.InitRoleList();
    }

    module.exports = ApplicationDetail;
})