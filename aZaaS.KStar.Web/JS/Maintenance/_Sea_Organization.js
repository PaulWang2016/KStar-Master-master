define(function (require, exports, module) {
    var ListIDMenuTree;
    var TypeMenuTree;

    function organizations() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/Organization/GetOrganization?ListID={0}&Type={1}&_t={2}", ListIDMenuTree, TypeMenuTree, new Date());
                    },
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "ID",               //绑定ID
                    hasChildren: "HasChildNode"  //绑定是否包含子节点                 
                }
            }
        });
    }

    var getOrgExFieldValue = function (key, ExFields) {
        var value = "";
        for (var i = 0; i < ExFields.length; i++) {
            if (ExFields[i].Name == key) {
                value = ExFields[i].Value;
                break;
            }
        }
        return value;
    }
    var getOrgExFields_save = function (type) {
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
    var getOrgExFields = function (type) {
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

    var ClearOrgExFields = function () {       
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
                else if(type=="text") {
                    $(this).val("");
                }
                else if (type == "checkbox")
                {                    
                    $(this).prop("checked", false);
                }
            }
        });        
    }


    function InitTreeView() {
        $("#Information").hide();
        var OrganizationTreeKendo = $("#OrganizationManageTreeView").data("kendoTreeView");
        if (!OrganizationTreeKendo) {
            $("#OrganizationManageTreeView").kendoTreeView({
                template: kendo.template($("#OrganizationManageTreeView-template").html()),
                dataSource: organizations(),

                select: function (e) {
                    $("#Information").data("kendoValidator").hideMessages();

                    $("#Organization_Save").siblings(".tips").css("visibility", "hidden");
                    $("#OrganizationManageTreeView").find("input").prop("checked", false);
                    $("#Information").show();
                    $("#AddContextMenu").parent().show();
                    $("#DeleteContextMenu").parent().show();
                    var select = $("#OrganizationManageTreeView_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息

                    var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());
                    //TypeMenuTree = item.NodeName;
                    var name, type, code;
                    switch (item.Type) {
                        case "Property":                            
                            $("#BasicInformation").empty().html($("#editproperty").html());                         
                            $("#BasicInformation .propertyName").val(item.NodeName);
                            $("#BasicInformation .propertyType").val(item.Type);
                            //$("#BasicInformation .propertyType").val(item.Type);
                            //ExFields                                                      
                            //.ExFields
                            //$("#OrganizationManageTreeView  .PropertyType").removeClass("PropertyType");
                            //$("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("PropertyType");
                            $("#AddContextMenu").parent().hide();
                            $("#OrganizationManaView").show();
                            break;
                        case "Cluster":
                            $("#BasicInformation").empty().html($("#SecondBasicInfo").html());
                            $("#BasicInformation .SecondName").val(item.NodeName);
                            $("#BasicInformation .SecondType").val(item.Type);

                            //$("#OrganizationManageTreeView  .Type").removeClass("Type");
                            //$("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("Type");
                            $("#OrganizationManaView").show();
                            break;
                        case "Division":
                            $("#BasicInformation").empty().html($("#FirstBasicInfo").html());
                            $("#BasicInformation .FirstName").val(item.NodeName);
                            $("#BasicInformation .FirstType").val(item.Type);

                            //$("#OrganizationManageTreeView  .Type").removeClass("Type");
                            //$("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("Type");
                            $("#OrganizationManaView").show();
                            break;
                        case "Company":
                            $("#BasicInformation").empty().html($("#OrgBasicInfo").html());
                            $("#BasicInformation .OrganizationName").val(item.NodeName);
                            $("#BasicInformation .OrganizationType").val(item.Type);

                            //$("#OrganizationManageTreeView  .Type").removeClass("Type");
                            //$("#OrganizationManageTreeView_tv_active .k-state-focused").find("input").parent().addClass("Type");
                            $("#OrganizationManaView").show();
                            break;
                    }
                    initNodePositionList();
                    initNodeUserList();                    
                    initNodeExFields(item.ExFields);
                    
                    $('#OrganizationManageTreeView .k-state-focused').WinContextMenu({
                        //cancel: '.cancel',
                        removeMenu: '#homeBody',
                        action: function (e) {
                            switch (e.id) {
                                case "AddContextMenu": addNode(); break;
                                case "DeleteContextMenu": delNode(); break;
                            }
                        }//自由设计项事件回调
                    });
                    $("#Information").children("ul").kendoPanelBar();
                },
                collapse: function (e) {
                    $("#OrganizationManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
                },
                expand: function (e) {
                    $("#OrganizationManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
                },
                dataBound: function (e) {
                    var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                    var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                    $("#OrganizationManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                    $("#OrganizationManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)

                    $("#OrganizationManageTreeView").data("kendoTreeView").expand(".k-first");
                }
            });
            OrganizationTreeKendo = $("#OrganizationManageTreeView").data("kendoTreeView");
        }
        else {
            OrganizationTreeKendo.setDataSource(organizations());
        }
    }

    function ExportCharts() {
        $.post("/Export/ExportCharttoXml", { chartId: ListIDMenuTree }, function (title) {
            window.location.replace("/Export/GetXml?title=" + title);
        });
    }

    var InitOrganizationSplitter = function () {
        $("#OrganizationManaView").kendoSplitter({
            panes: [
                { collapsible: false, size: "300px", min: "300px", max: "450px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        window.AddSplitters($("#OrganizationManaView").data("kendoSplitter"));
    }

    var InitOrganizationWindow = function () {
        var AddOtherWindows = $("#AddOtherWindows").data("kendoWindow");
        if (!AddOtherWindows) {
            $("#AddOtherWindows").kendoWindow({
                width: "500px",
                height: "380px",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddOtherWindows .windowCancel").bind("click", othercancel)
                    $("#AddOtherWindows .windowConfirm").bind("click", otherconfirm)
                },
                close: function (e) {
                    $("#AddOtherWindows .windowCancel").unbind("click", othercancel)
                    $("#AddOtherWindows .windowConfirm").unbind("click", otherconfirm)
                },
                resizable: false,
                modal: true
            });
            AddOtherWindows = $("#AddOtherWindows").data("kendoWindow");
            window.AddSplitters(AddOtherWindows);
        }

        return AddOtherWindows;
    }

    var cancel = function () {
        $("#Organization_window").data("kendoWindow").close()
    }
    var confirm = function ()
    {
        var validator = $("#Organization_window").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", confirm);
            showOperaMask("Organization_window");
            if ($("#AddInfo .OrganizationName").val() != null) {
                var tt = $("#OrganizationDrop").val();

                //var tt = $("#AddInfo .OrganizationName").attr("data-url");"Maintenance/Organization/AddOrganization"
                if ($("#AddInfo .OrganizationName").attr("data-url") == "Maintenance/Organization/AddOrganization")
                    $.post("Maintenance/Organization/AddOrganization", { Name: $("#AddInfo .OrganizationName").val() }, function (data) {

                        $("#OrganizationDrop").data("kendoDropDownList").dataSource.data(data);
                        $("#Organization_window").data("kendoWindow").close();
                    }).fail(function () {
                        that.bind("click", confirm);
                        hideOperaMask("Organization_window");
                    })
                else if ($("#AddInfo .OrganizationName").attr("data-url") == "Maintenance/Organization/EditOrganization")
                    $.post("Maintenance/Organization/EditOrganization", { Name: $("#AddInfo .OrganizationName").val(), OrgChartId: $("#OrganizationDrop").val() }, function (data) {

                        $("#OrganizationDrop").data("kendoDropDownList").dataSource.data(data);
                        $("#Organization_window").data("kendoWindow").close();
                    }).fail(function () {
                        that.bind("click", confirm);
                        hideOperaMask("Organization_window");
                    })
                return;
            }
            var select = $("#OrganizationManageTreeView_tv_active").find("input");
            if (null == select) return;
            var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());
            var name, type;
            var ExFields;
            switch (item.Type) {
                case "Cluster":
                    name = $("#Organization_window .propertyName").val();
                    type = $("#AddInfopropertyType").data("kendoDropDownList").value();//$("#Organization_window .propertyType").val();
                    ExFields = getOrgExFields("Property").join(',');
                    break;
                case "Company":
                    name = $("#Organization_window .FirstName").val();
                    type = $("#Organization_window .FirstType").val();
                    ExFields = getOrgExFields("Division").join(',');
                    break;
                case "Division":
                    name = $("#Organization_window .SecondName").val();
                    type = $("#Organization_window .SecondType").val();
                    ExFields = getOrgExFields("Cluster").join(',');
                    break;
                case "Property":
                    //不允许添加
                    break;
            }
            $.post("/Maintenance/Organization/AddNodesOrganization", {
                ListID: ListIDMenuTree,
                Type: TypeMenuTree,
                //OrgExField
                //EnglishName_Full: $("#Organization_window .englishName_Full").val(),
                //EnglishAddress_First: $("#Organization_window .englishAddress1").val(),
                //EnglishAddress_Second: $("#Organization_window .englishAddress2").val(),
                //EnglishAddress_Third: $("#Organization_window .englishAddress3").val(),
                //ChineseName_Full: $("#Organization_window .chineseName_Full").val(),
                //ChineseAddress_First: $("#Organization_window .chineseAddress1").val(),
                //ChineseAddress_Second: $("#Organization_window .chineseAddress2").val(),
                //ChineseAddress_Third: $("#Organization_window .chineseAddress3").val(),
                //Code: code,
                //.OrgExField
                ExFields: "[" + ExFields + "]",
                Type: type,
                ID: "0",
                NodeName: name,
                HasChildNode: "",
                ParentID: item.ID
            }, function (item) {
                var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                var select = treeview.select();

                if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                    treeview.append(item, select);
                }
                else {
                    treeview.expand(select);
                }
                $("#Organization_window").data("kendoWindow").close();
            }).fail(function () {
                that.bind("click", confirm);
                hideOperaMask("Organization_window");
            });
        }
    }

    var GetOrganizationWindow = function () {
        var AddOrganizationWindow = $("#Organization_window").data("kendoWindow");
        if (!AddOrganizationWindow) {
            $("#Organization_window").kendoWindow({
                minWidth: "600px",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#Organization_window .windowCancel").bind("click", cancel);
                    $("#Organization_window .windowConfirm").bind("click", confirm);
                },
                close: function (e) {
                    $("#Organization_window .windowCancel").unbind("click", cancel);
                    $("#Organization_window .windowConfirm").unbind("click", confirm);
                    hideOperaMask("Organization_window");
                },
                resizable: false,
                modal: true
            });
            var AddOrganizationWindow = $("#Organization_window").data("kendoWindow");
            window.AddSplitters(AddOrganizationWindow);
        }
        return AddOrganizationWindow;
    }

    var add = function () {
        AddOrganizationWindow = GetOrganizationWindow();
        AddOrganizationWindow.title(jsResxMaintenance_SeaOrganization.AddOrganization).center().open();
        $("#Organization_window").css("overflow", "hidden");
        $("#AddInfo").empty().html($("#OrgChart").html());
        $("#AddInfo .OrganizationName").attr("data-url", "Maintenance/Organization/AddOrganization");
        $("#Organization_window").data("kendoValidator").hideMessages();
    }
    var edit = function () {
        AddOrganizationWindow = GetOrganizationWindow();
        AddOrganizationWindow.title(jsResxMaintenance_SeaOrganization.EditOrganization).center().open();
        $("#Organization_window").css("overflow", "hidden");
        $("#AddInfo").empty().html($("#OrgChart").html());
        $("#AddInfo .OrganizationName").attr("data-url", "Maintenance/Organization/EditOrganization");
        $("#AddInfo .OrganizationName").val($("#OrganizationDrop").data("kendoDropDownList").text());
        $("#Organization_window").data("kendoValidator").hideMessages();
    }

    var initNodePositionList = function () {
        $.get("/Maintenance/Organization/GetPositionByNode", { id: $("#OrganizationManageTreeView_tv_active").find(":checked").val() }, function (items) {

            InitBaseKendoGridWidthPage("NodePositionList", PositionModel, findpositioncolumns, items,5, function () {
                bindGridCheckbox("NodePositionList");
                $("#NodePositionList .k-grid-content").find(":checkbox").click(function () {
                    $("#NodePositionList .k-grid-content").find(":checkbox").prop("checked", false);
                    $(this).prop("checked", true);
                });
                //$("#NodePositionList .k-grid-content").css("height", "190px")
            });
        })
    }
    var initNodeUserList = function () {
        $.get("/Maintenance/Organization/GetUserByNode", { id: $("#OrganizationManageTreeView_tv_active").find(":checked").val() }, function (items) {
            InitBaseKendoGridWidthPage("NodeUserList", StaffModel, employeecolumns, items,5, function () {
                bindGridCheckbox("NodeUserList");
                //$("#NodeUserList .k-grid-content").css("height", "190px")
            });
        })
    }
    var initNodeExFields = function (ExFields) {
        //console.log(ExFields);
        var extendinputs = $("#ExtendedInformation input");
        $.each(extendinputs, function (i) {
            if (this.id.length > 0) {
                var datavalue = getOrgExFieldValue(this.id, ExFields);          
                var type = this.type;
                var data_role = $(this).attr("data-role");
                if (data_role != undefined) {
                    switch(data_role)
                    {
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
    
    var addNodePosition = function () {
        SelectPosition(this);
    }
    //搜索职位
    var SelectPosition = function (obj) {       
        InitSelectPersonWindow(obj, "Position", function (json) {
            var list = json.Root.Positions.Item;
            var items = $("#NodePositionList").data("kendoGrid").dataSource._data;
            $.each(list, function (i, n) {                
                if (!ExistsSelectPosition(n, items)) {
                    $("#NodePositionList").data("kendoGrid").dataSource.add({ PositionID: n.Value, DisplayName: n.Name });
                }
            });
        });
    }

    function ExistsSelectPosition(item, items) {        
        var flag = false;
        $.each(items, function (i, n) {
            if (n.PositionID == item.Value) {
                flag = true;
            }
        });
        return flag;
    }
    //搜索职位
 
    var delNodePosition = function () {        
        var idList = new Array();
        $("#NodePositionList .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            for (var i = 0; i < idList.length; i++) {
                var dataItem = $("#NodePositionList").data("kendoGrid").dataSource.get(idList[i]);
                $("#NodePositionList").data("kendoGrid").dataSource.remove(dataItem);
            }
        }
        else {
            ShowTip(jsResxMaintenance_SeaOrganization.Pleaseselectemployee, "error");
        }
    }
    var addNodeUserList = function () {
        SelectUser(this);
    }

    //搜索用户
    var SelectUser = function (obj) {        
        InitSelectPersonWindow(obj, "Person", function (json) {
            var list = json.Root.Users.Item;
            var data = $("#NodeUserList").data("kendoGrid").dataSource._data;
            $.each(list, function (i, n) {
                if (!ExistsSelectUser(n, data)) {
                    $("#NodeUserList").data("kendoGrid").dataSource.add({ StaffId: n.Value, FirstName: '', LastName: '', DisplayName: n.Name, ChineseName: '', Email: '', MobileNo: '' });
                }
            });
        }, true);
    }

    function ExistsSelectUser(item, data) {        
        var flag = false;
        $.each(data, function (i, n) {
            if (n.StaffId == item.Value) {
                flag = true;
            }
        });
        return flag;
    }
    //搜索职位



    var delNodeUserList = function () {        
        var idList = new Array();
        $("#NodeUserList .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            for (var i = 0; i < idList.length; i++) {
                var dataItem = $("#NodeUserList").data("kendoGrid").dataSource.get(idList[i]);
                $("#NodeUserList").data("kendoGrid").dataSource.remove(dataItem);
            }
        }
        else {
            ShowTip(jsResxMaintenance_SeaOrganization.Pleaseselectemployee, "error");
        }
    }

    //弃用搜索用户
    var searchManager = function () {
        InitOrganizationWindow().title(jsResxMaintenance_SeaOrganization.SearchManager).center().open();
        $("#AddOtherWindows").css("overflow", "hidden");

        $("#AddOtherWindows .windowConfirm").attr("data-type", "Manager");
        $("#AddOtherWindows .ListGridPosition").hide();
        $("#AddOtherWindows .ListGridManager").show();
        InitBaseServerKendoGridWidthPage("AddOtherWindows .ListGridManager", StaffModel,
               findstaffcolumns, "/Maintenance/Staff/GetStaffs", {}, 10, function () {
                   bindGridCheckbox("AddOtherWindows .ListGridManager");
                   $("#AddOtherWindows .ListGridManager .k-grid-content").css("height", "190px")
               });
    }

    var othersearch = function () {
        var input = $("#AddOtherWindows .OtherInput").val();

        if ($("#AddOtherWindows .windowConfirm").attr("data-type") != "Position") {
            InitBaseServerKendoGridWidthPage("AddOtherWindows .ListGridManager", StaffModel,
                   findstaffcolumns, "/Maintenance/Staff/FindNameStaffs", { input: input }, 10, function () {
                       bindGridCheckbox("AddOtherWindows .ListGridManager");
                       $("#AddOtherWindows .ListGridManager .k-grid-content").css("height", "190px")
                   });
        }
        else {
            $.post("/Maintenance/Position/FindPosition", { input: input }, function (items) {

                InitBaseKendoGrid("AddOtherWindows .ListGridPosition", PositionModel, findpositioncolumns, items, function () {
                    bindGridCheckbox("AddOtherWindows .ListGridPosition");
                    $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").click(function () {
                        $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").prop("checked", false);
                        $(this).prop("checked", true);
                    });
                    $("#AddOtherWindows .ListGridPosition .k-grid-content").css("height", "190px")
                });
            })
        }
    }

    //弃用搜索职位
    var searchposition = function () {
        InitOrganizationWindow().title(jsResxMaintenance_SeaOrganization.SearchPosition).center().open();
        $("#AddOtherWindows").css("overflow", "hidden");

        $("#AddOtherWindows .windowConfirm").attr("data-type", "Position");
        $("#AddOtherWindows .ListGridPosition").show();
        $("#AddOtherWindows .ListGridManager").hide();
        $.get("/Maintenance/Position/GetPositionList", function (items) {
            $("#AddOtherWindows .OtherInput").val("");
            InitBaseKendoGrid("AddOtherWindows .ListGridPosition", PositionModel, findpositioncolumns, items, function () {
                bindGridCheckbox("AddOtherWindows .ListGridPosition");
                $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").click(function () {
                    $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checkbox").prop("checked", false);
                    $(this).prop("checked", true);
                });
                $("#AddOtherWindows .ListGridPosition .k-grid-content").css("height", "190px")
            });
        })
    }

    var othercancel = function () {
        $("#AddOtherWindows").data("kendoWindow").close()
    }

    var otherconfirm = function () {

        var that = $(this);
        that.unbind("click", otherconfirm);
        if ($("#AddOtherWindows .windowConfirm").attr("data-type") == "Position") {
            var idList = new Array();
            $("#AddOtherWindows .ListGridPosition .k-grid-content").find(":checked").each(function () {
                idList.push(this.value)
            })

            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    var item = $("#AddOtherWindows .ListGridPosition").data("kendoGrid").dataSource.get(idList[i]);
                    if (!$("#NodePositionList").data("kendoGrid").dataSource.get(idList[i]))
                        $("#NodePositionList").data("kendoGrid").dataSource.add(item);
                }
            }
        }
        else if ($("#AddOtherWindows .windowConfirm").attr("data-type") == "Manager") {
            var idList = new Array();
            $("#AddOtherWindows .ListGridManager .k-grid-content").find(":checked").each(function () {
                idList.push(this.value)
            })

            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    var item = $("#AddOtherWindows .ListGridManager").data("kendoGrid").dataSource.get(idList[i]);
                    if (!$("#NodeUserList").data("kendoGrid").dataSource.get(idList[i]))
                        $("#NodeUserList").data("kendoGrid").dataSource.add(item);
                }
            }
        }
        $("#AddOtherWindows").data("kendoWindow").close();
    }

    var addNode = function () {
        var select = $("#OrganizationManageTreeView_tv_active").find("input:checked");
        if (0 == select.length) return;
        var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());

        var name, type, code;
        var AddOrganizationWindow = GetOrganizationWindow();

        switch (item.Type) {
            case "Cluster":                                
                $("#AddInfo").empty().html($("#addproperty").html());
                InitPropertyType("AddInfo");                
                $("#Organization_window").show();
                AddOrganizationWindow.title(jsResxMaintenance_SeaOrganization.AddProperty).center().open();
                //$("#Organization_window").css("overflow", "hidden");
                //$("#Organization_window .propertyType").val("Property");
                break;
            case "Company":
                $("#AddInfo").empty().html($("#FirstBasicInfo").html());
                $("#Organization_window").show();
                AddOrganizationWindow.title(jsResxMaintenance_SeaOrganization.AddDivision).center().open();
                $("#Organization_window").css("overflow", "hidden");
                $("#Organization_window .FirstType").val("Division");
                break;
            case "Division":
                $("#AddInfo").empty().html($("#SecondBasicInfo").html());
                $("#Organization_window").show();
                AddOrganizationWindow.title(jsResxMaintenance_SeaOrganization.AddCluster).center().open();
                $("#Organization_window").css("overflow", "hidden");
                $("#Organization_window .SecondType").val("Cluster");
                break;
            case "Property":
                //不允许添加
                break;
        }
    }

    var delNode = function () {

        var select = $("#OrganizationManageTreeView_tv_active").find("input:checked");
        if (0 == select.length) return;
        bootbox.confirm(jsResxMaintenance_SeaOrganization.Areyousure, function (result) {
            if (result) {
                $.post("/Maintenance/Organization/DeleteNodesOrganization", {
                    ListID: ListIDMenuTree,
                    Type: TypeMenuTree,
                    id: select.val()
                }, function (id) {
                    var chartitem = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get(id);
                    if (chartitem != null) {
                        $("#OrganizationDrop").data("kendoDropDownList").dataSource.remove(chartitem);
                        var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                        treeview.dataSource.remove(treeview.dataSource.get(select.val()));
                    }
                    else {
                        var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                        treeview.dataSource.remove(treeview.dataSource.get(id));
                    }
                    $("#Information").hide();
                });
            }
        });

    }

    var saveNode = function () {
        var validator = $("#Information").data("kendoValidator");
        if (validator.validate()) {
            showOperaMask("Information");
            var select = $("#OrganizationManageTreeView_tv_active").find("input");
            if (null == select) { hideOperaMask("Information"); return; };
            var item = $("#OrganizationManageTreeView").data("kendoTreeView").dataSource.get(select.val());

            var name, type;
            switch (item.Type) {
                case "Company":
                    name = $("#BasicInformation .OrganizationName").val();
                    type = $("#BasicInformation .OrganizationType").val();
                    break;
                case "Division":
                    name = $("#BasicInformation .FirstName").val();
                    type = $("#BasicInformation .FirstType").val();
                    break;
                case "Cluster":
                    name = $("#BasicInformation .SecondName").val();
                    type = $("#BasicInformation .SecondType").val();
                    break;                
                case "Property":
                    name = $("#BasicInformation .propertyName").val();
                    type = $("#BasicInformation .propertyType").val();
                    break;
            }
            var ExFields = getOrgExFields_save(item.Type).join(',');
            var positionIdList = new Array();
            var managerIdList = new Array();

            var managedata = $("#NodeUserList").data("kendoGrid").dataSource._data;
            var positiondata = $("#NodePositionList").data("kendoGrid").dataSource._data;
            $.each(managedata,function (i,d) {
                managerIdList.push(d.StaffId);
            })
            $.each(positiondata,function(i,d) {
                positionIdList.push(d.PositionID);
            })
            $.ajax({
                url: "/Maintenance/Organization/SaveOrganization",
                type: "POST",
                data: {
                    PositionIdList: positionIdList,
                    ManagerIdList: managerIdList,
                    //ListID: ListIDMenuTree,
                    //TypeMenuTree: TypeMenuTree,
                    //OrgExField
                    //EnglishName_Full: $("#BasicInformation .englishName_Full").val(),
                    //EnglishAddress_First: $("#BasicInformation .englishAddress1").val(),
                    //EnglishAddress_Second: $("#BasicInformation .englishAddress2").val(),
                    //EnglishAddress_Third: $("#BasicInformation .englishAddress3").val(),
                    //ChineseName_Full: $("#BasicInformation .chineseName_Full").val(),
                    //ChineseAddress_First: $("#BasicInformation .chineseAddress1").val(),
                    //ChineseAddress_Second: $("#BasicInformation .chineseAddress2").val(),
                    //ChineseAddress_Third: $("#BasicInformation .chineseAddress3").val(),
                    //Code: code,
                    //.OrgExField
                    ExFields: "[" + ExFields + "]",
                    NodeName: name,
                    HasChildNode: item.HasChildNode,
                    ParentID: item.ParentID,
                    ID: select.val(),
                    Type: item.Type
                },
                traditional: true,
                success: function (item) {
                    var treeview = $("#OrganizationManageTreeView").data("kendoTreeView");
                    var model = treeview.dataSource.get(item.ID);
                    if (model) {
                        //model.set(" EnglishName_Full     ", item.EnglishName_Full);
                        //model.set(" EnglishAddress_First ", item.EnglishAddress_First);
                        //model.set(" EnglishAddress_Second", item.EnglishAddress_Second);
                        //model.set(" EnglishAddress_Third ", item.EnglishAddress_Third);
                        //model.set(" ChineseName_Full     ", item.ChineseName_Full);
                        //model.set(" ChineseAddress_First ", item.ChineseAddress_First);
                        //model.set(" ChineseAddress_Second", item.ChineseAddress_Second);
                        //model.set(" ChineseAddress_Third ", item.ChineseAddress_Third);
                        //model.set(" Code ", item.Code);
                        //model.set("ExFields ", item.ExFields);
                        //model.set("Manager", $("#Information .ManagerName").val());
                        //model.set("Position ", $("#Information .Position").val());
                        //model.set("ManagerID", $("#Information .ManagerName").attr("data-value"));
                        //model.set("PositionID ", $("#Information .Position").attr("data-value"));
                        //model.set("NodeName ", item.NodeName);
                        for (var key in item) {
                            model.set(key, item[key]);
                        }
                    }
                    var template = kendo.template($("#OrganizationManageTreeView-template").html())
                    var target = $("#OrganizationManageTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    $("#OrganizationManageTreeView_tv_active").find("input").first().prop("checked", true);
                    hideOperaMask("Information");
                    $("#Organization_Save").siblings(".tips").css("visibility", "visible");
                },
                dataType: "json"
            }).fail(function () { hideOperaMask("Information"); })
        }
    }

    var InitPropertyType=function(id)
    {        
        var kdd = $("#" + id + " .propertyType");
        kdd.attr("id", id + "propertyType");
        if (!kdd.data("kendoDropDownList")) {
            kdd.kendoDropDownList({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: [
                    { text: "Cluster", value: "Cluster" },
                    { text: "Property", value: "Property" },
                ],
                index: 0                
            });
        }
    }


    var LoadOrganizationView = function () {
        title = "Organization Management - Kendo UI";
        InitOrganizationSplitter();
        $.getJSON("/Maintenance/Organization/GetOrganizationsDrop", { _t: new Date() }, function (items) {
            $("#OrganizationDrop").kendoDropDownList({
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
                close: function () {
                    ListIDMenuTree = $("#OrganizationDrop").val();
                    TypeMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get($("#OrganizationDrop").val()).DisplayName;

                    InitTreeView();
                }
            });

            ListIDMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.data()[0].PositionID; //默认的listID
            TypeMenuTree = $("#OrganizationDrop").data("kendoDropDownList").dataSource.get(ListIDMenuTree).DisplayName;
            $("#OrganizationDrop").val(ListIDMenuTree);

            InitTreeView();

            TreeViewNodeToggle("OrganizationManageTreeView");
        })

        $("#OrganizationManaView .Add").click(add);
        $("#OrganizationManaView .Edit").click(edit);

        $("#Information  .searchManagerName").click(searchManager)
        $("#OtherSearch").click(othersearch)
        $("#Information  .searchPosition").click(searchposition)
        $("#AddOtherWindows .windowCancel").click(othercancel)
        $("#AddOtherWindows .windowConfirm").click(otherconfirm)
        $(".addNodePosition").click(addNodePosition)
        $(".delNodePosition").click(delNodePosition)
        $(".addNodeUserList").click(addNodeUserList)
        $(".delNodeUserList").click(delNodeUserList)
        $("#Organization_Save").click(saveNode);
        $("#ChartsExport").click(ExportCharts);       

        //$.contextMenu({
        //    selector: '#OrganizationManageTreeView_tv_active .Type',
        //    callback: function (key, options) {

        //        switch (key) {
        //            //case "edit": break;
        //            case "add": addNode(); break;
        //            case "delete": delNode(); break;
        //        }
        //    },
        //    items: {
        //        "add": { name: "Add", icon: "add" },
        //        "delete": { name: "Delete", icon: "delete" }
        //    }
        //});
        //$.contextMenu({
        //    selector: '#OrganizationManageTreeView_tv_active .PropertyType',
        //    callback: function (key, options) {

        //        switch (key) {
        //            //case "edit": break;
        //            //case "add": addNode(); break;
        //            case "delete": delNode(); break;
        //        }
        //    },
        //    items: {
        //        "delete": { name: "Delete", icon: "delete" }
        //    }
        //});
    }

    module.exports = LoadOrganizationView;
})