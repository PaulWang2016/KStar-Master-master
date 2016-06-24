var Settings = new kendo.data.HierarchicalDataSource({
    transport: {
        read: {
            url: "/Maintenance/Master/GetSettings",//GetSettings
            dataType: "json"
        }
    },
    schema: {
        id: "ID",
        model: {
            id: "ID",               //绑定ID
            hasChildren: "HasChildren",    //绑定是否包含子节点
        }
    }
});
var SettingModel = kendo.data.Model.define({
    id: "Name",
    fields: {
        Displayname: { type: "string" },
        Parent: { type: "string" },
        Name: { type: "string" },
        Sort: { type: "string" },
        Status: { type: "bool" }
    }
});
var Model = kendo.data.Model.define({
    fields: {
        Code: { type: "string" },
        Name: { type: "string" }
    }
});
var Column = [
    {
        title: "Checked", width: 35, template: "<input type='checkbox' value = '#= Code #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "Code", title: "Code" },
    { field: "Name", title: "Name" }
]
var settingcolumns = [
    {
        title: "Checked", width: 35, template: "<input type='checkbox' value = '#= Name #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "Displayname", title: "Displayname" },
    { field: "Sort", title: "Sort" },
    {
        field: "Status", title: "Status",
        width: 70,
        template: function (item) {
            return item.Status ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button circle'><span class='glyphicon glyphicon-ban-circle'></span></a>"
        }
    },
     {
         command: [{
             name: "edit", template:
                 "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); edit(data) }
         }], width: 58
     }
]
var settingcolumnsWithParent = [
    {
        title: "Checked", width: 35, template: "<input type='checkbox' value = '#= Name #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "Displayname", title: "Displayname" },
    { field: "Parent", title: "Parent" },
    { field: "Sort", title: "Sort" },
    {
        field: "Status", title: "Status",
        width: 70,
        template: function (item) {
            return item.Status ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button circle'><span class='glyphicon glyphicon-ban-circle'></span></a>"
        }
    },
     {
         command: [{
             name: "edit", template:
                 "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); edit(data) }
         }], width: 58
     }
]
var addsetting = function () {
    $("#AddSettingsWindow .windowConfirm").attr("data-url", "/Maintenance/Master/AddSettings");
    $("#AddSettingsWindow").data("kendoWindow").center().title("Add Settings Window").open();
}
var editsetting = function () {
    var id = $("#SettingTreeView_tv_active").find(":checked").val();
    var item = $("#SettingTreeView").data("kendoTreeView").dataSource.get(id);
    $("#AddSettingsWindow .windowConfirm").attr("data-url", "/Maintenance/Master/EditSettings").attr("data-Id", id);
    $("#AddSettings .k-textbox").val(item.Name);
    $("#AddSettingsWindow").data("kendoWindow").center().title("Edit Settings Window").open();

}
var delsetting = function () {
    var hidden = $("#SettingTreeView_tv_active").find(":checked").val();
    if (hidden) {
        $.post("/Maintenance/Master/DeleteSettings", { ID: hidden }, function (ID) {

            Settings.remove(Settings.get(ID));  //======后台删除数据
        })
    }
}

var reset = function () {
    $("#SettingView .Category").val("");
    $("#SettingView .Name").val("");
}

var check = function () {
    if (null == $("#SettingTreeView_tv_active").find(":checked").val()) return;

    $.post("/Maintenance/Master/DoCheck",
      {
          ID: $("#SettingTreeView_tv_active").find(":checked").val(),
          Category: $("#SettingView .Category").val(),
          Name: $("#SettingView .Name").val(),
          IsChecked: $("input:checkbox[name=IsNull]").prop("checked")
      }, function (data) {
          getKendoGrid("SettingGrid").dataSource.data(data);
      });
}

var add = function () {
    //   var hidden = $("#SettingTreeView_tv_active").find("input").first().prop("checked", true);  利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
    //if (hidden.val() == null) return;
    //getKendoGrid("SettingTreeView").dataSource.get(

    $("#AddSettingWindow .parent").hide();
    $("#Parent").val("");
    if ($("#SettingTreeView_tv_active").find("input").first().val() == "ElectionDistrict_DistDB") {
        $("#AddSettingWindow .parent").show();
        $("#Parent").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: {
                transport: {
                    read: "/Request/GetElection"
                },
            },
            optionLabel: "--Select Value--",
        }).data("kendoDropDownList");
    }
    $("#AddSettingWindow").data("kendoWindow").center().title("Add " + $("#SettingTreeView_tv_active :input").val()).open();
    $("#AddSettingWindow .windowConfirm").attr("data-url", "/Maintenance/Master/AddSettinig");
}

var edit = function (item) {
    $("#AddSettingWindow .parent").hide();
    $("#Parent").val("");
    if ($("#SettingTreeView_tv_active").find("input").first().val() == "ElectionDistrict_DistDB") {
        $("#Parent").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: {
                transport: {
                    read: "/Request/GetElection"
                },
            },
            value: item.Parent,
            optionLabel: "--Select Value--",
        }).data("kendoDropDownList");
        $("#AddSettingWindow .parent").show();
    }
    $("#AddSettingWindow").data("kendoWindow").center().title("Edit" + $("#SettingTreeView_tv_active :input").val()).open();
    $("#AddSettingWindow .windowConfirm").attr("data-url", "/Maintenance/Master/EditSettinig").attr("data-ID", item.Name);
    $("#DisplayName").val(item.Displayname);
    $("#Sort").val(item.Sort);
    $(":checkbox[name='Status']").prop("checked", item.Status);
}

var del = function () {
    var hidden = $("#SettingTreeView_tv_active :input").val();//$("#SettingTreeView_tv_active").find("input").first().prop("checked", true).val();  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
    if (!hidden) return;
    var nameList = [];
    var item = [];
    if (hidden == "ElectionDistrict_DistDB") {
        item = getKendoGrid("SettingGrid_Election").dataSource.get($("#SettingGrid_Election .k-grid-content").find(":checked").first().val());
        if (!item) return;
        $("#SettingGrid_Election .k-grid-content").find(":checked").each(function () {
            nameList.push(this.value);
        })
    }
    else {
        item = getKendoGrid("SettingGrid").dataSource.get($("#SettingGrid .k-grid-content").find(":checked").first().val());
        if (!item) return;
        $("#SettingGrid .k-grid-content").find(":checked").each(function () {
            nameList.push(this.value);
        })
    }

    $.ajax({
        url: "/Maintenance/Master/DeleteSettinig",
        type: "POST",
        data: { SettingsType: hidden, NameList: nameList },
        traditional: true,
        success: function (item) {
            for (var key in nameList) {
                if (hidden == "ElectionDistrict_DistDB")
                    getKendoGrid("SettingGrid_Election").dataSource.remove(getKendoGrid("SettingGrid_Election").dataSource.get(nameList[key]));
                else
                    getKendoGrid("SettingGrid").dataSource.remove(getKendoGrid("SettingGrid").dataSource.get(nameList[key]));
            }
        }
    })
}

var isnullcheckbox = function () {
    if (null == $("#SettingTreeView_tv_active").find(":checked").val()) return;
    $.post("/Maintenance/Master/ShowValueIsNull",
       {
           ID: $("#SettingTreeView_tv_active").find(":checked").val(),
           IsChecked: $(this).prop("checked")
       }, function (data) {
           getKendoGrid("SettingGrid").dataSource.data(data);
       });
}

var MastercontextMenu = function () {

    $('#SettingTreeView_tv_active .k-state-focused').WinContextMenu({
        //cancel: '.cancel',
        menu: "#MasterContextMenu",
        removeMenu: '#homeBody',
        action: function (e) {
            switch (e.id) {
                case "EditContextMenu":
                    //addsetting();
                    editsetting();
                    break;
                    //case "DeleteContextMenu": delsetting(); break;
            }
        }//自由设计项事件回调
    });
}


var SettingCancel = function () {
    $("#AddSettingWindow").data("kendoWindow").close();
}
var SettingConfirm = function () {
    var that = $(this);
    that.unbind("click", SettingConfirm);
    var hidden = $("#SettingTreeView_tv_active :input").val();  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
    var url = $("#AddSettingWindow .windowConfirm").attr("data-url");
    //======发送数据   parent数据清除    后台修改  添加+编辑
    $.post(url, {
        SettingsType: hidden,
        Name: $("#AddSettingWindow .windowConfirm").attr("data-ID"),
        DisplayName: $("#DisplayName").val(),
        Parent: $("#Parent").val(),
        Sort: $("#Sort").val(),
        Status: $(":checkbox[name='Status']").prop("checked"),
    }, function (item) {
        switch (url) {
            case "/Maintenance/Master/AddSettinig":
                if (hidden == "ElectionDistrict_DistDB")
                    getKendoGrid("SettingGrid_Election").dataSource.add(item);
                else
                    getKendoGrid("SettingGrid").dataSource.add(item);
                break;
            case "/Maintenance/Master/EditSettinig":
                var model = [];
                if (hidden == "ElectionDistrict_DistDB")
                    model = getKendoGrid("SettingGrid_Election").dataSource.get(item.Name);
                else
                    model = getKendoGrid("SettingGrid").dataSource.get(item.Name);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                    //model.set("Name", item.Name);
                    //model.set("Sort", item.Sort);
                    //model.set("Status", item.Status);
                    //model.set("Displayname", item.Displayname);
                }
                break;
        }
        $("#AddSettingWindow").data("kendoWindow").close();
    }).fail(function () {
        that.bind("click", SettingConfirm);
    })
}


var SettingsCancel = function () {
    $("#AddSettingsWindow").data("kendoWindow").close();
}
var SettingsConfirm = function () {
    var that = $(this);
    that.unbind("click", SettingsConfirm);
    $.post($("#AddSettingsWindow .windowConfirm").attr("data-url"), { Name: $("#AddSettingsWindow .k-textbox").val(), Id: $("#AddSettingsWindow .windowConfirm").attr("data-Id") }, function (item) {

        var treeview = $("#SettingTreeView").data("kendoTreeView");
        var model = treeview.dataSource.get(item.ID);
        if (model) {
            for (var key in item) {
                model.set(key, item[key]);
            }
        }
        //var temp = $("#SettingTreeView").data("kendoTreeView").dataSource.get(data.ParentID);//添加
        //temp.children.add(data);
        $("#AddSettingsWindow").data("kendoWindow").close()


    }).fail(function () {
        that.bind("click", SettingsConfirm);
    })
}

var InitSettingTreeView = function () {
    $("#SettingTreeView").kendoTreeView({
        template: kendo.template($("#SettingTreeView-template").html()),
        dataSource: Settings,
        dataTextField: "Name",
        collapse: function (e) {
            $("#SettingTreeView_tv_active").find(".k-sprite").first().removeClass("on");
        },
        expand: function (e) {
            $("#SettingTreeView_tv_active").find(".k-sprite").first().addClass("on");
        },
        select: function (e) {
            $("#SettingTreeView").find("input").prop("checked", false);
            //var hidden = $("#SettingTreeView_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
            //$.post("/Maintenance/Master/GetSetting", { ID: hidden.val() }, function (data) {
            //    InitGrid(data);
            //});
            InitGrid();
            MastercontextMenu();
        },
        dataBound: function (e) {
            //var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
            //$("#SettingTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
            var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
            var mousedown = function (e) { if (e.which == 3) $(this).click(); }
            $("#SettingTreeView").find(":checkbox").unbind(clickevent).bind(clickevent);
            $("#SettingTreeView").off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
        }
    });
}
var InitGrid = function () {
    var id = $("#SettingTreeView_tv_active").find("input").first().val();
    var hidden = $("#SettingTreeView_tv_active").find("input").first().prop("checked", true);
    if (id != "TradeMix" && id != "Nature" && id != "Reason" && id != "Attempt" && id != "AccreditationObtained" && id != "PostBackground" && id != "RecognitionAward" && id != "ElectionDistrict_LegCo" && id != "ElectionDistrict_DistDB") {
        $("#SettingsGrid").show();
        $("#SettingGrid").hide();
        $("#SettingGrid_Election").hide();
        $("#SettingGridAction").hide();
        InitBaseServerKendoGridWidthPage("SettingsGrid", Model,
        Column, "/Maintenance/Master/GetSetting", { ID: hidden.val() }, 15, function () { });
        //InitBaseKendoGridWidthPage("SettingsGrid", Model, Column, data, 15, function () { });
        return;
    }
    if (id == "ElectionDistrict_DistDB") {
        $("#SettingGrid").hide();
        $("#SettingGrid_Election").show();
        $("#SettingsGrid").hide();
        $("#SettingGridAction").show();
        $.post("/Maintenance/Master/GetSetting", { ID: hidden.val() }, function (data) {
            if (!getKendoGrid("SettingGrid_Election") && !getKendoGrid("SettingGrid")) {
                $("#SettingGridAction .Add").click(add);
                $("#SettingGridAction .Delete").click(del);
            }
            InitBaseKendoGridWidthPage("SettingGrid_Election", SettingModel, settingcolumnsWithParent, data, 15, function () {
                bindAndLoad("SettingGrid_Election");
                bindGridCheckbox("SettingGrid_Election");
            });
        });
    }
    else {
        $("#SettingGrid").show();
        $("#SettingGrid_Election").hide();
        $("#SettingsGrid").hide();
        $("#SettingGridAction").show();
        $.post("/Maintenance/Master/GetSetting", { ID: hidden.val() }, function (data) {
            if (!getKendoGrid("SettingGrid_Election") && !getKendoGrid("SettingGrid")) {
                $("#SettingGridAction .Add").click(add);
                $("#SettingGridAction .Delete").click(del);
            }
            InitBaseKendoGridWidthPage("SettingGrid", SettingModel, settingcolumns, data, 15, function () {
                bindAndLoad("SettingGrid");
                bindGridCheckbox("SettingGrid");
            });
        });
    }
}
var enableOrDisable = function () { // 更改状态
    $("#SettingGrid").on("click", ".glyphicon-ban-circle", function () {
        //$(this).removeClass("glyphicon-ban-circle");
        //$(this).addClass("glyphicon-ok");
        var name = $("#SettingGrid .k-grid-content").find(":checked").first().val();
        $.post("", { SettingsType: $("#SettingTreeView_tv_active :input").val(), Name: name, Status: true }, function () {
            var model = getKendoGrid("SettingGrid").dataSource.get(name);
            if (model) {
                model.set("Status", true);
            }
        })

    })
    $("#SettingGrid").on("click", ".glyphicon-ok", function () {
        //$(this).removeClass("glyphicon-ok");
        //$(this).addClass("glyphicon-ban-circle");
        var name = $("#SettingGrid .k-grid-content").find(":checked").first().val();
        $.post("", { SettingsType: $("#SettingTreeView_tv_active :input").val(), Name: name, Status: false }, function (item) {
            var model = getKendoGrid("SettingGrid").dataSource.get(item.Name);
            if (model) {
                model.set("Status", false);
            }
        })
    })
}

var InitAddSettingWindow = function () {
    $("#AddSettingWindow").kendoWindow({
        width: "380px",
        title: "Add Staff",
        actions: [
            "Close"
        ],
        open: function (e) {
            $("#AddSettingWindow .windowCancel").bind("click", SettingCancel);
            $("#AddSettingWindow .windowConfirm").bind("click", SettingConfirm);
        },
        close: function () {
            $("#AddSettingWindow").find("input").val("");
            $("#AddSettingWindow").find("textarea").val("");
            $("#AddSettingWindow .Name").attr("readonly", false);
            $("#AddSettingWindow .windowCancel").unbind("click", SettingCancel);
            $("#AddSettingWindow .windowConfirm").unbind("click", SettingConfirm);
        },
        resizable: false,
        modal: true
    });
    AddSplitters($("#AddSettingWindow").data("kendoWindow"));

}

var InitAddSettingsWindow = function () {
    $("#AddSettingsWindow").kendoWindow({
        width: "300px",
        title: "Add Staff",
        actions: [
            "Close"
        ],
        open: function (e) {
            $("#AddSettingsWindow .windowCancel").bind("click", SettingsCancel);
            $("#AddSettingsWindow .windowConfirm").bind("click", SettingsConfirm)
        },
        close: function () {
            $("#AddSettingsWindow").find("input").val("");
            $("#AddSettingsWindow .windowCancel").unbind("click", SettingsCancel);
            $("#AddSettingsWindow .windowConfirm").unbind("click", SettingsConfirm)
        },
        resizable: false,
        modal: true
    });
    AddSplitters($("#AddSettingsWindow").data("kendoWindow"));

}

var LoadMasterView = function () {
    title = "Leasing Management - Kendo UI";
    $("#SettingView").kendoSplitter({
        panes: [
            { collapsible: false, size: "250px", min: "250px", max: "300px", resizable: true },
            { collapsible: false, resizable: true }
        ]
    });
    AddSplitters($("#SettingView").data("kendoSplitter"));
    InitSettingTreeView();

    InitAddSettingWindow();
    InitAddSettingsWindow();
    //$("#SettingView .AddSetting").click(addsetting);
    //$("#SettingView .DeleteSetting").click(delsetting);

    $("#SettingView .Reset").click(reset);
    $("#SettingView .Check").click(check);
    $("input:checkbox[name= IsNull]").click(isnullcheckbox);
    $("#SettingView .Edit").click(edit);
}