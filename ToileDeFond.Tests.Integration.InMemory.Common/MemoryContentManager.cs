using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.ContentManagement;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities;

namespace ToileDeFond.Tests.Integration.InMemory.Common
{
    //On donne une plus haute priorité aux tests d'intégration
    [PrioritisedExport(typeof(IContentManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MemoryContentManager : IContentManager
    {
        private readonly List<Module> _modules;
        private readonly List<Content> _contents;

        public MemoryContentManager()
        {
            _modules = new List<Module>();
            _contents = new List<Content>();
        }

        public void Store(Module module)
        {
            var existingModule = _modules.SingleOrDefault(m => m.Name.Equals(module.Name));

            if(existingModule != null)
            {
                _modules.Remove(existingModule);
            }
            
            _modules.Add(module);
        }

        public Module LoadModule(string moduleFullName)
        {
            return _modules.SingleOrDefault(m => m.Name.Equals(moduleFullName));
        }

        public void Store(Content content)
        {
            var existingContent = _contents.SingleOrDefault(m => m.Id.Equals(content.Id));

            if (existingContent != null)
            {
                _contents.Remove(existingContent);
            }

            _contents.Add(content);
        }

        public Content LoadContent(Guid id)
        {
            return _contents.SingleOrDefault(m => m.Id.Equals(id));
        }

        public Module LoadModule(Guid id)
        {
            return _modules.SingleOrDefault(m => m.Id.Equals(id));
        }

        public ReadOnlyCollection<Publication> LoadPublications(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<Module> LoadAllModules()
        {
            return _modules.AsReadOnly();
        }

        //TODO: Changer l'implémentation pour supporter SaveChanges ?
        public void SaveChanges()
        {
            
        }

        public Dictionary<string, Module> LoadModules(List<string> moduleNames)
        {
            return _modules.Where(m => moduleNames.Contains(m.Name)).ToDictionary(m => m.Name, m => m);
        }

        public PagedCollection<Content> LoadContentsByContentType(string contentTypeFullName, int pageSize, int pageIndex = 1)
        {
            throw new NotImplementedException();
        }

        public void DeleteModuleById(Guid id)
        {
            var module = LoadModule(id);

            if (module != null)
                _modules.Remove(module);
        }

        public void DeleteContentById(Guid id)
        {
            var content = LoadContent(id);

            if (content != null)
                _contents.Remove(content);
        }

        public string GetModuleNameFromContentTypeFullName(string contentTypeFullName)
        {
            throw new NotImplementedException();
        }

        public string GetContentTypeNameFromContentTypeFullName(string contentTypeFullName)
        {
            throw new NotImplementedException();
        }

        public ContentType LoadContentType(string contentTypeFullName)
        {
            throw new NotImplementedException();
        }

        public string GetContentTypeFullNameFromPropertyFullName(string propertyFullName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
          
        }
    }
}