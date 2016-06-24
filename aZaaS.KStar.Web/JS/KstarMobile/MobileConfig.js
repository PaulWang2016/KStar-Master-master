define(function (require, exports, module) {
    function positions() {
        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/KstarMobile/MobileConfig/GetProcess?timestamp="+Date.parse(new Date()));
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
    //删除item
    function DelItemContextMenu() {
        bootbox.confirm(jsResxKstarMobile_MobileConfig["DeleteProcessConfirm"], function (result) {
            if (result) {
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                var that = $(this);
                $.post("/KstarMobile/MobileConfig/DelProcessDefinition", { ID: Math.abs(node.val()) }, function (data) {
                    //删除流程            
                    if (data.flag) {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Success"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Success"], "info");
                        //如果删除Header下的节点，同步删除Data对应节点
                        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
                        var parent = $("#PostionManageTreeView").data("kendoTreeView").parent(node);
                        if ($(parent).find("input").first().attr("data-DisplayName") == "Header") {
                            treeview.select(parent);
                            var childs = $("#PostionManageTreeView_tv_active").siblings().first().find(".k-group").children();
                            $.each(childs, function (n) {
                                if ($(childs[n]).find("input").first().attr("data-DisplayName") == "Data") {
                                    treeview.select(childs[n]);
                                    if ($("#PostionManageTreeView_tv_active").attr("isremote") == "1") {
                                        var items = $("#PostionManageTreeView_tv_active").find(".k-group").children();
                                        $.each(items, function (i) {
                                            if ($(items[i]).find("input").first().attr("data-ChildID") == node.attr("data-ChildID")) {
                                                treeview.select(items[i]);
                                                RemoveSelectNode();
                                            }
                                        });
                                    }
                                }
                            });
                            treeview.select(node);
                        }
                        RemoveSelectNode();
                        $("#dataSection").hide();
                        $("#groupSection").hide();
                        $("#itemSection").hide();
                    }
                    else {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    }
                }).fail(function () {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["DeleteNode"] + node.attr("data-DisplayName") + jsResxKstarMobile_MobileConfig["Failure"], "info");
                });
            }
        });
    }
    //添加group
    function AddGroupContextMenu()
    {        
        var AddProcessGroupWindow = $("#AddProcessGroupWindow").data("kendoWindow");
        if (!AddProcessGroupWindow) {
            $("#AddProcessGroupWindow").kendoWindow({
                width: "750px",
                title: jsResxKstarMobile_MobileConfig["AddGroup"],
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddProcessGroupWindow .windowCancel").bind("click", ProcessGroupCancel);
                    $("#AddProcessGroupWindow .windowConfirm").bind("click", ProcessGroupConfirm);
                },
                close: function (e) {
                    $("#AddProcessGroupWindow .windowCancel").unbind("click", ProcessGroupCancel);
                    $("#AddProcessGroupWindow .windowConfirm").unbind("click", ProcessGroupConfirm);
                    hideOperaMask("AddProcessGroupWindow");
                },
                resizable: false,
                modal: true
            });
            AddProcessGroupWindow = $("#AddProcessGroupWindow").data("kendoWindow").center();
           $(window.splitters).push(AddProcessGroupWindow);
        }

        $("#AddGroupName").val("");
        $("#AddGroupLabelName").val("");
        $("#AddGroupConnectionString").val("");
        $("#AddGroupMapping").val("");
        $("#AddGroupWhereString").val("");
        $("#AddGroupCollapsed").prop("checked", false);
        $("#AddGroupType").data("kendoDropDownList").select(0);
        $("#AddProcessGroupWindow").data("kendoValidator").hideMessages();
        AddProcessGroupWindow.open();
    }
    var ProcessGroupCancel = function () {
        $("#AddProcessGroupWindow").data("kendoWindow").close()
    }
    var ProcessGroupConfirm = function () {
        var that = $(this);
        that.unbind("click", ProcessGroupConfirm);        
        var validator = $("#AddProcessGroupWindow").data("kendoValidator");
        if (validator.validate()) {
            showOperaMask("AddProcessGroupWindow");
            var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
            var GroupName = $("#AddGroupName").val();
            var GroupLabelName = $("#AddGroupLabelName").val();

            var GroupConnectionString = $("#AddGroupConnectionString").val();
            var GroupMapping = $("#AddGroupMapping").val();
            var GroupWhereString = $("#AddGroupWhereString").val();

            var GroupCollapsed = $("#AddGroupCollapsed").prop("checked");
            var GroupType =$("#AddGroupType").val();
            var data = { ID: Math.abs(node.val()), ConnectionString: GroupConnectionString, Mapping: GroupMapping, WhereString: GroupWhereString, Name: GroupName, LabelName: GroupLabelName, Collapsed: GroupCollapsed, Type: GroupType };
            $.post("/KstarMobile/MobileConfig/AddProcessGroup", data, function (data) {
                if (data.flag) {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Success"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Success"], "info");
                    var node = {ID:"",DisplayName:"",ChildType:"",ChildID:"",ParentID:"",OrderNo:"",ConnectionString:""};
                    AppendNode(data.data);
                    //TreeViewReload();
                    //$("#dataSection").hide();
                    //$("#groupSection").hide();
                    //$("#itemSection").hide();
                    //关闭弹出框
                    $("#AddProcessGroupWindow").data("kendoWindow").close();
                }
                else {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Failure"], "info");
                }
            }).fail(function () {
                that.bind("click", ProcessGroupConfirm);
                hideOperaMask("AddProcessGroupWindow");
                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Failure"]);
                $(".top-title").siblings(".tips").css("visibility", "visible");
                //popupNotification.show(jsResxKstarMobile_MobileConfig["AddGroup"] + GroupName + jsResxKstarMobile_MobileConfig["Failure"], "info");
            })
        }
        else {
            that.bind("click", ProcessGroupConfirm);
        }
    }

    //添加item
    function AddItemContextMenu() {
        var AddProcessItemWindow = $("#AddProcessItemWindow").data("kendoWindow");
        if (!AddProcessItemWindow) {
            $("#AddProcessItemWindow").kendoWindow({
                width: "500px",
                title: jsResxKstarMobile_MobileConfig["AddItem"],
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddProcessItemWindow .windowCancel").bind("click", ProcessItemCancel);
                    $("#AddProcessItemWindow .windowConfirm").bind("click", ProcessItemConfirm);
                },
                close: function (e) {
                    $("#AddProcessItemWindow .windowCancel").unbind("click", ProcessItemCancel);
                    $("#AddProcessItemWindow .windowConfirm").unbind("click", ProcessItemConfirm);
                    hideOperaMask("AddProcessItemWindow");
                },
                resizable: false,
                modal: true
            });
            AddProcessItemWindow = $("#AddProcessItemWindow").data("kendoWindow").center();
            $(window.splitters).push(AddProcessItemWindow);
        }

        $("#AddItemName").val("");
        $("#AddItemLabelName").val("");   
        $("#AddItemMapping").val(""); 
        $("#AddItemFormat").val("");
        $("#AddItemVisible").prop("checked", false);
        $("#AddItemEditable").prop("checked", false);
        $("#AddProcessItemWindow").data("kendoValidator").hideMessages();
        AddProcessItemWindow.open();
    }
    var ProcessItemCancel = function () {
        $("#AddProcessItemWindow").data("kendoWindow").close()
    }
    var ProcessItemConfirm = function () {
        var that = $(this);
        that.unbind("click", ProcessItemConfirm);        
        var validator = $("#AddProcessItemWindow").data("kendoValidator");
        if (validator.validate()) {
            showOperaMask("AddProcessItemWindow");
            var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
            var ItemName = $("#AddItemName").val();
            var ItemLabelName = $("#AddItemLabelName").val();
            var ItemConnectionString = $("#AddItemConnectionString").val();
            var ItemMapping = $("#AddItemMapping").val();
            var ItemWhereString = $("#AddItemWhereString").val();
            var ItemFormat = $("#AddItemFormat").val();
            var ItemVisible = $("#AddItemVisible").prop("checked");
            var ItemEditable = $("#AddItemEditable").prop("checked");

            var data = { ID: Math.abs(node.val()), Mapping: ItemMapping, Name: ItemName, LabelName: ItemLabelName, Visible: ItemVisible, Editable: ItemEditable, Format: ItemFormat };
            $.post("/KstarMobile/MobileConfig/AddProcessItem", data, function (data) {
                if (data.flag) {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Success"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Success"], "info");
                    //如果添加Header下的节点，则同步添加节点到Data下                    
                    if (node.attr("data-DisplayName") == "Header") {                    
                        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
                        var childs = $("#PostionManageTreeView_tv_active").siblings().first().find(".k-group").children();                                                                      
                        $.each(childs, function (n) {
                            if ($(childs[n]).find("input").first().attr("data-DisplayName") == "Data") {
                                treeview.select(childs[n]);
                                if ($("#PostionManageTreeView_tv_active").attr("isremote") == "1") {
                                    AppendNode(data.extend);
                                }
                                treeview.select(node);
                            }
                        });                                                
                    }                    
                    AppendNode(data.data);
                    //TreeViewReload();
                    //$("#dataSection").hide();
                    //$("#groupSection").hide();
                    //$("#itemSection").hide();                    
                    //关闭弹出框
                    $("#AddProcessItemWindow").data("kendoWindow").close();
                }
                else {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Failure"], "info");
                }
            }).fail(function () {
                that.bind("click", ProcessItemConfirm);
                hideOperaMask("AddProcessItemWindow");
                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Failure"]);
                $(".top-title").siblings(".tips").css("visibility", "visible");
                //popupNotification.show(jsResxKstarMobile_MobileConfig["AddItem"] + ItemName + jsResxKstarMobile_MobileConfig["Failure"], "info");
            })
        }
        else {
            that.bind("click", ProcessItemConfirm);
        }
    }
     
    var PostionManageTreeView;
    var res, parId;
    var sourceparentnode;
    var InitPostionManageTreeView = function () {
        $("#PostionManageTreeView").kendoTreeView({            
            template: kendo.template($("#PostionManageTreeView-template").html()),
            dataSource: positions(),
            dataTextField: "DisplayName",
            select: function (e) {

                res = $("#PostionManageTreeView").data("kendoTreeView").dataSource.get($("#PostionManageTreeView_tv_active").find("input").first().val()).DisplayName;
                parId = $("#PostionManageTreeView_tv_active").find("input").first().val();
                
                $("#PostionManageTreeView").find("input").prop("checked", false);
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                var Type = node.attr("data-Type");
                var ChildID = node.attr("data-ChildID");
                $("#itemId").val(node.val());
                $("#itemName").val(node.attr("data-DisplayName"));
                $("#itemParentID").val(node.attr("data-parentid"))

                $("#GroupTips").html("").hide();
                $(".top-title").siblings(".tips").text("");
                $(".top-title").siblings(".tips").css("visibility", "hidden");
                if (node.attr("data-parentid") == "0" )
                {
                    $('#PostionManageTreeView .k-state-focused').WinContextMenu({
                        //cancel: '.cancel',
                        menu: "#PositionProcessContextMenu",
                        removeMenu: '#homeBody',
                        action: function (e) {
                            switch (e.id) {
                                case "DelProcessContextMenu": DelItemContextMenu(); break;                                
                            }
                        }
                    });
                    
                }
                else if ("Task,TaskInfo,BaseInfo,ExtendInfo,ProcBaseInfo,BizInfo,ProcLogInfo".indexOf(node.attr("data-DisplayName")) < 0 && Type.toLowerCase() != "group") {
                    var parent = $("#PostionManageTreeView").data("kendoTreeView").parent(node).find("input").first();
                    if (parent.attr("data-DisplayName") != "Data") {
                        $('#PostionManageTreeView .k-state-focused').WinContextMenu({
                            //cancel: '.cancel',
                            menu: "#PositionItemContextMenu",
                            removeMenu: '#homeBody',
                            action: function (e) {
                                switch (e.id) {
                                    case "DelItemContextMenu": DelItemContextMenu(); break;
                                }
                            }
                        });
                    }
                    else {
                        $(".WincontextMenu").hide();
                    }
                }
                else if (Type.toLowerCase() == "group") {
                    var flag = true;
                    if ("Task,TaskInfo,ProcLogInfo,Data".indexOf(node.attr("data-DisplayName")) > -1) {
                        $("#AddItemContextMenu").hide();
                        $("#DelGroupContextMenu").hide();                        
                        $("#AddGroupContextMenu").hide();
                        $("#AddGroup").hide();
                        $("#AddItem").hide();
                        flag = false;
                        $(".WincontextMenu").hide();
                    }
                    else if ("BaseInfo,ExtendInfo,ProcBaseInfo,BizInfo".indexOf(node.attr("data-DisplayName")) > -1) {
                        $("#DelGroupContextMenu").hide();                        
                        $("#AddGroupContextMenu").hide();
                        $("#AddGroup").hide();                        
                        $("#AddItemContextMenu").show();
                        $("#AddItem").show();
                        if (node.attr("data-DisplayName") == "BizInfo") {                            
                            $("#AddGroupContextMenu").show();
                            $("#AddGroup").show();
                            $("#AddItemContextMenu").hide();
                            $("#AddItem").hide();
                        }                        
                    }
                    else if ("Header,Row,Data,More".indexOf(node.attr("data-DisplayName")) > -1) {
                        if (node.attr("data-DisplayName") == "Row") {
                            $("#AddItemContextMenu").hide();
                            $("#AddItem").hide();
                        }
                        else {
                            $("#AddItemContextMenu").show();
                            $("#AddItem").show();
                        }                        
                        $("#AddGroupContextMenu").hide();
                        $("#AddGroup").hide();
                        $("#DelGroupContextMenu").hide();                                                                       
                    }
                    else {                        
                        $("#AddGroupContextMenu").hide();
                        $("#AddGroup").hide();
                        $("#DelGroupContextMenu").show();
                        $("#AddItemContextMenu").show();
                        $("#AddItem").show();
                    }

                    //显示提示信息
                    if ("BaseInfo,ProBaseInfo".indexOf(node.attr("data-DisplayName")) > -1) {
                        $("#GroupTips").html("(" + jsResxKstarMobile_MobileConfig["GroupTips"] + ")").show();
                    }

                    if (flag) {
                        $('#PostionManageTreeView .k-state-focused').WinContextMenu({
                            //cancel: '.cancel',
                            menu: "#PositionGroupContextMenu",
                            removeMenu: '#homeBody',
                            action: function (e) {
                                switch (e.id) {
                                    case "AddGroupContextMenu": AddGroupContextMenu(); break;
                                    case "AddItemContextMenu": AddItemContextMenu(); break;
                                    case "DelGroupContextMenu": DelItemContextMenu(); break;
                                }
                            }
                        });
                    }
                }                

                if (Type.toLowerCase() == "item") {                    
                    $("#dataSection").hide();
                    $("#groupSection").hide();                    
                    $.post("/KstarMobile/MobileConfig/GetProcessItem", { ID: Math.abs(node.val()), ChildID: ChildID }, function (data) {
                        if (data.flag) {
                            $("#ItemName").val(data.data.Name);
                            $("#ItemLabelName").val(data.data.LabelName);
                            $("#ItemConnectionString").val(data.extenddata.ConnectionString);
                            $("#ItemMapping").val(data.extenddata.Mapping);
                            $("#ItemWhereString").val(data.extenddata.WhereString);
                            $("#ItemFormat").val(data.data.Format);
                            $("#ItemVisible").prop("checked", data.data.Visible);
                            $("#ItemEditable").prop("checked", data.data.Editable);                            

                            $("#ItemChildID").val(data.data.ID);

                            $("#itemform").data("kendoValidator").hideMessages();
                            $("#itemSection").show();

                            var parent = $("#PostionManageTreeView").data("kendoTreeView").parent(node).find("input").first();
                            if (parent.attr("data-DisplayName") == "Data") {
                                $("#ItemName").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemLabelName").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemConnectionString").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemMapping").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemWhereString").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemFormat").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#ItemVisible").attr("disabled", true);
                                $("#ItemEditable").attr("disabled", true);
                            }
                            else {
                                if ((parent.attr("data-DisplayName") == "BaseInfo" && "SN,Destination".indexOf(node.attr("data-DisplayName")) > -1) || (parent.attr("data-DisplayName") == "ExtendInfo" && "StaffIcon,DisplayName".indexOf(node.attr("data-DisplayName")) > -1) || (parent.attr("data-DisplayName") == "ProcBaseInfo" && "SN,Folio,ProcessName".indexOf(node.attr("data-DisplayName")) > -1)) {
                                    $("#ItemName").attr("disabled", true).css("background-color", "#e3e3e3");
                                }
                                else {
                                    $("#ItemName").attr("disabled", false).css("background-color", "#ffffff");
                                }                                
                                $("#ItemLabelName").attr("disabled", false).css("background-color", "#ffffff");
                                $("#ItemConnectionString").attr("disabled", false).css("background-color", "#ffffff");
                                $("#ItemMapping").attr("disabled", false).css("background-color", "#ffffff");
                                $("#ItemWhereString").attr("disabled", false).css("background-color", "#ffffff");
                                $("#ItemFormat").attr("disabled", false).css("background-color", "#ffffff");
                                $("#ItemVisible").attr("disabled", false);
                                $("#ItemEditable").attr("disabled", false);
                            }

                        }
                        else {
                            $("#itemSection").hide();
                            //popupNotification.show("获取item内容失败", "info");
                        }
                    }).fail(function () {                        
                        //popupNotification.show("获取item内容失败", "info");
                    });
                }
                else {

                    if (node.attr("data-parentid") == "0")
                    {
                        $("#itemSection").hide();
                        $("#dataSection").show();
                        $("#groupSection").hide();
                        $("#ProcessName").val(node.attr("data-DisplayName"));
                        $("#ProcessID").val(Math.abs(node.val()));                        
                        $("#dataform").data("kendoValidator").hideMessages();

                        GetProcessPermissionList($("#ProcessName").val());
                        return;
                    }
                    $("#itemSection").hide();
                    $("#dataSection").hide();                    
                    $.post("/KstarMobile/MobileConfig/GetProcessGroup", { ID: Math.abs(node.val()), ChildID: ChildID }, function (data) {
                        if (data.flag) {
                            $("#GroupName").val(data.data.Name);
                            $("#GroupLabelName").val(data.data.LabelName);
                            $("#GroupConnectionString").val(data.extenddata.ConnectionString);
                            $("#GroupMapping").val(data.extenddata.Mapping);
                            $("#GroupWhereString").val(data.extenddata.WhereString);
                            $("#GroupCollapsed").prop("checked", data.data.Collapsed);                            
                            $("#GroupType").data("kendoDropDownList").value(data.data.Type);
                            $("#GroupChildID").val(data.data.ID);

                            $("#groupform").data("kendoValidator").hideMessages();
                            $("#groupSection").show();


                            if ("Task,TaskInfo,BaseInfo,ExtendInfo,ProcBaseInfo,BizInfo,ProcLogInfo,Header,Row,Data,More".indexOf(node.attr("data-DisplayName")) > -1) {
                                $("#GroupName").attr("disabled", true).css("background-color", "#e3e3e3");
                            }
                            else {
                                $("#GroupName").attr("disabled", false).css("background-color", "#ffffff");
                            }

                            if ("Task,TaskInfo,BaseInfo,ProcBaseInfo".indexOf(node.attr("data-DisplayName")) > -1) {                                
                                if (data.data.Name.toLowerCase() != "procbaseinfo") {
                                    $("#GroupLabelName").attr("disabled", true).css("background-color", "#e3e3e3");
                                }
                                else {
                                    $("#GroupLabelName").attr("disabled", false).css("background-color", "#ffffff");
                                }                                
                                $("#GroupConnectionString").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#GroupMapping").attr("disabled", true).css("background-color", "#e3e3e3");
                                $("#GroupWhereString").attr("disabled", true).css("background-color", "#e3e3e3");
                            }
                            else {                                                           
                                $("#GroupLabelName").attr("disabled", false).css("background-color", "#ffffff");
                                $("#GroupConnectionString").attr("disabled", false).css("background-color", "#ffffff");
                                $("#GroupMapping").attr("disabled", false).css("background-color", "#ffffff");
                                $("#GroupWhereString").attr("disabled", false).css("background-color", "#ffffff");                                
                            }                            
                        }
                        else {
                            $("#groupSection").hide();
                            //popupNotification.show("获取group内容失败", "info");
                        }
                    }).fail(function () {
                        //popupNotification.show("获取group内容失败", "info");
                    });
                }
            },
            collapse: function (e) {
                $("#PostionManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#PostionManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
                var node = e.node;
                if ($(node).attr("isRemote") == undefined)
                {
                    $(node).attr("isRemote", "1");
                }
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#PostionManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#PostionManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
                PostionManageTreeView.expand(".k-first");
            }
            ,
            dragAndDrop: true,
            drag: function (e) {
                //group节点只能进行移动到目标节点的前后位置操作，且必须与目标节点在同一个父节点之下
                //item节点可以移动到除了本身父节点和固有的流程节点之外的任何父节点下  
                //流程固有节点
                var processnode = "Task,TaskInfo,ProcLogInfo,Header,Row,Data";
                if (((e.statusClass.indexOf("add") >= 0 || parseInt($(e.dropTarget).find("input").attr("data-parentid")) != parseInt($(e.sourceNode).find("input").attr("data-parentid"))) && $(e.sourceNode).find("input").attr("data-type").toLowerCase() != "item") || (e.statusClass.indexOf("add") >= 0 && $(e.dropTarget).find("input").attr("data-type").toLowerCase() == "item") || (e.statusClass.indexOf("add") >= 0 && parseInt($(e.dropTarget).find("input").val()) == parseInt($(e.sourceNode).find("input").attr("data-parentid"))) || (e.statusClass.indexOf("add") >= 0 && $(e.sourceNode).find("input").attr("data-type").toLowerCase() == "item" && (processnode.indexOf($(e.dropTarget).find("input").attr("data-DisplayName")) > -1 || parseInt($(e.dropTarget).find("input").attr("data-parentid")) == 0))) {
                    e.setStatusClass("k-denied");
                }                
            },
            dragend: function (e) {                    
                /*            
                console.log("Source:"+$(e.sourceNode).find("input").attr("data-DisplayName"));
                console.log("destination:" + $(e.destinationNode).find("input").attr("data-DisplayName"));
                console.log("position:" + e.dropPosition);
                */
                if (e.dropPosition == "over") {                    
                    $.post("/KstarMobile/MobileConfig/CopyProcessItem", { itemid: parseInt($(e.sourceNode).find("input").val()), groupid: parseInt($(e.destinationNode).find("input").val()) }, function (data) {
                        if (data.flag) {
                            if (parseInt(data.extend) == 1) {
                                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["ExistsItemTip"]);
                                $(".top-title").siblings(".tips").css("visibility", "visible");
                                //popupNotification.show(jsResxKstarMobile_MobileConfig["ExistsItemTip"], "info");
                            }
                            else {
                                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Success"]);
                                $(".top-title").siblings(".tips").css("visibility", "visible");
                                //popupNotification.show(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Success"], "info");
                                var treeview = $("#PostionManageTreeView").data("kendoTreeView");
                                if (sourceparentnode != undefined)
                                {
                                    //还原移动节点
                                    treeview.select(sourceparentnode);
                                    AppendNode(e.sourceNode);

                                    //添加复制节点
                                    treeview.select(e.destinationNode);
                                    AppendNode(data.data);
                                }                                
                            }
                        }
                        else {
                            $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Failure"]);
                            $(".top-title").siblings(".tips").css("visibility", "visible");
                            //popupNotification.show(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Failure"], "info");
                        }
                    }).fail(function () {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["CopyNode"] + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    });
                }
                else {
                    $.post("/KstarMobile/MobileConfig/UpdateProcessOrderNo", { sourceid: parseInt($(e.sourceNode).find("input").val()), destinationid: parseInt($(e.destinationNode).find("input").val()), position: e.dropPosition }, function (data) {
                        if (data.flag) {
                            $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Success"]);
                            $(".top-title").siblings(".tips").css("visibility", "visible");
                            //popupNotification.show(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Success"], "info");
                        }
                        else {
                            $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Failure"]);
                            $(".top-title").siblings(".tips").css("visibility", "visible");
                            //popupNotification.show(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Failure"], "info");
                        }
                    }).fail(function () {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["MoveNode"] + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    });
                } 
            },
            dragstart: function (e) {
                if ($(e.sourceNode).parentsUntil(".k-treeview", ".k-item").length == 0) {
                    e.preventDefault();
                }
                sourceparentnode = $("#PostionManageTreeView").data("kendoTreeView").parent(e.sourceNode).find("input").first();
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
        $(window.splitters).push($("#PositionManaView").data("kendoSplitter"));
    }

    /*
    var ProcessMapContextMenu = function ()
    {
        var MapProcessWindow = $("#MapProcessWindow").data("kendoWindow");
        if (!MapProcessWindow) {
            $("#MapProcessWindow").kendoWindow({
                width: "500px",
                title: "配置映射",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#MapProcessWindow .windowCancel").bind("click", MapProcessCancel);
                    $("#MapProcessWindow .windowConfirm").bind("click", MapProcessConfirm);
                },
                close: function (e) {
                    $("#MapProcessWindow .windowCancel").unbind("click", MapProcessCancel);
                    $("#MapProcessWindow .windowConfirm").unbind("click", MapProcessConfirm);
                    hideOperaMask("MapProcessWindow");
                },
                resizable: false,
                modal: true
            });
            MapProcessWindow = $("#MapProcessWindow").data("kendoWindow").center();
            window.splitters.push(MapProcessWindow);
        }

        var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
        
        $("#MapConnectionString").val((node.attr("data-ConnectionString") == "null" ? "" : node.attr("data-ConnectionString")));
        $("#Mapping").val((node.attr("data-Mapping") == "null" ? "" : node.attr("data-Mapping")));
        $("#WhereString").val((node.attr("data-WhereString") == "null" ? "" : node.attr("data-WhereString")));
        $("#MapProcessID").val(node.val());
        MapProcessWindow.open();
    }

    var MapProcessCancel = function () {
        $("#MapProcessWindow").data("kendoWindow").close()
    }
    var MapProcessConfirm = function () {
        var that = $(this);
        that.unbind("click", MapProcessConfirm);        
        var validator = $("#MapProcessWindow").kendoValidator().data("kendoValidator");
        if (validator.validate()) {
            showOperaMask("MapProcessWindow");
            var ID = $("#MapProcessID").val();
            var ConnectionString = $("#MapConnectionString").val();
            var Mapping = $("#Mapping").val();
            var WhereString = $("#WhereString").val();

            $.post("/KstarMobile/MobileConfig/MapProcess", { ID: Math.abs(ID), ConnectionString: ConnectionString, Mapping: Mapping, WhereString: WhereString }, function (data) {
                if (data.flag) {
                    popupNotification.show("配置映射成功", "info");
                    TreeViewReload();
                    //关闭弹出框
                    $("#MapProcessWindow").data("kendoWindow").close();
                }
                else {
                    popupNotification.show("配置映射失败", "info");
                }
            }).fail(function () {
                that.bind("click", MapProcessConfirm);
                hideOperaMask("MapProcessWindow");
                popupNotification.show("配置映射失败", "info");
            })
        }
    }
    */

    var AddProcess = function (parentId) {
        var AddProcessWindow = $("#AddProcessWindow").data("kendoWindow");
        if (!AddProcessWindow) {
            $("#AddProcessWindow").kendoWindow({
                width: "500px",
                title: jsResxKstarMobile_MobileConfig["AddProcess"],
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddProcessWindow .windowCancel").bind("click", ProcessCancel);
                    $("#AddProcessWindow .windowConfirm").bind("click", ProcessConfirm);
                },
                close: function (e) {
                    $("#AddProcessWindow .windowCancel").unbind("click", ProcessCancel);
                    $("#AddProcessWindow .windowConfirm").unbind("click", ProcessConfirm);
                    hideOperaMask("AddProcessWindow");
                },
                resizable: false,
                modal: true
            });
            AddProcessWindow = $("#AddProcessWindow").data("kendoWindow").center();
           $(window.splitters).push(AddProcessWindow);
        }
        InitProcessList();        
        $("#ConnectionString").val("");
        $("#Mapping").val("");
        $("#WhereString").val("");        

        if (typeof (parentId) == "string") {
            $("#AddProcessWindow .windowConfirm").attr("data-parentId", parentId);
        }
        else {
            $("#AddProcessWindow .windowConfirm").attr("data-parentId", "");
        }
        AddProcessWindow.open();
    }

    var InitProcessList = function ()
    {
        $.getJSON("/KstarMobile/MobileConfig/GetProcessList", function (items) {
            $("#AddProcessName").kendoDropDownList({
                dataTextField: "ProcessFullName",
                dataValueField: "ProcessFullName",
                dataSource: {
                    data: items,
                    schema: {
                        model: {
                            id: "ProcessFullName",
                            fields: {
                                ProcessName: { type: "String" },
                                ProcessFullName: { type: "String" }
                            }
                        }
                    }
                }                
            });
         //   $("#AddProcessName").data("kendoDropDownList").value().select(0);
        });        
    }

    var ProcessCancel = function () {
        $("#AddProcessWindow").data("kendoWindow").close()
    }
    var ProcessConfirm = function () {        
        var ProcessName = $("#AddProcessName").data("kendoDropDownList").value();
        if (ProcessName == null || ProcessName == undefined || ProcessName.length == 0)
        {
            ShowTip(jsResxKstarMobile_MobileConfig.SelectProcess, "info");
            return;
        }
        var that = $(this);
        that.unbind("click", ProcessConfirm);                   
        showOperaMask("AddProcessWindow");        
        var ConnectionString = $("#ConnectionString").val();
        var Mapping = $("#Mapping").val();
        var WhereString = $("#WhereString").val();
        $.post("/KstarMobile/MobileConfig/AddProcess", { ProcessFullName: ProcessName, ConnectionString: ConnectionString, Mapping: Mapping, WhereString: WhereString }, function (data) {
            if (data.flag) {
                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Success"]);
                $(".top-title").siblings(".tips").css("visibility", "visible");
                //popupNotification.show(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Success"], "info");
                TreeViewReload();
                $("#dataSection").hide();
                $("#groupSection").hide();
                $("#itemSection").hide();
                //关闭弹出框
                $("#AddProcessWindow").data("kendoWindow").close();
            }
            else {
                $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Failure"]);
                $(".top-title").siblings(".tips").css("visibility", "visible");
                //popupNotification.show(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Failure"], "info");
            }
        }).fail(function () {
            that.bind("click", ProcessConfirm);
            hideOperaMask("AddProcessWindow");
            $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Failure"]);
            $(".top-title").siblings(".tips").css("visibility", "visible");
            //popupNotification.show(jsResxKstarMobile_MobileConfig["AddProcess"] + ProcessName + jsResxKstarMobile_MobileConfig["Failure"], "info");
        })        
    }

    function TreeViewReload()
    {
        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        var datasource = positions();
        treeview.setDataSource(datasource);
        datasource.read();
    }

    function SetTreeNodeText(text)
    {
        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        treeview.text("#PostionManageTreeView_tv_active", text);
    }

    function AppendNode(data)
    {
        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        var selectedNode = treeview.select();
        treeview.append(data, selectedNode);
    }

    function RemoveSelectNode()
    {
       var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        var selectedNode = treeview.select();
        treeview.remove(selectedNode);
    }
    function SelectNodeByText(text) {
        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        var data = treeview.findByText(text);
        treeview.select(data);
    }

    function SelectDataChildNodeBy()
    {
        var treeview = $("#PostionManageTreeView").data("kendoTreeView");
        var data = treeview.findByText("Data");
        var node=$(".k-item[data-id=2]");
        var childs = $(data).find(".k-group").children();
        var child = null;
        $.each(childs, function (n) {
            if (childs[n].innerText == "Item21") {
                child = childs[n];
            }
        });
        treeview.select(child);
    }
    
    function LoadPostionView() {
        window.title = "KstarMobile Management - Kendo UI";
        InitPositionSplitter();
        InitPostionManageTreeView();
        TreeViewNodeToggle("PostionManageTreeView");

        $("#ProcessAdd").click(function () {
            AddProcess(0);            
        })
        $("#saveitem").click(function () {
            var validator = $("#itemform").data("kendoValidator");
            if (validator.validate()) {
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                var data = { processid: Math.abs(node.val()), Mapping: $("#ItemMapping").val(), ID: $("#ItemChildID").val(), Name: $("#ItemName").val(), LabelName: $("#ItemLabelName").val(), Visible: $("#ItemVisible").prop("checked"), Editable: $("#ItemEditable").prop("checked"), Format: $("#ItemFormat").val() };
                $.post("/KstarMobile/MobileConfig/SaveProcessItem", data, function (data) {
                    if (data.flag) {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Success"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Success"], "info");
                        SetTreeNodeText($("#ItemName").val());
                    }
                    else {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    }
                }).fail(function () {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "item" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                });
            }            
        });

        $("#savegroup").click(function () {
            var validator = $("#groupform").data("kendoValidator");
            if (validator.validate()) {
                var GroupName = $("#GroupName").val();
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                //固定Group不允许修改名称
                if ("Task,TaskInfo,BaseInfo,ExtendInfo,ProcBaseInfo,BizInfo,ProcLogInfo,Header,Row,Data,More".indexOf(node.attr("data-DisplayName")) > -1)
                {
                    GroupName = node.attr("data-DisplayName");
                }
                var data = { processid: Math.abs(node.val()), ConnectionString: $("#GroupConnectionString").val(), Mapping: $("#GroupMapping").val(), WhereString: $("#GroupWhereString").val(), ID: $("#GroupChildID").val(), Name: GroupName, LabelName: $("#GroupLabelName").val(), Collapsed: $("#GroupCollapsed").prop("checked"), Type: $("#GroupType").val() };
                $.post("/KstarMobile/MobileConfig/SaveProcessGroup", data, function (data) {
                    if (data.flag) {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Success"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Success"], "info");
                        SetTreeNodeText(GroupName);
                    }
                    else {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    }
                }).fail(function () {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["Save"] + "group" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                });
            }
        });

        $("#saveprocess").click(function () {
            var validator = $("#dataform").data("kendoValidator");
            if (validator.validate()) {
                var ProcessName = $("#ProcessName").val();
                var node = $("#PostionManageTreeView_tv_active").find("input").first().prop("checked", true);
                var data = { ID: Math.abs(node.val()), ProcessFullName: ProcessName, ControllerFullName: $("#ProcessController").val(), ProcessActivitys: getCheckPermissionList() };

                $.post("/KstarMobile/MobileConfig/UpdateProcessName", "entityJson="+JSON.stringify(data), function (data) {
                    if (data.flag) {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Success"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Success"], "info");
                        SetTreeNodeText(ProcessName);
                    }
                    else {
                        $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Failure"]);
                        $(".top-title").siblings(".tips").css("visibility", "visible");
                        //popupNotification.show(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                    }
                }).fail(function () {
                    $(".top-title").siblings(".tips").text(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Failure"]);
                    $(".top-title").siblings(".tips").css("visibility", "visible");
                    //popupNotification.show(jsResxKstarMobile_MobileConfig["Update"] + "ProcessName" + jsResxKstarMobile_MobileConfig["Failure"], "info");
                });
            }
        });


        $("#AddGroup").click(function () {
            AddGroupContextMenu();
        });
        $("#AddItem").click(function () {
            AddItemContextMenu();
        });

        $("#DeleteProcess").click(function () {
            DelItemContextMenu();
        });
        $(".top-title").siblings(".tips").text("");
        $(".top-title").siblings(".tips").css("visibility", "hidden");
    }


    var ProcessPermissionListModel = kendo.data.Model.define({
        id: "ID",
        fields: {
            ID: { type: "string" },
            Name: { type: "string" },
            Checked: { type: 'string' }
        }
    });

    var ProcessPermissionListcolumns = [
   {
       title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= ID #' data-name='#= Name #' data-checked='#= Checked #'  #:Checked #/>",
       headerTemplate: "<input type='checkbox' />"
   },
   { field: "Name", title: "环节名称" }
    ];

    var InitProcessPermissionList = function (processFullName, $this) {
        showOperaMask();
        $.getJSON("/KstarMobile/MobileConfig/GetMaxProcess", { processName: processFullName, _t: new Date() }, function (items) {

            $("#ProcessController").val(items.ControllerFullName);

            InitBaseKendoGrid("PermissionList", ProcessPermissionListModel, ProcessPermissionListcolumns, items.ProcessActivitys, function () {
                hideOperaMask();
                bindGridCheckbox("PermissionList");
                InitPermissionListEven();
            });
        })
    };

    var InitPermissionListEven = function () {
        $("#PermissionList").on("click", ":checkbox", function () {
            var roleListCheckBox = $("#PermissionList").find(":checkbox");
            $.each(roleListCheckBox, function (index, checkBox) {
                var checked = $(checkBox).data("checked");
                if (checked == "checked") { 
                    if (checkBox.checked != true) {
                        $("#PermissionList").data("change", true);
                        return;
                    }
                } else {
                    if (checkBox.checked == true) {
                        $("#PermissionList").data("change", true);
                        return;
                    }
                }
            });
        });
    }

    var getCheckPermissionList = function () {
        var roleListCheckBox = $("#PermissionList").find(":checkbox");
        var checkList = new Array();
        $.each(roleListCheckBox, function (index, checkBox) {
            var checked = $(checkBox).data("checked");
            if (checkBox.checked) {
                var item = { ID: 0, Name: $(checkBox).data("name"), Checked: checked };
                checkList.push(item);
            }
          
        });
        return checkList;
    }

    //流程权限
    var GetProcessPermissionList = function (processFullName) {
        InitProcessPermissionList(processFullName,$(this))
        //$.getJSON("/KstarMobile/MobileConfig/GetMaxProcess?processName=" + processFullName, function (items) {

        //    var html = "<ul class=\"list-group\">";  
        //    var itemList = "";
        //    $.each(items, function (index, item) {
        //        itemList += " <li class=\"list-group-item\">" + item.Name + "</li> ";
        //    });
        //    html = html + itemList + "</ul>";
        //    $("#PermissionList").html(html);
        //});     
    }


    module.exports = LoadPostionView;
})







