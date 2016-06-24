define(function (require, exports, module) {
    var DateTimeFormat = require("/JS/DateTimeFormat");
    var format = DateTimeFormat();
    var intervalTypes = [{ Text: "By Minute", Value: "0" }, { Text: "By Hour", Value: "1" }, { Text: "Daily", Value: "2" }, { Text: "Weekly", Value: "3" }, { Text: "Monthly", Value: "4" }]
    var targetNames = [{ "TargetID": 1, "DisplayName": "FTP" }, { "TargetID": 2, "DisplayName": "Sql Server" }, { "TargetID": 3, "DisplayName": "Shared directory" }, { "TargetID": 4, "DisplayName": "Oracle" }];
    var sourceNames = [{ "SourceID": 1, "DisplayName": "FTP" }, { "SourceID": 2, "DisplayName": "Sql Server" }, { "SourceID": 3, "DisplayName": "Shared directory" }, { "SourceID": 4, "DisplayName": "Oracle" }];
    var intervalTypeDS = new kendo.data.DataSource({
        data: intervalTypes,
        schema: {
            model: {
                id: "Value",
                fields: {
                    Value: { type: "Number" },
                    Text: { type: "String" }
                }
            }
        }
    });
    var targetNameDS = new kendo.data.DataSource({
        data: targetNames,
        schema: {
            model: {
                id: "TargetID",
                fields: {
                    TargetID: { type: "Number" },
                    DisplayName: { type: "String" }
                }
            }
        }
    });
    var sourceNameDS = new kendo.data.DataSource({
        data: sourceNames,
        schema: {
            model: {
                id: "SourceID",
                fields: {
                    SourceID: { type: "Number" },
                    DisplayName: { type: "String" }
                }
            }
        }
    });


    var ScheduleConfigModel = kendo.data.Model.define({
        id: "TaskName",
        fields: {
            TaskName: { type: "string" },
            TypeName: { type: "string" },

            AssemblyName: { type: "string" },
            DateCreated: { type: "date" },
            Description: { type: "string" },
            LastRunTime: { type: "date" },
            PrivateBinPath: { type: "string" },
            RunCount: { type: "number" },
            Status: { type: "number" },

            NotificationReceiver: { type: "string" },
            OnError: { type: "boolean" },
            OnExec: { type: "boolean" },


            ExitOn: { type: "date" },
            Interval: { type: "number" },
            IntervalType: { type: "number" },
            StartTime: { type: "date" },

            TriggerDescription: { type: "string" },

            SystemName: { type: "string" },
            TargetName: { type: "string" },
            SourceName: { type: "string" }
        }
    });

    scheduleconfigcolumns = [
            {
                title: jsResxMaintenance_SeaSchedule.Checked, width: 35, template: function (item) {
                    return "<input type='checkbox' value='" + item.TaskName + "' />";
                }, headerTemplate: "<input type='checkbox' />", filterable: false
            },
        { field: "TaskName", title: jsResxMaintenance_SeaSchedule.TaskName, filterable: false },
        { field: "TypeName", title: jsResxMaintenance_SeaSchedule.TypePeriod, filterable: false },
        { field: "AssemblyName", title: jsResxMaintenance_SeaSchedule.AssemblyName, filterable: false },
        { field: "Description", title: jsResxMaintenance_SeaSchedule.Description, filterable: false },
        { field: "PrivateBinPath", title: jsResxMaintenance_SeaSchedule.PrivateBinPath, filterable: false },
        { field: "NotificationReceiver", title: jsResxMaintenance_SeaSchedule.NoticeReceiver, filterable: false },
        { field: "OnError", title: jsResxMaintenance_SeaSchedule.OnError, filterable: false, hidden: true },
        { field: "OnExec", title: jsResxMaintenance_SeaSchedule.OnExec, filterable: false, hidden: true },
        //{ field: "SystemName", title: "System Name", filterable: false, hidden: true },
        ////{
        ////    field: "TargetName", title: "Target Name", template: function (item) {
        ////        return targetNameDS.get(item.TargetName).DisplayName;
        ////    }, filterable: false, hidden: true
        ////},
        ////{
        ////    field: "SourceName", title: "Source Name", template: function (item) {
        ////        return sourceNameDS.get(item.SourceName).DisplayName;
        ////    }, filterable: false, hidden: true
        ////},
         { field: "Interval", title: jsResxMaintenance_SeaSchedule.Interval, filterable: false },
        {
            field: "IntervalType", title: jsResxMaintenance_SeaSchedule.IntervalType, template: function (item) {
                return intervalTypeDS.get(item.IntervalType).Text;
            }, filterable: false
        },
        { field: "DateCreated", title: jsResxMaintenance_SeaSchedule.DateCreated, format: format, filterable: false },
        { field: "LastRunTime", title: jsResxMaintenance_SeaSchedule.LastRunTime, format: format, filterable: false },
        { field: "ExitOn", title: jsResxMaintenance_SeaSchedule.ExitOn, format: format, filterable: false },
        { field: "TriggerDescription", title: jsResxMaintenance_SeaSchedule.TriggerDescription, filterable: false },
        {
            field: "Status", title: jsResxMaintenance_SeaSchedule.Status, width: 58, template: function (item) {
                return item.Status == 0 ? "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ok'></span></a>" : "<a href='javascript:void(0)' class='k-button'><span class='glyphicon glyphicon-ban-circle'></span></a>"
            }, filterable: false
        },
        { command: [{ name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-button-icontext k-grid-edit'><span class='glyphicon glyphicon-pencil'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); EditSchedule(data.TaskName) } }], width: 58 }
    ]
    //===============================Schedule Config View====================================
    var ScheduleCancel = function () {
        $("#AddScheduleWindow").data("kendoWindow").close()
    }
    var ScheduleConfirm = function () {
        var that = $(this);
        that.unbind("click", ScheduleConfirm);
        showOperaMask("AddScheduleWindow");
        var url = that.attr("data-url");
        var data = {
            TaskName: $("#taskName").val(),
            TypeName: $("#typeName").val(),

            AssemblyName: $("#assemblyName").val(),
            Description: $("#description").val(),
            PrivateBinPath: $("#privateBinPath").val(),

            NotificationReceiver: $("#notificationReceiver").val(),
            OnError: $("#onError").prop("checked"),
            OnExec: $("#onExec").prop("checked"),
            //Status: $("#status").data("kendoDropDownList").value(),

            ExitOn: $("#exitOn").val(),
            Interval: $("#interval").val(),
            IntervalType: $("#intervalType").data("kendoDropDownList").value(),
            StartTime: $("#startTime").val(),

            TriggerDescription: $("#triggerDescription").val(),

            //SystemName: $("#systemName").val(),
            //TargetName: $("#targetName").data("kendoDropDownList").value(),
            //SourceName: $("#sourceName").data("kendoDropDownList").value()
        }
        if (data.StartTime > data.ExitOn) {
            return false;
        }

        $.post(url, data, function (item) {
            var grid = getKendoGrid("ScheduleConfigView");
            var model = grid.dataSource.get(item.TaskName);
            item.StartTime = data.StartTime;
            item.ExitOn = data.ExitOn;
            if (model) {
                for (var key in item) {
                    model.set(key, item[key]);
                }
            }
            else {
                grid.dataSource.add(item)
            }
            $("#AddScheduleWindow").data("kendoWindow").close();
        }).fail(function () {
            that.bind("click", ScheduleConfirm);
            hideOperaMask("AddScheduleWindow");
        })
    }
    function resetAddScheduleWindow() {
        hideOperaMask("AddScheduleWindow");
        $("#taskName").prop("disabled", false);
        $("#AddScheduleWindow .k-textbox").val("");//清除输入框
        $("#startTime").data("kendoDatePicker").value(null);
        //$("#nextRunTime").data("kendoDatePicker").value(null);
        $("#exitOn").data("kendoDatePicker").value(null);

        $("#interval").data("kendoNumericTextBox").value(null);

        //$("#targetName").data("kendoDropDownList").select(0);
        //$("#sourceName").data("kendoDropDownList").select(0);
        $("#intervalType").data("kendoDropDownList").select(0);
        //$("#status").data("kendoDropDownList").select(0);//清除下拉框

        $("#status").prop("checked", true);
    }//重置表单
    var CreateAddScheduleWindow = function () {
        $("#scheduleTab").kendoPanelBar({ expandMode: "single" });
        var AddScheduleWindow = $("#AddScheduleWindow").data("kendoWindow");
        if (!AddScheduleWindow) {
            $("#AddScheduleWindow").kendoWindow({
                width: "600px",
                title: jsResxMaintenance_SeaSchedule.AddScheduleConfig,
                actions: [
                    "Close"
                ],
                open: function (e) {
                    $("#AddScheduleWindow .windowCancel").bind("click", ScheduleCancel)
                    $("#AddScheduleWindow .windowConfirm").bind("click", ScheduleConfirm)
                },
                close: function (e) {
                    resetAddScheduleWindow();
                    $("#AddScheduleWindow .windowCancel").unbind("click", ScheduleCancel)
                    $("#AddScheduleWindow .windowConfirm").unbind("click", ScheduleConfirm)
                },
                resizable: false,
                modal: true
            });
        }
    }
    //===============================Schedule Config View====================================
    var AddSche = function () {
        CreateAddScheduleWindow();
        $("#AddScheduleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Schedule/DoCreateSchedule");
        //$("#createTime").data("kendoDatePicker").enable(false);
        var _max = $("#exitOn").data("kendoDatePicker").max();
        $("#startTime").data("kendoDatePicker").value(new Date());
        $("#exitOn").data("kendoDatePicker").value(_max)
        $("#startTime").data("kendoDatePicker").max(_max);
        $("#exitOn").data("kendoDatePicker").min(new Date());
        $("#AddScheduleWindow").data("kendoWindow").title(jsResxMaintenance_SeaSchedule.AddScheduleConfig).center().open();
    }

    var EditSchedule = function (id) {
        CreateAddScheduleWindow();
        if (id) {
            var item = getKendoGrid("ScheduleConfigView").dataSource.get(id);
            $("#AddScheduleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Schedule/DoUpdateSchedule");

            //$("#displayName").val(item.DisplayName).attr("disabled", "disabled");
            //$("#intervalPeriod").val(item.IntervalPeriod);
            //$("#targetName").data("kendoDropDownList").search(item.TargetName);
            //$("#sourceName").data("kendoDropDownList").search(item.SourceName);
            //$("#createTime").data("kendoDatePicker").value(item.CreateTime);
            //$("#nextRunTime").data("kendoDatePicker").value(item.NextRunTime);
            //$("#terminationTime").data("kendoDatePicker").value(item.TerminationTime);
            //$("#status").prop("checked", item.Status);


            $("#taskName").val(item.TaskName).attr("disabled", "disabled");
            $("#typeName").val(item.TypeName);

            $("#assemblyName").val(item.AssemblyName);
            $("#description").val(item.Description);
            $("#privateBinPath").val(item.PrivateBinPath);
            $("#notificationReceiver").val(item.NotificationReceiver);
            $("#onError").prop("checked", item.OnError);
            $("#onExec").prop("checked", item.OnExec);
            //$("#status").data("kendoDropDownList").value(item.Status);

            $("#interval").data("kendoNumericTextBox").value(item.Interval);
            $("#intervalType").data("kendoDropDownList").value(item.IntervalType);
            $("#startTime").data("kendoDatePicker").value(item.StartTime);
            $("#exitOn").data("kendoDatePicker").value(item.ExitOn);
            $("#startTime").data("kendoDatePicker").max(item.ExitOn);
            $("#exitOn").data("kendoDatePicker").min(item.StartTime);

            $("#triggerDescription").val(item.TriggerDescription);

            //$("#systemName").val(item.SystemName);
            //$("#targetName").data("kendoDropDownList").value(item.TargetName);
            //$("#sourceName").data("kendoDropDownList").value(item.SourceName);

            $("#AddScheduleWindow").data("kendoWindow").title(jsResxMaintenance_SeaSchedule.EditScheduleConfig).center().open();
        }
        else {
            ShowTip(jsResxMaintenance_SeaSchedule.Pleaseselectschedule, "error");
        }
    }

    var DelSche = function () {
        var idList = new Array();
        $("#ScheduleConfigView .k-grid-content").find(":checked").each(function () {
            idList.push(this.value)
        })
        if (idList.length > 0) {
            bootbox.confirm(jsResxMaintenance_SeaSchedule.Areyousure, function (result) {
                if (result) {
                    $.ajax({
                        url: "/Maintenance/Schedule/DoDestroySchedule",
                        type: "POST",
                        data: { idList: idList },
                        traditional: true,
                        success: function (ids) {
                            var grid = getKendoGrid("ScheduleConfigView");
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
            ShowTip(jsResxMaintenance_SeaSchedule.Pleaseselectschedule, "error");
        }
    }
    var changeStatu = function () {
        $("#ScheduleConfigView").on("click", ".k-grid-content .glyphicon-ok", (function () {
            var taskName = $(this).parent().parent().parent().children().first().children().val();
            $.post("/Schedule/DisableTask", { taskName: taskName }, function () {
                var grid = getKendoGrid("ScheduleConfigView");
                var model = grid.dataSource.get(taskName);
                model.set("Status", "1");

            })
        }))
        $("#ScheduleConfigView").on("click", ".k-grid-content .glyphicon-ban-circle", (function () {
            var taskName = $(this).parent().parent().parent().children().first().children().val();
            $.post("/Schedule/EnableTask", { taskName: taskName }, function () {
                var grid = getKendoGrid("ScheduleConfigView");
                var model = grid.dataSource.get(taskName);
                model.set("Status", "0");
            })

        }))
    }
    function LoadScheduleView() {
        title = "Schedule Configuration - Kendo UI";
        $.getJSON("/Maintenance/Schedule/GetScheduleConfigList", { _t: new Date() }, function (items) {
            InitKendoExcelGrid("ScheduleConfigView", ScheduleConfigModel, scheduleconfigcolumns, items, 20, jsResxMaintenance_SeaSchedule.ScheduleConfiguration, function () {//
                bindAndLoad("ScheduleConfigView");
                bindGridCheckbox("ScheduleConfigView");

                $("#ScheduleConfigView .k-toolbar")
                    .append("<a id='ScheDel' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                    .append("<a id='ScheAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

                $("#ScheAdd").click(AddSche);
                $("#ScheDel").click(DelSche);
            });
            changeStatu();
        })
        $("#ScheduleConfigView").prev().find(".selectbtn").click(function () {
            var name = $("#workList").prev().find(".name").val();
            var startDate = $("#workList").prev().find("input[name=StartDate]").val();
            var endDate = $("#workList").prev().find("input[name=EndDate]").val();
            $.post("/Maintenance/Schedule/FindScheduleConfigList", { name: name, StartDate: startDate, EndDate: endDate }, function (items) {
                InitKendoExcelGrid("ScheduleConfigView", ScheduleConfigModel, scheduleconfigcolumns, items, 20, jsResxMaintenance_SeaSchedule.ScheduleConfiguration, function () {//
                    bindAndLoad("ScheduleConfigView");
                    bindGridCheckbox("ScheduleConfigView")
                });
                changeStatu();
            })
        })
        $("#interval").kendoNumericTextBox({
            spinners: false
        });
        //$("#targetName").kendoDropDownList({
        //    dataTextField: "DisplayName",
        //    dataValueField: "TargetID",
        //    dataSource: targetNameDS,
        //    optionLabel: "--Select Target--"
        //});
        //$("#sourceName").kendoDropDownList({
        //    dataTextField: "DisplayName",
        //    dataValueField: "SourceID",
        //    dataSource: sourceNameDS,
        //    optionLabel: "--Select Source--"
        //});
        $("#intervalType").kendoDropDownList({
            dataTextField: "Text",
            dataValueField: "Value",
            dataSource: intervalTypeDS,
            optionLabel: jsResxMaintenance_SeaSchedule.SelectIntervalType
        });
        //$("#status").kendoDropDownList({
        //    dataTextField: "Text",
        //    dataValueField: "Value",
        //    dataSource: {
        //        data: [{ Text: "Running", Value: "0" }, { Text: "Disabled", Value: "1" }, { Text: "Ready", Value: "2" }]
        //    },
        //    optionLabel: "--Select Status--"
        //});

        changeDatePicker();
    }
    module.exports = LoadScheduleView;

    var changeDatePicker = function () {
        $("#startTime").kendoDatePicker({
            change: function () {
                var value = this.value();
                $("#exitOn").data("kendoDatePicker").min(value);
            }
        });
        $("#exitOn").kendoDatePicker({
            change: function () {
                var value = this.value();
                $("#startTime").data("kendoDatePicker").max(value);
            }
        }).data("kendoDatePicker").min(new Date());
    }
})