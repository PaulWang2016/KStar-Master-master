define(function (require, exports, module) {
    function favs() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Report/ReportFavourite/Get_Favourite");
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
    var treeviewsprites;
    var treeviewspritesTreeView = function () {
        $("#treeviewsprites").kendoTreeView({
            template: kendo.template($("#PostionManageTreeView-template").html()),
            dataSource: favs(),
            select: function (e) {                
                var parId = $("#treeviewsprites_tv_active").find("input").first().val();
                //$("#reportList").load("/Report/ReportFavourite/GetReportList?parentID=" + parId);
                $("#reportList").load("/Report/ReportFavourite/GetID", { CateGoryID: parId }, function (response, status, xhr) {
                    $(this).find(".buttonGroup button").prop("disabled", true);
                });
                $("#treeviewsprites").find(":checkbox").prop("checked", false);
                $("#treeviewsprites_tv_active").find(":checkbox").prop("checked", true);
                $('#treeviewsprites .k-state-focused').WinContextMenu({
                    menu: "#PositionContextMenu",
                    removeMenu: '#homeBody',
                    action: function (e) {

                        switch (e.id) {
                            case "AddCategoryContextMenu":
                                AddCategory($("#treeviewsprites_tv_active").find("input").val()); break;
                            case "AddNameMenu": AddName($("#treeviewsprites_tv_active").find("input").attr("data-parentid")); break;
                            case "DelContextMenu": delFav(); break;

                        }
                    }

                });
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
                $("#treeviewsprites").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#treeviewsprites").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
            }
        });
    }
    var InitPositionSplitter = function () {
        $("#treeviewspritesSplitter").kendoSplitter({
            panes: [
                { collapsible: false, size: "300px", min: "250px", max: "450px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        $(window.splitters).push($("#treeviewspritesSplitter").data("kendoSplitter"));
    }

    var AddCategory = function (parentId) {
        var AddCategoryWindow = $("#AddCategoryWindow").data("kendoWindow");
        if (!AddCategoryWindow) {
            $("#AddCategoryWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaPosition.AddCategory,
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
            $(window.splitters).push(AddCategoryWindow)
        }
        $("#CategoryName").val("");
        $("#CategoryDesc").val("");
        $("#AddCategoryWindow").data("kendoValidator").hideMessages();
        if (typeof (parentId) == "string") {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", parentId);
        }
        else {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", "");
        }
        AddCategoryWindow.open();

    }
    var CategoryCancel = function () {
        $("#AddCategoryWindow").data("kendoWindow").close()
    }
    var CategoryConfirm = function () {
        var validator = $("#AddCategoryWindow").data("kendoValidator");
        if (!validator.validate()) {
            return false;
        }

        var that = $(this);
        that.unbind("click", CategoryConfirm);
        showOperaMask("AddCategoryWindow");
        var categoryName = $("#CategoryName").val();
        var categoryDesc = $("#CategoryDesc").val();
        $.post("/Report/ReportFavourite/AddFavouriteCategory", { Name: categoryName, ParnentID: $("#AddCategoryWindow .windowConfirm").attr("data-parentId"), Comment: categoryDesc }, function (item) {
            //添加子节点
            var treeview = $("#treeviewsprites").data("kendoTreeView");
            var select = treeview.select();
            if (item.ParentID == null || item.ParentID == "00000000-0000-0000-0000-000000000000") {
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

            //关闭弹出框
            $("#AddCategoryWindow").data("kendoWindow").close();

        }).fail(function () {
            that.bind("click", CategoryConfirm);
            hideOperaMask("AddCategoryWindow");
        })

    }

    var AddName = function (parentId) {
        var ModifyNameWindow = $("#ModifyNameWindow").data("kendoWindow");
        if (!ModifyNameWindow) {
            $("#ModifyNameWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaPosition.NameReview,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#ModifyNameWindow .windowCancel").bind("click", ModifyNameCancel);
                    $("#ModifyNameWindow .windowConfirm").bind("click", ModifyNameConfirm);
                },
                close: function (e) {
                    $("#ModifyNameWindow .windowCancel").unbind("click", ModifyNameCancel);
                    $("#ModifyNameWindow .windowConfirm").unbind("click", ModifyNameConfirm);
                    hideOperaMask("ModifyNameWindow");
                },
                resizable: false,
                modal: true
            });
            ModifyNameWindow = $("#ModifyNameWindow").data("kendoWindow").center();
            $(window.splitters).push(ModifyNameWindow)
        }
        $("#ModifyNameName").val("");

        $("#ModifyNameWindow").data("kendoValidator").hideMessages();

        if (typeof (parentId) == "string" && parentId != "00000000-0000-0000-0000-000000000000") {
            $("#ModifyNameWindow .windowConfirm").attr("data-parentId", parentId);
        }
        else {
            $("#ModifyNameWindow .windowConfirm").attr("data-parentId", "");
        }
        ModifyNameWindow.open();

    }
    var ModifyNameCancel = function () {
        $("#ModifyNameWindow").data("kendoWindow").close()
    }
    var ModifyNameConfirm = function () {
        var validator = $("#ModifyNameWindow").data("kendoValidator");
        if (!validator.validate()) {
            return false;
        }

        var that = $(this);
        that.unbind("click", ModifyNameConfirm);
        showOperaMask("AddName");
        var reportName = $("#ModifyNameName").val();
        var ID = $("#treeviewsprites_tv_active").find(":checkbox").val();
        $.ajaxFileUpload({
            url: "/Report/ReportFavourite/UpdateModifyName?id=" + ID,
            data: { Name: reportName },
            secureuri: false,

            dataType: 'json',
            success: function (data) {
                $("#ModifyNameWindow").data("kendoWindow").close();
                $("#ModifyNameName").val("");
                treeviewspritesTreeView();
                //添加到右边报表列表
            },
            error: function (data) {

                that.bind("click", ModifyNameConfirm);
                hideOperaMask("UpdateModifyName");
            }
        })
    }
    var delFav = function () {

        var treeView = $('#treeviewsprites').data("kendoTreeView");
        var selectedNode = $("#treeviewsprites_tv_active");
        var selectedId = selectedNode.find("input").val();
        bootbox.confirm(jsResxMaintenance_SeaPosition.Areyousuretodeleteallcontentinthiscategory, function (result) {
            if (result) {
                $.post("Report/ReportFavourite/DeleteFav", { favid: selectedId }, function (data) {
                    if (data) {
                        treeView.remove(selectedNode);
                    }
                });
            }
        });
    }
    function LoadPostionView() {

        InitPositionSplitter();
        treeviewspritesTreeView();
        $("#CategoryAddCollect").click(function () { AddCategory("00000000-0000-0000-0000-000000000000") })
    }
    module.exports = LoadPostionView;
})
//********************************  Report Management Center***********************
//$(function () {
//    var data = [
//        { text: "上架", value: "1" },
//        { text: "下架", value: "2" },
//    ];
//    $("#keySearch").kendoAutoComplete({
//        placeholder: "按名称, 责任部门,内部编号,说明关键字"
//    });
//    $("#color").kendoDropDownList({
//        dataTextField: "text",
//        dataValueField: "value",
//        dataSource: data,
//        index: 0,
//        change: onChange
//    });
//    function onChange() {
//        var value = $("#colorLevel").val();
//        $("#cap")
//        .toggleClass("black-cap", value == 1)
//        .toggleClass("orange-cap", value == 2)
//    };
//    var data = [
//        { text: "—所有级别—", value: "1" },
//        { text: "员工级", value: "2" },
//        { text: "部门级", value: "3" },
//        { text: "公司级", value: "4" }
//    ];
//    $("#colorLevel").kendoDropDownList({
//        dataTextField: "text",
//        dataValueField: "value",
//        dataSource: data,
//        index: 0,
//        change: onChange
//    });
//    function onChange() {
//        var value = $("#colorLevel").val();
//        $("#cap")
//        .toggleClass("black-cap", value == 1)
//        .toggleClass("orange-cap", value == 2)
//        .toggleClass("grey-cap", value == 3)
//        .toggleClass("gre-cap", value == 4);
//    };
//    var data = [
//       { text: "—所有类型—", value: "1" },
//       { text: "人事类", value: "2" },
//       { text: "财务类", value: "3" },
//       { text: "流程绩效类", value: "4" }
//    ];
//    $("#colorcategory").kendoDropDownList({
//        dataTextField: "text",
//        dataValueField: "value",
//        dataSource: data,
//        index: 0,
//        change: onChange
//    });
//    function onChange() {
//        var value = $("#colorcategory").val();
//        $("#cap")
//            .toggleClass("black-cap", value == 1)
//            .toggleClass("orange-cap", value == 2)
//            .toggleClass("grey-cap", value == 3);
//    };
//    $("#textButton").kendoButton();
//    $("#TimeTextButton").kendoButton();
//    $("#gradeTextButton").kendoButton();
//    $("#departmentTextButton").kendoButton();
//    $("#typeTextButton").kendoButton();
//    $("#levelTextButton").kendoButton();
//    function startChange() {
//        var startDate = start.value(),
//        endDate = end.value();

//        if (startDate) {
//            startDate = new Date(startDate);
//            startDate.setDate(startDate.getDate());
//            end.min(startDate);
//        } else if (endDate) {
//            start.max(new Date(endDate));
//        } else {
//            endDate = new Date();
//            start.max(endDate);
//            end.min(endDate);
//        }
//    }

//    function endChange() {
//        var endDate = end.value(),
//        startDate = start.value();

//        if (endDate) {
//            endDate = new Date(endDate);
//            endDate.setDate(endDate.getDate());
//            start.max(endDate);
//        } else if (startDate) {
//            end.min(new Date(startDate));
//        } else {
//            endDate = new Date();
//            start.max(endDate);
//            end.min(endDate);
//        }
//    }

//    var start = $("#start").kendoDatePicker({
//        change: startChange
//    }).data("kendoDatePicker");

//    var end = $("#end").kendoDatePicker({
//        change: endChange
//    }).data("kendoDatePicker");

//    start.max(end.value());
//    end.min(start.value());
//});
//******************************** .ReportEditor**********************************

$(function () {
    $("#MyFavouritereportEditorDepartment").kendoComboBox();
    $("#MyFavouriteEditorreportCategory").kendoComboBox();
    $("#MyFavouriteEditorreportLevel").kendoComboBox();
    function isInArray(date, dates) {
        for (var idx = 0, length = dates.length; idx < length; idx++) {
            var d = dates[idx];
            if (date.getFullYear() == d.getFullYear() &&
                date.getMonth() == d.getMonth() &&
                date.getDate() == d.getDate()) {
                return true;
            }
        }
        return false;
    }

    $(document).ready(function () {
        var today = new Date(),
            birthdays = [
               new Date(today.getFullYear(), today.getMonth(), 11, 10, 0, 0),
               new Date(today.getFullYear(), today.getMonth(), 11, 10, 30, 0),
               new Date(today.getFullYear(), today.getMonth(), 11, 14, 0, 0),
               new Date(today.getFullYear(), today.getMonth() + 1, 6, 20, 0, 0),
               new Date(today.getFullYear(), today.getMonth() + 1, 27, 8, 0, 0),
               new Date(today.getFullYear(), today.getMonth() + 1, 27, 18, 0, 0),
               new Date(today.getFullYear(), today.getMonth() - 1, 3, 12, 0, 0),
               new Date(today.getFullYear(), today.getMonth() - 2, 22, 16, 30, 0)
            ];

        $("#MyFavouriteEditordatepicker").kendoDateTimePicker({
            value: today,
            dates: birthdays,
            month: {
                content: '# if (isInArray(data.date, data.dates)) { #' +
                             '<div class="birthday"></div>' +
                         '# } #' +
                         '#= data.value #'
            },
            footer: "Today - #=kendo.toString(data, 'd') #",
            open: function () {
                var dateViewCalendar = this.dateView.calendar;
                if (dateViewCalendar) {
                    dateViewCalendar.element.width(300);
                }
            }
        });
    });
});
//*********************************.AddFeedback  **********************************
$(function () {
    var oStar = document.getElementById("star");
    var aLi = oStar.getElementsByTagName("li");
    var oUl = oStar.getElementsByTagName("ul")[0];
    var i = iScore = iStar = 0;
    for (i = 1; i <= aLi.length; i++) {
        aLi[i - 1].index = i;
        //鼠标移过显示分数
        aLi[i - 1].onmouseover = function () {
            fnPoint(this.index);
        };
        //鼠标离开后恢复上次评分
        aLi[i - 1].onmouseout = function () {
            fnPoint();
            //关闭浮动层
        };
        //点击后进行评分处理
        aLi[i - 1].onclick = function () {
            iStar = this.index;
        }
    }
    //评分处理
    function fnPoint(iArg) {
        //分数赋值
        iScore = iArg || iStar;
        for (i = 0; i < aLi.length; i++) aLi[i].className = i < iScore ? "on" : "";
    }
    $("#confirmAddFeedback").click(function () {
        showOperaMask("AddFeedbackWindow");
        var star = iScore;
        var FeedbackDesc = $("#FeedbackDesc").val();
        var datp = tempId;
        $.post("/Report/Feedback/AddFeedback", { Rate: star, Comment: FeedbackDesc, ReportInfoID: datp }, function (Report) {
            $("#AddFeedbackWindow").data("kendoWindow").close();
            $("#FeedbackDesc").val("");
            $("#star ul li").removeClass("on");
        }).fail(function () {
            hideOperaMask("AddFeedbackWindow");
        })
    });
});
//*********************************2014.5.23.yinhui ***************************************
function MyFavouriteReportEditor(reportid) {
    var MyFavouriteReportEditor = $("#MyFavouriteReportEditor").data("kendoWindow");
    if (!MyFavouriteReportEditor) {
        $("#MyFavouriteReportEditor").kendoWindow({
            width: "500px",
            title: jsResxMaintenance_SeaPosition.EditorReport,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#MyFavouriteReportEditor .windowCancel").bind("click", MyFavouriteReportEditorCancel);
                $("#MyFavouriteReportEditor .windowConfirm").bind("click", MyFavouriteReportEditorConfirm);
            },
            close: function (e) {
                $("#MyFavouriteReportEditor .windowCancel").unbind("click", MyFavouriteReportEditorCancel);
                $("#MyFavouriteReportEditor .windowConfirm").unbind("click", MyFavouriteReportEditorConfirm);
                hideOperaMask("MyFavouriteReportEditor");
            },
            resizable: false,
            modal: true
        });
        MyFavouriteReportEditor = $("#MyFavouriteReportEditor").data("kendoWindow").center();
        $(window.splitters).push(MyFavouriteReportEditor);
    }

    $.post("/Report/ReportInfo/GetReport", { id: reportid }, function (data) {
        $("#MyFavouritereportEditorName").val(data.Name);
        $("#MyFavouritereportEditorDepartment").data("kendoComboBox").value(data.Department);
        $("#MyFavouriteEditordatepicker").val(data.PublishedDate);
        $("#MyFavouriteEditorreportCode").val(data.ReportCode);
        $("#MyFavouriteEditorreportLevel").data("kendoComboBox").value(data.Level);
        $("#MyFavouriteEditorreportCategory").data("kendoComboBox").value(data.Category);
        $("#MyFavouriteEditorreportComment").val(data.Comment);
        $("#MyFavouriteEditortextfield").val(data.ImageThumbPath);
        $("#MyFavouriteEditorreportUrl").val(data.ReportUrl);

        if (data.Status == $("#MyFavouriteEditorreportStatus").find("input:first").val()) {
            $("#MyFavouriteEditorreportStatus").prop("checked", true);//.find("input:first").attr("checked", "checked");
        } else {
            $("#MyFavouriteEditorreportStatus").prop("checked", true);//.find("input:last").attr("checked", "checked");
        }
    }, 'json')

    if (typeof (reportid) == "string") {
        $("#MyFavouriteReportEditor .windowConfirm").attr("data-reportid", reportid);
    }
    else {
        $("#MyFavouriteReportEditor .windowConfirm").attr("data-reportid", "");
    }

    MyFavouriteReportEditor.open();
}
function MyFavouriteReportEditorCancel() {
    $("#MyFavouriteReportEditor").data("kendoWindow").close();
}

var MyFavouriteReportEditorConfirm = function () {
    var that = $(this);
    that.unbind("click", MyFavouriteReportEditorConfirm);
    showOperaMask("MyFavouriteReportEditor");
    var reportName = $("#MyFavouritereportEditorName").val();
    var reportDepartment = $("#MyFavouritereportEditorDepartment").data("kendoComboBox").value();
    var reportLevel = $("#MyFavouriteEditorreportLevel").data("kendoComboBox").value();
    var reportCode = $("#MyFavouriteEditorreportCode").val();
    var reportDate = $("#MyFavouriteEditordatepicker").val();
    var reportCategory = $("#MyFavouriteEditorreportCategory").data("kendoComboBox").value();
    var reportComment = $("#MyFavouriteEditorreportComment").val();
    var reportImageThumbPath = $("#MyFavouriteEditortextfield").val();
    var reportid = $(this).attr("data-reportid");
    var reportstatus = $("#MyFavouriteEditorreportStatus").find("input:checked").val();
    var reportUrl = $("#MyFavouriteEditorreportUrl").val();
    $.ajaxFileUpload({
        url: '/Report/ReportInfo/UpdateReport',
        data: { Name: reportName, Department: reportDepartment, PublishedDate: reportDate, Level: reportLevel, Category: reportCategory, ReportCode: reportCode, Comment: reportComment, ImageThumbPath: reportImageThumbPath, ID: reportid, Status: reportstatus, ReportUrl: reportUrl },
        secureuri: false,
        fileElementId: 'MyFavouriteEditorfileField',
        dataType: 'json',
        success: function (data, status) {
            var parId = $("#treeviewsprites_tv_active").find("input").first().val();
            $("#reportList").load("/Report/ReportFavourite/GetReportList?parentID=" + parId);
            //添加到右边报表列表
            $("#MyFavouriteReportEditor").data("kendoWindow").close();
            $("#MyFavouritereportEditorName").val("");
            //$("#datepicker").val("");
            $("#MyFavouriteEditorreportLevel").val("");
            $("#MyFavouriteEditorreportCode").val("");
            $("#MyFavouriteEditorreportCategory").val("");
            $("#MyFavouriteEditorreportComment").val("");
            $("#MyFavouritereportEditorDepartment").val("");
            $("#MyFavouriteEditorfileField").val("");
            $("#MyFavouriteEditortextfield").val("");
            $("#MyFavouriteEditorreportStatus").val("");
            $("#MyFavouriteEditorreportUrl").val("");
        },
        error: function (data, status, e) {
            that.bind("click", ReportConfirm);
            hideOperaMask("UpdateReport");
        }
    })
}
var Feedback = function (parentId) {
    tempId = parentId;
    var p = tempId;
    var AddFeedbackWindow = $("#AddFeedbackWindow").data("kendoWindow");
    if (!AddFeedbackWindow) {
        $("#AddFeedbackWindow").kendoWindow({
            width: "500px",
            title: jsResxMaintenance_SeaPosition.Feedback,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddFeedbackWindow .windowCancel").bind("click", AddFeedbackCancel);
                $("#AddFeedbackWindow .windowConfirm").bind("click", AddFeedbackConfirm);
            },
            close: function (e) {
                $("#AddFeedbackWindow .windowCancel").unbind("click", AddFeedbackCancel);
                $("#AddFeedbackWindow .windowConfirm").unbind("click", AddFeedbackConfirm);
                hideOperaMask("AddFeedbackWindow");
            },
            resizable: false,
            modal: true
        });
        AddFeedbackWindow = $("#AddFeedbackWindow").data("kendoWindow").center();
        $(window.splitters).push(AddFeedbackWindow);
    }
    AddFeedbackWindow.open();
}

var AddFeedbackCancel = function () {
    $("#AddFeedbackWindow").data("kendoWindow").close();
}
var AddFeedbackConfirm = function () {

}

function favReport(id) {
    var AddFavouriteWindow = $("#AddFavouriteWindow").data("kendoWindow");
    if (!AddFavouriteWindow) {
        $("#AddFavouriteWindow").kendoWindow({
            width: "500px",
            title: jsResxMaintenance_SeaPosition.FavoriteReport,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddFavouriteWindow .windowCancel").bind("click", FavCancel);
                $("#AddFavouriteWindow .windowConfirm").bind("click", FavConfirm);
            },
            close: function (e) {
                $("#AddFavouriteWindow .windowCancel").unbind("click", FavCancel);
                $("#AddFavouriteWindow .windowConfirm").unbind("click", FavConfirm);
                hideOperaMask("AddFavouriteWindow");
            },
            resizable: false,
            modal: true
        });
        AddFavouriteWindow = $("#AddFavouriteWindow").data("kendoWindow").center();
        $(window.splitters).push(AddFavouriteWindow);
    }

    if (typeof (id) == "string") {
        $("#AddFavouriteWindow .windowConfirm").attr("data-reportid", id);
    }
    else {
        $("#AddFavouriteWindow .windowConfirm").attr("data-reportid", "");
    }
    AddFavouriteWindow.open();
}
function FavCancel() {
    $("#AddFavouriteWindow").data("kendoWindow").close();
}

