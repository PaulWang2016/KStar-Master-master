
$(function () {
    //-自定义验证方法
    //$.validator.addMethod("ksf-required", $.validator.methods.required, "KStarForm自定义必填字段验证！");
    //-自定义Class验证规则
    //$.validator.addClassRules("ksf-required", { required: true });


    // override jquery validate plugin defaults
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'alert-danger',

        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
        },
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }     
        },

        onkeyup: function (element) {
            $(element).valid();
        }
    });

    /** Set form default trigger **/
    //$('form').each(function () {
    //    $(this).validate({
    //        onkeyup: function (element) {
    //            $(element).valid();
    //        }
    //    });
    //});
});