var ControlSettingWindow = {};

ControlSettingWindow = {
    $formControlSettingContainer: $('div[role=ControlSettingWindow]'),
    koControlSettingModel: {}
    , MessageType: {
        Success: 'success',
        Warning: 'warning'
    }
    ,Message:{
        formInvalidDefaultMsg: jsResxMaintenance_SeaWFConfig.formInvalidDefaultMsg,
        formHandleSuccessMsg: jsResxMaintenance_SeaWFConfig.formHandleSuccessMsg,
        formInvalidUrlMsg: jsResxMaintenance_SeaWFConfig.formInvalidUrlMsg,
        formInvalidViewUrlMsg: jsResxMaintenance_SeaWFConfig.formInvalidViewUrlMsg,
        formInvalidApproveUrlMsg: jsResxMaintenance_SeaWFConfig.formInvalidApproveUrlMsg,
        formInvalidStartUrlMsg: jsResxMaintenance_SeaWFConfig.formInvalidStartUrlMsg
    }
    , initControlSettingModel: function () {
        KStarForm.applyBindings(this.koControlSettingModel, this.$formControlSettingContainer[0]);
    }
    , initkoControlSettingModel: function () {
        this.koControlSettingModel = KStarForm.toKoModel({
            SysId: "00000000-0000-0000-0000-000000000000",
            ActivityId: 0,
            WorkMode: 0,
            ControlRenderId: 0,
            ControlName: "",
            ControlType: 0,
            IsHide: false,
            IsDisable: false,
            IsCustom: false,
            RenderTemplate: ""
        });
    }
    , initCheckAndLabel: function () {
        $.each($("table.table-striped"), function (i, table) {
            var trs = $(this).find("tr");
            $.each(trs, function (j, tr) {
                var td0 = $(this).find("td").eq(0);
                var id = $(td0).find("span").eq(0).html();
                var chkdiv0 = $(this).find("td").eq(2).find("div.row div").eq(0);
                var chkdiv1 = $(this).find("td").eq(2).find("div.row div").eq(1);
                var chkdiv2 = $(this).find("td").eq(2).find("div.row div").eq(2);

                $(chkdiv0).find("input").attr("id", "show" + id);
                $(chkdiv0).find("label").attr("for", "show" + id);
                $(chkdiv1).find("input").attr("id", "disabled" + id);
                $(chkdiv1).find("label").attr("for", "disabled" + id);
                $(chkdiv2).find("input").attr("id", "custom" + id);
                $(chkdiv2).find("label").attr("for", "custom" + id);
            });
        });
    }
    , disabledControlWindowCheck: function (id, type, value)
    {
        switch (type) {
            case "show":
                $("#" + id + " .chkshow").attr("disabled", value);
                $("#show" + id).attr("disabled", false);                                
                break;
            case "disabled":
                $("#" + id + " .chkdisabled").attr("disabled", value);
                $("#disabled" + id).attr("disabled", false);
                break;
            case "custom":
                $("#" + id + " .chkcustom").attr("disabled", value);
                $("#custom" + id).attr("disabled", false);                
                break;
        }        
    }
    , bindCustomtreeClick: function (type) {        
        $("." + type).click(function () {           
            var checked = $(this).prop("checked");
            var dropdowntree = this;
            var parentrow = $(this).parent().parent().next();
            if (type == "chkcustomtree")
            {
                dropdowntree = $(this).parent().parent().find("input.dropdowntree");
            }
            else if (type == "chkcustomcontroltree") {
                dropdowntree = $(this).parent().parent().next().find("input.dropdowntree");
            }
            if (checked) {
                if (type == "chkcustomcontroltree") {parentrow.show();}
                dropdowntree.bind("click", function () {
                    $.fn.initDropdownTree.toggleMenu(this, $(this).attr("swid"));
                });
            }
            else {
                if (type == "chkcustomcontroltree") { parentrow.hide(); }
                dropdowntree.unbind("click");
            }
        });
    }
    , initDropdownTreeEvent: function (type) {
        $("." + type).each(function () {
            var that = $(this);
            var checked = that.prop("checked");
            var dropdowntree = {};
            if (type == "chkcustomcontroltree") {
                dropdowntree = that.parent().parent().next().find("input.dropdowntree");
            }
            else if (type == "chkcustomtree") {
                dropdowntree = that.parent().parent().find("input.dropdowntree");
            }       
            if (!checked) {
                dropdowntree.unbind("click");
            }
        });
    }
    , initDropdownTreeStatus: function (type)
    {
        $("." + type).each(function (i,item) {
            var checked = $(this).prop("checked");            
            var parentrow =this
            if (type == "chkcustomtree") {
                parentrow = $(this).parent().next();
            }
            else if (type == "chkcustomcontroltree") {
                parentrow = $(this).parent().parent().next();
            }
            if (checked) {
                parentrow.show();               
            }
            else {
                parentrow.hide();               
            }
        });
    }
    , updateKoModelForCheck: function (model, type, value) {
        var _componentModel = model.ComponentModel;
        var _controlModel = model.ControlModel;
        switch (type) {
            case "show":
                _componentModel.IsHide(value);
                $.each(_controlModel(), function (i, item) {
                    item.IsHide(value);
                });
                break;
            case "disabled":
                _componentModel.IsDisable(value);
                $.each(_controlModel(), function (i, item) {
                    item.IsDisable(value);
                });
                break;
            case "custom":                
                _componentModel.IsCustom(value);
                $.each(_controlModel(), function (i, item) {                    
                    $("#" + item.ComponentId().toString() + " tbody tr").eq(i).find("input.chkcustomcontroltree").click();
                });
                break;
        }
    }
    , clearControlSettingWindow: function () {
        var controlSettingModel = this.koControlSettingModel;
        controlSettingModel.SysId("00000000-0000-0000-0000-000000000000");
        controlSettingModel.ActivityId(0);
        controlSettingModel.WorkMode(0);
        controlSettingModel.ControlRenderId(0);
        controlSettingModel.ControlName("");
        controlSettingModel.ControlType(0);
        controlSettingModel.IsHide(false);
        controlSettingModel.IsDisable(false);
        controlSettingModel.IsCustom(false);
        controlSettingModel.RenderTemplate("");
    }
    , initButtons: function () {        
        $("#QuickSettings button.btn-primary").unbind("click").bind("click", function () {
            ControlSettingWindow.ControlSettingAction("SaveSettings");
        });

        $("#alldisabled").unbind("click").bind("click", function () {
            var checked = $(this).prop("checked");
            var components = $(".kstar-component");          
            $.each(components, function (i, item) {
                var model = eval($(item).attr("model"));
                ControlSettingWindow.updateKoModelForCheck(model, "disabled", checked);
            });          
            //$(".chkdisabled").attr("disabled", checked);
        });
        $("#allcustom").unbind("click").bind("click", function () {
            var checked = $(this).prop("checked");            
            var components = $(".kstar-component");
            $.each(components, function (i, item) {
                var model = eval($(item).attr("model"));
                ControlSettingWindow.updateKoModelForCheck(model, "custom", checked);                
            });               
            ControlSettingWindow.bindCustomtreeClick("chkcustomtree");

            //$(".chkcustom").attr("disabled", checked);
        });

        //$("#allshow").click(function () {
        //    var checked = $(this).prop("checked");
        //    ControlSettingWindow.updateKoModelForCheck(__kstarform_apply_componentkocomponentModel, "show", checked);
        //    ControlSettingWindow.updateKoModelForCheck(__kstarform_attachment_componentkocomponentModel, "show", checked);
        //    ControlSettingWindow.updateKoModelForCheck(__kstarform_toolbar_componentkocomponentModel, "show", checked);
        //    //$(".chkshow").attr("disabled", checked);
        //});
    }
    , isValid: function () {

        var is_forms_valid = false;

        $('form').each(function () {
            is_forms_valid = $(this).valid();
            if (!is_forms_valid) { return false; }
        });
        return is_forms_valid;
    }
    , EditControlSetting: function (sysId) {
        var selfControlSettingModel = this.koControlSettingModel;
        ko.dependentObservable(function () {
            $.getJSON($('#controlsettinggrid').attr("Singleurl"), { sysId: sysId }, function (userData) {
                selfControlSettingModel.SysId(userData.SysId);
                selfControlSettingModel.ActivityId(userData.ActivityId);
                selfControlSettingModel.WorkMode(userData.WorkMode);
                selfControlSettingModel.ControlRenderId(userData.ControlRenderId);
                selfControlSettingModel.ControlName(userData.ControlName);
                selfControlSettingModel.ControlType(userData.ControlType);
                selfControlSettingModel.IsHide(userData.IsHide);
                selfControlSettingModel.IsDisable(userData.IsDisable);
                selfControlSettingModel.IsCustom(userData.IsCustom);
                selfControlSettingModel.RenderTemplate(userData.RenderTemplate);
            });
        }, selfControlSettingModel);

        //KStarForm.applyBindings(selfControlSettingModel, this.$formControlSettingContainer[0]);

        $(".alert-danger").hide();
        $('#ControlSettingWindow').modal('show');
        $("#ControlSettingWindow  button.btn-primary").unbind("click").bind("click", function () {
            ControlSettingWindow.ControlSettingAction("EditControlSetting");
        });
    }
    , AlertWarning: function (type, message,time) {        
        var i_error = '<div  class="alert alert-' + type + ' alert-dismissible" role="alert">'
                         + '<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>'
                         + '<span>' + message + '</span></div>';

        $('#_controlSetting_error_placement').append($(i_error));        
        setTimeout(function () { $("#_controlSetting_error_placement span.sr-only").click(); }, (time==undefined?3000:time));
    }
    , ControlSettingAction: function (action) {
        ControlSettingWindow.DisableButton("saveControlSetting", true);
        var components = $(".kstar-component");
        var modelarr = new Array();
        $.each(components, function (i, item) {
            var model = eval($(item).attr("model"));
            modelarr.push(model.ComponentModel);
            $.each(model.ControlModel(), function (i, item) {
                modelarr.push(item);
            });
        });
        var settingModel = ko.toJS(modelarr);
        $postJSON($("#ControlSettingWindow").attr("requsetUrl") + action, JSON.stringify(settingModel), function (items) {           
            $.each(components, function (i, item) {                
                var model = eval($(item).attr("model"));
                ko.viewmodel.updateFromModel(model, items[$(item).attr("model")]);                
            });
            ControlSettingWindow.initCheckAndLabel();
            ControlSettingWindow.DisableButton("saveControlSetting", false);
            ControlSettingWindow.AlertWarning(ControlSettingWindow.MessageType.Success, ControlSettingWindow.Message.formHandleSuccessMsg);
        }, null);
    }
   , DisableButton: function (id, flag) {
       if (flag) {
           $('#' + id).attr('disabled', 'disabled');
       } else {
           $('#' + id).removeAttr('disabled');
       }
   }




}
