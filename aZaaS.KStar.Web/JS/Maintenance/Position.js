var Positions = {
    type: "json",
    schema: {
        model: {
            id: "PositionID",
            hasChildren: false,
            fields: {
                PositionID: { type: "String" },
                DisplayName: { type: "String" }
            }
        }
    },
    transport: {
        read: {
            url: function (options) {
                return kendo.format("/Maintenance/Position/GetPosition?id={0}", options.CategoryID);
            }
        }
    }
};

var Categories = new kendo.data.HierarchicalDataSource({
    type: "json",
    transport: {
        read: {
            url: "/Maintenance/Position/GetCategory"
        }
    },
    schema: {
        model: {
            hasChildren: true,
            id: "CategoryID",
            children: Positions,
            fields: {
                CategoryID: { type: "String" },
                DisplayName: { type: "String" }
            }
        }
    }
});
//===============================Position View====================================
var CategoryCancel = function () {
    $("#AddCategoryWindow").data("kendoWindow").close()
}
var CategoryConfirm = function () {
    var that = $(this);
    that.unbind("click", CategoryConfirm);
    showOperaMask("AddCategoryWindow");
    var categoryName = $("#CategoryName").val();
    var categoryDesc = $("#CategoryDesc").val();
    $.post("/Maintenance/Position/CreateCategory", { DisplayName: categoryName }, function (item) {
        Categories.add(item);
        $("#AddCategoryWindow").data("kendoWindow").close();
    }).fail(function () {
        that.bind("click", CategoryConfirm);
        hideOperaMask("AddCategoryWindow");
    })
}

var PositionCancel = function () {
    $("#AddPositionWindow").data("kendoWindow").close()
}
var PositionConfirm = function () {
    var that = $(this);
    that.unbind("click", PositionConfirm);
    showOperaMask("AddPositionWindow");
    var positionName = $("#PositionName").val();
    var positionDesc = $("#PositionDesc").val();
    //var id = $("#PostionManageTreeView_tv_active").find("input").val();
    //var item = Positions.get(id);
    //var parentID = item.ParentID == 0 ? item.PositionID : item.ParentID;
    var node = $("#PostionManageTreeView_tv_active").find("input");
    var haschild = node.attr("data-parentid");

    var parentID;
    if (haschild != 0) {
        parentID = node.attr("data-parentid");
    }
    else {
        parentID = node.val();
    }
    $.post("/Maintenance/Position/CreatePosition", { DisplayName: positionName, CategoryID: parentID }, function (item) {
        var parent = Categories.get(item.CategoryID);
        parent.children.add(item);
        $("#AddPositionWindow").data("kendoWindow").close()
    }).fail(function () {
        that.bind("click", PositionConfirm);
        hideOperaMask("AddPositionWindow");
    })
}
//===============================Position View====================================

var PostionManageTreeView;
var InitPostionManageTreeView = function () {
    $("#PostionManageTreeView").kendoTreeView({
        template: kendo.template($("#PostionManageTreeView-template").html()),
        dataSource: Categories,
        select: function (e) {
            $("#itemSave").siblings(".tips").css("visibility", "hidden");
            $("#PostionManageTreeView").find("input").prop("checked", false);
            var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
            var haschild = node.attr("data-parentid");
            $("#itemId").val(node.val());
            $("#itemName").val(node.attr("data-DisplayName"));
            $("#itemParentID").val(node.attr("data-parentid"))

            if (haschild != 0) {
                $("#PostionInfomation .info-title").text("Position Infomation");
                //$("#PostionManageTreeView").parent().siblings().last().children("ul").show();
                $("#PostionInfomation>ul>li").last().show();
                $.getJSON("/Maintenance/Position/GetEmployee", { _t: new Date(), id: node.val() }, function (items) {
                    $("#employeeList").children().show();
                    InitBaseKendoGrid("employeeList", StaffModel, employeecolumns, items, function () {
                        bindGridCheckbox("employeeList")
                    });
                })
            }
            else {
                $("#PostionInfomation .info-title").text("Category Infomation");
                //$("#PostionManageTreeView").parent().siblings().last().children("ul").last().hide();
                $("#PostionInfomation>ul>li").last().hide();
            }

        },
        collapse: function (e) {
            $("#PostionManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
        },
        expand: function (e) {
            $("#PostionManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
        },
        dataBound: function (e) {
            var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#PostionManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#PostionManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
        }
    });
     PostionManageTreeView = $("#PostionManageTreeView").data("kendoTreeView");
}
var InitPositionSplitter = function () {
    $("#PositionManaView").kendoSplitter({
        panes: [
            { collapsible: false, size: "300px", min: "250px", max: "450px", resizable: true },
            { collapsible: false, resizable: true }
        ]
    });
    AddSplitters($("#PositionManaView").data("kendoSplitter"));
}
var AddCategory = function () {
    var AddCategoryWindow = $("#AddCategoryWindow").data("kendoWindow");
    if (!AddCategoryWindow) {
        $("#AddCategoryWindow").kendoWindow({
            width: "500px",
            title: "Add Category",
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
        AddSplitters($("#AddCategoryWindow").data("kendoWindow"));

        AddCategoryWindow = $("#AddCategoryWindow").data("kendoWindow").center();
    }
    $("#CategoryName").val("");
    $("#CategoryDesc").val("");
    AddCategoryWindow.open();

}
var AddPosition = function () {
    var AddPositionWindow = $("#AddPositionWindow").data("kendoWindow");
    if (!AddPositionWindow) {
        $("#AddPositionWindow").kendoWindow({
            width: "500px",
            title: "Add Position",
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddPositionWindow .windowCancel").bind("click", PositionCancel);
                $("#AddPositionWindow .windowConfirm").bind("click", PositionConfirm);
            },
            close: function (e) {
                $("#AddPositionWindow .windowCancel").unbind("click", PositionCancel);
                $("#AddPositionWindow .windowConfirm").unbind("click", PositionConfirm);
                hideOperaMask("AddPositionWindow");
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddPositionWindow").data("kendoWindow"));

        AddPositionWindow = $("#AddPositionWindow").data("kendoWindow").center();
    }

    $("#PositionName").val("");
    $("#PositionDesc").val("");
    if (PostionManageTreeView.select().length > 0) {
        AddPositionWindow.open();
    }
    else {
        ShowTip("Please select category to add position!");
    }
}
var SaveItem = function () {
    showOperaMask("PostionInfomation");
    var id = $("#PostionManageTreeView_tv_active").find("input").val();
    var itemName = $("#itemName").val();

    if (id != "" && itemName != "") {
        var node = $("#PostionManageTreeView_tv_active").find("input");
        if (node.attr("data-parentid") == 0) {
            $.post("/Maintenance/Position/UpdateCategory", { CategoryID: id, DisplayName: itemName }, function (item) {

                var template = kendo.template($("#PostionManageTreeView-template").html())
                var target = $("#PostionManageTreeView_tv_active .k-state-selected");
                target.html(template({ item: item }));
                if (target.parent().parent().attr("aria-expanded")) {
                    target.find(".k-sprite").first().addClass("on");
                }
                hideOperaMask("PostionInfomation");
                $("#itemSave").siblings(".tips").css("visibility", "visible");
            }).fail(function () { hideOperaMask("PostionInfomation"); })
        }
        else {
            $.post("/Maintenance/Position/UpdatePosition", { PositionID: id, DisplayName: itemName, CategoryID: node.attr("data-parentid") }, function (item) {

                //var node = Positions.get(item.PositionID);
                //node.set("DisplayName", item.DisplayName);//更新数据
                var template = kendo.template($("#PostionManageTreeView-template").html())
                var target = $("#PostionManageTreeView_tv_active .k-state-selected");
                target.html(template({ item: item }));
                hideOperaMask("PostionInfomation");
                $("#itemSave").siblings(".tips").css("visibility", "visible");
            }).fail(function () { hideOperaMask("PostionInfomation"); })
        }
    }
    else {
        ShowTip("Please input name!", "error");
        hideOperaMask("PostionInfomation");
    }
}
var AddEmployee = function () {
   var AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
    if (!AddEmployeeWindow) {
        $("#AddEmployeeWindow").kendoWindow({
            width: "500px",
            height: "350px",
            title: "Add Employee",
            actions: [
                "Close"
            ],
            open: function (e) {
                $.get("/Maintenance/Staff/GetStaff", function (items) {
                    $("#EmployeeInput").val("");
                    InitBaseKendoGrid("StaffList", StaffModel, findstaffcolumns, items, function () {
                        bindGridCheckbox("StaffList");
                        $("#StaffList .k-grid-content").css("height", "190px")
                    });
                })
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddEmployeeWindow").data("kendoWindow"));

        AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
        AddEmployeeWindow.center();
        $("#EmployeeSelect").click(function () {
            var input = $("#EmployeeInput").val();
            $.post("/Maintenance/Staff/FindNameStaff", { input: input }, function (items) {
                InitBaseKendoGrid("StaffList", StaffModel, findstaffcolumns, items, function () {
                    bindGridCheckbox("StaffList");
                    $("#StaffList .k-grid-content").css("height", "190px")
                });
            })
        })
        $("#AddEmployeeWindow .windowCancel").click(function () {
            AddEmployeeWindow.close()
        })
        $("#AddEmployeeWindow .windowConfirm").click(function () {
            var idList = new Array();
            var id = $("#PostionManageTreeView_tv_active").find("input").val();
            $("#StaffList .k-grid-content").find(":checked").each(function () {
                idList.push(this.value)
            })

            if (idList.length > 0) {
                $.ajax({
                    url: "/Maintenance/Position/DoCreateEmployee",
                    type: "POST",
                    data: { id: id, idList: idList },
                    traditional: true,
                    success: function (items) {
                        for (var i = 0; i < items.length; i++) {
                            $("#employeeList").data("kendoGrid").dataSource.add(items[i]);
                        }
                        AddEmployeeWindow.close()
                    },
                    dataType: "json"
                })
            }
        })
    }
    AddEmployeeWindow.open();
}
var DelEmployee = function () {
    var idList = new Array();
    var id = $("#PostionManageTreeView_tv_active").find("input").val();
    $("#employeeList .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        $.ajax({
            url: "/Maintenance/Position/DoDestroyEmployee",
            type: "POST",
            data: { id: id, idList: idList },
            traditional: true,
            success: function (idList) {
                for (var i = 0; i < idList.length; i++) {
                    var dataItem = $("#employeeList").data("kendoGrid").dataSource.get(idList[i]);
                    $("#employeeList").data("kendoGrid").dataSource.remove(dataItem);
                }
            },
            dataType: "json"
        })
    }
    else {
        ShowTip("Please select employee!", "error");
    }
}
var DelPosition = function () {
    var node = $("#PostionManageTreeView_tv_active").find("input:checked");

    if (node.attr("data-parentid") != "0" && node.val() != null && node.length > 0) {
        bootbox.confirm("Are you sure to delete this position?", function (result) {
            if (result) {
                $.post("/Maintenance/Position/DestroyPosition", { positionID: node.val() }, function (positionID) {
                    var nodeposition = PostionManageTreeView.dataSource.get(node.attr("data-parentid")).children.get(positionID);
                    PostionManageTreeView.dataSource.remove(nodeposition);
                    $("#itemName").val("");
                    $("#PostionManageTreeView").parent().siblings().last().children("ul").last().hide();
                    //$("#PostionManageTreeView_tv_active").find("input").parent().parent().parent().remove();
                });
            }
        });
    }
    else if (node.attr("data-parentid") == 0 && node.val() != null) {
        bootbox.confirm("Are you sure to delete this category?", function (result) {
            if (result) {
                $.post("/Maintenance/Position/DestroyCategory", { categoryID: node.val() }, function (categoryID) {
                    PostionManageTreeView.dataSource.remove(PostionManageTreeView.dataSource.get(categoryID));
                    $("#PostionManageTreeView").parent().siblings().last().children("ul").last().hide();
                });
            }
        });
    }
    else { ShowTip("Please select the Position or Category to be removed!", "error"); }
}

function LoadPostionView() {
    title = "Postion Management - Kendo UI";
    InitPositionSplitter();
    InitPostionManageTreeView();
    $("#CategoryAdd").click(AddCategory)
    $("#PositionAdd").click(AddPosition)
    $("#itemSave").click(SaveItem)
    var AddEmployeeWindow;
    $("#EmployeeAdd").click(AddEmployee)
    $("#EmployeeDel").click(DelEmployee)

    $.contextMenu({
        selector: '#PostionManageTreeView_tv_active .k-state-selected',//input:checked .k-in .k-state-selected
        callback: function (key, options) {
            switch (key) {
                case "add": AddPosition(); break;
                case "delete": DelPosition(); break;
            }
        },
        items: {
            "add": { name: "Add", icon: "add" },
            "delete": { name: "Delete", icon: "delete" }
        }
    });
    $("#PostionInfomation").children("ul").kendoPanelBar();
}