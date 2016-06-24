define(function (require, exports, module) {
    var credits = {
        text: "",
        href: ""
    };
    function LoadWorkflowReport2View() {
        $("#txtStartDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#txtFinishDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#topOrbottom").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [{ text: '最短Top10', value: 0 }, { text: '最长Top10', value: 1 }],
            index: 0 // 当前默认选中项，索引从0开始。
        });
        $("#btnQuery").click(function () {
            InitReportChart();
        });
        $("#btnReset").bind("click", function () {
            $("#txtStartDate").val("");
            $("#txtFinishDate").val("");
            $("#ProcessCategory").data("kendoDropDownList").select(0);
            $("#topOrbottom").data("kendoDropDownList").select(0);
            $("#stProcessName").data("kendoMultiSelect").value([0]);
            loadProcActddl(-1);
            InitReportChart();
        });
        //获取分类
        $.ajax({
            url: "/Report/Home/GetAllCategoryList", async: true, dataType: "json", success: function (items) {
                InitDropDownList("ProcessCategory", "Name", "ID", "--全部分类--", items);
            }
        });
        //$.ajax({
        //    url: "/Dashboard/ProcessSupervise/GetProcess", async: true, dataType: "json", success: function (items) {
        //        InitDropDownList("stProcessName", "ProcessName", "ProcSetID", "--全部流程--", items);
        //    }
        //});

        $("#stProcessName").kendoMultiSelect({
            dataTextField: "ProcessName",
            dataValueField: "ProcSetID",
            width: 300,
            dataSource: {
                transport: {
                    read: {
                        //以下其实就是$.ajax的参数
                        type: "POST",
                        url: "/Dashboard/MyProcessInstance/GetProcess",
                        dataType: "json"
                    }
                }
            },
            dataBound: function () {
                $("#stProcessName").parent().css("display", "inline-block");
            }, change: function () {
                var procSetID = getStProcessNameValue();
                loadProcActddl(procSetID);
            }
        });
    }

    var loadProcActddl = function (procSetID) {
        $.ajax({
            url: "/Report/ProcessApprovalConsumingSecond/getProcActList", data: { _procSetID: procSetID }, async: true, dataType: "json", success: function (items) {
                InitDropDownList("stProcessAct", "ActivityName", "ActID", "--全部环节--", items);
            }
        });
    }

    var loadProcInstActInfo = function (procinstid) {
        $.getJSON("/Report/ProcessApprovalConsumingSecond/GetProcessActConsumingSecondData", { _procIntID: procinstid }, function (Data) {
            var categories = [], seriesData = [], i, Folio="";
            if (Data.length > 0)
                Folio = Data[0].Folio;
            for (i in Data) {
                categories.push(Data[i].ActivityName);
                seriesData.push({ y: Data[i].CasumeHour, CasumeSecondFomatStr: Data[i].CasumeSecondFomatStr, procInstID: Data[i].ProcInstID });
            }
            $('#Items').highcharts({
                credits: credits,
                title: {
                    text: Folio
                },
                yAxis: {
                    labels: {
                        format: '{value} 时'
                    },
                    title:{
                        text:'处理时长（小时）'
                    }
                },
                xAxis: {
                    name:Folio,
                    categories: categories,
                    labels:{
                        rotation: -45
                    }
                    
                },
                tooltip: {
                    useHTML: true,
                    headerFormat: '<small>{point.key}</small><table>',
                    pointFormat: '<tr><td style="color: {series.color}">{series.name}: </td>' +
                        '<td style="text-align: right"><b>{point.CasumeSecondFomatStr}</b></td></tr>',
                    footerFormat: '</table>',
                    valueDecimals: 2
                },
                series: [{
                    name: '环节名称',
                    data: seriesData
                }]
            });
        });
    }

    function getStProcessNameValue() {
        var multiSelect = $("#stProcessName").data("kendoMultiSelect")
        var pn = '';
        var pName = multiSelect.value();
        if (pName != undefined && pName != null) {
            for (var i = 0; i < pName.length; i++) {
                pn += pName[i] + ",";
            }
        }
        if (pName.length > 1) {
            return -1;
        }
        else
            return pn.substr(0,pn.length-1);
    }

    function InitReportChart() {
        var procSetIDs = getStProcessNameValue();
        if (procSetIDs == -1) {
            alert("一次只能选择一个流程");
            return;
        }
        var ajaxParams = {

            _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
            _topOrbottom: $("#topOrbottom").val(), _procSetID: getStProcessNameValue(), _actId: $("#stProcessAct").val()
        };
        $.ajax({
            type: 'get',
            url: '/Report/ProcessApprovalConsumingSecond/GetProcessApprovalConsumingSecondData',
            data: ajaxParams,
            cache: false,
            dataType: 'json',
            success: function (Data) {
                var categories = [], seriesData = [], obj, chartTitle;
                var ddlProcess = "";//getStProcessNameValue();
                var ddlAct = $("#stProcessAct").val() > 0?$("#stProcessAct").data("kendoDropDownList").text():'';

                if (ajaxParams._topOrbottom == "0") {
                    chartTitle = ddlProcess + ddlAct + "处理时长最短Top10";
                } else {
                    chartTitle = ddlProcess + ddlAct + "处理时长最短Top10";
                }
                for(obj in Data){
                    categories.push(Data[obj].YFProcInstID);
                    seriesData.push({ y: Data[obj].CasumeHour, CasumeSecondFomatStr: Data[obj].CasumeSecondFomatStr, procInstID: Data[obj].ProcInstID, YFProcInstID: Data[obj].YFProcInstID });

                }
                var chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'container1',
                        type: 'column',
                        margin: 75,
                        options3d: {
                            enabled: false,
                            alpha: $('#R0').val(),
                            beta: $('#R1').val(),
                            depth: 50,
                            viewDistance: 25
                        }
                    },
                    credits: credits,
                    tooltip: {
                        useHTML: true,
                        headerFormat: '<small>{point.key}</small><table>',
                        pointFormat: '<tr><td style="color: {series.color}">{series.name}: </td>' +
                            '<td style="text-align: right"><b>{point.CasumeSecondFomatStr}</b></td></tr>',
                        footerFormat: '</table>',
                        valueDecimals: 2
                    },
                    yAxis: {
                        labels: {
                            format: '{value} 时'
                        },
                        title: {
                            text: '处理时长（小时）'
                            ,offset: 60
                        }
                    },
                    xAxis: {
                        categories:categories

                    },
                    title: {
                        text: chartTitle
                    },
                    subtitle: {
                        text: '点击查看环节处理耗时'
                    },
                    plotOptions: {
                        column: {
                            depth: 25
                        },
                        series: {
                            cursor: 'pointer',
                            point: {
                                events: {
                                    click: function () {
                                        var window = $("#window");
                                        var procInstID = this.procInstID;
                                        var YFProcInstID = this.YFProcInstID;
                                        window.kendoWindow({
                                            width: "750px",
                                            height: "500px",
                                            title: "实例编号：" + YFProcInstID,
                                            actions: ["Pin", "Maximize", "Close"],
                                            open: function () {
                                                $(".k-window-title").html("实例编号：" + YFProcInstID);
                                                loadProcInstActInfo(procInstID);
                                            },
                                        });                           
                                        $("#window").data("kendoWindow").center().open();
                                    }
                                }
                            }
                        }
                    },
                    series: [{
                        name: '流程时长',
                        data: seriesData
                    }]
                });


                // Activate the sliders
                $('#R0').off('change').on('change', function () {
                    chart.options.chart.options3d.alpha = this.value;
                    showValues();
                    chart.redraw(false);
                });
                $('#R1').off('change').on('change', function () {
                    chart.options.chart.options3d.beta = this.value;
                    showValues();
                    chart.redraw(false);
                });

                function showValues() {
                    $('#R0-value').html(chart.options.chart.options3d.alpha);
                    $('#R1-value').html(chart.options.chart.options3d.beta);
                }
                //showValues();
            }
        });
    }
    module.exports = LoadWorkflowReport2View;
})