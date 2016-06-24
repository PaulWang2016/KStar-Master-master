var QuickSearchAJAX, LastCallbackText, LastCallbackColumn;
// Makes the AJAX call
var _MustShowPop = false;
var _busyLoad = true;
var separatorValue = '☺'; 
var secondSeparator = '☻';
var _returnType = "";
var _clientID = "";
var SearchTextBox_TimerStarted = false;
var SearchTextBox_EventSource;
var SearchTextBox_TimeoutId;
var SearchTextBox_HasFocus = false;
var SearchTextBox_Searhing = false;
var _filterName = "";
var _filterTextboxID = "";
var _criteriaItemID = "";
var _inEdit = false;
var _oldSettings = null;
var _oldOOOSettings = null;
var _enableRefresh = false;
var _worklistAutoRefresh;
var _currentUserOOO =""; //Displays in the title of the configuration

function SetNewXML()
{
    // Get the response, checking if we have an exception included
    if ((QuickSearchAJAX != null) && (QuickSearchAJAX.response != null))
    {
        if (QuickSearchAJAX.response.indexOf("[pageexceptionthrown]") == -1)
        {
            try
            {
            document.all("ViewFlow").Xml = QuickSearchAJAX.response;
            }
            catch(ex)
            {
            
            }
        }
        else
        {
            ShowError(QuickSearchAJAX.response);
        }   
        QuickSearchAJAX = null;     
    }
    window.parent.$id("imgRefresh").style.display = "";
    window.parent.$id("imgRefreshing").style.display = "none";
}

function ChangeFilterItem(context) {

    $id(context + "_txtStartRow").value = 1;
    GenericAjax(null, context, null);
}

function ChangeRefreshStatus()
{
    $id("ViewFlowContainer").contentWindow.UpdateRefreshStatus($id("chkAutoRefresh").checked);
}

function ViewFlow_Init()
{
    setTimeout("UpdateViewFlow()", 10000);
}

function UpdateRefreshStatus(Status)
{
    _enableRefresh = Status;
}

function UpdateViewFlow()
{
    if (_enableRefresh)
    {
     
        QuickSearchAJAX = new sack();

        // Setup the page used for the AJAX call
        QuickSearchAJAX.requestFile = "ViewFlow.aspx";

        // Setup the parameters
        QuickSearchAJAX.queryString = "Refresh~true|K2Server~" + $id("txtK2Server").value + "|ProcessID~" + $id("txtProcessID").value;

        // Add an event handler
        QuickSearchAJAX.onCompletion = SetNewXML;

        // Run the ajax
        QuickSearchAJAX.runAJAX();
        
        setTimeout("UpdateViewFlow()", 10000);
    }
    else
    {
        setTimeout("UpdateViewFlow()", 10000);
    }
}

function setRefreshViewFlow()
{
    $id("ViewFlowContainer").contentWindow.RefreshViewFlow();
}

function RefreshViewFlow()
{
    window.parent.$id("imgRefresh").style.display = "none";
    window.parent.$id("imgRefreshing").style.display = "";
        
    QuickSearchAJAX = new sack();

    // Setup the page used for the AJAX call
    QuickSearchAJAX.requestFile = "ViewFlow.aspx";

    // Setup the parameters
    QuickSearchAJAX.queryString = "Refresh~true|K2Server~" + $id("txtK2Server").value + "|ProcessID~" + $id("txtProcessID").value;

    // Add an event handler
    QuickSearchAJAX.onCompletion = SetNewXML;

    // Run the ajax
    QuickSearchAJAX.runAJAX();
}

function contextMouseOver()
{
    var src = event.srcElement;
    
    src.className = "black_context_txt_hover";
}
function contextMouseOut()
{
    var src = event.srcElement;
    
    src.className = "black_context_txt";
}
    
function UpdateDisplay(value,context)
{
    SearchTextBox_Searhing = false;
    
    var xmlDoc = $xml(value);
    var data = populateParameters(xmlDoc,context);
        
    switch (_returnType)
    {
        case "Default":
            PopulateTaskListTable(data);   
            hidePWait();
            if (_worklistAutoRefresh != null)
            {
                RestartWorklistRefreah();
            }
           // setTimeout("callGetOutOfOfficeCheck('" + context + "')", 1000);
            break; 
        case "AutoRefresh":
            var paramArr=data.split('~|~');
            PopulateTaskListTable(paramArr[0]);
            //Update the Out of Office Status image
            
            var outOfficeImageTitle=paramArr[1];
            var outOfficeImagePath=paramArr[2];
            $id(_clientID+"_WaitAnimationStatus").src=outOfficeImagePath;
            $id(_clientID+"_WaitAnimationStatus").alt=outOfficeImageTitle;
            
            hidePWait();
            if (_worklistAutoRefresh != null)
            {
                RestartWorklistRefreah();
            }
            break;
        case "displayDataFields":
            SetDataFieldsResult(data);
            hidePWait();
            break;
 
             
        case "SaveSettingConfirmation":
            var paramArr=data.split('|');
            var ex = "";
            if (paramArr[2]=="Success")
            {
                setTimeout("SaveLayout('" + context + "','"+ex+"')", 1000);
                //reset old settings as wizard closed and was success
                _oldSettings=$id(_filterTextboxID).value; //filterconfig
                if (paramArr[3]!=null && paramArr[3]!="")
                {
                    $getOOOElement(_clientID+"_OutOfOfficeXml").value=paramArr[3];
                    //reset old settings as wizard closed and was success
                    _oldOOOSettings=$getOOOElement(_clientID+"_OutOfOfficeXml").value
                    fillOOOPopup(_clientID);
                }
            }
            else
            {
                ex = "[pageexceptionthrown]";
                ex += "~|~" + paramArr[3];
                ex += "~|~" + paramArr[4];
                ex += "~|~" + paramArr[5];
                ShowError(ex);
            }
            break;
        case "Confirmation":
            hidePWait();
            PopulateInfo(data);
            break;    
        case "LoadActionsForProcess":
            addActionsForProcess(data);   
            break;
        case "LoadUserActionInstanceRights":
            loadActionInstanceRights(data);   
            IM_SetStatusImages();
            break;
        case "SaveUserActionInstanceRights":
            hidePWait();
            break;  
        case "ShowError":
            hidePWait();
            var paramArr=data.split('~|~');
            var errMessage=paramArr[1].replace("\\n","<br>");;
            var errType=paramArr[2];
            if (paramArr[3] != null)
            {   
                //this must be a refresh
                PopulateTaskListTable(paramArr[3]);
            }
            else
            {
                ShowErrorMessage(errMessage,errType);
            }
            break;    
        case "Info":
            hidePWait();
            PopulateInfo(data);
            break;
        case "OOOUsersConfirmation":
            ShowOutOfOfficeDelegateConfirmation(_clientID,data);
            break;
        case "LoadOOOSettings":
            var paramArr=data.split('|');
            var ex = "";
            if (paramArr[2]=="Success")
            {
                if (paramArr[3]!=null)
                {
                    $getOOOElement(_clientID+"_OutOfOfficeXml").value=paramArr[3];
                    _oldSettings = $id(_filterTextboxID).value;
                    _oldOOOSettings= paramArr[3];
                    _OOOpopupFilled=false;
                    _currentUserOOO=paramArr[4];
                    ShowFilerConfig(_clientID,_filterTextboxID);
                    fillOOOPopup(_clientID);
                }
            }
            else
            {
                hidePWait();
                ShowErrorMessage(paramArr[4],"error");
            }
            break;
        default:
            hidePWait();
        break;
    } 
    
    //hidePWait();
    
    try
    {
        i=0;
    }
    catch(ex)
    {}    
}


function SaveLayout(ClientID,errors)
{
    GenericAjax(null,ClientID,"GenericAction~SaveLayout|GroupBy~" + GetGroupByValue(ClientID) + "|ColumnSet~" + GetColumnSetValue(ClientID) + "|CustomColumns~" + GetCustomColumnsValues(ClientID)+errors);
}

function PopulateTaskListTable(data)
{
    $id(_clientID + "_TaskListTable").innerHTML = data; 
}

function SetDataFieldsResult(data)
{
    $id(_clientID + "_DataFieldsTableContent").innerHTML = data;
    $doControlPopupDetail(JSDataFields,"","",_clientID + "_DataFieldsTable",JSOK + "☺Close_DataFields()", "X", "$doHideControlPopup('" + _clientID + "_DataFieldsTable')","");            
    ResizeMainCells(_clientID + '_HeaderID_ActivityInstance',_clientID + '_BodyID_ActivityInstance');
    ResizeMainCells(_clientID + '_HeaderID_ProcessInstance',_clientID + '_BodyID_ProcessInstance');
}

function PopulateConfirmation(data)
{
    $showPopup(JSConfirmation, data, "", 75,300, "", "ActionCompleted('" + _clientID + "')", JSOK + "☺ActionCompleted('" + _clientID + "')", 1000, "", "", "", "", "", "","info");                                                
}

function PopulateInfo(data)
{
    $showPopup(JSInformation, data, "", 75, 300, "", "ActionCompleted('" + _clientID + "')", JSOK + "☺ActionCompleted('" + _clientID + "')", 1000, "", "", "", "", "", "", "info");                                                
}

function populateParameters(xmlObj,clientID)
{
    var xpResultKeyList = $mn(xmlObj,"Info/Load/*"); 
    var xpData = $sn(xmlObj,"Info/Data"); 

    for(var ii=0; ii < xpResultKeyList.length; ii++)
    {
        switch (xpResultKeyList[ii].nodeName)
        {
            case "ReturnType":
                _returnType = xpResultKeyList[ii].text;
                break;
            case "ClientID":
                _clientID = xpResultKeyList[ii].text;
                break;
            case "StartRow":
                $id(clientID + "_txtStartRow").value = xpResultKeyList[ii].text;
                break;
            case "LastRow":
                $id(clientID + "_txtLastRow").value = xpResultKeyList[ii].text;
                break;                
        }
    }
    return xpData.text;
}

function CallbackError(err)
{
    hidePWait();
    ShowError(err);
    try
    {
        i=0;
    }
    catch(ex)
    {} 
}

function replace(string,text,by) 
{
    var strLength = string.length, txtLength = text.length;
    if ((strLength == 0) || (txtLength == 0)) return string;

    var i = string.indexOf(text);
    if ((!i) && (text != string.substring(0,txtLength))) return string;
    if (i == -1) return string;

    var newstr = string.substring(0,i) + by;

    if (i+txtLength < strLength)
        newstr += replace(string.substring(i+txtLength,strLength),text,by);

    return newstr;
}

function RelativeTime_Click(ClientID)
{
    $id(ClientID + "_RelativeTime").checked = true;
    $id(ClientID + "_SleepDuration").disabled = false;
    $id(ClientID + "_SleepDuration").style.backgroundColor = "white";
    $id(ClientID + "_SleepDate").style.backgroundColor = "whitesmoke";       
    $id(ClientID + "_SleepDate").value = "";    
    $id(ClientID + "_cmbSleepUnit").disabled = false;
    $id(ClientID + "_SleepDuration").value = "1";
    $id(ClientID + "_child_SleepDate").style.display = "none";    
}

function AbsoluteTime_Click(ClientID)
{
    $id(ClientID + "_AbsoluteTime").checked = true;
    $id(ClientID + "_SleepDuration").disabled = true;
    $id(ClientID + "_SleepDuration").style.backgroundColor = "whitesmoke";   
    $id(ClientID + "_SleepDate").style.backgroundColor = "white";   
    $id(ClientID + "_cmbSleepUnit").disabled = true;
    $id(ClientID + "_SleepDuration").value = "";
    $id(ClientID + "_child_SleepDate").style.display = "";
}

function RefreshTasks(Context)
{
    $id(Context + "_txtStartRow").value = 1;
    GenericAjax(null,Context);
}

function btnSearch_Click(ClientID)
{
    var userName = "";
       
    userName = $id(ClientID + "_txtUser").value;
        
    if (!((validateSpecialChars(_clientID + "_txtUser", "Search","_"))))
    {
        return false;
    }        
        
    if (userName != "")
    {
        //UserName
        if ($id(ClientID + "_ddUser").value == "Starts With")
        {
            userName = userName + "*";
        }
        else if ($id(ClientID + "_ddUser").value == "Contains")
        {
            userName = "*" + userName + "*";
        }
        else if ($id(ClientID + "_ddUser").value == "Ends With")
        {
            userName = "*" + userName;
        }
    }
    
    GenericAjax(null,ClientID,"GenericAction~getUsers|userName~" + userName + "|securityLabel~" + $id(_clientID +  "_ddlUserProviderRedirect").value)  
}
function btnFwdUserSearch_Click(ClientID)
{
    var userName = "";
       
    userName = $id(ClientID + "_txtFwdUser").value;
        
    if (userName != "")
    {
        //UserName
        if ($id(ClientID + "_ddFwdUser").value == "Starts With")
        {
            userName = userName + "*";
        }
        else if ($id(ClientID + "_ddFwdUser").value == "Contains")
        {
            userName = "*" + userName + "*";
        }
        else if ($id(ClientID + "_ddFwdUser").value == "Ends With")
        {
            userName = "*" + userName;
        }
    }
    
    GenericAjax(null,ClientID,"GenericAction~getFwdUsers|userName~" + userName + "|securityLabel~" + $id(_clientID +  "_ddlUserProviderOOO").value)  
}

function hidePWait(_controlID)
{
    _controlID = "TaskList1_QuickSearch";
    if (_controlID.indexOf('part_') == -1)
    {
        _controlID = 'part_' + _controlID;
    } 
   
    if ($id('controlBusyDiv_' + _controlID)) 
    {
        var controlBusyDiv = ($id('controlBusyDiv_' + _controlID));
        controlBusyDiv.parentNode.removeChild(controlBusyDiv); 
    } 
    if ($id(_controlID))
    {
        $id(_controlID).disabled = "";
    }
    else
    {
        var divt = $id("div_SAVING");
        if(divt != null)
        {
            // Port_togglePopUpMenuDisplay(divt,"hiddenframe","none");
            $togglePopUp(divt, "hiddenframe","none");
            divt.style.display = "none";
        }
        HideDisableDiv();
       _canload_Portal = true; 
   }
}

function OpenDataFields(SerialNumber,Context)
{
    GenericAjax(null,Context,"GenericAction~displayDataFields|SerialNumber~" + SerialNumber);
}

function GenericAjax(evnt,Context,Additional, showLoading)
{
    if (showLoading == null)
   { 
        showPWait("",10001);
   }
   else if (showLoading == true)
    {
        showPWait("",10001);
     }
    var txtQuickSearch = GetItemByFriendlyName("input", Context + "_txtQuickSearch");
    var txtConfigurationXml = GetItemByFriendlyName("input", Context + "_txtConfigurationXml");
    var txtSortColumn = GetItemByFriendlyName("input", Context + "_txtSortColumn");
    var txtSortType = GetItemByFriendlyName("input", Context + "_txtSortType");
    var ddlColumns = GetItemByFriendlyName("select", Context + "_ddlColumns");
    var ddlFilter = GetItemByFriendlyName("select", Context + "_ddlFilter");
    
    LastCallbackText = txtQuickSearch.value;
    LastCallbackColumn = ddlColumns.value;
    GetItemByFriendlyName("tr", Context + "_BatchRow").style.display = "none";
    
    var execString = "";
    execString += "Filter~" + encodeURIComponent(txtQuickSearch.value);
    execString += "|Configuration~" + encodeURIComponent(txtConfigurationXml.value);
    execString += "|SortColumn~" + encodeURIComponent(txtSortColumn.value);
    execString += "|Column~" + encodeURIComponent(ddlColumns.value);
    execString += "|SelectedFilter~" + encodeURIComponent(ddlFilter.value);
    execString += "|SortType~" + encodeURIComponent(txtSortType.value);
    execString += "|K2Server~" + _k2Server;
    execString += "|StartRow~" + $id(Context + "_txtStartRow").value;
    execString += "|LastRow~" + $id(Context + "_txtLastRow").value;
    execString +=  Additional != "" ? "|" + Additional : "";
    
    WebForm_DoCallback(Context,execString,UpdateDisplay,Context,CallbackError,true);
    
    txtQuickSearch = null;
    txtConfigurationXml = null;
    txtSortColumn = null;
    txtSortType = null;
    ddlColumns = null;
}

function Close_DataFields()
{
    $doHideControlPopup(_clientID + "_DataFieldsTable");
}

function Close_Sleep(ClientID)
{
    $doHideControlPopup(ClientID + "_SleepTable");
}

function OK_Redirect()
{
    var userSelectedValue = document.getElementsByName(_userPickerClientID + "_UserSelectedValue");
    var itemFound = false;
    var selectedUser = "";
    
    // Loop through all the users
    for (var i=0; i<userSelectedValue.length; i++)
    {
        // See if the current radio is checked
        if (userSelectedValue[i].checked)
        {
            itemFound = true;
            selectedUser = userSelectedValue[i].value;
            break;
        }    
    }    
    
    if (itemFound)
    {
        $doHideControlPopup(_clientID + "_UserPickerTable");  
        Reset_UserPicker(); 
        GenericAjax(null,_clientID,"GenericAction~PerformActionRedirect|SerialNumber~" + _sn +  "|Action~" + _performAction +  "|selectedUser~" + selectedUser + "|securityLabel~" + $id(_userPickerClientID +  "_ddSecLabel").value+ "|OriginalDestinations~"+_OriginalDestinations);     
    }
    else
    {
        $showPopup(JSWarning, JSSelectUser, "", 75,300, "", "", JSOK + "☺doHidePopup()", 9999, "", "", "", "", "", "","warning");
    }
}

function Close_Redirect(ClientID)
{
    $doHideControlPopup(ClientID + "_UserPickerTable");
    Reset_UserPicker(); 
}               

function OK_FwdUser(ClientID,FilterTextboxID)
{
    var userSelectedValue = document.getElementsByName(ClientID + "_UserSelectedValue");
    var itemFound = false;
    var selectedUser = "";
    
    // Loop through all the users
    for (var i=0; i<userSelectedValue.length; i++)
    {
        // See if the current radio is checked
        if (userSelectedValue[i].checked)
        {
            itemFound = true;
            selectedUser = userSelectedValue[i].value;
            break;
        }    
    }    
    
    if (itemFound)
    {
        $doHideControlPopup(ClientID + "_FwdUserTable");   
        $id(ClientID + "_OOOFwdUser").value=selectedUser.split(":")[1];
        ShowOutOfOfficeConfig(ClientID,FilterTextboxID)
    }
    else
    {
        $showPopup(JSUsernotfound, JSSelectUser, "", 75, 300, "", "ActionCompleted('" + _clientID + "')", JSOK + "☺ActionCompleted('" + ClientID + "')", 9999, "", "", "", "", "", "", "warning");
    }
}

function Close_FwdUser(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_FwdUserTable");
    ShowOutOfOfficeConfig(ClientID,FilterTextboxID)
}

function OK_Sleep(ClientID)
{
    $doHideControlPopup(ClientID + "_SleepTable");
    if ($id(ClientID + "_RelativeTime").checked)
    {
        GenericAjax(null,ClientID,"GenericAction~PerformActionSleep|SerialNumber~" + _sn +  "|Action~" + _performAction +  "|Duration~" + $id(ClientID + "_SleepDuration").value +  "|Unit~" + $id(ClientID + "_cmbSleepUnit").value+ "|OriginalDestinations~"+_OriginalDestinations); 
    }
    else
    {
        GenericAjax(null,ClientID,"GenericAction~PerformActionSleep|SerialNumber~" + _sn +  "|Action~" + _performAction +  "|SleepDate~" + $id("hid_" + ClientID + "_SleepDate").value+ "|OriginalDestinations~"+_OriginalDestinations);     
    }
}

function resize_window()
{
    try
    {
        if (_clientID != "")
        {
            $id(_clientID + "_TaskListTable").style.width = (document.documentElement.clientWidth-300) + "px";
        }
    }
    catch(ex)
    {}     
}

function RunOnloadCode(CallbackEvent,Context) 
{
    _clientID = Context;
	LastCallbackText = "";
	LastCallbackColumn = "All";
	var txtQuickSearch = GetItemByFriendlyName("input", Context + "_txtQuickSearch");
	var ddlColumns = GetItemByFriendlyName("select", Context + "_ddlColumns");

    if (GetItemByFriendlyName("input", Context + "_txtSortType") != null)
    {
	    GetItemByFriendlyName("input", Context + "_txtSortType").value = "ASC";
	}

    if (GetItemByFriendlyName("input", Context + "_txtImagesPath") != null)
    {
	    setImages(GetItemByFriendlyName("input", Context + "_txtImagesPath").value);
    }
    
    //AJAX call - to load the user data
    RefreshWorklist();
}

function ShowError(message)
{
    $showPopup("Error", message, "", 75,300, "", "", JSOK + "☺doHidePopup()", 20000, "", "", "", "", "", "","error");                                                
}

function SearchTextBox_OnBlur()
{
    SearchTextBox_HasFocus = false;
}

function SearchTextBox_SetFocus()
{
    SearchTextBox_HasFocus = true;    
}

function SearchTextBox_OnKeyUp(ClientID)
{
    SearchTextBox_HasFocus = true;
    if (SearchTextBox_TimeoutId > 0)
    {
        SearchTextBox_TimerStarted = false;
        clearTimeout(SearchTextBox_TimeoutId);
    }
        
    SearchTextBox_EventSource = event.srcElement;
                
    if (SearchTextBox_EventSource == null)
        return;                     
    
    if (SearchTextBox_Searhing == true)
        return;

    //Fire event after 1 seconds
    if (SearchTextBox_TimerStarted != true)
    {
        SearchTextBox_TimerStarted = true;
        SearchTextBox_TimeoutId = setTimeout("SearchTextBox_Timed_OnKeyUp('" + ClientID + "')", 1500);
    }
}

function SearchTextBox_Timed_OnKeyUp(ClientID)
{
    SearchTextBox_TimerStarted = false;
    
    if (SearchTextBox_HasFocus == true)
    {
        SearchTextBox_Searhing = true;
        GenericAjax(null,ClientID,"NewQS~True");
    }
    else
    {
        SearchTextBox_Searhing = false;    
    }
}

function btnSearchDelegate_Click(ClientID)
{
    var userName = "";
       
    userName = $id(ClientID + "_getUsersDelegate").value;
        
    if (userName != "")
    {
        //UserName
        if ($id(ClientID + "_ddUserDelegate").value == "Starts With")
        {
            userName = userName + "*";
        }
        else if ($id(ClientID + "_ddUserDelegate").value == "Contains")
        {
            userName = "*" + userName + "*";
        }
        else if ($id(ClientID + "_ddUserDelegate").value == "Ends With")
        {
            userName = "*" + userName;
        }
    }
    
    GenericAjax(null,ClientID,"GenericAction~getUsersDelegate|userName~" + userName + "|securityLabel~" + $id(_clientID +  "_ddlUserProviderDelegate").value);
}

function btnAddDelegateUsers_Click(ClientID)
{
    var availableUsers = ($id(ClientID + "_dgUsers"));
    if (availableUsers)
    {
        var userSelectedValue = document.getElementsByName(ClientID + "_UserSelectedValue");
        var itemFound = false;
        var currentItem = "";
        
        // Loop through all the users
        for (var i=0; i<userSelectedValue.length; i++)
        {
            // See if the current radio is checked
            if (userSelectedValue[i].checked)
            {
                itemFound = true;
                currentItem =  userSelectedValue[i].parentNode.parentNode;
                break;
            }    
        }    
        
        if (itemFound)
        {
            ($id(ClientID + "_tbUsersSelected")).childNodes[0].appendChild(currentItem);
        } 
        else
        {
            $showPopup(JSAddUser, JSSelectUserAdd, "", "70", "200", "", "doHidePopup('msgdiv')", JSOK + "☺doHidePopup('msgdiv')", "11000", "msgdiv", "", "", "", "", "", "error");
        }
    }
}

function btnRemoveDelegateUsers_Click(ClientID)
{   
    var selectedUsers = ($id(ClientID + "_dgUsersDelegateSelected"));
    if (selectedUsers)
    {
         var userSelectedValue = document.getElementsByName(ClientID + "_UserSelectedValue");
        var itemFound = false;
        var currentItem = "";
        
        // Loop through all the users
        for (var i=0; i<userSelectedValue.length; i++)
        {
            // See if the current radio is checked
            if (userSelectedValue[i].checked)
            {
                itemFound = true;
                currentItem =  userSelectedValue[i].parentNode.parentNode;
                break;
            }    
        }    
        
        if (itemFound)
        {
            ($id(ClientID + "_tbUsers")).childNodes[0].appendChild(currentItem);
        } 
        else
        {
            $showPopup(JSRemoveUser, JSSelectUserRemove, "", "70", "200", "", "doHidePopup('msgdiv')", JSOK + "☺doHidePopup('msgdiv')", "11000", "msgdiv", "", "", "", "", "", "error");
        }
    } 
}

function btnDelegateUsersAdd_Click()
{
    var selectedUsers = ($id(_userPickerClientID + "_dgUsers"));
    if (selectedUsers)
    {
      var childRows = null;
      if (selectedUsers.childNodes[0] != null)
      {
       childRows = selectedUsers.childNodes[0].childNodes[0];
      }
      
      var newTable = ($id("BodyID"+ _clientID + "_DelegateAssignment"));
      var existingUserArray  = "";
      if (newTable.childNodes.length > 0)
      {
            if (newTable.childNodes[0].childNodes.length > 0)
            {
                newTable = newTable.childNodes[0].childNodes[0];  //get into the TBODY
                 for (var i=0; i<newTable.childNodes.length; i++)
                 {
                    existingUserArray += newTable.childNodes[i].childNodes[1].innerHTML.toUpperCase() + "|";
                 }
            }
            else
            {
                newTable.removeChild(newTable.firstChild);
                var Table = document.createElement("TABLE");
                var TBody = document.createElement("TBODY");
               Table.appendChild(TBody);
               newTable.appendChild(Table);  
               Table.style.width = "100%";
               Table.cellPadding = "0";
               Table.cellSpacing = "0";
               newTable = Table;
            }
      }
      
      if (childRows != null)
      {
          if (childRows.childNodes.length > 0)
          {
            var currentTable = ($id("BodyID"+ _clientID + "_DelegateAssignment"));
            var currentCounter = currentTable.childNodes[0].childNodes[0].childNodes.length;
            var highestnode = -1;
            for (var i=0; i<currentCounter; i++)
            {
                var nodelength = parseInt(currentTable.childNodes[0].childNodes[0].childNodes[i].childNodes.length);
                var currentnode = currentTable.childNodes[0].childNodes[0].childNodes[i].childNodes[nodelength-1].id.split('_')[1];
                if (currentnode > highestnode)
                {
                    highestnode = currentnode;
                }
            }
            
            var itemSelected = false;    
            var containingTable = ($id(_clientID + "_tblDelegateAssignment"));
            var containingCounter = containingTable.childNodes[0].childNodes[0].childNodes.length;
            for (var i=0; i<childRows.childNodes.length; i++)
            {
                
                highestnode++;
                if (childRows.childNodes[i].childNodes[1].childNodes[0].checked)
                {
                    itemSelected = true;
                    var currentName =  childRows.childNodes[i].childNodes[0].innerHTML;
                    var currentUserName = childRows.childNodes[i].childNodes[0].innerText;
                    var currentType =  childRows.childNodes[i].childNodes[0].getAttribute("actionertype");
                    if (existingUserArray.indexOf(currentName.toUpperCase() + "|") < 0)
                    {
                        //Build structure according to actions available
                       var TR = document.createElement("TR");
                       
                       var TD = document.createElement("TD");
                       TD.align = "left";
                       TD.className = "ms-vb-div";
                       TR.appendChild(TD);
                       
                       TD = document.createElement("TD");
                       TD.innerHTML = currentName;
                       TD.align = "left";
                       TD.className = "ms-vb";
                       TR.appendChild(TD);
                       
                       TD = document.createElement("TD");
                       TD.align = "left";
                       TD.className = "ms-vb-div";
                       TR.appendChild(TD);
                      
                       TD = document.createElement("TD");
                       TD.innerHTML = currentType;
                       TD.align = "left";
                       TD.className = "ms-vb";
                       TR.appendChild(TD);
                       
                       TD = document.createElement("TD");
                       TD.align = "left";
                       TD.className = "ms-vb-div";
                       TR.appendChild(TD); 
                       
                       for (var z=5; z<containingCounter-3; z++)
                       {
                             TD = document.createElement("TD");
                             TD.className = "ms-vb-div";
                             var checkbox = document.createElement("input");
                             TD.align = "center";
                             checkbox.type = "checkbox";
                             var id = containingTable.childNodes[0].childNodes[0].childNodes[z].innerText;
                             if (id.length > 0)
                             {
                                if (containingTable.childNodes[0].childNodes[0].childNodes[z].childNodes[0].tagName == null)
                                {
                                    checkbox.id = id;
                                    TD.className = "ms-vb";
                                    TD.appendChild(checkbox);
                                }
                             }
                             TR.appendChild(TD);
                       }
                       
                        TD = document.createElement("TD");
                        TD.className = "normal_context_arrow";  
                        TD.style.width = "20px";
                        TD.style.cursor = "hand";
                        TD.style.height  = "20px";
                        TD.align = "center";
                        TD.setAttribute("ClientID",_clientID); 
                        TD.setAttribute("currentUserName",currentUserName); 
                        TD.setAttribute("RowID",highestnode);  
                        TD.onclick = function() {ShowContextPopup_Delegate(this);}
                        TD.id = "tdDelegate_" + highestnode;
                        TD.innerHTML = "<IMG id='tdDelegateImg_" + highestnode + "' src='Images/black_large_context_arrow.gif'/>"  
                        TR.appendChild(TD); 
                       
                       if (newTable.tagName == "TBODY")
                       {
                            newTable.appendChild(TR);
                       }
                       else
                       {
                            newTable.firstChild.appendChild(TR);
                       } 
                    }
                    else 
                    {
                          //handles the currently deleted values, if added again, show only, don't add AND delete
                          var newTable = ($id("BodyID"+ _clientID + "_DelegateAssignment"));
                          if (newTable.childNodes.length > 0)
                          {
                                if (newTable.childNodes[0].childNodes.length > 0)
                                {
                                    newTable = newTable.childNodes[0].childNodes[0];  //get into the TBODY
                                     for (var i=0; i<newTable.childNodes.length; i++)
                                     {
                                        if (newTable.childNodes[i].style.display == "none")
                                        {
                                            if ( newTable.childNodes[i].childNodes[1].innerText.toUpperCase() == currentUserName.toUpperCase()) 
                                            {
                                                 newTable.childNodes[i].style.display = "";
                                                break; 
                                            }
                                        }
                                      }
                                 }
                            } 
                        }
                    }
                }
                if (itemSelected)
                {
                    $doHideControlPopup(_clientID + "_UserPickerTable");
                    Reset_UserPicker();
                }
                else
                {
                    $showPopup(JSWarning, JSSelectDelegationUser, "", "70", "250", "", "doHidePopup('msgdiv')", JSOK + "☺doHidePopup('msgdiv')", "11000", "msgdiv", "", "", "", "", "", "warning");    
                }
            } 
            else
            {
                $showPopup(JSWarning, JSSelectDelegationUser, "", "70", "250", "", "doHidePopup('msgdiv')", JSOK + "☺doHidePopup('msgdiv')", "11000", "msgdiv", "", "", "", "", "", "warning");
            } 
        }
        else
        {
            $showPopup(JSWarning, JSSelectDelegationUser, "", "70", "250", "", "doHidePopup('msgdiv')", JSOK + "☺doHidePopup('msgdiv')", "11000", "msgdiv", "", "", "", "", "", "warning");
        }         
    } 
    ResizeCells(_clientID + "_DelegateAssignment"); 
}

function loadActionsForProcess(ClientID)
{
    GenericAjax(null, ClientID, "GenericAction~LoadActionsForProcess|SerialNumber~" + _sn + "|Action~");     
}

function addActionsForProcess(result)
{
    var headerrow = ($id(_clientID + "_tblDelegateAssignment")).childNodes[0].childNodes[0];
    var xCounter =  headerrow.childNodes.length;
    
    for (var x=0; x < xCounter; x++)
    {
        headerrow.removeChild(headerrow.childNodes[0]);
    }
     
    var newTH = document.createElement("TH");
    newTH.innerHTML = "<IMG src='Images/column_left.gif' />";
    newTH.className = "ms-vh-div";
    newTH.style.width = "1px";
    newTH.vAlign = "top"; 
    headerrow.appendChild(newTH);
          
    newTH = document.createElement("TH");
    newTH.innerHTML = JSDelegate;
    newTH.className = "ms-vh2";
    headerrow.appendChild(newTH);
   
    newTH = document.createElement("TH");
    newTH.style.whiteSpace = "nowrap";
    newTH.style.vAlign = "middle"; 
    newTH.innerHTML = "<IMG src='Images/column_devider.gif'/>";
    newTH.className = "ms-vh-div";
    newTH.style.padding = "0px";
    headerrow.appendChild(newTH);
   
   newTH = document.createElement("TH");
    newTH.innerHTML = JSType;
    newTH.className = "ms-vh2";
    headerrow.appendChild(newTH);
   
    newTH = document.createElement("TH");
    newTH.style.whiteSpace = "nowrap";
    newTH.style.vAlign = "middle"; 
    newTH.innerHTML = "<IMG src='Images/column_devider.gif'/>";
    newTH.className = "ms-vh-div";
    newTH.style.padding = "0px";
    headerrow.appendChild(newTH); 
        
    var actions = result.split('~');
    for (var i=0; i<actions.length; i++)
    {
        if (actions[i].length > 0)
        {
            newTH = document.createElement("TH");
            newTH.innerHTML = actions[i];
            newTH.id = actions[i];
            newTH.style.vAlign = "middle"; 
            newTH.className = "ms-vh2";
            headerrow.appendChild(newTH);
            
            if (parseInt(i + 2) < parseInt(actions.length))
            {
                newTH = document.createElement("TH");
                newTH.style.whiteSpace = "nowrap";
                newTH.style.vAlign = "middle"; 
                newTH.innerHTML = "<IMG src='Images/column_devider.gif'/>";
                newTH.className = "ms-vh-div";
                headerrow.appendChild(newTH);
            }
        }
   }  
   
    newTH = document.createElement("TH");
    newTH.innerHTML = "";
    newTH.className = "ms-vh2";
    newTH.style.padding = "0px";
    headerrow.appendChild(newTH);
    
    newTH = document.createElement("TH");
    newTH.innerHTML = "";
    newTH.className = "ms-vh2";
    newTH.style.width = "5px";
    headerrow.appendChild(newTH);
   
    newTH = document.createElement("TH");
    newTH.innerHTML = "<IMG src='Images/column_right.gif' align='right'/>";
    newTH.align = "right"; 
    newTH.className = "ms-vh-div";
    newTH.vAlign = "top"; 
    headerrow.appendChild(newTH);
   
    ($id("BodyID" + _clientID + "_DelegateAssignment")).parentNode.colSpan = (actions.length * 2) - 1;
    
    setTimeout("getLoadUserActionInstanceRights()", 700);   
}

function getLoadUserActionInstanceRights()
{
    GenericAjax(null, _clientID, "GenericAction~LoadUserActionInstanceRights|SerialNumber~" + _sn + "|OriginalDestinations~" + _OriginalDestinations, false);  
}

function ResizeCells(controlName)
{
    var headerRowColumns = ($id("HeaderID" + controlName)).childNodes; 
    if (($id("BodyID" + controlName)).childNodes[0].childNodes[0].childNodes[0] != null)
    {
        var bodyColumns = $id("BodyID" + controlName).childNodes[0].childNodes[0].childNodes[0].childNodes;
        for (var i = 0; i < headerRowColumns.length-1; i++)
        {
	        if (headerRowColumns[i].tagName == "TD" || headerRowColumns[i].tagName == "TH")
	        {
		        SetWidth(headerRowColumns[i], bodyColumns[i]);
	        }
        }
        $id("BodyID" + controlName).style.overflow = "hidden";
        $id("BodyID" +controlName).style.overflowY = "scroll";		    
    }
}

function SetWidth(element1, element2)
{
    if (element2)
    {
 	    if (element2.offsetWidth < element1.offsetWidth)
	    {
	        element2.style.width = element1.offsetWidth+ "px";
	    }
	    else
	    {
	        element1.style.width = element2.offsetWidth+ "px";
	    }
	}
}

function saveActionInstanceRights(ClientID)
{
    var parentTable = ($id("BodyID" +  ClientID + "_DelegateAssignment")).childNodes[0].childNodes[0];
    var iCounter = parentTable.childNodes.length;
    var saveString = "";
    var deleteString = ""; 
    
    for (var i=0; i<iCounter; i++)
   {
        var zCounter =  parentTable.childNodes[i].childNodes.length;
        for (var z=0; z<zCounter; z++)
        {
            if (parentTable.childNodes[i].childNodes[z].childNodes.length > 0)
            {
                if ((parentTable.childNodes[i].childNodes[z].childNodes[0].tagName == "INPUT") && (parentTable.childNodes[i].childNodes[z].childNodes[0].type == "checkbox") )
               {
                    //var currentUser = parentTable.childNodes[i].childNodes[1].innerHTML;
                    var currentUser = parentTable.childNodes[i].childNodes[1].innerText;
                    var currentAction = parentTable.childNodes[i].childNodes[z].childNodes[0].id;
                    var checked = parentTable.childNodes[i].childNodes[z].childNodes[0].checked;
                    var actionerType = parentTable.childNodes[i].childNodes[3].innerText;
                    if (parentTable.childNodes[i].style.display == "none")
                    {
                        deleteString += "" + _sn + separatorValue + currentUser + separatorValue +  currentAction + separatorValue + checked + separatorValue + actionerType + secondSeparator ; 
                    }
                    else
                    {
                        saveString += "" + _sn + separatorValue + currentUser + separatorValue +  currentAction + separatorValue + checked  + separatorValue + actionerType + secondSeparator;
                    }
               } 
            }
        }
   }
   if ((saveString.length > 0) || (deleteString.length > 0))
   {
        
         GenericAjax(null, ClientID, "GenericAction~SaveUserActionInstanceRights|SerialNumber~" + _sn +  "|SaveValues~" + saveString + "|DeleteValues~" + deleteString + "|OriginalDestinations~" + _OriginalDestinations);     
   } 
   
}

function loadActionInstanceRights(result)
{
  var newTable = ($id("BodyID" +  _clientID + "_DelegateAssignment"));
  if (newTable.childNodes.length > 0)
  {
    newTable.removeChild(newTable.firstChild);
    var Table = document.createElement("TABLE");
    var TBody = document.createElement("TBODY");
    Table.appendChild(TBody);
    newTable.appendChild(Table);  
    Table.style.width = "582px";
    Table.cellPadding = "0";
    Table.cellSpacing = "0";
    Table.onmouseover = function() {ClosePopups();}
    newTable = Table;
  }
  
  if (result.length > 0)
  {
    var containingTable = ($id(_clientID + "_" + "tblDelegateAssignment"));
    var containingCounter = containingTable.childNodes[0].childNodes[0].childNodes.length;
    var resultValues = result.split('|');
    for (var i=0; i<resultValues.length; i++)
    {
        if (resultValues[i].length > 0)
        {
           var currentValue = resultValues[i].split('~~');
           var currentName =  currentValue[0]; 
           var currentActionerType = currentValue[1];
           var currentActionRights = currentValue[2].split('~');
            
           //Build structure according to actions available
           var TR = document.createElement("TR");
           newTable.firstChild.appendChild(TR);
           
           var TD = document.createElement("TD");
           TD.align = "left";
           TD.className = "ms-vb-div";
           TR.appendChild(TD);
           
           TD = document.createElement("TD");
           TD.innerHTML = currentName;
           TD.align = "left";
           TD.className = "ms-vb";
           TR.appendChild(TD);
           
           var currentUserName = TD.innerText;
           
           TD = document.createElement("TD");
           TD.className = "ms-vb-div";
           TR.appendChild(TD);
           
           TD = document.createElement("TD");
           TD.innerHTML = currentActionerType;
           TD.align = "left";
           TD.className = "ms-vb";
           TR.appendChild(TD);
           
           TD = document.createElement("TD");
           TD.className = "ms-vb-div";
           TR.appendChild(TD);
           
           for (var z=5; z<containingCounter-3; z++)
           {
             TD = document.createElement("TD");
             TR.appendChild(TD);
             TD.align = "center";
             TD.className = "ms-vb-div";
             var checkbox = document.createElement("input");
             checkbox.type = "checkbox";
             
             var id = containingTable.childNodes[0].childNodes[0].childNodes[z].innerText;
             if (id.length > 0)
             {
                if (containingTable.childNodes[0].childNodes[0].childNodes[z].childNodes[0].tagName == null)
                {
                    TD.className = "ms-vb";
                    TD.appendChild(checkbox);
                    checkbox.id = id;
                    
                    for (var x=0; x<currentActionRights.length; x++)
                    {
                        if (currentActionRights[x].split('=')[0] == id)
                        {
                            if (currentActionRights[x].split('=')[1] == "true")
                            {
                                checkbox.checked = true;
                            } 
                            else
                            {
                                checkbox.checked = false;
                            }
                            break; 
                        }//if
                     }//for
                  }//if
               }//if
            }//for
            
            TD = document.createElement("TD");
            TD.className = "normal_context_arrow";  
            TD.style.cursor = "hand";
            TD.style.height  = "20px";
            TD.style.width = "20px";
            TD.align = "center";
            TD.setAttribute("ClientID",_clientID); 
            TD.setAttribute("currentUserName",currentUserName); 
            TD.setAttribute("RowID",i);  
            TD.onclick = function() {ShowContextPopup_Delegate(this);}
            TD.id = "tdDelegate_" + i;
            TD.innerHTML = "<IMG id='tdDelegateImg_" + i + "' src='Images/black_large_context_arrow.gif'/>"  
            TR.appendChild(TD);                      
        }
    }
  }
  $doControlPopupDetail(JSPerformActionDelegateHeading,"","",_clientID + "_DelegateTable",JSOK + "☺OK_Delegate('" + _clientID + "')☺" + JSClose + "☺Close_Delegate('" + _clientID + "')", "X", "$doHideControlPopup('" + _clientID + "_DelegateTable')",""); 
  ResizeCells(_clientID + "_DelegateAssignment"); 
  hidePWait(); 
}

function ConfirmDeleteDelegate()
{
    CloseContextPopup_Delegate();
    $showPopup(JSConfirmation, JSPerformDeleteItem, "", 75,300, "", "", JSOK + "☺DelegateDeletionConfirmed()☺" + JSClose + "☺doHidePopup()", 20000, "", "", "", "", "", "","help");
}

function CloseContextPopup_Delegate()
{    
    var tdDelegate = $id("tdDelegate_" + _criteriaItemID);
    var tdDelegateImg = $id("tdDelegateImg_" + _criteriaItemID);
    var DelegateRowPopupDiv = $id("DelegateRowPopupDiv");   
    
    if (DelegateRowPopupDiv.style.display != "none")
    {
        DelegateRowPopupDiv.style.display = "none";
        if (tdDelegate != null)
        {
            if (tdDelegateImg != null)
            {
                tdDelegateImg.src = "../Images/black_large_context_arrow.gif";
            }
        }
        tdDelegate.className = "normal_context_arrow";
    }
}

function ShowContextPopup_Delegate(obj)
{
    var tdDelegate = $id("tdDelegate_" + obj.getAttribute("RowID"));
    var tdDelegateImg = $id("tdDelegateImg_" + obj.getAttribute("RowID"));
    var DelegateRowPopupDiv = $id("DelegateRowPopupDiv");    
    
    if (tdDelegate != null)
    {
        if (tdDelegateImg != null)
        {
            tdDelegateImg.src = "Images/white_large_context_arrow.gif";
        }
        _criteriaItemID = obj.getAttribute("RowID");
        tdDelegate.className = "tooltext_hover";
        showContextMenuWithScrollMain(obj.getAttribute("ClientID") + "_DelegateTable", "DelegateRowPopupDiv","BodyID" + obj.getAttribute("ClientID") + "_DelegateAssignment");
    }
}

function popupUserSearch(ClientID)
{
  _typeOfSearch = "MultiSelect";
  $doControlPopupDetail(JSSelectDelegate,"","",ClientID + "_UserPickerTable",JSOK +"☺OK_DelegateUsers('" + ClientID + "')☺" + JSClose + "☺Close_DelegateUsers('" + ClientID + "')", "X", "Close_DelegateUsers('" + ClientID + "')","");
  $id(_userPickerClientID + "_txtUser").focus();
}

function Close_DelegateUsers(ClientID)
{
    $doHideControlPopup(ClientID + "_UserPickerTable");
    Reset_UserPicker();
}

function OK_DelegateUsers(ClientID)
{
    btnDelegateUsersAdd_Click(ClientID);
}

function OK_Delegate(ClientID)
{
    //check to see if the users are out of office first
      var parentTable = ($id("BodyID" +  ClientID + "_DelegateAssignment")).childNodes[0].childNodes[0];
    var iCounter = parentTable.childNodes.length;
    var usersToCheck = false;
    var UsersXml=$xml("<Users></Users>");
    var xpathString="Users"
    var usersNode=$sn(UsersXml,xpathString);
    for (var i=0; i<iCounter; i++)
   {
        var zCounter =  parentTable.childNodes[i].childNodes.length;
        for (var z=0; z<zCounter; z++)
        {
            if (parentTable.childNodes[i].childNodes[z].childNodes.length > 0)
            {
                if ((parentTable.childNodes[i].childNodes[z].childNodes[0].tagName == "INPUT") && (parentTable.childNodes[i].childNodes[z].childNodes[0].type == "checkbox") )
               {
                    //var currentUser = parentTable.childNodes[i].childNodes[1].innerHTML;
                    var currentUser = parentTable.childNodes[i].childNodes[1].innerText;
                    if (parentTable.childNodes[i].style.display != "none")
                    {
                        usersToCheck=true;
                        
                        var newUser=UsersXml.createElement("User");
                        usersNode.appendChild(newUser);
                        
                        
                        var newUserNameNode=UsersXml.createElement("UserName");
                        var newUserNameNodeValue=UsersXml.createTextNode(currentUser);
                        newUserNameNode.appendChild(newUserNameNodeValue);
                        newUser.appendChild(newUserNameNode);
                    }
               } 
            }
        }
   }
   if (usersToCheck)
   {
        
         GenericAjax(null, ClientID, "GenericAction~GetOutOfOfficeUsers|UsersXml~"+UsersXml.xml);     
   } 
   else //nothing to check proceed with other delegation actions
   {
        Do_Delegation(ClientID);
   }
   
}
function ShowOutOfOfficeDelegateConfirmation(ClientID,userXML)
{
    
    var UsersXml=$xml(userXML);
    var xpathString="Users/User/UserName"
    var usersNameNodes=$mn(UsersXml,xpathString);
    var returnMsg=JSConfirmDelegateToOutOfOfficeUser+"<br><br>";
    if (usersNameNodes!=null && usersNameNodes.length>0)
    {
        for(var i=0;i<usersNameNodes.length;i++)
        {
            returnMsg+=usersNameNodes[i].text+"<br>";
        }
        $showPopup(JSConfirmation, returnMsg, "", 75,300, "", "", JSOK + "☺Do_Delegation('" + _clientID + "')☺"+JSClose+"☺doHidePopup()", 22000, "", "", "", "", "", "","help");
    }
    else
    {
        Do_Delegation(ClientID);
    }
}

function Do_Delegation(ClientID)
{
    ClosePopups();
    $doHideControlPopup(ClientID + "_DelegateTable");
    saveActionInstanceRights(ClientID);
}
function Close_Delegate(ClientID)
{
    ClosePopups();
    $doHideControlPopup(ClientID + "_DelegateTable");
}


function DelegateDeletionConfirmed()
{
   var UserName = ($id("tdDelegate_" + _criteriaItemID)).getAttribute("currentUserName");
   var newTable = ($id("BodyID"+ _clientID + "_DelegateAssignment"));
      if (newTable.childNodes.length > 0)
      {
            if (newTable.childNodes[0].childNodes.length > 0)
            {
                newTable = newTable.childNodes[0].childNodes[0];  //get into the TBODY
                 for (var i=0; i<newTable.childNodes.length; i++)
                 {
                    if ( newTable.childNodes[i].childNodes[1].innerText.toUpperCase() == UserName.toUpperCase())
                    {
                         newTable.childNodes[i].style.display = "none";
                        break; 
                    }
                  }
             }
        } 
        doHidePopup(); 
}

function FilterConfigTaskList(ClientID,FilterTextboxID)
{
    _filterTextboxID=FilterTextboxID;
    _clientID=ClientID;
    LoadOOOSettingsAjaxTL();    
}

function ShowFilerConfig(ClientID,FilterTextboxID)
{
    ShowMultiSelectButtons();
    $doHideControlPopup(ClientID + "_ConfigLayout");
    $doHideControlPopup(ClientID + "_OutOfOfficeTable");
    //$doControlPopupDetail(JSConfiguration,"","",ClientID + "_FilterConfig",JSBack + "☺FruitCake()☺" + JSNext + "☺Next_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')☺" + JSFinish + "☺FruitCake()☺" + JSClose + "☺Cancel_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')", "X", "Cancel_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')","");
    $doControlPopupDetail(JSConfiguration,"","",ClientID + "_FilterConfig",JSOK + "☺OK_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')☺" + JSClose + "☺Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')", "X", "Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')","","55001");
    BuildFilter(ClientID,FilterTextboxID);
    ResizeMainCells(ClientID + '_HeaderID_FilterConfig',ClientID + '_BodyID_FilterConfig');
   // setPopupButtonStatus('Back','disabled');
   // setPopupButtonStatus('Finish','disabled');    
}

function ShowFilerConfigLayout(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_FilterConfig");
    $doHideControlPopup(ClientID + "_OutOfOfficeTable");
    $doControlPopupDetail(JSConfiguration,"","",ClientID + "_ConfigLayout",JSOK + "☺OK_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')☺" + JSClose + "☺Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')", "X", "Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')","","55004");
}
function ShowOutOfOfficeConfig(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_FilterConfig");
     $doHideControlPopup(ClientID + "_ConfigLayout");
    //$doControlPopupDetail("Out of office configuration","","",ClientID + "_OutOfOfficeTable","Back☺ShowFilerConfigLayout('" + ClientID + "','" + FilterTextboxID + "')☺Next☺FruitCake()☺" + JSFinish + "☺OK_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')☺" + JSClose + "☺Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')", "X", "Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')","");
    $doControlPopupDetail(JSOOOConfiguration+" ("+_currentUserOOO+")","","",ClientID + "_OutOfOfficeTable",JSOK + "☺OK_FilterConfig('" + ClientID + "','" + FilterTextboxID + "')☺" + JSClose + "☺Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')", "X", "Cancel_FilterConfigLayout('" + ClientID + "','" + FilterTextboxID + "')","","55005");
    // setPopupButtonStatus('Next','disabled');
    _filterTextboxID=FilterTextboxID;
    fillOOOPopup(ClientID);
}

function FruitCake()
{
//return vrugte koeke
}

function Next_FilterConfig(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_FilterConfig");    
    ShowFilerConfigLayout(ClientID,FilterTextboxID);
}

function Next_OutOfOfficeConfigConfig(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_ConfigLayout");    
    ShowOutOfOfficeConfig(ClientID,FilterTextboxID);
}

function Cancel_FilterConfig(ClientID,FilterTextboxID)
{
    Cancel_All(ClientID,FilterTextboxID)
}

function Cancel_FilterConfigLayout(ClientID,FilterTextboxID)
{
    Cancel_All(ClientID,FilterTextboxID)
}
function Cancel_All(ClientID,FilterTextboxID)
{

    //Undo any changes incured
    UndoCustomColumnState(); //column layout
    if (_oldSettings!=null)
        $id(FilterTextboxID).value = _oldSettings; //filterconfig
    if (_oldOOOSettings!=null)
    $getOOOElement(ClientID+"_OutOfOfficeXml").value=_oldOOOSettings //Out of office
    
    //hide any thing thats open
    ClosePopups();
    $doHideControlPopup(ClientID + "_ConfigLayout"); 
    $doHideControlPopup(ClientID + "_FilterConfig"); 
    $doHideControlPopup(ClientID + "_OutOfOfficeTable"); 

}

function OK_FilterConfig(ClientID,FilterTextboxID)
{
    //make final changes to OOO xml
    var finalOOOxmlString=$getOOOElement(ClientID+"_OutOfOfficeXml").value;
    if (finalOOOxmlString!="")
    {
        saveOOOUserSettings(ClientID);
        finalOOOxmlString=$getOOOElement(ClientID+"_OutOfOfficeXml").value;
    }
    var passOOOparameter=""; 
    //this is the parameter that will make the tasklist call the tasklistOOOcontrol methods
    //It has been used here as an is dirty flag
    //The two reasons for this are 
            //to prevent uneeded server calls and 
            //to allow users to set other tasklist settings without OOO
    if (_oldOOOSettings!=finalOOOxmlString.replace(/"/gi,"'").replace(/\r\n/gi,"") ) //Replace " with ' and remove line feed +carriage return
    {
        passOOOparameter="OOOAction~True|";

        if (ValidateOOO(finalOOOxmlString)===true)
        {

        }
        else
        {
            return;
        }
    }
    ClosePopups();
    UpdateCustomColumnState();
    var ddlFilter = $id(ClientID + "_ddlFilter");
    var lblFilter = $id(ClientID + "_lblFilter");
    ddlFilter.options.length=0;
    
    var mapDoc = $xml($id(FilterTextboxID).value);
    
    var FilterItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem/FilterName");
    for(var i=0;i < FilterItemNodes.length; i++)
    {
        var oOption = document.createElement("OPTION");
        ddlFilter.options.add(oOption);
        oOption.innerText = FilterItemNodes[i].text;
        oOption.value = FilterItemNodes[i].text;
        if (FilterItemNodes[i].parentNode.childNodes[2].text == "true")
        {
            oOption.selected = true;
        }
    }
    
    if (FilterItemNodes.length > 0)
    {
        ddlFilter.style.display = "";
        lblFilter.style.display = "";
    }
    else
    {
        ddlFilter.style.display = "none";
        lblFilter.style.display = "none";
    }
    
    $doHideControlPopup(ClientID + "_FilterConfig");
    $doHideControlPopup(ClientID + "_ConfigLayout");
    $doHideControlPopup(ClientID + "_OutOfOfficeTable");
  
    GenericAjax(null,ClientID,passOOOparameter+"GenericAction~SaveSetting|SaveValue~" +  $id(FilterTextboxID).value+"|OOOsharingSettings~"+finalOOOxmlString);
}

function ChangeDefaultFilter(Filter,FilterTextboxID,ClientID)
{
    var mapDoc = $xml($id(FilterTextboxID).value);
    
    var FilterItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem/DefaultFilter");
    for(var i=0;i < FilterItemNodes.length; i++)
    {
        FilterItemNodes[i].text = "false";
    }
    var FilterItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + Filter + "']/DefaultFilter");
    FilterItemNode.text = "true";
    $id(ClientID + "_ddlFilter").value = Filter;
    $id(FilterTextboxID).value = mapDoc.xml;
}

function ShowContextPopup_CriteriaItem(obj)
{    
    var tdFilterCriteriaItem = $id("tdFilterCriteriaItem_" + obj.getAttribute("RowID"));
    var tdFilterCriteriaItemImg = $id("tdFilterCriteriaItemImg_" + obj.getAttribute("RowID"));
    var FilterRowPopupDiv = $id("FilterCriteriaItemRowPopupDiv");    
    
    if (tdFilterCriteriaItem != null)
    {
        if (tdFilterCriteriaItemImg != null)
        {
            tdFilterCriteriaItemImg.src = "../Images/white_large_context_arrow.gif";
        }
        _criteriaItemID = obj.getAttribute("RowID");
        tdFilterCriteriaItem.className = "tooltext_hover";
        showContextMenuWithScrollMain(obj.getAttribute("ClientID") + "_FilterCriteria", "FilterCriteriaItemRowPopupDiv", obj.getAttribute("ClientID") + "_BodyID_FilterCriteriaConfig");
    }
}

function ShowContextPopup(obj)
{    
    var ClientID = obj.getAttribute("ClientID");
    var FilterName = obj.getAttribute("FilterName");
    var FilterTextboxID = obj.getAttribute("FilterTextboxID");
    var tdFilterContext = $id("tdFilterContext_" + FilterName);
    var tdFilterContextImg = $id("tdFilterContextImg_" + FilterName);
    var FilterRowPopupDiv = $id("FilterRowPopupDiv");    
    
    if (tdFilterContext != null)
    {
        if (tdFilterContextImg != null)
        {
            tdFilterContextImg.src = "../Images/white_large_context_arrow.gif";
        }
        _filterName = FilterName;
        _filterTextboxID = FilterTextboxID;
        _clientID = ClientID;
        tdFilterContext.className = "tooltext_hover";
        showContextMenuWithScrollMain(ClientID + "_FilterConfig", "FilterRowPopupDiv", ClientID + "_BodyID_FilterConfig");
    }
}

function showContextMenuWithScrollMain(container, contextmenudiv,parentdiv)
{
    var displayLeft = event.x;
    var posx = 0;
    var posy = 0;        
    var contextMenuDiv = $id(contextmenudiv);
    var containerObj = $id(container);
    var parentDiv = $id(parentdiv);
    
    var divScrollHeight = parentDiv.scrollTop;
    
    if (!e) var e = window.event;
    if (e.pageX || e.pageY) 	
    {
        posx = e.pageX;
        posy = e.pageY;
    }
    else if (e.clientX || e.clientY) 	
    {
        posx = getElementTrueLeft(e.srcElement) + document.body.scrollLeft
		    + document.documentElement.scrollLeft;
	    posy = e.clientY + document.body.scrollTop
		    + document.documentElement.scrollTop;
    }
    
    var displayTop = (getElementTrueTop(e.srcElement) - divScrollHeight + 20) + "px";
    
    contextMenuDiv.style.display = "inline";
    contextMenuDiv.style.top = displayTop;
    contextMenuDiv.style.left = ((posx + 32 + containerObj.scrollLeft) - contextMenuDiv.clientWidth) + "px";       
}

function CloseContextPopup_CriteriaItem()
{    
    var tdFilterCriteriaItem = $id("tdFilterCriteriaItem_" + _criteriaItemID);
    var tdFilterCriteriaItemImg = $id("tdFilterCriteriaItemImg_" + _criteriaItemID);
    var FilterCriteriaItemRowPopupDiv = $id("FilterCriteriaItemRowPopupDiv");    
    
    if (FilterCriteriaItemRowPopupDiv.style.display != "none")
    {
        FilterCriteriaItemRowPopupDiv.style.display = "none";
        if (tdFilterCriteriaItem != null)
        {
            if (tdFilterCriteriaItemImg != null)
            {
                tdFilterCriteriaItemImg.src = "../Images/black_large_context_arrow.gif";
            }
        }
        tdFilterCriteriaItem.className = "normal_context_arrow";
    }
}

function CloseContextPopup()
{
    var tdFilterContext = $id("tdFilterContext_" + _filterName);
    var tdFilterContextImg = $id("tdFilterContextImg_" + _filterName);
    var FilterRowPopupDiv = $id("FilterRowPopupDiv");
    
    if (FilterRowPopupDiv.style.display != "none")
    {
        FilterRowPopupDiv.style.display = "none";
        if (tdFilterContext != null)
        {
            if (tdFilterContextImg != null)
            {
                tdFilterContextImg.src = "../Images/black_large_context_arrow.gif";
            }
            tdFilterContext.className = "normal_context_arrow";
        }
    }
}

function EditFilter(FilterName)
{
    _inEdit = true;
    CloseContextPopup();
    BuildFilterCriteria(FilterName);
    $id(_clientID + "_txtFilterName").value = FilterName;
    ShowFilterCriteria(_clientID);
}

function ClosePopups()
{
    CloseContextPopup_Delegate();
    CloseContextPopup();
    CloseContextPopup_CriteriaItem();
}

function UpdateFilterName()
{
    var mapDoc = $xml($id(_filterTextboxID).value);
    
    if (!((validateSpecialChars(_clientID + "_txtFilterName", "Filter Name","_"))))
    {
        return false;
    }
    
    if (_filterName == "")
    {
        var CurrentFilterName = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + $id(_clientID + "_txtFilterName").value + "']/FilterName");
        if (CurrentFilterName != null)
        {
            $showPopup(JSWarning, JSFilterExists, "", 75,300, "", "", JSOK + "☺doHidePopup()", 20000, "", "", "", "", "", "","warning");            
            return false;
        }
    }
    
    var FilterName = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterName");
    if (FilterName == null)
    {
        mapDoc = CreateOutline(mapDoc,$id(_clientID + "_txtFilterName").value);
        FilterName = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + $id(_clientID + "_txtFilterName").value + "']/FilterName");
    }
        
    FilterName.text = $id(_clientID + "_txtFilterName").value;
    _filterName = $id(_clientID + "_txtFilterName").value;
    $id(_filterTextboxID).value = mapDoc.xml;
    return true;
}

function OK_FilterCriteria()
{
    var retunValue;
    retunValue = UpdateFilterName();
    if (retunValue)
    {
        ForceDefault();
        BuildFilter(_clientID,_filterTextboxID);
        
        $doHideControlPopup(_clientID + "_FilterCriteria");
    }
}

function ForceDefault()
{
    var mapDoc = $xml($id(_filterTextboxID).value);
    var DefaultSetItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem[DefaultFilter='true']");
    var DefaultItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem/DefaultFilter");
    if (DefaultSetItemNodes.length == 0)
    {
        if (DefaultItemNodes.length > 0)
        {
            DefaultItemNodes[0].text = "true";
            $id(_filterTextboxID).value = mapDoc.xml;
        }
    }
}

function BuildFilter(ClientID,FilterTextboxID)
{
    //Build the Filter table
    var table = $id(ClientID + "_FilterTable");
    var mapDoc = $xml($id(FilterTextboxID).value);
    var FilterNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem");
    
    Deleteall(table);
    
    for(var i=0;i < FilterNodes.length; i++)
    {
        var Name = FilterNodes[i].childNodes[1].text;
        var Default = FilterNodes[i].childNodes[2].text;
        
        
        var newRow = table.insertRow(); 
        newRow.className = ((i % 2) == 1 ? "ms-vb" : "ms-vb ms-authoringcontrols");
        
        var newCell0 = newRow.insertCell(0);
        var newCell1 = newRow.insertCell(1);
        var newCell2 = newRow.insertCell(2);
        
        newCell0.innerHTML = Name;
        newCell0.className = "tableSpacing-left"
        newCell1.innerHTML = "<Input id='tdFilterContextCheckBox_" + Name + "' name='tdFilterContextCheckBox' type='radio' onclick=\"ChangeDefaultFilter('" + Name + "','" + FilterTextboxID + "','" + ClientID + "')\"" + ((Default == "true") ? "checked" : "") + "/>";
        
        newCell2.className = "normal_context_arrow";  
        newCell2.style.width = "20px";
        newCell2.style.cursor = "hand";
        newCell2.style.height  = "20px";
        newCell2.align = "center";
        newCell2.setAttribute("ClientID",ClientID);  
        newCell2.setAttribute("FilterTextboxID",FilterTextboxID);  
        newCell2.setAttribute("FilterName",Name);  
        newCell2.onclick = function() {ShowContextPopup(this);}
        newCell2.id = "tdFilterContext_" + Name;
        newCell2.innerHTML = "<IMG id='tdFilterContextImg_" + Name + "' src='../Images/black_large_context_arrow.gif'/>"
    }
}


function BuildFilterCriteria(FilterName)
{
    //Build the FilterCriteria table
    var table = $id(_clientID + "_FilterCriteriaTable");
    var mapDoc = $xml($id(_filterTextboxID).value);
    var FilterCriteriaItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + FilterName + "']/FilterCriteria/*");
    
    Deleteall(table);
    
    for(var i=0;i < FilterCriteriaItemNodes.length; i++)
    {
        var Field = FilterCriteriaItemNodes[i].childNodes[0].text;
        var Compare = FilterCriteriaItemNodes[i].childNodes[1].text;
        var Logical = FilterCriteriaItemNodes[i].childNodes[2].text;
        var FieldValue = FilterCriteriaItemNodes[i].childNodes[3].text;
        var Counter = FilterCriteriaItemNodes[i].childNodes[4].text;
        
        
        var newRow = table.insertRow(); 
        newRow.className = ((i % 2) == 1 ? "tableEvenRow" : "tableOddRow");
        
        var newCell0 = newRow.insertCell(0);
        newCell0.setAttribute("width","25%");
        var newCell1 = newRow.insertCell(1);
        newCell1.setAttribute("width","25%");
        var newCell2 = newRow.insertCell(2);  
        newCell2.setAttribute("width","25%");  
        var newCell3 = newRow.insertCell(3);  
        var newCell4 = newRow.insertCell(4);
        
        newCell0.innerHTML = Field;
        newCell1.innerHTML = Compare;
        newCell2.innerHTML = $rep(FieldValue,"^"," and ");
        newCell3.innerHTML = (i==0 ? "-" : Logical);
        
        newCell4.className = "normal_context_arrow";  
        newCell4.style.width = "20px";
        newCell4.style.cursor = "hand";
        newCell4.style.height  = "20px";
        newCell4.align = "center";
        newCell4.setAttribute("ClientID",_clientID);  
        newCell4.setAttribute("RowID",Counter);  
        newCell4.onclick = function() {ShowContextPopup_CriteriaItem(this);}
        newCell4.id = "tdFilterCriteriaItem_" + Counter;
        newCell4.innerHTML = "<IMG id='tdFilterCriteriaItemImg_" + Counter + "' src='../Images/black_large_context_arrow.gif'/>"
    }
}

function ConfirmDeleteFilter(FilterName)
{
    CloseContextPopup();
    _filterName = FilterName
    $showPopup(JSConfirmation, JSPerformDeleteItem, "", 75,300, "", "", JSOK + "☺DeleteFilter('" + _filterTextboxID + "')☺" + JSClose + "☺doHidePopup()", 20000, "", "", "", "", "", "","help");
}

function Deleteall(table)
{
   for(var i=table.rows.length-1; i > -1; i--)
       table.deleteRow(i); 
}

function DeleteFilter(FilterTextboxID)
{
    var mapDoc = $xml($id(FilterTextboxID).value);
    var FilterItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']");
    FilterItemNode.parentNode.removeChild(FilterItemNode);
    $id(FilterTextboxID).value = mapDoc.xml;
    
    ForceDefault();
    BuildFilter(_clientID,FilterTextboxID)
    
    doHidePopup();
}

function tableMouseEnter()
{
    CloseContextPopup();
}

function EditFilterCriteriaItem()
{
    _inEdit = true;
    var mapDoc = $xml($id(_filterTextboxID).value);
    var FilterCriteriaItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem[Counter='" + _criteriaItemID + "']");
    var caldOptional = $id(_clientID + "_caldOptional");
    var caldSingle = $id(_clientID + "_caldSingle"); 
    var caldBetween = $id(_clientID + "_caldBetween"); 
    
    CloseContextPopup_CriteriaItem();
    
    $id(_clientID + "_ddlFilterField").value = FilterCriteriaItemNode.childNodes[0].text;
    UpdateFilterUI();
    $id(_clientID + "_ddlFilterCompare").value = FilterCriteriaItemNode.childNodes[1].text;
    var selectedText = FilterCriteriaItemNode.childNodes[3].text;
    
    switch ($id(_clientID + "_ddlFilterField").options[$id(_clientID + "_ddlFilterField").selectedIndex].Type)
    {
        case "lookup":
            $id(_clientID + "_ddlFilterValue").value = selectedText;
        break;
    case "date":
        switch (FilterCriteriaItemNode.childNodes[1].text) {
            case "Equals":
                caldSingle.style.display = "none";
                caldBetween.style.display = "none";
                caldOptional.style.display = "";

                

                if ((selectedText == "Today") || (selectedText == "Yesterday") || (selectedText == "This Week") ||
                    (selectedText == "Last Week") || (selectedText == "This Month") || (selectedText == "Last Month") || 
                    (selectedText == "Last 120 Days") || (selectedText == "Last 30 Days") || (selectedText == "Last 60 Days") ||
                    (selectedText == "Last 7 Days") || (selectedText == "Last 90 Days") || (selectedText == "Last Year") ||
                    (selectedText == "This Year") || (selectedText == "Previous and Current Month") || (selectedText == "Previous and Current Year") ||
                    (selectedText == "Today and Yesterday") || (selectedText == "Previous and Current Week"))
                {
                    $id(_clientID + "_RelativeDateValue").checked = true;
                    $id(_clientID + "_ddlDateFilterValue").value = selectedText;

                    disableSpecificDate();
                }
                else {
                    $id(_clientID + "_SpecificDateValue").checked = true;
                    $id(_clientID + "_caldOptionalValue").value = selectedText;

                    disableRelativeDate();
                }
                break;
            case "Between":
                caldSingle.style.display = "none";
                caldBetween.style.display = "";
                caldOptional.style.display = "none";
                $id(_clientID + "_caldBetween1").value = selectedText.split("^")[0];
                $id(_clientID + "_caldBetween2").value = selectedText.split("^")[1];
                break;
            default:
                caldSingle.style.display = "";
                caldBetween.style.display = "none";
                caldOptional.style.display = "none";
                $id(_clientID + "_caldSingleValue").value = selectedText;
                break;
        }
        break;
        case "string":
            $id(_clientID + "_txtFilterValue").value = selectedText;
        break;
    }
    $id(_clientID + "_ddlFilterLogical").value = FilterCriteriaItemNode.childNodes[2].text;
    
    var FilterCriteriaItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem");
    var SmallestItem = true;
    
    for(var i=0;i < FilterCriteriaItemNodes.length; i++)
    {
        if (FilterCriteriaItemNodes[i].childNodes[4].text < _criteriaItemID)
        {
            SmallestItem = false;
        }
    }
    
    if (SmallestItem)
    {
        $id(_clientID + "_ddlFilterLogical").style.display = "none";
        $id(_clientID + "_tdFilterLogical").style.display = "none";
    }
    else
    {
        $id(_clientID + "_ddlFilterLogical").style.display = "";
        $id(_clientID + "_tdFilterLogical").style.display = "";    
    }
    
    ShowFilterCriteriaItem(_clientID);
}

function ShowFilterCriteriaItem(ClientID)
{
    $doControlPopupDetail(JSFilterCriteriaItem,"","",ClientID + "_FilterCriteriaItem",JSOK + "☺OK_FilterCriteriaItem('" + ClientID + "')☺" + JSClose + "☺$doHideControlPopup('" + ClientID + "_FilterCriteriaItem')", "X", "$doHideControlPopup('" + ClientID + "_FilterCriteriaItem')","","55003");
} 

function AddFilterCriteria()
{
    _inEdit = false;
    var retunValue;
    retunValue = UpdateFilterName();
    if (retunValue)
    {
        
        $id(_clientID + "_ddlFilterField").selectedIndex = 0;
        $id(_clientID + "_ddlFilterCompare").selectedIndex = 0;
        $id(_clientID + "_txtFilterValue").value = "";
        $id(_clientID + "_ddlFilterLogical").selectedIndex = 0;
        $id(_clientID + "_ddlFilterValue").selectedIndex = 0;
        
        UpdateFilterUI();
        
        var mapDoc = $xml($id(_filterTextboxID).value);
        var FilterCriteriaItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem");
        
        if (FilterCriteriaItemNodes.length == 0)
        {
            $id(_clientID + "_ddlFilterLogical").style.display = "none";
            $id(_clientID + "_tdFilterLogical").style.display = "none";
        }
        else
        {
            $id(_clientID + "_ddlFilterLogical").style.display = "";
            $id(_clientID + "_tdFilterLogical").style.display = "";    
        }    
           
        ShowFilterCriteriaItem(_clientID);
    }
}

function FilterField_Changed()
{
    UpdateFilterUI();
}

function UpdateFilterUI()
{
    var SelectedOption = $id(_clientID + "_ddlFilterField").options[$id(_clientID + "_ddlFilterField").selectedIndex];
    var Operators = SelectedOption.Operators;
    var LookUpValues = SelectedOption.LookUpValues;
    var UIType = SelectedOption.UIType;
    var Type = SelectedOption.Type;
    
    var ddlFilterCompare = $id(_clientID + "_ddlFilterCompare");
    var ddlFilterValue = $id(_clientID + "_ddlFilterValue");
    var txtFilterValue = $id(_clientID + "_txtFilterValue");
    var lblFilterValue = $id(_clientID + "_lblFilterValue");
    var ddlDateFilterValue = $id(_clientID + "_ddlDateFilterValue");
    var caldOptional = $id(_clientID + "_caldOptional");
    var caldSingle = $id(_clientID + "_caldSingle"); 
    var caldBetween = $id(_clientID + "_caldBetween"); 
    
    var arrOperators = Operators.split(",");
    var arrUIType
    if (UIType != null)
    {
      arrUIType = UIType.split(",");
    }
    
    ddlFilterCompare.options.length=0;
    for(var i=0;i < arrOperators.length; i++)
    {
        var oOption = document.createElement("OPTION");
        ddlFilterCompare.options.add(oOption);
        oOption.innerText = arrOperators[i];
        oOption.value = arrOperators[i];    
        if (UIType != null)
        {    
            oOption.setAttribute("UIType",arrUIType[i]);
        }
    }
    
    if (Type == "date")
    {
        txtFilterValue.style.display = "none";
        ddlFilterValue.style.display = "none";   
        lblFilterValue.style.display = "";  
        caldSingle.style.display = "none";
        caldBetween.style.display = "none";
        caldOptional.style.display = "";
        
        ddlDateFilterValue.options.length=0;
        if (LookUpValues != null)
        {
            var arrLookUpValues = LookUpValues.split(",");
            for(var i=0;i < arrLookUpValues.length; i++)
            {
                var oOption = document.createElement("OPTION");
                ddlDateFilterValue.options.add(oOption);
                oOption.innerText = arrLookUpValues[i];
                oOption.value = arrLookUpValues[i];        
            }
        }
        
    }
    else if (Type == "lookup")
    {
        lblFilterValue.style.display = "";
        txtFilterValue.style.display = "none";
        ddlFilterValue.style.display = "";
        caldOptional.style.display = "none";
        caldSingle.style.display = "none";
        caldBetween.style.display = "none";
        
        ddlFilterValue.options.length=0;
        if (LookUpValues != null)
        {
            var arrLookUpValues = LookUpValues.split(",");
            for(var i=0;i < arrLookUpValues.length; i++)
            {
                var oOption = document.createElement("OPTION");
                ddlFilterValue.options.add(oOption);
                oOption.innerText = arrLookUpValues[i];
                oOption.value = arrLookUpValues[i];        
            }
        }
    }
    else
    {
        lblFilterValue .style.display = "";
        txtFilterValue.style.display = "";
        ddlFilterValue.style.display = "none";
        caldOptional.style.display = "none";   
        caldSingle.style.display = "none";
        caldBetween.style.display = "none";         
    }
}

function RelativeDateValue_Click() {

    disableSpecificDate(); 
}

function SpecificDateValue_Click() {

    disableRelativeDate();
}

function disableRelativeDate() {

    //reload the comparison values
    UpdateFilterUI();
    //make the date disabled
    $id(_clientID + "_ddlDateFilterValue").disabled = true;
    $id(_clientID + "_child_caldOptionalValue").disabled = false;    
}

function disableSpecificDate() {

    //make the date disabled
    $id(_clientID + "_child_caldOptionalValue").disabled = true;
    $id(_clientID + "_ddlDateFilterValue").disabled = false;
    $id(_clientID + "_caldOptionalValue").value = "";
    $id(_clientID + "_ddlFilterCompare").options.length = 2;
}

function FilterCompare_Changed()
{
    var SelectedOption = $id(_clientID + "_ddlFilterCompare").options[$id(_clientID + "_ddlFilterCompare").selectedIndex];
    var caldOptional = $id(_clientID + "_caldOptional");
    var caldSingle = $id(_clientID + "_caldSingle"); 
    var caldBetween = $id(_clientID + "_caldBetween"); 
    
    var Type = SelectedOption.UIType;
    if (Type != null)
    {
        switch (Type)
        {
            case "Optional":
                caldOptional.style.display = "";
                caldSingle.style.display = "none";
                caldBetween.style.display = "none";
                break;
            case "Single":
                caldOptional.style.display = "none";
                caldSingle.style.display = "";
                caldBetween.style.display = "none";     
                break;                                
            case "Between":
                caldOptional.style.display = "none";
                caldSingle.style.display = "none";
                caldBetween.style.display = "";                     
                break;                 
        }  
    }
}

function ConfirmDeleteFilterCriteriaItem()
{
    CloseContextPopup_CriteriaItem();
    $showPopup(JSConfirmation, JSPerformDeleteItem, "", 75,300, "", "", JSOK + "☺DeleteFilterCriteriaItem()☺" + JSClose + "☺doHidePopup()", 20000, "", "", "", "", "", "","help");
}

function DeleteFilterCriteriaItem()
{
    var mapDoc = $xml($id(_filterTextboxID).value);
    var FilterCriteriaItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem[Counter='" + _criteriaItemID + "']");
    FilterCriteriaItemNode.parentNode.removeChild(FilterCriteriaItemNode);
    $id(_filterTextboxID).value = mapDoc.xml;
    
    BuildFilterCriteria(_filterName);
    
    doHidePopup();
}

function OK_FilterCriteriaItem()
{
    var mapDoc = $xml($id(_filterTextboxID).value);
    var currentID = 0;
    var InputValue = "";
    var Type;
        
    switch ($id(_clientID + "_ddlFilterField").options[$id(_clientID + "_ddlFilterField").selectedIndex].Type)
    {
        case "lookup":
            InputValue = $id(_clientID + "_ddlFilterValue").value;
            Type = 2;
        break;
        case "date":
            var SelectedOption = $id(_clientID + "_ddlFilterCompare").options[$id(_clientID + "_ddlFilterCompare").selectedIndex];
            var Type = SelectedOption.UIType;
            if (Type != null)
            {
                switch (Type)
                {
                    case "Optional":
                        if ($id(_clientID + "_RelativeDateValue").checked)
                        {
                            InputValue = $id(_clientID + "_ddlDateFilterValue").value;
                        }
                        else
                        {
                            if ($id(_clientID + "_caldOptionalValue").value == "")
                            {
                                $showPopup(JSWarning, JSSpecifyDate, "", 75,300, "", "", JSOK + "☺doHidePopup()", 20000, "", "", "", "", "", "","warning");            
                                return;
                            }
                            else
                            {                        
                                InputValue = $id(_clientID + "_caldOptionalValue").value;
                            }
                        }                        
                        break;
                    case "Single":
                        InputValue = $id(_clientID + "_caldSingleValue").value;
                        break;                                
                    case "Between":
                        InputValue = $id(_clientID + "_caldBetween1").value + "^" + $id(_clientID + "_caldBetween2").value ;
                        break;                 
                }  
            }      
            Type = 3;      
        break;
        case "string":
            InputValue = $id(_clientID + "_txtFilterValue").value;
            
            if ($id(_clientID + "_txtFilterValue").value.indexOf("|") != -1) 
            {
                doPopup(Generic_js_ValidateSpecialCharHeading, Generic_js_ValidateSpecialCharDescription + " 'Value' " + Generic_js_ValidateSpecialCharDescription1 + "  '|'.<br>" + Generic_js_ValidateSpecialCharDescription2, 400 , 70, Generic_js_ButtonOk, "doHidePopup('valIDSpec')|13", "", "", "", "", 11500, "valIDSpec", "", "", "warning");
                return;
            }  
                      
            if ($id(_clientID + "_txtFilterValue").value.indexOf("~") != -1) 
            {
                doPopup(Generic_js_ValidateSpecialCharHeading, Generic_js_ValidateSpecialCharDescription + " 'Value' " + Generic_js_ValidateSpecialCharDescription1 + "  '~'.<br>" + Generic_js_ValidateSpecialCharDescription2, 400 , 70, Generic_js_ButtonOk, "doHidePopup('valIDSpec')|13", "", "", "", "", 11500, "valIDSpec", "", "", "warning");
                return;
            }              
            Type = 1;
        break;
    }
    
    if (_inEdit)
    {
        var FilterCriteriaItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem[Counter='" + _criteriaItemID  + "']");
        FilterCriteriaItemNode.childNodes[0].text = $id(_clientID + "_ddlFilterField").value;
        FilterCriteriaItemNode.childNodes[1].text = $id(_clientID + "_ddlFilterCompare").value;
        FilterCriteriaItemNode.childNodes[3].text = "";
        FilterCriteriaItemNode.childNodes[3].appendChild(mapDoc.createCDATASection(InputValue));
        FilterCriteriaItemNode.childNodes[2].text = $id(_clientID + "_ddlFilterLogical").value;
        FilterCriteriaItemNode.childNodes[5].text = Type;
    }
    else
    {
        var FilterCriteriaItemNodes = $mn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria/SerializableFilterCriteriaItem");
        for(var i=0;i < FilterCriteriaItemNodes.length; i++)
        {
            if (FilterCriteriaItemNodes[i].childNodes[4].text > currentID)
            {
                currentID = FilterCriteriaItemNodes[i].childNodes[4].text;
            }
        }
    
        var FilterItemNode = $sn(mapDoc,"SerializableFilter/SerializableFilterItem[FilterName='" + _filterName + "']/FilterCriteria");
        var FilterCriteriaItemNode = mapDoc.createElement("SerializableFilterCriteriaItem");
        var FilterCriteriaItemFieldNode = mapDoc.createElement("Field");
        FilterCriteriaItemFieldNode.text = $id(_clientID + "_ddlFilterField").value;
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemFieldNode);
        var FilterCriteriaItemCompareNode = mapDoc.createElement("Compare");
        FilterCriteriaItemCompareNode.text = $id(_clientID + "_ddlFilterCompare").value;
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemCompareNode);
        var FilterCriteriaItemLogicalNode = mapDoc.createElement("Logical");
        FilterCriteriaItemLogicalNode.text = $id(_clientID + "_ddlFilterLogical").value;
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemLogicalNode);
        var FilterCriteriaItemValueNode = mapDoc.createElement("FieldValue");
        var dataNode = mapDoc.createCDATASection(InputValue);
        FilterCriteriaItemValueNode.appendChild(dataNode);
        FilterCriteriaItemValueNode.setAttribute("xsi:type","xsd:string");
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemValueNode);  
        var FilterCriteriaItemCounterNode = mapDoc.createElement("Counter");  
        FilterCriteriaItemCounterNode.text = parseInt(currentID) + 1;
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemCounterNode); 
        var FilterCriteriaItemTypeNode = mapDoc.createElement("Type");  
        FilterCriteriaItemTypeNode.text = Type;
        FilterCriteriaItemNode.appendChild(FilterCriteriaItemTypeNode);         
        FilterItemNode.appendChild(FilterCriteriaItemNode);
    }
    
    $id(_filterTextboxID).value = mapDoc.xml;
    
    $doHideControlPopup(_clientID + "_FilterCriteriaItem");
    BuildFilterCriteria(_filterName);
    ShowFilterCriteria(_clientID);
}

function ShowFilterCriteria(ClientID)
{
    $doControlPopupDetail(JSFilterCriteria,"","",ClientID + "_FilterCriteria",JSOK + "☺OK_FilterCriteria()☺" + JSClose + "☺$doHideControlPopup('" + ClientID + "_FilterCriteria')", "X", "$doHideControlPopup('" + ClientID + "_FilterCriteria')","","55002");
    ResizeMainCells(ClientID + '_HeaderID_FilterCriteriaConfig',ClientID + '_BodyID_FilterCriteriaConfig');
} 

function CreateOutline(mapDoc,Filtername)
{
    var SerializableFilterItem = mapDoc.createElement("SerializableFilterItem");
    var FilterName = mapDoc.createElement("FilterName");
    FilterName.text = Filtername;
    var DefaultFilter = mapDoc.createElement("DefaultFilter");
    DefaultFilter.text = "false";
    FilterItemNode = mapDoc.createElement("FilterCriteria");
    SerializableFilterItem.appendChild(FilterItemNode);
    SerializableFilterItem.appendChild(FilterName);
    SerializableFilterItem.appendChild(DefaultFilter);
    
    var SerializableFilter = $sn(mapDoc,"SerializableFilter");
    SerializableFilter.appendChild(SerializableFilterItem);
    return mapDoc;
}

function AddFilter(ClientID,FilterTextboxID)
{
    _inEdit = false;
    _clientID = ClientID;
    _filterTextboxID = FilterTextboxID;
    _filterName = "";
   
    var table = $id(_clientID + "_FilterCriteriaTable");
    Deleteall(table);
   
    CloseContextPopup();
    $id(ClientID + "_txtFilterName").value = "";
    ShowFilterCriteria(ClientID);
} 

function Paging_Navigate(Action)
{
    if (((Action == 'Next') &&  ($id(_clientID + "_txtLastRow").value.toUpperCase() == 'TRUE')) 
    || ((Action == 'Previous') &&  ($id(_clientID + "_txtStartRow").value == 1)))
    {
         return;
    }
    GenericAjax(null,_clientID,"PagerAction~" + Action);    
}

function checkKey()
{
    if ((event.keyCode<48) || (event.keyCode>59))
    {
        event.returnValue = false;
    } 
}

function ConfigTaskList(ClientID)
{
    $doControlPopupDetail(JSConfiguration,"","",ClientID + "_ConfigLayout",JSOK + "☺UseAJAXForConfig('" + ClientID + "')", "X", "$doHideControlPopup('" + ClientID + "_ConfigLayout')","");
}

function GetColumnSetValue(ClientID)
{
    var ConfigIFrame = $id(ClientID + "_ConfigIFrame");

    return GetItemByFriendlyName('select', 'ddlColumnSet').value
}

function GetCustomColumnsValues(ClientID)
{
    var ConfigIFrame = $id(ClientID + "_ConfigIFrame");
    var xmlConfig  = "<Columns></Columns>";
    var xmlDoc = $xml(xmlConfig);
    var xpResultKey = $sn(xmlDoc,"Columns");

    var FriendlyItems = document.getElementsByTagName('input');
    var ListedItems = "";
    var ddlColumns = GetItemByFriendlyName("select", ClientID + "_ddlColumns");
    
    // Loop through all the checkboxes   
    for (var i=0; i<FriendlyItems.length; i++)
    {
	    if ((FriendlyItems[i].getAttribute("friendlyname") != null) && (FriendlyItems[i].getAttribute("friendlyname") == 'CustomColumnsSelection'))
	    {
		    if (FriendlyItems[i].checked) {
		        		        
		        var Controlsim = xmlDoc.createElement("Column");
		        Controlsim.setAttribute("ColumnName",FriendlyItems[i].value);
		        Controlsim.setAttribute("ColumnFriendlyName",GetItemByFriendlyName('input', 'CustomColumnsFriendlyName_' +  FriendlyItems[i].id.split('_')[1]).value);
		        xpResultKey.appendChild(Controlsim);


		        //update the column dropdown list from the JS
		        //do this because the top quicksearch area does not refresh \
		        //when the AJAX call is made. i.e. the quick search area is only updated
		        //when the page does a post back. :(

		        var colValue = FriendlyItems[i].value;
		        var colText = FriendlyItems[i + 1].value;
		        ListedItems += colValue + "☺" + colText + "☻";

		    }
	    }
	}



	// this will remove any or all items that
	// no longer exist in the XML DOM but are still in the dropdown list.
	// going to do it like this because the top nav bar does not refresh during
	// the AJAX call back
	if (ListedItems != "") {
	    var maximus = ListedItems.split('☻');
	    ddlColumns.innerHTML = "";

	    AllOptions = ddlColumns.options.length;

	    //add default value
	    var option = document.createElement("option");
	    option.text = JSlblAll;
	    option.value = JSlblAll;
	    ddlColumns.options.add(option);

	    for (var t = 0; t < maximus.length; t++) {

	        if (maximus[t] != "") {
	            var validcolumn = false;
	            var runner = maximus[t].split('☺');

	            if (runner[0] != "" && runner[1] != "") {

	                var option = document.createElement("option");
	                option.text = runner[1];
	                option.value = runner[0];
	                //add the item
	                ddlColumns.options.add(option);
	            }
	        }
	    }
	}
    
    
    return xmlDoc.xml;
}

function GetGroupByValue(ClientID)
{
    var ConfigIFrame = $id(ClientID + "_ConfigIFrame");

    var SelectedValue = GetItemByFriendlyName('select', 'ddlColumnSet').value;
    switch(SelectedValue)
    {
    	case 'Custom':
    	{
        	return GetItemByFriendlyName('select', 'ddlCustomGroupBy').value;
        	break;
    	}
    	case 'Small':
    	{
        	return GetItemByFriendlyName('select', 'ddlSmallGroupBy').value;
        	break;
    	}
    	case 'Normal':
    	{
        	return GetItemByFriendlyName('select', 'ddlNormalGroupBy').value;
        	break;
    	}
    	case 'Detail':
    	{
        	return GetItemByFriendlyName('select', 'ddlDetailGroupBy').value;
        	break;
    	}
    }
}

function getItemActivated(ItemID, ItemInstID, ItemName, ItemType, HostServerName, HostServerPort, K2Server)
{
    if ((ItemType == 0) || (ItemType==2))
    {
        window.parent.$id("tdRealTimeState").style.display = "none";
        window.parent.$id("tdRefresh").style.display = "none";
        window.parent.$id("tdRefreshDivider").style.display = "none";
        window.parent.$id("tdBackButtonDivider").style.display = "none";
        window.parent.$id("tdBackButton").style.display = "inline";
        window.parent.$id("tdProcessProperties").style.display = "none";
        document.location = "ViewFlowReport.aspx?K2Server=" + K2Server + "&HostServerName=" + HostServerName + "&HostServerPort=" + HostServerPort + "&ProcInstID=" + $id("txtProcessID").value + "&ItemID=" + ItemID + "&ItemName=" + ItemName + "&ItemType=" + ItemType + "&ItemInstID=" + ItemInstID;
    }
}

function PreviousPage()
{
    window.parent.$id("tdRealTimeState").style.display = "inline";
    window.parent.$id("tdRefresh").style.display = "inline";
    window.parent.$id("tdRefreshDivider").style.display = "inline";
    //window.parent.$id("tdBackButtonDivider").style.display = "inline";
    window.parent.$id("tdBackButton").style.display = "none";
    window.parent.$id("tdProcessProperties").style.display = "inline";

    //Get the search url
    var url = document.location.search;    
    //Add it to the search url
    url = "ViewFlow.aspx" + url;
    //Just refresh the container
    $id("ViewFlowContainer").contentWindow.document.location = url;                                

    //history.go(-1);
}

function getProcessProperties(K2Server,HostServerName,HostServerPort,ProcessID)
{
    window.parent.$id("tdRealTimeState").style.display = "none";
    window.parent.$id("tdRefresh").style.display = "none";
    window.parent.$id("tdRefreshDivider").style.display = "none";
    window.parent.$id("tdBackButtonDivider").style.display = "none";
    window.parent.$id("tdBackButton").style.display = "inline";
    window.parent.$id("tdProcessProperties").style.display = "none";
    $id("ViewFlowContainer").contentWindow.document.location = "ViewFlowReport.aspx?K2Server=" + K2Server + "&HostServerName=" + HostServerName + "&HostServerPort=" + HostServerPort + "&ProcInstID=" + ProcessID + "&ItemID=0&ItemName=&ItemType=0&ItemInstID=0";    
}

	
	
function TasklistInit()
{
    // Do Out Of Office ajax call
  //OOO commented out  GenericAjax(null,_clientID,"GenericAction~GetOutOfOfficeStatus",false);     
    oobreports_init()
}

function callGetOutOfOfficeCheck(clientID)
{
    $id(_clientID + '_WaitAnimationStatus').src="images/blue_rotating_ball.gif";
    $id(_clientID + '_WaitAnimationStatus').alt="Please wait...";
    GenericAjax(null,clientID,"GenericAction~GetOutOfOfficeStatus",false);     

}

function RestartWorklistRefreah()
{
    clearTimeout(_worklistAutoRefresh);
    InitAutoUpdateWorklist();
}

function InitAutoUpdateWorklist(Interval)
{
    if (Interval != null)
    {
        _worklistAutoRefreshInterval = Interval;
    }
    
    _worklistAutoRefresh = setTimeout("RefreshWorklist()", _worklistAutoRefreshInterval);
}

function RefreshWorklist()
{
    GenericAjax(null,_clientID,"GenericAction~AutoRefresh",false);
}

function ResizeReportViewerViewFlow() {
    var viewerID = "";
    if ($id("hidReportViewerID") != null && $id("hidReportViewerID").value != "") {
        viewerID = $id("hidReportViewerID").value;
        var viewer = $id(viewerID);
        if (viewer != null) {
            var htmlheight = document.body.clientHeight;
            var htmlwidth = document.body.clientWidth;
            var subheight, subwidth = 0;

            if (htmlwidth > 740) {
                subheight = 102;
            }
            else {
                subheight = 132;
            }

            viewer.style.height = (htmlheight - subheight) + "px";
            viewer.style.width = (htmlwidth - subwidth) + "px";
        }
    }
    try {
        if (window.parent.document.body.onresize == null)//check if not already assigned
        { window.parent.document.body.onresize = intervaledResizeViewFlow; }
        else //check if function assigned is still valid in this context
        { testCatastrophic = window.parent.document.body.onresize.toString(); }
    }
    catch (icatastropicException) {
        window.parent.document.body.onresize = intervaledResizeViewFlow;
    }
}
function intervaledResizeViewFlow() {
    ResizeEvent();
    setTimeout("ResizeReportViewerViewFlow()", 50);
}

function WebForm_CallbackComplete() {
    var i;
    for (i = 0; i < __pendingCallbacks.length; i++) {
        var callbackObject = __pendingCallbacks[i];
        if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
            __pendingCallbacks[i] = null;
            WebForm_ExecuteCallback(callbackObject);
            if (!callbackObject.async) {
                __synchronousCallBackIndex = -1;
            }
            var callbackFrameID = "__CALLBACKFRAME" + i;
            var xmlRequestFrame = document.getElementById(callbackFrameID);
            if (xmlRequestFrame) {
                xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
            }
        }
    }
}

