

define(function (require, exports, module) {

    var Alltools = ["formatting", "bold", "italic", "underline", "strikethrough",
                     "justifyLeft", "justifyCenter", "justifyRight", "justifyFull",
                     "insertUnorderedList", "insertOrderedList", "indent", "outdent",
                     "createLink", "unlink", "insertImage", "createTable", "addRowAbove",
                     "addRowBelow", "addColumnLeft", "addColumnRight", "deleteRow",
                     "deleteColumn", "foreColor", "backColor", "viewHtml"]

    var Report1Model = kendo.data.Model.define({
        id: "Report1ID",
        fields: {
            ProcessFullname: { type: "string" },
            DisplayName: { type: "string" },
            TotalCount: { type: "string" },
            RunningCount: { type: "string" },
            Percentage: { type: "string" },
            CompletedCount: { type: "string" }
        }
    });

    var Report1Columns = [

        { field: "DisplayName", title: jsResxReport_SeaWorkflowReport1.ProcessFullname },
        { field: "TotalCount", title: jsResxReport_SeaWorkflowReport1.TotalCount, filterable: false },
        { field: "RunningCount", title: jsResxReport_SeaWorkflowReport1.RunningCount, filterable: false },
        { field: "CompletedCount", title: jsResxReport_SeaWorkflowReport1.CompletedCount, filterable: false },
    ]

    var Report1ItemModel = kendo.data.Model.define({
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

    var Report1ItemColumns = [

        { field: "DisplayName", title: jsResxReport_SeaWorkflowReport1.ActivityName, filterable: false },
        { field: "TotalCount", title: jsResxReport_SeaWorkflowReport1.TotalCount, filterable: false },
        { field: "RunningCount", title: jsResxReport_SeaWorkflowReport1.RunningCount, filterable: false },
        { field: "ExpiredCount", title: jsResxReport_SeaWorkflowReport1.ExpiredCount, filterable: false },
        { field: "CompletedCount", title: jsResxReport_SeaWorkflowReport1.CompletedCount, filterable: false },
    ];


    //饼图的颜色定义
    var colorAry = ["#9de219", "#90cc38", "#068c35", "#006634", "#004d38", "#033939"];

    //设置饼图颜色
    function setColor(chartData) {

        $.each(chartData, function (index) {
            chartData[index].color = colorAry[index % colorAry.length];

        });

        return chartData;

    } // function setColor

    //绘制饼形图
    function loadKendoChart(containerDiv, title, chartData) {
        chartData = setColor(chartData);
        $("<div/> ").appendTo(containerDiv).kendoChart({
            title: {
                position: "top",
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
    function LoadWorkflowReport1View() {
        title = jsResxReport_SeaWorkflowReport1.GetWorkflowReport1List;

        $.getJSON("/Report/WorkflowReport1/GetWorkflowReport1List", { _t: new Date() }, function (items) {
            grid = $("#WorkflowReport1View").kendoGrid({
                filterable: {
                    extra: false,
                    messages: {
                        info: jsResxReport_SeaWorkflowReport1.Showitemswithvaluethat,
                        clear: jsResxReport_SeaWorkflowReport1.Clear,
                        filter: jsResxReport_SeaWorkflowReport1.Filter
                    },
                    operators: {
                        string: {
                            eq: jsResxReport_SeaWorkflowReport1.Isequalto,
                            neq: jsResxReport_SeaWorkflowReport1.Isnotequalto,
                            startswith: jsResxReport_SeaWorkflowReport1.Startswith,
                            contains: jsResxReport_SeaWorkflowReport1.Contains,
                            doesnotcontain: jsResxReport_SeaWorkflowReport1.Doesnotcontain,
                            endswith: jsResxReport_SeaWorkflowReport1.Endswith
                        },
                    }
                },
                dataSource: new kendo.data.DataSource({
                    data: items,
                    schema: {
                        model: Report1Model
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
                        display: jsResxReport_SeaWorkflowReport1.Showing + "{0}-{1}" + jsResxReport_SeaWorkflowReport1.from + "{2}" + jsResxReport_SeaWorkflowReport1.dataitems,
                        itemsPerPage: jsResxReport_SeaWorkflowReport1.Thenumberofrecordsperpage,
                        empty: jsResxReport_SeaWorkflowReport1.Norecords
                    }
                },

                detailInit: function (e) {
                    $.getJSON("/Report/WorkflowReport1/GetWorkflowReport1ItemList/" + e.data.ProcessFullname.replace("\\", "/"), { _t: new Date() }, function (subItems) {

                        var containerDiv = $("<div style='margin:0 auto;'/>").appendTo(e.detailCell);

                        var chartData = [];
                        $.each(subItems, function (index) {
                            chartData.push({
                                category: subItems[index].DisplayName,
                                value: subItems[index].Percentage

                            });
                        });


                        $("<div />").appendTo(containerDiv).kendoGrid({
                            dataSource: new kendo.data.DataSource({
                                data: subItems,
                                schema: {
                                    model: Report1ItemModel
                                },
                                pageSize: 20
                            }),
                            scrollable: false,
                            sortable: true,
                            pageable: {
                                refresh: true,
                                pageSizes: true,
                                messages: {
                                    display: jsResxReport_SeaWorkflowReport1.Showing + "{0}-{1}" + jsResxReport_SeaWorkflowReport1.from + "{2}" + jsResxReport_SeaWorkflowReport1.dataitems,
                                    itemsPerPage: jsResxReport_SeaWorkflowReport1.Thenumberofrecordsperpage,
                                    empty: jsResxReport_SeaWorkflowReport1.Norecords
                                }
                            },
                            columns: Report1ItemColumns
                        });
                        loadKendoChart(containerDiv, "环节总量统计图", chartData);
                    });
                },

                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },

                columns: Report1Columns
            });
            var containerDiv = $("#chart");
            var chartData = [];

            $.each(items, function (item) {
                chartData.push({
                    category: items[item].DisplayName,
                    value: items[item].Percentage

                });
            });
            loadKendoChart(containerDiv, "流程总量统计图", chartData);
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

        });

    }
    module.exports = LoadWorkflowReport1View;
})