using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ToileDeFond.Modularity;
using Raven.Abstractions.Commands;
using Raven.Client;
using Raven.Client.Linq;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.RavenDB
{
    //TODO: Réfléchir comme il le faut aux CreationPolicy de MEF (voir TransientCompositionContainer)
    //même si shared (singleton) c'est pas trop grave si le container est recréé à chaque requête http... ca donne http scope....
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrioritisedExport(typeof(IContentManager), 1)]
    public class ContentManager : IContentManager, IDisposable
    {
        private readonly IDocumentSession _documentSession;

        [ImportingConstructor]
        public ContentManager(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
            //_contentJsonConverter = new ContentJsonConverter(this);
            //_moduleJsonConverter = new ModuleJsonConverter();
            //_publicationJsonConverter = new PublicationJsonConverter();

            //_documentSession.Advanced.DocumentStore.Conventions.CustomizeJsonSerializer = serializer =>
            //        {
            //            serializer.Converters.Add(_contentJsonConverter);
            //            serializer.Converters.Add(_moduleJsonConverter);
            //            serializer.Converters.Add(_publicationJsonConverter);
            //        };
        }

        public IDocumentSession DocumentSession
        {
            get { return _documentSession; }
        }

        public void Store(Module module)
        {
            DocumentSession.Store(module);
        }

        public Module LoadModule(string moduleFullName)
        {
            if (moduleFullName.IsNullOrEmpty())
                return null;

            return DocumentSession.Advanced.LuceneQuery<Module, ModulesByNameIndex>()
                .WhereEquals(m => m.Name, moduleFullName).WaitForNonStaleResults().FirstOrDefault();
        }

        public void Store(Content content)
        {
            DocumentSession.Store(content);
        }

        //TODO: LoadContents... très important avec RavenDB pour pas faire du N+1 et limite de query par session
        public Content LoadContent(Guid id)
        {
            //TODO - Pas certain que ca fonctionne les include en ce moment... à vérifier
            return DocumentSession
                .Include<Content, Module>(x => x.Module.Id)
                .Load(id);
        }

        public Module LoadModule(Guid id)
        {
            return DocumentSession.Load<Module>(id);
        }

        public ReadOnlyCollection<Publication> LoadPublications(IEnumerable<Guid> ids)
        {
            var enumerable = ids as Guid[] ?? ids.ToArray();
            if (!enumerable.Any())
                return new List<Publication>().AsReadOnly();

            if (enumerable.Length == 1)
                return new[] { DocumentSession.Load<Publication>("publications/" + enumerable.First().ToString()) }.ToList().AsReadOnly();

            return DocumentSession.Load<Publication>(enumerable.Select(id => "publications/" + id.ToString())).ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Module> LoadAllModules()
        {
            return DocumentSession.Query<Module>().Customize(q => q.WaitForNonStaleResults()).ToList().AsReadOnly();
        }

        public void SaveChanges()
        {
            DocumentSession.SaveChanges();
        }

        public Dictionary<string, Module> LoadModules(List<string> moduleNames)
        {
            moduleNames = moduleNames.Distinct().ToList();

            var modules = DocumentSession.Query<Module>("ModulesByNameIndex").Where(m => m.Name.In<string>(moduleNames)).ToList();
            // ..Advanced.LuceneQuery<Module, ModulesByNameIndex>().WhereIn<string>(m => m.Name, moduleNames.ToArray()).WaitForNonStaleResults().ToList();

            //TODO: Empecher le fait de devoir faire un distinct!!
            return modules.DistinctBy(m => m.Name).ToDictionary(m => m.Name, m => m);
        }

        //TODO: Utiliser les facettes ???
        public PagedCollection<Content> LoadContentsByContentType(string contentTypeFullName, int pageSize, int pageIndex = 1)
        {
            RavenQueryStatistics stats;

            return new PagedCollection<Content>(DocumentSession.Advanced.LuceneQuery<Content, ContentsByContentTypeNameIndex>()
                .Include(x => x.Module.Id)
                .WhereEquals("ContentType", contentTypeFullName).Statistics(out stats)
                    .Page(pageIndex, pageSize)
                    .ToList(), stats.TotalResults, pageIndex, pageSize);
        }

        public void DeleteModuleById(Guid id)
        {
            _documentSession.Advanced.Defer(new DeleteCommandData { Key = "modules/" + id.ToString() });
        }

        public void DeleteContentById(Guid id)
        {
            _documentSession.Advanced.Defer(new DeleteCommandData { Key = "contents/" + id.ToString() });
        }

        public string GetModuleNameFromContentTypeFullName(string contentTypeFullName)
        {
            return contentTypeFullName.SubstringBeforeLastIndexOf('.');
        }

        public string GetContentTypeNameFromContentTypeFullName(string contentTypeFullName)
        {
            return contentTypeFullName.SubstringAfterLastIndexOf('.');
        }

        public ContentType LoadContentType(string contentTypeFullName)
        {
            if (contentTypeFullName.IsNullOrEmpty())
                return null;

            var moduleName = GetModuleNameFromContentTypeFullName(contentTypeFullName);
            var contentTypeName = GetContentTypeNameFromContentTypeFullName(contentTypeFullName);

            ContentType contentType = null;

            if (!moduleName.IsNullOrEmpty() && !contentTypeName.IsNullOrEmpty())
            {
                var module = LoadModule(moduleName);

                if (module != null)
                {
                    module.TryGetContentType(contentTypeName, out contentType);
                }
            }

            return contentType;
        }

        public string GetContentTypeFullNameFromPropertyFullName(string propertyFullName)
        {
            if (propertyFullName.IsNullOrEmpty())
                return null;

            var x = propertyFullName.LastIndexOf('.');

            if (x == -1)
                return null;

            return propertyFullName.Substring(0, x);
        }

        public void Dispose()
        {
            _documentSession.Dispose();
        }
    }
}
