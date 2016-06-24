var userpickFillValue = function (jsonData, e) {
    target = $("#" + e.target.data("target"));
    if (jsonData == null || jsonData.length <= 0) return;
    //类型
    var controltype = e.target.data("controltype");
    if (controltype == null || $.trim(controltype) == "") return;
    //选择控件类型数组
    controltype = controltype.split(",");
    //绑定字符串
    var bindFieldString = target.data('bind');
    var bindFieldEntityString = "";
    var bindFieldArray = bindFieldString.split(",");
    $.each(bindFieldArray, function (inde, item) {
        if (item.indexOf("value") >= 0) {
            bindFieldString = item.replace("value", "");
            bindFieldString = bindFieldString.replace(":", "");
            bindFieldString = $.trim(bindFieldString);
            bindFieldEntityString = bindFieldString.substring(0, bindFieldString.lastIndexOf('.'));
        }
    });
   
    var userDataJson = { "Value": '', "Name": '', "UserName": '', "Types": '' }
    $.each(jsonData.Root, function (name, arrayJson) { 
        $.each(arrayJson.Item, function (index, item) {
            if (userDataJson.Name != "") {
                userDataJson.Name = userDataJson.Name + ',' + item.Name;
                userDataJson.UserName = userDataJson.UserName + ',' + (item.UserName || "undefing");
                userDataJson.Value = userDataJson.Value + ',' + item.Value;
                userDataJson.Types = userDataJson.Types + ',' + name;
            } else {
                userDataJson.Name = item.Name;
                userDataJson.UserName = (item.UserName || "undefined");
                userDataJson.Value = item.Value;
                userDataJson.Types = name;
                
            } 
        });
    }); 
    //var koModel = Workflow.utils.koModel.koContentModel.find(bindFieldEntityString);
    var koModel = eval("KStarForm.koContentModel." + bindFieldEntityString);
    var newKomodel = KStarForm.toKoModel(userDataJson);//转换后的komodel
    if (koModel == null || koModel == undefined) return;
    if (typeof koModel == "object") {
        koModel = newKomodel;
    }
    if (typeof koModel == "function") {
        koModel(newKomodel);
    }
    $(target).valid();//验证元素是否必填的js 脚本
}
 
//基础数据过滤条件
var BasisDataFilter = {
    GetInstance: function () {
        var entity = {
            Field: '',
            Value: '',
            Left: false,//type  bool
            Right: false,// type  bool
            Operate: '',//type    = ,<,>,<=,>=,like
            AndJoin:true  
            } 
        return entity;
    }
 
};

//单据状态
var EnumWorkMode = {
    View: "View",
    Draft: "Draft",
    Startup: "Startup",
    Approval: "Approval",
    Review: "Review"
}

//但是导出csv 格式的话， 使用Excel 打开会发现中文是乱码，但是用其他文本程序打开确是正常的。原因就是少了一个 BOM头 。  \ufeff。
var Download = {
    csvExport: function (jsonList, noShowField, fileName) {

        if (jsonList == null || jsonList.length <= 0) return;

        noShowField == noShowField || new Array();
        var data = "";
        // Column
        for (attr in jsonList[0]) {
            if (!($.inArray(attr, noShowField) >= 0)) {
                if (data == "") {
                    data = "\"" + attr + "\"";
                } else {
                    data += ",\"" + attr + "\"";
                }
            }
        }
        data += "\r\n";

        //fill Data
        $.each(jsonList, function (index, item) {

            $.each(item, function (attr, value) {
                if (!($.inArray(attr, noShowField) >= 0)) {
                    data += "\"" + (value + "").replace("\"", "\"\"") + "\",";
                }
            });

            data = data.substr(0, data.length - 1);
            data += "\r\n";
        });

        if ((!window.Blob) == false && (!window.navigator.msSaveOrOpenBlob) == false) {
            data = '\ufeff' + data;//Check BOM
            var blobObject = new Blob([data], { type: "text/csv;charset=UTF-8" });
            window.navigator.msSaveBlob(blobObject, fileName + ".csv");
        } else {
            data = '\ufeff' + data;
            var dataBlob = new Blob([data], { type: "text/csv;charset=Unicode" });
            var downloadUrl = window.webkitURL.createObjectURL(dataBlob);
            var anchor = document.createElement("a");
            anchor.href = downloadUrl;
            anchor.download = fileName + ".csv";
            anchor.click();
            window.URL.revokeObjectURL(data);
        }
    }
}
var CurrentWeekNumber = function(dateTime,weekStart) { // weekStart：每周开始于周几：周日：0，周一：1，周二：2 ...，默认为周日
        weekStart = (weekStart || 0) - 0;
        if(isNaN(weekStart) || weekStart > 6)
            weekStart = 0;	
        var currentDate = null;
        if (dateTime != undefined) {
            currentDate = new Date(dateTime);
        }
        var year = currentDate.getFullYear();
        var firstDay = new Date(year, 0, 1);
        var firstWeekDays = 7 - firstDay.getDay() + weekStart;
        var dayOfYear = (((new Date(year, currentDate.getMonth(), currentDate.getDate())) - firstDay) / (24 * 3600 * 1000)) + 1;
        return Math.ceil((dayOfYear - firstWeekDays) / 7) + 1;
    }