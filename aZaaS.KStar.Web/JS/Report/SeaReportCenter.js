

define(function (require, exports, module) {

    var categoryTreeView;
    var reportRoleGrid;
    var categoryRenderItems = 0;

    /* Report Grid */
    var ReportModel = kendo.data.Model.define({
        id: "ID",
        fields: {
            ID: { type: "string" },
            Name: { type: "string" },
            PublishedDate: { type: "date" },
            Department: { type: "string" },
            ImageThumbPath: { type: "string" },
            ReportUrl: { type: "string" },
            Category: { type: "string" },
            ReportCode: { type: "string" },
            Level: { type: "string" },
            Status: { type: "string" },
            Rate: { type: "string" },
            Comment: { type: "string" },
            ParnentID: { type: "string" },
            PermissionRoleNames: { type: "string" }
        }
    });

    var ReportColumns = [
        {
            title: jsResxReport_SeaReportCenter.ReportThumbnails, width: 150, template: function (item) {
                return '<a title="' + item.Comment + '" href="Report/ReportCenter/ViewReport?reportID=' + item.ID + '" style="display:block;text-align:center;" target="_blank" ><img src="' + item.ImageThumbPath + '" style="width:110px;height:110px;"  class="img-rounded"   /> </a>';
            }, filterable: false
        },
        {
            field: "Name", title: jsResxReport_SeaReportCenter.ReportName, template: function (item) {
                return '<a href="Report/ReportCenter/ViewReport?reportID=' + item.ID + '" target="_blank">' + item.Name + ' </a>';
            }, filterable: true, sortable: true
        },
        { field: "Department", title: jsResxReport_SeaReportCenter.Responsibledepartments, filterable: false, sortable: true },
        { field: "PublishedDate", title: jsResxReport_SeaReportCenter.DateAdded, format: getDateTimeFormat(), filterable: false, sortable: true },
        { field: "Level", title: jsResxReport_SeaReportCenter.Level, filterable: false, sortable: true },
        { field: "Category", title: jsResxReport_SeaReportCenter.Type, filterable: false, sortable: true },
        { field: "ReportCode", title: jsResxReport_SeaReportCenter.Internalnumber, filterable: false, sortable: true },
        { field: "Status", title: jsResxReport_SeaReportCenter.ReportStatus, filterable: false, sortable: true },
         {
             field: "FormattedRoles", title: jsResxReport_SeaReportCenter.ReportPermission, template: function (item) {
                 return '<div title="' + (item.FormattedRoles || '') + '">' + (item.FormattedRoles || '') + ' </div>';
             }, filterable: false, sortable: false
         },
        {
            command: [
                {
                    name: "edit", template: "<a  href='javascript:void(0)' data-mode='edit' class='k-button k-grid-edit '><span class='glyphicon glyphicon-pencil'></span></a>&nbsp;<a  href='javascript:void(0)' data-mode='remove' class='k-button k-grid-edit'><span class='glyphicon glyphicon-remove'></span></a>", click: function (e) {

                        e.preventDefault();
                        var mode = $(e.currentTarget).attr('data-mode');
                        var item = this.dataItem($(e.currentTarget).closest("tr"));
                        if (mode == 'edit') {
                            EditReportEvent('#AddReportWindow', item.ID);
                        } else if (mode == 'remove') {
                            RemoveReportEvent(item.ID);
                        }
                    }
                }],
            title: " ", width: 50, filterable: false, sortable: false
        }
    ]

    var categegoryDataSource = function () {

        return new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: function (options) {
                        return kendo.format("/Report/ReportCenter/GetCategories");
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


    var initFilterPanel = function () {

        $("#keySearch").kendoAutoComplete({
            placeholder: jsResxMaintenance_SeaPosition.NDIIK
        });

        var start = $("#start").kendoDatePicker({
            format: "yyyy-MM-dd"
        }).data("kendoDatePicker");
        var end = $("#end").kendoDatePicker({
            format: "yyyy-MM-dd"
        }).data("kendoDatePicker");

        start.max(end.value());
        end.min(start.value());

        var data = [
            { text: jsResxMaintenance_SeaPosition.AllLevels, value: "1" },
            { text: jsResxMaintenance_SeaPosition.Stafflevel, value: "2" },
            { text: jsResxMaintenance_SeaPosition.Departmental, value: "3" },
            { text: jsResxMaintenance_SeaPosition.Companylevel, value: "4" }
        ];

        $("#report_level").kendoDropDownList({
            index: 0,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: data
        });

        var data = [
            { text: jsResxMaintenance_SeaPosition.Added, value: "1" },
            { text: jsResxMaintenance_SeaPosition.shelves, value: "2" },
        ];

        $("#report_status").kendoDropDownList({
            index: 0,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: data

        });

        $('#ChooseFilter').click(function () {

            searchCategoryReports();
        });
    }

    var searchCategoryReports = function filterReports() {

        var categoryID = $('#' + categoryTreeView._ariaId).find("input").first().val();
        if (!categoryID) {
            categoryID = '00000000-0000-0000-0000-000000000000';
            categoryTreeView.select('.k-first');
            categoryTreeView.select().find('input[type="checkbox"]').first().prop('checked', true);
        }


        var search = $("#keySearch").val();
        var start = $("#start").val().toString();
        var end = $("#end").val().toString();
        var status = $("#report_status").data("kendoDropDownList").select();
        var level = $("#report_level").data("kendoDropDownList").select();

        var params = {
            keySearch: search,
            startDate: start,
            endDate: end,
            status: status,
            level: level,
            categoryID: categoryID
        };

        InitServerQueryReportKendoExcelGrid("reportList", ReportModel, ReportColumns, "/Report/ReportCenter/GetReports", params, $(window).height() - fullwidgetH, 'Title', '', function () {
            bindAndLoad("reportList");
            //bindGridCheckbox("reportList");
        });

    }

    //TODO:
    var initReportListView = function (reportListViewSelector) {

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "POST",
                    url: "/Report/ReportCenter/GetReports",
                    dataType: "json"
                }
            },
            pageSize: 20
        });

        $(reportListViewSelector).kendoListView({
            dataSource: dataSource,
            template: kendo.template($(reportListViewSelector + 'Template').html())
        });

        $(reportListViewSelector + 'Pager').kendoPager({
            dataSource: dataSource
        });
    }

    var initLayoutSplitter = function (splitterSelector) {
        $(splitterSelector).kendoSplitter({
            panes: [
                { collapsible: false, size: "200px", min: "200px", max: "450px", resizable: true },
                { collapsible: false, resizable: true }
            ]
        });
    }

    var initGeneralValidators = function (categoryWinSelector, reportWinSelector) {

        var $reportWindow = $(reportWinSelector);
        var $categoryWindow = $(categoryWinSelector);

        /* Input Validation */
        $categoryWindow.kendoValidator({
            validateOnBlur: true,
            messages: {
                required: '报表名称为必填项！',
            }
        });

        $reportWindow.kendoValidator({
            validateOnBlur: true
        });
    }

    var initGeneralControls = function (reportWinSelector) {

        var $reportWindow = $(reportWinSelector);

        /* Report window controls */

        $reportWindow.find("#reportLevel").kendoComboBox();

        $reportWindow.find("#reportDatepicker").kendoDatePicker({
            value: new Date(),
            format: "yyyy-MM-dd "
        });

    }

    var initWindow = function (winSelector, winTitle, openHandler, closeHandler) {

        var $window = $(winSelector);

        var win = $window.data("kendoWindow");
        if (!win) {

            var win = $window.kendoWindow({
                //width: "500px",
                title: winTitle,
                actions: [
                    "Close"
                ],
                resizable: false,
                modal: true
            });

            win = $window.data("kendoWindow").center();
            $(window.splitters).push(win);
        }

        win.unbind('open');
        win.unbind('close');

        win.bind('open', function (e) {
            openHandler(e);
        });
        win.bind('close', function (e) {
            closeHandler(e);
            hideOperaMask($window.attr('id'));
        });

        return win;
    }

    var initReportRoleGrid = function (reportRoleGridSelector) {
        $(reportRoleGridSelector).kendoGrid({
            dataSource: {
                schema: {
                    model: {
                        fields: {
                            RoleID: { type: "string" },
                            RoleType: { type: "string" },
                            RoleName: { type: "string" }
                        }
                    }
                },
                pageSize: 3
            },
            //height: 100,
            scrollable: true,
            groupable: false,
            sortable: false,
            pageable: true,
            columns: [{
                field: "RoleName",
                title: "显示名称"
            }, {
                field: "RoleType",
                title: "角色类型"
            }, {
                command: {
                    name: "edit", template: "<a  href='javascript:void(0)' class='k-button k-grid-edit'><span class='glyphicon glyphicon-remove'></span></a>",
                    click: function (e) {

                        e.preventDefault();
                        var item = this.dataItem($(e.currentTarget).closest("tr"));
                        $("#reportRoles").data("kendoGrid").dataSource.remove(item);
                    }
                }, title: " ", width: 50, filterable: false, sortable: false
            }]
        });

        reportRoleGrid = $(reportRoleGridSelector).data("kendoGrid");
        $("" + reportRoleGridSelector + " .k-grid-content").css("overflow-y", "scroll");

        $('#addRole').click(function (e) {

            InitSelectPersonWindow(this, "All", function (json) {
                var users = json.Root.Users.Item;
                $.each(users, function (i, item) {
                    if (!existsRoleInGrid(item.Value))
                        reportRoleGrid.dataSource.add({ RoleID: item.Value, RoleType: 'User', RoleName: item.Name });
                });

                var depts = json.Root.Depts.Item;
                $.each(depts, function (i, item) {
                    if (!existsRoleInGrid(item.Value))
                        reportRoleGrid.dataSource.add({ RoleID: item.Value, RoleType: 'Department', RoleName: item.Name });
                });

                var positions = json.Root.Positions.Item;
                $.each(positions, function (i, item) {
                    if (!existsRoleInGrid(item.Value))
                        reportRoleGrid.dataSource.add({ RoleID: item.Value, RoleType: 'Position', RoleName: item.Name });
                });

                var roles = json.Root.SystemRoles.Item;
                $.each(roles, function (i, item) {
                    if (!existsRoleInGrid(item.Value))
                        reportRoleGrid.dataSource.add({ RoleID: item.Value, RoleType: 'Role', RoleName: item.Name });
                });
            });

        });

        function existsRoleInGrid(roleId) {

            var items = reportRoleGrid.dataSource.data();
            var result = $.grep(items, function (item) {
                return item.RoleID == roleId;
            });

            return result.length > 0;
        }
    }

    /* Report Category */

    var AddCategoryEvent = function (categoryWinSelector, parentId) {

        var $window = $(categoryWinSelector);

        function closeCategoryWindow() {

            $window.data("kendoWindow").close()
        }

        function categoryAddConfirm() {

            var validator = $window.data("kendoValidator");
            if (!validator.validate()) {
                return false;
            }

            var parentID = $(this).attr("data-parentId");
            var categoryName = $window.find('#CategoryName').first().val();
            var categoryDesc = $window.find('#CategoryDesc').first().val();

            showOperaMask($window.attr('id'));

            $.post("/Report/ReportCenter/AddCategory",
                { DisplayName: categoryName, ParentID: parentID, Comment: categoryDesc },
                function (item) {

                    var select = categoryTreeView.select();
                    if (item.ParentID == null
                        || item.ParentID == "00000000-0000-0000-0000-000000000000") {
                        treeview.append(item);
                    }
                    else {
                        if (select.attr("aria-expanded") || select.find(".k-plus").length == 0) {
                            categoryTreeView.append(item, select);
                        }
                        else {
                            categoryTreeView.expand(select);
                        }
                    }

                    $window.data("kendoWindow").close();

                }).fail(function () {
                    showOperaMask($window.attr('id'));
                })
        }

        function bindButtonEvents(e) {

            $window.find('button.windowConfirm').bind("click", categoryAddConfirm);
            $window.find('button.windowCancel').bind("click", closeCategoryWindow);
        }

        function unbindButtonEvents(e) {

            $window.find('button.windowConfirm').unbind("click", categoryAddConfirm);
            $window.find('button.windowCancel').unbind("click", closeCategoryWindow);
        }

        $('#CategoryName').val('');
        $('#CategoryDesc').val('');
        $window.data("kendoValidator").hideMessages();
        $window.find('label').css('font-weight', 'normal');

        var btnConfirm = $window.find('button.windowConfirm');
        btnConfirm.attr("data-parentId", typeof (parentId) == "string" ? parentId : '');

        initWindow(categoryWinSelector, '添加分类', bindButtonEvents, unbindButtonEvents).center().open();
    }

    var EditCategoryEvent = function (categoryWinSelector, categoryID) {

        var $window = $(categoryWinSelector);

        function closeCategoryWindow() {

            $window.data("kendoWindow").close()
        }

        function categoryEditConfirm() {

            var validator = $window.data("kendoValidator");
            if (!validator.validate()) {
                return false;
            }

            var parentID = $(this).attr("data-parentId");
            var categoryName = $window.find('#CategoryName').first().val();
            var categoryDesc = $window.find('#CategoryDesc').first().val();

            showOperaMask($window.attr('id'));

            $.post("/Report/ReportCenter/EditCategory",
                { CategoryID: categoryID, DisplayName: categoryName, ParentID: parentID, Comment: categoryDesc },
                function (item) {

                    hideOperaMask($window.attr('id'));

                    categoryTreeView.setDataSource(categegoryDataSource());
                    //categoryTreeView.expand(".k-item");
                    //categoryTreeView.expand(categoryTreeView.select());
                    $window.data("kendoWindow").close();

                }).fail(function () {

                    hideOperaMask($window.attr('id'));
                    alert('更新失败！');
                })
        }

        function bindButtonEvents(e) {

            $window.find('button.windowConfirm').bind("click", categoryEditConfirm);
            $window.find('button.windowCancel').bind("click", closeCategoryWindow);
        }

        function unbindButtonEvents(e) {

            $window.find('button.windowConfirm').unbind("click", categoryEditConfirm);
            $window.find('button.windowCancel').unbind("click", closeCategoryWindow);
        }

        $.post("/Report/ReportCenter/GetCategory",
            { categoryID: categoryID },
            function (item) {

                $('#CategoryDesc').val(item.Comment);
                $('#CategoryName').val(item.Category);
                $window.find('button.windowConfirm').attr("data-parentId", item.ParnentID);
                $window.data("kendoValidator").hideMessages();
                $window.find('label').css('font-weight', 'normal');

                initWindow(categoryWinSelector, '编辑分类', bindButtonEvents, unbindButtonEvents).center().open();

            }).fail(function (err, status) {
                alert('数据加载失败!');
                return;
            });
    }

    var RemoveCategoryEvent = function (categoryID, currentActiveNode) {

        bootbox.confirm('您确定要删除该报表分类？', function (result) {
            if (result) {
                $.post("Report/ReportCenter/RemoveCategory", { categoryID: categoryID }, function (data) {

                    categoryTreeView.remove(currentActiveNode);
                });
            }
        });
    }

    /*   Report  */

    var AddReportEvent = function (reportWinSelector, parentId) {

        var $window = $(reportWinSelector);

        function closeReportWindow() {

            $window.data("kendoWindow").close()
        }

        function reportAddConfirm() {

            var self = $(this);
            var validator = $window.data("kendoValidator");
            if (!validator.validate()) {
                return false;
            }
            var roles = reportRoleGrid.dataSource.data();
            if (roles.length <= 0) {
                alert('请设置报表权限！');
                return false;
            }

            showOperaMask($window.attr('id'));

            var reportName = $("#reportName").val();
            var reportDepartment = $("#reportDepartment").first().val();
            var reportLevel = $("#reportLevel").data("kendoComboBox").value();
            var reportCode = $("#reportCode").val();
            var reportDate = $("#reportDatepicker").val();
            var reportCategory = $("#report_Category").val();
            var reportComment = $("#reportComment").val();
            var reportStatus = $("#reportStatus").find("input:checked").val()
            var reportUrl = $("#reportUrl").val();

            //var roleData = JSON.stringify(roles); !!! NOT SUPPORT !!!
            var roleData = '';
            for (var i = 0; i < roles.length; i++) {
                var role = roles[i];
                roleData += role.RoleID + '&' + role.RoleType + '&' + role.RoleName + ';';
            }

            //Fixed:
            jQuery.handleError = function (a, b, c, d) {


                searchCategoryReports();
                hideOperaMask($window.attr('id'));
                $window.data("kendoWindow").close();
            }

            $.ajaxFileUpload({
                url: '/Report/ReportCenter/AddReport',
                data: {
                    Name: reportName,
                    Department: reportDepartment,
                    PublishedDate: reportDate,
                    Level: reportLevel,
                    Category: reportCategory,
                    ReportCode: reportCode,
                    Comment: reportComment,
                    ParnentID: parentId,
                    Status: reportStatus,
                    ReportUrl: reportUrl,
                    Roles: roleData
                },
                secureuri: false,
                fileElementId: 'fileField',
                dataType: 'json',
                success: function (data, status) {

                    hideOperaMask($window.attr('id'));

                    searchCategoryReports();
                    $window.data("kendoWindow").close();
                },
                error: function (data, status, e) { }
            });
        }

        function bindButtonEvents(e) {

            $window.find('button.windowConfirm').bind("click", reportAddConfirm);
            $window.find('button.windowCancel').bind("click", closeReportWindow);
        }

        function unbindButtonEvents(e) {

            $window.find('button.windowConfirm').unbind("click");
            $window.find('button.windowCancel').unbind("click");
        }

        $window.find('form')[0].reset();
        reportRoleGrid.dataSource.data([]);
        $("#reportDatepicker").val(new Date().format('yyyy-MM-dd'));

        $('#report_Category').unbind('click');
        var select = categoryTreeView.select();
        $window.find("#report_Category").val(select.find('input[type=checkbox]').data('displayname'));

        $window.find('label').css('font-weight', 'normal');

        var btnConfirm = $window.find('button.windowConfirm');
        btnConfirm.attr("data-parentId", typeof (parentId) == "string" ? parentId : '');

        initWindow(reportWinSelector, '添加报表', bindButtonEvents, unbindButtonEvents).center().open();

    }

    var EditReportEvent = function (reportWinSelector, reportID) {

        var $window = $(reportWinSelector);

        function closeReportWindow() {

            $window.data("kendoWindow").close()
        }

        function reportEditConfirm() {

            var self = $(this);
            var validator = $window.data("kendoValidator");
            if (!validator.validate()) {
                return false;
            }
            var roles = reportRoleGrid.dataSource.data();
            if (roles.length <= 0) {
                alert('请设置报表权限！');
                return false;
            }

            showOperaMask($window.attr('id'));

            var reportName = $("#reportName").val();
            var reportDepartment = $("#reportDepartment").val();
            var reportLevel = $("#reportLevel").data("kendoComboBox").value();
            var reportCode = $("#reportCode").val();
            var reportDate = $("#reportDatepicker").val();
            var reportCategory = $("#report_Category").val();
            var reportComment = $("#reportComment").val();
            var reportStatus = $("#reportStatus").find("input:checked").val()
            var reportUrl = $("#reportUrl").val();

            //var roleData = JSON.stringify(roles); !!! NOT SUPPORT !!!
            var roleData = '';
            for (var i = 0; i < roles.length; i++) {
                var role = roles[i];
                roleData += role.RoleID + '&' + role.RoleType + '&' + role.RoleName + ';';
            }

            var parentId = $window.find('button.windowConfirm').attr("data-parentId");

            //Fixed:
            jQuery.handleError = function (a, b, c, d) {


                searchCategoryReports();
                hideOperaMask($window.attr('id'));
                $window.data("kendoWindow").close();
            }

            //WARNING: This ajax filed  upload library don't support post json string. 
            $.ajaxFileUpload({
                url: '/Report/ReportCenter/EditReport',
                data: {
                    ID: reportID,
                    Name: reportName,
                    Department: reportDepartment,
                    PublishedDate: reportDate,
                    Level: reportLevel,
                    Category: reportCategory,
                    ReportCode: reportCode,
                    Comment: reportComment,
                    ParnentID: parentId,
                    Status: reportStatus,
                    ReportUrl: reportUrl,
                    Roles: roleData
                },
                secureuri: false,
                fileElementId: 'fileField',
                dataType: 'json',
                success: function (data, status) {

                    filterReports('filter');
                    $window.data("kendoWindow").close();
                },
                error: function (data, status, e) { }
            });
        }

        function bindButtonEvents(e) {

            $window.find('button.windowConfirm').bind("click", reportEditConfirm);
            $window.find('button.windowCancel').bind("click", closeReportWindow);
        }

        function unbindButtonEvents(e) {

            $window.find('button.windowConfirm').unbind("click");
            $window.find('button.windowCancel').unbind("click");
        }

        $.ajax({
            type: 'POST',
            url: '/Report/ReportCenter/GetReport',
            data: { reportID: reportID }

        }).done(function (item) {

            $("#report_Category").val(item.Category);
            $("#reportName").val(item.Name);
            $("#reportDepartment").val(item.Department);
            var reportLevel = $("#reportLevel").data("kendoComboBox")
            if (reportLevel) {
                reportLevel.value(item.Level);
            }

            $("#reportCode").val(item.ReportCode);

            var pubDate = new Date(parseInt(item.PublishedDate.substr(6)));
            $("#reportDatepicker").val(pubDate.format('yyyy-MM-dd'));

            $("#reportComment").val(item.Comment);
            //$("#reportStatus").find("input:checked").val()
            $window.find('input[value="' + item.Status + '"]').prop('checked', true);

            $("#reportUrl").val(item.ReportUrl);

            var roles = JSON.parse(item.Roles);
            reportRoleGrid.dataSource.data(roles);

            var btnConfirm = $window.find('button.windowConfirm');
            btnConfirm.attr("data-parentId", item.ParnentID);

            $window.find('label').css('font-weight', 'normal');

            $('#report_Category').unbind('click').bind('click', function () {
                initPopupCategoryTreeView('#popupCategoryTreeView', '#popupCategoryTreeViewWindow', reportWinSelector);
            });

            initWindow(reportWinSelector, '编辑报表', bindButtonEvents, unbindButtonEvents).center().open();

        }).error(function (err, status) {

            alert('数据加载失败!');
            return;
        });


    }

    var RemoveReportEvent = function (reportID) {

        bootbox.confirm('您确定要删除所选择报表？', function (result) {
            if (result) {
                $.post("Report/ReportCenter/RemoveReport", { reportID: reportID }, function (data) {

                    searchCategoryReports();
                });
            }
        });
    }

    var initCategoryTreeView = function (treeviewSelector, ctxMenuSelector, categoryWinSelector, reportWinSelector) {

        var activeItemSelector = treeviewSelector + '_tv_active';

        categoryRenderItems = 0;

        $(treeviewSelector).kendoTreeView({
            template: kendo.template($("#PostionManageTreeView-template").html()),
            dataSource: categegoryDataSource(),
            select: function (e) {

                var categoryID = $(activeItemSelector).find("input").val();

                $(treeviewSelector).find("input").prop("checked", false);
                $(activeItemSelector).find("input").first().prop("checked", true);

                searchCategoryReports();

                $(treeviewSelector + ' .k-state-focused').WinContextMenu({
                    menu: ctxMenuSelector,
                    action: function (e) {

                        switch (e.id) {
                            case "AddCategoryContextMenu": AddCategoryEvent(categoryWinSelector, categoryID); break;
                            case "AddReportContextMenu": AddReportEvent(reportWinSelector, categoryID); break;
                            case "DelContextMenu": RemoveCategoryEvent(categoryID, $(activeItemSelector)); break;
                            case "ChangeNameMenu": EditCategoryEvent(categoryWinSelector, categoryID); break;
                        }
                    }
                });


            },
            collapse: function (e) {
                $(activeItemSelector).find(".k-sprite").first().removeClass("on");
            },
            expand: function (e) {
                $(activeItemSelector).find(".k-sprite").first().addClass("on");
            },
            dataBound: function (e) {

                categoryRenderItems++;
                if (categoryRenderItems >= 1)
                    categoryTreeView.expand(".k-item");

                var clickevent = { "click": checkboxUnChange, "dblclick": checkboxUnChange };
                var mousedown = function (e) { if (e.which == 3) $(this).click(); }
                $(treeviewSelector).find(":checkbox").unbind(clickevent).bind(clickevent);
                $(treeviewSelector).off("mousedown", ".k-state-hover", mousedown).on("mousedown", ".k-state-hover", mousedown)
            }
        });

        categoryTreeView = $(treeviewSelector).data("kendoTreeView");

    }

    var initPopupCategoryTreeView = function (popupTreeviewSelector, popupCategoryWinSelector, reportWinSelector) {

        var $window = $(popupCategoryWinSelector);
        var activeItemSelector = popupTreeviewSelector + '_tv_active';

        function closeCategoryWindow() {

            $window.data("kendoWindow").close()
        }

        function categorySelectConfirm() {

            var select = $(popupTreeviewSelector).data("kendoTreeView").select();

            var categoryID = select.find('input[type=checkbox]').val();
            var categoryName = select.find('input[type=checkbox]').attr('data-displayname');

            $(reportWinSelector).find('button.windowConfirm').attr("data-parentId", categoryID);
            $('#report_Category').val(categoryName);

            $window.data("kendoWindow").close()
        }

        function bindButtonEvents() {

            $window.find('button.windowConfirm').bind("click", categorySelectConfirm);
            $window.find('button.windowCancel').bind("click", closeCategoryWindow);
        }

        function unbindButtonEvents() {

            $window.find('button.windowConfirm').unbind("click", categorySelectConfirm);
            $window.find('button.windowCancel').unbind("click", closeCategoryWindow);
        }

        function popupCategoryWindow() {

            var win = $window.data("kendoWindow");
            if (!win) {

                $window.kendoWindow({
                    width: "550px",
                    title: '选择报表分类',
                    actions: [
                        "Close"
                    ],
                    open: function (e) {
                        bindButtonEvents();
                    },
                    close: function (e) {
                        unbindButtonEvents();
                    },
                    resizable: false,
                    modal: true
                });

                win = $window.data("kendoWindow").center();
                $(window.splitters).push(win);
            }

            return win;
        }

        function loadCategoryTreeView() {

            var treeview = $(popupTreeviewSelector).data("kendoTreeView");

            if (!treeview) {

                $(popupTreeviewSelector).kendoTreeView({
                    template: kendo.template($("#PostionManageTreeView-template").html()),
                    dataSource: categegoryDataSource(),
                    select: function (e) {

                        $(popupTreeviewSelector).find("input").prop("checked", false);
                        $(activeItemSelector).find("input").first().prop("checked", true);


                    },
                    collapse: function (e) {
                        $(activeItemSelector).find(".k-sprite").first().removeClass("on");
                    },
                    expand: function (e) {
                        $(activeItemSelector).find(".k-sprite").first().addClass("on");
                    },
                    dataBound: function (e) {

                        categoryRenderItems++;
                        if (categoryRenderItems >= 1)
                            $(popupTreeviewSelector).data("kendoTreeView").expand(".k-item");
                    }
                });

            } else {

                treeview.setDataSource(categegoryDataSource());
            }
        }

        categoryRenderItems = 0;

        loadCategoryTreeView();
        popupCategoryWindow().open();

    }

    function loadReportManageView() {
        //window.title = "Report Management - Kendo UI";

        initFilterPanel();
        initReportRoleGrid("#reportRoles");
        initLayoutSplitter("#layoutSplitter");
        initGeneralValidators('#AddCategoryWindow', '#AddReportWindow');
        initGeneralControls('#AddReportWindow');
        initCategoryTreeView("#PostionManageTreeView", "#CategoryContextMenu", '#AddCategoryWindow', '#AddReportWindow');
    }

    module.exports = loadReportManageView;
})

function selectResponsibledepartments() {
    var that = $(this);

    InitSelectPersonWindow(that, "Department", function (json) {

        that.val('');
        var list = json.Root.Depts.Item;
        if (list.length > 0) {
            that.val(list[0].Name);
        }
    })

}

function selectRolePermissions() {
    var that = $(this);
    InitSelectPersonWindow(that, "Role", function (json) {
        that.val('');
        that.attr('roles', '');

        var roleIds = new Array();
        var roleNames = new Array();

        var list = json.Root.SystemRoles.Item;
        if (list.length > 0) {
            $.each(list, function (idx, item) {
                roleIds.push(item.Value);
                roleNames.push(item.Name);
            });

            that.val(roleNames.join(','));
            that.attr('roles', roleIds.join(','));
        }
    })

}

