define(function (require, exports, module) {
    var procInstModel = kendo.data.Model.define({
        id: "Report2ID",
        fields: {
            ProcessFullname: { type: "string" },
            DisplayName: { type: "string" },
            Avg_Consuming_Second: { type: "string" },
            ProcInst_Count: { type: "string" }
        }
    });

    var ProcessFullname = "";

    var procInstColumns = [

        { field: "DisplayName", title: jsResxReport_SeaWorkflowReport2.ProcessFullname },
        { field: "Avg_Consuming_Second", title: jsResxReport_SeaWorkflowReport2.Avg_Consuming_Second, filterable: false },
        { field: "ProcInst_Count", title: jsResxReport_SeaWorkflowReport2.ProcInst_Count, filterable: false }
    ]

    var actInstModel = kendo.data.Model.define({
        id: "Report1ItemID",
        fields: {
            ActivityName: { type: "string" },
            DisplayName: { type: "string" },
            TotalCount: { type: "string" },
            RunningCount: { type: "string" },
            ExpiredCount: { type: "string" },
            CompletedCount: { type: "string" }
        }
    });

    var actInstColumns = [
        { field: "ActivityName", title: jsResxReport_SeaWorkflowReport2.ActivityName, filterable: false },
        { field: "Avg_Consuming_Second", title: jsResxReport_SeaWorkflowReport2.Avg_Consuming_Second, filterable: false },
        { field: "Percentage", title: jsResxReport_SeaWorkflowReport2.Percentage, width: 150, filterable: false, format: "{0}%" }
    ];

    var actInstSlotModel = kendo.data.Model.define({
        id: "actInstSlotItemID",
        fields: {
            ActivityName: { type: "string" },
            TotalCount: { type: "string" },
            RunningCount: { type: "string" },
            ExpiredCount: { type: "string" },
            CompletedCount: { type: "string" }
        }
    });

    var actInstSlotColumns = [
        { field: "User", title: jsResxReport_SeaWorkflowReport2.User, filterable: false },
        { field: "Avg_Consuming_Second", title: jsResxReport_SeaWorkflowReport2.Avg_Consuming_Second, filterable: false },
        { field: "Percentage", title: jsResxReport_SeaWorkflowReport2.Percentage, width: 150, filterable: false, format: "{0}%" }
    ];

    //审批绩效阀值，todo : 需要从数据库中获取
    var consumingThreshold = {
        Process: 600000,
        Activity: 1,
        User: 1
    };

    ////根据审批绩效阀值，设置Grid中的字体颜色
    function thresholdChangeColor(kendoGridDivObj, gridLevel) {

        var grid = kendoGridDivObj.data("kendoGrid");

        var rows = grid.tbody.find("tr").each(function () {
            var tr = $(this);
            if (grid.dataItem(tr).Avg_Consuming_Second_Value > consumingThreshold[gridLevel])
                tr.css("color", "red");
        });

    }
    var colorAry = ["#9de219", "#90cc38", "#068c35", "#006634", "#004d38", "#033939"];

    function setColor(chartData) {

        $.each(chartData, function (index) {
            chartData[index].color = colorAry[index % colorAry.length];

        });

        return chartData;

    }

    function loadProcessKendoChart(containerDiv, title, chartData) {

        chartData = setColor(chartData);

        $("<div/> ").appendTo(containerDiv).kendoChart({
            title: {
                position: "bottom",
                text: title
            },
            legend: {
                visible: false
            },
            chartArea: {
                background: ""
            },
            seriesDefaults: {
                labels: {
                    visible: true,
                    background: "transparent",
                    template: "#= category #: #= value#%"
                }
            },
            series: [{
                type: "pie",
                startAngle: 150,
                data: chartData
            }],
            tooltip: {
                visible: true,
                format: "{0}%"
            }
        });


    };

    function loadActInstKendoChart(containerDiv, title, chartData) {
        if (chartData.length <= 1) return;

        chartData = setColor(chartData);

        $("<div />").appendTo(containerDiv).kendoChart({
            title: {
                position: "bottom",
                text: title
            },
            legend: {
                visible: false
            },
            chartArea: {
                background: ""
            },
            seriesDefaults: {
                labels: {
                    visible: true,
                    background: "transparent",
                    template: "#= category #: #= value#%"
                }
            },
            series: [{
                type: "pie",
                startAngle: 150,
                data: chartData
            }],
            tooltip: {
                visible: true,
                format: "{0}%"
            }
        });


    };


    function loadActInstSlotKendoChart(containerDiv, title, chartData) {
        return;

        $(" <div /> ").appendTo(containerDiv).kendoChart({
            title: {
                position: "bottom",
                text: title
            },
            legend: {
                visible: false
            },
            chartArea: {
                background: ""
            },
            seriesDefaults: {
                labels: {
                    visible: true,
                    background: "transparent",
                    template: "#= category #: #= value#%"
                }
            },
            series: [{
                type: "pie",
                startAngle: 150,
                data: chartData
            }],
            tooltip: {
                visible: true,
                format: "{0}%"
            }
        });
    };

    var grid;
    function LoadWorkflowReport2View() {
        $.getJSON("/Report/WorkflowReport2/GetWorkflowReport2List", { _t: new Date() }, function (items) {
            grid = $("#WorkflowReport1View").kendoGrid({
                filterable: {
                    extra: false,
                    messages: {
                        info: jsResxReport_SeaWorkflowReport2.Showitemswithvaluethat,
                        clear: jsResxReport_SeaWorkflowReport2.Clear,
                        filter: jsResxReport_SeaWorkflowReport2.Filter
                    },
                    operators: {
                        string: {
                            eq: jsResxReport_SeaWorkflowReport2.Isequalto,
                            neq: jsResxReport_SeaWorkflowReport2.Isnotequalto,
                            startswith: jsResxReport_SeaWorkflowReport2.Startswith,
                            contains: jsResxReport_SeaWorkflowReport2.Contains,
                            doesnotcontain: jsResxReport_SeaWorkflowReport2.Doesnotcontain,
                            endswith: jsResxReport_SeaWorkflowReport2.Endswith
                        },
                    }
                },
                dataSource: new kendo.data.DataSource({
                    data: items,
                    schema: {
                        model: procInstModel
                    },
                    pageSize: 20
                }),
                toolbar: [
                    { template: kendo.template($("#template").html()) }
                ],
                sortable: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    messages: {
                        display: jsResxReport_SeaWorkflowReport2.Showing + "{0}-{1}" + jsResxReport_SeaWorkflowReport2.from + "{2}" + jsResxReport_SeaWorkflowReport2.dataitems,
                        itemsPerPage: jsResxReport_SeaWorkflowReport2.itemsperpage,
                        empty: jsResxReport_SeaWorkflowReport2.Noitemstodisplay
                    }
                },
                detailInit: function (e) {
                    var containerDiv = $("<div style='margin:0 auto;'/>").appendTo(e.detailCell);
                    ProcessFullname = e.data.ProcessFullname;
                    $.getJSON("/Report/WorkflowReport2/GetActInstData/" + ProcessFullname.replace("\\", "/"), { _t: new Date() }, function (actInstData) {
                        var chartData = [];
                        $.each(actInstData, function (index) {

                            chartData.push({
                                category: actInstData[index].DisplayName,
                                value: actInstData[index].Percentage
                            });

                        });// $.each


                        $("<div data-fullname='" + ProcessFullname + "' />").appendTo(containerDiv).kendoGrid({
                            dataSource: new kendo.data.DataSource({
                                data: actInstData,
                                schema: {
                                    model: actInstModel
                                },
                                pageSize: 20
                            }), // dataSource

                            scrollable: false,
                            sortable: true,
                            pageable: {
                                refresh: true,
                                pageSizes: true,
                                messages: {
                                    display: jsResxReport_SeaWorkflowReport2.Showing + "{0}-{1}" + jsResxReport_SeaWorkflowReport2.from + "{2}" + jsResxReport_SeaWorkflowReport2.dataitems,
                                    itemsPerPage: jsResxReport_SeaWorkflowReport2.itemsperpage,
                                    empty: jsResxReport_SeaWorkflowReport2.Noitemstodisplay
                                }
                            }, //pageable

                            detailInit: function (e) {
                                var curFullname = $(e.detailCell).parents("div:first").attr("data-fullname");

                                var containerDiv = $("<div style='margin:0 auto;'/>").appendTo(e.detailCell);
                                var userResultURL = "/Report/WorkflowReport2/GetActInstSlotData/"
                                    + curFullname.replace("\\", "/") + "," + e.data.ActivityName;
                                $.getJSON(userResultURL, { _t: new Date() }, function (actInstSlotData) {
                                    var chartData = [];
                                    $.each(actInstSlotData, function (index) {

                                        chartData.push({
                                            category: actInstSlotData[index].User,
                                            value: actInstSlotData[index].Percentage
                                        });

                                    }); 


                                    //loadActInstSlotKendoChart(containerDiv, "审批人平均审批统计报表", chartData);
                                    $("<div />").appendTo(containerDiv).kendoGrid({
                                        dataSource: new kendo.data.DataSource({
                                            data: actInstSlotData,
                                            schema: {
                                                model: actInstSlotModel
                                            },
                                            pageSize: 20
                                        }),
                                        scrollable: false,
                                        sortable: true,
                                        pageable: {
                                            refresh: true,
                                            pageSizes: true,
                                            messages: {
                                                display: jsResxReport_SeaWorkflowReport2.Showing + "{0}-{1}" + jsResxReport_SeaWorkflowReport2.from + "{2}" + jsResxReport_SeaWorkflowReport2.dataitems,
                                                itemsPerPage: jsResxReport_SeaWorkflowReport2.itemsperpage,
                                                empty: jsResxReport_SeaWorkflowReport2.Noitemstodisplay
                                            }
                                        },
                                        detailInit: function (e) {
                                            //  alert(e);
                                        },
                                        columns: actInstSlotColumns
                                    }); // $("<div />")

                                }); // $.getJSON

                            },
                            dataBound: function () {
                                //阀值变色
                                thresholdChangeColor(activityGridDivObj, "Activity");

                            },
                            columns: actInstColumns
                        });

                        loadActInstKendoChart(containerDiv, jsResxReport_SeaWorkflowReport2.Partoftheaveragestatisticalreportsforapproval, chartData);
                    });

                }, 

                dataBound: function () {
                    thresholdChangeColor($("#WorkflowReport1View"), "Process");
                },

                columns: procInstColumns
            });

            var containerDiv = $("#chart");
            var chartData = [];
            $.each(items, function (item) {
                chartData.push({
                    category: items[item].DisplayName,
                    value: items[item].Percentage

                });
            });
            $("#search_form").kendoButton();
            $("#datepicker1").kendoDatePicker();
            $("#datepicker2").kendoDatePicker();
            $("#search_form").click(function () {
                var fields = [
                   { field: "ProcessFullname", array: new Array($("#process_full_name").val()), operator: "startswith" },
                   { field: "RunningCount", array: new Array($("#datepicker1").val()), operator: "gte" },
                   { field: "CompletedCount", array: new Array($("#datepicker2").val()), operator: "lte" }
                ];
                var createFilter = function (fields) {
                    var filters_list = [];
                    var index = 0;
                    $.each(fields, function (i1, field) {
                        var filter = { logic: "or", filters: [] };
                        if (field.array.length == 0 || field.array[0] == "") {
                            return true;
                        }
                        $.each(field.array, function (i2, value) {
                            filter.filters[i2] = { field: field.field, operator: field.operator, value: value }
                        })
                        filters_list[index] = filter;
                        index++;
                    })
                    return filters_list;
                }
                grid.data("kendoGrid").dataSource.filter(createFilter(fields));
            })
            loadProcessKendoChart(containerDiv, jsResxReport_SeaWorkflowReport2.Theaverageapprovalprocessstatisticalreports, chartData);
        })
    }
    module.exports = LoadWorkflowReport2View;
})