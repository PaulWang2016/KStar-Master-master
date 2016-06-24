define(function (require, exports, module) {
    var Save = function () {
        data = {
            PortalTitle: $("#PortalTitle").val(),
            LogoImageUrl: $("#LogoImageUrl").val(),
            SubLogoImageUrl: $("#SubLogoImageUrl").val(),
            IsBannerImage: $("#IsBannerImage").prop("checked"),
            BannerImageUrl: $("#BannerImageUrl").val(),
            DateTimeFormat: $("#DateTimeFormat").val(),
            LogoTitle: $("#LogoTitle").val(),
            IsLogoHeader: $("#IsLogoHeader").prop("checked")
        }
        $.post("/Maintenance/Portal/Save", data, function (Tips) {
            bootbox.confirm(Tips + jsResxMaintenance_SeaPortal.Areyousuretoreload, function (result) {
                if (result) {
                    top.location = "/";
                }
            });
        })
    }
    var ExportPortals = function () {
        $.post("/Export/ExportPortalstoExcel", function (title) {
            window.location.replace("/Export/Get?title=" + title);
        });
    }
    var ImportPortals = function () {
        var ImportPortalsWindow = $("#ImportPortalsWindow").data("kendoWindow");
        if (!ImportPortalsWindow) {
            $("#ImportPortalsWindow").kendoWindow({
                width: "800px",
                height: "100px",
                title: jsResxMaintenance_SeaPortal.ImportWindow,
                actions: [
                                "Pin",
                                "Minimize",
                                "Maximize",
                                "Close"
                ],
                iframe: true,
                resizable: false,
                content: "/Export/ImportfromExcel?type=portal"
            });
            ImportPortalsWindow = $("#ImportPortalsWindow").data("kendoWindow");
            window.AddSplitters(ImportPortalsWindow);
        }
        ImportPortalsWindow.center().open();
        $("#ImportPortalsWindow").css("overflow", "hidden");
    }
    var LoadPortalView = function () {
        title = "Portal Management - Kendo UI";
        $.post("/Maintenance/Portal/Get", { "_t": new Date() }, function (item) {
            $("#PortalTitle").val(item.PortalTitle);
            $("#LogoImageUrl").val(item.LogoImageUrl);
            $("#SubLogoImageUrl").val(item.SubLogoImageUrl);
            $("#IsBannerImage").prop("checked", item.IsBannerImage);
            $("#BannerImageUrl").val(item.BannerImageUrl);
            $("#DateTimeFormat").val(item.DateTimeFormat);
            $("#LogoTitle").val(item.LogoTitle);
            $("#IsLogoHeader").prop("checked", item.IsLogoHeader);
        })
        $("#LogoImageFile").kendoUpload({
            async: {
                saveUrl: "/Maintenance/Portal/SaveImage",
                saveField: "files"
            },
            localization: {
                select: jsResxMaintenance_SeaPortal.Selectfiles
            },
            multiple: false,
            showFileList: false,
            success: function (e) {
                var files = e.files;
                if (e.operation == "upload") {
                    for (var i = 0; i < files.length; i++) {
                        $("#LogoImageUrl").val("/images/" + files[i].name);
                    }
                }
            },
            upload: function (e) {
                checkfiles(e);
                e.data = { Field: "LogoImageUrl" };
            }
        });
        $("#SubLogoImageFile").kendoUpload({
            async: {
                saveUrl: "/Maintenance/Portal/SaveImage",
                saveField: "files"
            },
            localization: {
                select: jsResxMaintenance_SeaPortal.Selectfiles
            },
            multiple: false,
            showFileList: false,
            success: function (e) {
                var files = e.files;
                if (e.operation == "upload") {
                    for (var i = 0; i < files.length; i++) {
                        $("#SubLogoImageUrl").val("/images/" + files[i].name);
                    }
                }
            },
            upload: function (e) {
                checkfiles(e);
                e.data = { Field: "SubLogoImageUrl" };
            }
        });
        $("#BannerImageFile").kendoUpload({
            async: {
                saveUrl: "/Maintenance/Portal/SaveImage",
                saveField: "files"
            },
            localization: {
                select: jsResxMaintenance_SeaPortal.Selectfiles
            },
            multiple: false,
            showFileList: false,
            success: function (e) {
                var files = e.files;
                if (e.operation == "upload") {
                    for (var i = 0; i < files.length; i++) {
                        $("#BannerImageUrl").val("/images/" + files[i].name);
                    }
                }
            },
            upload: function (e) {
                checkfiles(e);
                e.data = { Field: "BannerImageUrl" };
            }
        });
        var checkfiles = function (e) {
            var files = e.files;
            $.each(files, function () {
                var extension = this.extension.toLowerCase()
                if (extension != ".jpg" && extension != ".gif" && extension != ".png") {
                    bootbox.alert(getJSMsg("_Sea_PortalJS", "Checkfiles"))
                    e.preventDefault();
                }
            });
        }

        $("#PortalSave").on("click", Save);
        $("#PortalExport").on("click", ExportPortals);
        $("#PortalImport").on("click", ImportPortals);
    }
    module.exports = LoadPortalView;
})