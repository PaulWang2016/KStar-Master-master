define(function (require, exports, module) {
    var initParentId;

    var InitPositionSplitter = function () {
        $("#DataDictionaryView").kendoSplitter({
            panes: [
                { collapsible: false, size: "400px", min: "300px", max: "600px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
        $(window.splitters).push($("#DataDictionaryView").data("kendoSplitter"));
    }

    //分类数据源
    function DataDictionCategoryDataSource() {
        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/DataDictionary/GetDataDictionaryCategory?_t={0}", new Date());
                    },
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "Id",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    //分类树形    
    var InitDataDictionaryCategoryTreeView = function () {
        var DataDictionaryCategoryTreeView = $("#DataDictionaryCategoryTreeView").data("kendoTreeView");
        if (!DataDictionaryCategoryTreeView) {
            $("#DataDictionaryCategoryTreeView").kendoTreeView({
                template: kendo.template($("#DataDicTreeView-template").html()),
                dataSource: DataDictionCategoryDataSource(),
                select: function (e) {
                     
                    var item = e.sender.dataItem(e.node);
                    if (item != undefined && item.Type == 1) {
                        initParentId = item.id;
                        InitDataDictionaryTreeView();
                        ButtonControl(true);
                    }
                    else {
                        initParentId = "00000000-0000-0000-0000-000000000000";
                        InitDataDictionaryTreeView();
                        ButtonControl(false);
                    }
                },
                collapse: function (e) {
                    $("#DataDictionaryCategoryTreeView_tv_active").find(".k-sprite").first().removeClass("on");
                },
                expand: function (e) {
                    $("#DataDictionaryCategoryTreeView_tv_active").find(".k-sprite").first().addClass("on");
                },
                dataBound: function (e) {
                    var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                    var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                    $("#DataDictionaryCategoryTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);
                    $("#DataDictionaryCategoryTreeView").data("kendoTreeView").expand(".k-first");
                }
            });
            DataDictionaryCategoryTreeView = $("#DataDictionaryCategoryTreeView").data("kendoTreeView");
        }
        else {
            DataDictionaryCategoryTreeView.setDataSource(DataDictionCategoryDataSource());
        }
    }

    //字典数据源
    function DataDictionaryTreeViewDataSource() {
        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Maintenance/DataDictionary/GetDataDictionary?initParentId={0}&_t={1}", initParentId, new Date());
                    },
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "Id",               //绑定ID
                    hasChildren: "HasChildren"  //绑定是否包含子节点                 
                }
            }
        });
    }
    //字典树形    
    var InitDataDictionaryTreeView = function () {
        var DataDictionaryTreeView = $("#DataDictionaryTreeView").data("kendoTreeView");
        if (!DataDictionaryTreeView) {
            $("#DataDictionaryTreeView").kendoTreeView({
                template: kendo.template($("#DataDicTreeView-template").html()),
                dataSource: DataDictionaryTreeViewDataSource(),
                select: function (e) {
                     
                    var item = e.sender.dataItem(e.node);
                    if (item.Type == 1) {

                    }
                },
                collapse: function (e) {
                    $("#DataDictionaryTreeView_tv_active").find(".k-sprite").first().removeClass("on");
                },
                expand: function (e) {
                    $("#DataDictionaryTreeView_tv_active").find(".k-sprite").first().addClass("on");
                },
                dataBound: function (e) {
                     
                    var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                    var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                    $("#DataDictionaryTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown);                    
                    $("#DataDictionaryTreeView").data("kendoTreeView").expand(".k-first");                    
                }
            });
            DataDictionaryTreeView = $("#DataDictionaryTreeView").data("kendoTreeView");
            TreeViewNodeToggle("DataDictionaryTreeView");
        }
        else {
            DataDictionaryTreeView.setDataSource(DataDictionaryTreeViewDataSource());
        }
    }

    //操作分类 start
    var InitAddDataDicCategoryWindow = function () {
        var AddDataDicCategoryWindow = $("#AddDataDicCategoryWindow").data("kendoWindow");
        if (!AddDataDicCategoryWindow) {
            $("#AddDataDicCategoryWindow").kendoWindow({
                width: "500px",
                height: "280px",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddDataDicCategoryWindow .windowCancel").bind("click", DataDicCategoryCancel)
                    $("#AddDataDicCategoryWindow .windowConfirm").bind("click", DataDicCategoryConfirm)
                },
                close: function (e) {
                    $("#AddDataDicCategoryWindow .windowCancel").unbind("click", DataDicCategoryCancel)
                    $("#AddDataDicCategoryWindow .windowConfirm").unbind("click", DataDicCategoryConfirm)
                },
                resizable: false,
                modal: true
            });
            AddDataDicCategoryWindow = $("#AddDataDicCategoryWindow").data("kendoWindow");
            window.AddSplitters(AddDataDicCategoryWindow);
        }

        return AddDataDicCategoryWindow;
    }

    var DataDicCategoryCancel = function () {
        $("#AddDataDicCategoryWindow").data("kendoWindow").close()
    }

    var DataDicCategoryConfirm = function () {                
        var validator = $("#AddDataDicCategoryWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", DataDicCategoryConfirm);
            showOperaMask("AddDataDicCategoryWindow");
            var selectitem;
            var tempselect=GetSelectTreeItem("DataDictionaryCategoryTreeView");
            if (tempselect != undefined && tempselect.Type==0)
            {
                selectitem = tempselect;
            }
            var Code = $("#DataDicCategoryCode").val();
            var Name = $("#DataDicCategoryName").val();
            var Type = $("#DataDicCategoryType").data("kendoDropDownList").value();
            var Order = $("#DataDicCategoryOrder").data("kendoNumericTextBox").value();
            var Remark = $("#DataDicCategoryRemark").val();            

            $.post($("#AddDataDicCategoryWindow").attr("url"), { Id: (($("#hdDataDicCategoryId").val() != undefined && $("#hdDataDicCategoryId").val()!="")?$("#hdDataDicCategoryId").val():null), Code: Code, Name: Name, Type: Type, Order: (Order==null?1:Order), Remark: Remark, ParentId: ((selectitem == undefined || selectitem == null) ? null : selectitem.Id) }, function (item) {
                var treeview = $("#DataDictionaryCategoryTreeView").data("kendoTreeView");
                if ($("#AddDataDicCategoryWindow").attr("type") == "add") {
                    if ((selectitem == undefined || selectitem == null)) {
                        treeview.append(item, null);
                    }
                    else {
                        treeview.append(item, GetSelectTreeNode("DataDictionaryCategoryTreeView"));
                    }
                }
                else {
                    var template = kendo.template($("#DataDicTreeView-template").html())
                    var target = $("#DataDictionaryCategoryTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                }
                $("#AddDataDicCategoryWindow").data("kendoWindow").close();
                hideOperaMask("AddDataDicCategoryWindow");
            }).fail(function () {
                that.bind("click", DataDicCategoryConfirm);
                hideOperaMask("AddDataDicCategoryWindow");
            })
        }        
    }

    var AddCategory = function () {
        AddDataDicCategoryWindow = InitAddDataDicCategoryWindow();
        AddDataDicCategoryWindow.title(jsResxMaintenance_SeaDataDictionary.AddDataDicCategory).center().open();

        $("#DataDicCategoryType").data("kendoDropDownList").select(0);
        $("#DataDicCategoryType").data("kendoDropDownList").enable();
        $("#DataDicCategoryName").val("");
        $("#DataDicCategoryCode").val("").attr("readonly", false).css("background-color", "#ffffff");
        $("#DataDicCategoryOrder").data("kendoNumericTextBox").value(1);
        $("#DataDicCategoryRemark").val("");
        $("#hdDataDicCategoryId").val("");

        $("#AddDataDicCategoryWindow").attr("url", "/Maintenance/DataDictionary/AddDataDicCategory");
        $("#AddDataDicCategoryWindow").attr("type", "add");

        $("#AddDataDicCategoryWindow").data("kendoValidator").hideMessages();
    }

    var EditCategory = function () {
        var selectitem = GetSelectTreeItem("DataDictionaryCategoryTreeView");
        if (selectitem == undefined) {
            ShowTip(jsResxMaintenance_SeaDataDictionary.Pleaseselectdatadictionarycategory, "error");
            return;
        }
        AddDataDicCategoryWindow = InitAddDataDicCategoryWindow();
        AddDataDicCategoryWindow.title(jsResxMaintenance_SeaDataDictionary.EditDataDicCategory).center().open();       

        $.post("/Maintenance/DataDictionary/GetDataDictionaryById", { Id: selectitem.Id }, function (item) {
            $("#DataDicCategoryType").data("kendoDropDownList").value(item.Type);
            $("#DataDicCategoryType").data("kendoDropDownList").enable(false);
            $("#DataDicCategoryName").val(item.Name);
            $("#DataDicCategoryCode").val(item.Code).attr("readonly", true).css("background-color", "#e3e3e3");            
            $("#DataDicCategoryOrder").data("kendoNumericTextBox").value(item.Order);
            $("#DataDicCategoryRemark").val(item.Remark);
            $("#hdDataDicCategoryId").val(item.Id);
        });
        
        $("#AddDataDicCategoryWindow").attr("url", "/Maintenance/DataDictionary/EditDataDicCategory");
        $("#AddDataDicCategoryWindow").attr("type", "edit");

        $("#AddDataDicCategoryWindow").data("kendoValidator").hideMessages();
    }

    var DelDataDicCategory = function () {
        var selectitem = GetSelectTreeItem("DataDictionaryCategoryTreeView");
        if (selectitem == undefined) {
            ShowTip(jsResxMaintenance_SeaDataDictionary.Pleaseselectdatadictionarycategory, "error");
            return;
        }
        bootbox.confirm(jsResxMaintenance_SeaDataDictionary.ConfirmDelDataDicCategory, function (result) {
            if (result) {
                showOperaMask();
                var treeview = $("#DataDictionaryCategoryTreeView").data("kendoTreeView");                               
                $.post("/Maintenance/DataDictionary/DelDataDictionary", { ID: selectitem.Id }, function (flag) {
                    //删除字典分类           
                    if (flag) {
                        var selectedNode = treeview.select();
                        treeview.remove(selectedNode);
                        initParentId = "00000000-0000-0000-0000-000000000000";
                        InitDataDictionaryTreeView();
                        ButtonControl(false);
                    }
                    hideOperaMask();
                }).fail(function () {
                    hideOperaMask();
                });
            }
        });
    }

    //操作分类 end


    //操作字典 start
    var InitAddDataDicWindow = function () {
        var AddDataDicWindow = $("#AddDataDicWindow").data("kendoWindow");
        if (!AddDataDicWindow) {
            $("#AddDataDicWindow").kendoWindow({
                width: "500px",
                height: "280px",
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddDataDicWindow .windowCancel").bind("click", DataDicCancel)
                    $("#AddDataDicWindow .windowConfirm").bind("click", DataDicConfirm)
                },
                close: function (e) {
                    $("#AddDataDicWindow .windowCancel").unbind("click", DataDicCancel)
                    $("#AddDataDicWindow .windowConfirm").unbind("click", DataDicConfirm)
                },
                resizable: false,
                modal: true
            });
            AddDataDicWindow = $("#AddDataDicWindow").data("kendoWindow");
            window.AddSplitters(AddDataDicWindow);
        }

        return AddDataDicWindow;
    }

    var DataDicCancel = function () {
        $("#AddDataDicWindow").data("kendoWindow").close()
    }

    var DataDicConfirm = function () {    
        var validator = $("#AddDataDicWindow").data("kendoValidator");
        if (validator.validate()) {
            var that = $(this);
            that.unbind("click", DataDicConfirm);
            showOperaMask("AddDataDicWindow");

            var selectitem;
            var tempselect = GetSelectTreeItem("DataDictionaryTreeView");
            if (tempselect == null || tempselect == undefined) {
                selectitem = GetSelectTreeItem("DataDictionaryCategoryTreeView");
            }
            else {
                selectitem = GetSelectTreeItem("DataDictionaryTreeView");
            }        

            var Code = $("#DataDicCode").val();
            var Name = $("#DataDicName").val();
            var Value = $("#DataDicValue").val();
            var Order = $("#DataDicOrder").data("kendoNumericTextBox").value();
            var Remark = $("#DataDicRemark").val();

            $.post($("#AddDataDicWindow").attr("url"), { Id: (($("#hdDataDicId").val() != undefined && $("#hdDataDicId").val() != "") ? $("#hdDataDicId").val() : null), Code: Code, Name: Name, Type: 2, Value: Value, Order: (Order == null ? 1 : Order), Remark: Remark, ParentId: ((selectitem == undefined || selectitem == null) ? null : selectitem.Id) }, function (item) {
                var treeview = $("#DataDictionaryTreeView").data("kendoTreeView");
                if ($("#AddDataDicWindow").attr("type") == "add") {
                    if ((tempselect == null || tempselect == undefined)) {
                        treeview.append(item, null);
                    }
                    else {
                        treeview.append(item, GetSelectTreeNode("DataDictionaryTreeView"));
                    }
                }
                else {
                    var template = kendo.template($("#DataDicTreeView-template").html())
                    var target = $("#DataDictionaryTreeView_tv_active .k-state-selected");
                    target.html(template({ item: item }));
                }
                $("#AddDataDicWindow").data("kendoWindow").close();
                hideOperaMask("AddDataDicWindow");
            }).fail(function () {
                that.bind("click", DataDicConfirm);
                hideOperaMask("AddDataDicWindow");
            })
        }
    }

    var AddDataDic = function () {
        AddDataDicWindow = InitAddDataDicWindow();
        AddDataDicWindow.title(jsResxMaintenance_SeaDataDictionary.AddDataDictionary).center().open();

        $("#DataDicName").val("");
        $("#DataDicCode").val("").attr("readonly", false).css("background-color", "#ffffff");
        $("#DataDicValue").val("");
        $("#DataDicOrder").data("kendoNumericTextBox").value(1);
        $("#DataDicRemark").val("");
        $("#hdDataDicId").val("");

        $("#AddDataDicWindow").attr("url", "/Maintenance/DataDictionary/AddDataDictionary");
        $("#AddDataDicWindow").attr("type", "add");

        $("#AddDataDicWindow").data("kendoValidator").hideMessages();
    }

    var EditDataDic = function () {
        var selectitem = GetSelectTreeItem("DataDictionaryTreeView");
        if (selectitem == undefined) {
            ShowTip(jsResxMaintenance_SeaDataDictionary.Pleaseselectdatadictionary, "error");
            return;
        }

        AddDataDicWindow = InitAddDataDicWindow();
        AddDataDicWindow.title(jsResxMaintenance_SeaDataDictionary.EditDataDictionary).center().open();
        
        $.post("/Maintenance/DataDictionary/GetDataDictionaryById", { Id: selectitem.Id }, function (item) {
            $("#DataDicName").val(item.Name);
            $("#DataDicCode").val(item.Code).attr("readonly", true).css("background-color", "#e3e3e3");;
            $("#DataDicValue").val(item.Value);            
            $("#DataDicOrder").data("kendoNumericTextBox").value(item.Order);
            $("#DataDicRemark").val(item.Remark);
            $("#hdDataDicId").val(item.Id);
        });

        

        $("#AddDataDicWindow").attr("url", "/Maintenance/DataDictionary/EditDataDictionary");
        $("#AddDataDicWindow").attr("type", "edit");

        $("#AddDataDicWindow").data("kendoValidator").hideMessages();
    }

    var DelDataDic = function () {
        var selectitem = GetSelectTreeItem("DataDictionaryTreeView");
        if (selectitem == undefined) {
            ShowTip(jsResxMaintenance_SeaDataDictionary.Pleaseselectdatadictionary, "error");
            return;
        }
        bootbox.confirm(jsResxMaintenance_SeaDataDictionary.ConfirmDelDataDictionary, function (result) {
            if (result) {
                showOperaMask();
                var treeview = $("#DataDictionaryTreeView").data("kendoTreeView");
                $.post("/Maintenance/DataDictionary/DelDataDictionary", { ID: selectitem.Id }, function (flag) {
                    //删除字典        
                    if (flag) {
                        var selectedNode = treeview.select();
                        treeview.remove(selectedNode);
                    }
                    hideOperaMask();
                }).fail(function () {
                    hideOperaMask();
                });
            }
        });
    }

    //操作字典 end

    function GetSelectTreeItem(id) {
        var node = $("#" + id + "_tv_active");
        var item = $("#" + id).data("kendoTreeView").dataItem(node);
        return item;
    }

    function GetSelectTreeNode(id)
    {
        var treeview = $("#" + id).data("kendoTreeView");
        var selectedNode = treeview.select();
        return selectedNode;
    }

    function ButtonControl(flag)
    {
        if (flag) {
            $("#DataAdd").bind("click", AddDataDic);
            $("#DataEdit").bind("click", EditDataDic);
            $("#DataDel").bind("click", DelDataDic);
        }
        else {
            $("#DataAdd").unbind("click", AddDataDic).attr("e");
            $("#DataEdit").unbind("click", EditDataDic);
            $("#DataDel").unbind("click", DelDataDic);
        }             
    }

    function DataClearSelect(id)
    {
        if ($("#" + id).data("kendoTreeView")) {
            $("#" + id).data("kendoTreeView").select(null);
            var node = $("#" + id + "_tv_active");
            node.removeAttr("id");
        }
    }

    var LoadDataDictionaryManagement = function () {
        InitPositionSplitter();
        InitDataDictionaryCategoryTreeView();
        TreeViewNodeToggle("DataDictionaryCategoryTreeView");

        $("#CategoryAdd").click(AddCategory);
        $("#CategoryEdit").click(EditCategory);
        $("#CategoryDel").click(DelDataDicCategory);  

        $("#DataAdd").click(AddDataDic);
        $("#DataEdit").click(EditDataDic);
        $("#DataDel").click(DelDataDic);

        $("#DataClearSelect").click(function () {
            DataClearSelect("DataDictionaryTreeView");           
        });

        $("#CategoryClearSelect").click(function () {
            DataClearSelect("DataDictionaryCategoryTreeView");
            initParentId = "00000000-0000-0000-0000-000000000000";
            InitDataDictionaryTreeView();
            ButtonControl(false);
        });
        

        $("#DataDicCategoryType").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource:  [
                { text: jsResxMaintenance_SeaDataDictionary.Folder, value: "0" },
                { text: jsResxMaintenance_SeaDataDictionary.DataCategory, value: "1" },
            ],
            index: 0,
            readonly: true
        });

        $("#DataDicCategoryOrder").kendoNumericTextBox();
        $("#DataDicOrder").kendoNumericTextBox();

        ButtonControl(false);
    }
    
    module.exports = LoadDataDictionaryManagement;
})