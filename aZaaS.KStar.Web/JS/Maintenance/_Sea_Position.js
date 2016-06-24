define(function (require, exports, module) {
    var AddIdlist;
    var EmployeeIdlist;

    //var GetAddIdlist = function () {
    //    var items = new Array();
    //    $.each(AddIdlist, function (i, l) {
    //        if ($.inArray(l, RemoveIdlist) == -1) {
    //            items.push(l);
    //        }
    //    })
    //    return items;
    //}

    //var GetRemoveIdlist = function () {
    //    var items = new Array();
    //    $.each(RemoveIdlist, function (i, l) {
    //        if ($.inArray(l, AddIdlist) == -1) {
    //            items.push(l);
    //        }
    //    })
    //    return items;
    //} 

    var empModel = kendo.data.Model.define({
        id: "StaffId",
        fields: {
            StaffId: { type: "string" },
            UserName: { type: "string" },
            FirstName: { type: "string" },
            LastName: { type: "string" },
            DisplayName: { type: "string" },
            ChineseName: { type: "string" },
            Email: { type: "string" },
            TelNo: { type: "string" },
            FaxNo: { type: "string" },
            MobileNo: { type: "string" },
            Status: { type: "boolean" },
            Department: { type: "string" },
            Position: { type: "string" }
        }
    });

    var empColumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.StaffId + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    { field: "DisplayName", title: jsResxColumns.DisplayName, filterable: true },
    { field: "Department", title: jsResxColumns.Department, filterable: false },
    { field: "Position", title: jsResxColumns.JobTitle, filterable: false }
    ]

    var getPosExFieldValue = function (key, ExFields) {
        var value = "";
        for (var i = 0; i < ExFields.length; i++) {
            if (ExFields[i].Name == key) {
                value = ExFields[i].Value;
                break;
            }
        }
        return value;
    }
    var getPosExFields = function (type) {
        var ExFields = [];
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                if (this.type == "checkbox")
                {
                    ExFields.push({ Name: this.id, Value: $(this).prop("checked") });
                }
                else
                {
                    ExFields.push({ Name: this.id, Value: this.value });
                }                
            }
        });
        for (var index in ExFields) {
            ExFields[index] = obj2str(ExFields[index]);
        }
        return ExFields;
    }

    var ClearPosExFields = function () {
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

    var initPosExFields = function (ExFields) {       
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                var datavalue = getPosExFieldValue(this.id, ExFields);                                
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


    function positions() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/Position/GetManagePosition?_t={0}", new Date());
                    },
                    dataType: "json"
                },
                cache: false
            },
            schema: {
                model: {
                    id: "ID",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    var PostionManageTreeView;
    var InitPostionManageTreeView = function () {
        $("#PostionManageTreeView").kendoTreeView({
            template: kendo.template($("#PostionManageTreeView-template").html()),
            dataSource: positions(),
            select: function (e) {
                $("#PostionInfomation").data("kendoValidator").hideMessages();
                $("#AddContextMenu").parent().hide();
               // $("#DeleteContextMenu").parent().hide();
                //$("#DeleteContextMenu").parent().show();
                $("#AddPositionContextMenu").parent().show();
                //$("#AddCategoryContextMenu").parent().show();
                $("#itemSave").siblings(".tips").css("visibility", "hidden");
                $("#PostionManageTreeView").find("input").prop("checked", false);
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                var Type = node.attr("data-Type");
                $("#itemId").val(node.val());
                $("#itemName").val(node.attr("data-DisplayName"));
                $("#itemParentID").val(node.attr("data-parentid"))

                if (Type == "Position") {
                    $("#AddPositionContextMenu").parent().hide();
                   
                    //$("#AddCategoryContextMenu").parent().hide();
                    $("#PostionInfomation").attr("data-type", "Position");
                    $("#PostionInfomation .info-title").text(jsResxMaintenance_SeaPosition.PositionInfomation);
                    //$("#PostionManageTreeView").parent().siblings().last().children("ul").show();
                    $("#liRelatedUsers").show();
                    $("#liExtendedInformation").show();
                    $.getJSON("/Maintenance/Position/GetEmployee", { _t: new Date(), id: node.val() }, function (items) {
                        $("#employeeList").children().show();
                        UserIdlist = [];
                        for (var key in items) {
                            UserIdlist.push(items[key].StaffId);
                        }
                        InitBaseKendoGridWidthPage("employeeList", empModel, empColumns, items, 5, function () {
                            bindGridCheckbox("employeeList")
                        });
                    });
                    var sysID = node.val(); 
                    if (sysID.indexOf("_") > 0) {
                        sysID = sysID.split('_')[1];
                    }

                    $.getJSON("/Maintenance/Position/GetPositionExtend", { _t: new Date(), id: sysID }, function (items) {
                        initPosExFields(items);
                    })
                }
                else {
                    $("#PostionInfomation").attr("data-type", "Category");
                    $("#PostionInfomation .info-title").text(jsResxMaintenance_SeaPosition.CategoryInfomation);
                    //$("#PostionManageTreeView").parent().siblings().last().children("ul").last().hide();
                    $("#liRelatedUsers").hide();
                    $("#liExtendedInformation").hide();
                }
                $('#PostionManageTreeView .k-state-focused').WinContextMenu({
                    //cancel: '.cancel',
                    menu: "#PositionContextMenu",
                    removeMenu: '#homeBody',
                    action: function (e) {
                        switch (e.id) {
                            //case "AddCategoryContextMenu":
                            //    AddCategory($("#PostionManageTreeView_tv_active").find("input").val()); break;
                            case "AddPositionContextMenu": AddPosition(); break;
                            case "DelContextMenu": DelPosition(); break;
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
                $("#PostionManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#PostionManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);
                $("#PostionManageTreeView").data("kendoTreeView").expand(".k-first");
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
        window.AddSplitters($("#PositionManaView").data("kendoSplitter"));
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
            window.AddSplitters(AddCategoryWindow);
        }
        $("#CategoryName").val("");
        $("#CategoryDesc").val("");
        if (typeof (parentId) == "string") {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", parentId);
        }
        else {
            $("#AddCategoryWindow .windowConfirm").attr("data-parentId", "");
        }
        AddCategoryWindow.open();
        $("#AddCategoryWindow").data("kendoValidator").hideMessages();

    }
    var AddPosition = function () {
        var AddPositionWindow = $("#AddPositionWindow").data("kendoWindow");
        if (!AddPositionWindow) {
            $("#AddPositionWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaPosition.AddPosition,
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
            AddPositionWindow = $("#AddPositionWindow").data("kendoWindow").center();
            window.AddSplitters(AddPositionWindow);
        }

        $("#PositionName").val("");
        $("#PositionDesc").val("");
        if (PostionManageTreeView.select().length > 0) {
            AddPositionWindow.open();
            $("#AddPositionWindow").data("kendoValidator").hideMessages();
        }
        else {
            ShowTip(getJSMsg("_Sea_PositionJS", "SelectCategory"));//"Please select category to add position!");
        }
    }
    var DelPosition = function () {
        var node = $("#PostionManageTreeView_tv_active").find("input:checked");        
        var selectitem = $("#PostionManageTreeView").data("kendoTreeView").dataItem($("#PostionManageTreeView_tv_active"));   
        if (selectitem != undefined && selectitem.children != undefined && selectitem.children._data != undefined && selectitem.children._data.length != undefined && selectitem.children._data.length > 0)
        {
            ShowTip(jsResxMaintenance_SeaPosition.CantDeletePositionCategory, "error");
            return;
        }
        if (node.attr("data-Type") == "Position" && node.val() != null && node.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaPosition.Areyousuretodeletethisposition, function (result) {
                if (result) {
                    $.post("/Maintenance/Position/DestroyPosition", { positionID: node.val() }, function (positionID) {
                        //var nodeposition = PostionManageTreeView.dataSource.get(node.attr("data-parentid")).children.get(positionID);
                        var nodeposition = PostionManageTreeView.dataSource.get(positionID);
                        PostionManageTreeView.dataSource.remove(nodeposition);
                        $("#itemName").val("");
                        $("#PostionManageTreeView").parent().siblings().last().children("ul").children().last().hide();
                        //$("#PostionManageTreeView_tv_active").find("input").parent().parent().parent().remove();
                    });
                }
            });
        }
        else if (node.attr("data-Type") == "Category" && node.val() != null) {
            bootbox.confirm(jsResxMaintenance_SeaPosition.Areyousuretodeletethisposition, function (result) {
                if (result) {
                    $.post("/Maintenance/Position/DestroyCategory", { categoryID: node.val() }, function (categoryID) {
                        PostionManageTreeView.dataSource.remove(PostionManageTreeView.dataSource.get(categoryID));
                        $("#itemName").val("");
                        $("#PostionManageTreeView").parent().siblings().last().children("ul").children().last().hide();
                    });
                }
            });
        }
        else { ShowTip(jsResxMaintenance_SeaPosition.PleaseselectthePositionorCategorytoberemovederror, "error"); }
    }
    var CategoryCancel = function () {
        $("#AddCategoryWindow").data("kendoWindow").close()
    }
    var CategoryConfirm = function () {
        var validator = $("#AddCategoryWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", CategoryConfirm);
            showOperaMask("AddCategoryWindow");
            var categoryName = $("#CategoryName").val();
            var categoryDesc = $("#CategoryDesc").val();
            $.post("/Maintenance/Position/CreateCategory", { DisplayName: categoryName, ParentID: $("#AddCategoryWindow .windowConfirm").attr("data-parentId") }, function (item) {
                //Categories.add(item);

                var treeview = $("#PostionManageTreeView").data("kendoTreeView");
                var select = treeview.select();
                if (item.ParentID == null || item.ParentID == "") {
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
                $("#AddCategoryWindow").data("kendoWindow").close();
            }).fail(function () {
                that.bind("click", CategoryConfirm);
                hideOperaMask("AddCategoryWindow");
            })
        }
    }

    var PositionCancel = function () {
        $("#AddPositionWindow").data("kendoWindow").close()
    }
    var PositionConfirm = function () {
        var validator = $("#AddPositionWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", PositionConfirm);
            showOperaMask("AddPositionWindow");
            var positionName = $("#PositionName").val();
            var positionDesc = $("#PositionDesc").val();
            //var id = $("#PostionManageTreeView_tv_active").find("input").val();
            //var item = Positions.get(id);
            //var parentID = item.ParentID == 0 ? item.PositionID : item.ParentID;
            var parentID = $("#PostionManageTreeView_tv_active").find("input").val();
            //var haschild = node.attr("data-Type");
            //
            //var parentID;
            //if (haschild != 0) {
            //    parentID = node.attr("data-parentid");
            //}
            //else {
            //    parentID = node.val();
            //}
            $.post("/Maintenance/Position/CreatePosition", { DisplayName: positionName, CategoryID: parentID }, function (item) {
                //var parent = Categories.get(item.CategoryID);
                //parent.children.add(item);
                var treeview = $("#PostionManageTreeView").data("kendoTreeView");
                var select = treeview.select();

                if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                    treeview.append(item, select);
                }
                else {
                    treeview.expand(select);
                }
                $("#AddPositionWindow").data("kendoWindow").close()
            }).fail(function () {
                that.bind("click", PositionConfirm);
                hideOperaMask("AddPositionWindow");
            })
        }
    }

    var SaveItem = function () {
        var validator = $("#PostionInfomation").data("kendoValidator");
        if (validator.validate()) {
            showOperaMask("PostionInfomation");
            var id = $("#PostionManageTreeView_tv_active").find("input").val();
            var itemName = $("#itemName").val();

            if (id != ""&&id!=undefined && itemName != "") {
                var node = $("#PostionManageTreeView_tv_active").find("input");
                if (node.attr("data-type") == "Category") {
                    $.post("/Maintenance/Position/UpdateCategory", { CategoryID: id, DisplayName: itemName }, function (item) {

                        var template = kendo.template($("#PostionManageTreeView-template").html())
                        var target = $("#PostionManageTreeView_tv_active .k-state-selected");
                        target.html(template({ item: item }));
                        $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                        if (target.parent().parent().attr("aria-expanded")) {
                            target.find(".k-sprite").first().addClass("on");
                        }
                        hideOperaMask("PostionInfomation");
                        $("#itemSave").siblings(".tips").css("visibility", "visible");
                    }).fail(function () { hideOperaMask("PostionInfomation"); })
                }
                else {
                    //var adds = GetAddIdlist();
                    //var rems = GetRemoveIdlist();
                    var AddidList = [];
                    var RemoveidList = [];
                    var TempidList = [];

                    var data = $("#employeeList").data("kendoGrid").dataSource._data;
                    $.each(data, function (i, d){
                        AddidList.push(d.StaffId);
                        for (var key in UserIdlist) {
                            if (UserIdlist[key] == d.StaffId) {
                                AddidList.pop();
                                break;
                            }
                        }
                        if (!($.inArray(d.StaffId, UserIdlist) == -1)) {
                            TempidList.push(d.StaffId);
                        }
                    })
                    for (var key in UserIdlist) {
                        if (($.inArray(UserIdlist[key], TempidList) == -1)) {
                            RemoveidList.push(UserIdlist[key]);
                        }
                    }


                    $.ajax({
                        url: "/Maintenance/Position/UpdatePosition",
                        type: "POST",
                        data: { PositionID: id, DisplayName: itemName, CategoryID: node.attr("data-parentid"), addids: AddidList, removeids: RemoveidList, ExFields: "[" + getPosExFields().join(',') + "]" },
                        traditional: true,
                        success: function (item) {
                            //var node = Positions.get(item.PositionID);
                            //node.set("DisplayName", item.DisplayName);//更新数据
                            var template = kendo.template($("#PostionManageTreeView-template").html())
                            var target = $("#PostionManageTreeView_tv_active .k-state-selected");
                            target.html(template({ item: item }));
                            $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                            hideOperaMask("PostionInfomation");
                            $("#itemSave").siblings(".tips").css("visibility", "visible");
                        },
                        dataType: "json"
                    }).fail(function () { hideOperaMask("PostionInfomation"); })
                }
            }
            else {
                ShowTip(jsResxMaintenance_SeaPosition.Pleaseselectcategory);
                hideOperaMask(jsResxMaintenance_SeaPosition.PostionInfomation);
            }
        }
    }

    var AddEmployee = function ()
    {
        SelectEmployee(this);
    }

    var SelectEmployee = function (obj) {        
        InitSelectPersonWindow(obj, "Person", function (json) {
            var list = json.Root.Users.Item;
            var data = $("#employeeList").data("kendoGrid").dataSource._data;
            $.each(list, function (i, n) {
                if (!ExistsSelectPerson(n, data)) {
                    $("#employeeList").data("kendoGrid").dataSource.add({ StaffId: n.Value, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' });
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

    //停用
    //var AddEmployee = function () {
    //    var AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
    //    if (!AddEmployeeWindow) {
    //        $("#AddEmployeeWindow").kendoWindow({
    //            width: "500px",
    //            height: "380px",
    //            title: jsResxMaintenance_SeaPosition.AddEmployee,
    //            actions: [
    //                "Close"
    //            ],
    //            open: function (e) {
    //                InitBaseServerKendoGridWidthPage("StaffList", StaffModel,
    //                findstaffcolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
    //                    bindGridCheckbox("StaffList");
    //                    $("#StaffList .k-grid-content").css("height", "190px")
    //                });
    //            },
    //            resizable: false,
    //            modal: true
    //        });
    //        AddEmployeeWindow = $("#AddEmployeeWindow").data("kendoWindow");
    //        window.AddSplitters(AddEmployeeWindow);
    //        AddEmployeeWindow.center();
    //        $("#EmployeeSelect").click(function () {
    //            var input = $("#EmployeeInput").val();

    //            InitBaseServerKendoGridWidthPage("StaffList", StaffModel,
    //            findstaffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 10, function () {
    //                bindGridCheckbox("StaffList");
    //                $("#StaffList .k-grid-content").css("height", "190px")
    //            });
    //        })
    //        $("#AddEmployeeWindow .windowCancel").click(function () {
    //            AddEmployeeWindow.close()
    //        })
    //        $("#AddEmployeeWindow .windowConfirm").click(function () {
    //            var idList = new Array();
    //            var id = $("#PostionManageTreeView_tv_active").find("input").val();
    //            $("#StaffList .k-grid-content").find(":checked").each(function () {
    //                var item = $("#employeeList").data("kendoGrid").dataSource.get(this.value)
    //                if (!item) {
    //                    item = $("#StaffList").data("kendoGrid").dataSource.get(this.value);
    //                    $("#employeeList").data("kendoGrid").dataSource.add(item);
    //                    idList.push(this.value);
    //                }
    //            })
    //            if (idList.length > 0) {
    //                AddEmployeeWindow.close()
    //                //$.ajax({
    //                //    url: "/Maintenance/Position/DoCreateEmployee",
    //                //    type: "POST",
    //                //    data: { id: id, idList: idList },
    //                //    traditional: true,
    //                //    success: function (items) {
    //                //        for (var i = 0; i < items.length; i++) {
    //                //            $("#employeeList").data("kendoGrid").dataSource.add(items[i]);
    //                //        }
    //                //        AddEmployeeWindow.close()
    //                //    },
    //                //    dataType: "json"
    //                //})
    //            }
    //        })
    //    }
    //    AddEmployeeWindow.open();
    //}

    var DelEmployee = function () {
        var idList = new Array();
        var id = $("#PostionManageTreeView_tv_active").find("input").val();
        $("#employeeList .k-grid-content").find(":checked").each(function () {
            var item = $("#employeeList").data("kendoGrid").dataSource.get(this.value);
            $("#employeeList").data("kendoGrid").dataSource.remove(item);
            idList.push(this.value)
        })
        if (idList.length > 0) {
            //$.ajax({
            //    url: "/Maintenance/Position/DoDestroyEmployee",
            //    type: "POST",
            //    data: { id: id, idList: idList },
            //    traditional: true,
            //    success: function (idList) {
            //        for (var i = 0; i < idList.length; i++) {
            //            var dataItem = $("#employeeList").data("kendoGrid").dataSource.get(idList[i]);
            //            $("#employeeList").data("kendoGrid").dataSource.remove(dataItem);
            //        }
            //    },
            //    dataType: "json"
            //})
        }
        else {
            ShowTip(jsResxMaintenance_SeaPosition.Pleaseselectemployeeerror);
        }
    }

    function LoadPostionView() {        
        window.title = "Postion Management - Kendo UI";
        InitPositionSplitter();        
        InitPostionManageTreeView();      
        TreeViewNodeToggle("PostionManageTreeView");

        $("#CategoryAdd").click(function () { AddCategory(""); })
        $("#PositionAdd").click(AddPosition)
        $("#itemSave").click(SaveItem)
        var AddEmployeeWindow;
        $("#EmployeeAdd").click(AddEmployee)
        $("#EmployeeDel").click(DelEmployee)
        $("#PostionInfomation").children("ul").kendoPanelBar();

        //$.contextMenu({
        //    selector: '#PostionManageTreeView_tv_active .k-state-selected',//input:checked .k-in .k-state-selected
        //    callback: function (key, options) {
        //        switch (key) {
        //            case "add": AddPosition(); break;
        //            case "delete": DelPosition(); break;
        //        }
        //    },
        //    items: {
        //        "add": { name: "Add", icon: "add" },
        //        "delete": { name: "Delete", icon: "delete" }
        //    }
        //});
    }

    module.exports = LoadPostionView;
})