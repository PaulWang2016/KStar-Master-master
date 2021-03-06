USE [aZaaS.KStar]
GO
INSERT [dbo].[DynamicWidget] ([ID], [Key], [DisplayName], [RazorContent], [Description], [EnabledRoles], [MenuID]) VALUES (N'5717a1d3-9cc4-4cd1-b67a-2ce2add3157b', N'Dashboard', N'Dashboard', N'
<script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Dashboard_Parts_DashboardView"></script>
<div class="section" id="workListSection">
</div>
<div class="section">
    <div class="sectionGrid" id="myRequestTask"></div>
</div>
<div class="section">
    <div class="sectionGrid" id="onGoingTask"></div>
</div>
<!--
<div class="section">
    <div class="sectionGrid" id="completedTask"></div>
</div>
-->
<script type="text/javascript">
    var isInited = false;
    title = "Portal Site Page - Kendo UI";

    $.getJSON("/Dashboard/PendingTasks/Group", {
        "_t": new Date(),
		"page":1,
        "pageSize":5
    },
    function (json) {
var items=json.result;
var totals=json.totals;
        if (items.length > 0) {
            if (items[0].Key) {
var headhtml="";
headhtml+="<div class=\"top-heading\"><div class=\"top-title\" id=\"workListTitle\"></div></div>";
headhtml+="<div class=\"top-heading\">";
headhtml+="          <div class=\"k-block\" style=\"overflow: hidden;\">              ";
headhtml+="              <div class=\"toolbar\" style=\"min-width:900px;\">";
headhtml+="                    <label class=\"category-label\" for=\"Folio\"  style=\"width: 15%;\">"+jsResxDashboard_Parts_DashboardView.Folio+":</label>";
headhtml+="                    <input type=\"text\" id=\"Folio\"   class=\"k-textbox\"  style=\"width: 30%; margin-right:25px;\"/>";
headhtml+="                    <label class=\"category-label\" for=\"Originator\" style=\"width: 15%;\">"+jsResxDashboard_Parts_DashboardView.Originator+":</label>                    ";
headhtml+="                    <input type=\"text\" id=\"Originator\"   class=\"k-textbox\"   style=\"width: 30%; margin-right:20px;\" />";
headhtml+="              </div>";
headhtml+="              <div class=\"toolbar\" style=\"min-width:900px;margin-top:2px;\">";
headhtml+="                    <label class=\"category-label\" for=\"ProcStartDate\" style=\"width: 15%;\"> "+jsResxDashboard_Parts_DashboardView.ProcStartDate+":</label>";
headhtml+="                    <input type=\"text\" id=\"ProcStartDate\" name=\"ProcStartDate\" style=\"width: 15%; \" />";
headhtml+="                    <input type=\"text\" id=\"ProcEndDate\"  name=\"ProcEndDate\" style=\"width: 15%;margin-right:20px;\" />";
headhtml+="                    <label class=\"Process-label\" for=\"ProcessName\" style=\"width: 15%;\">"+jsResxDashboard_Parts_DashboardView.ProcessName+":</label>";
headhtml+="                    <input type=\"text\" id=\"ProcessName\" style=\"width: 30%; margin-right:50px;margin-top: 2px;vertical-align:middle\" data-placeholder=\""+jsResxDashboard_Parts_DashboardView.ProcessNamePlaceholder+"\"/>";
headhtml+="              </div>";
headhtml+="              <div class=\"toolbar\" style=\"min-width:900px;margin-top: 4px;\">";
headhtml+="                    <label class=\"category-label\"style=\"width: 60%;\"></label>";
headhtml+="                    <span  style=\"margin-left:35px;width:30%;display: -moz-inline-box;display: inline-block;overflow: hidden;height: 27px;\">";
headhtml+="                     <input type=''checkbox'' id=''showfullprocess'' style=''vertical-align: top;'' onchange=''ShowAllProcess();''>";
headhtml+="                    <label  for=''showfullprocess'' style=''cursor:pointer;width: 100px;overflow: hidden;height: 15px;''>"+jsResxDashboard_Parts_DashboardView.ShowAllProcess+"</label>";
headhtml+="                    <button id=\"SearchProcess\" class=\"k-button operacontent\" style=\"width: 75px; float: right;\">"+jsResxDashboard_Parts_DashboardView.Search+"</button>";
headhtml+="                    </span>";
headhtml+="              </div>";

headhtml+="          </div>";
headhtml+="</div>";
headhtml+="<div id=\"workList\"><ul></ul></div>";

              $("#workListSection").append(headhtml);
              $("#workListTitle").text(jsResxDashboard_Parts_DashboardView.MyPendingTasks);
              $("#ProcStartDate").kendoDatePicker();
              $("#ProcEndDate").kendoDatePicker();
              $("#SearchProcess").click(function(){SearchProcess();});              
                $(items).each(function (i, data) {

                  var dataKey= ''grid_''+i ;//data.Key.replace(new RegExp(" ","g"),"");
                  var dataprocessname=(data.Value.length>0?data.Value[0].ProcessName:data.Key);
                   if ( i == 0){
                    $("#workList").children("ul").append(''<li  class="k-state-active"><a style="font-weight:bold;" href="#">'' + (dataprocessname.length>0?dataprocessname:data.Key)  + ''</a><div class="sectionGrid" id="workList_'' + dataKey+ ''"></div></li>'');
                  }else{
                    $("#workList").children("ul").append(''<li><a style="font-weight:bold;" href="#">'' + (dataprocessname.length>0?dataprocessname:data.Key)  + ''</a><div class="sectionGrid" id="workList_'' + dataKey + ''"></div></li>'');
                  }                
                    var workInfo = getWorkInfo(data.Value);
var queryMap = {
          folio:'''',
          originator:'''',
          processname:data.Key,
          startdate:'''',
          enddate:'''',
      };
                    InitServerQueryPendingTaskKendoExcelGrid("workList_" + dataKey, workInfo.model, workInfo.columns,"/Dashboard/PendingTasks/Get", data.Value,parseInt(totals[data.Key]) ,$(window).height() - fullwidgetH, data.Key,queryMap ,null,
                          function () {
                              bindAndLoad("workList_" + dataKey); 
InitHistoryTooltip("workList_" + dataKey);
                          })
                });
                $("#workList").children().kendoPanelBar();
                InitProcessName("initialized");         
              // $("#ProcEndDate").parent().parent().css("margin-left","128px");
            }
            else {
                $("#workListSection").append(''<div class="sectionGrid" id="workList"></div>'');
                var workInfo = getWorkInfo(items);
                InitKendoExcelGrid("workList", workInfo.model, workInfo.columns, items, 5, jsResxDashboard_Parts_DashboardView.MyPendingTasks,
                function () {
                    bindAndLoad("workList")
                })
            }
        }
        else {
            $("#workListSection").append(''<div class="sectionGrid" id="workList"></div>'');
            InitKendoExcelGrid("workList", TaskModel, workcolumns, items, 5, jsResxDashboard_Parts_DashboardView.MyPendingTasks,
            function () {
                bindAndLoad("workList");
            })
        }
    });
    GetRequestTasksByPage(); 
    GetOnGoingTasksByPage();
    //GetCompletedTasks();
   // var processTimerId=Timer("SearchProcess(true)",300000);
  //  var requestTimerId=Timer(GetRequestTasks,300000);
   // var ongoingTimerId=Timer(GetOnGoingTasks,300000);
   //var completeTimerId=Timer(GetCompletedTasks,300000);
   //$("#scroller").data("processTimerId",processTimerId);
   //$("#scroller").data("requestTimerId",requestTimerId);
   //$("#scroller").data("ongoingTimerId",ongoingTimerId);
   //$("#scroller").data("completeTimerId",completeTimerId);
   
function GetRequestTasksByPage()
{    
	var parameterdata={
        startDate: null,
        endDate: null,
        folio:"",
        processName:""
    }
   InitServerQueryKendoExcelGrid("myRequestTask", TaskModel, requestcolumns, "/Dashboard/RequestTasks/Find",parameterdata,$(window).height() - fullwidgetH,jsResxDashboard_Parts_DashboardView.MyRequestTasks,
	function () {
		  bindAndLoad("myRequestTask");
		  InitHistoryTooltip("myRequestTask")     
	});  
}
   
function GetRequestTasks()
{
    $.getJSON("/Dashboard/RequestTasks/Get", {
        "_t": new Date()
    },
    function (items) {
        InitKendoExcelGrid("myRequestTask", TaskModel, requestcolumns, items, 5, jsResxDashboard_Parts_DashboardView.MyRequestTasks,
        function () {
            bindAndLoad("myRequestTask");
            InitHistoryTooltip("myRequestTask")
        })
    });
}


function GetOnGoingTasksByPage()
{    
	var parameterdata={
        startDate: null,
        endDate: null,
        folio:"",
        processName:""
    }
   InitServerQueryKendoExcelGrid("onGoingTask", TaskModel, onGoingcolumns, "/Dashboard/OnGoingTasks/Find",parameterdata,$(window).height() - fullwidgetH,jsResxDashboard_Parts_DashboardView.OnGoingTasks,
	function () {
		  bindAndLoad("onGoingTask");
		  InitHistoryTooltip("onGoingTask")     
	});  
}

function GetOnGoingTasks()
{
    $.getJSON("/Dashboard/OnGoingTasks/Get", {
        "_t": new Date()
    },
    function (items) {
        InitKendoExcelGrid("onGoingTask", TaskModel, onGoingcolumns, items, 5, jsResxDashboard_Parts_DashboardView.OnGoingTasks,
        function () {
            bindAndLoad("onGoingTask");
            InitHistoryTooltip("onGoingTask")
        })
    });
}
function GetCompletedTasks()
{
    $.getJSON("/Dashboard/CompletedTasks/Get", {
        "_t": new Date()
    },
    function (items) {
        InitKendoExcelGrid("completedTask", TaskModel, completedcolumns, items, 5, jsResxDashboard_Parts_DashboardView.CompletedTasks,
        function () {
            bindAndLoad("completedTask");
            InitHistoryTooltip("completedTask")
        })
    })
}
function ShowAllProcess()
{
        var checked=$("#showfullprocess").prop("checked");
        if(checked)
       {
              InitProcessName("");
       }
       else{
              InitProcessName("initialized");
        }
}
function InitProcessName(type)
{
      $.getJSON("/Dashboard/PendingTasks/GetProcessNameList", {
        		"_t": new Date(),
                        "type":type
		    },function(items){
                       var processname=$("#ProcessName").data("kendoMultiSelect");
                       if(!processname)
                        {
			        $("#ProcessName").kendoMultiSelect({
            			        dataTextField: "DisplayName",
            			       dataValueField: "FullName",
            			       dataSource: items
        		         });
                       }
                       else
                       {
                              processname.setDataSource(items);     
                       }
                       $("#ProcessName").parent().css("display","inline-block");
        });
}

   function SearchProcess(istimer)
  {

        if(istimer==undefined)
       {
           showOperaMask();
       }
       $("#workList ul").empty();
var queryMap = {
          _t: new Date(),
          folio:$("#Folio").val(),
          originator:$("#Originator").val(),
          processname:($("#ProcessName").data("kendoMultiSelect").value()==null?"":$("#ProcessName").data("kendoMultiSelect").value().join(",")),
          startdate:$("#workListSection").find("input[name=ProcStartDate]").val(),
          enddate:$("#workListSection").find("input[name=ProcEndDate]").val(),
      };
        $.getJSON("/Dashboard/PendingTasks/Group", queryMap ,
        function (json) {
       if(istimer==undefined)
       {
         hideOperaMask();
        }
var items=json.result;
var totals=json.totals;
           if (items.length > 0) {
                      if (items[0].Key) {
                                $(items).each(function (i, data) {
                                            var dataKey=data.Key.replace(new RegExp(" ","g"),"");
                  var dataprocessname=(data.Value.length>0?data.Value[0].ProcessName:data.Key);
                   if ( i == 0){
                    $("#workList").children("ul").append(''<li  class="k-state-active"><a style="font-weight:bold;" href="#">'' + (dataprocessname.length>0?dataprocessname:data.Key)  + ''</a><div class="sectionGrid" id="workList_'' + dataKey+ ''"></div></li>'');
                  }else{
                    $("#workList").children("ul").append(''<li><a style="font-weight:bold;" href="#">'' + (dataprocessname.length>0?dataprocessname:data.Key)  + ''</a><div class="sectionGrid" id="workList_'' + dataKey + ''"></div></li>'');
                  }  		     
                                          var workInfo = getWorkInfo(data.Value);
                    InitServerQueryPendingTaskKendoExcelGrid("workList_" + dataKey, workInfo.model, workInfo.columns,"/Dashboard/PendingTasks/Get", data.Value,parseInt(totals[data.Key]) ,$(window).height() - fullwidgetH, data.Key,queryMap ,null,
                          function () {
                              bindAndLoad("workList_" + dataKey); 
InitHistoryTooltip("workList_" + dataKey);
                          });
                                      });
                            $("#workList").children().kendoPanelBar();    
                      }
                     else {
                          //var workInfo = getWorkInfo(items);
                         // InitKendoExcelGrid("workList", workInfo.model, workInfo.columns, items, 5, jsResxDashboard_Parts_DashboardView.MyPendingTasks,
                           //function () {
                                //bindAndLoad("workList");
                         // })
                     }
           }
           else {   
           // InitKendoExcelGrid("workList", TaskModel, workcolumns, items, 5, jsResxDashboard_Parts_DashboardView.MyPendingTasks,
           // function () {
                //bindAndLoad("workList");
           // })
        }
     });
  }

</script>', NULL, N'', N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c')
INSERT [dbo].[DynamicWidget] ([ID], [Key], [DisplayName], [RazorContent], [Description], [EnabledRoles], [MenuID]) VALUES (N'0a5b6ca3-b41d-42e8-8ad2-2f5d76b66065', N'PendingTasks', N'PendingTasks', N' <!--pending_task-->
    <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Dashboard_Parts_PendingTasks"></script>
 <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Columns"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=baseInitView" type="text/javascript"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=kendoExcelGrid" type="text/javascript"></script>
<div class="section"><div class="selectbar1"><span>
		<span key="StartDate"></span>:<span key="From"></span>
		</span>
		<span>
			<input class="datepicker" name="StartDate" type="text" /></span>
		<span key="To"></span>
		<span>
			<input class="datepicker" name="EndDate" type="text" /></span>
                 <span key="Folio"></span>
		<span>
			<input class="k-textbox" name="Folio" type="text" /></span>
		<span key="ProcessName"></span>
		<span>
			<input class="k-textbox" name="ProcessName" type="text" /></span>
		<span>
			<input class="k-button selectbtn" name="select" type="button" key="Select" value="select" /></span>
	</div><div class="sectionGrid" id="workList"></div></div><!--/.pending_task-->
<script type="text/javascript">
	title = "Pending Tasks - Kendo UI  11111";
	$("#workList").prev().find(".selectbtn").click(function () {
                GetPendingTask();
        }).click();
//var processTimerId=Timer(GetPendingTask,300000);
//$("#scroller").data("processTimerId",processTimerId);
function pendingurl() {        
         var startdp = $("#workList").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#workList").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = startdp == null ? null : startdp.value();
    var endDate = enddp == null ? null : enddp.value();
    var folio=$("#workList").prev().find("input[name=Folio]").val();
    var processName=$("#workList").prev().find("input[name=ProcessName]").val();
var  parameterdata={
        start: startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate(),
        end: endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate(),
        folio:folio,
	processName:processName
    };
        return "/Dashboard/PendingTasks/GetPendingTaskForExcel?" + SerializeJsonObject(parameterdata);
  }
function GetPendingTask()
{  
try{
    var startdp = $("#workList").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#workList").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = startdp == null ? null : startdp.value();
    var endDate = enddp == null ? null : enddp.value();
    var folio=$("#workList").prev().find("input[name=Folio]").val();
    var processName=$("#workList").prev().find("input[name=ProcessName]").val();
var  parameterdata={
        start: startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate(),
        end: endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate(),
        folio:folio,
	processName:processName
    };
 var workInfo = getWorkInfo(new Array());
      var title= jsResxDashboard_Parts_PendingTasks.MyPendingTasks;    
        InitServerQueryKendoExcelGrid("workList", workInfo.model, workInfo.columns, "/Dashboard/PendingTasks/Find",parameterdata,$(window).height() - fullwidgetH,title,pendingurl,
        function () {
            bindAndLoad("workList");InitHistoryTooltip("workList");             
        },
       function(){
            $("#workList .k-grid-content").find("tr").each(function () {
                 if($(this).find("span.k-grid-status").length>0)
                  {
                      $(this).css("background-color","#96CDCD");
                   }
            })
      }
);   
}
catch(e){}
}
$("span").each(function (item) {
                var key = $(this).attr("key");
                $(this).html(jsResxDashboard_Parts_PendingTasks[key]);
            });
$("input").each(function (item) {
                var key = $(this).attr("key");
                $(this).val(jsResxDashboard_Parts_PendingTasks[key]);
            });
</script>', NULL, N'', N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c')
INSERT [dbo].[DynamicWidget] ([ID], [Key], [DisplayName], [RazorContent], [Description], [EnabledRoles], [MenuID]) VALUES (N'7572f9c5-a3d0-4773-bc4a-3026afb4144d', N'RequestTasks', N'RequestTasks', N'<!--request_task-->
 <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Columns"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=baseInitView" type="text/javascript"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=kendoExcelGrid" type="text/javascript"></script>
 <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Dashboard_Parts_RequestTasks"></script>
<script src="/JS/baseInitView.js?v=20140709"></script>
    <script src="/JS/Filters.js?v=20140709"></script>
    <script src="/JS/Columns.js?v=20140709"></script>
    <script src="/JS/models.js?v=20140709"></script>
<div class="section"><div class="selectbar1">
<span  key="StartDate"></span>:
  <span key="From"></span>
		<span>
			<input  class="datepicker" name="StartDate" type="text" /></span>
		<span key="To"></span>
		<span>
			<input  class="datepicker" name="EndDate" type="text" /></span>
  <span key="Folio"></span>
		<span>
			<input class="k-textbox" name="Folio" type="text" /></span>
  <span key="ProcessName"></span>
		<span>
			<input class="k-textbox" name="ProcessName" type="text" /></span>
		<span>
			<input class="k-button selectbtn" name="select" type="button" key="Select" value="select" /></span>
 
	</div>
<div class="sectionGrid" id="myRequestTask"></div></div><!--/.request_task-->
<script type="text/javascript">
	title = "Request Tasks - Kendo UI";
	$("#myRequestTask").prev().find(".selectbtn").click(function() {
                 GetRequestTask();
	}).click();
//var requestTimerId=Timer(GetRequestTask,300000);
//$("#scroller").data("requestTimerId",requestTimerId);
function  requesturl() {        
         var startdp = $("#myRequestTask").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#myRequestTask").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = (startdp == null ? null : startdp.value());
    var endDate = (enddp == null ? null : enddp.value());
    var folio=$("#myRequestTask").prev().find("input[name=Folio]").val();
    var processName=$("#myRequestTask").prev().find("input[name=ProcessName]").val();
   var parameterdata={
        startDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
        endDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
        folio:folio,
        processName:processName
    }
        return "/Dashboard/RequestTasks/GetRequestTasksForExcel?" + SerializeJsonObject(parameterdata);
    }
function GetRequestTask()
{
    var startdp = $("#myRequestTask").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#myRequestTask").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = (startdp == null ? null : startdp.value());
    var endDate = (enddp == null ? null : enddp.value());
    var folio=$("#myRequestTask").prev().find("input[name=Folio]").val();
    var processName=$("#myRequestTask").prev().find("input[name=ProcessName]").val();
   var parameterdata={
        startDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
        endDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
        folio:folio,
        processName:processName
    }
   InitServerQueryKendoExcelGrid("myRequestTask", TaskModel, requestcolumns, "/Dashboard/RequestTasks/Find",parameterdata,$(window).height() - fullwidgetH,jsResxDashboard_Parts_RequestTasks.MyRequestTasks,requesturl,
        function () {
              bindAndLoad("myRequestTask");
            InitHistoryTooltip("myRequestTask")     
        });      
}
  $("span").each(function (item) {
                var key = $(this).attr("key");
                $(this).html(jsResxDashboard_Parts_RequestTasks[key]);
            });
$("input").each(function (item) {
                var key = $(this).attr("key");
                $(this).val(jsResxDashboard_Parts_RequestTasks[key]);
            });

</script>', NULL, N'', N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c')
INSERT [dbo].[DynamicWidget] ([ID], [Key], [DisplayName], [RazorContent], [Description], [EnabledRoles], [MenuID]) VALUES (N'009bc79b-3346-4010-b24a-3493b5681177', N'OnGoingTasks', N'OnGoingTasks', N'<!--On-Going_Tasks-->
 <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Columns"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=baseInitView" type="text/javascript"></script>
<script src="/JSResource/GetJavascriptResx?jsPageName=kendoExcelGrid" type="text/javascript"></script>
    <script type="text/javascript" src="/JSResource/GetJavaScriptResx?jsPageName=Dashboard_Parts_OnGoingTasks"></script>
   <script src="/JS/Columns.js?v=20140709"></script>
<div class="section"><div class="selectbar1">
<span key="StartDate"></span>:
 <span key="From"></span>
		<span>
			<input class="datepicker" name="StartDate" type="text" /></span>
		<span  key="To"></span>
		<span>
			<input class="datepicker" name="EndDate" type="text" /></span>
                <span key="Folio"></span>
		<span>
			<input class="k-textbox" name="Folio" type="text" /></span>
               <span key="ProcessName"></span>
		<span>
			<input class="k-textbox" name="ProcessName" type="text" /></span>
		<span>
			<input class="k-button selectbtn" name="select" type="button" key="Select" value="select" /></span>
	</div><div class="sectionGrid" id="onGoingTask"></div></div><!--/.On-Going_Tasks-->
<script type="text/javascript">
	title = "On-Going Tasks - Kendo UI";
	$("#onGoingTask").prev().find(".selectbtn").click(function() {
                 GetOnGoingTask();
	}).click();
//var ongoingTimerId=Timer(GetOnGoingTask,300000);
//$("#scroller").data("ongoingTimerId",ongoingTimerId);
function ongoingurl() {        
       var startdp = $("#onGoingTask").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#onGoingTask").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = (startdp == null ? null : startdp.value());
    var endDate = (enddp == null ? null : enddp.value());
    var folio=$("#onGoingTask").prev().find("input[name=Folio]").val();
    var processName=$("#onGoingTask").prev().find("input[name=ProcessName]").val();
var parameterdata={
        StartDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
        EndDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
        folio:folio,
        processName:processName
    };
        return "/Dashboard/OnGoingTasks/GetOnGoingTasksForExcel?" + SerializeJsonObject(parameterdata);
    }
function GetOnGoingTask()
{
    var startdp = $("#onGoingTask").prev().find("input[name=StartDate]").data("kendoDatePicker");
    var enddp = $("#onGoingTask").prev().find("input[name=EndDate]").data("kendoDatePicker");
    var startDate = (startdp == null ? null : startdp.value());
    var endDate = (enddp == null ? null : enddp.value());
    var folio=$("#onGoingTask").prev().find("input[name=Folio]").val();
    var processName=$("#onGoingTask").prev().find("input[name=ProcessName]").val();
var parameterdata={
        StartDate: (startDate == null ? null : startDate.getFullYear() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getDate()),
        EndDate: (endDate == null ? null : endDate.getFullYear() + "-" + (endDate.getMonth() + 1) + "-" + endDate.getDate()),
        folio:folio,
        processName:processName
    };
   InitServerQueryKendoExcelGrid("onGoingTask",TaskModel, onGoingcolumns, "/Dashboard/OnGoingTasks/Find",parameterdata,$(window).height() - fullwidgetH,jsResxDashboard_Parts_OnGoingTasks.OnGoingTasks,ongoingurl,
        function () {
              bindAndLoad("onGoingTask")
              InitHistoryTooltip("onGoingTask")
        });      
}
  $("span").each(function (item) {
                var key = $(this).attr("key");

                $(this).html(jsResxDashboard_Parts_OnGoingTasks[key]);
            });
$("input").each(function (item) {
                var key = $(this).attr("key");
                $(this).val(jsResxDashboard_Parts_OnGoingTasks[key]);
            });
</script>', NULL, N'', N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c')
INSERT [dbo].[DynamicWidget] ([ID], [Key], [DisplayName], [RazorContent], [Description], [EnabledRoles], [MenuID]) VALUES (N'e69fe960-69fc-1f69-4d34-3fedd3cbbb0f', N'Draft', N'Draft', N'<div class="section">
<div class="fullwidget">
<iframe src="http://minierp.cloudapp.net:8069/#page=0&limit=80&view_type=list&model=stock.warehouse&action=400" frameborder=“0” scrolling="YES"  width="100%" height="100%"></iframe>
<div>
<script>$(window).resize();</script>
</div>


<!--draft_list-->
<!--<div class="section"><div class="sectionGrid" id="myDraft"></div></div>-->
<!--/.draft_list-->
<!--
<script type="text/javascript">
	$(function() {
		title = "My Draft - Kendo UI";
		$.getJSON("/eForm/Drafts/Get", {
			"_t": new Date()
		},
		function(items) {
			InitKendoExcelGrid("myDraft", DraftModel, Draftcolumns, items, 15, "My Draft",
			function() {
				bindAndLoad("myDraft")
			})
		})
	})
</script>
-->', NULL, N'', N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c')
