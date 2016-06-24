define(function (require, exports, module) {
    var pane;
    var myCCModel = kendo.data.Model.define({
        id: "SysId",
        fields: {
            SysId: { type: "string" },
            ProcessFolio: { type: "string" },
            ProcInstNo: { type: "string" },
            ProcessName: { type: "string" },
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

    var CCToMeColumns = [
        {
            field: "ProcInstNo", title: jsResxMaintenance_SeaHaveRead.ProcInstNo, filterable: false, sortable: false, template: function (item) {
                return "<a href='" + item.FormViewUrl + "' target='_blank' class='Folio ReadOnly' >"
                    + item.ProcInstNo + "</a>";
            }
        },
        { field: "ProcessFolio", title: jsResxMaintenance_SeaHaveRead.ProcSubject,width:300, filterable: false, sortable: false },
        { field: "ApplicantName", title: jsResxMaintenance_SeaHaveRead.ApplicantName, filterable: false },
        { field: "ApplicationDate", title: jsResxMaintenance_SeaHaveRead.ApplicationDate, filterable: false, format: getDateTimeFormat() },
        { field: "CreatedDate", title: jsResxMaintenance_SeaHaveRead.CreatedDate, filterable: false, format: getDateTimeFormat() },
        { field: "OriginatorName", title: jsResxMaintenance_SeaHaveRead.OriginatorName, filterable: false },
        { field: "ReceiverDate", title: jsResxMaintenance_SeaHaveRead.ReceiverDate, filterable: false, format: getDateTimeFormat() },
        { field: "Comment", title: jsResxMaintenance_SeaHaveRead.ReviewComment, filterable: false }
        //{ field: "ProcessName", title: jsResxMaintenance_SeaHaveRead.ProcessName, filterable: false, sortable: false }
    ];

    var SearchCCToMe = function (searchkey, isAjax) {       
        //showOperaMask();
        //InitServerQueryKendoExcelGrid("dataList", myCCModel, CCToMeColumns, "/Maintenance/MyFormCC/FormCCToMe", searchkey, $(window).height() - fullwidgetH, jsResxMaintenance_SeaHaveRead.GridHeader,
        //  function () {
        //      bindAndLoad("dataList");
        //      hideOperaMask();
        //  });
        $.ajax({
            url: '/Maintenance/MyFormCC/FormCCToMe',
            type: 'POST',
            data: searchkey,
            async: (isAjax == undefined ? true : isAjax),
            error: function () { popupNotification.show("error", "info"); },
            success: function (items) {
                InitKendoExcelGridWithHeight("dataList", myCCModel, CCToMeColumns, items, 20, jsResxMaintenance_SeaHaveRead.GridHeader, $(window).height() - fullwidgetH,
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

    var SearchClick = function () {
        var startdp = $("#StartDate").data("kendoDatePicker");
        var enddp = $("#EndDate").data("kendoDatePicker");
        var startDate = startdp == null ? null : startdp.value();
        var endDate = enddp == null ? null : enddp.value();
        var start = startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate();
        var end = endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate();
        var formSubject = $("#FormSubject").val();
        var processName = $("#ProcessName").val();
        var searchkey = { startDate: start, endDate: end, formSubject: formSubject, processName: processName, receiveStatus: 1, "_t": new Date() };
        SearchCCToMe(searchkey, true);
    }

    var LoadMyFormCC = function () {
        title = "My FormCC - Kendo UI";
        SearchClick();
        $("#Select").click(SearchClick);
    }

    module.exports = LoadMyFormCC;
})