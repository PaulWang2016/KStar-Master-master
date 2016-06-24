//script to load silverlight usercontrol dynamically
document.write('<div id="silverlightControlHost" style="width:100%;height:100%;">');
document.write('<object data="data:application/x-silverlight," type="application/x-silverlight-2" width="100%" height="100%">');
document.write('<param name="source" value="/ViewFlow/ClientBin/SourceCode.Viewflow.SLViewer.xap?version=');
document.write(document.getElementById("Version").value);
document.write('"/>');
document.write('<param name="onerror" value="onSilverlightError" />');
document.write('<param name="background" value="white" />');
document.write('<a href="http://go.microsoft.com/fwlink/?LinkID=124807" style="text-decoration: none;">');
document.write('<img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style: none"/>');
document.write('</a>');
document.write('</object>');
document.write('</div>');