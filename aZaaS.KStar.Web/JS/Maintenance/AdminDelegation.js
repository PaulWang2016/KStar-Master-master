var AddDelegation = function () {
    var AddWindow = $("#AddDelegationWindow").data("kendoWindow");
    if (!AddWindow) {
        $("#AddDelegationWindow").kendoWindow({
            width: "680px",
            title: jsResxMaintenance_AdminDelegation.Delegation,
            actions: [
                "Close"
            ],
            open: function (e) {
                $.getJSON("/Maintenance/Process/Get", { _t: new Date() }, function (items) {
                    InitBaseKendoGrid("AddDelegationWindow .processesList", ProcessModel, processColumns, items, function () {
                        $("#AddDelegationWindow .processesList").data("kendoGrid").bind("dataBound", function (e) {
                            $("#AddDelegationWindow .processesList .k-grid-header").find(":checkbox").prop("checked", false);
                        })
                        $("#AddDelegationWindow .processesList .k-grid-header").find(":checkbox").click(function () {
                            if ($(this).prop("checked")) {
                                $("#AddDelegationWindow .processesList .k-grid-content").find(":checkbox").prop("checked", true);
                            }
                            else {
                                $("#AddDelegationWindow .processesList .k-grid-content").find(":checkbox").prop("checked", false);
                            }
                        })

                        $("#AddDelegationWindow .processesList .k-grid-content").css("overflow-y", "auto").css("height", "130px");
                    });
                });
                $("#AddDelegationWindow .windowCancel").bind("click", AdminDelegationCancel);
                $("#AddDelegationWindow .windowConfirm").bind("click", AdminDelegationConfirm);
            },
            close: function (e) {
                hideOperaMask("AddDelegationWindow");
                $("#AddDelegationWindow .windowCancel").unbind("click", AdminDelegationCancel);
                $("#AddDelegationWindow .windowConfirm").unbind("click", AdminDelegationConfirm);
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddDelegationWindow").data("kendoWindow"));
        AddWindow = $("#AddDelegationWindow").data("kendoWindow").center();
    }
    $("#AddDelegationWindow .delegateTo").val("")
    $("#AddDelegationWindow .StartDate").val("");
    $("#AddDelegationWindow .EndDate").val("");
    $("#AddDelegationWindow .Remark").val("");
    AddWindow.open();
}

var DisableDelegation = function () {
    var idList = new Array();
    $("#adminDelegationList .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        $.ajax({
            url: "/Maintenance/Delegations/DoDisableAdminDelegation",
            type: "POST",
            data: { idList: idList },
            traditional: true,
            success: function (idsList) {
                $("#adminDelegationList .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                    .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                for (var i = 0; i < idsList.length; i++) {
                    getKendoGrid("adminDelegationList").dataSource.get(idsList[i]).set("Status", false);
                }
            },
            dataType: "json"
        })
    }
    else {
        ShowTip("Please select delegation!", "error");
    }
}

var AdminDelegationConfirm = function () {
    var that = $(this);
    that.unbind("click", AdminDelegationConfirm);
    showOperaMask("AddDelegationWindow");
    var isTo = false;
    var isFrom = false;
    var delegateTo = $("#AdmindelegateTo").val()
    var startDate = $("#AddDelegationWindow input.StartDate").val();
    var endDate = $("#AddDelegationWindow input.EndDate").val();
    var remark = $("#AddDelegationWindow .Remark").val();
    var processList = new Array();
    var delegateFrom = $("#AdmindelegateFrom").val();
    $.each(Staffitems, function () {
        if (this.DisplayName == delegateTo) {
            delegateTo = this.UserName
            isTo = true;
        }
        if (this.DisplayName == delegateFrom) {
            delegateFrom = this.UserName
            isFrom = true;
        }
    });
    if (!isTo) {
        alert("No find user");
        that.bind("click", AdminDelegationConfirm);
        hideOperaMask("AddDelegationWindow");
        return false;
    }
    if (!isFrom) {
        alert("No find user");
        that.bind("click", AdminDelegationConfirm);
        hideOperaMask("AddDelegationWindow");
        return false;
    }
    $("#AddDelegationWindow .processesList .k-grid-content").find(":checked").each(function () {
        processList.push($(this).parent().next().text());
    })
    if (processList.length > 0) {
        $.ajax({
            url: "/Maintenance/Delegations/DoCreateAdminDelegation",
            type: "POST",
            data: { DelegateFrom: delegateFrom, DelegateTo: delegateTo, StartDate: startDate, EndDate: endDate, Remarks: remark, processlist: processList },
            traditional: true,
            success: function (items) {
                for (var i = 0; i < items.length; i++) {
                    items[i].StartDate = startDate;
                    items[i].EndDate = endDate;
                    getKendoGrid("adminDelegationList").dataSource.add(items[i]);
                }
                $("#AddDelegationWindow").data("kendoWindow").close()
            },
            dataType: "json"
        }).fail(function () {
            that.bind("click", AdminDelegationConfirm);
            hideOperaMask("AddDelegationWindow");
        })
    }
    else {
        that.bind("click", AdminDelegationConfirm);
        hideOperaMask("AddDelegationWindow");
    }
}
var AdminDelegationCancel = function () {
    $("#AddDelegationWindow").data("kendoWindow").close()
}

var Staffitems;
LoadAdminDelegation = function () {
    title = "Delegation Management - Kendo UI";
    $.getJSON("/Maintenance/Delegations/GetAdminDelegation", { _t: new Date() }, function (items) {
        InitKendoExcelGrid("adminDelegationList", DelegationModel, delegationcolumns, items, 20, "Delegation Management", function () {
            bindAndLoad("adminDelegationList");
            bindGridCheckbox("adminDelegationList");

            $("#adminDelegationList .k-toolbar").append("<a id='delegationDisable' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>")
                .append("<a id='delegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#delegationAdd").click(AddDelegation)
            $("#delegationDisable").click(DisableDelegation)
        });
    })
    $.getJSON("/Maintenance/Staff/GetStaffNames", { _t: new Date() }, function (items) {
        Staffitems = items;
        $("#AdmindelegateTo").kendoAutoComplete({
            dataTextField: "DisplayName",
            dataSource: Staffitems
        });
        $("#AdmindelegateFrom").kendoAutoComplete({
            dataTextField: "DisplayName",
            dataSource: Staffitems
        });
    })
}