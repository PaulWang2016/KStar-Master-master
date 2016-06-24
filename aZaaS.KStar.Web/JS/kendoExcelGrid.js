(function ($) {
    var kendo = window.kendo;

    var ExcelGrid = kendo.ui.Grid.extend({
        init: function (element, options) {
            var that = this;
            var exportOption = options["export"];
           // debugger;
            if (exportOption) {
                // If the exportCssClass is not defined, then set a default image.
                exportOption.cssClass = exportOption.cssClass || "k-i-expand";

                // Add the export toolbar button.
                if (exportOption.allowExport) {
                    options.toolbar = $.merge([
                        {
                            name: "export",
                            template: kendo.format("<div class='k-grid-title'>" + (exportOption.createWidthOutHiddenUrl ? "" : "{0}") + "</div><a class='k-button k-grid-clear' style='display:none'><span class='k-icon k-i-funnel-clear'></span></a>" +
                                "<a class='k-button k-grid-export' title='" + jsResxkendoExcelGrid.ExporttoExcel + "'><div class='{1}'></div></a>" +
                                "<a class='k-button k-grid-exportcsv' title='" + jsResxkendoExcelGrid.ExporttoCSV + "'><div class='csv-ico'></div></a>",
                                //暂时屏蔽部分到出+(exportOption.createWidthOutHiddenUrl  ? "<a class='k-button k-grid-exportWithOutHidden' title='Export to Excel without Hidden'><div class='{1}'></div></a>" : "")
                                exportOption.title, exportOption.cssClass)
                        }
                    ], options.toolbar || []);
                }
                else {
                    options.toolbar = $.merge([
                       {
                           name: "export",
                           template: kendo.format("<div class='k-grid-title'>" + (exportOption.createWidthOutHiddenUrl ? "" : "{0}") + "</div>",
                               //暂时屏蔽部分到出+(exportOption.createWidthOutHiddenUrl  ? "<a class='k-button k-grid-exportWithOutHidden' title='Export to Excel without Hidden'><div class='{1}'></div></a>" : "")
                               exportOption.title)
                       }
                    ], options.toolbar || []);
                }
            }

            // Initialize the grid.
            kendo.ui.Grid.fn.init.call(that, element, options);

            // Add an event handler for the Export button.
            $(element).on("click", ".k-grid-export", { sender: that }, function (e) {
                //如果数据源为服务端分页，导出excel则不用客户端数据集
                if (exportOption.isDownloadFromServer != undefined && exportOption.isDownloadFromServer && exportOption.downloadFromServerUrl != undefined) {
                    e.data.sender.exportFromServer("excel");
                }
                else {
                    e.data.sender.exportToExcel();
                }
            });
            //$(element).on("click", ".k-grid-exportWithOutHidden", { sender: that }, function (e) {
            //    e.data.sender.exportToExcelWithOutHidden();
            //});
            $(element).on("click", ".k-grid-exportcsv", { sender: that }, function (e) {
                //如果数据源为服务端分页，导出csv则不用客户端数据集
                if (exportOption.isDownloadFromServer != undefined && exportOption.isDownloadFromServer && exportOption.downloadFromServerUrl != undefined) {
                    e.data.sender.exportFromServer("csv");
                }
                else {
                    e.data.sender.exportToCSV();
                }
            });
            $(element).on("click", ".k-grid-clear", { sender: that }, function (e) {
                e.data.sender.ClearCookies();
                $("#" + that.element.context.id).find(".k-grid-clear").hide();
            });
        },

        options: {
            name: "ExcelGrid"
        },
        exportFromServer: function (type) {            
            var that = this;
            var columns = [];
            $.each(that.columns, function (i) {
                if (that.columns[i].title!=undefined && that.columns[i].title.length > 0) {
                    columns.push(that.columns[i]);
                }
            });

            var exportOption = that.options["export"];
            $("#homeBody").addClass("onloading");            
            
            if (exportOption.downloadFromServerUrl == undefined || exportOption.downloadFromServerUrl == null)
            {
                $("#homeBody").removeClass("onloading");
                return;
            }
            var filterdata = kendo.stringify(that.options["filter"]);
            var posturl = exportOption.downloadFromServerUrl;
            if(typeof exportOption.downloadFromServerUrl=="function")
            {
                posturl=exportOption.downloadFromServerUrl();
            }            
            $.ajax({
                url: posturl,
                type: 'POST',
                dataType: 'json',
                data: { filter: filterdata },
                global: false,
                beforeSend: function (XMLHttpRequest) {
                    showOperaMask();
                },
                success: function (items) {                    
                var filter = that.dataSource.filter();
                var pageSize = that.dataSource.pageSize();              
                var data = {
                    column: JSON.stringify(columns),
                    data: JSON.stringify(items),
                    filter: filterdata,
                    title: exportOption.title
                };                
                // Create the spreadsheet.
                var createurl = (type == "excel" ? exportOption.createUrl : exportOption.createCSVUrl);
                var downloadUrl = (type == "excel" ? exportOption.downloadUrl : exportOption.downloadSCVUrl);
                $.post(createurl, data, function () {
                    // Download the spreadsheet.
                    window.location.replace(kendo.format("{0}?title={1}",
                        downloadUrl,
                        exportOption.title));
                    $("#homeBody").removeClass("onloading");
                });
                    hideOperaMask();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    hideOperaMask();
                }
            });
        }
        ,
        exportToExcel: function () {
            var that = this;
            var columns = [];
            $.each(that.columns, function (i) {
                if (that.columns[i].title != undefined && that.columns[i].title.length > 0) {
                    columns.push(that.columns[i]);
                }
            });
            $("#homeBody").addClass("onloading");
            var exportOption = that.options["export"];
            var filterdata = kendo.stringify(that.options["filter"]);
            var filter = that.dataSource.filter();
            var pageSize = that.dataSource.pageSize();
            that.dataSource.pageSize(that.dataSource.total());
            // Define the data to be sent to the server to create the spreadsheet.
            data = {
                column: JSON.stringify(columns),
                data: JSON.stringify(that.dataSource.view()),
                filter: filterdata,
                title: exportOption.title
            };
            that.dataSource.pageSize(pageSize);
            $.ajax({
                url: exportOption.createUrl,
                type: 'POST',
                dataType: 'json',
                data: data,
                global: false,
                beforeSend: function (XMLHttpRequest) {                    
                    showOperaMask();
                },
                success: function (items) {
                    window.location.replace(kendo.format("{0}?title={1}",
                   exportOption.downloadUrl,
                   exportOption.title));
                    $("#homeBody").removeClass("onloading");
                    hideOperaMask();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    hideOperaMask();
                }
            });
        },
        //exportToExcelWithOutHidden: function () {
        //    var that = this;

        //    var exportOption = that.options["export"];
        //    var filterdata = kendo.stringify(that.options["filter"]);
        //    var filter = that.dataSource.filter();
        //    var pageSize = that.dataSource.pageSize();
        //    that.dataSource.pageSize(that.dataSource.total());
        //    // Define the data to be sent to the server to create the spreadsheet.
        //    data = {
        //        column: JSON.stringify(that.columns),
        //        data: JSON.stringify(that.dataSource.view()),
        //        filter: filterdata,
        //        title: exportOption.title
        //    };
        //    that.dataSource.pageSize(pageSize)

        //    // Create the spreadsheet.
        //    $.post(exportOption.createWidthOutHiddenUrl, data, function () {
        //        // Download the spreadsheet.
        //        window.location.replace(kendo.format("{0}?title={1}",
        //            exportOption.downloadUrl,
        //            exportOption.title));
        //    });
        //},
        exportToCSV: function () {
            var that = this;
            $("#homeBody").addClass("onloading");
            var exportOption = that.options["export"];
            var filterdata = kendo.stringify(that.options["filter"]);
            var filter = that.dataSource.filter();
            var pageSize = that.dataSource.pageSize();
            that.dataSource.pageSize(that.dataSource.total());
            // Define the data to be sent to the server to create the spreadsheet.
            data = {
                column: JSON.stringify(that.columns),
                data: JSON.stringify(that.dataSource.view()),
                filter: filterdata,
                title: exportOption.title
            };
            that.dataSource.pageSize(pageSize)           
            $.ajax({
                url: exportOption.createCSVUrl,
                type: 'POST',
                dataType: 'json',
                data: data,
                global: false,
                beforeSend: function (XMLHttpRequest) {
                    showOperaMask();
                },
                success: function (items) {
                    window.location.replace(kendo.format("{0}?title={1}",
                   exportOption.downloadSCVUrl,
                   exportOption.title));
                    $("#homeBody").removeClass("onloading");
                    hideOperaMask();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    hideOperaMask();
                }
            });
        },
        ClearCookies: function () {
            ShowHiddenColumn(this.element.context.id);
            ClearFilters(this.element.context.id);
        }
    });

    kendo.ui.plugin(ExcelGrid);
})(jQuery);