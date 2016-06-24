define(function (require, exports, module) {
    var pane;
    var MenuManagement = function (p) {
        pane = p;
        dataSource = new kendo.data.HierarchicalDataSource({
            schema: {
                model: {
                    id: "ID",
                    hasChildren: "HasChildren",
                    ReportsTo: "ParentId",
                    fields: {
                        ID: { type: "string" },
                        Name: { type: "string" },
                        Link: { type: "string" },
                        IconKey: { type: "string" },
                        Target: { type: "string" },
                        Type: { type: "string" },
                        HasChildren: { type: "string" },
                        ParentId: { type: "string" }
                    }
                }
            },
            transport: {
                read: {
                    url: "/Maintenance/Applications/GetMenus?key=" + pane + "&_t=" + new Date(),
                    dataType: "json"
                }
            }
        });
    }
    var dataSource = {};

    var InitMenuSplitter = function () {
        $("#MenuManagementSplitter").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "250px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        AddSplitters($("#MenuManagementSplitter").data("kendoSplitter"));
    }
    var resetMenuWindow = function () {
        hideOperaMask("AddMenuWindow");
        $("#AddMenuWindow .k-textbox").val("");//清除输入框
        $("#MenuBarTarget").data("kendoDropDownList").select(0);
    }
    var MenucontextMenu = function () {

        $('#MenuManagementTreeView_tv_active .k-state-focused').WinContextMenu({
            //cancel: '.cancel',
            removeMenu: '#homeBody',
            action: function (e) {
                var item = dataSource.get($("#MenuManagementTreeView_tv_active .k-state-selected").find("input").val());
                var title;
                switch (e.id) {
                    case "AddContextMenu":
                        if (item.Type == "Menu")
                            MenuAddCate();
                        else if (item.HasChildren == "false")
                        { ShowTip(getJSMsg("_Sea_MenuManagementJS", "CanNotAdd"), "error"); }
                        else MenuAdd();
                        break;
                    case "DeleteContextMenu": MenuDel(); break;
                }
            }//自由设计项事件回调
        });
    }

    var InitMenuWindow = function () {
        $("#AddMenuWindow").kendoWindow({
            width: "800px",
            title: "Add Node",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddMenuWindow .windowCancel").bind("click", MenuCancel)
                $("#AddMenuWindow .windowConfirm").bind("click", MenuConfirm)
            },
            close: function (e) {
                resetMenuWindow();
                $("#AddMenuWindow .windowCancel").unbind("click", MenuCancel)
                $("#AddMenuWindow .windowConfirm").unbind("click", MenuConfirm)
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddMenuWindow").data("kendoWindow"));

        var window = $("#updateDisplayName"),
                        btn = $(".displayName-plus")
                                .bind("click", function () {
                                    window.data("kendoWindow").center().open();
                                });

        if (!window.data("kendoWindow")) {
            window.kendoWindow({
                width: "600px",
                title: "",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#updateDisplayName .windowCancel").bind("click", function () {
                        window.data("kendoWindow").close();
                    });

                    if ($("#AddMenuWindow").data("kendoWindow").options.visible) {//MenuAdd
                        $("#updateDisplayName .windowConfirm").bind("click", function () {
                            var data = new Object();
                            data.value = new Object();

                            var langArr = new Array();
                            $("#updateDisplayName .form-group").each(function () {
                                data.value[$(this).data("key")] = $.trim($("#" + $(this).data("key")).val());
                                langArr.push(data.value[$(this).data("key")])
                            });

                            var pass = false;
                            $.each(langArr, function (i, n) {
                                if (n != "") {
                                    pass = true;
                                }
                            });
                            if (!pass) {
                                return false;
                            }

                            $("#AddMenuWindow .DisplayName").val(langArr.join(";")).data("lang", data);
                            window.data("kendoWindow").close();
                        });

                        $("#updateDisplayName :text").val("");
                    }
                    else {//MenuEdit
                        $("#updateDisplayName .windowConfirm").bind("click", function () {
                            var data = $("#Menu_BasicInformation .DisplayName").data("lang");
                            var langArr = new Array();
                            $("#updateDisplayName .form-group").each(function () {
                                data.value[$(this).data("key")] = $.trim($("#" + $(this).data("key")).val());
                                langArr.push(data.value[$(this).data("key")])
                            });

                            $("#Menu_BasicInformation .DisplayName").val(langArr.join(";")).data("lang", data);
                            window.data("kendoWindow").close();
                        });

                        var data = $("#Menu_BasicInformation .DisplayName").data("lang");
                        for (var key in data.value) {
                            console.log(key);
                            if (key == "uid")
                                break;
                            if (key == "_events")
                                continue;
                            $("#" + key).val(data.value[key]);
                        }
                    }
                },
                close: function (e) {
                    $("#updateDisplayName .windowCancel").unbind("click");
                    $("#updateDisplayName .windowConfirm").unbind("click");
                },
                resizable: false,
                modal: true
            });
        }
    }
    var InitMenuManagementTreeView = function () {
        $("#MenuManagementTreeView").kendoTreeView({
            template: kendo.template($("#MenuManagementTreeView-template").html()),
            dataSource: dataSource,
            select: function (e) {
                $("#Menu_Information_Save").siblings(".tips").css("visibility", "hidden");
                $("#MenuBarTarget").parents("tr").show();
                $("#MenuBarTarget_show").parents(".row").show();
                $("#MenuManagementTreeView").find("input").prop("checked", false);
                $("#MenuManagementTreeView_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                var id = $("#MenuManagementTreeView_tv_active").find("input").val();
                var item = dataSource.get(id);
                if (item.Type == "Menu") {
                    //$("#Doc_Information").hide();
                    $("#Doc_Information").css("visibility", "hidden");
                    $("#AddContextMenu").parent().show();
                    $("#DeleteContextMenu").parent().hide()
                    $("#MenuBarTarget").parents("tr").hide();;

                    MenucontextMenu();
                    //$("#MenuManagementTreeView .menuType").removeClass("menuType");
                    //$("#MenuManagementTreeView_tv_active .k-state-focused").find("input").parent().addClass("menuType");
                    return;
                } else if (item.HasChildren == "true" || item.HasChildren == true) {
                    $("#AddContextMenu").parent().show();
                    $("#DeleteContextMenu").parent().show();
                    $("#MenuBarTarget_show").parents(".row").hide();
                    //$("#MenuManagementTreeView .LibraryType").removeClass("LibraryType");
                    //$("#MenuManagementTreeView_tv_active .k-state-focused").find("input").parent().addClass("LibraryType");
                } else {
                    $("#AddContextMenu").parent().hide();
                    $("#DeleteContextMenu").parent().show();
                    //$("#MenuManagementTreeView .ItemType").removeClass("ItemType");
                    //$("#MenuManagementTreeView_tv_active .k-state-focused").find("input").parent().addClass("ItemType");
                }
                //$("#Menu_BasicInformation .DisplayName").val(item.Name);
                var langArr = new Array();
                for (var key in item.Data.value) {                    
                    if (key == "uid")
                        break;
                    if (key == "_events")
                        continue;
                    langArr.push(item.Data.value[key]);
                }
                //langArr.push(item.Data.value.zhcn);
                //langArr.push(item.Data.value.zhtw);
                //langArr.push(item.Data.value.enus);
                $("#Menu_BasicInformation .DisplayName").val(langArr.join(";")).data("lang", item.Data);

                $("#Menu_BasicInformation .Hyperlink").val(item.Link);
                //$("#Menu_BasicInformation .MenuBarTarget").val(item.Target);
                $("#MenuBarTarget_show").data("kendoDropDownList").value(item.Target);
                $("#Menu_BasicInformation .IconKey").val(item.IconKey);
                $("#Menu_BasicInformation .OrderBy").val(item.OrderBy);
                //$("#Menu_Information").show();
                $("#Menu_Information").css("visibility", "visible");

                MenucontextMenu();
            },
            collapse: function (e) {
                $("#MenuManagementTreeView_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#MenuManagementTreeView_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#MenuManagementTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#MenuManagementTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
                $("#MenuManagementTreeView").data("kendoTreeView").expand(".k-first");
            }
        });
        //MenuManagementTreeView = $("#MenuManagementTreeView").data("kendoTreeView");
    }
    var MenuCancel = function () {
        $("#AddMenuWindow").data("kendoWindow").close()
    }
    var MenuConfirm = function () {
        var that = $(this);
        that.unbind("click", MenuConfirm);
        showOperaMask("AddMenuWindow");
        var url = $(this).attr("data-url");
        var item = dataSource.get($("#MenuManagementTreeView_tv_active .k-state-selected").find("input").val());
        var data = {
            ID: item.ID,
            Name: $("#AddMenuWindow .DisplayName").val(),
            Link: $("#AddMenuWindow .Hyperlink").val(),
            IconKey: $("#AddMenuWindow .IconKey").val(),
            OrderBy: $("#AddMenuWindow .OrderBy").val(),
            Type: item.Type,
            Target: $("#MenuBarTarget").data("kendoDropDownList").value(),
            ParentID: item.ParentId,
            Data: JSON.stringify($("#AddMenuWindow .DisplayName").data("lang"))
        }

        $.ajax({
            url: url,
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {

                var model = dataSource.get(item.ID);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                    var template = kendo.template($("#MenuManagementTreeView-template").html())
                    var target = $("#MenuManagementTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                }
                else {
                    var treeview = $("#MenuManagementTreeView").data("kendoTreeView");
                    var select = treeview.select();
                    if (select.attr("aria-expanded")) {
                        treeview.append(item, select);
                    }
                    else {
                        treeview.expand(select);
                    }
                }
                $("#AddMenuWindow").data("kendoWindow").close();
            }
        }).fail(function () { that.bind("click", MenuConfirm); hideOperaMask("AddMenuWindow"); });
    }
    var MenuAddCate = function () {
        $("#AddMenuWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/AddMenu").attr("data-id", "").attr("data-Kind", "1");
        $("#AddMenuWindow").data("kendoWindow").title(jsResxMaintenance_SeaMenuManagementent.AddMenu).center().open();
    }
    var MenuAdd = function () {
        var key = $("#MenuManagementTreeView").find(":checked").first().val();
        if (!key) {
            return;
        }
        var item = dataSource.get(key);
        $("#AddMenuWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/AddMenu").attr("data-id", "").attr("data-Kind", "0").attr("data-ParentId", item.Id);
        $("#AddMenuWindow").data("kendoWindow").title(jsResxMaintenance_SeaMenuManagementent.AddMenu).center().open();
    }
    var MenuEdit = function () {
        var key = $("#MenuManagementTreeView").find(":checked").first().val();
        if (key) {
            var item = dataSource.get(key);
            $("#AddMenuWindow .DisplayName").val(item.Name);
            $("#AddMenuWindow .Hyperlink").val(item.Link);
            $("#AddMenuWindow .IconKey").val(item.IconKey);
            $("#AddMenuWindow .OrderBy").val(item.OrderBy);
            $("#MenuBarTarget").data("kendoDropDownList").value(item.Target);
            $("#AddMenuWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/EditMenu").attr("data-id", item.id).attr("data-Kind", item.Kind).attr("data-ParentId", item.ParentId);
            $("#AddMenuWindow").data("kendoWindow").title(jsResxMaintenance_SeaMenuManagementent.EditMenu).center().open();
        }
    }
    var MenuDel = function () {
        var id = $("#MenuManagementTreeView_tv_active").find("input").val();
        if (id) {
            bootbox.confirm(getJSMsg("Base", "Confirm"), function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Applications/DeleteMenu",
                        type: "POST",
                        data: { id: id },
                        traditional: true,
                        success: function () {
                            dataSource.remove(dataSource.get(id));
                        },
                        dataType: "json"
                    })
                }
            });
        }
    }
    var MenuSave = function () {
        showOperaMask("Menu_Information");
        var item = dataSource.get($("#MenuManagementTreeView_tv_active .k-state-selected").find("input").val());
        var data = {
            ID: item.ID,
            Name: item.Name,
            Link: $("#Menu_BasicInformation .Hyperlink").val(),
            IconKey: $("#Menu_BasicInformation .IconKey").val(),
            OrderBy: $("#Menu_BasicInformation .OrderBy").val(),
            Type: item.Type,
            Target: $("#MenuBarTarget_show").data("kendoDropDownList").value(),
            ParentID: item.ParentID,
            Data: JSON.stringify($("#Menu_BasicInformation .DisplayName").data("lang"))
        }

        $.ajax({
            url: "/Maintenance/Applications/EditMenu",
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {
                 
                var model = dataSource.get(item.ID);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                    var template = kendo.template($("#MenuManagementTreeView-template").html())
                    var target = $("#MenuManagementTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    $("#MenuManagementTreeView_tv_active").find("input").first().prop("checked", true);
                }
                hideOperaMask("Menu_Information");
                $("#Menu_Information_Save").siblings(".tips").css("visibility", "visible");                
            }
        }).fail(function () {
            hideOperaMask("Menu_Information");
        })
    }

    var LoadMenuManagement = function () {
        $("#Menu_Information").children("ul").kendoPanelBar({
            collapse: function () {
                $("#Menu_Information").children("ul").find("a").removeClass("k-state-selected");
                $("#Menu_Information").children("ul").find("a").removeClass("k-state-focused");
            },
            expand: function () {
                $("#Menu_Information").children("ul").find("a").removeClass("k-state-selected");
                $("#Menu_Information").children("ul").find("a").removeClass("k-state-focused");
            },
        });

        var MenuManagementTreeView;
        InitMenuManagementTreeView();
        TreeViewNodeToggle("MenuManagementTreeView");
        InitMenuSplitter();
        InitMenuWindow();
        $("#MenuBarTarget").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: "PopUp", value: "PopUp" }, { text: "Redirect", value: "Redirect" }, { text: "Panel", value: "Panel" }, { text: "Window", value: "Window" }],
            optionLabel: "--Select Status--"
        });
        $("#MenuBarTarget_show").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: "PopUp", value: "PopUp" }, { text: "Redirect", value: "Redirect" }, { text: "Panel", value: "Panel" }, { text: "Window", value: "Window" }],
            optionLabel: "--Select Status--"
        });
        $("#Menu_Information_Save").click(MenuSave);
    }
    MenuManagement.prototype.init = LoadMenuManagement;
    module.exports = MenuManagement;
})