var CategoryTemplateWindow = {};

CategoryTemplateWindow = {
    $formCategoryTemplateContainer: $('div[role=CategoryTemplateWindow]'),
    koCategoryTemplateModel: {},
    template:{        
        load_mask_template: '<li id="load_mask_template" class="level0" tabindex="0" hidefocus="true" treenode=""><span class="button ico_loading"></span></li>'        
    },
    initGrid: function () {
        var $table = $('#categorytemplategrid');

        function zTreeOnClick(event, treeId, treeNode) {
            //重新加载当前选择类别的模板            
            $('#controltemplategrid').attr("selectCategory", treeNode.ID);
            $('#controltemplategrid').attr("selectCategoryType", treeNode.Type);
            $("#controltemplategrid").bootgrid("reload");
        };
        var firstAsyncSuccessFlag = 0;
        function zTreeOnAsyncSuccess(event, treeId, msg) {
            $("#load_mask_template").remove();
            if (firstAsyncSuccessFlag == 0) {
                var zTree = $.fn.zTree.getZTreeObj("categorytemplategrid");
                try {
                    //调用默认展开第一个结点   
                    var selectedNode = zTree.getSelectedNodes();
                    var nodes = zTree.getNodes();
                    zTree.expandNode(nodes[0], true);                   
                    firstAsyncSuccessFlag = 1;
                } catch (err) { }
            }
        }
        var firstBeforeAsyncFlag = 0;
        function zTreeBeforeAsync(treeId, treeNode) {
            if (firstBeforeAsyncFlag == 0) {
                $("#categorytemplategrid").append(CategoryTemplateWindow.template.load_mask_template);
                firstBeforeAsyncFlag = 1;
            }
        };

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
                url: $table.attr("url"),
                autoParam:["ID","Type"]            
            }
            , callback: {
                onClick: zTreeOnClick,
                onAsyncSuccess: zTreeOnAsyncSuccess,
                beforeAsync: zTreeBeforeAsync
            }
        };
        $.fn.zTree.init($table, setting);
    }
    , initCategoryTemplateModel: function () {
        KStarForm.applyBindings(this.koCategoryTemplateModel, this.$formCategoryTemplateContainer[0]);
    }
    , initkoCategoryTemplateModel: function () {
        this.koCategoryTemplateModel = KStarForm.toKoModel({
            SysId: "00000000-0000-0000-0000-000000000000",
            ParentId: null,            
            CategoryName: ""           
        });
    }
    , clearCategoryTemplateWindow: function () {
        var controlSettingModel = this.koCategoryTemplateModel;
        controlSettingModel.SysId("00000000-0000-0000-0000-000000000000");
        controlSettingModel.ParentId(null);        
        controlSettingModel.CategoryName("");        
    }
    , initButtons: function () {
        CategoryTemplateWindow.initkoCategoryTemplateModel();
        CategoryTemplateWindow.initCategoryTemplateModel();
        $("#AddCategoryTemplate").click(function () {
            CategoryTemplateWindow.clearCategoryTemplateWindow();
            $(".alert-danger").hide();
            $('#CategoryTemplateWindow').modal('show');
            $("#CategoryTemplateWindow  button.btn-primary").unbind("click").bind("click", function () {
                CategoryTemplateWindow.CategoryTemplateAction("AddCategoryTemplate");
            });
        });

        $("#EditCategoryTemplate").click(function () {
            var zTree = $.fn.zTree.getZTreeObj("categorytemplategrid");
            var selectedNode = zTree.getSelectedNodes();
            if (selectedNode.length == 0)
            {
                return;
            }
            CategoryTemplateWindow.EditCategoryTemplate(selectedNode[0].ID);
        });
        $("#DeleteCategoryTemplate").click(function () {
            var zTree = $.fn.zTree.getZTreeObj("categorytemplategrid");
            var selectedNode = zTree.getSelectedNodes();
            if (selectedNode.length == 0) {
                return;
            }
            $("#ConfirmWindow .modal-body").html("是否确认删除此分类?  ");
            $('#ConfirmWindow').modal('show');            
            $("#ConfirmWindow  button.btn-primary").unbind("click").bind("click", function () {
                CategoryTemplateWindow.CategoryTemplateAction("DeleteCategoryTemplate");
            });
           
        });
        $("#ClearSelect").click(function () {
            var treeObj = $.fn.zTree.getZTreeObj("categorytemplategrid");
            treeObj.cancelSelectedNode();
            $('#controltemplategrid').attr("selectCategory", "");
            $('#controltemplategrid').attr("selectCategoryType", "");
            $("#controltemplategrid").bootgrid("reload");
        });
    }
    , isValid: function () {
        var is_forms_valid = false;
        $('form').each(function () {
            is_forms_valid = $(this).valid();
            if (!is_forms_valid) { return false; }
        });
        return is_forms_valid;
    }
    , EditCategoryTemplate: function (sysId) {
        var selfCategoryTemplateModel = this.koCategoryTemplateModel;
        ko.dependentObservable(function () {
            $.getJSON($('#categorytemplategrid').attr("Singleurl"), { sysId: sysId }, function (userData) {
                selfCategoryTemplateModel.SysId(userData.SysId);
                selfCategoryTemplateModel.ParentId(userData.ParentId);                                        
                selfCategoryTemplateModel.CategoryName(userData.CategoryName);
            });
        }, selfCategoryTemplateModel);
        $(".alert-danger").hide();
        $('#CategoryTemplateWindow').modal('show');
        $("#CategoryTemplateWindow  button.btn-primary").unbind("click").bind("click", function () {
            CategoryTemplateWindow.CategoryTemplateAction("EditCategoryTemplate");
        });
    }
    , CategoryTemplateAction: function (action) {
        if (!CategoryTemplateWindow.isValid()) {
            return false;
        }
        var zTree = $.fn.zTree.getZTreeObj("categorytemplategrid");
        var selectedNode = zTree.getSelectedNodes();
        if (action == "AddCategoryTemplate" && selectedNode.length > 0)
        {
            var selfCategoryTemplateModel = this.koCategoryTemplateModel;            
            selfCategoryTemplateModel.ParentId(selectedNode[0].ID);
        }
        else if (action == "DeleteCategoryTemplate" && selectedNode.length > 0) {
            var selfCategoryTemplateModel = this.koCategoryTemplateModel;
            selfCategoryTemplateModel.SysId(selectedNode[0].ID);
        }

        //console.log(this.koCategoryTemplateModel);
        $postJSON($("#CategoryTemplateWindow").attr("url") + action, KStarForm.getJSON(this.koCategoryTemplateModel), function (data) {        
            if (action == "AddCategoryTemplate") {
                if (selectedNode.length > 0) {
                    zTree.addNodes(selectedNode[0], { ID: data.ID, ParentID: selectedNode[0].ID, IsParent: false, NodeName: CategoryTemplateWindow.koCategoryTemplateModel.CategoryName(), Type: "Category" })
                }
                else {
                    zTree.addNodes(null, { ID: data.ID, ParentID: null, IsParent: false, NodeName: CategoryTemplateWindow.koCategoryTemplateModel.CategoryName(), Type: "Category" })
                }
            }
            else if (action == "EditCategoryTemplate") {
                if (selectedNode.length > 0)
                {
                    selectedNode[0].NodeName = CategoryTemplateWindow.koCategoryTemplateModel.CategoryName();                    
                    zTree.updateNode(selectedNode[0]);
                }                
            }
            else if (action == "DeleteCategoryTemplate")
            {
                if (selectedNode.length > 0) {                    
                    zTree.removeNode(selectedNode[0]);
                    $('#controltemplategrid').attr("selectCategory", "");
                    $('#controltemplategrid').attr("selectCategoryType", "");
                    $("#controltemplategrid").bootgrid("reload");
                    $('#ConfirmWindow').modal('hide');
                }
            }
            $('#CategoryTemplateWindow').modal('hide');
        }, null);
    }




}
