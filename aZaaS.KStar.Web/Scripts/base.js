$(function () {
    $("#NeowayHeader").width($(window).width());
    InitCurrentMenu();
    var myScroll = {};
    var browser = IEVersion();
    if (browser.browser == "IE" && "6.0,7.0,8.0,9.0".indexOf(browser.version) > -1) {
        document.attachEvent('ontouchmove', function (e) { e.preventDefault(); });
        $("#sidebar-nav").css("overflow-y", "auto");
    }
    else {
        document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
        myScroll = new IScroll('#sidebar-nav', {
            scrollbars: true,
            scrollY: true,
            mouseWheel: true,
            interactiveScrollbars: true,
            shrinkScrollbars: 'scale',
            fadeScrollbars: true,
            click: true
        });
    }
    $("body").on("click", ".menuHandle", function () {
        $("#container").toggleClass("on");
        setTimeout(function () {
            $(window).resize();
        }, 500)
    })
    $("body").on("click", ".sub-head", function () {
        if ($(this).parent().toggleClass("on").hasClass("on"))
            $(this).parent().siblings().removeClass("on").find(".sub-menu").removeClass("on").addClass("off");

        $(this).next().toggleClass("on").toggleClass("off").hasClass("on")

        if (myScroll != null && typeof myScroll.refresh == "function") {
            myScroll.refresh();
        }
    })


    $("#right").on("click", ".home", function () {
        window.location.href = kendo.format("#pane={0}&widget={1}", CurrentApp.pane, $("#widget-" + CurrentApp.pane).data("default"))
    })
    $("body").on("click", "#LogOut", function () {
        var authType = $("#LogOut").attr('data-auth-type');
        if (!authType) { alert('Invalid Authentication type!'); return; }

        if (authType == "Windows") {
            ClearCookies(); document.execCommand('ClearAuthenticationCache'); window.top.location = '/Home/Logout'
        } else if (authType == "Form" || authType == "Forms") {
            $.post('/Account/Logout', function (data) {
                if (!data) {
                    alert('There may be something wrong with logout request!\nYou can try again or report to administrator!');
                    return;
                }

                if (!data.SFIntegrated) {
                    alert(jsResxbase.LogoutSuccessful);
                    location.href = '/Account/Login?ReturnUrl=' + encodeURIComponent('/');
                    return;
                }

                var clearTokenUR_frameHtml = $('<iframe id="sf_clearTokenUR_frame" runat="server" src="' + data.ClearTokenUR + '" width="0" height="0" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="no" allowtransparency="yes"></iframe>');
                $(clearTokenUR_frameHtml).appendTo($('body'));
                $(clearTokenUR_frameHtml).load(function () {
                    var sf_logout_frameHtml = $('<iframe id="sf_logout_frame" runat="server" src="' + data.LogoutUrl + '" width="0" height="0" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="no" allowtransparency="yes"></iframe>');
                    $(sf_logout_frameHtml).appendTo($('body'));
                    $(sf_logout_frameHtml).load(function () {
                        alert(jsResxbase.LogoutSuccessful);
                        location.href = '/Account/Login?ReturnUrl=' + encodeURIComponent('/');
                    });
                });



            }, 'json');
        }

    })

    InitLeft();
    $(window).resize(sizeChange).resize();
    window.onhashchange = hashchange;
    hashchange();
});
var IEVersion = function () {
    var userAgent = navigator.userAgent,
           rMsie = /(msie\s|trident.*rv:)([\w.]+)/,
           rFirefox = /(firefox)\/([\w.]+)/,
           rOpera = /(opera).+version\/([\w.]+)/,
           rChrome = /(chrome)\/([\w.]+)/,
           rSafari = /version\/([\w.]+).*(safari)/;
    var ua = userAgent.toLowerCase();
    var match = rMsie.exec(ua);
    if (match != null) {
        return { browser: "IE", version: match[2] || "0" };
    }
    var match = rFirefox.exec(ua);
    if (match != null) {
        return { browser: match[1] || "", version: match[2] || "0" };
    }
    var match = rOpera.exec(ua);
    if (match != null) {
        return { browser: match[1] || "", version: match[2] || "0" };
    }
    var match = rChrome.exec(ua);
    if (match != null) {
        return { browser: match[1] || "", version: match[2] || "0" };
    }
    var match = rSafari.exec(ua);
    if (match != null) {
        return { browser: match[2] || "", version: match[1] || "0" };
    }
    if (match != null) {
        return { browser: "", version: "0" };
    }
}