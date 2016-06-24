define(function (require, exports, module) {
    var pane;
    var UserIdlist;
    var first = 1;

    var ManagementSplitter = function () {
        $("#RoleManagementSplitter").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "250px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });

        AddSplitters($("#RoleManagementSplitter").data("kendoSplitter"));
    }
    var resetWindow = function () {
        hideOperaMask("AddRoleEmployeeWindow");
        $("#AddRoleEmployeeWindow .k-textbox").val("");//清除输入框
    }
    var InitUserList = function () {
        $("#RoleList .k-grid-content").on("click", ":checkbox", function () {
            $("#User_Role_Save").siblings(".tips").css("visibility", "hidden");
            showOperaMask();
            $("#RoleList").find(":checkbox").prop("checked", false);
            $(this).prop("checked", true);
            //$("#UserTab").show();
            $("#UserTab").css("visibility", "visible");
            $.getJSON("/Maintenance/Applications/GetUserByRole", { id: $(this).val(), "_t": new Date() }, function (items) {                
                hideOperaMask();
                UserIdlist = new Array();                
                for (var key in items) {
                    UserIdlist.push(items[key].StaffId);
                }
                InitBaseKendoGridWidthPage("UserList", StaffModel, employeecolumns, items,10, function () {
                    bindGridCheckbox("UserList");

                });
            })
        })
    }
    var InitRoleStaffList = function () {
        InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
        employeecolumns, "/Maintenance/Staff/GetStaffs", {}, 20, function () {
            bindGridCheckbox("RoleStaffList");
            $("#RoleStaffList .k-grid-content").css("height", "290px")
        });
    }

    var InitWindows = function () {

        $("#AddRoleEmployeeWindow").kendoWindow({
            width: "800px",
            title: jsResxMaintenance_SeaRolManagement.AddNode,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddRoleEmployeeWindow .windowCancel").bind("click", RoleEmployeeCancel)
                $("#AddRoleEmployeeWindow .windowConfirm").bind("click", RoleEmployeeConfirm)
            },
            close: function (e) {
                resetWindow();
                refreshCurrentScrolls();
                $("#AddRoleEmployeeWindow .windowCancel").unbind("click", RoleEmployeeCancel)
                $("#AddRoleEmployeeWindow .windowConfirm").unbind("click", RoleEmployeeConfirm)
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddRoleEmployeeWindow").data("kendoWindow"));


        $("#AddRoleWindow").kendoWindow({
            width: "400px",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddRoleWindow .windowCancel").bind("click", RoleCancel);
                $("#AddRoleWindow .windowConfirm").bind("click", RoleConfirm);
            },
            close: function () {
                $("#AddRoleWindow").find("input").val("");
                hideOperaMask("AddRoleWindow");
                $("#AddRoleWindow .windowCancel").unbind("click", RoleCancel);
                $("#AddRoleWindow .windowConfirm").unbind("click", RoleConfirm);
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddRoleWindow").data("kendoWindow"));

        $("#AddRoleCategoryWindow").kendoWindow({
            width: "400px",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddRoleCategoryWindow .windowCancel").bind("click", RoleCategoryCancel);
                $("#AddRoleCategoryWindow .windowConfirm").bind("click", RoleCategoryConfirm);
            },
            close: function () {
                $("#AddRoleCategoryWindow").find("input").val("");
                hideOperaMask("AddRoleCategoryWindow");
                $("#AddRoleCategoryWindow .windowCancel").unbind("click", RoleCategoryCancel);
                $("#AddRoleCategoryWindow .windowConfirm").unbind("click", RoleCategoryConfirm);
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddRoleCategoryWindow").data("kendoWindow"));
        
    }


    var InitRoleAutoComplete = function () {
        $.getJSON("/Maintenance/Applications/GetRoleList", { _t: new Date() }, function (data) {
            $("#DropDownRoleList").kendoAutoComplete({
                dataSource: data,
                //select: onSelect,
                //change: onChange,
                //close: onClose,
                //open: onOpen,
                //dataBound: onDataBound
            });
        });
    }

    var RoleEmployeeConfirm = function () {
        showOperaMask("AddRoleEmployeeWindow");
        var that = $(this);
        that.unbind("click", RoleEmployeeConfirm);
        var idList = new Array();
        $("#RoleStaffList .k-grid-content").find(":checked").each(function () {            
            if (!$("#UserList").data("kendoExcelGrid").dataSource.get(this.value)) {
                $("#UserList").data("kendoExcelGrid").dataSource.add($("#RoleStaffList").data("kendoGrid").dataSource.get(this.value));                
            }
        })
        $("#AddRoleEmployeeWindow").data("kendoWindow").close();
        hideOperaMask("AddRoleEmployeeWindow");
        //var id = $("#AddRoleEmployeeWindow .windowConfirm").attr("data-roleid");
        //if (idList.length > 0) {
        //    $.ajax({
        //        url: $("#AddRoleEmployeeWindow .windowConfirm").attr("data-url"),
        //        type: "POST",
        //        data: { roleId: id, userIds: idList },
        //        traditional: true,
        //        success: function (items) {
        //            $("#UserList").data("kendoExcelGrid").dataSource.data(items);
        //            $("#AddRoleEmployeeWindow").data("kendoWindow").close();
        //            hideOperaMask("AddRoleEmployeeWindow");
        //        },
        //        dataType: "json"
        //    }).fail(function () {
        //        that.bind("click", RoleEmployeeConfirm);
        //        hideOperaMask("AddRoleEmployeeWindow");
        //    })
        //}

    }
    var RoleEmployeeSearch = function () {
        var input = $("#RoleEmployeeInput").val();
        InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
        findstaffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 20, function () {
            bindGridCheckbox("RoleStaffList");
            $("#RoleStaffList .k-grid-content").css("height", "290px")
        });
    }
    var RoleEmployeeDel = function () {
        if (first == 1) {
            UserIdlist = new Array();
            var items = $("#UserList").data("kendoExcelGrid").dataSource._data;
            $.each(items, function (i, item) { UserIdlist.push(item.StaffId) });
            first++;
        }
        var idList = new Array();
        $("#UserList .k-grid-content").find(":checked").each(function () {
            idList.push(this.value);
            $("#UserList").data("kendoExcelGrid").dataSource.remove($("#UserList").data("kendoExcelGrid").dataSource.get(this.value));
            //RemoveIdlist.push(this.value);
        })
        //var id = $("#RoleList .k-grid-content").find(":checked").val();
        if (idList.length > 0) {
            //$.ajax({
            //    url: "/Maintenance/Applications/RemoveUsersfromRole",
            //    type: "POST",
            //    data: { roleId: id, userIds: idList },
            //    traditional: true,
            //    success: function (idList) {
            //        for (var i = 0; i < idList.length; i++) {
            //            var dataItem = $("#UserList").data("kendoExcelGrid").dataSource.get(idList[i]);
            //            $("#UserList").data("kendoExcelGrid").dataSource.remove(dataItem);
            //        }
            //    },
            //    dataType: "json"
            //})
        }
        else {
            ShowTip(jsResxMaintenance_SeaRolManagement.Cannolongerbeadded);
        }
    }
    var UserRoleSave = function () {        
        showOperaMask("User_Role_Save");
        var AddidList = new Array();
        var RemoveidList = new Array();
        var TempidList = new Array();

        var data = $("#UserList").data("kendoExcelGrid").dataSource._data;

        $.each(data,function (i,d) {
            AddidList.push(d.StaffId);
            for (var key in UserIdlist) {
                if (UserIdlist[key] == d.StaffId) {
                    AddidList.pop();
                    break;
                }
            }
            if (!($.inArray(d.StaffId, UserIdlist) == -1)) {
                TempidList.push(d.StaffId);
            }
            //$("#UserList").data("kendoExcelGrid").dataSource.remove($("#UserList").data("kendoExcelGrid").dataSource.get(this.value));
            //RemoveIdlist.push(this.value);
        })
        for (var key in UserIdlist) {
            if (($.inArray(UserIdlist[key], TempidList) == -1)) {
                RemoveidList.push(UserIdlist[key]);
            }
        }
        var id = $("#RoleList_tv_active").find("input").val();        
        $.ajax({
            url: "/Maintenance/Applications/SaveUsersTOrole",
            type: "POST",
            data: { roleId: id, addUserIds: AddidList, removeUserIds: RemoveidList },
            traditional: true,
            success: function (data) {
                $("#User_Role_Save").siblings(".tips").css("visibility", "visible");
                hideOperaMask("User_Role_Save");
            },
            dataType: "json"
        }).fail(function () {
            hideOperaMask("User_Role_Save");
        });
    }
    var RoleEmployeeAdd = function () {
        //弃用
        //$("#AddRoleEmployeeWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/AppendUserstoRole")
        //    .attr("data-RoleId", $("#RoleList .k-grid-content").find(":checked").val());
        //$("#AddRoleEmployeeWindow").data("kendoWindow").title(jsResxMaintenance_SeaRolManagement.AddUser).center().open();

        if (first == 1)
        {            
            UserIdlist = new Array();
            var items = $("#UserList").data("kendoExcelGrid").dataSource._data;
            $.each(items, function (i, item) { UserIdlist.push(item.StaffId) });
            first++;
        }
        SelectEmployee(this);
    }
    //选用户方法
    var SelectEmployee = function (obj) {
        var data = $("#UserList").data("kendoExcelGrid").dataSource._data;
        InitSelectPersonWindow(obj, "Person", function (json) {
            var list = json.Root.Users.Item;
            $.each(list, function (i, n) {
                if (!ExistsSelectPerson(n, data)) {
                    $("#UserList").data("kendoExcelGrid").dataSource.add({ StaffId: n.Value, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' });
                }
            });
        })
    }
    function ExistsSelectPerson(item, data) {        
        var flag = false;
        $.each(data, function (i, n) {
            if (n.StaffId == item.Value) {
                flag = true;
            }
        });
        return flag;
    }
    //选用户方法

    var RoleEmployeeCancel = function () {
        $("#AddRoleEmployeeWindow").data("kendoWindow").close()
    }


    var RoleCancel = function () {
        $("#AddRoleWindow").data("kendoWindow").close();
    }
    var RoleConfirm = function () {        
        var validator = $("#AddRoleWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", RoleConfirm);
            showOperaMask("AddRoleWindow");
            var categorySysId="";
            var nodetype=$("#RoleList_tv_active").find("input").attr("data-type");
            if (nodetype == "Role")
            {
                categorySysId=$("#RoleList_tv_active").find("input").attr("data-parentid");
            }
            else{
                categorySysId=$("#RoleList_tv_active").find("input").val();
            }
            $.post($(this).attr("data-url"), { ID: $("#RoleList_tv_active").find("input").val(), Name: $("#DropDownRoleList").val(), CategorySysId: categorySysId, Pane: pane }, function (item) {
                var treeview = $("#RoleList").data("kendoTreeView");
                var select = treeview.select();
                //if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                //    treeview.append(item, select);
                //}
                //else {
                //    treeview.expand(select);
                //}
                if (nodetype == "Role") {
                    var template = kendo.template($("#RoleManageTreeView-template").html())
                    var target = $("#RoleList_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    target.find("input").first().prop("checked", true);
                }
                else {
                    treeview.append(item, select);                    
                }
                //var grid = getKendoGrid("RolePermissionList");
                //if (grid.dataSource.get(item.ID)) {
                //    grid.dataSource.remove(grid.dataSource.get(item.ID));
                //}
                //grid.dataSource.add({
                //    RoleID:item.ID,
                //    DisplayName: item.DisplayName
                //});
                var RolePermissionTreeview = $("#RolePermissionList").data("kendoTreeView");
                if (RolePermissionTreeview!=undefined) {
                    $("#RolePermissionList").data("kendoTreeView").dataSource.read();
                }
                //InitUserList();

            })
            $("#AddRoleWindow").data("kendoWindow").close();
        }
    }

    var RoleAdd = function () {
        $("#AddRoleWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaRolManagement.AddRole).open();        
        $("#AddRoleWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/AddRole");
        $("#AddRoleWindow").data("kendoValidator").hideMessages();
    }
    var RoleEdit = function () {
        if ($("#RoleList_tv_active").find("input").length==0) {
            ShowTip(jsResxMaintenance_SeaRolManagement.Pleaseselectrole);
            return;
        }
        $("#AddRoleWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaRolManagement.EditRole).open();
        $("#AddRoleWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/EditRole");        
        $("#DropDownRoleList").val($("#RoleList_tv_active").find("input").attr("data-displayname"));
        $("#AddRoleWindow").data("kendoValidator").hideMessages();
    }
    var RoleDel = function () {
        var list = new Array();
        $("#RoleList").find(":checked").each(function () {
            if ($(this).val() == "on") return;
            list.push($(this).val());
        })
        if (list.length > 0) {
            if (list.indexOf("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8") > -1)
            {
                ShowTip(jsResxMaintenance_SeaRolManagement.CantDeleteSystemRole);
                return;
            }
            bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Applications/DeleteRole",
                        type: "POST",
                        data: { ListID: list, Pane: pane },
                        traditional: true,
                        success: function (idList) {
                            for (var i = 0; i < idList.length; i++) {
                                //var dataItem = getKendoGrid("RolePermissionList").dataSource.get(idList[i]);
                                //getKendoGrid("RolePermissionList").dataSource.remove(dataItem);
                                var dataItem = getKendoGrid("RoleList").dataSource.get(idList[i]);
                                getKendoGrid("RoleList").dataSource.remove(dataItem);
                            }
                            //ChangeMenuTree("RolePermissionList");  //隐藏
                            //bindGridCheckbox("RoleList");
                            //bindGridCheckbox("RolePermissionList");
                            //InitUserList();
                        },
                        dataType: "json"
                    })
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaRolManagement.Pleaseselectrole);
        }
    }


    var RoleCategoryCancel = function () {
        $("#AddRoleCategoryWindow").data("kendoWindow").close();
    }
    var RoleCategoryConfirm = function () {
        var validator = $("#AddRoleCategoryWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", RoleCategoryConfirm);
            showOperaMask("AddRoleCategoryWindow");
            var parentId = $("#AddRoleCategoryWindow .windowConfirm").attr("data-parentId");
            var type = $("#AddRoleCategoryWindow .windowConfirm").attr("data-type");
            $.post($(this).attr("data-url"), { CategoryID: $("#RoleList_tv_active").find("input").val(), DisplayName: $("#RoleCategory").val(), ParentID: parentId }, function (item) {
                var treeview = $("#RoleList").data("kendoTreeView");
                var select = treeview.select();              
                if (type == "edit") {
                    var template = kendo.template($("#RoleManageTreeView-template").html())
                    var target = $("#RoleList_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    target.find("input").first().prop("checked", true);
                }
                else {                    
                    if (item.ParentID == null || item.ParentID == "") {
                        treeview.append(item);
                    }
                    else {
                        treeview.append(item, select);
                    }
                }               

            })
            $("#AddRoleCategoryWindow").data("kendoWindow").close();
        }
    }
    var RoleCategoryAdd = function (parentId) {
        $("#AddRoleCategoryWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaRolManagement.AddRoleCategory).open();
        $("#AddRoleCategoryWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/CreateRoleCategory");
        $("#AddRoleCategoryWindow").data("kendoValidator").hideMessages();

        $("#AddRoleCategoryWindow .windowConfirm").attr("data-parentId", parentId);
        $("#AddRoleCategoryWindow .windowConfirm").attr("data-type", "add");
    }
    var RoleCategoryEdit = function (categoryId) {
        if ($("#RoleList_tv_active").find("input").length == 0) {
            ShowTip(jsResxMaintenance_SeaRolManagement.Pleaseselectrolecategory);
            return;
        }
        $("#AddRoleCategoryWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaRolManagement.EditRoleCategory).open();
        $("#AddRoleCategoryWindow .windowConfirm").attr("data-url", "/Maintenance/Applications/UpdateRoleCategory");
        $("#RoleCategory").val($("#RoleList_tv_active").find("input").attr("data-displayname"));
        $("#AddRoleCategoryWindow").data("kendoValidator").hideMessages();
        $("#AddRoleCategoryWindow .windowConfirm").attr("data-parentId", categoryId);
        $("#AddRoleCategoryWindow .windowConfirm").attr("data-type", "edit");
    }

    var DelRoleOrCategory = function () {
        var node = $("#RoleList_tv_active").find("input:checked");
        var selectitem = $("#RoleList").data("kendoTreeView").dataItem($("#RoleList_tv_active"));
        if (selectitem != undefined && selectitem.children != undefined && selectitem.children._data != undefined && selectitem.children._data.length != undefined && selectitem.children._data.length > 0) {
            ShowTip(jsResxMaintenance_SeaRolManagement.CantDeleteRoleCategory, "error");
            return;
        }
        if (node.attr("data-Type") == "Role" && node.val() != null && node.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaRolManagement.Areyousuretodeletethisrole, function (result) {
                if (result) {
                    var list = new Array();
                    list.push(node.val());                    
                    $.ajax({
                        url: "/Maintenance/Applications/DeleteRole",
                        dataType: "json",
                        type: "POST",
                        data: { ListID: list, Pane: pane },
                        traditional: true,
                        success: function (idList) {
                            for (var i = 0; i < idList.length; i++) {
                                RoleManageTreeView.dataSource.remove(RoleManageTreeView.dataSource.get(idList[i]));
                                //var grid = getKendoGrid("RolePermissionList");
                                //if (grid.dataSource.get(idList[i])) {
                                //    grid.dataSource.remove(grid.dataSource.get(idList[i]));
                                //}
                                var RolePermissionTreeview = $("#RolePermissionList").data("kendoTreeView");
                                if (RolePermissionTreeview != undefined) {
                                    $("#RolePermissionList").data("kendoTreeView").dataSource.read();
                                }
                            }                           
                        }                        
                    })
                }
            });
        }
        else if (node.attr("data-Type") == "Category" && node.val() != null) {
            bootbox.confirm(jsResxMaintenance_SeaRolManagement.Areyousuretodeletethiscategory, function (result) {
                if (result) {
                    $.post("/Maintenance/Applications/DeleteCategory", { categoryID: node.val() }, function (categoryID) {
                        RoleManageTreeView.dataSource.remove(RoleManageTreeView.dataSource.get(categoryID));                                                
                    });
                }
            });
        }
        else { ShowTip(jsResxMaintenance_SeaRolManagement.PleaseselecttheRoleCategorytoberemovederror, "error"); }
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
        $("#RoleList").kendoTreeView({
            template: kendo.template($("#RoleManageTreeView-template").html()),
            dataSource: RoleDataSource(),
            select: function (e) {
                $("#AddRoleContextMenu").parent().show();
                $("#EditRoleContextMenu").parent().show();                
                $("#AddCategoryContextMenu").parent().show();
                $("#EditCategoryContextMenu").parent().show();
                $("#RoleList").find("input").prop("checked", false);
                var node = $("#RoleList_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                var Type = node.attr("data-Type");

                if (Type == "Role") {
                    $("#AddRoleContextMenu").parent().hide();
                    $("#AddCategoryContextMenu").parent().hide();
                    $("#EditCategoryContextMenu").parent().hide();
                    $("#User_Role_Save").siblings(".tips").css("visibility", "hidden");
                    showOperaMask();                                                            
                    $("#UserTab").css("visibility", "visible");
                    //$.getJSON("/Maintenance/Applications/GetUserByRole", { id: node.val(), "_t": new Date() }, function (items) {
                    //    hideOperaMask();
                    //    UserIdlist = new Array();
                    //    for (var key in items) {
                    //        UserIdlist.push(items[key].StaffId);
                    //    }
                    //    InitBaseKendoGridWidthPage("UserList", StaffModel, employeecolumns, items, 10, function () {
                    //        bindGridCheckbox("UserList");
                    //    });                        
                    //})
                    InitServerKendoGrid("UserList", StaffModel, employeecolumns, "/Maintenance/Applications/GetRoleUsersWithPage?id=" + node.val() + "&_t=" + new Date(), 200, function () {
                        hideOperaMask();
                        bindGridCheckbox("UserList");
                        first = 1;
                    });
                }
                else {
                    $("#EditRoleContextMenu").parent().hide();                    
                    $("#UserTab").css("visibility", "hidden");
                }
                $('#RoleList .k-state-focused').WinContextMenu({
                    //cancel: '.cancel',
                    menu: "#RoleContextMenu",
                    removeMenu: '#homeBody',
                    action: function (e) {
                        switch (e.id) {
                            case "AddCategoryContextMenu":
                                RoleCategoryAdd($("#RoleList_tv_active").find("input").val()); break;
                            case "EditCategoryContextMenu":
                                RoleCategoryEdit($("#RoleList_tv_active").find("input").val()); break;
                            case "AddRoleContextMenu": RoleAdd(); break;
                            case "EditRoleContextMenu": RoleEdit(); break;
                            case "DelContextMenu": DelRoleOrCategory(); break;
                        }
                    }
                });

            },
            collapse: function (e) {
                $("#RoleList_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#RoleList_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#RoleList").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#RoleList").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);
                $("#RoleList").data("kendoTreeView").expand(".k-first");
            }
        });
        RoleManageTreeView = $("#RoleList").data("kendoTreeView");
    }
    var LoadRoleManagement = function () {

        InitWindows();
        InitRoleAutoComplete();
        InitRoleStaffList();
        ManagementSplitter();
        $("#AddRoleEmployeeWindow .windowCancel").click(RoleEmployeeCancel);
        $("#AddRoleEmployeeWindow .windowConfirm").click(RoleEmployeeConfirm);
        $("#UserTab .Add").click(RoleEmployeeAdd);
        $("#UserTab .Delete").click(RoleEmployeeDel);
        $("#RoleEmployeeSelect").click(RoleEmployeeSearch);
        $("#RoleTab .Add").click(function () { RoleCategoryAdd(""); });
        //$("#RoleTab .Edit").click(RoleEdit);
        //$("#RoleTab .Delete").click(RoleDel);
        $("#User_Role_Save").click(UserRoleSave);
    }

    var RoleManagement = function (p) {
        pane = p;
    }
    RoleManagement.prototype.init = LoadRoleManagement;
    RoleManagement.prototype.InitUserList = InitUserList;
    RoleManagement.prototype.InitRoleList = InitRoleManageTreeView;

    module.exports = RoleManagement;
})