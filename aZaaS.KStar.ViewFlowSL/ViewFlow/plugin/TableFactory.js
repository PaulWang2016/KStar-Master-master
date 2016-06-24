var MenuSelectedColumn;
var _sn;
var _performAction;
var _k2Server;
var __OriginalDestinations;

// Override, so that we can get the right images in the menu
function PrepContents(oMaster)
{	
	oMaster._fLargeIconMode=(oMaster.getAttribute("largeIconMode")=="true");
	oMaster._fCompactItemsWithoutIcons=(oMaster.getAttribute("CompactMode")=="true");
	if (!browseris.safari)
	{
		oMaster._oContents=document.createElement("span");
		oMaster._oContents.style.display="none";
		oMaster._oContents.innerHTML=oMaster.innerHTML;
	}
	else
	{
		oMaster._oContents=oMaster.cloneNode(true);
		oMaster._oContents.style.display="none";
	}
	if (oMaster._fLargeIconMode)
	{
		if (oMaster._fIsRtL)
			oMaster._wzMenuStyle="ms-MenuUILargeRtL";
		else
			oMaster._wzMenuStyle="ms-MenuUILarge";
	}
	else
	{
		if (oMaster._fIsRtL)
			oMaster._wzMenuStyle="ms-MenuUIRtL";
		else
			oMaster._wzMenuStyle="ms-MenuUI";
	}

    if (document.getElementById("GlobalTitleAreaImage") == null)
    {
        // We are not in sharepoint
	    oMaster._wzChkMrkPath="images/ChkMrk.gif";
	    oMaster._wzMArrPath="images/MArr.gif";
	    oMaster._wzMArrPathRtL="images/MArrRtL.gif";
	}
	else
	{
	    // We are in sharepoint
	    oMaster._wzChkMrkPath="_layouts/images/ChkMrk.gif";
	    oMaster._wzMArrPath="_layouts/images/MArr.gif";
	    oMaster._wzMArrPathRtL="_layouts/images/MArrRtL.gif";
	}
}

// Gets an item by the friendly name
function GetItemByFriendlyName(TagName, FriendlyName)
{
    var FriendlyItems = document.getElementsByTagName(TagName);
    var FoundItem = null;
    // Loop through all the checkboxes
    for (var i=0; i<FriendlyItems.length; i++)
    {
	    if ((FriendlyItems[i].getAttribute("friendlyname") != null) && (FriendlyItems[i].getAttribute("friendlyname") == FriendlyName))
	    {
		    FoundItem = FriendlyItems[i];
	    }
    }

    return (FoundItem);
}

// Toggle whether or not we are showing items for the current group
function ToggleRowsForGroup(GroupName, GroupItem)
{
    var TableRowsInDocument = document.getElementsByTagName('tr');
    var ComparisonItem;
    
    if (browseris.ie5up) 
    {
        try
        {
	        ComparisonItem = GroupItem.childNodes[1].firstChild.firstChild.firstChild.firstChild.firstChild;
	    }
	    catch(e)
	    {
	        ComparisonItem = GroupItem.firstChild.firstChild.firstChild.firstChild.firstChild.firstChild;
	    }

    	if (ComparisonItem.src.indexOf(Images[0]) > -1)
    	{
        	// Show all the items
		ComparisonItem.src = Images[1];
        
        	// Loop through all the table rows, hiding the ones in the selected group
        	for (var i=0; i<TableRowsInDocument.length; i++)
        	{
			if ((TableRowsInDocument[i].getAttribute("GroupBy") != null) && (TableRowsInDocument[i].getAttribute("GroupBy") == GroupName))
	        	{
	            		TableRowsInDocument[i].style["display"] = "block";
	        	}
        	}
    	}
    	else
    	{
        	// Hide all the items
        	ComparisonItem.src = Images[0];
        
        	// Loop through all the table rows, hiding the ones in the selected group
        	for (var i=0; i<TableRowsInDocument.length; i++)
        	{
	        	if ((TableRowsInDocument[i].getAttribute("GroupBy") != null) && (TableRowsInDocument[i].getAttribute("GroupBy") == GroupName))
	        		{
	            			TableRowsInDocument[i].style["display"] = "none";
	        		}
        	}       
    	}
    }
}

// Event handler for when a column header is clicked (need to show the menu)
function ClickedMenuOption(Option,ClientID)
{
    GetItemByFriendlyName("input", ClientID + "_txtSortColumn").value = MenuSelectedColumn;
    GetItemByFriendlyName("input", ClientID + "_txtSortType").value = Option;
    
    GenericAjax(null,ClientID,null);
}

// Event handler for when the actual text of a column header is clicked (don't need the menu)
function ClickedSortText(SelectedColumn,ClientID)
{
    var txtSortColumn = GetItemByFriendlyName("input", ClientID + "_txtSortColumn");
    var txtSortType = GetItemByFriendlyName("input", ClientID + "_txtSortType");
    
    if (SelectedColumn == txtSortColumn.value)
    {
        // Already have this column selected, so swap the sort type
        if (txtSortType.value == "ASC")
        {
            txtSortType.value = "DESC";
        }
        else
        {
            txtSortType.value = "ASC";
        }
    }
    else
    {
        txtSortType.value = "ASC";
        txtSortColumn.value = SelectedColumn;
    }
    
    GenericAjax(null,ClientID,null)
}

function ChangeCellHeaderHover(TableItem, Style, Visibility)
{
    if (browseris.ie5up) 
    { 
        TableItem.firstChild.firstChild.lastChild.style["visibility"] = Visibility;
        TableItem.className = Style;
    }
}

function ChangeCellHover(CellItem, Visibility)
{
    if (browseris.ie5up)
    {
        CellItem.firstChild.firstChild.lastChild.style["visibility"] = Visibility;
    }
}

function CheckAllBatchAction(SourceControl,ClientID)
{
    var ListOfCheckboxes = document.getElementsByName(ClientID + "_multipleselection");
    
    // Loop through all the checkboxes
    for (var i=0; i<ListOfCheckboxes.length; i++)
    {
        ListOfCheckboxes[i].checked = SourceControl.checked ? true : false;
    }
    SetupBatchActions(null,null,null,null,null,null,null,null,null,null,null,null,null,ClientID);
}

function SelectBatchAction(ClientID)
{
    var ddActions = $id(ClientID + "_ddlBatch");
    if (ddActions.value != "-1")
    {
        BatchActions(ddActions.value,
        ddActions.options(ddActions.selectedIndex).getAttribute("CellHeaderClass"),
        ddActions.options(ddActions.selectedIndex).getAttribute("CellClass"),
        ddActions.options(ddActions.selectedIndex).getAttribute("QuickSearchStyle"),
        ddActions.options(ddActions.selectedIndex).getAttribute("QuickSearchTitleStyle"),
        ddActions.options(ddActions.selectedIndex).getAttribute("ErrorStyle"),
        ddActions.options(ddActions.selectedIndex).getAttribute("BorderedCellStyle"),
        ddActions.options(ddActions.selectedIndex).getAttribute("K2Server"),
        ddActions.options(ddActions.selectedIndex).getAttribute("Platform"),
        ddActions.options(ddActions.selectedIndex).getAttribute("CSSFilePath"),
        ddActions.options(ddActions.selectedIndex).getAttribute("ImageFolderPath"),
        ddActions.options(ddActions.selectedIndex).getAttribute("JavascriptFolderPath"),
        ddActions.options(ddActions.selectedIndex).getAttribute("ASPFolderPath"),ClientID);
        ddActions.value = "-1";
    }
}

// Perform a batch action based on what is currently checked
function BatchActions(Action,CellHeaderClass , CellClass, QuickSearchStyle, QuickSearchTitleStyle, ErrorStyle, BorderedCellStyle, K2Server, Platform, CSSFilePath, ImageFolderPath, JavascriptFolderPath, ASPFolderPath, ClientID)
{
       // Get the list of checkboxes
    var ListOfCheckboxes = document.getElementsByName(ClientID + "_multipleselection");
    var SerialNumbers = "";
    var K2Servers = "";
    var OriginalDestinations= "";
    
    // Loop through all the checkboxes
    for (var i=0; i<ListOfCheckboxes.length; i++)
    {
        // See if the current one is checked
        if (ListOfCheckboxes[i].checked)
        {
            // Append to the current list of serial numbers
            // (The number is stored as an attribute of the checkbox)
            if (SerialNumbers == "")
            {
                SerialNumbers = ListOfCheckboxes[i].attributes["serial"].value;
                K2Servers = ListOfCheckboxes[i].attributes["k2server"].value;
                OriginalDestinations=ListOfCheckboxes[i].attributes["OriginalDestination"].value;
            }
            else
            {
                SerialNumbers += ";" + ListOfCheckboxes[i].attributes["serial"].value;
                K2Servers += ";" + ListOfCheckboxes[i].attributes["k2server"].value;
                OriginalDestinations+= ";" +ListOfCheckboxes[i].attributes["OriginalDestination"].value;
            }
        }
    }


    // Perform the action, sending the serial numbers attached to the current workflow
    PerformAction(Action, SerialNumbers,'ActionWindow', QuickSearchStyle, QuickSearchTitleStyle, ErrorStyle, BorderedCellStyle, K2Servers, Platform, CSSFilePath, ImageFolderPath, JavascriptFolderPath, ASPFolderPath,ClientID,OriginalDestinations);
}

function SetupBatchActions(CheckBox, CellHeaderClass, CellClass, QuickSearchStyle, QuickSearchTitleStyle, ErrorStyle, BorderedCellStyle, K2Server, Platform, CSSFilePath, ImageFolderPath, JavascriptFolderPath, ASPFolderPath,ClientID)
{
    // Generating the batch actions table also lets us see the actions we need
    // Code is kept for accessibility in Firefox, so might as well re-use it
    var Actions = SetupBatchActionsTable(ClientID);
    
    if (Actions != null)
    {
        var ActionsList = Actions.split(",");
        
        $id(ClientID + "_ddlBatch").options.length = 0;
        

        var key = "-1";
        var text = JSSelectionAction;    
        var newOption = new Option(text,key);    
        $id(ClientID + "_ddlBatch").options.add(newOption);            
        
        // Show just the ones we want
        for (var i=1; i<ActionsList.length; i++)
        {
            var key = ActionsList[i];
            var text = ActionsList[i];    
            var newOption = new Option(text,key);  
            newOption.setAttribute("CellHeaderClass",CellHeaderClass);
            newOption.setAttribute("CellClass",CellClass);
            newOption.setAttribute("QuickSearchStyle",QuickSearchStyle);
            newOption.setAttribute("QuickSearchTitleStyle",QuickSearchTitleStyle);
            newOption.setAttribute("ErrorStyle",ErrorStyle);
            newOption.setAttribute("BorderedCellStyle",BorderedCellStyle);
            newOption.setAttribute("K2Server",K2Server);
            newOption.setAttribute("Platform",Platform);
            newOption.setAttribute("CSSFilePath",CSSFilePath);
            newOption.setAttribute("ImageFolderPath",ImageFolderPath);
            newOption.setAttribute("JavascriptFolderPath",JavascriptFolderPath);
            newOption.setAttribute("ASPFolderPath",ASPFolderPath);
            $id(ClientID + "_ddlBatch").options.add(newOption);            
        }           
        
        // Show the dropdown
        GetItemByFriendlyName("tr", ClientID + "_BatchRow").style.display = "block";
    }
    else
    {
        // Hide the dropdown
        GetItemByFriendlyName("tr", ClientID + "_BatchRow").style.display = "none";
    }
}

// Setup the batch actions table,
// according to the full set of checked items
function SetupBatchActionsTable(ClientID)
{
    // Get all the document elements
    var ListOfCheckboxes = document.getElementsByName(ClientID + "_multipleselection");
    
    // A list of actions, that allows us to handle when we don't need the table
    var ListOfActions;
    var tempActions = new Array();
    var mergedActions = new Array();
    
    // Loop through all the checkboxes
    for (var i=0; i<ListOfCheckboxes.length; i++)
    {
        // See if the current checkbox is checked
        if (ListOfCheckboxes[i].checked)
        {
            tempActions = new Array();
            ListOfActions = "";
                        
            // Add it to the list
            var CurrentActions = ListOfCheckboxes[i].attributes["actions"].value.split(",");
            
            var bFound = false;
            // Loop through all the actions currently in the list
            for (var j=0; j<CurrentActions.length; j++)
            {
                // Check if the action can be added to the list (it is already there or it is the first time trying)
                if ((CurrentActions[j] != "") && ((GetArraySize(mergedActions) == 0) || (mergedActions[$rep(CurrentActions[j]," ","")]!= null)))
                {
                    tempActions[$rep(CurrentActions[j]," ","")] = CurrentActions[j];
                    ListOfActions += "," + CurrentActions[j];
                    bFound = true;
                }
            }
            
            if (!bFound)
            {
                return "";
            }
            
            if (GetArraySize(tempActions) > 0)
            {
                mergedActions = tempActions;
            }
        }
    }
    
    // Return the plain list of Actions
    return ListOfActions;
}

function GetArraySize(htToCalc)
{
    var size = 0;
    for (var i in htToCalc) 
    {
        if (htToCalc[i] != null) 
        size ++;
    }
    return size;
}

// Shows a set of properties for the user, allowing usage of the collaboration functionality
function ShowDestinationProperties(ProcessID, Target, QuickSearchStyle, QuickSearchTitleStyle, ErrorStyle, BorderedCellStyle, K2Server, Platform, CSSFilePath, ImageFolderPath, JavascriptFolderPath, ASPFolderPath)
{
    window.open(ASPFolderPath + "Collaborate.aspx?ProcessID=" + ProcessID + "&QuickSearchStyle=" + QuickSearchStyle + "&QuickSearchTitleStyle=" + QuickSearchTitleStyle + "&ErrorStyle=" + ErrorStyle + "&BorderedCellStyle=" + BorderedCellStyle + "&K2Server=" + K2Server + "&Platform=" + Platform + "&CSSFilePath=" + CSSFilePath + "&ImageFolderPath=" + ImageFolderPath +  "&JavascriptFolderPath=" + JavascriptFolderPath + "&ASPFolderPath=" + ASPFolderPath, Target, "height=400,location=no,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no,width=392");
}

function ShowDataFields(SerialNumber,Context,K2Server)
{
    _k2Server = K2Server;
    OpenDataFields(SerialNumber,Context);
}

function ShowViewFlow(ProcessID,Context,K2Server,ASPFolderPath,HostServerName,HostServerPort)
{
    window.open(ASPFolderPath + "ViewFlowMain.aspx?ProcessID=" + ProcessID + "&HostServerName=" + HostServerName + "&HostServerPort=" + HostServerPort + "&K2Server=" + K2Server, "_Blank", "'top=0,left=0,height=" + (screen.availHeight - 30) + ",width=" + (screen.availWidth - 10) + ",resizable=yes");
}

// Perform an action
function PerformAction(ActionName, SerialOrData, Target, QuickSearchStyle, QuickSearchTitleStyle, ErrorStyle, BorderedCellStyle, K2Server, Platform, CSSFilePath, ImageFolderPath, JavascriptFolderPath, ASPFolderPath,ClientID,OriginalDestinations,ManagedUser)
{
    var windowsetup = "height=400,location=no,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no,width=392";

    switch (ActionName)
    {
        case "Open":
        {
            var SharedUser = (OriginalDestinations == null) ? "" : (OriginalDestinations == "") ?  "" : "&SharedUser=" + OriginalDestinations;
            var managedUser = (ManagedUser == null) ? "" : (ManagedUser == "") ?  "" : "&ManagedUser=" + ManagedUser;
            
            // Open the worklist iteme
            window.open(SerialOrData + SharedUser + managedUser, Target);
            break;
        }
	    default:
	    {
	        // Open a normal action window
	        _sn  = SerialOrData;
	        _k2Server = K2Server
	        _performAction  = ActionName;
	        _OriginalDestinations=(OriginalDestinations==null)?"":OriginalDestinations;
	        
            switch (_performAction)
            {
                case "Release":
                    $showPopup(JSPerformActionReleaseHeading, JSPerformRelease, "", 75,300, "", "", JSOK + "☺PerformSelectedAction('PerformActionRelease','" + ClientID + "')☺" + JSClose + "☺doHidePopup()", 1000, "", "", "", "", "", "","help");
                    break;            
                case "SleepNow":
                    RelativeTime_Click(ClientID);
                    _performAction = "Sleep";
                    $doControlPopupDetail(JSPerformActionSleepHeading,"","",ClientID + "_SleepTable",JSOK + "☺OK_Sleep('" + ClientID + "')☺" + JSClose + "☺Close_Sleep('" + ClientID + "')", "X", "$doHideControlPopup('" + ClientID + "_SleepTable')","");
                    break;
                case "RedirectNow":
                    _performAction = "Redirect";
                    _typeOfSearch = "SingleSelect";
                    HideMultiSelectButtons();
                    $doControlPopupDetail(JSPerformActionRedirectHeading,"","",ClientID + "_UserPickerTable",JSOK +"☺OK_Redirect('" + ClientID + "')☺" + JSClose + "☺Close_Redirect('" + ClientID + "')", "X", "$doHideControlPopup('" + ClientID + "_UserPickerTable')","");
                    $id(_userPickerClientID + "_txtUser").focus();
                    break;   
                case "Delegate":
                      _performAction = "Delegate";
                      ShowMultiSelectButtons();               
                     loadActionsForProcess(ClientID); 
                    break;
                default:
                    $showPopup(JSPerformActionHeading, JSPerformAction + " '" + ActionName + "' " + JSPerformAction2, "", 75,300, "", "", JSOK + "☺PerformSelectedAction('PerformAction','" + ClientID + "')☺" + JSClose + "☺doHidePopup('" + ClientID + "')", 9020, "", "", "", "", "", "","help");
                break;
            }	        
	        break;
	    }
	}
}

function PerformSelectedAction(GenericAction,ClientID)
{
    doHidePopup();
    GenericAjax(null,ClientID,"GenericAction~" + GenericAction + "|SerialNumber~" + _sn +  "|Action~" + _performAction+ "|OriginalDestinations~"+_OriginalDestinations); 
}

function ActionCompleted(ClientID)
{
    doHidePopup();
    GenericAjax(null,ClientID,null);
}

// Shows a man on the form
function ShowManIfWeCan(Mannetjie, EMailText)
{
    if (browseris.ie5up) 
    { 
        IMNRC(EMailText);
    }
    else
    {
        Mannetjie.style.display = "none";
        Mannetjie.parentNode.textContent = EMailText;
    }
}

function ViewFlow_Onload() 
{

    try {
        if (document.getElementById('errormessage').value == '')
        {
            document.getElementById("ViewFlow").Xml = document.getElementById("strXML").value;
            document.getElementById("ViewFlow").ScrollBars = true;
        }
        else 
        {
            alert(document.getElementById('errormessage').value);
            window.parent.$id("tdRealTimeState").disabled = true;
            window.parent.$id("tdRefresh").disabled = true;
            window.parent.$id("tdProcessProperties").disabled = true;
        }		
        ViewFlow_Init();
    }
    catch (ex)
    {}

}
function ViewFlow_UnLoad() 
{

    try
    {
        document.getElementById("ViewFlow").Dispose();
    }
    catch (ex)
    {
    }

}
function ResizeMainCells(HeaderID,BodyID)
{
	var headerRowColumns = $id(HeaderID).childNodes[0].childNodes[0].childNodes[0].childNodes;
	if ($id(BodyID).childNodes[0].childNodes[0].childNodes[0] != null)
	{
	    var j = 0;
	    var bodyColumns = $id(BodyID).childNodes[0].childNodes[0].childNodes[0].childNodes;
	    for (var i = 0; i < headerRowColumns.length-1; i++)
	    {
	        if (headerRowColumns[i].getAttribute("className") != "ms-vh-div")
	        {
		        if (headerRowColumns[i].tagName == "TD" || headerRowColumns[i].tagName == "TH")
		        {
			        SetMainWidth(headerRowColumns[i], bodyColumns[j]);
			        j++;
		        }
		    }
	    }
        $id(BodyID).style.overflow = "hidden";
	    $id(BodyID).style.overflowY = "scroll";		    
	}
}

function SetMainWidth(element1, element2)
{
    if (element2 != null)
    {
	    element1.style.width = element2.offsetWidth + "px";
	}
}
function fwdUserEllipse_click(ClientID,FilterTextboxID)
{
    $doHideControlPopup(ClientID + "_OutOfOfficeTable");
    $id(ClientID + "_txtFwdUser").value = "";
    $id(ClientID + "_dgFwdUsers").innerHTML = "No data available";
    $doControlPopupDetail("Out of office - Forward to user","","",ClientID + "_FwdUserTable","OK☺OK_FwdUser('" + ClientID + "','"+FilterTextboxID+"')☺Cancel☺Close_FwdUser('" + ClientID + "','"+FilterTextboxID+"')", "X", "Close_FwdUser('" + ClientID + "','"+FilterTextboxID+"')","");

}
