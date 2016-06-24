define(function (require, exports, module) {

    var DocManagement = function (p) {
        pane = p;
        DocDataSource = new kendo.data.HierarchicalDataSource({
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
                    url: "/Maintenance/Applications/GetDocuemtns?key=" + pane + "&_t=" + new Date(),
                    dataType: "json"
                }
            }
        });
    }
    var DocDataSource = {}
    var InitDocSplitter = function () {
        $("#DocManagementSplitter").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "250px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        AddSplitters($("#DocManagementSplitter").data("kendoSplitter"));

    }

    var resetDocWindow = function () {
        hideOperaMask("AddDocWindow");
        $("#AddDocWindow .k-textbox").val("");//清除输入框
        $("#Key").attr("disabled", false);
    }
    var DoccontextMenu = function () {

        $('#DocManagementTreeView_tv_active .k-state-focused').WinContextMenu({
            //cancel: '.cancel',
            removeMenu: '#homeBody',
            action: function (e) {
                var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
                var title;
                switch (e.id) {
                    case "AddContextMenu":
                        if (item.Type == "Menu") {
                            $("#AddDocWindow .documentLibrary").show();
                            title = "Add DocumentLibrary";
                        }
                        else if (item.Type == "DocumentLibrary") {
                            $("#AddDocWindow .documentItem").show();
                            title = "Add DocumentItem";
                        }
                        else if (item.Type == "DocumentItem") {
                            ShowTip(getJSMsg("_Sea_DocumentManagementJS", "CanNotAdd"), "error");
                            return;
                        }
                        DocAdd();
                        break;
                    case "DeleteContextMenu": DocDel(); break;
                }
            }//自由设计项事件回调
        });

        //$.contextMenu({
        //    selector: '#DocManagementTreeView_tv_active .menuType',
        //    callback: function (key, options) {
        //        var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
        //        $("#AddDocWindow .documentLibrary").show();
        //        DocAdd();
        //    },
        //    items: {
        //        "add": { name: "Add", icon: "add" },
        //    }
        //});
        //$.contextMenu({
        //    selector: '#DocManagementTreeView_tv_active .DocumentItemType',// input:checked
        //    callback: function (key, options) {
        //        var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
        //        var title;
        //        DocDel();
        //    },
        //    items: {
        //        "delete": { name: "Delete", icon: "delete" }
        //    }
        //});
        //$.contextMenu({
        //    selector: '#DocManagementTreeView_tv_active .DocumentLibraryType',// input:checked
        //    callback: function (key, options) {
        //        var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
        //        var title;
        //        switch (key) {
        //            case "add":
        //                $("#AddDocWindow .documentItem").show();
        //                DocAdd();
        //                break;
        //            case "edit":
        //                $("#AddDocWindow .documentLibrary").show();
        //                DocEdit();
        //                break;
        //            case "delete": DocDel(); break;
        //        }
        //    },
        //    items: {
        //        "add": { name: "Add", icon: "add" },
        //        "delete": { name: "Delete", icon: "delete" }
        //    }
        //});
    }

    var InitDocWindow = function () {
        $("#AddDocWindow").kendoWindow({
            width: "800px",
            title: jsResxMaintenance_SeaDocumentManagement.AddNode,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddDocWindow .windowCancel").bind("click", DocCancel)
                $("#AddDocWindow .windowConfirm").bind("click", DocConfirm)
            },
            close: function (e) {
                resetDocWindow();
                $("#AddDocWindow .windowCancel").unbind("click", DocCancel)
                $("#AddDocWindow .windowConfirm").unbind("click", DocConfirm)
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddDocWindow").data("kendoWindow"));
    }



    var InitDocManagementTreeView = function () {
        $("#DocManagementTreeView").kendoTreeView({
            template: kendo.template($("#DocManagementTreeView-template").html()),
            dataSource: DocDataSource,
            select: function (e) {
                $("#Doc_Information_Save").siblings(".tips").css("visibility", "hidden");
                $("#DocManagementTreeView").find("input").prop("checked", false);
                $("#DocManagementTreeView_tv_active .k-state-focused").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息

                var id = $("#DocManagementTreeView_tv_active").find("input").val();
                var item = DocDataSource.get(id);

                if (item.Type == "Menu") {
                    $("#Doc_Information").css("visibility", "hidden");
                    $("#AddContextMenu").parent().show();
                    $("#DeleteContextMenu").parent().hide();
                    //$("#DocManagementTreeView .menuType").removeClass("menuType");
                    //$("#DocManagementTreeView_tv_active .k-state-focused").find("input").parent().addClass("menuType");
                    DoccontextMenu();
                    return;
                }

                if (item.Type == "DocumentItem") {
                    $("#AddContextMenu").parent().hide();
                    $("#DeleteContextMenu").parent().show();
                    //$("#DocManagementTreeView .DocumentItemType").removeClass("DocumentItemType");
                    //$("#DocManagementTreeView_tv_active .k-state-focused").find("input").parent().addClass("DocumentItemType");
                    $("#Doc_BasicInformation .documentItem").show();
                    $("#Doc_BasicInformation .documentLibrary").hide();

                }
                else if (item.Type == "DocumentLibrary") {
                    $("#AddContextMenu").parent().show();
                    $("#DeleteContextMenu").parent().show();
                    //$("#DocManagementTreeView .DocumentLibraryType").removeClass("DocumentLibraryType");
                    //$("#DocManagementTreeView_tv_active").find("input").parent().addClass("DocumentLibraryType");

                    $("#Doc_BasicInformation .documentLibrary").show();
                    $("#Doc_BasicInformation .documentItem").show();
                }
                $("#Doc_BasicInformation .DisplayName").val(item.Name);
                $("#Doc_BasicInformation .IconPath").val(item.Target);
                $("#Doc_BasicInformation .StorageUri").val(item.Link);
                $("#Doc_BasicInformation .Key").val(item.IconKey);
                $("#Doc_Information").css("visibility", "visible");

                DoccontextMenu();
            },
            collapse: function (e) {
                $("#DocManagementTreeView_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#DocManagementTreeView_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#DocManagementTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#DocManagementTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);
                $("#DocManagementTreeView").data("kendoTreeView").expand(".k-first");
            }
        });
        //DocManagementTreeView = $("#DocManagementTreeView").data("kendoTreeView");
    }
    var DocCancel = function () {
        $("#AddDocWindow .documentItem").hide();
        $("#AddDocWindow .documentLibrary").hide();
        $("#AddDocWindow .DisplayName").val("");
        $("#AddDocWindow .IconPath").val("");
        $("#AddDocWindow .StorageUri").val("");
        $("#AddDocWindow .Key").val("");
        $("#AddDocWindow").data("kendoWindow").close()
    }
    var DocConfirm = function () {
        var that = $(this);
        that.unbind("click", DocConfirm);
        showOperaMask("AddDocWindow");
        var url = $(this).attr("data-url");
        var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
        var data = {
            ID: item.ID,
            Name: $("#AddDocWindow .DisplayName").val(),
            Link: $("#AddDocWindow .StorageUri").val(),
            IconKey: $("#AddDocWindow .Key").val(),
            Type: item.Type,
            Target: $("#AddDocWindow .IconPath").val(),
            ParentID: item.ParentID
        }

        $.ajax({
            url: url,
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {

                var model = DocDataSource.get(item.ID);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                    var template = kendo.template($("#DocManagementTreeView-template").html())
                    var target = $("#DocManagementTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                }
                else {
                    var treeview = $("#DocManagementTreeView").data("kendoTreeView");
                    var select = treeview.select();
                    if (select.attr("aria-expanded")) {
                        treeview.append(item, select);
                    }
                    else {
                        treeview.expand(select);
                    }
                }

                $("#AddDocWindow").data("kendoWindow").close();
            }

        }).fail(function () {
            that.bind("click", DocConfirm);
            hideOperaMask("AddDocWindow");
        })
    }
    var DocAdd = function () {

        $("#AddDocWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/AddDoc");
        $("#AddDocWindow").data("kendoWindow").title(jsResxMaintenance_SeaDocumentManagement.AddDocument).center().open();
    }
    var DocEdit = function () {

        var key = $("#DocManagementTreeView").find(":checked").first().val();
        if (key) {
            var item = DocDataSource.get(key);
            $("#AddDocWindow .DisplayName").val(item.Name);
            $("#AddDocWindow .IconPath").val(item.Target);
            $("#AddDocWindow .StorageUri").val(item.Link);
            $("#AddDocWindow .Key").val(item.IconKey);
            $("#AddDocWindow .Key").attr("disabled", true);
            $("#AddDocWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Applications/EditDoc");
            $("#AddDocWindow").data("kendoWindow").title(jsResxMaintenance_SeaDocumentManagement.EditDocument).center().open();
        }
    }
    var DocDel = function () {
        var id = $("#DocManagementTreeView_tv_active").find(":checked").val();

        if (id) {
            bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Applications/DeleteDoc",
                        type: "POST",
                        data: { id: id },
                        traditional: true,
                        success: function () {
                            DocDataSource.remove(DocDataSource.get(id));
                        },
                        dataType: "json"
                    })
                }
            });
        }
    }
    var DocumentSave = function () {
        showOperaMask("Doc_Information");
        var item = DocDataSource.get($("#DocManagementTreeView_tv_active .k-state-selected").find("input").val());
        var data = {
            ID: item.ID,
            Name: $("#Doc_BasicInformation .DisplayName").val(),
            Link: $("#Doc_BasicInformation .StorageUri").val(),
            IconKey: $("#Doc_BasicInformation .Key").val(),
            Type: item.Type,
            Target: $("#Doc_BasicInformation .IconPath").val(),
            ParentID: item.ParentID
        }

        $.ajax({
            url: "/Maintenance/Applications/EditDoc",
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {
                var model = DocDataSource.get(item.ID);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                    var template = kendo.template($("#DocManagementTreeView-template").html())
                    var target = $("#DocManagementTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    $("#DocManagementTreeView_tv_active").find("input").first().prop("checked", true);
                }
                hideOperaMask("Doc_Information");
                $("#Doc_Information_Save").siblings(".tips").css("visibility", "visible");
            }
        }).fail(function () {
            hideOperaMask("Doc_Information");
        })
    }

    var LoadDocManagement = function () {
        $("#Doc_Information").children("ul").kendoPanelBar({
            collapse: function () {
                $("#Doc_Information").children("ul").find("a").removeClass("k-state-selected");
                $("#Doc_Information").children("ul").find("a").removeClass("k-state-focused");
            },
            expand: function () {
                $("#Doc_Information").children("ul").find("a").removeClass("k-state-selected");
                $("#Doc_Information").children("ul").find("a").removeClass("k-state-focused");
            },
        });
        var DocManagementTreeView;
        InitDocManagementTreeView();
        TreeViewNodeToggle("DocManagementTreeView");

        InitDocSplitter();
        InitDocWindow();
        $("#Doc_Information_Save").click(DocumentSave);
    }
    DocManagement.prototype.init = LoadDocManagement;
    module.exports = DocManagement;
})