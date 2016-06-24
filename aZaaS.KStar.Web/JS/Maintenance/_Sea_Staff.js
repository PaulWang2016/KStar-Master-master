define(function (require, exports, module) {
    //===================================column and model===========================

    var getStaffExFieldValue = function (key, ExFields) {
        var value = "";
        for (var i = 0; i < ExFields.length; i++) {
            if (ExFields[i].Name == key) {
                value = ExFields[i].Value;
                break;
            }
        }
        return value;
    }
    var getStaffExFields = function () {
        var ExFields = [];
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                if (this.type == "checkbox") {
                    ExFields.push({ Name: this.id, Value: $(this).prop("checked") });
                }
                else {
                    ExFields.push({ Name: this.id, Value: this.value });
                }
            }
        });
        for (var index in ExFields) {
            ExFields[index] = obj2str(ExFields[index]);
        }
        return ExFields;
    }

    var ClearStaffExFields = function () {
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                var type = this.type;
                var data_role = $(this).attr("data-role");
                if (data_role != undefined) {
                    switch (data_role) {
                        case "dropdownlist":
                            $(this).data("kendoDropDownList").select(0);
                            break;
                        case "numerictextbox":
                            $(this).data("kendoNumericTextBox").value(0);
                            break;
                        case "datepicker":
                            $(this).data("kendoDatePicker").value("");
                            break;
                    }
                }
                else if (type == "text") {
                    $(this).val("");
                }
                else if (type == "checkbox") {
                    $(this).prop("checked", false);
                }
            }
        });
    }

    var initStaffExFields = function (ExFields) {
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                var datavalue = getStaffExFieldValue(this.id, ExFields);
                var type = this.type;
                var data_role = $(this).attr("data-role");
                if (data_role != undefined) {
                    switch (data_role) {
                        case "dropdownlist":
                            $(this).data("kendoDropDownList").value(datavalue);
                            break;
                        case "numerictextbox":
                            $(this).data("kendoNumericTextBox").value(datavalue);
                            break;
                        case "datepicker":
                            $(this).data("kendoDatePicker").value(datavalue);
                            break;
                    }
                }
                else if (type == "text") {
                    $(this).val(datavalue);
                }
                else if (type == "checkbox") {                    
                    $(this).prop("checked", (datavalue.toLowerCase() == "true" ? true : false));
                }
            }
        });
    }

    var staffcolumns = [
     {
         title: jsResxMaintenance_SeaStaff.Checked, width: 35, template: function (item) {
             return "<input value='" + item.StaffId + "' type='checkbox' />";
         }, headerTemplate: "<input type='checkbox' />", filterable: false

     },
     /*{ field: "StaffId", title: "Staff Id", filterable: false },*/
     { field: "FirstName", title: jsResxMaintenance_SeaStaff.FirstName, filterable: false },
     { field: "LastName", title: jsResxMaintenance_SeaStaff.LastName, filterable: false },
     { field: "DisplayName", title: jsResxMaintenance_SeaStaff.DisplayName, filterable: false },     
     { field: "Email", title: jsResxMaintenance_SeaStaff.Email, filterable: false },  
     { field: "MobileNo", title: jsResxMaintenance_SeaStaff.MobileNo, filterable: false },
     {
         field: "Status", title: jsResxMaintenance_SeaStaff.Status, width: 58, template: function (item) {
             return item.Status ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>"
         }, filterable: false
     }     
    ]

    var StaffModelWithExtends = kendo.data.Model.define({
        id: "StaffId",
        fields: {
            StaffId: { type: "string" },
            FirstName: { type: "string" },
            LastName: { type: "string" },
            DisplayName: { type: "string" },            
            Email: { type: "string" },          
            MobileNo: { type: "string" },
            Status: { type: "boolean" }
        }
    });

    //===============================Staff View====================================
    function resetAddStaffWindow() {
        hideOperaMask("AddStaffWindow");
        $("#staffTab .k-textbox").val("");//清除输入框

        $("#StaffSex").data("kendoDropDownList").select(0);//清除下拉框
        //$("#StaffStatus").data("kendoDropDownList").select(0);
        $("#StaffStatus").data("kendoDropDownList").value("true");
    }//重置表单

    var StaffCancel = function () {
        $("#AddStaffWindow").data("kendoWindow").close()
    }
    var StaffConfirm = function () {
        var validator = $("#staffTab").data("kendoValidator");
        if (validator.validate()) {
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
            var Departmentidlist = new Array();
            $("#DepartmentList .k-grid-content").find("input").each(function () {
                Departmentidlist.push(this.value);
            })
            
            var url = $(this).attr("data-url");
            var data = {
                StaffId: $("#AddStaffWindow .windowConfirm").attr("data-id"),
                UserId: $("#UserId").val(),
                FirstName: $("#FirstName").val(),
                LastName: $("#LastName").val(),
                UserName: $("#StaffName").val(),
                Email: $("#Email").val(),
                MobileNo: $("#MobileNo").val(),
                Status: $("#StaffStatus").val(),
                Sex: $("#StaffSex").val(),
                Remark: $("#Remark").val(),
                Address: "",
                ExFields: "[" + getStaffExFields() + "]",
                ReportToidlist: ReportToidlist,
                Roleidlist: Roleidlist,
                Positionidlist: Positionidlist,
                Departmentidlist: Departmentidlist
            }

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                traditional: true,
                success: function (item) {
                    var grid = getKendoGrid("UserManaView");
                    grid.dataSource.read();
                    //var model = grid.dataSource.get(item.StaffId);

                    //if (model) {
                    //    for (var key in item) {
                    //        model.set(key, item[key]);
                    //    }
                    //}
                    //else {
                    //    grid.dataSource.add(item)
                    //}
                    $("#AddStaffWindow").data("kendoWindow").close()
                },
                dataType: "json"
            }).fail(function () {
                hideOperaMask("AddStaffWindow");
                that.bind("click", StaffConfirm);
            })
        }
    }

    var OpenAddStaff = function (e) {
        $("#AddStaffWindow .windowCancel").bind("click", StaffCancel)
        $("#AddStaffWindow .windowConfirm").bind("click", StaffConfirm)
        $("#staffOtherTab .mt-Add").bind("click", OtherAdd);
        $("#staffOtherTab .mt-Delete").bind("click", OtherDel);
        $("#DepartmentInformation .mt-Add").bind("click", AddDepartment);
        $("#DepartmentInformation .mt-Delete").bind("click", DelDepartment);
    }
    var CloseAddStaff = function (e) {
        resetAddStaffWindow();
        $("#AddStaffWindow .windowCancel").unbind("click", StaffCancel);
        $("#AddStaffWindow .windowConfirm").unbind("click", StaffConfirm);
        $("#staffOtherTab .mt-Add").unbind("click", OtherAdd);
        $("#staffOtherTab .mt-Delete").unbind("click", OtherDel);
        $("#DepartmentInformation .mt-Add").unbind("click", AddDepartment);
        $("#DepartmentInformation .mt-Delete").unbind("click", DelDepartment);
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
            switch ($("#AddOtherWindow").attr("addtype")) {
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
        if (target.attr("data-wtitle") == "ReportTo") {
            //AddEmployee().center().open();//.title(target.attr("data-wtitle"))
            SelectEmployee(target);
            return;
        }
        else if (target.attr("data-wtitle") == "Position")
        {
            SelectPosition(target);
            return;
        }
        var tt = target.attr("data-url");
        $.getJSON(target.attr("data-url"), { _t: new Date() }, function (items) {
            InitDropDownList("AddOther", target.attr("data-text"), target.attr("data-value"), target.attr("data-label"), items)
        })
        $("#AddOtherWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaStaff[target.attr("data-wtitle")]).open();
        $("#AddOtherWindow").attr("addtype", target.attr("data-wtitle"));
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

    var AddDepartment = function () {
        var target = $(this);
        SelectDepartment(target);       
    }

    var DelDepartment = function () {
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

    //代替AddEmployee
    var SelectEmployee = function (obj)
    {                    
        InitSelectPersonWindow(obj, "Person", function (json) {
            var userlist = json.Root.Users.Item;
            var data = $("#ReportToList").data("kendoGrid").dataSource._data;
            $.each(userlist, function (i, n) {
                if (!ExistsSelectPerson(n, data)) {
                    $("#ReportToList").data("kendoGrid").dataSource.add({ StaffId: n.Value, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' });
                }
            });            
        })
    }

    function ExistsSelectPerson(item, data) {        
        var flag = false;
        $.each(data, function (i, n) {
            if (n.StaffId == item.Value) {
                flag = true;
            }
        });
        return flag;
    }

    //选职位
    var SelectPosition = function (obj) {        
        InitSelectPersonWindow(obj, "Position", function (json) {
            var list = json.Root.Positions.Item;
            var data = $("#PositionList").data("kendoGrid").dataSource._data;
            $.each(list, function (i, n) {                
                if (!ExistsSelectPosition(n, data)) {
                    $("#PositionList").data("kendoGrid").dataSource.add({ PositionID: n.Value, DisplayName: n.Name });
                }
            });
        })
    }

    //选部门
    var SelectDepartment = function (obj)
    {       
        InitSelectPersonWindow(obj, "Department", function (json) {
            var list = json.Root.Depts.Item;
            $.each(list, function (i, n) {
                var item=$("#DepartmentList").data("kendoGrid").dataSource.get(n.Value);
                if (!item) {
                    $("#DepartmentList").data("kendoGrid").dataSource.add({ DepartmentID: n.Value, DisplayName: n.Name });
                }
            });
        })
    }

    function ExistsSelectPosition(item, data) {        
        var flag = false;
        $.each(data, function (i, n) {
            if (n.PositionID == item.Value) {
                flag = true;
            }
        });
        return flag;
    }

    //弃用
    var AddEmployee = function () {
        var AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
        if (!AddEmployeeWindow) {
            $("#AddEmployeeWindow").kendoWindow({
                width: "500px",
                height: "380px",
                title: jsResxMaintenance_SeaStaff.AddEmployee,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    InitBaseServerKendoGridWidthPage("StaffList", StaffModel,
                           reporttocolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
                               bindGridCheckbox("StaffList");
                               $("#StaffList .k-grid-content").css("height", "190px")
                           });
                },
                resizable: false,
                modal: true
            });
            AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
            window.AddSplitters(AddEmployeeWindow);
            AddEmployeeWindow.center();
            $("#EmployeeSelect").click(function () {
                var input = $("#EmployeeInput").val();
                InitBaseServerKendoGridWidthPage("StaffList", StaffModel,
                       reporttocolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 10, function () {
                           bindGridCheckbox("StaffList");
                           $("#StaffList .k-grid-content").css("height", "190px")
                       });
            })
            $("#AddEmployeeWindow .windowCancel").click(function () {
                AddEmployeeWindow.close()
            })
            $("#AddEmployeeWindow .windowConfirm").click(function () {

                $("#StaffList .k-grid-content").find(":checked").each(function () {
                    //idList.push(this.value)
                    var item = $("#StaffList").data("kendoGrid").dataSource.get(this.value);
                    if (!$("#ReportToList").data("kendoGrid").dataSource.get(this.value))
                        $("#ReportToList").data("kendoGrid").dataSource.add(item);
                })
                AddEmployeeWindow.close();
            })
        }
       return AddEmployeeWindow;
    }
    //==================局部滚动========================
    function isTouchDevice() {
        try {
            document.createEvent("TouchEvent");
            return true;
        } catch (e) {
            return false;
        }
    }
    function touchScroll(id) {
        if (isTouchDevice()) { //if touch events exist...
            var el = document.getElementById(id);
            var scrollStartPos = 0;

            document.getElementById(id).addEventListener("touchstart", function (event) {
                scrollStartPos = this.scrollTop + event.touches[0].pageY;
            }, false);

            document.getElementById(id).addEventListener("touchmove", function (event) {
                this.scrollTop = scrollStartPos - event.touches[0].pageY;
            }, false);
        }
    }
    //===============================Staff View====================================
    var InitUserWindow = function () {
        var width = $(window).width() - 100 + "px";
        var height = $(window).height() - 100 + "px";
        var heig = $(window).height();
        $("#staffTab").height(heig - 160);
        touchScroll("staffTab");
        $("#AddStaffWindow").kendoWindow({
            width: width,
            height: height,
            title: jsResxMaintenance_SeaStaff.AddStaff,
            actions: [
                "Close"
            ],
            open: OpenAddStaff,
            close: CloseAddStaff,
            resizable: false,
            modal: true
        });
        window.AddSplitters($("#AddStaffWindow").data("kendoWindow"));

        $("#AddOtherWindow").kendoWindow({
            width: "300px",
            title: jsResxMaintenance_SeaStaff.AddOther,
            animation: false,
            actions: [
                "Close"
            ],
            open: OpenAddOther,
            close: CloseAddOther,
            resizable: false,
            modal: true
        });
        window.AddSplitters($("#AddOtherWindow").data("kendoWindow"));

    }
    var InitUserWindowView = function () {
        //===============================TabStrip====================================
        $("#staffTab").children().kendoPanelBar();
        $("#staffOtherTab").children().kendoPanelBar();

        //===============================DropDownList====================================
        $("#StaffSex").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: jsResxMaintenance_SeaStaff.Male, value: "Male" }, { text: jsResxMaintenance_SeaStaff.Female, value: "Female" }],
            optionLabel: jsResxMaintenance_SeaStaff.SelectSex
        });
        $("#StaffStatus").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: jsResxMaintenance_SeaStaff.Inactive, value: "false" }, { text: jsResxMaintenance_SeaStaff.Active, value: "true" }],
            ptionLabel: jsResxMaintenance_SeaStaff.SelectStatus
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
                optionLabel: jsResxMaintenance_SeaStaff.SelectPosition
            });
        })
        //===============================DropDownList====================================

    }
    var AddUser = function () {
        $("#AddStaffWindow .windowConfirm").attr("data-id", "");
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
        InitBaseKendoGrid("DepartmentList", DepartmentModel, departmentcolumns, [], function () {
            bindGridCheckbox("DepartmentList");
        });
        ClearStaffExFields();
        
        $("#AddStaffWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaStaff.AddUser).open();
        $("#staffTab").data("kendoValidator").hideMessages();
        $("#StaffName").attr("disabled", false).css("background-color", "#ffffff");
        $("#StaffStatus").data("kendoDropDownList").value("true");
        $("#hdUserId").val("");
        $("#hdStaffName").val("");
    }

    var UserImportFromAD = function () {
        $.ajax({
            type: "POST",
            url: "/Maintenance/Staff/ImportStaffFromAD",
            data: null,
            dataType: "json",
            success: function (flag) {
                if (flag) {
                    var grid = getKendoGrid("UserManaView");
                    grid.dataSource.read();
                    ShowTip(jsResxMaintenance_SeaStaff.Success);
                }
                else {
                    ShowTip(jsResxMaintenance_SeaStaff.Failure);
                }
            },
            beforeSend: function (xhr) {
                showOperaMask();
            },
            complete: function (xht, ts) {
                hideOperaMask();
            }
        });             
    }

    var InitUserImportWindow = function () {             
        $("#UserImportWindow").kendoWindow({
            width: "500px",
            height: "160px",
            title: jsResxMaintenance_SeaStaff.ImportfromExcel,
            actions: [
                "Close"
            ],
            open: function () {
                $("#UserImportWindow .windowCancel").bind("click", UserImportFromExcelCancel);
                $("#UserImportWindow .windowConfirm").bind("click", UserImportFromExcelConfirm);
            },
            close: function () {
                $("#UserImportWindow .windowCancel").unbind("click", UserImportFromExcelCancel);
                $("#UserImportWindow .windowConfirm").unbind("click", UserImportFromExcelConfirm);
            },
            resizable: false,
            modal: true
        });
    }

    var UserImportFromExcel = function ()
    {
        $("#UserImportWindow .windowConfirm").attr("data-id", "");
        $("#UserImportWindow .windowConfirm").attr("data-url", "/Maintenance/Staff/UserImportFromExcel");
        $("#UserImportWindow").data("kendoWindow").center().open();
        $("#Editortextfield").val("");
    }

    var UserImportFromExcelCancel = function ()
    {
        $("#UserImportWindow").data("kendoWindow").close();
    }
    var UserImportFromExcelConfirm = function () {
        var that = $(this);
        that.unbind("click", UserImportFromExcelConfirm);
        showOperaMask();
        var file = $("#EditorfileField").val();       
        if (file.length > 0) {
            var point = file.lastIndexOf(".");
            var type = file.substr(point);
            if (".xls.xlsx".indexOf(type) >= 0) {
                $.ajaxFileUpload({
                    url: that.attr("data-url"),
                    data: {},
                    secureuri: false,
                    fileElementId: 'EditorfileField',
                    dataType: 'text/html',
                    success: function (data,status) {
                         
                        data = $(data).html();
                        data = jQuery.parseJSON(data);
                        if (data.flag) {
                            $("#UserImportWindow").data("kendoWindow").close();
                            hideOperaMask();
                            getKendoGrid("UserManaView").dataSource.read();
                            if (data.msg != undefined && data.msg.length > 0)
                            {
                                ShowTip(data.msg);
                            }
                        }
                        else {
                            ShowTip(jsResxMaintenance_SeaStaff.Importfailed);
                            that.bind("click", UserImportFromExcelConfirm);
                            hideOperaMask();
                        }
                    },
                    error: function (data, status, e) {
                        that.bind("click", UserImportFromExcelConfirm);
                        hideOperaMask();
                    }
                });                
            }
            else {
                //不允许格式
                that.bind("click", UserImportFromExcelConfirm);
                hideOperaMask();
                ShowTip(jsResxMaintenance_SeaStaff.AllowFileType);
            }
        }
        else {
            that.bind("click", UserImportFromExcelConfirm);
            hideOperaMask();
            ShowTip(jsResxMaintenance_SeaStaff.Pleaseselectfile);
        }
    }

    var EditStaff = function (id) {

        if (id != undefined) {
            $.getJSON("/Maintenance/Staff/GetStaffOtherInfo", { id: id, _t: new Date() }, function (item) {
                var model = getKendoGrid("UserManaView").dataSource.get(id);
                $("#AddStaffWindow .windowConfirm").attr("data-id", model.StaffId);
                $("#hdUserId").val(model.UserId);
                $("#hdStaffName").val(model.UserName);
                $("#UserId").val(model.UserId);
                $("#StaffName").val(model.UserName).attr("disabled", true).css("background-color", "#e3e3e3");
                $("#FirstName ").val(model.FirstName);
                $("#LastName").val(model.LastName);
                $("#Email").val(model.Email);
                $("#StaffStatus").data("kendoDropDownList").value(model.Status == true ? "true" : "false");
                $("#StaffSex").data("kendoDropDownList").value(model.Sex);
                $("#Remark").val(model.Remark);
                $("#MobileNo").val(model.MobileNo);      
                //$("#Department").val(model.Department);
                
                //console.log(item.ExtendItems);
                initStaffExFields(item.ExtendItems);

                InitBaseKendoGrid("ReportToList", StaffModel, reporttocolumns, item.ReportToList, function () {
                    bindGridCheckbox("ReportToList");
                });
                InitBaseKendoGrid("RoleList", RoleModel, rolecolumns, item.RoleList, function () {
                    bindGridCheckbox("RoleList"); 
                });
                InitBaseKendoGrid("PositionList", PositionModel, positioncolumns, item.PositionList, function () {
                    bindGridCheckbox("PositionList");
                });
                InitBaseKendoGrid("DepartmentList", DepartmentModel, departmentcolumns, item.departmentlist, function () {
                    bindGridCheckbox("DepartmentList");
                });
                
                $("#AddStaffWindow .windowConfirm").attr("data-url", "/Maintenance/Staff/EditStaff");
                $("#AddStaffWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaStaff.EditUser).open();
                $("#staffTab").data("kendoValidator").hideMessages();
            })
        }
        else {
            ShowTip(jsResxMaintenance_SeaStaff.Pleaseselectemployees);
        }
    }
    var DisableUser = function () {
        var idList = new Array();
        $("#UserManaView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            confirmDisableUser(idList);
        }
        else {
            ShowTip(jsResxMaintenance_SeaStaff.Pleaseselectemployeeserror);
        }
    }

    var confirmDisableUser = function (idList) {
        bootbox.confirm(jsResxMaintenance_SeaStaff.Areyousuredisableuser, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Staff/DoDisableStaff",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            //$("#UserManaView .k-grid-content").find(":checked").prop("checked", false).parent().parent()
                            //    .find(".k-icon").removeClass("k-i-tick").addClass("k-i-cancel");
                            //for (var i = 0; i < idList.length; i++) {
                            //    getKendoGrid("UserManaView").dataSource.get(idList[i]).set("Status", false);
                            //}
                            getKendoGrid("UserManaView").dataSource.read();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }

    var ActiveUser=function(idList) {
        bootbox.confirm(jsResxMaintenance_SeaStaff.Areyousureactiveuser, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Staff/DoActiveStaff",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {                       
                            getKendoGrid("UserManaView").dataSource.read();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }


    var DeleUsers = function () {        
        var idList = new Array();
        $("#UserManaView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaStaff.Areyousuredeleteuser, function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Staff/DoDeleUsers",
                        type: "POST",
                        data: { idList: idList },
                        traditional: true,
                        success: function (isSuccess) {
                            if (isSuccess) {
                                //for (var i = 0; i < idList.length; i++) {
                                //    var dataItem = getKendoGrid("UserManaView").dataSource.get(idList[i]);
                                //    getKendoGrid("UserManaView").dataSource.remove(dataItem);
                                //}
                                getKendoGrid("UserManaView").dataSource.read();
                            }
                        },
                        dataType: "json"
                    })
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaStaff.Pleaseselectemployeeserror);
        }
    }
    function LoadStaffView() {        
        $.ajax({
            url: "/Maintenance/Staff/GetStaffExtendFields",
            type: "POST",            
            traditional: true,
            async:false,
            success: function (fields) {                
                //初始化
                staffcolumns = [
                     {
                         title: jsResxMaintenance_SeaStaff.Checked, width: 35, template: function (item) {
                             return "<input value='" + item.StaffId + "' type='checkbox' />";
                         }, headerTemplate: "<input type='checkbox' />", filterable: false

                     },
                     /*{ field: "StaffId", title: "Staff Id", filterable: false },*/
                     //{ field: "FirstName", title: jsResxMaintenance_SeaStaff.FirstName, filterable: true },
                     //{ field: "LastName", title: jsResxMaintenance_SeaStaff.LastName, filterable: true },                     
                     { field: "UserId", title: jsResxMaintenance_SeaStaff.UserId, filterable: true },
                     {
                         field: "DisplayName", title: jsResxMaintenance_SeaStaff.DisplayName, template: function (item) {
                             if (item.Sex.length > 0) {
                                 return (item.Sex == "Male" ? "<img alt='男' width='15' height='20' src='/images/StaffIcons/male.png' style='vertical-align:bottom;' />" : "<img alt='女' width='15' height='20' src='/images/StaffIcons/female.png'  style='vertical-align:bottom;' />") + item.DisplayName;
                             }
                             else {
                                 return item.DisplayName;
                             }
                         }, filterable: false
                     },
                     { field: "UserName", title: jsResxMaintenance_SeaStaff.UserName, filterable: true },
                     { field: "MobileNo", title: jsResxMaintenance_SeaStaff.MobileNo, filterable: true },
                     { field: "Email", title: jsResxMaintenance_SeaStaff.Email, filterable: true },
                     { field: "Department", title: jsResxMaintenance_SeaStaff.Department, filterable: false, sortable: false },
                     { field: "Position", title: jsResxMaintenance_SeaStaff.Position, filterable: false, sortable: false },
                     { field: "ReportTo", title: jsResxMaintenance_SeaStaff.ReportTo, filterable: false, sortable: false },
                     {
                         field: "Status", title: jsResxMaintenance_SeaStaff.Status, width: 58, template: function (item) {
                             return item.Status ? "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.StaffId + "'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.StaffId + "'><span class='glyphicon glyphicon-ban-circle'></span></a>"
                         }, filterable: false
                     }
                ];
                //初始化
                StaffModelWithExtends = kendo.data.Model.define({
                    id: "StaffId",
                    fields: {
                        StaffId: { type: "string" },
                        UserId:{type:"string"},
                        DisplayName: { type: "string" },
                        UserName: { type: "string" },
                        FirstName: { type: "string" },
                        LastName: { type: "string" },                        
                        ChineseName: { type: "string" },
                        MobileNo: { type: "string" },
                        Email: { type: "string" },
                        Department: { type: "string" },
                        Position: { type: "string" },
                        ReportTo: { type: "string" },
                        Status: { type: "boolean" }
                    }
                });
                if (fields != undefined && fields != null && fields.length>0) {  
                    $.each(fields, function (i) {
                        //添加model                        
                        var name = fields[i].Name;
                        var displayname = fields[i].DisplayName;
                        var obj = {};
                        obj[name] = {};
                        if (fields[i].Type == "YesNoField") {
                            //添加column
                            staffcolumns.push({
                                field: name, title: displayname, width: 58, template: function (item) {
                                    return item[name]=="True" ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>"
                                }, filterable: false, hidden: true, sortable: false
                            });
                            obj[name].type = "boolean";
                        }
                        else {
                            //添加column
                            staffcolumns.push({ field: name, title: displayname, filterable: false, hidden: true, sortable: false });
                            obj[name].type = "string";
                        }
                        StaffModelWithExtends.fields[name] = obj[name];

                        
                    });                    
                    staffcolumns.push({ command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditStaff(data.StaffId) } }], width: 58 });
                }
            },
            dataType: "json"
        });
        title = "User Management - Kendo UI";        
        InitServerCustomKendoExcelGrid("UserManaView", StaffModelWithExtends, staffcolumns, "/Maintenance/Staff/GetStaffs", {}, $(window).height() - fullwidgetH, jsResxMaintenance_SeaStaff.UserManagement, "/Maintenance/Staff/GetAllStaffsForExcel", function () {
            bindAndLoad("UserManaView");
            bindGridCheckbox("UserManaView");
                   
            if ($("#AuthType").val() == "Windows") {
                $("#UserManaView .k-toolbar")
                .append("<a id='UserImport' class='k-button' title='" + jsResxMaintenance_SeaStaff.ImportfromAD + "' href='javascript:void(0)'><span class='k-grid-export-image k-icon'></span></a>")
            }
            $("#UserManaView .k-toolbar")                
                .append("<a id='UserImportFromExcel' class='more k-button' href='javascript:void(0)' style='height:26px;' title='" + jsResxMaintenance_SeaStaff.ImportfromExcel + "'><span class='glyphicon excel-ico'></span></a>")
                .append("<a id='UserDelete' class='more k-button' href='javascript:void(0)'><span class='glyphicon glyphicon-remove'></span></a>")
                .append("<a id='UserDisable' class='more k-button' href='javascript:void(0)'><span class='glyphicon glyphicon-ban-circle'></span></a>")
                .append(" <a id='UserAdd'  class='more k-button'  href='javascript:void(0)'><span class='glyphicon glyphicon-plus'></span></a>");            

            $("#UserImport").click(UserImportFromAD);
            $("#UserAdd").click(AddUser);
            $("#UserDisable").click(DisableUser);
            $("#UserDelete").click(DeleUsers);
            $("#UserImportFromExcel").click(UserImportFromExcel);
        });        

        $("#UserManaView").prev().find(".selectbtn").click(function () {
            var selectInput = $("#UserManaView").prev().find("input[name=selectInput]").val();

            InitServerQueryCustomKendoExcelGrid("UserManaView", StaffModelWithExtends, staffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: selectInput }, $(window).height() - fullwidgetH, jsResxMaintenance_SeaStaff.UserManagement, "/Maintenance/Staff/GetAllStaffsForExcel", function () {
                bindAndLoad("UserManaView");
                bindGridCheckbox("UserManaView");
            });
        })

        $("#UserManaView").delegate("a.k-Status", "click", function () {            
            var ok = $(this).find("span.glyphicon-ok");
            var idList = new Array();
            var id = $(this).attr("id");
            idList.push(id);            
            if (ok.length==1) {
                confirmDisableUser(idList);
            }
            else {                                
                ActiveUser(idList);
            }
        })

        InitUserWindow();
        InitUserWindowView();
        InitUserImportWindow();
    }
    module.exports = LoadStaffView;
})