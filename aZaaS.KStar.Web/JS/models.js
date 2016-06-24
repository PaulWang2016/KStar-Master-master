var WorkModel = kendo.data.Model.define({
    id: "Folio",
    fields: {
        Id: { type: "string" },
        Folio: { type: "string" },
        Process: { type: "string" },
        LastActivityDate: { type: "date" },
        LastActivityUser: { type: "string" },
        WorkflowStep: { type: "string" },
        Requester: { type: "string" },
        HyperLink: { type: "string" }
    }
});

var ProcessModel = kendo.data.Model.define({
    id: "FullName",
    fields: {
        Name: { type: "string" },
        Folder: { type: "string" },
        FullName: { type: "string" }
    }
});

var TaskModel = kendo.data.Model.define({
    id: "Procinstid",
    fields: {
        Procinstid: { type: "string" },
        ProcInstNo: { type: "string" },
        ProcSubject: { type: "string" },
        Priority: { type: "string" },
        Status: { type: "string" },
        ActivityName: { type: "string" },
        PrevApprovers: { type: "string" },
        StatusDesc: { type: "string" },
        StartDate: { type: "date" },
        FinishDate: { type: "date" },
        Originator: { type: "string" },
        Folio: { type: "string" },
        FullName: { type: "string" },
        ProcessName: { type: "string" },
        ViewFlowUrl: { type: "string" }

    }
});

var InsteadRequestModel = kendo.data.Model.define({
    id: "Procinstid",
    fields: {
        Procinstid: { type: "string" },
        Priority: { type: "string" },
        ProcInstNo: { type: "string" },
        Status: { type: "string" },
        ActivityName: { type: "string" },
        StatusDesc: { type: "string" },
        StartDate: { type: "date" },
        FinishDate: { type: "date" },
        Originator: { type: "string" },
        Folio: { type: "string" },
        FullName: { type: "string" },
        ProcessName: { type: "string" },
        ViewFlowUrl: { type: "string" },
        Submitter: { type: "string" }
    }
});

var DelegationModel = kendo.data.Model.define({
    id: "DelegationID",
    fields: {
        DelegationID: { type: "number" },
        FullName: { type: "string" },
        FromUser: { type: "string" },
        ToUser: { type: "string" },
        StartDate: { type: "date" },
        EndDate: { type: "date" },
        Reason: { type: "string" },
        IsEnable: { type: "boolean" }
    }
});
var DraftModel = kendo.data.Model.define({
    fields: {
        ID: { type: "string" },
        Requester: { type: "string" },
        Folio: { type: "string" },
        UnitCode: { type: "string" },
        SubmitDate: { type: "date" },
        ModifiedDate: { type: "date" },
        Type: { type: "string" },
        ClusterCode: { type: "string" },
        Status: { type: "string" }
    }
});

var StaffModel = kendo.data.Model.define({
    id: "StaffId",
    fields: {
        StaffId: { type: "string" },
        UserName:{ type: "string" },
        FirstName: { type: "string" },
        LastName: { type: "string" },
        DisplayName: { type: "string" },
        ChineseName: { type: "string" },
        Email: { type: "string" },
        TelNo: { type: "string" },
        FaxNo: { type: "string" },
        MobileNo: { type: "string" },      
        Status: { type: "boolean" }
    }
});


var DepartmentModel = kendo.data.Model.define({
    id: "DepartmentID",
    fields: {
        DepartmentID: { type: "string" },
        DisplayName: { type: "string" }
    }
});
var RoleModel = kendo.data.Model.define({
    id: "RoleID",
    fields: {
        RoleID: { type: "string" },
        DisplayName: { type: "string" }
    }
});
var PositionModel = kendo.data.Model.define({
    id: "PositionID",
    fields: {
        PositionID: { type: "string" },
        DisplayName: { type: "string" }
    }
});


var CustomerPropertyCodeModel = kendo.data.Model.define({
    id: "PropertyCode",
    fields: {
        PropertyCode: { type: "string" },
        PropertyName: { type: "string" },
        LeaseCode: { type: "string" }
    }
})

var CustomerUnitCodeModel = kendo.data.Model.define({
    id: "UnitCode",
    fields: {
        UnitCode: { type: "string" },
        UnitName: { type: "string" },
        LeaseCode: { type: "string" },
        Status: { type: "string" }
    }
})
var CustomerCodeModel = kendo.data.Model.define({
    id: "CustomerCode",
    fields: {
        CustomerCode: { type: "string" },
        CustomerName: { type: "string" }
    }
})

var UnitPropertyCodeModel = kendo.data.Model.define({
    id: "PropertyCode",
    fields: {
        PropertyCode: { type: "string" },
        PropertyName: { type: "string" }
    }
})
var UnitCodeModel = kendo.data.Model.define({
    id: "UnitCode",
    fields: {
        UnitCode: { type: "string" },
        UnitName: { type: "string" },
        Status: { type: "string" },
        LeaseCode: { type: "string" }
    }
})
var LeaseModel = kendo.data.Model.define({
    id: "LeaseCode",
    fields: {
        LeaseCode: { type: "string" },
        DOT: { type: "string" },
        LeaseName: { type: "string" }
    }
})

var OrganizationModel = kendo.data.Node.define({
    id: "id",               //绑定ID
    hasChildren: "HasEmployees",    //绑定是否包含子节点
    ReportsTo: "ReportsTo"          //绑定父ID
})
var TenantInfoModel = kendo.data.Model.define({
    id: "ID_TDW",
    fields: {
        Id: { type: "string" },
        ID_TDW: { type: "string" },
        DivisionCode: { type: "string" },
        ClusterCode: { type: "string" },
        PropertyCode: { type: "string" },
        PropertyName: { type: "string" },
        UnitCode: { type: "string" },
        UnitType: { type: "string" },
        TypeDescription: { type: "string" },
        CustomerCode: { type: "string" },
        CustomerName: { type: "string" },
        CustomerType: { type: "string" },
        TradeMix: { type: "string" },
        FormType: { type: "string" },
        RecordLevel: { type: "string" },
        SubmitDate: { type: "date" },
        Remarks: { type: "string" },
        Status: { type: "string" },
        UnitAddress: { type: "string" },
        LeaseID: { type: "string" },
        TenantName: { type: "string" },
        LeaseStartDate: { type: "date" },
        LeaseEndDate: { type: "date" },
        RequesterName: { type: "string" },
        RequestCreationDate: { type: "date" },
        RequestCompletionDate: { type: "date" },

        IssueDate: { type: "date" },
        Nature: { type: "string" },
        Details: { type: "string" },
        RecognitionAwardName: { type: "string" },
        Organizer: { type: "string" },
        Year: { type: "string" },
        ForGrouporIndividual: { type: "string" },
        UnitOrShop: { type: "string" },
        ServingNoticeDate: { type: "date" },
        DOT: { type: "date" },
        Association: { type: "string" },
        TermFrom: { type: "date" },
        TermTo: { type: "date" },
        Post: { type: "string" },
        PeriodofTermFrom: { type: "date" },
        PeriodofTermTo: { type: "date" },
        ElectionDistrict: { type: "string" },
        ElectionDistrict2: { type: "string" },
        IssueEmailDate: { type: "date" },
        ArrearFromDate: { type: "date" },
        ArrearToDate: { type: "date" },
        ExternalEmailDate: { type: "date" },
        ArrearSettleDate: { type: "date" },
        ArrearAmount: { type: "number" },
        RecoveryAmount: { type: "number" },
        LegalCost: { type: "number" },
        IsBillTenant: { type: "boolean" },
        LegalCostRecovery: { type: "number" },
        CostRecoveryDate: { type: "date" },
        IsFinalized: { type: "boolean" },
        IsPhase2: { type: "boolean" },
        IsSeizureAuction: { type: "boolean" },
        AuctionProperty: { type: "number" },
        NoOfAttempt: { type: "string" },
        BailiffAttemptDate1: { type: "date" },
        BailiffAttemptDate2: { type: "date" },
        BailiffAttemptDate3: { type: "date" },
        IsPhase3: { type: "boolean" },
        DesertionDate: { type: "date" },
        Creditobtained: { type: "string" }
    }
});

var WidgetModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "string" },
        DisplayName: { type: "string" },
        Key: { type: "string" },
        RazorContent: { type: "string" },
        Description: { type: "string" },
        Statu: { type: "boolean" },
        MenuID: { type: "string" }
    }
});

var WidgetPermissionModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "string" },
        Key: { type: "string" },
        DisplayName: { type: "string" },
        Status: { type: "boolean" }
    }
});

var FieldExtendModel = kendo.data.Model.define({
    id: "Name",
    fields: {
        Name: { type: "string" },
        DefalutValue: { type: "string" },
        Description: { type: "string" },
        DisplayName: { type: "string" },
        Value: { type: "string" }        
    }
});

var StartUserModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "string" },
        Key: { type: "string" },
        Value: { type: "string" },
        UserType: { type: "string" },
        RefID: { type: "string" }
    }
});

var ProcessVersionModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "string" },
        Configuration_ProcessSetID: { type: "string" },
        DeployDate: { type: "date" },
        IsCurrent: { type: "boolean" },
        ProcessVersionID: { type: "string" },
        VersionNo: { type: "string" }
    }
});


var LogRequestModel = kendo.data.Model.define({
    id: "ID",
    fields: {
        ID: { type: "number" },
        Name: { type: "string" },
        RequestUrl: { type: "string" },
        RequestType: { type: "string" },
        Parameters: { type: "string" },
        Message: { type: "string" },
        Details: { type: "string" },
        IPAddress: { type: "string" },
        RequestUser: { type: "string" },
        RequestTime: { type: "date" },
    }
});
