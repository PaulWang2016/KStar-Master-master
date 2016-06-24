_form_Workflow = {
    Import: {
        execl: function (e) {
            var targetElement = null;
            if (this.execl == undefined) {
                targetElement = this;
            } else {
                targetElement = e.target;
            }
            var fileURL = $(targetElement).val();
            var postfix = fileURL.substring(fileURL.lastIndexOf(".") + 1).toUpperCase();
            if (postfix == null || postfix == "") return;
            if (postfix == "XLSX" || postfix == "XLS") {
                var rowInitName = $(targetElement).data("execl-init-row");//row init 事件
                var beforeRowInitName = $(targetElement).data("before-execl-init-row");//row init 事件
                var importbefore = $(targetElement).data("execl-import-before");//导入之前执行
                var importafter = $(targetElement).data("execl-import-after");//导入之后执行
                var rowInitFunction = eval(rowInitName);//rowInitfunction
                var importafterFunction = eval(importafter);
                var importbeforeFunction = eval(importbefore);
                var importbeforeRowFunction = eval(beforeRowInitName);

                var ImportEvent = function (e, target) {
                    var file = target.files[0];
                    var reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onload = function () {
                        var data = reader.result;
                        $.post("/api/Custom_Upload/ImportExcel", { "": data }, function (data, textStatus) {
                            if (data != null && typeof data == "object" && data.length > 0) {
                                if (typeof importbeforeRowFunction == 'function') {
                                    if (importbeforeRowFunction(data)) {
                                        $.each(data, function (index, item) {
                                            if (rowInitFunction != null && rowInitFunction != undefined && typeof rowInitFunction == 'function') {
                                                rowInitFunction(index, item);
                                            }
                                        });
                                    }
                                } else {
                                    $.each(data, function (index, item) {
                                        if (rowInitFunction != null && rowInitFunction != undefined && typeof rowInitFunction == 'function') {
                                            rowInitFunction(index, item);
                                        }
                                    });
                                }
                           
                            }
                            if (importafterFunction != null && importafterFunction != undefined && typeof importafterFunction == 'function') {
                                importafterFunction(data);
                            }

                        }, "json")
                    };

                    var ie = (navigator.appVersion.indexOf("MSIE") != -1);//IE          
                    if (ie) {
                        $(target).select();
                        document.execCommand("delete");
                    } else {
                        $(target).val("");
                    }
                };
                if (importbeforeFunction != null && importbeforeFunction != undefined && typeof importbeforeFunction == 'function') {
                    importbeforeFunction(target);
                }
                ImportEvent(e, targetElement);
            } else {
                alert("只能选择Execl文件");
            }
        }
    }
} 