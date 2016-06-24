define(function (require, exports, module) {
    var procInstModel = kendo.data.Model.define({
        id: "ProcDealDuration",
        fields: {
            ProcInstID: { type: "string" },
            YFProcInstID: { type: "string" },
            ProcessName: { type: "string" },
            TotalConsumingSecond: { type: "string" },
            StartUser: { type: "string" },
            Startswith: { type: "date" },
            Finishwith: { type: "date" }
        }
    });

    var ProcessFullname = "";

    var procInstColumns = [

        { field: "YFProcInstID", title: jsResxReport_SeaProcDealDuration.ProcInstID, filterable: false },
        { field: "ProcessName", title: jsResxReport_SeaProcDealDuration.ProcessName, filterable: false },
        { field: "StartUser", title: jsResxReport_SeaProcDealDuration.StartUser, filterable: false },
        { field: "Startswith", title: jsResxReport_SeaProcDealDuration.Startswith, filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" },
        { field: "Finishwith", title: jsResxReport_SeaProcDealDuration.Finishwith, filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" },
        { field: "TotalConsumingSecondStr", title: jsResxReport_SeaProcDealDuration.TotalConsumingSecond, filterable: false }
    ]

    var actInstModel = kendo.data.Model.define({
        id: "ProcDealDurationItemID",
        fields: {
            ActivityName: { type: "string" },
            Arrivewith: { type: "date" },
            Submitwith: { type: "date" },
            TotalConsumingSecond: { type: "string" }
        }
    });

    var actInstColumns = [
        { field: "ActivityName", title: jsResxReport_SeaProcDealDuration.ActivityName, filterable: false },
        { field: "Arrivewith", title: jsResxReport_SeaProcDealDuration.Arrivewith, filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" },
        { field: "Submitwith", title: jsResxReport_SeaProcDealDuration.Submitwith, filterable: false, format: "{0:yyyy-MM-dd HH:mm:ss}" },
        { field: "TotalConsumingSecondStr", title: jsResxReport_SeaProcDealDuration.TotalConsumingSecond, filterable: false }
    ];

    var dataSource;
    var grid;
    function InitGrid() {
        var ajaxParams = {
            _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
            _startUserId: $("#startPerson").attr("data-values"), _procSetID: getStProcessNameValue(), _deptId: $("#txtDeptName").attr("data-values")
        };

        dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "post",
                    url: "/Report/ProcDealDuration/GetProcDealDurationList",
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
                        ajaxParams._procSetID = getStProcessNameValue();
                        ajaxParams._deptId = $("#txtDeptName").attr("data-values");
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

        grid = $("#WorkflowReport1View").kendoGrid({
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
            detailInit: function (e) {
                var containerDiv = $("<div style='margin:0 auto;'/>").appendTo(e.detailCell);
                var procInstId = e.data.ProcInstID;
                $.getJSON("/Report/ProcDealDuration/GetProcDealDurationItemList/", { _procInstId: procInstId }, function (actInstData) {

                    $("<div data-fullname='" + procInstId + "' />").appendTo(containerDiv).kendoGrid({
                        dataSource: new kendo.data.DataSource({
                            data: actInstData,
                            schema: {
                                model: actInstModel
                            },
                            pageSize: 20
                        }), // dataSource

                        sortable: true,
                        pageable: {
                            refresh: true,
                            pageSizes: true,
                            messages: {
                                display: jsResxReport_SeaProcDealDuration.Showing + "{0}-{1}" + jsResxReport_SeaProcDealDuration.from + "{2}" + jsResxReport_SeaProcDealDuration.dataitems,
                                itemsPerPage: jsResxReport_SeaProcDealDuration.itemsperpage,
                                empty: jsResxReport_SeaProcDealDuration.Noitemstodisplay
                            }
                        },
                        columns: actInstColumns
                    });

                });

            },
            columns: procInstColumns
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
        
        $("#txtStartDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#txtFinishDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#btnQuery").click(function () {
            //要求数据源重新读取(并指定切至第一页) 
            dataSource.read({ page: 1, skip: 0 });
            //Grid重新显示数据 
            $("#WorkflowReport1View").data("kendoGrid").refresh();
        });
        $("#btnReset").bind("click", function () {
            $("#txtStartDate").val("");
            $("#txtFinishDate").val("");
            $("#ProcessCategory").data("kendoDropDownList").select(0);
            $("#startPerson").val("").attr("data-values", "");
            $("#stProcessName").data("kendoMultiSelect").value([0]);
            $("#txtDeptName").val("").attr("data-values", "");
            InitGrid();
        });
        //获取分类
        $.ajax({
            url: "/Report/Home/GetAllCategoryList", async: false, dataType: "json", success: function (items) {
                InitDropDownList("ProcessCategory", "Name", "ID", "--全部分类--", items);
            }
        });
        //$.ajax({
        //    url: "/Dashboard/ProcessSupervise/GetProcess", async: false, dataType: "json", success: function (items) {
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
                InitGrid();
            }
        });
    }
    module.exports = LoadWorkflowReport2View;
})