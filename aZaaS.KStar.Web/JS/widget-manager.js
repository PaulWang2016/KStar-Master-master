$(document).ready(function () {
    $(".widget-container").each(function (e) {
        var renderMode = $(this).attr("widget-mode");
        if (renderMode == "iframe") return;

        var url = $(this).attr("widget-url");
        var postData = $(this).data();
        $(this).load(url, postData);
    });
});