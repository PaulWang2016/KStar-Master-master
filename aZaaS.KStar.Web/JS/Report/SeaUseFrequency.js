define(function (require, exports, module) {
    var procInstModel = kendo.data.Model.define({
        id: "Report2ID",
        fields: {
            ProcessName: { type: "string" },
            Avg_Consuming_Second: { type: "string" },
            UseCount: { type: "string" },
            //UseFrequency: { type: "string" },
            FrequencyType: { type: "string" }
        }
    });

    var ProcessFullname = "";

    var procInstColumns = [

        { field: "ProcessName", title: jsResxReport_SeaUseFrequency.ProcessName, filterable: false },
        { field: "UseCount", title: jsResxReport_SeaUseFrequency.UseCount, filterable: false },
        { field: "Avg_Consuming_SecondStr", title: jsResxReport_SeaUseFrequency.Avg_Consuming_Second, filterable: false },
        //{ field: "UseFrequency", title: jsResxReport_SeaUseFrequency.UseFrequency, filterable: false},
        { field: "FrequencyType", title: jsResxReport_SeaUseFrequency.FrequencyType, filterable: false }
    ];

    var dataSource;
    var grid;
    function InitGrid() {
        var ajaxParams = {
            _sDate: $("#txtStartDate").val(), _fDate: $("#txtFinishDate").val(), _processCategory: $("#ProcessCategory").val(),
            _procSetID: $("#stProcessName").val(), _deptId: $("#txtDeptName").attr("data-values")
        };
        dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "post",
                    url: "/Report/UseFrequency/GetUseFrequencyList",
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
                        ajaxParams._procSetID = $("#stProcessName").val();
                        ajaxParams._deptId = $("#txtDeptName").attr("data-values");
                        return kendo.stringify(ajaxParams);
                    }
                }
            },
            serverSorting : true,
            batch: true,
            pageSize: 20,
            schema: {
                data: function (d) {
                    return d.data;
                },
                total: function (d) {
                    return d.total;
                }
            },
            serverPaging: true
        });

        grid = $("#WorkflowReport1View").kendoGrid({
            dataSource: dataSource,
            filterable: {
                extra: false,
                messages: {
                    info: jsResxReport_SeaUseFrequency.Showitemswithvaluethat,
                    clear: jsResxReport_SeaUseFrequency.Clear,
                    filter: jsResxReport_SeaUseFrequency.Filter
                },
                operators: {
                    string: {
                        eq: jsResxReport_SeaUseFrequency.Isequalto,
                        neq: jsResxReport_SeaUseFrequency.Isnotequalto,
                        startswith: jsResxReport_SeaUseFrequency.Startswith,
                        contains: jsResxReport_SeaUseFrequency.Contains,
                        doesnotcontain: jsResxReport_SeaUseFrequency.Doesnotcontain,
                        endswith: jsResxReport_SeaUseFrequency.Endswith
                    },
                }
            },
            pageable: {
                refresh: true,
                pageSizes: true,
                messages: {
                    display: jsResxReport_SeaUseFrequency.Showing + "{0}-{1}" + jsResxReport_SeaUseFrequency.from + "{2}" + jsResxReport_SeaUseFrequency.dataitems,
                    itemsPerPage: jsResxReport_SeaUseFrequency.itemsperpage,
                    empty: jsResxReport_SeaUseFrequency.Noitemstodisplay
                }
            },
            columns: procInstColumns
        });
    }

    function LoadWorkflowReport2View() {
        InitGrid();
        $("#search_form").kendoButton();
        $("#txtStartDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#txtFinishDate").kendoDatePicker({ format: "dd/MM/yyyy" });
        $("#btnQuery").click(function () {
            //要求数据源重新读取(并指定切至第一页) 
            dataSource.read({ page: 1, skip: 0 });
            //Grid重新显示数据 
            $("#WorkflowReport1View").data("kendoGrid").refresh();
        });
        $("#btnReset").click(function () {
            $("#txtStartDate").val("");
            $("#txtFinishDate").val("");
            $("#ProcessCategory").data("kendoDropDownList").select(0);
            $("#txtDeptName").val("").attr("data-values", "");
            //要求数据源重新读取(并指定切至第一页) 
            dataSource.read({ page: 1, skip: 0 });
            //Grid重新显示数据 
            $("#WorkflowReport1View").data("kendoGrid").refresh();
        });

        //获取分类
        $.ajax({
            url: "/Report/Home/GetAllCategoryList", async: false, dataType: "json", success: function (items) {
                InitDropDownList("ProcessCategory", "Name", "ID", "--全部分类--", items);
            }
        });
        $.ajax({
            url: "/Dashboard/ProcessSupervise/GetProcess", async: false, dataType: "json", success: function (items) {
                InitDropDownList("stProcessName", "ProcessName", "ProcSetID", "--全部流程--", items);
            }
        });
    }
    module.exports = LoadWorkflowReport2View;
})