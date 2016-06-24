define(function (require, exports, module) {
    var ListIDPermissionTree;
    var PermissionIds = [];
   
    function DocPermissionTreeDatas() {

        return new kendo.data.HierarchicalDataSource({            
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/Applications/GetPermissionDoc?RoleID={0}&Type={1}&_t={2}", ListIDPermissionTree, pane, new Date());
                    },
                    dataType: "json",
                    async: false,

                }
            },
            schema: {
                id: "ID",
                model: {
                    id: "ID",
                    hasChildren: "HasChildren",
                    ReportsTo: "ReportsTo"
                }
            }
        });
    }
    function InitMenuPermissionTree() {
        $.getJSON("/Maintenance/Applications/GetPermissionMenus", { RoleID: ListIDPermissionTree, Type: pane, "_t": new Date() }, function (items) {
            var treeview = new kendo.data.HierarchicalDataSource({
                data: items,
                schema: {
                    id: "ID",
                    model: {
                        id: "ID",
                        hasChildren: "HasChildren",
                        children:"items"
                    }
                }
            });

            var MenuPermissionTreeKendo = $("#MenuPermissionTree").data("kendoTreeView");
            if (!MenuPermissionTreeKendo) {
                $("#MenuPermissionTree").kendoTreeView({
                    template: kendo.template($("#PermissionTree-template").html()),
                    dataSource: treeview,
                    collapse: function (e) {
                        $("#MenuPermissionTree_tv_active").find(".k-sprite").first().removeClass("on");
                    },
                    expand: function (e) {
                        $("#MenuPermissionTree_tv_active").find(".k-sprite").first().addClass("on");
                    },
                    dataBound: function (e) {                        
                        $("#MenuPermissionTree").data("kendoTreeView").expand(".k-first");
                    }
                });
                MenuPermissionTreeKendo = $("#MenuPermissionTree").data("kendoTreeView");
                $("#MenuPermissionTree").on("click", "#MenuPermissionTree_tv_active :checkbox", function () {                    
                    var temp = $("#MenuPermissionTree_tv_active").find(":checkbox").first().val();
                    if (!temp) return;

                    var items = PermissionIds["PermissionTree"];
                    if (!items) {
                        items = {};
                        PermissionIds["PermissionTree"] = items;
                    }
                    if (items[temp]) {
                        items[temp] = undefined;
                    }
                    else {
                        items[temp] = $("#MenuPermissionTree_tv_active").find("input").prop("checked");
                    }


                    var tempItem = $("#DocPermissionTree").data("kendoTreeView").dataSource.get(temp);
                    var item = $("#MenuPermissionTree").data("kendoTreeView").dataSource.get(temp);
                    //$.post("/Maintenance/Applications/SaveConfiguration", {
                    //    RoleID: ListIDPermissionTree,
                    //    Name: "",
                    //    Link: "",
                    //    ID: item.ID,
                    //    HasChildren: item.HasChildren,
                    //    ParentID: item.ParentID,
                    //    Status: $("#MenuPermissionTree_tv_active").find("input").first().prop("checked"),
                    //    Type: item.Type
                    //}, function (data) {
                    var status = $("#MenuPermissionTree_tv_active").find("input").prop("checked");
                    if (tempItem!=undefined && tempItem != null) {
                        $("#DocPermissionTree").find("input").each(function () {
                            if ($(this).val() == tempItem.ID) {
                                $(this).prop("checked", status);
                            }
                        });
                        tempItem.set("Status", status);
                    }
                    item.set("Status", status);
                    //});
                    if (status)
                        parentNode(item.id);
                    childNodes(item.id, status);
                })
                .on("dblclick", "#MenuPermissionTree_tv_active", function (e) { e.stopPropagation(); });
            }
            else {
                MenuPermissionTreeKendo.setDataSource(treeview);
            }

        });
    }
    function InitDocPermissionTree() {

        var DocPermissionTreeKendo = $("#DocPermissionTree").data("kendoTreeView");
        if (!DocPermissionTreeKendo) {
            $("#DocPermissionTree").kendoTreeView({
                template: kendo.template($("#PermissionTree-template").html()),
                dataSource: DocPermissionTreeDatas(),
                collapse: function (e) {
                    $("#DocPermissionTree_tv_active").find(".k-sprite").first().removeClass("on");
                },
                expand: function (e) {
                    $("#DocPermissionTree_tv_active").find(".k-sprite").first().addClass("on");
                },
                dataBound: function (e) {

                    //$("#PermissionTree_tv_active").find(":checkbox").click()
                    //$("#PermissionTree_tv_active").dblclick()
                    $("#DocPermissionTree").data("kendoTreeView").expand(".k-first");
                }
            });
            DocPermissionTreeKendo = $("#DocPermissionTree").data("kendoTreeView");
            $("#DocPermissionTree").on("click", "#DocPermissionTree_tv_active :checkbox", function () {
                var temp = $("#DocPermissionTree_tv_active").find(":checkbox").first().val();
                if (!temp) return;


                var items = PermissionIds["PermissionTree"];
                if (!items) {
                    items = {};
                    PermissionIds["PermissionTree"] = items;
                }
                if (items[temp]) {
                    items[temp] = undefined;
                }
                else {
                    items[temp] = $("#DocPermissionTree_tv_active").find("input").prop("checked");
                }

                var tempItem = $("#MenuPermissionTree").data("kendoTreeView").dataSource.get(temp);
                var item = $("#DocPermissionTree").data("kendoTreeView").dataSource.get(temp);
                //$.post("/Maintenance/Applications/SaveConfiguration", {
                //    RoleID: ListIDPermissionTree,
                //    Name: "",
                //    Link: "",
                //    ID: item.ID,
                //    HasChildren: item.HasChildren,
                //    ParentID: item.ParentID,
                //    Status: $("#DocPermissionTree_tv_active").find("input").first().prop("checked"),
                //    Type: item.Type
                //}, function (data) {
                var status = $("#DocPermissionTree_tv_active").find("input").prop("checked");
                if (tempItem != undefined&&tempItem != null) {
                    $("#MenuPermissionTree").find("input").each(function () {
                        if ($(this).val() == tempItem.ID) {
                            $(this).prop("checked", status);
                        }
                    });
                    tempItem.set("Status", status);
                }
                item.set("Status", status);
                //});

            })
            .on("dblclick", "#DocPermissionTree_tv_active", function (e) { e.stopPropagation(); });
        }
        else {
            DocPermissionTreeKendo.setDataSource(DocPermissionTreeDatas());
        }
    }
    function InitWidgetPermissionGrid() {        
        $.ajax({
            type: "get",            
            url: "/Maintenance/Applications/GetWidgetPermissionGrid",
            data: { RoleID: ListIDPermissionTree, pane: pane },
            dataType: 'json',
            async:false,
            success: function (items) {                
                InitBaseKendoGrid("WidgetPermissionGrid", WidgetPermissionModel, WidgetPermissioncolumns, items, function () {
                    bindGridCheckbox("WidgetPermissionGrid");

                });
                $("#WidgetPermissionGrid").on("click", ":checkbox", function () {
                    if ($(this).val() != "on") {
                        var items = PermissionIds["PermissionTree"];
                        if (!items) {
                            items = {};
                            PermissionIds["PermissionTree"] = items;
                        }
                        var Id = $(this).val();
                        if (items[Id]) {
                            items[Id] = undefined;
                        }
                        else {
                            items[Id] = $(this).prop("checked");
                        }
                    }
                    else {
                        $("#WidgetPermissionGrid .k-grid-content").find(":checkbox").each(function () {
                            var items = PermissionIds["PermissionTree"];
                            if (!items) {
                                items = {};
                                PermissionIds["PermissionTree"] = items;
                            }
                            var Id = $(this).val();
                            items[Id] = $(this).prop("checked");
                            deleGrid.dataSource.get(Id).set("Statu", items[Id]);

                        });
                    }
                    //$.post("/Maintenance/Applications/SaveWidgetPermission", {
                    //    RoleID: ListIDPermissionTree,
                    //    ID: $(this).val(),
                    //    Statu: $(this).prop("checked")
                    //}, function (data) {

                    //}).fail(function () { $(this).prop("checked", $(this).prop("checked") == true ? false : true) })
                });
            }
        });

        //$.getJSON("/Maintenance/Applications/GetWidgetPermissionGrid", { RoleID: ListIDPermissionTree, pane: pane }, function (items) {
        //    InitBaseKendoGrid("WidgetPermissionGrid", WidgetPermissionModel, WidgetPermissioncolumns, items, function () {
        //        bindGridCheckbox("WidgetPermissionGrid");

        //    });
        //    $("#WidgetPermissionGrid").on("click", ":checkbox", function () {
        //        if ($(this).val() != "on") {
        //            var items = PermissionIds["PermissionTree"];
        //            if (!items) {
        //                items = {};
        //                PermissionIds["PermissionTree"] = items;
        //            }
        //            var Id = $(this).val();
        //            if (items[Id]) {
        //                items[Id] = undefined;
        //            }
        //            else {
        //                items[Id] = $(this).prop("checked");
        //            }
        //        }
        //        else {
        //            $("#WidgetPermissionGrid .k-grid-content").find(":checkbox").each(function () {
        //                var items = PermissionIds["PermissionTree"];
        //                if (!items) {
        //                    items = {};
        //                    PermissionIds["PermissionTree"] = items;
        //                }
        //                var Id = $(this).val();
        //                items[Id] = $(this).prop("checked");
        //                deleGrid.dataSource.get(Id).set("Statu", items[Id]);

        //            });
        //        }
        //        //$.post("/Maintenance/Applications/SaveWidgetPermission", {
        //        //    RoleID: ListIDPermissionTree,
        //        //    ID: $(this).val(),
        //        //    Statu: $(this).prop("checked")
        //        //}, function (data) {

        //        //}).fail(function () { $(this).prop("checked", $(this).prop("checked") == true ? false : true) })
        //    })
        //});        
    }

    var parentNode = function (id) {
        var permissionids= PermissionIds["PermissionTree"];
        var item = $("#MenuPermissionTree").data("kendoTreeView").dataSource.get(id);
        if (item != null && item.ParentID != null) {
            $("#MenuPermissionTree").find(":checkbox").each(function () {

                if ($(this).val() == item.ParentID) {
                    $(this).prop("checked", true);
                    if (permissionids[item.ParentID] == undefined)
                    {
                        permissionids[item.ParentID] = true;
                    }
                    parentNode(item.ParentID);
                }
            })
        }

        return;
    }

    var childNodes = function (id, status) {
        var permissionids = PermissionIds["PermissionTree"];
        var item = $("#MenuPermissionTree").data("kendoTreeView").dataSource.get(id);
        if (item != null && item.items != null && item.items.length > 0) {
            var childids = {};
            $.each(item.items, function (i, obj) {
                childids[obj.ID] = obj.ID;
            });
            $("#MenuPermissionTree").find(":checkbox").each(function () {                
                if ($(this).val() == childids[$(this).val()]) {
                    $(this).prop("checked", status);
                    permissionids[$(this).val()] = status;
                    childNodes($(this).val(), status);
                }
            });
        }
    }


    //var childNode = function (id,status) {
    //    //var item = $("#MenuPermissionTree").data("kendoTreeView").dataSource.get(id);
    //    //if (item != null && item._childrenOptions.data != null) {
    //    $("#MenuPermissionTree").find(":checkbox").each(function () {

    //        if ($(this).val() == id) {
    //            $(this).find(":checkbox").prop("checked", status);
    //        }
    //    })
    //    //}
    //}
    var ManagementSplitter = function () {

        $("#PermissionView").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "250px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        AddSplitters($("#PermissionView").data("kendoSplitter"));
    }

    var PermissionSave = function () {
        showOperaMask("Permission_Information_Save");
        var PermissionTrees = PermissionIds["PermissionTree"];
        var items = new Array();
        for (var key in PermissionTrees) {
            var statusPermission = PermissionTrees[key] == true ? true : false;
            items.push("{ ID: '" + key + "', Status: '" + statusPermission + "' }");
        }

        var data = {
            RoleID: ListIDPermissionTree,
            Items: items
        }
        $.ajax({
            url: "/Maintenance/Applications/SaveConfiguration",
            type: "POST",
            data: data,
            traditional: true,
            success: function (data) {
                if (data)
                    $("#Permission_Information_Save").siblings(".tips").css("visibility", "visible");
                hideOperaMask("Permission_Information_Save");

            }
        }).fail(function () {
            hideOperaMask("Permission_Information_Save");
        })
    }

    var ChangePermissionTree = function () {
        $("#RolePermissionList .k-grid-content").on("click", ":checkbox", function () {
            showOperaMask();
            $("#PermissionLeft").find(":checkbox").prop("checked", false);
            $(this).prop("checked", true);
            ListIDPermissionTree =$(this).val();

            $("#Permission_Information_Save").siblings(".tips").css("visibility", "hidden");            
            InitMenuPermissionTree();
            InitDocPermissionTree();
            InitWidgetPermissionGrid();

            TreeViewNodeToggle("MenuPermissionTree");
            TreeViewNodeToggle("DocPermissionTree");

            $("#PermissionRight").css("visibility", "visible");
            //$("#PermissionRight").show();
            hideOperaMask();
            //setTimeout(function () { hideOperaMask(); }, 3000);
        });
    }


    function RoleDataSource() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/Applications/GetRoleTree?_t={0}", new Date());
                    },
                    dataType: "json"
                },
                cache: false
            },
            schema: {
                model: {
                    id: "ID",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    //初始化角色分类树形结构
    var RoleManageTreeView;
    var InitRoleManageTreeView = function () {
        $("#RolePermissionList").kendoTreeView({
            template: kendo.template($("#RolePermissionTreeView-template").html()),
            dataSource: RoleDataSource(),
            select: function (e) {              
                $("#RolePermissionList").find("input").prop("checked", false);
                var node = $("#RolePermissionList_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                var Type = node.attr("data-Type");

                if (Type == "Role") {                                    
                    showOperaMask();                  
                    ListIDPermissionTree = node.val();

                    $("#Permission_Information_Save").siblings(".tips").css("visibility", "hidden");
                    InitMenuPermissionTree();
                    InitDocPermissionTree();
                    InitWidgetPermissionGrid();

                    TreeViewNodeToggle("MenuPermissionTree");
                    TreeViewNodeToggle("DocPermissionTree");

                    $("#PermissionRight").css("visibility", "visible");
                    //$("#PermissionRight").show();
                    setTimeout(function () { hideOperaMask() }, 1000);
                }
                else {
                    $("#PermissionRight").css("visibility", "hidden");
                }                
            },
            collapse: function (e) {
                $("#RolePermissionList_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#RolePermissionList_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#RolePermissionList").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#RolePermissionList").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);
                $("#RolePermissionList").data("kendoTreeView").expand(".k-first");
            }
        });
        RoleManageTreeView = $("#RolePermissionList").data("kendoTreeView");
    }

    var LoadPermisssionManagement = function () {
        $("#PermissionRight").children("ul").kendoPanelBar({
            activate: function () {

                $(window).resize();
                //ManagementSplitter();
            },
        });

        ManagementSplitter();
        $("#Permission_Information_Save").click(PermissionSave);

    }

    var PermisssionManagement = function (p) {
        pane = p;
    }
    PermisssionManagement.prototype.init = LoadPermisssionManagement;
    PermisssionManagement.prototype.ChangePermissionTree = ChangePermissionTree;
    PermisssionManagement.prototype.InitRoleList = InitRoleManageTreeView;

    module.exports = PermisssionManagement;
})