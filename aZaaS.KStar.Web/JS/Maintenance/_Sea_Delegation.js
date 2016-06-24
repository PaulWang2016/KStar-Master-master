define(function (require, exports, module) {

    //var resetWindow = function () {
    //    hideOperaMask("AddRoleEmployeeWindow");
    //    $("#AddRoleEmployeeWindow .k-textbox").val("");//清除输入框
    //}
    //var InitWindows = function () {
    //    $("#AddRoleEmployeeWindow").kendoWindow({
    //        width: "500px",
    //        height: "380px",
    //        title: jsResxMaintenance_SeaDelegation.AddNode,
    //        actions: [
    //            "Close"
    //        ],
    //        open: function (e) {
    //            $("#AddRoleEmployeeWindow .windowCancel").bind("click", RoleEmployeeCancel)
    //            $("#AddRoleEmployeeWindow .windowConfirm").bind("click", RoleEmployeeConfirm)
    //        },
    //        close: function (e) {
    //            resetWindow();
    //            refreshCurrentScrolls();
    //            $("#AddRoleEmployeeWindow .windowCancel").unbind("click", RoleEmployeeCancel)
    //            $("#AddRoleEmployeeWindow .windowConfirm").unbind("click", RoleEmployeeConfirm)
    //        },
    //        resizable: false,
    //        animation: false,
    //        modal: true
    //    });

    //    InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
    //               findstaffcolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
    //                   bindGridCheckbox("RoleStaffList");
    //                   $("#RoleStaffList .k-grid-content").css("height", "190px")
    //               });
    //    AddSplitters($("#AddRoleEmployeeWindow").data("kendoWindow"));
    //}

    var RoleEmployeeConfirm = function () {
        showOperaMask("AddRoleEmployeeWindow");
        var that = $(this);
        that.unbind("click", RoleEmployeeConfirm);

        var multiselect = $("#delegateTo").data("kendoMultiSelect");
        var grid = $("#RoleStaffList").data("kendoGrid");
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

        //$("#delegateTo").attr("data-Name", item.UserName);
        //$("#delegateTo").val(item.DisplayName);


        hideOperaMask("AddRoleEmployeeWindow");
        $("#AddRoleEmployeeWindow").data("kendoWindow").close();
    }
    //var RoleEmployeeCancel = function () {
    //    $("#AddRoleEmployeeWindow").data("kendoWindow").close()
    //}

    //var RoleEmployeeSearch = function () {
    //    var input = $("#RoleEmployeeInput").val(); 
    //    InitBaseServerKendoGridWidthPage("RoleStaffList", StaffModel,
    //               findstaffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 10, function () {
    //                   bindGridCheckbox("RoleStaffList");
    //                   $("#RoleStaffList .k-grid-content").css("height", "190px")
    //               });
    //}   
   

    var RoleEmployeedelegateTo = function () {
        //$("#AddRoleEmployeeWindow").data("kendoWindow").title("Search User").center().open();
        SelectEmployee(this);
    }

    var SelectEmployee = function (obj) {        
        var multiselect = $("#delegateTo").data("kendoMultiSelect");
        InitSelectPersonWindow(obj, "Person", function (json) {            
            var userlist = json.Root.Users.Item;
            var listItems = new Array();
            $.each(multiselect.value(), function (i, item) {
                listItems.push(item);
            });
            $.each(userlist, function (i, n) {                
                var item = { StaffId: n.Value,UserName:n.UserName, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' };
                if (!ExistsEmployee(multiselect.dataSource._data, n.Value)) {
                    multiselect.dataSource.add(item);                    
                }
                if (listItems.join(',').indexOf(n.UserName) < 0)
                {
                    listItems.push(n.UserName);
                }
            });
            multiselect.value(listItems);
        })
    }

    var ExistsEmployee = function (dataSource, value)
    {
        var flag = false;
        $.each(dataSource, function (i, item) {
            if (item.StaffId == value)
            {
                flag = true;
            }
        });
        return flag;
    }

    var processColumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= FullName #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.ProcessName },
    ];
    var AddDelegation = function (delegationid) {
        var AddWindow = $("#AddWindow").data("kendoWindow");
        if (!AddWindow) {
            $("#AddWindow").kendoWindow({
                width: "680px",
                title: jsResxMaintenance_SeaDelegation.Delegation,
                actions: [
                    "Close"
                ], open: function (e) {
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

                            $("#processesList .k-grid-content").css("overflow-y", "auto").css("height", "145px");
                        });
                    });

                    $("#AddWindow .windowCancel").bind("click", DelegationCancel);
                    $("#AddWindow .windowConfirm").bind("click", DelegationConfirm);
                },
                close: function (e) {
                    hideOperaMask("AddWindow");
                    $("#delegateTo").data("kendoMultiSelect").setDataSource([]);
                    $("#AddWindow .windowCancel").unbind("click", DelegationCancel);
                    $("#AddWindow .windowConfirm").unbind("click", DelegationConfirm);
                },
                resizable: false,
                modal: true
            });
            window.AddSplitters($("#AddWindow").data("kendoWindow"));

            AddWindow = $("#AddWindow").data("kendoWindow").center();
        }
        if (delegationid != null && delegationid != undefined) {           
            $.getJSON("/Maintenance/Delegations/GetDelegationById", { delegateId: delegationid, _t: new Date() }, function (item) {                
                if (item != null) {
                    $("#processesList .k-grid-header").find(":checkbox").attr("disabled", true);
                    $("#AddWindow .searchprocess").unbind("click");
                    var multiselect = $("#delegateTo").data("kendoMultiSelect");
                    var listItems = new Array();
                    $.each(item.ToUserName, function (i, n) {
                        var item = { StaffId: n.SysId, UserName: n.UserName, FirstName: '', LastName: '', DisplayName: n.FullName, ChineseName: '', Email: '', MobileNo: '' };                        
                        listItems.push(n.UserName);
                        if (!ExistsEmployee(multiselect.dataSource._data, n.Value)) {
                            multiselect.dataSource.add(item);
                        }
                    });
                    multiselect.value(listItems);
                    $("#Remark").val(EndDate.Reason);
                    $("#processesList .k-grid-content").find("input[type='checkbox']").each(function () {
                        $(this).attr("disabled", true);
                        if ($(this).val().toString().toLowerCase() == item.FullName.toString().toLowerCase()) {
                            $(this).prop("checked", true);
                        }
                        else {
                            $(this).prop("checked", false);
                        }
                    });
                    $("#AddWindow .windowConfirm").data("delegationid", delegationid);
                }
            });            
        }
        else {           
            $("#delegateTo").data("kendoMultiSelect").value([]);            
            $("#Remark").val("");
            $("#AddWindow .windowConfirm").removeData("delegationid");
            $("#processesList .k-grid-content").find("input[type='checkbox']").each(function () {
                $(this).attr("disabled", false);
                $(this).prop("checked", false);
            });
            $("#processesList .k-grid-header").find(":checkbox").attr("disabled", false);
            $("#AddWindow .searchprocess").click(SearchProcess);
        }
        var beginDate = new Date();
        var endDate = new Date();
        endDate.setDate(beginDate.getDate() + 3);
        $("#StartDate").data("kendoDatePicker").value(beginDate);
        $("#EndDate").data("kendoDatePicker").value(endDate);
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
        $("#" + window.CurrentApp.pane + "delegationList .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaDelegation.Areyousuredeletedelegation, function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Delegations/DoDisableDelegation",
                        type: "POST",
                        data: { idList: idList, status: false },
                        traditional: true,
                        success: function (idsList) {
                            //$("#delegationList .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                            //    .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                            //for (var i = 0; i < idsList.length; i++) {
                            //    getKendoGrid("delegationList").dataSource.get(idsList[i]).set("Status", false);
                            //}
                            //getKendoGrid(window.CurrentApp.pane + "delegationList").dataSource.read();
                            $("#" + window.CurrentApp.pane + "delegationList").prev().find(".selectbtn").click();
                        },
                        dataType: "json"
                    });
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaDelegation.Pleaseselectdelegationerror, "info");
        }
    }

    var DelegationCheck = function (delegateTo, startDate, endDate, processList) {

        //Check Require
        var passed = true;
        
        //代表
        if (delegateTo.length < 1) {
            passed = false;
            var tips2 = $("#delegateTo").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips2.addClass("hide");
            }, 5000);
        }
        //开始日期
        if (startDate == null || startDate.length == 0) {
            passed = false;
            var tips3 = $("#StartDate").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips3.addClass("hide");
            }, 5000);
        }
        //结束日期
        if (endDate == null || endDate.length == 0) {
            passed = false;
            var tips4 = $("#EndDate").closest("td").find(".k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips4.addClass("hide");
            }, 5000);
        }

        //开始日期
        if (kendo.parseDate(startDate) == null) {
            passed = false;
            var tips5 = $("#StartDate").closest("td").find(".k-dateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips5.addClass("hide");
            }, 5000);
        }
        else {
            $("#StartDate").data("kendoDatePicker").value(kendo.parseDate(startDate));
        }
        //结束日期
        if (kendo.parseDate(endDate) == null) {
            passed = false;
            var tips6 = $("#EndDate").closest("td").find(".k-dateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips6.addClass("hide");
            }, 5000);
        }
        else {
            $("#EndDate").data("kendoDatePicker").value(kendo.parseDate(endDate));
        }

        //结束日期
        var sdate = kendo.parseDate(startDate);
        var edate = kendo.parseDate(endDate);
        if (sdate != null && edate != null && edate - sdate < 0) {
            passed = false;
            var tips7 = $("#EndDate").closest("td").find(".k-enddateinvalid-msg").removeClass("hide");
            setTimeout(function () {
                tips7.addClass("hide");
            }, 5000);
        }

        if (processList.length == 0) {
            passed = false;
            var tips8 = $("#processesList").siblings("span.k-invalid-msg").removeClass("hide");
            setTimeout(function () {
                tips8.addClass("hide");
            }, 5000);
        }
        return passed;
    }

    var DelegationConfirm = function () {
        var that = $(this);
        that.unbind("click", DelegationConfirm);        
        var isTo = false;
        //var delegateTo = $("#delegateTo").attr("data-Name");
        var delegateTo = $("#delegateTo").data("kendoMultiSelect").value();
        //console.log(delegateTo);
        var startDate = $("#StartDate").data("kendoDatePicker").value();
        var endDate = $("#EndDate").data("kendoDatePicker").value();
        var remark = $("#Remark").val();
        var processList = new Array();
        //$.each(Staffitems, function () {
        //    if (this.DisplayName == delegateTo) {
        //        delegateTo = this.UserName
        //        isTo = true;
        //    }
        //});
        //if (!isTo) {
        //    alert("No find user");
        //    that.bind("click", DelegationConfirm);
        //    hideOperaMask("AddWindow");
        //    return false;
        //}
        $("#processesList .k-grid-content").find(":checked").each(function () {
            processList.push($(this).val());
        })

        if (!DelegationCheck(delegateTo, $("#StartDate").val(), $("#EndDate").val(), processList)) {
            that.bind("click", DelegationConfirm);
            return false;
        }
        //验证后重新获取时间
        startDate = $("#StartDate").data("kendoDatePicker").value();
        endDate = $("#EndDate").data("kendoDatePicker").value();
        showOperaMask("AddWindow");        
        $.ajax({
            url: "/Maintenance/Delegations/DoCreateDelegation",
            type: "POST",
            data: { pane: window.CurrentApp.pane, delegatetoList: delegateTo,DelegationID:($("#AddWindow .windowConfirm").data("delegationid")==undefined?0:$("#AddWindow .windowConfirm").data("delegationid")), StartDate: startDate.format("yyyy-MM-dd HH:mm"), EndDate: endDate.format("yyyy-MM-dd HH:mm"), Reason: remark, processlist: processList },
            traditional: true,
            success: function (items) {
                //for (var i = 0; i < items.length; i++) {
                //    items[i].StartDate = kendo.toString($("#StartDate").data("kendoDatePicker").value(), window.DateTimeFormat);;
                //    items[i].EndDate = kendo.toString($("#EndDate").data("kendoDatePicker").value(), window.DateTimeFormat);
                //    getKendoGrid(window.CurrentApp.pane + "delegationList").dataSource.add(items[i]);
                //}
                //getKendoGrid(window.CurrentApp.pane + "delegationList").dataSource.read();
                $("#" + window.CurrentApp.pane + "delegationList").prev().find(".selectbtn").click();
                $("#AddWindow").data("kendoWindow").close();
                hideOperaMask("AddWindow");
            },
            dataType: "json"
        }).fail(function () {
            that.bind("click", DelegationConfirm);
            hideOperaMask("AddWindow");
        });
    }
    var DelegationCancel = function () {
        $("#AddWindow").data("kendoWindow").close()
    }    

    var Staffitems;
    LoadDelegation = function () {
        title = "My Delegation - Kendo UI";
        var pane = window.CurrentApp.pane
        var delegationList = pane + "delegationList";
        $("#delegationList").attr("id", pane + "delegationList");        

        $("#" + delegationList).prev().find("input[name='Status']").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: jsResxMaintenance_SeaDelegation.All, value: "" }, { text: jsResxMaintenance_SeaDelegation.Notexpired, value: "false" }, { text: jsResxMaintenance_SeaDelegation.Overdue, value: "true" }],
            index: 1
        });

        $("#" + delegationList).prev().find(".selectbtn").click(function () {
            var startdp = $("#" + delegationList).prev().find("input[name=StartDate]").data("kendoDatePicker");
            var enddp = $("#" + delegationList).prev().find("input[name=EndDate]").data("kendoDatePicker");
            var startDate = startdp == null ? null : startdp.value();
            var endDate = enddp == null ? null : enddp.value();            
            var delegateto = $("#" + delegationList).prev().find("input[name=DelegateTo]").val();           
            $.post("/Maintenance/Home/GetCurrentDatetime", { _t: new Date() }, function (date) {
                window.CurrentDate = new Date(date);
                showOperaMask();
                $.post("/Maintenance/Delegations/FindDelegations", {
                    pane: pane,
                    start: startDate == null ? null : startDate.format("yyyy-MM-dd HH:mm:ss"),
                    end: endDate == null ? null : endDate.format("yyyy-MM-dd HH:mm:ss"),
                    delegateTo: delegateto,
                    isOverdue: $("#" + delegationList).prev().find("input[name='Status']").data("kendoDropDownList").value()
                },
                function (items) {
                    hideOperaMask();
                    //var overtimecount = 0;
                    //var total = items.length;
                    var title = jsResxMaintenance_SeaDelegation.MyDelegation;
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
                    //title += "〔" + jsResxMaintenance_SeaDelegation.CurrentTaskCount + "：" + (total - overtimecount) + "〕";
                    //title += "〔" + jsResxMaintenance_SeaDelegation.CurrentOverTimeTaskCount + "：" + overtimecount + "〕";


                    InitKendoExcelGridWithHeight(delegationList, DelegationModel, delegationcolumns, items, 20, title, $(window).height() - fullwidgetH,
                    function () {
                        bindAndLoad(delegationList);
                        bindGridCheckbox(delegationList);

                        if ($("#delegationDisable").length == 0) {
                            $("#" + pane + "delegationList .k-toolbar").append("<a id='delegationDisable' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                                .append("<a id='delegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");


                            $("#delegationAdd").click(function () { AddDelegation(); })
                            $("#delegationDisable").click(DisableDelegation)
                        }
                    })
                });
            });
        }).click();

        //InitServerKendoExcelGrid(pane + "delegationList", DelegationModel, delegationcolumns,
        //    "/Maintenance/Delegations/GetDelegations?pane=" + pane, $(window).height() - fullwidgetH + 30, jsResxMaintenance_SeaDelegation.MyDelegation,
        //    function () {
        //        bindAndLoad(pane + "delegationList");
        //        bindGridCheckbox(pane + "delegationList");

        //        $("#" + pane + "delegationList .k-toolbar").append("<a id='delegationDisable' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>")
        //            .append("<a id='delegationAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");
                

        //        $("#pane-wrapper-" + pane + " #delegationAdd").click(AddDelegation)
        //        $("#pane-wrapper-" + pane + " #delegationDisable").click(DisableDelegation)
        //    });

        //$("#" + pane + "delegationList").delegate("a.k-Enable", "click", function () {
        //    var ok = $(this).find("span.glyphicon-ok");
        //    var idList = new Array();
        //    var id = $(this).attr("id");
        //    idList.push(id);
        //    if (ok.length == 1) {
        //        DisableDelegation(idList,false);
        //    }
        //    else {
        //        DisableDelegation(idList,true);
        //    }
        //})


        $("#AddWindow .windowCancel").click()
        $("#AddWindow .windowConfirm").click()

        //InitWindows();

        $("#delegateTo").kendoMultiSelect({
            dataSource: {
                data: [],
                schema: {
                    model: StaffModel
                }
            },
            dataTextField: "DisplayName",
            dataValueField: "UserName",
        });        

        $("#AddWindow .searchTo").click(RoleEmployeedelegateTo);
        
        //$("#RoleEmployeeSelect").click(RoleEmployeeSearch);

        $("#StartDate").kendoDatePicker({          
            format: window.DateTimeFormat
        });

        $("#EndDate").kendoDatePicker({
            format: window.DateTimeFormat
        });

        $("#" + window.CurrentApp.pane + "delegationList").prev().find("input[name=StartDate]").kendoDatePicker({ format: window.DateTimeFormat });
        $("#" + window.CurrentApp.pane + "delegationList").prev().find("input[name=EndDate]").kendoDatePicker({ format: window.DateTimeFormat });

        $("#" + window.CurrentApp.pane + "delegationList").delegate("a.k-grid-Enable", "click", function () {
            var delegationId = $(this).attr("data-delegationId");            
            AddDelegation(delegationId);
        })

    }
    module.exports = LoadDelegation;
})