define(function (require, exports, module) {
    var AddIdlist;
    var FieldsIdlist;
    var UserIdlist;

    var fieldextendcolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.Name + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    { field: "DisplayName", title: jsResxColumns.DisplayName, filterable: false },
    { field: "Description", title: jsResxColumns.ShortDescription, filterable: false },
    { field: "DefalutValue", title: jsResxColumns.Default, filterable: false },
    { field: "Value", title: jsResxColumns.SettingValue, filterable: false },
    { field: "FieldType", title: jsResxColumns.Type, filterable: false },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditField(data) } }], width: 58 }
    ]

    function fieldextend() {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/FieldExtend/GetFxExtend");
                    },
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "SysId",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    var FieldExtendManageTreeView;
    var InitFieldExtendManageTreeView = function () {
        $("#FieldExtendManageTreeView").kendoTreeView({
            template: kendo.template($("#FieldExtendManageTreeView-template").html()),
            dataSource: fieldextend(),
            select: function (e) {
                $("#itemSave").siblings(".tips").css("visibility", "hidden");
                $("#FieldExtendManageTreeView").find("input").prop("checked", false);
                var node = $("#FieldExtendManageTreeView_tv_active").find("input").first().prop("checked", true); //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息                
                $("#itemId").val(node.val());
                $("#itemName").val(node.attr("data-DisplayName"));
                $("#itemName").attr("disabled", true).css("background-color", "#e3e3e3");
                var item = $("#FieldExtendManageTreeView").data("kendoTreeView").dataSource.get(node.val());
                UserIdlist = item.fields;
                InitBaseKendoGrid("FieldList", FieldExtendModel, fieldextendcolumns, item.fields, function () {
                    bindGridCheckbox("FieldList")
                });

                $('#FieldExtendManageTreeView .k-state-focused').WinContextMenu({
                    //cancel: '.cancel',
                    menu: "#FieldExtendContextMenu",
                    removeMenu: '#homeBody',
                    action: function (e) {
                        switch (e.id) {
                            case "DelContextMenu": DelFieldExtend(); break;
                        }
                    }
                });
            },
            collapse: function (e) {
                $("#FieldExtendManageTreeView_tv_active").find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $("#FieldExtendManageTreeView_tv_active").find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {
                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $("#FieldExtendManageTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
                $("#FieldExtendManageTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
            }
        });
        FieldExtendManageTreeView = $("#FieldExtendManageTreeView").data("kendoTreeView");
    }
    var InitFieldExtendSplitter = function () {
        $("#FieldExtendManaView").kendoSplitter({
            panes: [
                { collapsible: false, size: "300px", min: "250px", max: "450px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        window.AddSplitters($("#FieldExtendManaView").data("kendoSplitter"));
    }

    var AddFieldExtend = function () {
        var AddFieldExtendWindow = $("#AddFieldExtendWindow").data("kendoWindow");
        if (!AddFieldExtendWindow) {
            $("#AddFieldExtendWindow").kendoWindow({
                width: "500px",
                title: jsResxMaintenance_SeaFieldExtend.AddFieldExtend,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddFieldExtendWindow .windowCancel").bind("click", FieldExtendCancel);
                    $("#AddFieldExtendWindow .windowConfirm").bind("click", FieldExtendConfirm);
                },
                close: function (e) {
                    $("#AddFieldExtendWindow .windowCancel").unbind("click", FieldExtendCancel);
                    $("#AddFieldExtendWindow .windowConfirm").unbind("click", FieldExtendConfirm);
                    hideOperaMask("AddFieldExtendWindow");
                },
                resizable: false,
                modal: true
            });
            AddFieldExtendWindow = $("#AddFieldExtendWindow").data("kendoWindow").center();
            window.AddSplitters(AddFieldExtendWindow);
        }

        $("#FieldExtendName").val("");
        AddFieldExtendWindow.open();
    }
    var DelFieldExtend = function () {
        var node = $("#FieldExtendManageTreeView_tv_active").find("input:checked");
        if (node.val() != null && node.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaFieldExtend.Areyousuretodeletethisextend, function (result) {
                if (result) {
                    $.post("/Maintenance/FieldExtend/DeleteFieldExtend", { Name: node.attr("data-DisplayName") }, function (FieldExtendID) {
                        var node = FieldExtendManageTreeView.dataSource.get(FieldExtendID);
                        FieldExtendManageTreeView.dataSource.remove(node);
                        $("#itemName").val("");
                        //$("#FieldExtendManageTreeView").parent().siblings().last().children("ul").children().last().hide();                     
                    });
                }
            });
        }
        else { ShowTip(jsResxMaintenance_SeaFieldExtend.PleaseselecttheFieldExtendtoberemovederror, "error"); }
    }

    var FieldExtendCancel = function () {
        $("#AddFieldExtendWindow").data("kendoWindow").close()
    }
    var FieldExtendConfirm = function () {
        var validator = $("#AddFieldExtendWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", FieldExtendConfirm);
            showOperaMask("AddFieldExtendWindow");
            var FieldExtendName = $("#FieldExtendName").val();
            $.post("/Maintenance/FieldExtend/CreateFieldExtend", { Name: FieldExtendName }, function (item) {
                var treeview = $("#FieldExtendManageTreeView").data("kendoTreeView");
                treeview.append(item, null);
                $("#AddFieldExtendWindow").data("kendoWindow").close()
            }).fail(function () {
                that.bind("click", FieldExtendConfirm);
                hideOperaMask("AddFieldExtendWindow");
            })
        }
    }

    var SaveItem = function () {
        showOperaMask("FieldExtendInfomation");
        var node = $("#FieldExtendManageTreeView_tv_active").find("input:checked");
        var itemName = $("#itemName").val();
        if (node.val() != "" && itemName != "") {
            var AddidList = [];
            var sourcearr = Array();
            $("#FieldList .k-grid-content").find(":checkbox").each(function () {
                sourcearr.length = 0;
                var item = $("#FieldList").data("kendoGrid").dataSource.get(this.value);
                if (item.FieldType == "ChooseField") {
                    var sr = item.Source;
                    for (var i = 0; i < sr.length; i++) {
                        sourcearr.push(sr[i]);
                    }
                }
                var citem = { DefalutValue: item.DefalutValue, Description: item.Description, DisplayName: item.DisplayName, Name: item.Name, Value: item.Value, FieldType: item.FieldType, Source: sourcearr };
                AddidList.push(obj2str(citem));
            })
            //console.log(AddidList.join(","));                
            $.ajax({
                url: "/Maintenance/FieldExtend/UpdateFieldExtend",
                type: "POST",
                data: { SysId: node.val(), Name: itemName, fieldlist: "[" + AddidList.join(",") + "]" },
                traditional: true,
                success: function (item) {
                    var template = kendo.template($("#FieldExtendManageTreeView-template").html())
                    var target = $("#FieldExtendManageTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                    target.find("input").first().prop("checked", true);
                    hideOperaMask("FieldExtendInfomation");
                    $("#itemSave").siblings(".tips").css("visibility", "visible");
                },
                dataType: "json"
            }).fail(function () { hideOperaMask("FieldExtendInfomation"); })
        }
        else {
            hideOperaMask(jsResxMaintenance_SeaFieldExtend.FieldExtendInfomation);
            ShowTip(jsResxMaintenance_SeaFieldExtend.PleaseselecttheFieldExtenderror, "error");
        }
    }
    var AddField = function () {
        $("#FieldName").val("");
        $("#AddFieldWindow").data("kendoValidator").hideMessages();
        var node = $("#FieldExtendManageTreeView_tv_active").find("input:checked");
        if (node.val() != null && node.length > 0) {

            $("#AddFieldWindow").kendoWindow({
                width: "780px",
                height: "240px",
                title: jsResxMaintenance_SeaFieldExtend.AddField,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    if ($("#FieldType").data("kendoDropDownList").value() == "ChooseField") {
                        $("#divsource").show();
                        initSource([]);
                    }
                    else {
                        $("#divsource").hide();
                    }
                    $("#Name").val("");
                    $("#DisplayName").val("");
                    $("#Description").val("");
                    $("#DefalutValue").val("");
                    $("#Value").val("");
                    $("#Type").val($("#FieldType").data("kendoDropDownList").value()).attr("disabled", true).css("background-color", "#e3e3e3");

                    $("#AddFieldWindow .windowCancel").bind("click", AddFieldItemCancel);
                    $("#AddFieldWindow .windowConfirm").bind("click", AddFieldItemConfirm);
                },
                close: function (e) {
                    $("#AddFieldWindow .windowCancel").unbind("click", AddFieldItemCancel);
                    $("#AddFieldWindow .windowConfirm").unbind("click", AddFieldItemConfirm);
                },
                resizable: false,
                modal: true
            });
            AddFieldWindow = $("#AddFieldWindow").data("kendoWindow");
            AddFieldWindow.title(jsResxMaintenance_SeaFieldExtend.AddField);
            window.AddSplitters(AddFieldWindow);
            AddFieldWindow.center();
            AddFieldWindow.open();
        }
        else {
            ShowTip(jsResxMaintenance_SeaFieldExtend.PleaseselecttheFieldExtendtobeadderror, "error");
        }
    }
    var AddFieldItemCancel = function () {        
        $("#AddFieldWindow").data("kendoWindow").close();
    }
    var AddFieldItemConfirm = function () {        
        var that = $(this);
        that.unbind("click", AddFieldItemConfirm);
        var validator = $("#AddFieldWindow").data("kendoValidator");
        if (validator.validate()) {
            var type = $("#FieldType").val();
            var source = [];
            if (type == "ChooseField") {
                source = $("#Source").data("kendoDropDownList").dataSource._data;
            }            
            var item = { DefalutValue: $("#DefalutValue").val(), Description: $("#Description").val(), DisplayName: $("#DisplayName").val(), Name: $("#Name").val(), Value: $("#Value").val(), FieldType: type, Source: source };
            if ($("#Name").val().length > 0) {
                $("#FieldList").data("kendoGrid").dataSource.add(item);                
                AddFieldItemCancel();
            }
        }
        else {
            that.bind("click", AddFieldItemConfirm);
        }        
    }


    var DelField = function () {
        var idList = new Array();
        var id = $("#FieldExtendManageTreeView_tv_active").find("input").val();
        $("#FieldList .k-grid-content").find(":checked").each(function () {
            var item = $("#FieldList").data("kendoGrid").dataSource.get(this.value);
            $("#FieldList").data("kendoGrid").dataSource.remove(item);
            idList.push(this.value)
        })
        if (idList.length > 0) {
        }
        else {
            ShowTip(jsResxMaintenance_SeaFieldExtend.Pleaseselectfielderror);
        }
    }
    var EditField = function (data) {
        $("#FieldName").val(data.Name);
        $("#AddFieldWindow").data("kendoValidator").hideMessages();
        var node = $("#FieldExtendManageTreeView_tv_active").find("input:checked");
        if (node.val() != null && node.length > 0) {
            $("#AddFieldWindow").kendoWindow({
                width: "780px",
                height: "240px",
                title: jsResxMaintenance_SeaFieldExtend.EditField,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    if (data.FieldType == "ChooseField") {
                        $("#divsource").show();                       
                        initSource(data.Source);
                    }
                    else {
                        $("#divsource").hide();
                    }
                    $("#Name").val(data.Name);
                    $("#DisplayName").val(data.DisplayName);
                    $("#Description").val(data.Description);
                    $("#DefalutValue").val(data.DefalutValue);
                    $("#Value").val(data.value);
                    $("#Type").val(data.FieldType).attr("disabled", true).css("background-color", "#e3e3e3");

                    $("#AddFieldWindow .windowCancel").bind("click", EditFieldItemCancel);
                    $("#AddFieldWindow .windowConfirm").bind("click", data,EditFieldItemConfirm);

                },
                close: function (e) {
                    $("#AddFieldWindow .windowCancel").unbind("click", EditFieldItemCancel);
                    $("#AddFieldWindow .windowConfirm").unbind("click", EditFieldItemConfirm);                    
                },
                resizable: false,
                modal: true
            });
            AddFieldWindow = $("#AddFieldWindow").data("kendoWindow");
            AddFieldWindow.title(jsResxMaintenance_SeaFieldExtend.EditField);
            window.AddSplitters(AddFieldWindow);
            AddFieldWindow.center();           
            AddFieldWindow.open();
        }
        else {
            ShowTip(jsResxMaintenance_SeaFieldExtend.PleaseselecttheFieldExtendtobeadderror, "error");
        }
    }

    var EditFieldItemCancel = function () {       
        $("#AddFieldWindow").data("kendoWindow").close();
    }
    var EditFieldItemConfirm = function (event) {     
        var data = event.data;
        var that = $(this);
        that.unbind("click", EditFieldItemConfirm);
        var validator = $("#AddFieldWindow").data("kendoValidator");
        if (validator.validate()) {           
            var item = $("#FieldList").data("kendoGrid").dataSource.get(data.Name);                   
            if (data.FieldType == "ChooseField") {   
                var source=[];            
                var _cfdata =  $("#Source").data("kendoDropDownList").dataSource._data;              
                $.each(_cfdata,function(i){      
                    source.push(_cfdata[i].value);                    
                });             
                item.set("Source", source);                
            }
            item.set("Name", $("#Name").val());
            item.set("DisplayName", $("#DisplayName").val());
            item.set("Description", $("#Description").val());
            item.set("DefalutValue", $("#DefalutValue").val());
            item.set("Value", $("#Value").val());       
            EditFieldItemCancel();
        }
        else {
            that.bind("click", EditFieldItemConfirm);
        }       
    }

    var initSource = function (arr) {
        var item;
        var data = $("#Source").data("kendoDropDownList").dataSource;
        data._data.length = 0;
        $.each(arr, function (i) {
            item = { text: arr[i], value: arr[i] };
            data.add(item);
        });
        $("#Source").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: data,
            index: 0,
            readonly: true,
            template: '#: data.text # <span class="glyphicon glyphicon-remove" style="float: right;line-height: 22px;" onclick="sourceDelete.apply(this)"></span>',
        });
    }
    var SourceAdd = function () {
        var SourceItem = $("#SourceItem").val();    
        if (SourceItem.length > 0) {
            var flag = true;
            var item = { text: SourceItem, value: SourceItem };
            var data = $("#Source").data("kendoDropDownList").dataSource;         
            $.each(data._data, function (i) {
                if (this.value.toLowerCase() == SourceItem.toLowerCase()) {
                    flag = false;
                }
            });        
            if (flag) {
                data.add(item);
                $("#Source").kendoDropDownList({
                    dataTextField: "text",
                    dataValueField: "value",
                    dataSource: data,
                    index: 0,
                    readonly: true,
                    template: '#: data.text # <span class="glyphicon glyphicon-remove" style="float: right;line-height: 22px;" onclick="sourceDelete.apply(this)"></span>',
                });
            }
        }
        else {
            ShowTip(jsResxMaintenance_SeaFieldExtend.Pleasesfillthevalueerror);
        }
    }

    var SourceDel = function () {
        var SourceItem = $("#SourceItem").val();       
        if (SourceItem.length > 0) {
            var flag = false;
            var item = { text: SourceItem, value: SourceItem };
            var data = $("#Source").data("kendoDropDownList").dataSource;
            $.each(data._data, function (i) {
                if (this.value.toLowerCase() == SourceItem.toLowerCase()) {
                    flag = true;
                }
            });           
            if (flag) {                          
                data._data.remove(item);            
                $("#Source").kendoDropDownList({
                    dataTextField: "text",
                    dataValueField: "value",
                    dataSource: data,
                    index: 0,
                    readonly: true,
                    template: '#: data.text # <span class="glyphicon glyphicon-remove" style="float: right;line-height: 22px;" onclick="sourceDelete.apply(this)"></span>',
                });
            }
        }
        else {
            var curitem = $("#Source").data("kendoDropDownList").value();
            var item = { text: curitem, value: curitem };
            var data = $("#Source").data("kendoDropDownList").dataSource;
            data._data.remove(item);
            $("#Source").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: data,
                index: 0,
                readonly: true,
                template: '#: data.text # <span class="glyphicon glyphicon-remove" style="float: right;line-height: 22px;" onclick="sourceDelete.apply(this)"></span>',
            });
        }
    }

    function LoadFieldExtendView() {
        window.title = "FieldExtend Management - Kendo UI";
        InitFieldExtendSplitter();
        InitFieldExtendManageTreeView();
        $("#FieldExtendAdd").click(function () { AddFieldExtend(); })
        $("#itemSave").click(SaveItem);
        $("#FieldAdd").click(AddField);
        $("#FieldeeDel").click(DelField);
        $("#SourceAdd").click(SourceAdd);
        $("#SourceDel").click(SourceDel);
        $("#FieldExtendInfomation").children("ul").kendoPanelBar();
     
    }
    module.exports = LoadFieldExtendView;
})