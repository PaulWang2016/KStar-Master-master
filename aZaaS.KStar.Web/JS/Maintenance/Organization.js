var ListIDMenuTree;
var TypeMenuTree;

function organizations() {

    return new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: function (options) {
                    return kendo.format("/Maintenance/Organization/GetOrganization?ListID={0}&Type={1}", ListIDMenuTree, TypeMenuTree);
                },
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: "ID",               //绑定ID
                hasChildren: "HasChildNode"  //绑定是否包含子节点                 
            }
        }
    });
}
function InitTreeView() {
    $("#Information").hide();
    var OrganizationTreeKendo = $("#OrganizationManageTreeView").data("kendoTreeView");
    if (!OrganizationTreeKendo) {
        $("#OrganizationManageTreeView").kendoTreeView({
            template: kendo.template($("#OrganizationManageTreeView-template").html()),
            dataSource: organizations(),

            select: function (e) {
                $("#Organization_Save").siblings(".tips").css("visibility", "hidden");
                $("#OrganizationManageTreeView").find("input").prop("checked", false);
                $("#Information").show();
                var select = $("#OrganizationManageTreeView_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息

                var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());
                //TypeMenuTree = item.NodeName;
                var name, type, code;
                switch (item.Type) {
                    case "Property":
                        $("#BasicInformation").empty().html($("#property").html());
                        $("#BasicInformation .propertyName").val(item.NodeName);
                        $("#BasicInformation .propertyCode").val(item.Code);
                        $("#BasicInformation .propertyType").val(item.Type);
                        $("#BasicInformation .englishName_Full").val(item.EnglishName_Full);
                        $("#BasicInformation .englishAddress1").val(item.EnglishAddress_First);
                        $("#BasicInformation .englishAddress2").val(item.EnglishAddress_Second);
                        $("#BasicInformation .englishAddress3").val(item.EnglishAddress_Third);
                        $("#BasicInformation .chineseName_Full").val(item.ChineseName_Full);
                        $("#BasicInformation .chineseAddress1").val(item.ChineseAddress_First);
                        $("#BasicInformation .chineseAddress2").val(item.ChineseAddress_Second);
                        $("#BasicInformation .chineseAddress3").val(item.ChineseAddress_Third);
                        $("#OrganizationManageTreeView  .PropertyType").removeClass("PropertyType");
                        $("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("PropertyType");
                        $("#OrganizationManaView").show();
                        break;
                    case "Cluster":
                    case "Division":
                        $("#BasicInformation").empty().html($("#SecondBasicInfo").html());
                        $("#BasicInformation .SecondName").val(item.NodeName);
                        $("#BasicInformation .SecondCode").val(item.Code);
                        $("#BasicInformation .SecondType").val(item.Type);

                        $("#OrganizationManageTreeView  .Type").removeClass("Type");
                        $("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("Type");
                        $("#OrganizationManaView").show();
                        break;
                    case "Company":
                        $("#BasicInformation").empty().html($("#FirstBasicInfo").html());
                        $("#BasicInformation .FirstName").val(item.NodeName);
                        $("#BasicInformation .FirstType").val(item.Type);

                        $("#OrganizationManageTreeView  .Type").removeClass("Type");
                        $("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("Type");
                        $("#OrganizationManaView").show();
                        break;
                }
                initNodePositionList();
                initNodeUserList();

                $("#Information").children("ul").kendoPanelBar();
            },
            collapse: function (e) {
                $("#OrganizationManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#OrganizationManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#OrganizationManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#OrganizationManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
            }
        });
        OrganizationTreeKendo = $("#OrganizationManageTreeView").data("kendoTreeView");
    }
    else {
        OrganizationTreeKendo.setDataSource(organizations());
    }
}

function ExportCharts() {
    $.post("/Export/ExportCharttoXml", { chartId: ListIDMenuTree }, function (title) {
        window.location.replace("/Export/GetXml?title=" + title);
    });
}

var InitOrganizationSplitter = function () {
    $("#OrganizationManaView").kendoSplitter({
        panes: [
            { collapsible: false, size: "300px", min: "300px", max: "450px", resizable: true },
            { collapsible: false, resizable: true }
        ]
    });
    AddSplitters($("#OrganizationManaView").data("kendoSplitter"));
}

var InitOrganizationWindow = function () {
    $("#AddOtherWindows").kendoWindow({
        width: "500px",
        height: "350px",
        actions: [
            "Close"
        ],
        open: function (e) {
            $("#AddOtherWindows .windowCancel").bind("click", SearchCancel)
            $("#AddOtherWindows .windowConfirm").bind("click", SearchConfirm)
        },
        close: function (e) {
            $("#AddOtherWindows .windowCancel").unbind("click", SearchCancel)
            $("#AddOtherWindows .windowConfirm").unbind("click", SearchConfirm)
        },
        resizable: false,
        modal: true
    });
    AddSplitters($("#AddOtherWindows").data("kendoWindow"));

}

var orgChartCancel = function () {
    $("#Organization_window").data("kendoWindow").close()
}
var orgChartConfirm = function () {
    var that = $(this);
    that.unbind("click", orgChartConfirm);
    showOperaMask("Organization_window");
    if ($("#AddInfo .OrganizationName").val() != null) {
        var tt = $("#OrganizationDrop").val();

        //var tt = $("#AddInfo .OrganizationName").attr("data-url");"Maintenance/Organization/AddOrganization"
        if ($("#AddInfo .OrganizationName").attr("data-url") == "Maintenance/Organization/AddOrganization")
            $.post("Maintenance/Organization/AddOrganization", { Name: $("#AddInfo .OrganizationName").val() }, function (data) {

                $("#OrganizationDrop").data("kendoDropDownList").dataSource.data(data);
                $("#Organization_window").data("kendoWindow").close();
            }).fail(function () {
                that.bind("click", confirm);
                hideOperaMask("Organization_window");
            })
        else if ($("#AddInfo .OrganizationName").attr("data-url") == "Maintenance/Organization/EditOrganization")
            $.post("Maintenance/Organization/EditOrganization", { Name: $("#AddInfo .OrganizationName").val(), OrgChartId: $("#OrganizationDrop").val() }, function (data) {

                $("#OrganizationDrop").data("kendoDropDownList").dataSource.data(data);
                $("#Organization_window").data("kendoWindow").close();
            }).fail(function () {
                that.bind("click", confirm);
                hideOperaMask("Organization_window");
            })
        return;
    }
    var select = $("#OrganizationManageTreeView_tv_active").find("input");
    if (null == select) return;
    var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());
    var name, type, code;
    switch (item.Type) {
        case "Cluster":
            name = $("#Organization_window .propertyName").val();
            code = $("#Organization_window .propertyCode").val();
            type = $("#Organization_window .propertyType").val();
            break;
        case "Company":
        case "Division":
            name = $("#Organization_window .SecondName").val();
            type = $("#Organization_window .SecondType").val();
            code = $("#Organization_window .SecondCode").val();
            break;
        case "Property":
            //不允许添加
            break;
    }
    $.post("/Maintenance/Organization/AddNodesOrganization", {
        ListID: ListIDMenuTree,
        Type: TypeMenuTree,
        EnglishName_Full: $("#Organization_window .englishName_Full").val(),
        EnglishAddress_First: $("#Organization_window .englishAddress1").val(),
        EnglishAddress_Second: $("#Organization_window .englishAddress2").val(),
        EnglishAddress_Third: $("#Organization_window .englishAddress3").val(),
        ChineseName_Full: $("#Organization_window .chineseName_Full").val(),
        ChineseAddress_First: $("#Organization_window .chineseAddress1").val(),
        ChineseAddress_Second: $("#Organization_window .chineseAddress2").val(),
        ChineseAddress_Third: $("#Organization_window .chineseAddress3").val(),
        Code: code,
        Type: type,
        Manager: "",
        Position: "",
        ID: "0",
        NodeName: name,
        HasChildNode: "",
        ParentID: item.ID

    }, function (item) {
        var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
        var select = treeview.select();
        if (select.attr("aria-expanded")) {
            treeview.append(item, select);
        }
        else {
            treeview.expand(select);
        }
        $("#Organization_window").data("kendoWindow").close();
    }).fail(function () {
        that.bind("click", confirm);
        hideOperaMask("Organization_window");
    });
}

var GetOrganizationWindow = function () {
    var AddOrganizationWindow = $("#Organization_window").data("kendoWindow");
    if (!AddOrganizationWindow) {
        $("#Organization_window").kendoWindow({
            minWidth: "600px",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#Organization_window .windowCancel").bind("click", orgChartCancel);
                $("#Organization_window .windowConfirm").bind("click", orgChartConfirm);
            },
            close: function (e) {
                $("#Organization_window .windowCancel").unbind("click", orgChartCancel);
                $("#Organization_window .windowConfirm").unbind("click", orgChartConfirm);
                hideOperaMask("Organization_window");
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#Organization_window").data("kendoWindow"));

        var AddOrganizationWindow = $("#Organization_window").data("kendoWindow");
    }
    return AddOrganizationWindow;
}

var addOrgChart = function () {
    AddOrganizationWindow = GetOrganizationWindow();
    AddOrganizationWindow.title("Add Organization").center().open();
    $("#Organization_window").css("overflow", "hidden");
    $("#AddInfo").empty().html($("#OrgChart").html());
    $("#AddInfo .OrganizationName").attr("data-url", "Maintenance/Organization/AddOrganization");
}
var editOrgChart = function () {
    $("#Organization_window").data("kendoWindow").title("Edit Organization").center().open();
    $("#Organization_window").css("overflow", "hidden");
    $("#AddInfo").empty().html($("#OrgChart").html());
    $("#AddInfo .OrganizationName").attr("data-url", "Maintenance/Organization/EditOrganization");

    $("#AddInfo .OrganizationName").val($("#OrganizationDrop").data("kendoDropDownList").text());
}

var initNodePositionList = function () {
    $.get("/Maintenance/Organization/GetPositionByNode", { id: $("#OrganizationManageTreeView_tv_active").find(":checked").val() }, function (items) {

        InitBaseKendoGrid("NodePositionList", PositionModel, findpositioncolumns, items, function () {
            bindGridCheckbox("NodePositionList");
            $("#NodePositionList .k-grid-content").find(":checkbox").click(function () {
                $("#NodePositionList .k-grid-content").find(":checkbox").prop("checked", false);
                $(this).prop("checked", true);
            });
            //$("#NodePositionList .k-grid-content").css("height", "190px")
        });
    })
}
var initNodeUserList = function () {
    $.get("/Maintenance/Organization/GetUserByNode", { id: $("#OrganizationManageTreeView_tv_active").find(":checked").val() }, function (items) {
        InitBaseKendoGrid("NodeUserList", StaffModel, employeecolumns, items, function () {
            bindGridCheckbox("NodeUserList");
            //$("#NodeUserList .k-grid-content").css("height", "190px")
        });
    })
}
var addNodePosition = function () {
    InitSearchposition();
}
var delNodePosition = function () {
    var idList = new Array();
    $("#NodePositionList").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        for (var i = 0; i < idList.length; i++) {
            var dataItem = $("#NodePositionList").data("kendoGrid").dataSource.get(idList[i]);
            $("#NodePositionList").data("kendoGrid").dataSource.remove(dataItem);
        }
    }
    else {
        ShowTip("Please select employee!", "error");
    }
}
var addNodeUserList = function () {
    InitSearchManager();
}
var delNodeUserList = function () {
    var idList = new Array();
    $("#NodeUserList").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        for (var i = 0; i < idList.length; i++) {
            var dataItem = $("#NodeUserList").data("kendoGrid").dataSource.get(idList[i]);
            $("#NodeUserList").data("kendoGrid").dataSource.remove(dataItem);
        }
    }
    else {
        ShowTip("Please select employee!", "error");
    }
}


var InitSearchManager = function () {
    $("#AddOtherWindows").data("kendoWindow").title("Search Manager").center().open();
    $("#AddOtherWindows").css("overflow", "hidden");

    $("#AddOtherWindows .windowConfirm").attr("data-type", "Manager");
    $("#AddOtherWindows .ListGridPosition").hide();
    $("#AddOtherWindows .ListGridManager").show();
    $.get("/Maintenance/Staff/GetStaff", function (items) {
        $("#AddOtherWindows .OtherInput").val("");

        InitBaseKendoGrid("AddOtherWindows .ListGridManager", StaffModel, findstaffcolumns, items, function () {
            bindGridCheckbox("AddOtherWindows .ListGridManager");
            $("#AddOtherWindows .ListGridManager .k-grid-content").css("height", "190px")
        });
    })
}

var SearchPositionORManager = function () {
    var input = $("#AddOtherWindows .OtherInput").val();

    if ($("#AddOtherWindows .windowConfirm").attr("data-type") != "Position") {
        $.post("/Maintenance/Staff/FindNameStaff", { input: input }, function (items) {
            InitBaseKendoGrid("AddOtherWindows .ListGridManager", StaffModel, findstaffcolumns, items, function () {
                bindGridCheckbox("AddOtherWindows .ListGridManager");
                $("#AddOtherWindows .ListGridManager .k-grid-content").css("height", "190px")
            });
        })
    }
    else {
        $.post("/Maintenance/Position/FindPosition", { input: input }, function (items) {

            InitBaseKendoGrid("AddOtherWindows .ListGridPosition", PositionModel, findpositioncolumns, items, function () {
                bindGridCheckbox("AddOtherWindows .ListGridPosition");
                $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").click(function () {
                    $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").prop("checked", false);
                    $(this).prop("checked", true);
                });
                $("#AddOtherWindows .ListGridPosition .k-grid-content").css("height", "190px")
            });
        })
    }
}

var InitSearchposition = function () {
    $("#AddOtherWindows").data("kendoWindow").title("Search Position").center().open();
    $("#AddOtherWindows").css("overflow", "hidden");

    $("#AddOtherWindows .windowConfirm").attr("data-type", "Position");
    $("#AddOtherWindows .ListGridPosition").show();
    $("#AddOtherWindows .ListGridManager").hide();
    $.get("/Maintenance/Position/GetPositionList", function (items) {
        $("#AddOtherWindows .OtherInput").val("");
        InitBaseKendoGrid("AddOtherWindows .ListGridPosition", PositionModel, findpositioncolumns, items, function () {
            bindGridCheckbox("AddOtherWindows .ListGridPosition");
            $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").click(function () {
                $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").prop("checked", false);
                $(this).prop("checked", true);
            });
            $("#AddOtherWindows .ListGridPosition .k-grid-content").css("height", "190px")
        });
    })
}

var SearchCancel = function () {
    $("#AddOtherWindows").data("kendoWindow").close()
}

var SearchConfirm = function () {

    var that = $(this);
    that.unbind("click", SearchConfirm);
    if ($("#AddOtherWindows .windowConfirm").attr("data-type") == "Position") {
        var idList = new Array();
        $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })

        if (idList.length > 0) {
            for (var i = 0; i < idList.length; i++) {
                var item = $("#AddOtherWindows .ListGridPosition").data("kendoGrid").dataSource.get(idList[i]);
                if (!$("#NodePositionList").data("kendoGrid").dataSource.get(idList[i]))
                    $("#NodePositionList").data("kendoGrid").dataSource.add(item);
            }
        }
    }
    else if ($("#AddOtherWindows .windowConfirm").attr("data-type") == "Manager") {
        var idList = new Array();
        $("#AddOtherWindows .ListGridManager .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })

        if (idList.length > 0) {
            for (var i = 0; i < idList.length; i++) {
                var item = $("#AddOtherWindows .ListGridManager").data("kendoGrid").dataSource.get(idList[i]);
                if (!$("#NodeUserList").data("kendoGrid").dataSource.get(idList[i]))
                    $("#NodeUserList").data("kendoGrid").dataSource.add(item);
            }
        }
    }
    $("#AddOtherWindows").data("kendoWindow").close();
}

var addNode = function () {
    var select = $("#OrganizationManageTreeView_tv_active").find("input:checked");
    if (0 == select.length) return;
    var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());

    var name, type, code;
    var AddOrganizationWindow = GetOrganizationWindow();

    switch (item.Type) {
        case "Cluster":
            $("#AddInfo").empty().html($("#property").html());
            $("#Organization_window").show();
            AddOrganizationWindow.title("Add Property").center().open();
            $("#Organization_window").css("overflow", "hidden");
            $("#Organization_window .propertyType").val("Property");
            break;
        case "Company":
            $("#AddInfo").empty().html($("#SecondBasicInfo").html());
            $("#Organization_window").show();
            AddOrganizationWindow.title("Add Division").center().open();
            $("#Organization_window").css("overflow", "hidden");
            $("#Organization_window .SecondType").val("Division");
            break;
        case "Division":
            $("#AddInfo").empty().html($("#SecondBasicInfo").html());
            $("#Organization_window").show();
            AddOrganizationWindow.title("Add Cluster").center().open();
            $("#Organization_window").css("overflow", "hidden");
            $("#Organization_window .SecondType").val("Cluster");
            break;
        case "Property":
            //不允许添加
            break;
    }
}

var delNode = function () {

    var select = $("#OrganizationManageTreeView_tv_active").find("input:checked");
    if (0 == select.length) return;
    bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
        if (result) {
            $.post("/Maintenance/Organization/DeleteNodesOrganization", {
                ListID: ListIDMenuTree,
                Type: TypeMenuTree,
                id: select.val()
            }, function (id) {
                var chartitem = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get(id);
                if (chartitem != null) {
                    $("#OrganizationDrop").data("kendoDropDownList").dataSource.remove(chartitem);
                    var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                    treeview.dataSource.remove(treeview.dataSource.get(select.val()));
                }
                else {
                    var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                    treeview.dataSource.remove(treeview.dataSource.get(id));
                }
                $("#Information").hide();
            });
        }
    });

}

var saveNode = function () {
    showOperaMask("Information");
    var select = $("#OrganizationManageTreeView_tv_active").find("input");
    if (null == select) { hideOperaMask("Information"); return; };
    var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());

    var name, type, code;
    switch (item.Type) {
        case "Company":
            name = $("#BasicInformation .FirstName").val();
            type = $("#BasicInformation .FirstType").val();
            break;
        case "Cluster":
        case "Division":
            name = $("#BasicInformation .SecondName").val();
            type = $("#BasicInformation .SecondType").val();
            code = $("#BasicInformation .SecondCode").val();
            break;
        case "Property":
            name = $("#BasicInformation .propertyName").val();
            code = $("#BasicInformation .propertyCode").val();
            type = $("#BasicInformation .propertyType").val();
            break;
    }
    var positionIdList = new Array();
    var managerIdList = new Array();
    $("#NodeUserList .k-grid-content").find(":checkbox").each(function () {
        managerIdList.push(this.value)
    })
    $("#NodePositionList .k-grid-content").find(":checkbox").each(function () {
        positionIdList.push(this.value)
    })

    $.ajax({
        url: "/Maintenance/Organization/SaveOrganization",
        type: "POST",
        data: {
            PositionIdList: positionIdList,
            ManagerIdList: managerIdList,
            //ListID: ListIDMenuTree,
            //TypeMenuTree: TypeMenuTree,
            EnglishName_Full: $("#BasicInformation .englishName_Full").val(),
            EnglishAddress_First: $("#BasicInformation .englishAddress1").val(),
            EnglishAddress_Second: $("#BasicInformation .englishAddress2").val(),
            EnglishAddress_Third: $("#BasicInformation .englishAddress3").val(),
            ChineseName_Full: $("#BasicInformation .chineseName_Full").val(),
            ChineseAddress_First: $("#BasicInformation .chineseAddress1").val(),
            ChineseAddress_Second: $("#BasicInformation .chineseAddress2").val(),
            ChineseAddress_Third: $("#BasicInformation .chineseAddress3").val(),
            NodeName: name,
            HasChildNode: item.HasChildNode,
            ParentID: item.ParentID,
            Code: code,
            ID: select.val(),
            Type: item.Type
        },
        traditional: true,
        success: function (item) {
            var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
            var model = treeview.dataSource.get(item.ID);
            if (model) {
                model.set(" EnglishName_Full     ", item.EnglishName_Full);
                model.set(" EnglishAddress_First ", item.EnglishAddress_First);
                model.set(" EnglishAddress_Second", item.EnglishAddress_Second);
                model.set(" EnglishAddress_Third ", item.EnglishAddress_Third);
                model.set(" ChineseName_Full     ", item.ChineseName_Full);
                model.set(" ChineseAddress_First ", item.ChineseAddress_First);
                model.set(" ChineseAddress_Second", item.ChineseAddress_Second);
                model.set(" ChineseAddress_Third ", item.ChineseAddress_Third);
                model.set(" Code ", item.Code);
                model.set(" Manager", $("#Information .ManagerName").val());
                model.set(" Position ", $("#Information .Position").val());
                model.set(" ManagerID", $("#Information .ManagerName").attr("data-value"));
                model.set(" PositionID ", $("#Information .Position").attr("data-value"));
                model.set(" NodeName ", item.NodeName);
            }
            var template = kendo.template($("#OrganizationManageTreeView-template").html())
            var target = $("#OrganizationManageTreeView_tv_active .k-state-selected");
            target.html(template({ item: item }));
            $("#OrganizationManageTreeView_tv_active").find("input").first().prop("checked", true);
            hideOperaMask("Information");
            $("#Organization_Save").siblings(".tips").css("visibility", "visible");
        },
        dataType: "json"
    }).fail(function () { hideOperaMask("Information"); })

}

var LoadOrganizationView = function () {
    title = "Organization Management - Kendo UI";
    InitOrganizationSplitter();
    InitOrganizationWindow();
    $.getJSON("/Maintenance/Organization/GetOrganizationsDrop", { _t: new Date() }, function (items) {
        $("#OrganizationDrop").kendoDropDownList({
            dataTextField: "DisplayName",
            dataValueField: "PositionID",
            dataSource: {
                data: items,
                schema: {
                    model: {
                        id: "PositionID",
                        fields: {
                            PositionID: { type: "String" },
                            DisplayName: { type: "String" }
                        }
                    }
                }
            },
            close: function () {
                ListIDMenuTree = $("#OrganizationDrop").val();
                TypeMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get($("#OrganizationDrop").val()).DisplayName;

                InitTreeView();
            }
        });

        ListIDMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.data()[0].PositionID; //默认的listID
        TypeMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get(ListIDMenuTree).DisplayName;
        $("#OrganizationDrop").val(ListIDMenuTree);

        InitTreeView();
    })

    $("#OrganizationManaView .Add").click(addOrgChart);
    $("#OrganizationManaView .Edit").click(editOrgChart);

    $("#Information  .searchManagerName").click(InitSearchManager)
    $("#OtherSearch").click(SearchPositionORManager)
    $("#Information  .searchPosition").click(InitSearchposition)
    $("#AddOtherWindows .windowCancel").click(SearchCancel)
    $("#AddOtherWindows .windowConfirm").click(SearchConfirm)

    //$("#Organization_AddNode").click(addNode)
    //$("#Organization_DeleteNode").click(delNode);

    $("#Organization_Save").click(saveNode);
    $("#ChartsExport").click(ExportCharts);
    $.contextMenu({
        selector: '#OrganizationManageTreeView_tv_active .Type',// input:checked
        callback: function (key, options) {

            switch (key) {
                //case "edit": break;
                case "add": addNode(); break;
                case "delete": delNode(); break;
            }
        },
        items: {
            //"edit": { name: "Edit", icon: "edit" },
            "add": { name: "Add", icon: "add" },
            "delete": { name: "Delete", icon: "delete" }
        }
    });
    $.contextMenu({
        selector: '#OrganizationManageTreeView_tv_active .PropertyType',// input:checked
        callback: function (key, options) {

            switch (key) {
                //case "edit": break;
                //case "add": addNode(); break;
                case "delete": delNode(); break;
            }
        },
        items: {
            //"edit": { name: "Edit", icon: "edit" },
            //"add": { name: "Add", icon: "add" },
            "delete": { name: "Delete", icon: "delete" }
        }
    });
    //$('#OrganizationManageTreeView_tv_active').on('click', function (e) {
    //    console.log('clicked', this);
    //})
}