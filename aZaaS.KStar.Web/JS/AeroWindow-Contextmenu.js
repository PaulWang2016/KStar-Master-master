/*
* Author:antianlu
* Date:2012-04-21
* Plugin name:jQuery.Contextmenu
* Address：http://www.oschina.net/code/snippet_153403_9880
* Version:0.2
* Email:atlatl333@126.com
*/
(function (cm) {
    jQuery.fn.WinContextMenu = function (options) {

        var defaults = {
            offsetX: 2,//鼠标在X轴偏移量
            offsetY: 2,//鼠标在Y轴偏移量
            speed: 300,//特效速度
            flash: !1,//特效是否开启，默认不开启
            flashMode: '',//特效模式,与flash为真时使用
            cancel: !1,//排除不出现右键菜单区域
            removeMenu: !1,//点击哪里的时候去掉菜单。
            items: [],//菜单项
            menu: "#WincontextMenu",
            action: $.noop()//自由菜单项回到事件
        };

        var opt = cm.extend(true, defaults, options);
        function create(e) {
            $(".WincontextMenu").remove();//清掉上次右击生成的菜单
            var m = cm('<ul class="WincontextMenu context-menu-list context-menu-root"></ul>').appendTo(document.body);
            //cm.each(opt.items, function (i, itm) {
            //    if (itm) {
            //        var row = cm('<li><a class="' + (itm.disable ? 'cmDisable' : '') + '" ref="sitem" href="javascript:void(0)"><span></span></a></li>').appendTo(m);
            //        itm.icon ? cm('<img src="' + itm.icon + '">').insertBefore(row.find('span')) : '';
            //        itm.text ? row.find('span').text(itm.text) : '';
            //        if (itm.action) {
            //            row.find('a').click(function () { this.className != 'cmDisable' ? itm.action(e) : null; });
            //        }
            //    }
            //});

            if (cm(opt.menu).html() != null) {
                //cm(cm('#WincontextMenu').html().replace(/#/g,'javascript:void(0)')).appendTo(m);}
                m.html(cm(opt.menu).html());
            }
            return m;
        }

        if (!(/iphone|ipad|ipod/i).test(navigator.userAgent)) {
            this.on('contextmenu', function (e) {//".k-state-selected",

                var m = create(e).show();
                var l = e.pageX + opt.offsetX,
                t = e.pageY + opt.offsetY,
                p = {
                    wh: cm(window).height(),
                    ww: cm(window).width(),
                    mh: m.height(),
                    mw: m.width()
                }
                t = (t + p.mh) >= p.wh ? (t -= p.mh) : t;//当菜单超出窗口边界时处理
                l = (l + p.mw) >= p.ww ? (l -= p.mw) : l;
                m.css({ zIndex: 1000001, left: l, top: t }).on('contextmenu', function () { return false; });
                m.find('a').click(function (e) {//呼叫新从页面增加的菜单项
                    var b = $(this).attr('ref');
                    if (b != 'sitem') { this.className != 'cmDisable' ? opt.action(this) : null; }
                    e.preventDefault();
                });
                cm(document.body).on('contextmenu click', function () {//防止有动态加载的标签失效问题
                    m.remove();
                });
                return false;
            });
        }
        else {
            if (opt.cancel) {//排除不出现右键菜单区域
                cm(opt.cancel).on('contextmenu', function (e) { return false });
            }
            this.parent().on('touchstart', ".k-state-selected", IosContext = function (e) {
                var touch = e.originalEvent.targetTouches[0];

                var y = touch.pageY;
                var x = touch.pageX;

                var that = $(this);
                var m = create(e).show();
                var l = e.pageX + opt.offsetX,
                t = e.pageY + opt.offsetY,
                p = {
                    wh: cm(window).height(),
                    ww: cm(window).width(),
                    mh: m.height(),
                    mw: m.width()
                }
                t = (t + p.mh) >= p.wh ? (t -= p.mh) : t;//当菜单超出窗口边界时处理
                l = (l + p.mw) >= p.ww ? (l -= p.mw) : l;
                t = y;
                l = x;
                m.css({ zIndex: 1000001, left: l, top: t }).on('contextmenu', function () { return false; });
                m.find('a').click(function (e) {//呼叫新从页面增加的菜单项
                    var b = $(this).attr('ref');
                    if (b != 'sitem') { this.className != 'cmDisable' ? opt.action(this) : null; }
                    e.preventDefault();
                });

                if (opt.removeMenu) {//去掉菜单
                    cm(opt.removeMenu).on('touchstart', function () { m.remove(); });
                }
                //$("#homeBody").on('touchstart', function () {//防止有动态加载的标签失效问题
                //    m.remove();
                //});
                cm(document.body).on('click', function () {//防止有动态加载的标签失效问题
                    m.remove();
                });
                that.unbind("touchstart", IosContext);

                return false;
            });
        }

        return this;
    }
})(jQuery);