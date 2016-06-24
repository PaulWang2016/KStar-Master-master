//===============================Staff View====================================
function resetAddStaffWindow() {
    hideOperaMask("AddStaffWindow");
    $("#staffTab .k-textbox").val("");//清除输入框

    $("#StaffSex").data("kendoDropDownList").select(0);//清除下拉框
    $("#StaffStatus").data("kendoDropDownList").select(0);
    $("#StaffPosition").data("kendoDropDownList").select(0);
}//重置表单

var StaffCancel = function () {
    $("#AddStaffWindow").data("kendoWindow").close()
}
var StaffConfirm = function () {
    var that = $(this);
    that.unbind("click", StaffConfirm);
    showOperaMask("AddStaffWindow");
    var ReportToidlist = new Array();
    $("#ReportToList .k-grid-content").find("input").each(function () {
        ReportToidlist.push(this.value);
    })
    var Roleidlist = new Array();
    $("#RoleList .k-grid-content").find("input").each(function () {
        Roleidlist.push(this.value);
    })
    var Positionidlist = new Array();
    $("#PositionList .k-grid-content").find("input").each(function () {
        Positionidlist.push(this.value);
    })
    var url = $(this).attr("data-url");
    var data = {
        StaffId: $("#AddStaffWindow .windowConfirm").attr("data-id"),
        StaffNo: $("#StaffNo").val(),
        FirstName: $("#FirstName").val(),
        LastName: $("#LastName").val(),
        DisplayName: $("#FirstName").val() + " " + $("#LastName").val(),
        UserName: $("#StaffName").val(),
        ChineseName: $("#ChineseName").val(),
        Email: $("#Email").val(),
        TelNo: $("#TelNo").val(),
        FaxNo: $("#FaxNo").val(),
        MobileNo: $("#MobileNo").val(),
        JobTitle: $("#JobTitle").val(),
        Department: $("#Department").val(),
        JobClass: $("#JobClass").val(),
        JobRank: $("#JobRank").val(),
        Status: $("#StaffStatus").val(),
        Sex: $("#StaffSex").val(),
        Remark: $("#Remark").val(),
        Address: "",
        ReportToidlist: ReportToidlist,
        Roleidlist: Roleidlist,
        Positionidlist: Positionidlist,
    }

    $.ajax({
        url: url,
        type: "POST",
        data: data,
        traditional: true,
        success: function (item) {

            var grid = getKendoGrid("UserManaView");
            var model = grid.dataSource.get(item.StaffId);

            if (model) {
                for (var key in item) {
                    model.set(key, item[key]);
                }
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddStaffWindow").data("kendoWindow").close()
        },
        dataType: "json"
    }).fail(function () {
        hideOperaMask("AddStaffWindow");
        that.bind("click", StaffConfirm);
    })
}

var OpenAddStaff = function (e) {
    $("#AddStaffWindow .windowCancel").bind("click", StaffCancel)
    $("#AddStaffWindow .windowConfirm").bind("click", StaffConfirm)
    $("#staffOtherTab .mt-Add").bind("click", OtherAdd)
    $("#staffOtherTab .mt-Delete").bind("click", OtherDel)
}
var CloseAddStaff = function (e) {
    resetAddStaffWindow();
    $("#AddStaffWindow .windowCancel").unbind("click", StaffCancel)
    $("#AddStaffWindow .windowConfirm").unbind("click", StaffConfirm)
    $("#staffOtherTab .mt-Add").unbind("click", OtherAdd)
    $("#staffOtherTab .mt-Delete").unbind("click", OtherDel)
}

var OtherCancel = function () {
    $("#AddOtherWindow").data("kendoWindow").close()
}
var OtherConfirm = function () {
    var that = $(this);
    that.unbind("click", OtherConfirm);
    showOperaMask("AddOtherWindow");
    var id = $("#AddOther").val();
    var item = $("#AddOther").data("kendoDropDownList").dataSource.get(id);
    if (id != "") {
        switch ($("#AddOtherWindow_wnd_title").text()) {
            case "ReportTo": $("#ReportToList").data("kendoGrid").dataSource.add(item);
                break
            case "Role": $("#RoleList").data("kendoGrid").dataSource.add(item);
                break;
            case "Position": $("#PositionList").data("kendoGrid").dataSource.add(item);
                break;
        }

        $("#AddOtherWindow").data("kendoWindow").close()
    }
    else {
        that.bind("click", OtherConfirm);
        hideOperaMask("AddOtherWindow");
    }
}

var OpenAddOther = function (e) {
    $("#AddOtherWindow .windowCancel").bind("click", OtherCancel);
    $("#AddOtherWindow .windowConfirm").bind("click", OtherConfirm);
}
var CloseAddOther = function (e) {
    hideOperaMask("AddOtherWindow");
    $("#AddOtherWindow .windowCancel").unbind("click", OtherCancel);
    $("#AddOtherWindow .windowConfirm").unbind("click", OtherConfirm);
}

var OtherAdd = function () {
    var target = $(this);
    var tt = target.attr("data-url");
    $.getJSON(target.attr("data-url"), { _t: new Date() }, function (items) {
        InitDropDownList("AddOther", target.attr("data-text"), target.attr("data-value"), target.attr("data-label"), items)
    })
    $("#AddOtherWindow").data("kendoWindow").center().title(target.attr("data-wtitle")).open();
}
var OtherDel = function () {
    var idlist = new Array();
    $(this).parents(".k-content").first().find(".k-grid-content").find(":checked").each(function () {
        idlist.push(this.value);
    })
    var grid = $(this).parents(".k-content").first().find(".k-grid.k-widget").data("kendoGrid");
    for (var i = 0; i < idlist.length; i++) {
        var item = grid.dataSource.get(idlist[i]);
        grid.dataSource.remove(item);
    }
}

//===============================Staff View====================================
var InitUserWindow = function () {
    var width = $(window).width() - 100 + "px";
    var height = $(window).height() - 100 + "px";
    var heig = $(window).height();
    $("#staffTab").height(heig - 160);

    $("#AddStaffWindow").kendoWindow({
        width: width,
        height: height,
        title: "Add Staff",
        actions: [
            "Close"
        ],
        open: OpenAddStaff,
        close: CloseAddStaff,
        resizable: false,
        modal: true
    });
    $("#AddOtherWindow").kendoWindow({
        width: "300px",
        title: "Add Other",
        animation: false,
        actions: [
            "Close"
        ],
        open: OpenAddOther,
        close: CloseAddOther,
        resizable: false,
        modal: true
    });
}
var InitUserWindowView = function () {
    //===============================TabStrip====================================
    $("#staffTab").children().kendoPanelBar({
        //collapse: function () {
        //    $("#staffTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-selected");
        //    $("#staffTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-focused");
        //},
        //expand: function () {
        //    $("#staffTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-selected");
        //    $("#staffTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-focused");
        //}
    });
    $("#staffOtherTab").children().kendoPanelBar({
        //collapse: function () {
        //    $("#staffOtherTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-selected");
        //    $("#staffOtherTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-focused");
        //},
        //expand: function () {
        //    $("#staffOtherTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-selected");
        //    $("#staffOtherTab").children("ul").find(".k-header.k-state-selected").removeClass("k-state-focused");
        //}
    });
    //===============================DropDownList====================================
    $("#StaffSex").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [{ text: "Male", value: "Male" }, { text: "Female", value: "Female" }],
        optionLabel: "--Select Sex--"
    });
    $("#StaffStatus").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [{ text: "Leave", value: "false" }, { text: "Work", value: "true" }],
        optionLabel: "--Select Status--"
    });
    //Extended Information
    $.getJSON("/Maintenance/Staff/GetPositionList", { _t: new Date() }, function (items) {
        $("#StaffPosition").kendoDropDownList({
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
            optionLabel: "--Select Position--"
        });
    })
    //===============================DropDownList====================================

}
var AddUser = function () {
    $("#AddStaffWindow .windowConfirm").attr("data-url", "/Maintenance/Staff/AddStaff");
    InitBaseKendoGrid("ReportToList", StaffModel, reporttocolumns, [], function () {
        bindGridCheckbox("ReportToList");
    });
    InitBaseKendoGrid("RoleList", RoleModel, rolecolumns, [], function () {
        bindGridCheckbox("RoleList");
    });
    InitBaseKendoGrid("PositionList", PositionModel, positioncolumns, [], function () {
        bindGridCheckbox("PositionList");
    });
    $("#AddStaffWindow").data("kendoWindow").center().title("Add User").open();
}

var EditStaff = function (id) {

    if (id != undefined) {
        $.getJSON("/Maintenance/Staff/GetStaffOtherInfo", { id: id, _t: new Date() }, function (item) {
            var model = getKendoGrid("UserManaView").dataSource.get(id);
            $("#AddStaffWindow .windowConfirm").attr("data-id", model.StaffId);
            $("#StaffNo").val(model.StaffNo);
            $("#JobClass").val(model.JobClass);
            $("#JobRank").val(model.JobRank);
            $("#JobTitle").val(model.JobTitle);
            $("#StaffName").val(model.UserName);
            $("#FirstName ").val(model.FirstName);
            $("#LastName").val(model.LastName);
            $("#Email").val(model.Email);
            $("#StaffStatus").data("kendoDropDownList").value(model.Status == true ? "true" : "false");
            $("#StaffSex").data("kendoDropDownList").value(model.Sex == "Man" ? "Man" : "Female");
            $("#Remark").val(model.Remark);
            $("#MobileNo").val(model.MobileNo);
            $("#TelNo").val(model.TelNo);
            $("#FaxNo").val(model.FaxNo);
            $("#ChineseName").val(model.ChineseName);
            $("#Department").val(model.Department);
            $("#StaffPosition").data("kendoDropDownList").value(item.PositionID);

            InitBaseKendoGrid("ReportToList", StaffModel, reporttocolumns, item.ReportToList, function () {
                bindGridCheckbox("ReportToList");
            });
            InitBaseKendoGrid("RoleList", RoleModel, rolecolumns, item.RoleList, function () {
                bindGridCheckbox("RoleList");
            });
            InitBaseKendoGrid("PositionList", PositionModel, positioncolumns, item.PositionList, function () {
                bindGridCheckbox("PositionList");
            });
            $("#AddStaffWindow .windowConfirm").attr("data-url", "/Maintenance/Staff/EditStaff");
            $("#AddStaffWindow").data("kendoWindow").center().title("Edit User").open();
        })
    }
    else {
        ShowTip("Please select employees!");
    }
}
var DisableUser = function () {
    var idList = new Array();
    $("#UserManaView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Staff/DoDisableStaff",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            $("#UserManaView .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                                .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                            for (var i = 0; i < idList.length; i++) {
                                getKendoGrid("UserManaView").dataSource.get(idList[i]).set("Status", false);
                            }
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }
    else {
        ShowTip("Please select employees!", "error");
    }
}
var DeleUsers = function () {
    var idList = new Array();
    $("#UserManaView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Staff/DoDeleUsers",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            for (var i = 0; i < idList.length; i++) {
                                var dataItem = getKendoGrid("UserManaView").dataSource.get(idList[i]);
                                getKendoGrid("UserManaView").dataSource.remove(dataItem);
                            }
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }
    else {
        ShowTip("Please select employees!", "error");
    }
}
function LoadStaffView() {
    title = "User Management - Kendo UI";
    $.getJSON("/Maintenance/Staff/GetStaff", { _t: new Date() }, function (items) {
        InitKendoExcelGrid("UserManaView", StaffModel, staffcolumns, items, 20, "User Management", function () {
            bindAndLoad("UserManaView");
            bindGridCheckbox("UserManaView");
            $("#UserManaView .k-toolbar")
                .append("<a id='UserDelete' class='more k-button' href='javascript:void(0)'><span class='glyphicon glyphicon-remove'></span></a>")
                .append("<a id='UserDisable' class='more k-button' href='javascript:void(0)'><span class='glyphicon glyphicon-ban-circle'></span></a>")
                .append(" <a id='UserAdd'  class='more k-button'  href='javascript:void(0)'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#UserAdd").click(AddUser);
            $("#UserDisable").click(DisableUser);
            $("#UserDelete").click(DeleUsers);
        });
    })

    $("#UserManaView").prev().find(".selectbtn").click(function () {
        var selectInput = $("#UserManaView").prev().find("input[name=selectInput]").val();
        $.post("/Maintenance/Staff/FindStaff", { input: selectInput }, function (items) {
            InitKendoExcelGrid("UserManaView", StaffModel, staffcolumns, items, 20, "User Management", function () {
                bindAndLoad("UserManaView");
            });
        })
    })

    InitUserWindow();
    InitUserWindowView();
}