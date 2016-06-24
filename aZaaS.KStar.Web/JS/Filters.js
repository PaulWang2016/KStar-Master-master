/*My Pending Tasks Filter*/
function folioFilter(element) {
    AutoCompleteFilter(element, "workList", "Folio");
}
function processFilter(element) {
    DropDownListFilter(element, "workList", "Type");
}
function originatorFilter(element){
    DropDownListFilter(element, "workList", "Originator");
}
function activityNameFilter(element) {
    DropDownListFilter(element, "workList", "ActivityName");
}


function wlRequesterFilter(element) {
    DropDownListFilter(element, "workList", "Requester");
}

function lastActivityUserFilter(element) {
    AutoCompleteFilter(element, "workList", "LastActivityUser");
}
function workflowStepFilter(element) {
    DropDownListFilter(element, "workList", "WorkflowStep");
}
/*End My Pending Tasks Filter*/

/*Request Tasks Filter */
function RequestFolioFilter(element) {
    AutoCompleteFilter(element, "myRequestTask", "Folio");
}
function RequestCustomerLevelFilter(element) {
    AutoCompleteFilter(element, "myRequestTask", "CustomerLevel");
}
function RequestPropertyCodeFilter(element) {
    AutoCompleteFilter(element, "myRequestTask", "PropertyCode");
}
function RequestUnitCodeFilter(element) {
    AutoCompleteFilter(element, "myRequestTask", "UnitCode");
}
function RequestCurrentActivityFilter(element) {
    DropDownListFilter(element, "myRequestTask", "CurrentActivity");
}
function RequestTypeFilter(element) {
    DropDownListFilter(element, "myRequestTask", "Type");
}
function RequestStatusFilter(element) {
    DropDownListFilter(element, "myRequestTask", "Status");
}
function RequestStepFilter(element) {
    DropDownListFilter(element, "myRequestTask", "WorkflowStep");
}
/*End Request Tasks Filter*/

/*On-Going Tasks Filter */
function OnGoingFolioFilter(element) {
    AutoCompleteFilter(element, "onGoingTask", "Folio");
}
function OnGoingCustomerLevelFilter(element) {
    AutoCompleteFilter(element, "onGoingTask", "CustomerLevel");
}
function OnGoingPropertyCodeFilter(element) {
    AutoCompleteFilter(element, "onGoingTask", "PropertyCode");
}
function OnGoingUnitCodeFilter(element) {
    AutoCompleteFilter(element, "onGoingTask", "UnitCode");
}
function OnGoingRequesterFilter(element) {
    AutoCompleteFilter(element, "onGoingTask", "Requester");
}
function OnGoingCurrentActivityFilter(element) {
    DropDownListFilter(element, "onGoingTask", "CurrentActivity");
}
function OnGoingTypeFilter(element) {
    DropDownListFilter(element, "onGoingTask", "Type");
}
function OnGoingStatusFilter(element) {
    DropDownListFilter(element, "onGoingTask", "Status");
}
function OnGoingStepFilter(element) {
    DropDownListFilter(element, "onGoingTask", "WorkflowStep");
}
/*End On-Going Tasks Filter*/

/*Completed Tasks Filter*/
function CompletedFolioFilter(element) {
    AutoCompleteFilter(element, "completedTask", "Folio");
}
function CompletedCustomerLevelFilter(element) {
    AutoCompleteFilter(element, "completedTask", "CustomerLevel");
}
function CompletedPropertyCodeFilter(element) {
    AutoCompleteFilter(element, "completedTask", "PropertyCode");
}
function CompletedUnitCodeFilter(element) {
    AutoCompleteFilter(element, "completedTask", "UnitCode");
}
function CompletedRequesterFilter(element) {
    AutoCompleteFilter(element, "completedTask", "Requester");
}
function CompletedCurrentActivityFilter(element) {
    DropDownListFilter(element, "completedTask", "CurrentActivity");
}
function CompletedTypeFilter(element) {
    DropDownListFilter(element, "completedTask", "Type");
}
function CompletedStatusFilter(element) {
    DropDownListFilter(element, "completedTask", "Status");
}
function CompletedStepFilter(element) {
    DropDownListFilter(element, "completedTask", "WorkflowStep");
}
/*End Completed Tasks Filter*/

/*Delegation  Filter*/
function delegationProcessFilter(element) {
    DropDownListFilter(element, window.CurrentApp.pane + "delegationList", "FullName");
}

function admindelegationProcessFilter(element) {
    DropDownListFilter(element, "adminDelegationList", "FullName");
}
//End Delegation Filter

/*Draft  Filter*/
function DraftFolioFilter(element) {
    AutoCompleteFilter(element, "myDraft", "Folio");
}
function DraftCustomerLevelFilter(element) {
    AutoCompleteFilter(element, "myDraft", "CustomerLevel");
}
function DraftPropertyCodeFilter(element) {
    AutoCompleteFilter(element, "myDraft", "PropertyCode");
}
function DraftUnitCodeFilter(element) {
    AutoCompleteFilter(element, "myDraft", "UnitCode");
}
function DraftRequesterFilter(element) {
    AutoCompleteFilter(element, "myDraft", "Requester");
}
function DraftTypeFilter(element) {
    DropDownListFilter(element, "myDraft", "Type");
}
/*End Draft Filter*/


//ReportFilter

function TenantReportStatusFilter(element) {
    DropDownListFilter(element, "tenantReportList", "Status");
}

function DetailReportStatusFilter(element) {
    DropDownListFilter(element, "detailReportList", "Status");
}
