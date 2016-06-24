define(function (require, exports, module) {
    var procInstModel = kendo.data.Model.define({
        id: "PorcInstList",
        fields: {
            ProcInstID: { type: "string" },
            YFProcInstID: { type: "string" },
            Folio: { type: "string" },
            StatusStr: { type: "string" },
            Status: { type: "string" },
            ActiveActName: { type: "string" },
            Startswith: { type: "date" },
            Finishwith: { type: "date" }
        }
    });

    var procInstColumns = [

        { field: "YFProcInstID", title: "流程实例编号", filterable: false },
        { field: "Folio", title: "流程主题", filterable: false },
        { field: "StatusStr", title: "状态", filterable: false },
        { field: "ActiveActName", title: "当前环节名称", filterable: false },
        { field: "Startswith", title: "申请时间", filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" },
        { field: "Finishwith", title: "结束时间", filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" }
    ]

    var ProcinstGroupModel = kendo.data.Model.define({
        id: "ProcinstGroupList",
        fields: {
            Num: { type: "string" },
            Status: { type: "string" },
            StatusStr: { type: "string" }
        }
    });

    var ProcinstGroupColumns = [
        { field: "StatusStr", title: "状态", filterable: false },
        { field: "Num", title: "实例数量", filterable: false }
    ];

    var chartparam = {
        type: 'gauge',
        plotBackgroundColor: null,
        plotBackgroundImage: null,
        plotBorderWidth: 0,
        plotShadow: false
    };

    var credits = {
        text: "",
        href: ""
    };
    var paneparam = {
        startAngle: -150,
        endAngle: 150,
        background: [{
            backgroundColor: {
                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                stops: [
                    [0, '#FFF'],
                    [1, '#333']
                ]
            },
            borderWidth: 0,
            outerRadius: '109%'
        }, {
            backgroundColor: {
                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                stops: [
                    [0, '#333'],
                    [1, '#FFF']
                ]
            },
            borderWidth: 1,
            outerRadius: '107%'
        }, {
            // default background
        }, {
            backgroundColor: '#DDD',
            borderWidth: 0,
            outerRadius: '105%',
            innerRadius: '103%'
        }]
    };
    var plotOptionsparams = {
        series: {
            dataLabels: {
                enabled: true,
                format: '{y} %'
            }
        }
    };
    var yAxisparams = {
        min: 0,
        max: 100,
        minorTickInterval: 'auto',
        minorTickWidth: 1,
        minorTickLength: 10,
        minorTickPosition: 'inside',
        minorTickColor: '#666',

        tickPixelInterval: 30,
        tickWidth: 2,
        tickPosition: 'inside',
        tickLength: 10,
        tickColor: '#666',
        labels: {
            step: 4,
            rotation: '0',
            format: '{value} %'
        },
        title: {
            text: '完成率'
        },
        plotBands: [{
            from: 0,
            to: 30,
            color: '#DF5353' // red
        }, {
            from: 30,
            to: 70,
            color: '#DDDF0D' // yellow
        }, {
            from: 70,
            to: 100,
            color: '#55BF3B' // green
        }]
    };

    var dataSource;
    var grid;

    function InitProcInstGroup() {
        $.getJSON("/Report/CompletionRate/GetProcInstGroupByStatus"
            ,{
                _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
                _startUserId: $("#startPerson").attr("data-values"), _procSetID: getStProcessNameValue(), _deptId: $("#txtDeptName").attr("data-values")
            },
            function (data) {
            $("#statusGroup").kendoGrid({
                dataSource: new kendo.data.DataSource({
                    data: data,
                    schema: {
                        model: ProcinstGroupModel
                    }
                }),
                columns: ProcinstGroupColumns
                //,
                //detailInit: function (e) {
                //    var procInstStatus = e.data.Status;
                //    $("#hidStatus").val(procInstStatus);
                //    //$("#ProcInstListWindow").kendoWindow({
                //    //    width: "750px",
                //    //    height: "500px",
                //    //    title: "流程状态：" + e.data.StatusStr,
                //    //    actions: ["Pin", "Maximize", "Close"],
                //    //    open: function () {
                //    //        $("#hidStatus").val(procInstStatus);
                //    //        InitProcInstGrid(procInstStatus);
                //    //    },
                //    //});
                //    //$("#ProcInstListWindow").data("kendoWindow").title("流程状态：" + e.data.StatusStr).center().open();
                    
                //    var containerDiv = $("<div style='margin:0 auto;'/>").appendTo(e.detailCell);
                //    InitProcInstGrid(containerDiv);
                //}
            });
        });
    }
    function InitProcInstGrid(containerDiv) {
        var ajaxParams = {
            _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
            _startUserId: $("#startPerson").attr("data-values"), _procSetID: getStProcessNameValue(), _deptId: $("#txtDeptName").attr("data-values")
        };

        dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "post",
                    url: "/Report/CompletionRate/GetProcInstByStatus",
                    dataType: "json",
                    contentType: "application/json"
                },
                parameterMap: function (options, operation) {
                    if (operation == "read") {
                        ajaxParams.page = options.page;
                        ajaxParams.pageSize = options.pageSize;
                        ajaxParams._sDate = $("#txtStartDate").val();
                        ajaxParams._fDate = $("#txtFinishDate").val();
                        ajaxParams._processCategory = $("#ProcessCategory").val();
                        ajaxParams._startUserId = $("#startPerson").attr("data-values");
                        ajaxParams._procSetID = $("#stProcessName").val();
                        ajaxParams._deptId = $("#txtDeptName").attr("data-values");
                        ajaxParams._status = $("#hidStatus").val();
                        return kendo.stringify(ajaxParams);
                    }
                }
            },
            serverSorting: true,
            batch: true,
            pageSize: 20,
            schema: {
                data: function (d) {
                    return d.data;
                },
                total: function (d) {
                    return d.total;
                },
                model: procInstModel
            },
            serverPaging: true
        });

        grid = containerDiv.kendoGrid({
            dataSource: dataSource,
            filterable: {
                extra: false,
                messages: {
                    info: jsResxReport_SeaProcDealDuration.Showitemswithvaluethat,
                    clear: jsResxReport_SeaProcDealDuration.Clear,
                    filter: jsResxReport_SeaProcDealDuration.Filter
                },
                operators: {
                    string: {
                        eq: jsResxReport_SeaProcDealDuration.Isequalto,
                        neq: jsResxReport_SeaProcDealDuration.Isnotequalto,
                        startswith: jsResxReport_SeaProcDealDuration.Startswith,
                        contains: jsResxReport_SeaProcDealDuration.Contains,
                        doesnotcontain: jsResxReport_SeaProcDealDuration.Doesnotcontain,
                        endswith: jsResxReport_SeaProcDealDuration.Endswith
                    },
                }
            },
            pageable: {
                refresh: true,
                pageSizes: true,
                messages: {
                    display: jsResxReport_SeaProcDealDuration.Showing + "{0}-{1}" + jsResxReport_SeaProcDealDuration.from + "{2}" + jsResxReport_SeaProcDealDuration.dataitems,
                    itemsPerPage: jsResxReport_SeaProcDealDuration.itemsperpage,
                    empty: jsResxReport_SeaProcDealDuration.Noitemstodisplay
                }
            },
            columns: procInstColumns
        });
    }
    function Initcharts() {
        $.ajax({
            type: 'get',
            url: '/Report/CompletionRate/GetCompletionRate',
            data: {
                _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
                _startUserId: $("#startPerson").attr("data-values"), _procSetID: getStProcessNameValue(), _deptId: $("#txtDeptName").attr("data-values")
            },
            cache: false,
            dataType: 'json',
            success: function (rateData) {
                $('#container1').highcharts({
                    credits: credits,
                    chart: chartparam,
                    title: {
                        text: ''
                    },
                    pane: paneparam,
                    plotOptions: plotOptionsparams,
                    // the value axis
                    yAxis: yAxisparams,
                    series: [{
                        name: '完成率',
                        data: [rateData],
                        tooltip: {
                            valueSuffix: '%'
                        }
                    }]
                });
                InitProcInstGroup();
            }
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
        return pn;
    }


    function LoadWorkflowReport2View() {
        
        //$.ajax({
        //    url: "/Dashboard/ProcessSupervise/GetProcess", async: true, dataType: "json", success: function (items) {
        //        InitDropDownList("stProcessName", "ProcessName", "ProcSetID", "--全部流程--", items);
        //    }
        //});
        //获取分类
        $.ajax({
            url: "/Report/Home/GetAllCategoryList", async: true, dataType: "json", success: function (items) {
                InitDropDownList("ProcessCategory", "Name", "ID", "--全部分类--", items);
            }
        });

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
                Initcharts();
            }
        });

        $("#btnQuery").bind("click", function () {
            Initcharts();
        });

        $("#btnReset").bind("click", function () {
            $("#txtStartDate").val("");
            $("#txtFinishDate").val("");
            $("#ProcessCategory").data("kendoDropDownList").select(0);
            $("#startPerson").val("").attr("data-values", "");
            $("#stProcessName").data("kendoMultiSelect").value([0]);
            $("#txtDeptName").val("").attr("data-values", "");
            Initcharts();
        });
    }
    module.exports = LoadWorkflowReport2View;
})