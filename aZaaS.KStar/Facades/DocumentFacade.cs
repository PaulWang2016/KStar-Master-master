using aZaaS.KStar.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework.Extensions;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar
{
    public class DocumentFacade
    {
        private DocumentManager _documentManager;
        private MenuManager _menumanager;
        public DocumentFacade()
        {
            _menumanager = new MenuManager();
            _documentManager = new DocumentManager();
        }


        public DocumentLibrary GetLibrary(Guid DocumentLibraryID, bool includeItems)
        {
            DocumentLibraryID.EmptyThrowArgumentEx("DocumentLibraryID");

            return _documentManager.GetDocLibraryById(DocumentLibraryID, includeItems);
            //return _documentManager.GetLibrary(DocumentLibraryID, includeItems);
        }
        public DocumentLibrary GetLibraryByKey(string DocumentLibraryKey, bool includeItems)
        {
            DocumentLibraryKey.NullOrEmptyThrowArgumentEx("DocumentLibraryKey");
            var id = _documentManager.GetDocLibraryIdByKey(DocumentLibraryKey);
            return _documentManager.GetDocLibraryById(id, includeItems);
        }
        public string GetMenuKeyById(Guid id)
        {
            id.EmptyThrowArgumentEx("id");

            return _menumanager.GetMenuKeyById(id);
            //return _documentManager.GetDocMenuKeyById(id);
        }
        public List<DocumentLibrary> GetAllLibrary()
        {
            return _documentManager.GetAllDocLibrary(false);
            //return _documentManager.GetAllLibrary();
        }
        public List<Guid> GetDocLibraryIDByMenuKey(string MenuKey)
        {
            MenuKey.NullOrEmptyThrowArgumentEx("MenuKey");
            var menuId = _menumanager.GetMenuIdByKey(MenuKey);
            return _documentManager.GetDocLibraryByMenuId(menuId).Select(s => s.Id).ToList();
            //return _documentManager.GetDocLibraryIDByMenuKey(MenuKey);
        }
        public List<string> GetDocLibraryKeyByMenuKey(string MenuKey)
        {
            MenuKey.NullOrEmptyThrowArgumentEx("MenuKey");

            var menuId = _menumanager.GetMenuIdByKey(MenuKey);
            return _documentManager.GetDocLibraryByMenuId(menuId).Select(s => s.Key).ToList();
            //return _documentManager.GetDocLibraryKeyByMenuKey(MenuKey);
        }
        public DocumentItem GetDocumentItem(Guid id)
        {
            id.EmptyThrowArgumentEx("DocumentItem id is null");
            return _documentManager.GetDocItemById(id);
            //return _documentManager.GetDocumentItem(id);
        }
        public DocumentLibrary GetDocumentLibrary(Guid id)
        {
            id.EmptyThrowArgumentEx("DocumentLibrary id is null");

            return _documentManager.GetDocLibraryById(id, false);
            //return _documentManager.GetDocumentLibrary(id);
        }

        public void CreateLibrary(DocumentLibrary lib, IEnumerable<DocumentItem> items)
        {
            lib.NullThrowArgumentEx("DocumentLibrary is null");
            if (items != null) lib.Items = items.ToList();
            _documentManager.CreateDocLibrary(lib);
            //_documentManager.CreateLibrary(lib, items);
        }
        public void CreateDocumentItem(DocumentItem dto, Guid DocumentLibraryID)
        {
            dto.NullThrowArgumentEx("DocumentItem is  null");
            DocumentLibraryID.EmptyThrowArgumentEx("DocumentLibraryID is Empty");
            dto.DocumentLibraryID = DocumentLibraryID;
            _documentManager.CreateDocItem(dto);
            //_documentManager.CreateDocumentItem(dto, DocumentLibraryID);
        }


        public void UpdateLibrary(DocumentLibrary lib)
        {
            lib.NullThrowArgumentEx("DocumentLibrary is null");

            _documentManager.UpdateDocLibrary(lib);
            //_documentManager.UpdateLibrary(lib);
        }
        public void UpdateDocumentItem(DocumentItem dto)
        {
            dto.NullThrowArgumentEx("DocumentItem is null");

            _documentManager.UpdateDocItem(dto);
            //_documentManager.UpdateDocumentItem(dto);
        }

        public List<Guid> DelLibrary(List<Guid> idList)
        {
            idList.NullOrEmptyThrowArgumentEx("idList is null");
            return _documentManager.DelDocLibrarysByIds(idList);
            //return _documentManager.DelLibrary(idList);
        }
        public List<Guid> DelDocumentItem(List<Guid> idlist)
        {
            idlist.NullOrEmptyThrowArgumentEx("idlist is null");

            return _documentManager.DelDocItemsByIds(idlist);
            //return _documentManager.DelDocumentItem(idlist);
        }

    }
}
