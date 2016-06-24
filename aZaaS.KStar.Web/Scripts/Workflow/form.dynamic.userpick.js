 
var initDynamicSelectPersonControl = function () {
	$(document).delegate("*[data-control='dynamicUserpick']", "click", function () {
		var that = $(this);
		//属性绑定事件
		var disable = that.attr("disabled");
		if (disable) {
			return false;
		}; 
		var datacontroltype = that.attr("data-controltype");
		var datacallback = that.attr("data-callback");
		var options = that.attr("data-options");
		var koModel = that.attr("data-koModel");
		if (options != undefined && options.length > 0) { options = eval('(' + options + ')'); }
		var callback = null;
		var mutilselect = true;
		if (datacontroltype == undefined) {
			datacontroltype = "Person";
		} 
		callback = function (json, e) {
			var result = new Array();
			var users = json.Root.Users.Item;
			var depts = json.Root.Depts.Item;
			var positions = json.Root.Positions.Item;
			var systemRoles = json.Root.SystemRoles.Item;
			var customRoles = json.Root.CustomRoles.Item;
	 
			if (users.length > 0) {
				//$target.data("data-users", users);
				$.each(users, function (i, item) {
					result.push({ Value: item.Value, Name: item.Name, UserName: item.UserName, Type: "Users" });
				});
			}
			if (depts.length > 0) {
				//$target.data("data-depts", depts);
				$.each(depts, function (i, item) {
					result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "Depts" });
				});
			}
			if (positions.length > 0) {
				//$target.data("data-positions", positions);
				$.each(positions, function (i, item) {
					result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "Positions" });
				});
			}
			if (systemRoles.length > 0) {
				//$target.data("data-systemRoles", systemRoles);
				$.each(systemRoles, function (i, item) {
					result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "SystemRoles" });
				});
			}
			if (customRoles.length > 0) {
				//$target.data("data-customRoles", customRoles);
				$.each(customRoles, function (i, item) {
					result.push({ Value: item.Value, Name: item.Name, UserName: "", Type: "CustomRoles" });
				});
			} 
			if (datacallback != undefined) {
				try {
					var obj = eval(datacallback);
					if (typeof obj == "function") {
						obj(json, e);
					}
				}
				catch (ex) {
					console.log("未定义回调函数");
				}
			}
		};
		if (options != undefined && options.mutilselect != undefined) {
			mutilselect = options.mutilselect;
		}
		$(this).initSelectPerson({
			type: datacontroltype,
			callback: callback,
			mutilselect: mutilselect,
			attrbind: true
		});
	});

}

initDynamicSelectPersonControl();