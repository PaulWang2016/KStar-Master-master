function RefreshPendingTaskMenuItem() {
    $.ajax({
        url: "/Dashboard/PendingTasks/Count",
        type: 'GET',
        dataType: 'json',
        data: { "_t": new Date() },
        global: false,
        success: function (item) {
            if (item != null) {
                if ($("#sidebar-nav ul").first().find("a[data-url='/DynamicWidget/PendingTasks']").find("span")) {
                    $("#sidebar-nav ul").first().find("a[data-url='/DynamicWidget/PendingTasks']").find("span").html(item.total);
                }
            }
        }
    });
}


function RefreshDraftTaskMenuItem() {
    $.ajax({
        url: "/Maintenance/MyDrafts/Get",
        type: 'POST',
        dataType: 'json',
        data: { "_t": new Date() },
        global: false,
        success: function (item) {
            if (item != null) {
                if ($("#sidebar-nav ul").first().find("a[data-url='/Maintenance/MyDrafts']").find("span").length > 0) {
                    $("#sidebar-nav ul").first().find("a[data-url='/Maintenance/MyDrafts']").find("span").html(item.total);
                }
            }
        }
    });
}

function RefreshDelegationTaskMenuItem() {
    $.ajax({
        url: "/Maintenance/Delegations/FindDelegations",
        type: 'POST',
        dataType: 'json',
        data: { pane: (CurrentApp.pane == undefined ? "Dashboard" : CurrentApp.pane), start: "", end: "", delegateTo: "", isOverdue: "", "_t": new Date() },
        global: false,
        success: function (item) {
            if (item != null) {
                if ($("#sidebar-nav ul").first().find("a[data-url='/Maintenance/Delegation']").find("span").length > 0) {
                    $("#sidebar-nav ul").first().find("a[data-url='/Maintenance/Delegation']").find("span").html(item.length);
                }
            }
        }
    });
}

$(function () {
    RefreshPendingTaskMenuItem();
    RefreshDraftTaskMenuItem();
    RefreshDelegationTaskMenuItem();
})
