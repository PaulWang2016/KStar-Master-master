using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Documents;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Menus;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Resources;
using aZaaS.Framework.Extensions;

namespace aZaaS.KStar.Facades
{
    public class ResourceFacade
    {
        private ResourceManager _resourceManager;

        public ResourceFacade()
        {
            this._resourceManager = new ResourceManager();
        }

        public List<Resource> GetResources()
        {
            return _resourceManager.GetResources();
        }
        public List<Resource> GetTreeResources(Guid? id, string type)
        {
            type.NullOrEmptyThrowArgumentEx("type id Null");

            return _resourceManager.GetTreeResources(id, type);
        }
        public List<Resource> GetMenuTreeResources(Guid? id, string type)
        {
            type.NullOrEmptyThrowArgumentEx("type id Null");

            return _resourceManager.GetMenuTreeResources(id, type);
        }
        public List<Resource> GetDocumentTreeResources(Guid? id, string type)
        {
            type.NullOrEmptyThrowArgumentEx("type id Null");

            return _resourceManager.GetDocumentTreeResources(id, type);
        }
        public string GetResourceType(Guid id)
        {
            id.EmptyThrowArgumentEx("id is Null");

            return _resourceManager.GetResourceType(id);
        }

        public void AddResource(Resource item)
        {
            item.NullThrowArgumentEx("Resource is Null");

            _resourceManager.AddResource(item);
        }
        public void DeleteResource(Guid id)
        {
            id.EmptyThrowArgumentEx("id  is Null");

            _resourceManager.DeleteResource(id);
        }
        public void EditResource(Resource item)
        {
            item.NullThrowArgumentEx("Resource is Null");

            _resourceManager.EditResource(item);
        }
        public List<Resource> FindResources(string input)
        {
            return _resourceManager.FindResources(input);
        }
    }
}
