using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using MefContrib.Hosting.Conventions;

namespace ToileDeFond.Modularity
{
    public abstract class StarterBase
    {
        private const string BinFolder = "Bin";
        private const string RootModulePath = @"App_Data\Modules";
        private static string _modulePath;
        private string _currentModuleBinDirectoryPath;
        private string _moduleBinDirectoryPath;

        protected virtual string ModuleDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_modulePath))
                {
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    int index = AppDomain.CurrentDomain.BaseDirectory.LastIndexOf("\\bin");

                    if (index > -1)
                    {
                        baseDirectory = baseDirectory.Substring(0, index);
                    }

                    _modulePath = baseDirectory.PathCombine(RootModulePath);
                }

                return _modulePath;
            }
        }

        protected virtual string ModuleBinDirectoryPath
        {
            get { return _moduleBinDirectoryPath ?? (_moduleBinDirectoryPath = ModuleDirectoryPath.PathCombine(BinFolder)); }
        }

        protected virtual string CurrentModuleBinDirectoryPath //LocalModuleBonPath
        {
            get
            {
                return _currentModuleBinDirectoryPath ??
                       (_currentModuleBinDirectoryPath =
                        ModuleBinDirectoryPath.PathCombine(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")));
            }
        }

        public virtual List<Tuple<Assembly, Type>> Start()
        {
            if (DependencyResolver.IsSet)
                return ModuleUtilities.GetLoadedModuleAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            string binPath = GetBinPath();
            IList<string> binDllFileNames = GetBinDllFileNames(binPath);
            List<ComposablePartCatalog> binComposablePartCatalogs = GetBinComposablePartCatalogs(binDllFileNames,
                                                                                                 binPath);

            CreateModuleDirectories();

            //TODO: Auto Register (Convention parts mef...) Action Handlers
            //TODO: Auto Register Controllers insinde module content (metadata) avec un action handler sur la création d'un module?


            // Set Assembly Resolver (TODO : A quoi ca sert??? encore utile?? - je l'Enleve jusqu'a preuve du contraire...)
            //Apparament ca sert dans la génération des vues Razor.. si je l'enlève les vues ne sont pas générées!!
            //http://stackoverflow.com/questions/12128890/best-practices-for-composed-asp-net-mvc-web-applications-mef-areas-di
            //TODO: Si ca sert juste pour Razor il faudrait pas chercher dans tous les assemblies!? (performance)
            AppDomain.CurrentDomain.AssemblyResolve += new AssemblyResolver().CurrentDomainAssemblyResolve;

            // Pre Modules and Parts Composition
            //var composableParts = new List<ComposablePartCatalog> { new DirectoryCatalog(GetBinPath()) };

            List<Assembly> allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                          .Where(
                                                              a =>
                                                              !a.IsDynamic &&
                                                              binDllFileNames.Contains(Path.GetFileName(a.Location)))
                                                          .ToList();
            List<Tuple<Assembly, Type>> loadedModuleAssemblies =
                ModuleUtilities.GetLoadedModuleAssemblies(allLoadedAssemblies);

            //On crée un DependencyResolver temporaire pour aller chercher les IPartRegistryRetriever
            List<Assembly> moduleDirectoryAssemblies = CopyDllToExecutionPath(true, ref loadedModuleAssemblies,
                                                                              ref allLoadedAssemblies);

            foreach (var loadedModuleAssembly in loadedModuleAssemblies)
            {
                CreateModuleInitializerInstance(loadedModuleAssembly.Item2).PreInitialize();
            }

            List<ComposablePartCatalog> allComposableParts =
                binComposablePartCatalogs.Union(
                    moduleDirectoryAssemblies.Select(
                        moduleDirectoryAssembly => new AssemblyCatalog(moduleDirectoryAssembly))).ToList();
            IDependencyResolver dependencyResolver = GetDependencyResolver(allComposableParts);
            DependencyResolver.SetResolver(dependencyResolver);

            //Ajouter les conventions
            List<IPartRegistryRetriever> composablePartCatalogRetrievers =
                DependencyResolver.Current.GetServices<IPartRegistryRetriever>().ToList();
            //PAs besoin de faire le join avec les modules assemblies car ils sont ajoutés a la collection allLoadedAssemblies
            //TODO: Rouler les conventions juste sur les module assemblies
            foreach (Assembly assembly in allLoadedAssemblies
                /*.Union(moduleDirectoryAssemblies).DistinctBy(a => a.FullName)*/)
            {
                //Attention si jamais on fait des conventions aussi dans LoadBinPartsAndSetTemporaryDependencyResolverForInitializationPeriod elles vont etre faites 2 fois...
                AddConventionParts(assembly, allComposableParts, composablePartCatalogRetrievers);
            }

            //Faire le DependencyResolver final (après conventions)
            dependencyResolver = GetDependencyResolver(allComposableParts);
            DependencyResolver.SetResolver(dependencyResolver);

            //On les fais tous d'u coup avec RavenDB - plus performant et limites de queries par défault
            InstallOrUpdateModules(loadedModuleAssemblies, dependencyResolver);
            InitializeModules(loadedModuleAssemblies, dependencyResolver);

            return loadedModuleAssemblies;
        }

        protected virtual void InitializeModules(List<Tuple<Assembly, Type>> moduleAssemblies,
                                                 IDependencyResolver dependencyResolver)
        {
            foreach (var moduleAssembly in moduleAssemblies)
            {
                //TODO: Faut-il vraiment forcer l'existance d'un ModuleItinitializer et throw si non présent
                if (moduleAssembly.Item2 == null)
                    throw new NotImplementedException("The module " + moduleAssembly.Item1.GetName().Name +
                                                      " has no ModuleItinitializer.");

                IModuleInitializer moduleInitializer = CreateModuleInitializerInstance(moduleAssembly.Item2);
                moduleInitializer.Initialize(dependencyResolver);
            }
        }

        protected virtual IModuleInitializer CreateModuleInitializerInstance(Type moduleInitializer)
        {
            //TODO: Meilleur erreur
            if (moduleInitializer == null)
                throw new ArgumentNullException("moduleInitializer");

            var moduleInitializerInstance = Activator.CreateInstance(moduleInitializer) as IModuleInitializer;

            return moduleInitializerInstance;
        }

        protected virtual List<ComposablePartCatalog> GetBinComposablePartCatalogs(IList<string> binDllFileNames,
                                                                                   string binPath)
        {
            // var composableParts = new List<ComposablePartCatalog>();

            //Au lieu d'aller chercher dans le repertoire bin.. on va chercher les loaded assemblies - devrait avoir le meme effet!
            //NON! - N'a pas le même effet car certains dll ne sont pas loadés si runtime pense qu'il en a pas besoin!!!
            //var primaryDependencyResolver = GetOriginalDependencyResolver();

            //TODO: Tellement lourd juste pour aller chercher quelques classes (sélectionner des assemblies précises!!)
            //var composableParts = new ComposablePartCatalog[] { new DirectoryCatalog(Sugar.GetBinPath()) }.ToList();

            var composableParts = new List<ComposablePartCatalog>();

            foreach (string binDllFileName in binDllFileNames)
            {
                composableParts.Add(new AssemblyCatalog(Path.Combine(binPath, binDllFileName)));
            }

            //foreach (var composablePartCatalog in composableParts)
            //{

            //    //Pas besoin des conventions ici ??
            //    //AddConventionParts();
            //}

            return composableParts;
        }

        //TODO: Trouver une meilleure solution car il se peut que la logique pour dire si un dll doit etre chargé ou non soit inconnue (drop dll runtime...)
        protected virtual IList<string> GetBinDllFileNames(string binPath)
        {
            List<string> dllFilePaths = Directory.GetFiles(binPath, "*.dll").Select(Path.GetFileName).ToList();

            return dllFilePaths;
        }

        private static void AddConventionParts(Assembly moduleDirectoryAssembly,
                                               List<ComposablePartCatalog> composableParts,
                                               IEnumerable<IPartRegistryRetriever> composablePartCatalogRetrievers)
        {
            foreach (IPartRegistryRetriever composablePartCatalogRetriever in composablePartCatalogRetrievers)
            {
                var conventionCatalog =
                    new ConventionCatalog(
                        composablePartCatalogRetriever.GetPartRegistryForAssembly(moduleDirectoryAssembly));
                composableParts.Add(conventionCatalog);
            }
        }

        protected virtual void InstallOrUpdateModules(List<Tuple<Assembly, Type>> moduleAssemblies,
                                                      IDependencyResolver dependencyResolver)
        {
            //On les fais tous d'un coup avec RavenDB - plus performant et limites de queries par défault

            var moduleManager = dependencyResolver.GetService<IModuleManager>();
            moduleManager.InstallOrUpdateModules(moduleAssemblies.Select(ma => ma.Item1).ToList());
        }

        protected virtual IDependencyResolver GetDependencyResolver(
            IEnumerable<ComposablePartCatalog> composablePartCatalogs)
        {
            return new MefDependencyResolver(composablePartCatalogs);
        }

        protected virtual void CreateModuleDirectories()
        {
            Directory.CreateDirectory(ModuleBinDirectoryPath);
        }

        //TODO: Faire des tests et refactorer
        //TODO: Gérer les Framework Dependecies (dépendances a des dll du framework .net - on veut pas les ajouter au répertoire du module... il faudrait faire un autre fichier text pe et les charger...)
        protected virtual List<Assembly> CopyDllToExecutionPath(bool withCleanUp,
                                                                ref List<Tuple<Assembly, Type>> loadedModuleAssemblies,
                                                                ref List<Assembly> allLoadedAssemblies)
        {
            var moduleDirectoryAssemblies = new List<Assembly>();

            // Clean and create Bin module path
            if (Directory.Exists(ModuleBinDirectoryPath) && withCleanUp)
                SafeRemovePath(ModuleBinDirectoryPath);

            string currentModulePathBinDirectory = CurrentModuleBinDirectoryPath;
            Directory.CreateDirectory(currentModulePathBinDirectory);

            // Modules Dll
            List<KeyValuePair<string, Tuple<FileInfo[], string[]>>> dllmodules =
                Directory.GetDirectories(ModuleDirectoryPath, "*.*", SearchOption.TopDirectoryOnly)
                         .Select(dir => new DirectoryInfo(dir))
                         .Where(dir => !dir.Name.EndsWith(".svn"))
                         .Where(dir => !dir.Name.EndsWith("Bin"))
                         .Select(
                             dir =>
                             new KeyValuePair<string, Tuple<FileInfo[], string[]>>(dir.Name,
                                                                                   new Tuple<FileInfo[], string[]>(
                                                                                       dir.GetFiles("*.dll"),
                                                                                       GetModuleDependencies(dir))))
                         .Where(m => !IsFilteredOut(m))
                         .ToList();

            //TODO: Refactorer... il se peut qu'on essaye d'ajouter un dll (dépendance d'un module) alors qu'on réfère déjà cette dépendence...!

            var notYetLoadedModules = new Dictionary<string, Tuple<FileInfo[], string[]>>();
            var dependencyManager = new DependencyManager();

            foreach (var mod in dllmodules)
            {
                Tuple<Assembly, Type> loaded =
                    loadedModuleAssemblies.FirstOrDefault(
                        q => q.Item1.GetName().Name.Equals(mod.Key, StringComparison.OrdinalIgnoreCase));

                if (loaded == null)
                {
                    notYetLoadedModules.Add(mod.Key, mod.Value);
                }

                dependencyManager.Add(mod.Key, mod.Value.Item2);
            }

            //TODO: Faire en sorte qu'une erreur précise découle d'une dépendences manquantes ou circulaire, etc..

            IEnumerable<string> sortedDependencies = dependencyManager.GetSortedDependencies();

            foreach (string sortedDependency in sortedDependencies)
            {
                Tuple<FileInfo[], string[]> module;

                notYetLoadedModules.TryGetValue(sortedDependency, out module);

                if (module != null)
                {
                    List<FileInfo> dlls = module.Item1.ToList();

                    foreach (FileInfo dll in dlls)
                    {
                        string src = Path.Combine(currentModulePathBinDirectory, dll.Name);

                        if (!File.Exists(src))
                        {
                            File.Copy(dll.FullName, src);
                            Assembly assembly = Assembly.LoadFile(src);
                            allLoadedAssemblies.Add(assembly);
                        }
                    }

                    Assembly moduleAssembly = allLoadedAssemblies.First(a => a.GetName().Name == sortedDependency);
                    Type moduleInitializer = ModuleUtilities.GetModuleInitializerFromAssembly(moduleAssembly);
                    loadedModuleAssemblies.Add(new Tuple<Assembly, Type>(moduleAssembly, moduleInitializer));
                    moduleDirectoryAssemblies.Add(moduleAssembly);
                }
            }

            return moduleDirectoryAssemblies;
        }

        protected virtual bool IsFilteredOut(KeyValuePair<string, Tuple<FileInfo[], string[]>> keyValuePair)
        {
            return false;
        }

        protected virtual string[] GetModuleDependencies(DirectoryInfo dir)
        {
            var dependencies = new HashSet<string>();

            FileInfo dependenciesFile = dir.GetFiles("Dependencies.txt").FirstOrDefault();

            if (dependenciesFile != null)
            {
                using (StreamReader sr = dependenciesFile.OpenText())
                {
                    string s;

                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.Trim().Length > 0)
                        {
                            dependencies.Add(s);
                        }
                    }
                }
            }

            return dependencies.ToArray();
        }

        private void SafeRemovePath(string path)
        {
            // Aller chercher tout les sous-folders
            foreach (string dir in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch
                {
                    // Si ça fail, tant pis, on fera le ménage du DLL locké le prochain coup
                }
            }
        }

        protected virtual string GetBinPath()
        {
            return string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath)
                       ? AppDomain.CurrentDomain.BaseDirectory
                       : AppDomain.CurrentDomain.RelativeSearchPath;
        }
    }
}