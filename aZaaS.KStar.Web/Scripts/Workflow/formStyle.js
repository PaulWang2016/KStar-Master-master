$().ready(function () {
   var ResetFormStyle=function() {
        var targetObject;
        if ($("div[role='_kstarform_toolbar']").length > 0) {
            targetObject = $("div[role='_kstarform_toolbar']")[0];
            targetObject = $(targetObject).find('.text-center');
            $(".container").prepend(targetObject);
            var placeholderDIV = "<div class=\"placeholderDIV\"></div>";
            placeholderDIV = $(placeholderDIV).height((targetObject.height()+20));
            $(".container").prepend(targetObject);
            $(".container").prepend(placeholderDIV); 
            var from = targetObject;
            from.css('position', 'fixed');
            from.css('top', '0px');
            from.css('left', '0px');
            from.css('width', '100%');
            from.css('padding', '10px');
            from.css('z-index', '1041');
            from.css('background-color', 'rgb(144, 140, 140)')
            from.prepend("<div style='float: left;padding: 10px;margin-right: 5%;'>操作选项</div>");
            from.css("text-align", "left");
            from.find(':button').css('padding', '8px');
            //设置申请信息默认收缩
            $('#_kstarform_header_panel')[0].className = "panel-collapse collapse";
            //设置操作选项必填不必填
            var actions = $("#rbl_ApprovalAction").find("input:radio");
            $.each(actions, function (index, item) {
                item.checked = false
            });
            //  disabled  FormSubject
            $('#txt_FormSubject').attr('disabled', 'disabled')
          
            var toolbar_form = $('#_kstarform_toolbar_form').find('input,textarea');

            if (toolbar_form.length ==0) {
                $('#_kstarform_toolbar_component').hide();
            }
            $('#_kstarform_toolbar_component .panel-title').find('a').text('审批信息')
        }
   }
   ResetFormStyle();
});