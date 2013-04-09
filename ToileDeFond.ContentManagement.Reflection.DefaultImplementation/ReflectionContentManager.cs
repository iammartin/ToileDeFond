using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ToileDeFond.Localization;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.Reflection.DefaultImplementation
{
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    [PrioritisedExport(typeof(IReflectionContentManager))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReflectionContentManager : IReflectionContentManager
    {
        private readonly IContentManager _contentManager;
        private readonly IReflectionModuleBuilder _moduleBuilder;
        private readonly IContentBuilder _contentBuilder;
        private readonly ICultureManager _cultureInfoManager;
        //private readonly IContentPublicationStateManager _contentPublicationStateManager;
        //private readonly IContentPublicationDateTimeManager _contentPublicationDateTimeManager;

        public ReflectionContentManager(IContentManager contentManager, IReflectionModuleBuilder moduleBuilder, 
            IContentBuilder contentBuilder, ICultureManager cultureInfoManager/*,
            IContentPublicationStateManager contentPublicationStateManager, IContentPublicationDateTimeManager contentPublicationDateTimeManager*/)
        {
            _contentManager = contentManager;
            _moduleBuilder = moduleBuilder;
            _contentBuilder = contentBuilder;
            _cultureInfoManager = cultureInfoManager;
            //_contentPublicationStateManager = contentPublicationStateManager;
            //_contentPublicationDateTimeManager = contentPublicationDateTimeManager;
        }

        //TODO: S'organiser pour ne pas avoir a mettre d'Attibuts.. exemple : Faire pour comme les COntroller .. avec la classe qui permet de faire des conventions Voir méthode Start...
        [ImportingConstructor]
        public ReflectionContentManager([ImportMany]IEnumerable<Lazy<IContentManager, IPrioritisedMefMetaData>> contentManagers, 
            IReflectionModuleBuilder moduleBuilder, IContentBuilder contentBuilder,
            [ImportMany]IEnumerable<Lazy<ICultureManager, IPrioritisedMefMetaData>> cultureInfoManagers/*,
            IContentPublicationStateManager contentPublicationStateManager, IContentPublicationDateTimeManager contentPublicationDateTimeManager*/)
            : this(contentManagers.OrderByDescending(x => x.Metadata.Priority).First().Value, moduleBuilder, contentBuilder, 
                cultureInfoManagers.OrderByDescending(x => x.Metadata.Priority).First().Value/*, contentPublicationStateManager, contentPublicationDateTimeManager*/)
        {
        }

        public List<ModuleInfo> GetModuleInfos()
        {
            var installedModules = _contentManager.LoadAllModules().ToList();
            var referencedModules = ModuleUtilities.GetLoadedModuleAssemblies();

            var moduleInfos = new List<ModuleInfo>();

            foreach (var referencedModule in referencedModules.Select(m => m.Item1))
            {
                var moduleName = _moduleBuilder.GetModuleNameFromAssembly(referencedModule);
                var installedModule = installedModules.FirstOrDefault(m => m.Name == moduleName);
                string moduleInstalledVersion = null;
                Guid? installedModuleId = null;

                if (installedModule != null)
                {
                    installedModules.Remove(installedModule);
                    moduleInstalledVersion = installedModule.Version;
                    installedModuleId = installedModule.Id;
                }

                var moduleInfo = new ModuleInfo(moduleName, moduleInstalledVersion, _moduleBuilder.GetModuleVersionFromAssembly(referencedModule), installedModuleId);

                moduleInfos.Add(moduleInfo);
            }

            moduleInfos.AddRange(installedModules.Select(installedModule => new ModuleInfo(installedModule.Name, installedModule.Version, null)));

            return moduleInfos;
        }

        public virtual CreateOrUpdateReport<Module> GetNewOrUpdatedModule(string assemblyName)
        {
            var assembly = GetAssemblyByAssemblyName(assemblyName);

            return GetNewOrUpdatedModule(assembly);
        }

        protected virtual  Assembly GetAssemblyByAssemblyName(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == assemblyName);
        }

        //TODO: Faire meme chose pour ContentTypeProperty
        //public virtual CreateOrUpdateReport<ContentType> GetNewOrUpdatedContentType(Type type, Module module = null)
        //{
        //    if (module == null)
        //        module = LoadModuleFromAssembly(type.Assembly);

        //    if (module == null)
        //        throw new NotImplementedException();

        //    var contentType = GetContentTypeFromType(type, module);
        //    var action = contentType == null ? CreateOrUpdateActions.Created : CreateOrUpdateActions.Updated;
        //    contentType = _moduleBuilder.UpdateContentTypeFromType(module, type);

        //    return new CreateOrUpdateReport<ContentType> { Action = action, Item = contentType };
        //}

        protected virtual ContentType GetContentTypeFromType(Type type, Module module)
        {
            ContentType contentType;

            module.TryGetContentType(_moduleBuilder.GetContentTypeNameFromType(type), out contentType);

            return contentType;
        }

        protected virtual Module GetModuleFromAssembly(Assembly assembly)
        {
            return _contentManager.LoadModule(_moduleBuilder.GetModuleNameFromAssembly(assembly));
        }

        protected virtual Dictionary<Assembly, Module> GetModuleFromAssemblies(IList<Assembly> assemblies)
        {
            var moduleNames = assemblies.Select(a => _moduleBuilder.GetModuleNameFromAssembly(a)).ToList();

            var x = _contentManager.LoadModules(moduleNames);

            return x.ToDictionary(y => assemblies.First(a => a.GetName().Name == y.Key), y => y.Value);
        }

        //TODO: Retourner Unchanged si c'est le cas
        public CreateOrUpdateReport<Content> GetNewOrUpdatedContent(object objectContent)
        {
            var contentId = _contentBuilder.GetContentId(objectContent);
            var content = contentId.HasValue ? _contentManager.LoadContent(contentId.Value) : null;
            var action = content == null ? CreateOrUpdateActions.Created : CreateOrUpdateActions.Updated;

            var cultureInfo = _contentBuilder.GetCultureInfo(objectContent) ?? _cultureInfoManager.GetDefaultCulture();

            if(action == CreateOrUpdateActions.Created)
            {
                var contentType = GetContentTypeFromType(objectContent.GetType());

                if(contentType == null)
                    throw new Exception("No content type found for " + objectContent.GetType().FullName);

                content = new Content(contentType, cultureInfo);
            }

            content = _contentBuilder.UpdateContent(objectContent, content, cultureInfo);

            return new CreateOrUpdateReport<Content> { Action = action, Item = content };
        }

        public List<CreateOrUpdateReport<Module>> GetNewOrUpdatedModules(IList<string> assemblyNames)
        {
            var assemblies = new List<Assembly>();

            foreach (var assemblyName in assemblyNames)
            {
                assemblies.Add(GetAssemblyByAssemblyName(assemblyName));
            }

            return GetNewOrUpdatedModules(assemblies);
        }

        public virtual CreateOrUpdateReport<Module> GetNewOrUpdatedModule(Assembly assembly)
        {
            var module = GetModuleFromAssembly(assembly);

            return GetNewOrUpdatedModule(module, assembly);
        }

        protected virtual CreateOrUpdateReport<Module> GetNewOrUpdatedModule(Module module, Assembly assembly)
        {
            var action = module == null ? CreateOrUpdateActions.Created : (module.Version == _moduleBuilder.GetModuleVersionFromAssembly(assembly) ? CreateOrUpdateActions.Unchanged : CreateOrUpdateActions.Updated);

            if (action != CreateOrUpdateActions.Unchanged)
            {
                module = _moduleBuilder.UpdateModuleFromAssembly(assembly, module);
            }

            return new CreateOrUpdateReport<Module> { Action = action, Item = module };
        }

        public List<CreateOrUpdateReport<Module>> GetNewOrUpdatedModules(IList<Assembly> assemblies)
        {
            var modulesAndAssemblies = GetModuleFromAssemblies(assemblies);
            var reports = new List<CreateOrUpdateReport<Module>>();

            foreach (var assembly in assemblies)
            {
                Module module;
                modulesAndAssemblies.TryGetValue(assembly, out module);

                reports.Add(GetNewOrUpdatedModule(module, assembly));
            }

            return reports;
        }

        public List<CreateOrUpdateReport<Content>> GetNewOrUpdatedContents(IList<object> contents)
        {
            throw new NotImplementedException();
        }

        public T GetObjectFromContent<T>(Content content, CultureInfo culture) where T : new()
        {
            if (culture == null) throw new ArgumentNullException("cultureName");
            //TODO: Rendu ici a tenir compte de AddWeakAggregationProperty/AddStrongAggregationProperty


            var type = typeof (T);
            var obj = new T();
            var translation = content[culture];

            foreach (var property in type.GetProperties())
            {
                //TODO: Revoir pour ne pas avoir a dupliquer cette logiue de recherche de propriété core (mettre au niveau du contentBuilder?)
                if (property.Name.Equals("Id") || property.HasAttribute<IdAttribute>())
                {
                    property.SetValue(obj, translation.Content.Id);
                }
                else if (property.Name.Equals("CultureName") || property.HasAttribute<CultureAttribute>())
                {
                    property.SetValue(obj, translation.Culture);
                }
                else
                {
                    IContentTypeProperty contentTypeProperty;

                    if (content.ContentType.TryGetProperty(property.Name, out contentTypeProperty))
                    {
                        //TODO: GetValue permettre de dire le type qu'on veut en retour... le type sur un ContentTypeProperty a-t-il tant d'importance....!!
                        //TODO: Dire quelle version (DateTime & draft ou non on veut récupérer - utiliser les services ou recevoir l'info en arguments)
                        property.SetValue(obj, translation[contentTypeProperty].GetValue(property.PropertyType));
                    }
                }
            }

            throw new NotImplementedException();

            return obj;
        }

        public T GetObjectFromContent<T>(ContentTranslationVersion contentProjection) where T : new()
        {
            //TODO: Rendu ici a tenir compte de AddWeakAggregationProperty/AddStrongAggregationProperty

            var type = typeof(T);
            var obj = new T();
            //var contentType = LoadContentType(contentProjection.ContentTypeFullName);

            foreach (var property in type.GetProperties())
            {
                //TODO: Revoir pour ne pas avoir a dupliquer cette logiue de recherche de propriété core (mettre au niveau du contentBuilder?)
                if (property.Name.Equals("Id") || property.HasAttribute<IdAttribute>())
                {
                    property.SetValue(obj, contentProjection.Id);
                }
                else if (property.Name.Equals("CultureName") || property.HasAttribute<CultureAttribute>())
                {
                    property.SetValue(obj, CultureInfo.GetCultureInfo(contentProjection.CultureName));
                }
                else
                {
                    var p = contentProjection.GetType().GetProperty(property.Name);

                    if (p != null)
                    {
                        //TODO: GetValue permettre de dire le type qu'on veut en retour... le type sur un ContentTypeProperty a-t-il tant d'importance....!!
                        //TODO: Dire quelle version (DateTime & draft ou non on veut récupérer - utiliser les services ou recevoir l'info en arguments)
                        property.SetValue(obj, p.GetValue(contentProjection));
                    }
                }
            }

            throw new NotImplementedException();

            return obj;
        }

        private ContentType GetContentTypeFromType(Type type)
        {
            var module = GetModuleFromAssembly(type.Assembly);

            if (module == null)
                return null;

            return GetContentTypeFromType(type, module);
        }

        protected virtual Module LoadModuleFromAssembly(Assembly assembly)
        {
            var module = _contentManager.LoadModule(_moduleBuilder.GetModuleNameFromAssembly(assembly));

            return module;
        }

        #region IContentManager Implementation

        public PagedCollection<Content> LoadContentsByContentType(string contentTypeFullName, int pageSize, int pageIndex = 1)
        {
            return _contentManager.LoadContentsByContentType(contentTypeFullName, pageSize, pageIndex);
        }

        public void DeleteModuleById(Guid id)
        {
            _contentManager.DeleteModuleById(id);
        }

        public void DeleteContentById(Guid id)
        {
            _contentManager.DeleteContentById(id);
        }

        public string GetModuleNameFromContentTypeFullName(string contentTypeFullName)
        {
            return _contentManager.GetModuleNameFromContentTypeFullName(contentTypeFullName);
        }

        public string GetContentTypeNameFromContentTypeFullName(string contentTypeFullName)
        {
            return _contentManager.GetContentTypeNameFromContentTypeFullName(contentTypeFullName);
        }

        public ContentType LoadContentType(string contentTypeFullName)
        {
            return _contentManager.LoadContentType(contentTypeFullName);
        }

        public string GetContentTypeFullNameFromPropertyFullName(string propertyFullName)
        {
            return _contentManager.GetContentTypeFullNameFromPropertyFullName(propertyFullName);
        }

        public void Dispose()
        {
            _contentManager.Dispose();
        }

        public void Store(Module module)
        {
            _contentManager.Store(module);
        }

        public Module LoadModule(string moduleFullName)
        {
            return _contentManager.LoadModule(moduleFullName);
        }

        public void Store(Content content)
        {
            _contentManager.Store(content);
        }

        public Content LoadContent(Guid id)
        {
            return _contentManager.LoadContent(id);
        }

        public Module LoadModule(Guid id)
        {
            return _contentManager.LoadModule(id);
        }

        public ReadOnlyCollection<Publication> LoadPublications(IEnumerable<Guid> ids)
        {
            return _contentManager.LoadPublications(ids);
        }

        public ReadOnlyCollection<Module> LoadAllModules()
        {
            return _contentManager.LoadAllModules();
        }

        public void SaveChanges()
        {
            _contentManager.SaveChanges();
        }

        public Dictionary<string, Module> LoadModules(List<string> moduleNames)
        {
            return _contentManager.LoadModules(moduleNames);
        }

        #endregion
    }
}
