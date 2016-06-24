var _formName = "form1";
var _currentBusyDiv = "";
var _keyEvents = new Array();
var isdrag=false;
var x = 0,y = 0;
var dobj;
var cobj;
var bobj;
var bx = 0, by = 0;
var cx = 0, cy = 0;

var _popEvents = new Array();
var popupControls = new Array();
var _pagePath;

//XML FIX
var BrowserIE = (window.navigator.userAgent.indexOf("MSIE") > 0); //Must be declared here;
if (document.implementation.hasFeature("XPath", "3.0") && !BrowserIE)
{
    if (typeof XMLDocument == "undefined") { XMLDocument = Document; }
    XMLDocument.prototype.selectNodes = function(cXPathString, xNode)
    {
        if (!xNode) { xNode = this; }
        var oNSResolver = this.createNSResolver(this.documentElement)
        var aItems = this.evaluate(cXPathString, xNode, oNSResolver, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null)
        var aResult = [];
        for (var i = 0; i < aItems.snapshotLength; i++) { aResult[i] = aItems.snapshotItem(i); }
        return aResult;
    }
    XMLDocument.prototype.selectSingleNode = function(cXPathString, xNode)
    {
        if (!xNode) { xNode = this; }
        var xItems = this.selectNodes(cXPathString, xNode);
        if (xItems.length > 0) { return xItems[0]; }
        else { return null; }
    }
    Element.prototype.selectNodes = function(cXPathString)
    {
        if (this.ownerDocument.selectNodes) { return this.ownerDocument.selectNodes(cXPathString, this); }
        else { throw "For XML Elements Only"; }
    }
    Element.prototype.selectSingleNode = function(cXPathString)
    {
        if (this.ownerDocument.selectSingleNode) { return this.ownerDocument.selectSingleNode(cXPathString, this); }
        else { throw "For XML Elements Only"; }
    }
    Element.prototype.__defineGetter__("text",
              function() { return (this.textContent); });
    Element.prototype.__defineSetter__("text",
              function(txt) { this.textContent = txt; });


}
//multi Nodes for xpath 
//old name executeXpathSelectNodes;
function $mn(xmlDocument, xp)
{
    var ResultNodes = xmlDocument.selectNodes(xp);
    return ResultNodes;
}

//Single Node for xpath
//old name executeXpathSingleNode
function $sn(xmlDocument, xp)
{
    var ResultNode = xmlDocument.selectSingleNode(xp);
    return ResultNode;
}
//END XML FIX 
function ActivateGlobalKeys(DivId)
{
    if (DivId != "document")
    {
        $addEvent($id(DivId),"keydown",ExecuteKeyPressEvent);
    }
    else
    {
        $addEvent(document,"keydown",ExecuteKeyPressEvent);
    }
    ClearKeyEvents(); 
}

function AddKeyEvents(Key,Event,Cntrl,DivId)
{
    var cntrl = false;
    
    if (Cntrl != null)
    {
        cntrl = Cntrl;
    }
    
    if (DivId != null)
    {
        ActivateGlobalKeys(DivId)
    }
    _keyEvents[Key] = Event + "|" + cntrl;
}

function ClearKeyEvents()
{
    _keyEvents = new Array();
}

function Key_size(Keys){
    var size = 0;
    for (var i in Keys) {
        if (_keyEvents[i] != null) 
            size ++;
    }
    return size;
}

function GetKeyPressed(e)
{
    if (!e) 
    {
        if (window.event) 
        {
            //Internet Explorer
            e = window.event;
        } 
        else 
        {
            return;
        }
    }
  
    if (typeof( e.keyCode ) == 'number') 
    {
        //DOM
        e = e.keyCode;
    } 
    else if( typeof( e.which ) == 'number' ) 
    {
        //NS 4 compatible
        e = e.which;
    } 
    else if( typeof( e.charCode ) == 'number'  ) 
    {
        //also NS 6+, Mozilla 0.9+
        e = e.charCode;
    } 
    else 
    {
        //total failure, we have no way of obtaining the key code
        return;
    }
    return e;
}

function CheckCtrlKey(e)
{
    var ctrlKeyPressed = false;
    if (!e) 
    {
        if (window.event) 
        {
            //Internet Explorer
            e = window.event;
        } 
        else 
        {
            return;
        }
    }
  
    try
    {
        ctrlKeyPressed =  e.ctrlKey;
    }
    catch(ex)
    {}
    return ctrlKeyPressed;
}

function ExecuteKeyPressEvent(e) 
{
    var validKey = false;
    if (parseInt(Key_size(_keyEvents)) != 0)
    {
        var keypressed = GetKeyPressed(e);
        var ctrlpressed = CheckCtrlKey(e);
        if (_keyEvents[keypressed]) 
        {
            try
            {
                if (_keyEvents[keypressed].split("|")[1] == "true")
                {
                    if (ctrlpressed)
                    {
                        validKey = true; 
                    }
                }
                else
                {
                    validKey = true;
                }
                if (validKey)
                {
                    eval(_keyEvents[keypressed].split("|")[0]);
                    return false;
                }
            }
            catch(ex)
            {}
        }
    }
}

function ExecuteKeyPressEventPopUp(e)
{
    if (parseInt(Key_size(_popEvents)) != 0)
    {
        var keypressed = GetKeyPressed(e) 
        if (_popEvents[keypressed]) 
        {
            try
            {
                eval(_popEvents[keypressed]);
            }
            catch(ex)
            {}
            return false;
        }
    }
}

//document.getElementByID
function $id(id) 
{
	return document.getElementById(id);
}
function $tn(id)
{
    return document.getElementsByName(id);
}
 
// evaluateXPath
// $xpath
function $xpath(aNode, aExpr) 
{
	var xpe = new XPathEvaluator();
	var nsResolver = xpe.createNSResolver(aNode.ownerDocument == null ? aNode.documentElement : aNode.ownerDocument.documentElement);
	var result = aNode.evaluate(aExpr, aNode, null, 0, null);
	var found = [];
	var res;
	while (res = result.iterateNext())
	{
		found.push(res);
    }
	return found;
}


function $SingleNodeMoz(aNode,sXPath)
{
	var xpe = new XPathEvaluator();
    var oResult = xpe.evaluate(sXPath, aNode, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
    //weird.. oResult is always object..
    if(oResult !=null)
    {
        if(oResult.singleNodeValue != null)
        {
            return oResult;
        }
        else
        {
           return null;
        }
    }
    else
    {
        return null;
    }
    /*if (oResult != null) 
    {
		return oResult;
    } 
    else 
    {
		return null;
    }  */   
}

//create Xml document
//createXMLObject is the old name
function $xml(xmlToLoad)
{
	var xmldoc;
    if (document.implementation && document.implementation.createDocument) 
	{
		var oXML = xmlToLoad;
		var parser = new DOMParser();
		var xmldoc = parser.parseFromString(oXML, "text/xml");
	} 
	else if (window.ActiveXObject) 
    {
		xmldoc = new ActiveXObject("Microsoft.XMLDOM");
        if(xmlToLoad != "")
        {
			xmldoc.loadXML(xmlToLoad);
		} 
	}
	return xmldoc;
}

//get Attribute for xml node 
//pass the xml node  <node surname="">
//AttrToGet to get , ex surname
function $xa(xmlNode,AttrToGet)
{
	if(BrowserIE)
    {
		if(xmlNode !=null)
        {
			return xmlNode.getAttribute(AttrToGet);
        }
        else
        {
			return null;
        }
	}
    else
    {
		//return xmlNode.singleNodeValue.attributes[AttrToGet].nodeValue
	    if(xmlNode == null)
	    {
	        return null;
	    }
	    if(xmlNode.toString() == "[object XPathResult]")
	    {
	    
	        if(xmlNode.singleNodeValue.nodeValue == null)
	        {
	            return null;
	        }
	        else
	        {
	            return xmlNode.singleNodeValue.attributes[AttrToGet].nodeValue;
	        }
	    }
	    else
	    {
	        if(xmlNode.attributes[AttrToGet] == null)
	        {
	            return null;
	        }
	        else
	        {
	            return xmlNode.attributes[AttrToGet].nodeValue;
	        }
	    }
	}
}

//get Attribute for xml node 
//pass the xml node  <node surname="">
//AttrToGet to get , ex surname
function $xaparent(xmlNode,AttrToGet)
{
	if(BrowserIE)
    {
		if(xmlNode !=null)
        {
			return xmlNode.parentNode.getAttribute(AttrToGet);
		}
        else
        {
			return null;
		}
	}
    else
    {
		return xmlNode.singleNodeValue.parentNode.attributes[AttrToGet].nodeValue
    }
}

// old name getTextFromXmlNodeSingle
//returns text from single node 
function $xt(XmlNode)
{
    var ret = "";
    if(BrowserIE)
    {
        ret = XmlNode.childNodes[0].nodeValue;
    }
    else
    {
        ret = XmlNode.singleNodeValue.textContent ; //XmlNode[0].childNodes[0].nodeValue;
    }
    return ret;
}  

// old name replace
// replaces text in a string
function $rep(string, text, by) 
{
    // Replaces text with by in string
    var strLength = string.length;
    var txtLength = text.length;
    
    if ((strLength == 0) || (txtLength == 0)) 
        return string;

    var i = string.indexOf(text);
    
    if ((!i) && (text != string.substring(0, txtLength))) 
        return string;
        
    if (i == -1) 
        return string;

    var newstr = string.substring(0, i) + by;

    if (i + txtLength < strLength)
        newstr += $rep(string.substring(i + txtLength, strLength), text, by);

    return newstr;
}

function $togglePopUp(div, frame, viewState, widthType)
{
	var state = false;
    var IfrRef = $id(frame);
    
   if (IfrRef == null) 
   {
        return;
   }
    IfrRef.style.display = "none";  
    
    div.style.zIndex = 1001;
   	
    if (viewState == "") 
    {
	    if (div.style.display == "none") 
	    {
		    state = true;
	    } 
	    else 
	    {
		    state = false;
	    }    
    }
    else
    {
        if (viewState == "none")
        {
		    state = false;        
        }
        else
        {
	        if (div.style.display == "none") 
	        {
		        state = true;
	        } 
	        else 
	        {
		        state = false;
	        }            
        }
    }

   	if(state)
   	{
        div.style.display = "block";
        if (widthType)
        {
            IfrRef.style.width = div.style.width;
    	    IfrRef.style.height = div.style.height;
    	}
    	else
    	{
    	    IfrRef.style.width = div.clientWidth;
    	    IfrRef.style.height = div.clientHeight;
    	}
    	IfrRef.style.top = div.offsetTop;
    	IfrRef.style.left = div.offsetLeft;
    	IfrRef.style.zIndex = div.style.zIndex - 1;
    	IfrRef.style.display = "block";
    }
   	else
   	{
        div.style.display = "none";
    	IfrRef.style.display = "none";
    	IfrRef.style.width = "0px";
    	IfrRef.style.height = "0px";
    	IfrRef.style.top = 0;
    	IfrRef.style.left = 0;
  	}
}

//popupdiv for view
function doViewPopup(headingText, contentText, viewID)
{
//    busyDivTag();
//    var HeadingText = headingText;
//    var ContentText = contentText;
    var viewText = $id(viewID);
    var viewHeight = viewText.childNodes[0].childNodes[0].offsetHeight;
    var viewWidth = viewText.childNodes[0].childNodes[0].offsetWidth;
    
    if  ((viewHeight == 0) && (viewWidth == 0))
    {
		viewText.style.display = "";
		viewHeight = viewText.childNodes[0].childNodes[0].offsetHeight;
		viewWidth = viewText.childNodes[0].childNodes[0].offsetWidth;
		viewText.style.display = "none";
    }
   
     if  (viewWidth < 500)
    {
        var maintbl  = ReplaceID(viewID, "part_","maintbl_");
        ($id(maintbl)).width = "500px"; 
		viewText.style.display = "";
		viewWidth = viewText.childNodes[0].childNodes[0].offsetWidth;
		viewText.style.width = "500px";
		viewText.style.display = "none";
    } 
//            
//    var screenL = window.screen.availWidth - viewWidth;
//    var screenH = window.screen.availHeight - viewHeight;
//    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
//    var newDiv = false;
//    var st = "top:" + (parseInt(top) - 40) + "px;z-index:9018;position:absolute;left:" + (parseInt(left) - 50) + "px;width:" + (viewWidth) + "px;height:" + (viewHeight) + "px";
//    
//    viewText.style.cssText = st;
//    
//    var divAlert = "";
//    var divExists = $id("containerView");
//    if(divExists == null)
//    {
//		divAlert = document.createElement("DIV");
//		divAlert.id = "containerView";
//		divAlert.className="dragme";
//        divAlert.attachEvent("onmousedown",selectmouse);
//        divAlert.attachEvent("onmouseup",new Function("isdrag=false"));
//		newDiv= true;
//    }
//    else
//    {
//		newDiv = false;
//		divAlert = test;
//    }
//    
//    var stDiv = "top:" + (parseInt(top) - 80) + "px;z-index:9018;position:absolute;left:" + (parseInt(left) - 60) + "px;width:" + (viewWidth+20) + "px;height:" + (viewHeight+20) + "px";
//    var stIframe = "top:" + (parseInt(top) - 70) + "px;z-index:9017;position:absolute;left:" + (parseInt(left) - 50) + "px;width:" + (viewWidth+5) + "px;height:" + (viewHeight+30) + "px;frameborder=0;";
//    
//    divAlert.style.cssText = "position:absolute;display:none;z-index:9019;" + stDiv;
//    divAlert.innerHTML += '<table id="tblContainerView" cellpadding="0" cellspacing="0" border="0"><tr><td><div id="poptl" class="poptl"></div></td><td><div id="poptc" class="poptc"><div style="font-family:Arial;padding-top:5px;color:White;font-weight:bold;">' + HeadingText + '</div></div></td><td><div id="poptr" class="poptr"></div></td></tr><tr><td><div id="popcl" class="popcl"></div></td><td align="center"><div id="popcc" class="popcc">' + ContentText + '</div></td><td><div id="popcr" class="popcr"></div></td></tr><tr><td><div id="popbl" class="popbl"></div></td><td><div id="popbc" class="popbc"></div></td><td><div id="popbr" class="popbr"></div></td></tr></table>';
//   
//    if(newDiv == true)
//    { 
//        document.body.appendChild(divAlert);
//    }
//    
//    //add iframe
//    var ifr = $id("iframepopup");
//    var table = $id("tblContainerView");
//    var stIf = "top:" + (parseInt(top) - 80) + "px;z-index:9017;position:absolute;left:" + (parseInt(left) - 90) + "px;width:" + (viewWidth+20) + "px;height:" + (viewHeight+20) + "px";
//  
//    $DivSetVisible(true,table,ifr,stIframe);
//    ifr.frameBorder="no";
//    ifr.style.backgroundColor = "green";
//    ifr.className="dragmeIfr"
//    var thediv = "";
//    thediv = document.getElementById("containerView");
//    thediv.style.display = "block";
//    
//    document.getElementById("poptc").style.width=viewWidth+'px';
//    document.getElementById("popcc").style.width=viewWidth+'px';
//    document.getElementById("popbc").style.width=viewWidth+'px';
//    document.getElementById("popcl").style.height=viewHeight+'px';
//    document.getElementById("popcc").style.height=viewHeight+'px';
//    document.getElementById("popcr").style.height=viewHeight+'px';
 
    $showPopup(headingText, contentText, viewID, viewHeight, viewWidth, "", "closeSubForm()", "", "", "", "", "", "", "", "", "");
    showSelectSpecific(viewID);
}

function doViewHidePopup()
{
    HideDisableDiv();
    var thediv = document.getElementById("containerView");
    
    if (thediv != null && thediv != "")
    {
        thediv.style.display = "none";
        thediv.parentNode.removeChild(thediv);
    }
    
}

// Removes leading whitespaces
function $LTrim( value ) 
{
	var re = /\s*((\S+\s*)*)/;
	return value.replace(re, "$1");
}

// Removes ending whitespaces
function $RTrim( value ) 
{
	var re = /((\s*\S+)*)\s*/;
	return value.replace(re, "$1");
}

// Removes leading and ending whitespaces
function $Trim( value ) 
{
	return $LTrim($RTrim(value));
}

//popup for alert in Iframe
function doPopupIframe(headingText, contentText, twidth , theight, btnValue1, fn1, btnValue2, fn2, btnValue3, fn3,zIndexSpec,zID, extraPath, helpID, popupImage)
{
//    if (!extraPath)
//   {
//        extraPath = "";
//   } 
//    //jvz : zIndexSpec is overloaded for popups on each other
//    if(zIndexSpec=="undefined")
//    {
//        busyDivTag();
//    }
//    else
//    {
//         busyDivTag(zIndexSpec,zID);
//    }
//    var HeadingText = headingText;
//    var ContentText = contentText;
//    var divAlert = document.createElement("DIV");
//    var inpValue = "";
//    var strValue = "";
//    var screenL = document.documentElement.offsetWidth - twidth ;
//    var screenH = document.documentElement.offsetHeight - theight;
////   if (infoText.length > 0)
////	{
////	    screenH = screenH - 110;
////	}
////	else
////	{
//	    screenH = screenH - 150;
////	} 
//    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
//    
//    
////    if (btnValue1 != "")
////    {
////        inpValue += '<input type="button" class="button" id="' + btnValue1 + '" value="' + btnValue1 + '" onclick=' + fn1 + '>&nbsp;';
////    }
////    
////    if (btnValue2 != "")
////    {
////        inpValue += '<input type="button" class="button" id="' + btnValue2 + '" value="' + btnValue2 + '" onclick=' + fn2 + '>&nbsp;';
////    }
////    
////    if (btnValue3 != "")
////    {
////        inpValue += '<input type="button" class="button" id="' + btnValue3 + '" value="' + btnValue3 + '" onclick=' + fn3 + '>';
////    }
////    
////    strValue = inpValue;
// 
    var buttonvalues = btnValue1 + "☺" +  fn1 ;
   if (btnValue2.length > 0)
   { 
        buttonvalues += "☺" +  btnValue2 + "☺" +  fn2 ;
   } 
   if (btnValue3.length > 0)
   {
        buttonvalues += "☺" +  btnValue3 + "☺" +  fn3 ;
   }
    
//    var stDiv = "top:" + parseInt(top - 5) + "px;left:" + parseInt(left  -5) + "px;width:" + parseInt(twidth  + 10) + "px;height:" + parseInt(theight + 10) + "px;"
//    
//    divAlert.style.cssText = "position:absolute;display:none;z-index:10020;" + stDiv;
//    //divAlert.innerHTML += '<table id="tblContainer" cellpadding="0" cellspacing="0"><tr><td><div id="poptlA" class="poptl"></div></td><td><div id="poptcA" class="poptc"><div valign="middle" align="left" class="smllabelboldwhite" style="font-family:Arial;padding-top:15px;color:White;font-weight:bold;">' + HeadingText + '</div></div></td><td><div id="poptrA" class="poptr"></div></td></tr><tr><td><div id="popclA" class="popcl"></div></td><td align="center"><div id="popccA" class="popcc"><table cellspacing="0" cellpadding="0" style="font-family:Arial;padding-top:15px;font-size:12px;"><tr><td>' + ContentText + '</td></tr><tr><td align="right">' + strValue + '</td></tr></table></div></td><td><div id="popcrA" class="popcr"></div></td></tr><tr><td><div id="popblA" class="popbl"></div></td><td><div id="popbcA" class="popbc"></div></td><td><div id="popbrA" class="popbr"></div></td></tr></table>';
//   divAlert.innerHTML += generatePopupHTML('tblContainer', HeadingText, '',  ContentText, buttonvalues, theight, twidth, '', '', "","",  extraPath);
//    divAlert.className="dragme";
//    divAlert.onmousedown=selectmouse;
//    divAlert.onmouseup=new Function("isdrag=false");    
//    divAlert.id = "container";
//        
//    document.body.appendChild(divAlert);
//    
//    //add iframe
//    //var ifr = $id("iframepopup");
//    var ifr = document.createElement("IFRAME");
//    ifr.id = "iframepopups"
//    ifr.frameBorder = "0";
//    ifr.className="dragmeIfr";
//    var table = $id("tblContainer");
//    //var st = "width:" + twidth + "px; height:" + theight + "px;";
//    var st = "top:" + parseInt(top) + "px;z-index:10019;position:absolute;left:" + parseInt(left) + "px;width:" + (twidth) + "px;height:" + (theight) + "px";
//    
//    divAlert.parentElement.appendChild(ifr);
//     
//    $DivSetVisible(true,table,ifr,st);
// 
//    var thediv = document.getElementById("container");
//    thediv.style.display = "block";
//        
////    document.getElementById("poptcA").style.width=twidth+'px';
////    document.getElementById("popccA").style.width=twidth+'px';
////    document.getElementById("popbcA").style.width=twidth+'px';
////    document.getElementById("popclA").style.height=theight+'px';
////    document.getElementById("popccA").style.height=theight+'px';
////    document.getElementById("popcrA").style.height=theight+'px';
if (!popupImage)
{
    popupImage = "";
}
if (!helpID)
{
    helpID = "";
}
    $showPopup(headingText, contentText, "", theight, twidth, "", "doHidePopup()", buttonvalues, zIndexSpec, zID, extraPath, "", helpID, "", "",popupImage)
}
//folowing two functions are used to make the popups dragable
function movemouse(e)
{
    var obj = null;
    var i = true;
   var eventclientX = event.clientX;
   var eventclientY = event.clientY; 
    if (isdrag)
    {
    //checks if the window has content ie. which type of popup?
        if(dobj != null)
        {
            dobj.style.left = parseInt(tx) + parseInt(eventclientX) - parseInt(x);
            dobj.style.top  = parseInt(ty) + parseInt(eventclientY) - parseInt(y);
            obj = dobj;
         }
         else if(bobj != null)
        {
            bobj.style.left = parseInt(tx) + parseInt(eventclientX) - parseInt(x);
            bobj.style.top  = parseInt(ty) + parseInt(eventclientY) - parseInt(y);
            obj = bobj;
          }
         
         if (obj != null)
         { 
            while(i==true)
            {
                if(obj.className!="dragmeIfr")
                {
                     if (obj.nextSibling != null)
                    {
                        obj = obj.nextSibling;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    i = false;
                }
            }
            if (obj != null)
            {
                obj.style.left = parseInt(tx) + parseInt(eventclientX) - parseInt(x) +12;
                obj.style.top = parseInt(ty) + parseInt(eventclientY) - parseInt(y) +22;
            }
             if (cobj != null)
            { 
                if (isNaN(cx)) cx = 0;
                if (isNaN(cy)) cy = 0;
                         
                cobj.style.left = parseInt(cx) + parseInt(eventclientX) - parseInt(x);
                cobj.style.top = parseInt(cy) + parseInt(eventclientY) - parseInt(y);
               
            }
            return false;
        }
    }
   
}

function selectmouse(e) 
{

    cobj = popupControls[popupControls.length-1];

  var fobj = event.srcElement; 
  var topelement = "BODY";
  var aobj = event.srcElement;
  
  if ((fobj != null) && (aobj != null))
  {
      while (fobj.tagName != topelement && fobj.className!="dragme")
      {
            fobj = fobj.parentElement;
      }
      while(aobj.tagName != topelement && aobj.className!="dragmain")
      {
        aobj=aobj.parentElement;
      } 
      if (fobj.className=="dragme")
      {
        aobj=null;
        bobj=null;
        isdrag = true;
        dobj = fobj;
        tx = parseInt(dobj.style.left+0);
        ty = parseInt(dobj.style.top+0);
        x = event.clientX;
        y = event.clientY;
        if (cobj)
        {
            cx = parseInt(cobj.style.left);
            cy = parseInt(cobj.style.top);
        }
        document.onmousemove=movemouse;
        return false;
      }
      if (aobj.className == "dragmain")
      {
        fobj = null;
        dobj = null;
        bobj = aobj;
        isdrag = true;
        tx = parseInt(bobj.style.left+0);
        ty = parseInt(bobj.style.top+0);
        x = event.clientX;
        y = event.clientY;
        if (cobj)
        {
            cx = parseInt(cobj.style.left);
            cy = parseInt(cobj.style.top);
        }
        document.onmousemove=movemouse;
        return false;  
      }
    }

}

//popup for alert
function doPopup(headingText, contentText, twidth , theight, btnValue1, fn1, btnValue2, fn2, btnValue3, fn3,zIndexSpec,zID, extraPath, helpID, popupImage)
{
//    //jvz : zIndexSpec is overloaded for popups on each other
//    if(zIndexSpec=="undefined")
//    {
//        busyDivTag();
//    }
//    else
//    {
//         busyDivTag(zIndexSpec,zID);
//    }
//    var HeadingText = headingText;
//    var ContentText = contentText;
//    var divAlert ;//= document.createElement("DIV");
//    var inpValue = "";
//    var strValue = "";
//    var screenL = window.screen.availWidth - twidth ;
//    var screenH = window.screen.availHeight - theight;
//   //   if (infoText.length > 0)
////	{
////	    screenH = screenH - 110;
////	}
////	else
////	{
//	    screenH = screenH - 150;
////	}  
//    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
//    
//    
////    if (btnValue1 != "")
////    {
////        if (fn1.split("|")[1] != null)
////        {
////            _popEvents[fn1.split("|")[1]] = fn1.split("|")[0];
////            fn1 = fn1.split("|")[0];            
////        }
////        inpValue += '<input type="button" class="button" id="' + btnValue1 + '" value="' + btnValue1 + '" onclick=' + fn1 + '>&nbsp;';
////    }
////    
////    if (btnValue2 != "")
////    {
////        if (fn2.split("|")[1] != null)
////        {
////            _popEvents[fn2.split("|")[1]] = fn2.split("|")[0];
////            fn2 = fn2.split("|")[0];            
////        }    
////        inpValue += '<input type="button" class="button" id="' + btnValue2 + '" value="' + btnValue2 + '" onclick=' + fn2 + '>&nbsp;';
////    }
////    
////    if (btnValue3 != "")
////    {
////        if (fn3.split("|")[1] != null)
////        {
////            _popEvents[fn3.split("|")[1]] = fn3.split("|")[0];
////            fn3 = fn3.split("|")[0];            
////        }    
////        inpValue += '<input type="button" class="button" id="' + btnValue3 + '" value="' + btnValue3 + '" onclick=' + fn3 + '>';
////    }
////    
////    strValue = inpValue;
//    
//    var stDiv = "top:" + parseInt(top-5) + "px;left:" + parseInt(left-5) + "px;width:" + parseInt(twidth+10) + "px;height:" + parseInt(theight+10) + "px;"
//    
//   if ($id("container"))
//   {
//        divAlert = ($id("container"));
//   }
//   else
//   {
//       divAlert = document.createElement("DIV"); 
//       divAlert.id = "container";
//       divAlert.className = "dragmain";
//       divAlert.attachEvent("onmousedown",selectmouse);
//       divAlert.attachEvent("onmouseup",new Function("isdrag=false"));
//       divAlert.attachEvent("onkeydown",ExecuteKeyPressEventPopUp);
//       document.body.appendChild(divAlert);
//   } 
//    
//    
    var buttonvalues = btnValue1 + "☺" +  fn1 ;
   if (btnValue2.length > 0)
   { 
        buttonvalues += "☺" +  btnValue2 + "☺" +  fn2 ;
   } 
   if (btnValue3.length > 0)
   {
        buttonvalues += "☺" +  btnValue3 + "☺" +  fn3 ;
   }
//    //divAlert.innerHTML = '<table id="tblContainer" cellpadding="0" cellspacing="0"><tr><td><div id="poptlA" class="poptl" /></td><td><div id="poptcA" class="poptc"><div valign="middle" align="left" class="smllabelboldwhite" style="font-family:Arial;padding-top:15px;color:White;font-weight:bold;">' + HeadingText + '</div></div></td><td><div id="poptrA" class="poptr" /></td></tr><tr><td><div id="popclA" class="popcl" /></td><td align="center"><div id="popccA" class="popcc"><table cellspacing="0" cellpadding="0" style="font-family:Arial;padding-top:2px;font-size:12px;"><tr><td><div style="overflow:auto;"><table cellspacing="0" cellpadding="0"><tr><td>' + ContentText + '</td></tr></table></div></td></tr><tr><td align="right">' + strValue + '</td></tr></table></div></td><td><div id="popcrA" class="popcr" /></td></tr><tr><td><div id="popblA" class="popbl" /></td><td><div id="popbcA" class="popbc" /></td><td><div id="popbrA" class="popbr" /></td></tr></table>';
//   divAlert.innerHTML = generatePopupHTML('tblContainer',HeadingText, '', ContentText, buttonvalues, theight, twidth, '', '');
//   divAlert.style.cssText = "position:absolute;display:none;z-index:10020;" + stDiv;
//            
//    //add iframe
//    //var ifr = $id("iframepopup");
//   var ifr;
//   if ($id("iframepopups"))
//   {
//     ifr = ($id("iframepopups"));
//   } 
//   else
//   {
//     ifr = document.createElement("IFRAME");
//     ifr.id = "iframepopups"
//     ifr.className="dragmeIfr";
//     divAlert.parentElement.appendChild(ifr);
//    }
//    ifr.frameBorder = "0";
//    var table = $id("tblContainer");
//    //var st = "width:" + twidth + "px; height:" + theight + "px;";
//    //var st = "top:" + parseInt(top + 10) + "px;z-index:10019;position:absolute;left:" + parseInt(left + 8) + "px;width:" + parseInt(twidth) + "px;height:" + parseInt(theight) + "px";
//   var st = "top:" + parseInt(top) + "px;z-index:10019;position:absolute;left:" + parseInt(left) + "px;width:" + parseInt(twidth-5) + "px;height:" + parseInt(theight-5) + "px"; 
//     
//    $DivSetVisible(true,table,ifr,st);
// 
//    var thediv = document.getElementById("container");
//    thediv.style.display = "block";
//        //divAlert.style.display = "block";
//        
////    document.getElementById("poptcA").style.width=twidth+'px';
////    document.getElementById("popccA").style.width=twidth+'px';
////    document.getElementById("popbcA").style.width=twidth+'px';
////    document.getElementById("popclA").style.height=theight+'px';
////    document.getElementById("popccA").style.height=theight+'px';
////    document.getElementById("popcrA").style.height=theight+'px';
//    divAlert.focus();
    if (!popupImage)
   {
        popupImage = "";
   } 
   if (!helpID)
   {
        helpID = "";
   }
   
   //if zIndex is specified, and the closefunction is not changed, the relevant busydiv is never hidden
   var closeFunction = "doHidePopup()";
   try
   {
       if (zID.length > 0)
       {
            closeFunction = "doHidePopup('" + zID +  "');"
       }
   }
   catch(error)
   {
   }
   
    $showPopup(headingText, contentText, "", theight, twidth, "", closeFunction, buttonvalues,   zIndexSpec, zID , extraPath,"", helpID, "", "", popupImage)       
}

function doHidePopup(ControlID, doHideSpecify)
{
    if (!ControlID)
   {
        ControlID = "";
   } 
   

   if (ControlID.length > 0)
   { 
        HideDisableDiv("busy_" + ControlID); 
        HideDisableDiv( ControlID); 
   }
   else
   { 
       HideDisableDiv();
   }
    var thediv = document.getElementById("container" + ControlID);
   if (!thediv)
   {
        ControlID = "";
        thediv = document.getElementById("container" + ControlID);
   } 
//    $removeEvent(thediv, "mousedown", selectmouse);
//    $removeEvent(thediv, "mouseup", new Function("isdrag=false"));
//    $removeEvent(thediv, "keydown", ExecuteKeyPressEventPopUp) ;   
    popupControls.pop(); 
   
//      var oldControl = popupControls[popupControls.length-1];
//      if (oldControl)
//      {
//        $addEvent(oldControl, "mousedown", selectmouse);
//        $addEvent(oldControl, "mouseup", new Function("isdrag=false"));
//        $addEvent(oldControl, "keydown", ExecuteKeyPressEventPopUp) ;  
//      
//         if ($id("container"+ oldControl.id))
//          {
//            oldControl = ($id("container"+ oldControl.id));
//            $addEvent(oldControl, "mousedown", selectmouse);
//            $addEvent(oldControl, "mouseup", new Function("isdrag=false"));
//            $addEvent(oldControl, "keydown", ExecuteKeyPressEventPopUp) ;  
//         } 
//     } 
    
    var ifr = window.parent.document.getElementById("iframepopups" + ControlID) != null ? window.parent.document.getElementById("iframepopups" + ControlID) : document.getElementById("iframepopups" + ControlID);
    
    if (thediv != null && thediv != "")
    {
//        var ifr = $id("iframepopup");
//        ifr.style.display = "none";
        var ifr = window.document.getElementById("iframepopups" + ControlID);
        if (ifr)
        {
            ifr.style.display = "none";
            ifr.parentNode.removeChild(ifr);
        }
        thediv.style.display = "none";
        thediv.parentNode.removeChild(thediv);
    }
   
   if (ControlID.length > 0)
   {
        if ($id(ControlID))
        {
            ($id(ControlID)).style.display = "none";
        }
   } 
//   if ((doHideSpecify != "false") || (doHideSpecify == null))
  if ((doHideSpecify != "false"))
   {
        hidePWait(); 
    }
}

//get Scrolling values for all browsers
function returnScrollDimensions(which) 
{
    //if which = 1 return x, if which = 0 return y
    var scrOfX = 0, scrOfY = 0;
    var d = document;
    if( typeof( window.pageYOffset ) == 'number' ) 
    {
        //Netscape compliant
        scrOfY = window.pageYOffset;
        scrOfX = window.pageXOffset;
    } 
    else if ( document.body  && ( document.body.scrollLeft || document.body.scrollTop ) ) 
    {
        //DOM compliant
        scrOfY = document.body.scrollTop;
        scrOfX = document.body.scrollLeft;
    } 
    else if( document.documentElement && ( document.documentElement.scrollLeft || document.documentElement.scrollTop ) ) 
    {
        //IE6 standards compliant mode
        scrOfY = document.documentElement.scrollTop;
        scrOfX = document.documentElement.scrollLeft;
    }
    if(which) 
    {
        return scrOfX;
    } 
    else 
    {
        return scrOfY;
    }
}	

//do popup control
function $doControlPopup(headingText, contentText, controlID, closeBtn, closeBtnFn, pathExtra)
{
//    if (!pathExtra)
//   {
//        pathExtra = "";
//   } 
//    _controlID = controlID
//    var HeadingText = headingText;
//    var ContentText = contentText;
//    var newDiv = false;
//    var headerClose = "";
//    var headerMain = "";
//    var controlText = $id(controlID);
//    controlText.style.display = "block";
//    
//    var controlValue = "";
//    var controlHeight = (parseInt(controlText.style.height.replace("px","")));
//    var controlWidth = (parseInt(controlText.style.width.replace("px","")));
//    
//    if ((controlHeight == "") || isNaN(controlHeight))
//		controlHeight = controlText.offsetHeight;
//		
//    if ((controlWidth == "") || isNaN(controlWidth))
//		controlWidth = controlText.offsetWidth;	
//		
//	var screenL = window.screen.availWidth - controlWidth ;
//    var screenH = window.screen.availHeight - controlHeight;
////   if (infoText.length > 0)
////	{
////	    screenH = screenH - 110;
////	}
////	else
////	{
//	    screenH = screenH - 150;
////	} 
//    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
//    var divAlert = "";
//    var divExists = $id("containerControl" + controlID); 
//   //control style 
//    //var st = "top:" + (parseInt(top) - parseInt(35)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(25))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight))+"px;";
//   //70 as dit infotext bevat
//   //var st = "top:" + (parseInt(top) + parseInt(70)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(5))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight+10))+"px;"; 
//   //40 as daar nie info is nie
//   var st = "top:" + (parseInt(top) + parseInt(40)) + "px;" + "position:absolute;left:" + (parseInt(left) + parseInt(2))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight ))+"px;"; 
//    
//    if (controlText.style.zIndex == "")
//    {
//         st += "z-index:9020;";
//    }
//    else
//    {
//        st += "z-index:" + controlText.style.zIndex + ";";
//    }
//    
//    if(divExists == null)
//    {
//		divAlert = document.createElement("DIV");
//		divAlert.id = "containerControl" + controlID;
//		divAlert.className = "dragme";
//		divAlert.attachEvent("onmousedown",selectmouse);
//        divAlert.attachEvent("onmouseup",new Function("isdrag=false"));
//		newDiv= true;
//    }
//    else
//    {
//		return;
//    }
//    
//    controlText.style.cssText = "";
//    controlText.style.cssText = st;
//    
//   //div se style 
//    var stDiv = "top:" + (parseInt(top) +parseInt(2)) + "px;" + "position:absolute;left:" + (parseInt(left) +  parseInt(2))  + "px;width:"+(parseInt(controlWidth )) + "px;height:"+(parseInt(controlHeight) )+"px;";
//   //iframe se style 
//    //var stMain = "top:" + (parseInt(top) - parseInt(60)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(25))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight)+20)+"px;";
//   var stMain = "top:" + (parseInt(top) + parseInt(2)) + "px;" + "position:absolute;left:" + (parseInt(left) + parseInt(2))  + "px;width:"+(parseInt(controlWidth - 12)) + "px;height:"+(parseInt(controlHeight) - 4)+"px;"; 
//    
//    var currentIndex = controlText.style.zIndex;    
//    stDiv += "z-index:" + (parseInt(currentIndex)-1) + ";";    
//    stMain += "z-index:" + (parseInt(currentIndex)-2) + ";";

//    busyDivTag((parseInt(currentIndex)-3), "busy_" + controlID);
//    
//    if (closeBtn != false)
//    {
//        headerClose = '<img id="btnClose' + controlID +  '" src="../images/clear.GIF" style="width:15px;height:15px;cursor:hand;">'
//    }
//    
//    //headerMain = '<table cellspacing="0" cellpadding="0" style="table-layout:fixed;"><tr><td valign="middle" align="left" class="smllabelboldwhite"><nobr>' + headingText + '</td><td align="right" style="width:15px;">' + headerClose + '</td></tr></table>';
//    
//    divAlert.style.cssText = "";
//    divAlert.style.cssText = "position:absolute;display:none;" + stDiv;
//    //divAlert.innerHTML += '<table id="tblContainerControl' + controlID + '" cellpadding="0" cellspacing="0" border="0"><tr><td><div id="poptl' + controlID + '" class="poptl"></div></td><td><div id="poptc' + controlID + '" class="poptc"><div style="font-family:Arial;padding-top:15px;color:White;font-weight:bold; width:100%;">' + headerMain + '</div></div></td><td><div id="poptr' + controlID + '" class="poptr"></div></td></tr><tr><td><div id="popcl' + controlID + '" class="popcl"></div></td><td align="center"><div align="center" id="popcc' + controlID + '" class="popcc">' + controlValue + '</div></td><td><div id="popcr' + controlID + '" class="popcr"></div></td></tr><tr><td><div id="popbl' + controlID + '" class="popbl"></div></td><td><div id="popbc' + controlID + '" class="popbc"></div></td><td><div id="popbr' + controlID + '" class="popbr"></div></td></tr></table>';
//   var closeName = "btnClose" + controlID ;
//   divAlert.innerHTML += generatePopupHTML('tblContainerControl' + controlID, headingText, '', controlValue, '', controlHeight + 'px', controlWidth + 'px', closeName, closeBtnFn,'','',pathExtra) 
//    
//    if(newDiv == true)
//    { 
//        //mozilla 
//        //controlText.parentElement.appendChild(divAlert);
//        controlText.parentNode.appendChild(divAlert);
//    }
//    
//    //add iframe
//    var ifr = document.createElement("IFRAME");
//    ifr.id = "containerframe" + controlID;
//    ifr.className = "dragmeIfr";
//    ifr.style.cssText = "";
//    ifr.style.cssText = "display: none;position:absolute";
//    
//    //Jaco Lubbe - Verander vir die property grid se popup
//    //document.body.appendChild(ifr);
//   //mozilla 
//    //controlText.parentElement.appendChild(ifr);
//   controlText.parentNode.appendChild(ifr); 
//    
//    
//    var table = $id("tblContainerControl" + controlID);
//    
//    $DivSetVisible(true,table,ifr,stMain);
//    
//    var thediv = "";
//    var divID = $id("containerControl" + controlID);
//    thediv = divID;
//    thediv.style.display = "block";
//    
////    $id("poptc" + controlID).style.width=controlWidth+'px';
////    $id("popcc" + controlID).style.width=controlWidth+'px';
////    $id("popbc" + controlID).style.width=controlWidth+'px';
////    $id("popcl" + controlID).style.height=controlHeight+'px';
////    $id("popcc" + controlID).style.height=controlHeight+'px';
////    $id("popcr" + controlID).style.height=controlHeight+'px';
//    
//    if (closeBtnFn != "") 
//		document.getElementById("btnClose" + controlID).attachEvent("onclick", closeBtnFn);
    //$showPopup(headingText, contentText, controlID, "", "", "", "doHidePopup()", "", "", "", pathExtra, "", "", "", "")
   $showPopup(headingText, contentText, controlID, "", "", "", "doHidePopup()", "", "", "", pathExtra, "", "", "", "");
    showSelectSpecific(controlID);	    	
}

function $doControlPopupDetail(headingText, contentText, infoText, controlID, buttons, closeBtn, closeBtnFn, pathExtra, helpID, popupImage)
{
//    if (!pathExtra)
//   {
//        pathExtra = "";
//   } 
//    _controlID = controlID
//    var HeadingText = headingText;
//    var ContentText = contentText;
//    var newDiv = false;
//    var headerClose = "";
//    var headerMain = "";
//    var controlText = $id(controlID);
//    controlText.style.display = "block";
//    
//    var controlValue = "";
//    var controlHeight = (parseInt(controlText.style.height.replace("px","")));
//    var controlWidth = (parseInt(controlText.style.width.replace("px","")));
//    
//    if ((controlHeight == "") || isNaN(controlHeight))
//		controlHeight = controlText.offsetHeight;
//		
//    if ((controlWidth == "") || isNaN(controlWidth))
//		controlWidth = controlText.offsetWidth;	
//		
//	var screenL = window.screen.availWidth - controlWidth ;
//	var screenH = window.screen.availHeight - controlHeight;
//	if (infoText.length > 0)
//	{
//	    screenH = screenH - 110;
//	}
//	else
//	{
//	    screenH = screenH - 150;
//	}
//    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
//    var divAlert = "";
//    var divExists = $id("containerControl" + controlID); 
//   //control style 
//    //var st = "top:" + (parseInt(top) - parseInt(35)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(25))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight))+"px;";
//   //70 as dit infotext bevat
//   var st = "";
//   if (infoText.length > 0)
//   {
//    st = "top:" + (parseInt(top) + parseInt(70)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(5))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight+10))+"px;"; 
//   }
//   else
//   {
//   //40 as daar nie info is nie
//    st = "top:" + (parseInt(top) + parseInt(40)) + "px;" + "position:absolute;left:" + (parseInt(left) + parseInt(2))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight ))+"px;"; 
//   }
//    
//    if (controlText.style.zIndex == "")
//    {
//         st += "z-index:9020;";
//    }
//    else
//    {
//        st += "z-index:" + controlText.style.zIndex + ";";
//    }
//    
//    if(divExists == null)
//    {
//		divAlert = document.createElement("DIV");
//		divAlert.id = "containerControl" + controlID;
//		divAlert.className = "dragme";
//		divAlert.attachEvent("onmousedown",selectmouse);
//        divAlert.attachEvent("onmouseup",new Function("isdrag=false"));
//		newDiv= true;
//    }
//    else
//    {
//		return;
//    }
//    
//    controlText.style.cssText = "";
//    controlText.style.cssText = st;
//    
//   //div se style 
//    var stDiv = "top:" + (parseInt(top) +parseInt(2)) + "px;" + "position:absolute;left:" + (parseInt(left) +  parseInt(2))  + "px;width:"+(parseInt(controlWidth )) + "px;height:"+(parseInt(controlHeight) )+"px;";
//   //iframe se style 
//    //var stMain = "top:" + (parseInt(top) - parseInt(60)) + "px;" + "position:absolute;left:" + (parseInt(left)+ parseInt(25))  + "px;width:"+(parseInt(controlWidth)) + "px;height:"+(parseInt(controlHeight)+20)+"px;";
//   var stMain = "top:" + (parseInt(top) + parseInt(2)) + "px;" + "position:absolute;left:" + (parseInt(left) + parseInt(2))  + "px;width:"+(parseInt(controlWidth - 12)) + "px;height:"+(parseInt(controlHeight) - 4)+"px;"; 
//    
//    var currentIndex = controlText.style.zIndex;    
//    stDiv += "z-index:" + (parseInt(currentIndex)-1) + ";";    
//    stMain += "z-index:" + (parseInt(currentIndex)-2) + ";";

//    busyDivTag((parseInt(currentIndex)-3), "busy_" + controlID);
//    
//    if (closeBtn != false)
//    {
//        headerClose = '<img id="btnClose' + controlID +  '" src="../images/clear.GIF" style="width:15px;height:15px;cursor:hand;">'
//    }
//    
//    //headerMain = '<table cellspacing="0" cellpadding="0" style="table-layout:fixed;"><tr><td valign="middle" align="left" class="smllabelboldwhite"><nobr>' + headingText + '</td><td align="right" style="width:15px;">' + headerClose + '</td></tr></table>';
//    
//    divAlert.style.cssText = "";
//    divAlert.style.cssText = "position:absolute;display:none;" + stDiv;
//    //divAlert.innerHTML += '<table id="tblContainerControl' + controlID + '" cellpadding="0" cellspacing="0" border="0"><tr><td><div id="poptl' + controlID + '" class="poptl"></div></td><td><div id="poptc' + controlID + '" class="poptc"><div style="font-family:Arial;padding-top:15px;color:White;font-weight:bold; width:100%;">' + headerMain + '</div></div></td><td><div id="poptr' + controlID + '" class="poptr"></div></td></tr><tr><td><div id="popcl' + controlID + '" class="popcl"></div></td><td align="center"><div align="center" id="popcc' + controlID + '" class="popcc">' + controlValue + '</div></td><td><div id="popcr' + controlID + '" class="popcr"></div></td></tr><tr><td><div id="popbl' + controlID + '" class="popbl"></div></td><td><div id="popbc' + controlID + '" class="popbc"></div></td><td><div id="popbr' + controlID + '" class="popbr"></div></td></tr></table>';
//   var closeName = "btnClose" + controlID ;
//   divAlert.innerHTML += generatePopupHTML('tblContainerControl' + controlID, headingText, infoText, controlValue, buttons, controlHeight + 'px', controlWidth + 'px', closeName, closeBtnFn, "","", pathExtra) 
//    
//    if(newDiv == true)
//    { 
//        //mozilla 
//        //controlText.parentElement.appendChild(divAlert);
//        controlText.parentNode.appendChild(divAlert);
//    }
//    
//    //add iframe
//    var ifr = document.createElement("IFRAME");
//    ifr.id = "containerframe" + controlID;
//    ifr.className = "dragmeIfr";
//    ifr.style.cssText = "";
//    ifr.style.cssText = "display: none;position:absolute";
//    
//    //Jaco Lubbe - Verander vir die property grid se popup
//    //document.body.appendChild(ifr);
//   //mozilla 
//    //controlText.parentElement.appendChild(ifr);
//   controlText.parentNode.appendChild(ifr); 
//    
//    
//    var table = $id("tblContainerControl" + controlID);
//    
//    $DivSetVisible(true,table,ifr,stMain);
//    
//    var thediv = "";
//    var divID = $id("containerControl" + controlID);
//    thediv = divID;
//    thediv.style.display = "block";
//    
////    $id("poptc" + controlID).style.width=controlWidth+'px';
////    $id("popcc" + controlID).style.width=controlWidth+'px';
////    $id("popbc" + controlID).style.width=controlWidth+'px';
////    $id("popcl" + controlID).style.height=controlHeight+'px';
////    $id("popcc" + controlID).style.height=controlHeight+'px';
////    $id("popcr" + controlID).style.height=controlHeight+'px';
//    
//    //if (closeBtnFn != "") 
//	//	document.getElementById("btnClose" + controlID).attachEvent("onclick", closeBtnFn);
    if (!helpID)
   {
        helpID = "";
   } 
   if (!popupImage)
   {
        popupImage = "";
   }
   if (!infoText)
   {
        infoText = "";
   }
    $showPopup(headingText, contentText, controlID, "", "", "", closeBtnFn, buttons, "", "", pathExtra, infoText, helpID, "", "", popupImage);
    showSelectSpecific(controlID);	    	
}

function $doHideControlPopup(controlID)
{
    doHidePopup(controlID);
//    var thediv = $id("containerControl" + controlID);
//    var theControlID = $id(controlID);
    var thediv = window.parent.document.getElementById("containerControl" + controlID) != null ? window.parent.document.getElementById("containerControl" + controlID) : document.getElementById("containerControl" + controlID);
    var theControlID = window.parent.document.getElementById(controlID) != null ? window.parent.document.getElementById(controlID) : document.getElementById(controlID);
    
    if (thediv != null && thediv != "")
    {
        theControlID.style.display = "none";
//        var ifr = $id("containerframe" + controlID);
        var ifr = window.parent.document.getElementById("containerframe" + controlID) != null ? window.parent.document.getElementById("containerframe" + controlID) : document.getElementById("containerframe" + controlID);
        ifr.style.display = "none";
        ifr.parentNode.removeChild(ifr);
        thediv.style.display = "none";
        thediv.parentNode.removeChild(thediv);
    }
   HideDisableDiv("busy_" + controlID);
   hidePWait();
   
}

//subforms;
function $DivSetVisible(state,DivRef,IfrRef,st)
{
    if(state)
    {
        DivRef.style.display = "block";
        IfrRef.style.cssText=st;
        //Jaco Lubbe - Uitgecomment vir Property grid se popup
        //IfrRef.style.zIndex = DivRef.style.zIndex - 1;
        IfrRef.border=0;
        IfrRef.style.display = "block";
    }
    else
    {
        DivRef.style.display = "none";
        IfrRef.style.display = "none";
    }
}

 /* replace function */
function ReplaceID( str, currentvalue, newvalue) 
{  
        str += '';
        var idx = str.indexOf( currentvalue );
        while ( idx > -1 ) {
            str = str.replace( currentvalue, newvalue ); 
            idx = str.indexOf( currentvalue );
        }
    return str;
}

/* return http://hostname/solutionname and passes in the rest of the path to a page */
function buildURL(path)
{
    
   path =  location.pathname.substring(0,location.pathname.indexOf('/',1,0)) + "/" + path
   //changed the buildURL function so the current solution (IIS) name of the project is not hardcoded
    if (path.substring(0,1) == "/")
    {
        return "//" + location.host + path;
        //return "http://" + location.host + path;
    }
    else
    {   
        return "//" + location.host + "/" + path;
        //return "http://" + location.host + "/" + path;
    }
}
    
//Validation for a valid e-mail address
function validateEmail(emailAddress)
{
  var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[(2([0-4]\d|5[0-5])|1?\d{1,2})(\.(2([0-4]\d|5[0-5])|1?\d{1,2})){3} \])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  return re.test(emailAddress);
}

//Validation on special chars
function validateSpecialChars(id, friendlyName, specialChar, allowBlank, extraPath)
{
    var validateResult = ""
    var objValue = "$id('" + id + "').value";
    var _path = "";
    
    if (_pagePath != null)
    {
        _path = _pagePath.replace("../","");
    }
   
   if (extraPath != null)
   {
        _path  = extraPath;
   } 
   
    
    if ($Trim(eval(objValue)).length == "")
    {
       if((allowBlank == null) || (allowBlank == false))
        {
            doPopup(Generic_js_ValidateHeading, Generic_js_ValidateDescription + " '" +  friendlyName + "'.", 400 , 75, Generic_js_ButtonOk, "doHidePopup('valID')|13", "", "", "", "", 11500, "valID", _path,"","error");
            validateResult = false;
            return validateResult;
        }
        else
            validateResult = true;
    }
    
    var iChars = "&!@#$%^&*()+=-[]\\\';,./{}|\":<>?_~";
    iChars = iChars.replace(specialChar,"");

    for (var i = 0; i < eval(objValue).length; i++) 
    {
        if (iChars.indexOf(eval(objValue).charAt(i)) != -1) {
            doPopup(Generic_js_ValidateSpecialCharHeading, Generic_js_ValidateSpecialCharDescription + " '" + friendlyName + "' " + Generic_js_ValidateSpecialCharDescription1 + "  '" + eval(objValue).charAt(i) + "'.<br>" + Generic_js_ValidateSpecialCharDescription2, 400 , 70, Generic_js_ButtonOk, "doHidePopup('valIDSpec')|13", "", "", "", "", 11500, "valIDSpec", _path, "", "warning");
            validateResult = false;
            return validateResult;
        }
        else
        {
            validateResult = true;
        }
    }
    return validateResult;
}

//check a string for special chars and return a 'true' or 'false'
//set a space delimited exclude list if you need to
function containsSpecialChars(inputString, excludeChars)
{
    var validateResult = false;
    var specialChars = "&@#$%?!^*()+=-[]\\';,./{}|:<>_~";
    var chinSpecialChars1 = "≈≠＝≤≥≤＜＞≮≯∷±＋－×÷∫∮∝∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√";
    var chinSpecialChars2 = "┼┽┾┿╀╁╂╃┬┭┮┯┰┰┱┱┲┳├┝┞┟┠┡┢┣┍┎┏┐┑┒┓┒─┄";
    var chinSpecialChars3 = "§№☆★○●◎◇◆□℃‰■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀";
    var chinSpecialChars4 = "〖〗【】［］｛｝《》「」『』々‖…—•ˉ〃";
    var chinSpecialChars5 = "ㄅㄉˇˋㄓˊˊ˙˙ㄚㄞㄢㄆㄊㄍㄐㄔㄗㄧㄛㄟㄣㄇㄋㄎㄑㄕㄘㄨㄜㄠㄈㄌㄏㄒㄖㄙㄩㄝㄡㄥ";
    
    var ExcludeList = excludeChars.split(" ");

    //remove excluded chars form list    
    for (var k = 0; k < ExcludeList.length; k++)
    {
        specialChars = specialChars.replace(ExcludeList[k],"");
        chinSpecialChars1 = chinSpecialChars1.replace(ExcludeList[k],"");
        chinSpecialChars2 = chinSpecialChars2.replace(ExcludeList[k],"");
        chinSpecialChars3 = chinSpecialChars3.replace(ExcludeList[k],"");
        chinSpecialChars4 = chinSpecialChars4.replace(ExcludeList[k],"");
        chinSpecialChars5 = chinSpecialChars5.replace(ExcludeList[k],"");

    }
    
    for (var i = 0; i < inputString.length; i++) 
    {
        if (specialChars.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars1.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars2.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars3.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars4.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars5.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
    }
    return validateResult;
}

 function addbookmark()
{
     if (document.all)
          window.external.AddFavorite(location,'WorkSpace')
}

//Fix texttoplace in for 1st time its created; 
function createPWait(texttoplace, zIndexValue)
{
    var divt = $id("div_SAVING");
    if(divt == null)
    {
        divt = document.createElement("DIV");
    var left =  (document.documentElement.clientWidth - 64) / 2;
      if (left <= 0)
      {
        left = (document.body.clientWidth - 64) / 2;
      }
       var top =  (document.documentElement.clientHeight  - 64) / 2; 
      if (top <= 0)
      {
        top = (document.body.clientHeight - 64) / 2;
      }
       // divt.style.cssText = "background:whitesmoke;border:1px solid gray;text-align: center;vertical-align: middle; line-height: normal; letter-spacing: normal;width: 381px; height: 51px; z-index: 1010; left: 390px; position: absolute; top: 209px;";
      divt.style.cssText = "border:none;width: 32px; height: 32px; z-index: 1010; left:" + left + "px; position: absolute; top: " + top +"px;"; 
        divt.innerHTML = textDiv(texttoplace); 
        //divt.style.zIndex=12500;
        divt.style.zIndex = zIndexValue;
        divt.id = "div_SAVING";
        
        document.body.appendChild(divt);                
    }
 
}

function textDiv(eventtodisplay)
{
   eventtodisplay = (typeof(eventtodisplay) == 'undefined') ? "" : " : " +  eventtodisplay
   
   //var d= "<img src='../images/please_wait.gif'>" + "<br />"+ "Please wait  " +  eventtodisplay + "<br />";
    var d= "<img id='imgBigGreenRot' src='../images/Green_Big_Rotate.gif'>";

   return d;
}

//duplicate
function showPWait(texttoplace, zIndex)
{
    var divt = $id("div_SAVING");
     var ifr = $id("iframepopup");
       
    if(divt != null)
    {
       
//        //divt.style.zIndex=9080;
//        //Port_togglePopUpMenuDisplay(divt,"hiddenframe",true);
//        $togglePopUp(divt, "hiddenframe", true);
//        divt.innerHTML = textDiv(texttoplace);
//        
//        divt.style.display = "inline";
        divt.parentNode.removeChild(divt);
    }
    
    
    
    
    var divtest = $id("div_SAVING");
   if (zIndex != null)
   {
        createPWait(texttoplace, (parseInt(zIndex) + 100));
        busyDivTag(zIndex);
   } 
   else
   {
        createPWait(texttoplace, 11500);
        busyDivTag(11400);
    }
   _canload_Portal = false; 
}

function showPWaitIframe(texttoplace)
{
    var divt = $id("div_SAVING");
    if(divt != null)
    {
        divt.parentNode.removeChild(divt); 
     }
        divt = document.createElement("DIV");
//       var left = (document.documentElement.offsetWidth) /2;
//       var top = (document.documentElement.offsetHeight) / 3; 
     var left =  (document.documentElement.clientWidth - 64) / 2;
      if (left <= 0)
      {
        left = (document.body.clientWidth - 64) / 2;
      }
       var top =  (document.documentElement.clientHeight  - 64) / 2; 
      if (top <= 0)
      {
        top = (document.body.clientHeight - 64) / 2;
      }
       // divt.style.cssText = "background:whitesmoke;border:1px solid gray;text-align: center;vertical-align: middle; line-height: normal; letter-spacing: normal;width: 381px; height: 51px; z-index: 1010; left: 390px; position: absolute; top: 209px;";
      divt.style.cssText = "border:none;width: 32px; height: 32px; z-index: 1010; left:" + left + "px; position: absolute; top: " + top +"px;"; 
        divt.innerHTML = textDiv(texttoplace); 
        divt.style.zIndex=9031;
        divt.id = "div_SAVING";
        
        document.body.appendChild(divt);                
     busyDivTag();
   _canload_Portal = false; 
 
}

//duplicate
function hidePWait()
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

function setFormName(customFormName)
{
    _formName = customFormName;
}

//Jvz : File and image upload div
function busyDivTagTester(zIndex, ID,Con)
{
    var d;
    var newDiv = false;
   
    if((ID != null) && (ID.length > 0))
    {
        d  = $id(ID); 
        if(d==null)
        {
		    d= document.createElement("div");
		    d.id = ID;
		    _currentBusyDiv = ID;
		    newDiv=true;
        }
    }
    else
    {
        d = $id("divbusywithtop"); 
        if(d==null)
        {
	        d= document.createElement("div");
	        d.id = "divbusywithtop";
	        newDiv=true;
        }
   }
   	
	d.style.backgroundColor = "black";
	d.style.filter = "Alpha(opacity=50)";
	d.style.top = "1px";
	d.style.left="1px";
	d.style.position="absolute";
	d.style.width = document.forms[0].offsetWidth;// screen.availWidth;//document.documentElement.clientWidth;//document.body.offsetWidth + "px";//document.documentElement.offsetWidth + "px";
//	d.style.height = screen.availHeight;//document.documentElement.clientHeight;
	if (document.forms[0].offsetHeight >100)
	{
	    d.style.height = document.forms[0].offsetHeight;//screen.availHeight;//document.documentElement.clientHeight;//document.body.offsetHeight + "px"; //document.documentElement.offsetHeight + "px";
	}
	else
	{
	    d.style.height = document.documentElement.clientHeight;
	}
	if ((zIndex != null) && (zIndex.toString().length > 0 )) //jvz : zIndex.length
	{
	    d.style.zIndex = zIndex;
	}
	else
	{
	    d.style.zIndex="9010";
	}
	d.className="alphadiv";
	d.style.display="inline";
	
	//Jvz het dit ingesit. Dis vir die FREAKING SUBFORMS!!!!!!!!!
	if(typeof(_SubFormChildID) != "undefined")
	{
	    hideSelectsButKeepID(_SubFormChildID);
	}
	else
	{
	    hideSelects();
	}
	if(newDiv==true)
	{
	    try
	    {
	        try
	        {
	            var form = $id(Con);
	            if (form != null)
	            {
		            $id(Con).appendChild(d);
		        }
		        else
		        {
		           $id(_formName).appendChild(d);
		        }
		    }
		    catch(e)
		    {
		        $id(_formName).appendChild(d);
		    } 
		}
		catch(e)
		{
		    document.forms[0].appendChild(d);
		}
	}
}



function busyDivTag(zIndex, ID)
{
    var d;
    var newDiv = false;
   
    if((ID != null) && (ID.length > 0))
    {
        d  = $id(ID); 
        if(d==null)
        {
		    d= document.createElement("div");
		    d.id = ID;
		    _currentBusyDiv = ID;
		    newDiv=true;
        }
    }
    else
    {
        d = $id("divbusywithtop"); 
        if(d==null)
        {
	        d= document.createElement("div");
	        d.id = "divbusywithtop";
	        newDiv=true;
        }
   }
   	
	d.style.backgroundColor = "black";
	d.style.filter = "Alpha(opacity=50)";
	d.style.top = "1px";
	d.style.left="1px";
	d.style.position="absolute";
	d.style.width = document.forms[0].offsetWidth;// screen.availWidth;//document.documentElement.clientWidth;//document.body.offsetWidth + "px";//document.documentElement.offsetWidth + "px";
//	d.style.height = screen.availHeight;//document.documentElement.clientHeight;
//	if (document.forms[0].offsetHeight >100)
//	{
//	    d.style.height = document.forms[0].offsetHeight;//screen.availHeight;//document.documentElement.clientHeight;//document.body.offsetHeight + "px"; //document.documentElement.offsetHeight + "px";
//	}
//	else
//	{
//	    d.style.height = document.documentElement.clientHeight;
//	}
    d.style.height  = document.documentElement.offsetHeight; //changed for Q4_2007 (bug 10309)
	if ((zIndex != null) && (zIndex.toString().length > 0 )) //jvz : zIndex.length
	{
	    d.style.zIndex = zIndex;
	}
	else
	{
	    d.style.zIndex="9010";
	}
	d.className="alphadiv";
	d.style.display="inline";
	
	//Jvz het dit ingesit. Dis vir die FREAKING SUBFORMS!!!!!!!!!
	if(typeof(_SubFormChildID) != "undefined")
	{
	    hideSelectsButKeepID(_SubFormChildID);
	}
	else
	{
	    hideSelects();
	}
	if(newDiv==true)
	{
	    try
	    {
	        try
	        {
	            var form = $id("form1");
	            if (form != null)
	            {
		            $id("form1").appendChild(d);
		        }
		        else
		        {
		           $id(_formName).appendChild(d);
		        }
		    }
		    catch(e)
		    {
		        $id(_formName).appendChild(d);
		    } 
		}
		catch(e)
		{
		    document.forms[0].appendChild(d);
		}
	}
}

function hideSelects()
{
    //mozilla
    //var selCount = document.all.tags("select");
   var selCount = document.getElementsByTagName("select"); 
    for (var i=0; i<selCount.length; i++)
    {
            selCount[i].style.visibility = "hidden";
    }
}
 
function showSelects()
{
    //mozilla
    //var selCount = document.all.tags("select");
    var selCount = document.getElementsByTagName("select");
    for (var i=0; i<selCount.length; i++)
    {
       selCount[i].style.visibility = "visible";
    }
   if ($id("textStyle"))
   {
        //mozilla
        //selCount = ($id("textStyle")).all.tags("select");
        selCount = ($id("textStyle")).getElementsByTagName("select");
        for (var i=0; i<selCount.length; i++)
        {
                selCount[i].style.visibility = "hidden";
        }   
   } 
}

function showSelectSpecific(controlID)
{
     if ($id(controlID))
   {
        //This is a fix for Mozilla browser.
        //selCount = ($id(controlID)).all.tags("select");
        selCount = ($id(controlID)).getElementsByTagName("select");
        for (var i=0; i<selCount.length; i++)
        {
                selCount[i].style.visibility = "visible";
        }   
   } 
} 

function hideSelectSpecific(controlID)
{
     if ($id(controlID))
   {
         //This is a fix for Mozilla browser.
        //selCount = ($id(controlID)).all.tags("select");
        selCount = ($id(controlID)).getElementsByTagName("select");
        for (var i=0; i<selCount.length; i++)
        {
                selCount[i].style.visibility = "hidden";
        }   
   } 
} 
 
function HideDisableDiv(ID)
{
    var df;// = $id("divbusywithtop");
    if ((ID != null) && (ID.length > 0))
    {
        df = $id(ID);
    }
    else
    {
        df = $id("divbusywithtop");
    }
    
    if(df!=null)
    {
        df.style.display="none";
        _currentBusyDiv = "";
	}
	showSelects();
}

function hideSelectsButKeepID(idtoKeep)
{
    //var selCount = _portaldocument.all.tags("select");
   var selCount = document.getElementsByTagName("select"); 
    
    for (var i=0; i<selCount.length; i++)
    {
      	var id = selCount[i].getAttribute("id");
		
		if(idtoKeep !=null)
       {
			if(getParamsNumber(id) != getParamsNumber(idtoKeep))
			{
				selCount[i].style.visibility = "hidden";
			}
			else
			{
				selCount[i].style.visibility = "visible";
			}
		}
		else
		{
			selCount[i].style.visibility = "hidden";
		}
		
      	//selCount[i].style.visibility = "hidden";
    }
}

//Shows certain selects but keep the id passed visible;
function showSelectsButKeepID(idtoKeep)
{
    var selCount = document.getElementsByTagName("select");
    for (var i=0; i<selCount.length; i++)
    {
       var id = selCount[i].getAttribute("id");
            
       if(idtoKeep !=null)
       {
			if(getParamsNumber(id) != getParamsNumber(idtoKeep))
			{
				selCount[i].style.visibility = "hidden";
			}
			else
			{
				selCount[i].style.visibility = "visible";
			}
		}
		else
		{
			selCount[i].style.visibility = "visible";
		}
    }
    
    //selCount = ($id("textStyle")).all.tags("select");
   selCount = ($id("textStyle")).getElementsByTagName("select"); 
    
    for (var i=0; i<selCount.length; i++)
    {
            selCount[i].style.visibility = "hidden";
    }   
    
}

function checkoutsideBottomPopUp(inputObj,height, scrollDivID)
{
	//var i = clienty + parseInt(divImageUpload.style.height);
	if (!scrollDivID)
	{
	    scrollDivID = "PageContentScrollDiv";
	}
	var i = getTopPos(inputObj) + parseInt(height.replace("px",""));
	if ($id(scrollDivID))
	{
	    var scrolltop = ($id(scrollDivID)).scrollTop;
	    var contentheight = ($id(scrollDivID)).offsetHeight;
	    if ((i   - scrolltop) > (contentheight))
	    {
	       i = getTopPos(inputObj) ;
	    }
	}
	else
	{
	    if( i > document.body.offsetHeight-10)
	    {
		    i = getTopPos(inputObj); //works
	    }
	    var scrolltop = 0;
	}
	
	return i = i - scrolltop - parseInt(height.replace("px",""));
}



function checkoutsideWidth(inputObj,width, scrollDivID)
{
	if (!scrollDivID)
	{
	    scrollDivID = "PageContentScrollDiv";
	}
	
	var i = getleftPos(inputObj)+ parseInt(width.replace("px",""));
	
	var scrolleft = 0;
	if ($id(scrollDivID))
	{
	    scrolleft = ($id(scrollDivID)).scrollLeft;
	}
	
	i = i - scrolleft;
	if( i > document.body.offsetWidth-10)
	{
		i =getleftPos(inputObj) - scrolleft - parseInt(width.replace("px","")); //out
	}
	else
	{
		i = i - scrolleft - parseInt(width.replace("px",""));
	}
	
	return i;
}

//jvz: function for Multivalue popup control;
function $ShowSelectPopUpControl(headingText, contentText, controlID, closeBtn, closeBtnFn)
{
    if(($id(controlID)).style.width == "" || ($id(controlID)).style.width.indexOf("%") >0)
    {
        ($id(controlID)).style.width = "100px";
    }
      
    $showPopup(headingText, contentText, controlID, "", "110", "", closeBtnFn, "", "", "", "", "", "", "", "", "");
//    var containerControl = "containerControlPOP";
//    var tblContainerControl = "tblContainerControlPOP";
//    var obj= event.srcElement;
//    var HeadingText = headingText;
//    var ContentText = contentText;
//    var newDiv = false;
//    var headerClose = "";
//    var headerMain = "";
//    var controlText = $id(controlID);
//    var controlValue = "";//controlText.innerHTML;
//    //Added 20px 24/nov due to bug in des?
//    if(controlText.style.height == "" || controlText.style.height == "20px")
//    {
//        controlText.style.height = "100px";
//    }
//    
//    if(controlText.style.width == "" || controlText.style.width.indexOf("%") >0)
//    {
//       
//        controlText.style.width = "100px";
//    }
//    
//    var iPos = checkoutsideWidth(obj,controlText.style.width);
//    var xPosRec = checkoutsideBottomPopUp(obj,controlText.style.height);

//    if (_SubFormChildID == null)
//    {
//        controlText.style.left = iPos;
//        controlText.style.top = xPosRec;
//    } 
//    
//    var controlHeight = controlText.style.height.replace("px","");
//    var controlWidth =  controlText.style.width.replace("px","");
//    var screenL = window.screen.availWidth - controlWidth ;
//    var screenH = window.screen.availHeight - controlHeight;
//    var top =controlText.style.top.replace("px","");  //(screenH/3) +  parseInt(returnScrollDimensions(0)); 
//    var left = controlText.style.left.replace("px",""); //(screenL/2) + parseInt(returnScrollDimensions(1));
//        
//    var divAlert = ""; 
//    var divExists = $id(containerControl); 
//    var ViewID = "part_" + getParamsNumber(controlID);
//    var View = $id(ViewID);
//	var vtop = View.offsetTop;
//    var vleft = View.offsetLeft;
//    
//    var dtop = (parseInt(top) + parseInt(vtop)) - parseInt(10);
//    var dleft =  (parseInt(left) + parseInt(vleft)) + parseInt(20);
//    var st = "top:" + dtop  + "px;" + "z-index:9025;position:absolute;left:" + dleft + "px;width:"+(parseInt(controlWidth)+10) + "px;height:"+(parseInt(controlHeight)+ 5)+"px";
//   
//    if(divExists == null)
//    {
//		divAlert = document.createElement("DIV");
//		divAlert.id = containerControl; //"containerControl";
//		newDiv= true;
//    }
//    else
//    {
//		//return;
//    }
//	 
//     var stTop = (parseInt(top) + parseInt(vtop)) - parseInt(35);
//     var stLeft = parseInt(left)+ parseInt(vleft) - parseInt(7);
//     
//     var stDiv = "top:" + stTop + "px;" + "z-index:9021;position:absolute;left:" +  stLeft  + "px;width:"+(parseInt(controlWidth)+30) + "px;height:"+(parseInt(controlHeight)+10)+"px";
//    
//    if (closeBtn != false)
//    {
//        headerClose = '<img id="btnClose" src="../images/clear.GIF" style="width:15px;height:15px;cursor:hand;">'
//    }
//    
//    headerMain = '<table cellspacing="0" cellpadding="0" style="table-layout:fixed;width:100%;"><tr><td  style="padding-top:10px" align="left" nowrap>' + headingText + '</td><td align="right">' + headerClose + '</td></tr></table>';
//    
//    //divAlert.style.cssText = "position:absolute;display:none;" + stDiv;
//    divAlert.style.cssText = controlText.style.cssText;
//    divAlert.style.top = parseInt(divAlert.style.top) - 36;
//    divAlert.style.left = parseInt(divAlert.style.left) - 10;  
//    divAlert.style.zIndex = "9995";
//    divAlert.innerHTML += '<table id="tblContainerControlPOP" cellpadding="0" cellspacing="0" border="0"><tr><td><div id="poptl" class="poptl"></div></td><td><div id="poptcPop" class="poptc"><div style="font-family:Arial;padding-top:5px;color:White;font-weight:bold;">' + headerMain + '</div></div></td><td><div id="poptrPOP" class="poptr"></div></td></tr><tr><td><div id="popclPOP" class="popcl"></div></td><td align="center"><div id="popccPOP" class="popcc">' + ""+ '</div></td><td><div id="popcrPOP" class="popcr"></div></td></tr><tr><td><div id="popblPOP" class="popbl"></div></td><td><div id="popbcPOP" class="popbc"></div></td><td><div id="popbrPOP" class="popbr"></div></td></tr></table>';
//   
//    if(newDiv == true)
//    { 
//         var cid = getParamsNumber(controlID);
//         $id("part_" + cid).appendChild(divAlert);
//        
//    }
//    
//    //add iframe
//   
//    var table = $id(tblContainerControl);
//    
//    var testi = $id("ifr")
//    var i = null;
//    if(testi == null)
//    {
//		i = document.createElement("Iframe");
//    }
//    else
//    {
//		i = $id("ifr");
//    }
//    i.id = "ifr";
//   
//    if(_SubViewIsOnTop ==true)
//    {
//		i.style.cssText = controlText.style.cssText;
//		i.style.zIndex = "9994"
//		//i.style.top = stTop - 35;
//		//i.style.left = stLeft - 7;  
//		//i.style.cssText = divAlert.style.cssText;
//        
//        if(testi == null)
//        {
//		    $id("part_" + cid).appendChild(i);
//        }
//        else
//        {
//		    i.style.display = "block";
//        }
//    
//    }
//    
//    
//   
//    
//    var thediv =null;
//    thediv = document.getElementById(containerControl);
//    thediv.style.display = "block";
//    
//        
//    document.getElementById("poptcPop").style.width=controlWidth+'px';
//    document.getElementById("popccPOP").style.width=controlWidth+'px';
//    document.getElementById("popbcPOP").style.width=controlWidth+'px';
//    document.getElementById("popclPOP").style.height=controlHeight+'px';
//    document.getElementById("popccPOP").style.height=controlHeight+'px';
//    document.getElementById("popcrPOP").style.height=controlHeight+'px';
//    
//    document.getElementById("btnClose").attachEvent("onclick", closeBtnFn);

    
}
//jvz multivalue popup;
function $doHideControlPopupControl(controlID)
{
    var thediv = window.parent.document.getElementById("containerControlPOP") != null ? window.parent.document.getElementById("containerControlPOP") : document.getElementById("containerControlPOP");
    var theControlID = window.parent.document.getElementById(controlID) != null ? window.parent.document.getElementById(controlID) : document.getElementById(controlID);
    
    if (thediv != null && thediv != "")
    {
        theControlID.style.display = "none";
        ///var ifr = window.parent.document.getElementById("containerframePOP") != null ? window.parent.document.getElementById("containerframe") : document.getElementById("containerframe");
        //ifr.style.display = "none";
         var testi = $id("ifr")
         if(testi !=null)
         {
			testi.style.display = "none";
         }
        thediv.style.display = "none";
        thediv.parentNode.removeChild(thediv);
    }
}

function RemoveAllTableRows(TableName)
{
	var thisTable = $id(TableName);
	
	if (thisTable != null)
	{
		var rows = thisTable.rows.length;
		
		for (var i = 0; i < rows; i++)
		{
			thisTable.deleteRow(0);
		}
	}
}

// Use for IFrame , in green div
// setups everything but doesn't show until being called;
function $doViewPopupIframe(headingText, contentText, src, viewHeight, viewWidth)
{
    showPWait()
    var HeadingText = headingText;
    var ContentText = contentText;
  
    if (viewHeight == null)
    {
        viewHeight = 350; 
    }
    
    if (viewWidth == null)
    {
        viewWidth = 318;
    }
    
    var screenL = (window.screen.availWidth/2) - (viewWidth/2);
    var screenH = window.screen.availHeight - viewHeight;
    
    var top = (screenH/3) +  parseInt(returnScrollDimensions(0)); 
    var left = (screenL) + parseInt(returnScrollDimensions(1));
    
    var newDiv = false;
    var divAlert = "";
    var divExists = $id("containerView");
    
    var IframeEx = false;
    
    if(divExists == null)
    {
		divAlert = document.createElement("DIV");
		divAlert.id = "containerView";
		newDiv= true;
    }
    else
    {
		newDiv = false;
		divAlert = divExists;
    }
    
    var IFrameSrc = $id("IFrameSrc");
    
    if(IFrameSrc == null)
    {
        IFrameSrc = document.createElement("IFRAME");
        IFrameSrc.id = "IFrameSrc";
        IFrameSrc.src = src;
        IframeEx = false;
        IFrameSrc.frameBorder =0;
    }
    else
    {
        IframeEx = true;
    }
    
    var stDiv = "top:" + (parseInt(top) - 100) + "px;z-index:9018;position:absolute;left:" + (parseInt(left) - 25) + "px;width:" + (viewWidth+20) + "px;height:" + (viewHeight+10) + "px";
    var IframeStyle = "top:" + (parseInt(top) - 59) + "px;z-index:9017;position:absolute;left:" + (parseInt(left)-15) + "px;width:" + (viewWidth+10) + "px;height:" + (viewHeight+30) + "px;frameborder=0;";
    
    IFrameSrc.style.cssText = IframeStyle;
    IFrameSrc.style.zIndex = IFrameSrc.style.zIndex +1;
    IFrameSrc.style.height = IFrameSrc.style.height.replace("px","") -40;
    IFrameSrc.style.width = IFrameSrc.style.width.replace("px","") -15;
    IFrameSrc.style.left = parseInt(IFrameSrc.style.left.replace("px","")) + (parseInt(5));
    IFrameSrc.style.display = "none";
    
    divAlert.style.cssText = "position:absolute;display:none;z-index:9019;" + stDiv;
    //divAlert.innerHTML += '<table id="tblContainerView" cellpadding="0" cellspacing="0" border="0"><tr><td><div id="poptl" class="poptl"></div></td><td><div id="poptc" class="poptc"><div style="font-family:Arial;padding-top:15px;color:White;font-weight:bold;">' + HeadingText + '</div></div></td><td><div id="poptr" class="poptr"></div></td></tr><tr><td><div id="popcl" class="popcl"></div></td><td align="center"><div id="popcc" class="popcc">' + ContentText + '</div></td><td><div id="popcr" class="popcr"></div></td></tr><tr><td><div id="popbl" class="popbl"></div></td><td><div id="popbc" class="popbc"></div></td><td><div id="popbr" class="popbr"></div></td></tr></table>';
   divAlert.innerHTML = generatePopupHTML("tblContainerView",headingText, "", ContentText,  "", viewHeight, viewWidth, "","", "","");
   
    if(newDiv == true)
    { 
        document.body.appendChild(divAlert);
    }
    if(IframeEx==false)
    {
        document.body.appendChild(IFrameSrc);
    }
    
   /* var thediv = "";
    thediv = document.getElementById("containerView");
    thediv.style.display = "block";
   */
//    document.getElementById("poptc").style.width=viewWidth+'px';
//    document.getElementById("popcc").style.width=viewWidth+'px';
//    document.getElementById("popbc").style.width=viewWidth+'px';
//    document.getElementById("popcl").style.height=viewHeight+'px';
//    document.getElementById("popcc").style.height=viewHeight+'px';
//    document.getElementById("popcr").style.height=viewHeight+'px';
}

//Helper
//this is to build the packet removing NAME_203 will return NAME
function $getParamWithOutNumber(idcomingin)
{
    var arrS = idcomingin.split('_');
   
    //return arrS[0];
    if (arrS != null)
    {    
        if(arrS.length == 1)
        {
            return arrS[0];
        }
            
        if(arrS.length == 2) //LOAD_155 Course_Code (2)
        {   
            return arrS[0]; 
        }
        else
        {
            //bug met _ op die einde. bv 
            //Send_for_Approval__723
             var Id = arrS[arrS.length-1];
             var sub = idcomingin.substring(0,idcomingin.indexOf('_'+Id));
             return sub; 
        }
    } 
}

function $doViewPopupShowIframe()
{
        var thediv = "";
        
        thediv = window.parent.document.getElementById("containerView")
        thediv.style.display = "block";
        var theIFrame = window.parent.document.getElementById("IFrameSrc")
        theIFrame.style.display = "block";
        window.parent.hidePWait();
        window.parent.busyDivTag();
}

function $doViewPopupCloseIframe()
{
    window.parent.HideDisableDiv();
    var IfClose = window.parent.document.getElementById("tblContainerView");
    if(IfClose !=null)
    {
        IfClose.parentNode.removeChild(IfClose);
    }
    
    var containerView = window.parent.document.getElementById("containerView");
    if(containerView !=null)
    {
        containerView.parentNode.removeChild(containerView);
    }
        
    var Iframe = window.parent.document.getElementById("IFrameSrc");
    if(Iframe !=null)
    {
        Iframe.parentNode.removeChild(Iframe);
    }
}

//Hover Context Menus
function showSelectedContext(obj)
{
    if (obj.className != "divSelected"){
        obj.className = "selectedItemCat";
    }
}

function hideSelectedContext(obj)
{
    obj.className = "divNormal";
}

//Hover Tree Items
function showSelected()
{
    var hoverID = event.srcElement;
    
    hoverID.className = "selectedItem";
    //selectedItem
}

function hideSelected()
{
    var hoverID = event.srcElement;
    
    hoverID.className = "smllabel";
}

//Resize of Window
function ResizeEvent()
{
    var busyD = $id("divbusywithtop"); 
    var CheckGlobalDiv = false;

    if (busyD == null)
    {
        CheckGlobalDiv = true;
    }
    else
    {
        if (busyD.style.display == "none")
        {
            CheckGlobalDiv = true;
        }    
    }
   
    if (CheckGlobalDiv)
    {
        if (_currentBusyDiv != null)
        {   
            if (_currentBusyDiv != "")
            {
                busyD = $id(_currentBusyDiv); 
            }
        }   
    }
  
    if (busyD != null)
    {
        busyD.style.width = document.forms[0].offsetWidth;
	    if (document.forms[0].offsetHeight >100)
	    {
	        busyD.style.height = document.forms[0].offsetHeight;
	    }
	    else
	    {
	        busyD.style.height = document.documentElement.clientHeight;
	    }
    }
    //the line below added by sean to recalculate all expressions on the page
    document.recalc(true);
}

 /* mozlla insertAdjacentHTML replacement function - not quite working */
     function mozilla_insertAdjacentHTML(control, sWhere, sHTML) 
     {
       var df;   // : DocumentFragment
       var r =control.ownerDocument.createRange();
        
       switch (String(sWhere).toLowerCase()) 
       {  // convert to string and unify case
          case "beforebegin":
             r.setStartBefore(control);
             df = r.createContextualFragment(sHTML);
             control.parentNode.insertBefore(df, control);
             break;
             
          case "afterbegin":
             r.selectNodeContents(control);
             r.collapse(control);
             df = r.createContextualFragment(sHTML);
             control.insertBefore(df, control.firstChild);
             break;
             
          case "beforeend":
             r.selectNodeContents(control);
             r.collapse(false);
             df = r.createContextualFragment(sHTML);
             control.appendChild(df);
             break;
             
          case "afterend":
             r.setStartAfter(control);
             df = r.createContextualFragment(sHTML);
             control.parentNode.insertBefore(df, control.nextSibling);
             break;
       }   
    }
   
     var mozilla_emptyTags = {
       "IMG":   true,
       "BR":    true,
       "INPUT": true,
       "META":  true,
       "LINK":  true,
       "PARAM": true,
       "HR":    true
    }

    /*mozilla outerHTML replacement function */
    function mozilla_outerHTML(control)
    {
            var attrs = control.attributes;
            var str = "<" + control.tagName;
            for (var i = 0; i < attrs.length; i++)
            str += " " + attrs[i].name + "=\"" + attrs[i].value + "\"";

            if (mozilla_emptyTags[this.tagName])
                return str + ">";

            return  str + ">" + control.innerHTML + "</" + control.tagName + ">";
    } 
        
//var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

//function encode64(input) 
//{
//    if (input == "") return input;
//    var output = "";
//    var chr1, chr2, chr3;
//    var enc1, enc2, enc3, enc4;
//    var i = 0;

//    do 
//    {
//        chr1 = input.charCodeAt(i++);
//        chr2 = input.charCodeAt(i++);
//        chr3 = input.charCodeAt(i++);

//        enc1 = chr1 >> 2;
//        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
//        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
//        enc4 = chr3 & 63;

//        if (isNaN(chr2)) 
//        {
//            enc3 = enc4 = 64;
//        } 
//        else if (isNaN(chr3)) 
//        {
//            enc4 = 64;
//        }

//        output = output + keyStr.charAt(enc1) + keyStr.charAt(enc2) + keyStr.charAt(enc3) + keyStr.charAt(enc4);
//    } while (i < input.length);
//   
//    return output;
//}

//function decode64(input) 
//{
//    if (input == "") return input;
//    var output = "";
//    var chr1, chr2, chr3;
//    var enc1, enc2, enc3, enc4;
//    var i = 0;

//    // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
//    input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

//    do 
//    {
//        enc1 = keyStr.indexOf(input.charAt(i++));
//        enc2 = keyStr.indexOf(input.charAt(i++));
//        enc3 = keyStr.indexOf(input.charAt(i++));
//        enc4 = keyStr.indexOf(input.charAt(i++));

//        chr1 = (enc1 << 2) | (enc2 >> 4);
//        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
//        chr3 = ((enc3 & 3) << 6) | enc4;

//        output = output + String.fromCharCode(chr1);

//        if (enc3 != 64) 
//        {
//            output = output + String.fromCharCode(chr2);
//        }
//        if (enc4 != 64) 
//        {
//            output = output + String.fromCharCode(chr3);
//        }
//    } while (i < input.length);

//    return output;
//}

function $createCookie(name,value,days) {
	if (days) {
		var date = new Date();
		date.setTime(date.getTime()+(days*24*60*60*1000));
		var expires = "; expires="+date.toGMTString();
	}
	else var expires = "";
	document.cookie = name+"="+value+expires+"; path=/";
}

function $readCookie(name) {
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for(var i=0;i < ca.length;i++) {
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
	}
	return null;
}

function $eraseCookie(name) {
	createCookie(name,"",-1);
}

function $containsSpecialChars(inputString, excludeChars)
{
    var validateResult = -1;
    var specialChars = "&!@#$%^&*()+=-[]\\\';,./{}|\":<>?_~";
    var chinSpecialChars1 = "≈≠＝≤≥≤＜＞≮≯∷±＋－×÷∫∮∝∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√";
    var chinSpecialChars2 = "┼┽┾┿╀╁╂╃┬┭┮┯┰┰┱┱┲┳├┝┞┟┠┡┢┣┍┎┏┐┑┒┓┒─┄";
    var chinSpecialChars3 = "§№☆★○●◎◇◆□℃‰■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀";
    var chinSpecialChars4 = "〖〗【】［］｛｝《》「」『』々‖…—•ˉ〃";
    var chinSpecialChars5 = "ㄅㄉˇˋㄓˊˊ˙˙ㄚㄞㄢㄆㄊㄍㄐㄔㄗㄧㄛㄟㄣㄇㄋㄎㄑㄕㄘㄨㄜㄠㄈㄌㄏㄒㄖㄙㄩㄝㄡㄥ";


    var ExcludeList = excludeChars.split(" ");

    //remove excluded chars form list    
    for (var k = 0; k < ExcludeList.length; k++)
    {
        specialChars = specialChars.replace(ExcludeList[k],"");
        chinSpecialChars1 = chinSpecialChars1.replace(ExcludeList[k],"");
        chinSpecialChars2 = chinSpecialChars2.replace(ExcludeList[k],"");
        chinSpecialChars3 = chinSpecialChars3.replace(ExcludeList[k],"");
        chinSpecialChars4 = chinSpecialChars4.replace(ExcludeList[k],"");
        chinSpecialChars5 = chinSpecialChars5.replace(ExcludeList[k],"");
        
    }
    
    for (var i = 0; i < inputString.length; i++) 
    {
        if (specialChars.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = i;
        }
        if (chinSpecialChars1.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars2.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars3.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars4.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }
        if (chinSpecialChars5.indexOf(inputString.charAt(i)) != -1) 
        {
            validateResult = true;
        }

    }
    return validateResult;
}

function convertToGuidString(normalstring)
{
    if (normalstring == "0")
   {
        normalstring = "00000000-0000-0000-0000-000000000000";
   } 
    var newstring = normalstring;
    var part1 = "";
    var part2 = "";
    var part3 = "";
    var part4 = "";
    var part5 = "";     
    if (normalstring.indexOf('-') == -1)
   {
        normalstring = ReplaceID(normalstring, 'part_', '');
        part1 = normalstring.substring(0, 8);
        part2 = normalstring.substring(8, 12);
        part3 = normalstring.substring(12, 16);
        part4 = normalstring.substring(16, 20);
        part5 = normalstring.substring(20);
        newstring = part1 + "-" + part2 + "-" + part3 + "-" + part4 + "-" + part5;
   } 
   return newstring;
}

function convertFromGuidString(guidstring)
{
    var newstring = ReplaceID(guidstring, "-", "");
    newstring = newstring.toUpperCase();
    return newstring;   
}

// Allows the developer to find HTML elements of a specific classname
function getElementsByClassName(node, classname)
{
    var a = [];
    var re = new RegExp('(^| )'+classname+'( |$)');
    var els = node.getElementsByTagName("*");
    for(var i=0,j=els.length; i<j; i++)
        if(re.test(els[i].className))a.push(els[i]);
    return a;
}

function hasClassName(el, name) {

    // Return true if the given element currently has the given class
    // name. 
    var re = new RegExp('(?:^|\\s)'+name+'(?:\\s|$)');
  
    if (el.className.match(re) != -1 && el.className.match(re) != null)
    {
        return true;
    }
    else
    {
        return false;
    }
  
}

function addClassName(el, name)
{
    if (!hasClassName(el, name)) el.className = (el.className+' '+name);
}

function removeClassName(el, name) {

    // Remove the given class name from the element's className property.
  
    /*var re = new RegExp('(^|\\s)'+name+'(?:\\s|$)');
    el.className = el.className.replace(re, '$1');*/
    
    var curClasses = el.className.split(" ");
    var newClasses = [];
    
    for (var i = 0; i < curClasses.length; i++)
    {
        if (curClasses[i] != "" && curClasses[i] != name) newClasses.push(curClasses[i]);
    }
    
    el.className = newClasses.join(" ");
      
}

function toggleClassName(el, name)
{
    if (hasClassName(el, name))
    {
        removeClassName(el, name);
    }
    else
    {
        addClassName(el, name);
    }
}

function getPageOffsetLeft(el) {

  var x;

  // Return the x coordinate of an element relative to the page.

  x = el.offsetLeft;
  if (el.offsetParent != null)
    x += getPageOffsetLeft(el.offsetParent);

  return x;
}

function getPageOffsetTop(el) {

  var y;

  // Return the x coordinate of an element relative to the page.

  y = el.offsetTop;
  if (el.offsetParent != null)
    y += getPageOffsetTop(el.offsetParent);

  return y;
}

function $showException(objErr,CloseMethod, extraPath)
{
    var finalErr = "";
    var closeMethod = "doHidePopup()";
    
    if ((CloseMethod != null) && (typeof(CloseMethod) != "undefined"))
    {
        closeMethod = CloseMethod;
    }
       
    

    finalErr += "<Table cellspacing=2 cellpadding=0 border=0><tr><td colspan=2 align=left class='smllabelbold'>An unexpected error occurred, please review the following information:</td></tr><tr><td colspan=2>&nbsp;</td></tr>";
    
    if (objErr.Title != null)
    {
        finalErr += "<tr><td valign=top align=left class='smllabelbold'>Title:</td><td align=left class='smllabel'>" + objErr.Title + "</td></tr>";
    }
    
    if (objErr.Type != null)
    {    
        finalErr += "<tr><td valign=top align=left class='smllabelbold'>Type:</td><td align=left class='smllabel'>" +  objErr.Type + "</td></tr>";
    }
    
    if (objErr.Description != null)
    {
        finalErr += "<tr><td valign=top align=left class='smllabelbold'>Description:</td><td align=left class='smllabel'>" + objErr.Description + "</td></tr>"
    }
    
    if (objErr.StackTrace != null)
    {
        finalErr += "<tr><td valign=top align=left class='smllabelbold'>StackTrace:</td><td align=left class='smllabel'>" + objErr.StackTrace + "</td></tr>"
    }    
    
    finalErr += "</table>";

    if (!extraPath)
   {
    extraPath = "";
   } 
    doPopupIframe("Error",finalErr,450,200,"OK",closeMethod,"","","","","20000","",extraPath, "", "error");
}

function $CustomError()
{
    this.Title;
    this.Type;
    this.Description;
    this.StackTrace;
}

function $addEvent(obj, evType, fn)
{
	//if(evType.indexOf("on"))
	//{
	//    evType = ReplaceID(evType,"on","");
	//}
	var evTypeRef = '__' + evType;

	if (obj[evTypeRef])
	{
		if (array_search(fn, obj[evTypeRef]) > -1) return;
	}
	else
	{
		obj[evTypeRef] = [];
		if (obj['on'+evType]) obj[evTypeRef][0] = obj['on'+evType];
		obj['on'+evType] = handleEvent;
	}

	obj[evTypeRef][obj[evTypeRef].length] = fn;
}

/* Adds multiple events to an object that fires the same function */
function $addEvents(obj, evs, fn)
{
    for (var i = evs.length; --i >= 0;)
	{
		$addEvent(el, evs[i], func);
	}
}

function $removeEvent(obj, evType, fn)
{
    var evTypeRef = '__' + evType;
    if (obj != null)
    {
	    if (obj[evTypeRef])
	    {
		    var i = array_search(fn, obj[evTypeRef]);
		    if (i > -1) delete obj[evTypeRef][i];
	    }
	}
}

/* Remove multiple events from an object that fires the same function */
function $removeEvents(obj, evs, fn)
{
    for (var i = evs.length; --i >= 0;)
	{
		$removeEvent(el, evs[i], func);
	}
}

function handleEvent(e)
{
	e = e || window.event;
	var evTypeRef = '__' + e.type;
	var retValue = true;

	for (var i = 0, j = this[evTypeRef].length; i < j; i++)
	{
		if (this[evTypeRef][i])
		{
			this.__fn = this[evTypeRef][i];
			retValue = this.__fn(e) && retValue;
		}
	}

	if (this.__fn) try { delete this.__fn; } catch(e) { this.__fn = null; }

	return retValue;
}

function array_search(val, arr)
{
	var i = arr.length;

	while (i--)
		if (arr[i] && arr[i] === val) break;

	return i;
}

/* SVE: 15/02/2007: shows a mini version of the busydivtag, when only a certain div should be disabled */
function miniBusyDiv(divTagID)
{
//    if (_MustShowPop)
//   {
//        return;
//   } 
   if ($id('miniBusy_' + divTagID)) 
   {
        var controlMiniState = ($id('miniBusy_' + divTagID));
        controlMiniState.parentNode.removeChild(controlMiniState); 
   }  
  
    var controlwidth = ($id(divTagID)).offsetWidth;
    var controlheight = ($id(divTagID)).offsetHeight;
    var controlzindex =   10000;
    var controltop  = getElementTrueTop($id(divTagID));
    var controlleft  = getElementTrueLeft($id(divTagID));
       
    var controlBusyDiv = document.createElement("div");
    controlBusyDiv.id = 'miniBusy_' + divTagID; 
    controlBusyDiv.style.width = controlwidth;
    controlBusyDiv.style.height = controlheight;
    controlBusyDiv.style.zIndex =  controlzindex;
    controlBusyDiv.style.left =  controlleft;
    controlBusyDiv.style.top =  controltop;  
    controlBusyDiv.style.position = "absolute";
    controlBusyDiv.style.background = "white";
    controlBusyDiv.style.border = "2px solid whitesmoke"; 
    controlBusyDiv.style.filter = "Alpha(opacity=85)"; 
    controlBusyDiv.innerHTML = "<table height='100%' width='100%'><tr valign='middle'><td align='center'><img src='../images/Blue_Big_Rotate.gif' /></td></tr></table>"; 
    ($id(divTagID)).disabled = "disabled"; 
    document.body.appendChild(controlBusyDiv);  
}

function miniBusyDone(divTagID)
{
    if ($id(divTagID))
   { 
        if ($id('miniBusy_' + divTagID)) 
       {
            var controlBusyDiv = ($id('miniBusy_' + divTagID));
            controlBusyDiv.parentNode.removeChild(controlBusyDiv); 
       } 
       ($id(divTagID)).disabled = "";
   }
}
/* end mini busy div */

function generatePopupHTML(id, heading, info, content, buttons, height, width, closeButtonName, closeFunction, infoHeader, infoContent, pathExtra,  popupImage)
{
    if (!pathExtra)
   {
        pathExtra = "";
   } 
   
   if (!popupImage)
   {
        popupImage = "";
   }
    var htmlValue = "";
   
    htmlValue += '<table width="100%" height="100%" cellpadding="0" border="0" cellspacing="0" id="' + id + '">'; 
    htmlValue += '			<tr>';
	htmlValue += '			<td valign=top align=left >';
    htmlValue += '		           <table id="body" width="100%" height="100%" border="0" cellpadding="0" cellspacing="0" >'; 
    htmlValue += '		            	<tr >';
    htmlValue += '							<td valign=top><IMG SRC="' + pathExtra +  '../images/popup_top_left.gif"></td>';
    htmlValue += '	    				    <td class="toptab_repeat  " width="100%" >';
    htmlValue += '								<table cellpadding="0" cellspacing="0" width="100%" >';
    htmlValue += '									<tr>';
    htmlValue += '										<td colspan="2"><IMG height="3px" SRC="' + pathExtra +  '../images/spacer.gif"></td>';
    htmlValue += '									</tr>';
    htmlValue += '									<tr>';
    htmlValue += '										<td >';
    htmlValue += '										    <table width=100%>';
    htmlValue += '										        <tr>';
    htmlValue += '										            <td class="white_heading" >&nbsp;' +  heading + '</td>';
    if  ((closeFunction != null) &&  (closeFunction.length > 0))
    { 
        htmlValue += '										             <td align=right>';
        htmlValue += '                                                         <img src="' + pathExtra +  '../images/close.gif" onclick="' + closeFunction + '" style="cursor:hand;" /></td>';

   } 
    htmlValue += '										        </tr>';   
    htmlValue += '										    </table>';
    htmlValue += '                                       </td>';
    htmlValue += '			                        </tr>';
    htmlValue += '								</table>';
    htmlValue += '							</td>';
    htmlValue += '							<td align="right" valign=top><IMG SRC="' + pathExtra +  '../images/popup_top_right.gif"></td>';
    htmlValue += '						   </tr>';
    htmlValue += '						<tr>';
    htmlValue += '							<td colspan="3" height="100%" bgcolor="white">';
    htmlValue += '								<table width="100%" height="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="white">';
    htmlValue += '									<tr>';
    htmlValue += '										<td class="tabs_left_plain_repeat" width="3px"><img width="3px" src="' + pathExtra +  '../images/spacer.gif"></td>';
    htmlValue += '										<td width="100%" height="100%">';
    htmlValue += '											<table width="100%" height="100%" cellpadding="1" cellspacing="1"  border="0">';
    htmlValue += '												<tr>';
    htmlValue += '										            <td style="width: 1px;"></td>';
    htmlValue += '										        </tr>';
    if (info.length > 0)
   { 
        htmlValue += '												<tr>';
        if (popupImage.length > 0)
        {
            htmlValue += '<td  colspan="2" valign="top" width="100%"><div id="info" style="height:30px; overflow:auto;width:100%;" class="info"> ' + info + '</div>';
        }
        else
        {
            htmlValue += '<td  valign="top" width="100%"><div id="info" style="height:30px; overflow:auto;width:100%;" class="info"> ' + info + '</div>';
        }
        htmlValue += '                                                   </td>';
        htmlValue += '												</tr>';
   } 
    htmlValue += '												<tr valign=top>';
  if (popupImage.length > 0)
   {
         htmlValue += '<td style=\"width:48px;\"><img src="' + pathExtra + '../images/' + popupImage + '48x48.gif" /></td>'; 
         htmlValue += '													<td   height=' +  (parseInt(height) + 10) ;
   }
   else
   {
        htmlValue += '													<td colspan="2" style="padding-left:10px;" height="' +  (parseInt(height) + 10) + 'px"';
   }
    if (content.length > 0)
    { 
        htmlValue +=                                                           ' class=black_bdtxt >' + content;
    } 
   else
   {
        htmlValue +=                                                         '>';
   } 
    htmlValue += '													</td>';
    htmlValue += '                                               </tr>';
    htmlValue += '											</table>';
    htmlValue += '										</td>';
    htmlValue += '										<td class="tabs_right_plain_repeat" style="border-right: solid 1px #E0E0E0" width=4px><img width=4px src="' + pathExtra +  '../images/spacer.gif"></td>';
    htmlValue += '									</tr>';
    htmlValue += '								</table>';
    htmlValue += '							</td>';
    htmlValue += '						</tr>';
   //if buttons should be added into the popup 
    if (buttons.length > 0)
   {  
   
        //left edge of buttons
        htmlValue += '					<tr>';
        htmlValue += '						    <td colspan=3>';
        htmlValue += '						        <table cellpadding=0 cellspacing=0 border=0 width="100%">';
        htmlValue += '						            <tr>';
        
        if (infoHeader.length > 0)
        {
            htmlValue += '						                <td rowspan=2><img src="' + pathExtra +  '../images/help_button.gif" onclick="ShowK2Help(' + infoHeader + ')" /></td>';
            //onclick=showHelpPopup("help' + id + '")
        }
        else
        {
           htmlValue += '						                <td rowspan=2><img src="' + pathExtra +  '../images/button_left_two_edges.gif" /></td>';
        }
        htmlValue += '						                <td class="button_bar_repeat" width=100%></td>';
        htmlValue += '							            <td class="button_bar_repeat" valign=top width=4px><img src="' + pathExtra +  '../images/button_left.gif" /></td>';
        
        var btnValues = buttons.split("☺");
        var btnValue = '';
        var fn = '';
        var looper = 0;
        for (var i=0; i<btnValues.length; i++)
        {
            if (i % 2 == 0)
            {
                //new button & values
                btnValue = btnValues[i];

                if (btnValue.length > 0)
               { 
                    fn = btnValues[i + 1];
                    if (fn.split("|")[1] != null)
                    {
                        _popEvents[fn.split("|")[1]] = fn.split("|")[0];
                        fn = fn.split("|")[0];            
                    }     
                
                    htmlValue += '						            <td style="cursor:hand;" class="button_repeat"  id="' + btnValue +'" onclick="' + fn +'" onmouseover=\'popupButtonMouseOver(event, "' +  pathExtra +'")\' onmouseout=\'popupButtonMouseOut(event, "' +  pathExtra +'")\' nowrap>' +  btnValue+ '</td>';
                    looper += 1; 
               }
               
                if ((i + 2) < btnValues.length)
                {
                    htmlValue += '							            <td ><img src="' + pathExtra +  '../images/toolbar_repeat-.gif" /></td>';
                    looper +=1;
                }
                else
                {
                    htmlValue += '							            <td rowspan=2 ><img src="' + pathExtra +  '../images/buttonbar_right2.gif" /></td>';
                    looper +=2;
                }
             }
        }
    
        //repeater for looper 
        htmlValue += '						            </tr>';
        htmlValue += '						            <tr>';
        htmlValue += '						                <td class=button_bar_bottom_repeat colspan=' + looper + ' width=100% height=4px ><img src="' + pathExtra +  '../images/spacer.gif" /></td>';
        htmlValue += '						            </tr>';
        htmlValue += '						        </table>';
        htmlValue += '						    </td>';
        htmlValue += '						</tr>';
        htmlValue += '					</table>';
        htmlValue += '				</td>';
        htmlValue += '			</tr>';
    }
   else // just the "normal" closing tags - without place for buttons
   {
       htmlValue += '					<tr>';
        htmlValue += '						    <td colspan=3>';
        htmlValue += '						        <table cellpadding=0 cellspacing=0 border=0 width="100%">';
        htmlValue += '						            <tr>';
        htmlValue += '						                <td rowspan=2><img src="' + pathExtra +  '../images/bottom_left_clear.gif" /></td>';
        htmlValue += '						                <td width=100% style="background:white;"></td>';
        htmlValue += '							            <td rowspan=2 ><img src="' + pathExtra +  '../images/buttonbar_right_clear.gif" /></td>';
        htmlValue += '						            </tr>';
        htmlValue += '						            <tr>';
        htmlValue += '						                <td class=button_bar_bottom_repeat width=100%  ><img src="' + pathExtra +  '../images/spacer.gif" /></td>';
        htmlValue += '						            </tr>';
        htmlValue += '						        </table>';
        htmlValue += '						    </td>';
        htmlValue += '						</tr>';
        htmlValue += '					</table>';
        htmlValue += '				</td>';
        htmlValue += '			</tr>';
   }  
    htmlValue += '		</table>';
   
//   htmlValue += '<tr><td>';
//   htmlValue += generateInfoPopupHTML('blablabla','morebla die bla <br>again and again', width, 0);
//   htmlValue += '</td></tr>';    
     
     return htmlValue; 
}

function setPopupButtonStatus(id, status)
{
    //id = id of control
   //status = new status (disabled, enabled)
   //todo:  remove the click events from the buttons (or do the check from the functions?)
   ($id(id)).setAttribute("status", status); 
   if (status == "disabled")
   {
        ($id(id)).className = "button_repeat_disabled";
   }
   else
   {
        ($id(id)).className = "button_repeat";
   }
}



function generateInfoPopupHTML(InfoHeader, InfoText, width, height, pathExtra)
{
    var htmlValue = '';
    if ((!width)||(width < 100))
   {
        width = 100;
   }  
    htmlValue += '<table cellpadding=0 cellspacing=0 border=0 width="' + width +'">';
    htmlValue += '   <tr>';
    htmlValue += '       <td width=6px><img src="' + pathExtra +  '../images/help_left_top.gif" /></td>';
    htmlValue += '       <td class=help_top_repeat></td>';
    htmlValue += '       <td ><img src="' + pathExtra +  '../images/help_top_right.gif" /></td>';
    htmlValue += '   </tr>';
    htmlValue += '   <tr>';
    htmlValue += '       <td class=help_left_repeat height=100% width=6px></td>';
    htmlValue += '       <td width=100% class=help_body><b><i>' + InfoHeader + '</i></b><br>';
    htmlValue += InfoText;
    htmlValue += '       </td>';
    htmlValue += '       <td class=help_right_repeat></td>';
    htmlValue += '   </tr>';
    htmlValue += '   <tr>';
    htmlValue += '       <td colspan=3>';
    htmlValue += '           <table cellpadding=0 cellspacing=0>';
    htmlValue += '               <tr>';
    htmlValue += '                   <td colspan=2 align=left width=52px><img src="' + pathExtra +  '../images/help_left_bottom.gif" /></td>';
    htmlValue += '                   <td class=help_bottom_repeat width=100%></td>';
    htmlValue += '                   <td valign=top><img src="' + pathExtra +  '../images/help_right_bottom.gif" /></td>';
    htmlValue += '               </tr>';
    htmlValue += '          </table>';
    htmlValue += '       </td>';
    htmlValue += '     </tr>';
    htmlValue += '</table>';
   
    return htmlValue; 
}

function createHelpPopup(ID, InfoHeader, InfoText, width, height, pathExtra)
{
    var helpDiv;
    if ($id(ID))
   {
        helpDiv = ($id(ID));
   }
   else
   {
        helpDiv = document.createElement("DIV");
        helpDiv.id = ID;
        document.appendChild(helpDiv);
   }      
   helpDiv.innerHTML = generateInfoPopupHTML(InfoHeader, InfoText, width, height, pathExtra);
   helpDiv.style.display = "none";
   helpDiv.style.position = "absolute";
}

function showHelpPopup(ID, top, left)
{
    var helpDiv;
    if ($id(ID))
   {
        helpDiv = ($id(ID));
        helpDiv.style.display = "";
        if (!top)
        {
            top = event.clientY;
        }
        if (!left)
        {
            left = event.clientX;
        }
        helpDiv.style.top = top;
        helpDiv.style.left = left;
   }
}

function hideHelpPopup(ID)
{
     var helpDiv;
    if ($id(ID))
    {
        helpDiv = ($id(ID));
        helpDiv.style.display = "none";
    }
}

function $showPopup(HeadingText, ContentText, ControlID, Height, Width, IframeSrc, CloseFunction, ButtonValues, ZIndex, BusyDivID, ExtraPath, InfoText, HelpHeader, HelpContents, Drag, popupImage)
{
    //Text in the header of the popup
    if (!HeadingText)
   {
        HeadingText = "";
   } 
   //Text to be displayed in the popup, if no control is popped up
   if (!ContentText)
   {
        ContentText = "";
   } 
    //control to display in the popup
   if (!ControlID)
   {
        ControlID = "";
   }
   //height of the popup
   if (!Height)
   {
        Height = "";
   } 
   //width of the popup
   if (!Width)
   {
        Width = "";
   }
   //src of the iframe that will be popped up
   if (!IframeSrc)
   {
        IframeSrc = "";
   }
   //function to execute on the close button
   if (!CloseFunction)
   {
        CloseFunction = "";
   }
   //list of buttons with relevant functions
   if (!ButtonValues)
   {
        ButtonValues = "";
   }
   //zindex of popup
   if (!ZIndex)
   {
        ZIndex = "";
   }
   //name of busy div to be displayed
   if (!BusyDivID)
   {
        BusyDivID = "";
   }
   //extra path which will be appended to src of images of popup
   if (!ExtraPath)
   {
        ExtraPath = "";
   }
   //information text of popup (green section)
   if (!InfoText)
   {
        InfoText = "";
   }
   //Help ID to be passed to the help file
   if (!HelpHeader)
   {
        HelpHeader = "";
   }
   //contents of help (redundant)
   if (!HelpContents)
   {
        HelpContents = "";
   }
   //whether the popup should be draggable - not implemented
   if (!Drag)
   {
        Drag = "";
   }
   //image to be displayed next to the message in the popup
   //options: warning, info, error, help
   if (!popupImage)
   {
        popupImage = "";
   }
   
   //declaration of variables
    var IframeZIndex = 10019;
    var DivZIndex = 10020; 
    var currentIndex = 9020; 
    var divAlert ; 
    var ControlDiv;
    var ifr; 
   
   //calculation of relevant zIndeces
    if (ControlID.length > 0)
   { 
        if (($id(ControlID)).style.zIndex)
        {
            currentIndex = ($id(ControlID)).style.zIndex;    
        }      
        else
        {
            ($id(ControlID)).style.zIndex = currentIndex;
        }
         DivZIndex = parseInt(parseInt(currentIndex) - parseInt(1));
         IframeZIndex = parseInt(parseInt(currentIndex) - parseInt(2));
   } 
    
   //generate/reference foreground iframe  
    if ((ControlID.length == 0) && (IframeSrc.length > 0))
   {
        var IframeControl = ($id("IFrameSrc"));
        if(!(IframeControl))
        {
            IframeControl = document.createElement("IFRAME");
            IframeControl.id = "IFrameSrc";
            IframeControl.frameBorder =0;   
            document.body.appendChild(IframeControl);
        }
        IframeControl.style.width = Width;
        IframeControl.style.height = (parseInt(Height) + 15);
        IframeControl.src = IframeSrc;
        
        IframeControl.style.zIndex = (parseInt(DivZIndex) + parseInt(2));
        ControlID = IframeControl.id;
   }

    //Busy Div tag & zIndex
    if (ZIndex == undefined)
   {
        ZIndex= "";
   } 
    if ((ZIndex=="undefined")||(ZIndex.length == 0))
    {
        
        if (ControlID.length > 0)
        {
            busyDivTag((parseInt(DivZIndex)-parseInt(2)), "busy_" + ControlID);
        } 
        else
        {
            if ((BusyDivID=="undefined")||(BusyDivID.length == 0))
            {
                busyDivTag();
             }
            else
            {
                busyDivTag((parseInt(DivZIndex)-parseInt(2)),  "busy_" + BusyDivID);
            }  
        }
    }
    else
    {
         IframeZIndex = (parseInt(ZIndex) + parseInt(1));
         DivZIndex = (parseInt(ZIndex) + parseInt(2));
         busyDivTag(ZIndex,BusyDivID);
    }
   
   //display of control
   if (!ControlID)
   {
        ControlID = "";
   }
   else  if ($id(ControlID))
   { 
        ControlDiv = $id(ControlID);
        ControlDiv.style.display = "block"; 
        if (!Height)
       {
            Height =(parseInt(ControlDiv.style.height.replace("px","")));
       } 
       
       if (!Width)
       {
            Width =  (parseInt(ControlDiv.style.width.replace("px","")));
       }
      
        if ((Height == "") || isNaN(Height))
	        Height = ControlDiv.offsetHeight;
    		
        if ((Width == "") || isNaN(Width))
	        Width = ControlDiv.offsetWidth;	
   }
   
   if ((CloseFunction.length == 0)||(CloseFunction == "doHidePopup()"))
   {
       //close function of the popup
       CloseFunction = "doHidePopup('" + ControlID + "')";
   }

    var screenL = document.documentElement.offsetWidth - Width ;
    var screenH = document.documentElement.offsetHeight - Height; 
    if (InfoText.length > 0)
    {
        screenH = screenH - 110;
    }
    var xtraPopupHeight=70;
    var top = ((screenH-xtraPopupHeight)/2) +  parseInt(returnScrollDimensions(0));
    if (top<0)
    {
        top=0;
    } 
    var left = (screenL/2) + parseInt(returnScrollDimensions(1));
    //poisitioning of div    
    var stDiv = "top:" + parseInt(top-5) + "px;left:" + parseInt(left-5) + "px;width:" + (parseInt(Width)+10) + "px;height:" + (parseInt(Height)+10) + "px;"
    
   if ($id("container" + ControlID))
   {
        divAlert = ($id("container" + ControlID));
   }
   else
   {
       divAlert = document.createElement("DIV"); 
       divAlert.id = "container" + ControlID;
       divAlert.className = "dragmain";

     
      //add control/parentdiv to array of controls which can be moved
      if ($id(ControlID))
      {
        popupControls.push(($id(ControlID)));
      }
      else
      {
        popupControls.push(divAlert);
      }
      
        if ((ControlID.length == 0)||(IframeSrc.length > 0))
        {
            document.body.appendChild(divAlert);
        }
        else
        {
            ($id(ControlID)).parentNode.appendChild(divAlert);
        }
   } 
    //set the innerhtml of the div
   divAlert.innerHTML = generatePopupHTML('tblContainer' + ControlID, HeadingText, InfoText, ContentText, ButtonValues, Height, Width, "X", CloseFunction, HelpHeader, HelpContents, ExtraPath, popupImage)
   divAlert.style.cssText = "position:absolute;display:none;z-index:"+ DivZIndex +";" + stDiv;
            
    //add background iframe
   if ($id("iframepopups" + ControlID))
   {
     ifr = ($id("iframepopups"  + ControlID));
   } 
   else
   {
     ifr = document.createElement("IFRAME");
     ifr.id = "iframepopups"  + ControlID;
     divAlert.parentElement.appendChild(ifr);
    }
   ifr.className="dragmeIfr"; 
    ifr.frameBorder = "0";
    var table = $id("tblContainer" + ControlID);
    var st = "top:" + parseInt(top) + "px;z-index:" + IframeZIndex+";position:absolute;left:" + parseInt(left) + "px;width:" + (parseInt(Width)-5) + "px;height:" + (parseInt(Height)-5) + "px"; 
    
    //toggle the relevant visibility of the div and iframe  
    $DivSetVisible(true,table,ifr,st);
     
     if (ControlDiv)
     {
        //Control style 
        var stcontrol = "";
        if (InfoText.length > 0)
        {
             stcontrol = ";top:" + (parseInt(top) + parseInt(75)) + "px;z-index:" + parseInt(parseInt(DivZIndex) + parseInt(1)) + ";position:absolute;left:" + (parseInt(left))  + "px;";
        }
        else
        { 
             stcontrol = ";top:" + (parseInt(top) + parseInt(33)) + "px;z-index:" + parseInt(parseInt(DivZIndex) + parseInt(1)) + ";position:absolute;left:" + (parseInt(left))  + "px;";
        } 
        ControlDiv.style.cssText += stcontrol;
   }
    divAlert.style.display = "block";
    divAlert.focus(); 
   
//     if (HelpHeader.length > 0)
//     {
//        createHelpPopup("help" + 'tblContainer' + ControlID, HelpHeader, HelpContents, 0, 200, ExtraPath);
//     } 
}

function getElementTrueLeft(inputObj)
{
    var returnValue = inputObj.offsetLeft;
    
    while((inputObj = inputObj.offsetParent) != null)
    {
        if(inputObj.tagName!='HTML')returnValue += inputObj.offsetLeft;
    }
    
    return returnValue;
}

function getElementTrueTop(inputObj)
{  
    var returnValue = inputObj.offsetTop;
    
    while((inputObj = inputObj.offsetParent) != null)
    {
        if(inputObj.tagName!='HTML')returnValue += inputObj.offsetTop;
    }

    return returnValue;
}

function popupButtonMouseOver(e, pathExtra)
{
    var openevent = (e) ? e : window.event;
    var tg = (e.target) ? e.target : e.srcElement
    if (tg.getAttribute("status") == "disabled")
   {
        return false;
   } 
    tg.className = "green_button_repeat3"; 
    if (tg.previousSibling)
    {
        if (tg.previousSibling.childNodes.length > 0)
       {
            if ((tg.previousSibling.childNodes[0].tagName.toLowerCase()  == "img") && (tg.previousSibling.childNodes[0].src.toLowerCase().indexOf("left") > 0))
            {
                tg.previousSibling.childNodes[0].src =   pathExtra + "../images/green_button_left.gif";
            }
       } 
    }
    if (tg.nextSibling)
    {
        if (tg.nextSibling.childNodes.length > 0)
       {
            if ((tg.nextSibling.childNodes[0].tagName.toLowerCase()  == "img") && (tg.nextSibling.childNodes[0].src.toLowerCase().indexOf("right") > 0))
            {
                tg.nextSibling.childNodes[0].src =   pathExtra + "../images/buttonbar_green_right.gif";
            }
       } 
    } 

    
}

function popupButtonMouseOut(e, pathExtra)
{
    var openevent = (e) ? e : window.event;
    var tg = (e.target) ? e.target : e.srcElement
    if (tg.getAttribute("status") == "disabled")
   {
        return false;
   }  
    tg.className = "button_repeat";
     if (tg.previousSibling)
    {
        if (tg.previousSibling.childNodes.length > 0)
       {
            if ((tg.previousSibling.childNodes[0].tagName.toLowerCase()  == "img") && (tg.previousSibling.childNodes[0].src.toLowerCase().indexOf("left") > 0))
            {
                tg.previousSibling.childNodes[0].src =   pathExtra + "../images/button_left.gif";
            }
       } 
    }
    if (tg.nextSibling)
    {
        if (tg.nextSibling.childNodes.length > 0)
       {
            if ((tg.nextSibling.childNodes[0].tagName.toLowerCase()  == "img") && (tg.nextSibling.childNodes[0].src.toLowerCase().indexOf("right") > 0))
            {
                tg.nextSibling.childNodes[0].src =   pathExtra + "../images/buttonbar_right2.gif";
            }
       } 
    }
}

//Pass the URL , it will return the filename
function $getFileName(URL)
{
    var tr = URL;
    var strBack = "";
    len = tr.length 
    rs = 0 
    for (i = len; i > 0; i--) 
    { 
            vb = tr.substring(i,i+1) 
            if (vb == "/" && rs == 0) 
             { 
                strBack =  tr.substring(i+1,len);
                rs = 1 
             } 
    }
    
    return strBack;
}

function $swopImage(URL,to,from)
{
    //http://bp1/images/area1.gif
    //area1.gif
    var filename = $getFileName(URL);
    var fileTemp = filename;
    if(filename == "")
    {
        alert('Error in swop Image');
    } 
    else
    {
        fileTemp = $rep(fileTemp,to,from);
        fileTemp = $rep(URL,filename,fileTemp);
    }
    
    return fileTemp;
}
//////////

function showSimplePopupDiv(popuptablename, nohide, top, left)
{
    //shows the context menu type, showing the popuptable name's innerHTML
   // need to add your own getTopPos & getLeftPos function in your own JS for  checkoutsideWidth & checkoutsideBottomPopUp to work - specifically done like this to reuse code, but work with calendar
    var htmlValue = '';
    htmlValue +=' <table class="context_menu_main"> ';
    htmlValue +='     <tr> ';
    htmlValue +='            <td class="context_menu_second"> ';
    if (!nohide)
    {
        htmlValue +='                <table cellpadding="1" cellspacing="0" onmouseleave="closeSimplePopupDiv()">';
    }
    else
    {
        htmlValue +='                <table cellpadding="1" cellspacing="0">';
    }
    htmlValue +=  ($id(popuptablename)).innerHTML;
    htmlValue +='                </table>' 
    htmlValue +='            </td> ';
    htmlValue +='       </tr> ';
    htmlValue +='    </table> ';
    
    var popupDiv =($id("simplePopupDiv"));
    if (popupDiv) 
    {
        popupDiv.innerHTML = htmlValue;
    }
    else
    {
        popupDiv = document.createElement("div");
        popupDiv.style.position = "absolute";
        popupDiv.style.display = "none";
        popupDiv.style.backgroundColor = "white";
        popupDiv.id= "simplePopupDiv";
        popupDiv.innerHTML = htmlValue;
        document.body.appendChild(popupDiv);
    }
    
    //do the physical popup section
    var IfrRef = $id("backgroundframe");
    if (!IfrRef)
    {
        IfrRef = document.createElement("iframe");
        IfrRef.id = "backgroundframe";
        document.body.appendChild(IfrRef);
    }
    
    popupDiv.style.zIndex=9031;
    popupDiv.style.display = "block";
    IfrRef.style.position = 'absolute';
    IfrRef.style.width = popupDiv.offsetWidth ;
    IfrRef.style.height = popupDiv.offsetHeight ;
    IfrRef.style.zIndex = popupDiv.style.zIndex - 1;
    IfrRef.style.display = "block";
    IfrRef.style.backgroundColor = "black";
    
     var iPos = 0;
     var xPosRec = 0;
     
    if (event)
    {
        var obj= event.srcElement;
    
        iPos = checkoutsideWidth(obj,IfrRef.style.width, "MenuStructureScrollDiv");
        xPosRec = checkoutsideBottomPopUp(obj,IfrRef.style.height, "MenuStructureScrollDiv");
    }
    else
    {
        iPos = left;
        xPosRec = top;
    }
    
   IfrRef.style.left = parseInt(iPos);
   IfrRef.style.top = parseInt(xPosRec);
   
   popupDiv.style.left = parseInt(iPos);
   popupDiv.style.top = parseInt(xPosRec);
}

function closeSimplePopupDiv()
{
    var IfrRef = $id("backgroundframe");
    var div = ($id("simplePopupDiv"));
    
    if (div)
    {
        div.style.display = "none";
    }
    if (IfrRef)
    {
        IfrRef.style.display = "none";
    }
    
     var tg = ($id("imageActionSelected"));
     if (tg)
     {
        tg.src = "../images/black_context_arrow.gif";
        tg.id = "";
        tg.className = "normalItem";
    }
}

function ShowK2Help(helpID)
{
    if (helpID == 0)
        helpID = "";
        
    if (helpID != "")
        window.open(buildURL("WorkspaceHelp/WorkspaceHelp.aspx?HelpID=" + helpID, null,"directories=0, location=0, menubar=0, titlebar=0, toolbar=0"));
    else
        window.open(buildURL("WorkspaceHelp/WorkspaceHelp.aspx", null,"directories=0, location=0, menubar=0, titlebar=0, toolbar=0"));
}

function ValidateDate(FromDate, ToDate, pagePath)
{
    if (FromDate > ToDate)
    {
        doPopup(Generic_js_DateValidationHeading, Generic_js_StartDateEndDate, 400 , 75, Generic_js_ButtonOk, "doHidePopup('dateCompare')|13", "", "", "", "", 11500, "dateCompare", pagePath,"","error");
        return true;
    }
    else
    {
        return false;
    }
}

function generateGuid()
{
    var result, i, j;
    result = '';
    for(j = 0; j < 32; j++)
    {
        if( j == 8 || j == 12|| j == 16|| j == 20)
        {
            result = result + '-';
        }
        i = Math.floor(Math.random()*16).toString(16).toUpperCase();
        result = result + i;
    }
    return result;
}
