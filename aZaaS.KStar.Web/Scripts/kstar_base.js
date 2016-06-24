﻿var KStar = {}, CurrentApp = {}, apps = [], splitters = [], myScrolls,
    fullwidgetH = 260, DateTimeFormat = 'yyyy-MM-dd HH:mm:ss';
//*  锚点改变事件  *@
var hashchange = function () {
    var hashRes = getHashValue(location.hash);
    //hashRes.pane = hashRes.pane ? hashRes.pane : CurrentApp.pane;
    if (hashRes.pane) {
        if (!apps[hashRes.pane]) {
            apps[hashRes.pane] = hashRes;
        }
        if (CurrentApp.pane != hashRes.pane) {
            CurrentApp = apps[hashRes.pane];
        }
        LoadPane(hashRes);
    } else {
        hashRes.pane = CurrentApp.pane ? CurrentApp.pane : "Dashboard";
        var hash = "pane=" + hashRes.pane;
        if (hashRes.widget) {
            hash += "&widget=" + hashRes.widget;
        }
        window.location.hash = hash;
    }
    InitCurrentMenu();
}

//*  浏览器大小改变事件  *@
var sizeChange = function (e) {
    $(".fullwidget").height($(window).height() - fullwidgetH);
    //appSize = $(window).width();
    //if (appSize > 768) {
    //    appSize -= leftSize;
    //}
    //$("#scroller .pane").width(appSize);
    //$("#scroller").width((appSize + 2) * appCount);
    //if (myScroll) {
    //    myScroll.refresh();
    //    if (CurrentApp && CurrentApp.pane) {
    //        myScroll.scrollToElement(document.querySelector("#pane-wrapper-" + CurrentApp.pane));
    //    }
    //} else if (CurrentApp && CurrentApp.pane) {//For IE 8
    //    $("#scroller").css("left", -$("#pane-wrapper-" + CurrentApp.pane)[0].offsetLeft + "px");
    //}
    //var photoH = $(".top-photo").height()
    //if ($("#headercontainer").css("display") == "block") {
    //    if (photoH != null && photoH > 0) {
    //        $("div.container-fixed,div.slide-out-div").css("margin-top", photoH - bannerH + "px");
    //    }
    //    else {
    //        $("div.container-fixed,div.slide-out-div").css("margin-top", "");
    //    }
    //}
    //LeftSlided();
}

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

//*  加载Widget事件  *@
var LoadPane = function (res) {
    ClearTimer();
    var pane = $("#widget-" + res.pane);
    //IsEnableScroll(res);
    //if (pane.prop("data-Loaded") &&
    //    (!res.widget ||
    //    (apps[res.pane].widget && res.widget == apps[res.pane].widget))) {
    //    return;
    //}    
    //判断不加载Widget
    apps[res.pane] = res;
    var url = res.widget ? res.widget : pane.data("default");//"/Dashboard/Index";    
    //if (url != "/DynamicWidget/Dashboard" && url != "/Maintenance/ApplicationDetail-Dashboard") {
    //    //除首页外，移除pane的滚动条
    //    pane.css("height", "auto");
    //}
    //else {
    //    pane.css("height", "100%");
    //}
    if (url) {
        DestroySplitters(res.pane);//
        $(document).find("div.bootdialog").remove();
        var subPane = pane.prop("data-Loaded", true);
        showOperaMask()
        $.post(url, { isAjax: true }, function (partialView) {
            //登陆失效
            if (partialView.indexOf("account") > 0 && partialView.indexOf("password") > 0) {
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
            //subPane.on({
            //    touchstart: StopPropagation,
            //    MSPointerDown: StopPropagation,
            //    mousedown: StopPropagation
            //}, "input,a,button,th.k-header,.k-datepicker,.k-grid-content,.k-dropdown,.k-pager-refresh,.k-treeview");
            //document.title = title;
            //refreshCurrentScrolls(true);
            setTimeout(function () {
                hideOperaMask();
            }, 250);

            //setTimeout(function () {
            //    $(".k-scrollable").on("mousewheel", function (e) { e.stopPropagation(); })
            //}, 500);

        }).fail(function (e) {
            LoadPane(res);
        });
    }
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

var DestroySplitters = function (pane) {
    pane = pane ? pane : 0;
    splitters[pane] = splitters[pane] ? splitters[pane] : [];
    $.each(splitters[pane], function () {
        if (this.destroy && typeof this.destroy == 'function') {
            try {
                this.destroy();
            }
            catch (e) {
                console.log(e.message);
            }
        }
    })
    splitters[pane] = [];
    kendo.destroy("body > .k-popup");
}

//截断 Html 并返回 纯文本 max:最大字符数量
function subHtml(html, max) {
    html = html != null ? html : "";
    var str = html.replace(/<[^>]+>/g, "");
    if (str.length > max) {
        return str.substr(0, max);
    }
    return str;
}
function showOperaMask(target) {
    $("<div class=\"modal-backdrop fade in onloading\"/>").appendTo("body")
}
function hideOperaMask(target) {
    $(".modal-backdrop.onloading").remove();
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

function GetDateDiff(startTime, endTime) {
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");
    var sTime = new Date(startTime); //开始时间
    var eTime = new Date(endTime);  //结束时间    
    var c = eTime - sTime;
    var result = { day: 0, hour: 0, minute: 0 };

    result.day = Math.floor(c / (3600000 * 24));
    result.hour = Math.floor((c / 3600000) % 24);
    result.minute = Math.floor((c % 3600000) / 60000);
    return result;
}

function ClearTimer() {
    var processTimerId = $("#scroller").data("processTimerId");
    var requestTimerId = $("#scroller").data("requestTimerId");
    var ongoingTimerId = $("#scroller").data("ongoingTimerId");
    var completeTimerId = $("#scroller").data("completeTimerId");

    window.clearInterval(processTimerId);
    window.clearInterval(requestTimerId);
    window.clearInterval(ongoingTimerId);
    window.clearInterval(completeTimerId);

}

function Timer(callback, timeout) {
    return setInterval(function () {
        setTimeout(function () {
            setTimeout(callback, 2000);
        }, 2000);
    }, timeout);
}

function getDateTimeFormat() {
    return '{0:' + window.DateTimeFormat + '}';
}


var checkboxUnChange = function () {
    this.checked = !this.checked;
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

function ShowTip(msg, type, time) {
    bootbox.alert(msg);
}

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

function InitLeft() {    
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

    //var accordion_menu_a = $('.accordion > li > ul.sub-menu >li > a');
    //accordion_menu_a.on('click', function (event) {        
    //    $.cookies.set('currentSelectMenu', $(this).attr("href"));
    //});    
}

function InitCurrentMenu() {    
    var temphash = location.hash;    
    var currenthashRes =temphash.split('?')[0];
    if (currenthashRes != null && currenthashRes.length>0) {
        $('.accordion > li > ul.sub-menu >li > a').removeClass('active');
        if (currenthashRes == "#pane=Dashboard") {
            var defaultwidget = $("#widget-Dashboard").attr("data-default");
            if (defaultwidget == null || defaultwidget == undefined) {
                defaultwidget = "/DynamicWidget/PendingTasks";
            }
            var currentitem = $("a[href='#pane=Dashboard&widget=" + defaultwidget + "']");
            currentitem.addClass('active');
            var ul = $(currentitem).parent().parent();
            var parent = $(currentitem).parent().parent().prev();
            if (parent != undefined && ul != undefined && ul.hasClass("off")) {
                parent.click();
            }
        }
        else {
            var currentitem = $("a[href='" + currenthashRes + "']");
            if (currentitem != undefined && currentitem != null) {
                currentitem.addClass('active');
                var ul= $(currentitem).parent().parent();
                var parent = $(currentitem).parent().parent().prev();
                if (parent != undefined && ul != undefined && ul.hasClass("off")) {
                    parent.click();
                }
            }
        }
        
    }
}


function RefreshPendingTaskMenuItem()
{
    $.ajax({
        url: "/Dashboard/PendingTasks/Find",
        type: 'POST',
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

function RefreshWaitReadTaskMenuItem() {    
    $.ajax({
        url: "/Maintenance/MyFormCC/FormCCToMe",
        type: 'POST',
        dataType: 'json',
        data: {receiveStatus: 0, "_t": new Date() },
        global: false,
        success: function (item) {            
            if (item != null) {
                if ($("#sidebar-nav ul").first().find("a[data-url='/Maintenance/WaitRead']").find("span").length > 0) {
                    $("#sidebar-nav ul").first().find("a[data-url='/Maintenance/WaitRead']").find("span").html(item.length);
                }
            }
        }
    });
}