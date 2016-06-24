define(function (require, exports, module) {
    var pane;
    var myCCModel = kendo.data.Model.define({
        id: "SysId",
        fields: {
            SysId: { type: "string" },
            ProcessFolio: { type: "string" },
            ProcInstNo: { type: "string" },
            ProcessName: {type:"string"},
            Originator: { type: "string" },
            OriginatorName: { type: "int" },
            Receiver: { type: "string" },
            ReceiverName: { type: "string" },
            Applicant: { type: "string" },
            ApplicantName: { type: "string" },
            ApplicationDate: { type: "date" },
            FormViewUrl: { type: "string" },
            CreatedDate: { type: "date" },
            ReceiverStatus: { type: "string" },
            ReceiverDate: { type: "date" },
            Comment: { type: "string" }
        }
    });

    var MyCCColumns = [
        {
            field: "ProcInstNo", title: jsResxMaintenance_SeaMyFormCC.ProcInstNo, filterable: false, sortable: false, template: function (item) {
                return "<a href='" + item.FormViewUrl + "' target='_blank' class='Folio ReadOnly' >"
                    + item.ProcInstNo + "</a>";
            }
        },
        { field: "ProcessFolio", title: jsResxMaintenance_SeaMyFormCC.ProcSubject, width:300,filterable: false, sortable: false },
        { field: "ApplicantName", title: jsResxMaintenance_SeaMyFormCC.ApplicantName, filterable: false },
        { field: "ApplicationDate", title: jsResxMaintenance_SeaMyFormCC.ApplicationDate, filterable: false, format: getDateTimeFormat() },
        { field: "CreatedDate", title: jsResxMaintenance_SeaMyFormCC.CreatedDate, filterable: false, format: getDateTimeFormat() },
        { field: "ReceiverName", title: jsResxMaintenance_SeaMyFormCC.ReceiverName, filterable: false },
        { field: "ReceiverDate", title: jsResxMaintenance_SeaMyFormCC.ReceiverDate, filterable: false, format: getDateTimeFormat() },
        { field: "Comment", title: jsResxMaintenance_SeaMyFormCC.ReviewComment, filterable: false }
        //{ field: "ProcessName", title: jsResxMaintenance_SeaMyFormCC.ProcessName, filterable: false, sortable: false }
    ];

    //var CCToMeColumns = [
    //    {
    //        field: "ProcessFolio", title: jsResxMaintenance_SeaMyFormCC.FormNo, filterable: false, template: function (item) {
    //            return "<a href='" + item.FormViewUrl + "' target='_blank' class='Folio ReadOnly' >"
    //                + item.ProcessFolio + "</a>";
    //        }
    //    },
    //    { field: "Originator", title: jsResxMaintenance_SeaMyFormCC.Originator, filterable: false },
    //    { field: "OriginatorName", title: jsResxMaintenance_SeaMyFormCC.OriginatorName, filterable: false },
    //    { field: "CreatedDate", title: jsResxMaintenance_SeaMyFormCC.CreatedDate, filterable: false, format: getDateTimeFormat() },
    //    { field: "ReceiverDate", title: jsResxMaintenance_SeaMyFormCC.ReceiverDate, filterable: false, format: getDateTimeFormat() },
    //    {
    //        title: jsResxMaintenance_SeaMyFormCC.ReceiverStatus, filterable: false, template: function (item) {
    //            if (item.ReceiverStatus == "true") {
    //                return jsResxMaintenance_SeaMyFormCC.Read;
    //            }
    //            else {
    //                return jsResxMaintenance_SeaMyFormCC.UnRead;
    //            };
    //        }
    //    },
    //    { field: "ReviewComment", title: jsResxMaintenance_SeaMyFormCC.ReviewComment, filterable: false }
    //];

    var SearchMyCC = function (searchkey, isAjax) {       
        showOperaMask();
        //InitServerQueryKendoExcelGrid("dataList", myCCModel, CCToMeColumns, "/Maintenance/MyFormCC/MyFormCC", searchkey, $(window).height() - fullwidgetH, jsResxMaintenance_SeaMyFormCC.GridHeader,
        //  function () {
        //      bindAndLoad("dataList");
        //      hideOperaMask();
        //  });
        $.ajax({
            url: '/Maintenance/MyFormCC/MyFormCC',
            type: 'POST',
            data: searchkey,
            async: (isAjax == undefined ? true : isAjax),
            error: function () { popupNotification.show("error", "info"); },
            success: function (items) {                                
                InitKendoExcelGridWithHeight("dataList", myCCModel, MyCCColumns, items, 20, jsResxMaintenance_SeaMyFormCC.GridHeader, $(window).height() - fullwidgetH,
                    function () {
                        bindAndLoad("dataList")
                    });
            },
            beforeSend: function () {
                showOperaMask();
            },
            complete: function () {
                hideOperaMask();
            }
        });
    }

    //var SearchCCToMe = function (searchkey, isAjax) {
    //    $.ajax({
    //        url: '/Maintenance/MyFormCC/FormCCToMe',
    //        type: 'POST',
    //        data: searchkey,
    //        async: (isAjax == undefined ? true : isAjax),
    //        error: function () { popupNotification.show("error", "info"); },
    //        success: function (items) {
    //            InitBaseKendoGridWidthPage("formCC_CCToMe", myCCModel, CCToMeColumns, items, 30, function () {
    //                bindGridCheckbox("formCC_CCToMe");
    //            });
    //        },
    //        beforeSend: function () {
    //            showOperaMask();
    //        },
    //        complete: function () {
    //            hideOperaMask();
    //        }
    //    });
    //}

    var SearchClick = function () {
        //var curindex = $("#tabstrip li.k-state-active").attr("tabindex");
        var startdp = $("#StartDate").data("kendoDatePicker");
        var enddp = $("#EndDate").data("kendoDatePicker");
        var startDate = startdp == null ? null : startdp.value();
        var endDate = enddp == null ? null : enddp.value();
        var start = startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate();
        var end = endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate();
        var formSubject = $("#FormSubject").val();
        var processName = $("#ProcessName").val();
        var receiveStatus = $("#ReviewStatus").val();
        var searchkey = { startDate: start, endDate: end, formSubject: formSubject, processName: processName, receiveStatus: receiveStatus };
        SearchMyCC(searchkey, true);
        //if (curindex == "0") {
        //    $("#formCC_MyCC ul").html("");
        //    SearchMyCC(searchkey, true);
        //}
        //else if (curindex == "1") {
        //    $("#formCC_CCToMe ul").html("");
        //    SearchCCToMe(searchkey, true);
        //}
    }

    var LoadMyFormCC = function () {
        title = "My FormCC - Kendo UI";
        //$("#tabstrip").kendoTabStrip({
        //    animation: {
        //        open: {
        //            effects: "fadeIn"
        //        }
        //    }
        //}).data("kendoTabStrip");
        //SearchMyCC({}, false);
        //SearchCCToMe({}, false);
        SearchClick();
        $("#Select").click(SearchClick);
    }

    module.exports = LoadMyFormCC;
})