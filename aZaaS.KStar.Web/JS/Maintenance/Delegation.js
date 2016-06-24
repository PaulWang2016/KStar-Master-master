var AddDelegation = function () {
    var AddWindow = $("#AddWindow").data("kendoWindow");
    if (!AddWindow) {
        $("#AddWindow").kendoWindow({
            width: "680px",
            title: "Delegation",
            actions: [
                "Close"
            ], open: function (e) {
                $.getJSON("/Maintenance/Process/Get", { "_t": new Date() }, function (items) {
                    InitBaseKendoGrid("processesList", ProcessModel, processColumns, items, function () {
                        $("#processesList").data("kendoGrid").bind("dataBound", function (e) {
                            $("#processesList .k-grid-header").find(":checkbox").prop("checked", false);
                        })
                        $("#processesList .k-grid-header").find(":checkbox").click(function () {
                            if ($(this).prop("checked")) {
                                $("#processesList .k-grid-content").find(":checkbox").prop("checked", true);
                            }
                            else {
                                $("#processesList .k-grid-content").find(":checkbox").prop("checked", false);
                            }
                        })

                        $("#processesList .k-grid-content").css("overflow-y", "auto").css("height", "100px");
                    });
                });
                $("#AddWindow .windowCancel").bind("click", DelegationCancel);
                $("#AddWindow .windowConfirm").bind("click", DelegationConfirm);
            },
            close: function (e) {
                hideOperaMask("AddWindow");
                $("#AddWindow .windowCancel").unbind("click", DelegationCancel);
                $("#AddWindow .windowConfirm").unbind("click", DelegationConfirm);
            },
            resizable: false,
            modal: true
        });
        AddSplitters($("#AddWindow").data("kendoWindow"));

        AddWindow = $("#AddWindow").data("kendoWindow").center();
    }
    $("#delegateTo").val("")
    $("#StartDate").val("");
    $("#EndDate").val("");
    $("#Remark").val("");
    AddWindow.open();
}

var DisableDelegation = function () {
    var idList = new Array();
    $("#delegationList .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        $.ajax({
            url: "/Maintenance/Delegations/DoDisableDelegation",
            type: "POST",
            data: { idList: idList },
            traditional: true,
            success: function (idsList) {
                $("#delegationList .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                    .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                for (var i = 0; i < idsList.length; i++) {
                    getKendoGrid("delegationList").dataSource.get(idsList[i]).set("Status", false);
                }
            },
            dataType: "json"
        })
    }
    else {
        ShowTip("Please select delegation!", "error");
    }
}


var DelegationConfirm = function () {
    var that = $(this);
    that.unbind("click", DelegationConfirm);
    showOperaMask("AddWindow");
    var isTo = false;
    var delegateTo = $("#delegateTo").val()
    var startDate = $("#StartDate").val();
    var endDate = $("#EndDate").val();
    var remark = $("#Remark").val();
    var processList = new Array();
    $.each(Staffitems, function () {
        if (this.DisplayName == delegateTo) {
            delegateTo = this.UserName
            isTo = true;
        }
    });
    if (!isTo) {
        alert("No find user");
        that.bind("click", DelegationConfirm);
        hideOperaMask("AddWindow");
        return false;
    }
    $("#processesList .k-grid-content").find(":checked").each(function () {
        processList.push($(this).parent().next().text());
    })
    if (processList.length > 0) {
        $.ajax({
            url: "/Maintenance/Delegations/DoCreateDelegation",
            type: "POST",
            data: { DelegateTo: delegateTo, StartDate: startDate, EndDate: endDate, Remarks: remark, processlist: processList },
            traditional: true,
            success: function (items) {
                for (var i = 0; i < items.length; i++) {
                    items[i].StartDate = startDate;
                    items[i].EndDate = endDate;
                    getKendoGrid("delegationList").dataSource.add(items[i]);
                }
                $("#AddWindow").data("kendoWindow").close()
            },
            dataType: "json"
        }).fail(function () {
            that.bind("click", DelegationConfirm);
            hideOperaMask("AddWindow");
        })
    }
    else {
        that.bind("click", DelegationConfirm);
        hideOperaMask("AddWindow");
    }
}
var DelegationCancel = function () {
    $("#AddWindow").data("kendoWindow").close()
}

var Staffitems;
LoadDelegation = function () {
    title = "My Delegation - Kendo UI";
    $.getJSON("/Maintenance/Delegations/GetDelegation", { "_t": new Date() }, function (items) {
        InitKendoExcelGrid("delegationList", DelegationModel, delegationcolumns, items, 20, "My Delegation", function () {
            bindAndLoad("delegationList");
            bindGridCheckbox("delegationList");

            $("#delegationList .k-toolbar").append("<a id='delegationDisable' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>")
            .append("<a id='delegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#delegationAdd").click(AddDelegation)
            $("#delegationDisable").click(DisableDelegation)
        });
    })




    $("#AddWindow .windowCancel").click()
    $("#AddWindow .windowConfirm").click()
    $.getJSON("/Maintenance/Staff/GetStaffNames", { "_t": new Date() }, function (items) {
        Staffitems = items;
        $("#delegateTo").kendoAutoComplete({
            dataTextField: "DisplayName",
            dataSource: Staffitems
        });
    })
}