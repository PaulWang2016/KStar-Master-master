function SerializeJsonObject(jsonObject) {
    var jsonstring = "";
    for (x in jsonObject) {
        jsonstring = jsonstring + "&" + x + "=" + jsonObject[x];
    }
    if (jsonstring.length > 0) {
        jsonstring = jsonstring.substring(1, jsonstring.length);
    }
    return jsonstring;
}

function GetSubString(str, length) {
    if (str != null && str != undefined && str.length > 0) {
        return str.substring(0, (length > str.length ? str.length : length)) + "...";
    }
    else {
        return "";
    }
}


//<!--------- Dashboard (KStar v1.8) ------------>

/**
* 初始化并生成使用服务端分页的待办任务分组列表
**/
function InitServerQueryPendingTaskKendoExcelGrid(target, viewModel, columns, url, items, total, height, title, queryMap, callBack, dataCompeleteCallBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");

    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: function (options) {
                if (parseInt(options.data.page) > 1) {
                    $.getJSON("/Dashboard/PendingTasks/List", {
                        "_t": new Date(),
                        folio: queryMap.folio,
                        originator: queryMap.originator,
                        startdate: queryMap.startdate,
                        enddate: queryMap.enddate,
                        processname: title,
                        page: options.data.page,
                        pageSize: options.data.pageSize
                    }, function (json) {
                        //var arr = json.totals;
                        //var index = 0;

                        //for (var key in arr) {
                        //    if(key==title)
                        //    {
                        //        break;    
                        //    }
                        //    index++;
                        //}                       
                        var result = { total: json.total, data: json.data };
                        options.success(result);
                    });
                }
                else {

                    var result = { total: total, data: items };
                    options.success(result);
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 5,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: [5],
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
                if (dataCompeleteCallBack) {
                    dataCompeleteCallBack();
                }
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

/**
* 初始化并生成使用服务端分页的流程任务列表
**/
function InitServerQueryKendoExcelGrid(target, viewModel, columns, url, parameterdata, height, title, callBack, dataCompeleteCallBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize
                    };
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
                if (dataCompeleteCallBack) {
                    dataCompeleteCallBack();
                }
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

//<!--------- Dashboard (KStar v1.8) ------------>

//初始化Grid       target:id名   viewModel:数据模型 columns:列的配置 items:数据列表 pageSize:分页大小 callBack:回调函数
function InitKendoGrid(target, viewModel, columns, items, pageSize, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns
        });
        grid = $("#" + target).data("kendoGrid");
        GridHeaderAppendDiv(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
}

function InitPendingTaskKendoExcelGrid(target, viewModel, columns, items, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            //"export": {
            //    cssClass: "excel-ico",//"glyphicon glyphicon-export",
            //    title: title,
            //    createUrl: "/Export/ToExcel",
            //    downloadUrl: "/Export/Get",
            //    createCSVUrl: "/Export/ToCSV",
            //    downloadSCVUrl: "/Export/GetCSV"
            //},
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitKendoExcelGrid(target, viewModel, columns, items, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }

    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //HideGridVerticalScroll(target);//隐藏Scroll
            }
        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitKendoExcelGridWithHeight(target, viewModel, columns, items, pageSize, title, height, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }

    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                allowExport: true,
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                //HideGridVerticalScroll(target);//隐藏Scroll
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");

        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");

        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitPaddingUrlKendoExcelGrid(target, viewModel, columns, url, parameterdata, pageSize, title, callBack) {
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    return parameterdata;
                }
            }
        },
        schema: {
            model: viewModel,
            parse: function (response) {
                var isChange = false;
                var oldcolumns = columns;
                var dbmodel = dataSource.options.schema.model;
                var tasks = [];
                $.each(response, function () {
                    var that = this;
                    var task = {};
                    for (var key in that) {
                        task[key] = that[key];
                    }
                    if (that.BusinessData) {
                        $.each(that.BusinessData, function () {
                            switch (this.ValueType) {
                                case "DateTime":
                                    task[this.Field] = new Date(this.ValueString);
                                    break;
                                default:
                                    task[this.Field] = this.ValueString;
                            }
                            if (!dbmodel.fields[this.Field]) {
                                isChange = true;
                                var mField = "";
                                var mColumn = {
                                    field: this.Field,
                                    title: this.DisplayName,
                                    hidden: !this.IsVisible,
                                    filterable: {
                                        extra: false,
                                        messages: {
                                            info: jsResxbaseInitView.Showitemswithvaluethat,
                                            clear: jsResxbaseInitView.Clear,
                                            filter: jsResxbaseInitView.Filter
                                        },
                                        operators: {
                                            string: {
                                                eq: jsResxbaseInitView.Isequalto,
                                                neq: jsResxbaseInitView.Isnotequalto,
                                                startswith: jsResxbaseInitView.Startswith,
                                                contains: jsResxbaseInitView.Contains,
                                                doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                                                endswith: jsResxbaseInitView.Endswith
                                            },
                                        }
                                    },
                                };
                                switch (this.ValueType) {
                                    case "DateTime":
                                        mField = { type: "date" };
                                        mColumn.format = getDateTimeFormat();
                                        break;
                                    default:
                                        mField = { type: "string" };
                                }
                                dbmodel.fields[this.Field] = mField;
                                oldcolumns.push(mColumn);
                            }
                        })
                    }
                    tasks.push(task)
                })
                //oldgrid.options.columns = oldcolumns;
                //oldgrid.refresh();

                if (!grid || isChange) {
                    if (grid) {
                        grid.destroy();
                    }
                    oldcolumns = InitializeColumnResize(oldcolumns, target);
                    $("#" + target).kendoExcelGrid({
                        dataSource: dataSource,
                        groupable: {
                            messages: {
                                empty: jsResxbaseInitView.Dropcolumnshere
                            }
                        },
                        selectable: false,
                        sortable: true,
                        //scrollable: false,
                        columnMenu: {
                            messages: {
                                sortAscending: jsResxbaseInitView.Sortasc,
                                sortDescending: jsResxbaseInitView.Sortdesc,
                                columns: jsResxbaseInitView.Choosecolumns,
                                filter: jsResxbaseInitView.Filter,
                            }
                        },
                        pageable: {
                            pageSizes: true,
                            messages: {
                                itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                                display: jsResxbaseInitView.datadisplay,
                                empty: jsResxbaseInitView.Noitemstodisplay
                            }
                        },
                        filterable: {
                            extra: false,
                            messages: {
                                info: jsResxbaseInitView.Showitemswithvaluethat,
                                clear: jsResxbaseInitView.Clear,
                                filter: jsResxbaseInitView.Filter
                            },
                            operators: {
                                string: {
                                    eq: jsResxbaseInitView.Isequalto,
                                    neq: jsResxbaseInitView.Isnotequalto,
                                    startswith: jsResxbaseInitView.Startswith,
                                    contains: jsResxbaseInitView.Contains,
                                    doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                                    endswith: jsResxbaseInitView.Endswith
                                },
                            }
                        },
                        reorderable: true,
                        resizable: true,
                        columns: oldcolumns,
                        "export": {
                            cssClass: "excel-ico",//"glyphicon glyphicon-export",
                            title: title,
                            createUrl: "/Export/ToExcel",
                            downloadUrl: "/Export/Get",
                            createCSVUrl: "/Export/ToCSV",
                            downloadSCVUrl: "/Export/GetCSV"
                        },
                        dataBound: function () {
                            refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                            HideGridVerticalScroll(target);//隐藏Scroll
                        }
                    });
                    grid = $("#" + target).data("kendoExcelGrid");
                    GridHeaderAppendDiv(target);
                    //HideGridVerticalScroll(target);
                    if (!isChange) {
                        AddSplitters(grid);
                    }

                    if (callBack) {
                        callBack();
                    }
                }

                return tasks;
            }
        },
        pageSize: pageSize
    });
    dataSource.read();
    //refreshCurrentScrolls();
}

function InitUrlKendoExcelGrid(target, viewModel, columns, url, parameterdata, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    return parameterdata;
                }
            }
        },
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.options.filter = parameterdata;
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
function InitDataKendoExcelGrid(target, viewModel, columns, data, parameterdata, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        //transport: {
        //    read: {
        //        url: url,
        //        type: "POST"
        //    },
        //    parameterMap: function (data, type) {
        //        if (type == "read") {
        //            // send take as "$top" and skip as "$skip"
        //            return parameterdata;
        //        }
        //    }
        //},
        data: data,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.options.filter = parameterdata;
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                createWidthOutHiddenUrl: "/Export/ToExcelWithOutHidden",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
function InitDataDetailKendoExcelGrid(target, viewModel, columns, data, parameterdata, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        //transport: {
        //    read: {
        //        url: url,
        //        type: "POST"
        //    },
        //    parameterMap: function (data, type) {
        //        if (type == "read") {
        //            // send take as "$top" and skip as "$skip"
        //            return parameterdata;
        //        }
        //    }
        //},
        data: data,
        schema: {
            model: viewModel,
            parse: function (response) {
                var details = [];
                $.each(response, function () {
                    var that = this;
                    var detail = {};
                    for (var key in that) {
                        detail[key] = that[key];
                    }

                    for (var key in detail.RecordContent) {
                        if (key != "ID" && key != "FK_Form_ID") {
                            detail[key] = detail.RecordContent[key];
                        }
                    }

                    details.push(detail)
                })
                return details;
            }
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.destroy();

        $("<div id='" + target + "'><div>").replaceAll("#" + target);

        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                createWidthOutHiddenUrl: "/Export/ToExcelWithOutHidden",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                createWidthOutHiddenUrl: "/Export/ToExcelWithOutHidden",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
function InitUrlDetailKendoExcelGrid(target, viewModel, columns, url, parameterdata, pageSize, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    return parameterdata;
                }
            }
        },
        schema: {
            model: viewModel,
            parse: function (response) {
                var details = [];
                $.each(response, function () {
                    var that = this;
                    var detail = {};
                    for (var key in that) {
                        detail[key] = that[key];
                    }

                    for (var key in detail.RecordContent) {
                        if (key != "ID" && key != "FK_Form_ID") {
                            detail[key] = detail.RecordContent[key];
                        }
                    }

                    details.push(detail)
                })
                return details;
            }
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.destroy();

        $("<div id='" + target + "'><div>").replaceAll("#" + target);

        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            "filter": parameterdata,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条

                HideGridVerticalScroll(target);//隐藏Scroll
            }

        });
        grid = $("#" + target).data("kendoExcelGrid");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
function InitServerKendoGrid(target, viewModel, columns, url, height, callBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var filter = [];
                    if (data.filter != undefined) {
                        var temp = data.filter.filters;
                        for (var index in temp) {
                            filter.push(obj2str({ Field: temp[index].field, Operator: temp[index].operator, Value: temp[index].value }));
                        }
                    }
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize,
                        filter: "[" + filter + "]"
                    }
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            //groupable: {
            //    messages: {
            //        empty: jsResxbaseInitView.Dropcolumnshere
            //    }
            //},
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitServerKendoExcelGrid(target, viewModel, columns, url, height, title, callBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var filter = [];
                    if (data.filter != undefined) {
                        var temp = data.filter.filters;
                        for (var index in temp) {
                            filter.push(obj2str({ Field: temp[index].field, Operator: temp[index].operator, Value: temp[index].value }));
                        }
                    }
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize,
                        filter: "[" + filter + "]"
                    }
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV"
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitServerQueryKendoExcelGrid(target, viewModel, columns, url, parameterdata, height, title, downloadurl, callBack, dataCompeleteCallBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize
                    };
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            title: title,
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                allowExport: true,
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV",
                isDownloadFromServer: true,
                downloadFromServerUrl: downloadurl
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
                if (dataCompeleteCallBack) {
                    dataCompeleteCallBack();
                }
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

//增加downloadurl参数 适用于服务器分页的grid，在导出excel，csv时，可以自定义返回数据源地址，避免默认返回客户端显示的数据集
function InitServerCustomKendoExcelGrid(target, viewModel, columns, url, parameterdata, height, title, downloadurl, callBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var filter = [];
                    if (data.filter != undefined) {
                        var temp = data.filter.filters;
                        for (var index in temp) {
                            filter.push(obj2str({ Field: temp[index].field, Operator: temp[index].operator, Value: temp[index].value }));
                        }
                    }
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize,
                        filter: "[" + filter + "]"
                    }
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                allowExport: true,
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV",
                isDownloadFromServer: true,
                downloadFromServerUrl: downloadurl
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}
//增加downloadurl参数 适用于服务器分页的grid，在导出excel，csv时，可以自定义返回数据源地址，避免默认返回客户端显示的数据集
function InitServerQueryCustomKendoExcelGrid(target, viewModel, columns, url, parameterdata, height, title, downloadurl, callBack) {
    columns = InitializeColumnResize(columns, target);
    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize
                    };
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            groupable: {
                messages: {
                    empty: jsResxbaseInitView.Dropcolumnshere
                }
            },
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            "export": {
                cssClass: "excel-ico",//"glyphicon glyphicon-export",
                title: title,
                createUrl: "/Export/ToExcel",
                downloadUrl: "/Export/Get",
                createCSVUrl: "/Export/ToCSV",
                downloadSCVUrl: "/Export/GetCSV",
                isDownloadFromServer: true,
                downloadFromServerUrl: downloadurl
            },
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        $("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}


//初始化BaseGrid       target:id名   viewModel:数据模型 columns:列的配置 items:数据列表 callBack:回调函数
function InitBaseKendoGrid(target, viewModel, columns, items, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        }
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            columns: columns,
            dataBound: function () {
                HideGridVerticalScroll(target);
            }
        });
        grid = $("#" + target).data("kendoGrid");
        HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
}


/* ************************************* 报表中心 定制列表 **************************** */

function InitServerQueryReportKendoExcelGrid(target, viewModel, columns, url, parameterdata, height, title, downloadurl, callBack) {
    columns = InitializeColumnResize(columns, target);

    var grid = $("#" + target).data("kendoExcelGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    // send take as "$top" and skip as "$skip"
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize
                    };
                    if (data.sort) {
                        newdata.sort = kendo.stringify(data.sort)
                    }
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: 20,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true//,
        //serverGrouping: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoExcelGrid({
            dataSource: dataSource,
            //groupable: {
            //    messages: {
            //        empty: jsResxbaseInitView.Dropcolumnshere
            //    }
            //},
            selectable: false,
            sortable: true,
            //scrollable: false,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            reorderable: true,
            resizable: true,
            columns: columns,
            //"export": {
            //    cssClass: "excel-ico",//"glyphicon glyphicon-export",
            //    title: title,
            //    createUrl: "/Export/ToExcel",
            //    downloadUrl: "/Export/Get",
            //    createCSVUrl: "/Export/ToCSV",
            //    downloadSCVUrl: "/Export/GetCSV",
            //    isDownloadFromServer: true,
            //    downloadFromServerUrl: downloadurl
            //},
            dataBound: function () {
                refreshCurrentScrolls();//数据绑定完成  刷新滚动条
                //if (dataSource.data().length == dataSource.pageSize()) {
                //    HideGridVerticalScroll(target);//隐藏Scroll
                //}
                $("#" + target + " .k-grid-content").css("height", "auto");
            },
            height: "auto"
        });
        grid = $("#" + target).data("kendoExcelGrid");
        $("#" + target + " .k-grid-content").css("overflow-y", "scroll");
        var limitheight = 20;
        $("#" + target + " .k-grid-content").siblings().each(function () {
            limitheight += $(this).height();
        })
        //$("#" + target + " .k-grid-content").css("max-height", height - limitheight + "px");
        GridHeaderAppendDiv(target);
        //HideGridVerticalScroll(target);
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}



//function InitBaseKendoGrid(target, viewModel, columns, items, callBack) {
//    columns = InitializeColumnResize(columns, target);
//    if (!items) {
//        items = Array();
//    }
//    var grid = $("#" + target).data("kendoGrid");
//    var dataSource = new kendo.data.DataSource({
//        data: items,
//        schema: {
//            model: viewModel
//        }
//    });
//    if (grid) {
//        grid.setDataSource(dataSource);
//    }
//    else {
//        $("#" + target).kendoGrid({
//            dataSource: dataSource,
//            columns: columns,
//            dataBound: function () {
//                HideGridVerticalScroll(target);
//            }
//        });
//        grid = $("#" + target).data("kendoGrid");
//        HideGridVerticalScroll(target);
//        AddSplitters(grid);
//    }
//    if (callBack) {
//        callBack();
//    }
//}
//初始化BaseGrid       target:id名   viewModel:数据模型 columns:列的配置 items:数据列表 pageSize:分页大小 callBack:回调函数
function InitBaseKendoGridWidthPage(target, viewModel, columns, items, pageSize, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            columnMenu: {
                messages: {
                    sortAscending: jsResxbaseInitView.Sortasc,
                    sortDescending: jsResxbaseInitView.Sortdesc,
                    columns: jsResxbaseInitView.Choosecolumns,
                    filter: jsResxbaseInitView.Filter,
                }
            },
            filterable: {
                extra: false,
                messages: {
                    info: jsResxbaseInitView.Showitemswithvaluethat,
                    clear: jsResxbaseInitView.Clear,
                    filter: jsResxbaseInitView.Filter
                },
                operators: {
                    string: {
                        eq: jsResxbaseInitView.Isequalto,
                        neq: jsResxbaseInitView.Isnotequalto,
                        startswith: jsResxbaseInitView.Startswith,
                        contains: jsResxbaseInitView.Contains,
                        doesnotcontain: jsResxbaseInitView.Doesnotcontain,
                        endswith: jsResxbaseInitView.Endswith
                    },
                }
            },
            pageable: {
                refresh: false
            },
            columns: columns,
            dataBound: function () {
                //refreshCurrentScrolls();
                HideGridVerticalScroll(target);
            }
        });
        grid = $("#" + target).data("kendoGrid");
        GridHeaderAppendDiv(target);
        HideGridVerticalScroll(target);

        //AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
    //refreshCurrentScrolls();
}

function InitBaseServerKendoGridWidthPage(target, viewModel, columns, url, parameterdata, pageSize, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    var newdata = {
                        take: data.take,
                        skip: data.skip,
                        page: data.page,
                        pageSize: data.pageSize
                    };
                    for (var key in parameterdata) {
                        newdata[key] = parameterdata[key];
                    }
                    return newdata;
                }
            }
        },
        schema: {
            model: viewModel,
            data: "data",
            total: "total"
        },
        pageSize: pageSize,
        serverPaging: true
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            columns: columns,
            dataBound: function () {
                //HideGridVerticalScroll(target);
            }
        });
        grid = $("#" + target).data("kendoGrid");
        //HideGridVerticalScroll(target);
    }
    if (callBack) {
        callBack();
    }
}

function InitBaseUrlKendoGridWidthPage(target, viewModel, columns, url, parameterdata, pageSize, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!pageSize) {
        pageSize = 5;
    }
    if (pageSize != 5 && pageSize != 10 && pageSize != 20) {
        pageSize = 20;
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: url,
                type: "POST"
            },
            parameterMap: function (data, type) {
                if (type == "read") {
                    return parameterdata;
                }
            }
        },
        schema: {
            model: viewModel
        },
        pageSize: pageSize
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            pageable: {
                pageSizes: true,
                messages: {
                    itemsPerPage: jsResxbaseInitView.dataitemsperpage,
                    display: jsResxbaseInitView.datadisplay,
                    empty: jsResxbaseInitView.Noitemstodisplay
                }
            },
            columns: columns,
            dataBound: function () {
                HideGridVerticalScroll(target);
            }
        });
        grid = $("#" + target).data("kendoGrid");
        HideGridVerticalScroll(target);
    }
    if (callBack) {
        callBack();
    }
}


function InitBaseEditableKendoGrid(target, viewModel, columns, items, callBack) {
    columns = InitializeColumnResize(columns, target);
    if (!items) {
        items = Array();
    }
    var grid = $("#" + target).data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: items,
        type: "post",
        schema: {
            model: viewModel
        }
    });
    if (grid) {
        grid.setDataSource(dataSource);
    }
    else {
        $("#" + target).kendoGrid({
            dataSource: dataSource,
            columns: columns,
            editable: true
        });
        grid = $("#" + target).data("kendoGrid");
        AddSplitters(grid);
    }
    if (callBack) {
        callBack();
    }
}

//把Grid  Header <th><a..<th/>  添加一个div  变成  <th><div><a..</div></th>
function GridHeaderAppendDiv(target) {
    $("#" + target).find(".k-grid-header").find(".k-header").each(function () {
        if ($(this).children("div").length == 0) {
            $("<div></div>").append($(this).contents()).prependTo($(this));
        }
    })
}
//把Grid Vertical Scroll 隐藏
function HideGridVerticalScroll(target) {
    //$("#" + target + " .k-grid-content").css("overflow-y", "hidden");
    $("#" + target + " .k-grid-header").first().css("padding-right", "0");
    $("#" + target + " .k-grid-header-wrap").first().css("border-right", "0");
}

//获取Grid中数据源 某列数据       (不重复)  target:id名 field:绑定列字段
function GetFilterItems(target, field) {
    var items = new Array();
    var datas = getKendoGrid(target).dataSource.data();
    for (var i = 0; i < datas.length; i++) {
        var item = datas[i][field];
        if ($.inArray(item, items) == -1) {
            items.push(item);
        }
    }
    return items;
}

//自定义Filter 为 AutoComplete 控件       element:控件 target:id名 field:绑定列字段
function AutoCompleteFilter(element, target, field) {
    var items = GetFilterItems(target, field);
    element.kendoAutoComplete({
        dataSource: items
    });
}

//自定义Filter 为 DropDownList 控件       element:控件 target:id名 field:绑定列字段
function DropDownListFilter(element, target, field) {
    var items = GetFilterItems(target, field);
    element.kendoDropDownList({
        dataSource: items,
        optionLabel: "--Select Value--"
    });
}

//自定义Filter 为 MultiSelect 控件       element:控件 target:id名 field:绑定列字段      -------用处不大
function MultiSelectFilter(element, target, field) {
    var items = GetFilterItems(target, field);
    element.kendoMultiSelect({
        dataSource: items,
        placeholder: "--Select Values--"
    })
}

function getKendoGrid(target) {
    var grid = $("#" + target).data("kendoGrid");
    if (!grid) {
        grid = $("#" + target).data("kendoExcelGrid");
    }
    return grid;
}

//绑定并初始化操作    From cookies And To cookies       target:id名
function bindAndLoad(target) {
    getKendoGrid(target).bind("columnShow", function (e) {
        SettingColumnVisible(target, e.column.field, false);
    });
    getKendoGrid(target).bind("columnHide", function (e) {
        SettingColumnVisible(target, e.column.field, true);
    });
    getKendoGrid(target).bind("columnReorder", function (e) {
        SettingReorderColumn(target, e);
    });
    getKendoGrid(target).bind("columnResize", function (e) {
        SettingColumnResize(target, e);
    });

    InitializeGridResize(target);
    InitializeReorderColumn(target);
    initializeColumnVisible(target);
    SettingFilters(target);
}

//绑定 Grid 中 Checkbox 的全选时间       target:id名
function bindGridCheckbox(target) {
    getKendoGrid(target).bind("dataBound", function (e) {
        $("#" + target + " .k-grid-header").find(":checkbox").prop("checked", false);
    })
    $("#" + target + " .k-grid-header").find(":checkbox").click(function () {
        if ($(this).prop("checked")) {
            $("#" + target + " .k-grid-content").find(":checkbox").prop("checked", true);
        }
        else {
            $("#" + target + " .k-grid-content").find(":checkbox").prop("checked", false);
        }
    })
}
function bindGridSingleCheckbox(target) {
    getKendoGrid(target).bind("dataBound", function (e) {
        $("#" + target + " .k-grid-header").find(":checkbox").hide();
    })
    $("#" + target).on("click", ".k-grid-content :checkbox", function () {
        $("#" + target + " .k-grid-content :checkbox").prop("checked", false);
        $(this).prop("checked", true);
    });
}
//eg:           bindDatePicker("input.MerchantChairmen.TermFrom","input.MerchantChairmen.TermTo")
function bindDatePicker(startin, endin) {
    var start = typeof (startin) != "string" ? start : $(startin).data("kendoDatePicker")
    var end = typeof (endin) != "string" ? end : $(endin).data("kendoDatePicker")
    if (start != null && end != null) {
        start.bind("change", function () {
            var startDate = start.value(),
                            endDate = end.value();

            if (startDate) {
                startDate = new Date(startDate);
                startDate.setDate(startDate.getDate());
                end.min(startDate);
            } else if (endDate) {
                start.max(new Date(endDate));
            } else {
                endDate = new Date();
                start.max(endDate);
                end.min(endDate);
            }
        })
        end.bind("change", function () {
            var endDate = end.value(),
                            startDate = start.value();

            if (endDate) {
                endDate = new Date(endDate);
                endDate.setDate(endDate.getDate());
                start.max(endDate);
            } else if (startDate) {
                end.min(new Date(startDate));
            } else {
                endDate = new Date();
                start.max(endDate);
                end.min(endDate);
            }
        })

        start.max(end.value());
        end.min(start.value());
    }
    else {
        setTimeout(function () {
            bindDatePicker(startin, endin);
        }, 500)
    }
}
function bindNoDatePicker(startin, endin) {
    var start = typeof (startin) != "string" ? start : $(startin).data("kendoDatePicker")
    var end = typeof (endin) != "string" ? end : $(endin).data("kendoDatePicker")
    if (start != null && end != null) {
        //start.bind("change", function () {
        //    var startDate = start.value(),
        //                    endDate = end.value();

        //    if (startDate) {
        //        startDate = new Date(startDate.getTime() + 1000 * 60 * 60 * 24);
        //        startDate.setDate(startDate.getDate());
        //        end.min(startDate);
        //    } else if (endDate) {
        //        start.max(new Date(endDate));
        //    } else {
        //        endDate = new Date();
        //        start.max(endDate);
        //        end.min(endDate);
        //    }
        //})
        //end.bind("change", function () {
        //    var endDate = end.value(),
        //                    startDate = start.value();

        //    if (endDate) {
        //        endDate = new Date(endDate.getTime() - 1000 * 60 * 60 * 24);
        //        endDate.setDate(endDate.getDate());
        //        start.max(endDate);
        //    } else if (startDate) {
        //        end.min(new Date(startDate));
        //    } else {
        //        endDate = new Date();
        //        start.max(endDate);
        //        end.min(endDate);
        //    }
        //})
        $(startin).bind("change", function () {
            var startDate = kendo.parseDate($(startin).val(), "dd/MM/yyyy"),
                            endDate = kendo.parseDate($(endin).val(), "dd/MM/yyyy");
            if (startDate <= start.max() && startDate != null) {
                startDate = new Date(startDate.getTime() + 1000 * 60 * 60 * 24);
                startDate.setDate(startDate.getDate());
                end.min(startDate);
            }
            else {
                $(startin).val("")
            }
        })
        $(endin).bind("change", function () {
            var endDate = kendo.parseDate($(endin).val(), "dd/MM/yyyy"),
                            startDate = kendo.parseDate($(startin).val(), "dd/MM/yyyy");
            if (endDate <= end.max() && endDate >= end.min() && endDate != null) {
                endDate = new Date(endDate.getTime() - 1000 * 60 * 60 * 24);
                endDate.setDate(endDate.getDate());
                start.max(endDate);
            } else {
                $(endin).val("");
            }
        })
        start.max(end.value());
        end.min(start.value());
    }
    else {
        setTimeout(function () {
            bindDatePicker(startin, endin);
        }, 500)
    }
}
function bindNoDatePickerLessThanFutureMonth(startin, endin) {
    var start = typeof (startin) != "string" ? start : $(startin).data("kendoDatePicker")
    var end = typeof (endin) != "string" ? end : $(endin).data("kendoDatePicker")
    if (start != null && end != null) {
        $(startin).bind("change", function () {
            var startDate = kendo.parseDate($(startin).val(), "dd/MM/yyyy"),
                            endDate = kendo.parseDate($(endin).val(), "dd/MM/yyyy");
            if (startDate <= start.max() && startDate != null) {
                startDate = new Date(startDate.getTime() + 1000 * 60 * 60 * 24);
                startDate.setDate(startDate.getDate());
                end.min(startDate);
            }
            else {
                $(startin).val("")
            }
        })
        $(endin).bind("change", function () {
            var endDate = kendo.parseDate($(endin).val(), "dd/MM/yyyy"),
                            startDate = kendo.parseDate($(startin).val(), "dd/MM/yyyy");
            if (endDate <= end.max() && endDate >= end.min() && endDate != null) {
                endDate = new Date(endDate.getTime() - 1000 * 60 * 60 * 24);
                endDate.setDate(endDate.getDate());
                start.max(endDate);
            } else {
                $(endin).val("");
            }
        })

        //start.bind("change", function () {
        //    var startDate = start.value(),
        //                    endDate = end.value();
        //    if (startDate) {
        //        startDate = new Date(startDate.getTime() + 1000 * 60 * 60 * 24);
        //        startDate.setDate(startDate.getDate());
        //        end.min(startDate);
        //    }
        //    //else if (endDate < LastDate && endDate != null) {
        //    //    start.max(new Date(endDate));
        //    //} else {
        //    //    endDate = LastDate;//new Date();
        //    //    start.max(endDate);
        //    //    end.max(endDate);
        //    //    //end.min(endDate);
        //    //}
        //})
        //end.bind("change", function () {
        //    var endDate = end.value(),
        //                    startDate = start.value();
        //    if (endDate) {
        //        endDate = new Date(endDate.getTime() - 1000 * 60 * 60 * 24);
        //        endDate.setDate(endDate.getDate());
        //        start.max(endDate);
        //    }
        //    //else if (startDate < LastDate) {
        //    //    end.min(new Date(startDate));
        //    //} else {
        //    //    endDate = new Date();
        //    //    start.max(endDate);
        //    //    end.min(endDate);
        //    //}
        //})

        var MonthDate = kendo.parseDate(kendo.toString(new Date(), "dd/MM/yyyy"), "dd/MM/yyyy");
        MonthDate.setDate(1);
        MonthDate.setMonth(MonthDate.getMonth() + 1);
        var LastDate = new Date(MonthDate.getTime() - 1000 * 60 * 60 * 24);

        start.max(LastDate);
        end.max(LastDate);
        //start.max(end.value());
        end.min(start.value());
    }
    else {
        setTimeout(function () {
            bindDatePicker(startin, endin);
        }, 500)
    }
}
function bindStartDatePicker(start, end) {
    start = typeof (start) != "string" ? start : $(start).data("kendoDatePicker")
    end = typeof (end) != "string" ? end : $(end).data("kendoDatePicker")

    start.bind("change", function () {
        var startDate = start.value(),
                        endDate = end.value();

        if (startDate) {
            startDate = new Date(startDate);
            startDate.setDate(startDate.getDate());
            end.min(startDate);
        } else if (endDate) {
            start.max(new Date(endDate));
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    })
    end.min(start.value());
}

function bindEndDatePicker(start, end) {
    start = typeof (start) != "string" ? start : $(start).data("kendoDatePicker")
    end = typeof (end) != "string" ? end : $(end).data("kendoDatePicker")

    end.bind("change", function () {
        var endDate = end.value(),
                        startDate = start.value();

        if (endDate) {
            endDate = new Date(endDate);
            endDate.setDate(endDate.getDate());
            start.max(endDate);
        } else if (startDate) {
            end.min(new Date(startDate));
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    })
    start.max(end.value());
}
function bindnumeri(Max, Value) {
    Max = typeof (Max) != "string" ? Max : $(Max).first().data("kendoNumericTextBox");
    Value = typeof (Value) != "string" ? Value : $(Value).first().data("kendoNumericTextBox");
    Max.bind("change", function () {
        var MaxValue = Max.value();
        Value.max(0);
        if (MaxValue > 0)
            Value.max(MaxValue);
    })
    Value.max(Max.value());
}
function InitDropDownList(target, dataText, dataValue, optionLabel, items) {
    $("#" + target).kendoDropDownList({
        dataTextField: dataText,
        dataValueField: dataValue,
        dataSource: {
            data: items,
            schema: {
                model: {
                    id: dataValue,
                    fields: {
                        dataValue: { type: "String" },
                        dataText: { type: "String" }
                    }
                }
            }
        },
        optionLabel: optionLabel
    });
}

// 刷新当前app 滚动条  不支持 IScroll 时  使用 overflow-y:auto 处理
var refreshCurrentScrolls = function (isInit) {
    if (myScrolls) {
        if (!myScrolls[CurrentApp.pane]) {
            if (document.addEventListener) {
                myScrolls[CurrentApp.pane] = new IScroll("#pane-wrapper-" + CurrentApp.pane, { scrollbars: true, mouseWheel: true, interactiveScrollbars: true, bounce: false, vScrollbar: false, fadeScrollbars: true, click: false })
                setTimeout(function () {
                    myScrolls[CurrentApp.pane].refresh();
                }, 500);
                //$(myScrolls[CurrentApp.pane].scroller).nextAll().css("visibility", "hidden");
            }
            else {
                $("#pane-wrapper-" + CurrentApp.pane).css("overflow-y", "auto");
            }
        } else {
            if (isInit) {
                myScrolls[CurrentApp.pane].refresh();
            }
            else {
                setTimeout(function () {
                    myScrolls[CurrentApp.pane].refresh();
                }, 500);
            }
        }
    }
}

//插入 上传控件
var insertUploadHeader = function (target, MaxSize) {
    var filesTarget = $(target).parents("div.k-upload").first().find("ul.k-upload-files");
    if (filesTarget.length != 0) {
        var fileheaderTarget = filesTarget.find("li.k-fileheader").first();
        if (fileheaderTarget.length == 0) {
            firstfiles = filesTarget.find("li:first");
            $("<li class=\"k-file k-file-success k-fileheader\">" +
                        "<div class=\"file-wrapper\">" +
                            "<table style=\"width: 100%;padding-right: 15px;\">" +
                                "<thead>" +
                                    "<tr>" +
                                        "<td style=\"width: 30%;\">File Name</td>" +
                                        "<td style=\"width: 20%;\">Uploaded Date/Time</td>" +
                                        "<td>Remark</td>" +
                                        "<td style=\"text-align: right;\">max size " + MaxSize + "MB</td>" +
                                    "</tr>" +
                                "</thead>" +
                            "</table>" +
                        "</div> " +
                "</li>").insertBefore(firstfiles);
        }
    }
}
var getJSMsg = function (Type, Key) {
    var msgs;
    try {
        msgs = App.JS.Msg[Type][Key];
    } catch (e) {

    }
    return msgs;
}
var getInputMsg = function (input) {
    var arr = input[0].name.split(' '),
        msgs = {};
    if (arr.length == 2) {
        try {
            msgs = App.TD.Msg[arr[0]][arr[1]];
        } catch (e) {

        }
    }
    return msgs;
}

var hasInputMsg = function (input, msg) {
    return getInputMsg(input).propertyIsEnumerable(msg);
}
var clearErrorsLog = function () {
    $(".errorslog").empty();
}
var InitkendoValidator = function (target) {
    $(target).kendoValidator({
        validateOnBlur: false,
        errorTemplate: "<div class='alert alert-xs alert-danger alert-dismissable'>"
                                    + "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>"
                                    + "#=message#"
                                + "</div>",
        rules: {
            requiredby: function (input) {
                if (hasInputMsg(input, "requiredby")) {
                    var type = input.data("type"),
                        value = input.data("requiredbyValue"),
                        other = $("[name='" + input.data("requiredbyField") + "']:checked").val(),
                        ispass = true,
                        otherValue = other ? other : $("[name='" + input.data("requiredbyField") + "']").val();
                    switch (type) {
                        case "date": ispass = kendo.parseDate(input.val(), "dd/MM/yyyy") != null; break;// input.data("kendoDatePicker").value() != null; break;
                        case "numeric": ispass = input.data("kendoNumericTextBox").value() != null; break;
                        default: ispass = input.val() != "";
                    }
                    return value != "" && (!other || otherValue == value) && (other || otherValue >= value) && ispass;
                }
                return true;
            },
            requiredbydate: function (input) {
                if (hasInputMsg(input, "requiredbydate")) {
                    var value = kendo.parseDate(input.val(), "dd/MM/yyyy"),// input.data("kendoDatePicker").value(),
                        other = $("[name='" + input.data("requiredbydateField") + "']").data("kendoDatePicker").value();
                    return other == null || value != null;
                }
                return true;
            },
            lessthantodate: function (input) {
                if (hasInputMsg(input, "lessthantodate") && input.val() != "") {
                    var date = kendo.parseDate(input.val(), "dd/MM/yyyy"),
                    todate = kendo.parseDate(kendo.toString(new Date(), "dd/MM/yyyy"), "dd/MM/yyyy");
                    //todate.setDate(todate.getDate());
                    return date != null && (todate.getTime() > date.getTime());
                }
                return true;
            },
            lessthanfuturemonth: function (input) {
                if (hasInputMsg(input, "lessthanfuturemonth") && input.val() != "") {
                    var date = kendo.parseDate(input.val(), "dd/MM/yyyy");
                    var nowDate = kendo.parseDate(new Date(), "dd/MM/yyyy");
                    nowDate.setDate(date.getDate());
                    return date != null && date < nowDate;
                }
                return true;
            },
            greaterdate: function (input) {
                if (hasInputMsg(input, "greaterdate") && input.val() != "") {
                    var date = kendo.parseDate(input.val(), "dd/MM/yyyy"),// input.data("kendoDatePicker").value(),
                        otherValue = $("[name='" + input.data("greaterdateField") + "']").val();
                    //otherDate = $("[name='" + input.data("greaterdateField") + "']").data("kendoDatePicker").value();
                    var otherDate = kendo.parseDate(otherValue, "dd/MM/yyyy");
                    $("[name='" + input.data("greaterdateField") + "']").data("kendoDatePicker").value(otherDate);

                    return date != null && ((otherValue != "" && otherDate != null) && otherDate.getTime() < date.getTime());
                }
                return true;
            },
            greaterzero: function (input) {
                if (hasInputMsg(input, "greaterzero") && input.val() != "") {
                    var number = input.data("kendoNumericTextBox").value();
                    return number != null && number > 0;
                }
                return true;
            },
            lessthannumber: function (input) {
                if (hasInputMsg(input, "lessthannumber") && input.val() != "") {
                    var number = input.data("kendoNumericTextBox").value(),
                        otherNumber = $("[name='" + input.data("lessthannumberField") + "']").data("kendoNumericTextBox").value();
                    return number != null && (otherNumber == null || otherNumber >= number);
                }
                return true;
            }
        },
        messages: {
            required: function (input) { return getInputMsg(input).required; },
            date: function (input) { return getInputMsg(input).date; },
            requiredby: function (input) { return getInputMsg(input).requiredby; },
            requiredbydate: function (input) { return getInputMsg(input).requiredbydate; },
            lessthantodate: function (input) { return getInputMsg(input).lessthantodate; },
            lessthanfuturemonth: function (input) { return getInputMsg(input).lessthanfuturemonth; },
            greaterdate: function (input) { return getInputMsg(input).greaterdate; },
            greaterzero: function (input) { return getInputMsg(input).greaterzero; },
            lessthannumber: function (input) { return getInputMsg(input).lessthannumber; }
        },
        validate: function (e) {
            if (e.valid === false) {
                this.hideMessages();
                var errors = this.errors();
                $(errors).each(function () {
                    $(".errorslog").append("<div class='alert alert-xs alert-danger alert-dismissable'>"
                                    + "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>"
                                    + this
                                + "</div>");
                });
            }
        }
    });
}

var InitSelectPersonWindow = function (target, type, callback, isShowNonReference, options) {
    if (isShowNonReference == undefined || isShowNonReference == null) {
        isShowNonReference = true;
    }
    var that = $(target);
    var curid = that.attr("swid");
    var arrtype = new Array();
    if (type.indexOf(",") > 0) {
        if (type.indexOf("All") >= 0) {
            arrtype.push("All");
        }
        else {
            var _arr = type.split(",");
            $.each(_arr, function (i, item) {
                if ($.inArray(item, arrtype) < 0) {
                    arrtype.push(item);
                }
            });
        }
    }
    else {
        arrtype.push(type);
    }

    var defaults = {
        actions: [
            "Pin",
            "Minimize",
            "Maximize",
            "Close"
        ],
        resizable: false,
        modal: true,
        draggable: true,
        mutilselect: true,
        pageSize: 15
    };
    $.extend(defaults, options);

    var toolbuttons = "";
    if (defaults.mutilselect) {
        toolbuttons = "            <td>"
                    + "                <input type=\"button\" value=\"&gt;&gt;\" id=\"btnAllRight\" class=\"btnAllRight\" style=\"width: 50px;\" /><br /><br />"
                    + "                <input type=\"button\" value=\"&gt;\" id=\"btnRight\" class=\"btnRight\" style=\"width: 50px;\" /><br /><br />"
                    + "                <input type=\"button\" value=\"&lt;\" id=\"btnLeft\" class=\"btnLeft\" style=\"width: 50px;\" /><br /><br />"
                    + "                <input type=\"button\" value=\"&lt;&lt;\" id=\"btnAllLeft\" class=\"btnAllLeft\" style=\"width: 50px;\" /><br />"
                    + "            </td>"
    }
    else {
        toolbuttons = "            <td>"
                   + "                <input type=\"button\" value=\"&gt;\" id=\"btnRight\" class=\"btnRight\" style=\"width: 50px;\" /><br /><br />"
                   + "                <input type=\"button\" value=\"&lt;\" id=\"btnLeft\" class=\"btnLeft\" style=\"width: 50px;\" /><br /><br />"
                   + "            </td>"
    }

    var userinfotemplate = "        <div id=\"SelectPersonInfo#swid#-#id#\" class=\"well\" style=\"height:80px;padding:0px;margin-top: 5px;\">"
                        + "            <div class=\"row\" style=\"margin-bottom:0px;\">      "
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"UserName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.UserName + ":</label><input id=\"UserName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"LastName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.LastName + ":</label><input id=\"LastName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"FirstName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.FirstName + ":</label><input id=\"FirstName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "            </div>"
                        + "            <div class=\"row\" style=\"margin-bottom:0px;\">      "
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"UserCompany#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.Company + ":</label><input id=\"UserCompany#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"UserDept#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.Department + ":</label><input id=\"UserDept#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "                 <div class=\"col-md-4\" style=\"margin-top:5px; display:inline;\">"
                        + "                   <label for=\"UserPosition#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;width:50px;\">" + jsResxbaseInitView.Position + ":</label><input id=\"UserPosition#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                        + "                 </div>"
                        + "            </div>"
                        + "        </div>";

    var template = "<div class=\"section\">"
                    + "    <div class=\"top-heading\">"
                    + "        <div class=\"top-title\" style=\"height:35px;line-height:32px; margin-left:90px;\">"
                    + "               <div class=\"col-lg-3 col-md-3  col-sm-3\" style=\"float:left;\">" + jsResxbaseInitView.Pleasefillinthekeyword + "</div>"
                    + "               <div class=\"col-lg-9 col-md-9  col-sm-9\">"
                    + "                      <input id=\"selectKey#swid#-#id#\"  class=\"selectKey k-textbox\" style=\"width: 45%\" /><span id=\"loading#swid#-#id#\" style=\"font-size: 12px;color: red;display:none;\"><img src=\"/CSS/kendoui/Default/loading_2x.gif\" width=\"20\"/>" + jsResxbaseInitView.Loading + "</span>"
                    + "               </div>"
                    + "        </div>"
                    + "    </div>"
                    + "    <div>"
                    + "        <div id=\"SelectPersonManaView#swid#-#id#\" class=\"well\" style=\"min-height:320px;padding:0px;margin-bottom:0px;\">"
                    + "            <div>"
                    + "                <div id=\"SelectPersonManageTreeView#swid#-#id#\" style=\"float:left;border: 1px solid #79b7e7;width: 240px; height:300px;margin-top: 1px;  margin-left:20px;overflow-y: auto;overflow-x: auto;-moz-border-radius: 5px;-webkit-border-radius: 5px;border-radius: 5px;\"></div>"
                    + "            </div>"
                    + "            <div id=\"SelectPersonInfomation#swid#-#id#\"  style=\" margin-left:270px;\">"
                                    + "<table>"
                                    + "        <tr>"
                                    + "            <td>"
                                    + "                <ul id=\"left#swid#-#id#\" class=\"listbox cleverlistbox ui-menu ui-widget ui-widget-content ui-corner-all\">"
                                    + "                </ul>"
                                    + "            </td>"
                                    + toolbuttons
                                    + "            <td>"
                                    + "                <ul id=\"right#swid#-#id#\" class=\"listbox cleverlistbox ui-menu ui-widget ui-widget-content ui-corner-all \">"
                                    + "                </ul>"
                                    + "            </td>"
                                    + "        </tr>"
                                    + "    </table>"
                    + "            </div>"
                    + "        </div>"
                    + "#userinfotemplate#"
                    + "    </div>"
                    + "</div>";

    if (!curid) {
        var id = "sw_" + Math.random().toString().substring(2);
        that.attr("swid", id);
        $("<div id=\"" + id + "\" style=\"display: none\">" +
        "    <div>" +
        "    <div id=\"tabstrip" + id + "\">" +
        "                    <ul>" +
        "                    </ul>" +
        "    </div>" +
        "    <div class=\"operabar\">" +
        "        <div class=\"operamask\"></div>" +
        "        <div class=\"operacontent\">" +
        "            <button class=\"k-button windowConfirm\" style=\"float: left;\">" + jsResxbaseInitView.Confirm + "</button>" +
        "            <button class=\"k-button windowCancel\" style=\"float: right\">" + jsResxbaseInitView.Cancel + "</button>" +
        "        </div>" +
        "    </div>" +
        "</div>").insertAfter($(target));
        curid = that.attr("swid");
    }

    var ClearHistory = function (jobj, type, id) {
        //清空历史数据
        jobj.data("persondata", []);
        jobj.data("positiondata", []);
        jobj.data("deptdata", []);
        jobj.data("custdata", []);

        if (id != undefined && id != null) {
            switch (type) {
                case "Person":
                case "Position":
                case "Department":
                case "Custom":
                case "Role":
                    $("#left" + id + "-" + type).html("");
                    $("#right" + id + "-" + type).html("");
                    $("#SelectPersonManageTreeView" + id + "-" + type).find("input").prop("checked", false);
                    $("#SelectPersonManageTreeView" + id + "-" + type).data("kendoTreeView").select($());

                    $("#selectKey" + id + "-" + type).val("");
                    break;
                default:
                    $("#left" + id + "-Person").html("");
                    $("#right" + id + "-Person").html("");
                    $("#left" + id + "-Position").html("");
                    $("#right" + id + "-Position").html("");
                    $("#left" + id + "-Department").html("");
                    $("#right" + id + "-Department").html("");
                    $("#left" + id + "-Custom").html("");
                    $("#right" + id + "-Custom").html("");
                    $("#left" + id + "-Role").html("");
                    $("#right" + id + "-Role").html("");

                    $("#SelectPersonManageTreeView" + id + "-Person").find("input").prop("checked", false);
                    $("#SelectPersonManageTreeView" + id + "-Position").find("input").prop("checked", false);
                    $("#SelectPersonManageTreeView" + id + "-Department").find("input").prop("checked", false);
                    $("#SelectPersonManageTreeView" + id + "-Custom").find("input").prop("checked", false);
                    $("#SelectPersonManageTreeView" + id + "-Role").find("input").prop("checked", false);

                    $("#SelectPersonManageTreeView" + id + "-Person").data("kendoTreeView").select($());

                    if ($("#SelectPersonManageTreeView" + id + "-Position").data("kendoTreeView")) {
                        $("#SelectPersonManageTreeView" + id + "-Position").data("kendoTreeView").select($());
                    }
                    if ($("#SelectPersonManageTreeView" + id + "-Department").data("kendoTreeView")) {
                        $("#SelectPersonManageTreeView" + id + "-Department").data("kendoTreeView").select($());
                    }
                    if ($("#SelectPersonManageTreeView" + id + "-Custom").data("kendoTreeView")) {
                        $("#SelectPersonManageTreeView" + id + "-Custom").data("kendoTreeView").select($());
                    }
                    if ($("#SelectPersonManageTreeView" + id + "-Role").data("kendoTreeView")) {
                        $("#SelectPersonManageTreeView" + id + "-Role").data("kendoTreeView").select($());
                    }

                    $("#selectKey" + id + "-Person").val("");
                    $("#selectKey" + id + "-Position").val("");
                    $("#selectKey" + id + "-Department").val("");
                    $("#selectKey" + id + "-Custom").val("");
                    $("#selectKey" + id + "-Role").val("");
                    break;
            }
        }
    }

    var swindow = $(document).find("#" + curid);
    if (!swindow.data("kendoWindow")) {

        swindow.kendoWindow({
            width: "800px",
            title: jsResxbaseInitView.SelectPersonWindowTitle,
            actions: defaults.actions,
            resizable: defaults.resizable,
            modal: defaults.modal,
            draggable: defaults.draggable,
            close: onClose
        });

        //清空listbox
        var clearlistbox = function (id) {
            $("#" + id).html("");
        }

        //listbox添加项目
        var addtolistbox = function (id, item) {
            var flag = listboxexistsitem(id, item);
            if (!flag && item.id.length > 0) {
                $("#" + id).append("<li class=\"ui-menu-item ui-corner-all\"><a id=\"" + item.id + "\" class=\"ui-corner-all\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\"  dataFirstName=\"" + item.FirstName + "\" dataText=\"" + item.text + "\" dataLastName=\"" + item.LastName + "\" dataDisplayName=\"" + item.DisplayName + "\">" + item.text + "</a></li>");
            }
        }

        //listbox添加Person项目
        var addPersontolistbox = function (id, item) {
            var flag = listboxexistsitem(id, item);
            if (!flag && item.id.length > 0) {
                $("#" + id).append("<li class=\"ui-menu-item ui-corner-all\"><a id=\"" + item.id + "\" class=\"ui-corner-all\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\" dataFirstName=\"" + item.FirstName + "\" dataText=\"" + item.text + "\" dataLastName=\"" + item.LastName + "\" dataDisplayName=\"" + item.DisplayName + "\" dataCompany=\"" + item.Company + "\" dataDepartment=\"" + item.Department + "\" dataPosition=\"" + item.Position + "\">" + item.DisplayName + "</a></li>");
            }
        }

        //listbox添加更多按钮
        var addMoretolistbox = function (type, id, pageIndex) {
            $("#More" + type).remove();
            $("#" + id).append("<li id=\"More" + type + "\" class=\"ui-menu-item ui-corner-all\"><a  class=\"ui-corner-more ui-corner-all\" tabindex=\"-1\" data-index=\"" + pageIndex + "\">" + jsResxbaseInitView.More + "</a></li>");
        }

        //listbox移除项目
        var removetolistbox = function (id, item) {
            $("#" + id).find("#" + item.id).parent().remove();
        }

        //获取listbox选中项
        var getlistboxselectitem = function (id) {
            var items = [];
            var as = $("#" + id + " li a.ui-state-active");

            $.each(as, function (i) {
                items.push({ id: as[i].id, text: $(as[i]).attr("dataText"), FirstName: $(as[i]).attr("dataFirstName"), LastName: $(as[i]).attr("dataLastName"), DisplayName: $(as[i]).attr("dataDisplayName"), Company: $(as[i]).attr("dataCompany"), Department: $(as[i]).attr("dataDepartment"), Position: $(as[i]).attr("dataPosition") });
            });
            return items;
        }

        //获取listbox当前全部项
        var getlistboxallitem = function (id) {
            var items = [];
            var as = $("#" + id + " li a");
            $.each(as, function (i) {
                items.push({ id: as[i].id, text: $(as[i]).attr("dataText"), FirstName: $(as[i]).attr("dataFirstName"), LastName: $(as[i]).attr("dataLastName"), DisplayName: $(as[i]).attr("dataDisplayName"), Company: $(as[i]).attr("dataCompany"), Department: $(as[i]).attr("dataDepartment"), Position: $(as[i]).attr("dataPosition") });
            });
            return items;
        }

        var listboxexistsitem = function (id, item) {
            var flag = false;
            var as = $("#" + id + " li a");
            $.each(as, function (i) {
                if (item.id == as[i].id) {
                    flag = true;
                }
            });
            return flag;
        }

        var onClose = function () {

        }
        //tree datasource
        var initPersonData = function () {
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/Maintenance/Organization/GetOrgChartTree?isshownonreference=" + isShowNonReference + "&tree=Person&Type={0}", "Root");
                        },
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",               //绑定ID
                        hasChildren: "HasChildNode"  //绑定是否包含子节点                 
                    }
                }
            });
        }
        var initPositionData = function () {
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/Maintenance/Position/GetPosition");
                        },
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",               //绑定ID
                        hasChildren: "HasChildren"  //绑定是否包含子节点                 
                    }
                }
            });
        }
        var initCommonData = function () {
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/Maintenance/Organization/GetOrgChartTree?Type={0}", "Root");
                        },
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",               //绑定ID
                        hasChildren: "HasChildNode"  //绑定是否包含子节点                 
                    }
                }
            });
        }

        var initCustomRoleData = function () {
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/Maintenance/CustomRole/GetCustomRoleByCommonControl");
                        },
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",               //绑定ID
                        hasChildren: "HasChildNode"  //绑定是否包含子节点                 
                    }
                }
            });
        }

        //系统角色
        var initSystemRoleData = function () {
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/Maintenance/Organization/GetRolesList?pane={0}", window.CurrentApp.pane);
                        },
                        dataType: "json"
                    }
                },
                schema: {
                    model: {
                        id: "ID",               //绑定ID
                        hasChildren: "HasChildNode"  //绑定是否包含子节点                 
                    }
                }
            });
        }

        var GetCurrentType = function () {
            return that.attr("tabcurtype");
        }


        //初始化tree
        var initTreeView = function (tabtype) {
            var treedatasource;
            var treetemplate = "# if(item.Type != \"Root\"){ #"
                         + "            <span class=\"k-sprite property\"></span>"
                         + "            # }else{ #"
                         + "            <span class=\"k-sprite folder\"></span>"
                         + "            # } #  "
                         + "            <input type=\"checkbox\" value=\"#= item.ID #\" data-type=\"#= item.Type #\" />"
                         + "            #: item.NodeName #";
            switch (tabtype) {
                case "Person":
                    treedatasource = initPersonData();
                    break;
                case "Position":
                    treedatasource = initPositionData();
                    treetemplate = "# if(item.Type != \"Category\"){ #"
                             + "            <span class=\"k-sprite property\"></span>"
                             + "            # }else{ #"
                             + "            <span class=\"k-sprite folder\"></span>"
                             + "            # } #  "
                             + "            <input type=\"checkbox\" value=\"#= item.ID #\" data-type=\"#= item.Type #\" />"
                             + "            #: item.NodeName #";
                    break;
                case "Department":
                    treedatasource = initCommonData();
                    break;
                case "Custom":
                    treedatasource = initCustomRoleData();
                    break;
                case "Role":
                    treedatasource = initSystemRoleData();
                    break;
            }
            var SelectPersonManageTreeView = $("#SelectPersonManageTreeView" + curid + "-" + tabtype).data("kendoTreeView");
            if (!SelectPersonManageTreeView) {
                $("#SelectPersonManageTreeView" + curid + "-" + tabtype).kendoTreeView({
                    template: kendo.template(treetemplate),
                    dataSource: treedatasource,
                    select: function (e) {
                        $("#SelectPersonManageTreeView" + curid + "-" + tabtype).find("input").prop("checked", false);
                        var select = $("#SelectPersonManageTreeView" + curid + "-" + tabtype + "_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
                        var item = $("#SelectPersonManageTreeView" + curid + "-" + tabtype).data("kendoTreeView").dataSource.get(select.val());
                        var itemtype = select.attr("data-type");

                        //clearlistbox("right" + curid + "-" + tabtype);

                        var key = $("#selectKey" + curid + "-" + tabtype).val();

                        $("#UserName" + curid + "-" + tabtype).val("");
                        $("#FirstName" + curid + "-" + tabtype).val("");
                        $("#LastName" + curid + "-" + tabtype).val("");
                        $("#UserCompany" + curid + "-" + tabtype).val("");
                        $("#UserDept" + curid + "-" + tabtype).val("");
                        $("#UserPosition" + curid + "-" + tabtype).val("");

                        switch (tabtype) {
                            case "Person":
                                $.getJSON("/Maintenance/Organization/GetSelectPersonUserByNode", { _t: new Date(), id: select.val().toString().substring(2), type: itemtype, keyword: key, pageIndex: 1, pageSize: defaults.pageSize, isshownonreference: isShowNonReference }, function (items) {
                                    clearlistbox("left" + curid + "-" + tabtype);
                                    for (var j = 0; j < items.length; j++) {
                                        addPersontolistbox("left" + curid + "-" + tabtype, items[j]);
                                    }
                                    if (items.length >= defaults.pageSize) {
                                        addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, 1);
                                    }
                                    that.data("persondata", items);
                                });
                                break;
                            case "Position":
                                $.getJSON("/Maintenance/Organization/GetSelectPersonPositionByNode", { _t: new Date(), id: select.val().toString(), type: item.Type, keyword: key, pageIndex: 1, pageSize: defaults.pageSize, isshownonreference: isShowNonReference }, function (items) {
                                    clearlistbox("left" + curid + "-" + tabtype);
                                    for (var j = 0; j < items.length; j++) {
                                        addtolistbox("left" + curid + "-" + tabtype, items[j]);
                                    }
                                    if (items.length >= defaults.pageSize) {
                                        addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, 1);
                                    }
                                    that.data("positiondata", items);
                                });
                                break;
                            case "Department":
                                $.getJSON("/Maintenance/Organization/GetSelectPersonDeptByNode", { _t: new Date(), id: select.val().toString().substring(2), type: itemtype, keyword: key, pageIndex: 1, pageSize: defaults.pageSize }, function (items) {
                                    clearlistbox("left" + curid + "-" + tabtype);
                                    for (var j = 0; j < items.length; j++) {
                                        addtolistbox("left" + curid + "-" + tabtype, items[j]);
                                    }
                                    if (items.length >= defaults.pageSize) {
                                        addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, 1);
                                    }
                                    that.data("deptdata", items);
                                });
                                break;
                            case "Custom":
                                $.getJSON("/Maintenance/CustomRole/GetClassifyByCommonControl", { _t: new Date(), id: select.val().toString(), type: itemtype, keyword: key, pageIndex: 1, pageSize: defaults.pageSize }, function (items) {
                                    clearlistbox("left" + curid + "-" + tabtype);
                                    for (var j = 0; j < items.length; j++) {
                                        addtolistbox("left" + curid + "-" + tabtype, items[j]);
                                    }
                                    if (items.length >= defaults.pageSize) {
                                        addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, 1);
                                    }
                                    that.data("deptdata", items);
                                });
                                break;
                            case "Role":
                                var roleid = select.val();
                                if (roleid.toString().substring(0, 1) == "1") {
                                    $.getJSON("/Maintenance/Organization/GetRolesListByCategory", { _t: new Date(), ID: roleid, pane: window.CurrentApp.pane, keyword: key, pageIndex: 1, pageSize: defaults.pageSize }, function (items) {
                                        clearlistbox("left" + curid + "-" + tabtype);
                                        for (var j = 0; j < items.length; j++) {
                                            addtolistbox("left" + curid + "-" + tabtype, items[j]);
                                        }
                                        if (items.length >= defaults.pageSize) {
                                            addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, 1);
                                        }
                                    });
                                }
                                else {
                                    var roleitem = { id: select.val().toString().substring(2), text: item.NodeName, FirstName: "", LastName: "", DisplayName: "" }
                                    clearlistbox("left" + curid + "-" + tabtype);
                                    addtolistbox("left" + curid + "-" + tabtype, roleitem);
                                }
                                break;
                        }
                    },
                    collapse: function (e) {
                        var tab = that.attr("tabcurtype");
                        $("#SelectPersonManageTreeView" + curid + "-" + tab + "_tv_active").find(".k-sprite").first().removeClass("on");
                    },
                    expand: function (e) {
                        var tab = that.attr("tabcurtype");
                        $("#SelectPersonManageTreeView" + curid + "-" + tab + "_tv_active").find(".k-sprite").first().addClass("on");
                    },
                    dataBound: function (e) {
                        var tab = that.attr("tabcurtype");
                        var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                        var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                        $("#SelectPersonManageTreeView" + curid + "-" + tab).find(":checkbox").unbind(clickevent).bind(clickevent);
                        $("#SelectPersonManageTreeView" + curid + "-" + tab).off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)

                        var list = document.getElementById("SelectPersonManageTreeView" + curid + "-" + tab).getElementsByTagName('ul')[0].children;
                        $.each(list, function (i, item) {
                            $("#SelectPersonManageTreeView" + curid + "-" + tab).data("kendoTreeView").expand(item);
                            var ul = item.getElementsByTagName('ul');
                            if (ul != undefined && ul != null && ul.length > 0) {
                                var lis = ul[0].children;
                                if (lis != undefined && lis != null && lis.length > 0) {
                                    $("#SelectPersonManageTreeView" + curid + "-" + tab).data("kendoTreeView").expand(lis[0]);
                                }
                            }
                        });
                    }
                });
                SelectPersonManageTreeView = $("#SelectPersonManageTreeView" + curid + "-" + tabtype).data("kendoTreeView");
                TreeViewNodeToggle("SelectPersonManageTreeView" + curid + "-" + tabtype);
            }
            else {
                //SelectPersonManageTreeView.setDataSource(treedatasource);
            }
        }
        $("#tabstrip" + curid).kendoTabStrip({
            animation: {
                open: {
                    effects: "fadeIn"
                }
            }, select: function (e) {
                var tabtype = $(e.item).find("font").attr("data-type");
                if (tabtype != undefined) {
                    that.attr("tabcurtype", tabtype);
                    initTreeView(tabtype);
                }
                else {
                    that.attr("tabcurtype", "Person");
                    initTreeView("Person");
                }
            }
        });

        $("#tabstrip" + curid).delegate("span.glyphicon-refresh", "click", function () {
            var _tabtype = $(this).next().attr("data-type");
            var _treeView = $("#SelectPersonManageTreeView" + curid + "-" + _tabtype).data("kendoTreeView");
            var _treedatasource;
            switch (_tabtype) {
                case "Person":
                    _treedatasource = initPersonData();
                    break;
                case "Position":
                    _treedatasource = initPositionData();
                    break;
                case "Department":
                    _treedatasource = initCommonData();
                    break;
                case "Custom":
                    _treedatasource = initCustomRoleData();
                    break;
                case "Role":
                    _treedatasource = initSystemRoleData();
                    break;
            }
            clearlistbox("left" + curid + "-" + _tabtype);
            _treeView.setDataSource(_treedatasource);
        });

        var tabStrip = $("#tabstrip" + curid).data("kendoTabStrip");
        var ShowTab = function (title, url, text, type) {
            tabStrip.append(
                  [{
                      text: "<span class='glyphicon glyphicon-refresh'></span><font data-type='" + type + "'>" + title + "</font>",
                      content: text,
                      encoded: false
                  }]
              );
        }
        var ShowAllTab = function () {
            RemoveAllTab();
            ShowTab(jsResxbaseInitView.Person, "", template.replace(/#userinfotemplate#/g, userinfotemplate).replace(/#id#/g, "Person").replace(/#swid#/g, curid), "Person");
            ShowTab(jsResxbaseInitView.Position, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Position").replace(/#swid#/g, curid), "Position");
            ShowTab(jsResxbaseInitView.Department, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Department").replace(/#swid#/g, curid), "Department");
            ShowTab(jsResxbaseInitView.CustomRoles, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Custom").replace(/#swid#/g, curid), "Custom");
            ShowTab(jsResxbaseInitView.SystemRoles, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Role").replace(/#swid#/g, curid), "Role");
        }

        var RemoveAllTab = function () {
            var items = tabStrip.items();
            $.each(items, function (i) {
                tabStrip.remove(0);
            })
        }
        //初始化tab
        if (arrtype.length > 0) {
            RemoveAllTab();
            $.each(arrtype, function (i, item) {
                switch (item) {
                    case "Person":
                        ShowTab(jsResxbaseInitView.Person, "", template.replace(/#userinfotemplate#/g, userinfotemplate).replace(/#id#/g, "Person").replace(/#swid#/g, curid), "Person");
                        break;
                    case "Position":
                        ShowTab(jsResxbaseInitView.Position, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Position").replace(/#swid#/g, curid), "Position");
                        break;
                    case "Department":
                        ShowTab(jsResxbaseInitView.Department, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Department").replace(/#swid#/g, curid), "Department");
                        break;
                    case "Custom":
                        ShowTab(jsResxbaseInitView.CustomRoles, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Custom").replace(/#swid#/g, curid), "Custom");
                        break;
                    case "Role":
                        ShowTab(jsResxbaseInitView.SystemRoles, "", template.replace(/#userinfotemplate#/g, "").replace(/#id#/g, "Role").replace(/#swid#/g, curid), "Role");
                        break;
                    case "All":
                        ShowAllTab();
                        break;
                    default:
                        ShowAllTab();
                        break;
                }
            });
        }
        else {
            RemoveAllTab();
            ShowAllTab();
        }
        //初始化选择项
        tabStrip.select(0);

        //确认
        $("#" + curid + " .windowConfirm").bind("click", function () {
            var json = { "Root": { "Users": { "Item": [] }, "Depts": { "Item": [] }, "Positions": { "Item": [] }, "CustomRoles": { "Item": [] }, "SystemRoles": { "Item": [] } } };
            var tabs = tabStrip.items();
            var count = 0;
            $.each(tabs, function (i) {
                var curtabtype = $(tabs[i]).find("font").attr("data-type");
                var items = getlistboxallitem("right" + curid + "-" + curtabtype);
                $.each(items, function (i) {
                    switch (curtabtype) {
                        case "Person":
                            json.Root.Users.Item.push({ Value: items[i].id, Name: items[i].DisplayName, UserName: items[i].text });
                            break;
                        case "Position":
                            json.Root.Positions.Item.push({ Value: items[i].id, Name: items[i].text });
                            break;
                        case "Department":
                            json.Root.Depts.Item.push({ Value: items[i].id, Name: items[i].text });
                            break;
                        case "Custom":
                            json.Root.CustomRoles.Item.push({ Value: items[i].id, Name: items[i].text });
                            break;
                        case "Role":
                            json.Root.SystemRoles.Item.push({ Value: items[i].id, Name: items[i].text });
                            break;
                    }
                });
                count += items.length;
            });
            that.data("currentJson", json);
            if (count <= 0) {
                ShowTip(jsResxbaseInitView.Donotchooseanyitem, "error");
                return;
            }
            if (callback != undefined) {
                var e = { target: that };
                callback(json, e);
            }
            if (defaults.onDetermine != undefined && defaults.onDetermine != null) {
                defaults.onDetermine();
            }
            else {
                swindow.data("kendoWindow").close();
            }
        });
        //取消
        $("#" + curid + " .windowCancel").bind("click", function () {
            if (defaults.onClose != undefined && defaults.onClose != null) {
                defaults.onClose();
            }
            else {
                swindow.data("kendoWindow").close();
            }
        });

        var getJsonToList = function (curtype, curid, allowpage, addside, clearside, url, data, pageIndex, isMore) {
            $("#loading" + curid + "-" + curtype).show();
            $.getJSON(url, data, function (items) {
                $("#loading" + curid + "-" + curtype).hide();
                if (!isMore || clearside == "left") {
                    clearlistbox(clearside + curid + "-" + curtype);
                }
                if (curtype == "Person") {
                    $.each(items, function (i, item) {
                        addPersontolistbox(addside + curid + "-" + curtype, item);
                    });
                }
                else {
                    $.each(items, function (i, item) {
                        addtolistbox(addside + curid + "-" + curtype, item);
                    });
                }
                if (allowpage && items.length >= defaults.pageSize) {
                    addMoretolistbox(curtype, addside + curid + "-" + curtype, pageIndex);
                }
                else {
                    $("#More" + curtype).remove();
                }
                if (isMore && items.length == 0) {
                    $("#More" + curtype).remove();
                }
            });
        }

        //搜索符合条件的人，职位，部门
        var searchperson = function (addside, clearside, allowpage, pageIndex, isMore) {
            var curtabtype = that.attr("tabcurtype");
            var key = $("#selectKey" + curid + "-" + curtabtype).val();
            var select = $("#SelectPersonManageTreeView" + curid + "-" + curtabtype + "_tv_active").find("input").first().prop("checked", true);
            var node = $("#SelectPersonManageTreeView" + curid + "-" + curtabtype).data("kendoTreeView").dataSource.get(select.val());
            var itemtype = "";
            if ((key.length == 0 || key.length >= 1)) {      //&& select != null && select != undefined && select.length == 1 
                var selectid = "";
                if (select != null && select != undefined && select.length > 0) {
                    selectid = select.val().toString().substring(2);
                    itemtype = select.attr("data-type");
                }
                var jsonparams = { _t: new Date(), id: selectid, type: itemtype, pageIndex: pageIndex, pageSize: defaults.pageSize, isshownonreference: isShowNonReference, keyword: key, allowpage: allowpage };
                switch (curtabtype) {
                    case "Person":
                        getJsonToList(curtabtype, curid, allowpage, addside, clearside, "/Maintenance/Organization/GetSelectPersonUserByNode", jsonparams, pageIndex, isMore);
                        break;
                    case "Position":
                        jsonparams.id = "00000000-0000-0000-0000-000000000000";
                        if (select != null && select != undefined && select.length > 0) {
                            jsonparams.id = select.val();
                        }
                        getJsonToList(curtabtype, curid, allowpage, addside, clearside, "/Maintenance/Organization/GetSelectPersonPositionByNode", jsonparams, pageIndex, isMore);
                        break;
                    case "Department":
                        getJsonToList(curtabtype, curid, allowpage, addside, clearside, "/Maintenance/Organization/GetSelectPersonDeptByNode", jsonparams, pageIndex, isMore);
                        break;
                    case "Custom":
                        jsonparams.id = "00000000-0000-0000-0000-000000000000";
                        if (select != null && select != undefined && select.length > 0) {
                            jsonparams.id = select.val();
                        }
                        getJsonToList(curtabtype, curid, allowpage, addside, clearside, "/Maintenance/CustomRole/GetClassifyByCommonControl", jsonparams, pageIndex, isMore);
                        break;
                    case "Role":
                        var roleid = "";
                        if (select != null && select != undefined && select.length > 0) {
                            var roleid = select.val();
                        }
                        if (roleid.toString().substring(0, 1) == "1" || roleid == "") {
                            getJsonToList(curtabtype, curid, allowpage, addside, clearside, "/Maintenance/Organization/GetRolesListByCategory", { _t: new Date(), ID: roleid, pane: window.CurrentApp.pane, pageIndex: pageIndex, pageSize: defaults.pageSize, keyword: key }, pageIndex, isMore);
                        }
                        else {
                            clearlistbox(clearside + curid + "-" + curtabtype);
                            if ((key.length > 0 && node.NodeName.toLowerCase().indexOf(key.toLowerCase()) >= 0) || key.length == 0) {
                                var roleitem = { id: select.val().toString().substring(2), text: node.NodeName, FirstName: "", LastName: "", DisplayName: "" }
                                addtolistbox(addside + curid + "-" + curtabtype, roleitem);
                            }
                        }
                        break;
                }
            }
        }

        $('ul.listbox').off('click mouseover mouseout dblclick').on('click mouseover mouseout dblclick', 'a', function (event) {
            if (event.type == 'dblclick' && !$(this).hasClass('ui-corner-more')) {
                var curtabtype = that.attr("tabcurtype");
                var curultype = $(this).attr("ultype");
                var curitem = { id: $(this).attr("id"), text: $(this).attr("dataText"), FirstName: $(this).attr("dataFirstName"), LastName: $(this).attr("dataLastName"), DisplayName: $(this).attr("dataDisplayName"), Company: $(this).attr("dataCompany"), Department: $(this).attr("dataDepartment"), Position: $(this).attr("dataPosition") };
                if (curultype == "l") {
                    //如果非多选则同时移除右边还原到左边
                    if (!defaults.mutilselect) {
                        var rightitems = getlistboxallitem("right" + curid + "-" + curtabtype);
                        //移除右边
                        $.each(rightitems, function (i) {
                            removetolistbox("right" + curid + "-" + curtabtype, rightitems[i]);
                        });

                        //判断是否包含更多选项
                        var limore = $("#" + "left" + curid + "-" + curtabtype).find("a.ui-corner-more");
                        var ismore = limore.length > 0 ? true : false;
                        var pageIndex = limore.attr("data-index");
                        //移除更多选项
                        $("#More" + curtabtype).remove();
                        if (curtabtype == "Person") {
                            $.each(rightitems, function (i) {
                                addPersontolistbox("left" + curid + "-" + curtabtype, rightitems[i]);
                            });
                        }
                        else {
                            $.each(rightitems, function (i) {
                                addtolistbox("left" + curid + "-" + curtabtype, rightitems[i]);
                            });
                        }
                        if (ismore) {
                            addMoretolistbox(curtabtype, "left" + curid + "-" + curtabtype, pageIndex);
                        }
                    }
                    //添加到右边
                    if (curtabtype == "Person") {
                        addPersontolistbox("right" + curid + "-" + curtabtype, curitem);
                    }
                    else {
                        addtolistbox("right" + curid + "-" + curtabtype, curitem);
                    }
                }
                else {
                    //添加到左边
                    //判断是否包含更多选项
                    var limore = $("#" + "left" + curid + "-" + curtabtype).find("a.ui-corner-more");
                    var ismore = limore.length > 0 ? true : false;
                    var pageIndex = limore.attr("data-index");
                    //移除更多选项
                    $("#More" + curtabtype).remove();
                    if (curtabtype == "Person") {
                        addPersontolistbox("left" + curid + "-" + curtabtype, curitem);
                    }
                    else {
                        addtolistbox("left" + curid + "-" + curtabtype, curitem);
                    }
                    if (ismore) {
                        addMoretolistbox(curtabtype, "left" + curid + "-" + curtabtype, pageIndex);
                    }
                }
                $(this).remove();
            }
            else if (event.type == 'click') {
                //更多
                if ($(this).hasClass('ui-corner-more')) {
                    var pageindex = $(this).attr("data-index");
                    pageindex = parseInt(pageindex) + 1;
                    searchperson("left", "right", true, pageindex, true);
                }
                else {
                    if (!defaults.mutilselect) {
                        //清除其他选择项
                        $('ul.listbox a').removeClass('ui-state-active');
                    }
                    var a = $(this);
                    if (a.hasClass('ui-state-active')) {
                        a.removeClass('ui-state-active');
                    } else {
                        a.addClass('ui-state-active');
                    }
                    var curtabtype = that.attr("tabcurtype");
                    var username = $(this).attr("dataText");
                    var firstname = $(this).attr("dataFirstName");
                    var lastname = $(this).attr("dataLastName");
                    var company = $(this).attr("dataCompany");
                    var department = $(this).attr("dataDepartment");
                    var position = $(this).attr("dataPosition");
                    if (username != undefined) {
                        $("#UserName" + curid + "-" + curtabtype).val(username);
                    }
                    if (firstname != undefined) {
                        $("#FirstName" + curid + "-" + curtabtype).val(firstname);
                    }
                    if (lastname != undefined) {
                        $("#LastName" + curid + "-" + curtabtype).val(lastname);
                    }
                    if (company != undefined) {
                        $("#UserCompany" + curid + "-" + curtabtype).val(company);
                    }
                    if (department != undefined) {
                        $("#UserDept" + curid + "-" + curtabtype).val(department);
                    }
                    if (position != undefined) {
                        $("#UserPosition" + curid + "-" + curtabtype).val(position);
                    }
                }
            }
            else if (event.type == 'mouseover' && !$(this).hasClass('ui-corner-more')) {
                if (!$(this).hasClass('ui-state-active')) {
                    $(this).addClass('ui-state-hover');
                }
            } else {
                $(this).removeClass('ui-state-hover');
            }
        });
        //为listbox添加按钮事件
        $(".btnRight").click(function () {
            var curtabtype = that.attr("tabcurtype");
            var items = getlistboxselectitem("left" + curid + "-" + curtabtype);
            //移除左边
            $.each(items, function (i) {
                removetolistbox("left" + curid + "-" + curtabtype, items[i]);
            });
            //如果非多选则同时移除右边还原到左边
            if (!defaults.mutilselect) {
                var rightitems = getlistboxallitem("right" + curid + "-" + curtabtype);
                //移除右边
                $.each(rightitems, function (i) {
                    removetolistbox("right" + curid + "-" + curtabtype, rightitems[i]);
                });
                //判断是否包含更多选项
                var limore = $("#" + "left" + curid + "-" + curtabtype).find("a.ui-corner-more");
                var ismore = limore.length > 0 ? true : false;
                var pageIndex = limore.attr("data-index");
                //移除更多选项
                $("#More" + curtabtype).remove();
                if (curtabtype == "Person") {
                    $.each(rightitems, function (i) {
                        addPersontolistbox("left" + curid + "-" + curtabtype, rightitems[i]);
                    });
                }
                else {
                    $.each(rightitems, function (i) {
                        addtolistbox("left" + curid + "-" + curtabtype, rightitems[i]);
                    });
                }
                if (ismore) {
                    addMoretolistbox(curtabtype, "left" + curid + "-" + curtabtype, pageIndex);
                }
            }
            //添加到右边
            if (curtabtype == "Person") {
                $.each(items, function (i) {
                    addPersontolistbox("right" + curid + "-" + curtabtype, items[i]);
                });
            }
            else {
                $.each(items, function (i) {
                    addtolistbox("right" + curid + "-" + curtabtype, items[i]);
                });
            }
        });

        $(".btnAllRight").click(function () {
            //获取所有
            searchperson("right", "left", false, 1);
        });

        $(".btnLeft").click(function () {
            var curtabtype = that.attr("tabcurtype");
            var items = getlistboxselectitem("right" + curid + "-" + curtabtype);
            //移除右边
            $.each(items, function (i) {
                removetolistbox("right" + curid + "-" + curtabtype, items[i]);
            });
            //添加到左边
            //判断是否包含更多选项
            var limore = $("#" + "left" + curid + "-" + curtabtype).find("a.ui-corner-more");
            var ismore = limore.length > 0 ? true : false;
            var pageIndex = limore.attr("data-index");
            //移除更多选项
            $("#More" + curtabtype).remove();
            if (curtabtype == "Person") {
                $.each(items, function (i) {
                    addPersontolistbox("left" + curid + "-" + curtabtype, items[i]);
                });
            }
            else {
                $.each(items, function (i) {
                    addtolistbox("left" + curid + "-" + curtabtype, items[i]);
                });
            }
            if (ismore) {
                addMoretolistbox(curtabtype, "left" + curid + "-" + curtabtype, pageIndex);
            }
        });

        $(".btnAllLeft").click(function () {
            searchperson("left", "right", true, 1);
        });

        $(".selectKey").keyup(function (event) {
            if (event.keyCode == 13) {
                searchperson("left", "left", true, 1);
            }
        });
    }
    $("#loading" + curid + "-" + that.attr("tabcurtype")).hide();
    swindow.data("kendoWindow").center().open();
    ClearHistory(that, that.attr("tabcurtype"), curid);
    var currentJson = that.data("currentJson");
    if (currentJson != null) {
        var listboxexistsitemforInitData = function (id, item) {
            var flag = false;
            var as = $("#" + id + " li a");
            $.each(as, function (i) {
                if (item.Value == as[i].id) {
                    flag = true;
                }
            });
            return flag;
        }

        //listbox添加项目
        var addtolistboxforInitData = function (id, item) {
            var flag = listboxexistsitemforInitData(id, item);
            if (!flag && item.Value.length > 0) {
                $("#" + id).append("<li class=\"ui-menu-item ui-corner-all\"><a id=\"" + item.Value + "\" class=\"ui-corner-all\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\" dataText=\"" + item.Name + "\"  dataFirstName=\"\" dataLastName=\"\" dataDisplayName=\"\" >" + item.Name + "</a></li>");
            }
        }

        //listbox添加Person项目
        var addPersontolistboxforInitData = function (id, item) {
            var flag = listboxexistsitemforInitData(id, item);
            if (!flag && item.Value.length > 0) {
                $("#" + id).append("<li class=\"ui-menu-item ui-corner-all\"><a id=\"" + item.Value + "\" class=\"ui-corner-all\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\" dataText=\"" + item.UserName + "\" dataFirstName=\"\" dataLastName=\"\" dataDisplayName=\"" + item.Name + "\" dataCompany=\"\" dataDepartment=\"\" dataPosition=\"\">" + item.Name + "</a></li>");
            }
        }

        var kendoTabStrip = $("#tabstrip" + curid).data("kendoTabStrip");
        var tabs = kendoTabStrip.items();
        $.each(tabs, function (i) {
            var curtabtype = $(tabs[i]).find("font").attr("data-type");
            switch (curtabtype) {
                case "Person":
                    var items = currentJson.Root.Users.Item;
                    $.each(items, function (i, item) {
                        addPersontolistboxforInitData("right" + curid + "-" + curtabtype, item);
                    });
                    break;
                case "Position":
                    var items = currentJson.Root.Positions.Item;
                    $.each(items, function (i, item) {
                        addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                    });
                    break;
                case "Department":
                    var items = currentJson.Root.Depts.Item;
                    $.each(items, function (i, item) {
                        addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                    });
                    break;
                case "Custom":
                    var items = currentJson.Root.CustomRoles.Item;
                    $.each(items, function (i, item) {
                        addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                    });
                    break;
                case "Role":
                    var items = currentJson.Root.SystemRoles.Item;
                    $.each(items, function (i, item) {
                        addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                    });
                    break;
            }
        });
    }
};

function InitSelectPersonControl() {
    $(document).delegate("*[data-control='userpick']", "click", function () {
        var that = $(this);
        //属性绑定事件
        var datatarget = that.attr("data-target");
        var datacontroltype = that.attr("data-controltype");
        var datacallback = that.attr("data-callback");
        var options = that.attr("data-options");
        if (options != undefined && options.length > 0) { options = eval('(' + options + ')'); }
        var callback = null;
        var defaultoptions = {};
        if (datacontroltype == undefined) {
            datacontroltype = "Person";
        }
        if (datatarget != undefined) {
            callback = function (json, e) {
                var result = new Array();
                var users = json.Root.Users.Item;
                var depts = json.Root.Depts.Item;
                var positions = json.Root.Positions.Item;
                var systemRoles = json.Root.SystemRoles.Item;
                var customRoles = json.Root.CustomRoles.Item;
                var $target = $("#" + datatarget);
                if (users.length > 0) {
                    //$target.data("data-users", users);
                    $.each(users, function (i, item) {
                        result.push({ Value: item.Value, Name: item.Name, UserName: item.UserName });
                    });
                }
                if (depts.length > 0) {
                    //$target.data("data-depts", depts);
                    $.each(depts, function (i, item) {
                        result.push({ Value: item.Value, Name: item.Name, UserName: "" });
                    });
                }
                if (positions.length > 0) {
                    //$target.data("data-positions", positions);
                    $.each(positions, function (i, item) {
                        result.push({ Value: item.Value, Name: item.Name, UserName: "" });
                    });
                }
                if (systemRoles.length > 0) {
                    //$target.data("data-systemRoles", systemRoles);
                    $.each(systemRoles, function (i, item) {
                        result.push({ Value: item.Value, Name: item.Name, UserName: "" });
                    });
                }
                if (customRoles.length > 0) {
                    //$target.data("data-customRoles", customRoles);
                    $.each(customRoles, function (i, item) {
                        result.push({ Value: item.Value, Name: item.Name, UserName: "" });
                    });
                }
                var values = new Array();
                var names = new Array();
                var usernames = new Array();
                $.each(result, function (i, item) {
                    values.push(item.Value);
                    names.push(item.Name);
                    if (item.UserName.length > 0) {
                        usernames.push(item.UserName);
                    }
                });
                $target.attr("data-values", values.join(','));
                $target.attr("data-names", names.join(','));
                $target.attr("data-usernames", usernames.join(','));
                switch ($target[0].tagName) {
                    case "INPUT":
                    case "TEXTAREA":
                        $("#" + datatarget).val(names.join(','));
                        break;
                    case "DIV":
                    case "LABEL":
                    case "DT":
                    case "DL":
                    case "DD":
                    case "UL":
                    case "LI":
                    case "A":
                    case "P":
                    case "TH":
                    case "TT":
                    case "TR":
                    case "TD":
                    case "SPAN":
                    case "OL":
                    case "H":
                    case "IMG":
                        $("#" + datatarget).html(names.join(','));
                        break;
                }
                if (datacallback != undefined) {
                    try {
                        var obj = eval(datacallback);
                        if (typeof obj == "function") {
                            obj(json, e);
                        }
                    }
                    catch (ex) {
                        console.log("未定义回调函数");
                    }
                }
            }
        }
        if (options != undefined && typeof options == "object") {
            defaultoptions = options;
        }
        InitSelectPersonWindow(this, datacontroltype, callback, null, defaultoptions);
    });
}
InitSelectPersonControl();

var lang = $.cookies.get("LANG");
switch (lang) {
    case "en-US":
        bootbox.setDefaults({ locale: "en" });
        break;
    case "zh-CN":
        bootbox.setDefaults({ locale: "zh_CN" });
        break;
    case "zh-TW":
        bootbox.setDefaults({ locale: "zh_TW" });
        break;
}

//首页收起按钮
$("#headersilde").unbind("click").bind("click", function (event) {
    // debugger;
    event.stopPropagation();
    var headercontainer = $("header");
    var left = $("#_Left");
    var right = $("#right");
    var lmagintop = left.css("margin-top");
    var rmagintop = right.css("margin-top");
    if (lmagintop == "auto") {
        lmagintop = 0;
    }

    if (rmagintop == "auto") {
        rmagintop = 0;
    }
    lmagintop = lmagintop.toString().replace("px", "");
    rmagintop = rmagintop.toString().replace("px", "");
    if (rmagintop == "0") {
        rmagintop = 9;
    }
    var curheight = headercontainer.height();
    if (headercontainer.css("display") == "block") {
        headercontainer.fadeOut();
        left.css({ "margin-top": (lmagintop - curheight) + "px" });
        right.css({ "margin-top": (rmagintop - curheight) + "px" });

        left.data("oldmagintop", lmagintop);
        left.data("newmagintop", (lmagintop - curheight));
        right.data("oldmagintop", rmagintop);
        right.data("newmagintop", (rmagintop - curheight));

        $(this).css("background-position", " 0 0px").attr("title", "点击展开").html("展开");
        $("#right").find(".k-grid-content").each(function () {
            var height = $(this).height();
            $(this).css({ "height": height + curheight + "px", "max-height": height + curheight + "px" });
            $(this).attr("isaddheight", "1");
        })
    }
    else {
        left.css({ "margin-top": left.data("oldmagintop") + "px" });
        right.css({ "margin-top": right.data("oldmagintop") + "px" });
        headercontainer.fadeIn();
        $(this).css("background-position", " 0 -20px").attr("title", "点击缩起").html("缩起");
        $("#right").find(".k-grid-content").each(function () {
            var height = $(this).height();
            var isaddheight = $(this).attr("isaddheight");
            if (isaddheight != undefined && isaddheight == "1") {
                $(this).css({ "height": height - curheight + "px", "max-height": height - curheight + "px" });
                $(this).removeAttr("isaddheight");
            }
        })


    }
});



////通用数据库连接字符串引导填写窗口
//var InitConnectionWindow = function (target, options) {
//     
//    var that = $(target);
//    var cwid = that.attr("cwid");    
//    var templates = {
//        connectionstring: "Data Source=#SQLServer#;Initial Catalog=#SQLDataBase#;Persist Security Info=True;User ID=#SQLUserName#;Password=#SQLPassWord#;",
//        iconinput: "<span id=\"OpenConnStrWin#cwid#\" class=\"glyphicon glyphicon-plus btn\" style=\"cursor: pointer;position: absolute;right: 20px;top: 7px;padding: 0px;\"></span>",
//        window: "<div id=\"ConnStrWindow#cwid#\" style=\"display: none;\">"
//                    +"<div>"
//                    +"    <div class=\"form-group\">"
//                    + "        <label for=\"SQLServer#cwid#\" class=\"col-sm-3 control-label\">服务器</label>"
//                    +"        <div class=\"col-sm-9\">"
//                    + "            <input type=\"text\" class=\"form-control SQLServer\" id=\"SQLServer#cwid#\" placeholder=\"服务器\">"
//                    +"        </div>"
//                    +"        <div class=\"clearfix\"></div>"
//                    +"    </div>"
//                    +"    <div class=\"form-group\">"
//                    + "        <label for=\"SQLDataBase#cwid#\" class=\"col-sm-3 control-label\">数据库</label>"
//                    +"        <div class=\"col-sm-9\">"
//                    + "            <input type=\"text\" class=\"form-control SQLDataBase\" id=\"SQLDataBase#cwid#\" placeholder=\"数据库\">"
//                    +"        </div>"
//                    +"        <div class=\"clearfix\"></div>"
//                    +"    </div>"
//                    +"    <div class=\"form-group\">"
//                    + "        <label for=\"SQLUserName#cwid#\" class=\"col-sm-3 control-label\">用户名</label>"
//                    +"        <div class=\"col-sm-9\">"
//                    + "            <input type=\"text\" class=\"form-control SQLUserName\" id=\"SQLUserName#cwid#\" placeholder=\"用户名\">"
//                    +"        </div>"
//                    +"        <div class=\"clearfix\"></div>"
//                    + "    </div>"
//                    + "    <div class=\"form-group\">"
//                    + "        <label for=\"SQLPassWord#cwid#\" class=\"col-sm-3 control-label\">密码</label>"
//                    + "        <div class=\"col-sm-9\">"
//                    + "            <input type=\"text\" class=\"form-control SQLPassWord\" id=\"SQLPassWord#cwid#\" placeholder=\"密码\">"
//                    + "        </div>"
//                    + "        <div class=\"clearfix\"></div>"
//                    + "    </div>"
//                    +"</div>"
//                    +"<div class=\"operabar\">"
//                    +"    <div class=\"operamask\"></div>"
//                    +"    <div class=\"operacontent\">"
//                    + "        <button class=\"k-button windowConfirm\" style=\"float: left;\">" + jsResxbaseInitView.Confirm + "</button>"
//                    + "        <button class=\"k-button windowCancel\" style=\"float: right\">" + jsResxbaseInitView.Cancel + "</button>"
//                    +"    </div>"
//                    +"</div>"
//                +"</div>"
//    }

//    var defaults = {
//        actions: [
//            "Pin",
//            "Minimize",
//            "Maximize",
//            "Close"
//        ],
//        resizable: false,
//        modal: true,
//        draggable: true,
//        callback:undefined
//    };
//    $.extend(defaults, options);
//    if (!cwid) {
//        var id = "cw_" + Math.random().toString().substring(2);
//        that.attr("cwid", id);
//        var buttonhtml = templates.iconinput.replace(/#cwid#/g, id);
//        $(target).parent().append(buttonhtml);
//        var windowhtml = templates.window.replace(/#cwid#/g, id);
//        $(target).parent().append(windowhtml);
//        cwid = that.attr("cwid");
//    }

//    var cwindow = $(document).find("#ConnStrWindow" + cwid);
//    if (!cwindow.data("kendoWindow")) {
//        cwindow.kendoWindow({
//            width: "600px",
//            title: jsResxbaseInitView.SelectConnectionWindowTitle,
//            actions: defaults.actions,
//            resizable: defaults.resizable,
//            modal: defaults.modal,
//            draggable: defaults.draggable            
//        });

//        $("#OpenConnStrWin" + cwid).click(function () {
//            if (that.attr("disabled") == "disabled" || that.attr("disabled") == "true") {
//                return;
//            }
//            var curconnectionstring = $(target).val();
//            if (curconnectionstring.length > 0) {
//                var arr = curconnectionstring.split(';');
//                $("#ConnStrWindow" + cwid + " input.SQLServer").val(arr[0].replace(/Data Source=/g, ""));
//                $("#ConnStrWindow" + cwid + " input.SQLDataBase").val(arr[1].replace(/Initial Catalog=/g, ""));
//                $("#ConnStrWindow" + cwid + " input.SQLUserName").val(arr[3].replace(/User ID=/g, ""));
//                $("#ConnStrWindow" + cwid + " input.SQLPassWord").val(arr[4].replace(/Password=/g, ""));
//            }
//            cwindow.data("kendoWindow").center().open();
//        });

//        //确定
//        $("#ConnStrWindow" + cwid + " .windowConfirm").bind("click", function () {
//            var server = $("#ConnStrWindow" + cwid + " input.SQLServer").val();
//            var database = $("#ConnStrWindow" + cwid + " input.SQLDataBase").val();
//            var username = $("#ConnStrWindow" + cwid + " input.SQLUserName").val();
//            var password = $("#ConnStrWindow" + cwid + " input.SQLPassWord").val();
//            $(target).val(templates.connectionstring.replace(/#SQLServer#/g, server).replace(/#SQLDataBase#/g, database).replace(/#SQLUserName#/g, username).replace(/#SQLPassWord#/g, password));
//            cwindow.data("kendoWindow").close();
//        });

//        //取消
//        $("#ConnStrWindow" + cwid + " .windowCancel").bind("click", function () {
//            if (defaults.onClose != undefined && defaults.onClose != null) {
//                defaults.onClose();
//            }
//            else {
//                cwindow.data("kendoWindow").close();
//            }
//        });
//    }

//};


////通用数据库字段，函数，存储过程映射引导填写窗口
//var InitMappingWindow = function (target, options) {
//     
//    var that = $(target);
//    var cwid = that.attr("cwid");
//    var templates = {        
//        iconinput: "<span id=\"OpenMappingWin#cwid#\" class=\"glyphicon glyphicon-plus btn\" style=\"cursor: pointer;position: absolute;right: 20px;top: 7px;padding: 0px;\"></span>",
//        window: "<div id=\"MappingWindow#cwid#\" style=\"display: none;\">"
//                    + "<div>"
//                    + "    <div class=\"form-group\">"
//                    + "        <label for=\"MappingField#cwid#\" class=\"col-sm-3 control-label\">" + jsResxbaseInitView.MappingField + "</label>"
//                    + "        <div class=\"col-sm-9\">"
//                    + "            <div class=\"MappingField\" id=\"MappingFieldTree#cwid#\" >"
//                    + "        </div>"
//                    + "        <div class=\"clearfix\"></div>"
//                    + "    </div>"                   
//                    + "</div>"
//                    + "<div class=\"operabar\">"
//                    + "    <div class=\"operamask\"></div>"
//                    + "    <div class=\"operacontent\">"
//                    + "        <button class=\"k-button windowConfirm\" style=\"float: left;\">" + jsResxbaseInitView.Confirm + "</button>"
//                    + "        <button class=\"k-button windowCancel\" style=\"float: right\">" + jsResxbaseInitView.Cancel + "</button>"
//                    + "    </div>"
//                    + "</div>"
//                + "</div>"
//    }

//    var defaults = {
//        actions: [
//            "Pin",
//            "Minimize",
//            "Maximize",
//            "Close"
//        ],
//        type:"Column",
//        resizable: false,
//        modal: true,
//        draggable: true,
//        callback: undefined
//    };
//    $.extend(defaults, options);
//    if (!cwid) {
//        var id = "cw_" + Math.random().toString().substring(2);
//        that.attr("cwid", id);
//        var buttonhtml = templates.iconinput.replace(/#cwid#/g, id);
//        $(target).parent().append(buttonhtml);
//        var windowhtml = templates.window.replace(/#cwid#/g, id);
//        $(target).parent().append(windowhtml);
//        cwid = that.attr("cwid");
//    }

//    var cwindow = $(document).find("#MappingWindow" + cwid);
//    if (!cwindow.data("kendoWindow")) {
//        cwindow.kendoWindow({
//            width: "600px",
//            title: jsResxbaseInitView.SelectMappingWindowTitle,
//            actions: defaults.actions,
//            resizable: defaults.resizable,
//            modal: defaults.modal,
//            draggable: defaults.draggable
//        });      

//        //确定
//        $("#MappingWindow" + cwid + " .windowConfirm").bind("click", function () {

//            cwindow.data("kendoWindow").close();
//        });

//        //取消
//        $("#MappingWindow" + cwid + " .windowCancel").bind("click", function () {
//            if (defaults.onClose != undefined && defaults.onClose != null) {
//                defaults.onClose();
//            }
//            else {
//                cwindow.data("kendoWindow").close();
//            }
//        });

//        //tree datasource
//        var treedatasource = function (id) {
//            return new kendo.data.HierarchicalDataSource({
//                transport: {
//                    read: {
//                        url: function (options) {
//                            return kendo.format("/KstarMobile/MobileConfig/GetDataBaseTree?Type={0}&id={1}", "Root", id);
//                        },
//                        dataType: "json"
//                    }
//                },
//                schema: {
//                    model: {
//                        id: "ID",               //绑定ID
//                        hasChildren: "HasChildNode"  //绑定是否包含子节点                 
//                    }
//                }
//            });
//        }


//        var initTreeView = function (id) {            
//            var MappingFieldTree = $("#MappingFieldTree" + cwid).data("kendoTreeView");
//            var treetemplate = "# if(item.Type != \"Root\"){ #"
//                         + "            <span class=\"k-sprite property\"></span>"
//                         + "            # }else{ #"
//                         + "            <span class=\"k-sprite folder\"></span>"
//                         + "            # } #  "
//                         + "            <input type=\"checkbox\" value=\"#= item.ID #\" data-type=\"#= item.Type #\" />"
//                         + "            #: item.NodeName #";
//            if (!MappingFieldTree) {
//                $("#MappingFieldTree" + cwid).kendoTreeView({
//                    template: kendo.template(treetemplate),
//                    dataSource: treedatasource(id),
//                    select: function (e) {
//                        $("#MappingFieldTree" + cwid).find("input").prop("checked", false);
//                        var select = $("#MappingFieldTree" + cwid + "_tv_active").find("input").first().prop("checked", true);  //利用 #treeview_tv_active 获取当前选中对象 查找 隐藏的 input 对象 提取隐藏信息
//                        var item = $("#MappingFieldTree" + cwid).data("kendoTreeView").dataSource.get(select.val());
//                        var itemtype = select.attr("data-type");

//                    },
//                    collapse: function (e) {                        
//                        $("#MappingFieldTree" + cwid + "_tv_active").find(".k-sprite").first().removeClass("on");
//                    },
//                    expand: function (e) {                        
//                        $("#MappingFieldTree" + cwid + "_tv_active").find(".k-sprite").first().addClass("on");
//                    },
//                    dataBound: function (e) {                        
//                        var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
//                        var mousedown = function (e) { if (e.which == 3) $(this).click(); }
//                        $("#MappingFieldTree" + cwid).find(":checkbox").unbind(clickevent).bind(clickevent);
//                        $("#MappingFieldTree" + cwid).off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
//                        $("#MappingFieldTree" + cwid).data("kendoTreeView").expand(".k-first");
//                    }
//                });
//                MappingFieldTree = $("#MappingFieldTree" + cwid).data("kendoTreeView");
//                TreeViewNodeToggle("#MappingFieldTree" + cwid);
//            }
//            else {
//                MappingFieldTree.setDataSource(treedatasource);
//            }
//        }

//        $("#OpenMappingWin" + cwid).click(function () {
//            var id = that.attr("ProcessId");
//            if (that.attr("disabled") == "disabled" || that.attr("disabled") == "true" ||id==null||id.length==0) {
//                return;
//            }            
//            initTreeView(id);
//            cwindow.data("kendoWindow").center().open();
//        });


//    }

//};