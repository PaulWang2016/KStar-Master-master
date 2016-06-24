using aZaaS.KStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Facades;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class DocumentController : BaseMvcController
    {
        private static readonly DocumentFacade _docfacade = new DocumentFacade();
        private static SuperADFacade _superADFacade = new SuperADFacade();
        private static readonly AcsFacade _acsFacade = new AcsFacade();

        public ActionResult RenderWidget()
        {
            var DocLibraryKey = this.Request["DocLibraryKey"];
            DocumentLibrary documentLibrary;
            if (_superADFacade.IsSuperAD(this.CurrentUser))
            {
                documentLibrary = _docfacade.GetLibraryByKey(DocLibraryKey, true);
            }
            else
            {
                documentLibrary = _acsFacade.AuthorityDocumentItem(this.CurrentUser, DocLibraryKey);
            }

            return PartialView(documentLibrary);
        }
    }
}
