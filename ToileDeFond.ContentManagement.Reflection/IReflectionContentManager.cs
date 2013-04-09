using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ToileDeFond.ContentManagement.Reflection
{
    //C'est imprtant que le IReflectionContentManager soit un IContentManager de sorte a etre certain qu'il partage la meme session (performance)

    public interface IReflectionContentManager : IContentManager
    {
        List<ModuleInfo> GetModuleInfos();
        CreateOrUpdateReport<Module> GetNewOrUpdatedModule(string assemblyName);
        CreateOrUpdateReport<Module> GetNewOrUpdatedModule(Assembly assembly);
        CreateOrUpdateReport<Content> GetNewOrUpdatedContent(object content);
        List<CreateOrUpdateReport<Module>> GetNewOrUpdatedModules(IList<string> assemblyNames);
        List<CreateOrUpdateReport<Module>> GetNewOrUpdatedModules(IList<Assembly> assemblies);
        List<CreateOrUpdateReport<Content>> GetNewOrUpdatedContents(IList<object> contents);
        T GetObjectFromContent<T>(Content content, CultureInfo culture = null) where T : new();
        T GetObjectFromContent<T>(ContentTranslationVersion contentProjection) where T : new();
    }
}