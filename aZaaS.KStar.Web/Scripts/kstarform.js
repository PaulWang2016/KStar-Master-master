 
/** Common Utilities **/

String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}

function isFunc(obj) {
    return !!(obj && obj.constructor && obj.call && obj.apply);
} 

function $postJSON(url, json, successCallback, failCallback) {
    var requstUrl = url + window.location.search;
    $.ajax({
        url: requstUrl,
        type: 'POST',
        data: { jsonData: json },
        dataType: 'json'
        //contentType: 'application/json; charset=UTF-8'
    }).done(successCallback).fail(failCallback);
}

function $post(url, data, successCallback, failCallback) {
    var requstUrl = url + window.location.search;
    $.ajax({
        url: requstUrl,
        type: 'POST',
        data: data,
        dataType: 'json',
        traditional: true
        //contentType: 'application/json; charset=UTF-8'
    }).done(successCallback).fail(failCallback);
}

function $getJSON(url, data, callback) {
    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        data: data,
        global: false,
        success: callback
    });
}

function jsonDate_deserializer(key, value) {

    if (typeof value === 'string') {
        var regexp;
        regexp = /^\d\d\d\d-\d\d-\d\dT\d\d:\d\d:(\d\d|\d\d.\d\d\d)Z$/.exec(value);
        if (regexp) {
            return new Date(value);
        }
    }
    return value;
}

function getUrlVars (name) {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars[name];
}

/** Common Alerts **/

KStarMessage = {
    formInvalidDefaultMsg: '表单校验未通过，请检查之后再重试！',
    formHandleSuccessMasg: '操作成功! <br/> 表单编号为: ',
    formInvalidOptionMasg: '请选择您的审批意见! '
};

function $alertWarning(type, message,callback) {
    var msgWindow = '<div class=\"modal fade\" data-backdrop=\"static\">'
+ '  <div class=\"modal-dialog\">'
+ '    <div class=\"modal-content\">'
+ '      <div class=\"modal-header\">'
+ '        <button type=\"button\" class=\"close\" data-dismiss=\"modal\"><span aria-hidden=\"true\">&times;</span><span class=\"sr-only\">Close</span></button>'
+ '        <h4 class=\"modal-title\">系统提示</h4>'
+ '      </div>'
+ '      <div class=\"modal-body\">'
+ '        <p>' + message + '</p>'
+ '      </div>'
+ '      <div class=\"modal-footer\">'
+ '        <button type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\">确定</button>'
+ '      </div>'
+ '    </div>'
+ '  </div>'
+ '</div>';

    $(msgWindow).modal('show').on('hidden.bs.modal', function (e) {
        if (type == 1) {//1:Exec Success
            if (callback != undefined && typeof callback == "function") {
                try {
                    callback();
                }
                catch (e) { }
            }
            KStarForm.autoClose();
        }
        else {
            if (callback != undefined && typeof callback == "function") {
                try {
                    callback();
                }
                catch (e) { }
            }
        }
    });
}

/** KStar & Utilities **/

KStar = {
    utils: {
        mask: function () {
            var maskModal;
            var modalHtml = '<div class="modal fade" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel" aria-hidden="true">'
                            + '<div class="modal-dialog modal-sm">'
                            + '<div class="modal-content">'
                            + '<div class="modal-header">'
                            + '<h4 class="modal-title" id="myModalLabel">系统提示</h4>'
                            + '</div>'
                            + '<div class="modal-body">'
                            + '<img src="/CSS/kendoui/Default/loading_2x.gif" />{0}'
                            + '</div>'
                            + '</div>'
                            + '</div>'
                            + '</div>';

            return {
                show: function (message) {
                    maskModal = $(String.format(modalHtml, message));
                    maskModal.modal('show');
                },
                hide: function () {
                    if (maskModal) {
                        maskModal.modal('hide');
                    }
                }
            };
        }(),
        other_tools_star_here: {}//TODO: (by Bingyi)
    },
    ajaxGlobal: function () {
        var self = this;
        $(document).ajaxStart(function () {
            self.utils.mask.show('系统正在处理，请您稍后...');
        });
        $(document).ajaxComplete(function () {
            self.utils.mask.hide();
        });
    },
    uploader: function (sender, successCallback, failCallback, changeCallback) {
        var uploader = sender;
        var url = $_attachmentUploadUrl + window.location.search;

        uploader.fileupload({
            url: url,
            iframe: true,
            dataType: 'json',
            autoUpload: true,
            acceptFileTypes: $_attachemntAllowedTypes,///(\.|\/)(gif|jpe?g|png)$/i,
            maxNumberOfFiles: 10,
            fileInput: uploader.find("input:file"),
            maxFileSize: $_attachemntMaxFileSize = 31457280,
            previewMaxWidth: 200,
            previewMaxHeight: 200,
            previewCrop: true,
            singleFileUploads: false,
            done: successCallback,
            fail:failCallback    
        }).on('fileuploadadd', function (e, data) {
            $.each(data.files, function (index,file) {
                if ($_attachemntMaxFileSize > 0 && $_attachemntMaxFileSize < file.size) {
                    var ops = {
                        msg: "上传文件不能超过30M。",
                        title: "系统提示",
                        btnok: "确定",
                        btncl: "取消"
                    };
                    KStar.Modaldialog.alert(ops);
                    return;
                }
            })
            var msg = e;
        });
            ////Fix firefox input disable attr issue
            //uploader.find("input:file").removeAttr('disabled');

            //KStarForm.$formAttachmentButton.dmUploader({
            //    url: $_attachmentUploadUrl,
            //    dataType: 'json',
            //    extFilter: $_attachemntExtFilter,
            //    allowedTypes: $_attachemntAllowedTypes,
            //    maxFileSize: $_attachemntMaxFileSize,
            //    onUploadSuccess: function (id, data) {
            //        if (data && data.length > 0) {
            //            for (var i = 0; i < data.length; i++) {
            //                var item = data[i];
            //                var itemViewModel = KStarForm.toKoModel(item);
            //                KStarForm.koAttachmentModel.push(itemViewModel);
            //            }
            //        }
            //    },
            //    onFileSizeError: function (file) {
            //        alert('File size of ' + file.name + ' exceeds the limit');
            //    },
            //    onFileTypeError: function (file) {
            //        alert('File type of ' + file.name + ' is not allowed: ' + file.type);
            //    },
            //    onFileExtError: function (file) {
            //        alert('File extension of ' + file.name + ' is not allowed');
            //    },
            //    onFallbackMode: function (message) {
            //        alert("Upload plugin can't be initialized: " + message);
            //    },
            //    onUploadError: function (id, message) {
            //        alert('Error trying to upload #' + id + ': ' + message);
            //    }
            //});
    },
    Modaldialog: function () {
        var msgWindow = '<div id="kstarform-dialog" class="modal">'
+'            <div class="modal-dialog modal-sm">'
+'                <div class="modal-content">'
+'                    <div class="modal-header">'
+'                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>'
+'                        <h5 class="modal-title"><i class="fa fa-exclamation-circle"></i> [Title]</h5>'
+'                    </div>'
+'                    <div class="modal-body small">'
+'                        <p>[Message]</p>'
+'                    </div>'
+'                    <div class="modal-footer" >'
+'                        <button type="button" class="btn btn-primary ok" data-dismiss="modal">[BtnOk]</button>'
+'                        <button type="button" class="btn btn-default cancel" data-dismiss="modal">[BtnCancel]</button>'
+'                    </div>'
+'                </div>'
+'            </div>'
+'        </div>';

        var reg = new RegExp("\\[([^\\[\\]]*?)\\]", "igm");
        var alr = $(msgWindow);
        var ahtml = alr.html();

        var _alert = function (options) {
            alr.html(ahtml);    // 复原
            //alr.find(".ok").removeClass("btn-success").addClass("btn-primary");
            alr.find(".cancel").hide();
            _dialog(options);

            return {
                on: function (callback) {
                    if (callback && callback instanceof Function) {
                        alr.find(".ok").click(function () { callback(true) });
                    }
                }
            };
        };

        var _confirm = function (options) {
            alr.html(ahtml); // 复原
            //alr.find(".ok").removeClass("btn-primary").addClass("btn-success");
            alr.find(".cancel").show();
            _dialog(options);

            return {
                on: function (callback) {
                    if (callback && callback instanceof Function) {
                        alr.find(".ok").click(function () { callback(true) });
                        alr.find(".cancel").click(function () { callback(false) });
                    }
                }
            };
        };

        var _dialog = function (options) {
            var ops = {
                msg: "提示内容",
                title: "系统提示",
                btnok: "确定",
                btncl: "取消"
            };

            $.extend(ops, options);

            var html = alr.html().replace(reg, function (node, key) {
                return {
                    Title: ops.title,
                    Message: ops.msg,
                    BtnOk: ops.btnok,
                    BtnCancel: ops.btncl
                }[key];
            });
            
            alr.html(html);
            alr.modal({
                width: 500,
                backdrop: "static"
            });
        }

        return {
            alert: _alert,
            confirm: _confirm
        }

    }(),
    StringHelper: function () {
       /**
       * 字符串长度-中文和全角符号为2，英文、数字和半角为1
       * @param str
       * @return {Number}
       */
        var _getLength = function (str) {
            return Math.ceil(str.replace(/^\s+|\s+$/ig, '').replace(/[^\x00-\xff]/ig, 'xx').length);
        };
        /**
        * 按字数截取字符串
        * @param str
        * @param len
        * @return {*}
        */
        var _subStr = function (str, len) {
            if (!str) {
                return '';
            }
            len = len > 0 ? len * 2 : 280;
            var count = 0, //计数：中文2字节，英文1字节
                temp = '';  //临时字符串
            for (var i = 0; i < str.length; i++) {
                if (str.charCodeAt(i) > 255) {
                    count += 2;
                }
                else {
                    count++;
                }
                //如果增加计数后长度大于限定长度，就直接返回临时字符串
                if (count > len) {
                    return temp;
                }
                //将当前内容加到临时字符串
                temp += str.charAt(i);
            }
            return str;
        };
        var _checkStrLength = function (str, minL, maxL) {
            var len = getLength($.trim(str));
            var data = {
                'checkL': (len >= minL && len <= maxL),
                'restL': maxL - len,
                'restStr': subStr(str, maxL)
            };
            return data;
        };

        return {
            getLength: _getLength,
            subStr: _subStr,
            checkStrLength: _checkStrLength
        }
    }()
}


/** KStarForm  Object **/

KStarForm = {

    /* Containers */
    $formHeaderContainer: $('div[role=_kstarform_header]'),
    $formContentContainer: $('div[role=_kstarform_content]'),
    $formAttachmentContainer: $('div[role=_kstarform_attachment]'),
    $formProcessLogContainer: $('div[role=_kstarform_processlog]'),
    $formToolbarContainer: $('div[role=_kstarform_toolbar]'),
    $formFooterContainer: $('div[role=_kstarform_footer]'),

    /* Buttons */

    $formSubmitButton: $('#_kstarform_btn_submit'),
    $formCloseButton: $('#_kstarform_btn_close'),
    $formDraftButton: $('#_kstarform_btn_draft'),
    $formSaveButton: $('#_kstarform_btn_save'),
    $formCountersignButton: $('#_kstarform_btn_countersign'),
    $formRedirectButton: $('#_kstarform_btn_redirect'),
    $formDelegateButton: $('#_kstarform_btn_delegate'),
    $formBackToButton: $('#_kstarform_btn_backTo'),
    $formCarbonCopyButton: $('#_kstarform_btn_carbonCopy'),
    $formReviewButton: $('#_kstarform_btn_Review'),
    $formDeleteButton: $('#_kstarform_btn_delete'),
    $formViewFlowButton: $('#_kstarform_btn_ViewFlow'),
    $formAttachmentButton: $('#_kstarform_attachment_zone'),
    $formAttachmentSubmitButton: $('#_kstarform_attachment_Submit'),
    $formChoosepersonButton: $('#_kstarform_btn_chooseperson'),
    $formUndoButton: $('#_kstarform_btn_Undo'), 
    $formSeerButton: $('#_kstarform_btn_seer'),
    

    /* KoModels */

    koHeaderModel: {},
    koContentModel: {},
    koAttachmentModel: [],
    koProcessLogModel: [],
    koToolbarModel: {},

    userPositionsModel: [],

    /* Infrastructure */

    toKoModel: function (jsModel, options) {

        return ko.viewmodel.fromModel(jsModel, options);
    },
    toJsModel: function (koModel) {

        return ko.viewmodel.toModel(koModel);
    },
    getJSON: function (koModel) {

        var jsModel = ko.toJS(koModel);
        return JSON.stringify(jsModel);
    },
    applyBindings: function (koModel, JsNode) {

        ko.applyBindings(koModel, JsNode);
    },

    /* Validation */

    isValid: function () {
        is_forms_valid = false;
        $(".alert").alert('close');

        $('form').each(function () {
            is_forms_valid = $(this).valid();
            if (!is_forms_valid) { return false; }
        });

        return is_forms_valid;
    },

    isNeedValid: true,

    /*postMessage 刷新父窗体 请求参数”refresh“=父窗体的域名
      * 1、refreshPendingTask  待办  { refresh: true, action: 'refreshPendingTask' }; 
      * 2、refreshWaitRead 待阅  { refresh: true, action: 'refreshWaitRead' }; 
     */
    /* Actions */
    refreshPendingTask: function () {
        if (window.opener != null) {
            var ssoRefresh = getUrlVars("refresh");
            if (ssoRefresh != undefined) {
                var refresh = { refresh: true, action: 'refreshPendingTask' };
                window.opener.postMessage(JSON.stringify(refresh), ssoRefresh);
            } else {
                if (window.opener.GetPendingTask != undefined && typeof window.opener.GetPendingTask == 'function') {
                    window.opener.GetPendingTask();
                }
                window.opener.RefreshPendingTaskMenuItem();
                window.opener.RefreshDraftTaskMenuItem();
            }
        }
    },
    refreshDraftTask: function () {
        if (window.opener != null) {
            if (window.opener.refreshDraft != undefined && typeof window.opener.refreshDraft == 'function') {
                window.opener.refreshDraft();
            }
            window.opener.RefreshDraftTaskMenuItem();
        }
    },
    refreshWaitRead: function () {
        if (window.opener != null) {
            var ssoRefresh = getUrlVars("refresh");
            if (ssoRefresh != undefined) {
                var refresh = { refresh: true, action: 'refreshWaitRead' };
                window.opener.postMessage(JSON.stringify(refresh), ssoRefresh);
            } else {
                window.opener.refreshWaitRead();
                window.opener.RefreshWaitReadTaskMenuItem();
            }
        }
    },
    formDraft: function () {
        if (KStarForm.OnDrafting && typeof (KStarForm.OnDrafting) == "function") {
            if (KStarForm.OnDrafting(this)) {
                KStarForm.draft(this, KStarForm.OnDrafted, KStarForm.refreshDraftTask);
            }
        }
        else {
            KStarForm.draft(this, KStarForm.OnDrafted, KStarForm.refreshDraftTask);
        }
    },
    formSave: function () {
        if (KStarForm.OnSaving && typeof (KStarForm.OnSaving) == "function") {
            if (KStarForm.OnSaving(this)) {
                KStarForm.save(this, KStarForm.OnSaved);
            }
        }
        else {
            KStarForm.save(this, KStarForm.OnSaved);
        }
    },
    formSubmit: function () {
        var isDraft = KStarForm.koHeaderModel.IsDraft();
        if (KStarForm.OnSubmitting && typeof (KStarForm.OnSubmitting) == "function") {
            if (KStarForm.OnSubmitting(this)) {
                if (isDraft) {
                    KStarForm.submit(this, KStarForm.OnSubmitted, KStarForm.refreshDraftTask);
                }
                else {
                    KStarForm.submit(this, KStarForm.OnSubmitted, KStarForm.refreshPendingTask);
                }
            }
        }
        else {
            if (isDraft) {
                KStarForm.submit(this, KStarForm.OnSubmitted, KStarForm.refreshDraftTask);
            }
            else {
                KStarForm.submit(this, KStarForm.OnSubmitted, KStarForm.refreshPendingTask);
            }
        }
    },
    formRedirect: function () {
        if (KStarForm.OnRedirecting && typeof (KStarForm.OnRedirecting) == "function") {
            if (KStarForm.OnRedirecting(this)) {
                KStarForm.showPersonChoose(this);
            }
        }
        else {
            KStarForm.showPersonChoose(this);
        }
    },
    formDelegate: function () {
        if (KStarForm.OnDelegating && typeof (KStarForm.OnDelegating) == "function") {
            if (KStarForm.OnDelegating(this)) {
                KStarForm.showPersonChoose(this);
            }
        }
        else {
            KStarForm.showPersonChoose(this);
        }
    },
    formGotoActivity: function () {
        var sender = this;
        if (KStarForm.OnGotoActivating && typeof (KStarForm.OnGotoActivating) == "function") {
            if (KStarForm.OnGotoActivating(sender)) {
                KStarForm.gotoActivity(sender, KStarForm.OnGotoactivated, KStarForm.refreshPendingTask);
            }
        }
        else {
            KStarForm.gotoActivity(sender, KStarForm.OnGotoactivated, KStarForm.refreshPendingTask);
        }
    },
    formAddSigner: function () {
        
         
        $("#AddSignerShow").modal('show').on('hidden.bs.modal', function (e) {});
    },
    formCarbonCopy: function () {
        if (KStarForm.OnCarbonCopying && typeof (KStarForm.OnCarbonCopying) == "function") {
            if (KStarForm.OnCarbonCopying(this)) {
                KStarForm.showPersonChoose(this);
            }
        }
        else {
            KStarForm.showPersonChoose(this);
        }
    },
    formReview: function () {
        if (KStarForm.OnReviewing && typeof (KStarForm.OnReviewing) == "function") {
            if (KStarForm.OnReviewing(this)) {
                KStarForm.review(this, KStarForm.OnReviewed, KStarForm.refreshWaitRead);
            }
        }
        else {
            KStarForm.review(this, KStarForm.OnReviewed, KStarForm.refreshWaitRead);
        }
    },
    formDelete: function () {
        var sender = this;
        if (KStarForm.koToolbarModel.ActionComment().length == 0) {
            KStar.Modaldialog.alert({
                msg: "请在'意见说明'中输入作废原因！"
            });

            return false;
        }
        KStar.Modaldialog.confirm(
        {
            msg: "是否确认作废？"
        })
        .on(function (e) {
            if (e) {
                if (KStarForm.OnDeleting && typeof (KStarForm.OnDeleting) == "function") {
                    if (KStarForm.OnDeleting(sender)) {
                        KStarForm.Delete(sender, KStarForm.OnDeleted, KStarForm.refreshPendingTask);
                    }
                }
                else {
                    KStarForm.Delete(sender, KStarForm.OnDeleted, KStarForm.refreshPendingTask);
                }
            }
        });
    },
    formUndo: function () {
        var sender = this;
        KStar.Modaldialog.confirm(
        {
            msg: "是否确认撤回？"
        })
        .on(function (e) {
            if (e) {
                if (KStarForm.OnUndoing && typeof (KStarForm.OnUndoing) == "function") {
                    if (KStarForm.OnUndoing(sender)) {
                        KStarForm.Undo(sender, KStarForm.OnUndoed, KStarForm.refreshPendingTask);

                    }
                }
                else {
                    KStarForm.Undo(sender, KStarForm.OnUndoed, KStarForm.refreshPendingTask);
                }
            }
        });
    },

    draft: function (sender, callback, onclosing) {
        var json = KStarForm.toJSON();
        var postUrl = $(sender).attr('data-url');
        $postJSON(postUrl, json,
         function (data) {
             var items = data;
             $.each(KStarForm.koAttachmentModel(), function (i) {
                 KStarForm.koAttachmentModel()[i].FileState(1);
             });
             for (var i = 0; i < items.length; i++) {
                 var item = items[i];
                 if (i == 0) {
                     if (!isNaN(item.Message)) {
                         KStarForm.koHeaderModel.FormId(item.Message);
                     }
                     else {
                         $alertWarning(item.Type, item.Message, onclosing);
                         return;
                     }
                 }
                 else {
                     $alertWarning(item.Type, item.Message, onclosing);
                 }
             }
             (callback && typeof (callback) == "function") && callback();
         }, function (err) {
             alert('Fail:' + err);
         });
    },
    save: function (sender, callback) {
        var json = KStarForm.toJSON();
        var postUrl = $(sender).attr('data-url');
        $postJSON(postUrl, json,
         function (data) {
             var items = data;
             $.each(KStarForm.koAttachmentModel(), function (i) {
                 KStarForm.koAttachmentModel()[i].FileState(1);
             });
             for (var i = 0; i < items.length; i++) {
                 var item = items[i];
                 $alertWarning(item.Type, item.Message);
             }
             (callback && typeof (callback) == "function") && callback();
         }, function (err) {
             alert('Fail:' + err);
         });
    },
    submit: function (sender, callback, onclosing) {
        if (this.isNeedValid && !KStarForm.isValid()) {
            $alertWarning('warning', KStarMessage.formInvalidDefaultMsg);
            return false;
        };

        var json = KStarForm.toJSON();
        var postUrl = $(sender).attr('data-url');
        $postJSON(postUrl, json,
         function (data) {
             if (data) {
                 var items = data;
                 for (var i = 0; i < items.length; i++) {
                     var item = items[i];
                     if (item.Type == 1) {
                         KStarForm.$formSubmitButton.attr("disabled", "disabled");
                     }
                     $alertWarning(item.Type, item.Message, onclosing);
                 }
             };
             (callback && typeof (callback) == "function") && callback(data);
         }, function (err) {
             alert('Fail:' + err);
         });
    },
    gotoActivity: function (sender, callback, onclosing) {
        if (KStarForm.koToolbarModel.ActionComment().length == 0) {
            KStar.Modaldialog.alert({
                msg: "请在'意见说明'中输入回退原因"
            });

            return false;
        }

        KStar.Modaldialog.confirm(
        {
            msg: "是否确认回退？"
        })
        .on(function (e) {
            if (e) {
                var actionName = KStarForm.$formBackToButton.attr("value");
                KStarForm.koToolbarModel.ActionName(actionName);
                var postUrl = KStarForm.$formBackToButton.attr('data-url');
                var json = KStarForm.toJSON();

                $post(postUrl, { jsonData: json, activityName: $(sender).find("span").html() },
                     function (data) {
                         var msgs = data;
                         for (var i = 0; i < msgs.length; i++) {
                             var item = msgs[i];
                             $alertWarning(item.Type, item.Message, onclosing);
                         };
                         (callback && typeof (callback) == "function") && callback();
                     }, function (err) {
                         alert('Fail:' + err);
                     });
            }
        });
    },
    review: function (sender, callback, onclosing) {
        var postUrl = $(sender).attr('data-url');
        var actionName = $(sender).val();
        KStarForm.koToolbarModel.ActionName(actionName);
        var json = KStarForm.toJSON();
        $postJSON(postUrl, json,
         function (data) {
             if (data) {
                 var items = data;
                 for (var i = 0; i < items.length; i++) {
                     var item = items[i];
                     $alertWarning(item.Type, item.Message, onclosing);
                 }
             };
             (callback && typeof (callback) == "function") && callback(data);
         }, function (err) {
             alert('Fail:' + err);
         });
    },
    Delete: function (sender, callback, onclosing) {
        var postUrl = KStarForm.$formDeleteButton.attr('data-url');
        var actionName = $(sender).val();
        KStarForm.koToolbarModel.ActionName(actionName);
        var json = KStarForm.toJSON();

        $postJSON(postUrl, json,
         function (data) {
             if (data) {
                 var items = data;
                 for (var i = 0; i < items.length; i++) {
                     var item = items[i];
                     $alertWarning(item.Type, item.Message, onclosing);
                 }
             };
             (callback && typeof (callback) == "function") && callback(data);
         }, function (err) {
             alert('Fail:' + err);
         });
    },
    chooseApplyAccountCallback: function (data) {
        var userName = data.Root.Users.Item[0].UserName;
        KStarForm.koHeaderModel.ApplicantAccount(userName);
    },
    autoClose: function (isConfirm) {
        window.onbeforeunload = null;
        if (getUrlVars("FromMail") == "1") {
            window.location.href = "/";
        } else {
            window.close();
        }
    },
    close: function () {
        if (getUrlVars("FromMail") == "1") {
            window.location.href = "/";
        } else {
            if (KStarForm.$formSaveButton.attr("id") || KStarForm.$formDraftButton.attr("id")) {
                KStar.Modaldialog.confirm(
                    {
                        msg: "是否确认关闭页面?"
                    }).on(function (e) {
                        if (e) {
                            window.close();
                        }
                    })
            }
            else {
                window.close();
            }
        }
    },
    Undo: function (sender, callback, onclosing) {
        var actionName = KStarForm.$formUndoButton.attr("value");
        KStarForm.koToolbarModel.ActionName(actionName);
        var postUrl = KStarForm.$formUndoButton.attr('data-url');
        var json = KStarForm.toJSON();

        $post(postUrl, { jsonData: json },
             function (data) {
                 var msgs = data;
                 for (var i = 0; i < msgs.length; i++) {
                     var item = msgs[i];
                     $alertWarning(item.Type, item.Message, onclosing);
                 };
                 (callback && typeof (callback) == "function") && callback();
             }, function (err) {
                 alert('Fail:' + err);
             });
    },
    initChoosePerson: function (sender, beforeCall, afterCall, options) {
        var url = $(sender).attr('data-url');
        
       
        var executeCall = function (data) {
            var postUrl = url;
            var userList = new Array();
            var items = data.Root.Users.Item;
            $.each(items, function (i) {
                userList.push(items[i].UserName);
            });
            var actionName = $(sender).val();
            KStarForm.koToolbarModel.ActionName(actionName);
            var json = KStarForm.toJSON();
            $post(postUrl, { jsonData: json, userList: userList },
                 function (data) {
                     var msgs = data;
                     for (var i = 0; i < msgs.length; i++) {
                         var item = msgs[i];
                         $alertWarning(item.Type, item.Message);
                     };
                     (afterCall && typeof (afterCall) == "function") && afterCall();
                 }, function (err) {
                     alert('Fail:' + err);
                 });
        };

        var ops = {
            type: 'Person',
            isshownonreference: true,
            callback: function (data) {
                if (beforeCall && typeof (beforeCall) == "function") {
                    if (beforeCall(this)) {
                        executeCall(data);
                    }
                }
                else {
                    executeCall(data);
                }
            },
            onDetermine: undefined,
            mutilselect: true
        };

        $.extend(ops, options);

        $(sender).initSelectPerson(ops)
    },
    showPersonChoose: function (sender) {
        //$(this).initSelectPerson();
    },
    uploaderSuccessCallback: function (e, data) {
        if (data && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var item = data.result[i];
                var itemViewModel = KStarForm.toKoModel(item);
                KStarForm.koAttachmentModel.push(itemViewModel);
            }
        }
    },
    uploaderFailCallback: function (e, data) {
        alert(data.messages.uploadedBytes);
    },
    init: function () {

        //Register ajax default behav
        KStar.ajaxGlobal();
        KStar.uploader(KStarForm.$formAttachmentButton, this.uploaderSuccessCallback, this.uploaderFailCallback);
        $("[data-toggle='popover']").popover();
        $.validator.setDefaults({
            errorPlacement: function (error, element) {
                if ((element.is(":radio") || element.is(":checkbox")) && element.data("bind") != undefined) {
                    error.appendTo(element.parent().parent().find('>:last'));
                }
                else if (element.is(":checkbox")) {
                    error.appendTo(element.next());
                }
                else {
                    error.appendTo(element.parent());
                }
            }
        });

        //this.initChoosePerson(this.$formCountersignButton, this.OnAddSignering, this.OnAddSignered);
        this.initChoosePerson(this.$formRedirectButton, this.OnRedirecting, this.OnRedirected, { mutilselect: false, type: "All" });
        this.initChoosePerson(this.$formDelegateButton, this.OnDelegating, this.OnDelegated);
        this.initChoosePerson(this.$formCarbonCopyButton, this.OnCarbonCopying, this.OnCarbonCopied);

        this.$formDraftButton.click(this.formDraft);
        this.$formSaveButton.click(this.formSave);
        this.$formSubmitButton.click(this.formSubmit);
        this.$formBackToButton.find("ul li").each(function () {
            $(this).click(KStarForm.formGotoActivity);
        });
        this.$formReviewButton.click(this.formReview);
        this.$formDeleteButton.click(this.formDelete);
        this.$formCloseButton.click(this.close);
        this.$formUndoButton.click(this.Undo);
        this.$formCountersignButton.click(this.formAddSigner);
        this.$formSeerButton.click(function (e) {

            //注册Handlebars 索引+1的helper
            Handlebars.registerHelper("addOne", function (index) {
                //返回+1之后的结果
                return index + 1;
            });

            $.getJSON("/api/Custom_Utilities/GetPrognosis?procInstID=" + KStarForm.koHeaderModel.ProcInstId() + "&actName=" + KStarForm.koHeaderModel.ActivityName(), null, function (data) {
                 
                var prognosisTemplate = Handlebars.compile($('#table-Prognosis').html())(data);
                prognosisTemplate = prognosisTemplate.trim();
                //prognosisTemplate = prognosisTemplate.replace(/\n/g, ''); 
                $(prognosisTemplate).modal('show').on('hide.bs.modal', function (e) {
                    $(e.target).remove();
                });
            });

           

        });

        $(document).delegate(".input-group-addon", "mouseover", function () {
            $(this).css("cursor", "pointer");
        });
        $(document).delegate(".input-group-addon", "click", function () {
            $(this).prev().click();
        });

        if (getUrlVars("FromMail") == "1") {
            this.$formCloseButton.val("首页");
        }

        //if (KStarForm.$formSaveButton.attr("id") || KStarForm.$formDraftButton.attr("id")) {
        //    window.onbeforeunload = function () {
        //        debugger;
        //        if  (event.clientX>document.body.clientWidth  &&  event.clientY<0 ||event.altKey)  
        //            window.event.returnValue="确定要退出吗？";  
        //    }

        //}
    },
    

    /* ApplyBindings */
    applyData: function (jsModel) {
        var jsonString = JSON.stringify(jsModel);
        jsModel = JSON.parse(jsonString, jsonDate_deserializer);

        if (isFunc(this.beforeApplyData)) {
            jsModel = this.beforeApplyData(jsModel);
        }

        this.koAttachmentModel = this.toKoModel(jsModel.Attachments);
        this.koProcessLogModel = this.toKoModel(jsModel.ProcessLogs);
        if (!$.isEmptyObject(jsModel.ContentData)) {
            this.koContentModel = this.toKoModel(jsModel.ContentData);
        }
        this.koToolbarModel = this.toKoModel(jsModel.Toolbar);

        delete jsModel.Attachments;
        delete jsModel.ProcessLogs;
        delete jsModel.ContentData;
        delete jsModel.Toolbar;

        //-- Request Infomation
        this.koHeaderModel = this.toKoModel(jsModel);
        var selfHeaderModel = this.koHeaderModel;
        selfHeaderModel.Positions = ko.observableArray();
        selfHeaderModel.Departments = ko.observableArray();

        selfHeaderModel.selectedPosition = !jsModel.ApplicantPositionID
            ? null : { PositionID: jsModel.ApplicantPositionID, PositionName: jsModel.ApplicantPositionName };

        selfHeaderModel.selectedOrgNode = !jsModel.ApplicantOrgNodeID
            ? null : { OrgNodeID: jsModel.ApplicantOrgNodeID, OrgNodeName: jsModel.ApplicantOrgNodeName };

        ko.dependentObservable(function () {
            $.getJSON($_userPickerUrl, { userName: selfHeaderModel.ApplicantAccount() }, function (userData) {
                selfHeaderModel.ApplicantDisplayName(userData.ApplicantDisplayName);
                selfHeaderModel.ApplicantTelNo(userData.ApplicantTelNo);
                selfHeaderModel.ApplicantEmail(userData.ApplicantEmail);
                var userPositions = userData.Positions;
                selfHeaderModel.Positions.removeAll();
                $.each(userPositions, function (n) {
                    if (userPositions[n].Name.substring(0, 1) != "v") {
                        selfHeaderModel.Positions.push(userPositions[n]);
                    }
                })
                var userDepartments = userData.Departments;
                selfHeaderModel.Departments.removeAll();
                $.each(userDepartments, function (n) {
                    if (userDepartments[n].Name.substring(0, 1) != "v") {
                        selfHeaderModel.Departments.push(userDepartments[n]);
                    }
                })
                //selfHeaderModel.Positions(userData.Positions);
                //selfHeaderModel.Departments(userData.Departments);
                KStarForm.userPositionsModel = userData.Positions;

                var selectedPosition = selfHeaderModel.selectedPosition;
                if (selectedPosition) {
                    selfHeaderModel.ApplicantPositionID(selectedPosition.PositionID);
                    selfHeaderModel.ApplicantPositionName(selectedPosition.PositionName);
                }

                var selectedOrgNode = selfHeaderModel.selectedOrgNode;
                if (selectedOrgNode) {
                    selfHeaderModel.ApplicantOrgNodeID(selectedOrgNode.OrgNodeID);
                    selfHeaderModel.ApplicantOrgNodeName(selectedOrgNode.OrgNodeName);
                }

            });
        }, selfHeaderModel.ApplicantAccount);

        if (this.$formHeaderContainer[0]) {
            this.applyBindings(this.koHeaderModel, this.$formHeaderContainer[0]);
        }

        //-- Attachment List
        var attModel = {
            AttachmentModel: ko.computed(function () {
                return ko.utils.arrayFilter(KStarForm.koAttachmentModel(), function (item) {
                    return item.FileState() != 3;
                });
            }),
            removeItem: (function (item) {
                var state = item.FileState();
                if (state == 2) {//Cached
                    KStarForm.koAttachmentModel.remove(item);
                } else if (state == 1) {//Edited
                    item.FileState(3);//Removed
                }
            })
        };

        if (this.$formAttachmentContainer[0]) {
            this.applyBindings(attModel, this.$formAttachmentContainer[0]);

            //Check Attachment download Permission

            $.getJSON("/api/Custom_Utilities/FileDownloadPermission", null, function (data) {
                var downloadPermissions = data.Result;
                if (downloadPermissions == false) {
                    var downloadObject = $('#_kstarform_attachment_list').find("tbody tr").find("a");
                    downloadObject.attr("href", "javascript:void(0)");
                    downloadObject.css("text-decoration", "none ");
                    downloadObject.attr("class", "glyphicon");
                }
            });
        }

        //-- ProcessLog List 
        if (this.$formProcessLogContainer[0]) {
            this.applyBindings(this.koProcessLogModel, this.$formProcessLogContainer[0]);
        }

        //-- Toolbar Operation
        if (this.koToolbarModel.TaskActions().length == 1) {
            this.koToolbarModel.ActionName = ko.observable(this.koToolbarModel.TaskActions()[0]);
        }

        this.koToolbarModel.ViewFlow = function () {
            window.open(KStarForm.koToolbarModel.ViewFlowUrl());
        }

        if (this.$formToolbarContainer[0]) {
            this.applyBindings(this.koToolbarModel, this.$formToolbarContainer[0]);
        }

        //-- Content FormBody        
        if (isFunc(this.extendModel)) {//Extend content model function
            this.koContentModel = this.extendModel(this.koContentModel);
        }

        if (isFunc(this.beforeBindingExtendModel)) {
            this.koContentModel = this.beforeBindingExtendModel(this.koContentModel);
        }

        if (this.$formContentContainer[0]) {
            this.applyBindings(this.koContentModel, this.$formContentContainer[0]);
        }

        if (isFunc(this.afterBindingExtendModel)) {
            this.koContentModel = this.afterBindingExtendModel(this.koContentModel);
        }

        if (isFunc(this.afterApplyData)) {
            this.afterApplyData();
        }
    },
    extendModel: {},
    registerModel: function (jsModel) {

        this.koContentModel = this.toKoModel(jsModel);
    },

    toJSON: function () {
        var jsModel = this.koHeaderModel;
        jsModel.Toolbar = this.koToolbarModel;
        jsModel.Attachments = this.koAttachmentModel;
        jsModel.ProcessLogs = this.koProcessLogModel;
        jsModel.ContentData = this.koContentModel;

        return this.getJSON(jsModel);
    },

    OnDrafting: {},
    OnDrafted: {},
    OnSaving: {},
    OnSaved: {},
    OnSubmitting: {},
    OnSubmitted: {},
    OnRedirecting: {},
    OnRedirected: {},
    OnDelegating: {},
    OnDelegated: {},
    OnUndoing: {},
    OnUndoed: {},
    OnGotoActivating: {},
    OnGotoactivated: {},
    OnAddSignering: {},
    OnAddSignered: {},
    OnCarbonCopying: {},
    OnCarbonCopied: {},
    OnReviewing: {},
    OnReviewed: {},
    OnDeleting: {},
    OnDeleted: {},
    beforeBindingExtendModel: {},
    afterBindingExtendModel: {},
    beforeApplyData: {},
    afterApplyData: {}
};



/** Auto run when page is loaded **/
//$(function () {

//    $('div[role=_kstarform_body] :input').each(function (index) {
//        //alert($(this).attr('type') +' is input: '+ $(this).is('input'));
//    });

//    KStarForm.init();
//});
