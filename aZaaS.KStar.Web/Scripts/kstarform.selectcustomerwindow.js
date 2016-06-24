//添加js string 格式化方法扩展
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g, function (s, i) {
        return args[i];
    });

    var InitSelectCustomerWindow = {
        init: function () {
            var html = "<div class=\"modal fade\" id=\"modalProcessingVendor\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\">";
            html += "<div class=\"modal-dialog\">    <div class=\"modal-content\">  <div class=\"modal-header\" style=\" padding:0px;\">";
            html += "<button type=\"button\" class=\"close\" data-dismiss=\"modal\"><span aria-hidden=\"true\">&times;</span><span class=\"sr-only\">Close</span></button>";
            //窗体 title
            html += "<p class=\"bg-primary\" style=\"padding: 10px;\"> <span class=\"caret\"></span>&nbsp; <b>{0}</b> </p></div>";
            html += "<div class=\"modal-body\">  <label class=\"control-label col-sm-2\" style=\"width:70px;\">查询:</label>";
            html += "<div class=\"col-sm-10 has-feedback\">"
            //查询input
            html += "<input class=\"form-control\" type=\"text\" name=\"SourceDepartment\" id=\"SourceDepartment\" data-rule-required=\"true\" data-msg-required=\"任务来源部门为必填项！\"   placeholder=\"请填写任务来源部门\">";
            html += "<span class=\"glyphicon glyphicon-search form-control-feedback\"></span></div>";
            html += "<table  class=\"table table-hover kstar-control\">";
            html += "<thead><tr>";
            html += "";//表头
            html += "</tr></thead>";
            html += "<tbody id=\"modalProcessingVendor_body\"></tbody></table>";
            html += "<div id=\"modal_ProcessingVendor_Paging\" style=\"width:90%; margin:auto;\"></div></div>"
            html += "<div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>"
            html += "<button type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\" onclick=\"ChoiceEvent()\">Confirm</button>";
            html += "</div>";
        }


    };

    function a() {

        InitSelectCustomerWindow.init
    }
}