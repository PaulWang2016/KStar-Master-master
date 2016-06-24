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
        ScheduleID: that.attr("data-id"),
        DisplayName: $("#displayName").val(),
        IntervalPeriod: $("#intervalPeriod").val(),
        TargetName: $("#targetName").data("kendoDropDownList").text(),
        SourceName: $("#sourceName").data("kendoDropDownList").text(),
        CreateTime: $("#createTime").val(),
        NextRunTime: $("#nextRunTime").val(),
        TerminationTime: $("#terminationTime").val(),
        Status: $("#status").prop("checked")
    }
    $.post(url, data, function (item) {
        var grid = getKendoGrid("ScheduleConfigView");
        var model = grid.dataSource.get(item.ScheduleID);
        item.CreateTime = data.CreateTime;
        item.NextRunTime = data.NextRunTime;
        item.TerminationTime = data.TerminationTime;
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
    $("#AddScheduleWindow .k-textbox").val("");//清除输入框
    $("#createTime").data("kendoDatePicker").value(null);
    $("#nextRunTime").data("kendoDatePicker").value(null);
    $("#terminationTime").data("kendoDatePicker").value(null);

    $("#targetName").data("kendoDropDownList").select(0);
    $("#sourceName").data("kendoDropDownList").select(0);//清除下拉框

    $("#status").prop("checked", true);
}//重置表单
var CreateAddScheduleWindow = function () {
    $("#scheduleTab").children().kendoPanelBar();
    var AddScheduleWindow = $("#AddScheduleWindow").data("kendoWindow");
    if (!AddScheduleWindow) {
        $("#AddScheduleWindow").kendoWindow({
            width: "600px",
            title: "Add Schedule Config",
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
    $("#AddScheduleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Schedule/DoCreateSchedule").attr("data-id", 0);
    $("#createTime").data("kendoDatePicker").enable(false);
    $("#createTime").data("kendoDatePicker").value(new Date());
    $("#terminationTime").data("kendoDatePicker").value($("#terminationTime").data("kendoDatePicker").max());
    $("#AddScheduleWindow").data("kendoWindow").title("Add Schedule Config").center().open();
}

var EditSchedule = function (id) {
    CreateAddScheduleWindow();
    if (id) {
        var item = getKendoGrid("ScheduleConfigView").dataSource.get(id);
        $("#AddScheduleWindow").find(".windowConfirm").attr("data-url", "/Maintenance/Schedule/DoUpdateSchedule").attr("data-id", item.ScheduleID);

        $("#displayName").val(item.DisplayName);
        $("#intervalPeriod").val(item.IntervalPeriod);
        $("#targetName").data("kendoDropDownList").search(item.TargetName);
        $("#sourceName").data("kendoDropDownList").search(item.SourceName);
        $("#createTime").data("kendoDatePicker").value(item.CreateTime);
        $("#nextRunTime").data("kendoDatePicker").value(item.NextRunTime);
        $("#terminationTime").data("kendoDatePicker").value(item.TerminationTime);
        $("#status").prop("checked", item.Status);

        $("#AddScheduleWindow").data("kendoWindow").title("Edit Schedule Config").center().open();
    }
    else {
        ShowTip("Please select schedule!");
    }
}

var DelSche = function () {
    var idList = new Array();
    $("#ScheduleConfigView .k-grid-content").find(":checked").each(function () {
        idList.push(this.value)
    })
    if (idList.length > 0) {
        bootbox.confirm(getJSMsg("Base","Confirm"), function (result) {
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
        ShowTip("Please select schedule!", "error");
    }
}

function LoadScheduleView() {
    title = "Schedule Configuration - Kendo UI";
    $.getJSON("/Maintenance/Schedule/GetScheduleConfigList", { _t: new Date() }, function (items) {
        InitKendoExcelGrid("ScheduleConfigView", ScheduleConfigModel, scheduleconfigcolumns, items, 20, "ScheduleConfiguration", function () {//
            bindAndLoad("ScheduleConfigView");
            bindGridCheckbox("ScheduleConfigView");

            $("#ScheduleConfigView .k-toolbar")
                .append("<a id='ScheDel' class='k-button'><span class='glyphicon glyphicon-remove'></span></a>")
                .append("<a id='ScheAdd' class='k-button'><span class='glyphicon glyphicon-plus'></span></a>");

            $("#ScheAdd").click(AddSche);
            $("#ScheDel").click(DelSche);
        });
    })
    $("#ScheduleConfigView").prev().find(".selectbtn").click(function () {
        var name = $("#workList").prev().find(".name").val();
        var startDate = $("#workList").prev().find("input[name=StartDate]").val();
        var endDate = $("#workList").prev().find("input[name=EndDate]").val();
        $.post("/Maintenance/Schedule/FindScheduleConfigList", { name: name, StartDate: startDate, EndDate: endDate }, function (items) {
            InitKendoExcelGrid("ScheduleConfigView", ScheduleConfigModel, scheduleconfigcolumns, items, 20, "ScheduleConfiguration", function () {//
                bindAndLoad("ScheduleConfigView");
                bindGridCheckbox("ScheduleConfigView")
            });
        })
    })
    $.getJSON("/Maintenance/Schedule/GetTargetList", { _t: new Date() }, function (items) {
        $("#targetName").kendoDropDownList({
            dataTextField: "DisplayName",
            dataValueField: "TargetID",
            dataSource: {
                data: items,
                schema: {
                    model: {
                        id: "TargetID",
                        fields: {
                            TargetID: { type: "Number" },
                            DisplayName: { type: "String" }
                        }
                    }
                }
            },
            optionLabel: "--Select Target--"
        });
    })
    $.getJSON("/Maintenance/Schedule/GetSourceList", { _t: new Date() }, function (items) {
        $("#sourceName").kendoDropDownList({
            dataTextField: "DisplayName",
            dataValueField: "SourceID",
            dataSource: {
                data: items,
                schema: {
                    model: {
                        id: "SourceID",
                        fields: {
                            SourceID: { type: "Number" },
                            DisplayName: { type: "String" }
                        }
                    }
                }
            },
            optionLabel: "--Select Source--"
        });
    })
}