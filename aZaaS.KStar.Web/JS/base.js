//-----------------------START--Set AjaxCache---------------------------------------
//$.ajaxSetup({
//    cache: true
//});
jQuery.support.cors = true;
$(document).ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
    var res = jqXHR.responseText;
    if (res.substr(0, 1) == '{' && res.substr(res.length - 1) == '}') {
        res = JSON.parse(res);
        if (res.error == true) {
            bootbox.dialog({
                message: res.message,
                title: jsResxbase.SystemAlert,//jqXHR.statusText
                className: "ErrorBox",
                buttons: {
                    danger: {
                        label: "OK",
                        className: "btn-danger",
                        callback: function () {

                        }
                    }
                }
            });
        }
    }
    else {
        bootbox.alert(res);
    }
});
//-----------------------END --Set AjaxCache-------------------------------------

String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}

String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}

//-----------------------START--Index---------------------------------------
//*  提取锚点信息 方法  *@
function getHashValue(url) {
    var result = {};
    var reg = new RegExp('([\\#|&])(.+?)=([^&?]*)', 'ig');
    var arr = reg.exec(url);

    while (arr) {
        result[arr[2]] = arr[3];

        arr = reg.exec(url);
    }
    return result;
}
var DestroySplitters = function (pane) {
    pane = pane ? pane : 0;
    splitters[pane] = splitters[pane] ? splitters[pane] : [];
    $.each(splitters[pane], function () {
        if (this.destroy && typeof this.destroy == 'function') {
            try{
                this.destroy();
        }
            catch (e){
                console.log(e.message);
            }
        }
    })
    splitters[pane] = [];
    kendo.destroy("body > .k-popup");
}
var AddSplitters = function (splitter) {
    if (typeof (CurrentApp) != "undefined") {
        var pane = CurrentApp.pane;
        splitters[pane] = splitters[pane] ? splitters[pane] : [];
        splitters[pane].push(splitter);
    }
    else {
        splitters.push(splitter);
    }
}

//*  以Window形式弹出 Widget 事件  *@
var PopUpWidget = function (widget) {
    widget += "?popUp=1"
    var Popwindow = $("#PopUpWindow").data("kendoWindow");
    if (!Popwindow) {
        var width = $(window).width() - 100 + "px"
        var height = $(window).height() - 100 + "px"
        $("#PopUpWindow").height($(window).height() - 100);
        $("#PopUpWindow").width($(window).width() - 100);
        $("#PopUpWindow").kendoWindow({
            width: width,
            height: height,
            title: "PopUp Window",
            actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
            ],
            iframe: true,
            resizable: false,
            content: widget
        });
        //AddSplitters($("#PopUpWindow").data("kendoWindow"));
        Popwindow = $("#PopUpWindow").data("kendoWindow");
    }
    var iframe = $("#PopUpWindow").find("iframe")
    if (iframe.attr("src") != widget) {
        iframe.attr("src", widget);
    }
    $("#PopUpWindow").find("iframe").attr("id", "iframeObject");
    $("#iframeObject").css({
        '-webkit-overflow': 'auto',
    });
    Popwindow.center().open();
    $("#PopUpWindow").css("overflow", "hidden");
}
//*  改变Navigation状态事件  *@
var navigationChange = function () {
    $(".top-menu-link .nav").find("a").each(function () {
        var href = getHashValue($(this).attr("href"));
        if (href.pane === CurrentApp.pane) {
            $(this).parent().addClass("active").siblings().removeClass("active");
        }
    })
}
//*  加载 左边 Menu 事件  *@
var LoadLeft = function () {
    $('#_MenuContext').addClass("onloading");//.animate({ opacity: 0.3 }, 150).html("<div class='loading'></div>");
    $("#_HelperContext").html("<div class='loading'></div>")
    $.get("/Home/LeftRight", { pane: CurrentApp.pane }, function (html) {
        var LeftRight = $("<div><div>");
        LeftRight.html(html);
        InitLeft(LeftRight.find("#Left").html());
        InitRight(LeftRight.find("#Right").html());
    })
    //$.get("/Home/Left", { pane: CurrentApp.pane }, InitLeft)
    //$.get("/Home/Right", { pane: CurrentApp.pane }, InitRight)
}
var InitLeft = function (html) {
    $("#_MenuContext").html(html);

    var menuItem = $("#_MenuContext").find("li");
    if (menuItem.length == 0) {
        toggleMenuHide();
        return;
    }
    $("#_MenuContext").find("a").each(function () {
        var target = $(this);
        switch (target.data("target")) {
            case "PopUp": target.attr("href", "javascript:PopUpWidget('" + target.data("url") + "')"); break;
            case "Redirect": target.attr("href", target.data("url").indexOf("javascript:") != -1 ? target.data("url") : target.data("url") + "?canClose=false"); break;
            case "Panel": target.attr("href", "#pane=" + target.data("scope") + "&widget=" + target.data("url")); break;
            case "Window": target.attr("href", target.data("url")).attr("target", "_blank"); break;
            default:
        }
    });
    if (document.addEventListener) {
        menuScroll = new IScroll('#sidebar-nav', { scrollbars: true, scrollX: false,interactiveScrollbars: true,bounce:false, scrollY: true, mouseWheel: true, click: true });
        menuScroll.on("wheelMax", function () {
            //if (this.indicator1) {
            //    this.indicator1.indicatorStyle['transition-duration'] = '150ms';
            //    this.indicator1.indicatorStyle['background-color'] = 'red';
            //}
            //if (this.indicator2) {
            //    this.indicator2.indicatorStyle['transition-duration'] = '150ms';
            //    this.indicator2.indicatorStyle['background-color'] = 'red';
            //}
            $.each(this.indicators, function () {
                this.indicatorStyle['transition-duration'] = '150ms';
                this.indicatorStyle['background-color'] = 'red';
            })
            var that = this;
            setTimeout(function () {
                //if (that.indicator1) {
                //    that.indicator1.indicatorStyle['transition-duration'] = '350ms';
                //    that.indicator1.indicatorStyle['background-color'] = 'rgba(0, 0, 0, 0.498039)';
                //}
                //if (that.indicator2) {
                //    that.indicator2.indicatorStyle['transition-duration'] = '350ms';
                //    that.indicator2.indicatorStyle['background-color'] = 'rgba(0, 0, 0, 0.498039)';
                //}
                $.each(that.indicators, function () {
                    this.indicatorStyle['transition-duration'] = '350ms';
                    this.indicatorStyle['background-color'] = 'rgba(0, 0, 0, 0.498039)';
                })
            }, 200);
        })
    }
    else {
        $("#sidebar-nav").css("overflow-y", "auto");
    }
    var accordion_head = $('.accordion > li > a.sub-head'),
    accordion_body = $('.accordion li > ul.sub-menu'), accordion_menu_a = $('.accordion > li > ul.sub-menu >li > a');

    accordion_head.on('click', function (event) {
        event.preventDefault();
        if ($(this).hasClass('active')) {
            $(this).next().stop(true, true).slideToggle('normal', LeftSlided);
            $(this).removeClass('active');
        }
        else {
            accordion_body.slideUp('normal');
            $(this).next().stop(true, true).slideToggle('normal', LeftSlided);
            accordion_head.removeClass('active');
            $(this).addClass('active');
        }
    });

    accordion_menu_a.on('click', function (event) {        
        var _temp = getHashValue($(this).attr("href"));
        var _href = "#";
        if (_temp.pane!=undefined)
        {
            _href+="pane="+_temp.pane;
        }
        if (_temp.widget!=undefined)
        {
            if (_href.length > 1) {
                _href += "&widget=" + _temp.widget;
            }
            else {
                _href += "widget=" + _temp.widget;
            }
        }        
        $(this).attr("href", _href + "&_t=" + Date.parse(new Date()));
    });
    LeftSlided();
    if ($("#_Left").hasClass("sr-only")) {//显示左边
        setTimeout(function () {
            $("#_Left").removeClass("sr-only").find(".menuHandle").click();
        }, 250);
    }
    $('#_MenuContext').removeClass("onloading");//.animate({ opacity: 1 }, 200)
}
var InitRight = function (html) {
    $("#_HelperContext").html(html);
    var docWidgets = $("#_HelperContext .widget-container");
    if (docWidgets.length == 0) {
        $("#_HelperContext").parent().css("visibility", "hidden");
    }
    else {
        $("#_HelperContext").parent().css("visibility", "visible");
        docWidgets.each(function (e) {
            var renderMode = $(this).attr("widget-mode");
            if (renderMode == "iframe") return;

            var url = $(this).attr("widget-url");
            var postData = $(this).data();
            $(this).load(url, postData, function () {
                $("#_HelperContext").parent().height($("#_HelperContext").height());
            });
        });
    }
}
//*  Menu 展开 或 关闭 后激活事件  *@
var LeftSlided = function () {
    if (menuScroll) {
        menuScroll.refresh();
    }
}
//*  控制左右滑动是否开启  *@
var IsEnableScroll = function (res) {
    res.widget = res.widget ? res.widget : apps[res.pane].widget;
    if (myScroll) {
        if (res.widget && res.widget != $("#pane-wrapper-" + res.pane).data("default")) {
            myScroll.disable();
        }
        else {
            myScroll.enable();
        }
    }
}
//*  加载Widget事件  *@
var LoadPane = function (res) {
    ClearTimer();
    var pane = $("#pane-wrapper-" + res.pane);
    IsEnableScroll(res);
    //if (pane.prop("data-Loaded") &&
    //    (!res.widget ||
    //    (apps[res.pane].widget && res.widget == apps[res.pane].widget))) {
    //    return;
    //}    
    //判断不加载Widget
    apps[res.pane] = res;
    var url = res.widget ? res.widget : pane.data("default");//"/Dashboard/Index";    
    if (url != "/DynamicWidget/Dashboard" && url != "/Maintenance/ApplicationDetail-Dashboard") {
        //除首页外，移除pane的滚动条
        pane.css("height", "auto");
    }
    else {
        pane.css("height", "100%");
    }    
    if (url) {
        DestroySplitters(res.pane);//
        $(document).find("div.bootdialog").remove();
        var subPane = pane.prop("data-Loaded", true).children().first();
        showOperaMask()
        $.post(url, { isAjax: true }, function (partialView) {            
            //登陆失效
            if (partialView.indexOf("account") > 0 && partialView.indexOf("password") > 0)
            {
                location.href = "Account/Login?ReturnUrl=%2F";
                return;
            }            
            var that = subPane.html(partialView).find(".datepicker");
            $(document).find("div.bootdialog").appendTo($('body'));
            that.kendoDatePicker({
                format: "dd/MM/yyyy",
                max: new Date(9999, 11, 31)
            });
            that.change(function () {
                if (!$(this).data("kendoDatePicker").value()) {
                    $(this).val("");
                }
            })
            //去除 input th.k-header 的事件冒泡            
            subPane.on({
                touchstart: StopPropagation,
                MSPointerDown: StopPropagation,
                mousedown: StopPropagation
            }, "input,a,button,th.k-header,.k-datepicker,.k-grid-content,.k-dropdown,.k-pager-refresh,.k-treeview");
            //document.title = title;
            refreshCurrentScrolls(true);
            setTimeout(function () {
                hideOperaMask();
            }, 250);

            setTimeout(function () {
                $(".k-scrollable").on("mousewheel", function (e) { e.stopPropagation(); })
            }, 500);            
            
        }).fail(function (e) {
            LoadPane(res);
        });
    }
}
//*  锚点改变事件  *@
var hashchange = function () {    
    var hashRes = getHashValue(location.hash);
    //hashRes.pane = hashRes.pane ? hashRes.pane : CurrentApp.pane;
    if (hashRes.pane) {
        if (!apps[hashRes.pane]) {
            apps[hashRes.pane] = hashRes;
        }
        if (CurrentApp.pane != hashRes.pane) {
            if (myScroll) {
                myScroll.scrollToElement(document.querySelector("#pane-wrapper-" + hashRes.pane));
            }
            else {
                $("#scroller").animate({ left: -$("#pane-wrapper-" + hashRes.pane)[0].offsetLeft }, 350);
            }
            CurrentApp = apps[hashRes.pane];
            LoadLeft();
        }
        navigationChange();
        LoadPane(hashRes);
    } else {
        hashRes.pane = CurrentApp.pane ? CurrentApp.pane : "Dashboard";
        var hash = "pane=" + hashRes.pane;
        if (hashRes.widget) {
            hash += "&widget=" + hashRes.widget;
        }        
        window.location.hash = hash;        
    }
}
//*  浏览器大小改变事件  *@
var sizeChange = function (e) {    
    appSize = $(window).width();
    if (appSize > 768) {
        appSize -= leftSize;
    }
    $("#scroller .pane").width(appSize);
    $("#scroller").width((appSize + 2) * appCount);
    if (myScroll) {
        myScroll.refresh();
        if (CurrentApp && CurrentApp.pane) {
            myScroll.scrollToElement(document.querySelector("#pane-wrapper-" + CurrentApp.pane));
        }
    } else if (CurrentApp && CurrentApp.pane) {//For IE 8
        $("#scroller").css("left", -$("#pane-wrapper-" + CurrentApp.pane)[0].offsetLeft + "px");
    }
    $(".fullwidget").height($(window).height() - fullwidgetH);
    var photoH = $(".top-photo").height()
    if ($("#headercontainer").css("display") == "block") {    
        if (photoH != null && photoH > 0) {
            $("div.container-fixed,div.slide-out-div").css("margin-top", photoH - bannerH + "px");
        }
        else {
            $("div.container-fixed,div.slide-out-div").css("margin-top", "");
        }
    }
    LeftSlided();
}
//*  左右滑动惯性滑动结束事件  使其 定住在某个App中  *@
var onmyScrollEnd = function () {
    if (this.x == 0 || this.x % (appSize + 2)) {
        var appIndex = Math.ceil((-this.x / (appSize + 2)) - 0.5);
        //myScroll.scrollToElement(document.querySelector('#scroller .pane:nth-child(' + appIndex + ')'));
        //appIndex--;
        pane = $("#scroller .pane:eq(" + appIndex + ")").data("pane");
        var hash = "pane=" + pane;
        if (apps[pane]) {
            if (apps[pane]["widget"]) {
                hash += "&widget=" + apps[pane]["widget"];
            }
        }
        if (window.location.hash == "#" + hash) {
            myScroll.scrollToElement(document.querySelector("#pane-wrapper-" + pane));
        }
        else {
            window.location.hash = hash;
        }
    }
    //$(this.scroller).nextAll().css("visibility", "hidden");
    //navigationChange();
    //LoadLeft();
    //LoadPane({ pane: CurrentApp.pane });
}


var InitIScroll = function () {
    document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
    myScroll = new IScroll('#wrapper', { scrollbars: true, scrollX: true, scrollY: false, mouseWheel: false, interactiveScrollbars: true, bounce: false, vScrollbar: false, hScrollbar: true, fadeScrollbars: true, click: false });
    myScroll.on('scrollEnd', onmyScrollEnd);
    //homeScroll = new IScroll('#homeBody', { scrollbars: true, scrollX: true, scrollY: true, mouseWheel: false });
}

var InitSlideOut = function () {
    $('.slide-out-div').show().tabSlideOut({
        tabHandle: '.helperHandle',
        pathToTabImage: '/images/mini_Helper.png',
        imageHeight: '130px',
        imageWidth: '31px',
        tabLocation: 'right',
        speed: 300,
        action: 'click',
        topPos: topPos,
        fixedPosition: false
    });
}

var toggleMenuHide = function () {
    if ($("#_Left").data("open")) {
        $("#_Left").animate({ left: 0 }, 350, function () {
            $("#_Left").data("open", false);
            leftSize = 210;
            $(window).resize();
        })
        $("#_Widget").animate({ left: 209 }, 350);
    }
    else {
        $("#_Left").animate({ left: -209 }, 350, function () {
            $("#_Left").data("open", true);
            leftSize = 0;
            $(window).resize();
        })
        $("#_Widget").animate({ left: 0 }, 350);
    }
}

var LoadAppPage = function () {
    //*  获取app数量  *
    appCount = $("#scroller .pane").length;
    //*  判读是否支持 IScroll  支持 则使用 IScroll  *
    if (document.addEventListener) { InitIScroll(); }
    //*  绑定浏览器大小改变事件和锚点改变事件  *
    $(window).resize(sizeChange).resize();
    window.onhashchange = hashchange;
    hashchange();
    //*  左边 Menu 开关事件  *
    $("#_Left").on("click", ".menuHandle", toggleMenuHide)
    $("#_Left").on("click", ".Home", function () {
        window.location.href = kendo.format("#pane={0}&widget={1}", CurrentApp.pane, $("#pane-wrapper-" + CurrentApp.pane).data("default"))
    })
    $("#LogOut").on("click", function () {
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

                /*
                var sf_logout_frameHtml = $('<iframe id="sf_logout_frame" runat="server" src="' + data.LogoutUrl + '" width="0" height="0" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="no" allowtransparency="yes"></iframe>');
                $(sf_logout_frameHtml).appendTo($('body'));
                $(sf_logout_frameHtml).load(function () {
                    alert(jsResxbase.LogoutSuccessful);
                    location.href = '/Account/Login?ReturnUrl=' + encodeURIComponent('/');
                });*/

            }, 'json');
        }

    })
    //*  右边 Helper 栏 生成方法  *
    InitSlideOut();
    //*  触发特殊事件  *
    onmyScrollEnd();
    setTimeout(function () {
        $(window).resize();
    }, 500);
}
//-----------------------END --Index-------------------------------------

//-----------------------START--cookie---------------------------------------
var AllArrayIn = {};

var LoadProfile = function () {
    bootbox.confirm(jsResxbase.AreyousuretoLoadProfile, function (result) {
        if (result) {
            $.getJSON("/Maintenance/UserProfile/LoadProfile", { _t: new Date() }, function (msg) {
                bootbox.confirm(jsResxbase.Areyousuretoreload, function (result) {
                    if (result) {
                        window.location.reload();
                    }
                });
            });
        }
    });
}
var SaveProfile = function () {
    bootbox.confirm(getJSMsg("BaseJS", "SaveProfile"), function (result) {
        if (result) {
            $.getJSON("/Maintenance/UserProfile/SaveProfile", { _t: new Date() }, function (msg) {
                bootbox.confirm(jsResxbase.Areyousuretoreload, function (result) {
                    if (result) {
                        window.location.reload();
                    }
                });
            });
        }
    });
}
function SettingColumnVisible(IdName, field, statu) {
    var items = AllArrayIn[IdName];
    if (!items) {
        items = {};
        AllArrayIn[IdName] = items;
    }
    if (!$.cookies.test()) {
        bootbox.alert(getJSMsg("BaseJS", "Warning"));
        return null;
    }

    if (items[field]) {
        items[field] = undefined;
    }
    else {
        items[field] = statu;
        $("#" + IdName).find(".k-grid-clear").show();
    }
    $.cookies.del(IdName + "ArrayIn")
    if (Object.getOwnPropertyNames(items).length > 0) {
        $.cookies.set(IdName + "ArrayIn", items, { hoursToLive: 1000 });
    }
    else if ($.cookies.get(IdName + "ArrayInFilters") == null) {
        $("#" + IdName).find(".k-grid-clear").hide();//当Filter 也为空时  清除按钮隐藏
    }
}
function initializeColumnVisible(IdName) {
    if (!$.cookies.test()) {
        bootbox.alert(jsResxbase.AreyousuretoLoadProfile);
        return null;
    }
    if ($.cookies.get(IdName + "ArrayIn") != null) {
        var onGoingTaskArrayOut = $.cookies.get(IdName + "ArrayIn");

        var onGoingTaskGrid = getKendoGrid(IdName);
        for (var field in onGoingTaskArrayOut) {
            var isHide = onGoingTaskArrayOut[field];
            if (isHide == true) {
                onGoingTaskGrid.hideColumn(field);
            }
            else if (isHide == false) {
                onGoingTaskGrid.showColumn(field);
            }
        }
        $("#" + IdName).find(".k-grid-clear").show();
    }
}
function ShowHiddenColumn(IdName) {
    if (!$.cookies.test()) {
        bootbox.alert(jsResxbase.AreyousuretoLoadProfile);
        return null;
    }
    if ($.cookies.get(IdName + "ArrayIn") != null) {
        var HiddenColumns = $.cookies.get(IdName + "ArrayIn");
        var grid = getKendoGrid(IdName);
        for (var i = 0; i < HiddenColumns.length; i++) {
            grid.showColumn(HiddenColumns[i]);
        }
    }
}
function SettingFilters(IdName) {
    if (!$.cookies.test()) {
        bootbox.alert(jsResxbase.AreyousuretoLoadProfile);
        return null;
    }
    var grid = getKendoGrid(IdName);
    grid.dataSource.bind("change", function (e) {
        $.cookies.del(IdName + "ArrayInFilters");
        $("#" + IdName).find(".k-header").find(".k-i-funnel-clear").remove();
        if (grid.dataSource.filter() != null) {
            var list = grid.dataSource.filter().filters;
            var ArrayIn = new Array();
            for (var i = 0; i < list.length; i++) {
                var field = list[i].field;
                var operator = list[i].operator;
                var value = list[i].value;
                str = field + "." + operator + "." + value;
                ArrayIn.push(str);
                GetColumnMenu(IdName, field);
            }
            $("#" + IdName).find(".k-grid-clear").show();
            if (ArrayIn.length > 0) {
                $.cookies.set(IdName + "ArrayInFilters", ArrayIn, { hoursToLive: 1000 });
            }
        }
        else if ($.cookies.get(IdName + "ArrayIn") == null) {
            $("#" + IdName).find(".k-grid-clear").hide();//当HiddenColumn 也为空时  清除按钮隐藏
        }
    })
    var workListArrayOut = $.cookies.get(IdName + "ArrayInFilters");
    if (workListArrayOut != null) {
        var filter = new Array();
        for (var i = 0; i < workListArrayOut.length; i++) {
            var worklistvalues = workListArrayOut[i].split(".");
            filter.push({ field: worklistvalues[0], operator: worklistvalues[1], value: worklistvalues[2] });
        }
        $("#" + IdName).find(".k-grid-clear").show();
        grid.dataSource.filter(filter);
    }
}
function ClearFilters(IdName) {
    //清空 Filters
    var grid = getKendoGrid(IdName);
    $.cookies.del(IdName + "ArrayInFilters");
    grid.dataSource.filter(null);
    //清空  隐藏或显示的列
    if ($.cookies.get(IdName + "ArrayIn") != null) {
        var ArrayOut = $.cookies.get(IdName + "ArrayIn");
        for (var field in ArrayOut) {
            var isHide = ArrayOut[field];
            if (isHide == true) {
                grid.showColumn(field);
            }
            else if (isHide == false) {
                grid.hideColumn(field);
            }
        }
    }
    $.cookies.del(IdName + "ArrayIn");
}
function GetColumnMenu(IdName, field) {
    $("#" + IdName).find(".k-header").each(function () {
        if ($(this).data("field") == field) {
            if ($(this).find(".k-i-funnel-clear").length == 0) {
                $("<span class=\"k-icon k-i-funnel-clear\"></span>").insertBefore($(this).find(".k-i-arrowhead-s"));
            }
        }
    })
}

function SettingReorderColumn(IdName, e) {
    var indexOrderArrayIn = new Array();
    var indexOrder = getKendoGrid(IdName);
    for (var i = 0; i < indexOrder.columns.length; i++) {
        indexOrderArrayIn.push(indexOrder.columns[i].field);
    }

    var temp = indexOrderArrayIn[e.oldIndex];
    if (e.oldIndex < e.newIndex)
        for (var i = e.oldIndex; i < e.newIndex; i++)
            indexOrderArrayIn[i] = indexOrderArrayIn[i + 1];
    else if (e.newIndex < e.oldIndex)
        for (var i = e.oldIndex; e.newIndex < i; i--)
            indexOrderArrayIn[i] = indexOrderArrayIn[i - 1];
    indexOrderArrayIn[i] = temp;

    $.cookies.del(IdName + "ReorderColumn");
    $.cookies.set(IdName + "ReorderColumn", indexOrderArrayIn, { hoursToLive: 1000 });
}
function InitializeReorderColumn(IdName) {
    if ($.cookies.get(IdName + "ReorderColumn") != null) {
        indexOrderArrayIn = $.cookies.get(IdName + "ReorderColumn");
        var indexOrder = getKendoGrid(IdName);

        for (var i = 0; i < indexOrderArrayIn.length; i++) {
            for (var j = 0; j < indexOrder.columns.length; j++) {
                if (indexOrderArrayIn[i] == indexOrder.columns[j].field && i != j) {
                    indexOrder.reorderColumn(i, indexOrder.columns[j]);
                }
            }
        }
    }

}
function SettingColumnResize(IdName, e) {
    ResizeArray = new Array();
    temp = true;
    ResizeArray.push(["tableWidth", $("#" + IdName).find("table").width()]);
    if ($.cookies.get(IdName + "ColumnResize") != null) {
        ResizeArray = $.cookies.get(IdName + "ColumnResize");
        ResizeArray[0] = ["tableWidth", $("#" + IdName).find("table").width()];
        for (var i = 0; i < ResizeArray.length; i++) {
            if (ResizeArray[i][0] == e.column.field) {
                ResizeArray[i][1] = e.newWidth;
                temp = false;//说明变化的宽度已经加入，不用再次添加
                break;
            }
        }
    }
    if (temp) {
        ResizeArray.push([e.column.field, e.newWidth]);
    }

    $.cookies.del(IdName + "ColumnResize");
    $.cookies.set(IdName + "ColumnResize", ResizeArray, { hoursToLive: 1000 });
}
function InitializeColumnResize(workcolumns, IdName) {
    var item = new Array();
    if ($.cookies.get(IdName + "ColumnResize") != null) {
        var ResizeArray = $.cookies.get(IdName + "ColumnResize");
        $.each(workcolumns, function (i, n) {
            for (var j = 0; j < ResizeArray.length; j++) {
                if (n.field == ResizeArray[j][0]) {
                    n.width = ResizeArray[j][1];
                    break;
                }
            }
            item.push(n);
        });
        return item;
    }
    else { return workcolumns; }
}
function InitializeGridResize(IdName) {
    if ($.cookies.get(IdName + "ColumnResize") != null) {
        var ResizeArray = $.cookies.get(IdName + "ColumnResize");
        $("#" + IdName).find("table").css("width", ResizeArray[0][1] + "px");
    }
}

function ClearCookies(target) {
    if (target) {
        var all = $.cookies.get();
        for (var key in all) {
            if (key.indexOf(target) == 0) {
                $.cookies.del(key)
            }
        }
    }
    else {
        var all = $.cookies.get();
        for (var key in all) {
            $.cookies.del(key)
        }
    }
    location.reload();
}
//-----------------------END--cookie-------------------------------------

//截断 Html 并返回 纯文本 max:最大字符数量
function subHtml(html, max) {
    html = html != null ? html : "";
    var str = html.replace(/<[^>]+>/g, "");
    if (str.length > max) {
        return str.substr(0, max);
    }
    return str;
}

var checkboxUnChange = function () {
    this.checked = !this.checked;
}

var StopPropagation = function (e) {
    e.stopPropagation();
}

var PreventDefault = function (e) {
    e.preventDefault();
}

//InitHistoryTooltip
function InitHistoryTooltip(target) {
    var urlFormat = "/Dashboard/ApprovalHistory?procInstID={0}";
    //Tooltip
    $("#" + target).kendoTooltip({
        filter: ".AHistory a",
        content: {
            url: "/Dashboard/ApprovalHistory"
        },
        autoHide: false,
        showOn: "click",
        iframe: true,
        width: 800,
        height: 150,
        position: "top",
        requestStart: function (e) {
            e.options.url = kendo.format(urlFormat, e.target.data("id"));
        },
        hide: function () {
            return false;
        }
    });
}
var isRemarks = false;
function InitRemarksTooltip(target) {
    var urlFormat = "/eForm/Remarks?Folio={0}";
    //Tooltip
    if (!isRemarks) {
        $("#" + target).kendoTooltip({
            filter: ".ARemarks a",
            content: {
                url: "/eForm/Remarks"
            },
            autoHide: false,
            showOn: "click",
            iframe: true,
            width: 800,
            height: 300,
            position: "top",
            requestStart: function (e) {
                e.options.url = kendo.format(urlFormat, e.target.data("id"));
            },
            contentLoad: function (e) {
                $(".k-tooltip-content").css("height", 300);
            }
        });
        isRemarks = true;
    }
}

//type:info success error
//time:default(5)
function ShowTip(msg, type, time) {
    //if (!type) {
    //    type = "info";
    //}
    //if (!time) {
    //    time = 5;
    //}
    //var target = $("<div class=\"k-block k-" + type + "-colored\" title=\"click to remove!\">" + msg + "</div>");
    //target.appendTo($("#TipsArea")).click(function () {
    //    target.remove();
    //})
    //setTimeout(function () { target.remove(); }, (time * 1000));

    bootbox.alert(msg);
}

function obj2str(o) {
    var r = [];
    if (typeof o == "string") return "\"" + o.replace(/([\'\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
    if (typeof o == "undefined") return "undefined";
    if (typeof o == "object") {
        if (o === null) return "null";
        else if (!o.sort) {
            for (var i in o)
                r.push(i + ":" + obj2str(o[i]))
            r = "{" + r.join() + "}"
        } else {
            for (var i = 0; i < o.length; i++)
                r.push(obj2str(o[i]))
            r = "[" + r.join() + "]"
        }
        return r;
    }
    return o.toString();
}
function showOperaMask(target) {
    //$("#" + target + " .operabar").find(".operamask").css("visibility", "visible");
    //$("#homeBody").addClass("onloading");
    $("<div class=\"modal-backdrop fade in onloading\"/>").appendTo("body")
}
function hideOperaMask(target) {
    //$("#homeBody").removeClass("onloading");
    //$("#" + target + " .operabar").find(".operamask").css("visibility", "hidden");
    $(".modal-backdrop.onloading").remove();
}

function getDateTimeFormat() {
    return '{0:' + window.DateTimeFormat + '}';
}

Date.prototype.format = function (formatStr) {
    var date = this;
    /*   
    函数：填充0字符   
    参数：value-需要填充的字符串, length-总长度   
    返回：填充后的字符串   
    */
    var zeroize = function (value, length) {
        if (!length) {
            length = 2;
        }
        value = new String(value);
        for (var i = 0, zeros = ''; i < (length - value.length) ; i++) {
            zeros += '0';
        }
        return zeros + value;
    };
    return formatStr.replace(/"[^"]*"|'[^']*'|\b(?:d{1,4}|M{1,4}|yy(?:yy)?|([hHmstT])\1?|[lLZ])\b/g, function ($0) {
        switch ($0) {
            case 'd': return date.getDate();
            case 'dd': return zeroize(date.getDate());
            case 'ddd': return ['Sun', 'Mon', 'Tue', 'Wed', 'Thr', 'Fri', 'Sat'][date.getDay()];
            case 'dddd': return ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][date.getDay()];
            case 'M': return date.getMonth() + 1;
            case 'MM': return zeroize(date.getMonth() + 1);
            case 'MMM': return ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][date.getMonth()];
            case 'MMMM': return ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'][date.getMonth()];
            case 'yy': return new String(date.getFullYear()).substr(2);
            case 'yyyy': return date.getFullYear();
            case 'h': return date.getHours() % 12 || 12;
            case 'hh': return zeroize(date.getHours() % 12 || 12);
            case 'H': return date.getHours();
            case 'HH': return zeroize(date.getHours());
            case 'm': return date.getMinutes();
            case 'mm': return zeroize(date.getMinutes());
            case 's': return date.getSeconds();
            case 'ss': return zeroize(date.getSeconds());
            case 'l': return date.getMilliseconds();
            case 'll': return zeroize(date.getMilliseconds());
            case 'tt': return date.getHours() < 12 ? 'am' : 'pm';
            case 'TT': return date.getHours() < 12 ? 'AM' : 'PM';
        }
    });
}

function TreeViewNodeToggle(id) {
    $("#" + id).delegate("li", "click", function () {
        var liid = $(this).attr("id");
        if (liid == id + "_tv_active") {
            var treeview = $("#" + id).data("kendoTreeView");
            var item = treeview.select();
            if ($(item).attr("aria-expanded") == "true") {
                treeview.collapse("#" + id + "_tv_active");
            }
            else {
                treeview.expand("#" + id + "_tv_active");
                //setTimeout(function () { treeview.expand("#" + id + "_tv_active"); }, 500);
            }
        }
    });
}


function GetDateDiff(startTime, endTime) {
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");    
    var sTime = new Date(startTime); //开始时间
    var eTime = new Date(endTime);  //结束时间    
    var c = eTime - sTime;
    var result = { day: 0, hour: 0, minute: 0 };
            
    result.day = Math.floor(c / (3600000 * 24));
    result.hour = Math.floor((c / 3600000)%24);
    result.minute = Math.floor((c % 3600000) / 60000);
    return result;
}

function Timer(callback, timeout) {
    return setInterval(function () {
        setTimeout(function () {
            setTimeout(callback, 2000);
        }, 2000);
    }, timeout);    
}

function ClearTimer()
{
    var processTimerId=$("#scroller").data("processTimerId");
    var requestTimerId=$("#scroller").data("requestTimerId");
    var ongoingTimerId=$("#scroller").data("ongoingTimerId");
    var completeTimerId = $("#scroller").data("completeTimerId");

    window.clearInterval(processTimerId);
    window.clearInterval(requestTimerId);
    window.clearInterval(ongoingTimerId);
    window.clearInterval(completeTimerId);

}

var KStar = {};