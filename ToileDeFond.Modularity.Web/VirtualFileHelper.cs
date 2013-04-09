using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToileDeFond.Modularity.Web
{
    //TODO: Convention des fichiers (resources web - views, js, css, etc.)
    //La convention prévoit que toutes resources de modules doivent être suffixées du [Module.Assembly Name du module]
    //Ceci permet au VirtualFilePathProvider d'aller chercher le contenu embedded des modules
    //Cette convention est facilitée par le UrlRewriter (pour les vues) et des extensions méthodes ajoutées à UrlHelper pour les assets (css, js, etc..)
    //Tout ça fonctionne en étroite relation avec les ModuleAreaRegistration (on utilise les routes constraintes pour le namespace qui est le Module.Assembly Name et qui est le Module Name)
    //Il est important que les modules respectent une convention pour les routes pour ne pas qu'il y est de conflit... il faut donc que toutes routes provenant d'une module soir pour une Area et un  Module.Assembly (namespace) constraint

    public static class VirtualFileHelper
    {
        //This method must be extremely fast because it is called a lot!
        public static bool IsModuleRessource(string virtualPath)
        {
            return Regex.Match(virtualPath, @"\[Module\..*\]", RegexOptions.IgnoreCase).Success;
        }

        public static string GetRealFileName(string virtualPath)
        {
            return GetRealFileName(virtualPath, false);
        }

        public static string GetRealFileName(string virtualPath, bool relative)
        {
            var path = Regex.Replace(virtualPath, @"\[Module\..*\]", string.Empty, RegexOptions.IgnoreCase);

            if (!relative)
                path = path.Replace("~", string.Empty);

            return path;
        }

        public static string GetModuleName(string virtualPath)
        {
            var match = Regex.Match(virtualPath, @"\[Module\.(.*)\]", RegexOptions.IgnoreCase);

            return match.Groups[1].Value;

            // return string.Format(@"{0}\{1}\{1}.dll", DependencyResolver.Current.GetService<IModularityConfiguration>().ModuleDirectoryPath, match.Groups[1].Value);
        }

        //public static string GetDllFileName(string virtualPath)
        //{
        //    var match = Regex.Match(virtualPath, @"\[Module\.(.*)\]", RegexOptions.IgnoreCase);

        //    return string.Format(@"{0}\{1}\{1}.dll", .Current.GetService<IModularityConfiguration>().ModuleDirectoryPath, match.Groups[1].Value);
        //}

        public static string GetVirtualModulePath(string virtualPath, string moduleName)
        {
            var match = Regex.Match(virtualPath, @"(.*)\.([a-z]+)$", RegexOptions.IgnoreCase);

            return string.Format("{0}[Module.{1}].{2}", match.Groups[1].Value, moduleName, match.Groups[2].Value);
        }

        public static bool ModuleFileExists(string virtualPath)
        {
            //var module = DependencyResolver.Current.GetService<IContentManager>().LoadModule(GetModuleName(virtualPath));
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name.Equals(GetModuleName(virtualPath)));

            if (assembly != null)
            {
                var realPath = GetRealFileName(virtualPath);
                var ressourceName = string.Format("{0}{1}", assembly.GetName().Name, realPath.Replace("/", "."));
                var resourceList = assembly.GetManifestResourceNames();
                var found = Array.Exists(resourceList, r => r.Equals(ressourceName, StringComparison.OrdinalIgnoreCase));

                return found;
            }

            return false;
        }

        public static Stream GetResourceStream(string virtualPath)
        {
            //var module =DependencyResolver.Current.GetService<IContentManager>().GetModuleById(new Guid(moduleId));
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name.Equals(GetModuleName(virtualPath)));

            if (assembly != null)
            {
                var realPath = GetRealFileName(virtualPath);
                var ressourceName = string.Format("{0}{1}", assembly.GetName().Name, realPath.Replace("/", "."));
                var resourceList = assembly.GetManifestResourceNames().ToList();

                ressourceName = resourceList.First(r => r.Equals(ressourceName, StringComparison.OrdinalIgnoreCase));

                return assembly.GetManifestResourceStream(ressourceName);
            }

            return null;
        }
    }
}
