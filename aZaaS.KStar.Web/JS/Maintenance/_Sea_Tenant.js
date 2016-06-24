define(function (require, exports, module) {
    //===================================column and model===========================
    var Tenantcolumns = [
     {
         title: jsResxMaintenance_SeaTenant.Checked, width: 35, template: function (item) {
             return "<input value='" + item.TenantID + "' type='checkbox' />";
         }, headerTemplate: "<input type='checkbox' />", filterable: false

     },
     { field: "TenantID", title: jsResxMaintenance_SeaTenant.TenantID, filterable: false },
     { field: "TenantName", title: jsResxMaintenance_SeaTenant.TenantName, filterable: false },
     { field: "ContactPerson", title: jsResxMaintenance_SeaTenant.ContactPerson, filterable: false },
     { field: "ContactMobile", title: jsResxMaintenance_SeaTenant.ContactMobile, filterable: false },
     { field: "ContactPosition", title: jsResxMaintenance_SeaTenant.ContactPosition, filterable: false },
     { field: "ExpireDate", title: jsResxMaintenance_SeaTenant.ExpireDate, filterable: false, format: getDateTimeFormat() },
     {
         field: "Status", title: jsResxMaintenance_SeaTenant.Status, width: 68, template: function (item) {
             return item.Status ? "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.TenantID + "'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.TenantID + "'><span class='glyphicon glyphicon-ban-circle'></span></a>"
         }, filterable: false
     }
    ]

    var Tenantmodel = kendo.data.Model.define({
        id: "TenantId",
        fields: {
            TenantID: { type: "string" },
            TenantName: { type: "string" },
            ContactPerson: { type: "string" },
            ContactMobile: { type: "string" },
            ContactPosition: { type: "string" },
            ExpireDate: { type: "date" },
            Status: { type: "boolean" }
        }
    });

    //===============================Tenant View====================================
    function resetAddTenantWindow() {
        hideOperaMask("AddTenantWindow");
    }//重置表单

    var TenantCancel = function () {
        $("#AddTenantWindow").data("kendoWindow").close()
    }
    var TenantConfirm = function () {
        var validator = $("#AddTenantWindow").data("kendoValidator");
        if (validator.validate()) { 
            var that = $(this);
            that.unbind("click", TenantConfirm);
            showOperaMask("AddTenantWindow");
            var url = $(this).attr("data-url");
            var chkExpireDate = $("#chkExpireDate").prop("checked");
            var date = $("#ExpireDate").data("kendoDatePicker").value();
            if (chkExpireDate && date.length == 0) {
                ShowTip(jsResxMaintenance_SeaTenant.Pleasefillexpiredateerror);
                return false;
            }
            var monthnum = $("#ddlExpireDate").data("kendoDropDownList").value();
            var mydate = new Date();
            if (monthnum != undefined && monthnum != null && monthnum.length > 0) {
                mydate.setMonth(mydate.getMonth() + parseInt(monthnum));
            }
            
            var expireDate = (chkExpireDate ? date : mydate);
            console.log(expireDate);
            var data = {
                tenantId: $("#TenantID").val(),
                tenantName: $("#TenantName").val(),
                expireDate: expireDate.format("yyyy-MM-dd")
            }

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                traditional: true,
                success: function (item) {
                    TenantManaViewDataRead();
                    $("#AddTenantWindow").data("kendoWindow").close()
                },
                dataType: "json"
            }).fail(function () {
                hideOperaMask("AddTenantWindow");
                that.bind("click", TenantConfirm);
            })
        }
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

    var OpenAddTenant = function (e) {
        $("#AddTenantWindow .windowCancel").bind("click", TenantCancel)
        $("#AddTenantWindow .windowConfirm").bind("click", TenantConfirm)
    }
    var CloseAddTenant = function (e) {
        resetAddTenantWindow();
        $("#AddStaffWindow .windowCancel").unbind("click", TenantCancel);
        $("#AddStaffWindow .windowConfirm").unbind("click", TenantConfirm);
    }


    //===============================Tenant View====================================
    var InitTenantWindow = function () {
        $("#AddTenantWindow").kendoWindow({
            width: 780,
            height: 200,
            title: jsResxMaintenance_SeaTenant.AddTenant,
            actions: [
                "Close"
            ],
            open: OpenAddTenant,
            close: CloseAddTenant,
            resizable: false,
            modal: true
        });
        window.AddSplitters($("#AddTenantWindow").data("kendoWindow"));
    }
    var InitTenantWindowView = function () {
        //===============================DropDownList====================================
        $("#ddlExpireDate").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: jsResxMaintenance_SeaTenant.OneMonth, value: "1" }, { text: jsResxMaintenance_SeaTenant.TwoMonth, value: "2" }
            , { text: jsResxMaintenance_SeaTenant.ThirdMonth, value: "3" }
            , { text: jsResxMaintenance_SeaTenant.SixMonth, value: "6" }
            , { text: jsResxMaintenance_SeaTenant.TwelveMonth, value: "12" }
            , { text: jsResxMaintenance_SeaTenant.TwentyfourMonth, value: "24" }
            , { text: jsResxMaintenance_SeaTenant.ThirtysixMonth, value: "36" }
            ],
            optionLabel: jsResxMaintenance_SeaTenant.SelectExpireDate
        });
        $("#ExpireDate").kendoDatePicker();


        var curdate = new Date();
        curdate.setDate(curdate.getDate() + 1);
        $("#ExpireDate").data("kendoDatePicker").min(curdate);
        //===============================DropDownList====================================

        $("#chkExpireDate").click(function () {
            var flag = $(this).prop("checked");
            if (flag) {
                $("#ddlExpireDate").data("kendoDropDownList").value("");
                $("#ddlExpireDate").data("kendoDropDownList").enable(false);
                $("#ExpireDate").data("kendoDatePicker").enable();
            }
            else {
                $("#ExpireDate").data("kendoDatePicker").value("");
                $("#ExpireDate").data("kendoDatePicker").enable(false);
                $("#ddlExpireDate").data("kendoDropDownList").enable();
            }
        });

        $("#ddlExpireDate").data("kendoDropDownList").enable();
        $("#ExpireDate").data("kendoDatePicker").enable(false);
    }
    var AddTenant = function () {
        $("#AddTenantWindow .windowConfirm").attr("data-url", "/Maintenance/Tenant/AddTenant");
        $("#AddTenantWindow").data("kendoWindow").center().title(jsResxMaintenance_SeaTenant.AddTenant).open();
        $("#AddTenantWindow").data("kendoValidator").hideMessages();
    }

    var DisableTenant = function () {
        var idList = new Array();
        $("#TenantManaView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            confirmDisableTenant(idList);
        }
        else {
            ShowTip(jsResxMaintenance_SeaTenant.PleaseselectTenanterror);
        }
    }

    var confirmDisableTenant = function (idList) {
        bootbox.confirm(jsResxMaintenance_SeaTenant.Areyousuredisabletenant, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Tenant/DoDisableTenant",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            TenantManaViewDataRead();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }

    var ActiveTenant = function (idList) {
        bootbox.confirm(jsResxMaintenance_SeaTenant.Areyousureactivetenant, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/Tenant/DoActiveTenant",
                    type: "POST",
                    data: { idList: idList },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            TenantManaViewDataRead();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }

    var TenantManaViewDataRead=function()
    {
        $.post("/Maintenance/Tenant/GetTenants", function (items) {
            var dataSource = new kendo.data.DataSource({
                data: items,
                schema: {
                    model: Tenantmodel
                },
                pageSize: 10
            });
            $("#TenantManaView").data("kendoExcelGrid").setDataSource(dataSource);
        });
    }

    function LoadTenantView() {
        title = "Tenant Management - Kendo UI";
        $.post("/Maintenance/Tenant/GetTenants",function (items) {
          InitKendoExcelGrid("TenantManaView", Tenantmodel, Tenantcolumns, items, 10, jsResxMaintenance_SeaTenant.TenantManagement, function () {
               bindAndLoad("TenantManaView");
               bindGridCheckbox("TenantManaView");
               $("#TenantManaView .k-toolbar").append(" <a id='TenantAdd'  class='more k-button'  href='javascript:void(0)'><span class='glyphicon glyphicon-plus'></span></a>");
               $("#TenantAdd").click(AddTenant);
           });
         });

        $("#TenantManaView").delegate("a.k-Status", "click", function () {
            var ok = $(this).find("span.glyphicon-ok");
            var idList = new Array();
            var id = $(this).attr("id");
            idList.push(id);
            if (ok.length == 1) {
                confirmDisableTenant(idList);
            }
            else {
                ActiveTenant(idList);
            }
        })

        InitTenantWindow();
        InitTenantWindowView();
    }
    module.exports = LoadTenantView;
})