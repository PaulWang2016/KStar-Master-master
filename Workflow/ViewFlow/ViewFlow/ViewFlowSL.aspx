<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewFlowSL.aspx.cs" Inherits="BPM.WebSite.ViewFlow.ViewFlowSL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>View Flow</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    </style>
    <script type="text/javascript" src="/ViewFlow/plugin/Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
       
    </script>
    
</head>
<body>
    <form id="form1" runat="server" style="height:100%;">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
         <input id="Version" name="Version" style="display:none" runat="server" value="" type="text" />
        <div style="width:100%;height:100%;">
            <script src="/ViewFlow/plugin/addSLControl.js" type="text/javascript"></script>
            <input id="WebSiteUrl" type="hidden" name="WebSiteUrl" runat="server" />
            <input id="WebServiceURL" type="hidden" name="WebSiteUrl" runat="server" />
            <input id="InstanceID" type="hidden" name="InstanceID" runat="server" />
            <input id="K2Server" type="hidden" name="K2Server" runat="server" />
            <input id="HostServer" type="hidden" name="HostServer" runat="server" />
            <input id="HostServerPort" type="hidden" name="HostServerPort" runat="server" />
            <input id="ViewType" type="hidden" name="ViewType" runat="server" />
            <input id="ViewFlowRefreshTime" type="hidden" name="ViewFlowRefreshTime" runat="server" />
        </div>
        
    </form>
</body>
</html>
