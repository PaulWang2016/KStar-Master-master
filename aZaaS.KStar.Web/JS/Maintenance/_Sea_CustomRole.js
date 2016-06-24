define(function (require, exports, module) {
    var ListIDMenuTree;
    var TypeMenuTree;
    var Rolecolumns;

    var CustomRoleRuleModel = kendo.data.Model.define({
        id: "Id",
        fields: {
            Id: { type: "string", editable: false },
            ClassName: { type: "string", editable: false },
            RoleName: { type: "string", editable: true },
            RoleKey: { type: "string", editable: false },
            AssembleName: { type: "string", editable: false },
            Status: { type: "string", editable: true },
            Category_SysId: { type: "string", editable: false }
        }
    });

    var CustomRolecolumns = [
        {
            title: jsResxColumns.Checked, width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.id + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
        { field: "ClassName", title: jsResxColumns.ClassName, filterable: false },
        { field: "RoleName", title: jsResxColumns.RoleName, filterable: false },
        { field: "Status", title: jsResxColumns.Status, filterable: false, width: 70, editor: StatusDropDownEditor }
    ]

    function StatusDropDownEditor(container, options) {
        $('<input required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataSource: [{ Value: "Y", Text: jsResxMaintenance_SeaCustomRole.Enabled },
                             { Value: "N", Text: jsResxMaintenance_SeaCustomRole.Disabled }],
                dataTextField: "Text",
                dataValueField: "Value",
                index:1
            });
    }
    function customroles() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/CustomRole/GetCustomRole?type={0}&_t={1}", ListIDMenuTree, new Date());
                    },
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "ID",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    var CustomRoleManageTreeView;
    var InitCustomRoleManageTreeView = function () {
        CustomRoleManageTreeView = $("#CustomRoleManageTreeView").data("kendoTreeView");
        if (!CustomRoleManageTreeView) {
            $("#CustomRoleManageTreeView").kendoTreeView({
                template: kendo.template($("#CustomRoleManageTreeView-template").html()),
                dataSource: customroles(),
                select: function (e) {
                    $("#AddContextMenu").parent().hide();
                    $("#DeleteContextMenu").parent().hide();
                    $("#DeleteContextMenu").parent().show();
                    $("#AddCustomRoleContextMenu").parent().show();
                    $("#AddCategoryContextMenu").parent().show();
                    $("#itemSave").siblings(".tips").css("visibility", "hidden");
                    $("#CustomRoleManageTreeView").find("input").prop("checked", false);
                    var node = $("#CustomRoleManageTreeView_tv_active").find("input").first().prop("checked", true);
                    var Type = node.attr("data-Type");
                    $("#itemId").val(node.val());
                    $("#itemName").val(node.attr("data-DisplayName"));
                    $("#itemParentID").val(node.attr("data-parentid"))

                    if (Type == "Classify") {
                        $("#AddCustomRoleContextMenu").parent().hide();
                        $("#AddCategoryContextMenu").parent().hide();
                        $("#liCategoryInfomation").hide();
                        $("#liClassifyInfomation").show();
                        $("#showAssembleList").children().hide();
                        $("#enabledFlag").kendoDropDownList({
                            dataSource: [{ Value: "Y", Text: jsResxMaintenance_SeaCustomRole.Enabled },
                             { Value: "N", Text: jsResxMaintenance_SeaCustomRole.Disabled }],
                            dataTextField: "Text",
                            dataValueField: "Value"
                        });
                        $.getJSON("/Maintenance/CustomRole/GetClassifyById", { _t: new Date(), id: node.val() }, function (item) {
                            $("#assembleName").val(item.AssembleName);
                            $("#className").val(item.ClassName);
                            $("#roleName").val(item.RoleName);
                            var dropdownlist = $("#enabledFlag").data("kendoDropDownList");
                            dropdownlist.value(item.Status);
                        });
                    }
                    else {
                        $("#CustomRoleInfomation .info-title").text(jsResxMaintenance_SeaCustomRole.CategoryInfomation);
                        $.getJSON("/Maintenance/CustomRole/GetClassifyByCategoryId", { _t: new Date(), id: node.val() }, function (items) {
                            $("#showAssembleList").children().show();
                            if (items != undefined && items != null && items.length > 0) {
                                $.each(items, function (i) {
                                    item = items[i];
                                    Rolecolumns = [
                                                    {
                                                        title: jsResxColumns.Checked, width: 35, template: function (item) {
                                                            return "<input type='checkbox' value='" + item.id + "' />";
                                                        }, headerTemplate: "<input type='checkbox' />", filterable: false
                                                    },
                                                    { field: "ClassName", title: jsResxColumns.ClassName, filterable: false },
                                                    { field: "RoleName", title: jsResxColumns.RoleName, filterable: false },
                                                    { field: "AssembleName", title: jsResxColumns.AssembleName, filterable: false },
                                                    {
                                                        field: "Status", title: jsResxColumns.Status, width: 58, template: function (item) {
                                                            return item.Status == "Y" ? "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.SysID + "'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.SysID + "'><span class='glyphicon glyphicon-ban-circle'></span></a>"
                                                        }, filterable: false
                                                    }
                                    ]
                                })
                            };
                            InitBaseKendoGridWidthPage("showAssembleList", CustomRoleRuleModel, Rolecolumns, items, 5, function () {
                                bindGridCheckbox("showAssembleList")
                            });
                        });
                        $("#liCategoryInfomation").show();
                        $("#liClassifyInfomation").hide();
                    }
                    $('#CustomRoleManageTreeView .k-state-focused').WinContextMenu({
                        menu: "#CustomRoleContextMenu",
                        removeMenu: '#homeBody',
                        action: function (e) {
                            switch (e.id) {
                                case "AddCategoryContextMenu":
                                    AddCategory($("#CustomRoleManageTreeView_tv_active").find("input").val(), false); break;
                                case "AddCustomRoleContextMenu": AddRoleClassify(); break;
                                case "DelContextMenu": DelCustomRole(); break;
                            }
                        }
                    });

                },
                collapse: function (e) {
                    $("#CustomRoleManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
                },
                expand: function (e) {
                    $("#CustomRoleManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
                },
                dataBound: function (e) {
                    var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                    var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                    $("#CustomRoleManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                    $("#CustomRoleManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
                }
            })
        }
        else {
            CustomRoleManageTreeView.setDataSource(customroles());
        };

        CustomRoleManageTreeView = $("#CustomRoleManageTreeView").data("kendoTreeView");
    }
    var InitCustomRoleSplitter = function () {
        $("#CustomRoleManaView").kendoSplitter({
            panes: [
                { collapsible: false, size: "300px", min: "250px", max: "450px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        window.AddSplitters($("#CustomRoleManaView").data("kendoSplitter"));
    }
    var AddCategory = function (parentId,isFirst) {
        var AddCategoryWindow = $("#AddCategoryWindow").data("kendoWindow");
        if (!AddCategoryWindow) {
            $("#AddCategoryWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaCustomRole.AddCategory,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddCategoryWindow .windowCancel").bind("click", CategoryCancel);
                    $("#AddCategoryWindow .windowConfirm").bind("click", CategoryConfirm);
                },
                close: function (e) {
                    $("#AddCategoryWindow .windowCancel").unbind("click", CategoryCancel);
                    $("#AddCategoryWindow .windowConfirm").unbind("click", CategoryConfirm);
                    hideOperaMask("AddCategoryWindow");
                },
                resizable: false,
                modal: true
            });
            AddCategoryWindow = $("#AddCategoryWindow").data("kendoWindow").center();
            window.AddSplitters(AddCategoryWindow);
        }
        $("#CategoryName").val("");
        $("#CategoryDesc").val("");
        $("#AddCategoryWindow .windowConfirm").attr("data-isFirst", isFirst);
        if (typeof (parentId) == "string") {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", parentId);
        }
        else {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", "");
        }
        $("#AddCategoryWindow").data("kendoValidator").hideMessages();
        AddCategoryWindow.open();

    }
    var CategoryConfirm = function () {
        var validator = $("#AddCategoryWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", CategoryConfirm);
            showOperaMask("AddCategoryWindow");
            var categoryName = $("#CategoryName").val();
            var categoryDesc = $("#CategoryDesc").val();
            $.post("/Maintenance/CustomRole/CreateCategory", { DisplayName: categoryName, ParentID: $("#AddCategoryWindow .windowConfirm").attr("data-parentId") }, function (item) {
                var treeview = $("#CustomRoleManageTreeView").data("kendoTreeView");
                var select = treeview.select();
                var isFirst = $("#AddCategoryWindow .windowConfirm").attr("data-isFirst");
                if (isFirst == "true") {
                    treeview.append(item);
                }
                else {
                    if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                        treeview.append(item, select);
                    }
                    else {
                        treeview.expand(select);
                    }
                }
                $("#AddCategoryWindow").data("kendoWindow").close();
            }).fail(function () {
                that.bind("click", CategoryConfirm);
                hideOperaMask("AddCategoryWindow");
            })
        }
    }
    var CategoryCancel = function () {
        $("#AddCategoryWindow").data("kendoWindow").close()
    }
    var AddRoleClassify = function () {
        var AddCustomRoleWindow = $("#AddCustomRoleWindow").data("kendoWindow");
        if (!AddCustomRoleWindow) {
            $("#AddCustomRoleWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaCustomRole.AddRoleClassify,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddCustomRoleWindow .windowCancel").bind("click", CustomRoleCancel);
                    $("#AddCustomRoleWindow .windowConfirm").bind("click", CustomRoleConfirm);
                },
                close: function (e) {
                    $("#AddCustomRoleWindow .windowCancel").unbind("click", CustomRoleCancel);
                    $("#AddCustomRoleWindow .windowConfirm").unbind("click", CustomRoleConfirm);
                    hideOperaMask("AddCustomRoleWindow");
                },
                resizable: false,
                modal: true
            });
            AddCustomRoleWindow = $("#AddCustomRoleWindow").data("kendoWindow").center();
            window.AddSplitters(AddCustomRoleWindow);
        }

        $("#UploadDllName").val("");
        $("#assembleList").children().hide();
        if (CustomRoleManageTreeView.select().length > 0) {
            AddCustomRoleWindow.open();
        }
        else {
            ShowTip(getJSMsg("_Sea_PositionJS", "SelectCategory"));//"Please select category to add position!");
        }
    }
    var CustomRoleConfirm = function () {
        var that = $(this);
        that.unbind("click", CustomRoleConfirm);
        showOperaMask("AddCustomRoleWindow");
        var parentID = $("#CustomRoleManageTreeView_tv_active").find("input").val();
        var assembleName = $("#UploadDllName").val().replace(".Zip", "");
        //var roleId = $("#EditCustomRoleActionWindow").find(".windowConfirm").attr("data-id");
        var assembleGrid = getKendoGrid("assembleList");
        $("#assembleList .k-grid-content").find("input:checkbox").each(function () {
            var roleId = $(this).val();
            var gridItem = assembleGrid.dataSource.get(roleId);
            var data = {
                Id: roleId,
                RoleName: gridItem.RoleName,
                RoleKey: gridItem.RoleKey,
                ClassName: gridItem.ClassName,
                AssembleName: assembleName,
                Status: gridItem.Status,
                Category_SysId: parentID
            }
            $.ajax({
                url: "/Maintenance/CustomRole/CreateCustomRole",
                type: "POST",
                data: data,
                traditional: true,
                success: function (item) {
                    var treeview = $("#CustomRoleManageTreeView").data("kendoTreeView");
                    var select = treeview.select();

                    if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                        treeview.append(item, select);
                    }
                    else {
                        treeview.expand(select);
                    }
                    $("#AddCustomRoleWindow").data("kendoWindow").close()
                }
            }).fail(function () {
                that.bind("click", CustomRoleConfirm);
                hideOperaMask("AddCustomRoleWindow");
            })
        })
    }
    var CustomRoleCancel = function () {
        $("#AddCustomRoleWindow").data("kendoWindow").close()
    }
    var DelCustomRole = function () {
        var node = $("#CustomRoleManageTreeView_tv_active").find("input:checked");

        if (node.attr("data-Type") == "Classify" && node.val() != null && node.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaCustomRole.Areyousuretodeletethisposition, function (result) {
                if (result) {
                    $.post("/Maintenance/CustomRole/DestroyRoleClassify", { customRoleID: node.val() }, function (customRoleID) {
                        var nodeposition = CustomRoleManageTreeView.dataSource.get(customRoleID);
                        CustomRoleManageTreeView.dataSource.remove(nodeposition);
                        $("#itemName").val("");
                        $("#CustomRoleManageTreeView").parent().siblings().last().children("ul").children().last().hide();
                    });
                }
            });
        }
        else if (node.attr("data-Type") == "Category" && node.val() != null) {
            bootbox.confirm(jsResxMaintenance_SeaCustomRole.Areyousuretodeletethisposition, function (result) {
                if (result) {
                    $.post("/Maintenance/CustomRole/DestroyCategory", { categoryID: node.val() }, function (categoryID) {
                        CustomRoleManageTreeView.dataSource.remove(CustomRoleManageTreeView.dataSource.get(categoryID));
                        $("#itemName").val("");
                        $("#CustomRoleManageTreeView").parent().siblings().last().children("ul").children().last().hide();
                    });
                }
            });
        }
        else { ShowTip(jsResxMaintenance_SeaCustomRole.PleaseselecttheCustomRoleorCategorytoberemovederror, "error"); }
    }
    var UpdateRoleStatus = function (id, status) {
        var msg;
        if (status == "Y") {
            msg = jsResxMaintenance_SeaCustomRole.AreyousureactiveRole;
        }
        else {
            msg = jsResxMaintenance_SeaCustomRole.AreyousuredisableRole;
        }

        bootbox.confirm(msg, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/CustomRole/UpdateCustomRoleStatus",
                    type: "POST",
                    data: { id: id, status: status },
                    traditional: true,
                    success: function (items) {
                        if (items != undefined && items != null && items.length > 0) {
                            $.each(items, function (i) {
                                item = items[i];
                                Rolecolumns = [
                                                {
                                                    title: jsResxColumns.Checked, width: 35, template: function (item) {
                                                        return "<input type='checkbox' value='" + item.id + "' />";
                                                    }, headerTemplate: "<input type='checkbox' />", filterable: false
                                                },
                                                { field: "ClassName", title: jsResxColumns.ClassName, filterable: false },
                                                { field: "RoleName", title: jsResxColumns.RoleName, filterable: false },
                                                { field: "AssembleName", title: jsResxColumns.AssembleName, filterable: false },
                                                {
                                                    field: "Status", title: jsResxColumns.Status, width: 58, template: function (item) {
                                                        return item.Status == "Y" ? "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.SysID + "'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.SysID + "'><span class='glyphicon glyphicon-ban-circle'></span></a>"
                                                    }, filterable: false
                                                }
                                ]
                            })
                        };
                        InitBaseKendoGridWidthPage("showAssembleList", CustomRoleRuleModel, Rolecolumns, items, 5, function () {
                            bindGridCheckbox("showAssembleList")
                        });
                        //getKendoGrid("showAssembleList").dataSource.read();
                    },
                    dataType: "json"
                })
            }
        });
    }
    var SaveItem = function () {
        showOperaMask("CustomRoleInfomation");
        var id = $("#CustomRoleManageTreeView_tv_active").find("input").val();

        if (id != "" && id != undefined && itemName != "") {
            var node = $("#CustomRoleManageTreeView_tv_active").find("input");
            var itemName = $("#itemName").val();
            if (node.attr("data-type") == "Category") {
                $.post("/Maintenance/CustomRole/UpdateCategory", { CategoryID: id, DisplayName: itemName }, function (item) {

                    var template = kendo.template($("#CustomRoleManageTreeView-template").html())
                    var target = $("#CustomRoleManageTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    if (target.parent().parent().attr("aria-expanded")) {
                        target.find(".k-sprite").first().addClass("on");
                    }
                    hideOperaMask("CustomRoleInfomation");
                    $("#itemSave").siblings(".tips").css("visibility", "visible");
                }).fail(function () { hideOperaMask("CustomRoleInfomation"); })
            }
            else {
                var AddidList = [];
                var RemoveidList = [];
                var TempidList = [];
                var roleName = $("#roleName").val();
                var status=$("#enabledFlag").val();
                $.ajax({
                    url: "/Maintenance/CustomRole/UpdateCustomRole",
                    type: "POST",
                    data: { CustomRoleID: id, DisplayName: roleName, CategoryID: node.attr("data-parentid"), Status: status },
                    traditional: true,
                    success: function (item) {
                        //var node = Positions.get(item.PositionID);
                        //node.set("DisplayName", item.DisplayName);//更新数据
                        var template = kendo.template($("#CustomRoleManageTreeView-template").html())
                        var target = $("#CustomRoleManageTreeView_tv_active .k-state-selected");
                        target.html(template({ item: item }));
                        hideOperaMask("CustomRoleInfomation");
                        $("#itemSave").siblings(".tips").css("visibility", "visible");
                    },
                    dataType: "json"
                }).fail(function () { hideOperaMask("CustomRoleInfomation"); })
            }
        }
        else {
            ShowTip(jsResxMaintenance_SeaCustomRole.Pleaseselectcategory);
            hideOperaMask(jsResxMaintenance_SeaCustomRole.CustomRoleInfomation);
        }
    }

    function openfile() {
        try {
            var fd = new ActiveXObject("MSComDlg.CommonDialog");
            fd.Filter = "压缩文件 (*.Zip)|*.Zip";
            fd.FilterIndex = 2;
            // 必须设置MaxFileSize. 否则出错
            fd.MaxFileSize = 128;
            fd.ShowOpen();
            document.getElementById("txtFileName").value = fd.Filename;
            document.getElementById("textImage").src = fd.FileName;
        } catch (e) { alert("你的浏览器不支持ActiveX！\r\n请启用ActiveX后重试．"); document.getElementById("txtFileName").value = ""; }
    }
    
    function LoadPostionView() {
        window.title = "CustomRole Management - Kendo UI";
        InitCustomRoleSplitter();
        $.getJSON("/Maintenance/CustomRole/GetAllCustomRoleType", { _t: new Date() }, function (items) {
            $("#CustomRoleTypeDrop").kendoDropDownList({
                dataTextField: "Name",
                dataValueField: "SysID",
                dataSource: {
                    data: items,
                    schema: {
                        model: {
                            id: "SysID",
                            fields: {
                                SysId: { type: "String" },
                                Name: { type: "String" }
                            }
                        }
                    }
                },
                close: function () {
                    ListIDMenuTree = $("#CustomRoleTypeDrop").val();
                    TypeMenuTree = $("#CustomRoleTypeDrop").data("kendoDropDownList").dataSource.get($("#CustomRoleTypeDrop").val()).Name;

                    InitCustomRoleManageTreeView();
                }
            });

            ListIDMenuTree = $("#CustomRoleTypeDrop").data("kendoDropDownList").dataSource.data()[0].SysID; //默认的listID
            TypeMenuTree = $("#CustomRoleTypeDrop").data("kendoDropDownList").dataSource.get(ListIDMenuTree).Name;
            $("#CustomRoleTypeDrop").val(ListIDMenuTree);

            InitCustomRoleManageTreeView();
        })
        $("#CategoryAdd").click(function () { AddCategory(ListIDMenuTree, true); })
        $("#itemSave").click(SaveItem)
        var AddEmployeeWindow;
        $("#CustomRoleInfomation").children("ul").kendoPanelBar();
        $("#UploadDllFile").kendoUpload({
            async: {
                saveUrl: "/Maintenance/CustomRole/UploadCustomRoleDll",
                saveField: "files"
            },
            localization: {
                select: jsResxMaintenance_SeaCustomRole.Browse,
                uploadSelectedFiles: "*.Zip"
            },
            multiple: false,
            showFileList: false,
            success: function (e) {
                var files = e.files;
                if (e.operation == "upload") {
                    for (var i = 0; i < files.length; i++) {
                        $("#UploadDllName").val(files[i].name);
                        $.getJSON("/Maintenance/CustomRole/GetAssembleInfo", { fileName: files[i].name }, function (items) {
                            $("#assembleList").children().show();

                            InitBaseEditableKendoGrid("assembleList", CustomRoleRuleModel, CustomRolecolumns, items, function () {
                                bindGridCheckbox("assembleList");
                                $("#assembleList").find(":checkbox").prop("checked", true);
                                //InitBaseKendoGridWidthPage("assembleList", CustomRoleRuleModel, CustomRolecolumns, items, 30, function () {
                                //    bindGridCheckbox("assembleList")
                                //    $("#assembleList").find(":checkbox").prop("checked", true);
                            });
                        });
                    }
                }
            },
            upload: function (e) {
                checkfiles(e);
                e.data = { Field: "UploadDllName" };
            }
        });
        var checkfiles = function (e) {
            var files = e.files;
            $.each(files, function () {
                var extension = this.extension.toLowerCase()
                if (extension != ".zip") {
                    bootbox.alert(getJSMsg("_Sea_CustomRoleJS", "Checkfiles"))
                    e.preventDefault();
                }
            });
        }
        $("#showAssembleList").delegate("a.k-Status", "click", function (e) {
            var ok = $(this).find("span.glyphicon-ok");
            var id = $(this).attr("id");
            var status;
            if (ok.length == 1) {
                status = "N";
            }
            else {
                status = "Y";
            }

            UpdateRoleStatus(id, status);
        })
    }

    module.exports = LoadPostionView;
})