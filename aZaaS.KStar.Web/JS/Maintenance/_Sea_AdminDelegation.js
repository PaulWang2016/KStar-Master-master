define(function (require, exports, module) {

    var resetWindow = function () {
        hideOperaMask("AddRoleEmployeeWindow");
        $("#RoleStaffList").off("click", ".k-grid-content :checkbox", Singleclick);
        $("#AddRoleEmployeeWindow .k-textbox").val("");//清除输入框
    }
    var Singleclick = function () {
        $("#RoleStaffList .k-grid-content :checkbox").prop("checked", false);
        $(this).prop("checked", true);
    }
    var InitWindows = function () {
        $("#AddRoleEmployeeWindow").kendoWindow({
            width: "500px",
            height: "380px",
            title: jsResxMaintenance_SeaAdminDelegation.AddNode,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddRoleEmployeeWindow .windowCancel").bind("click", RoleEmployeeCancel)
                $("#AddRoleEmployeeWindow .windowConfirm").bind("click", RoleEmployeeConfirm)
            },
            close: function (e) {
                resetWindow();
                $("#AddRoleEmployeeWindow .windowCancel").unbind("click", RoleEmployeeCancel)
                $("#AddRoleEmployeeWindow .windowConfirm").unbind("click", RoleEmployeeConfirm)
            },
            resizable: false,
            animation: false,
            modal: true
        });
        AddSplitters($("#AddRoleEmployeeWindow").data("kendoWindow"));
    }
    var RoleEmployeeConfirm = function () {
        showOperaMask("AddRoleEmployeeWindow");
        var that = $(this);
        that.unbind("click", RoleEmployeeConfirm);

        if ($("#AddRoleEmployeeWindow .windowConfirm").attr("data-type") == "delegateTo") {
            var grid = $("#RoleStaffList").data("kendoGrid");
            var multiselect = $("#AdmindelegateTo").data("kendoMultiSelect");
            var listItems = new Array();
            listItems = [];
            $("#RoleStaffList .k-grid-content").find(":checked").each(function () {
                var item = grid.dataSource.get($(this).val());
                if (!multiselect.dataSource.get($(this).val())) {
                    multiselect.dataSource.add(item)
                }
                listItems.push(item.UserName);
            });

            multiselect.value(listItems);
            //var item = grid.dataSource.get($("#RoleStaffList .k-grid-content").find(":checked").first().val());
            //$("#AdmindelegateTo").attr("data-Name", item.UserName);
            //$("#AdmindelegateTo").val(item.DisplayName);
        }
        else if ($("#AddRoleEmployeeWindow .windowConfirm").attr("data-type") == "delegateFrom") {
            var grid = $("#RoleStaffList").data("kendoGrid");
            var item = grid.dataSource.get($("#RoleStaffList .k-grid-content").find(":checked").first().val());
            $("#AdmindelegateFrom").attr("data-Name", item.UserName);
            $("#AdmindelegateFrom").val(item.DisplayName);
        }
        hideOperaMask("AddRoleEmployeeWindow");
        $("#AddRoleEmployeeWindow").data("kendoWindow").close();
    }

    var RoleEmployeeSearch = function () {
        var input = $("#RoleEmployeeInput").val();
        InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
                   findstaffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 10, function () {
                       bindGridCheckbox("RoleStaffList");
                   });
    }




    //搜索代表 旧版本弃用
    var RoleEmployeedelegateTo = function () {
        $("#AddRoleEmployeeWindow .windowConfirm").attr("data-type", "delegateTo");
        InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
                   findstaffcolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
                       bindGridCheckbox("RoleStaffList");
                       $("#RoleStaffList .k-grid-content").css("height", "190px")
                   });
        $("#AddRoleEmployeeWindow").data("kendoWindow").title(jsResxMaintenance_SeaAdminDelegation.SearchUser).center().open();
    }


    //搜索委托人  旧版本弃用
    var RoleEmployeedelegateFrom = function () {
        $("#AddRoleEmployeeWindow .windowConfirm").attr("data-type", "delegateFrom");
        InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
                   findstaffcolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
                       bindGridCheckbox("RoleStaffList");
                       $("#RoleStaffList").on("click", ".k-grid-content :checkbox", Singleclick);
                       $("#RoleStaffList .k-grid-content").css("height", "190px")
                   });
        $("#AddRoleEmployeeWindow").data("kendoWindow").title(jsResxMaintenance_SeaAdminDelegation.SearchUser).center().open();
    }

    //搜索代表
    var AdmindelegateTo = function () {
        SelectAdmindelegateTo(this);
    }

    var SelectAdmindelegateTo = function (obj) {        
        var multiselect = $("#AdmindelegateTo").data("kendoMultiSelect");
        InitSelectPersonWindow(obj, "Person", function (json) {            
            var userlist = json.Root.Users.Item;
            var listItems = new Array();
            $.each(multiselect.value(), function (i,item) {
                listItems.push(item);
            });
            $.each(userlist, function (i, n) {                
                var item = { StaffId: n.Value, UserName: n.UserName, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' };
                if (!ExistsUser(multiselect.dataSource._data, n.Value)) {
                    multiselect.dataSource.add(item)
                }
                if (listItems.join(',').indexOf(n.UserName) < 0) {
                    listItems.push(n.UserName);
                }
            });            
            multiselect.value(listItems);
        });
    }

    var ExistsUser= function (dataSource, value) {
        var flag = false;
        $.each(dataSource, function (i, item) {
            if (item.StaffId == value) {
                flag = true;
            }
        });
        return flag;
    }

    //搜索委托人
    var AdmindelegateFrom = function () {
        SelectAdmindelegateFrom(this);
    }

    var SelectAdmindelegateFrom = function (obj) {
        InitSelectPersonWindow(obj, "Person", function (json) {
            var userlist = json.Root.Users.Item;
            if (userlist != null && userlist.length > 0) {
                $("#AdmindelegateFrom").attr("data-Name", userlist[0].UserName);
                $("#AdmindelegateFrom").data("data-Name", userlist[0].UserName);
                $("#AdmindelegateFrom").attr("data-Text", userlist[0].Name);
                $("#AdmindelegateFrom").val(userlist[0].Name);
            }
        }, null, {mutilselect:false});
    }



    var RoleEmployeeCancel = function () {
        $("#AddRoleEmployeeWindow").data("kendoWindow").close()
    }
    var processColumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= FullName #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.ProcessName },
    ];
    var AddDelegation = function (delegationid) {
        var AddWindow = $("#AddDelegationWindow").data("kendoWindow");
        if (!AddWindow) {
            $("#AddDelegationWindow").kendoWindow({
                width: "680px",
                title: jsResxMaintenance_SeaAdminDelegation.Delegation,
                actions: [
                    "Close"
                ],
                open: function (e) {                    
                    $.getJSON("/Maintenance/Process/GetProcess", { key: "", _t: new Date() }, function (items) {
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

                            $("#processesList .k-grid-content").css("overflow-y", "auto").css("height", "170px");
                        });
                    });

                    $("#AddDelegationWindow .windowCancel").bind("click", AdminDelegationCancel);
                    $("#AddDelegationWindow .windowConfirm").bind("click", AdminDelegationConfirm);

                    $("#AddDelegationWindow .k-invalid-msg").addClass("hide");
                },
                close: function (e) {
                    hideOperaMask("AddDelegationWindow");
                    $("#processesApps").val("");
                    $("#AdmindelegateTo").data("kendoMultiSelect").setDataSource([]);                    
                    $("#AddDelegationWindow .windowCancel").unbind("click", AdminDelegationCancel);
                    $("#AddDelegationWindow .windowConfirm").unbind("click", AdminDelegationConfirm);
                },
                resizable: false,
                modal: true
            });
            AddWindow = $("#AddDelegationWindow").data("kendoWindow").center();
        }

        if (delegationid != null && delegationid != undefined) {
            $.getJSON("/Maintenance/Delegations/GetDelegationById", { delegateId: delegationid, _t: new Date() }, function (item) {                
                if (item != null) {
                    $("#processesList .k-grid-header").find(":checkbox").attr("disabled", true);
                    $("#AddDelegationWindow .searchprocess").unbind("click");
                    var delegateToSelect = $("#AdmindelegateTo").data("kendoMultiSelect");                    
                    var listItems = new Array();
                    $.each(item.ToUserName, function (i, n) {
                        var item = { StaffId: n.SysId, UserName: n.UserName, FirstName: '', LastName: '', DisplayName: n.FullName, ChineseName: '', Email: '', MobileNo: '' };
                        listItems.push(n.UserName);
                        if (!ExistsUser(delegateToSelect.dataSource._data, n.Value)) {
                            delegateToSelect.dataSource.add(item);
                        }
                    });
                    delegateToSelect.value(listItems);

                    $.each(item.FromUserName, function (i, n) {                        
                        $("#AdmindelegateFrom").attr("data-Name", n.UserName).attr("data-Text", n.FullName);
                        $("#AdmindelegateFrom").val(n.FullName);
                    });

                    $("#AddDelegationWindow .Remark").val(item.Reason);
                    $("#processesList .k-grid-content").find("input[type='checkbox']").each(function () {
                        $(this).attr("disabled", true);                        
                        if ($(this).val().toString().toLowerCase() == item.FullName.toString().toLowerCase()) {
                            $(this).prop("checked", true);
                        }
                        else {
                            $(this).prop("checked", false);
                        }
                    });
                    $("#AddDelegationWindow .windowConfirm").data("delegationid", delegationid);
                }
            });
        }
        else {
            $("#AdmindelegateTo").data("kendoMultiSelect").value([]);
            $("#AdmindelegateFrom").attr("data-Name", "").attr("data-Text", "");
            $("#AdmindelegateFrom").val("");
            $("#AddDelegationWindow .Remark").val("");

                        
            $("#AddDelegationWindow .windowConfirm").removeData("delegationid");
            $("#processesList .k-grid-content").find("input[type='checkbox']").each(function () {
                $(this).attr("disabled", false);
                $(this).prop("checked", false);
            });
            $("#processesList .k-grid-header").find(":checkbox").attr("disabled", false);
            $("#AddDelegationWindow .searchprocess").click(SearchProcess);
        }
        var beginDate = new Date();
        var endDate = new Date();
        endDate.setDate(beginDate.getDate() + 3);                
        $("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value(beginDate);
        $("#AddDelegationWindow input.EndDate").data("kendoDatePicker").value(endDate);        
        AddWindow.open();
    }

    var SearchProcess = function () {
        var key = $("#processesApps").val();
        $.getJSON("/Maintenance/Process/GetProcess", { key: key, _t: new Date() }, function (items) {
            var dataSource = new kendo.data.DataSource({
                data: items
            });
            $("#processesList").data("kendoGrid").setDataSource(dataSource);
        });
    }

    var DisableDelegation = function () {
        var idList = new Array();
        $("#adminDelegationList .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaAdminDelegation.Areyousuredeletedelegation, function (result) {
                if (result) {                    
                    $.ajax({
                        url: "/Maintenance/Delegations/DoDisableAdminDelegation",
                        type: "POST",
                        data: { idList: idList, status: false },
                        traditional: true,
                        success: function (idsList) {
                            //$("#adminDelegationList .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                            //    .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                            //for (var i = 0; i < idsList.length; i++) {
                            //    getKendoGrid("adminDelegationList").dataSource.get(idsList[i]).set("Status", false);
                            //}
                            //getKendoGrid("adminDelegationList").dataSource.read();
                            $("#adminDelegationList").prev().find(".selectbtn").click();
                        },
                        dataType: "json"
                    });
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaAdminDelegation.Pleaseselectadelegation, "info");
        }
    }

    var AdminDelegationCheck = function (delegateFrom, delegateTo, startDate, endDate, processList) {

        //Check Require
        var passed = true;
        //委托人
        if (delegateFrom == undefined || delegateFrom == "") {
            passed = false;
            var tips1 = $("#AdmindelegateFrom").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips1.addClass("hide");
            }, 5000);
        }
        //代表
        if (delegateTo.length < 1) {
            passed = false;
            var tips2 = $("#AdmindelegateTo").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips2.addClass("hide");
            }, 5000);
        }
        //开始日期
        if (startDate == null||startDate.length==0) {
            passed = false;
            var tips3 = $("#AddDelegationWindow input.StartDate").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips3.addClass("hide");
            }, 5000);
        }
        //结束日期
        if (endDate == null || endDate.length==0) {
            passed = false;
            var tips4 = $("#AddDelegationWindow input.EndDate").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips4.addClass("hide");
            }, 5000);
        }

        //开始日期
        if (kendo.parseDate(startDate) == null) {
            passed = false;
            var tips5 = $("#AddDelegationWindow input.StartDate").closest("td").find(".k-dateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips5.addClass("hide");
            }, 5000);
        }
        else {
            $("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value(kendo.parseDate(startDate));
        }
        //结束日期
        if (kendo.parseDate(endDate) == null) {
            passed = false;
            var tips6 = $("#AddDelegationWindow input.EndDate").closest("td").find(".k-dateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips6.addClass("hide");
            }, 5000);
        }
        else {
            $("#AddDelegationWindow input.EndDate").data("kendoDatePicker").value(kendo.parseDate(endDate));
        }

        //结束日期
        var sdate = kendo.parseDate(startDate);
        var edate = kendo.parseDate(endDate);
        if (sdate != null && edate != null && edate-sdate<0) {
            passed = false;
            var tips7 = $("#AddDelegationWindow input.EndDate").closest("td").find(".k-enddateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips7.addClass("hide");
            }, 5000);
        }
        
        if (processList.length==0) {
            passed = false;
            var tips8 = $("#processesList").siblings("span.k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips8.addClass("hide");
            }, 5000);
        }
        return passed;
    }

    var AdminDelegationConfirm = function () {        
        var that = $(this);
        that.unbind("click", AdminDelegationConfirm);
        var isTo = false;
        var isFrom = false;

        var delegateTo = $("#AdmindelegateTo").data("kendoMultiSelect").value();
        //var pane;
        //var proAppsList = $("#processesApps").data("kendoDropDownList");
        //if (proAppsList == undefined) {
        //    return false;
        //}
        //else {
        //    pane = $("#processesApps").data("kendoDropDownList").value();
        //}
        var startDate = $("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value();
        var endDate = $("#AddDelegationWindow input.EndDate").data("kendoDatePicker").value();
        var remark = $("#AddDelegationWindow .Remark").val();
        var processList = new Array();
        var delegateFrom = $("#AdmindelegateFrom").attr("data-Name");        
        $("#processesList .k-grid-content").find(":checked").each(function () {
            processList.push($(this).val());
        })

        if (!AdminDelegationCheck(delegateFrom, delegateTo, $("#AddDelegationWindow input.StartDate").val(), $("#AddDelegationWindow input.EndDate").val(), processList)) {
            that.bind("click", AdminDelegationConfirm);
            return false;
        }

        startDate = $("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value();
        endDate = $("#AddDelegationWindow input.EndDate").data("kendoDatePicker").value();

        showOperaMask("AddDelegationWindow");
        $.ajax({
            url: "/Maintenance/Delegations/DoCreateAdminDelegation",
            type: "POST",
            data: { pane: window.CurrentApp.pane, FromUser: delegateFrom, delegatetoList: delegateTo, DelegationID: ($("#AddDelegationWindow .windowConfirm").data("delegationid") == undefined ? 0 : $("#AddDelegationWindow .windowConfirm").data("delegationid")), StartDate: startDate.format("yyyy-MM-dd HH:mm"), EndDate: endDate.format("yyyy-MM-dd HH:mm"), Reason: remark, processlist: processList },
            traditional: true,
            success: function (items) {
                //for (var i = 0; i < items.length; i++) {
                //    items[i].StartDate = kendo.toString($("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value(), window.DateTimeFormat);
                //    items[i].EndDate = kendo.toString($("#AddDelegationWindow input.StartDate").data("kendoDatePicker").value(), window.DateTimeFormat);
                //    getKendoGrid("adminDelegationList").dataSource.add(items[i]);
                //}
                //getKendoGrid("adminDelegationList").dataSource.read();
                $("#adminDelegationList").prev().find(".selectbtn").click();
                $("#AddDelegationWindow").data("kendoWindow").close()
                hideOperaMask("AddDelegationWindow");
            },
            dataType: "json"
        }).fail(function () {
            that.bind("click", AdminDelegationConfirm);
            hideOperaMask("AddDelegationWindow");
        });
    }
    var AdminDelegationCancel = function () {
        $("#AddDelegationWindow").data("kendoWindow").close()
    }

    var Staffitems;
    LoadAdminDelegation = function () {
        title = "Delegation Management - Kendo UI";
        //$.getJSON(, { _t: new Date() }, function (items) {

        //})       

        $("#adminDelegationList").prev().find("input[name='Status']").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: jsResxMaintenance_SeaAdminDelegation.All, value: "" }, { text: jsResxMaintenance_SeaAdminDelegation.Notexpired, value: "false" }, { text: jsResxMaintenance_SeaAdminDelegation.Overdue, value: "true" }],
            index: 1
        });


        $("#adminDelegationList").prev().find(".selectbtn").click(function () {
            var startdp = $("#adminDelegationList").prev().find("input[name=StartDate]").data("kendoDatePicker");
            var enddp = $("#adminDelegationList").prev().find("input[name=EndDate]").data("kendoDatePicker");
            var startDate = startdp == null ? null : startdp.value();
            var endDate = enddp == null ? null : enddp.value();
            var delegatefrom = $("#adminDelegationList").prev().find("input[name=DelegateFrom]").val();
            var delegateto = $("#adminDelegationList").prev().find("input[name=DelegateTo]").val();
            $.post("/Maintenance/Home/GetCurrentDatetime", { _t: new Date() }, function (date) {
                window.CurrentDate = new Date(date);
                showOperaMask();
                $.post("/Maintenance/Delegations/FindAdminDelegations", {
                    start: startDate == null ? null : startDate.format("yyyy-MM-dd HH:mm:ss"),
                    end: endDate == null ? null : endDate.format("yyyy-MM-dd HH:mm:ss"),
                    delegateFrom: delegatefrom,
                    delegateTo: delegateto,
                    isOverdue: $("#adminDelegationList").prev().find("input[name='Status']").data("kendoDropDownList").value()
                },
                function (items) {
                    hideOperaMask();
                    //var overtimecount = 0;
                    //var total = items.length;
                    var title = jsResxMaintenance_SeaAdminDelegation.DelegationManagement;
                    //$.each(items, function (i, item) {
                    //    var startdate = window.CurrentDate;
                    //    startdate = new Date(startdate).format("yyyy-MM-dd HH:mm:ss");
                    //    var enddate = kendo.parseDate(item.EndDate);
                    //    if (enddate != null) {
                    //        enddate = enddate.format("yyyy-MM-dd HH:mm:ss");
                    //        var result = GetDateDiff(startdate, enddate);
                    //        if (result.day < 0 || result.hour < 0 || result.minute < 0) {
                    //            overtimecount += 1;
                    //        }
                    //    }
                    //});
                    //title += "〔" + jsResxMaintenance_SeaAdminDelegation.CurrentTaskCount + "：" + (total - overtimecount) + "〕";
                    //title += "〔" + jsResxMaintenance_SeaAdminDelegation.CurrentOverTimeTaskCount + "：" + overtimecount + "〕";
                    InitKendoExcelGridWithHeight("adminDelegationList", DelegationModel, admindelegationcolumns, items, 20, title, $(window).height() - fullwidgetH - 10,
                    function () {
                        bindAndLoad("adminDelegationList");
                        bindGridCheckbox("adminDelegationList");

                        if ($("#admindelegationDisable").length == 0) {
                            $("#adminDelegationList .k-toolbar").append("<a id='admindelegationDisable' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                                .append("<a id='admindelegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");
                            $("#admindelegationAdd").click(function () { AddDelegation(); })
                            $("#admindelegationDisable").click(DisableDelegation)
                        }
                    })
                });
            });
        }).click();     

        //InitServerKendoExcelGrid("adminDelegationList", DelegationModel, admindelegationcolumns,
        //    "/Maintenance/Delegations/GetAdminDelegations", $(window).height() - fullwidgetH + 30, jsResxMaintenance_SeaAdminDelegation.DelegationManagement, function () {
        //        bindAndLoad("adminDelegationList");
        //        bindGridCheckbox("adminDelegationList");

        //        $("#adminDelegationList .k-toolbar").append("<a id='admindelegationDisable' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>")
        //            .append("<a id='admindelegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");                

        //        $("#admindelegationAdd").click(AddDelegation)
        //        $("#admindelegationDisable").click(DisableDelegation)
        //    });
        $("#AdmindelegateTo").kendoMultiSelect({            
            dataTextField: "DisplayName",
            dataValueField: "UserName"
        });
        InitWindows();

        $("#AddDelegationWindow .searchTo").click(AdmindelegateTo);
        $("#AddDelegationWindow .searchFrom").click(AdmindelegateFrom);

        $("#AdmindelegateFrom").keyup(function () {            
            if ($(this).val() != $(this).attr("data-Text")) {
                $(this).removeAttr("data-Name");
            }
            else {
                var value = $(this).data("data-Name");
                if (value != undefined&&value.length>0)
                {
                    $(this).attr("data-Name",value);
                }
            }
        });
        
        $("#RoleEmployeeSelect").click(RoleEmployeeSearch);

        //$("#adminDelegationList").delegate("a.k-Enable", "click", function () {
        //    var ok = $(this).find("span.glyphicon-ok");
        //    var idList = new Array();
        //    var id = $(this).attr("id");
        //    idList.push(id);
        //    if (ok.length == 1) {
        //        DisableDelegation(idList, false);
        //    }
        //    else {
        //        DisableDelegation(idList, true);
        //    }
        //})

        //$.getJSON("/Maintenance/Staff/GetStaffNames", { _t: new Date() }, function (items) {
        //    Staffitems = items;
        //    $("#AdmindelegateTo").kendoAutoComplete({
        //        dataTextField: "DisplayName",
        //        dataSource: Staffitems
        //    });
        //    $("#AdmindelegateFrom").kendoAutoComplete({
        //        dataTextField: "DisplayName",
        //        dataSource: Staffitems
        //    });
        //})

        $("#AddDelegationWindow input.StartDate").kendoDatePicker({
            format: window.DateTimeFormat
        });
        $("#AddDelegationWindow input.EndDate").kendoDatePicker({
            format: window.DateTimeFormat
        });

        $("#adminDelegationList").prev().find("input[name=StartDate]").kendoDatePicker({ format: window.DateTimeFormat });
        $("#adminDelegationList").prev().find("input[name=EndDate]").kendoDatePicker({ format: window.DateTimeFormat });

        $("#adminDelegationList").delegate("a.k-grid-Enable", "click", function () {
            var delegationId = $(this).attr("data-delegationId");            
            AddDelegation(delegationId);
        })
    }
    module.exports = LoadAdminDelegation;
})