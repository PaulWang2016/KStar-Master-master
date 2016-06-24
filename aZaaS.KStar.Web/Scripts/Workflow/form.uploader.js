/** Workflow & uploader **/
(function($){
    $.fn.formuploaderUtils = {
        listGroup:{
            ItemTemplate: function (fileobject, isDownload) {
                var item = '<li class="list-group-item" style="padding: 6px 15px;"> ';
                item += ' <span class="">' + (fileobject.NewFileName() + fileobject.FileExtension()) + '</span> ';
                if (isDownload == true) {
                    item += '<a href="' + fileobject.DownloadUrl() + '" title=" 下 载 "><span class="glyphicon glyphicon-download-alt pull-right"></span></a> ';
                } else {
                    //没有权限
                    //item += '<a href="' + fileobject.DownloadUrl() + '" title=" 下 载 "><span class="glyphicon glyphicon-download-alt pull-right"></span></a> ';
                }
                item += ' <a href="javascript:void(0)" title="移除"> <span class="glyphicon glyphicon-trash pull-right" style="margin: 0 10px;"></span></a>';
                item += '</li>';
                return item;

            },
            ItemLoad: function (target, fileobjects, initLoadedFunction, removeFunction, isDownload) {
                $.each(fileobjects, function (index, item) { 
                    var liItem = $.fn.formuploaderUtils.listGroup.ItemTemplate(item, isDownload);
                    liItem = $(liItem);
                    liItem.data("FileGuid", item.FileGuid());//存储guid
                    liItem.appendTo(target);
                    var trash = liItem.find(".glyphicon-trash");
                    var disabled = target.parent().find('input:File').attr("disabled");
                    if (disabled != "disabled") {
                        trash.bind("click", function (e) {
                            var scopeDivtarget = e.target.parentElement.parentElement.parentElement.parentElement.parentElement;
                            var beforeDeleteFunction = eval($(scopeDivtarget).data("before-delete"));

                            if (typeof beforeDeleteFunction == "function") {
                                beforeDeleteFunction(e);
                            }

                            var guid = $(e.target.parentElement.parentElement).data("FileGuid");
                            Workflow.utils.attachment.remove(guid);//删除附件

                            if (typeof removeFunction == 'function')
                                removeFunction(e, guid);

                            $(e.target.parentElement.parentElement).remove();
                        });
                    } else {
                        trash.remove();
                    }
                  
                });
                if (typeof initLoadedFunction == 'function') {
                    initLoadedFunction();
                }
            }
        }, 
        uploader: {
            refresh: function () {
               this.scopeRefresh(undefined);
            },
            scopeRefresh: function ($$scope) {
                var items = null;
                if ($$scope == undefined)
                    items = $("span[data_bind_visible]");
                else
                    items = $($$scope).find("span[data_bind_visible]");

                //check download  permissions
                var downloadPermissions = true;
                if (items.length > 0) { 
                    $.getJSON("/api/Custom_Utilities/FileDownloadPermission", null, function (data) {
                        downloadPermissions = data.Result;
                        $.each(items, function (Name, Value) {
                            var isforeach = $(Value).data("foreach-type");
                            var splitkoModelProperty = $(Value).attr("data_bind_visible");
                            var koObject = null;
                            if (isforeach == true) {
                                var index = $(Value.parentElement.parentElement.parentElement).attr("index");//取显示数组下标
                                koObject = Workflow.utils.koModel.koContentModel.findArrayProperty(index, splitkoModelProperty);

                            } else {
                                koObject = Workflow.utils.koModel.koContentModel.find(splitkoModelProperty);
                            }

                            if (koObject == undefined) {
                                console.log(Value);
                                return true;//退出当前循环
                            }
                            var koObjectValue = koObject();
                            if (koObjectValue == undefined || koObjectValue == "" || koObjectValue == null) return;


                            var ulObject = $(Value.parentElement.parentElement).find("ul");//UI 显示列表容器
                            var inputText = $(Value.parentElement.parentElement).find("input:text");// 初始状态下的input。
                            var koObjectFileObject = Workflow.utils.attachment.finds(koObjectValue);//文件对象

                            $.fn.formuploaderUtils.listGroup.ItemLoad(ulObject, koObjectFileObject, function () {
                                inputText.hide();
                                ulObject.show();
                                $.fn.formuploaderUtils.uploader.refreshAttachment(koObjectValue);
                            }, function (e, fileGuid) {
                                var koModelBindString = $(e.target.parentElement.parentElement.parentElement).data("file_bind");
                                var scopeVisible = $(e.target.parentElement.parentElement.parentElement.parentElement).find("span[data_bind_visible]");
                                var isforeach = scopeVisible.data("foreach-type");
                                var scopeDivtarget = e.target.parentElement.parentElement.parentElement.parentElement.parentElement;
                                var afterFunction = $(scopeDivtarget).data("after-delete");;
                                var koModelobject = null;
                                var rowModel = null;
                                if (isforeach) {
                                    var index = $(scopeDivtarget).attr("index");//取显示数组下标
                                    koModelobject = Workflow.utils.koModel.koContentModel.findArrayProperty(index, koModelBindString);
                                    rowModel = Workflow.utils.koModel.koContentModel.findArrayRow(index, koModelBindString);
                                } else {
                                    koModelobject = Workflow.utils.koModel.koContentModel.find(koModelBindString);
                                }

                                var koModelValueString = koModelobject();
                                var newItemsFileGuid = "";
                                if (koModelValueString.lastIndexOf(";") >= 0) {
                                    newItemsFileGuid = koModelValueString.replace(fileGuid + ';', '');
                                    if (newItemsFileGuid == koModelValueString) {
                                        newItemsFileGuid = koModelValueString.replace(';' + fileGuid, '');
                                    }
                                } else {
                                    inputText.show();
                                    ulObject.hide();
                                }
                                koModelobject(newItemsFileGuid);

                                $.fn.formuploaderUtils.uploader.refreshAttachment(newItemsFileGuid);//设置附件

                                if (typeof afterFunction == "string" && typeof window[afterFunction] == "function") {
                                    window[afterFunction](e, (rowModel || koModelobject));
                                } else if (typeof afterFunction == "function") {
                                    afterFunction(e, (rowModel || koModelobject));
                                }
                            }, downloadPermissions);
                        });
                    });
                }

            },
            refreshAttachment: function (fileGuids) {
                Workflow.utils.attachment.hide(fileGuids);
            }
        }
         
    }
    $.fn.formuploader = function (opts) {

        var defaults = {
        }
        var option = $.extend(defaults, opts);
        var self = {};
        //填充 knockout 绑定控件的值   targetBindString 目标控件帮定字符串， value   试用范围 koContentModel
        self.fillKoValue = function (targetBindString, index, value, isforeach) {

            var koObject = null;
            if (isforeach == true) {
                koObject = Workflow.utils.koModel.koContentModel.findArrayProperty(index, targetBindString);
            } else {
                koObject = Workflow.utils.koModel.koContentModel.find(targetBindString);
            } 
            koObject(value);//执行
        }

        self.foreachFormuploaderStr = function ($index, fileBindKoName) {
            var inputstr = '<div class="input-group" data-file_control>';
            inputstr += '   <ul class="list-group"  style="display: none;margin: 0px;" data-file_bind="' + fileBindKoName + '"></ul> ';
            inputstr += '   <input type="text" class="form-control" />';
            inputstr += '   <span class="input-group-btn" style="vertical-align: top;">';
            inputstr += '     <span data_bind_visible="' + fileBindKoName + '"  data-foreach-type="true">';
            inputstr += '      </span>';
            inputstr += '      <input type="file" style="width: 60px;height: 35px;z-index:1;right: 0px;top:0px;position: absolute; opacity:0"/>';
            inputstr += '     <input type="button" class="btn btn-primary" title="浏览" value="浏览" />';
            inputstr += '   </span>';
            inputstr += '</div>';

            return inputstr;
        }

        self.formuploaderStr = function (fileBindKoName) {
            var inputstr = '<div class="input-group" data-file_control>';
            inputstr += '   <ul class="list-group"  style="display: none;margin: 0px;" data-file_bind="' + fileBindKoName + '"></ul> ';
            inputstr += '   <input type="text" class="form-control"/>';
            inputstr += '   <span class="input-group-btn" style="vertical-align: top;">';
            inputstr += '      <span data_bind_visible="' + fileBindKoName + '" data-foreach-type="false">';
            inputstr += '      </span>';
            inputstr += '      <input type="file" name="files[]"   multiple="multiple"  style="width: 60px;height: 35px;z-index:1;right: 0px;top:0px;position: absolute; opacity:0"/>';
            inputstr += '     <input type="button" class="btn btn-primary" title="浏览" value="浏览"/>';
            inputstr += '   </span>';
            inputstr += '</div>';
            return inputstr;
        }
        //回调
        self.callBack = function (target, data) {
            var $target= $(target.parentElement);
            var callback = $target.data("callback");

            var callbackFunction = window[callback];
            if (typeof callbackFunction === 'function') {
                callbackFunction($target, data);
            }
        };
        //$target Uploader 对象 
        self.ShowUploaderList = function ($target) {
            var inputText = $target.find('input:text');
            inputText.hide();
            var ul = $target.find('ul');
            ul.show();//显示
            inputText.val("valid");//填充值
            inputText.valid();//验证
            inputText.val("");//清空
        };

        self.addRequired = function (_this, target) {
            var required = _this.data("rule-required");
            var msgrequired = _this.data("msg-required");
            if (required) {
                var requiredInput = target.find("input:text");
                requiredInput.attr('name', self.Guid());
                requiredInput.data("rule-required", true);
                if (msgrequired == undefined || msgrequired == '') {
                    requiredInput.data("msg-required");
                } else {
                    requiredInput.data("msg-required", msgrequired);
                }

            }
        };

        self.Guid = function () {
            var S4 = function () {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            };
            return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
        }
    
        this.each(function () {
            var _this = $(this);
            var typeStr = _this.data("workflow-uploader-type");//控件类型
              
            var foreachType = false; 
            if (typeStr == null || typeStr == undefined) typeStr = "";

            var $index = 0;//foreach Type
            if (typeStr.toLowerCase() == "foreach") {
                $index = $(this).attr("index");
                foreachType = true;
            }
             
            var fileBindKoName = _this.data("workflow-uploader");//绑定上传的KO 字段 全路径
            if (fileBindKoName == undefined || fileBindKoName == null || $.trim(fileBindKoName) == "") return;
           
            //foreach 
            if (foreachType) {
                var fileBindKoName = _this.data("workflow-uploader");//绑定上传的KO 字段 全路径
                var foreachFormuploaderHtml = self.foreachFormuploaderStr($index, fileBindKoName);
                var formuploaderControl = $(foreachFormuploaderHtml);
              
                if (_this.attr("disabled") == "disabled") {
                    formuploaderControl.find("input").attr("disabled", "disabled");
                }
                self.addRequired(_this, formuploaderControl);

                _this.append(formuploaderControl);
              
                KStar.uploader(formuploaderControl, function (e, data) {
                    KStarForm.uploaderSuccessCallback(e, data);
                    var itemFileGuid = data.result[0].FileGuid;  //获取首行的guid
                    var index = $(e.target.parentElement).attr("index");
                    self.ShowUploaderList($(e.target));//显示文件上次列表
                    var groupList = $(e.target).find("ul");
                    groupList.find("li").remove();//单文件上传每次清空上次结果。
                    var fileBindString = groupList.data("file_bind");//获取绑定实体 
                    self.fillKoValue(fileBindString, index, itemFileGuid, true);//填充上次文件的guid值
                    $.fn.formuploaderUtils.uploader.scopeRefresh(groupList.parent());
                    self.callBack(e.target, data.result[0]);
                }, KStarForm.uploaderFailCallback);
            } else {
                var foreachFormuploaderHtml = self.formuploaderStr(fileBindKoName);
                var formuploaderControl = $(foreachFormuploaderHtml);
                if (_this.attr("disabled") == "disabled") {
                    formuploaderControl.find("input").attr("disabled", "disabled");
                }
                self.addRequired(_this, formuploaderControl);
                _this.append(formuploaderControl); 
                KStar.uploader(formuploaderControl, function (e, data) {
                    KStarForm.uploaderSuccessCallback(e, data);//向平台添加附件
                    var itemFileGuid = "";
                    $.each(data.result, function (index,item) {
                        if (index == 0) {
                            itemFileGuid += item.FileGuid;
                        } else {
                            itemFileGuid += ";" + item.FileGuid;
                        }
                    });//多文件 

                    self.ShowUploaderList($(e.target),"");//显示上次文件列表
                    var groupList = $(e.target).find("ul");
                    var fileBindString = groupList.data("file_bind");
                    var kofileBind = Workflow.utils.koModel.koContentModel.find(fileBindString);
                    var kofileBindvalue = kofileBind();
                    groupList.find("li").remove();
                    if (!(kofileBindvalue == null || kofileBindvalue == undefined ||  $.trim(kofileBindvalue)=="")){
                        itemFileGuid  =(kofileBindvalue == "" ? "" : kofileBindvalue+";")+ itemFileGuid;//添加以前的文件。
                    }
                      
                    self.fillKoValue(fileBindString, 0, itemFileGuid, false);//更新文件数据到对应file对应的实体
                    $.fn.formuploaderUtils.uploader.scopeRefresh(groupList.parent());
                    self.callBack(e.target, data.result);
                  
                });
            } 
        }); 
    }
})(jQuery)

/** Workflow & Utilities **/
Workflow = {
    utils: {
        ride: {
            init: function () {
                var items = $("input[data-ride_bind]");
                $.each(items, function (name, value) {
                    var tempItem = $(value);//doment 对象转换成jQuery对象

                    if (value.type == "radio" && tempItem.data('ride_bind') != undefined) {
                        var ride_bindObject = eval('(' + tempItem.data('ride_bind') + ')');

                        var koModelProperty = ride_bindObject.koModelProperty;
                        var splitkoModelProperty = koModelProperty.split('.');
                        var targetObject = null;
                        $.each(splitkoModelProperty, function (splitName, splitValue) {
                            if (targetObject == null) {
                                targetObject = KStarForm.koContentModel[splitValue];
                            } else {
                                targetObject = targetObject[splitValue];
                            }
                        });
                        if (targetObject == undefined) {
                            console.log(value);
                            return true;//退出当前循环
                        }
                        var IsNormal = targetObject();
                        var repeatOpen = KStarForm.koHeaderModel.ActivityName() + "";
                        if ($(value).data("ride-autofill") || value.disabled ||  repeatOpen.indexOf('015')>=0|| Workflow.utils.checkActivityName()) { //是否填充默认值
                            if (IsNormal == ride_bindObject.koModelvalue) {
                                value.checked = true;
                            } else {
                                value.checked = false;
                            }
                        }
                        //绑定事件
                        tempItem.bind('click', function () {
                            //Check is Normal type
                            var AutoCheckRadioToModel = function (IsNormal, event, koModelProperty) {
                                if (event.checked) {
                                    var splitkoModelProperty = koModelProperty.split('.');
                                    var targetObject = null;
                                    $.each(splitkoModelProperty, function (name, value) {
                                        if (targetObject == null) {
                                            targetObject = KStarForm.koContentModel[value];
                                        } else {
                                            targetObject = targetObject[value];
                                        }
                                    });
                                    targetObject(IsNormal);
                                }
                            };                
                            AutoCheckRadioToModel(ride_bindObject.koModelvalue, value, ride_bindObject.koModelProperty)
                        });
                    }
                });          
            }
        },
        koModel: {
            koContentModel: {
                //在koContentModel中查询对应的koModel 监控对象 
                //Location 数组下标，koModelBindString绑定的字符串
                findArrayProperty: function (Location, koModelBindString) {
                    var koObject = this.findArrayRow(Location, koModelBindString);
                    if (koObject == null || koObject == "") return null;
                    var splitkoModelProperty = koModelBindString.split('.');
                    var property = splitkoModelProperty[splitkoModelProperty.length - 1];
                    property = $.trim(property);
                    return koObject[property];
                },
                find: function (koModelBindString) {
                    var splitkoModelProperty = koModelBindString.split('.');
                    var koObject = null;
                    $.each(splitkoModelProperty, function (splitName, splitValue) {
                        splitValue = $.trim(splitValue);
                        if (koObject == null) {
                            koObject = (KStarForm.koContentModel[splitValue]);
                        } else {
                            koObject = koObject[splitValue];
                        }
                    });
                    return koObject
                },
                //查找对应绑定的row 对象
                findArrayRow: function (Location, koModelBindString) {
                    if (Location == undefined || koModelBindString == undefined || koModelBindString == "" || Location == "") return;

                    var splitkoModelProperty = koModelBindString.split('.');
                    var koObject = null;
                    var splitCount = splitkoModelProperty.length;
                    $.each(splitkoModelProperty, function (splitName, splitValue) {
                        splitValue = $.trim(splitValue);
                        if (splitValue.substring(splitValue.length - 1) == "]") { 
                            var  _splitValue  = splitValue.substring(0, splitValue.lastIndexOf("["));

                            var _indexString = splitValue.replace(_splitValue, "");
                            var _index = parseInt(_indexString.replace("[", "").replace("]", ""));
                            
                            if (koObject == null) {
                                koObject = (KStarForm.koContentModel[_splitValue]);
                                koObject = koObject()[_index];
                            } else {
                                if (splitName.toString() == (splitCount - 1).toString()) {
                                    koObject = koObject()[Location];
                                }
                                else {
                                    koObject = koObject[_splitValue];
                                    koObject = koObject()[_index]; 
                                }
                            }

                        } else {
                            if (koObject == null) {
                                koObject = (KStarForm.koContentModel[splitValue]);
                            } else {
                                if (splitName.toString() == (splitCount - 1).toString()) {
                                    koObject = koObject()[Location];
                                }
                                else {
                                    koObject = koObject[splitValue];
                                }
                            }
                        }
                    });

                    return koObject;
                }
            }
        },
        attachment: {
            find: function (fileGuid) {
                var attachments = KStarForm.koAttachmentModel();
                var attachment = null;
                //找到对应的附件
                $.each(attachments, function (name, value) {
                    if (value.FileGuid() == fileGuid) {
                        attachment = value;
                    }
                });
                return attachment;
            },
            finds: function (fileGuids) {
                fileGuids = fileGuids.split(";")
                var attachments = KStarForm.koAttachmentModel();
                var attachmentArray = [];
                //找到对应的附件
                $.each(attachments, function (name, value) {
                    var compareFileGuid = value.FileGuid();
                    if ($.inArray(compareFileGuid, fileGuids) >= 0) {
                        attachmentArray.push(value);
                    }
                });
                return attachmentArray;
            },
            remove: function (fileGuids) {

                var attachments = this.finds(fileGuids);
                //找到对应的附件 
                if (attachments != null)
                    $.each(attachments, function (index, item) {
                        KStarForm.koAttachmentModel.remove(item);//删除对应的附件
                    });
            },
            hide: function (fileGuids) {
                fileGuids = fileGuids.split(";");
                var attachments = KStarForm.koAttachmentModel();
                var attachmentUI = $("#_kstarform_attachment_list").find("tbody").find("tr");
                $.each(attachments, function (index, attachment) {
                    var compareFileGuid = attachment.FileGuid();
                    if ($.inArray(compareFileGuid, fileGuids) >= 0) {
                        $(attachmentUI[index]).hide();//附件隐藏
                    }
                });
            }
        },//check  ActivityName 是否存在过审批  存在是true， 不存在是false
        checkActivityName: function () {
            var activityName = KStarForm.koHeaderModel.ActivityName()
            var ActivityNames = KStarForm.koProcessLogModel();
            var isCheck = false;
            $.each(ActivityNames, function (index, Item) {
                if (Item["ActivityName"]() == activityName) {
                    isCheck = true;
                    return true;
                }
            });
            return isCheck;
        }
    }
}

$(document).ready(function(){
    var initFormuploader = function () {
        var items = $("div[data-workflow-uploader]");
        items.formuploader();
        $.fn.formuploaderUtils.uploader.refresh();
    } 
    initFormuploader();
});