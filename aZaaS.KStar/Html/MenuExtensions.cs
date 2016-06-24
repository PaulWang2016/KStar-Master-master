using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using aZaaS.KStar.Facades;

namespace aZaaS.KStar.Html
{
    public static class MenuExtensions
    {
        private static MenuFacade _menuFacade = new MenuFacade();
        private static SuperADFacade _superADFacade = new SuperADFacade();
        private static AcsFacade _acsFacade = new AcsFacade();
        private static DocumentFacade _docfacade = new DocumentFacade();


        public static MvcHtmlString TopBar(this HtmlHelper htmlHelper, string userName)
        {
            List<Menu> menu;
            if (_superADFacade.IsSuperAD(userName))
            {
                menu = _menuFacade.GetTop();
            }
            else
            {
                menu = _acsFacade.AuthorityTop(userName);
            }

            return PartialExtensions.Partial(htmlHelper, "_TopBar", menu);
        }

        public static MvcHtmlString IndexPage(this HtmlHelper htmlHelper, string userName)
        {
            List<Menu> menu;
            if (_superADFacade.IsSuperAD(userName))
            {
                menu = _menuFacade.GetTop();
            }
            else
            {
                menu = _acsFacade.AuthorityTop(userName);
            }
            return PartialExtensions.Partial(htmlHelper, "_IndexPage", menu);
        }

        public static MvcHtmlString MenuBar(this HtmlHelper htmlHelper, string menuKey, string userName, bool enableAffix = false)
        {
            return htmlHelper.MenuBar(menuKey, userName, "_MenuBar", false);
        }

        public static MvcHtmlString MenuBar(this HtmlHelper htmlHelper, string menuKey, string userName, string partialViewName, bool enableAffix = false)
        {
            Menu menu;
            if (_superADFacade.IsSuperAD(userName))
            {
                menu = _menuFacade.Get(menuKey);
            }
            else
            {
                menu = _acsFacade.AuthorityMenuBar(menuKey, userName);
            }

            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.Add("EnableAffix", enableAffix);

            return PartialExtensions.Partial(htmlHelper, partialViewName, menu, vdd);
        }

        public static MvcHtmlString DocumentBar(this HtmlHelper htmlHelper, string menuKey, string userName)
        {
            List<string> docLibrarys;
            if (_superADFacade.IsSuperAD(userName))
            {
                docLibrarys = _docfacade.GetDocLibraryKeyByMenuKey(menuKey);
            }
            else
            {
                docLibrarys = _acsFacade.AuthorityDocumentLibrary(userName, menuKey);
            }

            ViewDataDictionary vdd = new ViewDataDictionary();

            return PartialExtensions.Partial(htmlHelper, "_DocumentBar", docLibrarys, vdd);
        }

    }
}
