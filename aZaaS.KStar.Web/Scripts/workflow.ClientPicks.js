var InitSelectCustomerWindow = function (target, options) {

    var that = $(target);
    var curid = that.attr("swid");


    var defaults = {
        resizable: false,
        modal: true,
        callback: undefined,
        targetid: undefined
    };
    $.extend(defaults, options);

    if (!curid) {
        var id = "sw_" + Math.random().toString().substring(2);
        that.attr("swid", id);
        curid = that.attr("swid");
        var temp = '';
        temp += '<div class="modal fade" id="' + id + '" tabindex="-1" role="dialog"';
        temp += '   aria-labelledby="myModalLabel" aria-hidden="true">';
        temp += '   <div class="modal-dialog" style="min-width:750px">';
        temp += '      <div class="modal-content">';
        temp += '         <div class="modal-header" style="padding:6px;">';
        temp += '            <button type="button" class="close"';
        temp += '               data-dismiss="modal" aria-hidden="true">';
        temp += '                  &times;';
        temp += '            </button>';
        temp += '            <h4 class="modal-title" id="myModalLabel">';
        temp += '               选人控件';
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
        var curtomer = initGrid();
        var str = ""
        //确认
        $("#" + curid + " .windowConfirm").bind("click", function () {
            var grid = $("#custompeopleTab" + id).data("kendogrid");

            var selectitem = grid.getSelections();
            $("#" + targetid).val(selectitem.name);
            $('#' + curid).modal('hide');


        });
        $(that).click(function () {
            initGrid();
            $('#' + curid).modal('show');
        });
    }
    var initGrid = function () {
         
            return new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: function (options) {
                            return kendo.format("/YFDeliveryGoods/DeliveryGoodsController/GetCustomerInfo?customerName=" );
                        },
                        dataType: "json"
                    }
                }
            });
        
    }
}