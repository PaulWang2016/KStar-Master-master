define(function (require, exports, module) {
    var Alltools = ["formatting", "bold", "italic", "underline", "strikethrough", "justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "insertUnorderedList", "insertOrderedList", "indent", "outdent", "createLink", "unlink", "insertImage", "createTable", "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow", "deleteColumn", "foreColor", "backColor", "viewHtml"]
    var EmailtpmlModel = kendo.data.Model.define({
        id: "TpmlID",
        fields: {
            TpmlID: { type: "string" },
            Process: { type: "string" },
            ProcessName: { type: "string" },
            SubjectInfo: { type: "string" },
            ContentInfo: { type: "string" },
            TpmlName: { type: "string" }
        }
    });

    var emailtmplcolumns = [
        {
            title: jsResxMaintenance_SeaEmailTpml.Checked, width: 35, template: function (item) {
                return "<input type='checkbox' value='" + item.TpmlID + "' />";
            }, headerTemplate: "<input type='checkbox' />", filterable: false
        },
    { field: "TpmlName", title: jsResxMaintenance_SeaEmailTpml.Name, filterable: false },
    { field: "ProcessName", title: jsResxMaintenance_SeaEmailTpml.Process, filterable: false },
    {
        field: "SubjectInfo", title: jsResxMaintenance_SeaEmailTpml.Subject, template: function (item) {
            return subHtml(item.SubjectInfo, 100);
        }, filterable: false

    },
    {
        field: "ContentInfo", title: jsResxMaintenance_SeaEmailTpml.Content, template: function (item) {
            return subHtml(item.ContentInfo, 100);
        }, filterable: false
    },
    { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditTemplate(data.TpmlID) } }], width: 58 }
    ]
    //===============================Email Tpml View====================================
    function resetAddTpmlWindow() {
        hideOperaMask("AddTpmlWindow");//清除Mask
        $("#AddTpmlWindow .k-textbox").val("");//清除输入框
        $("#tpmlSubInfo").val("");
        $("#tpmlConInfo").data("kendoEditor").value("");
        $("#tpmlProcess").data("kendoDropDownList").select(0);//清除下拉框
    }//重置表单
    var TpmlCancel = function () {
        $("#AddTpmlWindow").data("kendoWindow").close()
    }
    var TpmlConfirm = function () {
        var that = $(this);
        that.unbind("click", TpmlConfirm);
        showOperaMask("AddTpmlWindow");
        var url = that.attr("data-url");
        var data = {
            TpmlID: that.attr("data-id"),
            Process: $("#tpmlProcess").data("kendoDropDownList").value(),
            SubjectInfo: $("#tpmlSubInfo").val(),
            ContentInfo: $("#tpmlConInfo").data("kendoEditor").value(),
            TpmlName: $("#tpmlName").val()
        }
        $.ajax({
            url: url,
            type: "POST",
            data: data,
            traditional: true,
            success: function (item) {
                var grid = getKendoGrid("EmailTmplView");
                var model = grid.dataSource.get(item.TpmlID);
                if (model) {
                    for (var key in item) {
                        model.set(key, item[key]);
                    }
                }
                else {
                    grid.dataSource.add(item)
                }
                $("#AddTpmlWindow").data("kendoWindow").close();
            }
        }).fail(function () {
            that.bind("click", TpmlConfirm);
            hideOperaMask("AddTpmlWindow");
        })
    }
    //===============================Email Tpml View====================================
    var Addtmpl = function () {
        $("#AddTpmlWindow").find(".windowConfirm").attr("data-url", "/Maintenance/EmailTpml/DoCreateEmailTpml").attr("data-id", 0);
        $("#AddTpmlWindow").data("kendoWindow").title(jsResxMaintenance_SeaEmailTpml.AddEmailTemplate).center().open();
    }

    var EditTemplate = function (id) {
        if (id) {
            var item = getKendoGrid("EmailTmplView").dataSource.get(id);
            $("#tpmlProcess").data("kendoDropDownList").value(item.Process);
            $("#tpmlName").val(item.TpmlName);
            $("#tpmlSubInfo").val(item.SubjectInfo);
            $("#tpmlConInfo").data("kendoEditor").value(item.ContentInfo);
            $("#AddTpmlWindow").find(".windowConfirm").attr("data-url", "/Maintenance/EmailTpml/DoUpdateEmailTpml").attr("data-id", item.TpmlID);
            $("#AddTpmlWindow").data("kendoWindow").title(jsResxMaintenance_SeaEmailTpml.EditEmailTtemplates).center().open();
        }
        else {
            ShowTip(jsResxMaintenance_SeaEmailTpml.PsEt);
        }
    }

    var Deltmpl = function () {
        var idList = new Array();
        $("#EmailTmplView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaEmailTpml.Areyousure, function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/EmailTpml/DoDestroyEmailTpml",
                        type: "POST",
                        data: { idList: idList },
                        traditional: true,
                        success: function (ids) {
                            var grid = getKendoGrid("EmailTmplView");
                            for (var i = 0; i < ids.length; i++) {
                                var item = grid.dataSource.get(ids[i])
                                grid.dataSource.remove(item);
                            }
                        },
                        dataType: "json"
                    })
                }
            });
        }
        else {
            ShowTip(jsResxMaintenance_SeaEmailTpml.PsEte, "error");
        }
    }

    function LoadEmailtpmlView() {
        title = "Email Template - Kendo UI";
        $.getJSON("/Maintenance/EmailTpml/GetEmailtpmlList", { _t: new Date() }, function (items) {
            InitKendoExcelGridWithHeight("EmailTmplView", EmailtpmlModel, emailtmplcolumns, items, 20, jsResxMaintenance_SeaEmailTpml.EmailTemplateManagement, $(window).height() - fullwidgetH, function () {
                bindAndLoad("EmailTmplView");
                bindGridCheckbox("EmailTmplView");
                $("#EmailTmplView .k-toolbar")
                    .append("<a id='tmplDel' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                    .append("<a id='tmplAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");
                $("#tmplAdd").click(Addtmpl);
                $("#tmplDel").click(Deltmpl);
            });
        })
        $("#tpmlConInfo").kendoEditor({
            tools: Alltools
        });

        $("#AddTpmlWindow").kendoWindow({
            width: "800px",
            title: jsResxMaintenance_SeaEmailTpml.AddMailtemplates,
            actions: [
                "Close"
            ],
            open: function (e) {
                $("#AddTpmlWindow .windowCancel").bind("click", TpmlCancel)
                $("#AddTpmlWindow .windowConfirm").bind("click", TpmlConfirm)
            },
            close: function (e) {
                resetAddTpmlWindow();
                $("#AddTpmlWindow .windowCancel").unbind("click", TpmlCancel)
                $("#AddTpmlWindow .windowConfirm").unbind("click", TpmlConfirm)
            },
            resizable: false,
            modal: true
        });
        $.getJSON("/Maintenance/Process/Get", { _t: new Date() }, function (items) {//EmailTpml/GetProcessList
            $("#tpmlProcess").kendoDropDownList({
                dataTextField: "DisplayName",
                dataValueField: "FullName",
                dataSource: {
                    data: items,
                    schema: {
                        model: {
                            id: "FullName",
                            fields: {
                                FullName: { type: "String" },
                                Name: { type: "String" },
                                DisplayName: { type: "String" }
                            }
                        }
                    }
                },
                optionLabel: jsResxMaintenance_SeaEmailTpml.SelectProcess
            });
        }).fail(function () {
            $("#tpmlProcess").kendoDropDownList({
                dataTextField: "FullName",
                dataValueField: "FullName",
                dataSource: {
                    data: [{ "Folder": "AMS_Process", "Name": "AMS", "FullName": "AMS_Process\\AMS" }, { "Folder": "EmployeeFee", "Name": "EmployeeFee", "FullName": "EmployeeFee\\EmployeeFee" }],
                    schema: {
                        model: {
                            id: "FullName",
                            fields: {
                                FullName: { type: "String" },
                                Name: { type: "String" }
                            }
                        }
                    }
                },
                optionLabel: jsResxMaintenance_SeaEmailTpml.SelectProcess
            });
        })
    }
    module.exports = LoadEmailtpmlView;
})