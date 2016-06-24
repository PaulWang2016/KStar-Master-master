var ControlTemplateWindow = {};

ControlTemplateWindow = {
    $formControlTemplateContainer: $('div[role=ControlTemplateWindow]'),
    ControlTemplateModel: {},
    initGrid: function () {
        var $table = $('#controltemplategrid');

        $table.bootgrid({
            ajax: true,
            navigation: 3,
            rowSelect: true,
            keepSelection: true,
            selection: true,
            post: function () {                
                return {
                    categoryid: $table.attr("selectCategory"),
                    categorytype: $table.attr("selectCategoryType")
                };
            },
            url: $table.attr("url"),
            formatters: {                
                "Options": function (column, row) {
                    return "<div class='btn-group'><button type='button' class='btn btn-default' onclick='ControlTemplateWindow.EditControlTemplate(\"" + row.SysId + "\")'><span class='glyphicon  glyphicon-pencil'>编辑</span></button><button type='button' class='btn btn-default' onclick='ControlTemplateWindow.DeleteControlTemplate(\"" + row.SysId + "\")'><span class='glyphicon  glyphicon-remove'>删除</span></button></div>";
                },
                "Checkbox": function (column, row) {
                    return "<input type='checkbox'  " + (row[column.id] ? "checked" : "") + " style='vertical-align:bottom;' disabled/>";
                },
                "Html": function (column, row) {                                     
                    var html = row[column.id];                    
                    if (html != null && html != undefined && html.length>0)
                    {                        
                        html = html.substr(0, 5);                        
                    }
                    return html + "...";
                }
            }
        }).on("selected.rs.jquery.bootgrid", function (e, rows) {
            var rowIds = [];
            for (var i = 0; i < rows.length; i++) {
                rowIds.push(rows[i].ActivityId);
            }
            // alert("Select: " + rowIds.join(","));
        }).on("deselected.rs.jquery.bootgrid", function (e, rows) {
            var rowIds = [];
            for (var i = 0; i < rows.length; i++) {
                rowIds.push(rows[i].ActivityId);
            }
            // alert("Deselect: " + rowIds.join(","));
        });
    }    
    , clearControlTemplateWindow: function () {
        $("#_kstarform_controltemplate_form").find("input").val("");
        $("#_kstarform_controltemplate_form").find("textarea").val("");
    }
    ,initControlTemplateModel:function()
    {
        ControlTemplateModel = {
            SysId: "00000000-0000-0000-0000-000000000000",
            DisplayName: "",
            HtmlTemplate: "",
            CategoryId: "00000000-0000-0000-0000-000000000000",
            CategoryName:""
        }
    }
    , initButtons: function () {
        ControlTemplateWindow.initControlTemplateModel();
        var setting = {
            async: {
                enable: true,
                url: $("#menuContent").attr("url"),
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
                     
                    var zTree = $.fn.zTree.getZTreeObj("categorytree"),
                    nodes = zTree.getSelectedNodes(),
                    v = "",
                    d = "";
                    nodes.sort(function compare(a, b) { return a.id - b.id; });
                    for (var i = 0, l = nodes.length; i < l; i++) {
                        v += nodes[i].NodeName + ",";
                        d += nodes[i].ID + ",";
                    }                    
                    if (v.length > 0) v = v.substring(0, v.length - 1);
                    if (d.length > 0) d = d.substring(0, d.length - 1);
                    var cityObj = $("#CategoryId");
                    cityObj.val(v);
                    cityObj.attr("data", d);
                }
            }
       };    
        $.fn.zTree.init($("#categorytree"), setting);

        $("#CategoryId").click(toggleMenu);

        $("#AddControlTemplate").click(function () {

            var zTree = $.fn.zTree.getZTreeObj("categorytemplategrid");
            var selectedNode = zTree.getSelectedNodes();
            if (selectedNode.length == 0 || (selectedNode.length > 0 && selectedNode[0].Type == "Template")) {
                alert("请选择一个分类");
                return;
            }
            ControlTemplateWindow.clearControlTemplateWindow();
            
            $("#CategoryId").val(selectedNode[0].NodeName);
            $("#CategoryId").attr("data", selectedNode[0].ID);

            $(".alert-danger").hide();
            $('#ControlTemplateWindow').modal('show');
            $("#ControlTemplateWindow  button.btn-primary").unbind("click").bind("click", function () {
                ControlTemplateWindow.ControlTemplateAction("AddControlTemplate");
            });
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
    , DeleteControlTemplate: function (sysId)
    {
        var selfControlTemplateModel = this.ControlTemplateModel;
        selfControlTemplateModel.SysId = sysId;

        $("#ConfirmWindow .modal-body").html("是否确认删除此模板? ");
        $('#ConfirmWindow').modal('show');
        $("#ConfirmWindow  button.btn-primary").unbind("click").bind("click", function () {
            ControlTemplateWindow.ControlTemplateAction("DeleteControlTemplate");
        });
    }
    , EditControlTemplate: function (sysId) {
        var selfControlTemplateModel = this.ControlTemplateModel;
        $.getJSON($('#controltemplategrid').attr("Singleurl"), { sysId: sysId }, function (userData) {
            selfControlTemplateModel.SysId = userData.SysId;
            $("#DisplayName").val(userData.DisplayName);
            selfControlTemplateModel.DisplayName = userData.DisplayName;
            $("#HtmlTemplate").val(userData.HtmlTemplate);
            selfControlTemplateModel.HtmlTemplate = userData.HtmlTemplate;
            $("#CategoryId").val(userData.CategoryName);
            $("#CategoryId").attr("data", userData.CategoryId);
            selfControlTemplateModel.CategoryId=userData.CategoryId;
            selfControlTemplateModel.CategoryName=userData.CategoryName;
        });
        $(".alert-danger").hide();
        $('#ControlTemplateWindow').modal('show');
        $("#ControlTemplateWindow  button.btn-primary").unbind("click").bind("click", function () {
            ControlTemplateWindow.ControlTemplateAction("EditControlTemplate");
        });
    }
    , ControlTemplateAction: function (action) {
         
        if (!ControlTemplateWindow.isValid()) {
            return false;
        }
        if (action != "DeleteControlTemplate") {
            var selfControlTemplateModel = this.ControlTemplateModel;
            selfControlTemplateModel.DisplayName = $("#DisplayName").val();
            selfControlTemplateModel.HtmlTemplate = $("#HtmlTemplate").val();
            selfControlTemplateModel.CategoryId = $("#CategoryId").attr("data");
        }
        $postJSON($("#ControlTemplateWindow").attr("url") + action, JSON.stringify(this.ControlTemplateModel), function () {
            $("#controltemplategrid").bootgrid("reload");
            $('#ControlTemplateWindow').modal('hide');
        }, null);
    }




}
