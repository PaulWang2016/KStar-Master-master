(function ($) {
    var template = {
        tab_content_template: '               <div id="menuContent#id#" class="dropdownTreeContent" url="#requesturl#" style="display: none; position: absolute; z-index:1051; overflow:auto;background: whitesmoke; height:230px;">'
                            + '                   <ul id="categorytree#id#" class="ztree"></ul>'
                            + '               </div>',
        load_mask_template: '<li id="load_mask_template" class="level0" tabindex="0" hidefocus="true" treenode=""><span class="button ico_loading"></span></li>'
    }


    $.fn.initDropdownTree = function (options) {
        var opts = $.extend({}, $.fn.initDropdownTree.defaults, options);

        $(this).each(function () {
            var that = $(this);

            var curid = that.attr("swid");

            if (!curid) {
                var id = "sw_" + Math.random().toString().substring(2);
                that.attr("swid", id);

                var temp = template.tab_content_template.replace(/#id#/g, id).replace(/#requesturl#/g, opts.requestUrl);
                $(that).parent().append(temp);
                curid = that.attr("swid");


                var setting = {
                    async: {
                        enable: true,
                        url: opts.requestUrl,
                        autoParam: ["ID"]
                    },
                    view: {
                        dblClickExpand: false
                    },
                    data: {
                        key: {
                            name: "NodeName"
                        },
                        simpleData: {
                            enable: true,
                            idKey: "ID",
                            pIdKey: "ParentID"
                        }
                    },
                    callback: {
                        beforeClick: function (treeId, treeNode) {

                        },
                        onClick: function (e, treeId, treeNode) {
                             
                            var zTree = $.fn.zTree.getZTreeObj("categorytree" + curid),
                            nodes = zTree.getSelectedNodes();
                            if (nodes.length > 0 && nodes[0].Type == "Template") {
                                var v = "", d = "";
                                nodes.sort(function compare(a, b) { return a.id - b.id; });
                                for (var i = 0, l = nodes.length; i < l; i++) {
                                    v += nodes[i].NodeName + ",";
                                    d += nodes[i].ID + ",";
                                }
                                if (v.length > 0) v = v.substring(0, v.length - 1);
                                if (d.length > 0) d = d.substring(0, d.length - 1);
                                var cityObj = $(that);
                                cityObj.val(v);
                                cityObj.attr("data", d);
                                if (opts.callback != undefined && opts.callback != null) {
                                    opts.callback(cityObj, v, d);
                                }
                            }
                        }
                    }
                };
                $.fn.zTree.init($("#categorytree" + curid), setting);

                $(that).click(function () {
                    $.fn.initDropdownTree.toggleMenu(that, curid);
                });

            }
        });
    }


    $.fn.initDropdownTree.toggleMenu = function (that, id) {
        var menuContent = $("#menuContent" + id);
        if (menuContent.css("display") == "block") {
            $.fn.initDropdownTree.hideMenu();
        }
        else {
            $.fn.initDropdownTree.showMenu(that, id);
        }
    }

    $.fn.initDropdownTree.showMenu = function (that, id) {
        var cityObj = $(that);
        var cityOffset = $(that).offset();
        //$("#menuContent" + id).css({ width: $(that).width(), left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");
        $("#menuContent" + id).css({ width: $(that).width() + 25 + "px", left: "15px", top: "34px" }).slideDown("fast");
        $("body").bind("mousedown", $.fn.initDropdownTree.onBodyDown);
    }
    $.fn.initDropdownTree.hideMenu = function () {
        $(".dropdownTreeContent").fadeOut("fast");
        $("body").unbind("mousedown", $.fn.initDropdownTree.onBodyDown);
    }
    $.fn.initDropdownTree.onBodyDown = function (event) {
        if (!(event.target.id.toString().indexOf("menuBtn") >= 0 || event.target.id.toString().indexOf("menuContent") >= 0 || $(event.target).parents(".dropdownTreeContent").length > 0)) {
            $.fn.initDropdownTree.hideMenu();
        }
    }
    $.fn.initDropdownTree.destory = function (id) {
        $("#" + id).unbind("click");
        $("#menuContent" + $("#" + id).attr("swid")).remove();
        $("#" + id).removeAttr("swid");
    };

    $.fn.initDropdownTree.defaults = {
        type: 'All',
        requestUrl: '',
        callback: undefined,
        onDetermine: undefined
    };
})(jQuery);