define(function (require, exports, module) {
    var Save = function () {
        data = [];
        $(".configDetail").find(":text").each(function () {
            data.push({ ConfigKey: $(this).attr("id"), ConfigValue: $(this).val() })
        })

        $.post("/Maintenance/TDConfig/Save", { configs: kendo.stringify(data) }, function (Tips) {
            $("#ConfigSave").siblings(".tips").css("visibility", "visible");
        })
    }
    var LoadPortalView = function () {
        title = "Portal Management - Kendo UI";
        $.post("/Maintenance/TDConfig/Get", { "_t": new Date() }, function (item) {
            for (var i = 0; i < item.length; i++) {
                $("#" + item[i].ConfigKey).val(item[i].ConfigValue);
            }
        })
        $("#ConfigSave").on("click", Save);
    }
    module.exports = LoadPortalView;
})