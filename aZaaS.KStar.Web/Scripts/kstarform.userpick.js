(function ($) {
    var template = {
        userinfotemplate: '             <div id=\"SelectPersonInfo#swid#-#id#\"   class="navbar navbar-default" style=\"height:80px;padding:0px;margin-top: 5px;margin-bottom:0px;\" >'
                            + '                  <div class="panel-body" style=\"padding:8px;\">'
                            + "                       <div class=\"row\" style=\"margin-bottom: 5px;\">      "
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                  <label for=\"UserName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">用户:</label><input id=\"UserName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                             </div>"
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                 <label for=\"LastName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">姓氏:</label><input id=\"LastName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                              </div>"
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                 <label for=\"FirstName#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">名字:</label><input id=\"FirstName#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                             </div>"
                            + "                       </div>"
                             + "                       <div class=\"row\" style=\"margin-bottom: 5px;\">      "
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                  <label for=\"UserCompany#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">公司:</label><input id=\"UserCompany#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                             </div>"
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                 <label for=\"UserDept#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">部门:</label><input id=\"UserDept#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                              </div>"
                            + "                             <div class=\"col-md-4\" style=\"display:inline;\">"
                            + "                                 <label for=\"UserPosition#swid#-#id#\" style=\"cursor: pointer;font-weight: normal;margin-left:3px;\">岗位:</label><input id=\"UserPosition#swid#-#id#\" disabled=\"disabled\" type=\"text\"  style=\"vertical-align: sub;height:28px;width:130px;text-align:center;border: 1px;border-bottom-style: none;border-top-style: none;border-left-style: none;border-right-style: none;background-color: #F5F5F5;\" />"
                            + "                             </div>"
                            + "                       </div>"
                            + "                 </div>"
                            + "        </div>",
        tab_content_template: '               <div class="tab-pane fade #tabcontentactive#" id="#tabcontenttype#-#swid#-#id#">'
                            + '                   <div class="input-group" style="margin-bottom:5px;">'
                            + '                       <span class="keyword input-group-addon">关键字</span>'
                            + '                       <input id="selectKey#swid#-#id#"  type="text" class="form-control selectKey" placeholder="#selectkeytips#">'
                            + '                   </div>'
                            + '                   <div  class="navbar navbar-default" style="height:220px; margin-bottom:0px;" >'
                            + '                       <div class="panel-body" style="height:219px;">'
                            + '                           <div class="row" style="height:218px;">'
                            + '                                <div class="col-lg-4 navbar navbar-default" style="width:240px;float:left;height:200px;overflow-y: scroll; margin-left:15px;margin-right:25px;padding:0px;">'
                            + '                                      <ul id="SelectPersonManageTreeView#swid#-#id#" class="ztree"></ul>'
                            + '                                </div>'
                            + '                                <div id="left#swid#-#id#" class="col-lg-3 navbar navbar-default itemcontainer" style="width:180px;float:left;height:200px;overflow-y: scroll; padding:0px;">'
                            + '                               </div>'
                            + '                               <div class="col-lg-1" style="width:60px;float:left;margin-top:30px; height:160px;">'
                            + ' #toolbuttons#'
                            + '                               </div>'
                            + '                               <div id="right#swid#-#id#" class="col-lg-3 navbar navbar-default itemcontainer" style="width:180px;float:left;height:200px; overflow-y: scroll;padding:0px;">'
                            + '                               </div>'
                            + '                         </div>'
                            + '                      </div>'
                            + '                  </div>'
                            + "   #userinfotemplate#"
                            + '             </div>',
        tab_header_template: ' <li #tabactive# tabtype="#tabtype#">'
                            + '               <a href="##tabtype#-#swid#-#id#" data-toggle="tab">#tabname#</a>'
                            + '             </li>',
        load_mask_template: '<li id="load_mask_template" class="level0" tabindex="0" hidefocus="true" treenode=""><span class="button ico_loading"></span></li>'
    }

    //初始化控件start
    $.fn.initSelectPerson = function (options) {
        //debug(this);                
        var defaults = {
            type: 'All',
            isshownonreference: true,
            callback: undefined,
            onDetermine: undefined,
            mutilselect: true,
            pageSize: 15,
            attrbind: false,
            clickTarget:null
        }; 
        var opts = $.extend({}, defaults, options);
        var arrtype = new Array();
        if (opts.type.indexOf(",") > 0) {
            if (opts.type.indexOf("All") >= 0) {
                arrtype.push("All");
            }
            else {
                var _arr = opts.type.split(",");
                $.each(_arr, function (i, item) {
                    if ($.inArray(item, arrtype) < 0) {
                        arrtype.push(item);
                    }
                });
            }
        }
        else {
            arrtype.push(opts.type);
        }


        var toolbuttons = "";
        if (opts.mutilselect) {
            toolbuttons = '                                   <a class="btnAllRight btn btn-default" style="margin-top:5px;">&raquo;</a>'
                        + '                                   <a class="btnRight btn btn-default" style="margin-top:5px;">&gt;</a>'
                        + '                                   <a class="btnLeft btn btn-default" style="margin-top:5px;">&lt;</a>'
                        + '                                   <a class="btnAllLeft btn btn-default" style="margin-top:5px;">&laquo;</a>';
        }
        else {
            toolbuttons = '                                   <a class="btnRight btn btn-default" style="margin-top:5px;">&gt;</a>'
                       + '                                   <a class="btnLeft btn btn-default" style="margin-top:5px;">&lt;</a>';
        }

        var that = $(this);
        var curid = that.attr("swid");

        if (!curid) {
            var id = "sw_" + Math.random().toString().substring(2);
            that.attr("swid", id);

            var NewLine = '\n';
            var temp = '';
            temp += '<div class="modal fade" data-backdrop="static" id="' + id + '" tabindex="-1" role="dialog"';
            temp += '   aria-labelledby="myModalLabel" aria-hidden="true">';
            temp += '   <div class="modal-dialog" style="min-width:750px">';
            temp += '      <div class="modal-content">';
            temp += '         <div class="modal-header" style="padding:6px;">';
            temp += '            <button type="button" class="close"';
            temp += '               data-dismiss="modal" aria-hidden="true">';
            temp += '                  &times;';
            temp += '            </button>';
            temp += '            <h4 class="modal-title" id="myModalLabel">';
            temp += '               人员列表';
            temp += '            </h4>';
            temp += '         </div>';
            temp += '          <ul id="custompeopleTab' + id + '" class="nav nav-tabs" style="padding-left: 0;margin-bottom: 0;list-style: none;">';
            temp += '         </ul>';
            temp += '         <div class="modal-body">';
            temp += '             <div id="custompeopleTabstrip' + id + '" class="tab-content">';
            temp += '            </div>';
            temp += '         </div>';
            temp += '         <div class="modal-footer">';
            temp += '            <button type="button" class="btn btn-default"';
            temp += '               data-dismiss="modal">关闭';
            temp += '            </button>';
            temp += '            <button type="button" class="btn btn-primary windowConfirm">';
            temp += '               确认';
            temp += '            </button>';
            temp += '         </div>';
            temp += '      </div>';
            temp += '</div>';

            $(temp).insertAfter(that);
            curid = that.attr("swid");

            //初始化tab
            if (arrtype.length > 0) {
                $.fn.initSelectPerson.removeAllTab(curid);
                $.each(arrtype, function (i, item) {
                    if (i == 0) {
                        that.attr("tabcurtype", item);
                    }
                    switch (item) {
                        case "Person":
                            $.fn.initSelectPerson.addTab(curid, "Person", "人员", i, toolbuttons);
                            break;
                        case "Position":
                            $.fn.initSelectPerson.addTab(curid, "Position", "岗位", i, toolbuttons);
                            break;
                        case "Department":
                            $.fn.initSelectPerson.addTab(curid, "Department", "部门", i, toolbuttons);
                            break;
                        case "Custom":
                            $.fn.initSelectPerson.addTab(curid, "Custom", "自定义", i, toolbuttons);
                            break;
                        case "SystemRole":
                            $.fn.initSelectPerson.addTab(curid, "SystemRole", "系统角色", i, toolbuttons);
                            break;
                        case "All":
                            that.attr("tabcurtype", "Person");
                            $.fn.initSelectPerson.addTab(curid, "Person", "人员", 0, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Position", "岗位", 1, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Department", "部门", 2, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Custom", "自定义", 3, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "SystemRole", "系统角色", 4, toolbuttons);
                            break;
                        default:
                            that.attr("tabcurtype", "Person");
                            $.fn.initSelectPerson.addTab(curid, "Person", "人员", 0, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Position", "岗位", 1, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Department", "部门", 2, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "Custom", "自定义", 3, toolbuttons);
                            $.fn.initSelectPerson.addTab(curid, "SystemRole", "系统角色", 4, toolbuttons);
                            break;
                    }
                });
            }
            else {
                $.fn.initSelectPerson.removeAllTab(curid);
                that.attr("tabcurtype", "Person");
                $.fn.initSelectPerson.addTab(curid, "Person", "人员", 0, toolbuttons);
                $.fn.initSelectPerson.addTab(curid, "Position", "岗位", 1, toolbuttons);
                $.fn.initSelectPerson.addTab(curid, "Department", "部门", 2, toolbuttons);
                $.fn.initSelectPerson.addTab(curid, "Custom", "自定义", 3, toolbuttons);
                $.fn.initSelectPerson.addTab(curid, "SystemRole", "系统角色", 4, toolbuttons);
            }




            //为listbox添加按钮事件
            $("#" + id + " .btnRight").click(function () {
                var tabtype = that.attr("tabcurtype");
                var items = $.fn.initSelectPerson.getlistboxselectitem("left" + curid + "-" + tabtype);
                //移除左边
                $.each(items, function (i) {
                    $.fn.initSelectPerson.removetolistbox("left" + curid + "-" + tabtype, items[i]);
                });

                //如果非多选则同时移除右边还原到左边
                if (!opts.mutilselect) {
                    var rightitems = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + tabtype);
                    //移除右边
                    $.each(rightitems, function (i) {
                        $.fn.initSelectPerson.removetolistbox("right" + curid + "-" + tabtype, rightitems[i]);
                    });

                    //判断是否包含更多选项
                    var limore = $("#" + "left" + curid + "-" + tabtype).find("a.ui-corner-more");
                    var ismore = limore.length > 0 ? true : false;
                    var pageIndex = limore.attr("data-index");
                    //移除更多选项
                    $("#More" + tabtype).remove();

                    if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                        $.each(rightitems, function (i) {
                            $.fn.initSelectPerson.addPersontolistbox("left" + curid + "-" + tabtype, rightitems[i]);
                        });
                    }
                    else {
                        $.each(rightitems, function (i) {
                            $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + tabtype, rightitems[i]);
                        });
                    }
                    if (ismore) {
                        $.fn.initSelectPerson.addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, pageIndex);
                    }
                }

                //添加到右边
                if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                    $.each(items, function (i) {
                        $.fn.initSelectPerson.addPersontolistbox("right" + curid + "-" + tabtype, items[i]);
                    });
                }
                else {
                    $.each(items, function (i) {
                        $.fn.initSelectPerson.addtolistbox("right" + curid + "-" + tabtype, items[i]);
                    });
                }
            });
            $("#" + id + " .btnAllRight").click(function () {
                //获取所有
                $.fn.initSelectPerson.searchperson(that, curid, opts.isshownonreference, "right", "left", false, opts, 1);
            });

            $("#" + id + " .btnLeft").click(function () {
                var tabtype = that.attr("tabcurtype");
                var items = $.fn.initSelectPerson.getlistboxselectitem("right" + curid + "-" + tabtype);
                //移除右边
                $.each(items, function (i) {
                    $.fn.initSelectPerson.removetolistbox("right" + curid + "-" + tabtype, items[i]);
                });
                //添加到左边
                //判断是否包含更多选项
                var limore = $("#" + "left" + curid + "-" + tabtype).find("a.ui-corner-more");
                var ismore = limore.length > 0 ? true : false;
                var pageIndex = limore.attr("data-index");
                //移除更多选项
                $("#More" + tabtype).remove();

                if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                    $.each(items, function (i) {
                        $.fn.initSelectPerson.addPersontolistbox("left" + curid + "-" + tabtype, items[i]);
                    });
                }
                else {
                    $.each(items, function (i) {
                        $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + tabtype, items[i]);
                    });
                }
                if (ismore) {
                    $.fn.initSelectPerson.addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, pageIndex);
                }
            });

            $("#" + id + " .btnAllLeft").click(function () {
                $.fn.initSelectPerson.searchperson(that, curid, opts.isshownonreference, "left", "right", true, opts, 1);
            });

            $("#" + id + " .selectKey").keyup(function (event) {

                if (event.keyCode == 13) {
                    $.fn.initSelectPerson.searchperson(that, curid, opts.isshownonreference, "left", "left", true, opts, 1, true);
                }
            });

            $("#custompeopleTab" + curid + ">li").each(function (index) {
                $(this).click(function () {

                    that.attr("tabcurtype", $(this).attr("tabtype"));



                    var type = $.fn.initSelectPerson.GetCurrentType(that);
                    var ztreeopts = {
                        url: "/CustomPeople/GetOrgChartTree",
                        autoParam: ["ID", "NodeName"],
                    }
                    if (type == "Position") {
                        ztreeopts.url = "/CustomPeople/GetPosition";
                    }
                    else if (type == "SystemRole") {
                        ztreeopts.url = "/CustomPeople/GetRolesList?pane=Dashboard";
                        ztreeopts.autoParam = ["ID", "NodeName"];
                    }
                    else if (type == "Custom") {
                        ztreeopts.url = "/CustomPeople/GetCustomRoleByCommonControl";
                    }
                    $.fn.initSelectPerson.clearlistbox("left" + curid + "-" + type);
                    $.fn.initTreeView(curid, opts.isshownonreference, type, ztreeopts, opts);
                })
            });

            $('#' + id + ' div.itemcontainer').off('click dblclick').on('click dblclick', 'a', function (event) {
                if (event.type == 'dblclick' && !$(this).hasClass('ui-corner-more')) {
                    var tabtype = that.attr("tabcurtype");
                    var curultype = $(this).attr("ultype");
                    var curitem = { id: $(this).attr("id"), text: $(this).attr("dataText"), FirstName: $(this).attr("dataFirstName"), LastName: $(this).attr("dataLastName"), DisplayName: $(this).attr("dataDisplayName"), Company: $(this).attr("dataCompany"), Department: $(this).attr("dataDepartment"), Position: $(this).attr("dataPosition") };
                    if (curultype == "l") {
                        //如果非多选则同时移除右边还原到左边
                        if (!opts.mutilselect) {
                            var rightitems = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + tabtype);
                            //移除右边
                            $.each(rightitems, function (i) {
                                $.fn.initSelectPerson.removetolistbox("right" + curid + "-" + tabtype, rightitems[i]);
                            });

                            //判断是否包含更多选项
                            var limore = $("#" + "left" + curid + "-" + tabtype).find("a.ui-corner-more");
                            var ismore = limore.length > 0 ? true : false;
                            var pageIndex = limore.attr("data-index");
                            //移除更多选项
                            $("#More" + tabtype).remove();
                            if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                                $.each(rightitems, function (i) {
                                    $.fn.initSelectPerson.addPersontolistbox("left" + curid + "-" + tabtype, rightitems[i]);
                                });
                            }
                            else {
                                $.each(rightitems, function (i) {
                                    $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + tabtype, rightitems[i]);
                                });
                            }
                            if (ismore) {
                                $.fn.initSelectPerson.addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, pageIndex);
                            }
                        }
                        //添加到右边
                        if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                            $.fn.initSelectPerson.addPersontolistbox("right" + curid + "-" + tabtype, curitem);
                        }
                        else {
                            $.fn.initSelectPerson.addtolistbox("right" + curid + "-" + tabtype, curitem);
                        }
                    }
                    else {
                        //添加到左边
                        //判断是否包含更多选项
                        var limore = $("#" + "left" + curid + "-" + tabtype).find("a.ui-corner-more");
                        var ismore = limore.length > 0 ? true : false;
                        var pageIndex = limore.attr("data-index");
                        //移除更多选项
                        $("#More" + tabtype).remove();
                        if ($.fn.initSelectPerson.GetCurrentType(that) == "Person") {
                            $.fn.initSelectPerson.addPersontolistbox("left" + curid + "-" + tabtype, curitem);
                        }
                        else {
                            $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + tabtype, curitem);
                        }
                        if (ismore) {
                            $.fn.initSelectPerson.addMoretolistbox(tabtype, "left" + curid + "-" + tabtype, pageIndex);
                        }
                    }
                    $(this).remove();
                }
                else if (event.type == 'click') {
                    if ($(this).hasClass('ui-corner-more')) {
                        var pageindex = $(this).attr("data-index");
                        pageindex = parseInt(pageindex) + 1;
                        $.fn.initSelectPerson.searchperson(that, curid, opts.isshownonreference, "left", "right", true, opts, pageindex, true);
                    }
                    else {
                        if (!opts.mutilselect) {
                            //清除其他选择项
                            $('#' + id + ' div.itemcontainer a').removeClass('active');
                        }

                        var a = $(this);
                        if (a.hasClass('active')) {
                            a.removeClass('active');
                        } else {
                            a.addClass('active');
                        }
                        var tabtype = that.attr("tabcurtype");
                        var username = $(this).attr("dataText");
                        var firstname = $(this).attr("dataFirstName");
                        var lastname = $(this).attr("dataLastName");
                        var company = $(this).attr("dataCompany");
                        var department = $(this).attr("dataDepartment");
                        var position = $(this).attr("dataPosition");
                        if (username != undefined) {
                            $("#UserName" + curid + "-" + tabtype).val(username);
                        }
                        if (firstname != undefined) {
                            $("#FirstName" + curid + "-" + tabtype).val(firstname);
                        }
                        if (lastname != undefined) {
                            $("#LastName" + curid + "-" + tabtype).val(lastname);
                        }
                        if (company != undefined) {
                            $("#UserCompany" + curid + "-" + tabtype).val(company);
                        }
                        if (department != undefined) {
                            $("#UserDept" + curid + "-" + tabtype).val(department);
                        }
                        if (position != undefined) {
                            $("#UserPosition" + curid + "-" + tabtype).val(position);
                        }
                    }
                }
            });


            //确认
            $("#" + curid + " .windowConfirm").bind("click", function () {
                var json = { "Root": { "Users": { "Item": [] }, "Depts": { "Item": [] }, "Positions": { "Item": [] }, "CustomRoles": { "Item": [] }, "SystemRoles": { "Item": [] } } };
                var tabs = $("#custompeopleTab" + curid + " li");
                var count = 0;
                $.each(tabs, function (i) {
                    var curtabtype = $(this).attr("tabtype");
                    switch (curtabtype) {
                        case "Person":
                            var items = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + curtabtype);
                            $.each(items, function (i) {
                                json.Root.Users.Item.push({ Value: items[i].id, Name: items[i].DisplayName, UserName: items[i].text, Department: items[i].Department, Position: items[i].Position });
                            });
                            count += items.length;
                            break;
                        case "Position":
                            var items = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + curtabtype);
                            $.each(items, function (i) {
                                json.Root.Positions.Item.push({ Value: items[i].id, Name: items[i].text });
                            });
                            count += items.length;
                            break;
                        case "Department":
                            var items = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + curtabtype);
                            $.each(items, function (i) {
                                json.Root.Depts.Item.push({ Value: items[i].id, Name: items[i].text });
                            });
                            count += items.length;
                            break;
                        case "Custom":
                            var items = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + curtabtype);
                            $.each(items, function (i) {
                                json.Root.CustomRoles.Item.push({ Value: items[i].id, Name: items[i].text });
                            });
                            count += items.length;
                            break;
                        case "SystemRole":
                            var items = $.fn.initSelectPerson.getlistboxallitem("right" + curid + "-" + curtabtype);
                            $.each(items, function (i) {
                                json.Root.SystemRoles.Item.push({ Value: items[i].id, Name: items[i].text });
                            });
                            count += items.length;
                            break;
                    }
                });
                //if (count <= 0) {
                //    //ShowTip(jsResxbaseInitView.Donotchooseanyitem, "error");
                //    alert("没有选择任何项");
                //    return;
                //}
                that.data("currentJson", json);
                if (opts.callback != undefined) {
                    var e = { target: that };
                    opts.callback(json, e);
                }
                if (opts.onDetermine != undefined && opts.onDetermine != null) {
                    opts.onDetermine();
                }
                else {
                    $('#' + curid).modal('hide');
                }
            });
        }
        if (opts.attrbind) {

            var ztreeopts = {
                url: "/CustomPeople/GetOrgChartTree",
                autoParam: ["ID", "NodeName"],
            }
            var curtabtype = $.fn.initSelectPerson.GetCurrentType(that);
            if (curtabtype == "Position") {
                ztreeopts.url = "/CustomPeople/GetPosition";
            }
            else if (curtabtype == "SystemRole") {
                ztreeopts.url = "/CustomPeople/GetRolesList?pane=Dashboard";
            }
            else if (curtabtype == "Custom") {
                ztreeopts.url = "/CustomPeople/GetCustomRoleByCommonControl";
            }
            $.fn.initTreeView(curid, opts.isshownonreference, curtabtype, ztreeopts, opts);
            $.fn.initSelectPerson.ClearHistory(curtabtype, curid);
            $('#' + curid).modal('show');
            $.fn.initSelectJsonData(that, curid);
        }
        else {
            var thatTarget = opts.clickTarget || that
            $(thatTarget).unbind("click").bind("click", function () {
                var ztreeopts = {
                    url: "/CustomPeople/GetOrgChartTree",
                    autoParam: ["ID", "NodeName"],
                }
                var curtabtype = $.fn.initSelectPerson.GetCurrentType(that);
                if (curtabtype == "Position") {
                    ztreeopts.url = "/CustomPeople/GetPosition";
                }
                else if (curtabtype == "SystemRole") {
                    ztreeopts.url = "/CustomPeople/GetRolesList?pane=Dashboard";
                }
                else if (curtabtype == "Custom") {
                    ztreeopts.url = "/CustomPeople/GetCustomRoleByCommonControl";
                }
                $.fn.initTreeView(curid, opts.isshownonreference, curtabtype, ztreeopts, opts);
                $.fn.initSelectPerson.ClearHistory(curtabtype, curid);
                $('#' + curid).modal('show');
                $.fn.initSelectJsonData(that, curid);
            });
        }
    };
    //初始化控件end

    $.fn.initSelectPerson.listboxexistsitemforInitData = function (id, item) {
        var flag = false;
        var as = $("#" + id + " li a");
        $.each(as, function (i) {
            if (item.Value == as[i].id) {
                flag = true;
            }
        });
        return flag;
    }

    //listbox添加项目
    $.fn.initSelectPerson.addtolistboxforInitData = function (id, item) {
        var flag = $.fn.initSelectPerson.listboxexistsitemforInitData(id, item);
        if (!flag && item.Value.length > 0) {
            $("#" + id).append("<a id=\"" + item.Value + "\" class=\"list-group-item\" title=\"" + item.Name + "\" style=\"width:auto; height:32px;cursor:pointer;overflow: hidden;\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\"  dataText=\"" + item.Name + "\"  >" + item.Name + "</a>");
        }
    }

    //listbox添加Person项目
    $.fn.initSelectPerson.addPersontolistboxforInitData = function (id, item) {
        var flag = $.fn.initSelectPerson.listboxexistsitemforInitData(id, item);
        if (!flag && item.Value.length > 0) {
            $("#" + id).append("<a id=\"" + item.Value + "\" class=\"list-group-item\" title=\"" + item.Name + "\" style=\"width:auto; height:32px;cursor:pointer;overflow: hidden;\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\"  dataText=\"" + item.UserName + "\"  dataDisplayName=\"" + item.Name + "\"  >" + item.Name + "</a>");
        }
    }

    $.fn.initSelectJsonData = function (target, curid) {
        var currentJson=null;
        var koModel = $(target).attr("data-koModel");
        if (koModel != undefined) {
            var model = eval(koModel);
            var datatarget = $(target).attr("data-target");
            var $target = $("#" + datatarget);
            var databind = $target.attr("data-bind");
            var arr = databind.split(',');
            var field = arr[0].split(":")[1];
            var currentvalue = null;
               if (typeof model == "object") {
                currentvalue = model;
            }
            if (typeof model == "function") {
                currentvalue = model();
            }
            if (currentvalue!=null && typeof currentvalue == "object") {
                var json = { "Root": { "Users": { "Item": [] }, "Depts": { "Item": [] }, "Positions": { "Item": [] }, "CustomRoles": { "Item": [] }, "SystemRoles": { "Item": [] } } };
                var names = new Array();
                var types = new Array();
                var usernames = new Array();
                var ids = new Array();
                if (currentvalue.Name != undefined)
                    names = currentvalue.Name().split(',');
                if (currentvalue.Types != undefined)
                    types = currentvalue.Types().split(',');
                if (currentvalue.UserName != undefined)
                    usernames = currentvalue.UserName().split(',');
                if (currentvalue.Value != undefined)
                    ids = currentvalue.Value().split(',');
            
                $.each(ids, function (i, item) {
                    switch (types[i])
                    {
                        case "Users":
                            json.Root.Users.Item.push({ Value: item, Name: names[i], UserName: usernames[i] });
                            break;
                        case "Depts":
                            json.Root.Depts.Item.push({ Value: item, Name: names[i] });
                            break;
                        case "Positions":
                            json.Root.Positions.Item.push({ Value: item, Name: names[i] });
                            break;
                        case "CustomRoles":
                            json.Root.CustomRoles.Item.push({ Value: item, Name: names[i] });
                            break;
                        case "SystemRoles":
                            json.Root.SystemRoles.Item.push({ Value: item, Name: names[i] });
                            break;
                    }
                });
                currentJson = json;
            }
        }
        else {
            currentJson = $(target).data("currentJson");            
        }
        if (currentJson != null) {
            var tabs = $("#custompeopleTab" + curid + " li");
            $.each(tabs, function (i) {
                var curtabtype = $(this).attr("tabtype");
                switch (curtabtype) {
                    case "Person":
                        $.fn.initSelectPerson.clearlistbox("right" + curid + "-" + curtabtype);
                        var items = currentJson.Root.Users.Item;
                        $.each(items, function (i, item) {
                            $.fn.initSelectPerson.addPersontolistboxforInitData("right" + curid + "-" + curtabtype, item);
                        });
                        break;
                    case "Position":
                        $.fn.initSelectPerson.clearlistbox("right" + curid + "-" + curtabtype);
                        var items = currentJson.Root.Positions.Item;
                        $.each(items, function (i, item) {
                            $.fn.initSelectPerson.addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                        });
                        break;
                    case "Department":
                        $.fn.initSelectPerson.clearlistbox("right" + curid + "-" + curtabtype);
                        var items = currentJson.Root.Depts.Item;
                        $.each(items, function (i, item) {
                            $.fn.initSelectPerson.addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                        });
                        break;
                    case "Custom":
                        $.fn.initSelectPerson.clearlistbox("right" + curid + "-" + curtabtype);
                        var items = currentJson.Root.CustomRoles.Item;
                        $.each(items, function (i, item) {
                            $.fn.initSelectPerson.addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                        });
                        break;
                    case "SystemRole":
                        $.fn.initSelectPerson.clearlistbox("right" + curid + "-" + curtabtype);
                        var items = currentJson.Root.SystemRoles.Item;
                        $.each(items, function (i, item) {
                            $.fn.initSelectPerson.addtolistboxforInitData("right" + curid + "-" + curtabtype, item);
                        });
                        break;
                }
            });
        }
    };

    $.fn.initTreeView = function (curid, isshownonreference, type, ztreeopts, options) {
        function zTreeOnClick(event, treeId, treeNode) {
            //$.fn.initSelectPerson.clearlistbox("right" + curid + "-" + type);
            var key = $("#selectKey" + curid + "-" + type).val();
            $("#UserName" + curid + "-" + type).val("");
            $("#FirstName" + curid + "-" + type).val("");
            $("#LastName" + curid + "-" + type).val("");
            $("#UserCompany" + curid + "-" + type).val("");
            $("#UserDept" + curid + "-" + type).val("");
            $("#UserPosition" + curid + "-" + type).val("");
            var before = function () {
                $('#' + curid + " div.modal-body").mask('系统正在处理，请稍后');
            }
            var error = function () {
                $('#' + curid + " div.modal-body").unmask();
            }
            var successmsg = function (items) {
                if (items == null || items.length <= 0) {
                    $('#' + curid + " div.modal-body").mask('没有匹配的人员信息');
                }
                $('#' + curid + " div.modal-body").unmask();
            }
            var success = function (items) {
                successmsg(items);
                $.fn.initSelectPerson.clearlistbox("left" + curid + "-" + type);
                for (var j = 0; j < items.length; j++) {
                    $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + type, items[j]);
                }
                if (items.length >= options.pageSize) {
                    $.fn.initSelectPerson.addMoretolistbox(type, "left" + curid + "-" + type, 1);
                }
            };
            switch (type) {
                case "Person":
                    var url="/CustomPeople/GetSelectPersonUserByNode";
                    var data={ _t: new Date(), id: treeNode.ID.toString().substring(2), type: treeNode.Type,keyword:key, pageIndex: 1, pageSize: options.pageSize, isshownonreference: isshownonreference };
                    var success = function (items) {
                        successmsg(items);
                        $.fn.initSelectPerson.clearlistbox("left" + curid + "-" + type);
                        for (var j = 0; j < items.length; j++) {
                            $.fn.initSelectPerson.addPersontolistbox("left" + curid + "-" + type, items[j]);
                        }
                        if (items.length >= options.pageSize) {
                            $.fn.initSelectPerson.addMoretolistbox(type, "left" + curid + "-" + type, 1);
                        }
                    };
                    $.fn.RequestAjax(url, data, before, success, error);                    
                    break;
                case "Position":
                    var url = "/CustomPeople/GetSelectPersonPositionByNode";
                    var data = { _t: new Date(), id: treeNode.ID.toString(), type: treeNode.Type, keyword: key, pageIndex: 1, pageSize: options.pageSize, isshownonreference: isshownonreference };
                    $.fn.RequestAjax(url, data, before, success, error);                  
                    break;
                case "Department":                    
                    var url = "/CustomPeople/GetSelectPersonDeptByNode";
                    var data = { _t: new Date(), id: treeNode.ID.toString().substring(2), type: treeNode.Type, keyword: key, pageIndex: 1, pageSize: options.pageSize };
                    $.fn.RequestAjax(url, data, before, success, error);
                    break;
                case "Custom":
                    var url = "/CustomPeople/GetClassifyByCommonControl";
                    var data = { _t: new Date(), id: treeNode.ID, keyword: key, pageIndex: 1, pageSize: options.pageSize };
                    $.fn.RequestAjax(url, data, before, success, error);                    
                    break;
                case "SystemRole":
                    var roleid = treeNode.ID;
                    if (roleid.toString().substring(0, 1) == "1") {                        
                        var url = "/CustomPeople/GetRolesListByCategory";
                        var data = { _t: new Date(), ID: roleid, pane: "Dashboard"/*window.CurrentApp.pane*/, keyword: key, pageIndex: 1, pageSize: options.pageSize };
                        $.fn.RequestAjax(url, data, before, success, error);
                    }
                    else {
                        var roleitem = { id: treeNode.ID.toString().substring(2), text: treeNode.NodeName, FirstName: "", LastName: "" }
                        $.fn.initSelectPerson.clearlistbox("left" + curid + "-" + type);
                        $.fn.initSelectPerson.addtolistbox("left" + curid + "-" + type, roleitem);
                    }                    
                    break;
            }

        };
        var firstAsyncSuccessFlag = 0;
        function zTreeOnAsyncSuccess(event, treeId, msg) {            
            $.ajaxSettings.global = true;
            $("#load_mask_template").remove();
            if (firstAsyncSuccessFlag<2) {
                var zTree = $.fn.zTree.getZTreeObj("SelectPersonManageTreeView" + curid + "-" + type);
                try {
                    //调用默认展开第一个结点   
                    var selectedNode = zTree.getSelectedNodes();
                    var nodes = zTree.getNodes();
                    if (firstAsyncSuccessFlag == 0) {
                        $.each(nodes, function (i, item) {
                            zTree.expandNode(item, true);
                        });                        
                    }
                    else {
                        $.each(nodes, function (i, item) {
                            if (item.children != null && item.children.length>0) {
                                zTree.expandNode(item.children[0], true);
                            }
                        });                                             
                    }
                    //var childNodes = zTree.transformToArray(nodes[0]);
                    //zTree.expandNode(childNodes[0], true); 
                    //zTree.selectNode(childNodes[1]);
                    //var childNodes1 = zTree.transformToArray(childNodes[1]);
                    //zTree.checkNode(childNodes1[1], true, true);
                    firstAsyncSuccessFlag++;
                } catch (err) { }
            }
        }
        var firstBeforeAsyncFlag = 0;
        function zTreeBeforeAsync(treeId, treeNode) {
            $.ajaxSettings.global = false;
            if (firstBeforeAsyncFlag == 0) {
                $("#SelectPersonManageTreeView" + curid + "-" + type).append(template.load_mask_template);
                firstBeforeAsyncFlag = 1;
            }
        };

        function zTreeonAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown)
        {
            $.ajaxSettings.global = true;
        }

        var setting = {
            data: {
                key: {
                    name: "NodeName"
                },
                simpleData: {
                    enable: true,
                    idKey: "ID",
                    pIdKey: "ParentID"
                }
            }
            , async: {
                enable: true,
                url: ztreeopts.url,
                autoParam: ztreeopts.autoParam,
                otherParam: { "isshownonreference": isshownonreference, "tree": type }
                //dataFilter: filter
            }
            , callback: {
                onClick: zTreeOnClick,
                onAsyncSuccess: zTreeOnAsyncSuccess,
                beforeAsync: zTreeBeforeAsync,
                onAsyncError: zTreeonAsyncError
            }
        };
        $.fn.zTree.init($('#SelectPersonManageTreeView' + curid + '-' + type), setting);
    }



    function debug($obj) {
        if (window.console && window.console.log)
            window.console.log('hilight selection count: ' + $obj.size());
    };

    $.fn.initSelectPerson.addTab = function (curid, type, tabname, index, toolbuttons) {
        $("#custompeopleTab" + curid).append(template.tab_header_template.replace(/#tabactive#/g, (index == 0 ? "class='active'" : "")).replace(/#tabname#/g, tabname).replace(/#tabtype#/g, type).replace(/#id#/g, type).replace(/#swid#/g, curid));

        var temp=template.tab_content_template;
        switch (type)
        {
            case "Person":
                temp=temp.replace(/#selectkeytips#/g,"请输入名字或拼音首字母，按回车键查询");
                break;
            case "Position":
                temp = temp.replace(/#selectkeytips#/g, "请输入岗位名称或拼音首字母，按回车键查询");
                break;
            case "Department":
                temp=temp.replace(/#selectkeytips#/g,"请输入部门名称或拼音首字母，按回车键查询");
                break;
            case "Custom":
                temp=temp.replace(/#selectkeytips#/g,"请输入自定义角色名称或拼音首字母，按回车键查询");
                break;
            case "SystemRole":                
                temp=temp.replace(/#selectkeytips#/g,"请输入系统角色名称或拼音首字母，按回车键查询");
                break;
        }

        $("#custompeopleTabstrip" + curid).append(temp.replace(/#toolbuttons#/g, toolbuttons).replace(/#userinfotemplate#/g, (type == "Person" ? template.userinfotemplate : "")).replace(/#tabcontentactive#/g, (index == 0 ? "in active" : "")).replace(/#tabcontenttype#/g, type).replace(/#id#/g, type).replace(/#swid#/g, curid));
    };

    $.fn.initSelectPerson.removeAllTab = function (curid) {
        $("#custompeopleTab" + curid).html("");
        $("#custompeopleTabstrip" + curid).html("");
    };

    //清空listbox
    $.fn.initSelectPerson.clearlistbox = function (id) {
        $("#" + id).html("");
    }

    //listbox添加项目
    $.fn.initSelectPerson.addtolistbox = function (id, item) {
        var flag = $.fn.initSelectPerson.listboxexistsitem(id, item);
        if (!flag && item.id.length > 0) {
            $("#" + id).append("<a id=\"" + item.id + "\" class=\"list-group-item\" title=\"" + item.text + "\" style=\"width:auto; height:32px;cursor:pointer;overflow: hidden;\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\" dataFirstName=\"" + item.FirstName + "\" dataText=\"" + item.text + "\" dataLastName=\"" + item.LastName + "\" dataDisplayName=\"" + item.DisplayName + "\">" + item.text + "</a>");
        }
    }

    //listbox添加Person项目
    $.fn.initSelectPerson.addPersontolistbox = function (id, item) {
        var flag = $.fn.initSelectPerson.listboxexistsitem(id, item);
        if (!flag && item.id.length > 0) {
            $("#" + id).append("<a id=\"" + item.id + "\" class=\"list-group-item\" title=\"" + item.DisplayName + "\" style=\"width:auto; height:32px;cursor:pointer;overflow: hidden;\" tabindex=\"-1\" ultype=\"" + id.substring(0, 1) + "\" dataFirstName=\"" + item.FirstName + "\" dataText=\"" + item.text + "\" dataLastName=\"" + item.LastName + "\" dataDisplayName=\"" + item.DisplayName + "\" dataCompany=\"" + item.Company + "\" dataDepartment=\"" + item.Department + "\" dataPosition=\"" + item.Position + "\">" + item.DisplayName + "</a>");
        }
    }

    //listbox添加更多按钮
    $.fn.initSelectPerson.addMoretolistbox = function (type, id, pageIndex) {
        $("#More" + type).remove();
        $("#" + id).append("<a id=\"More" + type + "\" class=\"list-group-item ui-corner-more\" style=\"width:auto; height:32px;cursor:pointer;overflow: hidden;\" tabindex=\"-1\" data-index=\"" + pageIndex + "\">" + "更多" + "</a>");
    }

    //listbox移除项目
    $.fn.initSelectPerson.removetolistbox = function (id, item) {
        $("#" + id).find("#" + item.id).remove();
    }

    //获取listbox选中项
    $.fn.initSelectPerson.getlistboxselectitem = function (id) {
        var items = [];
        var as = $("#" + id + " a.active");

        $.each(as, function (i) {
            items.push({ id: as[i].id, text: $(as[i]).attr("dataText"), FirstName: $(as[i]).attr("dataFirstName"), LastName: $(as[i]).attr("dataLastName"), DisplayName: $(as[i]).attr("dataDisplayName"), Company: $(as[i]).attr("dataCompany"), Department: $(as[i]).attr("dataDepartment"), Position: $(as[i]).attr("dataPosition") });
        });
        return items;
    }

    //获取listbox当前全部项
    $.fn.initSelectPerson.getlistboxallitem = function (id) {
        var items = [];
        var as = $("#" + id + " a");
        $.each(as, function (i) {
            items.push({ id: as[i].id, text: $(as[i]).attr("dataText"), FirstName: $(as[i]).attr("dataFirstName"), LastName: $(as[i]).attr("dataLastName"), DisplayName: $(as[i]).attr("dataDisplayName"), Company: $(as[i]).attr("dataCompany"), Department: $(as[i]).attr("dataDepartment"), Position: $(as[i]).attr("dataPosition") });
        });
        return items;
    }

    $.fn.initSelectPerson.listboxexistsitem = function (id, item) {
        var flag = false;
        var as = $("#" + id + " a");
        $.each(as, function (i) {
            if (item.id == as[i].id) {
                flag = true;
            }
        });
        return flag;
    }


    $.fn.initSelectPerson.GetCurrentType = function (that) {
        return that.attr("tabcurtype");
    }

    $.fn.initSelectPerson.format = function (txt) {
        return '<strong>' + txt + '</strong>';
    };

    $.fn.initSelectPerson.ClearHistory = function (type, id) {
        //清空历史数据    
        if (id != undefined && id != null) {
            switch (type) {
                case "Person":
                case "Position":
                case "Department":
                case "Custom":
                case "SystemRole":
                    $("#left" + id + "-" + type).html("");
                    $("#right" + id + "-" + type).html("");
                    $("#selectKey" + id + "-" + type).val("");
                    break;
                default:
                    $("#left" + id + "-Person").html("");
                    $("#right" + id + "-Person").html("");
                    $("#left" + id + "-Position").html("");
                    $("#right" + id + "-Position").html("");
                    $("#left" + id + "-Department").html("");
                    $("#right" + id + "-Department").html("");
                    $("#left" + id + "-Custom").html("");
                    $("#right" + id + "-Custom").html("");
                    $("#left" + id + "-SystemRole").html("");
                    $("#right" + id + "-SystemRole").html("");

                    $("#selectKey" + id + "-Person").val("");
                    $("#selectKey" + id + "-Position").val("");
                    $("#selectKey" + id + "-Department").val("");
                    $("#selectKey" + id + "-Custom").val("");
                    $("#selectKey" + id + "-SystemRole").val("");
                    break;
            }
        }
    }

    $.fn.initSelectPerson.getJsonToList = function (curtype, curid, allowpage, addside, clearside, url, data, opts, pageIndex, isMore) {
        var before=function(){
            $('#' + curid + " div.modal-body").mask('系统正在处理，请稍后');
        }
        var success=function(items){
            if (items == null || items.length <= 0) {                
                 $('#' + curid + " div.modal-body").mask('没有更多的人员信息');                                
            }
            $('#' + curid + " div.modal-body").unmask();
            if (!isMore || clearside=="left") {
                $.fn.initSelectPerson.clearlistbox(clearside + curid + "-" + curtype);
            }
            if (curtype == "Person") {
                $.each(items, function (i, item) {
                    $.fn.initSelectPerson.addPersontolistbox(addside + curid + "-" + curtype, item);
                });
            }
            else {
                $.each(items, function (i, item) {
                    $.fn.initSelectPerson.addtolistbox(addside + curid + "-" + curtype, item);
                });
            }
            if (allowpage && items.length >= opts.pageSize) {
                $.fn.initSelectPerson.addMoretolistbox(curtype, addside + curid + "-" + curtype, pageIndex);
            }
            else {
                $("#More" + curtype).remove();
            }
            if (isMore && items.length == 0) {
                $("#More" + curtype).remove();
            }
        }
        var error=function(){
            $('#' + curid + " div.modal-body").unmask();
        }        
        $.fn.RequestAjax(url, data, before, success, error);                
    }

    $.fn.RequestAjax = function (url, data, before, success, error) {
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            dataType: "json",
            global: false,
            beforeSend:function()
            {
                if (before != null) {
                    before();
                }
            }
            , success: function (items) {
                if (success != null) {
                    success(items);
                }
            }
            , error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (error != null) {
                    error();
                }
                console.log(textStatus);
            }
        });
    }

    //搜索符合条件的人，岗位，部门
    $.fn.initSelectPerson.searchperson = function (that, curid, isShowNonReference, addside, clearside, allowpage, opts, pageIndex, isMore) {
        var tabtype = that.attr("tabcurtype");
        var key = $("#selectKey" + curid + "-" + tabtype).val();
        var treeObj = $.fn.zTree.getZTreeObj("SelectPersonManageTreeView" + curid + "-" + tabtype);
        var nodes = treeObj.getSelectedNodes();
        var select = nodes[0];
        var itemtype = "";
        if ((key.length == 0 || key.length >= 1)) {  //&& select != null && select != undefined && nodes.length == 1
            var selectid = "";
            if (select != null && select != undefined && nodes.length > 0) {
                selectid = select.ID.toString().substring(2);
                itemtype = select.Type;
            }
            var jsonparams = { _t: new Date(), id: selectid, type: itemtype, pageIndex: pageIndex, pageSize: opts.pageSize, isshownonreference: isShowNonReference, keyword: key, allowpage: allowpage };
            switch (tabtype) {
                case "Person":
                    $.fn.initSelectPerson.getJsonToList(tabtype, curid, allowpage, addside, clearside, "/CustomPeople/GetSelectPersonUserByNode", jsonparams, opts, pageIndex, isMore);
                    break;
                case "Position":
                    jsonparams.id = "00000000-0000-0000-0000-000000000000";
                    if (select != null && select != undefined && nodes.length > 0) {
                        jsonparams.id = select.ID;
                    }
                    $.fn.initSelectPerson.getJsonToList(tabtype, curid, allowpage, addside, clearside, "/CustomPeople/GetSelectPersonPositionByNode", jsonparams, opts, pageIndex, isMore);
                    break;
                case "Department":
                    $.fn.initSelectPerson.getJsonToList(tabtype, curid, allowpage, addside, clearside, "/CustomPeople/GetSelectPersonDeptByNode", jsonparams, opts, pageIndex, isMore);
                    break;
                case "Custom":
                    jsonparams.id = "00000000-0000-0000-0000-000000000000";
                    if (select != null && select != undefined && nodes.length > 0) {
                        jsonparams.id = select.ID;
                    }
                    $.fn.initSelectPerson.getJsonToList(tabtype, curid, allowpage, addside, clearside, "/CustomPeople/GetClassifyByCommonControl", jsonparams, opts, pageIndex, isMore);
                    break;
                case "SystemRole":
                    var roleid = "";
                    if (select != null && select != undefined && nodes.length > 0) {
                        roleid = select.ID;
                    }
                    if (roleid.toString().substring(0, 1) == "1" || roleid == "") {
                        $.fn.initSelectPerson.getJsonToList(tabtype, curid, allowpage, addside, clearside, "/CustomPeople/GetRolesListByCategory", { _t: new Date(), ID: roleid, pane: "Dashboard", pageIndex: pageIndex, pageSize: opts.pageSize, keyword: key }, pageIndex, isMore);
                    }
                    else {
                        $.fn.initSelectPerson.clearlistbox(clearside + curid + "-" + tabtype);
                        if ((key.length > 0 && select.NodeName.toLowerCase().indexOf(key.toLowerCase()) >= 0) || key.length == 0) {
                            var roleitem = { id: select.ID.toString().substring(2), text: select.NodeName, FirstName: "", LastName: "" }
                            $.fn.initSelectPerson.addtolistbox(addside + curid + "-" + tabtype, roleitem);
                        }
                    }
                    break;
            }
        }
    }


    function initSelectPersonControl() {
        $(document).delegate("*[data-control='userpick']", "click", function () {
            var that = $(this);
            //属性绑定事件
            var disable = that.attr("disabled");
            if (disable) {
                return false;
            };
            var datatarget = that.attr("data-target");
            var datacontroltype = that.attr("data-controltype");
            var datacallback = that.attr("data-callback");
            var options = that.attr("data-options");
            var koModel = that.attr("data-koModel");
            if (options != undefined && options.length > 0) { options = eval('(' + options + ')'); }
            var callback = null;
            var mutilselect = true;
            if (datacontroltype == undefined) {
                datacontroltype = "Person";
            }
            if (datatarget != undefined) {
                callback = function (json, e) {
                    var result = new Array();
                    var users = json.Root.Users.Item;
                    var depts = json.Root.Depts.Item;
                    var positions = json.Root.Positions.Item;
                    var systemRoles = json.Root.SystemRoles.Item;
                    var customRoles = json.Root.CustomRoles.Item;
                    var $target = $("#" + datatarget);
                    if (users.length > 0) {
                        //$target.data("data-users", users);
                        $.each(users, function (i, item) {
                            result.push({ Value: item.Value, Name: item.Name, UserName: item.UserName,Type:"Users" });
                        });
                    }
                    if (depts.length > 0) {
                        //$target.data("data-depts", depts);
                        $.each(depts, function (i, item) {
                            result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "Depts" });
                        });
                    }
                    if (positions.length > 0) {
                        //$target.data("data-positions", positions);
                        $.each(positions, function (i, item) {
                            result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "Positions" });
                        });
                    }
                    if (systemRoles.length > 0) {
                        //$target.data("data-systemRoles", systemRoles);
                        $.each(systemRoles, function (i, item) {
                            result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "SystemRoles" });
                        });
                    }
                    if (customRoles.length > 0) {
                        //$target.data("data-customRoles", customRoles);
                        $.each(customRoles, function (i, item) {
                            result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "CustomRoles" });
                        });
                    }
                    var values = new Array();
                    var names = new Array();
                    var usernames = new Array();
                    var types = new Array();
                    $.each(result, function (i, item) {
                        values.push(item.Value);
                        names.push(item.Name);
                        if (item.UserName.length > 0) {
                            usernames.push(item.UserName);
                        }
                        types.push(item.Type);
                    });
                    $target.attr("data-values", values.join(','));
                    $target.attr("data-names", names.join(','));
                    $target.attr("data-usernames", usernames.join(','));
                    switch ($target[0].tagName) {
                        case "INPUT":
                        case "TEXTAREA":
                            $("#" + datatarget).val(names.join(','));
                            break;
                        case "DIV":
                        case "LABEL":
                        case "DT":
                        case "DL":
                        case "DD":
                        case "UL":
                        case "LI":
                        case "A":
                        case "P":
                        case "TH":
                        case "TT":
                        case "TR":
                        case "TD":
                        case "SPAN":
                        case "OL":
                        case "H":
                        case "IMG":
                            $("#" + datatarget).html(names.join(','));
                            break;
                    }
                    if (koModel) {
                        try {
                            var model = eval(koModel);
                            var databind = $target.attr("data-bind");
                            var arr = databind.split(',');
                            var field = arr[0].split(":")[1];

                            if (model.Name != undefined)
                                model.Name(names.join(','))
                            if (model.Types != undefined)
                                model.Types(types.join(','));
                            if (model.UserName != undefined)
                                model.UserName(usernames.join(','));
                            if (model.Value != undefined)
                                model.Value(values.join(','));
                            //model[field.replace(/(^\s*)|(\s*$)/g, "")](values.join(','));
                        }
                        catch (ex) {
                            console.log("update komodel failed");
                        }
                    }
                    if (datacallback != undefined) {
                        try {
                            var obj = eval(datacallback);
                            if (typeof obj == "function") {
                                obj(json, e);
                            }
                        }
                        catch (ex) {
                            console.log("未定义回调函数");
                        }
                    }
                }
            };
            if (options != undefined && options.mutilselect != undefined) {
                mutilselect = options.mutilselect;
            }
            $(this).initSelectPerson({
                type: datacontroltype,
                callback: callback,
                mutilselect: mutilselect,
                attrbind: true
            });
        });
    }
    initSelectPersonControl();

})(jQuery);