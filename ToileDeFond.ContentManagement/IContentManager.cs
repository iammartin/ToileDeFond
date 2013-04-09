using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    //TODO: Retirer le IDisposable et faire le ménage comme a Radio-Canada.. pu de session dans les repostories
    //et faire un session factory qui conserve la session en cours en cache dans http ... ou juste la session factory en singleton dans le contexte http (MEF)
    public interface IContentManager : IDisposable
    {
        void Store(Module module);
        Module LoadModule(string moduleFullName);
        void Store(Content content);
        Content LoadContent(Guid id);
        Module LoadModule(Guid id);
        ReadOnlyCollection<Publication> LoadPublications(IEnumerable<Guid> ids);
        ReadOnlyCollection<Module> LoadAllModules();
        void SaveChanges();
        Dictionary<string, Module> LoadModules(List<string> moduleNames);
        PagedCollection<Content> LoadContentsByContentType(string contentTypeFullName, int pageSize, int pageIndex = 1);
        void DeleteModuleById(Guid id);
        void DeleteContentById(Guid id);
        string GetModuleNameFromContentTypeFullName(string contentTypeFullName);
        string GetContentTypeNameFromContentTypeFullName(string contentTypeFullName);
        ContentType LoadContentType(string contentTypeFullName);
        string GetContentTypeFullNameFromPropertyFullName(string propertyFullName);
    }
}
