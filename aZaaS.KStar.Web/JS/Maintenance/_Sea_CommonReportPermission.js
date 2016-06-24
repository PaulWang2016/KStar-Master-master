define(function (require, exports, module) {
    var processnamelist;
    var ManagementSplitter = function () {
        $("#ProcessPermissionSplitter").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "250px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        AddSplitters($("#ProcessPermissionSplitter").data("kendoSplitter"));
    }   
    var InitProcessList = function () {
        showOperaMask();
        $.getJSON("/Maintenance/Process/GetProcess", { key: "", _t: new Date() }, function (items) {
            hideOperaMask();
            InitBaseKendoGrid("ProcessList", ProcessModel, processColumns, items, function () {
                $("#ProcessList").data("kendoGrid").bind("dataBound", function (e) {
                    $("#ProcessList .k-grid-header").find(":checkbox").prop("checked", false);
                })
                $("#ProcessList .k-grid-header").find(":checkbox").click(function () {
                    if ($(this).prop("checked")) {
                        $("#ProcessList .k-grid-content").find(":checkbox").prop("checked", true);
                    }
                    else {
                        $("#ProcessList .k-grid-content").find(":checkbox").prop("checked", false);
                    }
                })

                $("#ProcessList .k-grid-content").css("overflow-y", "auto").css("height", "170px");
            });
        });
    }


    var InitRoleProcess = function () {
        $("#RoleList .k-grid-content").on("click", ":checkbox", function () {
            debugger;
            showOperaMask();
            $("#RoleList").find(":checkbox").prop("checked", false); 
            $(this).prop("checked", true);
            $("#ProcessTab").css("visibility", "visible");
            $("#Process_Role_Save").siblings(".tips").css("visibility", "hidden");
            $.getJSON("/Maintenance/CommonReportPermission/GetProcessByRole", { roleid: $(this).val(), _t: new Date() }, function (items) {
                processnamelist = new Array();
                for (var key in items) {
                    processnamelist.push(items[key].FullName);
                }                
                var i = 0;
                var checkboxs = $("#ProcessList .k-grid-content").find("input[type='checkbox']");
                checkboxs.each(function () {
                    if ($.inArray($(this).val().toString(), processnamelist)>-1) {
                        $(this).prop("checked", true);
                    }
                    else {
                        $(this).prop("checked", false);
                    }
                    i++;
                });
                if (i == checkboxs.length)
                {
                    hideOperaMask();
                }                
            });
        })
    }
    var InitRoleList = function () {
        showOperaMask();
        $.getJSON("/Maintenance/Applications/GetRelevanceRoleList", { _t: new Date(), pane: window.CurrentApp.pane }, function (items) {
            InitBaseKendoGrid("RoleList", RoleModel, rolecolumns, items, function () {
                hideOperaMask();
                bindGridCheckbox("RoleList");
                InitRoleProcess();
            });           
        })
    }
    
    var ProcessRoleSave = function () {
        showOperaMask();
        var AddidList = new Array();
        var RemoveidList = new Array();        
         
        var checkeddata = new Array();
        var nocheckeddata = new Array();
        $("#ProcessList .k-grid-content").find("input[type='checkbox']").each(function () {
            if ($(this).prop("checked")) {
                checkeddata.push($(this).val());
            }
            else {
                nocheckeddata.push($(this).val());
            }
        });

        $.each(checkeddata, function (i, d) {            
            if ($.inArray(d, processnamelist)==-1) {
                AddidList.push(d);
            }                                  
        })
        $.each(nocheckeddata, function (i, d) {            
            if ($.inArray(d, processnamelist) >= 0) {
                RemoveidList.push(d);
            }                     
        })               
        var id = $("#RoleList .k-grid-content").find(":checked").val();
        $.ajax({
            url: "/Maintenance/CommonReportPermission/SaveRoleProcess",
            type: "POST",
            data: { roleId: id, addProcess: AddidList, removeProcess: RemoveidList },
            traditional: true,
            success: function (data) {
                var temp = new Array();
                for (var key in processnamelist) {
                    if ($.inArray(processnamelist[key],RemoveidList)==-1) {
                        temp.push(processnamelist[key]);                        
                    }
                }
                processnamelist = temp;
                processnamelist = $.merge(processnamelist, AddidList);                
                $("#Process_Role_Save").siblings(".tips").css("visibility", "visible");
                setTimeout(function () {
                    $("#Process_Role_Save").siblings(".tips").css("visibility", "hidden");
                }, 3000);
                hideOperaMask();
            },
            dataType: "json"
        }).fail(function () {
            hideOperaMask();
        });
    }

    var LoadProcessPermission = function () {
        InitRoleList();
        InitProcessList();        
        ManagementSplitter();
        $("#Process_Role_Save").click(ProcessRoleSave);
    }

    module.exports = LoadProcessPermission;
})