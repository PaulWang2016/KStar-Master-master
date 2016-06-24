define(function (require, exports, module) {
    var pane;
    var ProcessModel = kendo.data.Model.define({
        id: "Name",
        fields: {
            Name: { type: "string" },
            Statu: { type: "boolean" },
            //Folder: { type: "string" },
            //FullName: { type: "string" }
        }
    });
    var processColumns = [
        {
            title: "Checked", width: 35, template: function (item) {
                return "<input value='" + item.Name + "' " + (item.Statu == true ? "checked='true' " : "") + " type='checkbox' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    { field: "DisplayName", title: "Process Name" },
    ];

    var InitDelegateGrid = function () {

        $.getJSON("/Maintenance/Applications/GetProcess", { _t: new Date(), pane: pane }, function (items) {

            InitBaseKendoGridWidthPage("DelegateList", ProcessModel, processColumns, items, 10, function () {
                bindGridCheckbox("DelegateList");
            });
        })
    }
    var DelegateGrid = new Array();

    var DelegateSave = function () {
        var that = $(this);
        that.unbind("click", DelegateSave);
        showOperaMask("DelegateTab");

        var items = new Array();
        $("#DelegateList .k-grid-content").find(":checkbox").each(function () {
            var item = $("#DelegateList").data("kendoGrid").dataSource.get(this.value);                                           
            items.push("{ ID: '" + encodeURI(this.value) + "', Status: '" + ($(this).prop("checked")? "true" : "false") + "' }");            
        });

        //var DelegateGrids = DelegateGrid["DelegateGrid"];
        //var items = new Array();
        //for (var key in DelegateGrids) {
        //    items.push("{ ID: '" + encodeURI(key) + "', Status: '" + (DelegateGrids[key] == true ? "true" : "false") + "' }");
        //}

        var data = {
            pane: pane,
            Items: items
        }
        $.ajax({
            url: "/Maintenance/Applications/SaveDelegateByPane",
            type: "POST",
            data: data,
            traditional: true,
            success: function (data) {
                if (data) {
                    $("#Delegate_Save").siblings(".tips").css("visibility", "visible");
                }
                hideOperaMask("DelegateTab");
                that.bind("click", DelegateSave);

            }
        }).fail(function () {
            that.bind("click", DelegateSave);
            hideOperaMask("DelegateTab");
        })
    }

    //var selectGrid = function () {
    //    if ($(this).val() != "on") {
    //        var items = DelegateGrid["DelegateGrid"];
    //        if (!items) {
    //            items = {};
    //            DelegateGrid["DelegateGrid"] = items;
    //        }
    //        var Id = $(this).val();
    //        if (items[Id]) {
    //            items[Id] = undefined;
    //        }
    //        else {
    //            items[Id] = $(this).prop("checked");
    //            var deleGrid = $("#DelegateList").data("kendoGrid");
    //            deleGrid.dataSource.get(Id).set("Statu", items[Id]);

    //        }
    //    }
    //    else {
    //        $("#DelegateList .k-grid-content").find(":checkbox").each(function () {
    //            var items = DelegateGrid["DelegateGrid"];
    //            if (!items) {
    //                items = {};
    //                DelegateGrid["DelegateGrid"] = items;
    //            }
    //            var Id = $(this).val();
    //            items[Id] = $(this).prop("checked");
    //            var deleGrid = $("#DelegateList").data("kendoGrid");
    //            deleGrid.dataSource.get(Id).set("Statu", items[Id]);

    //        });
    //    }
    //}
    var LoadDelegationManagement = function () {
        InitDelegateGrid();
        //$("#DelegateList").on("click", ":checkbox", selectGrid);
        $("#Delegate_Save").click(DelegateSave);
    }

    var DelegationManagement = function (p) {
        pane = p;
    }
    DelegationManagement.prototype.init = LoadDelegationManagement;
    module.exports = DelegationManagement;
})