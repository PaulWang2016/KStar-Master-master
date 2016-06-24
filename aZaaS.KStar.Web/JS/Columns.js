function SaveUrlParam(target, procInstId) {
    
    var hyperLink = $(target).data("url")
    //相对路径 与绝对路径的打开方法 
    if (hyperLink.indexOf("http") == 0) {
        //打开的是全路径则表示第三方表单 传递 refresh 使用postmessage 产生刷新
        hyperLink = hyperLink + "&refresh=" + window.location.href.replace(("\/" + window.location.hash), '');

        window.open(hyperLink);
    } else {
        window.open(hyperLink);
    }
   
    var allargs = hyperLink.split("?")[1];
    if (allargs) {
        $.post("/Dashboard/PendingTasks/SaveTaskParam", { allArgs: allargs, procInstId: procInstId });
    }
}


var getWorkInfo = function (items) {
    var b_workcolumns = [
        {
            field: "IsReaded", title: "", width: 30, template: function (item) {
                return "<a onclick='window.open(\"" + item.ViewFlowUrl + "\")' ><img src='/images/ViewFlow.gif' /></a>";
            }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
        },
        {
            field: "ProcInstNo",width:100, title: jsResxColumns.ProcInstNo, filterable: false,sortable:false, template: function (item) {
                item.HyperLink = item.HyperLink.replace("192.168.1.35", window.location.hostname);
                var clickMethod = "SaveUrlParam(this,'" + item.ProcInstID + "')";
                return "<a href='javascript:void(0);' class='Folio' data-url=" + item.HyperLink + " onclick=" + clickMethod + ">" + item.ProcInstNo + "</a>";
            }
        },
        { field: "ProcSubject", title: jsResxColumns.ProcSubject, filterable: false, sortable: false},        
        { field: "Originator", width:100,title: jsResxColumns.Originator, filterable: false },
        { field: "StartDate", width:150,title: jsResxColumns.ProcStartDate, format: getDateTimeFormat(), filterable: false },
        { field: "ActivityName", width:180,title: jsResxColumns.ActivityName, filterable: false },
        { field: "LastActivityDate",width:150, title: jsResxColumns.ProcDestinationDate, format: getDateTimeFormat(), filterable: false },
        {
           title: jsResxColumns.Status, width: 60, template: function (item) {                              
                var startdate = window.CurrentDate;
                startdate = new Date(startdate).format("yyyy-MM-dd HH:mm:ss");
                var enddate = kendo.parseDate(item.ProcessTime);
                if (enddate != null) {
                    enddate = enddate.format("yyyy-MM-dd HH:mm:ss");
                    var result = GetDateDiff(startdate, enddate);
                    if (result.day < 0 || result.hour < 0 || result.minute < 0) {
                        return "<span class='k-grid-status' ></span>";
                    }
                }
                
                return "";
            }, filterable: false, hidden: true
        }
        //{ field: "ProcessName", title: jsResxColumns.ProcessName, filterable: false, sortable: false }
        //{ field: "Process", title: jsResxColumns.FormType, filterable: { ui: processFilter } },
        //{ field: "LastActivityDate", title: jsResxColumns.LastActivityDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" } },
        //{ field: "LastActivityUser", title: "Last Activity User", filterable: { ui: lastActivityUserFilter }, hidden: true },
        //{ field: "WorkflowStep", title: jsResxColumns.WorkflowStep, filterable: { ui: workflowStepFilter } }    
    ]
    var b_WorkModel = kendo.data.Model.define({
        id: "Folio",
        fields: {
            ProcInstID: { type: "string" },
            ProcInstNo: { type: "string" },
            ProcessName: { type: "string" },
            ProcSubject: { type: "string" },
            Folio: { type: "string" },
            Originator: { type: "string" },
            ProcStartDate: { type: "date" },
            ActivityName: { type: "string" },
            StartDate: { type: "date" },
            LastActivityDate: { type: "date" }
            //Process: { type: "string" },
            //LastActivityDate: { type: "date" },
            //LastActivityUser: { type: "string" },
            //WorkflowStep: { type: "string" },
            //HyperLink: { type: "string" }
        }
    });
    $.each(items, function () {
        var that = this;
        $.each(that.BusinessData, function () {
            switch (this.ValueType) {
                case "DateTime":
                    that[this.Field] = new Date(this.ValueString);
                    break;
                default:
                    that[this.Field] = this.ValueString;
            }
            if (!b_WorkModel.fields[this.Field]) {
                var mField = "";
                var mColumn = {
                    field: this.Field,
                    title: this.DisplayName,
                    hidden: !this.IsVisible,
                    filterable: false
                };
                switch (this.ValueType) {
                    case "DateTime":
                        mField = { type: "date" };
                        mColumn.format = getDateTimeFormat();
                        break;
                    default:
                        mField = { type: "string" };
                }
                b_WorkModel.fields[this.Field] = mField;
                b_workcolumns.push(mColumn);
            }

        })
    })

    //Apprval history view column
    var colApprovalHistory = { title: jsResxColumns.ApprovalHistory, width:60,template: "<a href='javascript:void(0)' data-id='#= ProcInstID #'>View<a>", attributes: { "class": "AHistory" } };
    b_workcolumns.push(colApprovalHistory);

    return { columns: b_workcolumns, model: b_WorkModel }
}
pendingTaskCoumn = [
        {
            field: "IsReaded", title: "", width: 30, template: function (item) {
                return "<a href='" + item.ViewFlowUrl + "' target='_blank'><img src='/images/ViewFlow.gif' /></a>";
            }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
        },
{
    field: "Folio", title: jsResxColumns.FormNo, filterable: { ui: folioFilter }, template: function (item) {
        item.HyperLink = item.HyperLink.replace("192.168.1.35", window.location.hostname);
        return "<a href='" + item.HyperLink + "' target='_blank' class='Folio' >" + item.Folio + "</a>";
    }
},
{ field: "Type", title: jsResxColumns.FormType, filterable: { ui: processFilter } },
{ field: "Status", title: jsResxColumns.Status, filterable: false },
{ field: "CustomerLevel", title: jsResxColumns.RecordLevel, filterable: false },
{ title: jsResxColumns.PropertyName, template: "<lable title='#= PropertyName #'>#= PropertyName #<lable>", filterable: false },
{ field: "LastActivityDate", title: jsResxColumns.LastActivityDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" } },
{ field: "LastActivityUser", title: jsResxColumns.LastActivityUser, filterable: false, },
{ title: jsResxColumns.ApprovalHistory, template: "<a href='javascript:void(0)' data-id='#= Folio #'>View<a>", attributes: { "class": "AHistory" } },
{ field: "Requester", title: jsResxColumns.Requester, filterable: false, hidden: true },
{ field: "WorkflowStep", title: jsResxColumns.WorkflowStep, filterable: { ui: workflowStepFilter }, hidden: true },
{ field: "PropertyCode", title: jsResxColumns.PropertyCode, filterable: false, hidden: true },
{ field: "UnitCode", title: jsResxColumns.UnitCode, filterable: false, hidden: true },
{ field: "SubmitDate", title: jsResxColumns.ApplicationDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ field: "CurrentActivity", title: jsResxColumns.RecordType, filterable: false, hidden: true },
{ field: "ClusterCode", title: jsResxColumns.ClusterCode, filterable: false, hidden: true },
//{ field: "ModifiedDate", title: "Modify Date", format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ title: jsResxColumns.CustomerName, template: "<lable title='#= CustomerName #'>#= CustomerName #<lable>", filterable: false, hidden: true },
]


workcolumns = [
{
    field: "IsReaded", title: jsResxColumns.IsReaded, width: 30, template: function (item) {
        return item.IsReaded ? "<img src='/images/Icon/ico-flag-red.png' />" : "<img src='/images/Icon/ico-flag-yellow.png' />";
    }, headerTemplate: "<img src='/images/Icon/ico-flag-grey.png' />", filterable: false
},
{ field: "Folio", title: jsResxColumns.FormNo, filterable: { ui: folioFilter }, template: "<a href='#= HyperLink #' target='_blank' class='Folio' >#= Folio #</a>" },
{ field: "Requester", title: jsResxColumns.Requester, filterable: { ui: wlRequesterFilter } },
{ field: "Process", title: jsResxColumns.FormType, filterable: { ui: processFilter } },
{ field: "LastActivityDate", title: jsResxColumns.LastActivityDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ field: "LastActivityUser", title: jsResxColumns.LastActivityUser, filterable: { ui: lastActivityUserFilter }, hidden: true },
{ field: "WorkflowStep", title: jsResxColumns.WorkflowStep, filterable: { ui: workflowStepFilter } }
]

requestcolumns = [
        {
            field: "IsReaded", title: "", width: 30, template: function (item) {
                return "<a href='" + item.ViewFlowUrl + "' target='_blank'><img src='/images/ViewFlow.gif' /></a>";
            }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
        },
        {
            field: "ProcInstNo", title: jsResxColumns.ProcInstNo, filterable: false, sortable: false, width: 100, template: function (item) {

                //return "<a href='" + item.ViewUrl + "?ProcInstID=" + item.Procinstid + "' target='_blank' class='Folio ReadOnly' >"
                //    + item.Folio + "</a>";
                return "<a href='" + item.ViewUrl + "' target='_blank' class='Folio ReadOnly' >"
                    + item.ProcInstNo + "</a>";

            }
        },
{ field: "ProcSubject", title: jsResxColumns.ProcSubject, filterable: false, sortable: false },
//{ field: "Priority", title: jsResxColumns.Priority, filterable: false, width: 80 },
{ field: "StatusDesc", title: jsResxColumns.Status, filterable: false, width: 80 },
{ field: "ActivityName", title: jsResxColumns.ActivityName, filterable: false, width: 120 },
{
    field: "PrevApprovers", title: jsResxColumns.PrevApprovers, filterable: false, width: 120,
    template: function (item) {        
        return "<span title='" + item.PrevApprovers + "'>" + GetSubString(item.PrevApprovers,20) + "</span>";
    }
},
{ field: "StartDate", width:150,title: jsResxColumns.ProcStartDate, filterable: false, format: getDateTimeFormat() },
{
    field: "FinishDate", width:150,title: jsResxColumns.FinishDate, filterable: false, format: getDateTimeFormat(),
    template: function (item) {
        var date = kendo.parseDate(item.FinishDate);
        if (date == null || date == "Invalid Date") {
            return "";
        }
        return item.Status == 3 ? new Date(item.FinishDate).format(window.DateTimeFormat) : "";
    }
}
//{ field: "ProcessName", title: jsResxColumns.ProcessName, filterable: false, }
]

var insteadrequestcolumns = [
        {
            field: "IsReaded", title: "", width: 30, template: function (item) {
                return "<a href='" + item.ViewFlowUrl + "' target='_blank'><img src='/images/ViewFlow.gif' /></a>";
            }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
        },
        {
            field: "ProcInstNo", title: jsResxColumns.ProcInstNo, filterable: false, sortable: false, width: 100, template: function (item) {

                //return "<a href='" + item.ViewUrl + "?ProcInstID=" + item.Procinstid + "' target='_blank' class='Folio ReadOnly' >"
                //    + item.Folio + "</a>";
                return "<a href='" + item.ViewUrl + "' target='_blank' class='Folio ReadOnly' >"
                    + item.ProcInstNo + "</a>";

            }
        },
{ field: "ProcSubject", title: jsResxColumns.ProcSubject, filterable: false, sortable: false, width: 200 },
{ field: "Priority", title: jsResxColumns.Priority, filterable: false, width: 80 },
{ field: "StatusDesc", title: jsResxColumns.Status, filterable: false, width: 80 },
{ field: "ActivityName", title: jsResxColumns.ActivityName, filterable: false, width: 120 },
{ field: "Submitter", title: jsResxColumns.Submitter, filterable: false, width: 120 },
{ field: "StartDate", title: jsResxColumns.ProcStartDate, filterable: false, format: getDateTimeFormat() },
{
    field: "FinishDate", title: jsResxColumns.FinishDate, filterable: false, format: getDateTimeFormat(),
    template: function (item) {
        var date = kendo.parseDate(item.FinishDate);
        if (date == null || date == "Invalid Date") {
            return "";
        }
        return item.Status == 3 ? new Date(item.FinishDate).format(window.DateTimeFormat) : "";
    }
},
{ field: "ProcessName", title: jsResxColumns.ProcessName, filterable: false, }
]

onGoingcolumns = [
            {
                field: "IsReaded", title: "", width: 30, template: function (item) {
                    return "<a href='" + item.ViewFlowUrl + "' target='_blank'><img src='/images/ViewFlow.gif' /></a>";
                }, headerTemplate: "<img src='/images/ViewFlow.gif' />", filterable: false
            },
            {
                field: "ProcInstNo", title: jsResxColumns.ProcInstNo, filterable: false, sortable: false, width: 100, template: function (item) {
                    return "<a href='" + item.ViewUrl + "' target='_blank' class='Folio ReadOnly' >"
                        + item.ProcInstNo + "</a>";
                }
            },
{ field: "ProcSubject", title: jsResxColumns.ProcSubject, filterable: false, sortable: false},
//{ field: "Priority", title: jsResxColumns.Priority, filterable: false, width: 80 },
{ field: "StatusDesc", title: jsResxColumns.Status, filterable: false, width: 80 },
{ field: "ActivityName", title: jsResxColumns.ActivityName, filterable: false, width: 120 },
{ field: "Originator", title: jsResxColumns.Originator, filterable: false, width: 80 },
{
    field: "PrevApprovers", title: jsResxColumns.PrevApprovers, filterable: false, width: 120,
    template: function (item) {
        return "<span title='" + item.PrevApprovers + "'>" + GetSubString(item.PrevApprovers, 20) + "</span>";
    }
},
{ field: "StartDate",width:150, title: jsResxColumns.ProcStartDate, filterable: false, format: getDateTimeFormat() },
{
    field: "FinishDate", width:150,title: jsResxColumns.FinishDate, filterable: false, format: getDateTimeFormat(),
    template: function (item) {
        var date = kendo.parseDate(item.FinishDate);
        if (date == null || date == "Invalid Date") {
            return "";
        }
        return item.Status == 3 ? new Date(item.FinishDate).format(window.DateTimeFormat) : ""
    }
}
//{ field: "ProcessName", title: jsResxColumns.ProcessName, filterable: false, }
]

completedcolumns = [
{
    field: "ProcInstNo", title: jsResxColumns.ProcInstNo, filterable: { ui: CompletedFolioFilter }, template: function (item) {
        if (item.Folio.startWith("TDRW"))
            return "<a href='/eForm/ReadOnly/" + item.Folio + "' target='_blank' class='Folio ReadOnly' >" + item.ProcInstNo + "</a>";
        else if (item.Folio.startWith("TDCW"))
            return "<a href='/eForm/CancelReadOnly/" + item.Folio + "' target='_blank' class='Folio ReadOnly' >" + item.ProcInstNo + "</a>";
    }
},
{ field: "ProcSubject", title: jsResxColumns.ProcSubject, filterable: { ui: CompletedFolioFilter } },
{ field: "Type", title: jsResxColumns.FormType, filterable: { ui: CompletedTypeFilter } },
{ field: "Status", title: jsResxColumns.Status, filterable: { ui: CompletedStatusFilter } },
{ field: "CustomerLevel", title: jsResxColumns.RecordLevel, filterable: { ui: CompletedCustomerLevelFilter } },
{ title: jsResxColumns.PropertyName, template: "<lable title='#= PropertyName #'>#= PropertyName #<lable>", filterable: false, },
{ field: "LastActivityDate", title: jsResxColumns.LastActivityDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" } },
{ field: "LastActivityUser", title: jsResxColumns.LastActivityUser, filterable: false, },
{ title: jsResxColumns.ApprovalHistory, template: "<a href='javascript:void(0)' data-id='#= Folio #'>View<a>", attributes: { "class": "AHistory" } },
{ field: "Requester", title: jsResxColumns.Requester, filterable: { ui: CompletedRequesterFilter }, hidden: true },
{ field: "WorkflowStep", title: jsResxColumns.WorkflowStep, filterable: { ui: CompletedStepFilter }, hidden: true },
{ field: "PropertyCode", title: jsResxColumns.PropertyCode, filterable: { ui: CompletedPropertyCodeFilter }, hidden: true },
{ field: "UnitCode", title: jsResxColumns.UnitCode, filterable: { ui: CompletedUnitCodeFilter }, hidden: true },
{ field: "SubmitDate", title: jsResxColumns.ApplicationDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ field: "CurrentActivity", title: jsResxColumns.RecordType, filterable: { ui: CompletedCurrentActivityFilter }, hidden: true },
{ field: "ClusterCode", title: jsResxColumns.CustomerCode, filterable: false, hidden: true },
//{ field: "ModifiedDate", title: "Modify Date", format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ title: jsResxColumns.CustomerName, template: "<lable title='#= CustomerName #'>#= CustomerName #<lable>", filterable: false, hidden: true },
]

Draftcolumns = [
{
    field: "Folio", title: jsResxColumns.FormNo, filterable: { ui: DraftFolioFilter }, template: function (item) {
        if (item.Folio.startWith("TDRW"))
            return "<a onclick='window.open(\"/eForm/RequestForm/" + item.Folio + "\")'>" + item.Folio + "</a>";
        else if (item.Folio.startWith("TDCW"))
            return "<a onclick='window.open(\"/eForm/CancelRequest/" + item.Folio + "\")'>" + item.Folio + "</a>";
    }

},
{ field: "Type", title: jsResxColumns.FormType, filterable: { ui: DraftTypeFilter }, },
{ field: "Status", title: jsResxColumns.Status, filterable: false, },
{ field: "CustomerLevel", title: jsResxColumns.RecordLevel, filterable: { ui: DraftCustomerLevelFilter } },
{ title: jsResxColumns.PropertyName, template: "<lable title='#= PropertyName #'>#= PropertyName #<lable>", filterable: false, },
{ field: "Requester", title: jsResxColumns.Requester, filterable: { ui: DraftRequesterFilter }, hidden: true },
{ field: "PropertyCode", title: jsResxColumns.PropertyCode, filterable: { ui: DraftPropertyCodeFilter }, hidden: true },
{ field: "UnitCode", title: jsResxColumns.UnitCode, filterable: { ui: DraftUnitCodeFilter }, hidden: true },
{ field: "SubmitDate", title: jsResxColumns.ApplicationDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ field: "ClusterCode", title: jsResxColumns.ClusterCode, filterable: false, hidden: true },
{ field: "ModifiedDate", title: jsResxColumns.ModifyDate, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: true },
{ field: "CurrentActivity", title: jsResxColumns.RecordType, filterable: { ui: OnGoingCurrentActivityFilter }, hidden: true },
{ title: jsResxColumns.CustomerName, template: "<lable title='#= CustomerName #'>#= CustomerName #<lable>", filterable: false, hidden: true },
]

delegationcolumns = [
{
    title: jsResxColumns.Checked, width: 35, template: function (item) {
        return item.IsReaded ? "<input value='" + item.DelegationID + "' type='checkbox' checked='checked' />" : "<input value='" + item.DelegationID + "' type='checkbox' />";
    }, headerTemplate: "<input type='checkbox' />", filterable: false
},
{ field: "FullName", title: jsResxColumns.Process, filterable: { ui: delegationProcessFilter } },
{ field: "FromUser", title: jsResxColumns.DelegateFrom, filterable: false, hidden: true },
{ field: "ToUser", title: jsResxColumns.DelegateTo, filterable: false },
{ field: "StartDate", title: jsResxColumns.StartDate, width: 160, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" } },
{ field: "EndDate", title: jsResxColumns.EndDate, width: 160, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: false },
{ field: "Reason", title: jsResxColumns.Remarks, filterable: false, hidden: true },
{
    field: "IsEnable", title: jsResxColumns.Status, width: 200, template: function (item) {
        var template = "<font style='color:red;font-weight:bold;'>" + jsResxColumns.DateDiff + "</font>";
        var timeouttemplate = "<font style='color:red;font-weight:bold;'>" + jsResxColumns.Overdue + "</font>";
        var startdate = window.CurrentDate;
        startdate = new Date(startdate).format("yyyy-MM-dd HH:mm:ss");
        var endDate = item.EndDate.format("yyyy-MM-dd HH:mm:ss");
        var result = GetDateDiff(startdate, endDate);
        var html = "";
        if (result.day > 0) {
            html += result.day + jsResxColumns.Day;
        }
        else if (result.day < 0) {
            return timeouttemplate;
        }
        if (result.hour > 0) {
            html += result.hour + jsResxColumns.Hour;
        }
        else if (result.hour < 0) {
            return timeouttemplate;
        }
        if (result.minute > 0) {
            html += result.minute + jsResxColumns.Minute;
        }
        else if (result.minute < 0) {
            return timeouttemplate;
        }
        return template.replace("{0}", html);
    }, filterable: false, hidden: false
},
{
    field: "DelegationID",
    template: function (item) {
        var startdate = window.CurrentDate;
        startdate = new Date(startdate).format("yyyy-MM-dd HH:mm:ss");
        if (item.EndDate != undefined) {
            var endDate = item.EndDate.format("yyyy-MM-dd HH:mm:ss");
            var result = GetDateDiff(startdate, endDate);
            if (result.day < 0 || result.hour < 0 || result.minute < 0) {
                return "<a title='" + jsResxColumns.Enable + "' href='javascript:void(0)' data-delegationId='" + item.DelegationID + "' class='k-button k-button-icontext k-grid-Enable'><span class='glyphicon glyphicon-play-circle' ></span></a>";
            }
        }
        return "";

    },
    width: 80,
    title: jsResxColumns.Configure
}
]

admindelegationcolumns = [
{
    title: jsResxColumns.Checked, width: 35, template: function (item) {
        return item.IsReaded ? "<input value='" + item.DelegationID + "' type='checkbox' checked='checked' />" : "<input value='" + item.DelegationID + "' type='checkbox' />";
    }, headerTemplate: "<input type='checkbox' />", filterable: false
},
{ field: "FullName", title: jsResxColumns.Process, filterable: { ui: admindelegationProcessFilter } },
{ field: "FromUser", title: jsResxColumns.DelegateFrom, width: 130, filterable: false },
{ field: "ToUser", title: jsResxColumns.DelegateTo, filterable: false },
{ field: "StartDate", title: jsResxColumns.StartDate, width: 160, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" } },
{ field: "EndDate", title: jsResxColumns.EndDate, width: 160, format: getDateTimeFormat(), filterable: { ui: "datetimepicker" }, hidden: false },
{ field: "Reason", title: jsResxColumns.Remarks, filterable: false, hidden: true },
{
    field: "IsEnable", title: jsResxColumns.Status, width: 200, template: function (item) {
        var template = "<font style='color:red;font-weight:bold;'>" + jsResxColumns.DateDiff + "</font>";
        var timeouttemplate = "<font style='color:red;font-weight:bold;'>" + jsResxColumns.Overdue + "</font>";
        var startdate = window.CurrentDate;
        startdate = new Date(startdate).format(window.DateTimeFormat);
        var endDate = item.EndDate.format(window.DateTimeFormat);
        var result = GetDateDiff(startdate, endDate);
        var html = "";
        if (result.day > 0) {
            html += result.day + jsResxColumns.Day;
        }
        else if (result.day < 0) {
            return timeouttemplate;
        }
        if (result.hour > 0) {
            html += result.hour + jsResxColumns.Hour;
        }
        else if (result.hour < 0) {
            return timeouttemplate;
        }
        if (result.minute > 0) {
            html += result.minute + jsResxColumns.Minute;
        }
        else if (result.minute < 0) {
            return timeouttemplate;
        }
        return template.replace("{0}", html);
    }, filterable: false, hidden: false
},
{
    field: "DelegationID",
    template: function (item) {
        var startdate = window.CurrentDate;
        startdate = new Date(startdate).format("yyyy-MM-dd HH:mm:ss");
        if (item.EndDate != undefined) {
            var endDate = item.EndDate.format("yyyy-MM-dd HH:mm:ss");
            var result = GetDateDiff(startdate, endDate);
            if (result.day < 0 || result.hour < 0 || result.minute < 0) {
                return "<a title='" + jsResxColumns.Enable + "' href='javascript:void(0)'  data-delegationId='" + item.DelegationID + "' class='k-button k-button-icontext k-grid-Enable'><span class='glyphicon glyphicon-play-circle' ></span></a>";
            }
        }
        return "";

    },
    width: 80,
    title: jsResxColumns.Configure
}
]



findpositioncolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.PositionID + "' />";
        }, headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.DisplayName },
]
var processesecolumns = [
{
    title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= Folio #' />",
    headerTemplate: "<input type='checkbox' />"
},
    { field: "Process", title: jsResxColumns.Process },
];

var processColumns = [
{
    title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= FullName #' />",
    headerTemplate: "<input type='checkbox' />"
},
    { field: "FullName", title: jsResxColumns.ProcessName },
];


reporttocolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= StaffId #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.ReportTo }
]
rolecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= RoleID #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.Role }
]
positioncolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= PositionID #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.Position }
]
departmentcolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value='#= DepartmentID #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.Department }
]

employeecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.StaffId + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    { field: "DisplayName", title: jsResxColumns.DisplayName, filterable: true, sortable: false },
    { field: "Department", title: jsResxColumns.Department, filterable: false, sortable: false },
    { field: "JobTitle", title: jsResxColumns.JobTitle, filterable: false, sortable: false }
]
findstaffcolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.StaffId + "' />";
        }, headerTemplate: "<input type='checkbox' />"
    },
    { field: "DisplayName", title: jsResxColumns.DisplayName },
]


TenantInfocolumns = [
    {
        field: "ID_TDW", title: jsResxColumns.FormID, filterable: { ui: folioFilter }, template: function (item) {
            if (item.FormType == "Request")
                return "<a href='/eForm/ReadOnly/" + item.ID_TDW + "' target='_blank' class='Folio ReadOnly' >" + item.ID_TDW + "</a>";
            else if (item.FormType == "Cancel" || item.FormType == "Change")
                return "<a href='/eForm/CancelReadOnly/" + item.ID_TDW + "' target='_blank' class='Folio ReadOnly' >" + item.ID_TDW + "</a>";
        }, filterable: false
    },
     { field: "DivisionCode", title: jsResxColumns.DivisionCode, filterable: false, hidden: true },
    { field: "ClusterCode", title: jsResxColumns.ClusterCode, filterable: false, hidden: true },
    { field: "PropertyCode", title: jsResxColumns.PropertyCode, filterable: false },
    { field: "PropertyName", title: jsResxColumns.PropertyName, filterable: false },
    { field: "UnitCode", title: jsResxColumns.UnitCode, filterable: false },
    //{ field: "UnitType", title: "Unit Type", filterable: false },
    { field: "RecordType", title: jsResxColumns.RecordType, filterable: false, hidden: true },
    { field: "CustomerCode", title: jsResxColumns.CustomerCode, filterable: false },
    { field: "CustomerName", title: jsResxColumns.CustomerName, filterable: false },
    //{ field: "CustomerType", title: "Customer Type ", filterable: false },
    { field: "TradeMix", title: jsResxColumns.TradeMix, filterable: false },
    { field: "FormType", title: jsResxColumns.FormType, filterable: false },
    { field: "RecordLevel", title: jsResxColumns.RecordLevel, filterable: false },
    { field: "SubmitDate", title: jsResxColumns.ApplicationDate, format: getDateTimeFormat(), filterable: false },
    { field: "UnitAddress", title: jsResxColumns.UnitAddress, filterable: false },
    { field: "LeaseID", title: jsResxColumns.LeaseID, filterable: false },
    { field: "TradeName", title: jsResxColumns.TradeName, filterable: false },
    { field: "LeaseEndDate", title: jsResxColumns.LeaseEndDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequestCreationDate", title: jsResxColumns.RequestCreationDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequestCompletionDate", title: jsResxColumns.RequestCompletionDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequesterName", title: jsResxColumns.Requester, filterable: false },
    { field: "TenantName", title: jsResxColumns.TenantName, filterable: false },
    { field: "LeaseStartDate", title: jsResxColumns.LeaseStartDate, format: getDateTimeFormat(), filterable: false },
    //{
    //    field: "Remarks", title: "Remarks",
    //    template: "<a href='javascript:void(0)' data-id='#= ID_TDW #'>Detail<a>", attributes: { "class": "ARemarks" }
    //},
    { field: "Status", title: jsResxColumns.Status, filterable: { ui: TenantReportStatusFilter } }
]


var getDeatilInfocolumns = function (recordType) {
    DeatilInfocolumns = [
    {
        field: "ID_TDW", title: jsResxColumns.FormID, filterable: { ui: folioFilter }, template: function (item) {
            if (item.FormType == "Request") {
                return "<a href='/eForm/ReadOnly/" + item.ID_TDW + "' target='_blank' class='Folio ReadOnly' >" + item.ID_TDW + "</a>";
            } else if (item.FormType == "Cancel" || item.FormType == "Change")
                return "<a href='/eForm/CancelReadOnly/" + item.ID_TDW + "' target='_blank' class='Folio ReadOnly' >" + item.ID_TDW + "</a>";
        }, filterable: false
    },
   { field: "DivisionCode", title: jsResxColumns.DivisionCode, filterable: false, hidden: true },
    { field: "ClusterCode", title: jsResxColumns.ClusterCode, filterable: false, hidden: true },
    { field: "PropertyCode", title: jsResxColumns.PropertyCode, filterable: false },
    { field: "PropertyName", title: jsResxColumns.PropertyName, filterable: false },
    { field: "UnitCode", title: jsResxColumns.UnitCode, filterable: false },
    //{ field: "UnitType", title: "Unit Type", filterable: false },
    { field: "RecordType", title: jsResxColumns.RecordType, filterable: false, hidden: false },
    { field: "CustomerCode", title: jsResxColumns.CustomerCode, filterable: false },
    { field: "CustomerName", title: jsResxColumns.CustomerName, filterable: false },
    //{ field: "CustomerType", title: "Customer Type ", filterable: false },
    { field: "TradeMix", title: jsResxColumns.TradeMix, filterable: false },
    { field: "FormType", title: jsResxColumns.FormType, filterable: false },
    { field: "RecordLevel", title: jsResxColumns.RecordLevel, filterable: false },
    { field: "SubmitDate", title: jsResxColumns.ApplicationDate, format: getDateTimeFormat(), filterable: false },
    { field: "UnitAddress", title: jsResxColumns.UnitAddress, filterable: false },
    { field: "LeaseID", title: jsResxColumns.LeaseID, filterable: false },
    { field: "TradeName", title: jsResxColumns.TradeName, filterable: false },
    { field: "LeaseEndDate", title: jsResxColumns.LeaseEndDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequestCreationDate", title: jsResxColumns.RequestCreationDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequestCompletionDate", title: jsResxColumns.RequestCompletionDate, format: getDateTimeFormat(), filterable: false },
    { field: "RequesterName", title: jsResxColumns.Requester, filterable: false },
    { field: "TenantName", title: jsResxColumns.TradeName, filterable: false },
    { field: "LeaseStartDate", title: jsResxColumns.LeaseEndDate, format: getDateTimeFormat(), filterable: false },
    {
        field: "Remarks", title: jsResxColumns.Remarks,
        template: "<a href='javascript:void(0)' data-id='#= ID_TDW #'>Detail<a>", attributes: { "class": "ARemarks" }
    },
    { field: "Status", title: jsResxColumns.Status, filterable: { ui: DetailReportStatusFilter } }
    ]
    switch (recordType) {
        case "Accreditation":
            DeatilInfocolumns.push({ field: "Creditobtained", title: jsResxColumns.AccreditationObtained, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Details", title: jsResxColumns.Details, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Year", title: jsResxColumns.Year, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ForGrouporIndividual", title: jsResxColumns.CreditFor, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "UnitOrShop", title: jsResxColumns.UnitShopNoandDescription, filterable: false, hidden: true });
            break;
        case "Activists":
            DeatilInfocolumns.push({ field: "Association", title: jsResxColumns.NameOfAssociationorDetails, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "TermFrom", title: jsResxColumns.TermFrom, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "TermTo", title: jsResxColumns.TermTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Post", title: jsResxColumns.PostBackground, filterable: false, hidden: true });
            break;
        case "ArrearsNTQ":
            DeatilInfocolumns.push({ field: "Nature", title: jsResxColumns.Nature, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ServingNoticeDate", title: jsResxColumns.NoticeDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "DOT", title: jsResxColumns.DOT, format: getDateTimeFormat(), filterable: false, hidden: true });
            break;
        case "DesertionNTQ":
            DeatilInfocolumns.push({ field: "DesertionDate", title: jsResxColumns.DesertionDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "DOT", title: jsResxColumns.DOT, format: getDateTimeFormat(), filterable: false, hidden: true });
            break;
        case "DistraintwithBailiff":
            DeatilInfocolumns.push({ field: "IssueEmailDate", title: jsResxColumns.LegalInstructionDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearFromDate", title: jsResxColumns.ArrearsPeriodForm, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearToDate", title: jsResxColumns.ArrearsPeriodTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsSeizureAuction", title: jsResxColumns.AuctionofSeizure, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "AuctionProperty", title: jsResxColumns.AuctionProperty, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearAmount", title: jsResxColumns.Arrears, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "NoOfAttempt", title: jsResxColumns.NoOfAttempt, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "BailiffAttemptDate1", title: jsResxColumns.Bailiff1stAttemptDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "BailiffAttemptDate2", title: jsResxColumns.Bailiff2stAttemptDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "BailiffAttemptDate3", title: jsResxColumns.Bailiff3stAttemptDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearSettleDate", title: jsResxColumns.SettlingArrearDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "LegalCost", title: jsResxColumns.Legalcost, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsBillTenant", title: jsResxColumns.LegalCostBilledToTenant, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "LegalCostRecovery", title: jsResxColumns.LegalCostRecovery, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "CostRecoveryDate", title: jsResxColumns.LegalCostRecoveryDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            //DeatilInfocolumns.push({ field: "IsFinalized", title: "Remark", filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsPhase2", title: jsResxColumns.Phase2, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsPhase3", title: jsResxColumns.Phase3, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "RecoveryAmount", title: jsResxColumns.ArrearRecovery, filterable: false, hidden: true });
            break;
        case "DistraintwithoutBailiff":
            DeatilInfocolumns.push({ field: "IssueEmailDate", title: jsResxColumns.ExternalLegalLetterDateOptional, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearFromDate", title: jsResxColumns.ArrearsPeriodForm, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearToDate", title: jsResxColumns.ArrearsPeriodTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ExternalEmailDate", title: jsResxColumns.ArrearsPeriodTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearSettleDate", title: jsResxColumns.SettlingArrearDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ArrearAmount", title: jsResxColumns.Arrears, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "RecoveryAmount", title: jsResxColumns.ArrearRecovery, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "LegalCost", title: jsResxColumns.Legalcost, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsBillTenant", title: jsResxColumns.LegalCostBilledToTenant, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "LegalCostRecovery", title: jsResxColumns.LegalCostRecovery, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "CostRecoveryDate", title: jsResxColumns.LegalCostRecoveryDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "IsPhase2", title: jsResxColumns.Phase2, filterable: false, hidden: true });
            break;
        case "DistrictBoardCouncilor":
            DeatilInfocolumns.push({ field: "PeriodofTermFrom", title: jsResxColumns.PeriodoftermFrom, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "PeriodofTermTo", title: jsResxColumns.PeriodoftermTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ElectionDistrict", title: jsResxColumns.ElectionGeographicalDistrict, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ElectionDistrict2", title: jsResxColumns.Electiondistrict, filterable: false, hidden: true });
            break;
        case "LegislativeCouncillor":
            DeatilInfocolumns.push({ field: "PeriodofTermFrom", title: jsResxColumns.PeriodoftermFrom, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "PeriodofTermTo", title: jsResxColumns.PeriodoftermTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ElectionDistrict", title: jsResxColumns.Electiondistrict, filterable: false, hidden: true });
            break;
        case "MerchantChairmen":
            DeatilInfocolumns.push({ field: "Association", title: jsResxColumns.NameOfAssociationorDetails, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "TermFrom", title: jsResxColumns.TermFrom, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "TermTo", title: jsResxColumns.TermTo, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Post", title: jsResxColumns.Post, filterable: false, hidden: true });
            break;
        case "ObstructionLegalLetter":
            DeatilInfocolumns.push({ field: "IssueDate", title: jsResxColumns.LegalLetterDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            break;
        case "ObstructionNTQ":
            DeatilInfocolumns.push({ field: "Nature", title: jsResxColumns.Nature, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ServingNoticeDate", title: jsResxColumns.NoticeDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "DOT", title: jsResxColumns.DOT, format: getDateTimeFormat(), filterable: false, hidden: true });
            break;
        case "PublicRecognition":
            DeatilInfocolumns.push({ field: "RecognitionAwardName", title: jsResxColumns.NameofRecognitionAward, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Details", title: jsResxColumns.Details, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Organizer", title: jsResxColumns.Organizer, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Year", title: jsResxColumns.Year, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "ForGrouporIndividual", title: jsResxColumns.RecognitionAwardFor, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "UnitOrShop", title: jsResxColumns.UnitShopNoandDescription, filterable: false, hidden: true });
            break;
        case "TABreach":
            DeatilInfocolumns.push({ field: "IssueDate", title: jsResxColumns.LegalLetterDate, format: getDateTimeFormat(), filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Nature", title: jsResxColumns.Nature, filterable: false, hidden: true });
            DeatilInfocolumns.push({ field: "Details", title: jsResxColumns.Details, filterable: false, hidden: true });
            break;
        case "BuildingIssue":
            DeatilInfocolumns.push({ field: "Details", title: jsResxColumns.Details, filterable: false, hidden: true });
            break;
        case "PremiumLetting":
            break;
        case "LandsIssue":
            break;
        case "TiedFlat":
            break;
        default:
            break;
    }
    return DeatilInfocolumns;
}


//    function (item) {
//    return "<a href='javascript:void(0)'  data-id='#= ID_TDW #'>Detail<a>";
//}, attributes: { "class": "ARemarks" }, hidden: true


CustomerPropertyCodecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox'  value = '#= PropertyCode #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "PropertyCode", width: 120, title: jsResxColumns.PropertyCode },
    //{ field: "LeaseCode", width: 120, title: "Lease Code" },
    { field: "PropertyName", title: jsResxColumns.PropertyName }
]
CustomerUnitCodecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value = '#= UnitCode #'/>",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "UnitCode", width: 110, title: jsResxColumns.UnitCode },
    //{ field: "LeaseName", title: "Lease Name" },
    { field: "UnitName", title: jsResxColumns.UnitName },
    { field: "Status", title: jsResxColumns.Status }
]
CustomerCodecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value = '#= CustomerCode #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "CustomerCode", width: 120, title: jsResxColumns.CustomerCode },
    { field: "CustomerName", title: jsResxColumns.CustomerName }
]
UnitPropertyCodecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox' value = '#= PropertyCode #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "PropertyCode", width: 120, title: jsResxColumns.PropertyCode },
    { field: "PropertyName", title: jsResxColumns.PropertyName }
]
UnitCodecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox'  value = '#= UnitCode #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "UnitCode", width: 110, title: jsResxColumns.UnitCode },
    //{ field: "LeaseName",  title: "Lease Name" },
    { field: "UnitName", title: jsResxColumns.UnitName },
    { field: "Status", title: jsResxColumns.Status }
]

Leasecolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: "<input type='checkbox'  value = '#= LeaseCode #' />",
        headerTemplate: "<input type='checkbox' />"
    },
    { field: "LeaseCode", width: 120, title: jsResxColumns.LeaseCode },
    { field: "LeaseName", title: jsResxColumns.LeaseName },
    { field: "DOT", width: 120, title: jsResxColumns.DOEDOT }//format: getDateTimeFormat(),
]

WidgetPermissioncolumns = [
{
    title: jsResxColumns.Checked, width: 35, template: function (item) {
        return "<input value='" + item.ID + "' " + (item.Status == true ? "checked='true' " : "") + " type='checkbox' />";
    }, headerTemplate: "<input type='checkbox' />", filterable: false
},
{ field: "Key", title: jsResxColumns.Key, filterable: false },
{ field: "DisplayName", title: jsResxColumns.DisplayName, filterable: false },

]

var StartUsercolumns = [
       {
           title: jsResxColumns.Checked, width: 35, template: function (item) {
               return "<input type='checkbox' value='" + item.ID + "' />";
           }, headerTemplate: "<input type='checkbox' />", filterable: false
       },
       { field: "Value", title: jsResxColumns.DisplayName, filterable: false },
       {
           field: "UserType", title: jsResxColumns.Type, filterable: false, template: function (item) {
               var temp = "";
               if (item.UserType != undefined) {
                   switch (parseInt(item.UserType)) {
                       case 0:
                           temp = "<p>Users</p>";
                           break;
                       case 1:
                           temp = "<p>Depts</p>";
                           break;
                       case 2:
                           temp = "<p>Positions</p>";
                           break;
                       case 3:
                           temp = "<p>CustomRoles</p>";
                           break;
                       case 4:
                           temp = "<p>SystemRoles</p>";
                           break;
                   }
               }
               return temp;
           }
       }
]



var ProcessVersioncolumns = [
    {
        title: jsResxColumns.Checked, width: 35, template: function (item) {
            return "<input type='checkbox' value='" + item.ID + "' />";
        }, headerTemplate: "<input type='checkbox' />", filterable: false
    },
    { field: "VersionNo", title: jsResxColumns.VersionNo, filterable: false },
    { field: "DeployDate", title: jsResxColumns.DeployDate, filterable: false, format: getDateTimeFormat() },
    { field: "IsCurrent", title: jsResxColumns.IsCurrent, filterable: false },
    {
        command: [{ name: "export", template: "<a title='" + jsResxColumns.Export + "'  href='javascript:void(0)' class='k-button k-button-icontext k-grid-export' style='float:left;margin-left: 20px;'><span class='k-grid-export-image k-icon'></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); window.location.href = '/Maintenance/WFConfig/ExportConfigurationVersion?configuration_ProcessVersionID=' + data.ID; } },
                  { name: "import", template: "<a title='" + jsResxColumns.Import + "' href='javascript:void(0)' class='k-button k-button-icontext k-grid-import' style='margin-left: 70px;'><span class='k-grid-export-image k-icon' ></span></a>", text: "", click: function (e) { var tr = $(e.target).closest("tr"); var data = this.dataItem(tr); VersionImport(data.ProcessVersionID); } }
        ], width: 150, title: jsResxColumns.Configure
    }
]



var logrequestcolumns = [    
    { field: "ID", title: jsResxColumns.LogID, filterable: false },
    { field: "Name", title: jsResxColumns.LogName, filterable: false},
    { field: "RequestUrl", title: jsResxColumns.LogRequestUrl, filterable: false },
    { field: "RequestType", title: jsResxColumns.LogRequestType, filterable: false },
    { field: "Parameters", title: jsResxColumns.LogParameters, filterable: false },
    { field: "Message", title: jsResxColumns.LogMessage, filterable: false },
    { field: "Details", title: jsResxColumns.LogDetails, filterable: false },
    { field: "IPAddress", title: jsResxColumns.LogIPAddress, filterable: false },
    { field: "RequestUser", title: jsResxColumns.LogRequestUser, filterable: false },
    { field: "RequestTime", title: jsResxColumns.LogRequestTime, filterable: false, format: getDateTimeFormat() }
]