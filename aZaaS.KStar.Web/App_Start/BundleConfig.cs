using System.Web;
using System.Web.Optimization;

namespace aZaaS.KStar.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/iscroll").Include("~/JS/iscroll.js"));
            bundles.Add(new ScriptBundle("~/bundles/kendoExcelGrid").Include("~/JS/kendoExcelGrid.js"));
            bundles.Add(new ScriptBundle("~/bundles/widget-manager").Include("~/JS/widget-manager.js"));
            bundles.Add(new ScriptBundle("~/bundles/basejs").Include(
                "~/JS/iscroll.js",
                "~/JS/widget-manager.js",
                "~/JS/kendoExcelGrid.js"));
            bundles.Add(new ScriptBundle("~/bundles/tabSlideOut").Include("~/JS/jquery.tabSlideOut.v1.3.js"));
            bundles.Add(new ScriptBundle("~/bundles/base").Include("~/JS/JsMsg.js","~/JS/base.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendobasejs").Include(
                "~/JS/baseInitView.js",
                "~/JS/Filters.js",
                "~/JS/Columns.js",
                "~/JS/models.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/JS/modernizr-*"));
            bundles.Add(new StyleBundle("~/CSS/kendoui/Customkendoui").Include("~/CSS/kendoui/Custom.kendoui.css"));
            bundles.Add(new StyleBundle("~/CSS/selectpersonwindow").Include("~/CSS/selectpersonwindow.css"));


            //kstarform
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout*"));

            bundles.Add(new ScriptBundle("~/bundles/kstarform").Include(
                        "~/Scripts/kstarform*"));

            bundles.Add(new ScriptBundle("~/bundles/dmuploader").Include(
                        "~/Scripts/dmuploader*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryuiwidget").Include(
                        "~/Scripts/jquery.ui.widget.js"));

            bundles.Add(new ScriptBundle("~/bundles/iframe-transport").Include(
                        "~/Scripts/jquery.iframe-transport.js"));

            bundles.Add(new ScriptBundle("~/bundles/fileupload").Include(
                        "~/Scripts/jquery.fileupload.js", "~/Scripts/jquery.fileupload-process.js", "~/Scripts/jquery.fileupload-validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryztree").Include(
                        "~/Scripts/jquery.ztree.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerybootgrid").Include(
                        "~/Scripts/jquery.bootgrid.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/jqueryztree").Include("~/CSS/KstarForm/ztree.css"));
            bundles.Add(new StyleBundle("~/Content/jquerybootgrid").Include("~/CSS/KstarForm/jquery.bootgrid.css"));
            bundles.Add(new StyleBundle("~/Content/uploader").Include("~/Content/uploader.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css").Include("~/CSS/KstarForm/ztree.css").Include("~/CSS/KstarForm/kstarform.css"));
           
            bundles.Add(new StyleBundle("~/bootstrap/css").Include(
                        "~/Content/bootstrap.css",
                //"~/Content/bootstrap-theme.css",
                        "~/Content/bootstrap-datetimepicker.css"));

            /***************  KSTAR Management Page Bootstrap-Style ******************/

            bundles.Add(new StyleBundle("~/kstar-boostrap-base/css").Include(
                        "~/Content/jquery.bootgrid.css",
                        "~/Content/bootstrap-datetimepicker.css"
                        ));

            /***************  KSTAR Management Page JavaScript Base-Libs ******************/

            bundles.Add(new ScriptBundle("~/bundles/kstar-management-base").Include(
                        "~/JS/seajs/sea.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/moment*",
                        "~/Scripts/knockout*",
                        "~/Scripts/kstarform.knockout.js",
                        "~/Scripts/kstarform.validate.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/jquery.bootgrid*"));
        }
    }
}