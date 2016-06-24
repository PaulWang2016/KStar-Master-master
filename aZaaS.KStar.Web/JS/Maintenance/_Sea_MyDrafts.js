define(function (require, exports, module) {
    var pane;
    var myDraftModel = kendo.data.Model.define({
        id: "FormId",
        fields: {
            FormId: { type: "int" },
            FormSubject: { type: "string" },
            ProcessName: { type: "string" },
            DraftUrl: { type: "string" },
            CreatedDate: { type: "date" }
        }
    });
    MyDraftColumns = [
{
    field: "FormSubject", title: jsResxMaintenance_SeaMyDrafts.FormSubject, filterable: false, template: function (item) {
        return "<a href='" + item.DraftUrl + "' target='_blank' class='Folio ReadOnly' >"
            + item.FormSubject + "</a>";
    }
},
{ field: "ProcessName", title: jsResxMaintenance_SeaMyDrafts.ProcessName, filterable: false, sortable: false },
{ field: "CreatedDate", title: jsResxMaintenance_SeaMyDrafts.SaveDate, filterable: false, format: getDateTimeFormat(), sortable: false },
{
    title: jsResxMaintenance_SeaMyDrafts.Operation, filterable: false, width: 58, template: function (item) {
        return "<a href='javascript:void(0)' class='k-button k-Status' id='" + item.FormId + "'><span class='glyphicon glyphicon-remove'></span></a>"
    }
}
    ];

    var DeleteDraft = function (draftId) {
        bootbox.confirm(jsResxMaintenance_SeaMyDrafts.Areyousuredeletedraft, function (result) {
            if (result) {
                $.ajax({
                    url: "/Maintenance/MyDrafts/DeleteDraft",
                    type: "POST",
                    data: { draftId: draftId },
                    traditional: true,
                    success: function (isSuccess) {
                        if (isSuccess) {
                            getKendoGrid("myDraftList").dataSource.read();
                        }
                    },
                    dataType: "json"
                })
            }
        });
    }
    var url = function () {        
        var startdp = $("#StartDate").data("kendoDatePicker");
        var enddp = $("#EndDate").data("kendoDatePicker");
        var startDate = (startdp == null ? null : startdp.value());
        var endDate = (enddp == null ? null : enddp.value());
        var folio = $("#Folio").val();
        var data = {
            StartDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
            EndDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
            folio: folio,
            "_t": new Date()
        };
        return "/Maintenance/MyDrafts/GetMyDraftsForExcel?" + SerializeJsonObject(data);
    }

    var LoadMyDrafts = function () {
        title = "My Drafts - Kendo UI";
        $("#selectbtn").click(function () {
            var startdp = $("#StartDate").data("kendoDatePicker");
            var enddp = $("#EndDate").data("kendoDatePicker");
            var startDate = (startdp == null ? null : startdp.value());
            var endDate = (enddp == null ? null : enddp.value());
            var folio = $("#Folio").val();            
            var parameterdata = {
                StartDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
                EndDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
                folio: folio,                
                "_t": new Date()
            };
            
            InitServerCustomKendoExcelGrid("myDraftList", myDraftModel, MyDraftColumns, "/Maintenance/MyDrafts/Get", parameterdata, $(window).height() - fullwidgetH, jsResxMaintenance_SeaMyDrafts.MyDrafts, url,
              function () {
                  bindAndLoad("myDraftList");
                  bindGridCheckbox("myDraftList");
              });
        }).click();



        $("#myDraftList").delegate("a.k-Status", "click", function (e) {
            var id = $(this).attr("id");
            DeleteDraft(id);
        })
    }

    module.exports = LoadMyDrafts;
})